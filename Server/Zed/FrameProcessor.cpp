#include "stdafx.h"
#include "FrameProcessor.h"
#include <iostream>
#include <chrono>

#include <opencv2/opencv.hpp>

#ifdef CUDA
#include <opencv2/cudaarithm.hpp>
#endif

FrameProcessor::FrameProcessor(int rows, int cols) {
	zeros = cv::Mat::zeros(rows, cols, CV_8UC4);

#ifdef CUDA
	zerosGpu.upload(zeros);
#endif
}

cv::Mat mask;
cv::Mat result;

#ifdef CUDA
cv::cuda::GpuMat maskGpu;
cv::cuda::GpuMat resultGpu;
#endif

void FrameProcessor::staticDifference(int config[], cv::Mat & frame, cv::Mat & background, cv::Mat & result)
{
	absdiff(frame, background, mask);

	if (mask.channels() > 1)
		cvtColor(mask, mask, CV_BGR2GRAY);

	cv::GaussianBlur(mask, mask, cv::Size(5, 5), 3.5, 3.5);

	threshold(mask, mask, config[0], 255, cv::THRESH_BINARY);
	// result.release();
	zeros.copyTo(result);
	frame.copyTo(result, mask);
}



void FrameProcessor::dilateDifference(int config[], cv::Mat & frame, cv::Mat & background, cv::Mat & result)
{
	// cv::imshow("Background", background);

	absdiff(frame, background, mask);

	if (mask.channels() > 1)
		cvtColor(mask, mask, CV_BGR2GRAY);
	
	cv::GaussianBlur(mask, mask, cv::Size(5, 5), 3.5, 3.5);

	threshold(mask, mask, config[0], 255, cv::THRESH_BINARY);

	cv::dilate(mask, mask, cv::Mat(), cv::Size(-1, -1), config[3]);
	cv::erode(mask, mask, cv::Mat(), cv::Size(-1, -1), config[2]);

	zeros.copyTo(result);
	frame.copyTo(result, mask);
}


#ifdef CUDA
void FrameProcessor::gpuDifference(int config[], cv::cuda::GpuMat & frame, cv::cuda::GpuMat & background, cv::Mat & result)
{
	cv::Ptr<cv::cuda::Filter> filter = cv::cuda::createGaussianFilter(CV_8UC4, CV_8UC4, cv::Size(5, 5), 3.5, 3.5);
	cv::cuda::absdiff(frame, background, maskGpu);

	// gaussian
	filter->apply(maskGpu, maskGpu);

	// convert to greyscale
	cv::cuda::cvtColor(maskGpu, maskGpu, cv::COLOR_RGBA2GRAY);

	// threshold
	cv::cuda::threshold(maskGpu, maskGpu, config[0], 255, cv::THRESH_BINARY);

	// cleanup

	cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(2 * config[6] + 1, 2 * config[6] + 1), cv::Point(config[6], config[6]));

	if (config[5] != 0) {
		//cv::Ptr<cv::cuda::Filter> closeFilterFirst = cv::cuda::createMorphologyFilter(cv::MORPH_CLOSE, mask.type(), kernel, cv::Size(-1, -1), config[5]);
		//closeFilterFirst->apply(maskGpu, maskGpu);
		cv::Ptr<cv::cuda::Filter> erodeFilterFirst = cv::cuda::createMorphologyFilter(cv::MORPH_OPEN, mask.type(), kernel, cv::Size(-1, -1), config[5]);
		erodeFilterFirst->apply(maskGpu, maskGpu);
	}

	//  dilate erode
	if (config[3] != 0) {
		cv::Ptr<cv::cuda::Filter> dilateFilter = cv::cuda::createMorphologyFilter(cv::MORPH_DILATE, mask.type(), kernel, cv::Size(-1, -1), config[3]);
		dilateFilter->apply(maskGpu, maskGpu);
	}

	if (config[2] != 0) {
		cv::Ptr<cv::cuda::Filter> erodeFilter = cv::cuda::createMorphologyFilter(cv::MORPH_ERODE, mask.type(), kernel, cv::Size(-1, -1), config[2]);
		erodeFilter->apply(maskGpu, maskGpu);
	}

	zerosGpu.copyTo(resultGpu);
	frame.copyTo(resultGpu, maskGpu);

	resultGpu.download(result);
}


