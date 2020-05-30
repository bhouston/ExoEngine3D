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

using ExoEngine;
using ExoEngine.Worldcraft;
using ExoEngine.Geometry;
using ExoEngine.Rendering;


namespace ExoEngine.Commands {
	
	//----------------------------------------------------------------------------------
	
	public class ApplicationAbout : CommandButton {
		public ApplicationAbout() {
			this.Text = "&About " + Application.ProductName + "...";
		}
		protected override bool OnExecute() {
			ExoEngine.MessageBox(
				"Copyright (c) 2001, 2002, Ben Houston\n" + 
				"Questions, comments, bugs?  Send them to ben@exocortex.org",
				"About " + Application.ProductName,
				MessageBoxButtons.OK,
				MessageBoxIcon.Information );
			return true;
		}
	}

	public class ApplicationForceCrash : CommandButton {
		public ApplicationForceCrash() {
			this.Text = "&Force Application Crash";
		}
		protected override bool OnExecute() {
			if( MessageBox.Show( null,
				"You are about to deliberately crash the application.\n" +
				"Do you really want to do this?",
				Application.ProductName,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning ) == DialogResult.Yes ) {
				((object)null).ToString();
			}
			return true;
		}
	}

	public class ApplicationExit : CommandButton {
		public ApplicationExit() {
			this.Text = "E&xit";
		}
		protected override bool OnExecute() {
			//ExoEngine.IdleThread.Abort();
			//new MainForm().Close();
			if( ExoEngine.ActiveWorld != null ) {
				if( ExoEngine.Commands.Execute( typeof( FileClose ) ) == false ) {
					return	false;
				}
			}

			Debug.Assert( ExoEngine.ActiveWorld == null );
			Application.Exit();
			return true;
		}
	}


	//----------------------------------------------------------------------------------

}