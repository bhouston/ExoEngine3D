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
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Exocortex;
using Exocortex.Diagnostics;
using Exocortex.Windows.Forms;
using Exocortex.OpenGL;
using ExoEngine.Commands;
using ExoEngine.Geometry;

namespace ExoEngine {

	public class MainForm : System.Windows.Forms.Form {
	
		public MainForm() {
		}
		~MainForm() {
		}

		protected override void Dispose( bool bDisposing ) {
			World world = this.ActiveWorld;
			//GL.glErrorCheck();

			if( world != null ) {
				//GL.glErrorCheck();
				this.ActiveWorld = null;
				//GL.glErrorCheck();
				world.Dispose();
				//GL.glErrorCheck();
			}
			
			base.Dispose( bDisposing );
			//GL.glErrorCheck();
			//Debug2.Pop();
		}
				 
		public void Initialize() {
			this.ClientSize = new System.Drawing.Size( 400, 400 );
			//this.Text = ExoEngine.sApplicationTitle;

			SetupControls();
			SetupMenus();

			this.KeyDown	+= new KeyEventHandler( this.Viewer.OnKeyDown );
			this.KeyUp		+= new KeyEventHandler( this.Viewer.OnKeyUp );
			//this.KeyPress	+= new KeyPressEventHandler( this.Viewer.OnKeyPress );
			this.KeyPreview = true;
		
			ExoEngine.UpdateAll();
		}

		protected override bool IsInputKey( Keys keys ) {
			return true;
		}
		
		/*protected void TriggerXMLSerializerCaching() {
			Debug2.Push( "TriggerXMLSerializerCaching" );

			ExoEngine.Commands.Execute( typeof( FileImport ), "C:\\3D\\Maps\\cube.map" );
			ExoEngine.Commands.Execute( typeof( FileSave ), "C:\\3D\\Worlds\\cube.wrl" );
			ExoEngine.Commands.Execute( typeof( FileOpen ), "C:\\3D\\Worlds\\cube.wrl" );
			ExoEngine.Commands.Execute( typeof( FileClose ) );

			Debug2.Pop();
		}*/

		protected override void OnClosing( CancelEventArgs e ) {
			//Debug2.Push();
			if( ExoEngine.ActiveWorld != null ) {
				if( ExoEngine.Commands.Execute( typeof( FileClose ) ) != true ) {
					e.Cancel = true;
				}
			}
			base.OnClosing( e );
			//Debug2.Pop();
		}

		protected override void OnClosed( EventArgs e ) {
			//Debug2.Push();
			//this.Dispose();
			base.OnClosed( e );
			//Debug2.Pop();
		}

		private void SetupControls() {
			this.SuspendLayout();

			_viewer.Dock = DockStyle.Fill;
			this.Controls.Add( _viewer );

			this.Controls.Add( _statusbar );
			
			this.ResumeLayout();
		}

