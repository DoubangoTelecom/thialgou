/*
* Copyright (C) 2013 Mamadou DIOP
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source "thialgou" project <http://code.google.com/p/thialgou/>
*/
#include "common_main_h264.h"
#include "common_h264_defs_internal.h"

#include <assert.h>

#ifdef _MSC_VER
#define snprintf _snprintf
#endif

#define COMMON_MSYS_UINT_MAX          0xFFFFFFFFU
#define MAX_LINE_LENGTH   1024
#define MAX_BITS_LENGTH   128

#define gMin(x,y) ((x)<(y)?(x):(y))
#define gMax(x,y) ((x)>(y)?(x):(y))

//
//	CommonMbH264
//

CommonMbH264::CommonMbH264(uint32_t uiAddress, uint32_t uiX, uint32_t uiY)
: CommonMb(uiAddress, uiX, uiY, CommonEltMbType_H264)
, m_nQP(INT_MAX)
, m_nQPC(INT_MAX)
, m_nCBP(INT_MAX)
, m_bInCropWindow(false)
, m_bResidualPredictionFlag(false)
, m_bBLSkippedFlag(false)
{
	
}

CommonMbH264::~CommonMbH264()
{
	m_mvd_l0.clear();
	m_mvd_l1.clear();
	m_mvL0.clear();
	m_mvL1.clear();
}

void CommonMbH264::getIntra4x4PredMode(Intra4x4PredModeFake *modes)const
{
	for (int i = 0; i < 16; i++)
	{
		modes[i] = m_Intra4x4PredMode[i];
	}
}

void CommonMbH264::getBlkModes(BlkModeFake *modes)const
{
	for (int i = 0; i < 4; i++)
	{
		modes[i] = m_BlkModes[i];
	}
}

void CommonMbH264::setMvdL0(int32_t i, int32_t x, int32_t y)const 
{
	if (const_cast<CommonMbH264*>(this)->m_mvd_l0.size() == 0) const_cast<CommonMbH264*>(this)->m_mvd_l0.reserve(16);
	CommonMv Mv = { x, y };
	const_cast<CommonMbH264*>(this)->m_mvd_l0.insert(const_cast<CommonMbH264*>(this)->m_mvd_l0.begin() + i, Mv);
}

void CommonMbH264::setMvdL1(int32_t i, int32_t x, int32_t y)const 
{
	if (const_cast<CommonMbH264*>(this)->m_mvd_l1.size() == 0) const_cast<CommonMbH264*>(this)->m_mvd_l1.reserve(16);
	CommonMv Mv = { x, y };
	const_cast<CommonMbH264*>(this)->m_mvd_l1.insert(const_cast<CommonMbH264*>(this)->m_mvd_l1.begin() + i, Mv);
}

void CommonMbH264::setMvL0(int32_t i, int32_t x, int32_t y)const 
{
	if (const_cast<CommonMbH264*>(this)->m_mvL0.size() == 0) const_cast<CommonMbH264*>(this)->m_mvL0.reserve(16);
	CommonMv Mv = { x, y };
	const_cast<CommonMbH264*>(this)->m_mvL0.insert(const_cast<CommonMbH264*>(this)->m_mvL0.begin() + i, Mv);
}

void CommonMbH264::setMvL1(int32_t i, int32_t x, int32_t y)const 
{
	if (const_cast<CommonMbH264*>(this)->m_mvL1.size() == 0) const_cast<CommonMbH264*>(this)->m_mvL1.reserve(16);
	CommonMv Mv = { x, y };
	const_cast<CommonMbH264*>(this)->m_mvL1.insert(const_cast<CommonMbH264*>(this)->m_mvL1.begin() + i, Mv);
}

std::vector<CommonMv> CommonMbH264::getMvdL0()const
{
	return m_mvd_l0;
}

std::vector<CommonMv> CommonMbH264::getMvdL1()const
{
	return m_mvd_l1;
}

std::vector<CommonMv> CommonMbH264::getMvL0()const
{
	return m_mvL0;
}

std::vector<CommonMv> CommonMbH264::getMvL1()const
{
	return m_mvL1;
}

