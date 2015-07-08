#include "h264_decoder_inst.h"
#include "H264AVCCommonLib/IoEvent.h"
#include "common/cpp/common_main_h264.h" /* <Thialgou> */
#include "common/cpp/common_h264_defs_internal.h" /* <Thialgou> */

H264AVCDecoderInst::BufferParameters::BufferParameters()
: m_uiLumaOffset  ( 0 )
, m_uiCbOffset    ( 0 )
, m_uiCrOffset    ( 0 )
, m_uiLumaHeight  ( 0 )
, m_uiLumaWidth   ( 0 )
, m_uiLumaStride  ( 0 )
, m_uiBufferSize  ( 0 )
{
  ::memset( m_auiCropping, 0x00, sizeof( m_auiCropping ) );
}

H264AVCDecoderInst::BufferParameters::~BufferParameters()
{
}

ErrVal
H264AVCDecoderInst::BufferParameters::init( const h264::AccessUnit& rcAccessUnit )
{
  const h264::SliceDataNALUnit* pcSliceDataNalUnit = rcAccessUnit.getLastVCLNalUnit();
  ROF( pcSliceDataNalUnit );
  UInt uiMbX        = pcSliceDataNalUnit->getFrameWidthInMb ();
  UInt uiMbY        = pcSliceDataNalUnit->getFrameHeightInMb();
  UInt uiAllocMbX   = pcSliceDataNalUnit->getAllocFrameWidthInMbs ();
  UInt uiAllocMbY   = pcSliceDataNalUnit->getAllocFrameHeightInMbs();
  UInt uiChromaSize = ( ( uiAllocMbX << 3 ) + YUV_X_MARGIN     ) * ( ( uiAllocMbY << 3 ) + YUV_Y_MARGIN );
  m_uiLumaOffset    = ( ( uiAllocMbX << 4 ) + YUV_X_MARGIN * 2 ) * YUV_Y_MARGIN     + YUV_X_MARGIN;
  m_uiCbOffset      = ( ( uiAllocMbX << 3 ) + YUV_X_MARGIN     ) * YUV_Y_MARGIN / 2 + YUV_X_MARGIN / 2 + uiChromaSize * 4;
  m_uiCrOffset      = ( ( uiAllocMbX << 3 ) + YUV_X_MARGIN     ) * YUV_Y_MARGIN / 2 + YUV_X_MARGIN / 2 + uiChromaSize * 5;
  m_uiLumaHeight    =   ( uiMbY      << 4 );
  m_uiLumaWidth     =   ( uiMbX      << 4 );
  m_uiLumaStride    =   ( uiAllocMbX << 4 ) + YUV_X_MARGIN * 2;
  m_uiBufferSize    = uiChromaSize * 6;
  m_auiCropping[0]  = pcSliceDataNalUnit->getCroppingRectangle()[0];
  m_auiCropping[1]  = pcSliceDataNalUnit->getCroppingRectangle()[1];
  m_auiCropping[2]  = pcSliceDataNalUnit->getCroppingRectangle()[2];
  m_auiCropping[3]  = pcSliceDataNalUnit->getCroppingRectangle()[3];
  return Err::m_nOK;
}





H264AVCDecoderInst::H264AVCDecoderInst()
: m_bInitialized          ( false )
, m_uiNumProcessed		  ( 0 )
, m_bFirstAccessUnit      ( true )
, m_pcH264AVCDecoder      ( 0 )
, m_pcParameter           ( 0 )
, m_pcReadBitstream       ( 0 )
#ifdef SHARP_AVC_REWRITE_OUTPUT
, m_pcWriteBitstream      ( 0 )
#else
, m_pcWriteYuv            ( 0 )
, m_iMaxPocDiff           ( MSYS_INT_MAX )
, m_iLastPoc              ( MSYS_UINT_MAX )
, m_pcLastFrame           ( 0 )
#endif
, m_pIoEventHdr			  ( NULL )
{
#ifdef SHARP_AVC_REWRITE_OUTPUT
  m_aucStartCodeBuffer[ 0 ] = 0;
  m_aucStartCodeBuffer[ 1 ] = 0;
  m_aucStartCodeBuffer[ 2 ] = 0;
  m_aucStartCodeBuffer[ 3 ] = 1;
  m_cBinDataStartCode.reset ();
  m_cBinDataStartCode.set   ( m_aucStartCodeBuffer, 4 );
#endif
}

