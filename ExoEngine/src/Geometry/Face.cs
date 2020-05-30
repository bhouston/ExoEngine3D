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
using System.Xml.Serialization;
using System.Xml;
using System.Drawing;

using Exocortex;
using Exocortex.Collections;
using Exocortex.Geometry3D;
using Exocortex.OpenGL;

namespace ExoEngine.Geometry {

	[XmlType("face")]
	public class Face : Polygon3D, ICloneable {
						
		public Face() : base() {
		}

		// ================================================================================
		
		private Face	CreateClone() {
			Face face = new Face();

			foreach( Vector3D pt in this.Points ) {
				face.Points.Add( (Vector3D) pt.Clone() );
			}
			
			face.Texture	= this.Texture;
			face.SAxis		= this.SAxis;
			face.TAxis		= this.TAxis;
			face.Plane		= this.Plane;
			face.GroupName	= this.GroupName;

			return face;
		}
		
		public new Face Clone() {
			return	CreateClone();
		}

		object ICloneable.Clone() {
			return	(object)	CreateClone();
		}
		
		// ================================================================================

		protected int _resetCount = 0;

		public virtual void Reset( World world, object parent ) {
			_resetCount ++;

			if( _resetCount > 1 ) {
				//Debug.WriteLine( "Face.Reset() - " + _resetCount );
			}

			// synchronize TextureIndex with actuall index of Texture in World.Textures.
			if( this.Texture == null ) {
				if( this.TextureIndex != -1 ) {
					Debug.Assert( this.TextureIndex < world.Textures.Count );
					this.Texture = world.Textures[ this.TextureIndex ];
				}
				else {
					this.TextureIndex = -1;
				}
			}
			else if( this.Texture != null ) {
				this.TextureIndex = world.Textures.IndexOf( this.Texture );
			}

			Debug.Assert( ( this.Texture == null ) == ( this.TextureIndex == -1 ) );

			if( this.IsTexture ) {
				// ensure bitmap of texture is loaded
				if( this.Texture.IsBitmapLoaded == false ) {
					this.Texture.LoadBitmap();
				}
		
				// generate texture coordinates
				Matrix3D xfrm = GetTextureXfrm();
				this.TextureCoords.Clear();
				foreach( Vector3D pt in this.Points ) {
					this.TextureCoords.Add( xfrm * pt );
				}
			}
			else {
				this.TextureCoords.Clear();
			}

			// create Normal and Plane
			//this.Normal = this.Plane.Normal;//GetNormal();
			//this.Plane = this.GetPlane();

			//if( this.Plane == null ) {
			this.Plane = this.GetPlane();
			//}

			// project points onto plane
			for( int i = 0; i < this.Points.Count; i ++ ) {
				Vector3D pt = this.Points[i];
				float fDistance = this.Plane.GetDistanceToPlane( pt );
				if( fDistance != 0 ) {
					//Debug.Write( "distance[" + i + "]   before(" + fDistance + ")  ->  " );
					pt = this.Plane.ProjectOntoPlane( pt );
					this.Points[i] = pt;
					//Debug.WriteLine( "after(" + this.Plane.GetDistanceToPlane( pt ) + ")" );
				}
			}

			// recreate side planes for Face
			Vector3D ptNormal = this.Normal;
			int count = this.Points.Count;
			this.SidePlanes.Clear();
			for( int i = 0; i < count; i ++ ) {
				Vector3D a = this.Points[ ( i + count - 1 ) % count ];
				Vector3D b = this.Points[ i ];
				Vector3D c = b + ptNormal;
				this.SidePlanes.Add( Plane3D.FromCoplanarPoints( a, b, c ) );
			}

			// 
			this.IsVertexNormals = ( this.VertexNormals.Count > 0 );
			if( this.IsVertexNormals ) {
				Debug.Assert( this.VertexNormals.Count == this.Points.Count );
			}

			// compute face midpoint
			this.MidPoint = this.GetMidPoint();
		}

