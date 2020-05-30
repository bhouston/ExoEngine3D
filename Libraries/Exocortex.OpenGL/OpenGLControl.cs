/*
 * BSD Licence:
 * Copyright (c) 2001, Lloyd Dupont (lloyd@galador.net)
 * <ORGANIZATION> 
 * All rights reserved.
 * 
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
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
//using Exocortex;

namespace Exocortex.OpenGL {
	[StructLayout(LayoutKind.Sequential)]
	internal class PPIXELFORMATDESCRIPTOR {

		internal PIXELFORMATDESCRIPTOR pix;

		public unsafe PPIXELFORMATDESCRIPTOR() {
			pix.nSize = (ushort) sizeof( PIXELFORMATDESCRIPTOR );
		}
		public PPIXELFORMATDESCRIPTOR( PIXELFORMATDESCRIPTOR aPix ) {
			pix = aPix;
		}
	}

	/// this "an opengl view" which means you could issue OpenGL
	/// command in its OnPaint method.
	/// It should also let you have a good control on pixel format
	/// and such stuff.
	/// Its ultimate goal is to disappear when SDL could be used
	/// with ordinary windows hierarchy. For now, with it, you
	/// could have OpenGL window mixed in an ordinary C# winform
	/// API.
	/// Of course to use it you must have a good knowledge of OpenGL
	/// API, you could bye the red & blue book, have a look at
	/// http://www.opengl.org.
	public class OpenGLControl : UserControl {
		
		//-------------------------------------------------------------------------------

		[DllImport("ExocortexNative")]
		static extern IntPtr	oglnCreateGC( IntPtr hWnd );
		[DllImport("ExocortexNative")]
		static extern bool		oglnReleaseGC( IntPtr hGC );
		[DllImport("ExocortexNative")]
		static extern void		oglnSetCurrentGC( IntPtr hGC );
		[DllImport("ExocortexNative")]
		static extern int		oglnGetNumPixelFormats( IntPtr hGC );
		[DllImport("ExocortexNative")]
		static extern int		oglnGetPixelFormats( IntPtr hGC, int index, PPIXELFORMATDESCRIPTOR pix );
		[DllImport("ExocortexNative")]
		static extern int		oglnGetCurrentFormat( IntPtr hGC);
		[DllImport("ExocortexNative")]
		static extern void		oglnSetNearestPixelFormat( IntPtr hGC, PPIXELFORMATDESCRIPTOR pix );
		[DllImport("ExocortexNative")]
		static extern void		oglnSetPixelFormat( IntPtr hGC, int format );
		[DllImport("ExocortexNative")]
		static extern void		oglnSwapBuffers( IntPtr hGC );

		//-------------------------------------------------------------------------------

		public OpenGLControl() {
			if( this.DesignMode ) {
			}
			else {
				_threadParent = Thread.CurrentThread;
				//Debug2.TraceThread();

				_hGC = oglnCreateGC( this.Handle );
				if( _hGC == IntPtr.Zero ) {
					throw new OpenGLException();
				}

				PixelFormat = PreferredPixelFormat;
			
				oglnSetCurrentGC( _hGC );

				InitGLContext();
				_openGLInitialized = true;
			}
			this.SetStyle( ControlStyles.ResizeRedraw, true );
		}

		~OpenGLControl() {
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false)is optimal in terms of
			// readability and maintainability.
			this.Dispose( false );
		}

		protected override void Dispose( bool disposing ) {
			if( this.DesignMode ) {
			}
			else {
				// If disposing equals true, dispose all managed 
				// and unmanaged resources.
				if( disposing ) {
				}
		
				// Release unmanaged resources. If disposing is false, 
				// only the following code is executed.
				if( _hGC != IntPtr.Zero ) {
					if( ! ( _threadParent == Thread.CurrentThread ) ) {
						string ownerName = ( _threadParent != null ) ? _threadParent.Name : "(null)";
						throw new OpenGLException( "disposing thread (" + Thread.CurrentThread.Name +") is not context owner thread (" + ownerName + ")" );
					}
					if( oglnReleaseGC( _hGC ) == false ) {
						throw new OpenGLException( "oglnReleaseGC( " + _hGC.ToString() + " ) failed" );
					}
					_hGC = IntPtr.Zero;
				}
			}
			base.Dispose( disposing );
		}

		//-------------------------------------------------------------------------------

		private void InitializeComponent() {
			// 
			// OpenGLControl
			// 
			this.Name = "OpenGLControl";
			this.Size = new System.Drawing.Size(360, 336);

		}
	
		//-------------------------------------------------------------------------------

		internal bool	_openGLInitialized = false;
		public bool	OpenGLInitialized {
			get	{	return	_openGLInitialized;		}
		}

		//-------------------------------------------------------------------------------

		[Browsable( false )]
		internal IntPtr _hGC = IntPtr.Zero;
		[Browsable( false )]
		internal Thread	_threadParent = null;
		
		/// this method is to be called each time a GL context is created.
		/// for example at the creation of the Control, before printing, etc..
		protected virtual void InitGLContext() {}
		
		/// get a list of available fomat. sadly it is not static
		/// as the underlying system call need a window id (HWND)
		[Browsable( false )]
		public PIXELFORMATDESCRIPTOR[] AvailablePixelFormats {
			get {
				if( this.DesignMode ) {
					return	null;
				}
				else {
					int formats = oglnGetNumPixelFormats( _hGC );
					if( formats == 0 ) {
						throw new OpenGLException();
					}
					PIXELFORMATDESCRIPTOR[] ret = new PIXELFORMATDESCRIPTOR[ formats ];
					PPIXELFORMATDESCRIPTOR ptr = new PPIXELFORMATDESCRIPTOR();
					for( int i = 0; i < formats; i++ ) {
						if( oglnGetPixelFormats( _hGC, i + 1, ptr ) == 0 ) {
							throw new OpenGLException();
						}
						ret[i] = ptr.pix;
					}
					return ret;
				}
			}
		}
		/// return current pixel format index or set one. it is a 0 based
		/// index unlike low level function. this index is the one in
		/// AvailablePixelFormats
		[Browsable( false )]
		internal int PixelFormatIndex {
			get {
				if( this.DesignMode ) {
					return	0;
				}
				else {
					int pixfmt = oglnGetCurrentFormat( _hGC ) - 1;
					if( pixfmt < 0 ) {
						throw new OpenGLException();
					}
					return pixfmt;
				}
			}
			set {
				if( this.DesignMode ) {
				}
				else {
					oglnSetPixelFormat( _hGC, value + 1 );
				}
			}
		}
		
		/// get and set current pixel format, should be set in 
		/// before any drawing. note that it is automatically set
		/// to preferred pixel format in constructor.
		/// Beware call this method destroy your current wglContext.
		/// <BUG>it sometimes fail for no good reason</BUG>
		[Browsable( false )]
		public virtual PIXELFORMATDESCRIPTOR PixelFormat {
			get {
				if( this.DesignMode ) {
					PPIXELFORMATDESCRIPTOR ptr = new PPIXELFORMATDESCRIPTOR();
					return	ptr.pix;
				}
				else {
					PPIXELFORMATDESCRIPTOR ptr = new PPIXELFORMATDESCRIPTOR();
					if( oglnGetPixelFormats( _hGC, PixelFormatIndex + 1, ptr ) == 0 ) {
						throw new OpenGLException();
					}
					return ptr.pix;
				}
			}
			set {
				if( this.DesignMode ) {
				}
				else {
					PPIXELFORMATDESCRIPTOR ptr = new PPIXELFORMATDESCRIPTOR( value );
					oglnSetNearestPixelFormat( _hGC, ptr );
				}
			}
		}
		
		/// default PIXELFORMATDESCRIPTOR for newly created OpenGL 
		/// control
		[Browsable( false )]
		public static PIXELFORMATDESCRIPTOR DefaultPixelFormat =
			new PIXELFORMATDESCRIPTOR(
			PixelFlag.PFD_DRAW_TO_WINDOW // support window
			|PixelFlag.PFD_SUPPORT_OPENGL // support OpenGL
			|PixelFlag.PFD_DOUBLEBUFFER, 24, 16);

		/// get the preferred pixel format, call by constructor
		/// to set the PIXELFORMAT, by default its value is 
		/// the DefaultPixelFormat static property.
		[Browsable( false )]
		public virtual PIXELFORMATDESCRIPTOR PreferredPixelFormat {
			get { return DefaultPixelFormat; }
		}

		/// call it to SwapBuffer if you are double buffered.
		/// it is also a good idea to do it before doing standart
		/// gdi drawing if you want to use this feature.
		protected virtual void SwapBuffer() {
			if( this.DesignMode ) {
			}
			else {
				if( _threadParent == Thread.CurrentThread ) {
					oglnSwapBuffers( _hGC );
				}
				else {
					//Debug2.TraceThread();
					Debug.WriteLine( "OpenGLControl: SwapBuffer() called from unknown thread: " + Thread.CurrentThread.Name );
				}
			}
		}

		protected override void OnPaintBackground( PaintEventArgs e ) {
		}

		protected override void OnPaint( PaintEventArgs e ) {
			if( this.DesignMode ) {
				Graphics g = e.Graphics;
				int x = this.Size.Width;
				int y = this.Size.Height;
				g.SetClip( new Rectangle( 0, 0, x, y ) );
				g.Clear( this.BackColor );
				g.DrawRectangle( new Pen( this.ForeColor, 3 ), 0, 0, x - 1, y - 1 );
				string name = this.Name;
				if( name == null || name == "" ) {
					name = "OpenGLControl";
				}
				g.DrawString( name, new Font( "Arial", 10 ), new SolidBrush( this.ForeColor ), 10, 10 );
			}
			else {
				Debug.Assert( _threadParent == Thread.CurrentThread );
				oglnSetCurrentGC( _hGC );
				this.OnPaint3D( e );
			}
		}

		protected virtual void OnPaint3D( PaintEventArgs e ) {
		}
		
		/// set glViewport. subclass to set frustrum...
		protected override void OnSizeChanged( EventArgs e ) {
			if( this.DesignMode ) {
			}
			else {
				if( _threadParent == Thread.CurrentThread ) {
					oglnSetCurrentGC( _hGC );
				}
				else {
					//Debug2.TraceThread();
					Debug.WriteLine( "OpenGLControl: OnSizeChanged() called from unknown thread: " + Thread.CurrentThread.Name );
				}
			}
			base.OnSizeChanged( e );
		}
	}
}
