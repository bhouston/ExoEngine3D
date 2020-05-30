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
using System.Xml.Serialization;

namespace Exocortex.Geometry3D {

	/// <summary>
	/// Represents a homogeneous 3D transform that is compatible with
	/// OpenGL's native format in memory organization.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class Matrix3D {

		// ================================================================================

		// |  0  4  8  12  |   |  RS R  R  T  |
		// |  1  5  9  13  |   |  R  RS R  T  |
		// |  2  6  10 14  | = |  R  R  RS T  |
		// |  3  7  11 15  |   |  0  0  0  1  |
		float[]		_elements = new float[16];
		
		// ================================================================================

		/// <summary>
		/// Create a new transform, values default to identity
		/// </summary>
		public	Matrix3D() {
			SetToIdentity();
		}

		/// <summary>
		/// Create a new matrix, values set to given elements
		/// </summary>
		/// <param name="elements"></param>
		public	Matrix3D( float[] elements ) {
			Debug.Assert( elements != null );
			Debug.Assert( elements.Length >= 16 );
			elements.CopyTo( _elements, 0 );
		}
		/// <summary>
		/// Creates a new matrix. Copy values from a given matrix.
		/// </summary>
		/// <param name="m"></param>
		public Matrix3D( Matrix3D m ) {
			Debug.Assert( m != null );
			m._elements.CopyTo( _elements, 0 );
		}

		// ================================================================================

		/// <summary>
		/// Create transform from given basis
		/// </summary>
		/// <param name="xAxis"></param>
		/// <param name="yAxis"></param>
		/// <param name="zAxis"></param>
		/// <returns></returns>
		static public Matrix3D	FromBasis( Vector3D xAxis, Vector3D yAxis, Vector3D zAxis ) {
			Matrix3D m = new Matrix3D();
			// results when m * pt:
			//    ( xAxis * pt.X + yAxis * pt.Y + zAxis * pt.Z )
			m[0] = xAxis.X;	m[4] = yAxis.X;	m[8]  = zAxis.X;	m[12] = 0;
			m[1] = xAxis.Y;	m[5] = yAxis.Y;	m[9]  = zAxis.Y;	m[13] = 0;
			m[2] = xAxis.Z;	m[6] = yAxis.Z;	m[10] = zAxis.Z;	m[14] = 0;
			m[3] = 0;		m[7] = 0;		m[11] = 0;		m[15] = 1;
			return	m;
		}

		/// <summary>
		/// Create rotation transform from a quaternion
		/// </summary>
		/// <param name="quat"></param>
		/// <returns></returns>
		static public Matrix3D	FromRotation( Quaternion quat ) {
			Matrix3D m = new Matrix3D();

			// 9 muls, 15 adds
			float x2 = quat.X + quat.X, y2 = quat.Y + quat.Y, z2 = quat.Z + quat.Z;
			float xx = quat.X * x2, xy = quat.X * y2, xz = quat.X * z2;
			float yy = quat.Y * y2, yz = quat.Y * z2, zz = quat.Z * z2;
			float wx = quat.W * x2, wy = quat.W * y2, wz = quat.W * z2;

			float[] elements = m._elements;
            elements[3] = elements[7] = elements[11] = elements[12] = elements[13] = elements[14] = 0; elements[15] = 1;
			elements[0] = 1-(yy+zz);	elements[4] = xy+wz;		elements[8] = xz-wy;
			elements[1] = xy-wz;		elements[5] = 1-(xx+zz);	elements[9] = yz+wx;
			elements[2] = xz+wy;		elements[6] = yz-wx;		elements[10] = 1-(xx+yy);
			return m;	
		}

		/// <summary>
		/// Create rotation transform from an axis and rotation
		/// </summary>
		/// <param name="axis"></param>
		/// <param name="radians"></param>
		/// <returns></returns>
		static public Matrix3D	FromRotation( Vector3D axis, float radians ) {
			return Matrix3D.FromRotation( Quaternion.FromAxisAngle( axis, radians ) );
		}

		/// <summary>
		/// Create a scaling transform
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		static public Matrix3D	FromScale( float x, float y, float z ) {
			Matrix3D m = new Matrix3D();
			m._elements[0] = x;
			m._elements[5] = y;
			m._elements[10] = z;
			return m;
		}

