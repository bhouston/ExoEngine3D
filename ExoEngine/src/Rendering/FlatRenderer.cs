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

	public class FlatRenderer : Renderer {
	
		public FlatRenderer() {
		}

		public override void Render( World world, Faces faces, RenderSettings settings ) {
			base.Render( world, faces, settings );
			
			//Faces visibleFaces = BSPTreeVizOperator.GetVisibleFaces( world.Camera.Translation, world.BSPTreeRoot );
			//world.PolygonCount += faces.Count;

			GL.glPolygonMode( GL.GL_FRONT_AND_BACK, ( settings.Wireframe ) ? GL.GL_LINE : GL.GL_FILL );
			GL.glDisable( GL.GL_BLEND );
			GL.glEnable( GL.GL_COLOR_MATERIAL );

			GL.glErrorCheck();
			if( settings.Textures ) {
				GL.glEnable( GL.GL_TEXTURE_2D );
			}
			else {
				GL.glDisable( GL.GL_TEXTURE_2D );
			}

			GL.glErrorCheck();

			if( settings.ZBuffer ) {
				GL.glEnable( GL.GL_DEPTH_TEST );
			}
			else {
				GL.glDisable( GL.GL_DEPTH_TEST );
			}

			GL.glErrorCheck();
			
			Vector3D ptCamera = world.Camera.ForwardAxis;
			Vector3D lightSource	= world.Light;

			Color clr = Color.White;//float	r = 1, g = 1, b = 1;

			foreach( Face face in faces ) {
				
				bool	texture = ( settings.Textures && face.IsTexture );
				if( texture ) {
					if( face.IsTexture ) {
						GL.glBindTexture( GL.Texture.Texture2D, face.Texture.OpenGLHandle );
					}
				}
				GL.glBegin( GL.Primative.Polygon );

				if( settings.FaceColors ) {
					clr = face.Color;
				}
								
				RenderUtils.glColor_GouraudIllumination( clr, face.MidPoint, face.Normal, lightSource );
				//GL.glColor3f( r * lightIntensity, g * lightIntensity, b * lightIntensity );
				int count = face.Points.Count;
				for( int i = 0; i < count; i ++ ) {
					if( texture ) {
						RenderUtils.glTexture( face.TextureCoords[i] );
					}
					RenderUtils.glVertex( face.Points[i] );
				}
				GL.glEnd();
			}
			GL.glErrorCheck();
		}
	}

}

