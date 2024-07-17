using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/** This class wraps a char sequence to be a char buffer.
 * <p>
 * Implementation notice:
 * <ul>
 * <li>Char sequence based buffer is always readonly.</li>
 * </ul>
 * </p>
 */
	sealed class CharSequenceAdapter : CharBuffer
	{

	static CharSequenceAdapter copy(CharSequenceAdapter other)
	{
		CharSequenceAdapter buf = new CharSequenceAdapter(other.sequence);
		buf._limit = other._limit;
		buf._position = other._position;
		buf._mark = other._mark;
		return buf;
	}

	readonly string sequence;

internal CharSequenceAdapter(string chseq)
	: base(chseq.Length)
		{
		
		sequence = chseq;
	}

	public override CharBuffer asReadOnlyBuffer()
	{
		return duplicate();
	}

		public override CharBuffer compact()
	{
		throw new ReadOnlyBufferException();
	}

		public override CharBuffer duplicate()
	{
		return copy(this);
	}

		public override char get()
	{
		if (_position == _limit)
		{
			throw new BufferUnderflowException();
		}
		return sequence[_position++];
	}

		public override char get(int index)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return sequence[index];
	}

	public override CharBuffer get(char[] dest, int off, int len)
	{
		int length = dest.Length;
		if ((off < 0) || (len < 0) || (long)off + (long)len > length)
		{
			throw new IndexOutOfBoundsException();
		}
		if (len > remaining())
		{
			throw new BufferUnderflowException();
		}
		int newPosition = _position + len;
		// TODO: Verify !!! sequence.ToString().getChars(position, newPosition, dest, off);
		sequence.CopyTo(_position, dest, protectedArrayOffset(), newPosition);
		_position = newPosition;
		return this;
	}
		public override bool isDirect()
		{
		return false;
	}

		public override bool isReadOnly()
	{
		return true;
	}

		public override ByteOrder order()
	{
		return ByteOrder.nativeOrder();
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
		throw new ReadOnlyBufferException();
	}

		public override CharBuffer put(int index, char c)
	{
		throw new ReadOnlyBufferException();
	}

		public override CharBuffer put(char[] src, int off, int len)
	{
		if ((off < 0) || (len < 0) || (long)off + (long)len > src.Length)
		{
			throw new IndexOutOfBoundsException();
		}

		if (len > remaining())
		{
			throw new BufferOverflowException();
		}

		throw new ReadOnlyBufferException();
	}

		public override CharBuffer put(String src, int start, int end)
	{
		if ((start < 0) || (end < 0) || (long)start + (long)end > src.Length)
		{
			throw new IndexOutOfBoundsException();
		}
		throw new ReadOnlyBufferException();
	}

		public override CharBuffer slice()
	{
		return new CharSequenceAdapter(sequence.Substring(_position, _limit));
	}

		public override string subSequence(int start, int end)
	{
		if (end < start || start < 0 || end > remaining())
		{
			throw new IndexOutOfBoundsException();
		}

		CharSequenceAdapter result = copy(this);
		result._position = _position + start;
		result._limit = _position + end;
		return result.ToString();
	}
}
}
