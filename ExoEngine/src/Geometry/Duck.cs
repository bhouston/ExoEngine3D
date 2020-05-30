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
using Exocortex.Text;
using Exocortex.Geometry3D;
using Exocortex.OpenGL;

			  
using ExoEngine.Geometry;
using ExoEngine.BSPTree;
using ExoEngine.Rendering;

namespace ExoEngine.Geometry
{
	/// <summary>
	/// Summary description for Duck.
	/// </summary>
	//[XmlName("duck")]
	public class Duck : Entity {

		public Vector3D	MidPoint	= Vector3D.Origin;
		public float	Scale		= 1.0f;
		protected	Color _clr	= Color.Yellow;

		public Duck() {
		}

		public Duck( Vector3D ptMin, Vector3D ptMax ) {
			this.MidPoint = ( ptMin + ptMax ) * 0.5f;
			this.Scale		= ( this.MidPoint - ptMin ).GetMagnitude();
		}

		public override void Reset( World world ) {
			this.Priority	= 1;
			//base.Reset( world );
			this.Renderer = ExoEngine.Viewer.AvailableRenderers.FindByType( typeof( PhongRenderer ) );
		}
		static public bool IsRender = true;

		protected Renderer	_renderer = ExoEngine.Viewer.AvailableRenderers.FindByType( typeof( ReflectionRenderer ) );
		[XmlIgnore]
		public	Renderer	Renderer	{
			get	{	return	_renderer;	}
			set	{	_renderer = value;	}
		}

		public override int Render( World world, RenderSettings settings ) {
			//this.Renderer = ExoEngine.Viewer.Renderer;
			if( Duck.IsRender ) {
				//int triangleCount = _trianglePlanes.Length;
				//Vector3D camera = world.Camera.Translation;
				//for( int t = 0; t < triangleCount; t ++ ) {
				//	_triangleVisible[ t ] = ( _trianglePlanes[ t ].GetDistanceToPlane( camera ) < 0 );
				//}
				((PhongRenderer) this.Renderer).RenderTriMesh( world, _clr, _vertices, _normals, _triangles, _trianglePlanes, settings );

				return	_triangles.Length;
				//base.Render( world, settings );
			}
			//Faces facesVisible = new Faces();
			//Vector3D	cameraDirection = -world.Camera..ForwardAxis;
			//foreach( Face face in this.Faces ) {
			//	if( Vector3D.Dot( face.Normal, cameraDirection ) > 0 ) {
			//		facesVisible.Add( face );
			//	}
			//}
			//this.Renderer.Render( world, this.Faces, settings );

			return	0;
		}

		Vector3D[]	_vertices	= null;
		Vector3D[]	_normals	= null;
		int[]		_triangles	= null;
		Plane3D[]	_trianglePlanes = null;

		public Vector3D[]	Vertices {
			get	{	return	_vertices;	}
			set {	_vertices = value;	}
		}
		public Vector3D[]	Normals  {
			get	{	return	_normals;	}
			set {	_normals = value;	}
		}
		public int[]		Triangles {
			get	{	return	_triangles;	}
			set {	_triangles = value;	}
		}
		public Plane3D[]	TrianglePlanes {
			get	{	return	_trianglePlanes;	}
			set {	_trianglePlanes = value;	}
		}

