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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;

namespace Exocortex.Geometry3D {

	/// <summary>
	/// Represents a location in 3-space
	/// </summary>
	[XmlType("point3d"),StructLayout(LayoutKind.Sequential)]
	public struct Vector3D : ICloneable {

		// ================================================================================

		/// <summary>
		/// Creates a new vector set to ( x, y, z )
		/// </summary>
		public Vector3D( float x, float y, float z ) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		/// <summary>
		/// Creates a new vector set to ( element[0], element[1], element[2] )
		/// </summary>
		public Vector3D( float[] elements ) {
			Debug.Assert( elements != null );
			Debug.Assert( elements.Length >= 3 );
			this.X = elements[0];
			this.Y = elements[1];
			this.Z = elements[2];
		}

		/// <summary>
		/// Creates a new vector set to the values of the given vector
		/// </summary>
		public Vector3D( Vector3D vec ) {
			this.X = vec.X;
			this.Y = vec.Y;
			this.Z = vec.Z;
		}
		
		// ================================================================================

		/// <summary>
		/// Create a new vector set to ( x, y, z )
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		static public Vector3D	FromXYZ( float x, float y, float z ) {
			return	new Vector3D( x, y, z );
		}

		/// <summary>
		/// Create a new vector set to ( element[0], element[1], element[2] )
		/// </summary>
		/// <param name="elements"></param>
		/// <returns></returns>
		static public Vector3D	FromXYZ( float[] elements ) {
			return	new Vector3D( elements );
		}
		
		// ================================================================================

		/// <summary>
		/// Set the X, Y and Z coordinates of the vector at once.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public void	Set( float x, float y, float z ) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		/// <summary>
		/// Set the vector to the same location as the given vector
		/// </summary>
		/// <param name="vec"></param>
		public void	Set( Vector3D vec ) {
			this.X = vec.X;
			this.Y = vec.Y;
			this.Z = vec.Z;
		}

		// ================================================================================

		object ICloneable.Clone() {
			return new Vector3D( this );
		}

		/// <summary>
		/// Create a copy of this vector
		/// </summary>
		/// <returns></returns>
		public Vector3D	Clone() {
			return new Vector3D( this );
		}

		// ================================================================================

		/// <summary>
		/// The X component of the vector
		/// </summary>
		[XmlAttribute("x")]
		public	float	X;

		/// <summary>
		/// The Y component of the vector
		/// </summary>
		[XmlAttribute("y")]
		public	float	Y;
		
		/// <summary>
		/// The Z component of the vector
		/// </summary>
		[XmlAttribute("z")]
		public	float	Z;

		/// <summary>
		/// An index accessor that maps [0] -> X, [1] -> Y and [2] -> Z.
		/// </summary>
		public float this[ int index ] {
			get	{
				Debug.Assert( 0 <= index && index <= 2 );
				if( index <= 1 ) {
					if( index == 0 ) {
						return this.X;
					}
					return this.Y;
				}
				return this.Z;
			}
			set {
				Debug.Assert( 0 <= index && index <= 2 );
				if( index <= 1 ) {
					if( index == 0 ) {
						this.X = value;
					}
					else {
						this.Y = value;
					}
				}
				else {
					this.Z = value;
				}
			}
		}

		// ================================================================================

		/// <summary>
		/// Get the magnitude of the vector [i.e. Sqrt( X*X + Y*Y + Z*Z ) ]
		/// </summary>
		/// <returns></returns>
		public float	GetMagnitude() {
			return	 (float) Math.Sqrt( X*X + Y*Y + Z*Z );
		}

		/// <summary>
		/// Get the squared magnitude of the vector [i.e. ( X*X + Y*Y + Z*Z ) ]
		/// </summary>
		/// <returns></returns>
		public float	GetMagnitudeSquared() {
			return	X*X + Y*Y + Z*Z;
		}

		// ================================================================================

		// ================================================================================

		/*static public	bool	IsNAN( Vector3D vec ) {
			return	Single.IsNaN( vec.X ) || Single.IsNaN( vec.Y ) || Single.IsNaN( vec.Z );
		}
		static public	bool	IsInfinity( Vector3D vec ) {
			return	Single.IsInfinity( vec.X ) || Single.IsInfinity( vec.Y ) || Single.IsInfinity( vec.Z );
		}
		static public	bool	IsZero( Vector3D vec ) {
			return	( vec.X == 0 ) && ( vec.Y == 0 ) && ( vec.Z == 0 );
		}*/

