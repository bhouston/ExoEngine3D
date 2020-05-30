using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Exocortex.Windows.MDIFramework {
	
	//===================================================================================
	// class MDIApplication
	//===================================================================================

	public abstract class MdiApplication {

		static protected MdiApplication	s_mdiApplication = null;
		static public MdiApplication	GetObject() {
			return	s_mdiApplication;
		}

		public MdiApplication( string title ) {
			_mainWindow = new MainWindow( title );
			_documents = new Documents( this );
			_windows = new Windows( this );
		}

		// execute a command
		public void ExecuteCommand( string commandName ) {
		}

		// quit application
		public void	Quit() {
		}

		// maintains a list of all open documents
		protected	Documents	_documents = null;
		public Documents	Documents	{
			get	{	return	_documents;	}
		}

		// the current active document
		protected	IDocument	_activeDocument	= null;
		public IDocument	ActiveDocument	{
			get	{	return	_activeDocument;	}
		}

		// a list of all open windows
		protected	Windows	_windows = null;
		public Windows		Windows	{
			get	{	return	_windows;	}
		}

		// the current active window
		protected	IWindow	_activeWindow = null;
		public IWindow	ActiveWindow	{
			get	{	return	_activeWindow;	}
		}
		
		// the main project
		protected Project		_project = null;
		public Project	Project	{
			get	{	return	_project;	}
		}

		// the main window
		protected MainWindow	_mainWindow	= null;
		public MainWindow	MainWindow	{
			get	{	return	_mainWindow;	}
		}

	}
}