void CommonMbH264::getRefIdxL0(int32_t *values)const
{
	for (int i = 0; i < 4; i++)
	{
		values[i] = m_ref_idx_L0[i];
	}
}

void CommonMbH264::getRefIdxL1(int32_t *values)const
{
	for (int i = 0; i < 4; i++)
	{
		values[i] = m_ref_idx_L1[i];
	}
}

void CommonMbH264::getPOC0(int32_t *values)const
{
	for (int i = 0; i < 4; i++)
	{
		values[i] = m_POC0[i];
	}
}

void CommonMbH264::getPOC1(int32_t *values)const
{
	for (int i = 0; i < 4; i++)
	{
		values[i] = m_POC1[i];
	}
}


bool CommonMbH264::isInter8x8()const { return m_eMode == INTRA_4X4-2 || m_eMode == INTRA_4X4-1; }
bool CommonMbH264::isIntra4x4()const { return m_eMode == INTRA_4X4; }
bool CommonMbH264::isIntra16x16()const { return m_eMode  > INTRA_4X4 && m_eMode < MODE_PCM; }
bool CommonMbH264::isIntra()const { return m_eMode >= INTRA_4X4; }

//
//	CommonH264Section (contains NALUs)
//
CommonH264Section::CommonH264Section()
: CommonElt(CommonEltType_Section)
{
}

CommonH264Section::~CommonH264Section()
{
}

//
//	CommonH264Nalu
//

CommonH264Nalu::CommonH264Nalu(/*enum NalUnitType*/CommonEnumFake eType)
: CommonElt(CommonEltType_Nalu)
, m_eType((enum NalUnitType)eType)
, m_uLayerId(0)
, m_uPictureId(0)
, m_uSliceId(0)
{
}

CommonH264Nalu::~CommonH264Nalu()
{
	
}

void CommonH264Nalu::setNaluType(CommonEnumFake eType)const
{
	const_cast<CommonH264Nalu*>(this)->m_eType = (enum NalUnitType)eType;
}

void CommonH264Nalu::setLayerId(unsigned v)const
{
	const_cast<CommonH264Nalu*>(this)->m_uLayerId = v;
}

void CommonH264Nalu::setPictureId(unsigned v)const
{
	const_cast<CommonH264Nalu*>(this)->m_uPictureId = v;
}

void CommonH264Nalu::setSliceId(unsigned v)const
{
	const_cast<CommonH264Nalu*>(this)->m_uSliceId = v;
}

bool CommonH264Nalu::isUndefined()const
{
	return (const_cast<CommonH264Nalu*>(this)->m_eType == NAL_UNIT_UNSPECIFIED_0);
}

//
//	CommonMainH264DQId
//

void CommonMainH264::CommonMainH264DQId::create(CommonMainH264DQId*& rpcCommonMainH264DQId,
                            const char* pucBaseName,
                            unsigned        uiDQIdplus1 )
{
  CommonMainH264DQId* p = new CommonMainH264DQId();
  assert(p);
  rpcCommonMainH264DQId = p;
}

void CommonMainH264::CommonMainH264DQId::destroy()
{
}

void CommonMainH264::CommonMainH264DQId::output(const char* pucLine)
{
}

void CommonMainH264::CommonMainH264DQId::storePos()
{
}

void CommonMainH264::CommonMainH264DQId::resetPos()
{
}
  
CommonMainH264::CommonMainH264DQId::CommonMainH264DQId()
{
}

CommonMainH264::CommonMainH264DQId::~CommonMainH264DQId()
{
	destroy();
}

//
//	CommonMainH264
//

CommonMainH264::CommonMainH264()
: CommonMain()
, m_pSection(NULL)
{
	m_uiDQIdplus3  = COMMON_MSYS_UINT_MAX;
	for(unsigned ui = 0; ui < COMMON_MAX_TRACE_LAYERS; ui++)
	{
		m_pCommonMainH264DQId       [ui]    = 0;
		m_uiFrameNum   [ui]    = 0;
		m_uiSliceNum   [ui]    = 0;
		m_uiPosCounter [ui]    = 0;
		m_uiSymCounter [ui]    = 0;
		m_uiStorePosCnt[ui]    = 0;
		m_uiStoreSymCnt[ui]    = 0;
		m_acType       [ui][0] = '\0';
		m_acPos        [ui][0] = '\0';
		m_acCode       [ui][0] = '\0';
		m_acBits       [ui][0] = '\0';
		m_acLine       [ui][0] = '\0';
	}
	setLayer(0);
}

