using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/** CharArrayBuffer, ReadWriteCharArrayBuffer and ReadOnlyCharArrayBuffer compose the implementation of array based char buffers.
 * <p>
 * CharArrayBuffer implements all the shared readonly methods and is extended by the other two classes.
 * </p>
 * <p>
 * All methods are marked final for runtime performance.
 * </p>
 */
	abstract class CharArrayBuffer : CharBuffer
	{

	internal protected readonly char[] backingArray;

	internal protected readonly int offset;

protected	CharArrayBuffer(char[] array)
	: this(array.Length, array, 0)
		{
		
	}

	protected CharArrayBuffer(int capacity)
	: this(capacity, new char[capacity], 0)
		{
	}

	protected CharArrayBuffer(int capacity, char[] backingArray, int offset)
	: base(capacity)
		{
		this.backingArray = backingArray;
		this.offset = offset;
	}

	public override char get()
	{
		if (_position == _limit)
		{
			throw new BufferUnderflowException();
		}
		return backingArray[offset + _position++];
	}

	public override char get(int index)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return backingArray[offset + index];
	}

	public override CharBuffer get(char[] dest, int off, int len)
	{
		int length = dest.Length;
		if ((off < 0) || (len < 0) || (long)off + (long)len > length)
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

		public override string subSequence(int start, int end)
	{
		if (start < 0 || end < start || end > remaining())
		{
			throw new IndexOutOfBoundsException();
		}

		CharBuffer result = duplicate();
		result.limit(_position + end);
		result.position(_position + start);
		return result.ToString();
	}

		public override String ToString()
	{
		return new string(backingArray, offset + _position, remaining());
	}
}
}
