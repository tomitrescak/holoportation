#include "stdafx.h"
#include "Zed.hpp"

#include <direct.h>
#define GetCurrentDir _getcwd
#include <iostream>

// ZED includes
#include <sl_zed/Camera.hpp>

// OpenCV includes
#include <opencv2/opencv.hpp>

#ifdef CUDA
#include <opencv2/cudaarithm.hpp>
#endif // CUDA



cv::Mat slMat2cvMat(sl::Mat& input);

#ifdef CUDA
cv::cuda::GpuMat slMat2cvCudaMat(sl::Mat& input);
#endif

bool file_exist(const char *file) {
	std::ifstream infile(file);
	return infile.good();
}


std::string GetCurrentWorkingDir(void) {
	char buff[FILENAME_MAX];
	GetCurrentDir(buff, FILENAME_MAX);
	std::string current_working_dir(buff);
	return current_working_dir;
}

namespace Core
{
	void Zed::load_background() {
		std::cout << "LOAD" << std::endl;
		if (file_exist("left_color.tiff")) {

			std::cout << "Loading existing background ..." << std::endl;
			background_color_left_ocv = cv::imread("left_color.tiff", cv::IMREAD_UNCHANGED);
			background_color_right_ocv = cv::imread("right_color.tiff", cv::IMREAD_UNCHANGED);

#ifdef CUDA
			if (cuda) {
				background_left_cuda.upload(background_color_left_ocv);
				background_right_cuda.upload(background_color_right_ocv);
			}
#endif

			testFrame = BackgroundFrames;
		}
		else {
			std::cout << "Background does not exist ..." << std::endl;
			testFrame = 0;
		}
	}

	void Zed::reset_background()
	{
#ifdef CUDA
		if (cuda) {
			frame_left_cuda.copyTo(background_left_cuda);
			frame_right_cuda.copyTo(background_right_cuda);
		}
#endif

		// normal

		frame_color_left_ocv.copyTo(background_color_left_ocv);
		frame_color_right_ocv.copyTo(background_color_right_ocv);

		// calculate

		std::cout << "Collecting frame ";
		std::cout << testFrame << std::endl;

		// cv::cvtColor(background_color_left_ocv, background_color_left_ocv, cv::COLOR_RGBA2RGB);
		cv::Mat left;
		cv::Mat right;
		background_color_left_ocv.convertTo(left, CV_32FC4);
		background_color_right_ocv.convertTo(right, CV_32FC4);

		leftTestFrames[testFrame] = left;
		rightTestFrames[testFrame] = right;

		testFrame = testFrame + 1;

		if (testFrame == BackgroundFrames) {
			std::cout << "Calculating background" << std::endl;

			cv::Mat leftTotal = cv::Mat::zeros(cv::Size(new_width, new_height), leftTestFrames[0].type());
			cv::Mat rightTotal = cv::Mat::zeros(cv::Size(new_width, new_height), leftTestFrames[0].type());

			for (int i = 0; i < BackgroundFrames; i++) {
				leftTotal = leftTotal + leftTestFrames[i];
				rightTotal = rightTotal + rightTestFrames[i];
			}

			leftTotal = leftTotal / (float)BackgroundFrames;
			leftTotal.convertTo(leftTotal, CV_8UC4);
			leftTotal.copyTo(background_color_left_ocv);

			rightTotal = rightTotal / (float)BackgroundFrames;
			rightTotal.convertTo(rightTotal, CV_8UC4);
			rightTotal.copyTo(background_color_right_ocv);

			// cv::imshow("LeftTotal", background_color_left_ocv);
			// cv::imshow("RightTotal", background_color_right_ocv);
		}
		else {
			return;
		}

		// cuda

		std::cout << "Writing background" << std::endl;
		cv::imwrite("left_color.tiff", background_color_left_ocv);
		cv::imwrite("right_color.tiff", background_color_right_ocv);

		if (sideBySide) {
			cv::Mat sbs;
			cv::hconcat(background_color_left_ocv, background_color_right_ocv, sbs);
			cv::imwrite("side_by_side.tiff", sbs);
			return;
		}

		if (processDepth) {
			frame_depth_left_ocv.copyTo(background_depth_left_ocv);
			frame_depth_right_ocv.copyTo(background_depth_right_ocv);

			cv::imwrite("left_depth.tiff", background_depth_left_ocv);
			cv::imwrite("right_depth.tiff", background_depth_right_ocv);
		}
	}