		/// <summary>
		/// Create a scaling transform
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		static public Matrix3D	FromScale( Vector3D pt ) {
			return Matrix3D.FromScale( pt.X, pt.Y, pt.Z );
		}

		/// <summary>
		/// Create a translating transform
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		static public Matrix3D FromTranslation( float x, float y, float z ) {
			Matrix3D m = new Matrix3D();
			m._elements[12] = x;
			m._elements[13] = y;
			m._elements[14] = z;
			return m;
		}

		/// <summary>
		/// create a translating transform
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		static public Matrix3D	FromTranslation( Vector3D pt ) {
			return Matrix3D.FromTranslation( pt.X, pt.Y, pt.Z );
		}

		/// <summary>
		/// create the composite transform
		/// </summary>
		/// <param name="m0"></param>
		/// <param name="m1"></param>
		/// <returns></returns>
		static public Matrix3D		FromMultiplication( Matrix3D m0, Matrix3D m1 ) {
			Matrix3D mResult = new Matrix3D();
			
			// make things easier to read
			float[]	e0		= m0._elements;
			float[] e1		= m1._elements;
			float[] result	= mResult._elements;

			result[0]  = e0[0]*e1[0]  + e0[4]*e1[1]  + e0[8]*e1[2];
			result[4]  = e0[0]*e1[4]  + e0[4]*e1[5]  + e0[8]*e1[6];
			result[8]  = e0[0]*e1[8]  + e0[4]*e1[9]  + e0[8]*e1[10];
			result[12] = e0[0]*e1[12] + e0[4]*e1[13] + e0[8]*e1[14] + e0[12];

			result[1]  = e0[1]*e1[0]  + e0[5]*e1[1]  + e0[9]*e1[2];
			result[5]  = e0[1]*e1[4]  + e0[5]*e1[5]  + e0[9]*e1[6];
			result[9]  = e0[1]*e1[8]  + e0[5]*e1[9]  + e0[9]*e1[10];
			result[13] = e0[1]*e1[12] + e0[5]*e1[13] + e0[9]*e1[14] + e0[13];

			result[2]  = e0[2]*e1[0]  + e0[6]*e1[1]  + e0[10]*e1[2];
			result[6]  = e0[2]*e1[4]  + e0[6]*e1[5]  + e0[10]*e1[6];
			result[10] = e0[2]*e1[8]  + e0[6]*e1[9]  + e0[10]*e1[10];
			result[14] = e0[2]*e1[12] + e0[6]*e1[13] + e0[10]*e1[14] + e0[14];

			result[3]  = 0;
			result[7]  = 0;
			result[11] = 0;
			result[15] = 1;

			return mResult;
		}

		// ================================================================================

		/// <summary>
		/// An OpenGL compatible linear indexer to the matrix elements
		/// |  0  4  8  12  |   |  RS R  R  T  |
		/// |  1  5  9  13  |   |  R  RS R  T  |
		/// |  2  6  10 14  | = |  R  R  RS T  |
		/// |  3  7  11 15  |   |  0  0  0  1  |
		/// </summary>
		public float	this[ int index ] {
			get {
				Debug.Assert( 0 <= index && index < 16 );
				return	_elements[ index ];	
			}
			set {
				Debug.Assert( 0 <= index && index < 16 );
				_elements[ index ] = value;	
			}
		}

		/// <summary>
		/// A standard (column, row) indexer to the matrix elements
		/// </summary>
		public float	this[ int column, int row ] {
			get {
				Debug.Assert( 0 <= column && column < 4 );
				Debug.Assert( 0 <= row && row < 4 );
				return	_elements[ column + row*4 ];	
			}
			set {
				Debug.Assert( 0 <= column && column < 4 );
				Debug.Assert( 0 <= row && row < 4 );
				_elements[ column + row*4 ] = value;	
			}
		}

		// ================================================================================

		/// <summary>
		/// Set the matrix to all zeros
		/// </summary>
		public void SetToZero() {
			_elements[0] = 0;
			_elements[1] = 0;
			_elements[2] = 0;
			_elements[3] = 0;
			_elements[4] = 0;
			_elements[5] = 0;
			_elements[6] = 0;
			_elements[7] = 0;
			_elements[8] = 0;
			_elements[9] = 0;
			_elements[10] = 0;
			_elements[11] = 0;
			_elements[12] = 0;
			_elements[13] = 0;
			_elements[14] = 0;
			_elements[15] = 0;
		}

