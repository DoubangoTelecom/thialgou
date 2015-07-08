/*
* Copyright (C) 2013 Mamadou DIOP
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source "thialgou" project <http://code.google.com/p/thialgou/>
*/
#ifndef _THIALGOU_COMMON_MAIN_H_
#define _THIALGOU_COMMON_MAIN_H_

#include "common_config.h"

#include <vector>
#include <string>

#if defined(_MSC_VER)
#       pragma warning( push )
#       pragma warning( disable : 4251 )
#endif

// "Common" and "JVSM" use same enum names. Use fake defs to avoid linking errors
typedef int CommonEnumFake;
typedef void* Int32Ptr;

class CommonMainH264;
class CommonSyntax;
class CommonControl;
class CommonError;
class CommonElt;
class CommonMb;
class CommonHeader;
class CommonFunction;

//
//	CommonElementType_t
//
typedef enum CommonEltType_e
{
	CommonEltType_Header,
	CommonEltType_Function,
	CommonEltType_Syntax,
	CommonEltType_Control,
	CommonEltType_Error,
	CommonEltType_Mb,

	/* H264 */
	CommonEltType_Section, // contains NALUs
	CommonEltType_Nalu,
}
CommonElementType_t;

typedef enum CommonEltMbDataType_e
{
	CommonEltMbDataType_Final, 
	CommonEltMbDataType_PreFilter,
	CommonEltMbDataType_Predicted, 
	CommonEltMbDataType_Residual,
	CommonEltMbDataType_ResidualBase,
	CommonEltMbDataType_IDCTCoeffsQuant,
	CommonEltMbDataType_IDCTCoeffsScale,
#if !defined (SWIG)
	COMMON_ELTMB_DATATYPE_MAX
#endif
}
CommonEltMbDataType_t;

typedef enum CommonEltMbType_e
{
	CommonEltMbType_Basic, 
	CommonEltMbType_H264
}
CommonEltMbType_t;

typedef enum CommonYuvLine_e
{
	CommonYuvLine_Y,
	CommonYuvLine_U,
	CommonYuvLine_V,
#if !defined (SWIG)
	COMMON_YUV_LINE_MAX
#endif
}
CommonYuvLine_t;

//
//	CommonElementType_t
//
typedef enum CommonHeaderType_e
{
	CommonHeaderType_None,
	CommonHeaderType_SPS,
	CommonHeaderType_PPS,
	CommonHeaderType_Slice
}
CommonHeaderType_t;

typedef struct CommonMv
{
	int32_t x;
	int32_t y;
}
CommonMv;


//
//	CommonElt
//
class COMMON_API CommonElt
{
protected:
	CommonElt(CommonElementType_t eType);
public:
	virtual ~CommonElt();
#if !defined(SWIG)
	const CommonElt* addElt(CommonElt** ppElt)const;
	const CommonSyntax* addSyntax(std::string strName, std::string strDescriptor, double dValue = 0.0)const;
	const CommonControl* addControl(std::string strName, std::string strExpression, double dValue = 0.0)const;
	const CommonFunction* addFunction(std::string strFuncName)const;
	const CommonError* addError(std::string strDescription)const;
	const CommonMb* addMb(uint32_t uiIndex, uint32_t uiX, uint32_t uiY)const;
	const CommonElt* find(CommonElementType_t eType)const;
	const CommonMb* findMb(uint32_t uiAddress, uint32_t uiLayerId)const;
	void setBitsStart(std::string strBitsStart)const { const_cast<CommonElt*>(this)->m_strBitsStart = strBitsStart; }
	void setBitsVal(std::string strBitsVal)const { const_cast<CommonElt*>(this)->m_strBitsVal = strBitsVal; }
	void setBitsCount(unsigned nCount)const { const_cast<CommonElt*>(this)->m_nBitsCount = nCount; }
#endif
	COMMON_INLINE virtual CommonElementType_t getType()const { return m_eType; }
	COMMON_INLINE virtual std::vector<CommonElt* > getElements()const { return m_Elements; }
	COMMON_INLINE virtual std::string getBitsStart() { return m_strBitsStart; }
	COMMON_INLINE virtual std::string getBitsVal() { return m_strBitsVal; }
	COMMON_INLINE virtual unsigned getBitsCount() { return m_nBitsCount; }

private:
	static bool del(CommonElt* pElt)
	{ 
		if(pElt)
		{
			delete pElt;
		}
		return true;
	}
private:
	std::vector<CommonElt *> m_Elements;
	CommonElementType_t m_eType;
	std::string m_strBitsStart;
	std::string m_strBitsVal;
	unsigned m_nBitsCount;
};



//
//	CommonSyntax
//
class COMMON_API CommonSyntax : public CommonElt
{
public:
	CommonSyntax(std::string strName, std::string strDescriptor, double dValue);
	virtual ~CommonSyntax();
	COMMON_INLINE std::string getName() { return m_strName; }
	COMMON_INLINE std::string getDescriptor() { return m_strDescriptor; }
	COMMON_INLINE double getValue() { return m_dValue; }
#if !defined(SWIG)
	COMMON_INLINE void setValue(double value)const { const_cast<CommonSyntax*>(this)->m_dValue = value; }
#endif
private:
	std::string m_strName;
	std::string m_strDescriptor;
	double m_dValue;
};

