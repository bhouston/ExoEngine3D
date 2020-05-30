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
using Exocortex.Imaging;
using Exocortex.Geometry3D;
using Exocortex.OpenGL;
	   
using ExoEngine.Geometry;
using ExoEngine.BSPTree;

namespace ExoEngine.Rendering {

	public class PhongRenderer : Renderer {
	
		public PhongRenderer() {
		}

		public override void	Initialize( World world ) {
			base.Initialize( world );
			RenderUtils.QuickAsinResolution = 1024;
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

		public override void Render( World world, Faces faces, RenderSettings settings ) {
			base.Render( world, faces, settings );

			Vector3D	viewerOrigin	= world.Camera.Translation;
			Vector3D viewerUp		= world.Camera.UpAxis;

			Vector3D lightOrigin	= world.Camera.Translation;// - world.Camera.RightAxis * world.Camera.ViewportWidth * 50;
			Vector3D lightUp		= world.Camera.UpAxis;
			

			GL.glPolygonMode( GL.GL_FRONT_AND_BACK, ( settings.Wireframe ) ? GL.GL_LINE : GL.GL_FILL);

			GL.glDisable( GL.GL_TEXTURE_2D );

			Color clr = Color.White;
			
			foreach( Face face in faces ) {

				int					faceVerticies		= face.Points.Count;
				Vector3DCollection	worldCoords		= face.Points;
				Vector3DCollection	textureCoords	= null;
				Vector3D				normal				= face.Normal;

				// draw base texture

				if( settings.ZBuffer ) {
					GL.glEnable( GL.GL_DEPTH_TEST );
				}

				GL.glDisable( GL.GL_BLEND );
				GL.glEnable( GL.GL_COLOR_MATERIAL );

				if( settings.FaceColors ) {
					clr = face.Color;
				}

				bool vertexNormals	= face.IsVertexNormals;
				bool texture		= settings.Textures && face.IsTexture;

				// draw base texture

				if ( texture ) {
					GL.glEnable( GL.GL_TEXTURE_2D );
					GL.glBindTexture( GL.Texture.Texture2D, (uint) face.Texture.OpenGLHandle );
					textureCoords	= face.TextureCoords;
				}
				else {
					GL.glDisable( GL.GL_TEXTURE_2D );
				}

				GL.glBegin( GL.Primative.Polygon );
				for( int index = 0; index < faceVerticies; index ++ ) {
					if( vertexNormals ) {
						normal = face.VertexNormals[ index ];
					}

					RenderUtils.glColor_GouraudIllumination( clr, worldCoords[ index ], normal, lightOrigin );

					if( texture ) {
						RenderUtils.glTexture( textureCoords[ index ] );
					}
					RenderUtils.glVertex( worldCoords[ index ] );
				}

				GL.glEnd();	  

				/*// blend in diffuse component

				GL.glEnable( GL.GL_BLEND );
				GL.glBlendFunc( GL.GL_ZERO, GL.GL_SRC_COLOR );
				GL.glColor( Color.White );
				GL.glEnable( GL.GL_TEXTURE_2D );
				GL.glBindTexture( GL.Texture.Texture2D, (uint) _diffuseMapHandle );
				GL.glBegin( GL.Primative.Polygon );

				if( RenderUtils.IsFacing( face.MidPoint, face.Normal, lightOrigin ) ) {
					for( int index = 0; index < faceVerticies; index ++ ) {
						Vector3D	worldCoord		= faceWorldCoords[ index ];

						if( vertexNormals ) {
							normal = face.VertexNormals[ index ];
						}
	
						RenderUtils.glTexture_AsinMapping( worldCoord, normal, lightOrigin, lightUp );
						RenderUtils.glVertex( worldCoord );
					}
				}
				else {
					for( int index = 0; index < faceVerticies; index ++ ) {
						Vector3D	worldCoord		= faceWorldCoords[ index ];

						if( vertexNormals ) {
							normal = face.VertexNormals[ index ];
						}
	
						RenderUtils.glTexture( new Vector3D( 0, 0, 0 ) );
						RenderUtils.glVertex( worldCoord );
					}
				}  
				GL.glEnd();	 */
				
				// determine if no light is hitting face

				if( RenderUtils.IsFacing( face.MidPoint, face.Normal, lightOrigin, viewerOrigin ) ) {
				
					// blend in specular component
				
					GL.glEnable( GL.GL_BLEND );
					GL.glBlendFunc( GL.GL_SRC_ALPHA, GL.GL_ONE );
					GL.glEnable( GL.GL_TEXTURE_2D );
					GL.glBindTexture( GL.Texture.Texture2D, (uint) _specularMapHandle );
					GL.glColor( Color.White );
					GL.glBegin( GL.Primative.Polygon );

					for( int index = 0; index < faceVerticies; index ++ ) {
						Vector3D	worldCoord		= worldCoords[ index ];

						if( vertexNormals ) {
							normal = face.VertexNormals[ index ];
						}
	
						RenderUtils.glTexture_PhongMapping( worldCoord, normal, lightOrigin, viewerOrigin, viewerUp );
						RenderUtils.glVertex( worldCoord );
					}

					GL.glEnd();
				}  
			}

			GL.glDisable( GL.GL_BLEND );
		}

		/*protected int			_cacheSize			= -1;
		protected Vector3D[]		_asinMappingCache	= null;
		protected Vector3D[]		_phongMappingCache	= null;
		

		public void RenderTriMesh( World world, Color clr, Vector3D[] vertices, Vector3D[] normals, int[,] triangles, RenderSettings settings ) {

			base.Render( world, null, settings );

			int vertexCount		= vertices.Length;
			int triangleCount	= triangles.GetLength( 0 );

			Debug.Assert( vertexCount == normals.Length );

			if( _cacheSize < vertexCount ) {

				_cacheSize = Math.Max( _cacheSize, vertexCount );

				_asinMappingCache	= new Vector3D[ _cacheSize ];
				_phongMappingCache	= new Vector3D[ _cacheSize ];

				for( int i = 0; i < _cacheSize; i ++ ) {
					_asinMappingCache[ i ] = Vector3D.FromXYZ( 0, 0, 0 );
					_phongMappingCache[ i ] = Vector3D.FromXYZ( 0, 0, 0 );
				}
			}

			world.PolygonCount += triangleCount;

			Vector3D	viewerOrigin	= world.Camera.Translation;
			Vector3D viewerUp		= world.Camera.UpAxis;

			Vector3D lightOrigin	= world.Camera.Translation - world.Camera.RightAxis * world.Camera.ViewportWidth * 50;
			Vector3D lightUp		= world.Camera.UpAxis;

			RenderUtils.PreCalculate_AsinMapping( vertices, normals, viewerOrigin, viewerUp, _asinMappingCache );
			RenderUtils.PreCalculate_PhongMapping( vertices, normals, lightOrigin, viewerOrigin, viewerUp, _phongMappingCache );

			int		index;
			//Vector3D	vertex;
			//Vector3D	texture;

			// ---------------------------------------------------
			//
			// draw base texture
			//

			GL.glDisable( GL.Option.Blend );

			GL.glEnable( GL.Option.Texture2D );
			GL.glEnable( GL.Option.ColorMaterial );
			GL.glBindTexture( GL.Texture.Texture2D, (uint) _diffuseMapHandle );

			GL.glErrorCheck();

			GL.glBegin( GL.Primative.Triangles );
			
			GL.glColor( clr );

			for( int t = 0; t < triangleCount; t ++ ) {
				for( int v = 0; v < 3; v ++ ) {
					index = triangles[ t, v ];
					GL.glTexCoord2( _asinMappingCache[ index ] );
					GL.glVertex3( vertices[ index ] );
				}
			}

			GL.glEnd();

			GL.glErrorCheck();

			// ---------------------------------------------------
			//
			// add specular highlight
			//

			GL.glEnable( GL.Option.Blend );
			GL.glBlendFunc( GL.GL_SRC_ALPHA, GL.GL_ONE );

			GL.glEnable( GL.Option.Texture2D );
			GL.glBindTexture( GL.Texture.Texture2D, (uint) _specularMapHandle );

			GL.glErrorCheck();

			GL.glBegin( GL.Primative.Triangles );

			GL.glColor( Color.White );
			
			for( int t = 0; t < triangleCount; t ++ ) {
				for( int v = 0; v < 3; v ++ ) {
					index = triangles[ t, v ];
					GL.glTexCoord2( _phongMappingCache[ index ] );
					GL.glVertex3( vertices[ index ] );
				}
			}

			GL.glEnd();

			GL.glErrorCheck();
		}	  */
		
		//===============================================================================

		protected int			_cacheSize			= -1;
		protected float[]		_lightingCache		= null;
		protected Vector3D[]		_asinMappingCache	= null;
		protected Vector3D[]		_phongMappingCache	= null;

		public void RenderTriMesh( World world, Color clr, Vector3D[] vertices, Vector3D[] normals, int[] triangles, Plane3D[] trianglePlanes, RenderSettings settings ) {

			base.Render( world, null, settings );

			int vertexCount		= vertices.Length;
			int triangleCount	= triangles.GetLength( 0 ) / 3;

			Debug.Assert( vertexCount == normals.Length );

			if( _cacheSize < vertexCount ) {

				_cacheSize = Math.Max( _cacheSize, vertexCount );

				_lightingCache		= new float[ _cacheSize ];
				_asinMappingCache	= new Vector3D[ _cacheSize ];
				_phongMappingCache	= new Vector3D[ _cacheSize ];

				for( int i = 0; i < _cacheSize; i ++ ) {
					_asinMappingCache[ i ] = Vector3D.FromXYZ( 0, 0, 0 );
					_phongMappingCache[ i ] = Vector3D.FromXYZ( 0, 0, 0 );
				}
			}


			Vector3D	viewerOrigin	= world.Camera.Translation;
			Vector3D viewerUp		= world.Camera.UpAxis;

			Vector3D lightOrigin	= world.Light - world.Camera.RightAxis * world.Camera.ViewportWidth * 50;
			Vector3D lightUp		= world.Camera.UpAxis;

			RenderUtils.PreCalculate_GouraudIllumination( vertices, normals, lightOrigin, _lightingCache );
			//RenderUtils.PreCalculate_AsinMapping( vertices, normals, viewerOrigin, viewerUp, _asinMappingCache );
			RenderUtils.PreCalculate_PhongMapping( vertices, normals, lightOrigin, viewerOrigin, viewerUp, _phongMappingCache );

			int		index;
			float	intensity;
			//Vector3D	vertex;
			//Vector3D	texture;

			// ---------------------------------------------------
			//
			// draw base texture
			//

			GL.glDisable( GL.Option.Blend );

			GL.glDisable( GL.Option.Texture2D );
			//GL.glBindTexture( GL.Texture.Texture2D, (uint) _diffuseMapHandle );
			GL.glEnable( GL.Option.ColorMaterial );

			GL.glErrorCheck();

			GL.glBegin( GL.Primative.Triangles );
			
			//GL.glColor( clr );

			for( int t = 0; t < triangleCount; t ++ ) {
				if( trianglePlanes[ t ].GetDistanceToPlane( viewerOrigin ) > 0 ) {
					for( int v = 0; v < 3; v ++ ) {
						index = triangles[ t * 3 + v ];

						intensity = _lightingCache[ index ];
						GL.glColor3f( clr.R * intensity, clr.G * intensity, clr.B * intensity );
					
						GL.glVertex3( vertices[ index ] );
					}
				}
			}

			GL.glEnd();

			GL.glErrorCheck();

			// ---------------------------------------------------
			//
			// add specular highlight
			//

			GL.glEnable( GL.Option.Blend );
			GL.glBlendFunc( GL.GL_SRC_ALPHA, GL.GL_ONE );

			GL.glDisable( GL.Option.ColorMaterial );
			GL.glEnable( GL.Option.Texture2D );
			GL.glBindTexture( GL.Texture.Texture2D, (uint) _specularMapHandle );

			GL.glErrorCheck();

			GL.glBegin( GL.Primative.Triangles );

			GL.glColor( Color.White );
			
			for( int t = 0; t < triangleCount; t ++ ) {
				if( trianglePlanes[ t ].GetDistanceToPlane( viewerOrigin ) > 0 ) {
					for( int v = 0; v < 3; v ++ ) {
						index = triangles[ t * 3 + v ];
						GL.glTexCoord2( _phongMappingCache[ index ] );
						GL.glVertex3( vertices[ index ] );
					}
				}
			}

			GL.glEnd();

			GL.glErrorCheck();
		}

		//===============================================================================
	}

}