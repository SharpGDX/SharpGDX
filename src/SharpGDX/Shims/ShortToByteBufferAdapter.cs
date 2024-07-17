using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** This class wraps a byte buffer to be a short buffer.
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
	sealed class ShortToByteBufferAdapter : ShortBuffer , IByteBufferWrapper
	{
// implements DirectBuffer {

	internal static ShortBuffer wrap(ByteBuffer byteBuffer)
	{
		return new ShortToByteBufferAdapter(byteBuffer.slice());
	}

	private readonly ByteBuffer byteBuffer;

	ShortToByteBufferAdapter(ByteBuffer byteBuffer)
			: base((byteBuffer.capacity() >> 1))
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

		public override ShortBuffer asReadOnlyBuffer()
	{
		ShortToByteBufferAdapter buf = new ShortToByteBufferAdapter(byteBuffer.asReadOnlyBuffer());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

		public override ShortBuffer compact()
	{
		if (byteBuffer.isReadOnly())
		{
			throw new ReadOnlyBufferException();
		}
		byteBuffer.limit(_limit << 1);
		byteBuffer.position(_position << 1);
		byteBuffer.compact();
		byteBuffer.clear();
		_position = _limit - _position;
		_limit = _capacity;
		_mark = UNSET_MARK;
		return this;
	}

		public override ShortBuffer duplicate()
	{
		ShortToByteBufferAdapter buf = new ShortToByteBufferAdapter(byteBuffer.duplicate());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

		public override short get()
	{
		if (_position == _limit)
		{
			throw new BufferUnderflowException();
		}
		return byteBuffer.getShort(_position++ << 1);
	}

		public override short get(int index)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return byteBuffer.getShort(index << 1);
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

		protected override short[] protectedArray()
		{
			// TODO: Not sure if this stays or not. -RP
			var data = byteBuffer.array();
		short[] sdata = new short[(int)Math.Ceiling(data.Length / 2.0)];
		System.Buffer.BlockCopy(data, 0, sdata, 0, data.Length);
			//throw new UnsupportedOperationException();

			return sdata;
		}

		protected override int protectedArrayOffset()
	{
		throw new UnsupportedOperationException();
	}

		protected override bool protectedHasArray()
	{
		return false;
	}

		public override ShortBuffer put(short c)
	{
		if (_position == _limit)
		{
			throw new BufferOverflowException();
		}
		byteBuffer.putShort(_position++ << 1, c);
		return this;
	}

		public override ShortBuffer put(int index, short c)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		byteBuffer.putShort(index << 1, c);
		return this;
	}

		public override ShortBuffer slice()
	{
		byteBuffer.limit(_limit << 1);
		byteBuffer.position(_position << 1);
		ShortBuffer result = new ShortToByteBufferAdapter(byteBuffer.slice());
		byteBuffer.clear();
		return result;
	}

	public ByteBuffer getByteBuffer()
	{
		return byteBuffer;
	}

}
}
