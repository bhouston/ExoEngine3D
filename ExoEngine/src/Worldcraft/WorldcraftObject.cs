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
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;
using System.Xml;
using Exocortex;
using Exocortex.Mathematics;
using Exocortex.Geometry3D;
using Exocortex.Text;
using ExoEngine.Geometry;

namespace ExoEngine.Worldcraft {

	//================================================================================
	//================================================================================

	public class WorldcraftObject {

		//----------------------------------------------------------------------------

		public WorldcraftObject() {
		}

		//----------------------------------------------------------------------------

		protected Hashtable	_properties	= new Hashtable();
		public Hashtable	Properties {
			get {	return	_properties;	}
		}

		protected Faces		_faces			= new Faces();
		public Faces		Faces {
			get {	return	_faces;			}
		}

		//----------------------------------------------------------------------------

		static public WorldcraftObject FromTokenReader( TokenReader tr, Textures textures ) {
			WorldcraftObject wo = new WorldcraftObject();

			Debug.Assert( tr.LookAhead == "{" );
			tr.GetToken();

			while( tr.LookAhead != null && tr.LookAhead != "}" ) {
				if( tr.LookAhead == "{" ) {
					tr.GetToken();
					WorldcraftSidePlanes sidePlanes = new WorldcraftSidePlanes();
					while( tr.LookAhead != null && tr.LookAhead == "(" ) {
						sidePlanes.Add( WorldcraftSidePlane.FromTokenReader( tr, textures ) );
					}
					wo.Faces.AddRange( ConvertSidePlanesToFaces( wo, sidePlanes, textures ) );

					Debug.Assert( tr.LookAhead == "}" );
					tr.GetToken();
				}
				else {
					string key = tr.GetToken();
					string val = tr.GetToken();
					if( wo._properties.Contains( key ) == false ) {
						wo._properties.Add( key, val );
					}
				}
			}

			Debug.Assert( tr.LookAhead != null );
			tr.GetToken();

			Debug.Assert( wo.ClassName != null );

			/*Color clrMaterial = wo.MaterialColor;
			foreach( Face face in wo.Faces ) {
				face.Color = clrMaterial;
			}*/

			return wo;
		}
	
		static protected void CutPolygonByPlanes( Polygon3D polygon, WorldcraftSidePlanes sidePlanes, int iSkipPlaneIndex ) {
			for( int i = 0; i < sidePlanes.Count; i ++ ) {
				if( i != iSkipPlaneIndex ) {
					sidePlanes[i].plane.ClipPolygon( polygon );
				}
			}
		}
		
		static protected Faces ConvertSidePlanesToFaces( WorldcraftObject wo, WorldcraftSidePlanes sidePlanes, Textures textures ) {
			Faces faces = new Faces();

			for( int i = 0; i < sidePlanes.Count; i ++ ) {

				//Consider this side plane and it's associated heavily used parts.
				WorldcraftSidePlane sp		= sidePlanes[i];

				Polygon3D polygon = sp.plane.CreatePolygon( 10000 );
				if( Vector3D.Dot( polygon.GetNormal(), sp.plane.Normal ) > 0 ) {
					polygon.Flip();
				}
				CutPolygonByPlanes( polygon, sidePlanes, i );

				if( polygon.Points.Count >= 3 ) {

					Face face = new Face();
					face.Texture	= sp.texture;
					face.SAxis		= sp.sAxis;
					face.TAxis		= sp.tAxis;
					face.Plane		= sp.plane.GetFlipped();
					face.GroupName	= wo.ClassName;

					foreach( Vector3D pt in polygon.Points ) {
						face.Points.Add( face.Plane.ProjectOntoPlane( pt ) );
					}

					faces.Add( face );
				}
			}

			return faces;
		}

		//----------------------------------------------------------------------------

		public bool		IsProperty( string propertyName ) {
			return	_properties.Contains( propertyName );
		}

		public Color	GetPropertyColor( string propertyName, Color defaultValue ) {
			if( IsProperty( propertyName ) == true ) {
				string colorString = (string) _properties[ propertyName ];
				//Debug.WriteLine( "colorString = '" + colorString + "'" );
				string[] tokens = StringTokenizer.ToTokens( colorString );
				Color color = Color.FromArgb( 255, int.Parse( tokens[0] ), int.Parse( tokens[1] ), int.Parse( tokens[2] ) );
				//Debug.WriteLine( "color = " + color );
				return color;
			}
			return	defaultValue;
		}

		public string	GetPropertyString( string propertyName ) {
			Debug.Assert( IsProperty( propertyName ) == true );
			return (string) _properties[ propertyName ];
		}

		public float	GetPropertyFloat( string propertyName, float defaultValue ) {
			if( IsProperty( propertyName ) == true ) {
				return float.Parse( (string) _properties[ propertyName ] );
			}
			return defaultValue;
		}

		public int		GetPropertyInteger( string propertyName, int defaultValue ) {
			if( IsProperty( propertyName ) == true ) {
				return int.Parse( (string) _properties[ propertyName ] );
			}
			return	defaultValue;
		}

		//----------------------------------------------------------------------------

		public string		ClassName {
			get {	return	GetPropertyString( "classname" );	}
		}

		//----------------------------------------------------------------------------
	}
	
	//================================================================================
	//================================================================================

	public class WorldcraftObjects : CollectionBase {
		public virtual void Add( WorldcraftObject o ){
			this.List.Add( o );        
		}
		public virtual WorldcraftObject this[int Index]{
			get { return (WorldcraftObject) this.List[Index];	}
		}
		public WorldcraftObject	FindByClassName( string className ) {
			foreach( WorldcraftObject wo in this ) {
				if( wo.ClassName == className ) {
					return	wo;
				}
			}
			return	null;
		}
	}

	//================================================================================
	//================================================================================

}
