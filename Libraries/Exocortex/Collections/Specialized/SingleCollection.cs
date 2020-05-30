using System;
using System.Collections;


namespace Exocortex.Collections.Specialized {
	/***begin code***/

	using T = System.Single;

	/// <summary>
	/// A collection of planes
	/// </summary>
	[Serializable]
	public class SingleCollection : IList, ICollection, IEnumerable, ICloneable {
		//members
		private T[] _data;
		private int _size;
		private static int _defaultCapacity = 16;

		//constructors
		public SingleCollection() {
			_data = new T[_defaultCapacity];
		}

		public SingleCollection(SingleCollection x) {
			_data = new T[x.Count * 2];
			AddRange(x);
		}

		public SingleCollection(T[] x) {
			_data = new T[x.Length * 2];
			AddRange(x);
		}

		//properties
		public T this[int index] {
			get {
				if (index >= _size)
					throw new ArgumentOutOfRangeException("index");
				return _data[index];
			}

			set {
				if (index >= _size)
					throw new ArgumentOutOfRangeException("index");
				_data[index] = value;
			}
		}

		object IList.this[int index] {
			get {
				if (index >= _size)
					throw new ArgumentOutOfRangeException("index");
				return _data[index];
			}

			set {
				if (index >= _size)
					throw new ArgumentOutOfRangeException("index");
				_data[index] = (T)value;
			}
		}

		bool IList.IsFixedSize{
			get{ return false;}
		}

		bool IList.IsReadOnly{
			get{ return false;}
		}


		public int Count{
			get{ return _size;}
		}

		bool ICollection.IsSynchronized{
			get{ return _data.IsSynchronized;}
		}

		object ICollection.SyncRoot{
			get{ return _data.SyncRoot;}
		}

		// Methods
		private void EnsureCapacity(int capacity){
			int currentCapacity = _data != null? _data.Length: 0;
			if (currentCapacity >= capacity)
				return;

			if (currentCapacity == 0)
				SetCapacity(_defaultCapacity);
			else
				SetCapacity(currentCapacity * 2);

		}

		public void SetCapacity(int capacity){
			int currentCapacity = _data != null? _data.Length: 0;
			if (currentCapacity == capacity)
				return;

			if (currentCapacity > capacity)
				throw new ArgumentOutOfRangeException("value",
					"ArgumentOutOfRange_SmallCapacity");

			T[] temp = new T[capacity];

			if (_data != null)
				System.Array.Copy(_data, 0, temp, 0, currentCapacity);

			_data = temp;
		}

		public void Clear(){
			_size = 0;
			_data = null;
		}

		int IList.Add(object val){
			return Add((T)val);
		}

		public int Add(T x) {
			EnsureCapacity(_size + 1);
			_data[_size] = x;
			return _size++;
		}

		public void AddRange(T[] x) {
			for (int i=0; i < x.Length; ++i)
				Add(x[i]);
		}

		public void AddRange(SingleCollection x) {
			for (int i=0; i < x.Count; ++i)
				Add(x[i]);
		}

		bool IList.Contains(object x) {return Contains((T)x);}

		public bool Contains(T x) {
			for(int cnt = 0; cnt < _size; ++cnt){
				if (_data[cnt] == x)
					return true;
			}
			return false;
		}

		public void CopyTo(T[] array, int index) {
			System.Array.Copy(_data, 0, array, 0, _size);
		}

		int IList.IndexOf(object x) {return IndexOf((T)x);}

		public int IndexOf(T x) {
			for(int cnt = 0; cnt < _size; ++cnt){
				if (_data[cnt] == x)
					return cnt;
			}
			return -1;
		}

		void IList.Insert(int index, object x) {Insert(index, (T)x);}

		public void Insert(int index, T x) {
			if (index > _size)
				throw new ArgumentOutOfRangeException("index");
			if (index == _size)
				this.Add(x);

			T[] tail = new T[_size-index];
			Array.Copy(_data, index, tail, 0, _size-index);
			_data[index] = x;
			Array.Copy(tail, 0, _data, index + 1, _size-index);
			++_size;
		}

		void IList.Remove(object x) {Remove((T)x);}

		public void Remove(T x) {
			for(int cnt = 0; cnt < _size; ++cnt){
				if (_data[cnt] == x){
					IList il = (IList)this;
					il.RemoveAt(cnt);
					return;
				}
			}
		}

		void IList.RemoveAt(int index){

			if (index > _size)
				throw new ArgumentOutOfRangeException("index");

			T[] tail = new T[_size-index];
			Array.Copy(_data, index+1, tail, 0, _size-index-1);
			Array.Copy(tail, 0, _data, index, _size-index-1);
			--_size;
			return;
		}

		void ICollection.CopyTo(Array array, int index){
			Array.Copy(_data, 0, array, index, _size);
		}


		IEnumerator IEnumerable.GetEnumerator() {
			return new SingleEnumerator(this);
		}

		public SingleEnumerator GetEnumerator() {
			return new SingleEnumerator(this);
		}



		object ICloneable.Clone(){
			SingleCollection c = new SingleCollection();
			c._size = this._size;
			c._data = (T[])_data.Clone();
			return c;
		}

		public void	FromArray( Single[] array ) {
			if( array != null ) {
				this.Clear();
				foreach( Single o in array ) {
					this.Add( o );
				}
			}
		}
		
		public Single[] ToArray() {
			Single[] array = new Single[ this.Count ];
			this.CopyTo( array, 0 );
			return array;
		}

		public void Reverse() {
			int count = this.Count;
			for( int i = 0; i < count / 2; i ++ ) {
				Single temp = this[i];
				this[i] = this[count - i - 1];
				this[count - i - 1] = temp;
			}
		}
	}

	public class SingleEnumerator : IEnumerator {

		private SingleCollection temp;
		private T curr;
		private int cursor;

		public SingleEnumerator(SingleCollection col) {
			temp = col;
			cursor = -1;
		}

		public void Reset() { cursor = -1; curr = (float) 0; }

		public T Current {
			get {
				return curr;
			}
		}

		object IEnumerator.Current {
			get {
				return Current;
			}
		}

		public bool MoveNext() {
			++cursor;
			if (cursor < temp.Count) {
				curr = temp[cursor];
				return true;
			}
			else
				return false;
		}
	}


}
