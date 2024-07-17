using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** This class wraps a byte buffer to be a float buffer.
 * <p>
 * Implementation notice:
 * <ul>
 * <li>After a byte buffer instance is wrapped, it becomes privately owned by the adapter. It must NOT be accessed outside the
 * adapter any more.</li>
 * <li>The byte buffer's position and limit are NOT linked with the adapter. The adapter extends Buffer, thus has its own position
 * and limit.</li>
 * </ul>
 * </p>
 */
	sealed class FloatToByteBufferAdapter : FloatBuffer
	{
// implements DirectBuffer {

	internal static FloatBuffer wrap(ByteBuffer byteBuffer)
	{
		return new FloatToByteBufferAdapter(byteBuffer.slice());
	}

	private readonly ByteBuffer byteBuffer;

	FloatToByteBufferAdapter(ByteBuffer byteBuffer)
			: base((byteBuffer.capacity() >> 2))
		{
		
		this.byteBuffer = byteBuffer;
		this.byteBuffer.clear();
	}

	// public int getByteCapacity() {
	// if (byteBuffer instanceof DirectBuffer) {
	// return ((DirectBuffer) byteBuffer).getByteCapacity();
	// }
	// assert false : byteBuffer;
	// return -1;
	// }
	//
	// public PlatformAddress getEffectiveAddress() {
	// if (byteBuffer instanceof DirectBuffer) {
	// return ((DirectBuffer) byteBuffer).getEffectiveAddress();
	// }
	// assert false : byteBuffer;
	// return null;
	// }
	//
	// public PlatformAddress getBaseAddress() {
	// if (byteBuffer instanceof DirectBuffer) {
	// return ((DirectBuffer) byteBuffer).getBaseAddress();
	// }
	// assert false : byteBuffer;
	// return null;
	// }
	//
	// public boolean isAddressValid() {
	// if (byteBuffer instanceof DirectBuffer) {
	// return ((DirectBuffer) byteBuffer).isAddressValid();
	// }
	// assert false : byteBuffer;
	// return false;
	// }
	//
	// public void addressValidityCheck() {
	// if (byteBuffer instanceof DirectBuffer) {
	// ((DirectBuffer) byteBuffer).addressValidityCheck();
	// } else {
	// assert false : byteBuffer;
	// }
	// }
	//
	// public void free() {
	// if (byteBuffer instanceof DirectBuffer) {
	// ((DirectBuffer) byteBuffer).free();
	// } else {
	// assert false : byteBuffer;
	// }
	// }

		public override FloatBuffer asReadOnlyBuffer()
	{
		FloatToByteBufferAdapter buf = new FloatToByteBufferAdapter(byteBuffer.asReadOnlyBuffer());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

		public override FloatBuffer compact()
	{
		if (byteBuffer.isReadOnly())
		{
			throw new ReadOnlyBufferException();
		}
		byteBuffer.limit(_limit << 2);
		byteBuffer.position(_position << 2);
		byteBuffer.compact();
		byteBuffer.clear();
		_position = _limit - _position;
		_limit = _capacity;
		_mark = UNSET_MARK;
		return this;
	}

		public override FloatBuffer duplicate()
	{
		FloatToByteBufferAdapter buf = new FloatToByteBufferAdapter(byteBuffer.duplicate());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

		public override float get()
	{
		if (_position == _limit)
		{
			throw new BufferUnderflowException();
		}
		return byteBuffer.getFloat(_position++ << 2);
	}

		public override float get(int index)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return byteBuffer.getFloat(index << 2);
	}

		public override bool isDirect()
	{
		return byteBuffer.isDirect();
	}

		public override bool isReadOnly()
	{
		return byteBuffer.isReadOnly();
	}

		public override ByteOrder order()
	{
		return byteBuffer.order();
	}

		protected override float[] protectedArray()
		{
			// TODO: This is all kinds of wrong, just trying to get somewhere
			var bytes = ((DirectReadWriteByteBuffer)byteBuffer).byteArray.toArray();
			float[] result = new float[bytes.Length / sizeof(int)];
			System.Buffer.BlockCopy(bytes, 0, result, 0, result.Length);
			//throw new UnsupportedOperationException();
			return result;
		}

		protected override int protectedArrayOffset()
	{
		throw new UnsupportedOperationException();
	}

		protected override bool protectedHasArray()
	{
		return false;
	}

		public override FloatBuffer put(float c)
	{
		if (_position == _limit)
		{
			throw new BufferOverflowException();
		}
		byteBuffer.putFloat(_position++ << 2, c);
		return this;
	}

		public override FloatBuffer put(int index, float c)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		byteBuffer.putFloat(index << 2, c);
		return this;
	}

		public override FloatBuffer slice()
	{
		byteBuffer.limit(_limit << 2);
		byteBuffer.position(_position << 2);
		FloatBuffer result = new FloatToByteBufferAdapter(byteBuffer.slice());
		byteBuffer.clear();
		return result;
	}

}
}
