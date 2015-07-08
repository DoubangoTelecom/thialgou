
#if !defined(AFX_READBITSTREAMIF_H__A16C5223_7579_474B_A554_3A6FAEA78126__INCLUDED_)
#define AFX_READBITSTREAMIF_H__A16C5223_7579_474B_A554_3A6FAEA78126__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class h264HdrEventParsing;

class H264AVCVIDEOIOLIB_API ReadBitstreamIf
{
public:
  ReadBitstreamIf() { m_pIoEventHdr = NULL; };
  virtual ~ReadBitstreamIf() {};

  virtual ErrVal extractPacket( BinData*& rpcBinData, Bool& rbEOS ) = 0;
  virtual ErrVal releasePacket( BinData* pcBinData ) = 0;

  virtual ErrVal getPosition( Int& iPos ) = 0;
  virtual ErrVal setPosition( Int  iPos ) = 0;
  virtual Int64  getFilePos ()            = 0;

  virtual ErrVal uninit() = 0;
  virtual ErrVal destroy() = 0;

  virtual void setIoEventHdr(const h264HdrEventParsing* pIoEventHdr) { m_pIoEventHdr = pIoEventHdr; }
  virtual const h264HdrEventParsing* getIoEventHdr() { return m_pIoEventHdr; }

private:
	const h264HdrEventParsing* m_pIoEventHdr;
};

#endif // !defined(AFX_READBITSTREAMIF_H__A16C5223_7579_474B_A554_3A6FAEA78126__INCLUDED_)
