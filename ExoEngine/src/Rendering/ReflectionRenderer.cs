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
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;
using Exocortex;
using Exocortex.Collections;
using Exocortex.Geometry3D;
using Exocortex.OpenGL;
	   
using ExoEngine.Geometry;
using ExoEngine.BSPTree;

namespace ExoEngine.Rendering {

	public class ReflectionRenderer : Renderer {
	
		public ReflectionRenderer() {
		}

		public override void	Initialize( World world ) {
			base.Initialize( world );
			RenderUtils.QuickAsinResolution = 1024;
			_textureReflection = world.Textures.RequestTexture( "reflection" );
			Debug.Assert( _textureReflection != null );
			if( _textureReflection.IsBitmapLoaded == false ) {
				_textureReflection.LoadBitmap();
			}
			_textureReflection.InternalUse = true;

			uint[] data = _textureReflection.Data;
			int size = data.Length;
			for( int i = 0; i < size; i ++ ) {
				Color colorOriginal = Color.FromArgb( (int) data[i] );
				Color colorModified = Color.FromArgb( 255, colorOriginal );
				data[i] = (uint) colorModified.ToArgb();
			}
			_textureReflection.MinFilter = GL.TextureFilter.Linear;
			_textureReflection.MaxFilter = GL.TextureFilter.Linear;

			if( _textureReflection.IsOpenGLLoaded == false ) {
				_textureReflection.LoadOpenGL();
				Debug.Assert( _textureReflection.IsOpenGLLoaded == true );
			}

			CreateMaps();
		}

		static protected int _diffuseMapHandle = -1;
		static protected int _specularMapHandle = -1;

		protected unsafe void CreateMaps() {
			int xSize = 256;
			int ySize = 256;

			float[] diffuseMap	= new float[ xSize * ySize ];
			float[] specularMap	= new float[ xSize * ySize ];

			Vector3D	lightDirection = new Vector3D( 0, 0, 1 );

			//float diffuseCoefficient	= 0.7f;

			//float specularCoefficient	= 0.6f;
			float specularShininess		= 50.0f;

			for( int y = 0; y < ySize; y ++ ) {
				for( int x = 0; x < xSize; x ++ ) {

					// convert [0,0] - [xSize,ySize] -> [0,0] - [1,1]
					float xx = ((float) x ) / xSize;
					float yy = ((float) y ) / ySize;
					// convert [0,0] - [1,1] -> [-2,2] - [2,2]
					float xxx = xx * 4 - 2;
					float yyy = yy * 4 - 2;

					float cosTheta;

					float magnitudeSqrd = xxx*xxx + yyy*yyy;
					if( Math.Sqrt( magnitudeSqrd ) > 1 ) {
						cosTheta = 0;
					}
					else {
						cosTheta = (float) Math.Sqrt( 1 - magnitudeSqrd );
					}

					float diffuseLuminance	= cosTheta;

					float specularLuminance	= (float) Math.Pow( cosTheta, specularShininess );

					int offset = x + y * xSize;
					diffuseMap[ offset ] = diffuseLuminance;

					//specularMap[ offset * 2 + 0 ] = 1;//specularLuminance;
					specularMap[ offset ] = specularLuminance;

					/*if( x == xSize / 2 || y == ySize / 2 ) {
						diffuseMap[ offset ] = 0;
						specularMap[ offset * 2 + 0 ] = 0;
					}*/
				}
			}

			//Bitmap bitmap = BitmapUtils.ConvertIntensityArrayToBitmap( diffuseMap, xSize, ySize );
			//bitmap.Save( "diffuse.bmp" );


			// load opengl texture
			GL.glEnable( GL.Option.Texture2D );
			uint[] handles = new uint[2];
			GL.glGenTextures( 2, handles );
			_diffuseMapHandle	= (int) handles[0];
			_specularMapHandle	= (int) handles[1];
			GL.glErrorCheck();


			GL.glBindTexture( GL.Texture.Texture2D, (uint) _diffuseMapHandle );
			GL.glTexParameteri( GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, GL.GL_CLAMP );
			GL.glTexParameteri( GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, GL.GL_CLAMP );
			GL.glTexParameteri( GL.Texture.Texture2D, GL.TexParamPName.TextureMinFilter, GL.TextureFilter.Linear );
			GL.glTexParameteri( GL.Texture.Texture2D, GL.TexParamPName.TextureMagFilter, GL.TextureFilter.Linear );
			fixed( float *pDiffuseMap = & (diffuseMap[0]) ) {
				GL.glTexImage2D( GL.GL_TEXTURE_2D, 0, (int) GL.GL_LUMINANCE8, xSize, ySize, 0,
					GL.GL_LUMINANCE, GL.GL_FLOAT, (void*) pDiffuseMap );
			}
			GL.glErrorCheck();

			// load specularMap into OpenGL
			GL.glBindTexture( GL.Texture.Texture2D, (uint) _specularMapHandle );
			GL.glTexParameteri( GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, GL.GL_CLAMP );
			GL.glTexParameteri( GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, GL.GL_CLAMP );
			GL.glTexParameteri( GL.Texture.Texture2D, GL.TexParamPName.TextureMinFilter, GL.TextureFilter.Linear );
			GL.glTexParameteri( GL.Texture.Texture2D, GL.TexParamPName.TextureMagFilter, GL.TextureFilter.Linear );
			fixed( float *pSpecularMap = & (specularMap[0]) ) {
				GL.glTexImage2D( GL.GL_TEXTURE_2D, 0, (int) GL.GL_ALPHA8, xSize, ySize, 0,
					GL.GL_ALPHA, GL.GL_FLOAT, (void*) pSpecularMap );
			}
			GL.glErrorCheck();

		}

