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
 * ReadOnlyShortArrayBuffer extends ShortArrayBuffer with all the write methods throwing read only exception.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	sealed class ReadOnlyShortArrayBuffer : ShortArrayBuffer
	{

	internal static ReadOnlyShortArrayBuffer copy(ShortArrayBuffer other, int markOfOther)
	{
		ReadOnlyShortArrayBuffer buf = new ReadOnlyShortArrayBuffer(other.capacity(), other.backingArray, other.offset);
		buf._limit = other.limit();
		buf._position = other.position();
		buf._mark = markOfOther;
		return buf;
	}

	ReadOnlyShortArrayBuffer(int capacity, short[] backingArray, int arrayOffset)
			: base(capacity, backingArray, arrayOffset)
		{
		
	}

	public override ShortBuffer asReadOnlyBuffer()
	{
		return duplicate();
	}

		public override ShortBuffer compact()
	{
		throw new ReadOnlyBufferException();
	}

		public override ShortBuffer duplicate()
	{
		return copy(this, _mark);
	}

		public override bool isReadOnly()
	{
		return true;
	}

		protected override short[] protectedArray()
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

		public override ShortBuffer put(ShortBuffer buf)
	{
		throw new ReadOnlyBufferException();
	}

		public override ShortBuffer put(short c)
	{
		throw new ReadOnlyBufferException();
	}

		public override ShortBuffer put(int index, short c)
	{
		throw new ReadOnlyBufferException();
	}

		public override ShortBuffer put(short[] src, int off, int len)
	{
		throw new ReadOnlyBufferException();
	}

		public override ShortBuffer slice()
	{
		return new ReadOnlyShortArrayBuffer(remaining(), backingArray, offset + _position);
	}

	}
}
