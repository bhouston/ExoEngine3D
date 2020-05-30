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

namespace ExoEngine.Geometry {

	/// <summary>
	/// Summary description for FaceSetEntity.
	/// </summary>
	//[XmlName("faceSet")]
	public class FaceSet : Entity {

		//------------------------------------------------------------------------

		public FaceSet() {
		}

		//------------------------------------------------------------------------

		public override void Reset( World world ) {
			foreach( Face face in this.Faces ) {
				face.Reset( world, this );
			}
		}
		public override int Render( World world, RenderSettings settings ) {
			Faces facesVisible = new Faces();
			Vector3D	cameraLocation = world.Camera.Translation;
			foreach( Face face in this.Faces ) {
				if( face.Plane.GetSign( cameraLocation ) >= 0 ) {
					facesVisible.Add( face );
				}
			}
			this.Renderer.Render( world, facesVisible, settings );

			return	facesVisible.Count;
		}

		//------------------------------------------------------------------------

		protected Renderer	_renderer = ExoEngine.Viewer.AvailableRenderers.FindByType( typeof( ReflectionRenderer ) );
		[XmlIgnore]
		public	Renderer	Renderer	{
			get	{	return	_renderer;	}
			set	{	_renderer = value;	}
		}

		//------------------------------------------------------------------------

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

	}
}
