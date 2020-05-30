using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Exocortex;
using Exocortex.Diagnostics;

namespace Exocortex.Windows.Forms {

	public abstract class CommandButton : CommandControl {

		public CommandButton() {
		}

		protected Icon _icon = null;
		public Icon Icon {
			get {	return	_icon;	}
			set {	_icon = value;	}
		}

		protected bool _checked = false;
		public bool Checked {
			get {	return	_checked;	}
			set {	_checked = value;	}
		}

		protected object[] _params = null;
		public object[] Params {
			get {	return	_params;	}
			set {	_params = value;	}
		}

		//----------------------------------------------------------------------------

		public bool Execute() {
			//Debug.WriteLine( "Command: " + this.GetType().Name );

			BugTracking.SetWatch( "CommandButton.LastExecute", this );

			this.CommandControls.ExecutionCount ++;
			bool bResult = this.OnExecute();
			this.CommandControls.ExecutionCount --;
			
			// reset params, if any
			if( this.Params != null ) {
				this.Params = null;
			}

			return bResult;
		}
		protected void OnExecuteEvent( object o, EventArgs e ) {
			this.Execute();
		}
		protected virtual bool OnExecute() {
			return true;
		}

		//----------------------------------------------------------------------------

		public override void Update() {
			this.OnUpdate();
			//_menuItem.Click += new EventHandler( this.OnExecuteEvent );
			/*_menuItem.Enabled	= this.Enabled;
			_menuItem.Text		= this.Text;
			_menuItem.Checked	= this.Checked;
			_menuItem.Shortcut	= this.Shortcut;*/
			this.OnChanged();
		}
		protected virtual void OnUpdate() {
		}

		//----------------------------------------------------------------------------

	}


}
