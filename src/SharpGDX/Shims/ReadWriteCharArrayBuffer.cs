using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** CharArrayBuffer, ReadWriteCharArrayBuffer and ReadOnlyCharArrayBuffer compose the implementation of array based char buffers.
 * <p>
 * ReadWriteCharArrayBuffer extends CharArrayBuffer with all the write methods.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	sealed class ReadWriteCharArrayBuffer : CharArrayBuffer
	{

	static ReadWriteCharArrayBuffer copy(CharArrayBuffer other, int markOfOther)
	{
		ReadWriteCharArrayBuffer buf = new ReadWriteCharArrayBuffer(other.capacity(), other.backingArray, other.offset);
		buf._limit = other.limit();
		buf._position = other.position();
		buf._mark = markOfOther;
		return buf;
	}

internal 	ReadWriteCharArrayBuffer(char[] array)
	: base(array)
		{
	}

	internal ReadWriteCharArrayBuffer(int capacity)
	: base(capacity)
		{
		
	}

	internal ReadWriteCharArrayBuffer(int capacity, char[] backingArray, int arrayOffset)
	: base(capacity, backingArray, arrayOffset)
		{
		
	}

	public override CharBuffer asReadOnlyBuffer()
	{
		return ReadOnlyCharArrayBuffer.copy(this, _mark);
	}

		public override CharBuffer compact()
	{
		Array.Copy(backingArray, _position + offset, backingArray, offset, remaining());
		_position = _limit - _position;
		_limit = _capacity;
		_mark = UNSET_MARK;
		return this;
	}

		public override CharBuffer duplicate()
	{
		return copy(this, _mark);
	}

		public override bool isReadOnly()
	{
		return false;
	}

		protected override char[] protectedArray()
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

		public override CharBuffer put(char c)
	{
		if (_position == _limit)
		{
			throw new BufferOverflowException();
		}
		backingArray[offset + _position++] = c;
		return this;
	}

		public override CharBuffer put(int index, char c)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		backingArray[offset + index] = c;
		return this;
	}

		public override CharBuffer put(char[] src, int off, int len)
	{
		int length = src.Length;
		if (off < 0 || len < 0 || (long)len + (long)off > length)
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

		public override CharBuffer slice()
	{
		return new ReadWriteCharArrayBuffer(remaining(), backingArray, offset + _position);
	}

}
}
