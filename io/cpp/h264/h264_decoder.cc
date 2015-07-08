/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source "thialgou" project <http://code.google.com/p/thialgou/>
*/
#include "h264_decoder.h"
#include "H264AVCCommonLib/IoEvent.h"

h264Decoder::h264Decoder(const char* inputFilePath, const char* outputFilePath)
: m_bInitialized(false)
, m_pH264AVCDecoderInst(NULL)
, m_pReadStream(NULL)
#ifdef SHARP_AVC_REWRITE_OUTPUT
, m_pWriteStream(NULL)
#else
, m_pWriteYuv(NULL)
#endif
, m_pParameter(NULL)
, m_pIoEventHdr(NULL)
{
	ErrVal errVal;
	m_pParameter = new H264DecoderParameter();
	assert(m_pParameter);

	errVal = ReadBitstreamFile::create(m_pReadStream);
	assert(errVal == Err::m_nOK);

#ifdef SHARP_AVC_REWRITE_OUTPUT
	errVal = WriteBitstreamToFile::create(m_pWriteStream);
#else
	errVal = WriteYuvToFile::create(m_pWriteYuv);
#endif
	assert(errVal == Err::m_nOK);

	errVal = H264AVCDecoderInst::create(m_pH264AVCDecoderInst);
	assert(errVal == Err::m_nOK);

	errVal = m_pParameter->init(kProjectName, (const Char*)inputFilePath, (const Char*)outputFilePath);
	assert(errVal == Err::m_nOK);

	errVal = m_pReadStream->init(m_pParameter->cBitstreamFile);
	assert(errVal == Err::m_nOK);

#ifdef SHARP_AVC_REWRITE_OUTPUT
	errVal = m_pWriteStream->init(m_pParameter->cYuvFile);
	assert(errVal == Err::m_nOK);
	errVal = pcH264AVCDecoderTest->init(m_pParameter, m_pReadStream, m_pWriteStream);
	assert(errVal == Err::m_nOK);
#else
	errVal = m_pWriteYuv->init(m_pParameter->cYuvFile);
	assert(errVal == Err::m_nOK);
	errVal = m_pH264AVCDecoderInst->init(m_pParameter, m_pReadStream, m_pWriteYuv);
	assert(errVal == Err::m_nOK);
#endif

	m_bInitialized = true;
}

bool h264Decoder::IsInitialized()
{ 
	return  m_bInitialized; 
}

bool h264Decoder::SetIoEventHdr(const h264HdrEventParsing* pIoEventHdr)
{
	m_pIoEventHdr = pIoEventHdr;
	if(m_pH264AVCDecoderInst){
		m_pH264AVCDecoderInst->setIoEventHdr(m_pIoEventHdr);
		m_pReadStream->setIoEventHdr(m_pIoEventHdr);
	}
	return true;
}

bool h264Decoder::Decode(uint32_t frameCount)
{
	assert(m_bInitialized);

	ErrVal errVal;

	errVal = m_pH264AVCDecoderInst->go(frameCount);
	return (errVal == Err::m_nOK);
}

h264Decoder::~h264Decoder()
{
	ErrVal errVal;

	if(m_pH264AVCDecoderInst){
		errVal = m_pH264AVCDecoderInst->uninit();
		errVal = m_pH264AVCDecoderInst->destroy();
	}
	if(m_pReadStream){
		errVal = m_pReadStream->uninit();
		errVal = m_pReadStream->destroy();
	}
#ifdef SHARP_AVC_REWRITE_OUTPUT
	if(m_pWriteStream){
		errVal = m_pWriteStream->uninit();
		errVal = m_pWriteStream->destroy();
	}
#else
	if(m_pWriteYuv){
		errVal = m_pWriteYuv->uninit();
		errVal = m_pWriteYuv->destroy();
	}
#endif
	if(m_pParameter){
		delete m_pParameter, m_pParameter = NULL;
	}
}

