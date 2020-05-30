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

using Exocortex.Mathematics;

namespace Exocortex.Imaging {

	/// <summary>
	/// Summary description for Imaging.
	/// </summary>

	public class FloatArray {

		static public void GetHistogram( float[] array, float fMinimum, float fMaximum, float[] histogram ) {
			// zero histogram counters
			for( int i = 0; i < histogram.Length; i ++ ) {
				histogram[i] = 0;
			}
			// populate histogram
			float fRange2BinScale = ((float) histogram.Length - 1) / ( fMaximum - fMinimum );
			for( int i = 0; i < array.Length; i ++ ) {
				int bin = (int)( ( array[i] - fMinimum ) * fRange2BinScale );
				if( bin >= 0 && bin < histogram.Length ) {
					histogram[bin]++;
				}
			}
		}

		static public void ClampRange( float[] array, float fMinimum, float fMaximum ) {
			for( int i = 0; i < array.Length; i ++ ) {
				array[i] = Math.Max( fMinimum, Math.Min( fMaximum, array[i] ) );
			}
		}

		static public void GetRange( float[] array, ref float fMinimum, ref float fMaximum ) {
			fMinimum = +float.MaxValue;
			fMaximum = -float.MaxValue;
			for( int i = 0; i < array.Length; i ++ ) {
				float fTemp = array[i];
				fMinimum = Math.Min( fTemp, fMinimum );
				fMaximum = Math.Max( fTemp, fMaximum );
			}
		}

		static public void Offset( float[] array, float fOffset ) {
			int length = array.Length;
			for( int i = 0; i < length; i ++ ) {
				array[i] += fOffset;
			}
		}

		static public void Scale( float[] array, float fScale ) {
			int length = array.Length;
			for( int i = 0; i < length; i ++ ) {
				array[i] *= fScale;
			}
		}

		static public void Normalize( float[] array ) {
			float fMin = 0, fMax = 0;
			GetRange( array, ref fMin, ref fMax );
			Scale( array, 1 / ( fMax - fMin ) );
			Offset( array, - fMin / ( fMax - fMin ) );
		}

		static public void Invert( float[] array ) {
			float fMin = 0, fMax = 0;
			GetRange( array, ref fMin, ref fMax );
			Scale( array, -1 );
			Offset( array, fMin + fMax );
		}

		/*static public Complex[]	ConvertToComplayArray( float[] array ) {
			Complex[] newArray = new Complex[ array.Length ];
			for( int i = 0; i < array.Length; i ++ ) {
				newArray[i].Real = array[i];
			}
			return	newArray;
		}  */


	}
}
