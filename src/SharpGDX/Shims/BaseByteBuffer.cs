using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/** Serves as the root of other byte buffer impl classes, implements common methods that can be shared by child classes. */
	abstract class BaseByteBuffer : ByteBuffer
	{

		protected BaseByteBuffer(int capacity)
			: base(capacity)
		{

		}

		public override CharBuffer asCharBuffer()
		{
			return CharToByteBufferAdapter.wrap(this);
		}

		public override DoubleBuffer asDoubleBuffer()
		{
			return DoubleToByteBufferAdapter.wrap(this);
		}

		public override FloatBuffer asFloatBuffer()
		{
			return FloatToByteBufferAdapter.wrap(this);
		}

		public override IntBuffer asIntBuffer()
		{
			return IntToByteBufferAdapter.wrap(this);
		}

		public override LongBuffer asLongBuffer()
		{
			return LongToByteBufferAdapter.wrap(this);
		}

		public override ShortBuffer asShortBuffer()
		{
			return ShortToByteBufferAdapter.wrap(this);
		}

		public override char getChar()
		{
			return (char)getShort();
		}

		public override char getChar(int index)
		{
			return (char)getShort(index);
		}

		public override ByteBuffer putChar(char value)
		{
			return putShort((short)value);
		}

		public override ByteBuffer putChar(int index, char value)
		{
			return putShort(index, (short)value);
		}
	}
}