	void Zed::init_processor()
	{
	}

	Zed::Zed(int setup[]) {

		std::string file = GetCurrentWorkingDir() + "\\left_color.tiff";
		std::cout << file << std::endl;
		std::cout << file_exist(file.c_str()) << std::endl;

		// init resolution
		sl::RESOLUTION resolution = static_cast<sl::RESOLUTION>(setup[0]);
		sl::DEPTH_MODE depth = static_cast<sl::DEPTH_MODE>(setup[1]);
		sl::SENSING_MODE sensing = static_cast<sl::SENSING_MODE>(setup[2]);
		sideBySide = setup[4] == 1;
		cuda = setup[3] == 1;
		camera = setup[5];
		processDepth = depth != sl::DEPTH_MODE_NONE;
		int sizeDivision = 2; // sideBySide ? 1 : 2;


		std::cout << "Configuration v 0.0.2" << std::endl;
		std::cout << "------------------------" << std::endl;
		std::cout << "Camera: ";
		std::cout << cameras[camera] << std::endl;
		std::cout << "Resolution: ";
		std::cout << resolution << std::endl;
		std::cout << "Depth: ";
		std::cout << depth << std::endl;
		std::cout << "Sensing: ";
		std::cout << sensing << std::endl;
		if (sideBySide) {
			std::cout << "Side-By-Side" << std::endl;
		}
		if (processDepth) {
			std::cout << "Processing depth" << std::endl;
		}
#ifdef CUDA
		if (cuda) {
			std::cout << "Using CUDA" << std::endl;
		}
#endif

		std::cout << "------------------------" << std::endl;

		if (camera == 0) {
			// Set configuration parameters
			sl::InitParameters init_params;
			init_params.camera_resolution = resolution;
			init_params.depth_mode = depth; // DEPTH_MODE_PERFORMANCE;
			init_params.coordinate_units = sl::UNIT_METER;

			if (processDepth) {
				init_params.enable_right_side_measure = true;
			}

			/*if (!svo.empty()) {
				init_params.svo_input_filename.set(svo.c_str());
			}*/

			// Open the camera
			sl::ERROR_CODE err = zed.open(init_params);
			if (err != sl::SUCCESS) {
				printf("%s\n", toString(err).c_str());
				zed.close();
				return; // Quit if an error occurred
			}

			// Set runtime parameters after opening the camera
			runtime_parameters.sensing_mode = sensing;

			// Prepare new image size to retrieve half-resolution images
			sl::Resolution image_size = zed.getResolution();
			new_width = image_size.width / sizeDivision;
			new_height = image_size.height / sizeDivision;
		}

		if (camera == 1) {
			cap = cv::VideoCapture(0);
			if (!cap.isOpened()) { // check if we succeeded
				throw std::invalid_argument("Cannot open camera!");
			}

			// new_width = cap.get(CV_CAP_PROP_FRAME_WIDTH);
			// new_height = cap.get(CV_CAP_PROP_FRAME_HEIGHT);
			if (resolution == sl::RESOLUTION_HD2K) {
				new_width = 2560 / 2;
				new_height = 1440 / 2;
			}
			else if (resolution == sl::RESOLUTION_HD1080) {
				new_width = 1980 / 2;
				new_height = 1080 / 2;
			}
			else if (resolution == sl::RESOLUTION_HD720) {
				new_width = 1280 / 2;
				new_height = 720 / 2;
			}
			else if (resolution == sl::RESOLUTION_VGA) {
				new_width = 672 / 2;
				new_height = 3756 / 2;
			}

			std::cout << "Width (px): ";
			std::cout << new_width << std::endl;
			std::cout << "Height (px): ";
			std::cout << new_height << std::endl;
		}

		// HELPERS

		mask_ocv = cv::Mat::zeros(new_height, new_width, CV_8UC1);
		zeros = cv::Mat::zeros(new_height, new_width, CV_8UC4);

		// color LEFT / SIDE-BY-SIDE

		frame_left_zed = new sl::Mat(new_width, new_height, sl::MAT_TYPE_8U_C4);
		frame_color_left_ocv = slMat2cvMat(*frame_left_zed);

		// color RIGHT

		frame_right_zed = new sl::Mat(new_width, new_height, sl::MAT_TYPE_8U_C4);
		frame_color_right_ocv = slMat2cvMat(*frame_right_zed);

		// CUDA

#ifdef CUDA
		if (cuda) {
			frame_left_zed_gpu = new sl::Mat(new_width, new_height, sl::MAT_TYPE_8U_C4);
			frame_left_cuda = slMat2cvCudaMat(*frame_left_zed_gpu);

			frame_right_zed_gpu = new sl::Mat(new_width, new_height, sl::MAT_TYPE_8U_C4);
			frame_right_cuda = slMat2cvCudaMat(*frame_right_zed_gpu);
		}
#endif

		// load background

		this->load_background();
		this->processor = new FrameProcessor{ new_height, new_width };

		if (sideBySide) {
			// side-by-side is only using color image
			return;
		}

		// depth LEFT

		if (processDepth) {
			depth_image_left_zed = new sl::Mat(new_width, new_height, sl::MAT_TYPE_8U_C4);
			frame_depth_left_ocv = slMat2cvMat(*depth_image_left_zed);
			depth_image_right_zed = new sl::Mat(new_width, new_height, sl::MAT_TYPE_8U_C4);
			frame_depth_right_ocv = slMat2cvMat(*depth_image_left_zed);
		}
	}