		// ================================================================================

		/*public void Flatten() {
			Plane3D plane = this.Plane;
			// project points onto plane
			for( int i = 0; i < this.Points.Count; i ++ ) {
				Vector3D pt = this.Points[i];
				float fDistance = plane.GetDistanceToPlane( pt );
				if( fDistance != 0 ) {
					Debug.Write( "distance[" + i + "]   before(" + fDistance + ")  ->  " );
					pt = pt - plane.Normal * fDistance;
					this.Points[i] = pt;
					Debug.WriteLine( "after(" + plane.GetDistanceToPlane( pt ) + ")" );
				}
			}
		}	*/

		// ================================================================================

		public virtual void Render( World world ) {
			//Debug.Assert( this.Points.Count == 0 );
			//Debug.Write(".");

			GL.glBindTexture( GL.Texture.Texture2D, this.Texture.OpenGLHandle );
			GL.glBegin( GL.Primative.Polygon );
			
			// set normal
			//Vector3D n = this.GetNormal();
			//GL.glNormal3f( n.X, n.Y, n.Z );

			int count = this.Points.Count;
			for( int i = 0; i < count; i ++ ) {
				Vector3D w = this.Points[i];
				Vector3D t = this.TextureCoords[i];
				GL.glTexCoord2f( t.X, t.Y );
				GL.glVertex3f( w.X, w.Y, w.Z );
			}
			GL.glEnd();

			/*GL.glBegin( GL.Primative.Lines );
			Vector3D m = this.MidPoint;
			GL.glVertex3f( m.X, m.Y, m.Z );
			m += (n * 100);
			GL.glVertex3f( m.X, m.Y, m.Z );
			GL.glEnd();*/

			
			//GL.glErrorCheck();
		}

		// ================================================================================

		public Matrix3D	GetTextureXfrm() {
			// ensure bitmap of texture is loaded
			if( this.Texture.IsBitmapLoaded == false ) {
				this.Texture.LoadBitmap();
			}

			// generate texture transform
			Matrix3D xfrm = Matrix3D.FromBasis( this.SAxis, this.TAxis, Vector3D.Cross( this.SAxis, this.TAxis ) );
			xfrm.Scale( this.Texture.Width, this.Texture.Height, 1 );
			xfrm.Invert();
			return xfrm;
		}

		public Vector3D	GetMidPoint() {
			Vector3D sum = new Vector3D();
			foreach( Vector3D pt in this.Points ) {
				sum += pt;
			}
			return	sum / this.Points.Count;
		}

		public bool Visible = true;

		public bool	IsContained( Vector3D pt ) {
			//		Debug2.Push( "IsContained( " + pt + " )" );
			if( this.SidePlanes == null || this.SidePlanes.Count != this.Points.Count ) {
				// recreate side planes for Face
				Vector3D ptNormal = this.GetNormal();
				int count = this.Points.Count;
				this.SidePlanes.Clear();
				for( int i = 0; i < count; i ++ ) {
					Vector3D a = this.Points[ ( i + count - 1 ) % count ];
					Vector3D b = this.Points[ i ];
					Vector3D c = b + ptNormal;
					this.SidePlanes.Add( Plane3D.FromCoplanarPoints( a, b, c ) );
				}
			}

			Debug.Assert( this.SidePlanes.Count == this.Points.Count );
			bool bInside = true;
			foreach( Plane3D plane in this.SidePlanes ) {
//				Debug.WriteLine( "plane " + plane + " sign " + plane.GetSign( pt ) );
				if( plane.GetSign( pt ) > 0 ) {
					bInside = false;
//					Debug.WriteLine( "  outside!" );
					break;
				}
			}

//			Debug2.Pop();
			return bInside;
		}

		protected int _textureIndex = -1;
		[XmlAttribute("texture")]
		public int TextureIndex {
			get {	return	_textureIndex; }
			set {	_textureIndex = value;	}
		}