H264AVCDecoderInst::~H264AVCDecoderInst()
{
}

ErrVal
H264AVCDecoderInst::create( H264AVCDecoderInst*& rpcH264AVCDecoderInst )
{
  rpcH264AVCDecoderInst = new H264AVCDecoderInst;
  ROT( NULL == rpcH264AVCDecoderInst );
  return Err::m_nOK;
}

ErrVal
H264AVCDecoderInst::destroy()
{
  ROT( m_bInitialized );
#ifdef SHARP_AVC_REWRITE_OUTPUT
  m_cBinDataStartCode.reset();
#endif
  delete this;
  return Err::m_nOK;
}

ErrVal
H264AVCDecoderInst::init( H264DecoderParameter* pcDecoderParameter,
                          ReadBitstreamIf*  pcReadBitstream,
#ifdef SHARP_AVC_REWRITE_OUTPUT
                          WriteBitstreamIf* pcWriteBitStream )
#else
                          WriteYuvIf*       pcWriteYuv )
#endif
{
  ROT( m_bInitialized );
  ROF( pcDecoderParameter );
  ROF( pcReadBitstream );
#ifdef SHARP_AVC_REWRITE_OUTPUT
  ROF( pcWriteBitStream );
#else
  ROF( pcWriteYuv );
#endif

  h264::CreaterH264AVCDecoder* pcH264AVCDecoder = 0;
  RNOK( h264::CreaterH264AVCDecoder::create( pcH264AVCDecoder ) );

  m_bInitialized          = true;
  m_pcH264AVCDecoder      = pcH264AVCDecoder;
  m_pcParameter           = pcDecoderParameter;
  m_pcReadBitstream       = pcReadBitstream;
#ifdef SHARP_AVC_REWRITE_OUTPUT
  m_pcWriteBitstream      = pcWriteBitStream;
#else
  m_pcWriteYuv            = pcWriteYuv;
  m_iMaxPocDiff           = (Int)m_pcParameter->uiMaxPocDiff;
  m_iLastPoc              = MSYS_UINT_MAX;
  m_pcLastFrame           = 0;
#endif

  m_pcParameter->nResult  = -1;

  RNOK( m_pcH264AVCDecoder->init( true ) );

  return Err::m_nOK;
}

ErrVal
H264AVCDecoderInst::uninit()
{
  ROF( m_bInitialized );
  ROF( m_cActivePicBufferList.empty() );

  if( m_pcH264AVCDecoder )
  {
    RNOK( m_pcH264AVCDecoder->uninit  ( true ) );
    RNOK( m_pcH264AVCDecoder->destroy () );
  }

  //===== delete picture buffer =====
  PicBufferList::iterator iter;
  for( iter = m_cUnusedPicBufferList.begin(); iter != m_cUnusedPicBufferList.end(); iter++ )
  {
    delete [] (*iter)->getBuffer();
    delete    (*iter);
  }
  for( iter = m_cActivePicBufferList.begin(); iter != m_cActivePicBufferList.end(); iter++ )
  {
    delete [] (*iter)->getBuffer();
    delete    (*iter);
  }

#ifdef SHARP_AVC_REWRITE_OUTPUT
#else
  delete [] m_pcLastFrame;
#endif

  m_bInitialized = false;
  return Err::m_nOK;
}

