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
using System.Xml.Serialization;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;
using Exocortex;
using Exocortex.Imaging;
using Exocortex.OpenGL;

namespace ExoEngine {

	[XmlType("texture")]
	public class Texture {
	
		//------------------------------------------------------------------------------

		public Texture() {
		}
		public Texture( string fileName ) {
			Debug.Assert( fileName != null );
			_fileName = Path.GetFileNameWithoutExtension( fileName );
		}
		public Texture( int width, int height, uint[] data ) {
			Debug.Assert( width*height > 0 );
			Debug.Assert( data != null );

			_width			= width;
			_height			= height;
			_data			= data;
			_fileName		= "InternalUse #" + _iID;
			_internalUse	= true;
		}

		public Texture Clone() {
			Texture texture = new Texture();
			texture._fileName	= this._fileName;
			texture._bClamp		= this._bClamp;
			texture._bMipmap	= this._bMipmap;
			texture._minFilter	= this._minFilter;
			texture._maxFilter	= this._maxFilter;
			texture._internalUse	= this._internalUse;
			// leave Bitmap fields uninitialized.
			// leave OpenGL fields uninitialized.
			// leave ParentTextures field uninitialized.
			return texture;
		}
		
		~Texture() {
			Debug.Assert( this.IsOpenGLLoaded == false, "did not release OpenGL texture handle" );
		}

		//------------------------------------------------------------------------------

		static protected int s_nextID = 0;

		protected int	_iID = s_nextID ++;

		protected string _fileName = null;
		[XmlAttribute("fileName")]
		public string FileName {
			get {	return	_fileName;	}
			set {	_fileName = value;	}
		}
		
		/*protected Textures _parentTextures = null;
		[XmlIgnore]
		public Textures ParentTextures {
			get {	return	_parentTextures;	}
			set {	_parentTextures = value;	}
		}*/

		protected bool _bClamp = false;
		[XmlAttribute("clamp")]
		public bool Clamp {
			get {	return	_bClamp;	}
			set {	
				Debug.Assert( this.IsOpenGLLoaded == false );
				_bClamp = value;
			}
		}

		protected bool _bMipmap = false;
		[XmlAttribute("mipmap")]
		public bool Mipmap {
			get {	return	_bMipmap;	}
			set {
				Debug.Assert( this.IsOpenGLLoaded == false );
				_bMipmap = value;
			}
		}

		protected bool _internalUse = false;
		[XmlIgnore]
		public bool InternalUse {
			get {	return	_internalUse;	}
			set {	_internalUse = value;	}
		}

		protected GL.TextureFilter	_minFilter = GL.TextureFilter.Linear;
		[XmlAttribute("minFilter")]
		public GL.TextureFilter	MinFilter {
			get {	return	_minFilter;	}
			set {
				Debug.Assert( this.IsOpenGLLoaded == false );
				_minFilter = value;
			}
		}

		protected GL.TextureFilter	_maxFilter = GL.TextureFilter.Linear;
		[XmlAttribute("maxFilter")]
		public GL.TextureFilter	MaxFilter {
			get {	return	_maxFilter;	}
			set {
				Debug.Assert( this.IsOpenGLLoaded == false );
				_maxFilter = value;
			}
		}

		//------------------------------------------------------------------------------

		[XmlIgnore]
		public bool IsLoaded {
			get {	return	this.IsBitmapLoaded && this.IsOpenGLLoaded;	}
		}
		
		public void Load() {
			//Debug.WriteLine( "FileName = " + _strFileName + " ID = " + _iID );

			Debug.Assert( this.IsLoaded == false );

			if( ! this.IsBitmapLoaded ) {
				this.LoadBitmap();
			}
			if( ! this.IsOpenGLLoaded ) {
				this.LoadOpenGL();
			}

			Debug.Assert( this.IsLoaded == true );
		}

		[XmlIgnore]
		public bool IsUnloaded {
			get {
				return	( this.IsBitmapLoaded == false ) &&
					( this.IsOpenGLLoaded == false );
			}
		}

