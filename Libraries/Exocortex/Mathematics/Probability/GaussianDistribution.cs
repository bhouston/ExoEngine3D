using System;
using System.Diagnostics;

namespace Exocortex.Mathematics.Probability {
	/// <summary>
	/// Summary description for GaussianDistribution.
	/// </summary>
	public class GaussianDistribution {
	
		//===============================================================================

		static private double	s_Sqrt2PI	= Math.Sqrt( 2 * Math2.PI );

		//===============================================================================
		
		static public double	GetDensity( double x, double stdDev ) {
			return	( ( 1 / ( s_Sqrt2PI * stdDev ) ) * Math.Exp( - ( x*x ) / ( 2*stdDev*stdDev ) ) );
		}

		//===============================================================================

		private	double		_Sqrt2PIStdDevRecip;
		private	double		_Neg2SqrtStdDevRecip;
		private	double		_stdDev;

		private	int			_lookupLength = -1;
		private double[]	_lookupTable = null;
		private	double		_lookupScale = 1;
		private float[]		_lookupTableF = null;
		private	float		_lookupScaleF = 1;

		public GaussianDistribution( double stdDev ) {
			Setup( stdDev, 2048, (int) Math.Ceiling( stdDev * 4 ) );
		}

		public GaussianDistribution( double stdDev, int lookupLength ) {
			Setup( stdDev, lookupLength, (int) Math.Ceiling( stdDev * 4 ) );
		}

		public GaussianDistribution( double stdDev, int lookupLength, int maxValue ) {
			Setup( stdDev, lookupLength, maxValue );
		}

		protected void Setup( double stdDev, int lookupLength, int maxValue ) {
			Debug.Assert( stdDev != 0 );
			Debug.Assert( lookupLength >= 1 );

			//Debug2.WriteVar( "stdDev", stdDev );
			//Debug2.WriteVar( "lookupLength", lookupLength );

			_stdDev					= stdDev;
			_Sqrt2PIStdDevRecip		= 1.0 / ( s_Sqrt2PI * stdDev );
			_Neg2SqrtStdDevRecip	= - 1.0 / ( 2 * stdDev * stdDev );

			_lookupLength		= lookupLength;
			_lookupTable		= new double[ _lookupLength ];
			_lookupTableF		= new float[ _lookupLength ];
			_lookupScale		= ((double) _lookupLength ) / ( maxValue );
			_lookupScaleF		= (float) _lookupScale;

			for( int i = 0; i < _lookupLength; i ++ ) {
				double x = (double) i / _lookupScale;
				double density = this.CalculateDensity( x );
				_lookupTable[ i ] = density;
				_lookupTableF[ i ] = (float) density;
			}
		}

		public double	CalculateDensity( double x ) {
			return	_Sqrt2PIStdDevRecip * Math.Exp( x * x * _Neg2SqrtStdDevRecip );
		}
		public float	CalculateDensityF( float x ) {
			return	(float) ( _Sqrt2PIStdDevRecip * Math.Exp( x * x * _Neg2SqrtStdDevRecip ) );
		}

		public double	LookupDensity( double x ) {
			int i = Math.Abs( (int) ( x * _lookupScale ) );
			if( i < _lookupLength ) {
				return	_lookupTable[ i ];
			}
			return	0;
		}

		public float	LookupDensityF( float x ) {
			int i = Math.Abs( (int) ( x * _lookupScaleF ) );
			if( i < _lookupLength ) {
				return	_lookupTableF[ i ];
			}
			return	0;
		}

	}
}
