#warning THIS IS BETA 1.07, 2002 MARCH 12 8:35 GMT

// LinkedList.cs
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
using System.Diagnostics;
using System.Runtime.Serialization;

namespace ParksComputing.Collections
{
    /// <summary>
    /// Provides an implementation of <c>ILinkedList</c> on a 
    /// doubly-linked list.
    /// </summary>
    /// <remarks>
    /// Documented at http://www.parkscomputing.com/dotnet/list.aspx
    /// </remarks>
    [Serializable]
    public class LinkedList : ILinkedList, IDeserializationCallback
    {
        #region Constructors
        /// <summary>
        /// Default constructor creates an empty list.
        /// </summary>
        public LinkedList()
        {
            this.head.value = this;
            this.enumerator = new NodeEnumerator(this.head);
        }

        /// <summary>
        /// Populates a new linked list with the values copied from existing 
        /// <c>System.Collections.ICollection</c>.
        /// </summary>
        /// <param name="collection">
        /// An object that implements the 
        /// <c>System.Collections.ICollection</c> interface.
        /// </param>
        public LinkedList(ICollection collection) : this()
        {
            foreach (object o in collection)
            {
                this.AddBack(o);
            }
        }
        #endregion

        #region Operators
        /// <summary>
        /// Add an object to the list.
        /// </summary>
        /// <param name="lhs">The list to hold the object.</param>
        /// <param name="rhs">The object to add.</param>
        /// <returns>A reference to the list.</returns>
        public static LinkedList operator + (LinkedList lhs, object rhs)
        {
            lhs.AddBack(rhs);
            return lhs;
        }

