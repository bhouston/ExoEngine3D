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
using System.Text;

namespace Exocortex {

	public class Math2 {
		
		//===============================================================================

		// Prevent from begin created
		private Math2() {
		}
		
		//===============================================================================

		/*public static Complex Pow( Complex cBase, float fExponent ) {
			return Complex.FromPolarCoords( (float) Math.Pow( cBase.Length, fExponent ), cBase.Angle * fExponent );
		}
		public static Complex Sqrt( Complex cBase ) {
			return Pow( cBase, 0.5f );
		}  */

		//===============================================================================

		// pi, a useful mathematical constant
		public const double	PI		= System.Math.PI;
		public const float	PIF		= (float)( System.Math.PI );

		// 2pi, a useful mathematical constant
		public const double	PI2		= System.Math.PI * 2;
		public const float	PI2F	= (float)( System.Math.PI * 2 );

		// epsilon, a fairly small value
		public const double	Epsilon		= 1.0e-07;
		public const float	EpsilonF	= 1.0e-04F;

		//===============================================================================

		private const double	cDegreesToRadian		= ( Math2.PI / 180.0 ); 
		private const double	cRadianToDegrees		= ( 180.0 / Math2.PI ); 

		/// <summary>
		/// Convert degrees (base 360) into radians (base 2pi)
		/// </summary>
		/// <param name="degrees"></param>
		/// <returns></returns>
		static public double ToRadians( double degrees ) {
			return	degrees * cDegreesToRadian;
		}

		/// <summary>
		/// Convert radians (base 2pi) into degrees (base 360) 
		/// </summary>
		/// <param name="radians"></param>
		/// <returns></returns>
		static public double ToDegrees( double radians ) {
			return	radians * cRadianToDegrees;
		}

		/// <summary>
		/// constrain radian to range [ 0, 2pi )
		/// </summary>
		/// <param name="radians"></param>
		/// <returns></returns>
		static public double ToUnitRadian( double radian ) {
			if( radian <= - Math2.PI ) {
				return	radian + Math2.PI2 * Math.Floor( - ( radian - Math2.PI ) / Math2.PI2 );
			}
			else if( radian > Math2.PI ) {
				return	radian - Math2.PI2 * ( Math.Ceiling( ( radian + Math2.PI ) / Math2.PI2 ) - 1 );
			}
			if( - Math2.PI < radian && radian <= Math2.PI ) {
				return radian;
			}
			throw new ArithmeticException( "invalid radian value: " + radian.ToString() );
		}

		/// <summary>
		/// Convert value to a string representation and constain to a unit radian
		/// </summary>
		/// <param name="radians"></param>
		/// <returns></returns>
		static public string RadiansToString( double radians ) {
			return ( Math2.ToUnitRadian( radians ) / Math2.PI ).ToString() + "PI";
		}

		/// <summary>
		/// Convert value to a string representation and constain to a unit radian
		/// </summary>
		/// <param name="radians"></param>
		/// <param name="significantDigits"></param>
		/// <returns></returns>
		static public string RadiansToString( double radians, int significantDigits ) {
			return Math2.ToString( ToUnitRadian( radians ) / Math2.PI, significantDigits ) + "PI";
		}

		//===============================================================================

		static private Random	_random = new Random();

		static public int		RandomInt() {
			return	_random.Next();
		}
		static public int		RandomInt( int maxValue ) {
			return	_random.Next( maxValue );
		}
		static public int		RandomInt( int minValue, int maxValue ) {
			Debug.Assert( minValue <= maxValue );
			return	_random.Next( maxValue - minValue ) + minValue;
		}

		static public float		RandomSingle() {
			return	(float) _random.NextDouble();
		}
		static public float		RandomSingle( float maxValue ) {
			return	((float) _random.NextDouble() ) * maxValue;
		}
		static public float		RandomSingle( float minValue, float maxValue ) {
			Debug.Assert( minValue <= maxValue );
			return	((float) _random.NextDouble() ) * ( maxValue - minValue ) + minValue;
		}

		static public double	RandomDouble() {
			return	_random.NextDouble();
		}
		static public double		RandomDouble( double maxValue ) {
			return	_random.NextDouble() * maxValue;
		}
		static public double		RandomDouble( double minValue, double maxValue ) {
			Debug.Assert( minValue <= maxValue );
			return	_random.NextDouble() * ( maxValue - minValue ) + minValue;
		}
		
