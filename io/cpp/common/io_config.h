/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
#ifndef THIALGOU_IO_CONFIG_H_
#define THIALGOU_IO_CONFIG_H_

#if defined(WIN32)|| defined(_WIN32) || defined(_WIN32_WCE)
#	define IO_UNDER_WINDOWS	1
#endif

#if (IO_UNDER_WINDOWS || defined(__SYMBIAN32__)) && defined(IO_EXPORTS)
# 	define IO_API		__declspec(dllexport)
# 	define IO_GEXTERN __declspec(dllexport)
#elif (IO_UNDER_WINDOWS || defined(__SYMBIAN32__)) /*&& defined(IO_IMPORTS)*/
# 	define IO_API __declspec(dllimport)
# 	define IO_GEXTERN __declspec(dllimport)
#else
#	define IO_API
#	define IO_GEXTERN	extern
#endif

/* Guards against C++ name mangling */
#ifdef __cplusplus
#	define IO_BEGIN_DECLS extern "C" {
#	define IO_END_DECLS }
#else
#	define IO_BEGIN_DECLS 
#	define IO_END_DECLS
#endif

/* Disable some well-known warnings */
#ifdef _MSC_VER
#	define _CRT_SECURE_NO_WARNINGS
#endif

#include <stdint.h>

#ifndef kProjectName
#	define kProjectName "thialgou::io"
#endif

#if HAVE_CONFIG_H
	#include <config.h>
#endif

#endif /* THIALGOU_IO_CONFIG_H_ */
