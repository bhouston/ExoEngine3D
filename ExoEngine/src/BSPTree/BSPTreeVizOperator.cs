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
using ExoEngine.BSPTree;

namespace ExoEngine.BSPTree {

	public class BSPTreeVizOperator {
	
		//------------------------------------------------------------------------

		static public Faces GetVisibleFaces( Vector3D origin, BSPTreeNode node ) {
			Debug.Assert( node != null );
			Faces faces = new Faces();
			TraverseBSPTree( origin, node, faces );
			return faces;
		}

		static protected void TraverseBSPTree( Vector3D origin, BSPTreeNode node, Faces faces ) {
			if( node != null ) {

				int sign = (int) Math3D.GetSign( node.Plane.GetDistanceToPlane( origin ) );

				if( sign > 0 ) {
					TraverseBSPTree( origin, node.NegativeChild, faces );
					foreach( Face face in node.Faces ) {
						if( face.Visible == true ) {
							faces.Add( face );
						}
					}
					TraverseBSPTree( origin, node.PositiveChild, faces );
				}
				else /*if( sign <= 0 )*/ {
					TraverseBSPTree( origin, node.PositiveChild, faces );
					TraverseBSPTree( origin, node.NegativeChild, faces );
				}
			}
		}

		//------------------------------------------------------------------------

	}

}
