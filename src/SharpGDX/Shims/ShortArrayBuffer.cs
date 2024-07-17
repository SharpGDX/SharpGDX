using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/** ShortArrayBuffer, ReadWriteShortArrayBuffer and ReadOnlyShortArrayBuffer compose the implementation of array based short
 * buffers.
 * <p>
 * ShortArrayBuffer implements all the shared readonly methods and is extended by the other two classes.
 * </p>
 * <p>
 * All methods are marked final for runtime performance.
 * </p>
 */
	abstract class ShortArrayBuffer : ShortBuffer
	{

	internal protected readonly short[] backingArray;

	internal protected readonly int offset;

	protected ShortArrayBuffer(short[] array)
			: this(array.Length, array, 0)
		{
		
	}

	protected ShortArrayBuffer(int capacity)
			: this(capacity, new short[capacity], 0)
		{
		
	}

	protected ShortArrayBuffer(int capacity, short[] backingArray, int offset)
			: base(capacity)
		{
		
		this.backingArray = backingArray;
		this.offset = offset;
	}

	public override short get()
	{
		if (_position == _limit)
		{
			throw new BufferUnderflowException();
		}
		return backingArray[offset + _position++];
	}

	public override short get(int index)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return backingArray[offset + index];
	}

	public override ShortBuffer get(short[] dest, int off, int len)
	{
		int length = dest.Length;
		if (off < 0 || len < 0 || (long)off + (long)len > length)
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
