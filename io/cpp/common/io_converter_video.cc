/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source "thialgou" project <http://code.google.com/p/thialgou/>
*/
#include "io_converter_video.h"
#include "assert.h"

#include <libyuv/libyuv.h>

using namespace libyuv;

//
//	VideoConverter
// 

VideoConverter::VideoConverter(VideoChroma_t eChromaInput, VideoChroma_t eChromaOutput)
: m_eChromaInput(eChromaInput)
, m_eChromaOutput(eChromaOutput)
{
	
}

VideoConverter::~VideoConverter()
{

}


//
//	VideoConverterYUV420ToRGB
// 

VideoConverterYUV420ToRGB32::VideoConverterYUV420ToRGB32()
: VideoConverter(VideoChroma_YUV420, VideoChroma_RGB32)
{
	
}

VideoConverterYUV420ToRGB32::~VideoConverterYUV420ToRGB32()
{
}

bool VideoConverterYUV420ToRGB32::convert(
				const void* pLum,
				const void* pCb,
				const void* pCr,
				unsigned uiWidth,
				unsigned uiHeight,
				unsigned uiStride,
				void* pOutput,
				unsigned nOutputSize)
{
	static const int nOutputSampleStride = 0;
	assert(nOutputSize == ((uiHeight * uiWidth) << 2));

	return (ConvertFromI420(
					(const uint8*)pLum, (uiStride),
                    (const uint8*)pCb, ((uiStride + 1) >> 1),
                    (const uint8*)pCr, ((uiStride + 1) >> 1),
                    (uint8*)pOutput, nOutputSampleStride,
					uiWidth, uiHeight,
					(uint32) FOURCC_ARGB) == 0);
}