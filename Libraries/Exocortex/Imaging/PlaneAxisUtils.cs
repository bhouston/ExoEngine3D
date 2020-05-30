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

namespace Exocortex.Imaging
{
	/// <summary>
	/// Summary description for PlaneAxisUtils.
	/// </summary>
	public class PlaneAxisUtils
	{
		//--------------------------------------------------------------------------------

		static public Axis	ToAxis( int index ) {
			Debug.Assert( index >= 0 );
			Debug.Assert( index <= 2 );

			switch( index ) {
			case 0:
				return	Axis.X;
			case 1:
				return	Axis.Y;
			case 3:
				return	Axis.Z;
			}

			Debug.Assert( false );
			return	Axis.X;
		}
		
		static public Axis	ToPerpendicularAxis( Plane plane ) {
			switch( plane ) {
			case Plane.XY:
				return	Axis.Z;
			case Plane.YZ:
				return	Axis.X;
			case Plane.ZX:
				return	Axis.Y;
			default:
				Debug.Assert( false );
				return	Axis.Z;
			}
		}
		
		static public Axis[]	ToParallelAxes( Plane plane ) {
			switch( plane ) {
			case Plane.XY:
				return	new Axis[]{ Axis.X, Axis.Y };
			case Plane.YZ:
				return	new Axis[]{ Axis.Y, Axis.Z };
			case Plane.ZX:
				return	new Axis[]{ Axis.Z, Axis.X };
			default:
				Debug.Assert( false );
				return	new Axis[]{ Axis.X, Axis.Y };
			}
		}

		//--------------------------------------------------------------------------------

		static public Plane	ToPlane( int index ) {
			Debug.Assert( index >= 0 );
			Debug.Assert( index <= 2 );

			switch( index ) {
			case 0:
				return	Plane.YZ;
			case 1:
				return	Plane.ZX;
			case 2:
				return	Plane.XY;
			}

			Debug.Assert( false );
			return	Plane.YZ;
		}
		
		static public Plane	ToPerpendicularPlane( Axis axis ) {
			switch( axis ) {
			case Axis.X:
				return	Plane.YZ;
			case Axis.Y:
				return	Plane.ZX;
			case Axis.Z:
				return	Plane.XY;
			default:
				Debug.Assert( false );
				return	Plane.XY;
			}
		}

		//--------------------------------------------------------------------------------

	}
}