	Zed::~Zed()
	{
	}

	bool Zed::grab()
	{
		if (camera == 1) {
			cv::Mat img;
			cap >> img;

			cv::resize(img, img, cv::Size(new_width, new_height), 0, 0, cv::INTER_CUBIC); // resize to 1024x768 resolution
			
			frame_color_left_ocv = img;
			frame_color_right_ocv = img;
		}
		if (camera == 0) {
			if (zed.grab(runtime_parameters) == sl::SUCCESS) {

				// cuda experiment

				/*if (cuda) {
				sl::Mat left;
				sl::Mat right;

				zed.retrieveImage(left, sl::VIEW_LEFT, sl::MEM_GPU, new_width, new_height);
				zed.retrieveImage(right, sl::VIEW_RIGHT, sl::MEM_GPU, new_width, new_height);

				frame_left_cuda = slMat2cvCudaMat(left);
				frame_right_cuda = slMat2cvCudaMat(right);
				}
				else {*/
				zed.retrieveImage(*frame_left_zed, sl::VIEW_LEFT, sl::MEM_CPU, new_width, new_height);
				zed.retrieveImage(*frame_right_zed, sl::VIEW_RIGHT, sl::MEM_CPU, new_width, new_height);
				//}

			}
			else {
				return false;
			}
		}


		/*zed.retrieveImage(*frame_left_zed_gpu, VIEW_LEFT, MEM_GPU, new_width, new_height);
		zed.retrieveImage(*frame_right_zed_gpu, VIEW_RIGHT, MEM_GPU, new_width, new_height);*/

		// Retrieve the left image, depth image in half-resolution


#ifdef CUDA
		if (cuda) {
			frame_left_cuda.upload(frame_color_left_ocv);
			frame_right_cuda.upload(frame_color_right_ocv);
	}
#endif


		// myFrame.upload(frame_color_left_ocv);

		if (processDepth && config[0] != 0) {
			zed.retrieveImage(*depth_image_left_zed, sl::VIEW_DEPTH, sl::MEM_CPU, new_width, new_height);
			zed.retrieveImage(*depth_image_right_zed, sl::VIEW_DEPTH_RIGHT, sl::MEM_CPU, new_width, new_height);

#ifdef CUDA
			if (cuda) {
				depth_left_cuda.upload(frame_depth_left_ocv);
				depth_right_cuda.upload(frame_depth_right_ocv);
		}
#endif
}


		if (testFrame < BackgroundFrames) {
			reset_background();
			return false;
		}

#ifdef CUDA
		if (cuda) {
			if (processDepth && config[0] != 0) {
				//processor->gpuDifference2(config, frame_left_cuda, background_left_cuda, frame_right_cuda, background_right_cuda, result_left_ocv, result_right_ocv);
				processor->gpuDepthDifference(config, frame_left_cuda, background_left_cuda, depth_left_cuda, result_left_ocv);
				processor->gpuDepthDifference(config, frame_right_cuda, background_right_cuda, depth_right_cuda, result_right_ocv);
			}
			else {
				processor->gpuDifference(config, frame_left_cuda, background_left_cuda, result_left_ocv);
				processor->gpuDifference(config, frame_right_cuda, background_right_cuda, result_right_ocv);
			}
			}
#endif
		if (!cuda) {
			if (processDepth && config[0] != 0) {
				processor->depthDifference(config, frame_color_left_ocv, background_color_left_ocv, frame_depth_left_ocv, background_depth_left_ocv, result_left_ocv);
				processor->depthDifference(config, frame_color_right_ocv, background_color_right_ocv, frame_depth_right_ocv, background_depth_right_ocv, result_right_ocv);
			}
			else {
				processor->dilateDifference(config, frame_color_left_ocv, background_color_left_ocv, result_left_ocv);
				processor->dilateDifference(config, frame_color_right_ocv, background_color_right_ocv, result_right_ocv);
			}
		}

		// contrast
		if (config[4] != 0) {
			result_left_ocv.convertTo(result_left_ocv, -1, config[4] / (float)50, 0);
			result_right_ocv.convertTo(result_right_ocv, -1, config[4] / (float)50, 0);
		}

		if (sideBySide) {
			cv::hconcat(result_left_ocv, result_right_ocv, result_sbs_ocv);	

			//int step = result_sbs_ocv.step;
			//int step1 = result_sbs_ocv.step1();
		}


		// Retrieve the RGBA point cloud in half-resolution
		// To learn how to manipulate and display point clouds, see Depth Sensing sample
		// zed.retrieveMeasure(point_cloud, MEASURE_XYZRGBA, MEM_CPU, new_width, new_height);

		// Display image and depth using cv:Mat which share sl:Mat data
		// cv::imshow("Left", result_left_ocv);
		// cv::imshow("Right", result_right_ocv);

		// Handle key event

		return true;




	}

