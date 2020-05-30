// DirectoryTreeView.cs
// Copyright (C) 2000 HyunKeun Cho
//
// http://www.digifilters.net/hkcho/csharp.asp

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices; 

using Exocortex.Diagnostics;

namespace Exocortex.Windows.Forms {

	public class DirectoryTreeView : TreeView, IComparer {

		public enum DriveType { 
			Unknown = 0, 
			NoRootDir = 1, 
			Removable = 2, 
			Fixed = 3, 
			Remote = 4, 
			CdRom = 5, 
			RamDisk = 6 
		}
	    
		[DllImport("kernel32.dll")] 
		public static extern DriveType GetDriveType(string rootPathName); 
    	
		int IComparer.Compare(object x, object y) { return ((DirectoryInfo)x).Name.CompareTo(((DirectoryInfo)y).Name); }
		
		
		private void InitializeComponent () {
			// Initialize ImageList
			ImageList imglist = new ImageList();
			imglist.Images.Add( ExocortexResources.GetBitmap( "folderClosed.bmp" ) );
			imglist.Images.Add( ExocortexResources.GetBitmap( "folderOpen.bmp" ) );
			imglist.Images.Add( ExocortexResources.GetBitmap( "driveFloppy.bmp" ) );
			imglist.Images.Add( ExocortexResources.GetBitmap( "driveStandard.bmp" ) );
			imglist.Images.Add( ExocortexResources.GetBitmap( "driveCdrom.bmp" ) );
			imglist.Images.Add( ExocortexResources.GetBitmap( "driveNetwork.bmp" ) );

			this.ImageList = imglist;                          
			// Add event handler
			this.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler (this.OnBeforeSelect);
			this.AfterSelect += new System.Windows.Forms.TreeViewEventHandler (this.OnAfterSelect);
			this.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler (this.OnBeforeExpand);
			this.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler (this.OnBeforeCollapse);
		}

		protected void OnBeforeSelect (object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
			// Set icon as closed
			if (this.SelectedNode != null && this.SelectedNode.Parent != null) {
				this.SelectedNode.ImageIndex = this.SelectedNode.SelectedImageIndex = 0;
			}
		}

		protected void OnAfterSelect (object sender, System.Windows.Forms.TreeViewEventArgs e) {
			// Set icon as opened
			if (e.Node.Parent != null) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
			}
		}

		protected void OnBeforeExpand (object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
			// Set cursor as wait
			Cursor.Current = Cursors.WaitCursor;

			// Populate sub directory in depth 2
			try {
				PopulateSubDirectory(e.Node, 2);
				Cursor.Current = Cursors.Default;

				// Set icon as opened
				if (e.Node.Parent != null) {
					//e.node.ImageIndex = e.node.SelectedImageIndex = 1;
				}
			}
			catch (Exception excpt) {
				// Show error message and cancel expand
				BugTracking.WriteExceptionRecord( excpt );
				MessageBox.Show( excpt.Message, "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				e.Cancel = true;
			}

			// Set cursor as default
			Cursor.Current = Cursors.Default;
		}

		protected void OnBeforeCollapse (object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
			// Set icon as closed
			if (e.Node.Parent != null) {
				//e.node.ImageIndex = e.node.SelectedImageIndex = 0;
			}
		}

		private void PopulateSubDirectory(TreeNode curNode, int depth) {
			//Debug.WriteLine( "scanning directory '" + curNode.FullPath + "'" );
			if (--depth < 0) {
				return;
			}

			if (curNode.Nodes.Count==1 && curNode.Nodes[0].Text=="") {
				try {
					// Get sub directory list
					//Directory[] DirectoryList;
					string[] DirectoryList = Directory.GetDirectories(curNode.FullPath + "\\");
					Array.Sort( (Array)DirectoryList );

					// Clear dummy node and Populate TreeView
					curNode.Nodes.Clear();

					// Populate sub directory list
					foreach (string dir in DirectoryList) {
						DirectoryInfo mDir = new DirectoryInfo( dir );
						if ((mDir.Attributes & System.IO.FileAttributes.Hidden ) == 0) {	// Hidden = 0x2
							// Add each directory
							TreeNode node;
							node = curNode.Nodes.Add(mDir.Name);
							node.ImageIndex = node.SelectedImageIndex = 0;

							// Add dummy child node to make node expandable
							node.Nodes.Add("");

							// Populate sub directory
							PopulateSubDirectory(node, depth);
						}
					}

				}
				catch (Exception e) {
					BugTracking.WriteExceptionRecord( e );
					throw new Exception(e.Message);
				}
			}
			else {
				foreach (TreeNode node in curNode.Nodes) {
					// Populate sub directory
					PopulateSubDirectory(node, depth);
				}
			}
		}
	
		public DirectoryTreeView() {
			InitializeComponent();
			InitializeShellTree();
		}

		public DirectoryTreeView(string strIniPath) {
			InitializeComponent();
			InitializeShellTree();
			PopulateShellTree(strIniPath);
		}

		public string Path {
			get {
				return this.SelectedNode.FullPath;
			}
			set {
				PopulateShellTree(value);
			}
		}

		// Initialize DirectoryTreeView with current logical drive list
		// It dosen't support OS specific drive list yet. (eg. My Documents, Network Neighborhood)
		private void InitializeShellTree() {
			this.PathSeparator = "\\";

			// Get Logical Drive List
			try {
				string[] strDriveList;
				strDriveList = Directory.GetLogicalDrives();	
			
				foreach (string strDrive in strDriveList) {
					// Add each drive
					TreeNode node;
					node = this.Nodes.Add(strDrive.Substring(0, strDrive.Length - 1));
					
					switch(GetDriveType(strDrive)) {
					case DriveType.Removable:
						node.ImageIndex = node.SelectedImageIndex = 2;
						break;
					case DriveType.Fixed:
						node.ImageIndex = node.SelectedImageIndex = 3;
						break;
					case DriveType.CdRom:
						node.ImageIndex = node.SelectedImageIndex = 4;
						break;
					case DriveType.Remote:
						node.ImageIndex = node.SelectedImageIndex = 5;
						break;
					default:
						node.ImageIndex = node.SelectedImageIndex = 3;
						break;
					}
						
					// Add dummy child node to make node expandable
					node.Nodes.Add("");
				}
			}
			catch (Exception e) {
				BugTracking.WriteExceptionRecord( e );
				MessageBox.Show(e.Message, "Can't initialize device list", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
			}
		}

		private void PopulateShellTree(string strPath) {
			// Split path with a '\' char
			string[] strPathList = strPath.Split(new char[] {'\\'});
			
			// Get top node list
			TreeNodeCollection curNodes = this.Nodes;
			
			foreach(string strPopulatePath in strPathList) {
				foreach(TreeNode curChildNode in curNodes) {
					if (curChildNode.Text.ToLower().Equals(strPopulatePath.ToLower()) == true) {
						// Set node as selected
						this.SelectedNode = curChildNode;

						// Populate and expand child node
						PopulateSubDirectory(curChildNode, 2);
						curChildNode.Expand();

						curNodes = curChildNode.Nodes;
						break;
					}
				}
			}
		}
	}
}

