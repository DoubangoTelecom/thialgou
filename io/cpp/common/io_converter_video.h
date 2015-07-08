/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source "thialgou" project <http://code.google.com/p/thialgou/>
*/
#ifndef _THIALGOU_IO_COMMON_VIDEO_CONVERTER_H_
#define _THIALGOU_IO_COMMON_VIDEO_CONVERTER_H_

#include "io_config.h"

typedef enum VideoChroma_e
{
	VideoChroma_YUV420,
	VideoChroma_RGB24,
	VideoChroma_RGB32
}
VideoChroma_t;

class VideoConverter
{
public:
	VideoConverter(VideoChroma_t eChromaInput, VideoChroma_t eChromaOutput);
	virtual ~VideoConverter();

private:
	VideoChroma_t m_eChromaInput;
	VideoChroma_t m_eChromaOutput;
};

class VideoConverterYUV420ToRGB32 : public VideoConverter
{
public:
	VideoConverterYUV420ToRGB32();
	virtual ~VideoConverterYUV420ToRGB32();
	static bool convert(const void* pLum,
				const void* pCb,
				const void* pCr,
				unsigned uiWidth,
				unsigned uiHeight,
				unsigned uiStride,
				void* pOutput,
				unsigned nOutputSize
				);
};

#endif /* _THIALGOU_IO_COMMON_VIDEO_CONVERTER_H_ */