	void Zed::stop()
	{
		delete frame_left_zed;
		delete frame_right_zed;
		delete depth_image_left_zed;
		delete depth_image_right_zed;
	}
}

/**
* Conversion function between sl::Mat and cv::Mat
**/
cv::Mat slMat2cvMat(sl::Mat& input) {
	// Mapping between MAT_TYPE and CV_TYPE
	int cv_type = -1;
	switch (input.getDataType()) {
	case sl::MAT_TYPE_32F_C1: cv_type = CV_32FC1; break;
	case sl::MAT_TYPE_32F_C2: cv_type = CV_32FC2; break;
	case sl::MAT_TYPE_32F_C3: cv_type = CV_32FC3; break;
	case sl::MAT_TYPE_32F_C4: cv_type = CV_32FC4; break;
	case sl::MAT_TYPE_8U_C1: cv_type = CV_8UC1; break;
	case sl::MAT_TYPE_8U_C2: cv_type = CV_8UC2; break;
	case sl::MAT_TYPE_8U_C3: cv_type = CV_8UC3; break;
	case sl::MAT_TYPE_8U_C4: cv_type = CV_8UC4; break;
	default: break;
	}

	// Since cv::Mat data requires a uchar* pointer, we get the uchar1 pointer from sl::Mat (getPtr<T>())
	// cv::Mat and sl::Mat will share a single memory structure
	return cv::Mat(input.getHeight(), input.getWidth(), cv_type, input.getPtr<sl::uchar1>(sl::MEM_CPU));
}

