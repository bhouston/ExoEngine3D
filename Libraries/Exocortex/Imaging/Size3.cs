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

namespace Exocortex.Imaging
{
	/// <summary>
	/// Summary description for Size3.
	/// </summary>
	public struct Size3 {

		//----------------------------------------------------------------------------
		
		private int _width;
		private int _height;
		private int _depth;

		//----------------------------------------------------------------------------
		
		public Size3( int width, int height, int depth ) {
			_width = width;	
			_height = height;
			_depth = depth;	
		}

		//----------------------------------------------------------------------------
		
		public int Width {
			get {	return	_width;	}
			set	{
				_width = value;	
			}
		}

		public int Height {
			get {	return	_height;	}
			set	{
				_height = value;
			}
		}
	
		public int Depth {
			get {	return	_depth;	}
			set	{
				_depth = value;	
			}
		}

		//----------------------------------------------------------------------------

		public bool	IsPositive {
			get {
				return	( _width >= 0 ) && ( _height >= 0 ) && ( _depth >= 0 );
			}
		}
		
		//----------------------------------------------------------------------------

		public int	GetTotalLength() {
			return	_width * _depth * _height;
		}

		public int	GetOffset( int x, int y, int z ) {
			return	x + y * ( _width ) + z * ( _width * _depth );
		}

		public Size3	GetCenter() {
			return	new Size3( ( _width >> 1 ), ( _height >> 1 ), ( _depth >> 1 ) );
		}
		
		//----------------------------------------------------------------------------
		
		public int this[ int dimension ] {
			get {
				switch( dimension ) {
				case 0:
					return	this.Width;
				case 1:
					return	this.Height;
				case 2:
					return	this.Depth;
				default:
					throw new ArgumentOutOfRangeException( "dimension", dimension, "Dimension must be 0, 1 or 2" );
				}
			}
			set {
				switch( dimension ) {
				case 0:
					this.Width = value;
					break;
				case 1:
					this.Height = value;
					break;
				case 2:
					this.Depth = value;
					break;
				default:
					throw new ArgumentOutOfRangeException( "dimension", dimension, "Dimension must be 0, 1 or 2" );
				}
			}

		}

		//----------------------------------------------------------------------------

		public override string ToString() {
			return	"[ " + this.Width + ", " + this.Height + ", " + this.Depth + " ]";
		}

		//-----------------------------------------------------------------------------------------

		public override bool Equals( object o ) {
			if( o is Size3 ) {
				Size3 size = (Size3) o;
				return	( this.Width == size.Width ) && ( this.Height == size.Height ) && ( this.Depth == size.Depth );
			}
			return	false;
		}

		public override int	GetHashCode() {
			return	( this.Width.GetHashCode() ^ this.Height.GetHashCode() ^ this.Depth.GetHashCode() );
		}
		
		//-----------------------------------------------------------------------------------------
		
		public static bool operator==( Size3 a, Size3 b ) {
			return	( a.Width == b.Width ) && ( a.Height == b.Height ) && ( a.Depth == b.Depth );
		}
		public static bool operator!=( Size3 a, Size3 b ) {
			return	( a.Width != b.Width ) || ( a.Height != b.Height ) || ( a.Depth != b.Depth );
		}

		
		public static Size3 operator+( Size3 a, Size3 b ) {
			return	new Size3( a.Width + b.Width, a.Height + b.Height, a.Depth + b.Depth );
		}
		public static Size3 operator-( Size3 a, Size3 b ) {
			return	new Size3( a.Width - b.Width, a.Height - b.Height, a.Depth - b.Depth );
		}
		public static Size3 operator*( Size3 a, float f ) {
			return	new Size3( (int)( a.Width * f ), (int)( a.Height * f ), (int)( a.Depth * f ) );
		}

		//----------------------------------------------------------------------------

		static public Size3	Empty {
			get {	return new Size3( 0, 0, 0 );	}
		}

		//----------------------------------------------------------------------------

		static public Size3	CeilingToBase( Size3 size, int b ) {
			return	new Size3(
				Math2.CeilingToBase( size.Width, b ),
				Math2.CeilingToBase( size.Height, b ),
				Math2.CeilingToBase( size.Depth, b ) );
		}

		static public Size3	RoundToBase( Size3 size, int b ) {
			return	new Size3(
				Math2.RoundToBase( size.Width, b ),
				Math2.RoundToBase( size.Height, b ),
				Math2.RoundToBase( size.Depth, b ) );
		}

		static public Size3	FloorToBase( Size3 size, int b ) {
			return	new Size3(
				Math2.FloorToBase( size.Width, b ),
				Math2.FloorToBase( size.Height, b ),
				Math2.FloorToBase( size.Depth, b ) );
		}

		//----------------------------------------------------------------------------

		static public bool	IsPowerOfBase( Size3 size, int b ) {
			return
				Math2.IsPowerOf2( size.Width ) && 
				Math2.IsPowerOf2( size.Height ) &&
				Math2.IsPowerOf2( size.Depth );
		}

		//----------------------------------------------------------------------------

		static public Size3	Max( Size3 a, Size3 b ) {
			return new Size3(
				Math.Max( a.Width, b.Width ),
				Math.Max( a.Height, b.Height ),
				Math.Max( a.Depth, b.Depth ) );
		}
	
		static public Size3	Min( Size3 a, Size3 b ) {
			return new Size3(
				Math.Min( a.Width, b.Width ),
				Math.Min( a.Height, b.Height ),
				Math.Min( a.Depth, b.Depth ) );
		}

		//----------------------------------------------------------------------------

	}
}
