#ifndef	__TIFF_NATIVE
#define	__TIFF_NATIVE

//================================================================================
//================================================================================


#pragma comment (lib, "libtiff.lib")
#include "tiff.h"
#include "tiffio.h"


EXOCORTEX_API int	TestFunction( void );

EXOCORTEX_API int	TiffLibOpen( char *szFileName );
EXOCORTEX_API void	TiffLibClose();

EXOCORTEX_API int TiffLibGetField( int param );

EXOCORTEX_API unsigned char* TiffLibLockData();
EXOCORTEX_API void	TiffLibUnlockData( unsigned char *pData );

EXOCORTEX_API int TiffLibNumberOfTiles();
EXOCORTEX_API int TiffLibNumberOfStrips();
EXOCORTEX_API int TiffLibScanlineSize();
EXOCORTEX_API int TiffLibStripSize();
EXOCORTEX_API int TiffLibTileRowSize();
EXOCORTEX_API int TiffLibTileSize();

//================================================================================
//================================================================================

#endif	__TIFF_NATIVE