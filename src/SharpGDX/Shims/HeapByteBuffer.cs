using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/** HeapByteBuffer, ReadWriteHeapByteBuffer and ReadOnlyHeapByteBuffer compose the implementation of array based byte buffers.
 * <p>
 * HeapByteBuffer implements all the shared readonly methods and is extended by the other two classes.
 * </p>
 * <p>
 * All methods are marked final for runtime performance.
 * </p>
 */
	abstract class HeapByteBuffer : BaseByteBuffer
	{

	internal protected readonly byte[] backingArray;

	internal protected readonly int offset;

protected	HeapByteBuffer(byte[] backingArray)
			: this(backingArray, backingArray.Length, 0)
		{
		
	}

	protected HeapByteBuffer(int capacity)
			: this(new byte[capacity], capacity, 0)
		{
		
	}

	protected HeapByteBuffer(byte[] backingArray, int capacity, int offset)
			: base(capacity)
		{
		
		this.backingArray = backingArray;
		this.offset = offset;

		if (offset + capacity > backingArray.Length)
		{
			throw new IndexOutOfBoundsException();
		}
	}

		/*
		 * Override ByteBuffer.get(byte[], int, int) to improve performance.
		 * 
		 * (non-Javadoc)
		 * 
		 * @see java.nio.ByteBuffer#get(byte[], int, int)
		 */
		public override ByteBuffer get(byte[] dest, int off, int len)
	{
		int length = dest.Length;
		if (off < 0 || len < 0 || (long)off + (long)len > length)
		{
			throw new IndexOutOfBoundsException();
		}
		if (len > remaining())
		{
			throw new BufferUnderflowException();
		}
		Array.Copy(backingArray, offset + _position, dest, off, len);
		_position += len;
		return this;
	}

	public override byte get()
	{
		if (_position == _limit)
		{
			throw new BufferUnderflowException();
		}
		return backingArray[offset + _position++];
	}

	public override byte get(int index)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return backingArray[offset + index];
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
		if (newPosition > _limit)
		{
			throw new BufferUnderflowException();
		}
		int result = loadInt(_position);
		_position = newPosition;
		return result;
	}

	public override int getInt(int index)
	{
		if (index < 0 || index + 4 > _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return loadInt(index);
	}

	public override long getLong()
	{
		int newPosition = _position + 8;
		if (newPosition > _limit)
		{
			throw new BufferUnderflowException();
		}
		long result = loadLong(_position);
		_position = newPosition;
		return result;
	}

	public override long getLong(int index)
	{
		if (index < 0 || index + 8 > _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return loadLong(index);
	}

	public override short getShort()
	{
		int newPosition = _position + 2;
		if (newPosition > _limit)
		{
			throw new BufferUnderflowException();
		}
		short result = loadShort(_position);
		_position = newPosition;
		return result;
	}

	public override short getShort(int index)
	{
		if (index < 0 || index + 2 > _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return loadShort(index);
	}

	public override bool isDirect()
	{
		return false;
	}

	protected int loadInt(int index)
	{
		int baseOffset = offset + index;
		int bytes = 0;
		if (_order == Endianness.BIG_ENDIAN)
		{
			for (int i = 0; i < 4; i++)
			{
				bytes = bytes << 8;
				bytes = bytes | (backingArray[baseOffset + i] & 0xFF);
			}
		}
		else
		{
			for (int i = 3; i >= 0; i--)
			{
				bytes = bytes << 8;
				bytes = bytes | (backingArray[baseOffset + i] & 0xFF);
			}
		}
		return bytes;
	}

	protected long loadLong(int index)
	{
		int baseOffset = offset + index;
		long bytes = 0;
		if (_order == Endianness.BIG_ENDIAN)
		{
			for (int i = 0; i < 8; i++)
			{
				bytes = bytes << 8;
				bytes = bytes | (backingArray[baseOffset + i] & 0xFF);
			}
		}
		else
		{
			for (int i = 7; i >= 0; i--)
			{
				bytes = bytes << 8;
				bytes = bytes | (backingArray[baseOffset + i] & 0xFF);
			}
		}
		return bytes;
	}

	protected short loadShort(int index)
	{
		int baseOffset = offset + index;
		short bytes = 0;
		if (_order == Endianness.BIG_ENDIAN)
		{
			bytes = (short)(backingArray[baseOffset] << 8);
			bytes |= (short)(backingArray[baseOffset + 1] & 0xFF);
		}
		else
		{
			bytes = (short)(backingArray[baseOffset + 1] << 8);
			bytes |= (short)(backingArray[baseOffset] & 0xFF);
		}
		return bytes;
	}

	protected void store(int index, int value)
	{
		int baseOffset = offset + index;
		if (_order == Endianness.BIG_ENDIAN)
		{
			for (int i = 3; i >= 0; i--)
			{
				backingArray[baseOffset + i] = (byte)(value & 0xFF);
				value = value >> 8;
			}
		}
		else
		{
			for (int i = 0; i <= 3; i++)
			{
				backingArray[baseOffset + i] = (byte)(value & 0xFF);
				value = value >> 8;
			}
		}
	}

	protected void store(int index, long value)
	{
		int baseOffset = offset + index;
		if (_order == Endianness.BIG_ENDIAN)
		{
			for (int i = 7; i >= 0; i--)
			{
				backingArray[baseOffset + i] = (byte)(value & 0xFF);
				value = value >> 8;
			}
		}
		else
		{
			for (int i = 0; i <= 7; i++)
			{
				backingArray[baseOffset + i] = (byte)(value & 0xFF);
				value = value >> 8;
			}
		}
	}

	protected void store(int index, short value)
	{
		int baseOffset = offset + index;
		if (_order == Endianness.BIG_ENDIAN)
		{
			backingArray[baseOffset] = (byte)((value >> 8) & 0xFF);
			backingArray[baseOffset + 1] = (byte)(value & 0xFF);
		}
		else
		{
			backingArray[baseOffset + 1] = (byte)((value >> 8) & 0xFF);
			backingArray[baseOffset] = (byte)(value & 0xFF);
		}
	}
}
}
