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
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.InteropServices;


namespace Exocortex.Geometry3D {

	/// <summary>
	/// Represents a plane in 3-space.
	/// xA + yB + zC - D = 0
	/// </summary>
	[XmlType("plane"),StructLayout(LayoutKind.Sequential)]
	public struct Plane3D : ICloneable {

		//--------------------------------------------------------------------------------

		/// <summary>
		/// Create a new plane
		/// </summary>
		/// <param name="normal"></param>
		/// <param name="constant"></param>
		public Plane3D( Vector3D normal, float constant ) {
			this.Normal = normal;
			this.Constant = constant;
		}

		//--------------------------------------------------------------------------------

		/// <summary>
		/// Create the plane that has the given normal and is coplanar with the given point
		/// </summary>
		/// <param name="normal"></param>
		/// <param name="coplanarPoint"></param>
		/// <returns></returns>
		static public Plane3D	FromNormalAndPoint( Vector3D normal, Vector3D coplanarPoint ) {
			return	new Plane3D( normal.GetUnit(), Vector3D.Dot( normal, coplanarPoint ) );
		}
		
		/// <summary>
		/// Create the plane defined by the three given points
		/// </summary>
		/// <param name="pt0"></param>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		/// <returns></returns>
		static public Plane3D	FromCoplanarPoints( Vector3D pt0, Vector3D pt1, Vector3D pt2 ) {
			Vector3D normal = Vector3D.Cross( pt2 - pt1, pt0 - pt1 ).GetUnit();
			return	new Plane3D( normal, Vector3D.Dot( normal, pt0 ) );
		}
		

		object	ICloneable.Clone() {
			Plane3D plane	= new Plane3D();
			plane.Constant	= this.Constant;
			plane.Normal	= (Vector3D) this.Normal.Clone();
			return plane;
		}

		/// <summary>
		/// Copy
		/// </summary>
		/// <returns></returns>
		public Plane3D	Clone() {
			return	new Plane3D( this.Normal, this.Constant );
		}

		//--------------------------------------------------------------------------------

		/// <summary>
		/// The Normal parameter of the plane when defined as "Normal*( pt ) - Constant = 0" 
		/// </summary>
		[XmlIgnore]
		public	Vector3D	Normal;

		/// <summary>
		/// The Constant parameter of the plane when defined as "Normal*( pt ) - Constant = 0"
		/// </summary>
		[XmlIgnore]
		public	float	Constant;

		/// <summary>
		/// The A parameter of the plane when defined as "Ax + By + Cz - D = 0"
		/// </summary>
		[XmlAttribute]
		public	float	A {
			get {	return	Normal.X;	}
			set {	Normal.X = value;	}
		}

		/// <summary>
		/// The B parameter of the plane when defined as "Ax + By + Cz - D = 0"
		/// </summary>
		[XmlAttribute]
		public	float	B {
			get {	return	Normal.Y;	}
			set {	Normal.Y = value;	}
		}
		
		/// <summary>
		/// The C parameter of the plane when defined as "Ax + By + Cz - D = 0"
		/// </summary>
		[XmlAttribute]
		public	float	C {
			get {	return	Normal.Z;	}
			set {	Normal.Z = value;	}
		}
		
		/// <summary>
		/// The D parameter of the plane when defined as "Ax + By + Cz - D = 0"
		/// </summary>
		[XmlAttribute]
		public	float	D {
			get {	return	Constant;	}
			set {	Constant = value;	}
		}
			
		//--------------------------------------------------------------------------------
	
		/// <summary>
		/// Flip the plane.
		/// </summary>
		public	void	Flip() {
			Normal = - Normal;
			Constant = - Constant;
		}

		/// <summary>
		/// Create a new plane that is identical the current but flipped.
		/// </summary>
		/// <returns></returns>
		public Plane3D	GetFlipped() {
			Plane3D plane = (Plane3D) this.Clone();
			plane.Flip();
			return plane;
		}

		/// <summary>
		/// Get the shortest distance from the point to the plane
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public	float	GetDistanceToPlane( Vector3D pt ) {
			return	Vector3D.Dot( pt, Normal ) - Constant; 
		}

		/// <summary>
		/// Get the sign of the point in relation to the plane
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public	Sign		GetSign( Vector3D pt ) {
			return Math3D.GetSign( GetDistanceToPlane( pt ), Math3D.EpsilonF );
		}

