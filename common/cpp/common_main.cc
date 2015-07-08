/*
* Copyright (C) 2013 Mamadou DIOP
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source "thialgou" project <http://code.google.com/p/thialgou/>
*/
#include "common_main.h"
#include "common_main_h264.h"

#include <algorithm>    /* std::remove_if */
#include <assert.h>

//
//	CommonElt
//

CommonElt::CommonElt(CommonElementType_t eType)
: m_eType(eType)
, m_nBitsCount(0)
{
}
CommonElt::~CommonElt()
{
	if(CommonMain::getMain()->getEltLatest() == this)
	{
		CommonMain::getMain()->setEltLatest(NULL);
	}
	std::remove_if(m_Elements.begin(), m_Elements.end(), &CommonElt::del);
}

const CommonElt* CommonElt::addElt(CommonElt** ppElt)const
{
	if(ppElt && *ppElt)
	{
		CommonElt* pcElt = *ppElt;
		const_cast<CommonElt*>(this)->m_Elements.push_back(pcElt), CommonMain::getMain()->setEltLatest(pcElt);
		*ppElt = NULL;
		return pcElt;
	}
	return NULL;
}

const CommonSyntax* CommonElt::addSyntax(std::string strName, std::string strDescriptor, double dValue)const
{
	CommonSyntax* pSyntax = new CommonSyntax(strName, strDescriptor, dValue);
	if(pSyntax) const_cast<CommonElt*>(this)->m_Elements.push_back(pSyntax), CommonMain::getMain()->setEltLatest(pSyntax);
	return pSyntax;
}

const CommonControl* CommonElt::addControl(std::string strName, std::string strExpression, double dValue)const
{
	CommonControl* pControl = new CommonControl(strName, strExpression, dValue);
	if(pControl) const_cast<CommonElt*>(this)->m_Elements.push_back(pControl), CommonMain::getMain()->setEltLatest(pControl);
	return pControl;
}

const CommonFunction* CommonElt::addFunction(std::string strFuncName)const
{
	CommonFunction* pFunction = new CommonFunction(strFuncName);
	if(pFunction) const_cast<CommonElt*>(this)->m_Elements.push_back(pFunction), CommonMain::getMain()->setEltLatest(pFunction);
	return pFunction;
}

const CommonError* CommonElt::addError(std::string strDescription)const
{
	CommonError* pError = new CommonError(strDescription);
	if(pError) const_cast<CommonElt*>(this)->m_Elements.push_back(pError), CommonMain::getMain()->setEltLatest(pError);
	return pError;
}

const CommonMb* CommonElt::addMb(uint32_t uiIndex, uint32_t uiX, uint32_t uiY)const
{
	CommonMb* pMb = new CommonMbH264/*CommonMb*/(uiIndex, uiX, uiY); // FIXME: for now only H.264 is supported
	if(pMb) const_cast<CommonElt*>(this)->m_Elements.push_back(pMb), CommonMain::getMain()->setEltLatest(pMb);
	return pMb;
}

const CommonElt* CommonElt::find(CommonElementType_t eType)const
{
	CommonElt* This = const_cast<CommonElt*>(this);
	const CommonElt* pcElt;
	for(std::vector<CommonElt* >::iterator it = This->m_Elements.begin() ; it != This->m_Elements.end(); ++it)
	{
		if((*it)->m_eType == eType)
		{
			return (*it);
		}
		if((pcElt = (*it)->find(eType)))
		{
			return pcElt;
		}
	}
	return NULL;
}

const CommonMb* CommonElt::findMb(uint32_t uiAddress, uint32_t uiLayerId)const
{
	CommonElt* This = const_cast<CommonElt*>(this);
	const CommonMb* pcMb;
	for(std::vector<CommonElt* >::iterator it = This->m_Elements.begin() ; it != This->m_Elements.end(); ++it)
	{
		if((*it)->m_eType == CommonEltType_Mb)
		{
			if((pcMb = dynamic_cast<const CommonMb*>((*it))) && pcMb->getAddress() == uiAddress) {
				return pcMb;
			}
		}
		else {
			if ((*it)->m_eType == CommonEltType_Nalu && ((const CommonH264Nalu*)(*it))->getLayerId() != uiLayerId)
			{
				continue;
			}
			if((pcMb = (*it)->findMb(uiAddress, uiLayerId)))
			{
				return pcMb;
			}
		}
	}
	return NULL;
}


//
//	CommonSyntax
//

CommonSyntax::CommonSyntax(std::string strName, std::string strDescriptor, double dValue)
: CommonElt(CommonEltType_Syntax)
, m_strName(strName) 
, m_strDescriptor(strDescriptor) 
, m_dValue(dValue)
{
}

CommonSyntax::~CommonSyntax()
{

}

//
//	CommonHeader
//

CommonHeader::CommonHeader(CommonHeaderType_t eType, std::string strFuncName)
: CommonElt(CommonEltType_Header)
, m_eType(eType)
, m_strFuncName(strFuncName)
{
}

CommonHeader::~CommonHeader()
{

}

//
//	CommonFunction
//

