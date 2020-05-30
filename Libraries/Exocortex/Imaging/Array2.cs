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
using System.Reflection;


namespace Exocortex.Imaging
{

	public delegate void ArrayElementCopyFunction( Array dest, int destOffset, Array source, int sourceOffset );

	/// <summary>
	/// Summary description for Array2.
	/// </summary>
	public class Array2 {

		static public void GetPlaneExtractionParams(
			int xSamples, int ySamples, int zSamples, Plane plane, 
			out int alphaLength, out int alphaInc, out int betaLength, out int betaInc, out int gammaOffset ) {

			Debug.Assert( xSamples*ySamples*zSamples > 0 );
	
			Axis[]	axes = PlaneAxisUtils.ToParallelAxes( plane );
			Axis	axis = PlaneAxisUtils.ToPerpendicularAxis( plane );

			alphaLength = ( axes[0] == Axis.X ) ? xSamples : ( ( axes[0] == Axis.Y ) ? ySamples : zSamples );
			alphaInc =  ( axes[0] == Axis.X ) ? 1 : ( ( axes[0] == Axis.Y ) ? xSamples : xSamples*ySamples );
			betaLength = ( axes[1] == Axis.X ) ? xSamples : ( ( axes[1] == Axis.Y ) ? ySamples : zSamples );
			betaInc =  ( axes[1] == Axis.X ) ? 1 : ( ( axes[1] == Axis.Y ) ? xSamples : xSamples*ySamples );

			gammaOffset =  ( axis == Axis.X ) ? 1 : ( ( axis == Axis.Y ) ? xSamples : xSamples*ySamples );
		}

		static public bool IsCast( MethodInfo mi, Type source, Type dest ) {
		//	Debug2.Push( "IsCast( mi = " + mi.ToString() + " )" );
			if( mi.ReturnType == dest ) {
		//		Debug.WriteLine( "1" );
				if( mi.IsStatic == true ) {
		//			Debug.WriteLine( "2" );
					if( ( mi.Name == "op_Explicit" ) ||
						( mi.Name == "op_Implicit" ) ) {
		//				Debug.WriteLine( "3" );
						ParameterInfo[] parameters = mi.GetParameters();
						if( ( parameters.Length == 1 ) &&
							( parameters[0].ParameterType == source ) ) {
		//					Debug.WriteLine( "4" );
		//					Debug2.Pop();
							return	true;
						}
					}
				}
			}
		//	Debug2.Pop();
			return	false;
		}

		static public MethodBase GetConversionMethod( Type source, Type dest ) {
			foreach( MethodInfo mi in dest.GetMethods() ) {
				if( IsCast( mi, source, dest ) == true ) {
					return mi;
				}
			}
			foreach( MethodInfo mi in source.GetMethods() ) {
				if( IsCast( mi, source, dest ) == true ) {
					return mi;
				}
			}
			return	null;
		}

		static public void ExtractPlane(
			Array source, int xLength, int yLength, int zLength, Plane plane, int n, Array dest ) {

			Debug.Assert( source != null );
			Debug.Assert( source.Length == xLength*yLength*zLength );

			int aInc, aLength, bInc, bLength, cInc;

			//Debug.WriteLine( "Plane = " + plane );
			//Debug.WriteLine( "xLength = " + xLength );
			//Debug.WriteLine( "yLength = " + yLength );
			//Debug.WriteLine( "zLength = " + zLength );
			//Debug.WriteLine( "xyzLength = " + (xLength*yLength*zLength) );
			Array2.GetPlaneExtractionParams( xLength, yLength, zLength, plane, 
				out aLength, out aInc, out bLength, out bInc, out cInc );

			//Debug.WriteLine( "aLength = " + aLength );
			//Debug.WriteLine( "aInc = " + aInc );
			//Debug.WriteLine( "bLength = " + bLength );
			//Debug.WriteLine( "bInc = " + bInc );
			//Debug.WriteLine( "abLength = " + (aLength*bLength) );

			Debug.Assert( dest != null );
			Debug.Assert( dest.Length == ( aLength*bLength ) );

			Type destElementType = dest.GetType().GetElementType();
			Type sourceElementType = source.GetType().GetElementType();
			//Debug.WriteLine( "destElementType: " + destElementType.ToString() );
			//Debug.WriteLine( "sourceElementType: " + sourceElementType.ToString() );

			//Debug2.Push( "castMethod" );
			MethodBase castMethod = null;
			if( ( destElementType != sourceElementType ) ||
				( destElementType.IsValueType == false ) ) {
				castMethod = Array2.GetConversionMethod( sourceElementType, destElementType );
				Debug.Assert( castMethod != null );
				//	Debug.WriteLine( "Using castMethod: " + castMethod.ToString() );
			}

			//Debug2.Pop();

			//Debug2.Push( "copy" );

			int cOffset = ( n * cInc );
			int destOffset = 0;
			int bOffset = 0;
			for( int b = 0; b < bLength; b ++ ) {
				int aOffset = 0;
				for( int a = 0; a < aLength; a ++ ) {
					if( castMethod != null ) {
						dest.SetValue( castMethod.Invoke( null, new object[]{ source.GetValue( aOffset + bOffset + cOffset ) } ), destOffset );
					}
					else {
						dest.SetValue( source.GetValue( aOffset + bOffset + cOffset ), destOffset );
					}
					destOffset ++;
					aOffset += aInc;
				}
				bOffset +=  bInc;
			}
			//Debug2.Pop();
		}

	}
}
