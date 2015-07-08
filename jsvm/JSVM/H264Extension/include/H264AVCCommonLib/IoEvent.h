#ifndef _H264AVCCOMMONLIBSTATIC_IOEVENT_H_
#define _H264AVCCOMMONLIBSTATIC_IOEVENT_H_

#include "H264AVCCommonLib.h"

typedef enum h264HdrType_e
{
	h264HdrType_None,
	h264HdrType_SPS,
	h264HdrType_PPS,
	h264HdrType_Slice
}
h264HdrType_t;

//
//	h264HdrEventParsing
//
class h264HdrEventParsing
{
public:
	h264HdrEventParsing() {}
	virtual ~h264HdrEventParsing(){};

public:
	virtual int OnBeginHdr(h264HdrType_t eType, const char* pFuncName = NULL)const{ return 0; }
	virtual int OnEndHdr(h264HdrType_t eType)const{ return 0; }
	virtual int OnErrorHdr(h264HdrType_t eType)const{ return 0; }
	virtual int OnBeginFunc(const char* pFuncName)const{ return 0; }
	virtual int OnEndFunc(const char* pFuncName)const{ return 0; }
	virtual int OnSyntaxElt(const char* pName, const char* pDescriptor, int value)const { return 0; }
	virtual int OnBeginCtrl(const char* pName, const char* pExpression, int value)const { return 0; }
	virtual int OnEndCtrl(const char* pName)const { return 0; }
	virtual int OnFrameDraw(const void* pLum,
				const void* pCb,
				const void* pCr,
				unsigned uiWidth,
				unsigned uiHeight,
				unsigned uiStride)const { return 0; }
};

#endif /* _H264AVCCOMMONLIBSTATIC_IOEVENT_H_ */
