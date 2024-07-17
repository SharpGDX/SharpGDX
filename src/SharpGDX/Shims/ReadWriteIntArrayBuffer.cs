using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** IntArrayBuffer, ReadWriteIntArrayBuffer and ReadOnlyIntArrayBuffer compose the implementation of array based int buffers.
 * <p>
 * ReadWriteIntArrayBuffer extends IntArrayBuffer with all the write methods.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	sealed class ReadWriteIntArrayBuffer : IntArrayBuffer
	{

	static ReadWriteIntArrayBuffer copy(IntArrayBuffer other, int markOfOther)
	{
		ReadWriteIntArrayBuffer buf = new ReadWriteIntArrayBuffer(other.capacity(), other.backingArray, other.offset);
		buf._limit = other.limit();
		buf._position = other.position();
		buf._mark = markOfOther;
		return buf;
	}

	internal ReadWriteIntArrayBuffer(int[] array)
	: base(array)
		{
	}

	internal ReadWriteIntArrayBuffer(int capacity)
	: base(capacity)
		{
		
	}

	internal ReadWriteIntArrayBuffer(int capacity, int[] backingArray, int arrayOffset)
	: base(capacity, backingArray, arrayOffset)
		{
		
	}

	public override IntBuffer asReadOnlyBuffer()
	{
		return ReadOnlyIntArrayBuffer.copy(this, _mark);
	}

		public override IntBuffer compact()
	{
		Array.Copy(backingArray, _position + offset, backingArray, offset, remaining());
		_position = _limit - _position;
		_limit = _capacity;
		_mark = UNSET_MARK;
		return this;
	}

	public override IntBuffer duplicate()
	{
		return copy(this, _mark);
	}

	public override bool isReadOnly()
	{
		return false;
	}

	protected override int[] protectedArray()
	{
		return backingArray;
	}

	protected override int protectedArrayOffset()
	{
		return offset;
	}

	protected override bool protectedHasArray()
	{
		return true;
	}

	public override IntBuffer put(int c)
	{
		if (_position == _limit)
		{
			throw new BufferOverflowException();
		}
		backingArray[offset + _position++] = c;
		return this;
	}

	public override IntBuffer put(int index, int c)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		backingArray[offset + index] = c;
		return this;
	}

		public override IntBuffer put(int[] src, int off, int len)
	{
		int length = src.Length;
		if (off < 0 || len < 0 || (long)off + (long)len > length)
		{
			throw new IndexOutOfBoundsException();
		}
		if (len > remaining())
		{
			throw new BufferOverflowException();
		}
		Array.Copy(src, off, backingArray, offset + _position, len);
		_position += len;
		return this;
	}

	public override IntBuffer slice()
	{
		return new ReadWriteIntArrayBuffer(remaining(), backingArray, offset + _position);
	}

}
}
