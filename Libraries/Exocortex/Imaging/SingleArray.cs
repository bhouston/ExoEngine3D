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
	/// Summary description for SingleArray.
	/// </summary>

	public class SingleArray {

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

		//----------------------------------------------------------------------------------------
		
		/*static public void	ConvertToComplexMath( float[] source, Complex[] dest ) {
			Debug.Assert( source != null );
			Debug.Assert( dest != null );
			Debug.Assert( source.Length == dest.Length );

			int length = source.Length;
			for( int i = 0; i < length; i ++ ) {
				dest[ i ] = (Complex) source[ i ];
			}
		}

		static public Complex[]	ConvertToComplexMath( float[] source ) {
			Debug.Assert( source != null );

			Complex[] dest = new Complex[ source.Length ];
			SingleArray.ConvertToComplexMath( source, dest );
			return	dest;
		}	*/

		//----------------------------------------------------------------------------------------

		static public void Shift( float[] array, int offset ) {
			Debug.Assert( array != null );
			Debug.Assert( offset >= 0 );
			Debug.Assert( offset < array.Length );

			if( offset == 0 ) {
				return;
			}

			int		length	= array.Length;
			float[]	temp	= new float[ length ];

			for( int i = 0; i < length; i ++ ) {
				temp[ ( i + offset ) % length ] = array[ i ];
			}
			for( int i = 0; i < length; i ++ ) {
				array[ i ] = temp[ i ];
			}
		}

		//----------------------------------------------------------------------------------------

		static public float	Sum( float[] array ) {
			Debug.Assert( array != null );

			int		length = array.Length;
			if( length == 0 ) {
				return	0;
			}

			int		sectionLength = 2048;
			int		sections = (int) Math.Ceiling( ((double)length) / sectionLength );

			float	sum = 0;
			
			for( int s = 0; s < sections; s ++ ) {
				
				int		start	= s * sectionLength;
				int		end		= Math.Min( start + sectionLength, length );
				float	partialSum	= 0;
				for( int i = start; i < end; i ++ ) {
					partialSum	+= array[ i ];
				}
			
				sum	+= partialSum;
			}

			return sum;
		}

		//----------------------------------------------------------------------------------------

		static public void	ConvertToRGBAArray( float[] source, RGBA rgba, RGBA[] dest ) {
			Debug.Assert( source != null );
			Debug.Assert( dest != null );
			Debug.Assert( source.Length == dest.Length );

			int length = source.Length;
			for( int i = 0; i < length; i ++ ) {
				dest[ i ] = rgba * source[ i ];
			}
		}

		static public RGBA[]	ConvertToRGBAArray( float[] source, RGBA rgba ) {
			Debug.Assert( source != null );

			RGBA[] dest = new RGBA[ source.Length ];
			SingleArray.ConvertToRGBAArray( source, rgba, dest );
			return	dest;
		}

		//----------------------------------------------------------------------------------------
		
		/*static public void ExtractPlane(
			float[] source, int xSamples, int ySamples, int zSamples, Plane plane, int n, float[] dest ) {

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
			float[] source, int xSamples, int ySamples, int zSamples, Plane plane, int n, RGBA[] dest ) {

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
					dest[ abDestOffset ] = (RGBA) source[ abSourceOffset ];
				}
			}
		}   */

		//----------------------------------------------------------------------------------------

	}
}
