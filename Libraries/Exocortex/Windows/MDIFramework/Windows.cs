using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Exocortex.Windows.MDIFramework {

	//===================================================================================
	// class Windows
	//===================================================================================

	public class Windows : IEnumerable {

		protected MdiApplication	_mdiApplication = null;

		protected ArrayList	_vWindows = new ArrayList();

		// create a new windows list
		internal Windows( MdiApplication mdiApplication ) {
			_mdiApplication = mdiApplication;
			_parent = mdiApplication;
		}
		
		// create a new window
		public	IWindow	CreateWindow( IDocument document ) {
			// TODO: create window class based on document type
			return	null;
		}

		// find windows by their index
		public	IWindow	this[ int index ] {
			get {	return	(IWindow) _vWindows[ index ]; }
		}

		// number of windows
		public	int			Count {
			get	{	return	_vWindows.Count;	}
		}

		// the parent of this list, in this case CV3D
		protected object	_parent	= null;
		public	object		Parent {
			get	{	return	_parent;	}
		}

		public IEnumerator	GetEnumerator() {
			return	_vWindows.GetEnumerator();
		}

	}

}