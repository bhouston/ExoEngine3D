using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Exocortex.Windows.Forms {
	/// <summary>
	/// Summary description for ImportProgressDialog.
	/// </summary>
	public class LongProcessDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ProgressBar _progressBar;
		private System.Windows.Forms.Button _buttonMain;
		private System.Windows.Forms.Label _labelSummary;
		private System.Windows.Forms.ListView _listTasks;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LongProcessDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this._progressBar.Minimum	= 0;
			this._progressBar.Maximum	= 100;
			this._progressBar.Value		= 0;

			this._buttonMain.Enabled = false;
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

		public void Finished( string errorMessage ) {
			//Debug2.Push();
			if( errorMessage != null ) {
				if( _listTasks.Items.Count > 0 ) {
					ListViewItem item = _listTasks.Items[ _listTasks.Items.Count - 1 ];
					if( item.SubItems[1].Text != "Failed" ) {
						item.SubItems[1].Text = "Failed";
					}
				}
				this.SetSummaryText( "ERROR: " + errorMessage );
			}
			else {
				if( _listTasks.Items.Count > 0 ) {
					ListViewItem item = _listTasks.Items[ _listTasks.Items.Count - 1 ];
					if( item.SubItems[1].Text != "Completed" ) {
						item.SubItems[1].Text = "Completed";
					}
				}
			}
			this._buttonMain.Enabled = true;
			this._buttonMain.Update();
			//Debug2.Pop();
		}	 

		public void SetSummaryText( string summaryText ) {
			//Debug2.TraceFunction();
			this._labelSummary.Text = summaryText;
		}

		public void AddItemToTaskList( string taskName ) {
			//Debug2.TraceFunction();
			foreach( ListViewItem item in this._listTasks.Items ) {
				if( item.SubItems[1].Text != "Completed" ) {
					item.SubItems[1].Text = "Completed";
				}
			}
			this._listTasks.Items.Add( new ListViewItem( new string[] { taskName, "In Progress" } ) );
			this._listTasks.Update();
		}
		public void SetProgress( int percentComplete ) {
			//Debug2.TraceFunction();
			this._progressBar.Value = percentComplete;
			this._progressBar.Update();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._buttonMain = new System.Windows.Forms.Button();
			this._labelSummary = new System.Windows.Forms.Label();
			this._listTasks = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// _progressBar
			// 
			this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this._progressBar.Location = new System.Drawing.Point(8, 40);
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(472, 16);
			this._progressBar.Step = 1;
			this._progressBar.TabIndex = 1;
			// 
			// _buttonMain
			// 
			this._buttonMain.Location = new System.Drawing.Point(408, 184);
			this._buttonMain.Name = "_buttonMain";
			this._buttonMain.TabIndex = 2;
			this._buttonMain.Text = "OK";
			this._buttonMain.Click += new System.EventHandler(this._buttonMain_Click);
			// 
			// _labelSummary
			// 
			this._labelSummary.Location = new System.Drawing.Point(8, 0);
			this._labelSummary.Name = "_labelSummary";
			this._labelSummary.Size = new System.Drawing.Size(472, 32);
			this._labelSummary.TabIndex = 4;
			this._labelSummary.Text = "0 of N tasks completed.";
			this._labelSummary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _listTasks
			// 
			this._listTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.columnHeader1,
																						 this.columnHeader2});
			this._listTasks.FullRowSelect = true;
			this._listTasks.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._listTasks.LabelWrap = false;
			this._listTasks.Location = new System.Drawing.Point(8, 64);
			this._listTasks.MultiSelect = false;
			this._listTasks.Name = "_listTasks";
			this._listTasks.Size = new System.Drawing.Size(472, 110);
			this._listTasks.TabIndex = 5;
			this._listTasks.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Task";
			this.columnHeader1.Width = 300;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Status";
			this.columnHeader2.Width = 100;
			// 
			// LongProcessDialog
			// 
			this.AcceptButton = this._buttonMain;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(490, 215);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this._listTasks,
																		  this._labelSummary,
																		  this._buttonMain,
																		  this._progressBar});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "LongProcessDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Task Tracking Dialog";
			this.ResumeLayout(false);

		}
		#endregion

		private void _buttonMain_Click(object sender, System.EventArgs e) {
			this.Hide();
		}
	}
}
