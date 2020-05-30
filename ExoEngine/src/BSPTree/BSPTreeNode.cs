// Exocortex Technologies
// http://www.exocortex.org
// Copyright (c) 2001, 2002 Ben Houston (ben@exocortex.org).  All Rights Reserved.


using System;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;
using Exocortex;
using Exocortex.Collections;
using Exocortex.Geometry3D;
using Exocortex.OpenGL;

using ExoEngine.Geometry;

namespace ExoEngine.BSPTree {

	public class BSPTreeNode {

		//------------------------------------------------------------------------

		public BSPTreeNode() {
		}

		//------------------------------------------------------------------------

		static public BSPTreeNode	FromFaces( Faces faces ) {
			//Debug2.Push();

			if( faces.Count == 0 ) {
				//Debug.WriteLine( "faces == 0" );
				//Debug2.Pop();
				return null;
			}

			/*else if( faces.Count == 1 ) {
				Face face = faces[0];

				BSPTreeNode node = new BSPTreeNode();
				node.Plane = face.GetPlane();
				node.Faces.Add( face );
				node.PositiveChild	= null;
				node.NegativeChild	= null;

				return node;
			}
			else {*/

			BSPProfiler profilerMinCost = null;

			foreach( Face face in faces ) {
				BSPProfiler profiler = new BSPProfiler( face.Plane, faces );
				if( profilerMinCost == null || profilerMinCost.GetCost() > profiler.GetCost() ) {
					profilerMinCost = profiler;
				}
			}

			BSPTreeNode node = new BSPTreeNode();
			node.Plane = profilerMinCost.Plane;
			node.Faces = profilerMinCost.CoplanarFaces;

			node.PositiveChild	= BSPTreeNode.FromFaces( profilerMinCost.PositiveFaces );
			node.NegativeChild	= BSPTreeNode.FromFaces( profilerMinCost.NegativeFaces );

			return node;
		}

		//------------------------------------------------------------------------
		
		public void Reset( World world ) {
			foreach( Face face in this.Faces ) {
				face.Reset( world, this );
			}
			if( this.IsPositiveChild ) {
				this.PositiveChild.Reset( world );
			}
			if( this.IsNegativeChild ) {
				this.NegativeChild.Reset( world );
			}
		}

		//------------------------------------------------------------------------

		public	void	CollectFaces( Faces faces ) {
			if( this.IsPositiveChild ) {
				this.PositiveChild.CollectFaces( faces );
			}
			faces.AddRange( this.Faces );
			if( this.IsNegativeChild ) {
				this.NegativeChild.CollectFaces( faces );
			}
		}
		
		//------------------------------------------------------------------------

		public	Faces	GetFaces( Vector3D pt ) {
			Faces faces = new Faces();

			int sign = (int) Math3D.GetSign( _plane.GetDistanceToPlane( pt ) );

			if( sign == 0 ) {
				foreach( Face face in _faces ) {
					if( face.IsContained( pt ) ) {
						faces.Add( face );
					}
				}
				if( _positiveChild != null ) {
					faces.AddRange( _positiveChild.GetFaces( pt ) );
				}
				if( _negativeChild != null ) {
					faces.AddRange( _negativeChild.GetFaces( pt ) );
				}
				return faces;
			}

			if( sign > 0 ) {
				if( _positiveChild != null ) {
					faces.AddRange( _positiveChild.GetFaces( pt ) );
				}
				return faces;
			}

			if( sign < 0 ) {
				if( _negativeChild != null ) {
					faces.AddRange( _negativeChild.GetFaces( pt ) );
				}
				return faces;
			}

			return faces;
		}

