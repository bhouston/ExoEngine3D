

#include "stdafx.h"


//=================================================================================
//=================================================================================

EXOCORTEX_API HGC oglnCreateGC( HWND hWnd ) {
	_ASSERT( hWnd != NULL );

	// get hGC
	HGC hGC = (HGC) malloc( sizeof( GC ) );
	if( hGC == NULL ) {
		_ERROR();
		return NULL;
	}
	memset( hGC, 0, sizeof( GC ) );

	// set hGC->hWnd
	hGC->hWnd	= hWnd;

	// get hGC->hDC
	hGC->hDC	= GetDC( hWnd );
	if( hGC->hDC == NULL ) {
		_ERROR();
		return NULL;
	}

	return hGC;
}

EXOCORTEX_API bool oglnReleaseGC( HGC hGC ) {
	_ASSERT( hGC != NULL );
	_LOG( "a\n" );

	// release hGC->hGLRC
	if( hGC->hGLRC != NULL ) {
		_ASSERT( hGC->hDC != NULL );
		log( "b: hGC->hDC= %x\n", (int)( hGC->hDC ) );
		/*if( wglMakeCurrent( hGC->hDC, NULL ) != TRUE ) {
			_ERROR();
		}*/
		log( "c: hGC->hGLRC= %x\n", (int)( hGC->hGLRC ) );
		if( wglDeleteContext( hGC->hGLRC ) != TRUE ) {
			_ERROR();
			return false;
		}
		_LOG( "d\n" );
		hGC->hGLRC = NULL;
	}

	_LOG( "e\n" );

	// release hGC->hDC
	if( hGC->hDC != NULL ) {
		_ASSERT( hGC->hWnd != NULL );
		_LOG( "f\n" );
		if( ReleaseDC( hGC->hWnd, hGC->hDC ) == 0 ) {
			_LOG( "Warning: ReleaseDC did not result in a complete release of DC" );
		}
		_LOG( "g\n" );
		hGC->hDC = NULL;
	}

	_LOG( "h\n" );

	// release hGC
	free( hGC );

	return true;
}

//=================================================================================
//=================================================================================

EXOCORTEX_API void oglnSetCurrentGC( HGC hGC ) {
	_ASSERT( hGC != NULL );
	_ASSERT( hGC->hDC != NULL );
	_ASSERT( hGC->hGLRC != NULL );

	if( wglMakeCurrent( hGC->hDC, hGC->hGLRC ) != TRUE ) {
		_ERROR();
	}
}

EXOCORTEX_API void oglnSwapBuffers( HGC hGC ) {
	_ASSERT( hGC != NULL );
	_ASSERT( hGC->hDC != NULL );

	if( SwapBuffers( hGC->hDC ) != TRUE ) {
		_ERROR();
	}
}

//=================================================================================
//=================================================================================

/* return the number of available pixel format for this context or 0 */
EXOCORTEX_API int oglnGetNumPixelFormats( HGC hGC ) {
	_ASSERT( hGC != NULL );
	_ASSERT( hGC->hDC != NULL );

	if( DescribePixelFormat( hGC->hDC, 1, 0, NULL ) == 0 ) {
		_ERROR();
		return false;
	}
	return true;
}

/* return the current pixel format index (1 based) or 0 */
EXOCORTEX_API int oglnGetCurrentFormat( HGC hGC ) {
	_ASSERT( hGC != NULL );
	_ASSERT( hGC->hDC != NULL );

	int format = GetPixelFormat( hGC->hDC );
	if( format == 0 ) {
		_ERROR();
		return 0;
	}
	return format;
}

/* return the current pixel format */
EXOCORTEX_API bool oglnGetPixelFormats( HGC hGC, int index, PIXELFORMATDESCRIPTOR *pPDF ) {
	_ASSERT( hGC != NULL );
	_ASSERT( hGC->hDC != NULL );
	_ASSERT( pPDF != NULL );
	
	if( DescribePixelFormat( hGC->hDC, index, pPDF->nSize, pPDF ) == 0 ) {
		_ERROR();
		return false;
	}
	return true;
}