		public void Unload() {
			//Debug.WriteLine( "FileName = " + _strFileName + " ID = " + _iID );
			Debug.Assert( this.IsUnloaded == false ); 
			if( this.IsOpenGLLoaded == true ) {
				this.UnloadOpenGL();
			}
			if( this.IsBitmapLoaded == true ) {
				this.UnloadBitmap();
			}
			Debug.Assert( this.IsUnloaded == true ); 
		}

		//------------------------------------------------------------------------------

		protected int _width = 0, _height = 0;
		[XmlIgnore]
		public int Width {
			get {	return	_width;		}
		}
		[XmlIgnore]
		public int Height {
			get {	return	_height;	}
		}

		protected uint[] _data = null;
		[XmlIgnore]
		public uint[] Data {
			get {	return	_data;	}
		}

		[XmlIgnore]
		public bool IsBitmapLoaded {
			get {
				Debug.Assert( ( _data != null ) == ( _width > 0 ) );
				Debug.Assert( ( _data != null ) == ( _height > 0 ) );
				return	( _data != null );
			}
		}

		public unsafe void LoadBitmap() {
			//Debug.WriteLine( "FileName = " + _strFileName + " ID = " + _iID );
			//Debug.Assert( this.ParentTextures != null );
			Debug.Assert( this.IsBitmapLoaded == false );
			Debug.Assert( this.FileName != null );

			string fileName = Path.Combine( ExoEngine.sTexturePath, Path.GetFileNameWithoutExtension( this.FileName ) );

			foreach( string extension in ExoEngine.sTextureExtensions ) {
				fileName = Path.ChangeExtension( fileName, extension );
				if( File.Exists( fileName ) == true ) {
					break;
				}
			}

			if( File.Exists( fileName ) == false ) {
				throw new FileNotFoundException( "'" + this.FileName + "' couldn't be resolved to a file", "this.FileName" );
			}

			Bitmap bitmap = new Bitmap( fileName );
			Debug.Assert( bitmap != null );

			_width  = Math2.RoundToBase( bitmap.Width, 2 );
			_height = Math2.RoundToBase( bitmap.Height, 2 );

			// resize to a power of two if necessary.
			if( _width != bitmap.Width || _height != bitmap.Height ) {
				bitmap = new Bitmap( bitmap, _width, _height );
			}

			_data =	BitmapUtils.ConvertBitmapToRGBAArray( bitmap );
			BitmapUtils.FlipVerticallyRGBAArray( _data );

			// load bitmap data, and flip it
			/*int size = _width * _height;
			_data = new uint[ size ];
			Rectangle	rect		= new Rectangle( 0, 0, _width, _height );
			BitmapData	bitmapdata	= bitmap.LockBits( rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
			uint* pBitmapData = (uint*) bitmapdata.Scan0.ToPointer();
			for( int i = 0; i < size; i ++ ) {
				_data[size - i - 1] = pBitmapData[i];
			}
			bitmap.UnlockBits( bitmapdata );*/

			Debug.Assert( this.IsBitmapLoaded == true );
		}

		public void UnloadBitmap() {
			//Debug.WriteLine( "FileName = " + _strFileName + " ID = " + _iID );
			Debug.Assert( this.IsBitmapLoaded == true );

			_data	= null;
			_width	= 0;
			_height	= 0;
			
			Debug.Assert( this.IsBitmapLoaded == false );
		}

		//------------------------------------------------------------------------------

		protected int _handle = -1;
		[XmlIgnore]
		public uint OpenGLHandle {
			get {	return	(uint) _handle;	}
		}

		protected Thread	_threadParent = null;

		[XmlIgnore]
		public bool IsOpenGLLoaded {
			get {	return _handle != -1;	}
		}

		static public int OpenGLTextureCount = 0;