ErrVal
H264AVCDecoderInst::go(UInt32 frameCount /*= _UI32_MAX*/)
{
  h264::AccessUnit  cAccessUnit;
  while( ! cAccessUnit.isEndOfStream() && (m_uiNumProcessed <  frameCount))
  {	
    RNOK( xProcessAccessUnit( cAccessUnit, m_bFirstAccessUnit, m_uiNumProcessed ) );
  }
#ifdef SHARP_AVC_REWRITE_OUTPUT
  printf("\n%d NAL units rewritten\n\n",  m_uiNumProcessed );
#else
  printf("\n%d frames decoded\n\n",       m_uiNumProcessed );
#endif
  m_pcParameter->nFrames = m_uiNumProcessed;
  m_pcParameter->nResult = 0;

  return Err::m_nOK;
}

ErrVal
H264AVCDecoderInst::xProcessAccessUnit( h264::AccessUnit& rcAccessUnit, Bool& rbFirstAccessUnit, UInt& ruiNumProcessed )
{
  ROT( rcAccessUnit.isEndOfStream() );
  ROT( rcAccessUnit.isComplete   () );

  const CommonMainH264* pcMainH264 = CommonMain::getMainH264();
  CommonH264Section* pThialgouSection = new CommonH264Section();
  ASSERT(pcMainH264 != NULL);
  ASSERT(pThialgouSection != NULL);

  // Set current section
  pcMainH264->setSectionCurrent(pThialgouSection);

  //===== read next access unit ====
  while( !rcAccessUnit.isComplete() )
  {
    BinData*  pcBinData     = 0;
    Bool      bEndOfStream  = false; // dummy
	CommonH264Nalu fakeNALU(NAL_UNIT_UNSPECIFIED_0);
	pcMainH264->setEltCurrent(&fakeNALU); // fake NALU used for Backward compatibility
    RNOK( m_pcReadBitstream ->extractPacket ( pcBinData, bEndOfStream ) );
    RNOK( m_pcH264AVCDecoder->initNALUnit   ( pcBinData, rcAccessUnit ) );
    RNOK( m_pcReadBitstream ->releasePacket ( pcBinData ) );
	pcMainH264->setEltCurrent(NULL); // reset() current NALU.
  }

  //===== get buffer dimensions =====
  if( rbFirstAccessUnit )
  {
    RNOK( m_cBufferParameters.init( rcAccessUnit ) );
    rbFirstAccessUnit = false;
#ifdef SHARP_AVC_REWRITE_OUTPUT
#else
    m_pcLastFrame     = new UChar [ m_cBufferParameters.getBufferSize() ];
    ROF ( m_pcLastFrame );
#endif
  }

  printf( "---------- new ACCESS UNIT ----------\n" );

  //===== decode access unit =====
  while( rcAccessUnit.isComplete() )
  {
    h264::NALUnit*  pcNalUnit   = 0;
    PicBuffer*      pcPicBuffer = 0;
    PicBufferList   cPicBufferOutputList;
    PicBufferList   cPicBufferUnusedList;
    BinDataList     cBinDataList;
	
	CommonH264Nalu* pThialgouNalu = new CommonH264Nalu(NAL_UNIT_UNSPECIFIED_0); // Create NALU.
	ASSERT(pThialgouNalu != NULL);

	

	pcMainH264->setEltCurrent(pThialgouNalu);
    RNOK( rcAccessUnit.getAndRemoveNextNalUnit( pcNalUnit ) );
    RNOK( xGetNewPicBuffer( pcPicBuffer, m_cBufferParameters.getBufferSize() ) );
    RNOK( m_pcH264AVCDecoder->processNALUnit( pcPicBuffer, cPicBufferOutputList, cPicBufferUnusedList, cBinDataList, *pcNalUnit ) );
    RNOK( xOutputNALUnits ( cBinDataList,         ruiNumProcessed ) );
    RNOK( xOutputPicBuffer( cPicBufferOutputList, ruiNumProcessed ) );
    RNOK( xRemovePicBuffer( cPicBufferUnusedList ) );
	
	pThialgouNalu->setPictureId(pcMainH264->getCurrentPictureId());
	pThialgouNalu->setSliceId(pcMainH264->getCurrentSliceId());
	pThialgouNalu->setLayerId(pcMainH264->getCurrentLayerId());

	if (pcNalUnit->isVCLNALUnit() && ((h264::SliceDataNALUnit*)pcNalUnit)->isLastSliceInLayerRepresentation() /*&& ((h264::SliceDataNALUnit*)pcNalUnit)->isLastSliceInDependencyRepresentation()*/)
	{
		const_cast<CommonMainH264*>(CommonMain::getMainH264())->startPicture();
	}

	pcMainH264->setEltCurrent(NULL); // reset() current NALU.
	pThialgouSection->addElt((CommonElt**)(&pThialgouNalu)); // add current NALU to the section
	if (pThialgouNalu) { // only delete the NALU if not done by "addElt" function
		delete pThialgouNalu, pThialgouNalu = NULL;
	}	
	
	delete pcNalUnit;
  }

  pcMainH264->OnElt(pThialgouSection); // deliver() current section to the UI.
  delete pThialgouSection, pThialgouSection = NULL; // delete the section

  return Err::m_nOK;
}

