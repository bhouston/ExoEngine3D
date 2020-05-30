using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Exocortex.Windows.MDIFramework {

	//===================================================================================
	// ProjectItems
	//===================================================================================

	public class ProjectItems {

		internal ArrayList	_vProjectItems	= new ArrayList();

		// create a file project item
		public	ProjectItem	AddFromFile( string fileName ) {
			Debug.Assert( File.Exists( fileName ) == true );
			ProjectItem projitem = new ProjectItem( null, Path.GetFileName( fileName ), fileName );
			this._vProjectItems.Add( projitem );
			return	projitem;
		}

		// create a folder project item
		public	ProjectItem	AddFromFolder( string name ) {
			// does this folder already exist?
			ProjectItem projitem = this[ name ];
			if( projitem != null ) {
				// TODO: new AddProjectItemException( "A folder or file with the name '" + name + "' already exists.  Please choose a alternative name." );
			}

			projitem = new ProjectItem( null, name, name );
			/*projitem.Name		= name;
			projitem.Kind		= cvProjectItemKind.Folder;
			projitem.Document	= null;
			projitem.Saved		= true;
			projitem.Parent		= this.Parent;*/
			this._vProjectItems.Add( projitem );

			return	projitem;
		}

		// find items by their index
		public	ProjectItem	this[ int index ] {
			get {	return	(ProjectItem) _vProjectItems[ index ]; }
		}

		// find items by their names
		public	ProjectItem	this[ string name ] {
			get {	
				foreach( ProjectItem projitem in _vProjectItems ) {
					if( String.Compare( projitem.Name, name, false ) == 0 ) {
						return projitem;
					}
				}
				return null;
			}
		}

		public	int			Count {
			get	{	return	_vProjectItems.Count;	}
		}

		// the parent of this list, either a project item or a project.
		protected object	_parent	= null;
		public	object		Parent {
			get	{	return	_parent;	}
		}
	}

}
