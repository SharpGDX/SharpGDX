using System.Collections;
using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX.Graphics;

public sealed partial class VertexAttributes
{
    private class ReadonlyIterator<T>(T[] array) : IEnumerator<T>, IEnumerable<T>
    {
        internal int Index;
        internal bool Valid = true;

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (!Valid)
            {
                throw new GdxRuntimeException("#iterator() cannot be used nested.");
            }

            return Index < array.Length;
        }

        public T Current
        {
            get
            {
                if (Index >= array.Length)
                {
                    throw new NoSuchElementException(Index.ToString());
                }

                if (!Valid)
                {
                    throw new GdxRuntimeException("#iterator() cannot be used nested.");
                }

                return array[Index++];
            }
        }

        public void Reset()
        {
            Index = 0;
        }

        object? IEnumerator.Current => Current;
    }
}