ErrVal
H264AVCDecoderInst::xGetNewPicBuffer( PicBuffer*& rpcPicBuffer, UInt uiSize )
{
  if( m_cUnusedPicBufferList.empty() )
  {
    rpcPicBuffer = new PicBuffer( new UChar[ uiSize ] );
  }
  else
  {
    rpcPicBuffer = m_cUnusedPicBufferList.popFront();
  }
  m_cActivePicBufferList.push_back( rpcPicBuffer );
  return Err::m_nOK;
}

ErrVal
H264AVCDecoderInst::xOutputNALUnits( BinDataList& rcBinDataList, UInt& ruiNumNALUnits )
{
#ifdef SHARP_AVC_REWRITE_OUTPUT
  while( ! rcBinDataList.empty() )
  {
    BinData* pcBinData = rcBinDataList.popFront();
    ROF( pcBinData );
    RNOK( m_pcWriteBitstream->writePacket( &m_cBinDataStartCode ) );
    RNOK( m_pcWriteBitstream->writePacket( pcBinData ) );
    ruiNumNALUnits++;
    pcBinData->deleteData();
    delete pcBinData;
  }
#else
  ROF( rcBinDataList.empty() );
#endif
  return Err::m_nOK;
}

ErrVal
H264AVCDecoderInst::xOutputPicBuffer( PicBufferList& rcPicBufferOutputList, UInt& ruiNumFrames )
{
#ifdef SHARP_AVC_REWRITE_OUTPUT
  rcPicBufferOutputList.clear();
#else
  while( ! rcPicBufferOutputList.empty() )
  {
    PicBuffer* pcPicBuffer = rcPicBufferOutputList.popFront();
    ROF( pcPicBuffer );

    while( abs( m_iLastPoc + m_iMaxPocDiff ) < abs( (Int)pcPicBuffer->getCts() ) )
    {
      ROF ( m_pcLastFrame );
#if 0
	  xDrawFrame	( m_pcLastFrame + m_cBufferParameters.getLumaOffset (),
                                      m_pcLastFrame + m_cBufferParameters.getCbOffset   (),
                                      m_pcLastFrame + m_cBufferParameters.getCrOffset (),
                                      m_cBufferParameters.getLumaHeight (),
                                      m_cBufferParameters.getLumaWidth  (),
                                      m_cBufferParameters.getLumaStride (),
                                      m_cBufferParameters.getCropping   () );
      RNOK( m_pcWriteYuv->writeFrame( m_pcLastFrame + m_cBufferParameters.getLumaOffset (),
                                      m_pcLastFrame + m_cBufferParameters.getCbOffset   (),
                                      m_pcLastFrame + m_cBufferParameters.getCrOffset (),
                                      m_cBufferParameters.getLumaHeight (),
                                      m_cBufferParameters.getLumaWidth  (),
                                      m_cBufferParameters.getLumaStride (),
                                      m_cBufferParameters.getCropping   () ) );
#endif
      printf("REPEAT FRAME\n");
      ruiNumFrames++;
      m_iLastPoc  += m_iMaxPocDiff;
    }

    m_pcLastFrame = pcPicBuffer->switchBuffer( m_pcLastFrame );
    ROF ( m_pcLastFrame );
#if 0
	xDrawFrame ( m_pcLastFrame + m_cBufferParameters.getLumaOffset (),
                                      m_pcLastFrame + m_cBufferParameters.getCbOffset   (),
                                      m_pcLastFrame + m_cBufferParameters.getCrOffset (),
                                      m_cBufferParameters.getLumaHeight (),
                                      m_cBufferParameters.getLumaWidth  (),
                                      m_cBufferParameters.getLumaStride (),
                                      m_cBufferParameters.getCropping   () );
    RNOK( m_pcWriteYuv->writeFrame( m_pcLastFrame + m_cBufferParameters.getLumaOffset (),
                                    m_pcLastFrame + m_cBufferParameters.getCbOffset   (),
                                    m_pcLastFrame + m_cBufferParameters.getCrOffset (),
                                    m_cBufferParameters.getLumaHeight (),
                                    m_cBufferParameters.getLumaWidth  (),
                                    m_cBufferParameters.getLumaStride (),
                                    m_cBufferParameters.getCropping   () ) );
#endif
    ruiNumFrames++;
    m_iLastPoc  += (Int)pcPicBuffer->getCts();
  }
#endif
  return Err::m_nOK;
}

