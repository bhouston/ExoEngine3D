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
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

//using Exocortex.SignalProcessing;

namespace Exocortex.Imaging {

	/// <summary>
	/// Summary description for RGBA.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct RGBA {

		//------------------------------------------------------------------------------------

		public	byte	Red;
		public	byte	Green;
		public	byte	Blue;
		public	byte	Alpha;

		//------------------------------------------------------------------------------------

		static public RGBA FromRGB( byte red, byte green, byte blue ) {
			RGBA rgba;
			rgba.Red	= red;
			rgba.Green	= green;
			rgba.Blue	= blue;
			rgba.Alpha	= 255;
			return	rgba;
		}
		static public RGBA FromRGB( int red, int green, int blue ) {
			RGBA rgba;
			rgba.Red	= ToByte( red );
			rgba.Green	= ToByte( green );
			rgba.Blue	= ToByte( blue );
			rgba.Alpha	= 255;
			return	rgba;
		}
		static public RGBA FromRGB( float red, float green, float blue ) {
			RGBA rgba;
			rgba.Red	= ToByte( red );
			rgba.Green	= ToByte( green );
			rgba.Blue	= ToByte( blue );
			rgba.Alpha	= 255;
			return	rgba;
		}
		static public RGBA FromRGB( double red, double green, double blue ) {
			RGBA rgba;
			rgba.Red	= ToByte( red );
			rgba.Green	= ToByte( green );
			rgba.Blue	= ToByte( blue );
			rgba.Alpha	= 255;
			return	rgba;
		}

		static public RGBA FromRGBA( byte red, byte green, byte blue, byte alpha ) {
			RGBA rgba;
			rgba.Red	= red;
			rgba.Green	= green;
			rgba.Blue	= blue;
			rgba.Alpha	= alpha;
			return	rgba;
		}
		static public RGBA FromRGBA( int red, int green, int blue, int alpha ) {
			RGBA rgba;
			rgba.Red	= ToByte( red );
			rgba.Green	= ToByte( green );
			rgba.Blue	= ToByte( blue );
			rgba.Alpha	= ToByte( alpha );
			return	rgba;
		}
		static public RGBA FromRGBA( float red, float green, float blue, float alpha ) {
			RGBA rgba;
			rgba.Red	= ToByte( red );
			rgba.Green	= ToByte( green );
			rgba.Blue	= ToByte( blue );
			rgba.Alpha	= ToByte( alpha );
			return	rgba;
		}
		static public RGBA FromRGBA( double red, double green, double blue, double alpha ) {
			RGBA rgba;
			rgba.Red	= ToByte( red );
			rgba.Green	= ToByte( green );
			rgba.Blue	= ToByte( blue );
			rgba.Alpha	= ToByte( alpha );
			return	rgba;
		}

		static public RGBA FromRGBA( RGBA rgba, byte alpha ) {
			rgba.Alpha	= alpha;
			return	rgba;
		}
		static public RGBA FromRGBA( RGBA rgba, int alpha ) {
			rgba.Alpha	= ToByte( alpha );
			return	rgba;
		}
		static public RGBA FromRGBA( RGBA rgba, float alpha ) {
			rgba.Alpha	= ToByte( alpha );
			return	rgba;
		}
		static public RGBA FromRGBA( RGBA rgba, double alpha ) {
			rgba.Alpha	= ToByte( alpha );
			return	rgba;
		}

		static public RGBA FromRGBA( int packedBytes ) {
			RGBA rgba;
			rgba.Red   = (byte)( ( packedBytes & 0x000000ff ) );
			rgba.Green = (byte)( ( packedBytes & 0x0000ff00 ) >> 8 );
			rgba.Blue  = (byte)( ( packedBytes & 0x00ff0000 ) >> 16 );
			rgba.Alpha = (byte)( ( packedBytes & 0xff000000 ) >> 24 );
			return	rgba;
		}

		static public RGBA	FromLuminance( int luminance ) {
			RGBA rgba;
			rgba.Red	= 255;
			rgba.Green	= 255;
			rgba.Blue	= 255;
			rgba.Alpha	= (byte) ( luminance & 0x000000ff );
			return	rgba;
		}
		static public RGBA	FromLuminance( float luminance ) {
			RGBA rgba;
			rgba.Red	= 255;
			rgba.Green	= 255;
			rgba.Blue	= 255;
			rgba.Alpha	= (byte) ( (int)( luminance * 255 ) & 0x000000ff );
			return	rgba;
		}
		static public RGBA	FromLuminance( double luminance ) {
			RGBA rgba;
			rgba.Red	= 255;
			rgba.Green	= 255;
			rgba.Blue	= 255;
			rgba.Alpha	= (byte) ( (int)( luminance * 255 ) & 0x000000ff );
			return	rgba;
		}

			
		//------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------
		
