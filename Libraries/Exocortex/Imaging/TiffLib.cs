/*
 * BSD Licence:
 * Copyright (c) 2001, Ben Houston [ ben@exocortex.org ]
 * Exocortex Technologies [ www.exocortex.org ]
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright 
 * notice, this list of conditions and the following disclaimer in the 
 * documentation and/or other materials provided with the distribution.
 * 3. Neither the name of the <ORGANIZATION> nor the names of its contributors
 * may be used to endorse or promote products derived from this software
 * without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
 * DAMAGE.
 */


using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using Exocortex;

namespace Exocortex.Imaging {

	public class TiffLib {

		//-----------------------------------------------------------------------

		static protected readonly int 	TIFFTAG_IMAGEWIDTH	= 256;	/* image width in pixels */
		static protected readonly int	TIFFTAG_IMAGELENGTH	= 257;	/* image height in pixels */

		[DllImport("ExocortexNative")]
		static protected extern int	TiffLibOpen( byte[] szFileName );
		[DllImport("ExocortexNative")]
		static protected extern	void TiffLibClose();
		[DllImport("ExocortexNative")]
		static protected extern	int TiffLibGetField( int field );
		[DllImport("ExocortexNative")]
		static protected extern	int TiffLibScanlineSize();
		[DllImport("ExocortexNative")]
		static protected extern unsafe byte* TiffLibLockData();
		[DllImport("ExocortexNative")]
		static protected extern unsafe void TiffLibUnlockData( byte* pData );

		//-----------------------------------------------------------------------
		
		static public unsafe Bitmap	LoadBitmap( string strFileName ) {
			if( File.Exists( strFileName ) == false ) {
				throw new FileNotFoundException( "LoadBitmap passed bad parameter.", strFileName );
			}

			byte[] fileName = new byte[ strFileName.Length + 1 ];
			for( int i = 0; i < strFileName.Length; i ++ ) {
				fileName[i] = (byte) strFileName[i];
			}
			fileName[ strFileName.Length ] = (byte) '\0';

			if ( TiffLibOpen( fileName ) == 0 ) {
				throw new FileLoadException( "TiffLibOpen failed", strFileName );
			}

			int imageWidth   = TiffLibGetField( TIFFTAG_IMAGEWIDTH );
			int imageLength  = TiffLibGetField( TIFFTAG_IMAGELENGTH );
			int scanlineSize = TiffLibScanlineSize(); 

			Bitmap bitmap = new Bitmap( imageWidth, imageLength, PixelFormat.Format16bppGrayScale );
			Rectangle rectImage = Rectangle.FromLTRB( 0, 0, imageWidth, imageLength );

			BitmapData bd = bitmap.LockBits( rectImage, ImageLockMode.WriteOnly, PixelFormat.Format16bppGrayScale );

			byte* pBitmapData = (byte*) bd.Scan0.ToPointer();
			byte* pData = TiffLibLockData();

			for( int i = 0; i < imageLength * scanlineSize; i ++ ) {
				pBitmapData[i] = pData[i];
			}
			
			TiffLibUnlockData( pData );
			bitmap.UnlockBits( bd );
						
			TiffLibClose();

			return	bitmap;
		}

		//-----------------------------------------------------------------------

	}

}
