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
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;

using Exocortex;
using Exocortex.Diagnostics;
using Exocortex.Windows.Forms;
using Exocortex.Geometry3D;

using ExoEngine.Worldcraft;
using ExoEngine.Geometry;
using ExoEngine.Rendering;


namespace ExoEngine.Commands {
	
	//----------------------------------------------------------------------------------
	

	public class FileOpen : CommandButton {
		public FileOpen() {
			this.Text = "&Open World...";
		}
		protected override bool OnExecute() {
			if( ExoEngine.ActiveWorld != null ) {
				if( ExoEngine.Commands.Execute( typeof( FileClose ) ) != true ) {
					return false;
				}
			}

			string strFileName = null;
			if( this.Params != null ) {
				if( this.Params[0] is string ) {
					strFileName = (string) this.Params[0];
				}
			}
			else {
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Multiselect	= false;
				ofd.Title		= "Open World";
				ofd.Filter		= ExoEngine.sWorldFileFilter;
				ofd.InitialDirectory = ExoEngine.sWorldPath;

				DialogResult result = ofd.ShowDialog();
				if( result != DialogResult.OK ) {
					return false;
				}

				strFileName	= ofd.FileName;
			}

			if( strFileName == null || File.Exists( strFileName ) == false ) {
				return false;
			}

			// deserialize XML -> World
			//Debug2.Push( "XmlSerializer.Deserialize( world )" );
			World world = null;
			try {
				FileStream sIn = new FileStream( strFileName, FileMode.Open );
				XmlSerializer xmlIn = new System.Xml.Serialization.XmlSerializer( typeof( World ) );
				world = (World) xmlIn.Deserialize( sIn );
				sIn.Close();
			}
			catch( Exception e ) {
				string fileName = BugTracking.WriteExceptionRecord( e );
				ExoEngine.Error( "Error loading World from file '" + strFileName + "'.\n" + 
					"Please send this bug report to bugs@exocortex.org:\n" + 
					fileName );
				return false;
			}
			//Debug2.Pop();

			world.FileName	= strFileName;
			world.Dirty		= false;
			world.Reset();

			ExoEngine.ActiveWorld = world;
			ExoEngine.UpdateAll();

			return true;
		}
		protected override void OnUpdate() {
		}
	}

	public class FileSave : CommandButton {
		public FileSave() {
			this.Text = "&Save World";
		}
		protected override bool OnExecute() {
			World world = ExoEngine.ActiveWorld;

			// serialize World -> XML
			//Debug2.Push( "XmlSerializer.Serialize( world )" );
			try {
				StreamWriter sOut = new StreamWriter( world.FileName, false, System.Text.Encoding.Default, 1 );
				XmlSerializer xmlOut = new System.Xml.Serialization.XmlSerializer( typeof( World ) );
				xmlOut.Serialize( sOut, world, new XmlSerializerNamespaces() );
				sOut.Close();
			}
			catch( Exception e ) {
				string fileName = BugTracking.WriteExceptionRecord( e );
				ExoEngine.Error( "Error save World to file '" + world.FileName + "'.\n" + 
					"Please send this bug report to bugs@exocortex.org:\n" + 
					fileName );
				return false;
			}
			//Debug2.Pop();

			ExoEngine.ActiveWorld.Dirty = false;
			ExoEngine.UpdateAll();

			return true;
		}
		protected override void OnUpdate() {
			World world = ExoEngine.ActiveWorld;
			this.Enabled = ( world != null ) && ( world.Dirty == true ) && ( world.FileName != null );
		}
	}

	public class FileSaveAs : CommandButton {
		public FileSaveAs() {
			this.Text = "Save World &As...";
		}
		protected override bool OnExecute() {
			World world = ExoEngine.ActiveWorld;

			string strFileName = null;
			if( this.Params != null ) {
				if( this.Params[0] is string ) {
					strFileName = (string) this.Params[0];
				}
			}
			else {
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.Title		= "Save World";
				sfd.Filter		= ExoEngine.sWorldFileFilter;
				sfd.FileName	= ( world.FileName != null ) ? world.FileName : "untitled.wrl";
				sfd.DefaultExt	= ExoEngine.sWorldExtension;
				sfd.InitialDirectory = ExoEngine.sWorldPath;
			
				DialogResult result = sfd.ShowDialog();
				if( result != DialogResult.OK ) {
					return false;
				}
				strFileName	= sfd.FileName;
			}

			if( strFileName == null ) {
				return false;
			}

			world.Dirty		= true;
			world.FileName	= strFileName;

			return	ExoEngine.Commands.Execute( typeof( FileSave ) );
		}
		protected override void OnUpdate() {
			World world = ExoEngine.ActiveWorld;
			this.Enabled = ( world != null );
		}
	}