		// Generates numbers from a gaussian dist with mean = 0, std = 1
		static public double GaussRand() {
			double x1, x2, w;

			do {
				x1 = (2.0 * _random.NextDouble() ) - 1;
				x2 = (2.0 * _random.NextDouble() ) - 1;
				w = x1*x1 + x2*x2;
			}
			while (w >= 1.0);

			w = Math.Sqrt(-2*Math.Log(w)/w);
			return(w*x2);
		}

		//===============================================================================

		static public	double	Sigmoid( double delta ) {
			return	1.0 / ( 1.0 - Math.Exp( - delta ) );
		}
		static public	double	Sigmoid( double delta, double curveBias ) {
			return	1.0 / ( 1.0 - Math.Exp(  - delta / curveBias ) );
		}
		
		//===============================================================================

		static public char		ToDigit( int digitIndex, int digitBase ) {
			// ensure the base is valid
			if( ! ( digitBase >= 2 ) ) {
				throw new ArgumentOutOfRangeException( "digitBase", digitBase,
					"digitBase must be greater or equal to 2." );
			}
			// ensure the index of the digit is valid
			if( ! ( 0 <= digitIndex && digitIndex < digitBase ) ) {
				throw new ArgumentOutOfRangeException( "digitIndex", digitIndex,
					"digitIndex must be greater or equal to 0 and less than digitBase." );
			}

			if( digitIndex < 10 ) {
				return (char)( '0' + digitIndex );
			}
			else {
				return (char)( 'A' + digitIndex - 10 );
			}
		}

		static public string ToDigitString( double d, int digitBase ) {
			// ensure the base is valid
			if( ! ( digitBase >= 2 ) ) {
				throw new ArgumentOutOfRangeException( "digitBase", digitBase,
					"digitBase must be greater or equal to 2." );
			}

			StringBuilder digitString = new StringBuilder();
						
			if( d < 0 ) {
				digitString.Append( '-' );
			}

			double	dRemainder = Math.Abs( d );
			int		digitPlace = (int) Math.Floor( Math.Log( dRemainder, digitBase ) );
			
			while( digitPlace >= 0 || d > 0 ) {
				if( digitPlace == -1 ) {
					digitString.Append( '.' );
				}
				
				double	digitMultiplier = Math.Pow( digitBase, digitPlace );
				int		digitIndex = (int) Math.Floor( dRemainder / digitMultiplier );
				dRemainder -= ( digitIndex * digitMultiplier );

				digitString.Append( Math2.ToDigit( digitIndex, digitBase ) );

				Debug.Assert( d < digitIndex );

				digitPlace --;
			}

			return digitString.ToString();
		}

		//===============================================================================
		
		static public double Round( double d, int digits ) {
			// scale number such that specified rounding precision is at the origin,
			//     round to the nearest whole number and then remove scaling factor.
			double	scale = Math.Pow( 10, digits );
			return	Math.Round( d * scale ) / scale;
		}

		static public double Round( double d, int digits, int digitBase ) {
			// ensure the base is valid
			if( ! ( digitBase >= 2 ) ) {
				throw new ArgumentOutOfRangeException( "digitBase", digitBase,
					"digitBase must be greater or equal to 2." );
			}
			// scale number such that specified rounding precision is at the origin,
			//     round to the nearest whole number and then remove scaling factor.
			double	scale = Math.Pow( digitBase, digits );
			return	Math.Round( d * scale ) / scale;
		}

		//===============================================================================

		static public int	PositiveModulus( int numerator, int denominator ) {
			// validate denominator
			if( ! ( denominator > 0 ) ) {
				throw new ArgumentOutOfRangeException( "denominator", denominator, "Denominator must be positive." );
			}

			if( numerator < 0 ) {
				return	denominator - ( Math.Abs( numerator ) % denominator );
			}
			return	( numerator % denominator );
		}

		//===============================================================================

		static public int		RoundToBase( int a, int b ) {
			return (int) Math2.RoundToBase( (double)a, b );
		}
		static public float		RoundToBase( float a, int b ) {
			return (float) Math2.RoundToBase( (double)a, b );
		}
		static public double	RoundToBase( double a, int b ) {
			// ensure the base is valid
			if( ! ( b >= 2 ) ) {
				throw new ArgumentOutOfRangeException( "b", b,
					"digitBase must be greater or equal to 2." );
			}

			return (double) Math.Pow( b, Math.Round( Math.Log( a, b ) ) );
		}

