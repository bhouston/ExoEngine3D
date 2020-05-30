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
	/// Represents a quaternion.  Basically a quaternion is a 4D complex number as well
	/// as an efficient way to represent rotations in 3-space.
	/// </summary>
	[XmlType("quaternion"),StructLayout(LayoutKind.Sequential)]
	public struct Quaternion : ICloneable {

		// ================================================================================

		/// <summary>
		/// get the x (real?) component
		/// </summary>
		public	float	X;

		/// <summary>
		/// get the y (real?) component
		/// </summary>
		public	float	Y;
		
		/// <summary>
		/// get the z (real?) component
		/// </summary>
		public	float	Z;

		/// <summary>
		/// get the w (imaginary?) component
		/// </summary>
		public	float	W;

		// ================================================================================
		
		/// <summary>
		/// Create a quaternion by specifying its components in cartesian space
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="w"></param>
		public Quaternion( float x, float y, float z, float w ) {
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		// ================================================================================

		/// <summary>
		/// Clone this quaternion.
		/// </summary>
		/// <returns></returns>
		public Quaternion Clone() {
			return	new	Quaternion( this.X, this.Y, this.Z, this.W );
		}

		object ICloneable.Clone() {
			return	(object) this.Clone();
		}

		// ================================================================================

		/// <summary>
		/// Create a quaternion given an axis and a rotation around that axis
		/// </summary>
		/// <param name="axis"></param>
		/// <param name="radians"></param>
		/// <returns></returns>
										 
		static public Quaternion	FromAxisAngle( Vector3D axis, float radians ) {
			axis.Normalize();
			Vector3D scaledAxis = axis * (float)Math.Sin( radians * 0.5f );
			return	new Quaternion( scaledAxis.X, scaledAxis.Y, scaledAxis.Z, (float) Math.Cos( radians * 0.5f ) );
		}

		/// <summary>
		/// Create a quaternion given an axis and a rotation around that axis
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="fRadians"></param>
		/// <returns></returns>
		static public Quaternion	FromAxisAngle( float x, float y, float z, float fRadians ) {
			return	Quaternion.FromAxisAngle( Vector3D.FromXYZ( x, y, z ), fRadians );
		}
		
		/// <summary>
		/// Create a quaternion from a 3D homogeneous transform
		/// </summary>
		/// <param name="xfrm"></param>
		/// <returns></returns>
		static public Quaternion	FromTransform( Matrix3D xfrm ) {
			Quaternion quat = new Quaternion();
			
			// Check the sum of the diagonal
			float tr = xfrm[ 0, 0 ] + xfrm[ 1, 1 ] + xfrm[ 2, 2 ];
			if(tr > 0.0f) {
				// The sum is positive
				// 4 muls, 1 div, 6 adds, 1 trig function call
				float s = (float) Math.Sqrt( tr + 1.0f );
				quat.W = s * 0.5f;
				s = 0.5f / s;
				quat.X = ( xfrm[ 1, 2 ] - xfrm[ 2, 1 ] ) * s;
				quat.Y = ( xfrm[ 2, 0 ] - xfrm[ 0, 2 ] ) * s;
				quat.Z = ( xfrm[ 0, 1 ] - xfrm[ 1, 0 ] ) * s;
			}
			else {
				// The sum is negative
				// 4 muls, 1 div, 8 adds, 1 trig function call
				int[] nIndex = { 1, 2, 0 };
				int i, j, k;
				i = 0;
				if( xfrm[ 1, 1 ] >  xfrm[ i, i ] )
					i = 1;
				if( xfrm[ 2, 2 ] > xfrm[ i, i ] )
					i = 2;
				j = nIndex[i];
				k = nIndex[j];

				float s = (float) Math.Sqrt(( xfrm[ i, i ] - ( xfrm[ j, j ] + xfrm[ k, k ] ) ) + 1.0f );
				quat[ i ] = s * 0.5f;
				if( s != 0.0 ) {
					s = 0.5f / s;
				}
				quat[ j ] = ( xfrm[ i, j ] + xfrm[ j, i ] ) * s;
				quat[ k ] = ( xfrm[ i, k ] + xfrm[ k, i ] ) * s;
				quat[ 3 ] = ( xfrm[ j, k ] - xfrm[ k, j ] ) * s;
			}
			return quat;
		}

		// ================================================================================

		/*  currently, thinking it's better to use functions for costly conversions such as this.
		public static explicit operator Quaternion( Matrix3D xfrm ) {
			return Quaternion.FromTransform( xfrm );
		}*/

		// ================================================================================

		/// <summary>
		/// An index accessor that maps [0] -> X, [1] -> Y, [3] -> Z and [3] -> W.
		/// </summary>
		public float this[ int index ] {
			get	{
				Debug.Assert( 0 <= index && index <= 3 );
				if( index <= 1 ) {
					if( index == 0 ) {
						return this.X;
					}
					return this.Y;
				}
				if( index == 2 ) {
					return this.Z;
				}
				return this.W;
			}
			set {
				Debug.Assert( 0 <= index && index <= 3 );
				if( index <= 1 ) {
					if( index == 0 ) {
						this.X = value;
					}
					else {
						this.Y = value;
					}
				}
				else {
					if( index == 2 ) {
						this.Z = value;
					}
					else {
						this.W = value;
					}
				}
			}
		}
		
		// ================================================================================
		

		/// <summary>
		/// Extract the quaternion's it's axis and corresponding rotation.
		/// </summary>
		/// <param name="axis"></param>
		/// <param name="radians"></param>
		public void	GetAxisAngle( out Vector3D axis, out float radians ) {
			radians = (float) Math.Acos( this.W );
			if( radians != 0 ) {
				axis = new Vector3D( this.X, this.Y, this.Z );
				axis /= (float) Math.Sin( radians );
				axis.Normalize();
				radians *= 2;
			}
			else {
				axis = Vector3D.XAxis;
			}
		}

		// ================================================================================

		/// <summary>
		/// Are two quaterions equal?
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public	bool	operator==( Quaternion a, Quaternion b ) {
			return ( a.X == b.X ) && ( a.Y == b.Y ) && ( a.Z == b.Z ) && ( a.W == b.W );
		}

		/// <summary>
		/// Are two quaterions not equal?
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public	bool	operator!=( Quaternion a, Quaternion b ) {
			return ( a.X != b.X ) || ( a.Y != b.Y ) || ( a.Z != b.Z ) || ( a.W != b.W );
		}

		/// <summary>
		/// Is given object equal to current quaterions?
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool	Equals( object o ) {
			if( o is Quaternion ) {
				Quaternion q = (Quaternion) o;
				return ( this.X == q.X ) && ( this.Y == q.Y ) && ( this.Z == q.Z ) && ( this.W == q.W );
			}
			return false;
		}

		/// <summary>
		/// Get the hashcode of the Quaternion
		/// </summary>
		/// <returns></returns>
		public override int		GetHashCode() {
			return	this.X.GetHashCode() ^ ( ( ~ this.Y.GetHashCode() ) ^ ( this.Z.GetHashCode() ^ ( ~ this.W.GetHashCode() ) ) );
		}

		// ================================================================================

		/// <summary>
		/// add two quaternions
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public Quaternion	operator+( Quaternion a, Quaternion b ) {
			return new Quaternion( a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W );
		}	

		/// <summary>
		/// subtract two quaternions
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public Quaternion	operator-( Quaternion a, Quaternion b ) {
			return new Quaternion( a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W );
		}	
		/// <summary>
		/// multiply quaternion by a real number
		/// </summary>
		/// <param name="a"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		static public Quaternion	operator*( Quaternion a, float f ) {
			return new Quaternion( a.X * f, a.Y * f, a.Z * f, a.W * f );
		}	

		/// <summary>
		/// divide quaternion by a real number
		/// </summary>
		/// <param name="a"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		static public Quaternion	operator/( Quaternion a, float f ) {
			if( f == 0 ) {
				throw new DivideByZeroException( "Dividing quaternion by zero" );
			}
			return new Quaternion( a.X / f, a.Y / f, a.Z / f, a.W / f );
		}	
			
		/// <summary>
		/// rotate point by quaternion
		/// </summary>
		/// <param name="a"></param>
		/// <param name="pt"></param>
		/// <returns></returns>
		static public Vector3D		operator*( Quaternion a, Vector3D pt ) {
			Quaternion quatResult = a * new Quaternion( pt.X, pt.Y, pt.Z, 0 ) * a.GetUnitInverse();
			return	Vector3D.FromXYZ( quatResult.X, quatResult.Y, quatResult.Z );
		}

		/// <summary>
		/// rotate quaternion by another quaterion
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public Quaternion	operator*( Quaternion a, Quaternion b ) {
			// TODO: verify the validity of this function
			Debug.WriteLine( "Quaternion.operator*( Quaternion a, Quaternion b ) - warning, this function has not be checked for validity." );
			
			// 12 muls, 30 adds
			float E = (a.X + a.Z) * (b.X + b.Y);
			float F = (a.Z - a.X) * (b.X - b.Y);
			float G = (a.W + a.Y) * (b.W - b.Z);
			float H = (a.W - a.Y) * (b.W + b.Z);
			float A = F - E;
			float B = F + E;
			return new Quaternion(
				(a.W - a.X) * (b.Y + b.Z) + (B + G - H) * 0.5f,
				(a.Y + a.Z) * (b.W - b.X) + (B - G + H) * 0.5f,
				(a.Z - a.Y) * (b.Y - b.Z) + (A + G + H) * 0.5f,
				(a.W + a.X) * (b.W + b.X) + (A - G - H) * 0.5f );
		}
		
		// ================================================================================

		/// <summary>
		/// get the conjugate of this quaternion (UNVERIFIED)
		/// </summary>
		public	Quaternion	GetConjugate() {
			// TODO: verify the validity of this function
			Debug.WriteLine( "Quaternion.GetConjugate() - warning, this function has not be checked for validity." );
			return new Quaternion( -this.X, -this.Y, -this.Z, this.W );
		}

		/// <summary>
		/// get the inverse of this quaternion (UNVERIFIED) 
		/// </summary>
		public	Quaternion	GetInverse() {
			// TODO: verify the validity of this function
			Debug.WriteLine( "Quaternion.GetInverse() - warning, this function has not be checked for validity." );
			return this.GetConjugate() / this.GetMagnitudeSquared();
		}
		
		/// <summary>
		/// get the unit inverse of this quaternion (UNVERIFIED) 
		/// </summary>
		public	Quaternion	GetUnitInverse() {
			// TODO: verify the validity of this function
			Debug.WriteLine( "Quaternion.GetUnitInverse() - warning, this function has not be checked for validity." );
			return this.GetConjugate();
		}

		/// <summary>
		/// Get the magnitude of the quaternion
		/// </summary>
		public float	GetMagnitude() {
			return	 (float) Math.Sqrt( this.W*this.W + this.X*this.X + this.Y*this.Y + this.Z*this.Z );
		}

		/// <summary>
		/// Get the squared magnitude of the quaternion
		/// </summary>
		public float	GetMagnitudeSquared() {
			return	  this.W*this.W + this.X*this.X + this.Y*this.Y + this.Z*this.Z;
		}

		/// <summary>
		/// Scale quaternion so it's magnitude is one.
		/// </summary>
		public void		Normalize() {
			float length = this.GetMagnitude();
			if( length == 0 ) {
				throw new DivideByZeroException( "Can not normalize quaternion when it's magnitude is zero." );
			}
			this.X /= length;
			this.Y /= length;
			this.Z /= length;
			this.W /= length;
		}

		/// <summary>
		/// Get a parallel unit quaternion.
		/// </summary>
		/// <returns></returns>
		public Quaternion	GetUnit() {
			Quaternion q = this.Clone();
			q.Normalize();
			return	q;
		}

		// ================================================================================

		/// <summary>
		/// Calculate the dot product (inner product) between two quaternions
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public float	DotProduct( Quaternion a, Quaternion b ) {
			return	a.X * b.X + 
					a.Y * b.Y + 
					a.Z * b.Z + 
					a.W * b.W;
		}
				
		/// <summary>
		/// Get the spherically interpolated quaternion at position t between
		/// a and b (UNVERIFIED)
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		static public Quaternion Slerp( Quaternion a, Quaternion b, float t ) {
			// TODO: verify the validity of this function
			Debug.WriteLine( "Quaternion.Slerp() - warning, this function has not be checked for validity." );
			
			// Calculate the cosine of the angle between the two
			float fScale0, fScale1;
			double dCos = Quaternion.DotProduct( a, b );

			// If the angle is significant, use the spherical interpolation
			if( ( 1.0 - Math.Abs( dCos ) ) > 1e-6f ) {
				double dTemp = Math.Acos( Math.Abs( dCos ) );
				double dSin = Math.Sin( dTemp );
				fScale0 = (float)( Math.Sin( ( 1.0 - t ) * dTemp ) / dSin );
				fScale1 = (float)( Math.Sin( t * dTemp ) / dSin );
			}
				// Else use the cheaper linear interpolation
			else {
				fScale0 = 1.0f - t;
				fScale1 = t;
			}
			if(dCos < 0.0)
				fScale1 = -fScale1;

			// Return the interpolated result
			return (a * fScale0) + (b * fScale1);
		}
		
		// ================================================================================

		/// <summary>
		/// Get a string representation of the quaternion
		/// </summary>
		/// <returns></returns>
		public override string	ToString() {
			return	string.Format( "( x={0}, y={1}, z={2}, w={3} )", this.X, this.Y, this.Z, this.W );
		}

		// ================================================================================
	
		/// <summary>
		/// Zero (0,0,0,0)
		/// </summary>
		static public readonly Quaternion	Zero	= new Quaternion( 0, 0, 0, 0 );

		/// <summary>
		/// Origin (0,0,0,0)
		/// </summary>
		static public readonly Quaternion	Origin	= new Quaternion( 0, 0, 0, 0 );

		/// <summary>
		/// X-axis unit vector (1,0,0,0)
		/// </summary>
		static public readonly Quaternion	XAxis	= new Quaternion( 1, 0, 0, 0 );

		/// <summary>
		/// Y-axis unit vector (0,1,0,0)
		/// </summary>
		static public readonly Quaternion	YAxis	= new Quaternion( 0, 1, 0, 0 );
		
		/// <summary>
		/// Z-axis unit vector (0,0,1,0)
		/// </summary>
		static public readonly Quaternion	ZAxis	= new Quaternion( 0, 0, 1, 0 );

		/// <summary>
		/// W-axis unit vector (0,0,0,1)
		/// </summary>
		static public readonly Quaternion	WAxis	= new Quaternion( 0, 0, 0, 1 );

		// ================================================================================
	}
}
