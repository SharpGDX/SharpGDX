using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/** IntArrayBuffer, ReadWriteIntArrayBuffer and ReadOnlyIntArrayBuffer compose the implementation of array based int buffers.
 * <p>
 * IntArrayBuffer implements all the shared readonly methods and is extended by the other two classes.
 * </p>
 * <p>
 * All methods are marked final for runtime performance.
 * </p>
 */
	abstract class IntArrayBuffer : IntBuffer
	{

	internal protected readonly int[] backingArray;

	internal protected readonly int offset;

	protected IntArrayBuffer(int[] array)
	: this(array.Length, array, 0)
		{
		
	}

	protected IntArrayBuffer(int capacity)
	: this(capacity, new int[capacity], 0)
		{
		
	}

	protected IntArrayBuffer(int capacity, int[] backingArray, int offset)
	: base(capacity)
		{
		this.backingArray = backingArray;
		this.offset = offset;
	}

	public override int get()
	{
		if (_position == _limit)
		{
			throw new BufferUnderflowException();
		}
		return backingArray[offset + _position++];
	}

		public override int get(int index)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return backingArray[offset + index];
	}

		public override IntBuffer get(int[] dest, int off, int len)
	{
		int length = dest.Length;
		if (off < 0 || len < 0 || (long)len + (long)off > length)
		{
			throw new IndexOutOfBoundsException();
		}
		if (len > remaining())
		{
			throw new BufferUnderflowException();
		}
		Array.Copy(backingArray, offset + _position, dest, off, len);
		_position += len;
		return this;
	}

		public override bool isDirect()
	{
		return false;
	}

		public override ByteOrder order()
	{
		return ByteOrder.nativeOrder();
	}

	}
}
