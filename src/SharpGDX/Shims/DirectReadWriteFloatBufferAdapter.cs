using SharpGDX.Utils;
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
sealed class DirectReadWriteFloatBufferAdapter : FloatBuffer , HasArrayBufferView {
// implements DirectBuffer {

	internal static FloatBuffer wrap (DirectReadWriteByteBuffer byteBuffer) {
		return new DirectReadWriteFloatBufferAdapter((DirectReadWriteByteBuffer)byteBuffer.slice());
	}

	private readonly DirectReadWriteByteBuffer byteBuffer;
	private readonly Float32Array floatArray;

	DirectReadWriteFloatBufferAdapter (DirectReadWriteByteBuffer byteBuffer) 
	: base((byteBuffer.capacity() >> 2))
	{
		
		this.byteBuffer = byteBuffer;
		this.byteBuffer.clear();
			//this.floatArray = new Float32Array(byteBuffer.byteArray.buffer(), byteBuffer.byteArray.byteOffset(), capacity);

			// TODO: Not sure if this stays or not. -RP
			var data = byteBuffer.byteArray.buffer();
			float[] sdata = new float[_capacity];
			var g = byteBuffer.byteArray.byteOffset();
			System.Buffer.BlockCopy(data, byteBuffer.byteArray.byteOffset(), sdata, 0, _capacity);
			floatArray = new Float32Array(sdata);
		}

	// TODO(haustein) This will be slow
	public override FloatBuffer asReadOnlyBuffer ()
	{
		throw new NotImplementedException();
		//DirectReadOnlyFloatBufferAdapter buf = new DirectReadOnlyFloatBufferAdapter(byteBuffer);
		//buf.limit = _limit;
		//buf.position = _position;
		//buf.mark = mark;
		//return buf;
	}

	public override FloatBuffer compact () {
		byteBuffer.limit(_limit << 2);
		byteBuffer.position(_position << 2);
		byteBuffer.compact();
		byteBuffer.clear();
		_position = _limit - _position;
		_limit = _capacity;
		_mark = UNSET_MARK;
		return this;
	}

	public override FloatBuffer duplicate () {
		DirectReadWriteFloatBufferAdapter buf = new DirectReadWriteFloatBufferAdapter(
			(DirectReadWriteByteBuffer)byteBuffer.duplicate());
		buf._limit = _limit;
		buf._position = _position;
		buf._mark = _mark;
		return buf;
	}

	public override float get () {
// if (position == limit) {
// throw new BufferUnderflowException();
// }
		return floatArray.get(_position++);
	}

	public override float get (int index) {
// if (index < 0 || index >= limit) {
// throw new IndexOutOfBoundsException();
// }
		return floatArray.get(index);
	}

	public override bool isDirect () {
		return true;
	}

	public override bool isReadOnly () {
		return false;
	}

	public override ByteOrder order () {
		return byteBuffer.order();
	}

		protected override float[] protectedArray () {
			// TODO: Not sure about this.
			return floatArray.items;
			throw new UnsupportedOperationException();
		}

	protected override int protectedArrayOffset () {
			throw new UnsupportedOperationException();
		}

	protected override bool protectedHasArray () {
		return false;
	}

	public override FloatBuffer put (float c) {
// if (position == limit) {
// throw new BufferOverflowException();
// }
		floatArray.set(_position++, c);
		return this;
	}

	public override FloatBuffer put (int index, float c) {
// if (index < 0 || index >= limit) {
// throw new IndexOutOfBoundsException();
// }
		floatArray.set(index, c);
		return this;
	}

	public override FloatBuffer slice () {
		byteBuffer.limit(_limit << 2);
		byteBuffer.position(_position << 2);
		FloatBuffer result = new DirectReadWriteFloatBufferAdapter((DirectReadWriteByteBuffer)byteBuffer.slice());
		byteBuffer.clear();
		return result;
	}

	//public ArrayBufferView getTypedArray () {
	//	return floatArray;
	//}

	public int getElementSize () {
		return 4;
	}
}
}
