using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/** CharArrayBuffer, ReadWriteCharArrayBuffer and ReadOnlyCharArrayBuffer compose the implementation of array based char buffers.
 * <p>
 * ReadOnlyCharArrayBuffer extends CharArrayBuffer with all the write methods throwing read only exception.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	sealed class ReadOnlyCharArrayBuffer : CharArrayBuffer
	{

	internal static ReadOnlyCharArrayBuffer copy(CharArrayBuffer other, int markOfOther)
	{
		ReadOnlyCharArrayBuffer buf = new ReadOnlyCharArrayBuffer(other.capacity(), other.backingArray, other.offset);
		buf._limit = other.limit();
		buf._position = other.position();
		buf._mark = markOfOther;
		return buf;
	}

	internal ReadOnlyCharArrayBuffer(int capacity, char[] backingArray, int arrayOffset)
			: base(capacity, backingArray, arrayOffset)
		{
		
	}

	public override CharBuffer asReadOnlyBuffer()
	{
		return duplicate();
	}

		public override CharBuffer compact()
	{
		throw new ReadOnlyBufferException();
	}

		public override CharBuffer duplicate()
	{
		return copy(this, _mark);
	}

		public override bool isReadOnly()
	{
		return true;
	}

		protected override char[] protectedArray()
	{
		throw new ReadOnlyBufferException();
	}

		protected override int protectedArrayOffset()
	{
		throw new ReadOnlyBufferException();
	}

		protected override bool protectedHasArray()
	{
		return false;
	}

		public override CharBuffer put(char c)
	{
		throw new ReadOnlyBufferException();
	}

		public override CharBuffer put(int index, char c)
	{
		throw new ReadOnlyBufferException();
	}

		public override CharBuffer put(char[] src, int off, int len)
	{
		throw new ReadOnlyBufferException();
	}

		public override CharBuffer put(CharBuffer src)
	{
		throw new ReadOnlyBufferException();
	}

		public override CharBuffer put(String src, int start, int end)
	{
		if ((start < 0) || (end < 0) || (long)start + (long)end > src.Length)
		{
			throw new IndexOutOfBoundsException();
		}
		throw new ReadOnlyBufferException();
	}

		public override CharBuffer slice()
	{
		return new ReadOnlyCharArrayBuffer(remaining(), backingArray, offset + _position);
	}
}
}
