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
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using System.Drawing;

using Exocortex;
using Exocortex.Collections;
using Exocortex.Geometry3D;
using Exocortex.OpenGL;

using ExoEngine.Rendering;

namespace ExoEngine.Geometry
{
	/// <summary>
	/// Summary description for SkyBox.
	/// </summary>
	public class SkyBox {

		public SkyBox() {
		}

		public SkyBox( string texturePrefix ) {
			_texturePrefix = texturePrefix;
		}

		protected string _texturePrefix = null;
		[XmlAttribute]
		public string TexturePrefix {
			get {	return	_texturePrefix;	}
			set {	_texturePrefix = value;	}
		}

		public void Reset( World world ) {
			//Debug.WriteLine( "World.Textures.Count = " + world.Textures.Count  );
			// create 6 faces
			Vector3D forward = new Vector3D( 0, 500, 0 );
			Vector3D left = new Vector3D( 500.1f, 0, 0 );
			Vector3D up = new Vector3D( 0, 0, 500.1f );
			
			this.Faces.Add( this.CreateFace( forward, up, left, world.Textures, _texturePrefix + "-front" ) );
			this.Faces.Add( this.CreateFace( -left, up, forward, world.Textures, _texturePrefix + "-right" ) );
			this.Faces.Add( this.CreateFace( -forward, up, -left, world.Textures, _texturePrefix + "-back"  ) );
			this.Faces.Add( this.CreateFace( left, up, -forward, world.Textures, _texturePrefix + "-left" ) );
			this.Faces.Add( this.CreateFace( up, -forward, left, world.Textures, _texturePrefix + "-up" ) );
			this.Faces.Add( this.CreateFace( -up, forward, left, world.Textures, _texturePrefix + "-down" ) );

			//Debug.WriteLine( "World.Textures.Count = " + world.Textures.Count  );
		}

		protected Face CreateFace( Vector3D axis, Vector3D x, Vector3D y, Textures textures, string textureFileName ) {
			Face face = new Face();
			
			// create verticies and texture coordinates
			face.Points.Add( axis + x + y );
			face.TextureCoords.Add( new Vector3D( 255f/256, 255f/256, 1f/256 ) );
			face.Points.Add( axis - x + y );
			face.TextureCoords.Add( new Vector3D( 255f/256, 1f/256, 1f/256 ) );
			face.Points.Add( axis - x - y );
			face.TextureCoords.Add( new Vector3D( 1f/256, 1f/256, 1f/256 ) );
			face.Points.Add( axis + x - y );
			face.TextureCoords.Add( new Vector3D( 1f/256, 255f/256, 1f/256 ) );

			face.Points.Reverse();
			face.TextureCoords.Reverse();

			face.Plane		= Plane3D.FromNormalAndPoint( -axis, axis );
			face.Texture	= textures.RequestTexture( textureFileName );
			face.Texture.Clamp			= true;
			face.Texture.InternalUse	= true;
			//if( face.Texture.IsLoaded == false ) {
			//	face.Texture.Load();
			//	Debug.Assert( face.Texture.IsLoaded == true, "can not load texture: " + face.Texture.FileName );
			//}

			return face;
		}

		public int Render( World world, RenderSettings settings ) {

			Quaternion q = world.Camera.Transform.ExtractRotation();
			Matrix3D xfrm = (Matrix3D) q;

			GL.glPolygonMode( GL.GL_FRONT_AND_BACK, ( settings.Wireframe ) ? GL.GL_LINE : GL.GL_FILL);
	
			GL.glPushMatrix();
			GL.glLoadMatrixf( (float[]) xfrm );
 			
			GL.glDisable( GL.GL_DEPTH_TEST );
			GL.glDisable( GL.GL_BLEND );

			if( settings.Textures == true ) {
				GL.glEnable( GL.GL_TEXTURE_2D );
			}
			else {
				GL.glDisable( GL.GL_TEXTURE_2D );
			}

			if( settings.Textures == true ) {
				GL.glDisable( GL.GL_COLOR_MATERIAL );
				GL.glColor( Color.White );
			}
			else {
				GL.glEnable( GL.GL_COLOR_MATERIAL );
				GL.glColor( Color.DarkMagenta );
			}

			foreach( Face face in this.Faces ) {
				GL.glBindTexture( GL.Texture.Texture2D, face.Texture.OpenGLHandle );
				GL.glBegin( GL.Primative.Polygon );
				int count = face.Points.Count;
				for( int i = 0; i < count; i ++ ) {
					Vector3D w = face.Points[i];
					Vector3D t = face.TextureCoords[i];
					GL.glTexCoord2f( t.X, t.Y );
					GL.glVertex3f( w.X, w.Y, w.Z );
				}
				GL.glEnd();
			}

			GL.glPopMatrix();

			return	6;
		}

		protected Faces _faces = new Faces();
		[XmlIgnore]
		public Faces Faces {
			get {	return	_faces;	}
		}
	}
}
