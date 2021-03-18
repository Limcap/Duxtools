using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataUtils
{
	public abstract class BasicList<G> : RawList<G>
	{
		public BasicList( int capacity = 10 ) :  base( capacity )
		{}


		public int Count => _list.Count;

		public virtual void Add( G item ) { _list.Add( item ); }

		public virtual void Remove( G item ) { _list.Remove( item ); }

		public virtual void RemoveAt( int index ) { _list.RemoveAt( index ); }

		public virtual void Clear() { _list.Clear(); }
	}








	public abstract class RawList<G> : IEnumerable<G>
	{
		protected readonly List<G> _list;

		public RawList( int capacity = 10 ) {
			_list = new List<G>( capacity );
		}

		public G this[int key] {
			get => _list[key];
			set => _list[key] = value;
		}

		public IEnumerator<G> GetEnumerator()
		{
			return ((IEnumerable<G>)_list).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_list).GetEnumerator();
		}
	}








	public class Dict<K, V> : Dictionary<K, V>
	{
		public Func<V> GetDefaultValue = () => default;
		private readonly OrderedDictionary _list2 = new OrderedDictionary();
		public new V this[K key] {
			get => this.ContainsKey(key) ? this[key] : GetDefaultValue();
			set => this[key] = value;
		}
	}
	public class Odict<K, V> : OrderedDictionary
	{
		public Func<V> GetDefaultValue = () => default;
		private readonly OrderedDictionary _list2 = new OrderedDictionary();
		public V this[K key] {
			get => this.Contains(key) ? this[key] : GetDefaultValue();
			set => this[key] = value;
		}
	}
}
