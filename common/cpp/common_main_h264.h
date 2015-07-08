/*
* Copyright (C) 2013 Mamadou DIOP
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source "thialgou" project <http://code.google.com/p/thialgou/>
*/
#ifndef _THIALGOU_COMMON_H264_MAIN_H_
#define _THIALGOU_COMMON_H264_MAIN_H_

#include "common_config.h"
#include "common_main.h"

#if defined(_MSC_VER)
#       pragma warning( push )
#       pragma warning( disable : 4251 )
#endif

#define COMMON_MAX_TRACE_LAYERS  (16*8+3)
#define COMMON_MAX_BITS_LENGTH   128
#define COMMON_MAX_LINE_LENGTH   1024

#if defined(SWIG)
typedef enum MbMode MbModeFake;
typedef enum BlkMode BlkModeFake;
typedef enum SliceType SliceTypeFake;
typedef enum Intra4x4PredMode Intra4x4PredModeFake;
typedef enum IntraChromaPredMode IntraChromaPredModeFake;
#else
typedef int MbModeFake;
typedef int BlkModeFake;
typedef int SliceTypeFake;
typedef int Intra4x4PredModeFake;
typedef int IntraChromaPredModeFake;

#endif

//
//	CommonMbH264
//
class COMMON_API CommonMbH264 : public CommonMb
{
public:
	CommonMbH264(uint32_t uiAddress, uint32_t uiX, uint32_t uiY);
	virtual ~CommonMbH264();
#if !defined(SWIG)
	void setMode(MbModeFake eMode)const { const_cast<CommonMbH264*>(this)->m_eMode = eMode; }
	void setSliceType(SliceTypeFake eSliceType)const { const_cast<CommonMbH264*>(this)->m_eSliceType = eSliceType; }
	void setMbType(uint32_t uMbType)const { const_cast<CommonMbH264*>(this)->m_uMbType = uMbType; }
	void setQP(int32_t nQP)const { const_cast<CommonMbH264*>(this)->m_nQP = nQP; }
	void setQPC(int32_t nQP)const { const_cast<CommonMbH264*>(this)->m_nQPC = nQP; }
	void setCBP(int32_t nCBP)const { const_cast<CommonMbH264*>(this)->m_nCBP = nCBP; }
	void setBlkModes(int32_t lumaBlk8x8Idx, BlkModeFake mode)const { const_cast<CommonMbH264*>(this)->m_BlkModes[lumaBlk8x8Idx] = mode; }
	void setIntra4x4PredMode(int32_t lumaBlkIdx, Intra4x4PredModeFake mode)const { const_cast<CommonMbH264*>(this)->m_Intra4x4PredMode[lumaBlkIdx] = mode; }
	void setIntraChromaPredMode(IntraChromaPredModeFake mode)const { const_cast<CommonMbH264*>(this)->m_IntraChromaPredMode = mode; }
	void setMvdL0(int32_t i, int32_t x, int32_t y)const;
	void setMvdL1(int32_t i, int32_t x, int32_t y)const;
	void setMvL0(int32_t i, int32_t x, int32_t y)const;
	void setMvL1(int32_t i, int32_t x, int32_t y)const;
	void setRefIdxL0(int32_t i, int32_t value)const { const_cast<CommonMbH264*>(this)->m_ref_idx_L0[i] = value; }
	void setRefIdxL1(int32_t i, int32_t value)const { const_cast<CommonMbH264*>(this)->m_ref_idx_L1[i] = value; }
	void setPOC0(int32_t i, int32_t value)const { const_cast<CommonMbH264*>(this)->m_POC0[i] = value; }
	void setPOC1(int32_t i, int32_t value)const { const_cast<CommonMbH264*>(this)->m_POC1[i] = value; }
	void setInCropWindow(bool value)const { const_cast<CommonMbH264*>(this)->m_bInCropWindow = value; }; // SVC
	void setResidualPredictionFlag(bool value)const { const_cast<CommonMbH264*>(this)->m_bResidualPredictionFlag = value; }; // SVC
	void setBLSkippedFlag(bool value)const { const_cast<CommonMbH264*>(this)->m_bBLSkippedFlag = value; }; // SVC
#endif
	MbModeFake getMode()const { return m_eMode; }
	SliceTypeFake getSliceType()const { return m_eSliceType; }
	void getIntra4x4PredMode(Intra4x4PredModeFake *modes)const;
	IntraChromaPredModeFake getIntraChromaPredMode()const { return m_IntraChromaPredMode; }
	void getBlkModes(BlkModeFake *modes)const;
	std::vector<CommonMv> getMvdL0()const; // TODO: use pointer
	std::vector<CommonMv> getMvdL1()const; // TODO: use pointer
	std::vector<CommonMv> getMvL0()const; // TODO: use pointer
	std::vector<CommonMv> getMvL1()const; // TODO: use pointer
	void getRefIdxL0(int32_t *values)const;
	void getRefIdxL1(int32_t *values)const;
	void getPOC0(int32_t *values)const;
	void getPOC1(int32_t *values)const;
	uint32_t getMbType()const { return m_uMbType; }
	int32_t getQP()const { return m_nQP; }
	int32_t getQPC()const { return m_nQPC; }
	int32_t getCBP()const { return m_nCBP; }
	bool isInter8x8()const;
    bool isIntra4x4()const;
    bool isIntra16x16()const;
    bool isIntra()const;
	bool isInCropWindow()const { return m_bInCropWindow; }; // SVC
	bool isResidualPredictionFlag()const { return m_bResidualPredictionFlag; }; // SVC
	bool isBLSkippedFlag()const { return m_bBLSkippedFlag; }; // SVC
private:
	MbModeFake m_eMode;
	SliceTypeFake m_eSliceType;
	uint32_t m_uMbType;
	int32_t m_nQP;
	int32_t m_nQPC;
	int32_t m_nCBP;
	Intra4x4PredModeFake m_Intra4x4PredMode[16];
	IntraChromaPredModeFake m_IntraChromaPredMode;
	BlkModeFake m_BlkModes[4]; // for Inter8x8 only
	std::vector<CommonMv> m_mvd_l0;
	std::vector<CommonMv> m_mvd_l1;
	std::vector<CommonMv> m_mvL0;
	std::vector<CommonMv> m_mvL1;
	int32_t m_ref_idx_L0[4/*PartNum(8x8 blk)*/];
    int32_t m_ref_idx_L1[4/*PartNum(8x8 blk)*/];
	int32_t m_POC0[4/*PartNum(8x8 blk)*/];
	int32_t m_POC1[4/*PartNum(8x8 blk)*/];
	bool m_bInCropWindow; // SVC "InCropWindow"
	bool m_bResidualPredictionFlag; // SVC "residual_prediction_flag"
	bool m_bBLSkippedFlag; // SVC "base_mode_flag"
};

