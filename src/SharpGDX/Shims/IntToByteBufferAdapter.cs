using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** This class wraps a byte buffer to be a int buffer.
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
	sealed class IntToByteBufferAdapter : IntBuffer , IByteBufferWrapper
	{
// implements DirectBuffer {

	internal static IntBuffer wrap(ByteBuffer byteBuffer)
	{
		return new IntToByteBufferAdapter(byteBuffer.slice());
	}

	private readonly ByteBuffer byteBuffer;

	IntToByteBufferAdapter(ByteBuffer byteBuffer)
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

	public override IntBuffer asReadOnlyBuffer()
	{
		IntToByteBufferAdapter buf = new IntToByteBufferAdapter(byteBuffer.asReadOnlyBuffer());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

	public override IntBuffer compact()
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

	public override IntBuffer duplicate()
	{
		IntToByteBufferAdapter buf = new IntToByteBufferAdapter(byteBuffer.duplicate());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

	public override int get()
	{
		if (_position == _limit)
		{
			throw new BufferUnderflowException();
		}
		return byteBuffer.getInt(_position++ << 2);
	}

	public override int get(int index)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return byteBuffer.getInt(index << 2);
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

	protected override int[] protectedArray()
	{
		// TODO: This is all kinds of wrong, just trying to get somewhere
		var bytes = ((DirectReadWriteByteBuffer)byteBuffer).byteArray.toArray();
		int[] result = new int[bytes.Length / sizeof(int)];
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

	public override IntBuffer put(int c)
	{
		if (_position == _limit)
		{
			throw new BufferOverflowException();
		}
		byteBuffer.putInt(_position++ << 2, c);
		return this;
	}

	public override IntBuffer put(int index, int c)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		byteBuffer.putInt(index << 2, c);
		return this;
	}

	public override IntBuffer slice()
	{
		byteBuffer.limit(_limit << 2);
		byteBuffer.position(_position << 2);
		IntBuffer result = new IntToByteBufferAdapter(byteBuffer.slice());
		byteBuffer.clear();
		return result;
	}

	public ByteBuffer getByteBuffer()
	{
		return byteBuffer;
	}

}
}