		protected Texture _textureReflection = null;

		public override void Render( World world, Faces faces, RenderSettings settings ) {
			base.Render( world, faces, settings );


			Vector3D	viewerOrigin	= world.Camera.Translation;
			Vector3D viewerUp		= world.Camera.UpAxis;

			Vector3D lightOrigin	= world.Camera.Translation - world.Camera.RightAxis * world.Camera.ViewportWidth * 50;
			Vector3D lightUp		= world.Camera.UpAxis;
			
			GL.glPolygonMode( GL.GL_FRONT_AND_BACK, ( settings.Wireframe ) ? GL.GL_LINE : GL.GL_FILL);
			GL.glDisable( GL.GL_BLEND );
			//GL.glBlendFunc( GL.BlendSrc.SrcAlpha, GL.BlendDest.OneMinusSrcAlpha );
			GL.glEnable( GL.GL_COLOR_MATERIAL );
			GL.glEnable( GL.GL_TEXTURE_2D );

			if( ! settings.FaceColors ) {
				GL.glColor( Color.White );
			}
			
			if( settings.ZBuffer ) {
				GL.glEnable( GL.GL_DEPTH_TEST );
			}
			else {
				GL.glDisable( GL.GL_DEPTH_TEST );
			}

			Vector3D lightSource	= world.Camera.Translation;
			Color clr = Color.White;
			//float	r = 1, g = 1, b = 1;
			
			foreach( Face face in faces ) {

				int			faceVerticies		= face.Points.Count;
				Vector3DCollection	faceWorldCoords		= face.Points;
				Vector3DCollection	faceTextureCoords	= face.TextureCoords;
				Vector3D		normal				= face.Normal;

				// blend in reflection component
				
				GL.glBindTexture( GL.Texture.Texture2D, (uint) _textureReflection.OpenGLHandle );
				GL.glBegin( GL.Primative.Polygon );

				if( settings.FaceColors ) {
					clr = face.Color;
				}


				bool bVertexNormals = face.IsVertexNormals;

				for( int index = 0; index < faceVerticies; index ++ ) {
					Vector3D	worldCoord		= faceWorldCoords[ index ];

					if( bVertexNormals ) {
						normal = face.VertexNormals[ index ];
					}

					RenderUtils.glColor_GouraudIllumination( clr, worldCoord, normal, lightSource );
					RenderUtils.glTexture_AsinMapping( worldCoord, normal, viewerOrigin, viewerUp );
					RenderUtils.glVertex( worldCoord );
				}

				GL.glEnd();

				/*GL.glBegin( GL.Primative.Lines );

				if( bVertexNormals ) {
					for( int index = 0; index < faceVerticies; index ++ ) {
						Vector3D	worldCoord		= faceWorldCoords[ index ];
						normal		= face.VertexNormals[ index ];

						RenderUtils.glColor( normal.ToColor() );
						RenderUtils.glVertex( worldCoord );
						RenderUtils.glVertex( worldCoord + normal * 30 );
					}
				}
				else {
					RenderUtils.glColor( Color.Yellow );
					RenderUtils.glVertex( face.MidPoint );
					RenderUtils.glVertex( face.MidPoint + normal * 30 );
				}

				GL.glEnd();*/

				GL.glErrorCheck();
			}
		}		
		
		protected int			_xCacheSize	= -1;
		protected int			_yCacheSize	= -1;
		protected float[,]		_lightingCache	= null;
		protected Vector3D[,]	_textureCache	= null;
		protected Vector3D[,]	_phongMappingCache	= null;
		