        /// <summary>
        /// Test for equality of two LinkedList objects.
        /// </summary>
        /// <param name="lhs">An object of type <c>LinkedList</c>.</param>
        /// <param name="rhs">An object of type <c>LinkedList</c>.</param>
        /// <returns>
        /// <c>true</c> if the objects are equal; 
        /// <c>false</c> otherwise.
        /// </returns>
        public static bool operator == (LinkedList lhs, LinkedList rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Test for inequality of two LinkedList objects.
        /// </summary>
        /// <param name="lhs">An object of type <c>LinkedList</c>.</param>
        /// <param name="rhs">An object of type <c>LinkedList</c>.</param>
        /// <returns>
        /// <c>true</c> if the objects are not equal; 
        /// <c>false</c> otherwise.
        /// </returns>
        public static bool operator != (LinkedList lhs, LinkedList rhs)
        {
            return !lhs.Equals(rhs);
        }
        #endregion

        #region Class implementation
        /// <summary>
        /// The head node always points to the first and last elements in 
        /// the list. When the list is empty, it is self-referential. Well, 
        /// okay, before the class is fully constructed it's <c>null</c>.
        /// The first node in a populated list points back to head, and 
        /// the last node points forward to head.
        /// </summary>
        ListHead head = new ListHead();

        /// <summary>
        /// The total number of elements in the list.
        /// </summary>
        int count = 0;

        /// <summary>
        /// When a user calls <c>GetEnumerator</c> or 
        /// <c>GetIterator</c> the collection goes into 
        /// "iteration" state. This is because a collection is not 
        /// allowed to be modified while inside a <c>foreach</c> block. 
        /// If a <c>MoveNext</c> calls is made on an enumerator for a 
        /// list and the list has been modified, <c>MoveNext</c> will 
        /// throw an <c>InvalidOperationException</c>.
        /// </summary>
        [NonSerialized] bool isIterating = false;

        /// <summary>
        /// This is an internal enumerator used for seeking from the 
        /// front or the back of the list. Rather than create a new 
        /// instance on every method call when it's needed, this instance
        /// is shared.
        /// </summary>
        [NonSerialized] NodeEnumerator enumerator;

        /// <summary>
        /// Add the specified value to the collection immediately before 
        /// the specified <c>ListNode</c>.
        /// </summary>
        /// <param name="value">
        /// The value to be inserted into the collection.
        /// </param>
        /// <param name="insertNode">
        /// The node before which the value will be inserted.
        /// </param>
        protected virtual void InsertNode(object value, ListNode insertNode)
        {
            #region Assert
            Debug.Assert(
                insertNode != null,
                "Null insertion node",
                "Attempted to insert before a null node"
                );

            Debug.Assert(
                insertNode.Next != null && insertNode.Previous != null,
                "Null reference found",
                "A null reference was found in the insertion node"
                );
            #endregion

            if (this.IsReadOnly)
            {
                throw new NotSupportedException();
            }

            ListNode newNode = new ListNode(insertNode, insertNode.previous, value);
            insertNode.previous.next = newNode;
            insertNode.previous = newNode;
            
            ++count;
            isIterating = false;
        }

        /// <summary>
        /// Insert an existing <c>ILinkedList</c> collection before  
        /// the specified node.
        /// </summary>
        /// <param name="spliceList">
        /// The <c>ILinkedList</c> to splice.
        /// </param>
        /// <param name="insertNode">
        /// The node before which the list will be spliced.
        /// </param>
        protected virtual void SpliceList(ILinkedList spliceList, ListNode insertNode)
        {
            #region Assert
            Debug.Assert(
                insertNode != null,
                "Null insertion node",
                "Attempted to insert before a null node"
                );

            Debug.Assert(
                insertNode.Next != null && insertNode.Previous != null,
                "Null reference found",
                "A null reference was found in the insertion node"
                );

            Debug.Assert(
                spliceList != null,
                "Null splice list",
                "Attempted to splice a null reference to a list."
                );

            Debug.Assert(
                spliceList.Head.Next != null && spliceList.Head.Previous != null,
                "Splice list head contains null references",
                "Splice list head contains null references"
                );

            #endregion

            if (this.IsReadOnly)
            {
                throw new NotSupportedException();
            }

            insertNode.previous.next = spliceList.Head.next;
            spliceList.Head.next.previous = insertNode.previous;
            insertNode.previous = spliceList.Head.previous;
            spliceList.Head.previous.next = insertNode;

            this.count += spliceList.Count;
            spliceList.Clear();
        }
        #endregion

        #region IDeserializationCallback methods
        /// <summary>
        /// Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <param name="sender">
        /// The object that initiated the callback. The functionality 
        /// for the this parameter is not currently implemented.
        /// </param>
        public virtual void OnDeserialization(object sender)
        {
            this.isIterating = false;
            this.enumerator = new NodeEnumerator(this.head);
        }
        #endregion

        #region ILinkedList methods
        /// <summary>
        /// Adds an item to the beginning of the 
        /// <c>ILinkedList</c> collection.
        /// </summary>
        /// <param name="value">
        /// The Object to add to the <c>ILinkedList</c>.
        /// </param>
        public virtual void AddFront(object value)
        {
            this.InsertNode(value, this.head.next);
        }

        /// <summary>
        /// Adds an item to the end of the 
        /// <c>ILinkedList</c> collection.
        /// </summary>
        /// <param name="value">
        /// The Object to add to the <c>Collections.IList</c>.
        /// </param>
        public virtual void AddBack(object value)
        {
            this.InsertNode(value, this.head);
        }


        /// <summary>
        /// Insert an existing <c>ILinkedList</c> collection at the 
        /// front of the current collection.
        /// </summary>
        /// <param name="spliceList">
        /// The <c>ILinkedList</c> to splice.
        /// </param>
        public virtual void SpliceFront(ILinkedList spliceList)
        {
            this.SpliceList(spliceList, this.head.next);
        }

        /// <summary>
        /// Append an existing <c>ILinkedList</c> collection at the 
        /// end of the current collection.
        /// </summary>
        /// <param name="spliceList">
        /// The <c>ILinkedList</c> to splice.
        /// </param>
        public virtual void SpliceBack(ILinkedList spliceList)
        {
            this.SpliceList(spliceList, this.head);
        }

        /// <summary>
        /// Adds an item to the <c>ILinkedList</c> collection immediately 
        /// before the node containing the specified value.
        /// </summary>
        /// <param name="insertValue">
        /// The object to add to the <c>ILinkedList</c> collection.
        /// </param>
        /// <param name="testValue">
        /// The method finds the first node in the <c>ILinkedList</c> 
        /// collection that contains a value equal to this value. 
        /// If found, the new value is inserted before the node.
        /// </param>
        public virtual void AddBefore(object insertValue, object testValue)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException();
            }