	public class FileClose : CommandButton {
		public FileClose() {
			this.Text = "&Close World";
		}
		protected override bool OnExecute() {
			if( ExoEngine.ActiveWorld.Dirty == true ) {
				DialogResult result = ExoEngine.MessageBox( "Current world has been modified.  Do you want to save your changes to '" + ExoEngine.ActiveWorld.FileName + "'?",
					"Close World", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question );
				switch( result ) {
				case DialogResult.Yes:
					if( ExoEngine.Commands.Execute( typeof( FileSave ) ) != true ) {
						return false;
					}
					break;
				case DialogResult.No:
					break;
				case DialogResult.Cancel:
					return false;
				}
			}

			World world = ExoEngine.ActiveWorld;
			ExoEngine.ActiveWorld = null;
			world.Dispose();
			ExoEngine.UpdateAll();
			
			return true;
		}
		protected override void OnUpdate() {
			this.Enabled = ( ExoEngine.ActiveWorld != null );
		}
	}

	public class FileImport : CommandButton {
		public FileImport() {
			this.Text = "&Import Worldcraft Map...";
		}
		public class ImportProcess : LongProcess {
			public ImportProcess() : base( "ExoEngine Import Progress", 2 ) {}
			public string	WorldcraftFileName = null;
			public World	World = null;
			public override bool DoProcess() {
				//Debug2.Push();
				Debug.Assert( this.WorldcraftFileName != null );
				Debug.Assert( this.World == null );

				this.BeginTask( "Loading Worldcraft Map..." );

				WorldcraftMap wm = WorldcraftMap.FromFile( this.WorldcraftFileName, ExoEngine.sTexturePath );
				if( wm == null ) {
					this.Finished( false, "Error loading WorldcraftMap from file '" + this.WorldcraftFileName + "'." );
					//Debug2.Pop();
					return false;
				}
			
				this.BeginTask( "Converting Worldcraft Map to internal representation..." );

				World world = World.FromWorldcraftMap( wm );
				if( world == null ) {
					this.Finished( false, "Error converting WorldcraftMap to World." );
					//Debug2.Pop();
					return false;
				}
        	
				this.Finished( true );

				this.World = world;

				//Debug2.Pop();
				return	true;
			}
		}
		protected override bool OnExecute() {
			if( ExoEngine.ActiveWorld != null ) {
				if( ExoEngine.Commands.Execute( typeof( FileClose ) ) != true ) {
					return false;
				}
			}

			string strFileName = null;
			if( this.Params != null ) {
				if( this.Params[0] is string ) {
					strFileName = (string) this.Params[0];
				}
			}
			else {
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Multiselect			= false;
				ofd.Title				= "Import Worldcraft Map";
				ofd.Filter				= ExoEngine.sWorldcraftMapFileFilter;
				ofd.InitialDirectory	= ExoEngine.sWorldcraftMapPath;

				DialogResult result = ofd.ShowDialog();
				if( result != DialogResult.OK ) {
					return false;
				}
				strFileName	= ofd.FileName;
			}

			if( strFileName == null || File.Exists( strFileName ) == false ) {
				return false;
			}

			ExoEngine.MainForm.Update();

			ImportProcess ip = new ImportProcess();
			ip.WorldcraftFileName = strFileName;
			//Debug2.Push();
			if( ip.Execute( ExoEngine.MainForm ) != true ) {
				return false;
			}
			//Debug2.Pop();

			ip.World.Reset();
			ip.World.Dirty = true;
			
			ExoEngine.ActiveWorld = ip.World;
			ExoEngine.UpdateAll();

			return true;
		}
	}

	public class FileExportScreenshot : CommandButton {
		public FileExportScreenshot() {
			this.Text = "&Export Screenshot...";
		}
		protected override bool OnExecute() {
			string strFileName = null;
			if( this.Params != null ) {
				if( this.Params[0] is string ) {
					strFileName = (string) this.Params[0];
				}
			}
			else {
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.Title				= "Export Screenshot";
				sfd.Filter				= ExoEngine.sScreenshotFileFilter;
				sfd.DefaultExt			= ExoEngine.sScreenshotExtension;
				sfd.InitialDirectory	= ExoEngine.sScreenshotPath;
			
				DialogResult result = sfd.ShowDialog();
				if( result != DialogResult.OK ) {
					return false;
				}
				strFileName	= sfd.FileName;
			}

			if( strFileName == null ) {
				return false;
			}

			Bitmap bitmapScreenshot = ExoEngine.Viewer.ToBitmap();
			bitmapScreenshot.Save( strFileName );

			return true;
		}
	}

	//----------------------------------------------------------------------------------

}