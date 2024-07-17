using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** ShortArrayBuffer, ReadWriteShortArrayBuffer and ReadOnlyShortArrayBuffer compose the implementation of array based short
 * buffers.
 * <p>
 * ReadWriteShortArrayBuffer extends ShortArrayBuffer with all the write methods.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	sealed class ReadWriteShortArrayBuffer : ShortArrayBuffer
	{

	static ReadWriteShortArrayBuffer copy(ShortArrayBuffer other, int markOfOther)
	{
		ReadWriteShortArrayBuffer buf = new ReadWriteShortArrayBuffer(other.capacity(), other.backingArray, other.offset);
		buf._limit = other.limit();
		buf._position = other.position();
		buf._mark = markOfOther;
		return buf;
	}

	internal ReadWriteShortArrayBuffer(short[] array)
			: base(array)
		{
		
	}

	internal ReadWriteShortArrayBuffer(int capacity)
			: base(capacity)
		{
		
	}

	internal ReadWriteShortArrayBuffer(int capacity, short[] backingArray, int arrayOffset)
			: base(capacity, backingArray, arrayOffset)
		{
		
	}

	public override ShortBuffer asReadOnlyBuffer()
	{
		return ReadOnlyShortArrayBuffer.copy(this, _mark);
	}

		public override ShortBuffer compact()
	{
		Array.Copy(backingArray, _position + offset, backingArray, offset, remaining());
		_position = _limit - _position;
		_limit = _capacity;
		_mark = UNSET_MARK;
		return this;
	}

		public override ShortBuffer duplicate()
	{
		return copy(this, _mark);
	}

		public override bool isReadOnly()
	{
		return false;
	}

		protected override short[] protectedArray()
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

		public override ShortBuffer put(short c)
	{
		if (_position == _limit)
		{
			throw new BufferOverflowException();
		}
		backingArray[offset + _position++] = c;
		return this;
	}

		public override ShortBuffer put(int index, short c)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		backingArray[offset + index] = c;
		return this;
	}

		public override ShortBuffer put(short[] src, int off, int len)
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

		public override ShortBuffer slice()
	{
		return new ReadWriteShortArrayBuffer(remaining(), backingArray, offset + _position);
	}

}
}
