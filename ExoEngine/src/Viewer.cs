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
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Drawing.Imaging;

using Exocortex;
using Exocortex.Diagnostics;
using Exocortex.Geometry3D;
using Exocortex.OpenGL;
using Exocortex.Windows.Forms;

using ExoEngine.Geometry;
using ExoEngine.Rendering;


namespace ExoEngine {

	public class Viewer : OpenGLControl  {

		//-----------------------------------------------------------------------------------

		static public readonly float sNearClippingPlane	= 70;
		static public readonly float sFarClippingPlane	= 7500;

		//-----------------------------------------------------------------------------------
		//-----------------------------------------------------------------------------------

		public Viewer() {
			Application.Idle += new EventHandler( this.OnIdle );
		}

		protected override void Dispose( bool bDisposing ) {
			this.Hide();
			//Application.Idle -= new EventHandler( this.OnIdle );
			base.Dispose( bDisposing );
		}

		//-----------------------------------------------------------------------------------
		
		protected override bool IsInputKey( Keys keys ) {
			return true;
		}

		public void OnKeyDown( object source, KeyEventArgs keyEvent ) {
			if( this.World != null ) {
				this.World.Player.OnKeyDown( keyEvent.KeyCode );
			}
		}
		public void OnKeyUp( object source, KeyEventArgs keyEvent ) {
			if( this.World != null ) {
				this.World.Player.OnKeyUp( keyEvent.KeyCode );
			}
		}

		//-----------------------------------------------------------------------------------

		protected override void InitGLContext() {

			// depth testing
			//GL.glEnable( GL.GL_DEPTH_TEST );
			//GL.glDepthMask( (byte) GL.GL_TRUE );
			GL.glDepthFunc( GL.GL_LEQUAL );
			//GL.glDepthRange( sNearClippingPlane, sFarClippingPlane );
			
			// face culling
			//GL.glFrontFace( GL.GL_CW );
			//GL.glCullFace( GL.GL_BACK ); 
			//GL.glEnable( GL.GL_CULL_FACE );

			GL.glHint( GL.Hint.PerspectiveCorrection, GL.Quality.Nicest );

			// texture blending
			GL.glEnable( GL.GL_COLOR_MATERIAL );
			GL.glTexEnvi( GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, (int) GL.GL_MODULATE );
			GL.glEnable( GL.GL_BLEND );
			GL.glBlendFunc( GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA );

			GL.glEnable( GL.GL_CULL_FACE );
			GL.glCullFace( GL.GL_BACK );
			GL.glFrontFace( GL.GL_CW );

			//this.CameraTransform.RotateZ( (float) Math.PI );

			GL.glErrorCheck();
		}
		
		//-----------------------------------------------------------------------------------
		//-----------------------------------------------------------------------------------

		public bool		IsCrossHairs	= false;
		public bool		IsGrid			= false;
		public Color	BackgroundColor		= Color.Black;

		//-----------------------------------------------------------------------------------

		protected World	_world = null;
		public World	World {
			get {	return	_world;		}
			set {	
				if( _world == value ) {
					return;
				}
				_world = value;
			}
		}
		
		//-----------------------------------------------------------------------------------
			
		protected RenderSettings	_renderSettings = new RenderSettings();
		public	RenderSettings	RenderSettings {
			get {	return	_renderSettings;	}
		}
		
		protected Renderers	_renderers = new Renderers();
		public	Renderers	AvailableRenderers {
			get {	return	_renderers;	}
		}

		protected Renderer	_renderer = null;
		public	Renderer	Renderer	{
			get	{	
				if( _renderer == null ) {
					_renderer = _renderers.FindByType( typeof( GouraudRenderer ) );
				}
				return	_renderer;
			}
			set	{
				_renderer = value;
				BugTracking.SetWatch( "Viewer.Renderer", _renderer );
			}
		}
				
		//-----------------------------------------------------------------------------------

		protected void OnIdle( object source, EventArgs e ) {
			if( this.World != null ) {
				this.Invalidate();
			}
		}
		

		//-----------------------------------------------------------------------------------

		protected void DrawCrossHairs() {
			Debug.Assert( this.IsCrossHairs == true );
			int size = 750;
			GL.glDisable( GL.Option.Texture2D );
			GL.glBegin( GL.Primative.Lines );
			GL.glColor( Color.FromArgb( 128, 255, 0, 0 ) );
			GL.glVertex3f(  0,  0,  0 );
			GL.glVertex3f( size,  0,  0 );
			GL.glColor( Color.FromArgb( 128, 0, 255, 0 ) );
			GL.glVertex3f( 0,  0,  0 );
			GL.glVertex3f( 0, size,  0 );
			GL.glColor( Color.FromArgb( 128, 0, 0, 255 ) );
			GL.glVertex3f(  0,  0,  0 );
			GL.glVertex3f(  0,  0, size );
			GL.glEnd();
			GL.glErrorCheck();
		}

