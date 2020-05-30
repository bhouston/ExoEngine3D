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
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using Exocortex;
using Exocortex.Diagnostics;
using Exocortex.Text;
using Exocortex.Windows.Forms;

using ExoEngine.Geometry;

namespace ExoEngine {

	public class ExoEngine {

		//-----------------------------------------------------------------------

		static public string sWorldPath			= null;
		static public string sWorldExtension	= ".ewrl";
		static public string sWorldFileFilter	= "ExoEngine World (*.ewrl)|*.ewrl|All files (*.*)|*.*";
		
		static public string	sTexturePath		= null;		  
		static public string[]	sTextureExtensions	= new string[]{ ".gif", ".jpg", ".bmp" };

		static public string sWorldcraftMapPath			= null;
		static public string sWorldcraftMapExtension	= ".map";
		static public string sWorldcraftMapFileFilter	= "Worldcraft Map (*.map)|*.map|All files (*.*)|*.*";

		static public string sScreenshotPath		= null;
		static public string sScreenshotExtension	= ".bmp";
		static public string sScreenshotFileFilter	= "Bitmap (*.bmp)|*.bmp";

		static public string sSpritePath			= null;
		
		//-----------------------------------------------------------------------

		static string GetDataPath( string dataDirectory ) {
			string startupPath = Path.Combine( Application.StartupPath, dataDirectory );
			if( Directory.Exists( startupPath ) == true ) {
				return	startupPath;
			}

			string execPath = Path.Combine( Application.ExecutablePath, dataDirectory );
			if( Directory.Exists( execPath ) == true ) {
				return	execPath;
			}

			string assemblyLocation = Path.GetDirectoryName( typeof( ExoEngine ).Assembly.Location );
			string assemblyPath = Path.Combine( assemblyLocation, dataDirectory );
			if( Directory.Exists( assemblyPath ) == true ) {
				return	assemblyPath;
			}

			throw new FileNotFoundException( "The '" + dataDirectory + "' data directory not found.  The following suspected full paths not found:\n" + 
				"startupPath: '" + startupPath + "'\n" + 
				"execPath: '" + execPath + "'\n" + 
				"assemblyPath: '" + assemblyPath + "'",
			   dataDirectory );
		}

		[STAThread]
		static void Main() {
			try {

				// setup global bug tracker
				Application.ThreadException += new ThreadExceptionEventHandler( BugTracking.OnThreadException );

				// setup main thread
				Thread mainThread = Thread.CurrentThread;
				mainThread.Name = "ExoEngine.MainThread";

				// setup data directories
				sTexturePath		= GetDataPath( "Textures" );
				sWorldcraftMapPath	= GetDataPath( "Maps" );
				sWorldPath			= GetDataPath( "Worlds" );
				sSpritePath			= GetDataPath( "Sprites" );
				sScreenshotPath		= GetDataPath( "" );

				// create main form
				_mainform = new MainForm();
				_mainform.Initialize();

				// run application
				Application.Run( _mainform );
				_mainform = null;
			
			}
			catch( Exception e ) {
				BugTracking.LogException( e );
			}
		}

		//-----------------------------------------------------------------------
		
		//-----------------------------------------------------------------------

		static protected MainForm _mainform = null;
		static public MainForm	MainForm {
			get	{	return	_mainform;	}
		}

		//-----------------------------------------------------------------------

		static public Viewer	Viewer {
			get {	return	_mainform.Viewer;	}
		}

		//-----------------------------------------------------------------------

		static public World ActiveWorld {
			get	{	return	_mainform.ActiveWorld;	}
			set {	_mainform.ActiveWorld = value;	}
		}

		//-----------------------------------------------------------------------

		static public CommandControls Commands {
			get	{	return	_mainform.Commands;	}
		}

		//-----------------------------------------------------------------------

		static public void UpdateAll() {
			_mainform.UpdateAll();
		}
		
		//-----------------------------------------------------------------------

		static public Stream	GetResourceStream( string streamName ) {
			string streamPath = "ExoEngine." + streamName;
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream( streamPath );
			if( stream == null ) {
				throw new ResourceNotFoundException( "Could not find a ManifestResourceStream for stream (name:'" + streamName + "',fullName:'" + streamPath + "')" );
			}
			return stream;
		}
		
		static public Icon		GetResourceIcon( string fileName ) {
			return	new Icon( GetResourceStream( "Icons." + fileName ) );
		}

		//-----------------------------------------------------------------------

		static public void Error( string strMessage ) {
			System.Windows.Forms.MessageBox.Show( _mainform, strMessage, "ExoEngine", MessageBoxButtons.OK, MessageBoxIcon.Error );
		}
		static public DialogResult MessageBox( string strMessage, string strCaption ) {
			return System.Windows.Forms.MessageBox.Show( _mainform, strMessage, strCaption );
		}
		static public DialogResult MessageBox( string strMessage, string strCaption, MessageBoxButtons buttons ) {
			return System.Windows.Forms.MessageBox.Show( _mainform, strMessage, strCaption, buttons );
		}
		static public DialogResult MessageBox( string strMessage, string strCaption, MessageBoxButtons buttons, MessageBoxIcon icon ) {
			return System.Windows.Forms.MessageBox.Show( _mainform, strMessage, strCaption, buttons, icon );
		}

		//-----------------------------------------------------------------------

	}
}
