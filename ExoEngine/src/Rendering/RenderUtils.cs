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
using System.Drawing;
using Exocortex.Geometry3D;
using Exocortex.OpenGL;

namespace ExoEngine.Rendering {


	public class RenderUtils {

		//===============================================================================

		static protected float _colorConstant = 1.0f / 255.0f;
		static protected float _colorAmbient = 0.1f / 255.0f;

		//===============================================================================

		static public void	glColor( Color clr ) {
			// color 
			GL.glColor3f( clr.R * _colorConstant, clr.G * _colorConstant, clr.B * _colorConstant );
		}
		static public void	glColor_GouraudIllumination( Color clr, Vector3D world, Vector3D normal, Vector3D light ) {
			// color (gouraud illumination)
			Vector3D	lightDirection = ( world - light );
			float intensity = - Vector3D.Dot( lightDirection, normal ) / lightDirection.GetMagnitude() * _colorConstant;
			GL.glColor3f( clr.R * intensity, clr.G * intensity, clr.B * intensity );
		}

		static public void	PreCalculate_GouraudIllumination( Vector3D[,] vertices, Vector3D[,] normals, Vector3D light, float[,] intensities ) {
			int xLength = vertices.GetLength( 0 );
			int yLength = vertices.GetLength( 1 );

			for( int y = 0; y < yLength; y ++ ) {
				for( int x = 0; x < xLength; x ++ ) {
					Vector3D	lightDirection = ( vertices[ x, y ] - light );
					intensities[ x, y ] = - Vector3D.Dot( lightDirection, normals[ x, y ] ) / lightDirection.GetMagnitude() * _colorConstant;
				}
			}
		}

		static public void	PreCalculate_GouraudIllumination( Vector3D[] vertices, Vector3D[] normals, Vector3D light, float[] intensities ) {
			int length = vertices.Length;

			for( int i = 0; i < length; i ++ ) {
				Vector3D	lightDirection = ( vertices[ i ] - light );
				intensities[ i ] = - Vector3D.Dot( lightDirection, normals[ i ] ) / lightDirection.GetMagnitude() * _colorConstant;
			}
		}

		static public void	glColor_PhongIllumination( Color clr, Vector3D world, Vector3D normal, Vector3D light, Color clrLight, Vector3D viewer, float shininess ) {
			// color (gouraud illumination)
			Vector3D	lightDirection	= ( world - light );
			float intensity = - Vector3D.Dot( lightDirection, normal ) / lightDirection.GetMagnitude() * _colorConstant;

			lightDirection.Normalize();
			Vector3D lightReflection = lightDirection - 2 * Vector3D.Dot( normal, lightDirection ) * normal;
			lightReflection.Normalize();

			Vector3D	viewerDirection = ( viewer - world );
			float lightIntensity = (float) Math.Pow( Math.Max( 0, Vector3D.Dot( viewerDirection, lightReflection ) ) / viewerDirection.GetMagnitude(), shininess ) * _colorConstant;
			GL.glColor3f(
				clr.R * intensity + clrLight.R * lightIntensity,
				clr.G * intensity + clrLight.G * lightIntensity,
				clr.B * intensity + clrLight.B * lightIntensity
				);
		}

		//===============================================================================

		static public void	glVertex( Vector3D world ) {
			// vertex
			GL.glVertex3f( world.X, world.Y, world.Z );
		}

		//===============================================================================
		
		static public void	glTexture( Vector3D texture ) {
			// texture
			GL.glTexCoord2f( texture.X, texture.Y );
		}

		static public bool	IsFacing( Vector3D world, Vector3D normal, Vector3D light ) {
			Vector3D	lightDirection	= ( world - light );
			float w = - Vector3D.Dot( normal, lightDirection ) / lightDirection.GetMagnitude();
			if( w < 0 ) {
				return false;
			}
			return true;
		}

		static public bool	IsFacing( Vector3D world, Vector3D normal, Vector3D light, Vector3D viewer ) {
			Vector3D	lightDirection	= ( world - light );
			lightDirection.Normalize();
					
			Vector3D lightReflection = lightDirection - 2 * Vector3D.Dot( normal, lightDirection ) * normal;
			lightReflection.Normalize();

			Vector3D	viewerDirection	= - ( world - viewer );
			viewerDirection.Normalize();
			float w = - Vector3D.Dot( lightReflection, viewerDirection );
			if( w > -0.5 ) {
				return false;
			}
			return true;
		}