		public void RenderQuadMesh( World world, Color clr, Vector3D[,] vertices, Vector3D[,] normals, RenderSettings settings ) {
			//Debug2.Push();
			base.Render( world, null, settings );

			int xSize = vertices.GetLength( 0 );
			int ySize = vertices.GetLength( 1 );

			//Debug.WriteLine( "xSize = " + xSize );
			//Debug.WriteLine( "ySize = " + ySize );

            Debug.Assert( xSize == normals.GetLength( 0 ) );
			Debug.Assert( ySize == normals.GetLength( 1 ) );

			if( _xCacheSize < xSize || _yCacheSize < ySize ) {
				_xCacheSize = Math.Max( _xCacheSize, xSize );
				_yCacheSize = Math.Max( _yCacheSize, ySize );

				_lightingCache	= new float[ _xCacheSize, _yCacheSize ];
				_textureCache	= new Vector3D[ _xCacheSize, _yCacheSize ];
				_phongMappingCache	= new Vector3D[ _xCacheSize, _yCacheSize ];
				for( int y = 0; y < _yCacheSize; y ++ ) {
					for( int x = 0; x < _xCacheSize; x ++ ) {
						_textureCache[ x, y ] = Vector3D.FromXYZ( 0, 0, 0 );
						_phongMappingCache[ x, y ]	= Vector3D.FromXYZ( 0, 0, 0 );
					}
				}
			}

			Vector3D	viewerOrigin	= world.Camera.Translation;
			Vector3D viewerUp		= world.Camera.UpAxis;

			Vector3D lightOrigin	= world.Light - world.Camera.RightAxis * world.Camera.ViewportWidth * 50;
			Vector3D lightUp		= world.Camera.UpAxis;
			
			//RenderUtils.PreCalculate_GouraudIllumination( vertices, normals, lightOrigin, _lightingCache );
			//RenderUtils.PreCalculate_AsinMapping( vertices, normals, viewerOrigin, viewerUp, _asinMappingCache );

			GL.glPolygonMode( GL.GL_FRONT_AND_BACK, ( settings.Wireframe ) ? GL.GL_LINE : GL.GL_FILL);
			GL.glEnable( GL.GL_BLEND );
			GL.glBlendFunc( GL.BlendSrc.SrcAlpha, GL.BlendDest.OneMinusSrcAlpha );
			GL.glEnable( GL.GL_COLOR_MATERIAL );
			GL.glEnable( GL.GL_TEXTURE_2D );

			if( ! settings.FaceColors ) {
				clr = Color.White;
			}
			//clr = Color.FromArgb( 128, clr );
			
			if( settings.ZBuffer ) {
				GL.glEnable( GL.GL_DEPTH_TEST );
			}
			else {
				GL.glDisable( GL.GL_DEPTH_TEST );
			}

			Vector3D lightSource	= world.Camera.Translation;

			RenderUtils.PreCalculate_GouraudIllumination( vertices, normals, lightSource, _lightingCache );
			RenderUtils.PreCalculate_AsinMapping( vertices, normals, viewerOrigin, viewerUp, _textureCache );
//			RenderUtils.PreCalculate_PhongMapping( vertices, normals, lightOrigin, viewerOrigin, viewerUp, _phongMappingCache );

			GL.glErrorCheck();

			Vector3D	vertex;
			Vector3D	texture;
			float	intensity;

			GL.glBindTexture( GL.Texture.Texture2D, (uint) _textureReflection.OpenGLHandle );

			for( int y = 0; y < ( ySize - 1 ); y ++ ) {
				GL.glBegin( GL.Primative.QuadStrip );

				for( int x = 0; x < xSize; x ++ ) {
					
					intensity = _lightingCache[ x, y + 1 ];
					GL.glColor4f( clr.R * intensity, clr.G * intensity, clr.B * intensity, 0.75f );
					
					texture = _textureCache[ x, y + 1 ];
					GL.glTexCoord2f( texture.X, texture.Y );
					
					vertex = vertices[ x, y + 1 ];
					GL.glVertex3f( vertex.X, vertex.Y, vertex.Z );

					intensity = _lightingCache[ x, y ];
					GL.glColor4f( clr.R * intensity, clr.G * intensity, clr.B * intensity, 0.75f );
					
					texture = _textureCache[ x, y ];
					GL.glTexCoord2f( texture.X, texture.Y );
					
					vertex = vertices[ x, y ];
					GL.glVertex3f( vertex.X, vertex.Y, vertex.Z );

				}

				GL.glEnd();
			}
			
			GL.glErrorCheck();
			
			/*GL.glEnable( GL.Option.Blend );
			GL.glBlendFunc( GL.GL_SRC_ALPHA, GL.GL_ONE );

			GL.glDisable( GL.Option.ColorMaterial );
			GL.glEnable( GL.Option.Texture2D );
			GL.glBindTexture( GL.Texture.Texture2D, (uint) _specularMapHandle );

			GL.glErrorCheck();

			GL.glColor( Color.White );

			for( int y = 0; y < ( ySize - 1 ); y ++ ) {
				GL.glBegin( GL.Primative.QuadStrip );

				for( int x = 0; x < xSize; x ++ ) {
					
					GL.glTexCoord2( _phongMappingCache[ x, y + 1 ] );
					GL.glVertex3( vertices[ x, y + 1 ] );

					GL.glTexCoord2( _phongMappingCache[ x, y ] );
					GL.glVertex3( vertices[ x, y ] );
				}

				GL.glEnd();
			}

			GL.glErrorCheck();   */
			//Debug2.Pop();
		}	
		
		//===============================================================================

	}

}