		/// <summary>
		/// Get the aggregate sign of the polygon in relation to the plane
		/// </summary>
		/// <param name="polygon"></param>
		/// <returns></returns>
		public	Sign		GetSign( Polygon3D polygon ) {
			Sign sign = Sign.Undefined;

			foreach( Vector3D p in polygon.Points ) {
				sign = Math3D.CombineSigns( sign, this.GetSign( p ) );
			}

			return sign;
		}

		/// <summary>
		/// Project a point onto the plane
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public	Vector3D	ProjectOntoPlane( Vector3D pt ) {
			return	pt + ( Constant - Vector3D.Dot( pt, Normal ) ) * Normal;
		}

		/// <summary>
		/// Determine whether the line connecting pt0 to pt1 intersects the plane
		/// </summary>
		/// <param name="pt0"></param>
		/// <param name="pt1"></param>
		/// <returns></returns>
		public	bool	IsIntersection( Vector3D pt0, Vector3D pt1 ) {
			int sign0 = (int) Math3D.GetSign( GetDistanceToPlane( pt0 ) );
			int sign1 = (int) Math3D.GetSign( GetDistanceToPlane( pt1 ) );
			
			return ( sign0 < 0 && sign1 > 0 ) || ( sign0 > 0 && sign1 <= 0 );
		}

		/// <summary>
		/// Determine the exact location where the given line segment intersects the plane
		/// </summary>
		/// <param name="pt0"></param>
		/// <param name="pt1"></param>
		/// <returns></returns>
		public	Vector3D	GetIntersection( Vector3D pt0, Vector3D pt1 ) {
			Debug.Assert( IsIntersection( pt0, pt1 ) == true );

			Vector3D ptDirection = pt1 - pt0;
			float denominator = Vector3D.Dot( Normal, ptDirection );
			if( denominator == 0 ) {
				throw new DivideByZeroException( "Can not get the intersection of a plane with a line when they are parallel to each other." );
			}
			float t = ( Constant - Vector3D.Dot( Normal, pt0 ) ) / denominator;
			return	pt0 + ptDirection * t;
		}

		//--------------------------------------------------------------------------------

		/// <summary>
		/// Clip the given polygon by the plane so that only the regions
		/// located positive of the plane remain.
		/// </summary>
		/// <param name="polygon"></param>
		public	void	ClipPolygon( Polygon3D polygon ) {

			Vector3D ptNormal = polygon.GetNormal();

			Vector3DCollection	vNewPoints = new Vector3DCollection();
			int iPoints = polygon.Points.Count;

			for ( int i = 0; i < iPoints; i ++ ) {
				Vector3D pt0 = polygon.Points[ ( i + iPoints - 1 ) % iPoints ];
				Vector3D pt1 = polygon.Points[ i ];

				int sign0 = (int) Math3D.GetSign( this.GetDistanceToPlane( pt0 ), Math3D.EpsilonF );
				int sign1 = (int) Math3D.GetSign( this.GetDistanceToPlane( pt1 ), Math3D.EpsilonF );

				if( sign0 > 0 ) {
					// line is infront
					if ( sign1 >= 0 ) {
						vNewPoints.Add( pt1 );
					}
						// line is entering plane
					else if( sign1 < 0 ) {
						Debug.Assert( sign0 > 0 && sign1 < 0 );
						vNewPoints.Add( this.GetIntersection( pt0, pt1 ) );
					}
				}
				else if( sign0 == 0 ) {
					// line is infront
					if( sign1 > 0 ) {
						vNewPoints.Add( pt1 );
					}
						// line is coplanar
					else if( sign1 == 0 ) {
						vNewPoints.Add( pt1 );
					}
						// line is behind
					else if( sign1 < 0 ) {
					}
				}
				else if( sign0 < 0 ) {
					// line is leaving plane
					if( sign1 > 0 ) {
						Debug.Assert( sign0 < 0 && sign1 > 0 );
						vNewPoints.Add( this.GetIntersection( pt0, pt1 ) );
						vNewPoints.Add( pt1 );
					}
						// line is leaving plane
					else if( sign1 == 0 ) {
						vNewPoints.Add( pt1 );
					}
						// line is behind
					else if( sign1 < 0 ) {
					}
				}
			}

			// set new points
			polygon.Points.Clear();
			polygon.Points.AddRange( vNewPoints );

			/*if( this.Points.Count >= 3 ) {
				if( Vector3D.Dot( this.GetNormal(), ptNormal ) < 0 ) {
					this.Flip();
				}
			}  */

			polygon.Optimize();
		}

