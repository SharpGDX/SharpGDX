using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class Map<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> _dictionary = new();

		public IEnumerable<TValue> values()
		{
			return _dictionary.Values;
		}

		public IEnumerable<TKey> keySet()
		{
			return _dictionary.Keys;
		}

		public void clear()
		{
			_dictionary.Clear();
		}

		public bool containsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		public TValue get(TKey key, TValue defaultValue)
		{
			return _dictionary.GetValueOrDefault(key, defaultValue);
		}

		public TValue? get(TKey key)
		{
			_dictionary.TryGetValue(key, out var value);
			return value;
		}

		public void remove(TKey key)
		{
			_dictionary.Remove(key);}

		public IEnumerable<KeyValuePair<TKey, TValue>> entrySet()
		{
			return _dictionary;
		}

		public void put(TKey key, TValue value)
		{
			_dictionary[key] = value;
		}
	}
}