ErrVal
H264AVCDecoderInst::xRemovePicBuffer( PicBufferList& rcPicBufferUnusedList )
{
  while( ! rcPicBufferUnusedList.empty() )
  {
    PicBuffer* pcBuffer = rcPicBufferUnusedList.popFront();
    if( pcBuffer )
    {
      PicBufferList::iterator  begin = m_cActivePicBufferList.begin();
      PicBufferList::iterator  end   = m_cActivePicBufferList.end  ();
      PicBufferList::iterator  iter  = std::find( begin, end, pcBuffer );

      ROT( iter == end ); // there is something wrong if the address is not in the active list
      ROT( pcBuffer->isUsed() );

      m_cUnusedPicBufferList.push_back( pcBuffer );
      m_cActivePicBufferList.erase    (  iter );
    }
  }
  return Err::m_nOK;
}

ErrVal 
H264AVCDecoderInst::xDrawFrame	( const UChar*  pLum,
                            const UChar*  pCb,
                            const UChar*  pCr,
                            UInt          uiLumHeight,
                            UInt          uiLumWidth,
                            UInt          uiLumStride,
                            const UInt    rauiCropping[] )
{
  uiLumHeight   -= rauiCropping[0] + rauiCropping[1];
  uiLumWidth  -= rauiCropping[2] + rauiCropping[3];

  (this);

  if(m_pIoEventHdr){
	  const UChar* _pLum = pLum + ( rauiCropping[0] + rauiCropping[2] * uiLumStride );
	  const UChar* _pCb = pCb + ( ( rauiCropping[0] + rauiCropping[2] * ((uiLumStride + 1) >> 1) ) >> 1 );
	  const UChar* _pCr = pCr + ( ( rauiCropping[0] + rauiCropping[2] * ((uiLumStride + 1) >> 1) ) >> 1 );
	  m_pIoEventHdr->OnFrameDraw( _pLum,
				_pCb,
				_pCr,
				uiLumWidth,
				uiLumHeight,
				uiLumStride);
  }

  return Err::m_nOK;
}


ErrVal
H264AVCDecoderInst::setIoEventHdr ( const h264HdrEventParsing* pIoEventHdr )
{
	m_pIoEventHdr = pIoEventHdr;
	if(m_pcH264AVCDecoder){
		return m_pcH264AVCDecoder->SetIoEventHdr( pIoEventHdr );
	}
	return Err::m_nOK;
}

const h264HdrEventParsing*
H264AVCDecoderInst::getIoEventHdr (  )
{
	return m_pIoEventHdr;
}