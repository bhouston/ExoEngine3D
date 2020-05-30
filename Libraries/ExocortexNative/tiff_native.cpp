#include "stdafx.h"


TIFF* pTiff = NULL;

EXOCORTEX_API int TiffLibOpen( char *szFileName ) {
	pTiff = TIFFOpen( szFileName, "r" );
	if( pTiff != NULL ) {
		return	1;
	}
	else {
		return	0;
	}
}

EXOCORTEX_API void TiffLibClose() {
	TIFFClose( (TIFF*) pTiff );
	pTiff = NULL;
}


EXOCORTEX_API int TiffLibGetField( int param ) {
 	int result = 0;
	TIFFGetField( pTiff, param, &result );
	return	result;
}

EXOCORTEX_API unsigned char* TiffLibLockData() {
	int imageWidth = TiffLibGetField( TIFFTAG_IMAGEWIDTH );
	int imageLength = TiffLibGetField( TIFFTAG_IMAGELENGTH );
	int scanlineSize = TiffLibScanlineSize(); 
	
	unsigned char *pData = new unsigned char[ scanlineSize * imageLength ];
	tdata_t pBuffer =  _TIFFmalloc( scanlineSize );
	
	for( int row = 0; row < imageLength; row++ ) {
	    TIFFReadScanline( pTiff, pBuffer, row );

		int offset = scanlineSize * row;
		for( int i = 0; i < scanlineSize; i ++ ) {
			pData[offset + i] = ((unsigned char*)pBuffer)[i];
		}
	}
	_TIFFfree( pBuffer );

	return	pData;
}

EXOCORTEX_API void	TiffLibUnlockData( unsigned char *pData ) {
	_ASSERT( pData != NULL );
	delete[] pData;
}

EXOCORTEX_API int TiffLibNumberOfTiles() {
	return	TIFFNumberOfTiles( pTiff );
}

EXOCORTEX_API int TiffLibNumberOfStrips() {
	return	TIFFNumberOfStrips( pTiff );
}

EXOCORTEX_API int TiffLibScanlineSize() {
	return	TIFFScanlineSize( pTiff );
}

EXOCORTEX_API int TiffLibStripSize() {
	return	TIFFStripSize( pTiff );
}

EXOCORTEX_API int TiffLibTileRowSize() {
	return	TIFFTileRowSize( pTiff );
}

EXOCORTEX_API int TiffLibTileSize() {
	return	TIFFTileSize( pTiff );
}