		public	Face	GetCollision( Vector3D pt0, Vector3D pt1 ) {
			
			int sign0 = (int) Math3D.GetSign( _plane.GetDistanceToPlane( pt0 ) );
			int sign1 = (int) Math3D.GetSign( _plane.GetDistanceToPlane( pt1 ) );

			if( sign0 == 0 && sign1 == 0 ) {
				if( _positiveChild != null ) {
					Face face = _positiveChild.GetCollision( pt0, pt1 );
					if( face != null ) {
						return face;
					}
				}			
				if( _negativeChild != null ) {
					Face face = _negativeChild.GetCollision( pt0, pt1 );
					if( face != null ) {
						return face;
					}
				}
				return null;
			}

			if( sign0 > 0 && sign1 == 0 ) {
				if( _positiveChild != null ) {
					Face face = _positiveChild.GetCollision( pt0, pt1 );
					if( face != null ) {
						return face;
					}
				}			
				foreach( Face face in _faces ) {
					if( face.IsContained( pt1 ) ) {
						return face;
					}
				}
				return null;
			}
			if( sign0 >= 0 && sign1 >= 0 ) {
				return ( _positiveChild != null ) ? _positiveChild.GetCollision( pt0, pt1 ) : null;
			}
			if( sign0 <= 0 && sign1 <= 0 ) {
				return ( _negativeChild != null ) ? _negativeChild.GetCollision( pt0, pt1 ) : null;
			}

			if( sign0 > 0 && sign1 < 0 ) {
				Vector3D ptI = _plane.GetIntersection( pt0, pt1 );
				if( _positiveChild != null ) {
					Face face = _positiveChild.GetCollision( pt0, ptI );
					if( face != null ) {
						return face;
					}
				}
				foreach( Face face in _faces ) {
					if( face.IsContained( ptI ) ) {
						return	face;
					}
				}
				return ( _negativeChild != null ) ? _negativeChild.GetCollision( ptI, pt1 ) : null;
			}

			if( sign0 < 0 && sign1 > 0 ) {
				Vector3D ptI = this.Plane.GetIntersection( pt0, pt1 );
				if( _negativeChild != null ) {
					Face face = _negativeChild.GetCollision( pt0, ptI );
					if( face != null ) {
						return face;
					}
				}
				/*foreach( Face face in _faces ) {
					if( face.IsContained( ptI ) ) {
						return face;
					}
				}  */
				return ( _positiveChild != null ) ? _positiveChild.GetCollision( ptI, pt1 ) : null;
			}

			return null;
		}
		
		//------------------------------------------------------------------------

		protected Plane3D	_plane = Plane3D.Zero;
		[XmlElement("plane")]
		public Plane3D	Plane {
			get	{	return	_plane;	}
			set {	_plane = value;	}
		}

		protected Faces	_faces = new Faces();
		[XmlIgnore]
		public Faces	Faces {
			get {	return	_faces;	}
			set {	_faces = value;	}
		}

		[XmlElement("face")]
		public	Face[]	FacesXMLDummyValue {
			get {	return	_faces.ToArray();	}
			set {	_faces.FromArray( value );	}
		}

		//------------------------------------------------------------------------
		//------------------------------------------------------------------------

		protected BSPTreeNode	_positiveChild	= null;
		[XmlIgnore]
		public bool			IsPositiveChild {
			get {	return	_positiveChild != null;	}
		}
		[XmlElement("positiveChild")]
		public BSPTreeNode	PositiveChild {
			get	{	return	_positiveChild;	}
			set {	_positiveChild = value;	}
		}

		protected BSPTreeNode	_negativeChild	= null;
		[XmlIgnore]
		public bool			IsNegativeChild {
			get {	return	_negativeChild != null;	}
		}
		[XmlElement("negativeChild")]
		public BSPTreeNode	NegativeChild {
			get	{	return	_negativeChild;	}
			set {	_negativeChild = value; }
		}

		//------------------------------------------------------------------------
		//------------------------------------------------------------------------

		public int	CountNodes() {
			int nodeCount = 1;
			if( this.IsPositiveChild ) {
				nodeCount += this.PositiveChild.CountNodes();
			}
			if( this.IsNegativeChild ) {
				nodeCount += this.NegativeChild.CountNodes();
			}
			return	nodeCount;
		}

		public int	CountFaces() {
			int faceCount = this.Faces.Count;
			if( this.IsPositiveChild ) {
				faceCount += this.PositiveChild.CountFaces();
			}
			if( this.IsNegativeChild ) {
				faceCount += this.NegativeChild.CountFaces();
			}
			return	faceCount;
		}

		//------------------------------------------------------------------------
		//------------------------------------------------------------------------

	}

}
