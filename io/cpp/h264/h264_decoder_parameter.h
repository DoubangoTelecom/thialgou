
#if !defined(AFX_DECODERPARAMETER_H__79149AEA_06A8_49CE_AB0A_7FC9ED7C05B5__INCLUDED_)
#define AFX_DECODERPARAMETER_H__79149AEA_06A8_49CE_AB0A_7FC9ED7C05B5__INCLUDED_

#include "h264_config.h"

class H264DecoderParameter
{
public:
  H264DecoderParameter();
  virtual ~H264DecoderParameter();

  ErrVal init( const Char* appName, const Char* inputFile, const Char* outputFile );
  ErrVal init( Int argc, Char** argv );

  std::string  cBitstreamFile;
  std::string  cYuvFile;
  Int          nResult;
  UInt         nFrames;
  UInt         uiMaxPocDiff;
  UInt         uiErrorConceal;
protected:
  ErrVal xPrintUsage( Char** argv );
};

#endif // !defined(AFX_DECODERPARAMETER_H__79149AEA_06A8_49CE_AB0A_7FC9ED7C05B5__INCLUDED_)