		static public void	glTexture_AsinMapping( Vector3D world, Vector3D normal, Vector3D viewer, Vector3D viewerUp ) {
			// texture coord (reflection mapping coordinate)
			Vector3D	viewerDirection	= - ( world - viewer );
			viewerDirection.Normalize();

			Vector3D uAxis	= Vector3D.Cross( viewerDirection, viewerUp );
			Vector3D vAxis	= Vector3D.Cross( viewerDirection, uAxis );

			float u = Vector3D.Dot( normal, uAxis ) / uAxis.GetMagnitude();
			float v = Vector3D.Dot( normal, vAxis ) / vAxis.GetMagnitude();
			float w = - Vector3D.Dot( normal, viewerDirection );

			//float uCorrected	= u;//RenderUtils.QuickAsin( u );
			//float vCorrected	= v;//RenderUtils.QuickAsin( v );
				
			if( w > 0 ) {
				float magnitudeInverse = 1 / (float) Math.Sqrt( u*u + v*v );
                u = u * ( 2 * magnitudeInverse - 1 );
				v = v * ( 2 * magnitudeInverse - 1 );
			}
			GL.glTexCoord2f( u * 0.25f + 0.5f, v * 0.25f + 0.5f );//( uCorrected + 2 ) * 0.25f, ( vCorrected + 2 ) * 0.25f );
		}

		//static public void	glTexture_ReflectionMapping( Vector3D world, Vector3D normal, Vector3D viewer, Vector3D viewerUp ) {
		//	glTexture_AsinMapping( world, normal, viewer, viewerUp );
		//}
	
		static public void	glTexture_PhongMapping( Vector3D world, Vector3D normal, Vector3D light, Vector3D viewer, Vector3D viewerUp ) {
			Vector3D	lightDirection	= ( world - light );
			lightDirection.Normalize();
					
			Vector3D lightReflection = lightDirection - 2 * Vector3D.Dot( normal, lightDirection ) * normal;
			lightReflection.Normalize();

			glTexture_AsinMapping( world, lightReflection, viewer, viewerUp );
		}

		
		static public void PreCalculate_AsinMapping( Vector3D[,] vertices, Vector3D[,] normals, Vector3D viewer, Vector3D viewerUp, Vector3D[,] textureCache ) {
			int xLength = vertices.GetLength( 0 );
			int yLength = vertices.GetLength( 1 );

			for( int y = 0; y < yLength; y ++ ) {
				for( int x = 0; x < xLength; x ++ ) {
					Vector3D	viewerDirection	= - ( vertices[ x, y ] - viewer );
					viewerDirection.Normalize();

					Vector3D uAxis	= Vector3D.Cross( viewerDirection, viewerUp );
					Vector3D vAxis	= Vector3D.Cross( viewerDirection, uAxis );

					float u = Vector3D.Dot( normals[ x, y ], uAxis ) / uAxis.GetMagnitude();
					float v = Vector3D.Dot( normals[ x, y ], vAxis ) / vAxis.GetMagnitude();
					float w = - Vector3D.Dot( normals[ x, y ], viewerDirection );

					//float uCorrected	= u;//RenderUtils.QuickAsin( u );
					//float vCorrected	= v;//RenderUtils.QuickAsin( v );
				
					if( w > 0 ) {
						float magnitudeInverse = 1 / (float) Math.Sqrt( u*u + v*v );
						u = u * ( 2 * magnitudeInverse - 1 );
						v = v * ( 2 * magnitudeInverse - 1 );
					}
					textureCache[ x, y ].Set( u * 0.25f + 0.5f, v * 0.25f + 0.5f, 0 );
				}
			}
		}

		static public void PreCalculate_AsinMapping( Vector3D[] vertices, Vector3D[] normals, Vector3D viewer, Vector3D viewerUp, Vector3D[] textureCache ) {
			int length = vertices.Length;

			for( int i = 0; i < length; i ++ ) {
				Vector3D	viewerDirection	= - ( vertices[ i ] - viewer );
				viewerDirection.Normalize();

				Vector3D uAxis	= Vector3D.Cross( viewerDirection, viewerUp );
				Vector3D vAxis	= Vector3D.Cross( viewerDirection, uAxis );

				float u = Vector3D.Dot( normals[ i ], uAxis ) / uAxis.GetMagnitude();
				float v = Vector3D.Dot( normals[ i ], vAxis ) / vAxis.GetMagnitude();
				float w = - Vector3D.Dot( normals[ i ], viewerDirection );

				if( w > 0 ) {
					float magnitudeInverse = 1 / (float) Math.Sqrt( u*u + v*v );
					u = u * ( 2 * magnitudeInverse - 1 );
					v = v * ( 2 * magnitudeInverse - 1 );
				}
				textureCache[ i ].Set( u * 0.25f + 0.5f, v * 0.25f + 0.5f, 0 );
			}
		}

