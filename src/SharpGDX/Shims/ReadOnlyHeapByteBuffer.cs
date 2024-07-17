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
 * ReadOnlyHeapByteBuffer extends HeapByteBuffer with all the write methods throwing read only exception.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	sealed class ReadOnlyHeapByteBuffer : HeapByteBuffer
	{

	internal static ReadOnlyHeapByteBuffer copy(HeapByteBuffer other, int markOfOther)
	{
		ReadOnlyHeapByteBuffer buf = new ReadOnlyHeapByteBuffer(other.backingArray, other.capacity(), other.offset);
		buf._limit = other.limit();
		buf._position = other.position();
		buf._mark = markOfOther;
		buf.order(other.order());
		return buf;
	}

	ReadOnlyHeapByteBuffer(byte[] backingArray, int capacity, int arrayOffset)
			: base(backingArray, capacity, arrayOffset)
		{
		
	}

	public override ByteBuffer asReadOnlyBuffer()
	{
		return copy(this, _mark);
	}

		public override ByteBuffer compact()
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer duplicate()
	{
		return copy(this, _mark);
	}

		public override bool isReadOnly()
	{
		return true;
	}

		protected override byte[] protectedArray()
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

		public override ByteBuffer put(byte b)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer put(int index, byte b)
	{
		throw new ReadOnlyBufferException();
	}

	public override ByteBuffer put(byte[] src, int off, int len)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer putDouble(double value)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer putDouble(int index, double value)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer putFloat(float value)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer putFloat(int index, float value)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer putInt(int value)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer putInt(int index, int value)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer putLong(int index, long value)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer putLong(long value)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer putShort(int index, short value)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer putShort(short value)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer put(ByteBuffer buf)
	{
		throw new ReadOnlyBufferException();
	}

		public override ByteBuffer slice()
	{
		ReadOnlyHeapByteBuffer slice = new ReadOnlyHeapByteBuffer(backingArray, remaining(), offset + _position);
		slice._order = _order;
		return slice;
	}
}
}
