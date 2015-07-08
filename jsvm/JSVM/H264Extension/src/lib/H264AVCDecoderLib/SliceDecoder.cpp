
#include "H264AVCDecoderLib.h"
#include "MbDecoder.h"
#include "SliceDecoder.h"
#include "H264AVCCommonLib/SliceHeader.h"
#include "DecError.h"

#include "H264AVCCommonLib/MbDataCtrl.h"
#include "H264AVCCommonLib/Frame.h"

#include "H264AVCCommonLib/CFMO.h"

#include "common/cpp/common_main_h264.h" /* <Thialgou> */

H264AVC_NAMESPACE_BEGIN


SliceDecoder::SliceDecoder()
: m_pcMbDecoder       ( 0 )
, m_pcControlMng      ( 0 )
, m_bInitDone         ( false )
{
}

SliceDecoder::~SliceDecoder()
{
}

ErrVal
SliceDecoder::create( SliceDecoder*& rpcSliceDecoder )
{
  rpcSliceDecoder = new SliceDecoder;
  ROT( NULL == rpcSliceDecoder );
  return Err::m_nOK;
}

ErrVal
SliceDecoder::destroy()
{
  ROT( m_bInitDone );
  delete this;
  return Err::m_nOK;
}

ErrVal
SliceDecoder::init( MbDecoder*        pcMbDecoder,
                    ControlMngIf*     pcControlMng )
{
  ROT( m_bInitDone );
  ROF( pcMbDecoder );
  ROF( pcControlMng );

  m_pcMbDecoder       = pcMbDecoder;
  m_pcControlMng      = pcControlMng;
  m_bInitDone         = true;
  return Err::m_nOK;
}


ErrVal
SliceDecoder::uninit()
{
  ROF( m_bInitDone );

  m_pcMbDecoder       = 0;
  m_pcControlMng      = 0;
  m_bInitDone         = false;
  return Err::m_nOK;
}


