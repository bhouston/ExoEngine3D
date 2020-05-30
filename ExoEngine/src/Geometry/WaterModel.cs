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
using Exocortex;

namespace ExoEngine.Geometry {

	public class WaterModel {

		//-------------------------------------------------------------------------

		public WaterModel( int xSize, int ySize ) {
			Debug.Assert( xSize*ySize > 0 );

			_xSize = xSize;
			_ySize = ySize;

			_currentHeights = new short[ _xSize * _ySize ];
			Array.Clear( _currentHeights, 0, _currentHeights.Length );

			_oldHeights = new short[ _xSize * _ySize ];
			Array.Clear( _oldHeights, 0, _oldHeights.Length );
		}

		//-------------------------------------------------------------------------

		protected int		_xSize	= -1;
		public int	XSize {
			get	{	return	_xSize;	}
		}

		protected int		_ySize	= -1;
		public int	YSize {
			get	{	return	_ySize;	}
		}

		protected short[]	_currentHeights	= null;
		protected short[]	_oldHeights	= null;

		//-------------------------------------------------------------------------

		public	void	Advance() {
			// swap buffer pointers
			short[] temp = _currentHeights;
			_currentHeights = _oldHeights;
			_oldHeights = temp;

			// apply algorithnm to every non-edge pixel
			for( int y = 1; y < _ySize - 1; y ++ ) {
				int yInc = _xSize * y;
				for( int x = 1; x < _xSize - 1; x ++ ) {
					int xyInc = yInc + x;
					_currentHeights[ xyInc ] = 
						(short)((( _oldHeights[ xyInc - 1 ] + 
						_oldHeights[ xyInc + 1 ] + 
						_oldHeights[ xyInc - _ySize ] + 
						_oldHeights[ xyInc + _ySize ] ) / 2 - 
						_currentHeights[ xyInc ] ) * 0.95f );
				}
			}
		}

		public short this[ int x, int y ] {
			get {	return	_currentHeights[ x + y * _xSize ];	}
			set {	_currentHeights[ x + y * _xSize ] = value;	}
		}

		//-------------------------------------------------------------------------

	}

}