		static public void PreCalculate_PhongMapping( Vector3D[] vertices, Vector3D[] normals, Vector3D light, Vector3D viewer, Vector3D viewerUp, Vector3D[] textureCache ) {
			int length = vertices.Length;

			for( int i = 0; i < length; i ++ ) {
				Vector3D	lightDirection	= ( vertices[ i ] - light );
				lightDirection.Normalize();
					
				Vector3D lightReflection = lightDirection - 2 * Vector3D.Dot( normals[ i ], lightDirection ) * normals[ i ];
				lightReflection.Normalize();
					
				Vector3D	viewerDirection	= - ( vertices[ i ] - viewer );
				viewerDirection.Normalize();

				Vector3D uAxis	= Vector3D.Cross( viewerDirection, viewerUp );
				Vector3D vAxis	= Vector3D.Cross( viewerDirection, uAxis );

				float u = Vector3D.Dot( lightReflection, uAxis ) / uAxis.GetMagnitude();
				float v = Vector3D.Dot( lightReflection, vAxis ) / vAxis.GetMagnitude();
				float w = - Vector3D.Dot( lightReflection, viewerDirection );

				if( w > 0 ) {
					float magnitudeInverse = 1 / (float) Math.Sqrt( u*u + v*v );
					u = u * ( 2 * magnitudeInverse - 1 );
					v = v * ( 2 * magnitudeInverse - 1 );
				}
				textureCache[ i ].Set( u * 0.25f + 0.5f, v * 0.25f + 0.5f, 0 );
			}
		}

		static public void PreCalculate_PhongMapping( Vector3D[,] vertices, Vector3D[,] normals, Vector3D light, Vector3D viewer, Vector3D viewerUp, Vector3D[,] textureCache ) {
			int xLength = vertices.GetLength( 0 );
			int yLength = vertices.GetLength( 1 );

			for( int y = 0; y < yLength; y ++ ) {
				for( int x = 0; x < xLength; x ++ ) {
					Vector3D	lightDirection	= ( vertices[ x, y ] - light );
					lightDirection.Normalize();
					
					Vector3D lightReflection = lightDirection - 2 * Vector3D.Dot( normals[ x, y ], lightDirection ) * normals[ x, y ];
					lightReflection.Normalize();
					
					Vector3D	viewerDirection	= - ( vertices[ x, y ] - viewer );
					viewerDirection.Normalize();

					Vector3D uAxis	= Vector3D.Cross( viewerDirection, viewerUp );
					Vector3D vAxis	= Vector3D.Cross( viewerDirection, uAxis );

					float u = Vector3D.Dot( lightReflection, uAxis ) / uAxis.GetMagnitude();
					float v = Vector3D.Dot( lightReflection, vAxis ) / vAxis.GetMagnitude();
					float w = - Vector3D.Dot( lightReflection, viewerDirection );

					//float uCorrected	= u;//RenderUtils.QuickAsin( u );
					//float vCorrected	= v;//RenderUtils.QuickAsin( v );
				
					if( w > 0 ) {
						float magnitudeInverse = 1 / (float) Math.Sqrt( u*u + v*v );
						u = u * ( 2 * magnitudeInverse - 1 );
						v = v * ( 2 * magnitudeInverse - 1 );
					}
					textureCache[ x, y ].Set( u * 0.25f + 0.5f, v * 0.25f + 0.5f, 0 );
				}
			}
		}
		
		//===============================================================================
		
		static protected int		_asinResolution		= -1;
		static protected int		_asinHalfResolution	= -1;
		static protected float[]	_asinLookup		= null;
		static protected float[]	_asinRALookup	= null;

		/// <summary>
		/// Set resolution of quick Asin lookup functions.
		/// A good resolution is 1024.
		/// A resolution of -1 resets the quick asin functions to initial values.
		/// A resolution of less than -1, 0, or 1 is invalid since they are not
		///    useful.
		/// </summary>
		static public int QuickAsinResolution {
			set { 	
				if( value == _asinResolution ) {
					return;
				}
				if( value == -1 ) {
					_asinResolution		= -1;
					_asinHalfResolution	= -1;
					_asinLookup		= null;
					_asinRALookup	= null;
					return;
				}

				Debug.Assert( value > 1 );

				_asinHalfResolution = ( value / 2 );
				_asinResolution = _asinHalfResolution * 2 + 1;
				_asinLookup = new float[ _asinResolution ];
				_asinRALookup = new float[ _asinResolution ];
				for( int i = 0; i < _asinResolution; i ++ ) {
					float dotProduct = (float)( i - _asinHalfResolution ) / _asinHalfResolution;
					_asinLookup[i] = (float) Math.Asin( dotProduct ) ;
					_asinRALookup[i] = (float)( 0.5 + 0.5 * Math.Asin( dotProduct ) / ( Math.PI / 2.0 ) );
				}
			}
			get {
				return	_asinResolution;
			}
		}

		/// <summary>
		/// Quick Asin Lookup.
		/// Output range is [ -pi/2 ... pi/2 ].
		/// Resolution is set using InitQuickAsin().
		/// </summary>
		static public float QuickAsin( float dotProduct ) {
			return _asinLookup[ (int)( dotProduct * _asinHalfResolution ) + _asinHalfResolution ];
		}

		/// <summary>
		/// Quick Asin Lookup, integrated range adjustment.
		/// Output range is [ 0 ... 1 ].
		/// Resolution is set using InitQuickAsin().
		/// </summary>
		static public float QuickAsinRA( float dotProduct ) {
			return _asinRALookup[ (int)( dotProduct * _asinHalfResolution ) + _asinHalfResolution ];
		}
		
		//===============================================================================
	
	}

}
