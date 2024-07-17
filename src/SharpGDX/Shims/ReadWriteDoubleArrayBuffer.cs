using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** DoubleArrayBuffer, ReadWriteDoubleArrayBuffer and ReadOnlyDoubleArrayBuffer compose the implementation of array based double
 * buffers.
 * <p>
 * ReadWriteDoubleArrayBuffer extends DoubleArrayBuffer with all the write methods.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	sealed class ReadWriteDoubleArrayBuffer : DoubleArrayBuffer
	{

	static ReadWriteDoubleArrayBuffer copy(DoubleArrayBuffer other, int markOfOther)
	{
		ReadWriteDoubleArrayBuffer buf = new ReadWriteDoubleArrayBuffer(other.capacity(), other.backingArray, other.offset);
		buf._limit = other.limit();
		buf._position = other.position();
		buf._mark = markOfOther;
		return buf;
	}

	internal ReadWriteDoubleArrayBuffer(double[] array)
			: base(array)
		{
		
	}

	internal ReadWriteDoubleArrayBuffer(int capacity)
			: base(capacity)
		{
		
	}

	internal ReadWriteDoubleArrayBuffer(int capacity, double[] backingArray, int arrayOffset)
			: base(capacity, backingArray, arrayOffset)
		{
		
	}

	public override DoubleBuffer asReadOnlyBuffer()
	{
		return ReadOnlyDoubleArrayBuffer.copy(this, _mark);
	}

		public override DoubleBuffer compact()
	{
		Array.Copy(backingArray, _position + offset, backingArray, offset, remaining());
		_position = _limit - _position;
		_limit = _capacity;
		_mark = UNSET_MARK;
		return this;
	}

		public override DoubleBuffer duplicate()
	{
		return copy(this, _mark);
	}

		public override bool isReadOnly()
	{
		return false;
	}

		protected override double[] protectedArray()
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

		public override DoubleBuffer put(double c)
	{
		if (_position == _limit)
		{
			throw new BufferOverflowException();
		}
		backingArray[offset + _position++] = c;
		return this;
	}

		public override DoubleBuffer put(int index, double c)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		backingArray[offset + index] = c;
		return this;
	}

		public override DoubleBuffer put(double[] src, int off, int len)
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

		public override DoubleBuffer slice()
	{
		return new ReadWriteDoubleArrayBuffer(remaining(), backingArray, offset + _position);
	}

}
}