		/// <summary>
		/// Set the matrix to identity
		/// </summary>
		public void SetToIdentity() {
			_elements[0] = 1;
			_elements[1] = 0;
			_elements[2] = 0;
			_elements[3] = 0;
			_elements[4] = 0;
			_elements[5] = 1;
			_elements[6] = 0;
			_elements[7] = 0;
			_elements[8] = 0;
			_elements[9] = 0;
			_elements[10] = 1;
			_elements[11] = 0;
			_elements[12] = 0;
			_elements[13] = 0;
			_elements[14] = 0;
			_elements[15] = 1;
		}
		
		/// <summary>
		/// Set the elements of the matrix to the given array
		/// </summary>
		/// <param name="elements"></param>
		public void SetToArray( float[] elements ) {
			Debug.Assert( elements != null );
			Debug.Assert( elements.Length >= 16 );
			elements.CopyTo( _elements, 0 );
		}

		// ================================================================================
		
		/// <summary>
		/// Set the basis transform of the matrix, also set the translation to zero.
		/// </summary>
		/// <param name="xAxis"></param>
		/// <param name="yAxis"></param>
		/// <param name="zAxis"></param>
		public	void	ChangeBasis( Vector3D xAxis, Vector3D yAxis, Vector3D zAxis ) {
			Matrix3D mTemp = this * Matrix3D.FromBasis( xAxis, yAxis, zAxis );
			SetToArray( mTemp._elements );
		}

		/// <summary>
		/// Extract the matrix' current basis transform
		/// </summary>
		/// <param name="xAxis"></param>
		/// <param name="yAxis"></param>
		/// <param name="zAxis"></param>
		public	void	ExtractBasis( out Vector3D xAxis, out Vector3D yAxis, out Vector3D zAxis ) {
			xAxis = new Vector3D( this[0], this[1], this[2] );
			yAxis = new Vector3D( this[4], this[5], this[6] );
			zAxis = new Vector3D( this[8], this[9], this[10] );
		}

		// ================================================================================

		/// <summary>
		/// Extract the matrix' current rotation as an axis and a rotation
		/// </summary>
		/// <param name="axis"></param>
		/// <param name="radians"></param>
		public	void	ExtractAxisAngle( out Vector3D axis, out float radians ) {
			Quaternion quat = Quaternion.FromTransform( this );
			quat.GetAxisAngle( out axis, out radians );
		}
		
		/// <summary>
		/// Extract a quaternion that represents the matrix' current rotation
		/// </summary>
		/// <returns></returns>
		public	Quaternion	ExtractRotation() {
			return Quaternion.FromTransform( this );
		}

		/// <summary>
		/// Extract a point that represents the matrix' current translation
		/// </summary>
		/// <returns></returns>
		public	Vector3D	ExtractTranslation() {
			return	Vector3D.FromXYZ( _elements[12], _elements[13], _elements[14] );
		}

		/// <summary>
		/// Extract a point that represents the scale associated with each row
		/// </summary>
		/// <returns></returns>
		public	Vector3D	ExtractScale() {
			Matrix3D mTemp = new Matrix3D( this );

			// undo translation
			mTemp._elements[12] = 0;
			mTemp._elements[13] = 0;
			mTemp._elements[14] = 0;
			
			// undo rotation
			Quaternion quat = Quaternion.FromTransform( this );
			Quaternion quatInverse = quat.GetInverse();
			mTemp.Rotate( quatInverse );

			// all that's left is scale?
			return	Vector3D.FromXYZ( mTemp._elements[0], mTemp._elements[5], mTemp._elements[10] );
		}

		// ================================================================================

		/// <summary>
		/// Add matrix to self.
		/// </summary>
		/// <param name="m"></param>
		public void Add( Matrix3D m ) {
			for( int i = 0; i < 16; i++ ) {
				_elements[i] += m._elements[i];
			}
		}

		/// <summary>
		/// Substract matrix from self.
		/// </summary>
		/// <param name="m"></param>
		public void Substract( Matrix3D m ) {
			for( int i = 0; i < 16; i++ ) {
				_elements[i] -= m._elements[i];
			}
		}
		
