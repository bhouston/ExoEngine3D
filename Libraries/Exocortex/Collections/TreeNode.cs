/*
 * BSD Licence:
 * Copyright (c) 2001, Ben Houston [ ben@exocortex.org ]
 * Exocortex Technologies [ www.exocortex.org ]
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright 
 * notice, this list of conditions and the following disclaimer in the 
 * documentation and/or other materials provided with the distribution.
 * 3. Neither the name of the <ORGANIZATION> nor the names of its contributors
 * may be used to endorse or promote products derived from this software
 * without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
 * DAMAGE.
 */

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;


namespace Exocortex.Collections {

	public enum	TreeTraversal {
		PreFix,
		InFix,
		PostFix
	}
	
	/// <summary>
	/// Summary description for TreeNode.
	/// </summary>
	public class TreeNode : IEnumerable, ICloneable {
		
		//--------------------------------------------------------------------------------
		//--------------------------------------------------------------------------------

		static private int	s_treeNodeCount	 = 0;
		public TreeNode() {
			s_treeNodeCount ++;
			//Debug.WriteLine( "TreeNode() " + s_treeNodeCount );
		}
		~TreeNode() {
			//Debug.WriteLine( "~TreeNode() " + s_treeNodeCount );
			s_treeNodeCount --;
		}

		public void	Dispose() {
			Debug.Assert( _parent == null );
			foreach( TreeNode child in this._treeNodes ) {
				child._parent = null;
				child.Dispose();
			}
			_treeNodes.Clear();
		}

		//--------------------------------------------------------------------------------

		object	ICloneable.Clone() {
			return	new TreeNode();
		}

		public	TreeNode	Clone() {
			return	(TreeNode)	((ICloneable)this).Clone();
		}

		public	virtual	TreeNode	CloneSubTree() {
			TreeNode	selfClone = this.Clone();
			foreach( TreeNode child in this._treeNodes ) {
				selfClone.Add( child.CloneSubTree() );
			}
			return	selfClone;
		}
		
		//--------------------------------------------------------------------------------

		private	TreeNodeCollection _treeNodes = new TreeNodeCollection();

		public	void		Add( TreeNode treeNode ){
			Debug.Assert( treeNode != null );
			Debug.Assert( _treeNodes.Contains( treeNode ) == false );
			Debug.Assert( treeNode._parent == null );
			
			_treeNodes.Add( treeNode );
			treeNode._parent = this;
		}
		public	void		Remove( TreeNode treeNode ){
			Debug.Assert( treeNode != null );
			Debug.Assert( _treeNodes.Contains( treeNode ) == true );
			Debug.Assert( treeNode._parent == this );

			_treeNodes.Remove( treeNode );
			treeNode._parent = null;
		}		
		public	void		SetAt( int index, TreeNode treeNode ){
			Debug.Assert( 0 <= index && index < this.Count );
			Debug.Assert( treeNode != null );
			Debug.Assert( _treeNodes.Contains( treeNode ) == false );
			Debug.Assert( treeNode._parent == null );

			TreeNode oldTreeNode = _treeNodes[ index ];
			oldTreeNode._parent = null;

			_treeNodes[ index ] = treeNode;
			treeNode._parent = this;
		}		
		public	TreeNode		GetAt( int index ){
			Debug.Assert( 0 <= index && index < this.Count );
			TreeNode treeNode = _treeNodes[ index ];
			Debug.Assert( treeNode._parent == this );
			return	treeNode;
		}		
		public	bool		Contains( TreeNode treeNode ) {
			Debug.Assert( treeNode != null );
			bool contains = _treeNodes.Contains( treeNode );
			if( contains == true ) {
				Debug.Assert( treeNode._parent == this );
				return	true;
			}
			else {
				foreach( TreeNode node in this._treeNodes ) {
					if( node.Contains( treeNode ) == true ) {
						return	true;
					}
				}
			}
			return	false;
		}

