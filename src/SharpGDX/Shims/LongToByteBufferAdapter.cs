using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** This class wraps a byte buffer to be a long buffer.
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
	sealed class LongToByteBufferAdapter : LongBuffer
	{// implements DirectBuffer {

	internal static LongBuffer wrap(ByteBuffer byteBuffer)
	{
		return new LongToByteBufferAdapter(byteBuffer.slice());
	}

	private readonly ByteBuffer byteBuffer;

	LongToByteBufferAdapter(ByteBuffer byteBuffer)
			: base((byteBuffer.capacity() >> 3))
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

		public LongBuffer asReadOnlyBuffer()
	{
		LongToByteBufferAdapter buf = new LongToByteBufferAdapter(byteBuffer.asReadOnlyBuffer());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

		public LongBuffer compact()
	{
		if (byteBuffer.isReadOnly())
		{
			throw new ReadOnlyBufferException();
		}
		byteBuffer.limit(_limit << 3);
		byteBuffer.position(_position << 3);
		byteBuffer.compact();
		byteBuffer.clear();
		_position = _limit - _position;
		_limit = _capacity;
		_mark = UNSET_MARK;
		return this;
	}

		public LongBuffer duplicate()
	{
		LongToByteBufferAdapter buf = new LongToByteBufferAdapter(byteBuffer.duplicate());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

	
	public long get()
	{
		if (_position == _limit)
		{
			throw new BufferUnderflowException();
		}
		return byteBuffer.getLong(_position++ << 3);
	}

		public long get(int index)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return byteBuffer.getLong(index << 3);
	}

		public bool isDirect()
	{
		return byteBuffer.isDirect();
	}

		public override bool isReadOnly()
	{
		return byteBuffer.isReadOnly();
	}

		public ByteOrder order()
	{
		return byteBuffer.order();
	}

		protected long[] protectedArray()
	{
		throw new UnsupportedOperationException();
	}

		protected int protectedArrayOffset()
	{
		throw new UnsupportedOperationException();
	}

		protected bool protectedHasArray()
	{
		return false;
	}

		public LongBuffer put(long c)
	{
		if (_position == _limit)
		{
			throw new BufferOverflowException();
		}
		byteBuffer.putLong(_position++ << 3, c);
		return this;
	}

		public LongBuffer put(int index, long c)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		byteBuffer.putLong(index << 3, c);
		return this;
	}

		public LongBuffer slice()
	{
		byteBuffer.limit(_limit << 3);
		byteBuffer.position(_position << 3);
		LongBuffer result = new LongToByteBufferAdapter(byteBuffer.slice());
		byteBuffer.clear();
		return result;
	}

}
}