		private void SetupMenus() {
			
			MenuItem menuFile = new MenuItem( "&File" );
			menuFile.MenuItems.Add( Commands.CreateMenuItem( typeof( FileOpen ) ) );
			menuFile.MenuItems.Add( Commands.CreateMenuItem( typeof( FileSave ) ) );
			menuFile.MenuItems.Add( Commands.CreateMenuItem( typeof( FileSaveAs ) ) );
			menuFile.MenuItems.Add( Commands.CreateMenuItem( typeof( FileClose ) ) );
			menuFile.MenuItems.Add( new MenuItem( "-" ) );
			menuFile.MenuItems.Add( Commands.CreateMenuItem( typeof( FileImport ) ) );
			menuFile.MenuItems.Add( new MenuItem( "-" ) );
			menuFile.MenuItems.Add( Commands.CreateMenuItem( typeof( FileExportScreenshot ) ) );
			menuFile.MenuItems.Add( new MenuItem( "-" ) );
			menuFile.MenuItems.Add( Commands.CreateMenuItem( typeof( ApplicationExit ) ) );

			/*MenuItem menuEffects = new MenuItem( "&Effects" );
			menuEffects.MenuItems.Add( Commands.CreateMenuItem( typeof( EffectsBouncingBalls ) ) );
			menuEffects.MenuItems.Add( Commands.CreateMenuItem( typeof( EffectsRemoveBalls ) ) );
			menuEffects.MenuItems.Add( Commands.CreateMenuItem( typeof( EffectsShowFaces ) ) );*/

			MenuItem menuRenderer = new MenuItem( "&Renderer" );
			menuRenderer.MenuItems.Add( Commands.CreateMenuItem( typeof( RendererBasic ) ) );
			menuRenderer.MenuItems.Add( Commands.CreateMenuItem( typeof( RendererFlat ) ) );
			menuRenderer.MenuItems.Add( Commands.CreateMenuItem( typeof( RendererGouraud ) ) );
			menuRenderer.MenuItems.Add( Commands.CreateMenuItem( typeof( RendererPhong ) ) );
			//menuRenderer.MenuItems.Add( Commands.CreateMenuItem( typeof( RendererCartoon ) ) );
			menuRenderer.MenuItems.Add( Commands.CreateMenuItem( typeof( RendererPencil ) ) );
			menuRenderer.MenuItems.Add( Commands.CreateMenuItem( typeof( RendererReflection ) ) );

			MenuItem menuTextureOptions = new MenuItem( "&Texture" );
			menuTextureOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( TextureOptionLowQuality ) ) );
			menuTextureOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( TextureOptionMediumQuality ) ) );
			menuTextureOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( TextureOptionHighQuality ) ) );

			MenuItem menuOptions = new MenuItem( "&Options" );
			menuOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( RenderOptionWaterAdvance ) ) );
			menuOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( RenderOptionWaterUpdateVertices ) ) );
			menuOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( RenderOptionWaterUpdateNormals ) ) );
			menuOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( RenderOptionWaterRender ) ) );
			menuOptions.MenuItems.Add( new MenuItem( "-" ) );

			menuOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( RenderOptionDuckRender ) ) );
			menuOptions.MenuItems.Add( new MenuItem( "-" ) );

			menuOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( RenderOptionBackground ) ) );
			menuOptions.MenuItems.Add( new MenuItem( "-" ) );

			menuOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( RenderOptionWireframe ) ) );
			menuOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( RenderOptionZBuffer ) ) );
			menuOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( RenderOptionFaceColors ) ) );
			menuOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( RenderOptionTextures ) ) );
			menuOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( ViewerOptionXYGrid ) ) );
			menuOptions.MenuItems.Add( Commands.CreateMenuItem( typeof( ViewerOptionCrossHairs ) ) );
			menuOptions.MenuItems.Add( menuTextureOptions );

			MenuItem menuHelp = new MenuItem( "&Help" );
			menuHelp.MenuItems.Add( Commands.CreateMenuItem( typeof( ApplicationForceCrash ) ) );
			menuHelp.MenuItems.Add( new MenuItem( "-" ) );
			menuHelp.MenuItems.Add( Commands.CreateMenuItem( typeof( ApplicationAbout ) ) );

			MainMenu mainmenu = new MainMenu();
			mainmenu.MenuItems.Add( menuFile );
			//mainmenu.MenuItems.Add( menuEffects );
			mainmenu.MenuItems.Add( menuRenderer );
			mainmenu.MenuItems.Add( menuOptions );
			mainmenu.MenuItems.Add( menuHelp );
			this.Menu = mainmenu;

			this.Icon = ExoEngine.GetResourceIcon( "exocortexLogo.ico" );
		}

		//------------------------------------------------------------------------------

		protected CommandControls _commands = new CommandControls();
		public CommandControls Commands {
			get	{	return	_commands;	}
		}

		protected Viewer	_viewer = new Viewer();
		public Viewer Viewer {
			get	{	return	_viewer;	}
		}

		protected StatusBar	_statusbar = new StatusBar();
		public StatusBar StatusBar {
			get	{	return	_statusbar;	}
		}

		//------------------------------------------------------------------------------

		protected World _world = null;
		public World ActiveWorld {
			get	{	return	_world;	}
			set {
				if( _world == value ) {
					return;
				}
				
				BugTracking.SetWatch( "MainForm.World", _world );
				_world = value;
				this.Viewer.World = _world;
			}
		}

		//------------------------------------------------------------------------------
		
		/*public void OnIdle() {
			while( true ) {
				if( _world != null ) {
					//lock( _world ) {
					//	if( _world != null ) {
							this.Viewer.Invalidate();
						//	this.Viewer.Update();
							Thread.Sleep( 0 );
					//	}
					//}
				}
				else {
					Thread.Sleep( 100 );
				}
			}
		}  */
		
		//------------------------------------------------------------------------------

		public void UpdateAll() {
			string strAppTitle = Application.ProductName;//ExoEngine.sApplicationTitle;
			if( this.ActiveWorld != null ) {
				World world = this.ActiveWorld;
				strAppTitle += " - " + ((world.FileName != null) ? Path.GetFileName( world.FileName ) : "untitled.wrl");
				if( world.Dirty == true ) {
					strAppTitle += "*";
				}
			}
			if( this.Text.Equals( strAppTitle ) != true ) {
				this.Text = strAppTitle;
			}

			this.Commands.UpdateAll();
			this.Viewer.Invalidate();
			this.Viewer.Update();
		}
		
		//------------------------------------------------------------------------------

	}

}