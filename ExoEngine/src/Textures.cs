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
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using Exocortex;
using Exocortex.Collections;
using Exocortex.OpenGL;

namespace ExoEngine {

	[XmlType("textureList")]
	public class Textures : CollectionBase2 {

		//static protected int s_nextID = 0;
		//protected int _iID = s_nextID ++;

		public Textures() : base() {
		}
		~Textures() {
			foreach( Texture texture in this.List ) {
				Debug.Assert( texture.IsOpenGLLoaded == false, texture.FileName );
			}
			//foreach( Texture texture in this.List ) {
			//	Debug.Assert( texture.ParentTextures == this );
			//	texture.ParentTextures = null;
			//}
		}

		/*public virtual Texture CreateTexture( string fileName ) {
			if( File.Exists( fileName ) == false && this.DefaultPath != null ) {
				//Debug.WriteLine( "can not find file: '" + strFileName + "'" );
				fileName = Path.Combine( this.DefaultPath, Path.GetFileName( fileName ) );
			}
			if( File.Exists( fileName ) == false && this.DefaultExtension != null ) {
				//Debug.WriteLine( "can not find file: '" + strFileName + "'" );
				fileName = Path.ChangeExtension( fileName, this.DefaultExtension );
			}
			if( File.Exists( fileName ) == false ) {
				//Debug.WriteLine( "can not find file: '" + strFileName + "', giving up." );
				return null;
			}

			Texture texture = this.FindByFileName( fileName );
			if( texture == null ) {
				texture = new Texture( fileName );
				this.Add( texture );
			}

			return texture;
		}*/

		public virtual Texture FindByFileName( string fileName ) {
			Debug.Assert( fileName != null );
			fileName = Path.GetFileName( fileName );
			foreach( Texture texture in this.List ) {
				if( texture.FileName == fileName ) {
					return texture;
				}
			}
			return null;
		}

		public virtual Texture RequestTexture( string textureName ) {
			Debug.Assert( textureName != null );

			string shortTextureName = Path.GetFileNameWithoutExtension( textureName );
			Texture texture = this.FindByFileName( shortTextureName );
			if( texture == null ) {
				texture = new Texture( shortTextureName );
				texture.MinFilter		= this.DefaultMinFilter;
				texture.MaxFilter		= this.DefaultMaxFilter;
				texture.Mipmap			= this.DefaultMipmap;
				this.Add( texture );
			}

			return texture;
		}

		/*protected string	GetFileName( string fileName ) {
			if( File.Exists( fileName ) == true ) {
				return fileName;
			}
			
			if( this.DefaultPath != null ) {
				fileName = Path.Combine( this.DefaultPath, Path.GetFileName( fileName ) );
				if( File.Exists( fileName ) == true ) {
					return fileName;
				}
			}

			if( this.DefaultExtension != null ) {
				fileName = Path.ChangeExtension( fileName, this.DefaultExtension );
				if( File.Exists( fileName ) == true ) {
					return fileName;
				}
			}
			
            return null;
		}  */

		public virtual void Add( Texture texture ) {
			//Debug.Assert( texture.ParentTextures == null );
			this.List.Add( texture );
			//texture.ParentTextures = this;
		}

		public virtual Texture this[ int index ] {
			get {	return (Texture) this.List[index]; 	}
		}

		//-----------------------------------------------------------------------------------

		/*protected string _defaultPath = null;
		public string DefaultPath {
			get {	return	_defaultPath;	}
			set	{	_defaultPath = value;	}
		}
		
		protected string _defaultExtension = ".bmp";
		public string DefaultExtension{
			get {	return	_defaultExtension;	}
			set	{	_defaultExtension= value;	}
		}	*/

		protected GL.TextureFilter _defaultMinFilter = GL.TextureFilter.Nearest;
		public GL.TextureFilter DefaultMinFilter{
			get {	return	_defaultMinFilter;	}
			set	{	_defaultMinFilter = value;	}
		}

		protected GL.TextureFilter _defaultMaxFilter = GL.TextureFilter.Nearest;
		public GL.TextureFilter DefaultMaxFilter{
			get {	return	_defaultMaxFilter;	}
			set	{	_defaultMaxFilter = value;	}
		}

		protected bool	_defaultMipmap	= false;
		public bool DefaultMipmap {
			get {	return	_defaultMipmap;	}
			set	{	_defaultMipmap= value;	}
		}

		//-----------------------------------------------------------------------------------

		public void	OverideTextureQuality( bool bMipmap, GL.TextureFilter minFilter, GL.TextureFilter maxFilter ) {
			foreach( Texture texture in this.List ) {
				if( texture.Mipmap != bMipmap || texture.MinFilter != minFilter || texture.MaxFilter != maxFilter ) {
					bool bGLLoaded = texture.IsOpenGLLoaded;
					if( bGLLoaded ) {
						texture.UnloadOpenGL();
					}
					texture.Mipmap		= bMipmap;
					texture.MinFilter	= minFilter;
					texture.MaxFilter	= maxFilter;
					if( bGLLoaded ) {
						texture.LoadOpenGL();
					}
				}
			}
			this.DefaultMipmap			= bMipmap;
			this.DefaultMinFilter		= minFilter;
			this.DefaultMaxFilter		= maxFilter;
		}
		
		//-----------------------------------------------------------------------------------

		//public void Reset() {
			//foreach( Texture texture in this ) {
			//	if( texture.ParentTextures != this ) {
			//		Debug.Assert( texture.ParentTextures == null );
			//		texture.ParentTextures = this;
			//	}
			//}			
		///	LoadAll();
		//}
		public void LoadAll() {
			foreach( Texture texture in this ) {
				//Debug.Assert( texture.ParentTextures == this );
				if( texture.IsLoaded == false ) {
					texture.Load();
				}
			}
		}

		public void UnloadAll() {
			foreach( Texture texture in this.List ) {
				//Debug.Assert( texture.ParentTextures == this );
				//GL.glErrorCheck();
				if( texture.IsUnloaded == false ) {
					texture.Unload();
					GL.glErrorCheck();
				}
				GL.glErrorCheck();
			}
		}
		public void	FromArray( Texture[] array ) {
			this.Clear();
			foreach( Texture o in array ) {
				this.Add( o );
			}
		}
		public Texture[] ToArray( bool bRemoveInteralUseTextures ) {
			IList textures = null;
			if( bRemoveInteralUseTextures ) {
				textures = new ArrayList();
				IEnumerator enumerator = this.List.GetEnumerator();
				while( enumerator.MoveNext() ) {
					Texture texture = (Texture) enumerator.Current;
					if( texture.InternalUse == false ) {
						textures.Add( texture );
					}
				}
			}
			else {
				textures = this.List;
			}
			Texture[] array = new Texture[ textures.Count ];
			textures.CopyTo( array, 0 );
			return array;

		}

		//-----------------------------------------------------------------------------------

	}
}
