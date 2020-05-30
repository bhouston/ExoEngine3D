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
	/// Summary description for Color2.
	/// </summary>
	public class Color2 {

		//------------------------------------------------------------------------------------

		static public Color	FromArgb( int a, int r, int g, int b ) {
			return	Color.FromArgb(
				Math2.Clamp( a, 0, 255 ),
				Math2.Clamp( r, 0, 255 ),
				Math2.Clamp( g, 0, 255 ),
				Math2.Clamp( b, 0, 255 ) );
		}

		static public Color	Max( Color c0, Color c1 ) {
			return	Color.FromArgb(
				Math.Max( c0.A, c1.A ),
				Math.Max( c0.R, c1.R ),
				Math.Max( c0.G, c1.G ),
				Math.Max( c0.B, c1.B ) );
		}

		static public Color	Min( Color c0, Color c1 ) {
			return	Color.FromArgb(
				Math.Min( c0.A, c1.A ),
				Math.Min( c0.R, c1.R ),
				Math.Min( c0.G, c1.G ),
				Math.Min( c0.B, c1.B ) );
		}

		//------------------------------------------------------------------------------------


	}
}
