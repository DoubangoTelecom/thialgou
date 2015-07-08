/*
* Copyright (C) 2013 Mamadou DIOP
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
#ifndef THIALGOU_COMMON_CONFIG_H_
#define THIALGOU_COMMON_CONFIG_H_

#if defined(WIN32)|| defined(_WIN32) || defined(_WIN32_WCE)
#	define COMMON_UNDER_WINDOWS	1
#endif

#if (COMMON_UNDER_WINDOWS || defined(__SYMBIAN32__)) && defined(COMMON_EXPORTS)
# 	define COMMON_API		__declspec(dllexport)
# 	define COMMON_GEXTERN __declspec(dllexport)
#elif (COMMON_UNDER_WINDOWS || defined(__SYMBIAN32__)) /*&& defined(COMMON_IMPORTS)*/
# 	define COMMON_API __declspec(dllimport)
# 	define COMMON_GEXTERN __declspec(dllimport)
#else
#	define COMMON_API
#	define COMMON_GEXTERN	extern
#endif

/* Guards against C++ name mangling */
#ifdef __cplusplus
#	define COMMON_BEGIN_DECLS extern "C" {
#	define COMMON_END_DECLS }
#else
#	define COMMON_BEGIN_DECLS 
#	define COMMON_END_DECLS
#endif

#ifdef _MSC_VER
#       define _CRT_SECURE_NO_WARNINGS
#       define COMMON_INLINE        _inline
#else
#       define COMMON_INLINE        inline
#endif

#include <stdint.h>

#ifndef kProjectName
#	define kProjectName "thialgou::common"
#endif

#if HAVE_CONFIG_H
	#include <config.h>
#endif

#endif /* THIALGOU_COMMON_CONFIG_H_ */
