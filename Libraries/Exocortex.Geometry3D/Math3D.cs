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

namespace Exocortex.Geometry3D {

	/// <summary>
	/// Signs
	/// </summary>
	public enum Sign : int {
		/// <summary>
		/// undefined relationship
		/// </summary>
		Undefined	= -2,
		/// <summary>
		/// equal or lesser than zero
		/// </summary>
		Negative	= -1,
		/// <summary>
		/// equal to zero
		/// </summary>
		Zero		= 0,
		/// <summary>
		/// equal or greater than zero
		/// </summary>
		Positive	= 1,
		/// <summary>
		/// conflicted relation, both greater and less than zero
		/// </summary>
		Mixed		= 2
	}

	/// <summary>
	/// A static library of useful mathematical routines
	/// </summary>
	public class Math3D {
		
		// don't allow creation
		private Math3D() {
		}

		//===============================================================================

		/// <summary>
		/// pi, a useful mathematical constant
		/// </summary>
		public const double PI  = System.Math.PI;

		/// <summary>
		///  2*PI, a useful mathematical constant
		/// </summary>
		public const double PI2 = System.Math.PI * 2;

		/// <summary>
		/// Epsilon, a fairly small value for a single precision floating point
		/// </summary>
		public const float EpsilonF = 1.0e-04F;

		//===============================================================================

		/// <summary>
		/// Get the sign of a number
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		static public Sign GetSign( int i ) {
			if( i > 0 )	{
				return	Sign.Positive;
			}
			if( i < 0 )	{
				return	Sign.Negative;
			}
			if( i == 0 ) {
				return	Sign.Zero;
			}
			throw new ArithmeticException( "can not get sign of invalid number: " + i.ToString() );
		}

		/// <summary>
		/// Get the sign of a number
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		static public Sign GetSign( float f ) {
			if( f > 0 )	{
				return	Sign.Positive;
			}
			if( f < 0 )	{
				return	Sign.Negative;
			}
			if( f == 0 ) {
				return	Sign.Zero;
			}
			throw new ArithmeticException( "can not get sign of invalid number: " + f.ToString() );
		}

		/// <summary>
		/// Get the sign of a number, defining zero loosely as the region (-epsilon, +epsilon)
		/// </summary>
		/// <param name="f"></param>
		/// <param name="epsilon"></param>
		/// <returns></returns>
		static public Sign GetSign( float f, float epsilon ) {
			if( f > epsilon )	{					
				return	Sign.Positive;
			}
			if( f < -epsilon )	{
				return	Sign.Negative;
			}
			if( -epsilon < f && f < epsilon ) {
				return	Sign.Zero;
			}
			throw new ArithmeticException( "can not get sign of invalid number: " + f.ToString() );
		}

		/// <summary>
		/// Determine the aggregate of two signs.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public Sign CombineSigns( Sign a, Sign b ) {
			//   undefined + undefined = undefined
			if( a == Sign.Undefined && b == Sign.Undefined ) {
				return Sign.Undefined;
			}

			//   undefined + negative  = negative
			//   undefined + zero      = zero
			//   undefined + positive  = positive
			//   undefined + mixed     = mixed
			if( a == Sign.Undefined ) {
				return b;
			}

			//   negative  + undefined = negative
			//   zero      + undefined = zero
			//   positive  + undefined = positive
			//   mixed     + undefined = mixed
			if( b == Sign.Undefined ) {
				return a;
			}
			
			//   zero     + zero = zero
			if( a == Sign.Zero && b == Sign.Zero ) {
				return Sign.Zero;
			}

			//   zero + negative = negative
			//   zero + positive = positive
			//   zero + mixed    = mixed
			if( a == Sign.Zero ) {
				return b;
			}

			//   negative + zero = negative
			//   positive + zero = positive
			//   mixed    + zero = mixed
			if( b == Sign.Zero ) {
				return a;
			}

			//   negative + negative = negative
			//   positive + positive = positive
			//   mixed    + mixed    = mixed
			if( a == b ) {
				return a;
			}

			//   negative + positive = mixed
			//   negative + mixed    = mixed
			//   positive + negative = mixed
			//   positive + mixed    = mixed
			return	Sign.Mixed;
		}

		//===============================================================================

		/// <summary>
		/// Compute distance between two locations.
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <returns></returns>
		static public float		Distance( int x1, int y1, int x2, int y2 ) {
			int dX = x2 - x1;
			int dY = y2 - y1;
			return	(float)	Math.Sqrt( dX*dX + dY*dY );
		}

		/// <summary>
		/// Compute the distance between two locations.
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <returns></returns>
		static public float		Distance( float x1, float y1, float x2, float y2 ) {
			float dX = x2 - x1;
			float dY = y2 - y1;
			return	(float)	Math.Sqrt( dX*dX + dY*dY );
		}

		/// <summary>
		/// Compute the distance between two locations.
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="z1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <param name="z2"></param>
		/// <returns></returns>
		static public float		Distance( int x1, int y1, int z1, int x2, int y2, int z2 ) {
			int dX = x2 - x1;
			int dY = y2 - y1;
			int dZ = z2 - z1;
			return	(float)	Math.Sqrt( dX*dX + dY*dY + dZ*dZ );
		}

		/// <summary>
		/// Compute the distance between two locations.
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="z1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <param name="z2"></param>
		/// <returns></returns>
		static public float		Distance( float x1, float y1, float z1, float x2, float y2, float z2 ) {
			float dX = x2 - x1;
			float dY = y2 - y1;
			float dZ = z2 - z1;
			return	(float)	Math.Sqrt( dX*dX + dY*dY + dZ*dZ );
		}
		
		//===============================================================================

	}
}