		protected void DrawGrid() {
			Debug.Assert( this.IsGrid == true );
			GL.glDisable( GL.Option.Texture2D );
			GL.glColor( Color.FromArgb( 128, 92, 92, 92 ) );
			int size = 1500, step = 150;
			GL.glBegin( GL.Primative.Lines );
			for( int x = -size; x <= size; x += step ) {
				GL.glVertex3i( x, -size, 0 );
				GL.glVertex3i( x,  size, 0 );
			}
			for( int y = -size; y <= size; y += step ) {
				GL.glVertex3i( -size, y, 0 );
				GL.glVertex3i(  size, y, 0 );
			}
			GL.glEnd();
			GL.glErrorCheck();
		}

		protected DateTime	_datetimeBegin = System.DateTime.Now;
		protected int		_paintCount = 0, _paintRefresh = 3;
		protected int		_polygonCount = 0;

		
		protected override void OnPaint3D( PaintEventArgs e ) {
			// clear window
			GL.glClearColor( BackgroundColor.R / 255f, BackgroundColor.G / 255f, BackgroundColor.B / 255f, 0.0f );
			GL.glClearDepth( 1.0 );
			GL.glClear(
				( ( this.RenderSettings.Background ) ? 0 : GL.Buffers.Color ) |
				( ( this.RenderSettings.ZBuffer ) ? GL.Buffers.Depth : 0 ) );

			if( this.World != null ) {

				// update player
				this.World.Player.Advance();
				this.World.Camera.SetCamera( this.World.Player.GetLookFrameOfReference() );

				// set camera
				GL.glMatrixMode( GL.MatrixMode.ModelView );
				GL.glPushMatrix();	
				GL.glMultMatrixf( (float[]) this.World.Camera.Transform );

				if( this.IsCrossHairs ) {
					DrawCrossHairs();
				}
				if( this.IsGrid ) {
					DrawGrid();
				}

				_polygonCount += this.World.Render( this.RenderSettings );

				GL.glMatrixMode( GL.MatrixMode.ModelView );
				GL.glPopMatrix();
			}

			// clean up and flip
			GL.glFinish();
			this.SwapBuffer();	 
			GL.glErrorCheck();

			// update statistics
			UpdateStatistics();
		}

		protected void UpdateStatistics() {
			if( _paintCount % _paintRefresh == 0 ) {
				double totalSeconds = ( System.DateTime.Now - _datetimeBegin ).TotalSeconds;
				if( totalSeconds > 0.333 ) {
					
					int framesPerSecond = (int) Math.Round( _paintCount / totalSeconds );
					int	polysPerSecond = (int) Math.Round( _polygonCount / totalSeconds );
					int	polysPerFrame = (int) Math.Round( (float)_polygonCount / _paintCount );

					ExoEngine.MainForm.StatusBar.PanelTextA = "Frames/Sec: " +	framesPerSecond;
					ExoEngine.MainForm.StatusBar.PanelTextB = "Polys/Sec: " + polysPerSecond;
					ExoEngine.MainForm.StatusBar.PanelTextC = "Polys/Frame: " + polysPerFrame;
					
					_datetimeBegin = System.DateTime.Now;
					_paintRefresh = (int)( framesPerSecond * 0.4 );
					if( _paintRefresh <= 0 ) {
						_paintRefresh = 1;
					}
					_paintCount = 0;
					_polygonCount = 0;
				}
			}		
			_paintCount ++;
		}

		//-------------------------------------------------------------------------------

