using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Exocortex.Windows.MDIFramework {

	//===================================================================================
	// Project
	//===================================================================================

	public class Project {

		// create a new project
		public	void	Create( string path, string name ) {
			Debug.Assert( this.IsOpen == false );

			_projectItems = new ProjectItems();

			_saved = false;
			_isOpen = true;
		}

		// open a project
		public	void	Open( string fileName ) {
			Debug.Assert( this.IsOpen == false );

			_name = null;
			_path = null;
			_projectItems = new ProjectItems();

			_isOpen = true;
			_saved	= true;
		}
		
		// save the current project
		public	void	SaveAs( string fileName ) {
			Debug.Assert( this.IsOpen == true );

			_saved = true;
		}
		
		// close the current project
		public	void	Close( bool saveChanges ) {
			Debug.Assert( this.IsOpen == true );

			if( saveChanges == true && this.Saved == false ) {
				this.SaveAs( this.FullName );
			}

			_projectItems = null;
			_name = null;
			_path = null;
			_saved	= false;
			_isOpen	= false;
		}

		// is a project currently open?
		protected bool	_isOpen	= false;
		public	bool	IsOpen {
			get	{	return	_isOpen;	}
		}

		// the bottom level project items contained in this project
		protected	ProjectItems	_projectItems = null;
		public	ProjectItems	ProjectItems {
			get	{	return	_projectItems;	}
		}

		// has the project been saved since it was last changed?
		protected bool	_saved	= false;
		public	bool	Saved {
			get	{	return	_saved;	}
		}

		// what is the name of the project
		protected string _name = null;
		public	string	Name {
			get	{	return	_name;	}
		}

		// what is the path of the project
		protected string _path = null;
		public	string	Path {
			get	{	return	_path;	}
		}

		// what is the full name of the project file
		public	string	FullName {
			get {	
				if( _name == null ) {
					return null;
				}
				return	System.IO.Path.ChangeExtension( System.IO.Path.Combine( _path, _name ), ".exn" );
			}
		}

	}

}
