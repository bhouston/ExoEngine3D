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
using ExoEngine.Rendering;

namespace ExoEngine.Geometry
{
	public class Water : Entity {

		//------------------------------------------------------------------------

		public Water() {
		}
		
		public Water( Vector3D ptMin, Vector3D ptMax, float waveHeight ) {
			this.WaveHeight = waveHeight;
			this.Origin = Vector3D.FromXYZ( ptMin.X, ptMin.Y, ptMax.Z );
			this.XSize = ptMax.X - ptMin.X;
			this.YSize = ptMax.Y - ptMin.Y;
		}

		//------------------------------------------------------------------------

		protected Vector3D[,]	_points = null;
		protected Vector3D[,]	_normals = null;

		/*protected Face CreateFace( int x0, int y0, int x1, int y1, int x2, int y2 ) {
			Face face = new Face();
			face.Points.Add( _points[ x0, y0 ] );
			face.Points.Add( _points[ x1, y1 ] );
			face.Points.Add( _points[ x2, y2 ] );
			face.VertexNormals.Add( _normals[ x0, y0 ] );
			face.VertexNormals.Add( _normals[ x1, y1 ] );
			face.VertexNormals.Add( _normals[ x2, y2 ] );
			return face;
		}	*/

		public override void Reset( World world ) {
			this.Priority	= 10;
			
			Vector3D xStep = Vector3D.FromXYZ( ((float) this.XSize ) / this.XSteps, 0, 0 );
			Vector3D yStep = Vector3D.FromXYZ( 0, ((float) this.YSize ) / this.YSteps, 0 );

			_points = new Vector3D[ ( this.XSteps + 1 ), ( this.YSteps + 1 ) ];
			_normals = new Vector3D[ ( this.XSteps + 1 ), ( this.YSteps + 1 ) ];
			for( int y = 0; y <= this.YSteps; y ++ ) {
				for( int x = 0; x <= this.XSteps; x ++ ) {
					_points[ x, y ] = this.Origin + (xStep * x) + (yStep * y);
					_normals[ x, y ] = Vector3D.FromXYZ( 0, 0, 1 );
				}
			}

			/*this.Faces = new Faces();

			for( int y = 0; y < this.YSteps; y ++ ) {
				for( int x = 0; x < this.XSteps; x ++ ) {
					Face faceA, faceB;
					if( ( x + y ) % 2 == 0 ) {
						faceA = CreateFace( x, y, x + 1, y, x + 1, y + 1 );
						faceB = CreateFace( x, y, x + 1, y + 1, x, y + 1 );
					}
					else {
						faceA = CreateFace( x, y, x + 1, y, x, y + 1 );
						faceB = CreateFace( x + 1, y, x + 1, y + 1, x, y + 1 );
					}
					faceA.IsVertexNormals = true;
					faceA.Color = this.Color;
					this.Faces.Add( faceA );

					faceB.IsVertexNormals = true;
					faceB.Color = this.Color;
					this.Faces.Add( faceB );
				}
			}

			foreach( Face face in this.Faces ) {
				face.Reset( world, this );
			}  */

			_waterModel = new WaterModel( this.XSteps + 1, this.YSteps + 1 );
		}

		static public bool IsAdvance = true;
		static public bool IsUpdateVertices = true;
		static public bool IsUpdateNormals = true;
		static public bool IsRender = true;

		protected DateTime	_timeLastAdvance = DateTime.Now;

		public override int Render( World world, RenderSettings settings ) {
			if( Math2.RandomInt( 3 ) == 0 ) {
				int x = Math2.RandomInt( this.XSteps - 3 ) + 2;
				int y = Math2.RandomInt( this.YSteps - 3 ) + 2;
				_waterModel[ x, y ] = 500;
				_waterModel[ x + 1, y ] = 300;
				_waterModel[ x - 1, y ] = 300;
				_waterModel[ x, y + 1 ] = 300;
				_waterModel[ x, y - 1 ] = 300;
				_waterModel[ x + 1, y + 1 ] = 200;
				_waterModel[ x + 1, y - 1 ] = 200;
				_waterModel[ x - 1, y + 1 ] = 200;
				_waterModel[ x - 1, y - 1 ] = 200;
			}
			if( Water.IsAdvance ) {
				DateTime now = DateTime.Now;
				TimeSpan span = now - _timeLastAdvance;
				if( span.Milliseconds >= 50 ) {
					_waterModel.Advance();
					_timeLastAdvance = now;
				}
			}

			if( Water.IsUpdateVertices ) {
				for( int y = 0; y <= this.YSteps; y ++ ) {
					for( int x = 0; x <= this.XSteps; x ++ ) {
						_points[ x, y ].Z = this.Origin.Z + ((float)_waterModel[ x, y ]) / 500 * this.WaveHeight;
					}
				}
			}

			if( Water.IsUpdateNormals ) {
				float xStepSize2Inv = 1 / ( 2 * this.XSize / this.XSteps );
				float yStepSize2Inv = 1 / ( 2 * this.YSize / this.YSteps );

				for( int y = 1; y < this.YSteps; y ++ ) {
					for( int x = 1; x < this.XSteps; x ++ ) {
						float xDelta = _points[ x + 1, y ].Z - _points[ x - 1, y ].Z;
						float yDelta = _points[ x, y + 1 ].Z - _points[ x, y - 1 ].Z;
						_normals[ x, y ].Set( - xDelta * xStepSize2Inv, - yDelta * yStepSize2Inv, 1 );
						_normals[ x, y ].Normalize();
					}
				}
			}

			if( Water.IsRender ) {
				this.Renderer.RenderQuadMesh( world, this.Color, _points, _normals, settings );
			}

			return	( this.YSteps - 1 ) * ( this.XSteps - 1 );
		}

		//------------------------------------------------------------------------

		protected ReflectionRenderer	_renderer = (ReflectionRenderer) ExoEngine.Viewer.AvailableRenderers.FindByType( typeof( ReflectionRenderer ) );
		[XmlIgnore]
		public	ReflectionRenderer	Renderer	{
			get	{	return	_renderer;	}
			set	{	_renderer = value;	}
		}

		//------------------------------------------------------------------------
															 
		protected WaterModel	_waterModel = null;

		public Vector3D	Origin = Vector3D.Origin;
		public float	XSize	= 10;
		public float	YSize	= 10;
		public float	WaveHeight = 5;
		[XmlIgnore]
		public Color	Color	= Color.Blue;
		
		public int		XSteps	= 50;
		public int		YSteps	= 50;

		//[XmlIgnore]
		//public Faces	Faces	= null;

		//------------------------------------------------------------------------

	}

}