ErrVal
SliceDecoder::decode( SliceHeader&  rcSH,
                      MbDataCtrl*   pcMbDataCtrl,
                      MbDataCtrl*   pcMbDataCtrlBase,
                      Frame*        pcFrame,
                      Frame*        pcResidualLF,
                      Frame*        pcResidualILPred,
                      Frame*        pcBaseLayer,
                      Frame*        pcBaseLayerResidual,
                      RefFrameList* pcRefFrameList0,
                      RefFrameList* pcRefFrameList1,
                      MbDataCtrl*   pcMbDataCtrl0L1,
                      Bool          bReconstructAll )
{
  ROF( m_bInitDone );

  const CommonMainH264* pcMainH264 = CommonMain::getMainH264();
  ASSERT(pcMainH264 != NULL);
  const CommonElt* pcCurrent = pcMainH264->getEltCurrent();
  ASSERT(pcCurrent != NULL && pcCurrent->getType() == CommonEltType_Nalu);
  const CommonMbH264* pcThialgouMb;

  //====== initialization ======
  RNOK( pcMbDataCtrl->initSlice( rcSH, DECODE_PROCESS, true, pcMbDataCtrl0L1 ) );

  const PicType ePicType = rcSH.getPicType();
	if( ePicType != FRAME )
	{
		if( pcFrame )             RNOK( pcFrame->            addFieldBuffer( ePicType ) );
    if( pcResidualLF )        RNOK( pcResidualLF->       addFieldBuffer( ePicType ) );
    if( pcResidualILPred )    RNOK( pcResidualILPred->   addFieldBuffer( ePicType ) );
		if( pcBaseLayer )         RNOK( pcBaseLayer->        addFieldBuffer( ePicType ) );
		if( pcBaseLayerResidual ) RNOK( pcBaseLayerResidual->addFieldBuffer( ePicType ) );
	}

  //===== loop over macroblocks =====
  UInt uiMbAddress     = rcSH.getFirstMbInSlice();
  UInt uiNumMbsInSlice = rcSH.getNumMbsInSlice();
  for( UInt uiNumMbsDecoded = 0; uiNumMbsDecoded < uiNumMbsInSlice; uiNumMbsDecoded++ )
  {
    MbDataAccess* pcMbDataAccess     = NULL;
    MbDataAccess* pcMbDataAccessBase = NULL;
    UInt          uiMbY, uiMbX;

	// FIXME:
  if (/*pcMainH264->getCurrentLayerId() == 16 && pcMainH264->getCurrentPictureId() == (109-1) &&*/ uiNumMbsDecoded == 77)
  {
	int	a = 0;
  }


    rcSH.getMbPositionFromAddress( uiMbY, uiMbX, uiMbAddress                            );

	pcThialgouMb = dynamic_cast<const CommonMbH264*>(pcCurrent->addMb(uiMbAddress, uiMbX, uiMbY));
	pcMainH264->setEltCurrent(pcThialgouMb);

    RNOK( pcMbDataCtrl  ->initMb            (  pcMbDataAccess,     uiMbY, uiMbX ) );
    if( pcMbDataCtrlBase )
    {
      RNOK( pcMbDataCtrlBase->initMb        (  pcMbDataAccessBase, uiMbY, uiMbX ) );
    }
    RNOK( m_pcControlMng->initMbForDecoding ( *pcMbDataAccess,     uiMbY, uiMbX, false ) );

    if( pcMbDataAccess->getMbData().getBLSkipFlag() && ! m_apabBaseModeFlagAllowedArrays[ ePicType == FRAME ? 0 : 1 ][ uiMbAddress ] )
    {
      printf( "CIU constraint violated (MbAddr=%d) ==> ignore\n", uiMbAddress );
    }

    RNOK( m_pcMbDecoder ->decode            ( *pcMbDataAccess,
                                              pcMbDataAccessBase,
                                              pcFrame                                  ->getPic( ePicType ),
                                              pcResidualLF                             ->getPic( ePicType ),
                                              pcResidualILPred                         ->getPic( ePicType ),
                                              pcBaseLayer         ? pcBaseLayer        ->getPic( ePicType ) : NULL,
                                              pcBaseLayerResidual ? pcBaseLayerResidual->getPic( ePicType ) : NULL,
                                              pcRefFrameList0,
                                              pcRefFrameList1,
                                              bReconstructAll ) );
	
	MbData& mbData = pcMbDataAccess->getMbData();
	pcThialgouMb->setMode(mbData.getMbMode());
	pcThialgouMb->setMbType(mbData.getMbType());
	pcThialgouMb->setSliceType(mbData.getSliceType());
	pcThialgouMb->setBitsCount(mbData.getBitsCount());
	pcThialgouMb->setQP((int32_t)mbData.getQp());
	pcThialgouMb->setCBP((int32_t)mbData.getMbCbp());
	pcThialgouMb->setInCropWindow(mbData.getInCropWindowFlag()); // SVC
	pcThialgouMb->setResidualPredictionFlag(mbData.getResidualPredFlag()); // SVC
	pcThialgouMb->setBLSkippedFlag(mbData.getBLSkipFlag()); // SVC
	//pcThialgouMb->setsetQPC(mbData.getQp4LF
	if (mbData.getMbMode() == INTRA_4X4)
	{
		for( B4x4Idx cIdx; cIdx.isLegal(); cIdx++ )
		{
			pcThialgouMb->setIntra4x4PredMode(cIdx, mbData.intraPredMode(cIdx));
		}
	}
	if( mbData.isInter8x8() )
	{
		for( int i = 0; i < 4; i++ )
        {
			pcThialgouMb->setBlkModes(i, mbData.getBlkMode((Par8x8)i));
		}
	}
	if (mbData.isIntra())
	{
		pcThialgouMb->setIntraChromaPredMode(mbData.getChromaPredMode());
	}
	else
	{	
		MbMvData& mvdData = mbData.getMbMvdData(LIST_0);
		MbMotionData& mvData = mbData.getMbMotionData(LIST_0);
		for( B4x4Idx cIdx; cIdx < 16; cIdx++ )
        {
			pcThialgouMb->setMvdL0(cIdx, mvdData.getMv(cIdx).getHor(), mvdData.getMv(cIdx).getVer());
			pcThialgouMb->setMvL0(cIdx, mvData.getMv(cIdx).getHor(), mvData.getMv(cIdx).getVer());
		}
		for( int i = 0; i < 4; i++ )
        {
			pcThialgouMb->setRefIdxL0(i, mvData.getRefIdx((Par8x8)i) - 1);
			pcThialgouMb->setPOC0(i, mvData.getRefPicIdc((Par8x8)i).getPoc());
		}
		if(mbData.getSliceType() == B_SLICE)
		{
			mvdData = mbData.getMbMvdData(LIST_1);
			mvData = mbData.getMbMotionData(LIST_1);
			for( B4x4Idx cIdx; cIdx < 16; cIdx++ )
			{
				pcThialgouMb->setMvdL1(cIdx, mvdData.getMv(cIdx).getHor(), mvdData.getMv(cIdx).getVer());
				pcThialgouMb->setMvL1(cIdx, mvData.getMv(cIdx).getHor(), mvData.getMv(cIdx).getVer());
			}
			for( int i = 0; i < 4; i++ )
			{
				pcThialgouMb->setRefIdxL1(i, mvData.getRefIdx((Par8x8)i) - 1);
				pcThialgouMb->setPOC1(i, mvData.getRefPicIdc((Par8x8)i).getPoc());
			}
		}

		
	}


    uiMbAddress=rcSH.getFMO()->getNextMBNr(uiMbAddress);
  }
  pcMainH264->setEltCurrent(pcCurrent);

  if( ePicType!=FRAME )
	{
		if( pcFrame )             RNOK( pcFrame->            removeFieldBuffer( ePicType ) );
    if( pcResidualLF )        RNOK( pcResidualLF->       removeFieldBuffer( ePicType ) );
    if( pcResidualILPred )    RNOK( pcResidualILPred->   removeFieldBuffer( ePicType ) );
		if( pcBaseLayer )         RNOK( pcBaseLayer->        removeFieldBuffer( ePicType ) );
		if( pcBaseLayerResidual ) RNOK( pcBaseLayerResidual->removeFieldBuffer( ePicType ) );
	}

  return Err::m_nOK;
}