		public unsafe void LoadOpenGL() {
			//Debug.Assert( this.ParentTextures != null );
			Debug.Assert( this.IsBitmapLoaded == true );
			Debug.Assert( this.IsOpenGLLoaded == false );

			// load opengl texture
			GL.glEnable( GL.Option.Texture2D );
			
			// create new OpenGL texture handle if needed.
			if( _handle == -1 ) {
				uint[] handles = new uint[1];
				GL.glGenTextures( 1, handles );
				_handle = (int) handles[0];
				//OpenGLTextureCount ++;
				GL.glErrorCheck();
			}

			GL.glBindTexture( GL.Texture.Texture2D, (uint) _handle );
			GL.glErrorCheck();

			if( this.Clamp ) {
				GL.glTexParameteri( GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, GL.GL_CLAMP );
				GL.glTexParameteri( GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, GL.GL_CLAMP );
			}
			else {
				GL.glTexParameteri( GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, GL.GL_REPEAT );
				GL.glTexParameteri( GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, GL.GL_REPEAT );
			}
			GL.glErrorCheck();

			if( this.Mipmap == false ) {
				Debug.Assert( this.MinFilter != GL.TextureFilter.LinearMipmapLinear, this.Mipmap + "  " + this.MinFilter );
				Debug.Assert( this.MinFilter != GL.TextureFilter.LinearMipmapNearest, this.Mipmap + "  " + this.MinFilter );
				Debug.Assert( this.MinFilter != GL.TextureFilter.NearestMipmapLinear, this.Mipmap + "  " + this.MinFilter );
				Debug.Assert( this.MinFilter != GL.TextureFilter.NearestMipmapNearest, this.Mipmap + "  " + this.MinFilter );
			}
			Debug.Assert( this.MaxFilter != GL.TextureFilter.LinearMipmapLinear, this.Mipmap + "  " + this.MaxFilter );
			Debug.Assert( this.MaxFilter != GL.TextureFilter.LinearMipmapNearest, this.Mipmap + "  " + this.MaxFilter );
			Debug.Assert( this.MaxFilter != GL.TextureFilter.NearestMipmapLinear, this.Mipmap + "  " + this.MaxFilter );
			Debug.Assert( this.MaxFilter != GL.TextureFilter.NearestMipmapNearest, this.Mipmap + "  " + this.MaxFilter );
			
			GL.glTexParameteri( GL.Texture.Texture2D, GL.TexParamPName.TextureMinFilter, this.MinFilter );
			GL.glErrorCheck();
			GL.glTexParameteri( GL.Texture.Texture2D, GL.TexParamPName.TextureMagFilter, this.MaxFilter );
			GL.glErrorCheck();

			fixed( uint *pData = & (_data[0]) ) {
				if( this.Mipmap == true ) {
					GLU.gluBuild2DMipmaps( GL.GL_TEXTURE_2D, (int) GL.GL_RGBA8, _width, _height,
						GL.GL_BGRA_EXT, GL.GL_UNSIGNED_BYTE, (void*) pData );
					GL.glErrorCheck();
				}
				else {
					GL.glTexImage2D( GL.Texture.Texture2D, 0, GL.InternalFormat.RGBA8, _width, _height, 0,
						GL.PixelFormat.BGRAExt, GL.DataType.UnsignedByte, (void*) pData );	
					GL.glErrorCheck();
				}
			}

			_threadParent = Thread.CurrentThread;

			Debug.Assert( this.IsOpenGLLoaded == true );
		}

		public void UnloadOpenGL() {
			Debug.Assert( this.IsOpenGLLoaded == true );
			
			Thread threadCurrent = Thread.CurrentThread; 
			if( _threadParent == threadCurrent ) {
				uint[] handles = new uint[1];
				handles[0] = (uint) _handle;
				GL.glDeleteTextures( 1, handles );
				_handle = -1;
				//OpenGLTextureCount --;
				GL.glErrorCheck();
			}
			else {
				Debug.WriteLine( "Texture.UnloadOpenGL() -- unable to unload, parent thread != current thread" );
				Debug.WriteLine( "   _threadParent  = " + _threadParent.Name );
				Debug.WriteLine( "   threadCurrentd = " + threadCurrent.Name );
				_handle = -1;
			}

			Debug.Assert( this.IsOpenGLLoaded == false );
		}
	}

}
