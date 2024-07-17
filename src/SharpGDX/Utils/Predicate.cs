using System.Collections;

namespace SharpGDX.Utils
{
	/** Interface used to select items within an iterator against a predicate.
 * @author Xoppa */
	public interface IPredicate<T>
	{

		/** @return true if the item matches the criteria and should be included in the iterator's items */
		bool evaluate(T arg0);

		public class PredicateIterator<T> : IEnumerator<T> {
		public IEnumerator<T> iterator;
		public IPredicate<T> predicate;
		public bool end = false;
		public bool peeked = false;
		public T next = default;

		public void Dispose(){}

		public PredicateIterator(IEnumerable<T> iterable, IPredicate<T> predicate)
		: this(iterable.GetEnumerator(), predicate)
			{
			
		}

		public PredicateIterator(IEnumerator<T> iterator,  IPredicate<T> predicate)
		{
			set(iterator, predicate);
		}

		public void set(IEnumerable<T> iterable,  IPredicate<T> predicate)
		{
			set(iterable.GetEnumerator(), predicate);
		}

		public void set(IEnumerator<T> iterator, IPredicate<T> predicate)
		{
			this.iterator = iterator;
			this.predicate = predicate;
			end = peeked = false;
			next = default;
		}

		public bool MoveNext()
		{
			if (end) return false;
			if (next != null) return true;
			peeked = true;
			while (iterator.MoveNext())
			{
				T n = iterator.Current;
				if (predicate.evaluate(n))
				{
					next = n;
					return true;
				}
			}
			end = true;
			return false;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}

		object IEnumerator.Current => Current;

		public T? Current
		{
			get
			{
				if (next == null && !MoveNext()) return default;
				 T result = next;
				next = default;
				peeked = false;
				return result;
			}
		}

		public void remove()
		{
			if (peeked) throw new GdxRuntimeException("Cannot remove between a call to hasNext() and next().");
			// TODO: iterator.remove();
			throw new NotImplementedException();
		}
	}

	public class PredicateIterable<T> : IEnumerable<T> {
		public IEnumerable<T> iterable;
	public IPredicate<T> predicate;
	public PredicateIterator<T> iterator = null;

	public PredicateIterable(IEnumerable<T> iterable, IPredicate<T> predicate)
	{
		set(iterable, predicate);
	}

	public void set(IEnumerable<T> iterable, IPredicate<T> predicate)
	{
		this.iterable = iterable;
		this.predicate = predicate;
	}

	/** Returns an iterator. Remove is supported.
	 * <p>
	 * If {@link Collections#allocateIterators} is false, the same iterator instance is returned each time this method is
	 * called. Use the {@link Predicate.PredicateIterator} constructor for nested or multithreaded iteration. */
	public IEnumerator<T> GetEnumerator()
	{
		if (Collections.allocateIterators) return new PredicateIterator<T>(iterable.GetEnumerator(), predicate);
		if (iterator == null)
			iterator = new PredicateIterator<T>(iterable.GetEnumerator(), predicate);
		else
			iterator.set(iterable.GetEnumerator(), predicate);
		return iterator;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	}
}
}