CommonFunction::CommonFunction(std::string strFuncName)
: CommonElt(CommonEltType_Function)
, m_strFuncName(strFuncName)
{

}
CommonFunction::~CommonFunction()
{

}

//
//	CommonControl
//


CommonControl::CommonControl(std::string strName, std::string strExpression, double dValue)
: CommonElt(CommonEltType_Control)
, m_strName(strName)
, m_strExpression(strExpression)
, m_dValue(dValue)
{
}

CommonControl::~CommonControl()
{
}

//
//	CommonError
//

CommonError::CommonError(std::string strDescription)
: CommonElt(CommonEltType_Error)
, m_strDescription(strDescription)
{
}

CommonError::~CommonError()
{
}

//
//	CommonMb
//

CommonMb::CommonMb(uint32_t uiAddress, uint32_t uiX, uint32_t uiY, CommonEltMbType_t eMbType)
: CommonElt(CommonEltType_Mb)
, m_uiAddress(uiAddress)
, m_uiX(uiX)
, m_uiY(uiY)
, m_eMbType(eMbType)
{
	memset(m_pData, 0, sizeof(m_pData));
	for(int i = 0; i < COMMON_YUV_LINE_MAX; ++i)
	{
		// Default: YUV420
		m_nWidth[i] = i ? 8 : 16;
		m_nHeight[i] = i ? 8 : 16;
	}
	for(int i = 0; i < COMMON_ELTMB_DATATYPE_MAX; ++i)
	{
		for(int j = 0; j < COMMON_YUV_LINE_MAX; ++j)
		{
			m_pData[i][j] = (int32_t*)calloc(16 * 16, sizeof(int32_t));
			assert(m_pData[i][j]);
		}
	}
}

CommonMb::~CommonMb()
{
	for(int i = 0; i < COMMON_ELTMB_DATATYPE_MAX; ++i)
	{
		for(int j = 0; j < COMMON_YUV_LINE_MAX; ++j)
		{
			if(m_pData[i][j]) free(m_pData[i][j]), m_pData[i][j] = NULL;
		}
	}
}

Int32Ptr CommonMb::getData(CommonEltMbDataType_t eType, CommonYuvLine_t eLine)
{
	assert(eType < COMMON_ELTMB_DATATYPE_MAX);
	assert(eLine < COMMON_YUV_LINE_MAX);
	return m_pData[eType][eLine];
}

void CommonMb::setData(CommonEltMbDataType_t eType, CommonYuvLine_t eLine, const void* pcSrcData, uint32_t nSrcPixelSize, uint32_t nSrcStride, uint32_t nSrcWidth, uint32_t nSrcHeight)const
{
	int32_t* pDst = (int32_t*)const_cast<CommonMb*>(this)->getData(eType, eLine);
	const uint8_t* pSrc = (const uint8_t*)pcSrcData;
	assert(nSrcPixelSize == 1 || nSrcPixelSize == 2 || nSrcPixelSize == 4);
	assert(nSrcWidth <= 16 && nSrcHeight <= 16);
	unsigned nDstSize = (nSrcWidth * nSrcHeight); // dst stride is always equal to zero
	for(unsigned i = 0; i < nDstSize; ++i)
	{
		switch(nSrcPixelSize)
		{
		case 1:
			{
				pDst[i] = *pSrc, ++pSrc;
				break;
			}
		case 2:
			{
				pDst[i] = *((int16_t*)pSrc) , pSrc += 2;
				break;
			}
		case 4:
			{
				pDst[i] = *((int32_t*)pSrc) , pSrc += 4;
				break;
			}
		}

		if(((i + 1) % nSrcWidth) == 0)
		{
			pSrc += ((nSrcStride - nSrcWidth) * nSrcPixelSize);
			// pSrc = ((const uint8_t*)pcSrcData) + (((i + 1) >> 4) * nSrcStride * nSrcPixelSize);
		}
	}
}

void CommonMb::setData(CommonEltMbDataType_t eType, 
			const void* pSrcY, uint32_t nSrcYStride, uint32_t nSrcYWidth, uint32_t nSrcYHeight,
			const void* pSrcU, uint32_t nSrcUStride, uint32_t nSrcUWidth, uint32_t nSrcUHeight,
			const void* pSrcV, uint32_t nSrcVStride, uint32_t nSrcVWidth, uint32_t nSrcVHeight,
			uint32_t nSrcPixelSize
		)const
{
	setData(eType, CommonYuvLine_Y, pSrcY, nSrcPixelSize, nSrcYStride, nSrcYWidth, nSrcYHeight);
	setData(eType, CommonYuvLine_U, pSrcU, nSrcPixelSize, nSrcUStride, nSrcUWidth, nSrcUHeight);
	setData(eType, CommonYuvLine_V, pSrcV, nSrcPixelSize, nSrcVStride, nSrcVWidth, nSrcVHeight);
}


//
//	CommonMain
//

const CommonMainH264* CommonMain::g_pMainH264 = NULL;

CommonMain::CommonMain()
: m_pEltCurrent(NULL)
, m_pEltLatest(NULL)
{

}

CommonMain::~CommonMain()
{

}
