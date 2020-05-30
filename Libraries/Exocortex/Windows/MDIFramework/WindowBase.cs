using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Exocortex.Windows.MDIFramework {

	//===================================================================================
	// class WindowBase
	//===================================================================================

	public class WindowBase	{

		protected ContainerControl	_containerControl;

		// Moves the focus to the current item and makes it active.
		public	void	Activate() {
			_containerControl.Focus();
		}
		// Deletes the window and releases the document, if any, hosted in the window.
		public	void	Close( bool saveChanges ) {
			// is this the last window open to a document that needs to be saved?
			if( ( _document != null ) && ( _document.Saved == false ) ) {
				int count = 0;
				foreach( IWindow window in MdiApplication.GetObject().Windows ) {
					if( window.Document == _document ) {
						count ++;
					}
				}
				if( count == 1 ) {
					_document.Close( saveChanges );
				}
			}
		}

		// Returns a GUID String indicating the kind or type of the object.
		protected cvWindowType	_windowType = cvWindowType.Unknown;
		public	cvWindowType	WindowType	{
			get {	return	_windowType;	}
		}
		// Returns a string containing the title of the window.
		public	string			Caption		{ 
			get {
				if( _document != null ) {
					return	_document.Name;
				}
				return	"unknown";
			}
		}
		

		// Returns the ProjectItem Object associated with the given window
		protected ProjectItem	_projectItem = null;
		public	ProjectItem		ProjectItem	{ 
			get {	return	_projectItem; }
		}
		
		// Returns the Document Object associated with the window, if one exists.
		protected IDocument		_document = null;
		public	IDocument		Document	{ 
			get {	return	_document; }
		}

		// Returns the type of the Window.Object object, which is a GUID string representing the tool contained in the window.
		public	Type			ObjectKind	{
			get {
				if( _document != null ) {
					return	_document.GetType();
				}
				// TODO: WindowBase.ObjectKind - how to handle when window isn't a document container?
				return null;
			}
		}
		// Returns an interface or object that can be accessed at run time by name.
		public	object			Object		{ 
			get {
				if( _document != null ) {
					return	_document;
				}
				// TODO: WindowBase.Object - how to handle when window isn't a document container?
				return null;
			}
		}
		// Returns an object representing the current selection on the object if the window's owner has an automation object for the selection.
		public	object			Selection	{ 
			get {
				if( _document != null ) {
					return	_document.Selection;
				}
				// TODO: WindowBase.Selection - how to handle when window isn't a document container?
				return null;
			}

		}
	}

}