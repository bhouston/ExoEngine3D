using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Exocortex.Windows.MDIFramework {

	//===================================================================================
	// class ProjectItem
	//===================================================================================

	public class ProjectItem {

		// create a new project item
		internal ProjectItem( Project containingProject, string name, string fileName ) {
			Debug.Assert( _containingProject != null );
			_containingProject	= containingProject;
			_name				= name;
			_fileName			= fileName;
		}

		// get the document associated with this tree node 
		protected IDocument	_document = null;
		public	IDocument		Document {
			get {	return	_document;	}
		}

		// get the children of this tree node
		protected ProjectItems	_projectItems = new ProjectItems();
		public	ProjectItems	ProjectItems {
			get {	return	_projectItems;	}
		}

		// the containing project
		protected Project	_containingProject;
		public	Project	ContainingProject {
			get	{	return	_containingProject;	}
		}

		// the name of the item
		protected string	_name	= null;
		public	string	Name {
			get	{	return	_name;	}
		}

		// the name of the item
		protected string	_fileName	= null;
		public	string	FileName {
			get	{	return	_fileName;	}
		}

		// open the assoc. document
		public	IWindow	Open() {
			if( _document == null ) {
				//_document = MdiApplication.GetObject().ItemOperators.OpenFile( _fileName );
			}
			return	_document.Activate();
		}

		// save the assoc. document
		public	void	Save() {
			Debug.Assert( _document != null );
			_document.Save();
		}
		
		// save the assoc. document with a fileName
		public	void	SaveAs( string fileName ) {
			Debug.Assert( fileName != null );
			Debug.Assert( _document != null );
			_document.SaveAs( fileName );
		}

		// remove this node from the project
		public	void	Remove() {
			// TODO: remove project item from project.
			_containingProject = null;
		}

		// does the assoc. document need saving?
		public	bool	Saved {
			get	{	
				Debug.Assert( _document != null );
				return	_document.Saved;
			}
		}

		// is the project item open
		public	bool	IsOpen {
			get	{	return	_document != null;	}
		}

	}

}