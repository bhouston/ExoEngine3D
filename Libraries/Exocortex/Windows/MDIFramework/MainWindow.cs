using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Exocortex.Windows.MDIFramework {
	
	//===================================================================================
	// MainWindow
	//===================================================================================

	public class MainWindow : Form, IWindow {

		protected	string	_title	= null;

		public MainWindow( string title ) {
			_title = title;
			this.Text = _title;
		}

		protected MdiClient	_mdiClient = new MdiClient();
		public MdiClient	MdiClient	{
			get	{	return	_mdiClient;	}
		}

		//-------------------------------------------------------------------------------
		//	Implementation of IWindow
		//-------------------------------------------------------------------------------
		
		public	string			Caption			{ 
			get {	return	this.Text; }
		}
		public	ProjectItem		ProjectItem		{ 
			get {	return	null;	}
		}
		public	IDocument		Document		{ 
			get {	return	null;	}
		}
		public	Type			ObjectKind	{
			get {	return null;	}
		}
		public	object			Object		{ 
			get {	return null;	}
		}
		public	cvWindowType	WindowType		{ 
			get {	return	cvWindowType.MainWindow; }
		}
		public	object			Selection		{ 
			get {	return	null;	}
		}

		//-------------------------------------------------------------------------------

	}

}