		/// <summary>
		/// Get the vector with the shortest magnitude
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public	Vector3D	Min( Vector3D a, Vector3D b ) {
			return	( a.GetMagnitude() >= b.GetMagnitude() ) ? b : a;
		}

		/// <summary>
		/// Get the vector with the greatest magnitude
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public	Vector3D	Max( Vector3D a, Vector3D b ) {
			return	( a.GetMagnitude() >= b.GetMagnitude() ) ? a : b;
		}

		/// <summary>
		/// Get the smallest of each of the components in vector a and vector b
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public	Vector3D	MinXYZ( Vector3D a, Vector3D b ) {
			return	new Vector3D(
				Math.Min( a.X, b.X ),
				Math.Min( a.Y, b.Y ),
				Math.Min( a.Z, b.Z ) );
		}

		/// <summary>
		/// Get the greatest of each of the components in vector a and vector b
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public	Vector3D	MaxXYZ( Vector3D a, Vector3D b ) {
			return	new Vector3D(
				Math.Max( a.X, b.X ),
				Math.Max( a.Y, b.Y ),
				Math.Max( a.Z, b.Z ) );
		}

		// ================================================================================

		/// <summary>
		/// Get the vector's unit vector.
		/// </summary>
		/// <returns></returns>
		public Vector3D	GetUnit() {
			Vector3D vec = new Vector3D( this );
			vec.Normalize();
			return vec;
		}

		/// <summary>
		/// Scale point so that the magnitude is one
		/// </summary>
		public void		Normalize() {
			float length = GetMagnitude();
			if ( length == 0 ) {
				throw new DivideByZeroException( "Can not normalize a vector when it's magnitude is zero." );
			}
			this.X = X / length;
			this.Y = Y / length;
			this.Z = Z / length;
		}

		/// <summary>
		/// Transform the point
		/// </summary>
		/// <param name="xfrm"></param>
		public void		Transform( Matrix3D xfrm ) {
			float x = X, y = Y, z = Z;
			X = x*xfrm[0] + y*xfrm[4] + z*xfrm[8] + xfrm[12];
			Y = x*xfrm[1] + y*xfrm[5] + z*xfrm[9] + xfrm[13];
			Z = x*xfrm[2] + y*xfrm[6] + z*xfrm[10] + xfrm[14];
		}

		// ================================================================================

		/// <summary>
		/// Convert the point into the array 'new float[]{ X, Y, Z }'.  Note that this
		/// function causes a new float[] array to be allocated with each call.  Thus it 
		/// is somewhat inefficient.
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		static public explicit operator float[]( Vector3D vec ) {
			float[] elements = new float[3];
			elements[0] = vec.X;
			elements[1] = vec.Y;
			elements[2] = vec.Z;
			return	elements;
		}

		// ================================================================================

		/// <summary>
		/// A human understandable descrivecion of the point.
		/// </summary>
		/// <returns></returns>
		public override string	ToString() {
			return	string.Format( "( x={0}, y={1}, z={2} )", X, Y, Z );
		}

		/*public Color	ToColor() {
			int r = (int)Math3D.Clamp( Math.Abs( this.X * 255 ), 0, 255 );
			int g = (int)Math3D.Clamp( Math.Abs( this.Y * 255 ), 0, 255 );
			int b = (int)Math3D.Clamp( Math.Abs( this.Z * 255 ), 0, 255 );
			return	Color.FromArgb( 255, r, g, b );
		}  */

		// ================================================================================

		/// <summary>
		/// Are two points equal?
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public	bool	operator==( Vector3D a, Vector3D b ) {
			return ( a.X == b.X ) && ( a.Y == b.Y ) && ( a.Z == b.Z );
		}

		/// <summary>
		/// Are two point not equal?
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public	bool	operator!=( Vector3D a, Vector3D b ) {
			return ( a.X != b.X ) || ( a.Y != b.Y ) || ( a.Z != b.Z );
		}

		/// <summary>
		/// Is given object equal to current point?
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool	Equals( object o ) {
			if( o is Vector3D ) {
				Vector3D vec = (Vector3D) o;
				return ( this.X == vec.X ) && ( this.Y == vec.Y ) && ( this.Z == vec.Z );
			}
			return false;
		}

		/// <summary>
		/// Get the hashcode of the point
		/// </summary>
		/// <returns></returns>
		public override int		GetHashCode() {
			return	this.X.GetHashCode() ^ ( this.Y.GetHashCode() ^ ( ~ this.Z.GetHashCode() ) );
		}

		// ================================================================================

