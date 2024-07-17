using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/** DoubleArrayBuffer, ReadWriteDoubleArrayBuffer and ReadOnlyDoubleArrayBuffer compose the implementation of array based double
 * buffers.
 * <p>
 * DoubleArrayBuffer implements all the shared readonly methods and is extended by the other two classes.
 * </p>
 * <p>
 * All methods are marked final for runtime performance.
 * </p>
 */
	abstract class DoubleArrayBuffer : DoubleBuffer
	{

	internal protected readonly double[] backingArray;

internal	protected readonly int offset;

	protected DoubleArrayBuffer(double[] array)
			: this(array.Length, array, 0)
		{
		
	}

	protected DoubleArrayBuffer(int capacity)
			: this(capacity, new double[capacity], 0)
		{
		
	}

	protected DoubleArrayBuffer(int capacity, double[] backingArray, int offset)
			: base(capacity)
		{		
		this.backingArray = backingArray;
		this.offset = offset;
	}

	public override double get()
	{
		if (_position == _limit)
		{
			throw new BufferUnderflowException();
		}
		return backingArray[offset + _position++];
	}

	public override double get(int index)
	{
		if (index < 0 || index >= _limit)
		{
			throw new IndexOutOfBoundsException();
		}
		return backingArray[offset + index];
	}

	public override DoubleBuffer get(double[] dest, int off, int len)
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

	public override  bool isDirect()
	{
		return false;
	}

	public override ByteOrder order()
	{
		return ByteOrder.nativeOrder();
	}

	}
}
