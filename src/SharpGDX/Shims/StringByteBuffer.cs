namespace SharpGDX.Shims
{
	class StringByteBuffer : BaseByteBuffer
	{

	private String s;

	internal StringByteBuffer(String s)
	: base(s.Length)
		{
		
		this.s = s;
		order(ByteOrder.LITTLE_ENDIAN);
	}

	internal StringByteBuffer(String s, int position, int limit)
	: this(s)
		{
		this._position = position;
		this._limit = limit;
	}

	public override ByteBuffer asReadOnlyBuffer()
	{
		return this;
	}

	protected override byte[] protectedArray()
	{
		throw new UnsupportedOperationException();
	}

	protected override int protectedArrayOffset()
	{
		throw new UnsupportedOperationException();
	}

	protected override bool protectedHasArray()
	{
		throw new UnsupportedOperationException();
	}

		public override ByteBuffer compact()
	{
		return this;
	}

	public override ByteBuffer duplicate()
	{
		return this;
	}

		public override byte get()
	{
		return get(_position++);
	}

		public override byte get(int index)
	{
		return get(s, index);
	}

		public override double getDouble()
	{
		return Numbers.longBitsToDouble(getLong());
	}

		public override double getDouble(int index)
	{
		return Numbers.longBitsToDouble(getLong(index));
	}

		public override float getFloat()
	{
		return Numbers.intBitsToFloat(getInt());
	}

		public override float getFloat(int index)
	{
		return Numbers.intBitsToFloat(getInt(index));
	}

		public override int getInt()
	{
		int newPosition = _position + 4;
		int result = loadInt(_position);
		_position = newPosition;
		return result;
	}

		public override int getInt(int index)
	{
		return loadInt(index);
	}

		public override long getLong()
	{
		throw new UnsupportedOperationException();
	}

		public override long getLong(int index)
	{
		throw new UnsupportedOperationException();
	}

		public override short getShort()
	{
		int newPosition = _position + 2;
		short result = loadShort(_position);
		_position = newPosition;
		return result;
	}

		public override short getShort(int index)
	{
		return loadShort(index);
	}

		public override bool isDirect()
	{
		return false;
	}

		public override ByteBuffer put(byte b)
	{
		throw new UnsupportedOperationException();
	}

		public override ByteBuffer put(int index, byte b)
	{
		throw new UnsupportedOperationException();
	}

	public override ByteBuffer putDouble(double value)
	{
		throw new UnsupportedOperationException();
	}

	public override ByteBuffer putDouble(int index, double value)
	{
		throw new UnsupportedOperationException();
	}

		public override ByteBuffer putFloat(float value)
	{
		throw new UnsupportedOperationException();
	}

		public override ByteBuffer putFloat(int index, float value)
	{
		throw new UnsupportedOperationException();
	}

	public override ByteBuffer putInt(int value)
	{
		throw new UnsupportedOperationException();
	}

	public override ByteBuffer putInt(int index, int value)
	{
		throw new UnsupportedOperationException();
	}

		public override ByteBuffer putLong(long value)
	{
		throw new UnsupportedOperationException();
	}

		public override ByteBuffer putLong(int index, long value)
	{
		throw new UnsupportedOperationException();
	}

		public override ByteBuffer putShort(short value)
	{
		throw new UnsupportedOperationException();
	}

		public override ByteBuffer putShort(int index, short value)
	{
		throw new UnsupportedOperationException();
	}

	public override ByteBuffer slice()
	{
		// TODO(jgw): I don't think this is right, but might work for our purposes.
		StringByteBuffer slice = new StringByteBuffer(s, _position, _limit);
		slice._order = _order;
		return slice;
	}

	public override bool isReadOnly()
	{
		return true;
	}

	private extern byte get(String s, int i) /*-{
															var x = s.charCodeAt(i) & 0xff;
															if (x > 127) x -= 256;
															return x;
															}-*/;

	protected  int loadInt(int baseOffset)
	{
		int bytes = 0;
		for (int i = 3; i >= 0; i--)
		{
			bytes = bytes << 8;
			bytes = bytes | (get(baseOffset + i) & 0xFF);
		}
		return bytes;
	}

	protected  short loadShort(int baseOffset)
	{
		short bytes = 0;
		bytes = (short)(get(baseOffset + 1) << 8);

		// TODO: Not sure if this is 100% correct -RP
		bytes |= (short)(get(baseOffset) & 0xFF);
		return bytes;
	}
}
}
