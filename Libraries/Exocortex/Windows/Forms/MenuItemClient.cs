using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Exocortex.Windows.Forms {

	public class MenuItemClient  : MenuItem {

		public MenuItemClient( CommandButton commandButton ) {
			_commandButton = commandButton;
			_commandButton.Changed += new CommandControlChangedEventHandler( this.ClientUpdate );
		}

		protected override void Dispose( bool disposing ) {
			_commandButton.Changed -= new CommandControlChangedEventHandler( this.ClientUpdate );
			base.Dispose( disposing );
		}

		public void ClientUpdate( CommandControl commandControl ) {
			Debug.Assert( commandControl == _commandButton );
			this.Text		= _commandButton.Text;
			this.Enabled	= _commandButton.Enabled;
			this.Checked	= _commandButton.Checked;
			this.Shortcut	= _commandButton.Shortcut;
			//if( _commandButton.Icon != null ) {
			//	this.Icon		= _commandButton.Icon.ToBitmap();
			//}
		}

		protected override void OnClick( EventArgs e ) {
			_commandButton.Execute();
		}

		protected CommandButton	_commandButton = null;
		public CommandButton	CommandButton {
			get {	return	_commandButton;	}
		}

	}

}
