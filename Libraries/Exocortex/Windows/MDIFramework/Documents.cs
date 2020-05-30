using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Exocortex.Windows.MDIFramework {

	//===================================================================================
	// class Documents
	//===================================================================================

	public class Documents : IEnumerable {

		protected MdiApplication	_mdiApplication = null;

		protected ArrayList	_vDocuments	= new ArrayList();

		// create a new document list
		internal Documents( MdiApplication mdiApplication ) {
			_mdiApplication = mdiApplication;
		}

		// save all documents
		public	void		SaveAll() {
			foreach( IDocument document in _vDocuments ) {
				if( document.Saved == false ) {
					document.Save();
				}
			}
		}

		// close all documents
		public	void		CloseAll( bool saveChanges ) {
			foreach( IDocument document in _vDocuments ) {
				document.Close( saveChanges );
			}
			_vDocuments.Clear();
		}

		// document indexer
		public	IDocument	this[ int index ]	{
			get	{	return	(IDocument)	_vDocuments[index];	}
		}

		// number of open documents
		public	int			Count				{
			get	{	return	_vDocuments.Count;	 }
		}

		public IEnumerator	GetEnumerator() {
			return	_vDocuments.GetEnumerator();
		}

	}

}