		//--------------------------------------------------------------------------------
		
		/// <summary>
		/// Are two planes equal?
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public	bool	operator==( Plane3D a, Plane3D b ) {
			return ( a.Constant == b.Constant ) && ( a.Normal == b.Normal );
		}

		/// <summary>
		/// Are two planes not equal?
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public	bool	operator!=( Plane3D a, Plane3D b ) {
			return ( a.Constant != b.Constant ) || ( a.Normal != b.Normal );
		}

		/// <summary>
		/// Is given object equal to current planes?
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool	Equals( object o ) {
			if( o is Plane3D ) {
				Plane3D plane = (Plane3D) o;
				return ( this.Constant == plane.Constant ) && ( this.Normal == plane.Normal );
			}
			return false;
		}

		/// <summary>
		/// Get the hashcode of the planes
		/// </summary>
		/// <returns></returns>
		public override int		GetHashCode() {
			return	this.Constant.GetHashCode() ^ this.Normal.GetHashCode();
		}
		
		//--------------------------------------------------------------------------------

		/// <summary>
		/// Get an orthogonal basis for the plane
		/// </summary>
		/// <param name="u"></param>
		/// <param name="v"></param>
		public void		GetBasis( out Vector3D u, out Vector3D v ) {
			Vector3D pt = this.ProjectOntoPlane( Vector3D.Origin );

			u = this.ProjectOntoPlane( pt + Vector3D.XAxis ) - pt;
			u = Vector3D.Max( u, this.ProjectOntoPlane( pt + Vector3D.YAxis ) - pt );
			u = Vector3D.Max( u, this.ProjectOntoPlane( pt + Vector3D.ZAxis ) - pt );

			v = Vector3D.Cross( u, this.Normal );

			u.Normalize();
			v.Normalize();
		}

		/// <summary>
		/// Create a polygon coplanar with the current plane that extends to the given extent
		/// </summary>
		/// <param name="fExtent"></param>
		/// <returns></returns>
		public Polygon3D	CreatePolygon( float fExtent ) {
			int a, b, c;

			float[] n = new float[3] {
										 Math.Abs( Normal.X ),
										 Math.Abs( Normal.Y ),
										 Math.Abs( Normal.Z )
									 };

			if( n[0] >= n[1] && n[0] >= n[2] ) {
				a = 1;	b = 2;	c = 0;
			}
			else if( n[1] >= n[0] && n[1] >= n[2] ) {
				a = 0;	b = 2;	c = 1;
			}
			else if( n[2] >= n[0] && n[2] >= n[1] ) {
				a = 0;	b = 1;	c = 2;
			}
			else {
				a = b = c = -1;
				Debug.Assert( false );
			}

			//Debug.WriteLine( " normal[" + Normal.ToString() + " a[" + a + "] b[" + b + "] c[" + c + "]" );

			int[]	aSigns = new int[4] {  1,  1, -1, -1 };
			int[]	bSigns = new int[4] {  1, -1, -1,  1 };

			Polygon3D	poly = new Polygon3D();

			for( int i = 0; i < 4; i ++ ) {
				Vector3D pt = new Vector3D();

				pt[a] = fExtent * aSigns[i];
				pt[b] = fExtent * bSigns[i];
				pt[c] = ( Constant -  Normal[a] * pt[a] - Normal[b] * pt[b] ) / Normal[c];

				//Debug.WriteLine( " pt[" + i + "] = " + pt.ToString() + " dTp = " + DistanceToPlane( pt ) );
			
				poly.Points.Add( pt );
			}

			//Debug.WriteLine( " plane.Normal = " + Normal ); 
			//Debug.WriteLine( " plane.Constant = " + Constant ); 

			if( Vector3D.Dot( Normal, poly.GetNormal() ) < 0 ) {
				poly.Flip();
			}

			//Debug.WriteLine( " polygon.Normal = " + poly.Normal().ToString() ); 

			return	poly;
		}

		// ================================================================================

		/// <summary>
		/// Get a description of the plane.
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return	"Plane3D [ n=" + Normal.ToString() + " c=" + Constant + " ]";
		}

		/// <summary>
		/// Zero
		/// </summary>
		static public readonly Plane3D	Zero	= new Plane3D();
	}

	// ================================================================================
	// ================================================================================

}