		public float	RedF {
			get {	return RGBA.ToSingle( Red );	}
			set {	Red = RGBA.ToByte( value );		}
		}
		public float	GreenF {
			get {	return RGBA.ToSingle( Green );	}
			set {	Green = RGBA.ToByte( value );	}
		}
		public float	BlueF {
			get {	return RGBA.ToSingle( Blue );	}
			set {	Blue = RGBA.ToByte( value );	}
		}
		public float	AlphaF {
			get {	return RGBA.ToSingle( Alpha );	}
			set {	Alpha = RGBA.ToByte( value );	}
		}

		//------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------

		public int		GetLuminance() {
			return	( ( (int) Red + (int) Green + (int) Blue ) * (int) Alpha ) / ( 3 * 255 );
		}

		public float	GetLuminanceF() {
			return	( ( (float) Red + (float) Green + (float) Blue ) * (float) Alpha ) / ( 3f * 255f * 255f );
		}

		//------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------
		
		public override string	ToString() {
			return	"RGBA [R=" + Red + " G=" + Green + " B=" + Blue + " A=" + Alpha + "; RGBA=" + ((int)this) + "]";
		}
		
		public int ToPackedBits() {
			return
				( (int)Red ) + 
				( (int)Green << 8 ) + 
				( (int)Blue << 16 ) + 
				( (int)Alpha << 24 );
		}

		//------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------

		// Color <-> RGBA conversion operators

		static public explicit operator RGBA( Color color ) {
			return	RGBA.FromRGBA( color.R, color.G, color.B, color.A );
		}
		static public explicit operator Color( RGBA rgba ) {
			return	Color.FromArgb( rgba.Alpha, rgba.Red, rgba.Green, rgba.Blue );
		}

		// Complex <-> RGBA conversion operators

		/*public static explicit operator RGBA ( Complex c ) {
			return	 RGBA.FromLuminance( c.Real );
		}
		public static explicit operator Complex ( RGBA rgba ) {
			return	(Complex) rgba.GetLuminanceF();
		}

		// ComplexF <-> RGBA conversion operators

		public static explicit operator RGBA ( ComplexF c ) {
			return	 RGBA.FromLuminance( c.Real );
		}
		public static explicit operator ComplexF ( RGBA rgba ) {
			return	(ComplexF) rgba.GetLuminanceF();
		}	*/
		
		// float <--> RGBA conversion operators

		public static explicit operator RGBA ( float f ) {
			return	 RGBA.FromLuminance( f );
		}
		public static explicit operator float ( RGBA rgba ) {
			return	rgba.GetLuminanceF();
		}

		// double <--> RGBA conversion operators

		public static explicit operator RGBA ( double d ) {
			return	 RGBA.FromLuminance( d );
		}
		public static explicit operator double ( RGBA rgba ) {
			return	(double) rgba.GetLuminanceF();
		}

		//------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------

		// offset rgba value by a constant
		static public RGBA operator+( RGBA rgba, int offset ) {
			rgba.Red	= RGBA.ToByte( rgba.Red + offset );
			rgba.Green	= RGBA.ToByte( rgba.Green + offset );
			rgba.Blue	= RGBA.ToByte( rgba.Blue + offset );
			rgba.Alpha	= RGBA.ToByte( rgba.Alpha + offset );
			return	rgba;
		}
		static public RGBA operator+( RGBA rgba, float f ) {
			int offset = (int)( f * 255 );
			rgba.Red	= RGBA.ToByte( rgba.Red + offset );
			rgba.Green	= RGBA.ToByte( rgba.Green + offset );
			rgba.Blue	= RGBA.ToByte( rgba.Blue + offset );
			rgba.Alpha	= RGBA.ToByte( rgba.Alpha + offset );
			return	rgba;
		}
		static public RGBA operator+( RGBA rgba, double d ) {
			int offset = (int)( d * 255 );
			rgba.Red	= RGBA.ToByte( rgba.Red + offset );
			rgba.Green	= RGBA.ToByte( rgba.Green + offset );
			rgba.Blue	= RGBA.ToByte( rgba.Blue + offset );
			rgba.Alpha	= RGBA.ToByte( rgba.Alpha + offset );
			return	rgba;
		}

