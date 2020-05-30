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
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;

using Exocortex;
using Exocortex.Windows.Forms;
using Exocortex.Geometry3D;

using ExoEngine.Worldcraft;
using ExoEngine.Geometry;
using ExoEngine.Rendering;


namespace ExoEngine.Commands {
	
	//----------------------------------------------------------------------------------
	
	public class RendererCommand : CommandButton {
		protected Type	_typeRenderer = null;
		public RendererCommand( Type typeRenderer, string text ) {
			this.Text = text;
			this._typeRenderer = typeRenderer;
		}
		protected override bool OnExecute() {
			this.Checked = true;
			ExoEngine.Viewer.Renderer = ExoEngine.Viewer.AvailableRenderers.FindByType( this._typeRenderer );
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = ( ExoEngine.Viewer.Renderer.GetType() == this._typeRenderer );
		}
	}

	public class RendererBasic : RendererCommand {
		public RendererBasic() : base( typeof( BasicRenderer ), "No shading" ) {
		}
	}
	public class RendererFlat : RendererCommand {
		public RendererFlat() : base( typeof( FlatRenderer ), "Flat Shading" ) {
		}
	}
	public class RendererGouraud : RendererCommand {
		public RendererGouraud() : base( typeof( GouraudRenderer ), "Gouraud Shading" ) {
		}
	}
	public class RendererPhong : RendererCommand {
		public RendererPhong() : base( typeof( PhongRenderer ), "Phong Shading" ) {
		}
	}
//	public class RendererCartoon : RendererCommand {
//		public RendererCartoon() : base( typeof( CartoonRenderer ), "Cartoon" ) {
//		}
//	}
	public class RendererPencil : RendererCommand {
		public RendererPencil() : base( typeof( PencilRenderer ), "Pencil Drawing" ) {
		}
	}
	public class RendererReflection : RendererCommand {
		public RendererReflection() : base( typeof( ReflectionRenderer ), "Metallic Shading" ) {
		}
	}


	//----------------------------------------------------------------------------------

}