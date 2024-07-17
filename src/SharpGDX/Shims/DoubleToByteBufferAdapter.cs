using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** This class wraps a byte buffer to be a double buffer.
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
	sealed class DoubleToByteBufferAdapter : DoubleBuffer
	{
	// implements DirectBuffer {

	internal static DoubleBuffer wrap(ByteBuffer byteBuffer)
	{
		return new DoubleToByteBufferAdapter(byteBuffer.slice());
	}

	private readonly ByteBuffer byteBuffer;

	DoubleToByteBufferAdapter(ByteBuffer byteBuffer)
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

	public override DoubleBuffer asReadOnlyBuffer()
	{
		DoubleToByteBufferAdapter buf = new DoubleToByteBufferAdapter(byteBuffer.asReadOnlyBuffer());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

	public override DoubleBuffer compact()
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

	public override DoubleBuffer duplicate()
	{
		DoubleToByteBufferAdapter buf = new DoubleToByteBufferAdapter(byteBuffer.duplicate());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

	public override double get()
	{
		if (_position == _limit)
		{
			throw new BufferUnderflowException();
		}
		return byteBuffer.getDouble(_position++ << 3);
	}

	public override double get(int index)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return byteBuffer.getDouble(index << 3);
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

	protected override double[] protectedArray()
	{
		throw new UnsupportedOperationException();
	}

	protected override int protectedArrayOffset()
	{
		throw new UnsupportedOperationException();
	}

	protected override bool protectedHasArray()
	{
		return false;
	}

	public override DoubleBuffer put(double c)
	{
		if (_position == _limit)
		{
			throw new BufferOverflowException();
		}
		byteBuffer.putDouble(_position++ << 3, c);
		return this;
	}

	public override DoubleBuffer put(int index, double c)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		byteBuffer.putDouble(index << 3, c);
		return this;
	}

	public override DoubleBuffer slice()
	{
		byteBuffer.limit(_limit << 3);
		byteBuffer.position(_position << 3);
		DoubleBuffer result = new DoubleToByteBufferAdapter(byteBuffer.slice());
		byteBuffer.clear();
		return result;
	}

}
}
