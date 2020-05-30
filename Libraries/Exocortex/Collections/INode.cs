#warning THIS IS BETA 1.07, 2002 MARCH 12 8:35 GMT

// IListNode.cs
// 
//
// paul@parkscomputing.com
// http://www.parkscomputing.com/dotnet/list.aspx
// 
// Copyright (c) 2002, Paul M. Parks
// All Rights Reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright 
//   notice, this list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above 
//   copyright notice, this list of conditions and the following 
//   disclaimer in the documentation and/or other materials provided 
//   with the distribution.
// 
// * Neither the name of Paul M. Parks nor the names of his 
//   contributors may be used to endorse or promote products derived 
//   from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS 
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
// COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using System.Collections;

namespace ParksComputing.Collections
{
    /// <summary>
    /// Doubly-linked list node interface.
    /// </summary>
    /// <remarks>
    /// Documented at http://www.parkscomputing.com/dotnet/list.aspx
    /// </remarks>
    public interface IListNode
    {
        /// <summary>
        /// Reference to the next node in the linked list.
        /// </summary>
        IListNode Next
        {
            get;
        }

        /// <summary>
        /// Reference to the previous node in the linked list.
        /// </summary>
        IListNode Previous
        {
            get;
        }

        /// <summary>
        /// Reference to the value stored in this node.
        /// </summary>
        object Value
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Implementation of <c>IListNode</c> used in the <c>LinkedList</c> 
    /// collection.
    /// </summary>
    [Serializable]
    public class ListNode : IListNode
    {
        internal ListNode next;
        internal ListNode previous;
        internal object value;

        /// <summary>
        /// Construct a Node instance.
        /// </summary>
        public ListNode()
        {
            previous = next = this;
        }

        /// <summary>
        /// Construct a Node instance from another Node instance.
        /// </summary>
        /// <param name="source">Existing instance of Node</param>
        public ListNode(ListNode source)
        {
            this.next = source.next;
            this.previous = source.previous;
            this.value = source.Value;
        }

        /// <summary>
        /// construct a Node from t
        /// </summary>
        /// <param name="next"></param>
        /// <param name="previous"></param>
        /// <param name="value"></param>
        public ListNode(ListNode next, ListNode previous, object value)
        {
            this.next = next;
            this.previous = previous;
            this.value = value;
        }

        /******************************************************************
        IListNode methods
        *****************************************************************/

        /// <summary>
        /// Reference to the next node in the linked list.
        /// </summary>
        public IListNode Next
        {
            get { return this.next; }
        }

        /// <summary>
        /// Reference to the previous node in the linked list.
        /// </summary>
        public IListNode Previous
        {
            get { return this.previous; }
        }

        /// <summary>
        /// Reference to the value stored in this node.
        /// </summary>
        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }

    /// <summary>
    /// A type, derived from <c>IListNode</c> that may only be 
    /// instantiated as a list head.
    /// </summary>
    public interface IListHead : IListNode
    {
    }

    /// <summary>
    /// Implementation of type <c>IListHead</c> for <c>LinkedList</c> 
    /// collections.
    /// </summary>
    [Serializable]
    public class ListHead : ListNode, IListHead
    {
    }
}