		static public int		FloorToBase( int a, int b ) {
			return (int) Math2.FloorToBase( (double) a, b );
		}
		static public float		FloorToBase( float a, int b ) {
			return (float) Math2.FloorToBase( (double) a, b );
		}
		static public double	FloorToBase( double a, int b ) {
			// ensure the base is valid
			if( ! ( b >= 2 ) ) {
				throw new ArgumentOutOfRangeException( "b", b,
					"digitBase must be greater or equal to 2." );
			}
			
			return (double) Math.Pow( b, Math.Floor( Math.Log( a, b ) ) );
		}

		static public int		CeilingToBase( int a, int b ) {
			return (int) Math2.CeilingToBase( (double) a, b );
		}
		static public float		CeilingToBase( float a, int b ) {
			return (float) Math2.CeilingToBase( (double) a, b );
		}
		static public double	CeilingToBase( double a, int b ) {
			// ensure the base is valid
			if( ! ( b >= 2 ) ) {
				throw new ArgumentOutOfRangeException( "b", b,
					"digitBase must be greater or equal to 2." );
			}
			
			return (double) Math.Pow( b, Math.Ceiling( Math.Log( a, b ) ) );
		}

		static public bool		IsPowerOfBase( int number, int b ) {
			// ensure the base is valid
			if( ! ( b >= 2 ) ) {
				throw new ArgumentOutOfRangeException( "b", b,
					"digitBase must be greater or equal to 2." );
			}
			
			return	( (double) number == Math2.RoundToBase( (double) number, b ) );
		}

		static public bool	IsPowerOf2( int x ) {
			return	( x == Math2.Pow2( Math2.Log2( x ) ) );
		}
		static public int	Pow2( int exponent ) {
			if( exponent >= 0 && exponent < 31 ) {
				return	1 << exponent;
			}
			return	0;
		}
		static public int	Log2( int x ) {
			if( x <= 65536 ) {
				if( x <= 256 ) {
					if( x <= 16 ) {
						if( x <= 4 ) {	
							if( x <= 2 ) {
								if( x <= 1 ) {
									return	0;
								}
								return	1;	
							}
							return	2;				
						}
						if( x <= 8 )
							return	3;			
						return	4;				
					}
					if( x <= 64 ) {
						if( x <= 32 )
							return	5;	
						return	6;				
					}
					if( x <= 128 )
						return	7;		
					return	8;				
				}
				if( x <= 4096 ) {	
					if( x <= 1024 ) {	
						if( x <= 512 )
							return	9;			
						return	10;				
					}
					if( x <= 2048 )
						return	11;			
					return	12;				
				}
				if( x <= 16384 ) {
					if( x <= 8192 )
						return	13;			
					return	14;				
				}
				if( x <= 32768 )
					return	15;	
				return	16;	
			}
			if( x <= 16777216 ) {
				if( x <= 1048576 ) {
					if( x <= 262144 ) {	
						if( x <= 131072 )
							return	17;			
						return	18;				
					}
					if( x <= 524288 )
						return	19;			
					return	20;				
				}
				if( x <= 4194304 ) {
					if( x <= 2097152 )
						return	21;	
					return	22;				
				}
				if( x <= 8388608 )
					return	23;		
				return	24;				
			}
			if( x <= 268435456 ) {	
				if( x <= 67108864 ) {	
					if( x <= 33554432 )
						return	25;			
					return	26;				
				}
				if( x <= 134217728 )
					return	27;			
				return	28;				
			}
			if( x <= 1073741824 ) {
				if( x <= 536870912 )
					return	29;			
				return	30;				
			}
			//	since int is unsigned it can never be higher than 2,147,483,647
			//	if( x <= 2147483648 )
			//		return	31;	
			//	return	32;	
			return	31;
		}

		//===============================================================================

		static public string RadianToString( double radians ) {
			return ( Math2.ToUnitRadian( radians ) / Math2.PI ).ToString() + " " + ((char) 0x03C0);
		}
		static public string RadianToString( double radians, int significantDigits ) {
			return Math2.ToString( ToUnitRadian( radians ) / Math2.PI, significantDigits ) + " " + ((char) 0x03C0);
		}


