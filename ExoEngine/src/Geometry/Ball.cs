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
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;
using Exocortex;
using Exocortex.Collections;
using Exocortex.Geometry3D;
using Exocortex.OpenGL;



namespace ExoEngine.Geometry
{
	/// <summary>
	/// Summary description for Ball.
	/// </summary>
	/*public class Ball : Entity {

		protected Vector3D _g = new Vector3D( 0, 0, -9.8f );
		protected float	  _e = 0.8f;//0.9f;
		protected float   _f = 0.95f;

		protected Vector3D _x = null;
		protected Vector3D _v = null;
		protected BSPTreeNode _root = null;

		protected Color _clr = Color.White;

		protected Vector3DCollection _vTrail = new Vector3DCollection();

		public Ball( Vector3D position, Vector3D velocity, float e, BSPTreeNode root, Color clr ) {
			_x = position;
			_v = velocity;
			_e = e;
			_root = root;
			_clr = clr;
		}


		public void Reset( World world ) {
		}

		//protected bool _bBounce = false;

		protected Face _face = null;

		public void Advance( float deltaT ) {
			Vector3D	velocityT0 = _v;
			Vector3D positionT0 = _x;
			Vector3D gravity = _g;
			float mass = 1.0f;
			float elasticity = _e;
			float fCoefficient = 0.09f;

			// get the faces that the ball is resting upon, if any
			Faces facesT0 = _root.GetFaces( positionT0 );
			if( facesT0.Count > 1 ) {
				Debug.Write( facesT0.Count + " " );
			}
			
			Vector3D Fgravity = gravity * mass;
			Vector3D Fmovement = Fgravity.Clone();
			float Ffriction = 0;
			
			foreach( Face face in facesT0 ) {
				Vector3D normal = face.Normal;

				float v = - Vector3D.Dot( velocityT0, normal );
				if( v > 0 ) {
					velocityT0 += normal * v;
				}

				Ffriction += mass * fCoefficient;
			}

			Vector3D velocityT1 = ( velocityT0 + Fgravity * deltaT ) * ( 1 - Ffriction * deltaT );
			Vector3D positionT1 = positionT0 + 0.5f * ( velocityT0 + velocityT1 ) * deltaT;

			foreach( Face face in facesT0 ) {
				Plane3D plane = face.Plane;
				if( plane.GetDistanceToPlane( positionT1 ) < 0 ) {
					// Debug.WriteLine( "  adjusting positionT1" );
					positionT1 = plane.ProjectOntoPlane( positionT1 );
				}
			}

			//Debug.WriteLine( "p0 = " + positionT0 + "  p1 = " + positionT1 + "  v0 = " + velocityT0 + "  v1 = " + velocityT1 );

			Face faceCollision = _root.GetCollision( positionT0, positionT1 );
			if( faceCollision == null ) {
				if( ( positionT1 - positionT0 ).GetMagnitude() < ( Math2.EpsilonF * 10 ) ) {
					_x = null;
					return;
				}
				_x = positionT1;
				_v = velocityT1;
				return;
			}
			//Debug.WriteLine( "  Collision" );
	  
			/*if( _face.Plane.IsIntersection( x0, x1 ) == false ) {
				_x = null;
				return;
			}* /

			Vector3D positionTI = faceCollision.Plane.GetIntersection( positionT0, positionT1 );
			positionTI = faceCollision.Plane.ProjectOntoPlane( positionTI );

			float deltaTI = 2 * ( positionTI - positionT0 ).GetMagnitude() /
				( velocityT0 + velocityT1 ).GetMagnitude();
			
			Vector3D velocityTI = velocityT0 + Fgravity * deltaTI;
			
			/*float dotnvi = Vector3D.Dot( n, vi );
			if( dotnvi > 0 ) {
				_v = v1;
				_x = x1;
				return;
			}* /

			Vector3D planeNormal = faceCollision.Plane.Normal;
			Vector3D velocityTR = ( velocityTI - 2 * planeNormal * Vector3D.Dot( planeNormal, velocityTI ) ) * elasticity;
			
			_x = positionTI;
			_v = velocityTR;

			if( ( deltaT - deltaTI ) != 0 ) {
				Advance( deltaT - deltaTI );
			}
		}

		public void Draw( World world ) {
			Advance( .5f );

			if( _x == null || _x.Z < - 1000 ) {
				world.Entities.Remove( this );
				return;
			}

			// draw trail
			/*GL.glColor3f( 0.5f, 0.5f, 0.5f );
			GL.glLineWidth( 1.0f );
			GL.glBegin( GL.Primative.LineStrip );
			foreach( Vector3D p in _vTrail ) {
				GL.glVertex3f( p.X, p.Y, p.Z );
			}
			GL.glEnd();* /

			world.PolygonCount ++;

			
			// draw ball
			//if( _face != null ) {
			//	GL.glPointSize( 5.0f );
			//}
			//else {
			GL.glPointSize( 3.0f );
			//}
			
			_clr = Color.FromArgb( 255, 
				(int) Math2.Clamp( Math.Abs( _v.X * 10 ), 0, 255 ),
				(int) Math2.Clamp( Math.Abs( _v.Y * 10 ), 0, 255 ),
				(int) Math2.Clamp( Math.Abs( _v.Z * 10 ), 0, 255 ) );

			GL.glColor( _clr );
			GL.glBegin( GL.Primative.Points );
			GL.glVertex3f( _x.X, _x.Y, _x.Z );
			GL.glEnd();
		}
	}	*/
}