CommonMainH264::~CommonMainH264()
{
	for(unsigned ui = 0; ui < COMMON_MAX_TRACE_LAYERS; ui++)
	{
		if(m_pCommonMainH264DQId[ui])
		{
			delete m_pCommonMainH264DQId[ui];
			m_pCommonMainH264DQId[ui] = 0;
		}
	}
}

void CommonMainH264::setLayer(int iDQId)
{
  assert( (( iDQId >= -3 ) && ( iDQId+3 < COMMON_MAX_TRACE_LAYERS )) );
  m_uiDQIdplus3 = unsigned( iDQId + 3 );
  if( m_pCommonMainH264DQId[ m_uiDQIdplus3 ] == 0 )
  {
     CommonMainH264::CommonMainH264DQId::create( m_pCommonMainH264DQId[ m_uiDQIdplus3 ], "m_pucBaseName", m_uiDQIdplus3 );
    assert ( m_pCommonMainH264DQId[ m_uiDQIdplus3 ] );
  }
}

void CommonMainH264::startPicture()
{
   m_uiFrameNum[ m_uiDQIdplus3 ]++;
   m_uiSliceNum[ m_uiDQIdplus3 ]=0;
}

void CommonMainH264::startSlice()
{
  //Char acSliceHeader[256];
  //::snprintf( acSliceHeader, 255, "Slice # %u in Picture # %u", sm_uiSliceNum[ sm_uiDQIdplus3 ], sm_uiFrameNum[ sm_uiDQIdplus3 ] );
  m_uiSliceNum[m_uiDQIdplus3]++;
}

void CommonMainH264::startMb(unsigned uiMbAddress)
{
	// ::snprintf( acMbHeader, 127, "MB # %u", uiMbAddress );
   // RNOK ( printHeading( acMbHeader, false ) );
}

void CommonMainH264::printHeading(const char* pcString, bool bReset)
{
	char acMbHeader[MAX_LINE_LENGTH];
	::snprintf( acMbHeader, MAX_LINE_LENGTH-1, "-------------------- %s --------------------\n", pcString );
	if (m_pCommonMainH264DQId[m_uiDQIdplus3])
	{
		m_pCommonMainH264DQId[m_uiDQIdplus3]->output( acMbHeader );
	}
	if( bReset )
	{
		m_uiPosCounter[ m_uiDQIdplus3 ] = 0;
		m_uiSymCounter[ m_uiDQIdplus3 ] = 0;
	}
}

void CommonMainH264::countBits(unsigned uiBitCount)
{
	m_uiPosCounter[ m_uiDQIdplus3 ] += uiBitCount;
}

void CommonMainH264::printPos()
{
	::snprintf( m_acPos[m_uiDQIdplus3], 15, "@%d", m_uiPosCounter[ m_uiDQIdplus3 ] );
}

void CommonMainH264::addBits(unsigned uiVal, unsigned uiLength)
{
  if ( uiLength > 100 ) return;

  char acBuffer[101];
  unsigned uiOverLength = (unsigned)gMax(0, (signed)uiLength - 32);
  unsigned i;
  for ( i = 0; i < uiOverLength; i++ )
  {
    acBuffer[i] = '0';
  }
  for ( i = uiOverLength; i < uiLength; i++ )
  {
    acBuffer[i] = '0' + ( ( uiVal & ( 1 << ( uiLength - i - 1 ) ) ) >> ( uiLength - i - 1 ) );
  }
  acBuffer[uiLength] = '\0';

  i = (unsigned)strlen( m_acBits[m_uiDQIdplus3] );
  if( i < 2 )
  {
    m_acBits[m_uiDQIdplus3][0] = '[';
    m_acBits[m_uiDQIdplus3][1] = '\0';
  }
  else
  {
    m_acBits[m_uiDQIdplus3][i-1] = '\0';
  }
  m_acBits[m_uiDQIdplus3][ sizeof( m_acBits[m_uiDQIdplus3] ) - 1 ] = '\0';
  strncat( m_acBits[m_uiDQIdplus3], acBuffer, sizeof( m_acBits[m_uiDQIdplus3] ) - 1 - strlen( m_acBits[m_uiDQIdplus3] ) );
  strncat( m_acBits[m_uiDQIdplus3], "]",      sizeof( m_acBits[m_uiDQIdplus3] ) - 1 - strlen( m_acBits[m_uiDQIdplus3] ) );
}

