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
//using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;

namespace Exocortex.Geometry3D {

	/// <summary>
	/// Represents a polygon is 3-space.  Composed of a list of points.
	/// </summary>
	[XmlType("polygon")]
	public class Polygon3D : ICloneable {

		//-------------------------------------------------------------------------------

		/// <summary>
		/// Create a new polygon
		/// </summary>
		public Polygon3D() {
		}

		/// <summary>
		/// Create a new polygon with a set of points
		/// </summary>
		/// <param name="points"></param>
		public Polygon3D( Vector3DCollection points ) {
			Debug.Assert( points != null );
			_points.AddRange( points );
		}

		//-------------------------------------------------------------------------------

		private	Polygon3D	CreateClone() {
			Polygon3D polygon = new Polygon3D();
			for( int i = 0; i < this.Points.Count; i ++ ) {
				polygon.Points.Add( (Vector3D) this.Points[i].Clone() );
			}
			return polygon;
		}

		/// <summary>
		/// Create a deep copy of the polygon.
		/// </summary>
		/// <returns></returns>
		public Polygon3D	Clone() {
			return	CreateClone();
		}

		object	ICloneable.Clone() {
			return	(object) CreateClone();
		}
		
		//-------------------------------------------------------------------------------

		private	Vector3DCollection	_points = new Vector3DCollection();

		/// <summary>
		/// The list of points that compose this polygon
		/// </summary>
		[XmlIgnore]
		public	Vector3DCollection	Points {
			get	{	return	_points;	}
		}

		/// <summary>
		/// A variable that is used in XML serialization and deserialization
		/// </summary>
		[XmlElement("vertex")]
		public	Vector3D[]	PointsXMLDummyValue {
			get {	return	_points.ToArray();	}
			set {	_points.FromArray( value );	}
		}

		//-------------------------------------------------------------------------------

		/// <summary>
		/// Calculate the normal of the polygon
		/// </summary>
		/// <returns></returns>
		public	Vector3D		GetNormal() {
			if( _points.Count >= 3 ) {
				return	Vector3D.CrossProduct( _points[2] - _points[1], _points[0] - _points[1] ).GetUnit();
			}
			return Vector3D.Zero;
		}

		/// <summary>
		/// Calculate the plane of the polygon
		/// </summary>
		/// <returns></returns>
		public	Plane3D		GetPlane() {
			if( _points.Count < 3 ) {
				throw new PlaneIllDefinedException( "verticies in polygon are less than 3." );
			}
			// TODO: ensure that polygon is actually planar... currently no checking is done.
			return	Plane3D.FromCoplanarPoints( _points[0], _points[1], _points[2] );
		}

		//-------------------------------------------------------------------------------

		/// <summary>
		/// Flip the order of the points in the polygon and thus flipping its plane
		/// </summary>
		public void	Flip() {
			_points.Reverse();
		}

		/// <summary>
		/// Optimize the polygon by removing coincident points
		/// and collinear points.
		/// </summary>
		/// <returns>The number of points removed</returns>
		public	int	Optimize() {
			int pointsRemoved = 0;

			// remove coincident points
			if( this.Points.Count >= 2 ) {
				int					count		= _points.Count;
				Vector3DCollection	newPoints	= new Vector3DCollection();

				for( int i = 0; i < count; i ++ ) {
					Vector3D pt0	= _points[ ( i + count - 1 ) % count ];
					Vector3D pt1 = _points[ i ];
				
					if( ( pt1 - pt0 ).GetMagnitude() > Math3D.EpsilonF ) {
						newPoints.Add( pt1 );
					}
				}

				Debug.Assert( newPoints.Count <= _points.Count );
				if( newPoints.Count < _points.Count ) {
					pointsRemoved += _points.Count - newPoints.Count;
					_points = newPoints;
				}
			}

			// remove collinear points
			if( this.Points.Count >= 3 ) {
				int					count		= _points.Count;
				Vector3DCollection	newPoints	= new Vector3DCollection();

				for( int i = 0; i < count; i ++ ) {
					Vector3D pt0 = _points[ ( i + count - 1 ) % count ];
					Vector3D pt1 = _points[ i ];
					Vector3D pt2 = _points[ ( i + 1 ) % count ];

					if( Vector3D.Orientation( pt0, pt1, pt2 ) != 0 ) {
						newPoints.Add( pt1 );
					}
				}

				Debug.Assert( newPoints.Count <= _points.Count );
				if( newPoints.Count < _points.Count ) {
					pointsRemoved += _points.Count - newPoints.Count;
					_points = newPoints;
				}
			}

			return	pointsRemoved;
		}
		
		// ================================================================================
	}
}
