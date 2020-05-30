using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using Exocortex;

namespace Exocortex.Windows.Forms
{
	/// <summary>
	/// Summary description for Task.
	/// </summary>
	public abstract class LongProcess {

		//--------------------------------------------------------------------------

		protected LongProcess( string title, int totalTasks ) {
			//Debug2.TraceFunction();
			Debug.Assert( title != null );
			Debug.Assert( totalTasks > 0 );

			_totalTasks = totalTasks;

			_dialog = new LongProcessDialog();
			_dialog.Text = title;
		}

		//--------------------------------------------------------------------------

		public bool	Execute( Form form ) {
			//Debug2.Push();
			_threadProcess = new Thread( new ThreadStart( this.ExecuteWrapper ) );
			_threadProcess.Start();
			_dialog.ShowDialog( form );
			//Debug2.Pop();
			return	_processResult;
		}

		//--------------------------------------------------------------------------

		public abstract bool DoProcess();

		//--------------------------------------------------------------------------
		
		protected	void	BeginTask( string task ) {
			//Debug2.Push();
			_taskIndex ++;
			_dialog.AddItemToTaskList( task );
			UpdateDialog();
			//Debug2.Pop();
		}

		protected	void	Finished( bool success, string message ) {
			//Debug2.Push();
			_taskIndex ++;
			UpdateDialog();
			_dialog.Finished( message );
			//Debug2.Pop();
		}
		protected	void	Finished( bool success ) {
			//Debug2.Push();
			//Debug2.Pop();
			this.Finished( success, null );
		}

		//--------------------------------------------------------------------------


		private void ExecuteWrapper() {
			//Debug2.Push();
			_processResult = this.DoProcess();
			//Debug2.Pop();
		}

		private void	UpdateDialog() {
			//Debug2.Push();
			_dialog.SetSummaryText( _taskIndex + " of " + _totalTasks + " are completed." );
			_dialog.SetProgress( _taskIndex * 100 / _totalTasks );
			//Debug2.Pop();
		}

		//--------------------------------------------------------------------------

		private LongProcessDialog	_dialog	= new LongProcessDialog();
		
		private int		_totalTasks = 0;
		private int		_taskIndex = -1;
		private bool	_processResult = false;

		private Thread	_threadProcess	= null;

		//--------------------------------------------------------------------------
	}
}
