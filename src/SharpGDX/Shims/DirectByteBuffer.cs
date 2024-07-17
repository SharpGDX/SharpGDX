using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/** DirectByteBuffer, DirectReadWriteByteBuffer and DirectReadOnlyHeapByteBuffer compose the implementation of direct byte
 * buffers.
 * <p>
 * DirectByteBuffer implements all the shared readonly methods and is extended by the other two classes.
 * </p>
 * <p>
 * All methods are marked final for runtime performance.
 * </p>
 */
	abstract class DirectByteBuffer : BaseByteBuffer, HasArrayBufferView
	{
		// TODO: this might need to actually be a 'UInt8Array'.
		internal Int8Array byteArray;

		protected DirectByteBuffer(int capacity)
		: this(new byte[capacity], capacity, 0)
		{

		}

		protected DirectByteBuffer(byte[] buf)
		: this(buf, 1, 0)
		//: this(buf, buf.byteLength(), 0)
		{

		}

		protected DirectByteBuffer(byte[] buffer,
				   int capacity, int offset)
		: base(capacity)
		{
			// TODO: Should this be ordered?
			byteArray = new Int8Array(true, buffer, offset, capacity);
			//byteArray = Int8Array.create(buffer, offset, capacity);
			//byteArray = new Int8Array(capacity);
		}

		//public ArrayBufferView getTypedArray()
		//{
		//	return byteArray;
		//}

		public int getElementSize()
		{
			return 1;
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

			for (int i = 0; i < len; i++)
			{
				dest[i + off] = get(_position + i);
			}

			_position += len;
			return this;
		}

		public override byte get()
		{
			// if (position == limit) {
			// throw new BufferUnderflowException();
			// }
			return (byte)byteArray.get(_position++);
		}

		public override byte get(int index)
		{
			// if (index < 0 || index >= limit) {
			// throw new IndexOutOfBoundsException();
			// }
			return (byte)byteArray.get(index);
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
			// if (newPosition > limit) {
			// throw new BufferUnderflowException();
			// }
			int result = loadInt(_position);
			_position = newPosition;
			return result;
		}

		public override int getInt(int index)
		{
			// if (index < 0 || index + 4 > limit) {
			// throw new IndexOutOfBoundsException();
			// }
			return loadInt(index);
		}

		public override long getLong()
		{
			int newPosition = _position + 8;
			// if (newPosition > limit) {
			// throw new BufferUnderflowException();
			// }
			long result = loadLong(_position);
			_position = newPosition;
			return result;
		}

		public override long getLong(int index)
		{
			// if (index < 0 || index + 8 > limit) {
			// throw new IndexOutOfBoundsException();
			// }
			return loadLong(index);
		}

		public override short getShort()
		{
			int newPosition = _position + 2;
			// if (newPosition > limit) {
			// throw new BufferUnderflowException();
			// }
			short result = loadShort(_position);
			_position = newPosition;
			return result;
		}

		public override short getShort(int index)
		{
			// if (index < 0 || index + 2 > limit) {
			// throw new IndexOutOfBoundsException();
			// }
			return loadShort(index);
		}

		public override bool isDirect()
		{
			return false;
		}

		protected int loadInt(int baseOffset)
		{
			int bytes = 0;
			if (_order == Endianness.BIG_ENDIAN)
			{
				for (int i = 0; i < 4; i++)
				{
					bytes = bytes << 8;
					bytes = bytes | (byteArray.get(baseOffset + i) & 0xFF);
				}
			}
			else
			{
				for (int i = 3; i >= 0; i--)
				{
					bytes = bytes << 8;
					bytes = bytes | (byteArray.get(baseOffset + i) & 0xFF);
				}
			}
			return bytes;
		}

		protected long loadLong(int baseOffset)
		{
			long bytes = 0;
			if (_order == Endianness.BIG_ENDIAN)
			{
				for (int i = 0; i < 8; i++)
				{
					bytes = bytes << 8;
					bytes = bytes | (byteArray.get(baseOffset + i) & 0xFF);
				}
			}
			else
			{
				for (int i = 7; i >= 0; i--)
				{
					bytes = bytes << 8;
					bytes = bytes | (byteArray.get(baseOffset + i) & 0xFF);
				}
			}
			return bytes;
		}

		protected short loadShort(int baseOffset)
		{
			short bytes = 0;
			if (_order == Endianness.BIG_ENDIAN)
			{
				bytes = (short)(byteArray.get(baseOffset) << 8);
				bytes |= (short)(byteArray.get(baseOffset + 1) & 0xFF);
			}
			else
			{
				bytes = (short)(byteArray.get(baseOffset + 1) << 8);
				bytes |= (short)(byteArray.get(baseOffset) & 0xFF);
			}
			return bytes;
		}

		protected void store(int baseOffset, int value)
		{
			if (_order == Endianness.BIG_ENDIAN)
			{
				for (int i = 3; i >= 0; i--)
				{
					byteArray.set(baseOffset + i, (byte)(value & 0xFF));
					value = value >> 8;
				}
			}
			else
			{
				for (int i = 0; i <= 3; i++)
				{
					byteArray.set(baseOffset + i, (byte)(value & 0xFF));
					value = value >> 8;
				}
			}
		}

		protected void store(int baseOffset, long value)
		{
			if (_order == Endianness.BIG_ENDIAN)
			{
				for (int i = 7; i >= 0; i--)
				{
					byteArray.set(baseOffset + i, (byte)(value & 0xFF));
					value = value >> 8;
				}
			}
			else
			{
				for (int i = 0; i <= 7; i++)
				{
					byteArray.set(baseOffset + i, (byte)(value & 0xFF));
					value = value >> 8;
				}
			}
		}

		protected void store(int baseOffset, short value)
		{
			if (_order == Endianness.BIG_ENDIAN)
			{
				byteArray.set(baseOffset, (byte)((value >> 8) & 0xFF));
				byteArray.set(baseOffset + 1, (byte)(value & 0xFF));
			}
			else
			{
				byteArray.set(baseOffset + 1, (byte)((value >> 8) & 0xFF));
				byteArray.set(baseOffset, (byte)(value & 0xFF));
			}
		}
	}
}
