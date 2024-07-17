using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** HeapByteBuffer, ReadWriteHeapByteBuffer and ReadOnlyHeapByteBuffer compose the implementation of array based byte buffers.
 * <p>
 * ReadWriteHeapByteBuffer extends HeapByteBuffer with all the write methods.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	sealed class ReadWriteHeapByteBuffer : HeapByteBuffer
	{

	static ReadWriteHeapByteBuffer copy(HeapByteBuffer other, int markOfOther)
	{
		ReadWriteHeapByteBuffer buf = new ReadWriteHeapByteBuffer(other.backingArray, other.capacity(), other.offset);
		buf._limit = other.limit();
		buf._position = other.position();
		buf._mark = markOfOther;
		buf.order(other.order());
		return buf;
	}

	internal ReadWriteHeapByteBuffer(byte[] backingArray)
			: base(backingArray)
		{
		
	}

	internal ReadWriteHeapByteBuffer(int capacity)
			: base(capacity)
		{		
	}

	internal ReadWriteHeapByteBuffer(byte[] backingArray, int capacity, int arrayOffset)
			: base(backingArray, capacity, arrayOffset)
		{
		
	}

	public override ByteBuffer asReadOnlyBuffer()
	{
		return ReadOnlyHeapByteBuffer.copy(this, _mark);
	}

		public override ByteBuffer compact()
	{
		Array.Copy(backingArray, _position + offset, backingArray, offset, remaining());
		_position = _limit - _position;
		_limit = _capacity;
		_mark = UNSET_MARK;
		return this;
	}

		public override ByteBuffer duplicate()
	{
		return copy(this, _mark);
	}

		public override bool isReadOnly()
	{
		return false;
	}

		protected override byte[] protectedArray()
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

		public override ByteBuffer put(byte b)
	{
		if (_position == _limit)
		{
			throw new BufferOverflowException();
		}
		backingArray[offset + _position++] = b;
		return this;
	}

		public override ByteBuffer put(int index, byte b)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		backingArray[offset + index] = b;
		return this;
	}

		/*
		 * Override ByteBuffer.put(byte[], int, int) to improve performance.
		 * 
		 * (non-Javadoc)
		 * 
		 * @see java.nio.ByteBuffer#put(byte[], int, int)
		 */
		public override ByteBuffer put(byte[] src, int off, int len)
	{
		if (off < 0 || len < 0 || (long)off + (long)len > src.Length)
		{
			throw new IndexOutOfBoundsException();
		}
		if (len > remaining())
		{
			throw new BufferOverflowException();
		}
		if (isReadOnly())
		{
			throw new ReadOnlyBufferException();
		}
		Array.Copy(src, off, backingArray, offset + _position, len);
		_position += len;
		return this;
	}

		public override ByteBuffer putDouble(double value)
	{
		return putLong(Numbers.doubleToRawLongBits(value));
	}

		public override ByteBuffer putDouble(int index, double value)
	{
		return putLong(index, Numbers.doubleToRawLongBits(value));
	}

		public override ByteBuffer putFloat(float value)
	{
		return putInt(Numbers.floatToIntBits(value));
	}

		public override ByteBuffer putFloat(int index, float value)
	{
		return putInt(index, Numbers.floatToIntBits(value));
	}

		public override ByteBuffer putInt(int value)
	{
		int newPosition = _position + 4;
		if (newPosition > _limit)
		{
			throw new BufferOverflowException();
		}
		store(_position, value);
		_position = newPosition;
		return this;
	}

		public override ByteBuffer putInt(int index, int value)
	{
		if (index < 0 || (long)index + 4 > _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		store(index, value);
		return this;
	}

		public override ByteBuffer putLong(int index, long value)
	{
		if (index < 0 || (long)index + 8 > _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		store(index, value);
		return this;
	}

		public override ByteBuffer putLong(long value)
	{
		int newPosition = _position + 8;
		if (newPosition > _limit)
		{
			throw new BufferOverflowException();
		}
		store(_position, value);
		_position = newPosition;
		return this;
	}

		public override ByteBuffer putShort(int index, short value)
	{
		if (index < 0 || (long)index + 2 > _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		store(index, value);
		return this;
	}

		public override ByteBuffer putShort(short value)
	{
		int newPosition = _position + 2;
		if (newPosition > _limit)
		{
			throw new BufferOverflowException();
		}
		store(_position, value);
		_position = newPosition;
		return this;
	}

		public override ByteBuffer slice()
	{
		ReadWriteHeapByteBuffer slice = new ReadWriteHeapByteBuffer(backingArray, remaining(), offset + _position);
		slice._order = _order;
		return slice;
	}
}
}