		public unsafe virtual Bitmap ToBitmap() {
			int[] b = new int[4];
			GL.glGetIntegerv(GL.GL_VIEWPORT, b);
			
			Bitmap img = new Bitmap( b[2], b[3], System.Drawing.Imaging.PixelFormat.Format24bppRgb );
			BitmapData tex;
			Rectangle rect = new Rectangle(0, 0, b[2], b[3]);
			tex = img.LockBits(rect, ImageLockMode.ReadWrite, 
				System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			
			GL.glReadPixels( b[0], b[1], b[2], b[3], GL.GL_BGR_EXT, GL.GL_UNSIGNED_BYTE, (void*) tex.Scan0 );
			
			img.UnlockBits(tex);
			img.RotateFlip(RotateFlipType.RotateNoneFlipY);
			return img;
		}

		//-------------------------------------------------------------------------------

		protected override void OnSizeChanged( EventArgs e ) {
			base.OnSizeChanged( e );
			SetupViewport( Size.Width, Size.Height );
			SetupFrustum( Size.Width, Size.Height, sNearClippingPlane, sFarClippingPlane );
		}

		protected void	SetupViewport( int width, int height ) {
			// if width or height are invalid do nothing
			if ( width == 0 || height == 0 ) {
				return;
			}

			GL.glMatrixMode( GL.MatrixMode.ModelView );
			GL.glViewport( 0, 0, width, height );
		}
		protected void	SetupFrustum( int width, int height, float fNearClip, float fFarClip ) {
			// if width or height are invalid do nothing
			if ( width == 0 || height == 0 ) {
				return;
			}

			float	xRatio = 50f;
			float	yRatio = 50f;
			if( width > height ) {
				xRatio *= ( (float) width ) / height;
			}
			else {
				yRatio *= ( (float) height ) / width;
			}
			GL.glMatrixMode( GL.MatrixMode.Projection );
			GL.glLoadIdentity();
			GL.glFrustum( -xRatio, xRatio, yRatio, -yRatio, fNearClip, fFarClip );
			if( ExoEngine.ActiveWorld != null ) {
				ExoEngine.ActiveWorld.Camera.SetClipDistances( fNearClip, fFarClip );
				ExoEngine.ActiveWorld.Camera.SetViewport( xRatio * 2, yRatio * 2 );
			}
			GL.glErrorCheck();
		}

		//-------------------------------------------------------------------------------

		/*public void OnActiveDocumentChanged( Document document ) {
			if( _renderer != null ) {
				_renderer.VoxelSpace = (( document != null ) ? document.VoxelSpace : null );
			}

			this.Invalidate();
			this.Update();
		}*/

		//-------------------------------------------------------------------------------

		/*protected	SelectionManipulator	_navPointer		= new SelectionManipulator();
		protected	TrackballManipulator	_navTrackball	= new TrackballManipulator();
		protected	ZoomManipulator		_navZoom		= new ZoomManipulator();
		protected	PanManipulator		_navPan			= new PanManipulator();

		protected	bool	_bLeftButtonDown = false;
		protected	bool	_bRightButtonDown = false;

		protected	Manipulator	GetManipulator( MouseButtons mb, bool bButtonDown ) {
			//Debug.WriteLine( "MouseButtons = " + ((int)mb) );
			if( mb == MouseButtons.Left ) {
				_bLeftButtonDown = bButtonDown;
			}
			if( mb == MouseButtons.Right ) {
				_bRightButtonDown = bButtonDown;
			}

			if( _bLeftButtonDown && ! _bRightButtonDown ) {
				return	_navTrackball;
			}
			if( ! _bLeftButtonDown && _bRightButtonDown ) {
				return	_navZoom;
			}
			if( _bLeftButtonDown && _bRightButtonDown ) {
				return	_navPan;
			}

			return	_navPointer;
		}

		//-------------------------------------------------------------------------------

		protected	Manipulator		_navCurrent	= null;

		protected	void	UpdateManipulator( MouseButtons mb, bool bButtonDown, Point pt ) {
			Manipulator	nav = GetManipulator( mb, bButtonDown );
			if( nav != _navCurrent ) {
				if( _navCurrent != null ) {
					_navCurrent.Disengage();
					this.Cursor = Cursors.Arrow;
				}
				_navCurrent = nav;
				if( _navCurrent != null ) {
					_navCurrent.Engage( this, pt );
					this.Cursor = _navCurrent.Cursor;
				}
			}
		}

		protected override void OnMouseDown( MouseEventArgs e ) {
			UpdateManipulator( e.Button, true, new Point( e.X, e.Y ) );
		}
		protected override void OnMouseUp( MouseEventArgs e ) {
			UpdateManipulator( e.Button, false, new Point( e.X, e.Y ) );
		}

		protected override void OnMouseMove( MouseEventArgs e ) {
			if( _navCurrent != null ) {
				Matrix3D xfrm = _navCurrent.Update( new Point( e.X, e.Y ) );
				if( xfrm != null ) {
					_xfrmView = xfrm * _xfrmView;
					this.Invalidate();
					this.Update();
				}
			}  
		}	 */
			
		//-------------------------------------------------------------------------------

	}

}
