using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
    /** A sorted double linked list which uses ints for indexing
 * 
 * @param <E> */
public class SortedIntList<E> : IEnumerable<SortedIntList<E>.Node> {
	private NodePool nodePool = new NodePool(); // avoid allocating nodes
		[NonSerialized]
	private Iterator iterator;
	int _size = 0;

	Node first;

	/** Creates an ascending list */
	public SortedIntList () {
	}

	/** Inserts an element into the list at the given index
	 * 
	 * @param index Index of the element
	 * @param value Element to insert
	 * @return Element replaced by newly inserted element, null if nothing was replaced */
	public E? insert (int index, E value) {
		if (first != null) {
			Node c = first;
			// iterate to the right until we can't move any further because the next number is bigger than index
			while (c.n != null && c.n.index <= index) {
				c = c.n;
			}
			// add one to the right
			if (index > c.index) {
				c.n = nodePool.obtain(c, c.n, value, index);
				if (c.n.n != null) {
					c.n.n.p = c.n;
				}
                _size++;
			}
			// the new element is smaller than every other element
			else if (index < c.index) {
				Node newFirst = nodePool.obtain(null, first, value, index);
				first.p = newFirst;
				first = newFirst;
                _size++;
			}
			// that element already exists so swap the value
			else {
				c.value = value;
			}
		} else {
			first = nodePool.obtain(null, null, value, index);
            _size++;
		}
		return default;
	}

	/** Retrieves an element at a given index
	 * 
	 * @param index Index of the element to retrieve
	 * @return Matching element, null otherwise */
	public E get (int index) {
		E? match = default;
		if (first != null) {
			Node c = first;
			while (c.n != null && c.index < index) {
				c = c.n;
			}
			if (c.index == index) {
				match = c.value;
			}
		}
		return match;
	}

	/** Clears list */
	public void clear () {
		for (; first != null; first = first.n) {
			nodePool.free(first);
		}
        _size = 0;
	}

	/** @return size of list equal to elements contained in it */
	public int size () {
		return _size;
	}

	/** Returns true if the list has one or more items. */
	public bool notEmpty () {
		return _size > 0;
	}

	/** Returns true if the list is empty. */
	public bool isEmpty () {
		return _size == 0;
	}

	/** Returns an iterator to traverse the list.
	 * <p>
	 * If {@link Collections#allocateIterators} is false, the same iterator instance is returned each time this method is called.
	 * Use the {@link Iterator} constructor for nested or multithreaded iteration. */
	public IEnumerator<Node> GetEnumerator () {
		if (Collections.allocateIterators) return new Iterator(this);
		if (iterator == null) return iterator = new Iterator(this);
        iterator.Reset();
		return iterator;
	}

	public class Iterator : IEnumerator<Node> {
        private readonly SortedIntList<E> _sortedIntList;
        private Node position;
		private Node previousPosition;

		public Iterator (SortedIntList<E> sortedIntList)
        {
            _sortedIntList = sortedIntList;
            Reset();
        }

		public bool MoveNext () {
			return position != null;
		}

        public Node Current
        {
            get
            {
                previousPosition = position;
                position = position.n;
                return previousPosition;
            }
        }

        public void remove () {
			// the contract specifies to remove the last returned element, if nothing was returned yet assumably do nothing
			if (previousPosition != null) {
				// if we are at the second element set it as the first element
				if (previousPosition == _sortedIntList.first) {
                    _sortedIntList.first = position;
				}
				// else remove last returned element by changing the chain
				else {
					previousPosition.p.n = position;
					if (position != null) {
						position.p = previousPosition.p;
					}
				}
                _sortedIntList._size--;
			}
		}

		public void Reset () {
			position = _sortedIntList.first;
			previousPosition = null;
		}

        object? IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }

	public  class Node {
		/** Node previous to this */
		protected internal Node p;
		/** Node next to this */
		protected internal Node n;
		/** Value held */
		public E value;
		/** Index value in list */
		public int index;
	}

	class NodePool : Pool<Node> {
		protected internal override Node newObject () {
			return new Node();
		}

		public Node obtain (Node p, Node n, E value, int index) {
			Node newNode = base.obtain();
			newNode.p = p;
			newNode.n = n;
			newNode.value = value;
			newNode.index = index;
			return newNode;
		}
	}

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
}