void CommonMainH264::printCode(unsigned uiVal)
{
	 ::snprintf( m_acCode[m_uiDQIdplus3], 15, "%u", uiVal );
}

void CommonMainH264::printCode(signed iVal)
{
	::snprintf( m_acCode[m_uiDQIdplus3], 15, "%i", iVal );
}

void CommonMainH264::printString(const char* pcString)
{
	::strncat( m_acLine[m_uiDQIdplus3], pcString, MAX_LINE_LENGTH-1 );
}

void CommonMainH264::printType(const char* pcString)
{
	::snprintf( m_acType[m_uiDQIdplus3], 15, "%s", pcString );
}

void CommonMainH264::printVal(unsigned uiVal)
{
	char tmp[16];
  ::snprintf( tmp, 15, "%3u", uiVal);
  ::strncat ( m_acLine[m_uiDQIdplus3], tmp, MAX_LINE_LENGTH-1 );
}

void CommonMainH264::printVal(signed iVal)
{
	char tmp[16];
  ::snprintf( tmp, 15, "%3i", iVal );
  ::strncat ( m_acLine[m_uiDQIdplus3], tmp, MAX_LINE_LENGTH-1 );
}

void CommonMainH264::printXVal(unsigned uiVal)
{
	 char tmp[16];
  ::snprintf( tmp, 15, "0x%04x", uiVal );
  ::strncat ( m_acLine[m_uiDQIdplus3], tmp, MAX_LINE_LENGTH-1 );
}

void CommonMainH264::newLine()
{
  char acTmp[1024];
  const CommonElt* pcEltLatest = getEltLatest();

  if(pcEltLatest) {
	  switch(pcEltLatest->getType()) {
		case CommonEltType_Syntax:
			{
				pcEltLatest->setBitsStart(std::string(m_acPos[m_uiDQIdplus3]));
				pcEltLatest->setBitsVal(std::string(m_acBits[m_uiDQIdplus3]));
				break;
			}
	  }
  }

  ::snprintf( acTmp, 1023, "%-6s %-50s %-8s %5s %s\n",
              m_acPos[m_uiDQIdplus3], m_acLine[m_uiDQIdplus3], m_acType[m_uiDQIdplus3], m_acCode[m_uiDQIdplus3], m_acBits[m_uiDQIdplus3] );
  m_pCommonMainH264DQId[ m_uiDQIdplus3 ]->output( acTmp );
  m_acLine[m_uiDQIdplus3][0] ='\0';
  m_acType[m_uiDQIdplus3][0] ='\0';
  m_acCode[m_uiDQIdplus3][0] ='\0';
  m_acBits[m_uiDQIdplus3][0] ='\0';
  m_acPos [m_uiDQIdplus3][0] ='\0';
}

// @Override (CommonMain)
unsigned CommonMainH264::getCurrentPictureId()const
{
	return (m_uiDQIdplus3 >= sizeof(m_uiFrameNum)/sizeof(m_uiFrameNum[0])) ? 1 : m_uiFrameNum[m_uiDQIdplus3];
}

// @Override (CommonMain)
unsigned CommonMainH264::getCurrentSliceId()const
{
	return (m_uiDQIdplus3 >= sizeof(m_uiSliceNum)/sizeof(m_uiSliceNum[0])) ? 0 : m_uiSliceNum[m_uiDQIdplus3];
}

// @Override (CommonMain)
unsigned CommonMainH264::getCurrentLayerId()const
{
	return (m_uiDQIdplus3 == COMMON_MSYS_UINT_MAX) ? 0 : m_uiDQIdplus3 - 3;
}
