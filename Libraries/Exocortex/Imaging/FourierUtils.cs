using System;
using System.Diagnostics;

namespace Exocortex.DSP {

	/// <summary>
	/// Various utility functions for doing fourier transforms.
	/// Ben Houston (ben@exocortex.org)
	/// March 7, 2002
	/// </summary>
	public class FourierUtils {

		//-------------------------------------------------------------------------------------

		private FourierUtils() {
		}

		//-------------------------------------------------------------------------------------
		
		private const int	cMaxLength	= 4096;
		private const int	cMinLength	= 1;

		private const int	cMaxBits	= 12;
		private const int	cMinBits	= 0;
	
		//-------------------------------------------------------------------------------------
		
		static private float[][]	_complexCoeffsF	= new float[ cMaxLength ][];
		static public float[]	GetComplexCoeffsF( int period, int length ) {
			Debug.Assert( period >= 0, period.ToString() );
			Debug.Assert( length >= cMinLength, length.ToString() );
			Debug.Assert( length < cMaxLength, length.ToString() );
			//Debug.Assert( FourierUtils.IsPowerOf2( length ) == true, length.ToString() );
		
			if( ( _complexCoeffsF[ period ] == null ) ||
				( _complexCoeffsF[ period ].Length < ( length * 2 ) ) ) {
				float[] data = new float[ length * 2 ];
				for( int i = 0; i < length; i ++ ) {
					double theta = Math.PI * 2 * i * period / length;
					data[ i*2 ]		= (float) Math.Cos( theta );
					data[ i*2 + 1 ]	= (float) Math.Sin( theta );
				}
				_complexCoeffsF[ period ] = data;
			}

			Debug.Assert( _complexCoeffsF[ period ].Length >= length*2 );
			return	_complexCoeffsF[ period ];
		}
		static public float[]	GetComplexCoeffsF( int period ) {
			return	GetComplexCoeffsF( period, period );
		}

		static private double[][]	_complexCoeffs	= new double[ cMaxLength ][];
		static public double[]	GetComplexCoeffs( int period, int length ) {
			Debug.Assert( period >= 0 );
			Debug.Assert( length >= cMinLength );
			Debug.Assert( length < cMaxLength );
			//Debug.Assert( FourierUtils.IsPowerOf2( length ) == true );
		
			if( ( _complexCoeffs[ period ] == null ) ||
				( _complexCoeffs[ period ].Length < ( length * 2 ) ) ) {
				double[] data = new double[ length * 2 ];
				for( int i = 0; i < length; i ++ ) {
					double theta = Math.PI * 2 * i * period / length;
					data[ i*2 ]		= (double) Math.Cos( theta );
					data[ i*2 + 1 ]	= (double) Math.Sin( theta );
				}
				_complexCoeffs[ period ] = data;
			}

			Debug.Assert( _complexCoeffs[ period ].Length >= length*2 );
			return	_complexCoeffs[ period ];
		}
		static public double[]	GetComplexCoeffs( int period ) {
			return	GetComplexCoeffs( period, period );
		}

		//-------------------------------------------------------------------------------------
		//-------------------------------------------------------------------------------------

		static public void Swap( ref float a, ref float b ) {
			float temp = a;
			a = b;
			b = temp;
		}
		static public void Swap( ref double a, ref double b ) {
			double temp = a;
			a = b;
			b = temp;
		}
		static public void Swap( ref ComplexF a, ref ComplexF b ) {
			ComplexF temp = a;
			a = b;
			b = temp;
		}
		static public void Swap( ref Complex a, ref Complex b ) {
			Complex temp = a;
			a = b;
			b = temp;
		}
		
		//-------------------------------------------------------------------------------------
		//-------------------------------------------------------------------------------------

		static public int	ReverseBits( int index, int numberOfBits ) {
			Debug.Assert( numberOfBits >= cMinBits );
			Debug.Assert( numberOfBits <= cMaxBits );

			int reversedIndex = 0;
			for( int i = 0; i < numberOfBits; i ++ ) {
				reversedIndex = ( reversedIndex << 1 ) | ( index & 1 );
				index = ( index >> 1 );
			}
			return reversedIndex;
		}

