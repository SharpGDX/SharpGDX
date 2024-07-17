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
 * ReadOnlyFloatArrayBuffer extends FloatArrayBuffer with all the write methods throwing read only exception.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	sealed class ReadOnlyFloatArrayBuffer : FloatArrayBuffer
	{

	internal static ReadOnlyFloatArrayBuffer copy(FloatArrayBuffer other, int markOfOther)
	{
		ReadOnlyFloatArrayBuffer buf = new ReadOnlyFloatArrayBuffer(other.capacity(), other.backingArray, other.offset);
		buf._limit = other.limit();
		buf._position = other.position();
		buf._mark = markOfOther;
		return buf;
	}

	ReadOnlyFloatArrayBuffer(int capacity, float[] backingArray, int arrayOffset)
			: base(capacity, backingArray, arrayOffset)
		{
		
	}

	public override FloatBuffer asReadOnlyBuffer()
	{
		return duplicate();
	}

		public override FloatBuffer compact()
	{
		throw new ReadOnlyBufferException();
	}

		public override FloatBuffer duplicate()
		{
		return copy(this, _mark);
	}

		public override bool isReadOnly()
	{
		return true;
	}

		protected override float[] protectedArray()
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

		public override FloatBuffer put(float c)
	{
		throw new ReadOnlyBufferException();
	}

		public override FloatBuffer put(int index, float c)
	{
		throw new ReadOnlyBufferException();
	}

		public override FloatBuffer put(FloatBuffer buf)
	{
		throw new ReadOnlyBufferException();
	}

		public override FloatBuffer put(float[] src, int off, int len)
	{
		throw new ReadOnlyBufferException();
	}

		public override FloatBuffer slice()
	{
		return new ReadOnlyFloatArrayBuffer(remaining(), backingArray, offset + _position);
	}

	}
}