void FrameProcessor::gpuDepthDifference(int config[], cv::cuda::GpuMat & frame, cv::cuda::GpuMat & background_color, cv::cuda::GpuMat & depth, cv::Mat & result)
{
	// depth

	cv::cuda::GpuMat depthMaskGpu;
	cv::cuda::threshold(depth, depthMaskGpu, config[1], 255, cv::THRESH_BINARY);

	cv::cuda::cvtColor(depthMaskGpu, depthMaskGpu, cv::COLOR_RGBA2GRAY);

	// color

	cv::Ptr<cv::cuda::Filter> filter = cv::cuda::createGaussianFilter(CV_8UC4, CV_8UC4, cv::Size(5, 5), 3.5, 3.5);
	cv::cuda::absdiff(frame, background_color, maskGpu);

	// gaussian
	filter->apply(maskGpu, maskGpu);

	// convert to greyscale
	cv::cuda::cvtColor(maskGpu, maskGpu, cv::COLOR_RGBA2GRAY);

	// threshold
	cv::cuda::threshold(maskGpu, maskGpu, config[0], 255, cv::THRESH_BINARY);

	//  dilate erode
	cv::Ptr<cv::cuda::Filter> dilateFilter = cv::cuda::createMorphologyFilter(cv::MORPH_DILATE, mask.type(), cv::Mat(), cv::Size(-1, -1), config[3]);
	dilateFilter->apply(maskGpu, maskGpu);

	cv::Ptr<cv::cuda::Filter> erodeFilter = cv::cuda::createMorphologyFilter(cv::MORPH_ERODE, mask.type(), cv::Mat(), cv::Size(-1, -1), config[2]);
	erodeFilter->apply(maskGpu, maskGpu);

	// merge

	cv::cuda::GpuMat finalMask;
	maskGpu.copyTo(finalMask, depthMaskGpu);

	// copy

	zerosGpu.copyTo(resultGpu);
	frame.copyTo(resultGpu, finalMask);

	resultGpu.download(result);
}
#endif


//void FrameProcessor::gpuDifference2(int config[], cv::cuda::GpuMat & left, cv::cuda::GpuMat & left_background, cv::cuda::GpuMat & right, cv::cuda::GpuMat & right_background, cv::Mat & result_left, cv::Mat & result_right)
//{
//	cv::cuda::absdiff(left, left_background, maskGpu);
//
//	// gaussian
//	filter->apply(maskGpu, maskGpu);
//
//	// convert to greyscale
//	cv::cuda::cvtColor(maskGpu, maskGpu, cv::COLOR_RGBA2GRAY);
//
//	// threshold
//	cv::cuda::threshold(maskGpu, maskGpu, config[0], 255, cv::THRESH_BINARY);
//
//	//  dilate erode
//	cv::Ptr<cv::cuda::Filter> dilateFilter = cv::cuda::createMorphologyFilter(cv::MORPH_DILATE, mask.type(), cv::Mat(), cv::Size(-1, -1), config[3]);
//	dilateFilter->apply(maskGpu, maskGpu);
//
//	cv::Ptr<cv::cuda::Filter> erodeFilter = cv::cuda::createMorphologyFilter(cv::MORPH_ERODE, mask.type(), cv::Mat(), cv::Size(-1, -1), config[2]);
//	erodeFilter->apply(maskGpu, maskGpu);
//
//	zerosGpu.copyTo(resultGpu);
//	left.copyTo(resultGpu, maskGpu);
//
//	resultGpu.download(result_left);
//
//	// RIGHT
//
//	cv::cuda::absdiff(right, right_background, maskGpu);
//
//	// gaussian
//	filter->apply(maskGpu, maskGpu);
//
//	// convert to greyscale
//	cv::cuda::cvtColor(maskGpu, maskGpu, cv::COLOR_RGBA2GRAY);
//
//	// threshold
//	cv::cuda::threshold(maskGpu, maskGpu, config[0], 255, cv::THRESH_BINARY);
//
//	dilateFilter = cv::cuda::createMorphologyFilter(cv::MORPH_DILATE, mask.type(), cv::Mat(), cv::Size(-1, -1), config[3]);
//	dilateFilter->apply(maskGpu, maskGpu);
//
//	erodeFilter = cv::cuda::createMorphologyFilter(cv::MORPH_ERODE, mask.type(), cv::Mat(), cv::Size(-1, -1), config[2]);
//	erodeFilter->apply(maskGpu, maskGpu);
//
//	zerosGpu.copyTo(resultGpu);
//	right.copyTo(resultGpu, maskGpu);
//
//	resultGpu.download(result_right);
//}

void FrameProcessor::depthDifference(int config[], cv::Mat & frame, cv::Mat & background_color, cv::Mat & depth, cv::Mat & background_depth, cv::Mat & result)
{
	std::cout << config[1] << std::endl;
	// depth

	cv::Mat depthMask;
	threshold(depth, depthMask, config[1], 255, cv::THRESH_BINARY);

	if (depthMask.channels() > 1)
		cvtColor(depthMask, depthMask, CV_BGR2GRAY);

	//cv::imshow("Depth Mask", depthMask);

	// color

	absdiff(frame, background_color, mask);

	if (mask.channels() > 1)
		cvtColor(mask, mask, CV_BGR2GRAY);

	cv::GaussianBlur(mask, mask, cv::Size(5, 5), 3.5, 3.5);

	threshold(mask, mask, config[0], 255, cv::THRESH_BINARY);
	cv::dilate(mask, mask, cv::Mat(), cv::Size(-1, -1), config[3]);
	cv::erode(mask, mask, cv::Mat(), cv::Size(-1, -1), config[2]);

	// cv::imshow("Color Mask", mask);

	// cv::imshow("Color Mask", mask);

	cv::Mat final;
	mask.copyTo(final, depthMask);

	//cv::imshow("Mask", final);
	//cv::waitKey(5);

	// cv::imshow("Final Mask", mask);

	zeros.copyTo(result);
	frame.copyTo(result, final);
}

