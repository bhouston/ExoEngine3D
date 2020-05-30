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
using System.Drawing;

namespace ExoEngine {

	public class StatusBar : System.Windows.Forms.StatusBar {

		//--------------------------------------------------------------------
		
		public StatusBar() {
			this.SuspendLayout();

			_sbpanelA.BeginInit();
			_sbpanelA.MinWidth = 120;
			this.Panels.Add( _sbpanelA );
			_sbpanelA.EndInit();

			_sbpanelB.BeginInit();
			_sbpanelB.MinWidth = 120;
			this.Panels.Add( _sbpanelB );
			_sbpanelB.EndInit();

			_sbpanelC.BeginInit();
			_sbpanelC.MinWidth = 120;
			this.Panels.Add( _sbpanelC );
			_sbpanelC.EndInit();

			this.ShowPanels = true;

			this.ResumeLayout();
		}

		//--------------------------------------------------------------------

		protected StatusBarPanel	_sbpanelA = new StatusBarPanel();
		public string	PanelTextA {
			get {	return	_sbpanelA.Text; }
			set {	_sbpanelA.Text = value;	}
		}

		protected StatusBarPanel	_sbpanelB = new StatusBarPanel();
		public string	PanelTextB {
			get {	return	_sbpanelB.Text; }
			set {	_sbpanelB.Text = value;	}
		}

		protected StatusBarPanel	_sbpanelC = new StatusBarPanel();
		public string	PanelTextC {
			get {	return	_sbpanelC.Text; }
			set {	_sbpanelC.Text = value;	}
		}

		//--------------------------------------------------------------------

	}

}
