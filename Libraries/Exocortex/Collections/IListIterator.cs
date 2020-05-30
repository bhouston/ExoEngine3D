#warning THIS IS BETA 1.06, 2002 MARCH 12 4:24 GMT

// IListIterator.cs
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
    /// This interface provides bi-directional iteration over a collection. 
    /// Unlike IEnumerator, it allows insertion, deletion, and modification 
    /// of the underlying collection. 
    /// </summary>
    /// <remarks>
    /// Documented at http://www.parkscomputing.com/dotnet/list.aspx
    /// </remarks>
    public interface IListIterator : IEnumerator
    {
        /// <summary>
        /// Move the iterator to the previous position if there is an 
        /// element at that position.
        /// </summary>
        /// <returns>
        /// true if there is an object at the previous position; 
        /// false if the list is empty or if the iterator is at the 
        /// beginning of the collection.
        /// </returns>
        bool MovePrevious();

        /// <summary>
        /// Move the iterator to the head of the list, one position 
        /// before the first element in the list.
        /// </summary>
        /// <returns>
        /// true if the iterator was moved; false if the list is empty
        /// </returns>
        bool MoveBegin();

        /// <summary>
        /// Move the iterator to the tail of the list, one position past 
        /// the last element in the collection.
        /// </summary>
        /// <returns>
        /// true if the iterator was moved; false if the list is empty
        /// </returns>
        bool MoveEnd();

        /// <summary>
        /// Move the iterator to the element at the specified zero-based
        /// index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to make current.
        /// </param>
        /// <returns>
        /// true if the iterator was successfully moved to the 
        /// specified element; false if the index is out of range.
        /// </returns>
        bool MoveTo(int index);

        /// <summary>
        /// Insert a value at the current iterator location.
        /// </summary>
        /// <param name="value">
        /// The Object to insert into the list.
        /// </param>
        void Insert(object value);

        /// <summary>
        /// When implemented by a class, removes the current element 
        /// from the list.
        /// </summary>
        void Remove();

        /// <summary>
        /// Indicate whether or not there is an item in the 
        /// collection after the current location
        /// </summary>
        bool HasNext
        {
            get;
        }

        /// <summary>
        /// Indicate whether or not there is an item in the 
        /// collection before the current location
        /// </summary>
        bool HasPrevious
        {
            get;
        }

        /// <summary>
        /// Returns a reference to the current node.
        /// </summary>
        ListNode Node
        {
            get;
        }

        /// <summary>
        /// Splices an <c>ILinkedList</c> collection into the iterator 
        /// before the current node.
        /// </summary>
        /// <param name="spliceList">
        /// The list to splice into the current collection.
        /// </param>
        void Splice(ILinkedList spliceList);

    }
}