		public void LoadDataSet( string fileName, Color clr ) {
			//Debug.Assert( this.Faces.Count == 0 );
			//Debug2.Push( "Parse Object Definition File" );

			TokenReader tr = new TokenReader( fileName, ' ' );

			int vertexCount		= int.Parse( tr.GetToken() );
			int triangleCount	= int.Parse( tr.GetToken() );

			_vertices	= new Vector3D[ vertexCount ];
			_normals	= new Vector3D[ vertexCount ];
			_triangles	= new int[ triangleCount * 3 ];

			_trianglePlanes = new Plane3D[ triangleCount ];

			for( int v = 0; v < vertexCount; v ++ ) {
				tr.GetToken();
				_vertices[ v ] = new Vector3D(
					float.Parse( tr.GetToken() ),
					float.Parse( tr.GetToken() ),
					float.Parse( tr.GetToken() ) );
				_normals[ v ] = new Vector3D( 0, 0, 0 );
			}

			// translate vertices
			Vector3D ptOldMin, ptOldMax;
			FaceUtils.GetExtents( _vertices, out ptOldMin, out ptOldMax );
			Vector3D ptOldMidPoint = ( ptOldMin + ptOldMax ) * 0.5f;
			float fOldScale = ( ptOldMidPoint - ptOldMin ).GetMagnitude();

			for( int i = 0; i < vertexCount; i ++ ) {
				_vertices[i] = ( _vertices[i] - ptOldMidPoint ) / fOldScale * this.Scale + this.MidPoint;
			}			
			
			for( int t = 0; t < triangleCount; t ++ ) {
				tr.GetToken();

				// triangle vertices
				_triangles[ t * 3 + 0 ] = int.Parse( tr.GetToken() );
				_triangles[ t * 3 + 1 ] = int.Parse( tr.GetToken() );
				_triangles[ t * 3 + 2 ] = int.Parse( tr.GetToken() );

				// skip extra triangle info
				tr.GetToken();
				tr.GetToken();
				tr.GetToken();

				// calculate triangle normal
				Vector3D vertexA = _vertices[ _triangles[ t * 3 + 0 ] ];
				Vector3D vertexB = _vertices[ _triangles[ t * 3 + 1 ] ];
				Vector3D vertexC = _vertices[ _triangles[ t * 3 + 2 ] ];
				_trianglePlanes[ t ] = Plane3D.FromCoplanarPoints( vertexA, vertexB, vertexC );
				//Vector3D.Cross( vertexC - vertexB, vertexA - vertexB ).GetUnit();
			}

			tr.Close();

			//Debug2.Pop();
			/*for( int v = 0; v < vertexCount; v ++ ) {
				if( vertexNormals[ v ].GetMagnitude() > 0.1 ) {
					vertexNormals[ v ].Normalize();
				}
				else {
					vertexNormals[ v ] = new Vector3D( 1, 0, 0 );
				}				
			}  */

			//Debug2.Push( "Calculate Face Normals" );

			float fLimit = 0f;

			for( int t = 0; t < triangleCount; t ++ ) {
				for( int v = 0; v < 3; v ++ ) {
					int vIndex = _triangles[ t * 3 + v ];
					Vector3D normalSum = _trianglePlanes[ t ].Normal;
					Vector3D normal = _trianglePlanes[ t ].Normal;
					for( int t2 = 0; t2 < triangleCount; t2 ++ ) {
						if( t2 == t ) {
							continue;
						}
						bool bMatch = false;
						for( int v2 = 0; v2 < 3; v2 ++ ) {
							if( vIndex == _triangles[ t2 * 3 +v2 ] ) {
								bMatch = true;
								break;
							}
						}
						if( bMatch == true ) {
							Vector3D normal2 = _trianglePlanes[ t2 ].Normal;
							if( Vector3D.Dot( normal, normal2 ) > fLimit ) {
								normalSum += normal2;
							}
						}
					}
					if( normalSum.GetMagnitude() > 0.01 ) {
						_normals[ vIndex ] = normalSum.GetUnit();
					}
					else {
						_normals[ vIndex ] = (Vector3D) normal.Clone();
					}				
				}
			}
			//Debug2.Pop();
			
			/*
				int a = faceIndices[ f, 0 ];
				int b = faceIndices[ f, 1 ];
				int c = faceIndices[ f, 2 ];

				Face face = this.Faces[f];
				face.VertexNormals = new Vector3DCollection();
				face.VertexNormals.Add( vertexNormals[ a ].Clone() );
				face.VertexNormals.Add( vertexNormals[ b ].Clone() );
				face.VertexNormals.Add( vertexNormals[ c ].Clone() );
			}*/
		}
	}
}
