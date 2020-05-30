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
using System.Diagnostics;
using System.Xml.Serialization;

namespace Exocortex.Collections {

	public class BinaryTreeNode {

		//----------------------------------------------------------------------------

		private	bool			_bParentNode	= false;
		private	BinaryTreeNode	_parentNode		= null;

		protected bool				IsParentNode {
			get {	return	_bParentNode;	}
		}
		protected BinaryTreeNode	ParentNode {
			get	{	return	_parentNode;	}
		}

		//----------------------------------------------------------------------------

		private bool			_bLeftChild	= false;
		private BinaryTreeNode	_leftChild	= null;

		protected bool				IsLeftChild {
			get {	return	_bLeftChild;	}
		}
		protected BinaryTreeNode	LeftChild {
			get	{	return	_leftChild;		}
			set {
				if( _bLeftChild ) {
					Debug.Assert( _leftChild != null );
					Debug.Assert( _leftChild._parentNode == this );
					_leftChild._parentNode	= null;
					_leftChild._bParentNode	= false;
				}				
				_leftChild = value;
				_bLeftChild = ( _leftChild != null );
				if( _bLeftChild ) {
					Debug.Assert( _leftChild != null );
					Debug.Assert( _leftChild._parentNode == null );
					_leftChild._parentNode	= this;
					_leftChild._bParentNode	= true;
				}
			}
		}

		private bool			_bRightChild	= false;
		private BinaryTreeNode	_rightChild		= null;

		protected bool				IsRightChild {
			get {	return	_bRightChild;	}
		}
		protected BinaryTreeNode	RightChild {
			get	{	return	_rightChild;	}
			set {
				if( _bRightChild ) {
					Debug.Assert( _rightChild != null );
					Debug.Assert( _rightChild._parentNode == this );
					_rightChild._parentNode		= null;
					_rightChild._bParentNode	= false;
				}				
				_rightChild = value;
				_bRightChild = ( _rightChild != null );
				if( _bRightChild ) {
					Debug.Assert( _rightChild != null );
					Debug.Assert( _rightChild._parentNode == null );
					_rightChild._parentNode		= this;
					_rightChild._bParentNode	= true;
				}
			}
		}

		//----------------------------------------------------------------------------
	
		/*[XmlIgnore]
		public bool	IsRoot {
			get {	return	_parentNode != null;	}
		}
		[XmlIgnore]
		public bool	IsLeaf {
			get {	return	( _leftChild == null ) && ( _rightChild == null );	}
		}*/

		//----------------------------------------------------------------------------

	}
}