//
//	CommonHeader
//
class COMMON_API CommonHeader : public CommonElt
{
public:
	CommonHeader(CommonHeaderType_t eType, std::string strFuncName);
	virtual ~CommonHeader();
	COMMON_INLINE virtual CommonHeaderType_t getHeaderType() { return m_eType; }
	COMMON_INLINE virtual std::string getFuncName() { return m_strFuncName; }
private:
	CommonHeaderType_t m_eType;
	std::string m_strFuncName;
};

//
//	CommonFunction
//
class COMMON_API CommonFunction : public CommonElt
{
public:
	CommonFunction(std::string strFuncName);
	virtual ~CommonFunction();
	COMMON_INLINE virtual std::string getFuncName() { return m_strFuncName; }
private:
	std::string m_strFuncName;
};

//
//	CommonControl
//
class COMMON_API CommonControl : public CommonElt
{
public:
	CommonControl(std::string strName, std::string strExpression, double dValue);
	virtual ~CommonControl();
	COMMON_INLINE virtual std::string getName() { return m_strName; }
	COMMON_INLINE virtual std::string getExpression() { return m_strExpression; }
	COMMON_INLINE virtual double getValue() { return m_dValue; }
private:
	std::string m_strName;
	std::string m_strExpression;
	double m_dValue;
};


//
//	CommonError
//
class COMMON_API CommonError : public CommonElt
{
public:
	CommonError(std::string strDescription);
	virtual ~CommonError();
	COMMON_INLINE virtual std::string getDescription() { return m_strDescription; }
private:
	std::string m_strDescription;
};

//
//	CommonMb
//
class COMMON_API CommonMb : public CommonElt
{
public:
	CommonMb(uint32_t uiAddress, uint32_t uiX, uint32_t uiY, CommonEltMbType_t eMbType = CommonEltMbType_Basic);
	virtual ~CommonMb();
	virtual Int32Ptr getData(CommonEltMbDataType_t eType, CommonYuvLine_t eLine);
	COMMON_INLINE CommonEltMbType_t getMbType()const { return m_eMbType; }
	COMMON_INLINE uint32_t getAddress()const { return m_uiAddress; }
	COMMON_INLINE uint32_t getX()const { return m_uiX; }
	COMMON_INLINE uint32_t getY()const { return m_uiY; }
	COMMON_INLINE uint32_t getWidth(CommonYuvLine_t eLine) { return m_nWidth[(int)eLine]; }
	COMMON_INLINE uint32_t getHeight(CommonYuvLine_t eLine) { return m_nHeight[(int)eLine]; }
#if !defined(SWIG)
	COMMON_INLINE void setWidth(CommonYuvLine_t eLine, uint32_t nVal) { m_nWidth[(int)eLine] = nVal; }
	COMMON_INLINE void setHeight(CommonYuvLine_t eLine, uint32_t nVal) { m_nHeight[(int)eLine] = nVal; }
	COMMON_INLINE void setSize(CommonYuvLine_t eLine, uint32_t nWidth, uint32_t nHeight) { setWidth(eLine, nWidth), setHeight(eLine, nHeight); }
	void setData(CommonEltMbDataType_t eType, CommonYuvLine_t eLine, const void* pcSrcData, uint32_t nSrcPixelSize, uint32_t nSrcStride, uint32_t nSrcWidth, uint32_t nSrcHeight)const;
	void setData(CommonEltMbDataType_t eType, 
			const void* pSrcY, uint32_t nSrcYStride, uint32_t nSrcYWidth, uint32_t nSrcYHeight,
			const void* pSrcU, uint32_t nSrcUStride, uint32_t nSrcUWidth, uint32_t nSrcUHeight,
			const void* pSrcV, uint32_t nSrcVStride, uint32_t nSrcVWidth, uint32_t nSrcVHeight,
			uint32_t nSrcPixelSize
		)const;

#endif
private:
	CommonEltMbType_t m_eMbType;
	uint32_t m_uiAddress, m_uiX, m_uiY;
	uint32_t m_nWidth[COMMON_YUV_LINE_MAX], m_nHeight[COMMON_YUV_LINE_MAX];
	int32_t *m_pData[COMMON_ELTMB_DATATYPE_MAX][COMMON_YUV_LINE_MAX]; /*(CommonEltMbDataType_t, CommonYuvLine_t)*/
};

//
//	CommonMain
//
class COMMON_API CommonMain
{
public:
	CommonMain();
	virtual ~CommonMain();

	virtual int OnElt(const CommonElt* pcElt)const { return 0; }
	virtual unsigned getCurrentPictureId()const = 0;
	virtual unsigned getCurrentSliceId()const = 0;
	virtual unsigned getCurrentLayerId()const = 0;

	static void setMainH264(const CommonMainH264* pMainH264) { g_pMainH264 = pMainH264; }
	static const CommonMainH264* getMainH264() { return g_pMainH264; }
	static const CommonMain* getMain() { return (const CommonMain*)getMainH264(); } // FIXME: for now only H.264 is supported
#if !defined(SWIG)
	void setEltCurrent(const CommonElt* pElt)const { const_cast<CommonMain*>(this)->m_pEltCurrent = pElt; }
	const CommonElt* getEltCurrent()const { return m_pEltCurrent; }
	void setEltLatest(const CommonElt* pElt)const { const_cast<CommonMain*>(this)->m_pEltLatest = pElt; }
	const CommonElt* getEltLatest()const { return m_pEltLatest; }
#endif
private:
	static const CommonMainH264* g_pMainH264;
	const CommonElt* m_pEltCurrent;
	const CommonElt* m_pEltLatest;
};

#if defined(_MSC_VER)
#       pragma warning( pop )
#endif

#endif /* _THIALGOU_COMMON_MAIN_H_ */
