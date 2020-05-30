using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Exocortex.Windows.MDIFramework {

	//===================================================================================
	// enum cvWindowType
	//===================================================================================

	public enum cvWindowType {
		Voxel,
		Isocontour,
		Project,
		Properties,
		Unknown,
		MainWindow,
	}

	//===================================================================================
	// interface IWindows
	//===================================================================================

	public interface IWindow	{

		// Moves the focus to the current item and makes it active.
		void			Activate();
		// Deletes the window and releases the document, if any, hosted in the window.
		void			Close();

		// Returns a GUID String indicating the kind or type of the object.
		cvWindowType	WindowType	{ get; }
		// Returns a string containing the title of the window.
		string			Caption		{ get; }
		

		// Returns the ProjectItem Object associated with the given window
		ProjectItem		ProjectItem	{ get; }
		// Returns the Document Object associated with the window, if one exists.
		IDocument		Document	{ get; }
		// Returns the type of the Window.Object object, which is a GUID string representing the tool contained in the window.
		Type			ObjectKind	{ get; }
		// Returns an interface or object that can be accessed at run time by name.
		object			Object		{ get; }
		// Returns an object representing the current selection on the object if the window's owner has an automation object for the selection.
		object			Selection	{ get; }

	}

}