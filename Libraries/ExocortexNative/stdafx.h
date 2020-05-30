// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

//---------------------------------------------------------------------------

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <stdlib.h>
#include <stdio.h>
#include <crtdbg.h>

//---------------------------------------------------------------------------

#ifdef EXOCORTEX_EXPORTS
	#define EXOCORTEX_API extern "C" __declspec(dllexport)
#else
	#define EXOCORTEX_API __declspec(dllimport)
#endif

//---------------------------------------------------------------------------

void	log( char *szFormatString, ... );
void	log_assert( bool bExpression, char *szFile, int iLine );
void	log_error( char *szFile, int iLine );

#ifdef _DEBUG
	#define	_LOG( x )	log( x )
#else
	#define	_LOG( x )
#endif

#ifdef _ASSERT
	#undef _ASSERT
#endif

#define	_ASSERT( x )	log_assert( x, __FILE__, __LINE__ )
#define	_ERROR()		log_error( __FILE__, __LINE__ )

//---------------------------------------------------------------------------

#include "opengl_native.h"
#include "tiff_native.h"

//---------------------------------------------------------------------------
