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
using System.Collections;
using System.IO;
using System.Diagnostics;
using Exocortex;
using Exocortex.Geometry3D;
using Exocortex.Text;
using ExoEngine.Geometry;

namespace ExoEngine.Worldcraft {

	//================================================================================
	//================================================================================

	public class WorldcraftSidePlane {
		public Plane3D		plane	= Plane3D.Zero;
		//public Matrix3D	xfrm	= null;
		public Texture		texture	= null;
		public Vector3D		sAxis	= Vector3D.Zero;
		public Vector3D		tAxis	= Vector3D.Zero;

		static public WorldcraftSidePlane FromTokenReader( TokenReader tr, Textures textures ) {
			WorldcraftSidePlane sp = new WorldcraftSidePlane();

			Debug.Assert( tr.LookAhead == "(" );
			tr.GetToken();
			Vector3D pt1 = ReadVector3D( tr );
			Debug.Assert( tr.LookAhead == ")" );
			tr.GetToken();

			Debug.Assert( tr.LookAhead == "(" );
			tr.GetToken();
			Vector3D pt2 = ReadVector3D( tr );
			Debug.Assert( tr.LookAhead == ")" );
			tr.GetToken();

			Debug.Assert( tr.LookAhead == "(" );
			tr.GetToken();
			Vector3D pt3 = ReadVector3D( tr );
			Debug.Assert( tr.LookAhead == ")" );
			tr.GetToken();

			sp.plane = Plane3D.FromCoplanarPoints( pt2, pt1, pt3 );
			sp.plane.Flip();
			//Debug.Assert( sp.plane != null );

			sp.texture = textures.RequestTexture( tr.GetToken() );
			Debug.Assert( sp.texture != null );
			if( sp.texture.IsBitmapLoaded == false ) {
				sp.texture.LoadBitmap();
			}

			Debug.Assert( tr.LookAhead == "[" );
			tr.GetToken();
			Vector3D sAxis = ReadVector3D( tr );
			float	sOffset = float.Parse( tr.GetToken() );
			Debug.Assert( tr.LookAhead == "]" );
			tr.GetToken();

			Debug.Assert( tr.LookAhead == "[" );
			tr.GetToken();
			Vector3D tAxis = ReadVector3D( tr );
			float	tOffset = float.Parse( tr.GetToken() );
			Debug.Assert( tr.LookAhead == "]" );
			tr.GetToken();

			Vector3D	rAxis = Vector3D.Cross( sAxis, tAxis );

			float	rotation	= float.Parse( tr.GetToken() );
			float	sScale		= float.Parse( tr.GetToken() );
			float	tScale		= float.Parse( tr.GetToken() );

			Matrix3D xfrm = new Matrix3D();
			//xfrm.Scale( sScale, tScale, 1 );
			//xfrm.Translate( -sOffset, -tOffset, 0 );
			sp.sAxis = sAxis;
			sp.tAxis = tAxis;

			//xfrm.ChangeBasis( sAxis, tAxis, rAxis );
			//xfrm.Scale( sp.texture.Width, sp.texture.Height, 1 );

			//xfrm.Invert();
			//sp.xfrm = xfrm;

			return sp;
		}

		static protected Vector3D ReadVector3D( TokenReader tr ) {
			return Vector3D.FromXYZ( float.Parse( tr.GetToken() ), float.Parse( tr.GetToken() ), float.Parse( tr.GetToken() ) );
		}

		public override string	ToString() {
			return "SidePlane";
		}
	}
	
	// ================================================================================
	// ================================================================================
	
	public class WorldcraftSidePlanes {
		protected ArrayList	_arraylist = new ArrayList();

		public WorldcraftSidePlanes() {
		}

		public int Add( WorldcraftSidePlane sp ) {
			return	_arraylist.Add( sp );
		}
		public void Remove( WorldcraftSidePlane sp ) {
			_arraylist.Remove( sp );
		}
		public bool Contains( WorldcraftSidePlane sp ) {
			return _arraylist.Contains( sp );
		}
		public int Count {
			get { return _arraylist.Count; }
		}
		public WorldcraftSidePlane this[ int index ] {
			get { return (WorldcraftSidePlane) _arraylist[index];	}
		}
		static public explicit operator ArrayList ( WorldcraftSidePlanes sps ) {
			return	sps._arraylist;
		}
	}

	//================================================================================
	//================================================================================

}
