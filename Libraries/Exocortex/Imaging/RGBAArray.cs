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


namespace Exocortex.Imaging {

	/// <summary>
	/// Summary description for Imaging.
	/// </summary>

	public class RGBAArray {

		static public void GetRange( RGBA[] array, ref float minimum, ref float maximum ) {
			minimum = 255;
			maximum = 0;
			for( int i = 0; i < array.Length; i ++ ) {
				float temp = array[i].GetLuminanceF();
				minimum = Math.Min( temp, minimum );
				maximum = Math.Max( temp, maximum );
			}
		}

		static public void Offset( RGBA[] array, float fOffset ) {
			for( int i = 0; i < array.Length; i ++ ) {
				array[i] += fOffset;
			}
		}

		static public void Scale( RGBA[] array, float fScale ) {
			for( int i = 0; i < array.Length; i ++ ) {
				array[i] *= fScale;
			}
		}

		static public void Normalize( RGBA[] array ) {
			float min = 0, max = 0;
			GetRange( array, ref min, ref max );
			Scale( array, 1 / ( max - min ) );
			Offset( array, - min / ( max - min ) );
		}

		static public void Invert( RGBA[] array ) {
			float min = 0, max = 0;
			GetRange( array, ref min, ref max );
			Scale( array, -1 );
			Offset( array, min + max );
		}

		//----------------------------------------------------------------------------------------
		
		/*static public void ExtractPlane(
			RGBA[] source, int xSamples, int ySamples, int zSamples, Plane plane, int n, RGBA[] dest ) {

			Debug.Assert( source != null );
			Debug.Assert( source.Length == xSamples*ySamples*zSamples );
			Debug.Assert( dest != null );

			int offset;
			int alphaStep, alphaLength;
			int betaStep, betaLength;
			int destLength;

			Array2.GetPlaneExtractionParams( xSamples, ySamples, zSamples, plane, n,
				out offset, out alphaStep, out alphaLength, out betaStep, out betaLength, out destLength );

			Debug.Assert( dest.Length >= destLength );

			for( int b = 0; b < betaLength; b ++ ) {
				int bSourceOffset = offset + b * betaStep;
				int bDestOffset = b * betaLength;
				for( int a = 0; a < alphaLength; a ++ ) {
					int abSourceOffset = bSourceOffset + a * alphaStep;
					int abDestOffset = bDestOffset + a;
					dest[ abDestOffset ] = source[ abSourceOffset ];
				}
			}
		}
		
		static public void ExtractPlane(
			RGBA[] source, int xSamples, int ySamples, int zSamples, Plane plane, int n, float[] dest ) {

			Debug.Assert( source != null );
			Debug.Assert( source.Length == xSamples*ySamples*zSamples );
			Debug.Assert( dest != null );

			int offset;
			int alphaStep, alphaLength;
			int betaStep, betaLength;
			int destLength;

			Array2.GetPlaneExtractionParams( xSamples, ySamples, zSamples, plane, n,
				out offset, out alphaStep, out alphaLength, out betaStep, out betaLength, out destLength );

			Debug.Assert( dest.Length >= destLength );

			for( int b = 0; b < betaLength; b ++ ) {
				int bSourceOffset = offset + b * betaStep;
				int bDestOffset = b * betaLength;
				for( int a = 0; a < alphaLength; a ++ ) {
					int abSourceOffset = bSourceOffset + a * alphaStep;
					int abDestOffset = bDestOffset + a;
					dest[ abDestOffset ] = (float) source[ abSourceOffset ];
				}
			}
		}	*/

		//----------------------------------------------------------------------------------------

	}
}