//
//	CommonH264Section (contains NALUs)
//
class COMMON_API CommonH264Section : public CommonElt
{
public:
#if !defined(SWIG)
	CommonH264Section();
#endif
	virtual ~CommonH264Section();
private:
};

//
//	CommonH264Nalu
//
class COMMON_API CommonH264Nalu : public CommonElt
{
public:
#if !defined(SWIG)
	CommonH264Nalu(/*enum NalUnitType*/CommonEnumFake eType);
#endif
	virtual ~CommonH264Nalu();
	COMMON_INLINE enum NalUnitType getNaluType() { return m_eType; }
#if !defined(SWIG)
	bool isUndefined()const;
	void setNaluType(CommonEnumFake eType)const;
	void setLayerId(unsigned v)const;
	void setPictureId(unsigned v)const;
	void setSliceId(unsigned v)const;
#endif
	unsigned getLayerId()const { return m_uLayerId; }
	unsigned getPictureId()const { return m_uPictureId; }
	unsigned getSliceId()const { return m_uSliceId; }
private:
	enum NalUnitType m_eType;
	unsigned m_uLayerId;
	unsigned m_uPictureId;
	unsigned m_uSliceId;
};

class COMMON_API CommonMainH264 : public CommonMain
{
#if !defined(SWIG)
class CommonMainH264DQId
  {
  public:
    static void create  ( CommonMainH264DQId*& rpcCommonMainH264DQId,
                            const char* pucBaseName,
                            unsigned        uiDQIdplus1 );
    void        destroy ();
    void        output  ( const char* pucLine );
    void        storePos();
    void        resetPos();
  ~CommonMainH264DQId();
  protected:
    CommonMainH264DQId();
  private:
  };
#endif
public:
	CommonMainH264();
	virtual ~CommonMainH264();

	// @Override (CommonMain)
	virtual unsigned getCurrentPictureId()const;
	virtual unsigned getCurrentSliceId()const;
	virtual unsigned getCurrentLayerId()const;

#if !defined(SWIG)
	void setLayer(int iDQId);
	void startPicture();
	void startSlice();
	void startMb(unsigned uiMbAddress);

	void printHeading(const char* pcString, bool bReset = true);


	void countBits(unsigned uiBitCount);
	void printPos();
	void addBits(unsigned uiVal, unsigned uiLength);
	void printCode(unsigned uiVal);
	void printCode(signed iVal);
	void printString(const char* pcString);
	void printType(const char* pcString);
	void printVal(unsigned uiVal);
	void printVal(signed iVal);
	void printXVal(unsigned uiVal);
	void newLine();

	void setSectionCurrent(const CommonH264Section* pSection)const { const_cast<CommonMainH264*>(this)->m_pSection = pSection; }
	const CommonH264Section* getSectionCurrent()const { return m_pSection; }
#endif

private:
  unsigned         m_uiDQIdplus3;
  CommonMainH264DQId*   m_pCommonMainH264DQId[COMMON_MAX_TRACE_LAYERS];
  unsigned         m_uiFrameNum   [COMMON_MAX_TRACE_LAYERS];
  unsigned         m_uiSliceNum   [COMMON_MAX_TRACE_LAYERS];
  unsigned         m_uiPosCounter [COMMON_MAX_TRACE_LAYERS];
  unsigned         m_uiSymCounter [COMMON_MAX_TRACE_LAYERS];
  unsigned         m_uiStorePosCnt[COMMON_MAX_TRACE_LAYERS];
  unsigned         m_uiStoreSymCnt[COMMON_MAX_TRACE_LAYERS];
  char         m_acType       [COMMON_MAX_TRACE_LAYERS][16];
  char         m_acPos        [COMMON_MAX_TRACE_LAYERS][16];
  char         m_acCode       [COMMON_MAX_TRACE_LAYERS][16];
  char         m_acBits       [COMMON_MAX_TRACE_LAYERS][COMMON_MAX_BITS_LENGTH];
  char         m_acLine       [COMMON_MAX_TRACE_LAYERS][COMMON_MAX_LINE_LENGTH];
  const			CommonH264Section* m_pSection;
};

#if defined(_MSC_VER)
#       pragma warning( pop )
#endif

#endif /* _THIALGOU_COMMON_H264_MAIN_H_ */