		static public RGBA operator*( RGBA rgba, float scale ) {
			rgba.Red	= RGBA.ToByte( rgba.Red * scale );
			rgba.Green	= RGBA.ToByte( rgba.Green * scale );
			rgba.Blue	= RGBA.ToByte( rgba.Blue * scale );
			rgba.Alpha	= RGBA.ToByte( rgba.Alpha * scale );
			return	rgba;
		}
		static public RGBA operator*( RGBA rgba, double scale ) {
			rgba.Red	= RGBA.ToByte( rgba.Red * scale );
			rgba.Green	= RGBA.ToByte( rgba.Green * scale );
			rgba.Blue	= RGBA.ToByte( rgba.Blue * scale );
			rgba.Alpha	= RGBA.ToByte( rgba.Alpha * scale );
			return	rgba;
		}

		static public RGBA operator+( RGBA rgba0, RGBA rgba1 ) {
			int r = rgba0.Red   * rgba0.Alpha + rgba1.Red   * rgba1.Alpha;
			int g = rgba0.Green * rgba0.Alpha + rgba1.Green * rgba1.Alpha;
			int b = rgba0.Blue  * rgba0.Alpha + rgba1.Blue  * rgba1.Alpha;
			int a = rgba0.Alpha + rgba1.Alpha;
			
			if( a != 0 ) {
				return	RGBA.FromRGBA( r / a, g / a, b / a, a );
			}
			else {
				return	RGBA.FromRGBA( 0, 0, 0, 0 );
			}
		}

		//////////////////////////////////////////////////////////////////////////////
		
		public void	InvertRGB() {
			Red	= (byte)( 255 - Red );
			Green	= (byte)( 255 - Green );
			Blue	= (byte)( 255 - Blue );
		}

		//////////////////////////////////////////////////////////////////////////////
		
		static private string ToBits( int input ) {
			string bits = "";
			for( int i = 0; i < 32; i ++ ) {
				bits = ( input & 1 ).ToString() + bits;
				input >>= 1;
			}
			return bits;
		}
		static private string ToComponents( Color clr ) {
			return "Color [A=" + clr.A + " R=" + clr.R + " G=" + clr.G + " B=" + clr.B + "]"; 
		}
		
		static public void TestRGBA() {
			do {
				Debug.WriteLine( "RGBA test ------------------------------------------" );

				ColorDialog cd = new ColorDialog();
				cd.ShowDialog();
				Color clr = cd.Color;
				Debug.WriteLine( "Color = " + clr + " / " + ToComponents( clr ) );
				int bitsClr = clr.ToArgb();
				Debug.WriteLine( "(int)Color = " + bitsClr + " / " + ToBits( bitsClr ) );
				RGBA rgba = (RGBA) clr;
				Debug.WriteLine( "RGBA = " + rgba );
				int bitsRgba = (int) rgba;
				Debug.WriteLine( "(int)RGBA = " + bitsRgba + " / " + ToBits( bitsRgba ) );

				Debug.WriteLine( "----------------------------------------------------" );

				Debug.WriteLine( "RGBA = " + rgba );
				clr = (Color) rgba;
				Debug.WriteLine( "RGBA->Color = " + clr + " / " + ToComponents( clr ) );
				rgba = (RGBA) clr;
				Debug.WriteLine( "Color->RGBA = " + rgba );
			
				Debug.WriteLine( "----------------------------------------------------" );
			
			} while ( MessageBox.Show( null, "Continue?", "RGBA/Color/Bits Test", MessageBoxButtons.YesNo ) == DialogResult.Yes );
		}

		//------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------

		static private byte	ToByte( int i ) {
			return	(byte)( i & 0x000000ff );
		}
		static private byte	ToByte( float f ) {
			return	(byte)( (int) ( f * 255 ) & 0x000000ff );
		}
		static private byte	ToByte( double d ) {
			return	(byte)( (int) ( d * 255 ) & 0x000000ff );
		}

		static private float	_byteToFloatScale	= 1.0f / 255f;
		static private float	ToSingle( byte b ) {
			return	( (float) b ) * _byteToFloatScale;
		}

		static private double	_byteToDoubleScale	= 1.0 / 255;
		static private double	ToDouble( byte b ) {
			return	( (double) b ) * _byteToDoubleScale;
		}

		//------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------


	}
}
