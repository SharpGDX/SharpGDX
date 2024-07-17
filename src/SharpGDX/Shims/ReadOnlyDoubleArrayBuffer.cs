using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/** DoubleArrayBuffer, ReadWriteDoubleArrayBuffer and ReadOnlyDoubleArrayBuffer compose the implementation of array based double
 * buffers.
 * <p>
 * ReadOnlyDoubleArrayBuffer extends DoubleArrayBuffer with all the write methods throwing read only exception.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	sealed class ReadOnlyDoubleArrayBuffer : DoubleArrayBuffer
	{

	internal static ReadOnlyDoubleArrayBuffer copy(DoubleArrayBuffer other, int markOfOther)
	{
		ReadOnlyDoubleArrayBuffer buf = new ReadOnlyDoubleArrayBuffer(other.capacity(), other.backingArray, other.offset);
		buf._limit = other.limit();
		buf._position = other.position();
		buf._mark = markOfOther;
		return buf;
	}

	ReadOnlyDoubleArrayBuffer(int capacity, double[] backingArray, int arrayOffset)
			: base(capacity, backingArray, arrayOffset)
		{
		
	}

	public override DoubleBuffer asReadOnlyBuffer()
	{
		return duplicate();
	}

		public override DoubleBuffer compact()
	{
		throw new ReadOnlyBufferException();
	}

		public override DoubleBuffer duplicate()
	{
		return copy(this, _mark);
	}

		public override bool isReadOnly()
	{
		return true;
	}

		protected override double[] protectedArray()
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

		public override DoubleBuffer put(double c)
	{
		throw new ReadOnlyBufferException();
	}

		public override DoubleBuffer put(int index, double c)
	{
		throw new ReadOnlyBufferException();
	}

		public override DoubleBuffer put(double[] src, int off, int len)
	{
		throw new ReadOnlyBufferException();
	}

		public override DoubleBuffer put(DoubleBuffer buf)
	{
		throw new ReadOnlyBufferException();
	}

		public override DoubleBuffer slice()
	{
		return new ReadOnlyDoubleArrayBuffer(remaining(), backingArray, offset + _position);
	}

	}
}
