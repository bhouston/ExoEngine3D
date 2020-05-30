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
using Exocortex;
using Exocortex.Mathematics;

namespace Exocortex.Imaging {

	public class BitmapUtils {

		/*static unsafe public Complex[]	ConvertBitmapToComplexArray( Bitmap bitmap ) {
			Complex[] array = new Complex[ ( bitmap.Width * bitmap.Height ) ];

			Rectangle rect = Rectangle.FromLTRB( 0, 0, bitmap.Width, bitmap.Height );
			BitmapData bd = bitmap.LockBits( rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
			
			uint *pData = (uint *) bd.Scan0.ToPointer();
			
			for( int i = 0; i < ( bitmap.Width * bitmap.Height ); i ++ ) {
				uint pixel = pData[ i ];
				uint sum = 0;
				sum += ( pixel & 0x00ff0000 ) >> 16;
				sum += ( pixel & 0x0000ff00 ) >> 8;
				sum += ( pixel & 0x000000ff ) >> 0;
				array[ i ].Real		= (float)sum / ( 255 * 3 );
				array[ i ].Imaginary	= 0;
			}
			
			bitmap.UnlockBits( bd );
		
			return array;
		}	*/

		static unsafe public float[]	ConvertBitmapToIntensityArray( Bitmap bitmap ) {
			//Debug2.Push( "ConvertBitmapToIntensityArray()" );
			int iSize = bitmap.Width * bitmap.Height;
			float[] aIntensity = new float[ iSize ];
			Rectangle rect = Rectangle.FromLTRB( 0, 0, bitmap.Width, bitmap.Height );
			BitmapData bd = bitmap.LockBits( rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
			uint *pData = (uint *) bd.Scan0.ToPointer();
			for( int i = 0; i < iSize; i ++ ) {
				uint pixel = pData[ i ];
				uint sum = 0;
				sum += ( pixel & 0x00ff0000 ) >> 16;
				sum += ( pixel & 0x0000ff00 ) >> 8;
				sum += ( pixel & 0x000000ff ) >> 0;
				aIntensity[ i ] = (float)sum / ( 255 * 3 );
			}
			bitmap.UnlockBits( bd );
			//Debug2.Pop();
			return aIntensity;
		}

		static unsafe public uint[]	ConvertBitmapToRGBAArray( Bitmap bitmapSrc ) {
			//Debug2.Push( "ConvertBitmapToRGBAArray()" );
			
			int		width	= bitmapSrc.Width;
			int		height	= bitmapSrc.Height;
			int		size	= width * height;
			uint[]	rgbaDest = new uint[ size ];

			BitmapData bd = bitmapSrc.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
			uint *rgbaSrc = (uint *) bd.Scan0.ToPointer();
			for( int i = 0; i < size; i ++ ) {
				rgbaDest[i] = rgbaSrc[i];
			}
			bitmapSrc.UnlockBits( bd );
			
			//Debug2.Pop();
			return rgbaDest;
		}

		static unsafe public void	SetTransparency( Bitmap bitmap, Color clrTrans ) {
			int		width	= bitmap.Width;
			int		height	= bitmap.Height;
			int		size	= width * height;

			BitmapData bd = bitmap.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );
			int *rgba = (int *) bd.Scan0.ToPointer();
			for( int i = 0; i < size; i ++ ) {
				Color clr = Color.FromArgb( rgba[i] );
				if( clr.R == clrTrans.R && clr.G == clrTrans.G && clr.B == clrTrans.B ) {
					rgba[i] = 0;//clr.ToArgb( 0, clr.R, clr.G, clr.B );
				}
			}
			bitmap.UnlockBits( bd );
		}

		static unsafe public void	CopyBitmapData( Bitmap bitmapSource, Bitmap bitmapDest ) {
			Debug.Assert( bitmapSource.Width == bitmapDest.Width );
			Debug.Assert( bitmapSource.Height == bitmapDest.Height );

			int		width	= bitmapSource.Width;
			int		height	= bitmapSource.Height;
			int		size	= width * height;

			BitmapData bdSource = bitmapSource.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
			BitmapData bdDest = bitmapDest.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );

			int *rgbaSource = (int *) bdSource.Scan0.ToPointer();
			int *rgbaDest = (int *) bdDest.Scan0.ToPointer();
			for( int i = 0; i < size; i ++ ) {
				rgbaDest[i] = rgbaSource[i];
			}

			bitmapSource.UnlockBits( bdSource );
			bitmapDest.UnlockBits( bdDest );
		}
		
		static unsafe public void CreateDisabledBitmap( Bitmap bitmap, int alpha ) {
			//Debug2.Push( "ConvertBitmapToRGBAArray()" );
			
			int		width	= bitmap.Width;
			int		height	= bitmap.Height;
			int		size	= width * height;

			BitmapData bd = bitmap.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );
			int *rgba = (int *) bd.Scan0.ToPointer();
			//float scale = ( 1 / 3f );
			for( int i = 0; i < size; i ++ ) {
				Color clr = Color.FromArgb( rgba[i] );
				int intensity = (int)clr.R + (int)clr.G + (int)clr.B;
				if( clr.A > 0 && intensity < (255+255+1) ) {
					rgba[i] = Color.FromArgb( alpha, 0, 0, 0 ).ToArgb();
				}
				else {
					rgba[i] = Color.FromArgb( 0, 0, 0, 0 ).ToArgb();
				}
			}
			bitmap.UnlockBits( bd );
			
			//Debug2.Pop();
		}

		static public void FlipVerticallyRGBAArray( uint[] data ) {
			int size = data.Length;
			int halfSize = size / 2;
			for( int i = 0; i < halfSize; i ++ ) {
				uint temp = data[i];
				data[i] = data[ size - i - 1 ];
				data[ size - i - 1 ] = temp;
			}
		}

		static protected uint[]	_intensityArray = null;
		static public uint[]	GetIntensityArray() {
			if( _intensityArray == null ) {
				_intensityArray = new uint[ 256 ];
				for( uint i = 0; i < 256; i ++ ) {
					_intensityArray[ i ] = (uint)( ((uint)0xff << 24) | ( i << 16) | ( i << 8) | i );
				}
			}
			return	_intensityArray;
		}

		static unsafe public Bitmap	ConvertIntensityArrayToBitmap( float[] data, int width, int height ) {
			//Debug2.Push( "ConvertIntensityArrayToBitmap()" );
			return	BitmapUtils.ConvertIntensityArrayToBitmap( null, data, width, height );
			//Debug2.Pop();
		}

		static unsafe public Bitmap	ConvertIntensityArrayToBitmap( Bitmap bitmap, float[] data, int width, int height ) {
			if( ( bitmap == null ) ||
				( bitmap.Width != width ) ||
				( bitmap.Height != height ) ||
				( bitmap.PixelFormat != PixelFormat.Format32bppArgb ) ) {
				bitmap = new Bitmap( width, height, PixelFormat.Format32bppArgb );
			}

			//Debug2.Push( "ConvertIntensityArrayToBitmap()" );
			int size = width * height;
			Rectangle rect = Rectangle.FromLTRB( 0, 0, width, height );
			BitmapData bd = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
			uint[] intensities = BitmapUtils.GetIntensityArray();
			uint *pData = (uint *) bd.Scan0.ToPointer();
			for( int i = 0; i < size; i ++ ) {
				float element = data[ i ];
				if( element < 0 ) {
					pData[ i ] = intensities[ 0 ];
				}
				else if ( element > 1 ) {
					pData[ i ] = intensities[ 255 ];
				}
				else {
					pData[ i ] = intensities[ (int)( 255 * element ) ];
				}
			}
			bitmap.UnlockBits( bd );
			//Debug2.Pop();
			return	bitmap;
		}
	}
}
