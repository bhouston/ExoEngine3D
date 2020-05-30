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
using System.Threading;
using Exocortex;
using Exocortex.Geometry3D;
using Exocortex.Text;
using ExoEngine.Geometry;

namespace ExoEngine.Worldcraft {

	//================================================================================
	//================================================================================


	public class WorldcraftMap {
		
		public WorldcraftMap() {
			_objects = new WorldcraftObjects();
		}

		static public WorldcraftMap  FromFile( string strFileName, string strTexturePath ) {
			//Debug2.Push( "Reading Worldcraft Map" );
			
			WorldcraftMap wm = new WorldcraftMap();
			wm.FileName = strFileName;
			//wm.Textures.DefaultPath			= strTexturePath;
			//wm.Textures.DefaultExtension	= ".bmp";

//			progressTracker.UpdateProgress( 20 );
//			progressTracker.UpdateTask( "Parsing worldcraft map: '" + strFileName + "'..." );

			TokenReader tr = new TokenReader( strFileName, ' ' );

			int faceCount = 0;
			while( tr.LookAhead != null && tr.LookAhead == "{" ) {
				WorldcraftObject wo = WorldcraftObject.FromTokenReader( tr, wm.Textures );
				faceCount += wo.Faces.Count;
				wm.Objects.Add( wo );
			}
			
			tr.Close();

			//Thread.Sleep( 200 );
										   
//			progressTracker.UpdateTask( "      worldcraft objects: " + wm.Objects.Count );
//			progressTracker.UpdateTask( "      totals faces count: " + faceCount );
//			progressTracker.UpdateTask( "      # of textures in use: " + wm.Textures.Count );

			//Debug2.Pop();

			return wm;
		}

		//---------------------------------------------------------------------------

		protected string	_strFileName = "";
		public string	FileName {
			get {	return	_strFileName;	}
			set {	_strFileName = value;	}
		}

		protected Textures	_textures = new Textures();
		public Textures		Textures {
			get {	return	_textures;	}
		}

		protected WorldcraftObjects _objects = null;
		public WorldcraftObjects Objects {
			get {	return _objects;	}
		}

		//---------------------------------------------------------------------------
	}

	//================================================================================
	//================================================================================

}
