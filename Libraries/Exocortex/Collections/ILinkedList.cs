#warning THIS IS BETA 1.06, 2002 MARCH 12 4:24 GMT

// ILinkedList.cs
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
    /// This interface declares methods for managing a linked list.
    /// </summary>
    /// <remarks>
    /// Documented at http://www.parkscomputing.com/dotnet/list.aspx
    /// </remarks>
    public interface ILinkedList : IList
    {
        /// <summary>
        /// Add an item to the beginning of the 
        /// <c>ILinkedList</c> collection.
        /// </summary>
        /// <param name="value">
        /// The Object to add to the <c>ILinkedList</c>.
        /// </param>
        void AddFront(object value);

        /// <summary>
        /// Add an item to the end of the 
        /// <c>ILinkedList</c> collection.
        /// </summary>
        /// <param name="value">
        /// The Object to add to the <c>Collections.IList</c>.
        /// </param>
        void AddBack(object value);

        /// <summary>
        /// Insert an existing <c>ILinkedList</c> collection at the 
        /// front of the current collection.
        /// </summary>
        /// <param name="spliceList">
        /// The <c>ILinkedList</c> to splice.
        /// </param>
        void SpliceFront(ILinkedList spliceList);

        /// <summary>
        /// Append an existing <c>ILinkedList</c> collection at the 
        /// end of the current collection.
        /// </summary>
        /// <param name="spliceList">
        /// The <c>ILinkedList</c> to splice.
        /// </param>
        void SpliceBack(ILinkedList spliceList);

        /// <summary>
        /// Add an item to the <c>ILinkedList</c> collection immediately 
        /// before the node that contains the specified value.
        /// </summary>
        /// <param name="insertValue">
        /// The object to add to the <c>ILinkedList</c> collection.
        /// </param>
        /// <param name="beforeValue">
        /// The method finds the first node in the <c>ILinkedList</c> 
        /// collection that contains a value equal to this value. 
        /// If found, the new value is inserted before the node.
        /// </param>
        void AddBefore(object insertValue, object beforeValue);

        /// <summary>
        /// Add an item to the <c>ILinkedList</c> collection immediately 
        /// after the node containing the specified value.
        /// </summary>
        /// <param name="insertValue">
        /// The object to add to the <c>ILinkedList</c> collection.
        /// </param>
        /// <param name="afterValue">
        /// The method finds the first node in the <c>ILinkedList</c> 
        /// collection that contains a value equal to this value. 
        /// If found, the new value is inserted after the node.
        /// </param>
        void AddAfter(object insertValue, object afterValue);

        /// <summary>
        /// Returns an <c>IListIterator</c> for the current collection.
        /// </summary>
        /// <returns>A <c>Collections.IEnumerator</c> that can be used to 
        /// iterate backwards through the collection.</returns>
        IListIterator GetIterator();

        /// <summary>
        /// Returns a reference to the head node in the linked list.
        /// </summary>
        ListHead Head
        {
            get;
        }
    }
}
