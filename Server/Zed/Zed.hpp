#pragma once

#include <sl_zed/Camera.hpp>
#include <opencv2/opencv.hpp>
#include "FrameProcessor.h" 

const int BackgroundFrames = 50;

namespace Core
{
	class Zed
	{
	public:
		FrameProcessor * processor;
		int testFrame;
		cv::Mat leftTestFrames[BackgroundFrames];
		cv::Mat rightTestFrames[BackgroundFrames];

		sl::Camera zed;
		sl::Mat* frame_left_zed;
		sl::Mat* frame_right_zed;

		sl::Mat* depth_image_left_zed;
		sl::Mat* depth_image_right_zed;
		
		sl::Mat point_cloud;
		sl::RuntimeParameters runtime_parameters;

		cv::Mat mask_ocv;
		cv::Mat zeros;

		cv::Mat frame_color_left_ocv;
		cv::Mat frame_depth_left_ocv;		
		cv::Mat background_color_left_ocv;
		cv::Mat background_depth_left_ocv;
		cv::Mat result_left_ocv;

		cv::Mat frame_color_right_ocv;
		cv::Mat frame_depth_right_ocv;
		cv::Mat background_color_right_ocv;
		cv::Mat background_depth_right_ocv;
		cv::Mat result_right_ocv;
		cv::Mat result_sbs_ocv;

		// cuda experiment

		sl::Mat *frame_left_zed_gpu;
		sl::Mat *frame_right_zed_gpu;

#ifdef CUDA
		cv::cuda::GpuMat frame_left_cuda;
		cv::cuda::GpuMat frame_right_cuda;

		cv::cuda::GpuMat depth_left_cuda;
		cv::cuda::GpuMat depth_right_cuda;

		cv::cuda::GpuMat background_left_cuda;
		cv::cuda::GpuMat background_right_cuda;
#endif

		int new_width;
		int new_height;
		bool sideBySide = false;
		bool cuda = false;
		bool processDepth = false;
		int camera;
		std::string cameras[2] = { "Zed Camera", "Web Camera" };
		int config[7] = { 25, 150, 3, 7, 0, 1, 1 };

		cv::VideoCapture cap;
	private:
		sl::Mat img;
		void init_processor();
		
	public:
		/// <summary>Initialises the camera
		/// <param name="setup">
		/// Sets up the camera. Expects 4 integers:<br />
		/// 0 = RESOLUTION (0 - 2K, 1 - HD1980, 2 - HD720, 3 - VGA)<br />
		/// 1 = DEPTH MODE (0 - NONE, 1 - Perofmance, 2 - Medium, 3 - High, 4 - Ultra)
		/// 2 = SENSING MODE (0 - Standard, 1 - Fill)
		/// 3 = SIDE_BY_SIDE (0 - OFF, 1 - ON)
		/// </param>
		/// </summary>
		Zed(int setup[]);
		~Zed();
		
		bool grab();
		void stop();
		
		void reset_background();
		void load_background();
	};
}

extern "C"
{
	__declspec(dllexport) void* __cdecl create_zed(int setup[]);

	__declspec(dllexport) void __cdecl init_camera(void* zed, int setup[], std::string svo);

	__declspec(dllexport) void __cdecl destroy_zed(void* zed);

	__declspec(dllexport) bool __cdecl grab_zed(void* zed);

	__declspec(dllexport) void __cdecl stop_zed(void* zed);

	__declspec(dllexport) void* __cdecl get_left(void* zed);

	__declspec(dllexport) void* __cdecl get_right(void* zed);

	__declspec(dllexport) void* __cdecl get_sbs(void* zed);

	__declspec(dllexport) void __cdecl reset_background(void* zed);

	__declspec(dllexport) void __cdecl setup(void* zed, int setup[]);

	__declspec(dllexport) void __cdecl hello();
}