/**
* Conversion function between sl::Mat and cv::Mat
**/
cv::cuda::GpuMat slMat2cvCudaMat(sl::Mat& input) {
	// Mapping between MAT_TYPE and CV_TYPE
	int cv_type = -1;
	switch (input.getDataType()) {
	case sl::MAT_TYPE_32F_C1: cv_type = CV_32FC1; break;
	case sl::MAT_TYPE_32F_C2: cv_type = CV_32FC2; break;
	case sl::MAT_TYPE_32F_C3: cv_type = CV_32FC3; break;
	case sl::MAT_TYPE_32F_C4: cv_type = CV_32FC4; break;
	case sl::MAT_TYPE_8U_C1: cv_type = CV_8UC1; break;
	case sl::MAT_TYPE_8U_C2: cv_type = CV_8UC2; break;
	case sl::MAT_TYPE_8U_C3: cv_type = CV_8UC3; break;
	case sl::MAT_TYPE_8U_C4: cv_type = CV_8UC4; break;
	default: break;
	}

	// Since cv::Mat data requires a uchar* pointer, we get the uchar1 pointer from sl::Mat (getPtr<T>())
	// cv::Mat and sl::Mat will share a single memory structure
	return cv::cuda::GpuMat(input.getHeight(), input.getWidth(), cv_type, input.getPtr<sl::uchar1>(sl::MEM_GPU));
}

void * create_zed(int setup[])
{
	return new Core::Zed(setup);
}

void init_camera(void * zed, int setup[], std::string svo)
{
}

void hello()
{
	std::cout << "Hello" << std::endl;
}

void destroy_zed(void * zed)
{
	Core::Zed* zed_ptr = reinterpret_cast<Core::Zed*>(zed);
	zed_ptr->stop();
	delete zed_ptr;
}

bool grab_zed(void * zed)
{
	Core::Zed* zed_ptr = reinterpret_cast<Core::Zed*>(zed);
	return zed_ptr->grab();
}

void stop_zed(void * zed)
{
	Core::Zed* zed_ptr = reinterpret_cast<Core::Zed*>(zed);
	zed_ptr->stop();
}

void * get_left(void * zed)
{
	Core::Zed* zed_ptr = reinterpret_cast<Core::Zed*>(zed);
	return zed_ptr->result_left_ocv.data;
}

void * get_sbs(void * zed)
{
	Core::Zed* zed_ptr = reinterpret_cast<Core::Zed*>(zed);
	return zed_ptr->result_sbs_ocv.data;
}

void * get_right(void * zed)
{
	Core::Zed* zed_ptr = reinterpret_cast<Core::Zed*>(zed);
	return zed_ptr->result_right_ocv.data;
}

void reset_background(void * zed)
{

	Core::Zed* zed_ptr = reinterpret_cast<Core::Zed*>(zed);

	zed_ptr->testFrame = 0;
	// zed_ptr->reset_background();
}

void setup(void * zed, int setup[])
{
	Core::Zed* zed_ptr = reinterpret_cast<Core::Zed*>(zed);
	zed_ptr->config[0] = setup[0];
	zed_ptr->config[1] = setup[1];
	zed_ptr->config[2] = setup[2];
	zed_ptr->config[3] = setup[3];
	zed_ptr->config[4] = setup[4];
	zed_ptr->config[5] = setup[5];
	zed_ptr->config[6] = setup[6];
}