		public	TreeNode	this[ int index ] {
			get	{	return	this.GetAt( index );	}
			set {	this.SetAt( index, value );		}
		}
		public	int			Count {
			get	{	return	_treeNodes.Count;	}
		}
		public IEnumerator	GetEnumerator() {
			return	_treeNodes.GetEnumerator();
		}

		public	void	SwapChildren( int a, int b ) {
			Debug.Assert( 0 <= a && a < this.Count );
			Debug.Assert( 0 <= b && b < this.Count );
			
			if( a != b ) {
				TreeNode temp = _treeNodes[ a ];
				_treeNodes[ a ] = _treeNodes[ b ];
				_treeNodes[ b ] = temp;
			}
		}

		//--------------------------------------------------------------------------------

		/*public override	bool	Equals( object o ) {
			if( o is TreeNode ) {
				TreeNode treeNode = (TreeNode) o;
				int children = treeNode.Count;
				if( children != this.Count ) {
					return	false;
				}
				for( int i = 0; i < this.Count; i ++ ) {
					if( this[i].Equals( treeNode[i] ) == false ) {
						return	false;
					}
				}
				return	true;
			}
			return	false;
		}

		public override int		GetHashCode() {
			int	hashCode = base.GetHashCode();
			foreach( TreeNode child in this ) {
				hashCode = ( child.GetHashCode() + hashCode ) ^ hashCode;
			}
			return	hashCode;
		}*/
		
		//--------------------------------------------------------------------------------

		protected	TreeNode	_parent	= null;
		public	TreeNode	Parent {
			get	{	return	_parent;	}
		}

		public	bool		IsLeaf {
			get	{	return	this.Count == 0;	}
		}

		public	bool		IsRoot	{
			get	{	return	_parent == null;	}
		}

		public	TreeNode	GetRoot() {
			if( this.IsRoot == false ) {
				return	this.Parent.GetRoot();
			}
			return	this;
		}

		//--------------------------------------------------------------------------------

		public	int		GetSubTreeDepth() {
			int maxDepth = 0;
			foreach( TreeNode treeNode in _treeNodes ) {
				maxDepth = Math.Max( maxDepth, treeNode.GetSubTreeDepth() );
			}
			return	maxDepth + 1;
		}

		public	int		GetSubTreeCount() {
			int treeCount = 0;
			foreach( TreeNode treeNode in _treeNodes ) {
				treeCount += treeNode.GetSubTreeCount();
			}
			return	treeCount + 1;
		}

		public	TreeNodeCollection	GetSubTreeNodes() {
			return	GetSubTreeNodes( TreeTraversal.PreFix );
		}
		public	TreeNodeCollection	GetSubTreeNodes( TreeTraversal treeTraversal ) {
			Debug.Assert( treeTraversal != TreeTraversal.InFix );
			TreeNodeCollection	treeNodes = new TreeNodeCollection();
			this.CollectTreeNodes( treeNodes, treeTraversal );
			return	treeNodes;
		}
		protected	void	CollectTreeNodes( TreeNodeCollection treeNodes, TreeTraversal treeTraversal ) {
			Debug.Assert( treeNodes != null );
			if( treeTraversal == TreeTraversal.PreFix ) {
				treeNodes.Add( this );
			}
			foreach( TreeNode treeNode in _treeNodes ) {
				treeNode.CollectTreeNodes( treeNodes, treeTraversal );
			}
			if( treeTraversal == TreeTraversal.PostFix ) {
				treeNodes.Add( this );
			}
		}

		//--------------------------------------------------------------------------------

		public override string	ToString() {
			return	"TreeNode[children " + this.Count + "]";
		}

		//--------------------------------------------------------------------------------

		public virtual	void		OutputDebugStructure() {
			Debug2.Push( this.ToString() );
			foreach( TreeNode treeNode in this._treeNodes ) {
				treeNode.OutputDebugStructure();
			}
			Debug2.Pop();
		}

		//--------------------------------------------------------------------------------

	}
}