		/// <summary>
		/// Rotate matrix around X axis
		/// </summary>
		/// <param name="radians"></param>
		public	void	RotateX( float radians ) {
			Rotate( Vector3D.XAxis, radians );
		}
		/// <summary>
		/// Rotate matrix around Y axis
		/// </summary>
		/// <param name="radians"></param>
		public	void	RotateY( float radians ) {
			Rotate( Vector3D.YAxis, radians );
		}
		/// <summary>
		/// Rotate matrix around Z axis
		/// </summary>
		/// <param name="radians"></param>
		public	void	RotateZ( float radians ) {
			Rotate( Vector3D.ZAxis, radians );
		}

		/// <summary>
		/// rotate matrix around an arbitrary axis
		/// </summary>
		/// <param name="axis"></param>
		/// <param name="radians"></param>
		public	void	Rotate( Vector3D axis, float radians ) {
			Rotate( Quaternion.FromAxisAngle( axis, radians ) );
		}

		/// <summary>
		/// rotate matrix as defined by the quatnerion
		/// </summary>
		/// <param name="quat"></param>
 		public	void	Rotate( Quaternion quat ) {
			Matrix3D mTemp = this * Matrix3D.FromRotation( quat );
			SetToArray( (float[]) mTemp );
		}

		/// <summary>
		/// Scale the matrix rows by given (x,y,z)
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public	void	Scale( float x, float y, float z ) {
			_elements[0] *= x;
			_elements[1] *= x;
			_elements[2] *= x;
			_elements[4] *= y;
			_elements[5] *= y;
			_elements[6] *= y;
			_elements[8]  *= z;
			_elements[9]  *= z;
			_elements[10] *= z;
		}

		/// <summary>
		/// Scale the matrix rows by given point
		/// </summary>
		/// <param name="pt"></param>
		public	void	Scale( Vector3D pt ) {
			Scale( pt.X, pt.Y, pt.Z );
		}

		/// <summary>
		/// Translate matrix by given (x,y,z)
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public	void	Translate( float x, float y, float z ) {
			_elements[12] = _elements[0]*x + _elements[4]*y + _elements[8]*z  + _elements[12];
			_elements[13] = _elements[1]*x + _elements[5]*y + _elements[9]*z  + _elements[13];
			_elements[14] = _elements[2]*x + _elements[6]*y + _elements[10]*z + _elements[14];
		}

		/// <summary>
		/// Translate matrix by given point
		/// </summary>
		/// <param name="pt"></param>
		public	void	Translate( Vector3D pt ) {
			Translate( pt.X, pt.Y, pt.Z );
		}

		/// <summary>
		/// Multiply this matrix by the given matrix
		/// </summary>
		/// <param name="m"></param>
		public	void	Multiply( Matrix3D m ) {
			Matrix3D mTemp = Matrix3D.FromMultiplication( this, m );
			SetToArray( (float[]) mTemp );
		}

		private void Swap( ref float a, ref float b ) {
			float c = a;
			a = b;
			b = c;
		}
		/// <summary>
		/// transpose the matrix
		/// </summary>
 		public	void	Transpose() {
			this.Swap( ref _elements[1],  ref _elements[4]  );
			this.Swap( ref _elements[2],  ref _elements[8]  );
			this.Swap( ref _elements[3],  ref _elements[12] );
			this.Swap( ref _elements[6],  ref _elements[9]  );
			this.Swap( ref _elements[7],  ref _elements[13] );
			this.Swap( ref _elements[11], ref _elements[14] );
		}

		/// <summary>
		/// Get the determinant of the matrix
		/// </summary>
		/// <returns></returns>
		public float GetDeterminant() {
			float det;
			det = _elements[0] * _elements[5] * _elements[10]* _elements[15];
			det+= _elements[4] * _elements[9] * _elements[14]* _elements[3];
			det+= _elements[8] * _elements[13]* _elements[2] * _elements[7];
			det+= _elements[12]* _elements[1] * _elements[6] * _elements[11];

			det-= _elements[3] * _elements[6] * _elements[9] * _elements[12];
			det-= _elements[7] * _elements[10]* _elements[13]* _elements[0];
			det-= _elements[11]* _elements[14]* _elements[1] * _elements[4];
			det-= _elements[15]* _elements[2] * _elements[5] * _elements[8];

			return det;
		}
		/*public float	GetDeterminant() {
			double[] matrix = Utils.create_m4();
			Utils.transform_2_m4( this, matrix );
			return	(float) Utils.m4_det( matrix );
		}*/

