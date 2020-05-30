using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Exocortex.Windows.MDIFramework {

	//===================================================================================
	// interface IDocument
	//===================================================================================

	public interface IDocument {

		// saves the document
		void		Save();
		// saves the document at the given location
		void		SaveAs( string fileName );
		// closes the document
		void		Close( bool saveChanges );

		// creates a new window
		IWindow		NewWindow();
		// set the focus to a window of the document
		IWindow		Activate();

		// is the document read only?
		bool		ReadOnly	{ get; } 

		// the current selection in the document
		object		Selection	{ get; }

		// what type of document is this?
		Type		Kind		{ get;	}

		// does the document need to be saved
		bool		Saved		{ get; }

		// name
		string		Name		{ get; }
		// path
		string		Path		{ get; }
		// path + name
		string		FullName	{ get; }

	}

}