/* set the (1 based index) pixel format */
EXOCORTEX_API void oglnSetPixelFormat( HGC hGC, int format ) {
	_ASSERT( hGC != NULL );
	_ASSERT( format != 0 );

	if( hGC->hGLRC != NULL ) {
		if( wglMakeCurrent( hGC->hDC, NULL ) != TRUE ) {
			_ERROR();
		}
		if( wglDeleteContext( hGC->hGLRC ) != TRUE ) {
			_ERROR();
		}
		hGC->hGLRC = NULL;
	}

	PIXELFORMATDESCRIPTOR pdf;
	pdf.nSize = sizeof( PIXELFORMATDESCRIPTOR );

	if( DescribePixelFormat( hGC->hDC, format, pdf.nSize, &pdf ) == 0 ) {
		_ERROR();
	}

	if( SetPixelFormat( hGC->hDC, format, &pdf ) != TRUE ) {
		_ERROR();
	}

	hGC->hGLRC = wglCreateContext( hGC->hDC );
	if( hGC->hGLRC == NULL ) {
		_ERROR();
	}
}

/* choose a pixel format near the one asked and return a gl context */
EXOCORTEX_API void oglnSetNearestPixelFormat( HGC hGC, PIXELFORMATDESCRIPTOR *pPDF ) {
	_ASSERT( hGC != NULL );
	_ASSERT( hGC->hDC != NULL );
	_ASSERT( pPDF != NULL );

	int format = ChoosePixelFormat( hGC->hDC, pPDF );
	if( format == 0 ) {
		_ERROR();
	}

	if( hGC->hGLRC != NULL ) {
		if( wglMakeCurrent( hGC->hDC, NULL ) != TRUE ) {
			_ERROR();
		}
		if( wglDeleteContext( hGC->hGLRC ) != TRUE ) {
			_ERROR();
		}
		hGC->hGLRC = NULL;
	}
	
	if( SetPixelFormat( hGC->hDC, format, pPDF ) != TRUE ) {
		_ERROR();
	}

	hGC->hGLRC = wglCreateContext( hGC->hDC );
	if( hGC->hGLRC == NULL ) {
		_ERROR();
	}
}

//=================================================================================
//=================================================================================

EXOCORTEX_API void oglnEnterFullScreen( int width, int height, int bpp ) {

	DEVMODE dmScreenSettings; // Device Mode
	memset( &dmScreenSettings, 0, sizeof( dmScreenSettings ) );		// Makes Sure Memory's Cleared
	dmScreenSettings.dmSize			= sizeof( dmScreenSettings );	// Size Of The Devmode Structure
	dmScreenSettings.dmPelsWidth	= width;						// Selected Screen Width
	dmScreenSettings.dmPelsHeight	= height;						// Selected Screen Height
	dmScreenSettings.dmBitsPerPel	= bpp;							// Selected Bits Per Pixel
	dmScreenSettings.dmFields		= DM_BITSPERPEL | DM_PELSWIDTH | DM_PELSHEIGHT;
	
	// Try To Set Selected Mode And Get Results.
	//	NOTE: CDS_FULLSCREEN Gets Rid Of Start Bar.
	int result = ChangeDisplaySettings( &dmScreenSettings, CDS_FULLSCREEN );
	if( result != DISP_CHANGE_SUCCESSFUL ) {
		_ERROR();
	}
}

EXOCORTEX_API void oglnLeaveFullScreen() {
	int result = ChangeDisplaySettings( NULL, 0 );
	if( result != DISP_CHANGE_SUCCESSFUL ) {
		_ERROR();
	}

	ShowCursor( TRUE );
}

//=================================================================================
//=================================================================================

