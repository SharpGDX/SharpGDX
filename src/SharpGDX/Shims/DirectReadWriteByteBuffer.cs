namespace SharpGDX.Shims
{
	/** DirectByteBuffer, DirectReadWriteByteBuffer and DirectReadOnlyByteBuffer compose the implementation of direct byte buffers.
 * <p>
 * DirectReadWriteByteBuffer extends DirectByteBuffer with all the write methods.
 * </p>
 * <p>
 * This class is marked final for runtime performance.
 * </p>
 */
	 sealed class DirectReadWriteByteBuffer : DirectByteBuffer
	{

		//static DirectReadWriteByteBuffer copy(DirectByteBuffer other, int markOfOther)
		//{
		//	DirectReadWriteByteBuffer buf = new DirectReadWriteByteBuffer(other.byteArray.buffer(), other.capacity(),
		//		other.byteArray.byteOffset());
		//	buf._limit = other.limit();
		//	buf._position = other.position();
		//	buf._mark = markOfOther;
		//	buf.order(other.order());
		//	return buf;
		//}

		//DirectReadWriteByteBuffer(ArrayBuffer backingArray)
		//: base(backingArray)
		//{
			
		//}

		public DirectReadWriteByteBuffer(int capacity)
		: base(capacity)
		{
			
		}

		DirectReadWriteByteBuffer(byte[] backingArray, int capacity, int arrayOffset)
		: base(backingArray, capacity, arrayOffset)
		{

		}

		public override FloatBuffer asFloatBuffer()
		{
			return DirectReadWriteFloatBufferAdapter.wrap(this);
		}

		public override IntBuffer asIntBuffer()
		{

			// TODO: return order() == ByteOrder.nativeOrder() ? DirectReadWriteIntBufferAdapter.wrap(this) :
				return base.asIntBuffer();
		}

		public override ShortBuffer asShortBuffer()
		{
			return order() == ByteOrder.nativeOrder() ? DirectReadWriteShortBufferAdapter.wrap(this) : base.asShortBuffer();
		}

		public override ByteBuffer asReadOnlyBuffer()
		{
			throw new NotImplementedException();
			//return DirectReadOnlyByteBuffer.copy(this, mark);
		}

		public override ByteBuffer compact()
		{
			// System.arraycopy(backingArray, position + offset, backingArray, offset,
			// remaining());

			int rem = remaining();
			for (int i = 0; i < rem; i++)
			{
				byteArray.set(i, byteArray.get(_position + i));
			}

			_position = _limit - _position;
			_limit = _capacity;
			_mark = UNSET_MARK;
			return this;
		}

		public override ByteBuffer duplicate()
		{
			throw new NotImplementedException();
			//return copy(this, _mark);
		}

		public override bool isReadOnly()
		{
			return false;
		}

		protected override byte[] protectedArray()
		{
			// TODO: Not sure if this stays. -RP

			byte[] unsigned = new byte[this.byteArray.items.Length];
			System.Buffer.BlockCopy(this.byteArray.items, 0, unsigned, 0, this.byteArray.items.Length);
			return unsigned;
			//throw new UnsupportedOperationException();
		}

		protected override int protectedArrayOffset()
		{
			throw new UnsupportedOperationException();
		}

		protected override bool protectedHasArray()
		{
			return true;
		}

		public override ByteBuffer put(byte b)
		{
			// if (position == limit) {
			// throw new BufferOverflowException();
			// }
			// TODO: I added the sbyte cast.
			byteArray.set(_position++, b);
			return this;
		}

		public override ByteBuffer put(int index, byte b)
		{
			// if (index < 0 || index >= limit) {
			// throw new IndexOutOfBoundsException();
			// }
			// TODO: I added the sbyte cast.
			byteArray.set(index, b);
			return this;
		}

		/*
		 * Override ByteBuffer.put(byte[], int, int) to improve performance.
		 * 
		 * (non-Javadoc)
		 * 
		 * @see java.nio.ByteBuffer#put(byte[], int, int)
		 */
		public override ByteBuffer put(byte[] src, int off, int len)
		{
			if (off < 0 || len < 0 || (long)off + (long)len > src.Length)
			{
				throw new IndexOutOfBoundsException();
			}
			if (len > remaining())
			{
				throw new BufferOverflowException();
			}
			if (isReadOnly())
			{
				throw new ReadOnlyBufferException();
			}
			for (int i = 0; i < len; i++)
			{
				// TODO: I added the sbyte cast.
				byteArray.set(i + _position, src[off + i]);
			}
			_position += len;
			return this;
		}

		public override ByteBuffer putDouble(double value)
		{
			return putLong(Numbers.doubleToRawLongBits(value));
		}

		public override ByteBuffer putDouble(int index, double value)
		{
			return putLong(index, Numbers.doubleToRawLongBits(value));
		}

		public override ByteBuffer putFloat(float value)
		{
			return putInt(Numbers.floatToIntBits(value));
		}

		public override ByteBuffer putFloat(int index, float value)
		{
			return putInt(index, Numbers.floatToIntBits(value));
		}

		public override ByteBuffer putInt(int value)
		{
			int newPosition = _position + 4;
			// if (newPosition > limit) {
			// throw new BufferOverflowException();
			// }
			store(_position, value);
			_position = newPosition;
			return this;
		}

		public override ByteBuffer putInt(int index, int value)
		{
			// if (index < 0 || (long)index + 4 > limit) {
			// throw new IndexOutOfBoundsException();
			// }
			store(index, value);
			return this;
		}

		public override ByteBuffer putLong(int index, long value)
		{
			// if (index < 0 || (long)index + 8 > limit) {
			// throw new IndexOutOfBoundsException();
			// }
			store(index, value);
			return this;
		}

		public override ByteBuffer putLong(long value)
		{
			int newPosition =_position + 8;
			// if (newPosition > limit) {
			// throw new BufferOverflowException();
			// }
			store(_position, value);
			_position = newPosition;
			return this;
		}

		public override ByteBuffer putShort(int index, short value)
		{
			// if (index < 0 || (long)index + 2 > limit) {
			// throw new IndexOutOfBoundsException();
			// }
			store(index, value);
			return this;
		}

		public override ByteBuffer putShort(short value)
		{
			int newPosition = _position + 2;
			// if (newPosition > limit) {
			// throw new BufferOverflowException();
			// }
			store(_position, value);
			_position = newPosition;
			return this;
		}

		public override ByteBuffer slice()
		{
			DirectReadWriteByteBuffer slice = new DirectReadWriteByteBuffer(byteArray.buffer(), remaining(),
				byteArray.byteOffset() + _position);
			slice._order = _order;
			return slice;
		}
	}
}
