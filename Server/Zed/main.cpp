///////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2017, STEREOLABS.
//
// All rights reserved.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
///////////////////////////////////////////////////////////////////////////

/***********************************************************************************************
** This sample demonstrates how to use the ZED SDK with OpenCV. 					  	      **
** Depth and images are captured with the ZED SDK, converted to OpenCV format and displayed. **
***********************************************************************************************/

#include "stdafx.h"

// ZED includes
#include <sl_zed/Camera.hpp>

// OpenCV includes
#include <opencv2/opencv.hpp>

#include "Zed.hpp"

using namespace sl;

int fps = 0;
auto start = std::chrono::system_clock::now();
const int slider_max_255 = 255;
const int slider_max_30 = 255;

int depth_slider;
int erode_slider;
int dilate_slider;
int threshold_slider;
int cleanup_slider;
int kernel_slider;

Core::Zed *zed_ptr;

void pfps() {
	// count fps
	auto end = std::chrono::system_clock::now();
	std::chrono::duration<double> diff = end - start;

	if (diff.count() > 1) {
		start = std::chrono::system_clock::now();
		std::cout << fps << std::endl;
		fps = 0;
	}
	else {
		fps += 1;
	}
}

void erode_trackbar(int, void*)
{
	zed_ptr->config[2] = erode_slider;
}

void dilate_trackbar(int, void*)
{
	zed_ptr->config[3] = dilate_slider;
}

void depth_trackbar(int, void*)
{
	zed_ptr->config[1] = depth_slider;
}

void threshold_trackbar(int, void*)
{
	zed_ptr->config[0] = threshold_slider;
}

void cleanup_trackbar(int, void*)
{
	zed_ptr->config[5] = cleanup_slider;
}

void kernel_trackbar(int, void*)
{
	zed_ptr->config[6] = kernel_slider;
}

int main() {

	int setup[] = { RESOLUTION_HD720, DEPTH_MODE_NONE, SENSING_MODE_FILL, 0, 1, 1 };
	Core::Zed zed{ setup }; // = Core::Zed();

							// rememeber zed
	zed_ptr = &zed;

	// std::string svo;
	// zed.init_camera(setup, svo);

	// window with slider
	cv::namedWindow("Left", cv::WINDOW_AUTOSIZE); // Create Window

												  // depth
	depth_slider = zed.config[1];
	erode_slider = zed.config[3];
	dilate_slider = zed.config[2];
	threshold_slider = zed.config[0];
	kernel_slider = zed.config[6];
	cleanup_slider = zed.config[5];


	cv::createTrackbar("Threshold", "Left", &threshold_slider, slider_max_255, threshold_trackbar);
	cv::createTrackbar("Depth", "Left", &depth_slider, slider_max_255, depth_trackbar);
	cv::createTrackbar("Cleanup", "Left", &cleanup_slider, slider_max_30, cleanup_trackbar);
	cv::createTrackbar("Erode", "Left", &erode_slider, slider_max_30, erode_trackbar);
	cv::createTrackbar("Dilate", "Left", &dilate_slider, slider_max_30, dilate_trackbar);
	cv::createTrackbar("Kernel", "Left", &kernel_slider, slider_max_30, kernel_trackbar);


	// Loop until 'q' is pressed
	char key = ' ';
	while (key != 'q') {

		if (zed.grab()) {

			// cv::imshow("Image", zed.result_left_ocv);
			// cv::waitKey(10);
			// cv::imshow("Left", zed.result_left_ocv);
			cv::imshow("Leftt", zed.result_left_ocv);
			//cv::imshow("Right", zed.result_right_ocv);

			char key = cv::waitKey(10);

			if (key == 'r') {
				std::cout << "Resetting background" << std::endl;
				zed.testFrame = 0;
			}
			pfps();
		}
	}
	zed.stop();
	return 0;
}

