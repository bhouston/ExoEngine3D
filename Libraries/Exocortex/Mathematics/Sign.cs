// Exocortex Technologies
// http://www.exocortex.org
// Copyright (c) 2001, 2002 Ben Houston (ben@exocortex.org).  All Rights Reserved.


using System;
using System.Diagnostics;

namespace Exocortex.Mathematics
{
	/*public class Sign {

		static public Sign Undefined	= new Sign( -2 );
		static public Sign Negative	= new Sign( -1 );
		static public Sign Zero		= new Sign( 0 );
		static public Sign Positive	= new Sign( 1 );
		static public Sign Mixed		= new Sign( 2 );

		public override string	ToString() {
			switch( this._state ) {
			case -2:
				return "Undefined";
			case -1:
				return "Negative";
			case 0:
				return "Zero";
			case 1:
				return "Positive";
			case 2:
				return "Mixed";
			}
			Debug.Assert( false );
			return null;
		}

		static public Sign FromSingle( float f ) {
			if( f > Math2.EpsilonF ) {
				return Sign.Positive;
			}
			if( f < -Math2.EpsilonF ) {
				return Sign.Negative;
			}
			return Sign.Zero;
		}
		
		public Sign() {
			_state = -2;
		}

		protected Sign( int state ) {
			_state = state;
		}

		protected int _state;

		public override bool Equals( object o ) {
			if( o is Sign ) {
				Sign s = (Sign) o;
				return ( s._state == this._state );
			}
			return base.Equals( o );
		}
		public override int GetHashCode() {
			return	_state;
		}

		static public explicit operator int ( Sign sign ) {
			return	sign._state;
		}

		static public bool operator==( Sign a, Sign b ) {
			return	( a._state == b._state );
		}

		static public bool operator!=( Sign a, Sign b ) {
			return	( a._state != b._state );
		}

		static public bool operator==( Sign a, int i ) {
			if( a._state == -2 || a._state == 2 ) {
				return false;
			}
			return	( a._state == i );
		}

		static public bool operator!=( Sign a, int i ) {
			if( a._state == -2 || a._state == 2 ) {
				return false;
			}
			return	( a._state != i );
		}

		static public bool operator>( Sign a, int i ) {
			if( a._state == -2 || a._state == 2 ) {
				return false;
			}
			return ( a._state > i );
		}
		static public bool operator>=( Sign a, int i ) {
			if( a._state == -2 || a._state == 2 ) {
				return false;
			}
			return ( a._state >= i );
		}
		static public bool operator<( Sign a, int i ) {
			if( a._state == -2 || a._state == 2 ) {
				return false;
			}
			return ( a._state < i );
		}
		static public bool operator<=( Sign a, int i ) {
			if( a._state == -2 || a._state == 2 ) {
				return false;
			}
			return ( a._state <= i );
		}

		static public Sign operator+( Sign a, Sign b ) {
			if( a._state == -2 ) {
				return new Sign( b._state );
			}
			if( b._state == -2 ) {
				return new Sign( a._state );
			}

			if( a._state == 0 ) {
				return new Sign( b._state );
			}
			if( b._state == 0 ) {
				return new Sign( a._state );
			}

			if( a._state == b._state ) {
				return new Sign( a._state );
			}

			return	Sign.Mixed;
		}

	}*/ 
}