		/// <summary>
		/// Do nothing.
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		static	public	Vector3D	operator+( Vector3D vec ) {
			return	Vector3D.FromXYZ( + vec.X, + vec.Y, + vec.Z );
		}
		/// <summary>
		/// Invert the direction of the point.  Result is ( -vec.X, -vec.Y, -vec.Z ).
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		static	public	Vector3D	operator-( Vector3D vec ) {
			return	Vector3D.FromXYZ( - vec.X, - vec.Y, - vec.Z );
		}

		/*// add a constant to a point
		static public	Vector3D	operator+( Vector3D vecA, float f ) {
			return	Vector3D.FromXYZ( vecA.X + f, vecA.Y + f, vecA.Z + f );
		}
		// sub a constant to a point
		static public	Vector3D	operator-( Vector3D vecA, float f ) {
			return	Vector3D.FromXYZ( vecA.X - f, vecA.Y - f, vecA.Z - f );
		}*/

		/// <summary>
		/// Multiply vector vec by f.  Result is ( vec.X*f, vec.Y*f, vec.Z*f ).
		/// </summary>
		/// <param name="f"></param>
		/// <param name="vec"></param>
		/// <returns></returns>
		static public	Vector3D	operator*( float f, Vector3D vec ) {
			return	Vector3D.FromXYZ( vec.X * f, vec.Y * f, vec.Z * f );
		}

		/// <summary>
		/// Multiply vector vec by f.  Result is ( vec.X*f, vec.Y*f, vec.Z*f ).
		/// </summary>
		/// <param name="f"></param>
		/// <param name="vec"></param>
		/// <returns></returns>
		static public	Vector3D	operator*( Vector3D vec, float f ) {
			return	Vector3D.FromXYZ( vec.X * f, vec.Y * f, vec.Z * f );
		}
	
		/// <summary>
		/// Divide vector vec by f.  Result is ( vec.X/f, vec.Y/f, vec.Z/f ).
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		static public	Vector3D	operator/( Vector3D vec, float f ) {
			if( f == 0 ) {
				throw new DivideByZeroException( "can not divide a vector by zero" );
			}
			return	Vector3D.FromXYZ( vec.X / f, vec.Y / f, vec.Z / f );
		}

		/// <summary>
		/// Add two vectors.  Result is ( a.X + b.X, a.Y + b.Y, a.Z + b.Z )
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public	Vector3D	operator+( Vector3D a, Vector3D b ) {
			return	Vector3D.FromXYZ( a.X + b.X, a.Y + b.Y, a.Z + b.Z );
		}

		/// <summary>
		/// Subtract two vectors.  Result is ( a.X - b.X, a.Y - b.Y, a.Z - b.Z )
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public	Vector3D	operator-( Vector3D a, Vector3D b ) {
			return	Vector3D.FromXYZ( a.X - b.X, a.Y - b.Y, a.Z - b.Z );
		}

		/// <summary>
		/// Transform point vec by xfrm.
		/// </summary>
		/// <param name="xfrm"></param>
		/// <param name="vec"></param>
		/// <returns></returns>
		static public Vector3D operator*( Matrix3D xfrm, Vector3D vec ) {
			Vector3D vecNew = (Vector3D) vec.Clone();
			vecNew.Transform( xfrm );
			return vecNew;
		}

		// ================================================================================

		/// <summary>
		/// Add p to self.  Much quicker than using '+' operator since no new objects are created.
		/// </summary>
		/// <param name="vec"></param>
		public	void	Add( Vector3D vec ) {
			this.X += vec.X;
			this.Y += vec.Y;
			this.Z += vec.Z;
		}

		/// <summary>
		/// Subtract p from self.  Much quicker than using '-' operator since no new objects are created.
		/// </summary>
		/// <param name="vec"></param>
		public	void	Subtract( Vector3D vec ) {
			this.X -= vec.X;
			this.Y -= vec.Y;
			this.Z -= vec.Z;
		}

		/// <summary>
		/// Multiply self by f.  Much quicker than using '*' operator since no new objects are created.
		/// </summary>
		/// <param name="f"></param>
		public	void	Multiply( float f ) {
			this.X *= f;
			this.Y *= f;
			this.Z *= f;
		}
		
		/// <summary>
		/// Divide self by f.  Much quicker than using '/' operator since no new objects are created.
		/// </summary>
		/// <param name="f"></param>
		public	void	Divide( float f ) {
			if( f == 0 ) {
				throw new DivideByZeroException( "can not divide a vector by zero" );
			}
			this.X /= f;
			this.Y /= f;
			this.Z /= f;
		}

		// ================================================================================

		/// <summary>
		/// Calculate the dot project (i.e. inner product) of a and b.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public float		Dot( Vector3D a, Vector3D b ) {
			return	DotProduct( a, b );
		}

