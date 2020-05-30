using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Exocortex.Windows.MDIFramework {

	internal class DocumentBase : IDocument {

		//-------------------------------------------------------------------------------
		//	Implementation of IDocument
		//-------------------------------------------------------------------------------
		
		// saves the document
		public	void		Save() {
		}
		
		// saves the document at the given location
		public	void		SaveAs( string fileName ) {
		}
		
		// closes the document
		public	void		Close( bool saveChanges ) {
		}

		// creates a new window
		public	IWindow		NewWindow() {
			return	MdiApplication.GetObject().Windows.CreateWindow( this );
		}

		// set the focus to a window of the document
		public	IWindow		Activate() {
			IWindow documentWindow = null;
			foreach( IWindow window in MdiApplication.GetObject().Windows ) {
				if( window.Document == this ) {
					documentWindow = window;
					break;
				}
			}
			Debug.Assert( documentWindow != null );
			documentWindow.Activate();
			return documentWindow;
		}

		// is the document read only?
		protected bool	_readOnly = false;
		public	bool		ReadOnly	{ 
			get {	return	_readOnly; }
		}

		// the current selection in the document
		protected object	_selection = null;
		public	object		Selection	{ 
			get {	return	_selection; }
		}

		// what type of document is this?
		public	Type		Kind	{ 
			get {	return	this.GetType();	}
		}

		// has the document been saved since it was last changed?
		protected bool	_saved	= true;
		public	bool	Saved {
			get	{	return	_saved;	}
		}

		// the name of the document
		protected string _name = null;
		public	string	Name {
			get	{	return	_name;	}
		}

		// the path of the document
		protected string _path = null;
		public	string	Path {
			get	{	return	_path;	}
		}

		// the full name of the document
		public	string	FullName {
			get {	
				if( _name == null ) {
					return null;
				}
				return	System.IO.Path.Combine( _path, _name );
			}
		}

		//-------------------------------------------------------------------------------

	}

}
