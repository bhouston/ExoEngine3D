using System;

namespace Exocortex.Windows.Forms
{
	public delegate void ProgressChangedEventHandler( float currentProgress );
	
	/// <summary>
	/// Summary description for ProgressTracker.
	/// </summary>
	public class ProgressTracker {

		protected float _progressOffset = 0;
		protected float _progressLength = 0;

		public ProgressTracker( float progressOffset, float progressLength ) {
			_progressOffset = progressOffset;
			_progressLength = progressLength;
		}

		public event ProgressChangedEventHandler ProgressChanged;

		public void UpdateProgress( float fraction ) {
			if( this.ProgressChanged != null ) {
				this.ProgressChanged( _progressOffset + _progressLength * fraction ); 
			}
		}
	}
}
