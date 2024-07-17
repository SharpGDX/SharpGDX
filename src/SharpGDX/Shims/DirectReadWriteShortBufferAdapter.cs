using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Utils;
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
	sealed class DirectReadWriteShortBufferAdapter : ShortBuffer , HasArrayBufferView
	{
// implements DirectBuffer {

	internal static ShortBuffer wrap(DirectReadWriteByteBuffer byteBuffer)
	{
		return new DirectReadWriteShortBufferAdapter((DirectReadWriteByteBuffer)byteBuffer.slice());
	}

	private readonly DirectReadWriteByteBuffer byteBuffer;
	private readonly ShortArray shortArray;

	DirectReadWriteShortBufferAdapter(DirectReadWriteByteBuffer byteBuffer)
	: base((byteBuffer.capacity() >> 1))
		{
		
		this.byteBuffer = byteBuffer;
		//this.byteBuffer.clear();
			// this.shortArray = new ShortArray(byteBuffer.byteArray.buffer(), byteBuffer.byteArray.byteOffset(), capacity);

			// TODO: Not sure if this stays or not. -RP
			var data = byteBuffer.byteArray.buffer();
			short[] sdata = new short[_capacity];
			System.Buffer.BlockCopy(data, byteBuffer.byteArray.byteOffset(), sdata, 0, _capacity);
			shortArray = new ShortArray(sdata);
		}

	// TODO(haustein) This will be slow
	public override ShortBuffer asReadOnlyBuffer()
	{
		// TODO: Implement
		throw new NotImplementedException();
		//DirectReadOnlyShortBufferAdapter buf = new DirectReadOnlyShortBufferAdapter(byteBuffer);
		//buf.limit = limit;
		//buf.position = position;
		//buf.mark = mark;
		//return buf;
	}

	public override ShortBuffer compact()
	{
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
		DirectReadWriteShortBufferAdapter buf = new DirectReadWriteShortBufferAdapter(
			(DirectReadWriteByteBuffer)byteBuffer.duplicate());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

	public override short get()
	{
		// if (position == limit) {
		// throw new BufferUnderflowException();
		// }
		return (short)shortArray.get(_position++);
	}

	public override short get(int index)
	{
		// if (index < 0 || index >= limit) {
		// throw new IndexOutOfBoundsException();
		// }
		return (short)shortArray.get(index);
	}

	public override bool isDirect()
	{
		return true;
	}

	public override bool isReadOnly()
	{
		return false;
	}

	public override ByteOrder order()
	{
		return byteBuffer.order();
	}

	protected override short[] protectedArray()
	{
		// TODO: Not sure about this.
		return shortArray.items;
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

	public override ShortBuffer put(short c)
	{
		// if (position == limit) {
		// throw new BufferOverflowException();
		// }
		shortArray.set(_position++, c);
		return this;
	}

	public override ShortBuffer put(int index, short c)
	{
		// if (index < 0 || index >= limit) {
		// throw new IndexOutOfBoundsException();
		// }
		shortArray.set(index, c);
		return this;
	}

	public override ShortBuffer slice()
	{
		byteBuffer.limit(_limit << 1);
		byteBuffer.position(_position << 1);
		ShortBuffer result = new DirectReadWriteShortBufferAdapter((DirectReadWriteByteBuffer)byteBuffer.slice());
		byteBuffer.clear();
		return result;
	}

	// TODO: public ArrayBufferView getTypedArray()
	//{
	//	return shortArray;
	//}

	public int getElementSize()
	{
		return 2;
	}

}
}
