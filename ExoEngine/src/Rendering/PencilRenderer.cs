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

	public class PencilRenderer : Renderer {
	
		public PencilRenderer() {
		}

		public override void Render( World world, Faces faces, RenderSettings settings ) {
			base.Render( world, faces, settings );
			
			GL.glDisable( GL.GL_BLEND );
			GL.glDisable( GL.GL_COLOR_MATERIAL );
			GL.glDisable( GL.GL_TEXTURE_2D );

			GL.glPolygonMode( GL.GL_FRONT_AND_BACK, ( settings.Wireframe ) ? GL.GL_LINE : GL.GL_FILL);

			// draw faces with cartoon shading
			foreach( Face face in faces ) {

				int			faceVerticies		= face.Points.Count;
				Vector3DCollection	faceWorldCoords		= face.Points;
				Vector3DCollection	faceTextureCoords	= face.TextureCoords;
				Vector3D		faceNormal			= face.Normal;

				GL.glLineWidth( 1f );
				if( settings.FaceColors ) {
					GL.glColor( face.Color );
				}
				else {
					GL.glColor3f( 1, 1, 1 );
				}
				GL.glBegin( GL.Primative.Polygon );

				for( int index = 0; index < faceVerticies; index ++ ) {
					Vector3D	worldCoord		= faceWorldCoords[ index ];
					GL.glVertex3f( worldCoord.X, worldCoord.Y, worldCoord.Z );
				}

				GL.glEnd();

				GL.glLineWidth( 1f );
				GL.glColor3f( 0, 0, 0 );
				GL.glBegin( GL.Primative.LineLoop );

				for( int index = 0; index < faceVerticies; index ++ ) {
					Vector3D	worldCoord		= faceWorldCoords[ index ];
					GL.glVertex3f( worldCoord.X, worldCoord.Y, worldCoord.Z );
				}

				GL.glEnd();
			}

		}	
	}

}