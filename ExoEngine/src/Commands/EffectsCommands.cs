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
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;

using Exocortex;
using Exocortex.Windows.Forms;
using Exocortex.Geometry3D;

using ExoEngine.Worldcraft;
using ExoEngine.Geometry;
using ExoEngine.Rendering;


namespace ExoEngine.Commands {
	
	//----------------------------------------------------------------------------------
	
	public class EffectsBouncingBalls : CommandButton {
		public EffectsBouncingBalls() {
			this.Text = "&Bouncing Balls";
		}
		protected override bool OnExecute() {
			/*Random r = new Random();
			World world = ExoEngine.ActiveWorld;
			for( int i = 0; i < 100; i ++ ) {
				Vector3D x = new Vector3D( r.Next( 1000 ) - 500, r.Next( 1000 ) - 500, 500 + r.Next( 250 ) );
				Vector3D v = new Vector3D( r.Next( 100 ) - 50, r.Next( 100 ) - 50, r.Next( 100 ) - 50 );
				Color clr = Color.FromArgb( 255, r.Next( 255 ), r.Next( 255 ), r.Next( 255 ) );
				world.Entities.Add( new Ball( x, v, 0.5f + (float)r.NextDouble() * 0.5f, (BSPTreeNode) world.Entities[0], clr ) );
			}	 */
			return true;
		}
		protected override void OnUpdate() {
			this.Enabled = false;//( ExoEngine.ActiveWorld != null );
		}
	}

	public class EffectsRemoveBalls : CommandButton {
		public EffectsRemoveBalls() {
			this.Text = "&Remove Bouncing Balls";
		}
		protected override bool OnExecute() {
			return true;
		}
		protected override void OnUpdate() {
			this.Enabled = false;//( ExoEngine.ActiveWorld != null );
		}
	}

	public class EffectsShowFaces : CommandButton {
		public EffectsShowFaces() {
			this.Text = "&Show Faces";
		}
		protected override bool OnExecute() {
			return true;
		}
		protected override void OnUpdate() {
			this.Enabled = false;//( ExoEngine.ActiveWorld != null );
		}
	}


	//----------------------------------------------------------------------------------

}