		/// <summary>
		/// Calculate the dot project (i.e. inner product) of a and b.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public float		DotProduct( Vector3D a, Vector3D b ) {
			// make it easier to see what is happening
			float x0 = a.X, y0 = a.Y, z0 = a.Z;
			float x1 = b.X, y1 = b.Y, z1 = b.Z;
			return	x0*x1 + y0*y1 + z0*z1;
		}

		/// <summary>
		/// Calculate the cross product (i.e. outer product) of a and b
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public Vector3D	Cross( Vector3D a, Vector3D b ) {
			return	CrossProduct( a, b );
		}

		/// <summary>
		/// Calculate the cross product (i.e. outer product) of a and b
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public Vector3D	CrossProduct( Vector3D a, Vector3D b ) {
			// make it easier to see what is happening
			float x0 = a.X, y0 = a.Y, z0 = a.Z;
			float x1 = b.X, y1 = b.Y, z1 = b.Z;
			return Vector3D.FromXYZ( y0*z1 - z0*y1, z0*x1 - x0*z1, x0*y1 - y0*x1 );
		}

		/// <summary>
		/// Determine whether sequency a-b-c is Clockwise, Counterclockwise or Collinear
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <returns></returns> 
		static public int		Orientation( Vector3D a, Vector3D b, Vector3D c ) {
			float length = Vector3D.CrossProduct( b - a, b - c ).GetMagnitude();
			if( length > 0 ) {
				return 1;
			}
			if( length < 0 ) {
				return -1;
			}
			if( length == 0 ) {
				return 0;
			}
			Debug.Fail( "should never get here" );
			return 0;
		}
		// interpolate between two points
		/*static public	Vector3D	Interpolate( Vector3D a, Vector3D b, float f ) {
			Debug.Assert( f >= 0 );
			Debug.Assert( f <= 1 );
			float alpha = 1 - f;
			float beta = f;
			return Vector3D.FromXYZ(
				a.X * alpha + b.X * beta, 
				a.Y * alpha + b.Y * beta, 
				a.Z * alpha + b.Z * beta );
		}  */

		// ================================================================================

		/// <summary>
		/// Zero (0,0,0)
		/// </summary>
		static public readonly Vector3D	Zero	= Vector3D.FromXYZ( 0, 0, 0 );

		/// <summary>
		/// Origin (0,0,0)
		/// </summary>
		static public readonly Vector3D	Origin	= Vector3D.FromXYZ( 0, 0, 0 );

		/// <summary>
		/// X-axis unit vector (1,0,0)
		/// </summary>
		static public readonly Vector3D	XAxis	= Vector3D.FromXYZ( 1, 0, 0 );

		/// <summary>
		/// Y-axis unit vector (0,1,0)
		/// </summary>
		static public readonly Vector3D	YAxis	= Vector3D.FromXYZ( 0, 1, 0 );
		
		/// <summary>
		/// Z-axis unit vector (0,0,1)
		/// </summary>
		static public readonly Vector3D	ZAxis	= Vector3D.FromXYZ( 0, 0, 1 );

		// ================================================================================
	}

	// ================================================================================
	// ================================================================================
	
	/*public class Vector3DCollection : CollectionBase {

		public virtual void Add( Vector3D o ){
			this.List.Add( o );        
		}

		public virtual Vector3D this[int index]	{
			get { return (Vector3D) this.List[index];	}
			set { this.List[index] = value;				}
		}
		
		public void	FromArray( Vector3D[] array ) {
			if( array != null ) {
				this.Clear();
				foreach( Vector3D o in array ) {
					this.Add( o );
				}
			}
		}
		
		public Vector3D[] ToArray() {
			Vector3D[] array = new Vector3D[ this.Count ];
			this.List.CopyTo( array, 0 );
			return array;
		}

		public void AddRange( IEnumerable e ) {
			IEnumerator enumerator = e.GetEnumerator();
			while( enumerator.MoveNext() != false ) {
				this.List.Add( enumerator.Current );
			}
		}

		public void Reverse() {
			int count = this.List.Count;
			for( int i = 0; i < count / 2; i ++ ) {
				object temp = this.List[i];
				this.List[i] = this.List[count - i - 1];
				this.List[count - i - 1] = temp;
			}
		}

		public int IndexOf( Vector3D o ) {
			return this.List.IndexOf( o );
		}

		public bool Contains( Vector3D o ) {
			return this.List.Contains( o );
		}

	}	  */

	// ================================================================================
	// ================================================================================

}