		//-------------------------------------------------------------------------------------
		
		static private int[][]	_reversedBits	= new int[ cMaxBits ][];
		static public int[]		GetReversedBits( int numberOfBits ) {
			Debug.Assert( numberOfBits >= cMinBits );
			Debug.Assert( numberOfBits <= cMaxBits );
			if( _reversedBits[ numberOfBits - 1 ] == null ) {
				int		maxBits = FourierUtils.Pow2( numberOfBits );
				int[]	reversedBits = new int[ maxBits ];
				for( int i = 0; i < maxBits; i ++ ) {
					int oldBits = i;
					int newBits = 0;
					for( int j = 0; j < numberOfBits; j ++ ) {
						newBits = ( newBits << 1 ) | ( oldBits & 1 );
						oldBits = ( oldBits >> 1 );
					}
					reversedBits[ i ] = newBits;
				}
				_reversedBits[ numberOfBits - 1 ] = reversedBits;
			}
			return	_reversedBits[ numberOfBits - 1 ];
		}

		//-------------------------------------------------------------------------------------

		static public void ReorderArray( float[] data ) {
			Debug.Assert( data != null );

			int length = data.Length / 2;
			
			Debug.Assert( FourierUtils.IsPowerOf2( length ) == true );
			Debug.Assert( length >= cMinLength );
			Debug.Assert( length <= cMaxLength );

			int[] reversedBits = FourierUtils.GetReversedBits( FourierUtils.Log2( length ) );
			for( int i = 0; i < length; i ++ ) {
				int swap = reversedBits[ i ];
				if( swap > i ) {
					FourierUtils.Swap( ref data[ (i<<1) ], ref data[ (swap<<1) ] );
					FourierUtils.Swap( ref data[ (i<<1) + 1 ], ref data[ (swap<<1) + 1 ] );
				}
			}
		}

		static public void ReorderArray( double[] data ) {
			Debug.Assert( data != null );

			int length = data.Length / 2;
			
			Debug.Assert( FourierUtils.IsPowerOf2( length ) == true );
			Debug.Assert( length >= cMinLength );
			Debug.Assert( length <= cMaxLength );

			int[] reversedBits = FourierUtils.GetReversedBits( FourierUtils.Log2( length ) );
			for( int i = 0; i < length; i ++ ) {
				int swap = reversedBits[ i ];
				if( swap > i ) {
					FourierUtils.Swap( ref data[ i<<1 ], ref data[ swap<<1 ] );
					FourierUtils.Swap( ref data[ i<<1 + 1 ], ref data[ swap<<1 + 1 ] );
				}
			}
		}

		static public void ReorderArray( Complex[] data ) {
			Debug.Assert( data != null );
	
			int length = data.Length;
			
			Debug.Assert( FourierUtils.IsPowerOf2( length ) == true );
			Debug.Assert( length >= cMinLength );
			Debug.Assert( length <= cMaxLength );
			
			int[] reversedBits = FourierUtils.GetReversedBits( FourierUtils.Log2( length ) );
			for( int i = 0; i < length; i ++ ) {
				int swap = reversedBits[ i ];
				if( swap > i ) {
					Complex temp = data[ i ];
					data[ i ] = data[ swap ];
					data[ swap ] = temp;
				}
			}
		}

		static public void ReorderArray( ComplexF[] data ) {
			Debug.Assert( data != null );

			int length = data.Length;
			
			Debug.Assert( FourierUtils.IsPowerOf2( length ) == true );
			Debug.Assert( length >= cMinLength );
			Debug.Assert( length <= cMaxLength );

			int[] reversedBits = FourierUtils.GetReversedBits( FourierUtils.Log2( length ) );
			for( int i = 0; i < length; i ++ ) {
				int swap = reversedBits[ i ];
				if( swap > i ) {
					ComplexF temp = data[ i ];
					data[ i ] = data[ swap ];
					data[ swap ] = temp;
				}
			}
		}

		//-------------------------------------------------------------------------------------

		static public bool	IsPowerOf2( int x ) {
			return	( x == Pow2( Log2( x ) ) );
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

		//-------------------------------------------------------------------------------------

	}
}