		protected Vector3D	_sAxis = Vector3D.Zero;
		[XmlElement("sAxis")]
		public Vector3D	SAxis {
			get {	return	_sAxis;	}
			set {	_sAxis = value;	}
		}

		protected Vector3D	_tAxis = Vector3D.Zero;
		[XmlElement("tAxis")]
		public Vector3D	TAxis {
			get {	return	_tAxis;	}
			set {	_tAxis = value;	}
		}
		
		protected Texture _texture = null;
		[XmlIgnore]
		public Texture Texture {
			get {	return	_texture;	}
			set {	_texture = value;	}
		}

		[XmlIgnore]
		public bool IsTexture {
			get {	return	( _textureIndex != -1 );	}
		}

		protected string _groupName = null;
		[XmlAttribute]
		public string GroupName {
			get {	return	_groupName;	}
			set {	_groupName = value;	}
		}

		/*protected Group _group = null;
		[XmlIgnore]
		public Group Group {
			get {	return	_group;	}
			set {	_group = value;	}
		}*/

		// ================================================================================

		protected Vector3DCollection  _textureCoords = new Vector3DCollection();
		[XmlIgnore]
		public Vector3DCollection TextureCoords {
			get {	return	_textureCoords;	}
			set {	_textureCoords = value;	}
		}

		protected Vector3DCollection  _vertexNormals = new Vector3DCollection();
		[XmlIgnore]
		public Vector3DCollection VertexNormals {
			get {	return	_vertexNormals;	}
			set {	_vertexNormals = value;	}
		}

		protected bool	_isVertexNormals	= false;
		[XmlIgnore]
		public bool		IsVertexNormals {
			get	{	return	_isVertexNormals;	}
			set {	_isVertexNormals = value;	}
		}

		[XmlElement("verNorm")]
		public	Vector3D[]	VertexNormalsDummyValue {
			get {	return	_vertexNormals.ToArray();	}
			set {	_vertexNormals.FromArray( value );	}
		}

		[XmlIgnore]
		public Vector3D	Normal {
			get {	return	_plane.Normal;	}
		}

		protected Plane3D	_plane = Plane3D.Zero;
		[XmlIgnore]
		public Plane3D	Plane {
			get {	return	_plane;	}
			set {	_plane = value;	}
		}

		protected Vector3D	_midpoint = Vector3D.Zero;
		[XmlIgnore]
		public Vector3D	MidPoint {
			get {	return	_midpoint;	}
			set {	_midpoint = value;	}
		}

		protected Plane3DCollection	_sidePlanes = new Plane3DCollection();
		[XmlIgnore]
		public Plane3DCollection	SidePlanes {
			get {	return	_sidePlanes;	}
			set {	_sidePlanes = value;	}
		}

		protected Color		_clr =  Color.White;
		[XmlIgnore]
		public Color	Color {
			get {	return	_clr;	}
			set {	_clr = value;	}
		}

		[XmlAttribute("rgba")]
		public	int	ColorXMLDummyValue {
			get	{	return	_clr.ToArgb();			}
			set	{	_clr = Color.FromArgb( value );	}
		}
		
		// ================================================================================
	}

	// ================================================================================
	// ================================================================================
	
	public class Faces : CollectionBase2 {
		public virtual void Add( Face o ){
			this.List.Add( o );        
		}
		public virtual Face this[int Index]{
			get { return (Face) this.List[Index];	}
		}
		public void Remove( Face o ) {
			this.List.Remove( o );
		}
		public void	FromArray( Face[] array ) {
			this.Clear();
			foreach( Face o in array ) {
				this.Add( o );
			}
		}
		public Face[] ToArray() {
			Face[] array = new Face[ this.Count ];
			this.List.CopyTo( array, 0 );
			return array;
		}
	}

	// ================================================================================
	// ================================================================================


}
