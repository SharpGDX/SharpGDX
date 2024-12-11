using System.Collections;
using SharpGDX.Utils;

namespace SharpGDX.Graphics;

public sealed partial class VertexAttributes
{
    private class ReadonlyIterable<T>(T[] array) : IEnumerable<T>
    {
        private ReadonlyIterator<T>? _iterator1;
        private ReadonlyIterator<T> _iterator2 = null!;

        public IEnumerator<T> GetEnumerator()
        {
            if (Collections.allocateIterators)
            {
                return new ReadonlyIterator<T>(array);
            }

            if (_iterator1 == null)
            {
                _iterator1 = new ReadonlyIterator<T>(array);
                _iterator2 = new ReadonlyIterator<T>(array);
            }

            if (!_iterator1.Valid)
            {
                _iterator1.Index = 0;
                _iterator1.Valid = true;
                _iterator2.Valid = false;
                return _iterator1;
            }

            _iterator2.Index = 0;
            _iterator2.Valid = true;
            _iterator1.Valid = false;

            return _iterator2;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}