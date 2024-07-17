using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** FloatArrayBuffer, ReadWriteFloatArrayBuffer and ReadOnlyFloatArrayBuffer compose the implementation of array based float
 * buffers.
 * <p>
 * ReadWriteFloatArrayBuffer extends FloatArrayBuffer with all the write methods.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	sealed class ReadWriteFloatArrayBuffer : FloatArrayBuffer
	{

	static ReadWriteFloatArrayBuffer copy(FloatArrayBuffer other, int markOfOther)
	{
		ReadWriteFloatArrayBuffer buf = new ReadWriteFloatArrayBuffer(other.capacity(), other.backingArray, other.offset);
		buf._limit = other.limit();
		buf._position = other.position();
		buf._mark = markOfOther;
		return buf;
	}

internal	ReadWriteFloatArrayBuffer(float[] array)
			: base(array)
		{		
	}

	internal ReadWriteFloatArrayBuffer(int capacity)
		:base(capacity)
	{
		
	}

	internal ReadWriteFloatArrayBuffer(int capacity, float[] backingArray, int arrayOffset)
			: base(capacity, backingArray, arrayOffset)
		{		
	}

	public override FloatBuffer asReadOnlyBuffer()
	{
		return ReadOnlyFloatArrayBuffer.copy(this, _mark);
	}

		public override FloatBuffer compact()
	{
		// System.arraycopy(backingArray, position + offset, backingArray, offset, remaining());
		for (int i = _position + offset, j = offset, k = 0; k < remaining(); i++, j++, k++)
		{
			backingArray[j] = backingArray[i];
		}
		_position = _limit - _position;
		_limit = _capacity;
		_mark = UNSET_MARK;
		return this;
	}

		public override FloatBuffer duplicate()
	{
		return copy(this, _mark);
	}

		public override bool isReadOnly()
	{
		return false;
	}

		protected override float[] protectedArray()
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

		public override FloatBuffer put(float c)
	{
		if (_position == _limit)
		{
			throw new BufferOverflowException();
		}
		backingArray[offset + _position++] = c;
		return this;
	}

		public override FloatBuffer put(int index, float c)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		backingArray[offset + index] = c;
		return this;
	}

		public override FloatBuffer put(float[] src, int off, int len)
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

	public override FloatBuffer slice()
	{
		return new ReadWriteFloatArrayBuffer(remaining(), backingArray, offset + _position);
	}

}
}
