#pragma once

#include <opencv2/opencv.hpp>
#ifdef CUDA
#include <opencv2/cudaarithm.hpp>
#endif

class FrameProcessor
{
public:
	cv::Mat zeros;

#ifdef CUDA
	cv::cuda::GpuMat zerosGpu;
	void gpuDifference(int config[], cv::cuda::GpuMat &frame, cv::cuda::GpuMat &background, cv::Mat &result);
	void gpuDepthDifference(int config[], cv::cuda::GpuMat &frame, cv::cuda::GpuMat &background_color, cv::cuda::GpuMat &depth, cv::Mat &result);
#endif

	FrameProcessor(int rows, int cols);

	void staticDifference(int config[], cv::Mat &frame, cv::Mat &background, cv::Mat &result);
	void dilateDifference(int config[], cv::Mat &frame, cv::Mat &background, cv::Mat &result);	
	void depthDifference(int config[], cv::Mat &framee, cv::Mat &background_color, cv::Mat &depthe, cv::Mat &background_depth, cv::Mat &result);
	
};