            enumerator.Reset();

            while (enumerator.MoveNext())
            {
                ListNode current = enumerator.Current as ListNode;

                if (current.Value.Equals(testValue))
                {
                    this.InsertNode(insertValue, current);
                    return;
                }
            }
        }


        /// <summary>
        /// Adds an item to the <c>ILinkedList</c> collection immediately 
        /// after the node containing the specified value.
        /// </summary>
        /// <param name="insertValue">
        /// The object to add to the <c>ILinkedList</c> collection.
        /// </param>
        /// <param name="testValue">
        /// The method finds the first node in the <c>ILinkedList</c> 
        /// collection that contains a value equal to this value. 
        /// If found, the new value is inserted after the node.
        /// </param>
        public virtual void AddAfter(object insertValue, object testValue)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException();
            }

            enumerator.Reset();

            while (enumerator.MoveNext())
            {
                ListNode current = enumerator.Current as ListNode;

                if (current.Value.Equals(testValue))
                {
                    this.InsertNode(insertValue, current.next);
                    return;
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that can iterate backwards through a 
        /// collection.
        /// </summary>
        /// <returns>A <c>Collections.IEnumerator</c> that can be used to 
        /// iterate backwards through the collection.</returns>
        public virtual IListIterator GetIterator()
        {
            return new LinkedListIterator(this.head);
        }

        /// <summary>
        /// Returns a reference to the head node in the linked list.
        /// </summary>
        public virtual ListHead Head
        {
            get { return this.head; }
        }
        #endregion

        #region IList methods
        /// <summary>
        /// Adds an item to the <c>Collections.IList</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int Add(object value)
        {
            AddBack(value);
            return count - 1;
        }

        /// <summary>
        /// Removes all items from the <c>Collections.IList</c>.
        /// </summary>
        public virtual void Clear()
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException();
            }

            this.head.next = this.head.previous = head;
            this.count = 0;
            isIterating = false;
        }

        /// <summary>
        /// When implemented by a class, determines whether the 
        /// <c>Collections.IList</c> contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The Object to locate in the <c>Collections.IList</c>.
        /// </param>
        /// <returns>
        /// true if the Object is found in the <c>Collections.IList</c>; 
        /// otherwise, false.
        /// </returns>
        public virtual bool Contains(object value)
        {
            foreach (object o in this)
            {
                if (o.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines the index of a specific item in the list.
        /// </summary>
        /// <param name="value">The object to locate in the list.</param>
        /// <returns>
        /// The index of the first occurrence of the object if found; 
        /// -1 otherwise.
        /// </returns>
        public virtual int IndexOf(object value)
        {
            int index = 0;

            foreach (object o in this)
            {
                if (o.Equals(value))
                {
                    return index;
                }

                ++index;
            }

            return -1;
        }

        /// <summary>
        /// Insert a value at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which value should be inserted.
        /// </param>
        /// <param name="value">
        /// The Object to insert into the IList.
        /// </param>
        public virtual void Insert(int index, object value)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException();
            }

            if (index == count)
            {
                this.AddBack(value);
            }
            else
            {
                enumerator.MoveTo(index);

                if (enumerator.MoveNext())
                {
                    ListNode insert = (ListNode)enumerator.Current;
                    this.InsertNode(value, insert);
                }
            }
        }

        /// <summary>
        /// When implemented by a class, removes the first occurrence of 
        /// a specific object from the <c>Collections.IList</c>.
        /// NOTE that calling this method will reset the iterator.
        /// </summary>
        /// <param name="value">The Object to remove from the 
        /// <c>Collections.IList</c>.</param>
        public virtual void Remove(object value)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException();
            }

            enumerator.Reset();

            while (enumerator.MoveNext())
            {
                ListNode remove = (ListNode)enumerator.Current;

                if (remove.Value.Equals(value))
                {
                    remove.previous.next = remove.next;
                    remove.next.previous = remove.previous;
                    isIterating = false;
                    --count;
                    return;
                }
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the 
        /// System.Collections.IList.
        /// </summary>
        /// <param name="index">
        /// The System.Object to remove from the System.Collections.IList.
        /// </param>
        public virtual void RemoveAt(int index)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException();
            }

            enumerator.MoveTo(index);

            if (enumerator.MoveNext())
            {
                ListNode remove = (ListNode)enumerator.Current;

                remove.previous.next = remove.next;
                remove.next.previous = remove.previous;
                --count;
                isIterating = false;
            }
        }

        /// <summary>
        /// Retrieve the element at the specified index.
        /// </summary>
        public virtual object this[int index]
        {
            get 
            {
                enumerator.MoveTo(index);

                if (enumerator.MoveNext())
                {
                    return ((ListNode)enumerator.Current).Value;
                }

                return null;
            }

            set
            {
                if (this.IsReadOnly)
                {
                    throw new NotSupportedException();
                }

                enumerator.MoveTo(index);

                if (enumerator.MoveNext())
                {
                    ((ListNode)enumerator.Current).value = value;
                    isIterating = false;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <c>System.Collections.IList</c> 
        /// has a fixed size.
        /// </summary>
        public virtual bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the <c>System.Collections.IList</c> 
        /// is read-only.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }
        #endregion

        #region ICollection methods
        /// <summary>
        /// When implemented by a class, copies the elements of the 
        /// <c>Collections.ICollection</c> to an Array, starting at a 
        /// particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <c>Array</c> that is the destination of the 
        /// elements copied from <c>Collections.ICollection</c>. The 
        /// <c>Array</c> must have zero-based indexing.
        /// </param>
        /// <param name="index">
        /// The zero-based index in array at which copying begins.
        /// </param>
        public virtual void CopyTo(System.Array array , System.Int32 index)
        {
            IListNode node = head.Next;

            while (node != head)
            {
                array.SetValue(node.Value, index);

                index++;
                node = node.Next;
            }
        }

        /// <summary>
        /// When implemented by a class, gets the number of elements 
        /// contained in the <c>Collections.ICollection</c>.
        /// </summary>
        public virtual int Count
        {
            get { return this.count; }
        }

        /// <summary>
        /// When implemented by a class, gets a value indicating whether 
        /// access to the <c>Collections.ICollection</c> is synchronized 
        /// (thread-safe).
        /// </summary>
        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// When implemented by a class, gets an object that can be used to 
        /// synchronize access to the <c>Collections.ICollection</c>.
        /// </summary>
        public virtual object SyncRoot
        {
            get { return this; }
        }
        #endregion

        #region IEnumerable methods
        /// <summary>
        /// Returns an enumerator that can iterate through a collection.
        /// </summary>
        /// <returns>A <c>Collections.IEnumerator</c> that can be used to 
        /// iterate through the collection.</returns>
        public virtual System.Collections.IEnumerator GetEnumerator()
        {
            isIterating = true;
            return new LinkedListEnumerator(this.head);
        }
        #endregion

        #region IEnumerator implementation
        /// <summary>
        /// Implementation of <c>IEnumerator</c> on a linked list.
        /// </summary>
        [Serializable]
        protected internal class LinkedListEnumerator : IEnumerator
        {
            /// <summary>
            /// Current node.
            /// </summary>
            protected ListNode node;

            /// <summary>
            /// Reference to linked list object that created this enumerator.
            /// </summary>
            protected LinkedList list;

            /// <summary>
            /// Head node of the list.
            /// </summary>
            protected ListNode head;

            /// <summary>
            /// Delegate for setting the current node reference.
            /// </summary>
            protected delegate bool MoveDelegate();

            /// <summary>
            /// Instantiation of <c>MoveDelegate</c> used for forward or 
            /// backward iteration.
            /// </summary>
            [NonSerialized] protected MoveDelegate MovePointer;

            /// <summary>
            /// Create an enumerator on a linked list.
            /// </summary>
            /// <param name="head">
            /// The head node for the list.
            /// </param>
            public LinkedListEnumerator(IListHead head)
            {
                #region Assert
                Debug.Assert(
                    head != null,
                    "Null list header",
                    "A null list header was passed to the constructor."
                    );

                Debug.Assert(
                    head.Next != null && head.Previous != null,
                    "Null head reference",
                    "The list header contains a null reference."
                    );
                #endregion

                this.head = head as ListNode;

                if (this.head == null)
                {
                    throw new InvalidOperationException();
                }

                this.list = head.Value as LinkedList;
                this.list.isIterating = true;
                this.node = this.head.next.previous;
            }

            /// <summary>
            /// Create an enumerator on a linked list that will 
            /// start at the specified index.
            /// </summary>
            /// <param name="head">
            /// Head node for the list.
            /// </param>
            /// <param name="index">
            /// Position to which the enumerator will advance.
            /// </param>
            public LinkedListEnumerator(IListHead head, int index) 
                : this(head)
            {
                this.MoveTo(index);
            }

            /// <summary>
            /// Move the current node pointer to the next node.
            /// </summary>
            /// <returns>
            /// true if the pointer was moved; false otherwise.
            /// </returns>
            protected bool MoveForward()
            {
                if (this.node.next != head)
                {
                    this.node = this.node.next;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Move the current node pointer to the previous node.
            /// </summary>
            /// <returns>
            /// true if the pointer was moved; false otherwise.
            /// </returns>
            protected bool MoveBackward()
            {
                if (this.node.previous != head)
                {
                    this.node = this.node.previous;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Set the iterator to the specified element in the collection.
            /// </summary>
            /// <param name="index">
            /// The zero-based index of the element to make current.
            /// </param>
            /// <returns>
            /// true if the iterator was successfully moved to the 
            /// specified element; false if the index is out of range.
            /// </returns>
            public virtual bool MoveTo(int index)
            {
                if (index >= this.list.count || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                this.node = this.head;

                // If the starting index is past the midpoint of the 
                // list, walk the list from the back instead.
                if (index > (this.list.count / 2) )
                {
                    index = (this.list.count - index) + 1;
                    MovePointer = new MoveDelegate(MoveBackward);
                }
                else
                {
                    MovePointer = new MoveDelegate(MoveForward);
                }

                // Walk the chain until the current node is just before 
                // the node at index (or just after, if iterating backwards).
                // If index is 0, the current node is head and there's 
                // no need to move the pointer.
                while (index > 0)
                {
                    // MovePointer() returns false when the current node 
                    // is the head node. That indicates a corrupt list. 
                    if (!MovePointer())
                    {
                        throw new IndexOutOfRangeException();
                    }

                    --index;

                    #region Assert
                    Debug.Assert(
                        node != null,
                        "Null list reference",
                        "A null reference was found in the list chain."
                        );
                    #endregion
                }

                #region Assert
                // If this happens, the chain is corrupted.
                Debug.Assert(
                    index >= 0,
                    "Index out of range",
                    "Index out of range"
                    );
                #endregion

                return true;
            }

            #region IEnumerator methods
            /// <summary>
            /// Advances the iterator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the iterator was successfully advanced to the 
            /// next element; false if the iterator has passed the end 
            /// of the collection.
            /// </returns>
            public virtual System.Boolean MoveNext()
            {
                if (!list.isIterating)
                {
                    throw new InvalidOperationException();
                }

                // return this.MovePointer();
                return this.MoveForward();
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is 
            /// before the first element in the collection.
            /// </summary>
            public virtual void Reset()
            {
                if (!list.isIterating)
                {
                    throw new InvalidOperationException();
                }

                this.node = this.head.next.previous;
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public virtual object Current
            {
                get 
                { 
                    if (this.node == head)
                    {
                        throw new InvalidOperationException();
                    }

                    return this.node.Value;
                }

                set 
                { 
                    if (this.list.IsReadOnly)
                    {
                        throw new NotSupportedException();
                    }

                    if (this.node == head)
                    {
                        throw new InvalidOperationException();
                    }

                    this.node.value = value;
                }
            }
            #endregion
        }
        #endregion

        #region NodeEnumerator implementation
        /// <summary>
        /// Create an enumerator on a list that accesses the 
        /// list nodes rather than the objects in each node.
        /// </summary>
        [Serializable]
        protected internal class NodeEnumerator : LinkedListEnumerator
        {
            /// <summary>
            /// Create a node enumerator on a linked list.
            /// </summary>
            public NodeEnumerator(IListHead head) : base(head)
            {
            }

            /// <summary>
            /// Create a node enumerator on a linked list that will 
            /// start at the specified index.
            /// </summary>
            public NodeEnumerator(IListHead head, int index) : base(head, index)
            {
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public override object Current
            {
                get 
                { 
                    if (this.node == this.head)
                    {
                        throw new InvalidOperationException();
                    }

                    return this.node;
                }

                set 
                { 
                    if (this.list.IsReadOnly)
                    {
                        throw new NotSupportedException();
                    }

                    if (this.node == this.head)
                    {
                        throw new InvalidOperationException();
                    }

                    if (value is ListNode)
                    {
                        this.node = (ListNode)value;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }
        #endregion

        #region IListIterator implementation
        /// <summary>
        /// Create an <c>IListIterator</c> instance on a linked list.
        /// </summary>
        [Serializable]
        protected internal class LinkedListIterator : 
            LinkedListEnumerator, IListIterator
        {
            /// <summary>
            /// Create a node enumerator on a linked list.
            /// </summary>
            public LinkedListIterator(IListHead head) : base(head)
            {
            }

            /// <summary>
            /// Create a node enumerator on a linked list that will 
            /// start at the specified index.
            /// </summary>
            public LinkedListIterator(IListHead head, int index) : base(head, index)
            {
            }

            #region LinkedList IEnumerator methods
            /// <summary>
            /// Advances the iterator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the iterator was successfully advanced to the 
            /// next element; false if the iterator has passed the end 
            /// of the collection.
            /// </returns>
            public override System.Boolean MoveNext()
            {
                return this.MoveForward();
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is 
            /// before the first element in the collection.
            /// </summary>
            public override void Reset()
            {
                this.node = this.head.next.previous;
            }
            #endregion

            #region LinkedListIterator IListIterator methods
            /// <summary>
            /// Move the iterator to the previous position if there is an 
            /// element at that position.
            /// </summary>
            /// <returns>
            /// true if there is an object at the previous position; 
            /// false if the list is empty or if the iterator is at the 
            /// beginning of the collection.
            /// </returns>
            public virtual bool MovePrevious()
            {
                return this.MoveBackward();
            }

            /// <summary>
            /// Move the iterator to the head of the list, one position 
            /// before the first element in the list.
            /// </summary>
            /// <returns>
            /// true if the iterator was moved; false if the list is empty
            /// </returns>
            public virtual bool MoveBegin()
            {
                this.node = this.head.next.previous;
                return (this.node.Next != this.head);
            }

            /// <summary>
            /// Move the iterator to the tail of the list, one position past 
            /// the last element in the collection.
            /// </summary>
            /// <returns>
            /// true if the iterator was moved; false if the list is empty
            /// </returns>
            public virtual bool MoveEnd()
            {
                this.node = this.head.previous;
                return (this.node.Next != this.head);
            }

            // Implemented in ListEnumerator
            /*
            public override bool MoveTo(int index)
            */

            /// <summary>
            /// Insert a value at the current iterator location.
            /// </summary>
            /// <param name="value">
            /// The Object to insert into the list.
            /// </param>
            public virtual void Insert(object value)
            {
                list.InsertNode(value, node);
            }

            /// <summary>
            /// Removes the current item.
            /// </summary>
            public virtual void Remove()
            {
                if (this.list.IsReadOnly)
                {
                    throw new NotSupportedException();
                }

                if (this.node == this.head)
                {
                    throw new InvalidOperationException();
                }

                this.node.previous.next = this.node.next;
                this.node.next.previous = this.node.previous;
                this.list.count--;
            }

            /// <summary>
            /// Indicate whether or not there is an item in the 
            /// collection after the current location
            /// </summary>
            public virtual bool HasNext
            {
                get
                {
                    return this.node.Next != this.head;
                }
            }

            /// <summary>
            /// Indicate whether or not there is an item in the 
            /// collection before the current location
            /// </summary>
            public virtual bool HasPrevious
            {
                get
                {
                    return this.node.Previous != this.head;
                }
            }

            /// <summary>
            /// Returns a reference to the head node in the linked list.
            /// </summary>
            public virtual ListNode Node
            {
                get { return this.node; }
            }

            /// <summary>
            /// Splices an <c>ILinkedList</c> collection into the iterator 
            /// before the current node.
            /// </summary>
            /// <param name="spliceList">
            /// The list to splice into the current collection.
            /// </param>
            public virtual void Splice(ILinkedList spliceList)
            {
                this.list.SpliceList(spliceList, this.node);
            }
            #endregion

            #region LinkedListIterator System.Object method overrides
            /// <summary>
            /// Determines whether the specified System.Object is equal to the 
            /// current System.Object.
            /// </summary>
            /// <param name="compare">
            /// The <c>System.Object</c> to compare with the current 
            /// <c>System.Object</c>.
            /// </param>
            /// <returns>
            /// <c>true</c> if the specified System.Object is equal to the current 
            /// <c>System.Object</c>; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object compare)
            {
                return this.list.Equals(compare);
            }

            /// <summary>
            /// Serves as a hash function for a particular type, suitable for 
            /// use in hashing algorithms and data structures like a hash table.
            /// </summary>
            /// <returns>
            /// A hash code for the current <c>System.Object</c>.
            /// </returns>
            public override int GetHashCode()
            {
                return this.list.GetHashCode();
            }
            #endregion
        }
        #endregion

        #region System.Object overrides
        /// <summary>
        /// Determines whether the specified System.Object is equal to the 
        /// current System.Object.
        /// </summary>
        /// <param name="compare">
        /// The <c>System.Object</c> to compare with the current 
        /// <c>System.Object</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified System.Object is equal to the current 
        /// <c>System.Object</c>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object compare)
        {
            // If compare is another reference to this, they're the same.
            if (base.Equals(compare))
            {
                return true;
            }
            else
            {
                LinkedList compareList = compare as LinkedList;

                // If compare is of type LinkedList, and it has the same 
                // number of elements as this object, continue test.
                if (compareList != null && this.Count == compareList.Count)
                {
                    IEnumerator thisEnum = this.GetEnumerator();
                    IEnumerator compareEnum = compareList.GetEnumerator();

                    // Compare each element in compare to each element 
                    // in this list. If an element does not match, these 
                    // lists are not equal.
                    while (thisEnum.MoveNext() && compareEnum.MoveNext())
                    {
                        if (!thisEnum.Current.Equals(compareEnum.Current))
                        {
                            return false;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type, suitable for 
        /// use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <c>System.Object</c>.
        /// </returns>
        public override int GetHashCode()
        {
            int hashCode = 0;

            foreach (object o in this)
            {
                hashCode ^= o.GetHashCode();
            }

            return hashCode;
        }
        #endregion
    }
}