		/// <summary>
		/// Create a new matrix that is the inverse of the current object.
		/// </summary>
		/// <returns></returns>
		public Matrix3D	GetInverse() {
			Matrix3D	m = new Matrix3D( this );
			m.Invert();
			return m;
		}

		/// <summary>
		/// Invert the matrix.  Will throw a MatrixNotInvertableException if the determine is 0.
		/// </summary>
		public	void	Invert() {
			double[]	matrix = Utils.create_m4();
			double[]	matrixInverse = Utils.create_m4();

			Utils.transform_2_m4( this, matrix );

			if( Utils.m4_inverse( matrixInverse, matrix ) == 0 ) {
				Debug.WriteLine( "Matrix:" );
				Debug.WriteLine( this.ToString() );
				Debug.WriteLine( "Determinant: " + this.GetDeterminant() );
				throw new MatrixNotInvertableException( "Matrix Not Invertable" );
			}

			Utils.m4_2_transform( matrixInverse, this );
		}

		// ================================================================================

		/// <summary>
		/// Extract the OpenGL compatible array of matrix elements.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
 		public static explicit operator float[]( Matrix3D m ) {
			return	m._elements;
		}

		/// <summary>
		/// Convert quaternion into equivalent rotation matrix
		/// </summary>
		/// <param name="quat"></param>
		/// <returns></returns>
		public static explicit operator Matrix3D( Quaternion quat ) {
			return Matrix3D.FromRotation( quat );
		}
		
		// ================================================================================

		/// <summary>
		/// Multiply two transforms together
		/// </summary>
		/// <param name="m0"></param>
		/// <param name="m1"></param>
		/// <returns></returns>
		static public Matrix3D operator*( Matrix3D m0, Matrix3D m1 ) {
			return	Matrix3D.FromMultiplication( m0, m1 );
		}

		// ================================================================================

		/// <summary>
		/// A user friendly multi-line string representation of the object
		/// </summary>
		/// <returns></returns>
		public override string	ToString() {				
			String str = null;
			str += String.Format( "[ rs{0}, rs{1}, rs{2}, t{3} ]\n", _elements[0], _elements[4], _elements[8], _elements[12] );
			str += String.Format( "[ rs{0}, rs{1}, rs{2}, t{3} ]\n", _elements[1], _elements[5], _elements[9], _elements[13] );
			str += String.Format( "[ rs{0}, rs{1}, rs{2}, t{3} ]\n", _elements[2], _elements[6], _elements[10], _elements[14] );
			str += String.Format( "[ z{0}, z{1}, z{2}, o{3} ]", _elements[3], _elements[7], _elements[11], _elements[15] );
			return	str;
		}

		// ================================================================================
		// ================================================================================

		/// <summary>
		/// functions for inverse, determinants, etc...
		/// Based on the Matrix and Quaternion FAQ:
		/// http://www.cs.ualberta.ca/~andreas/math/matrfaq_latest.html
		/// </summary>
		internal class Utils {

			/// <summary>
			/// create a 3x3 matrix
			/// </summary>
			/// <returns></returns>
			static public double[] create_m3() {
				double[] m = new double[9];
				m3_identity( m );
				return m;
			}

			/// <summary>
			/// create a 4x4 matrix
			/// </summary>
			/// <returns></returns> 
			static public double[] create_m4() {
				double[] m = new double[16];
				m4_identity( m );
				return m;
			}

			/// <summary>
			/// set 3x3 matrix to identity
			/// </summary>
			/// <param name="m"></param> 
			static public  void	m3_identity( double[] m ) {
				m[0] = m[4] = m[8] = 1;
				m[1] = m[2] = m[3] = m[5] = m[6] = m[7] = 0;
			}

			/// <summary>
			/// set 4x4 matrix to identity
			/// </summary>
			/// <param name="m"></param>
			static public  void	m4_identity( double[] m ) {
				m[0] = m[5] = m[10] = m[15] = 1;
				m[1] = m[2] = m[3] = m[4] = m[6] = m[7] = 
					m[8] = m[9] = m[11] = m[12] = m[13] = m[14] = 0;
			}

