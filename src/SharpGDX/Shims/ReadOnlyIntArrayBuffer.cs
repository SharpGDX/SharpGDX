using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** IntArrayBuffer, ReadWriteIntArrayBuffer and ReadOnlyIntArrayBuffer compose the implementation of array based int buffers.
 * <p>
 * ReadOnlyIntArrayBuffer extends IntArrayBuffer with all the write methods throwing read only exception.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	sealed class ReadOnlyIntArrayBuffer : IntArrayBuffer
	{

	internal static ReadOnlyIntArrayBuffer copy(IntArrayBuffer other, int markOfOther)
	{
		ReadOnlyIntArrayBuffer buf = new ReadOnlyIntArrayBuffer(other.capacity(), other.backingArray, other.offset);
		buf._limit = other.limit();
		buf._position = other.position();
		buf._mark = markOfOther;
		return buf;
	}

	ReadOnlyIntArrayBuffer(int capacity, int[] backingArray, int arrayOffset)
			: base(capacity, backingArray, arrayOffset)
		{
		
	}

	public override IntBuffer asReadOnlyBuffer()
	{
		return duplicate();
	}

		public override IntBuffer compact()
	{
		throw new ReadOnlyBufferException();
	}

		public override IntBuffer duplicate()
		{
		return copy(this, _mark);
	}

		public override bool isReadOnly()
	{
		return true;
	}

		protected override int[] protectedArray()
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

		public override IntBuffer put(int c)
	{
		throw new ReadOnlyBufferException();
	}

		public override IntBuffer put(int index, int c)
	{
		throw new ReadOnlyBufferException();
	}

		public override IntBuffer put(IntBuffer buf)
	{
		throw new ReadOnlyBufferException();
	}

		public override IntBuffer put(int[] src, int off, int len)
	{
		throw new ReadOnlyBufferException();
	}

		public override IntBuffer slice()
	{
		return new ReadOnlyIntArrayBuffer(remaining(), backingArray, offset + _position);
	}

	}
}