ErrVal
SliceDecoder::decodeMbAff( SliceHeader&   rcSH,
                           MbDataCtrl*    pcMbDataCtrl,
                           MbDataCtrl*    pcMbDataCtrlBase,
                           MbDataCtrl*    pcMbDataCtrlBaseField,
                           Frame*         pcFrame,
                           Frame*         pcResidualLF,
                           Frame*         pcResidualILPred,
                           Frame*         pcBaseLayer,
                           Frame*         pcBaseLayerResidual,
                           RefFrameList*  pcRefFrameList0,
                           RefFrameList*  pcRefFrameList1,
                           MbDataCtrl*    pcMbDataCtrl0L1,
                           Bool           bReconstructAll )
{
  ROF( m_bInitDone );

  //====== initialization ======
  RNOK( pcMbDataCtrl->initSlice( rcSH, DECODE_PROCESS, true, pcMbDataCtrl0L1 ) );

  RefFrameList acRefFrameList0[2];
  RefFrameList acRefFrameList1[2];

  RNOK( gSetFrameFieldLists( acRefFrameList0[0], acRefFrameList0[1], *pcRefFrameList0 ) );
  RNOK( gSetFrameFieldLists( acRefFrameList1[0], acRefFrameList1[1], *pcRefFrameList1 ) );

  rcSH.setRefFrameList( &(acRefFrameList0[0]), TOP_FIELD, LIST_0 );
  rcSH.setRefFrameList( &(acRefFrameList0[1]), BOT_FIELD, LIST_0 );
  rcSH.setRefFrameList( &(acRefFrameList1[0]), TOP_FIELD, LIST_1 );
  rcSH.setRefFrameList( &(acRefFrameList1[1]), BOT_FIELD, LIST_1 );

  RefFrameList* apcRefFrameList0[2];
  RefFrameList* apcRefFrameList1[2];

  apcRefFrameList0[0] = ( NULL == pcRefFrameList0 ) ? NULL : &acRefFrameList0[0];
  apcRefFrameList0[1] = ( NULL == pcRefFrameList0 ) ? NULL : &acRefFrameList0[1];
  apcRefFrameList1[0] = ( NULL == pcRefFrameList1 ) ? NULL : &acRefFrameList1[0];
  apcRefFrameList1[1] = ( NULL == pcRefFrameList1 ) ? NULL : &acRefFrameList1[1];

  Frame* apcFrame            [4] = { NULL, NULL, NULL, NULL };
  Frame* apcResidualLF       [4] = { NULL, NULL, NULL, NULL };
  Frame* apcResidualILPred   [4] = { NULL, NULL, NULL, NULL };
  Frame* apcBaseLayer        [4] = { NULL, NULL, NULL, NULL };
  Frame* apcBaseLayerResidual[4] = { NULL, NULL, NULL, NULL };

	RNOK( gSetFrameFieldArrays( apcFrame,             pcFrame             ) );
  RNOK( gSetFrameFieldArrays( apcResidualLF,        pcResidualLF        ) );
  RNOK( gSetFrameFieldArrays( apcResidualILPred,    pcResidualILPred    ) );
  RNOK( gSetFrameFieldArrays( apcBaseLayer,         pcBaseLayer         ) );
  RNOK( gSetFrameFieldArrays( apcBaseLayerResidual, pcBaseLayerResidual ) );

  //===== loop over macroblocks =====
  Bool bSNR             = rcSH.getTCoeffLevelPredictionFlag() || rcSH.getSCoeffResidualPredFlag();
  UInt uiMbAddress      = rcSH.getFirstMbInSlice();
  UInt uiLastMbAddress  = rcSH.getFirstMbInSlice() + rcSH.getNumMbsInSlice() - 1;
  for( ; uiMbAddress <= uiLastMbAddress; uiMbAddress+=2 )
  {
    for( UInt eP = 0; eP < 2; eP++ )
    {
      UInt uiMbAddressMbAff = uiMbAddress + eP;
      MbDataAccess* pcMbDataAccess     = NULL;
      MbDataAccess* pcMbDataAccessBase = NULL;
      UInt          uiMbY, uiMbX;

      RefFrameList* pcRefFrameList0F;
      RefFrameList* pcRefFrameList1F;

      rcSH.getMbPositionFromAddress( uiMbY, uiMbX, uiMbAddressMbAff                     );

      RNOK( pcMbDataCtrl->      initMb       (  pcMbDataAccess,    uiMbY, uiMbX       ) );
      pcMbDataAccess->setFieldMode( pcMbDataAccess->getMbData().getFieldFlag()          );

      if( ( pcMbDataAccess->getMbPicType()==FRAME || bSNR ) && pcMbDataCtrlBase )
      {
        RNOK( pcMbDataCtrlBase->initMb        ( pcMbDataAccessBase, uiMbY, uiMbX  ) );
      }
      else if( pcMbDataAccess->getMbPicType()<FRAME && !bSNR && pcMbDataCtrlBaseField )
      {
        RNOK( pcMbDataCtrlBaseField->initMb   ( pcMbDataAccessBase, uiMbY, uiMbX  ) );
      }

      if( bSNR && rcSH.getSliceSkipFlag() )
      {
        pcMbDataAccess->setFieldMode( pcMbDataAccessBase->getMbData().getFieldFlag() );
      }

      RNOK( m_pcControlMng->initMbForDecoding( *pcMbDataAccess,    uiMbY, uiMbX, true ) );

      const PicType eMbPicType  = pcMbDataAccess->getMbPicType();
			const UInt    uiLI        = eMbPicType - 1;
      if( FRAME == eMbPicType )
      {
        pcRefFrameList0F = pcRefFrameList0;
        pcRefFrameList1F = pcRefFrameList1;
      }
      else
      {
        pcRefFrameList0F = apcRefFrameList0[eP];
        pcRefFrameList1F = apcRefFrameList1[eP];
      }

      if( pcMbDataAccess->getMbData().getBLSkipFlag() && ! m_apabBaseModeFlagAllowedArrays[ eMbPicType == FRAME ? 0 : 1 ][ uiMbAddressMbAff ] )
      {
        printf( "CIU constraint violated (MbAddr=%d) ==> ignore\n", uiMbAddressMbAff );
      }

      RNOK( m_pcMbDecoder->decode ( *pcMbDataAccess,
                                    pcMbDataAccessBase,
                                    apcFrame            [uiLI],
                                    apcResidualLF       [uiLI],
                                    apcResidualILPred   [uiLI],
                                    apcBaseLayer        [uiLI],
                                    apcBaseLayerResidual[uiLI],
                                    pcRefFrameList0F,
                                    pcRefFrameList1F,
                                    bReconstructAll ) );
    }
  }

  if( pcFrame )             RNOK( pcFrame->            removeFrameFieldBuffer() );
  if( pcResidualLF )        RNOK( pcResidualLF->       removeFrameFieldBuffer() );
  if( pcResidualILPred )    RNOK( pcResidualILPred->   removeFrameFieldBuffer() );
  if( pcBaseLayer )         RNOK( pcBaseLayer->        removeFrameFieldBuffer() );
  if( pcBaseLayerResidual ) RNOK( pcBaseLayerResidual->removeFrameFieldBuffer() );

	return Err::m_nOK;
}


H264AVC_NAMESPACE_END
