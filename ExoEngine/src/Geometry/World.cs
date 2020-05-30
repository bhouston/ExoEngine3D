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
using System.Drawing;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using Exocortex;
using Exocortex.Geometry3D;
using Exocortex.OpenGL;
using ExoEngine.Worldcraft;

using ExoEngine.BSPTree;
using ExoEngine.Rendering;

namespace ExoEngine.Geometry {

	[XmlRoot("world")]
	public class World {

		//-----------------------------------------------------------------------------

		[XmlIgnore]
		public Vector3D		LocalForwardAxis	= Vector3D.FromXYZ( 0, -1, 0 );	// forward = + z
		[XmlIgnore]
		public Vector3D		LocalUpAxis			= Vector3D.FromXYZ( 0, 0, 1 );	// up = + y
		[XmlIgnore]
		public Vector3D		LocalRightAxis		= Vector3D.FromXYZ( 1, 0, 0 );	// right = - x
		
		/*[XmlIgnore]
		public Vector3D		LocalForwardAxis	= Vector3D.FromXYZ( 0, 0, 1 );	// forward = + z
		[XmlIgnore]
		public Vector3D		LocalUpAxis			= Vector3D.FromXYZ( 0, 1, 0 );	// up = + y
		[XmlIgnore]
		public Vector3D		LocalRightAxis		= Vector3D.FromXYZ( -1, 0, 0 );	// right = - x
		*/

		//-----------------------------------------------------------------------------

		public World() {
		}

		//-----------------------------------------------------------------------------

		static public World  FromWorldcraftMap( WorldcraftMap wm ) {
			
			// create new world
			World world	= new World();

			world.Textures		= wm.Textures;
			world.SkyBox		= new SkyBox( "redSky" );
			world.FileName		= Path.Combine( ExoEngine.sWorldPath, Path.ChangeExtension( Path.GetFileName( wm.FileName ), ExoEngine.sWorldExtension ) );
			world.Dirty			= true;
		

			// find the starting location
			WorldcraftObject woStartPosition = wm.Objects.FindByClassName( "StartPosition" );
			if( woStartPosition == null ) {
				return null;
			}

			Vector3D ptStartPosition;
			FaceUtils.GetMidPoint( woStartPosition.Faces, out ptStartPosition );
			world.StartPosition = ptStartPosition;
			world.StartOrientation = woStartPosition.GetPropertyInteger( "orientation", 0 );

			// find the main light
			WorldcraftObject woLight = wm.Objects.FindByClassName( "Light" );
			if( woLight == null ) {
				return null;
			}

			Vector3D ptLight;
			FaceUtils.GetMidPoint( woLight.Faces, out ptLight );
			world.Light = ptLight;

			// find "Default" group... it is the static world
			WorldcraftObject woDefault = wm.Objects.FindByClassName( "Default" );
			if( woDefault == null ) {
				return null;
			}
		
			// setup BSP tree
			Faces faces = new Faces();
			
			// handle the other objects
			foreach( WorldcraftObject wo in wm.Objects ) {
				if( wo.ClassName == "Water" ) {
					Vector3D ptMin, ptMax;
					FaceUtils.GetExtents( wo.Faces, out ptMin, out ptMax );
					Water water = new Water( ptMin, ptMax, wo.GetPropertyFloat( "waveHeight", 100 ) );
					water.Color = wo.GetPropertyColor( "color", Color.Azure );
					world.Entities.Add( water );
				}
				else if( wo.ClassName == "Duck" ) {
					Vector3D ptMin, ptMax;
					FaceUtils.GetExtents( wo.Faces, out ptMin, out ptMax );
					Duck duck = new Duck( ptMin, ptMax );
					duck.LoadDataSet( Path.Combine( ExoEngine.sSpritePath, "duck.odf" ), wo.GetPropertyColor( "color", Color.Yellow ) );
					world.Entities.Add( duck );

					foreach( Face face in wo.Faces ) {
						face.Visible = false;
					}
					faces.AddRange( wo.Faces );
				}
			}		
		
			world.Entities.SortByPriority();

			faces.AddRange( woDefault.Faces );
			FaceUtils.OptimizeFaces( faces );
			world.BSPTreeRoot	= BSPTreeNode.FromFaces( faces );

			return world;
		}

		//-----------------------------------------------------------------------------

		public void Reset() {
			if( this.SkyBox != null ) {
				this.SkyBox.Reset( this );
			}

			this.BSPTreeRoot.Reset( this );

			foreach( Entity entity in this.Entities ) {
				entity.Reset( this );
			}

			this.Textures.LoadAll();

			this.Player.TranslationBody = this.StartPosition;
			this.Player.RotationBody = (float)Math2.ToRadians( this.StartOrientation );
			this.Player.Reset( this );
			
			this.Camera.SetCamera( this.Player.GetBodyFrameOfReference() );
		}

		public void Dispose() {
			GL.glErrorCheck();
			this.Textures.UnloadAll();
			GL.glErrorCheck();
		}

		//-----------------------------------------------------------------------------

		public int Render( RenderSettings settings ) {
			int polygonCount = 0;
			GL.glErrorCheck();
			if( ( settings.Background == true ) && 
				( this.SkyBox != null ) ) {
				polygonCount += this.SkyBox.Render( this, settings );
			}
			GL.glErrorCheck();

			Faces visibleFaces = BSPTreeVizOperator.GetVisibleFaces( this.Camera.Translation, this.BSPTreeRoot );
			ExoEngine.Viewer.Renderer.Render( this, visibleFaces, settings );
			polygonCount += visibleFaces.Count;
			GL.glErrorCheck();
		
			foreach( Entity entity in this.Entities ) {
				polygonCount += entity.Render( this, settings );
			}
			GL.glErrorCheck();

			return polygonCount;
		}
		
		//-----------------------------------------------------------------------------

		[XmlElement("light")]
		public	Vector3D	Light	= new Vector3D( 0, 0, -3750 );

		[XmlIgnore]
		public Textures		Textures = new Textures();


		[XmlElement("texture")]
		public	Texture[]	TexturesXMLDummyValue {
			get {	return	Textures.ToArray( true );	}
			set {	Textures.FromArray( value );	}
		}

		[XmlElement("startPosition")]
		public Vector3D	StartPosition = Vector3D.Zero;
		[XmlElement("startLocation")]
		public int	StartOrientation = 0;

		[XmlElement("skyBox")]
		public	SkyBox	SkyBox  = null;
		
		[XmlElement("bspTreeRoot")]
		public BSPTreeNode	BSPTreeRoot = null;

		[XmlArray("entities"),XmlArrayItem(typeof(Duck)),XmlArrayItem(typeof(FaceSet)),XmlArrayItem(typeof(Water))]
		public Entities	Entities = new Entities();

		[XmlElement("camera")]
		public	Camera	Camera	= new Camera();

		[XmlElement("player")]
		public	Player	Player	= new Player();

		//-----------------------------------------------------------------------------

		[XmlIgnore] // ignore filename
		public string FileName = null;

		[XmlIgnore] // ignore dirty flag
		public bool	Dirty = false;

		//-----------------------------------------------------------------------------

		public override string ToString() {
			return	"World[" + ( ( this.FileName != null ) ? this.FileName : "(null)" ) + "]";
		}
		
		//-----------------------------------------------------------------------------

	}

}
