
#ifndef __H264AVCDECODERTEST_H_D65BE9B4_A8DA_11D3_AFE7_005004464B79
#define __H264AVCDECODERTEST_H_D65BE9B4_A8DA_11D3_AFE7_005004464B79


#include <algorithm>
#include <list>

#include "h264_decoder_parameter.h"
#include "ReadBitstreamFile.h"
#ifdef SHARP_AVC_REWRITE_OUTPUT
#include "WriteBitstreamToFile.h"
#else
#include "WriteYuvToFile.h"
#endif

#include "H264AVCDecoderLib.h"
#include "CreaterH264AVCDecoder.h"

class h264HdrEventParsing;

class H264AVCDecoderInst
{
private:
  class BufferParameters
  {
  public:
    BufferParameters();
    virtual ~BufferParameters();

    ErrVal      init( const h264::AccessUnit& rcAccessUnit );

    UInt        getLumaOffset ()  const { return m_uiLumaOffset; }
    UInt        getCbOffset   ()  const { return m_uiCbOffset; }
    UInt        getCrOffset   ()  const { return m_uiCrOffset; }
    UInt        getLumaHeight ()  const { return m_uiLumaHeight; }
    UInt        getLumaWidth  ()  const { return m_uiLumaWidth; }
    UInt        getLumaStride ()  const { return m_uiLumaStride; }
    UInt        getBufferSize ()  const { return m_uiBufferSize; }
    const UInt* getCropping   ()  const { return m_auiCropping; }

  private:
    UInt  m_uiLumaOffset;
    UInt  m_uiCbOffset;
    UInt  m_uiCrOffset;
    UInt  m_uiLumaHeight;
    UInt  m_uiLumaWidth;
    UInt  m_uiLumaStride;
    UInt  m_uiBufferSize;
    UInt  m_auiCropping[4];
  };

protected:
  H264AVCDecoderInst();
  virtual ~H264AVCDecoderInst();

public:
  static ErrVal create  ( H264AVCDecoderInst*&  rpcH264AVCDecoderInst );
  ErrVal        destroy ();
  ErrVal        init    ( H264DecoderParameter*     pcDecoderParameter,
                          ReadBitstreamIf*      pcReadBitStream,
#ifdef SHARP_AVC_REWRITE_OUTPUT
                          WriteBitstreamIf*     pcWriteBitstream );
#else
                          WriteYuvIf*           pcWriteYuv );
#endif
  ErrVal        uninit  ();
  ErrVal        go      (UInt32 frameCount = _UI32_MAX);

  ErrVal		setIoEventHdr ( const h264HdrEventParsing* pIoEventHdr );
  const h264HdrEventParsing*		getIoEventHdr (  );

private:
  ErrVal  xProcessAccessUnit( h264::AccessUnit& rcAccessUnit,
                              Bool&             rbFirstAccessUnit,
                              UInt&             ruiNumProcessed );
  ErrVal  xGetNewPicBuffer  ( PicBuffer*&       rpcPicBuffer,
                              UInt              uiSize );
  ErrVal  xOutputNALUnits   ( BinDataList&      rcBinDataList,
                              UInt&             ruiNumNALUnits );
  ErrVal  xOutputPicBuffer  ( PicBufferList&    rcPicBufferOutputList,
                              UInt&             ruiNumFrames );
  ErrVal  xRemovePicBuffer  ( PicBufferList&    rcPicBufferUnusedList );

  ErrVal xDrawFrame	( const UChar*  pLum,
                            const UChar*  pCb,
                            const UChar*  pCr,
                            UInt          uiLumHeight,
                            UInt          uiLumWidth,
                            UInt          uiLumStride,
                            const UInt    rauiCropping[] );

private:
  Bool                          m_bInitialized;
  UInt							m_uiNumProcessed;
  Bool							m_bFirstAccessUnit;
  h264::CreaterH264AVCDecoder*  m_pcH264AVCDecoder;
  H264DecoderParameter*             m_pcParameter;
  ReadBitstreamIf*              m_pcReadBitstream;
#ifdef SHARP_AVC_REWRITE_OUTPUT
  WriteBitstreamIf*             m_pcWriteBitstream;
#else
  WriteYuvIf*                   m_pcWriteYuv;
#endif

  PicBufferList                 m_cActivePicBufferList;
  PicBufferList                 m_cUnusedPicBufferList;
  BufferParameters              m_cBufferParameters;
#ifdef SHARP_AVC_REWRITE_OUTPUT
  UChar                         m_aucStartCodeBuffer[5];
  BinData                       m_cBinDataStartCode;
#else
  Int                           m_iMaxPocDiff;
  Int                           m_iLastPoc;
  UChar*                        m_pcLastFrame;
#endif

  const h264HdrEventParsing* m_pIoEventHdr;
};

#endif //__H264AVCDECODERTEST_H_D65BE9B4_A8DA_11D3_AFE7_005004464B79
