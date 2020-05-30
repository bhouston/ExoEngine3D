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
using Exocortex.Geometry3D;
using Exocortex.Windows.Forms;
using Exocortex.OpenGL;

using ExoEngine.Worldcraft;
using ExoEngine.Geometry;
using ExoEngine.Rendering;


namespace ExoEngine.Commands {
	
	//----------------------------------------------------------------------------------

	public class RenderOptionWaterAdvance : CommandButton {
		public RenderOptionWaterAdvance() {
			this.Text = "WaterAdvance";
		}
		protected override bool OnExecute() {
			Water.IsAdvance = ! this.Checked;
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = Water.IsAdvance;
		}
	}

	public class RenderOptionWaterUpdateVertices : CommandButton {
		public RenderOptionWaterUpdateVertices() {
			this.Text = "WaterUpdateVerticies";
		}
		protected override bool OnExecute() {
			Water.IsUpdateVertices = ! this.Checked;
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = Water.IsUpdateVertices;
		}
	}

	public class RenderOptionWaterUpdateNormals: CommandButton {
		public RenderOptionWaterUpdateNormals() {
			this.Text = "WaterUpdateNormals";
		}
		protected override bool OnExecute() {
			Water.IsUpdateNormals = ! this.Checked;
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = Water.IsUpdateNormals;
		}
	}

	public class RenderOptionWaterRender : CommandButton {
		public RenderOptionWaterRender() {
			this.Text = "WaterRender";
		}
		protected override bool OnExecute() {
			Water.IsRender = ! this.Checked;
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = Water.IsRender;
		}
	}

	public class RenderOptionDuckRender : CommandButton {
		public RenderOptionDuckRender() {
			this.Text = "DuckRender";
		}
		protected override bool OnExecute() {
			Duck.IsRender = ! this.Checked;
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = Duck.IsRender;
		}
	}

	//----------------------------------------------------------------------------------
	
	public class RenderOptionBackground : CommandButton {
		public RenderOptionBackground() {
			this.Text = "Background";
		}
		protected override bool OnExecute() {
			ExoEngine.Viewer.RenderSettings.Background = ! this.Checked;
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = ExoEngine.Viewer.RenderSettings.Background;
		}
	}

	public class RenderOptionWireframe : CommandButton {
		public RenderOptionWireframe() {
			this.Text = "Wireframe";
		}
		protected override bool OnExecute() {
			ExoEngine.Viewer.RenderSettings.Wireframe = ! this.Checked;
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = ExoEngine.Viewer.RenderSettings.Wireframe;
		}
	}

	public class RenderOptionZBuffer : CommandButton {
		public RenderOptionZBuffer() {
			this.Text = "Z-Buffer";
		}
		protected override bool OnExecute() {
			ExoEngine.Viewer.RenderSettings.ZBuffer = ! this.Checked;
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = ExoEngine.Viewer.RenderSettings.ZBuffer;
		}
	}

	public class RenderOptionTextures : CommandButton {
		public RenderOptionTextures() {
			this.Text = "Textures";
		}
		protected override bool OnExecute() {
			ExoEngine.Viewer.RenderSettings.Textures = ! this.Checked;
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = ExoEngine.Viewer.RenderSettings.Textures;
		}
	}

	public class RenderOptionFaceColors : CommandButton {
		public RenderOptionFaceColors() {
			this.Text = "Face Colors";
		}
		protected override bool OnExecute() {
			ExoEngine.Viewer.RenderSettings.FaceColors = ! this.Checked;
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = ExoEngine.Viewer.RenderSettings.FaceColors;
		}
	}

	//----------------------------------------------------------------------------------

	public class TextureOptionQuality : CommandButton {
		protected bool				_bMipmap;
		protected GL.TextureFilter	_minFilter;
		protected GL.TextureFilter	_maxFilter;

		public TextureOptionQuality( string text, bool bMipmap, GL.TextureFilter minFilter, GL.TextureFilter maxFilter ) {
			this.Text = text;
			_bMipmap	= bMipmap;
			_minFilter	= minFilter;
			_maxFilter	= maxFilter;
		}
		protected override bool OnExecute() {
			ExoEngine.ActiveWorld.Textures.OverideTextureQuality( _bMipmap, _minFilter, _maxFilter );
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			World world = ExoEngine.ActiveWorld;
			if( world != null ) {
				this.Enabled = true;
				this.Checked =
					( world.Textures.DefaultMipmap == _bMipmap ) &&
					( world.Textures.DefaultMinFilter == _minFilter ) &&
					( world.Textures.DefaultMaxFilter == _maxFilter );
			}
			else {
				this.Enabled = false;
				this.Checked = false;
			}
		}
	}

	public class TextureOptionHighQuality : TextureOptionQuality {
		public TextureOptionHighQuality() : base( "High Quality", true, GL.TextureFilter.LinearMipmapLinear, GL.TextureFilter.Linear ) {}
	}

	public class TextureOptionMediumQuality : TextureOptionQuality {
		public TextureOptionMediumQuality() : base( "Medium Quality", false, GL.TextureFilter.Linear, GL.TextureFilter.Linear ) {}
	}
		
	public class TextureOptionLowQuality : TextureOptionQuality {
		public TextureOptionLowQuality() : base( "Low Quality", false, GL.TextureFilter.Nearest, GL.TextureFilter.Nearest ) {}
	}
		
	//----------------------------------------------------------------------------------

	public class ViewerOptionCrossHairs : CommandButton {
		public ViewerOptionCrossHairs() {
			this.Text = "Cross Hairs";
		}
		protected override bool OnExecute() {
			ExoEngine.MainForm.Viewer.IsCrossHairs = ! this.Checked;
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = ExoEngine.MainForm.Viewer.IsCrossHairs;
		}
	}

	public class ViewerOptionXYGrid : CommandButton {
		public ViewerOptionXYGrid() {
			this.Text = "X-Y Grid";
		}
		protected override bool OnExecute() {
			ExoEngine.MainForm.Viewer.IsGrid = ! this.Checked;
			ExoEngine.UpdateAll();
			return true;
		}
		protected override void OnUpdate() {
			this.Checked = ExoEngine.MainForm.Viewer.IsGrid;
		}
	}

	//----------------------------------------------------------------------------------

}