			/// <summary>
			/// calculate determinant of 3x3 matrix
			/// </summary>
			/// <param name="mat"></param>
			/// <returns></returns>
			static public  double m3_det( double[] mat ) {
				return	mat[0] * ( mat[4]*mat[8] - mat[7]*mat[5] )
					- mat[1] * ( mat[3]*mat[8] - mat[6]*mat[5] )
					+ mat[2] * ( mat[3]*mat[7] - mat[6]*mat[4] );
			}

			/// <summary>
			/// extract a 3x3 sub-matrix from a 4x4 matrix
			/// </summary>
			/// <param name="mr"></param>
			/// <param name="mb"></param>
			/// <param name="i"></param>
			/// <param name="j"></param>
			static public  void	m4_submat( double[] mr, double[] mb, int i, int j ) {
				for( int di = 0; di < 3; di ++ ) {
					for( int dj = 0; dj < 3; dj ++ ) {
						int si = di + ( ( di >= i ) ? 1 : 0 );
						int sj = dj + ( ( dj >= j ) ? 1 : 0 );
						mb[di * 3 + dj] = mr[si * 4 + sj];
					}
				}
			}

			/// <summary>
			/// calculate determinant of 4x4 matrix
			/// </summary>
			/// <param name="mr"></param>
			/// <returns></returns> 
			static public  double m4_det( double[] mr ) {
				double	det, result = 0, i = 1;
				double[] msub3 = create_m3();
				int     n;

				for ( n = 0; n < 4; n++, i *= -1 ) {
					m4_submat( mr, msub3, 0, n );

					det     = m3_det( msub3 );
					result += mr[n] * det * i;
				}

				return result;
			}

			/// <summary>
			/// create a 4x4 matrix from a Matrix3D
			/// </summary>
			/// <param name="m"></param>
			/// <param name="mx"></param>
			static public  void transform_2_m4( Matrix3D m, double[] mx ) {
				for( int i = 0; i < 4; i ++ ) {
					for( int j = 0; j < 4; j ++ ) {
						mx[i * 4 + j] = m[i + j * 4];
					}
				}
			}

			/// <summary>
			/// set Matrix3D to a 4x4 matrix
			/// </summary>
			/// <param name="mx"></param>
			/// <param name="m"></param>
			static public  void	m4_2_transform( double[] mx, Matrix3D m ) {
				for( int i = 0; i < 4; i ++ ) {
					for( int j = 0; j < 4; j ++ ) {
						m[i + j * 4] = (float) mx[i * 4 + j];
					}
				}
			}

			/// <summary>
			/// calculate inverse of 4x4 matrix, returns 1 if successful
			/// </summary>
			/// <param name="mr"></param>
			/// <param name="ma"></param>
			/// <returns></returns> 
			static public  int m4_inverse( double[] mr, double[] ma ) {
				double  mdet = m4_det( ma );
				double[] mtemp = create_m3();
				int     i, j, sign;

				if ( Math.Abs( mdet ) < 0.000005 )
					return 0;

				for ( i = 0; i < 4; i++ ) {
					for ( j = 0; j < 4; j++ ) {
						sign = ((( i + j ) % 2 ) == 0 ) ? 1 : -1;
						m4_submat( ma, mtemp, i, j );
						mr[i+j*4] = ( m3_det( mtemp ) * sign ) / mdet;
					}
				}

				return( 1 );
			}

			/// <summary>
			/// converts 3x3 matrix to a string
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			static public  string m3_tostring( double[] m ) {
				string str = "";
				for( int i = 0; i < 3; i ++ ) {
					int offset = i * 3;
					str += String.Format( "[ {0} {1} {2} ]\n", m[offset+0], m[offset+1], m[offset+2] );
				}
				return str;
			}

			/// <summary>
			/// converts 4x4 matrix to a string
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			static public  string m4_tostring( double[] m ) {
				string str = "";
				for( int i = 0; i < 4; i ++ ) {
					int offset = i * 4;
					str += String.Format( "[ {0} {1} {2} {3} ]\n", m[offset+0], m[offset+1], m[offset+2], m[offset+3] );
				}
				return str;
			}
		}	
		
		// ================================================================================
		// ================================================================================

	}

}
