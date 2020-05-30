#ifndef	__OPENGL_NATIVE
#define	__OPENGL_NATIVE

//================================================================================
//================================================================================


#pragma comment (lib, "opengl32.lib")
#include <GL/gl.h>


struct GC {
	HWND  hWnd;
	HDC   hDC;
	HGLRC hGLRC;
};
typedef GC*	HGC;


EXOCORTEX_API HGC	oglnCreateGC( HWND hWnd );
EXOCORTEX_API bool	oglnReleaseGC( HGC hGC );

EXOCORTEX_API void	oglnSetCurrentGC( HGC hGC );

EXOCORTEX_API void	oglnSwapBuffers( HGC hGC );

EXOCORTEX_API void	oglnEnterFullScreen( int width, int height, int bpp ); 
EXOCORTEX_API void	oglnLeaveFullScreen();
	 
EXOCORTEX_API int	oglnGetNumPixelFormats( HGC hGC );
EXOCORTEX_API int	oglnGetCurrentFormat( HGC hGC );
EXOCORTEX_API void	oglnSetPixelFormats( HGC hGC, int index, PIXELFORMATDESCRIPTOR *pPDF );
EXOCORTEX_API void	oglnSetPixelFormat( HGC hGC, int format );
EXOCORTEX_API void	oglnSetNearestPixelFormat( HGC hGC, PIXELFORMATDESCRIPTOR *pPDF );


//================================================================================
//================================================================================

#endif	__OPENGL_NATIVE
