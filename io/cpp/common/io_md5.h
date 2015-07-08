/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source "thialgou" project <http://code.google.com/p/thialgou/>
*/
#ifndef _THIALGOU_IO_COMMON_MD5_H_
#define _THIALGOU_IO_COMMON_MD5_H_

#include "io_config.h"

#if !defined(SWIG)
#define HL_MD5_DIGEST_SIZE		16
#define HL_MD5_BLOCK_SIZE		64

#define HL_MD5_EMPTY			"d41d8cd98f00b204e9800998ecf8427e"

#define HL_MD5_STRING_SIZE		(HL_MD5_DIGEST_SIZE*2)
typedef char hl_md5string_t[HL_MD5_STRING_SIZE+1]; /**< Hexadecimal MD5 string. */
typedef uint8_t hl_md5digest_t[HL_MD5_DIGEST_SIZE]; /**< MD5 digest bytes. */
#endif /* !SWIG */

class Md5
{
public:
	Md5();
	virtual ~Md5();
	void init();
	void update(const void* p_ptr, size_t u_size);
	const char* final();
	const char* compute(const void* p_ptr, size_t u_size);

private:
	struct hl_md5context_s* m_pCtx;
	hl_md5string_t m_result;
};

#endif /* _THIALGOU_IO_COMMON_MD5_H_ */
