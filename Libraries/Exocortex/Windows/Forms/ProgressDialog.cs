using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Exocortex.Windows.Forms
{
	/// <summary>
	/// Summary description for ProgressDialog.
	/// </summary>
	public class ProgressDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ProgressBar _progressBar;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProgressDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public float Progress {
			set {
				this._progressBar.Value = (int)( 1000 * value );
				this._progressBar.Update();
			}
		}

		public void SetProgress( float fraction ) {
			this.Progress = fraction;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// _progressBar
			// 
			this._progressBar.Location = new System.Drawing.Point(8, 24);
			this._progressBar.Maximum = 1000;
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(400, 16);
			this._progressBar.Step = 0;
			this._progressBar.TabIndex = 0;
			this._progressBar.Value = 50;
			// 
			// ProgressDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(416, 73);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this._progressBar});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgressDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Progress Dialog";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
