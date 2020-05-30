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
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;
using Exocortex;
using Exocortex.Collections;
using Exocortex.Geometry3D;
using Exocortex.OpenGL;


namespace ExoEngine.Geometry {

	/*public class Ray : Entity {

		protected Vector3D _pt0 = null;
		protected Vector3D _pt1 = null;
		protected Vector3D _ptI = null;
		protected Vector3D _ptJ = null;

		public Ray( Vector3D pt0, Vector3D pt1, BSPTreeNode root ) {
			_pt0 = pt0;
			_pt1 = pt1;
			//_ptI = pt1;
			//_ptJ = pt0;
			Face faceI = root.GetCollision( pt0, pt1 );
			if( faceI != null ) {
				_ptI = faceI.GetPlane().GetIntersection( pt0, pt1 );
			}
			Face faceJ = root.GetCollision( pt1, pt0 );
			if( faceJ != null ) {
				_ptJ = faceJ.GetPlane().GetIntersection( pt1, pt0 );
			}
		}

		public void Reset( World world ) {
		}

		public void Draw( World world ) {
			if( _ptI == null && _ptJ == null ) {
				return;
			}
			GL.glColor3f( 0.5f, 0.5f, 0.5f );
			GL.glLineWidth( 1.0f );
			GL.glBegin( GL.Primative.Lines );
			
			if( _ptI != null ) {
				GL.glVertex3f( _pt0.X, _pt0.Y, _pt0.Z );
				GL.glVertex3f( _ptI.X, _ptI.Y, _ptI.Z );
			}
			if( _ptJ != null ) {
				GL.glVertex3f( _ptJ.X, _ptJ.Y, _ptJ.Z );
				GL.glVertex3f( _pt1.X, _pt1.Y, _pt1.Z );
			}

			GL.glEnd();

			GL.glPointSize( 3.0f );
			GL.glBegin( GL.Primative.Points );
			GL.glColor3f( 1, 1, 1 );
			if( _ptI != null ) {
				GL.glVertex3f( _ptI.X, _ptI.Y, _ptI.Z );
			}
			if( _ptJ != null ) {
				GL.glVertex3f( _ptJ.X, _ptJ.Y, _ptJ.Z );
			}
			GL.glEnd();
			GL.glLineWidth( 1.0f );
		}

	}   */
}
