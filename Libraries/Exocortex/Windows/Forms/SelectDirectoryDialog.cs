using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Exocortex.Windows.Forms
{
	/// <summary>
	/// Summary description for SelectDirectoryDialog.
	/// </summary>
	public class SelectDirectoryDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button _buttonSelect;
		private Exocortex.Windows.Forms.DirectoryTreeView _directoryTreeView;
		private System.Windows.Forms.TextBox _editDirectory;
		private System.Windows.Forms.Button _buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SelectDirectoryDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//this._directoryTreeView.Nodes.Clear();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public string Directory {
			get {	return	_directoryTreeView.Path;	}
			set {	_directoryTreeView.Path = value;	}
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
			this._buttonSelect = new System.Windows.Forms.Button();
			this._directoryTreeView = new Exocortex.Windows.Forms.DirectoryTreeView();
			this._editDirectory = new System.Windows.Forms.TextBox();
			this._buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _buttonSelect
			// 
			this._buttonSelect.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this._buttonSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._buttonSelect.Location = new System.Drawing.Point(152, 296);
			this._buttonSelect.Name = "_buttonSelect";
			this._buttonSelect.TabIndex = 0;
			this._buttonSelect.Text = "Select";
			// 
			// _directoryTreeView
			// 
			this._directoryTreeView.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this._directoryTreeView.Location = new System.Drawing.Point(8, 8);
			this._directoryTreeView.Name = "_directoryTreeView";

			this._directoryTreeView.Size = new System.Drawing.Size(296, 248);
			this._directoryTreeView.TabIndex = 2;
			this._directoryTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvShellTree_AfterSelect);
			// 
			// _editDirectory
			// 
			this._editDirectory.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this._editDirectory.Location = new System.Drawing.Point(8, 264);
			this._editDirectory.Name = "_editDirectory";
			this._editDirectory.Size = new System.Drawing.Size(296, 20);
			this._editDirectory.TabIndex = 2;
			this._editDirectory.Text = "";
			this._editDirectory.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPath_KeyPress);
			// 
			// _buttonCancel
			// 
			this._buttonCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this._buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._buttonCancel.Location = new System.Drawing.Point(232, 296);
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.TabIndex = 3;
			this._buttonCancel.Text = "Cancel";
			// 
			// SelectDirectoryDialog
			// 
			this.AcceptButton = this._buttonSelect;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this._buttonCancel;
			this.ClientSize = new System.Drawing.Size(312, 326);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this._buttonCancel,
																		  this._directoryTreeView,
																		  this._buttonSelect,
																		  this._editDirectory});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectDirectoryDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Directory";
			this.ResumeLayout(false);

		}
		#endregion

		protected void txtPath_KeyPress (object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if (e.KeyChar == 13) {
				this._directoryTreeView.Path = this._editDirectory.Text;
				this._editDirectory.Text = this._directoryTreeView.Path;
				this._directoryTreeView.Focus();
			}
		}

		//protected void btnGo_Click (object sender, System.EventArgs e) {
			//this._directoryTreeView.Path = txtPath.Text;
			//this._editDirectory.Text = trvShellTree.Path;
			//this._directoryTreeView.Focus();
		//}

		protected void trvShellTree_AfterSelect (object sender, System.Windows.Forms.TreeViewEventArgs e) {
			this._editDirectory.Text = this._directoryTreeView.Path;
		}
	}
}
