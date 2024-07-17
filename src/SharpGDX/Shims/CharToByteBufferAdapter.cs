using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Shims
{
	/** This class wraps a byte buffer to be a char buffer.
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
	sealed class CharToByteBufferAdapter : CharBuffer
	{ // implements DirectBuffer {

	internal static CharBuffer wrap(ByteBuffer byteBuffer)
	{
		return new CharToByteBufferAdapter(byteBuffer.slice());
	}

	private readonly ByteBuffer byteBuffer;

	CharToByteBufferAdapter(ByteBuffer byteBuffer)
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

	public override CharBuffer asReadOnlyBuffer()
	{
		CharToByteBufferAdapter buf = new CharToByteBufferAdapter(byteBuffer.asReadOnlyBuffer());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

		public override CharBuffer compact()
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

		public override CharBuffer duplicate()
	{
		CharToByteBufferAdapter buf = new CharToByteBufferAdapter(byteBuffer.duplicate());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

	public override char get()
	{
		if (_position == _limit)
		{
			throw new BufferUnderflowException();
		}
		return byteBuffer.getChar(_position++ << 1);
	}

		public override char get(int index)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return byteBuffer.getChar(index << 1);
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

	protected override char[] protectedArray()
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

		public override CharBuffer put(char c)
	{
		if (_position == _limit)
		{
			throw new BufferOverflowException();
		}
		byteBuffer.putChar(_position++ << 1, c);
		return this;
	}

	public override CharBuffer put(int index, char c)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		byteBuffer.putChar(index << 1, c);
		return this;
	}

	public override CharBuffer slice()
	{
		byteBuffer.limit(_limit << 1);
		byteBuffer.position(_position << 1);
		CharBuffer result = new CharToByteBufferAdapter(byteBuffer.slice());
		byteBuffer.clear();
		return result;
	}

	public override string subSequence(int start, int end)
	{
		if (start < 0 || end < start || end > remaining())
		{
			throw new IndexOutOfBoundsException();
		}

		CharBuffer result = duplicate();
		result.limit(_position + end);
		result.position(_position + start);
		return result.ToString();
	}
}
}
