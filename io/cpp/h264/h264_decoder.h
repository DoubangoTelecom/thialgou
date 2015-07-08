/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source "thialgou" project <http://code.google.com/p/thialgou/>
*/
#ifndef _THIALGOU_IO_H264_DECODER_H_
#define _THIALGOU_IO_H264_DECODER_H_

#include "h264_decoder_inst.h"

class h264HdrEventParsing;

class h264Decoder
{
public:
	h264Decoder(const char* inputFilePath, const char* outputFilePath);
	virtual ~h264Decoder();		
	virtual bool IsInitialized();
	virtual bool SetIoEventHdr(const h264HdrEventParsing* pIoEventHdr);
	virtual bool Decode(uint32_t frameCount);

private:
	bool m_bInitialized;
	
	H264AVCDecoderInst*   m_pH264AVCDecoderInst;
	ReadBitstreamFile*    m_pReadStream;
#ifdef SHARP_AVC_REWRITE_OUTPUT
	WriteBitstreamToFile* m_pWriteStream;
#else
	WriteYuvToFile*       m_pWriteYuv;
#endif
	H264DecoderParameter*      m_pParameter;
	const h264HdrEventParsing* m_pIoEventHdr;
};

#endif /* _THIALGOU_IO_H264_DECODER_H_ */