		/// <summary>
		/// Convert value to a string with a given number of significant digits
		/// </summary>
		/// <param name="f"></param>
		/// <param name="significantDigits"></param>
		/// <returns></returns>
		static public string ToString( float f, int significantDigits ) {
			double sd = Math.Pow( 10, significantDigits );
			return	( Math.Round( f * sd ) / sd ).ToString();
		}

		/// <summary>
		/// Convert value to a string with a given number of significant digits
		/// </summary>
		/// <param name="d"></param>
		/// <param name="significantDigits"></param>
		/// <returns></returns>
		static public string ToString( double d, int significantDigits ) {
			double sd = Math.Pow( 10, significantDigits );
			return	( Math.Round( d * sd ) / sd ).ToString();
		}

		//===============================================================================

		static public int	Factorial( int number ) {
			if( number <= 0 ) {
				throw new ArithmeticException( "can not calc factorial of a number <= 0: " + number.ToString() );
			}
			int result = 1;
			for( int i = 2; i <= number; i ++ ) {
				result *= i;
			}
			return	result;
		}

		//===============================================================================

		/// <summary>
		///  clamp value to range [minimum, maximum]
		/// </summary>
		/// <param name="value"></param>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		/// <returns></returns>
		static public float Clamp( float value, float minimum, float maximum ) {
			if( value > maximum ) {
				return maximum;
			}
			if( value < minimum ) {
				return minimum;
			}
			if( minimum < value && value < maximum ) {
				return value;
			}
			throw new ArithmeticException( "can not clamp invalid number: " + value.ToString() );
		}

		/// <summary>
		///  clamp value to range [minimum, maximum]
		/// </summary>
		/// <param name="value"></param>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		/// <returns></returns>
		static public double Clamp( double value, double minimum, double maximum ) {
			if( value > maximum ) {
				return maximum;
			}
			if( value < minimum ) {
				return minimum;
			}
			if( minimum < value && value < maximum ) {
				return value;
			}
			throw new ArithmeticException( "can not clamp invalid number: " + value.ToString() );
		}

		/// <summary>
		///  clamp value to range [minimum, maximum]
		/// </summary>
		/// <param name="value"></param>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		/// <returns></returns>
		static public int Clamp( int value, int minimum, int maximum ) {
			if( value > maximum ) {
				return maximum;
			}
			if( value < minimum ) {
				return minimum;
			}
			if( minimum < value && value < maximum ) {
				return value;
			}
			throw new ArithmeticException( "can not clamp invalid number: " + value.ToString() );
		}

		//===============================================================================

		/// <summary>
		/// Swap two variables
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		static public void Swap( ref int a, ref int b ) {
			int temp = a;
			a = b;
			b = temp;
		}

		/// <summary>
		/// Swap two variables
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		static public void Swap( ref float a, ref float b ) {
			float temp = a;
			a = b;
			b = temp;
		}
		
		/// <summary>
		/// Swap two variables
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		static public void Swap( ref double a, ref double b ) {
			double temp = a;
			a = b;
			b = temp;
		}
		
		/// <summary>
		/// Swap two variables
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		static public void Swap( ref object a, ref object b ) {
			object temp = a;
			a = b;
			b = temp;
		}

		//===============================================================================

		static public float		Distance( int x1, int y1, int x2, int y2 ) {
			int dX = x2 - x1;
			int dY = y2 - y1;
			return	(float)	Math.Sqrt( dX*dX + dY*dY );
		}

		static public float		Distance( float x1, float y1, float x2, float y2 ) {
			float dX = x2 - x1;
			float dY = y2 - y1;
			return	(float)	Math.Sqrt( dX*dX + dY*dY );
		}

		static public float		Distance( int x1, int y1, int z1, int x2, int y2, int z2 ) {
			int dX = x2 - x1;
			int dY = y2 - y1;
			int dZ = z2 - z1;
			return	(float)	Math.Sqrt( dX*dX + dY*dY + dZ*dZ );
		}

		static public float		Distance( float x1, float y1, float z1, float x2, float y2, float z2 ) {
			float dX = x2 - x1;
			float dY = y2 - y1;
			float dZ = z2 - z1;
			return	(float)	Math.Sqrt( dX*dX + dY*dY + dZ*dZ );
		}
		
		//===============================================================================

	}
}
