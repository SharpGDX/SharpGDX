using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class BufferedOutputStream : FilterOutputStream {
    private static readonly int DEFAULT_INITIAL_BUFFER_SIZE = 512;
    private static readonly int DEFAULT_MAX_BUFFER_SIZE = 8192;

    // initialized to null when BufferedOutputStream is sub-classed
    private readonly object _lock;

    /**
     * The internal buffer where data is stored.
     */
    protected byte[] buf;

    /**
     * The number of valid bytes in the buffer. This value is always
     * in the range {@code 0} through {@code buf.length}; elements
     * {@code buf[0]} through {@code buf[count-1]} contain valid
     * byte data.
     */
    protected int count;

    /**
     * Max size of the internal buffer.
     */
    private readonly int maxBufSize;

    /**
     * Returns the buffer size to use when no output buffer size specified.
     */
    private static int initialBufferSize() {
        // TODO: Is this even possible in .Net? -RP
        //if (VM.isBooted() && Thread.currentThread().isVirtual()) {
        //    return DEFAULT_INITIAL_BUFFER_SIZE;
        //} else {
            return DEFAULT_MAX_BUFFER_SIZE;
        //}
    }

    /**
     * Creates a new buffered output stream.
     */
    private BufferedOutputStream(OutputStream @out, int initialSize, int maxSize) 
    : base(@out)
    {
        

        if (initialSize <= 0) {
            throw new IllegalArgumentException("Buffer size <= 0");
        }

        if (GetType() == typeof(BufferedOutputStream)) {
            // use InternalLock and resizable buffer when not sub-classed
            this._lock = new object();// InternalLock.newLockOrNull();
            this.buf = new byte[initialSize];    // resizable
        } else {
            // use monitors and no resizing when sub-classed
            this._lock = null;
            this.buf = new byte[maxSize];
        }
        this.maxBufSize = maxSize;
    }

    /**
     * Creates a new buffered output stream to write data to the
     * specified underlying output stream.
     *
     * @param   out   the underlying output stream.
     */
    public BufferedOutputStream(OutputStream @out) 
    : this(@out, initialBufferSize(), DEFAULT_MAX_BUFFER_SIZE)
    {
        
    }

    /**
     * Creates a new buffered output stream to write data to the
     * specified underlying output stream with the specified buffer
     * size.
     *
     * @param   out    the underlying output stream.
     * @param   size   the buffer size.
     * @throws  IllegalArgumentException if size &lt;= 0.
     */
    public BufferedOutputStream(OutputStream @out, int size) 
    : this(@out, size, size)
    {
        
    }

    /** Flush the internal buffer */
    private void flushBuffer() {
        if (count > 0) {
            @out.write(buf, 0, count);
            count = 0;
        }
    }

    /**
     * Grow buf to fit an additional len bytes if needed.
     * If possible, it grows by len+1 to avoid flushing when len bytes
     * are added. A no-op if the buffer is not resizable.
     *
     * This method should only be called while holding the lock.
     */
    private void growIfNeeded(int len) {
        int neededSize = count + len + 1;
        if (neededSize < 0)
            neededSize = int.MaxValue;
        int bufSize = buf.Length;
        if (neededSize > bufSize && bufSize < maxBufSize) {
            int newSize = Math.Min(neededSize, maxBufSize);
            buf = Arrays.copyOf(buf, newSize);
        }
    }

    /**
     * Writes the specified byte to this buffered output stream.
     *
     * @param      b   the byte to be written.
     * @throws     IOException  if an I/O error occurs.
     */
    public override void write(int b)
    {
        if (_lock != null)
        {
            lock (_lock)
                try
                {
                    implWrite(b);
                }
                finally
                {

                }
        }
        else
        {
            lock (this)
            {
                implWrite(b);
            }
        }
    }

    private void implWrite(int b)  {
        growIfNeeded(1);
        if (count >= buf.Length) {
            flushBuffer();
        }
        buf[count++] = (byte)b;
    }

    /**
     * Writes {@code len} bytes from the specified byte array
     * starting at offset {@code off} to this buffered output stream.
     *
     * <p> Ordinarily this method stores bytes from the given array into this
     * stream's buffer, flushing the buffer to the underlying output stream as
     * needed.  If the requested length is at least as large as this stream's
     * buffer, however, then this method will flush the buffer and write the
     * bytes directly to the underlying output stream.  Thus redundant
     * {@code BufferedOutputStream}s will not copy data unnecessarily.
     *
     * @param      b     the data.
     * @param      off   the start offset in the data.
     * @param      len   the number of bytes to write.
     * @throws     IOException  if an I/O error occurs.
     * @throws     IndexOutOfBoundsException {@inheritDoc}
     */
    public override void write(byte[] b, int off, int len)  {
        if (_lock != null) {
            lock(_lock)
            try {
                implWrite(b, off, len);
            } finally {
                
            }
        } else {
            lock (this) {
                implWrite(b, off, len);
            }
        }
    }

    private void implWrite(byte[] b, int off, int len) {
        if (len >= maxBufSize) {
            /* If the request length exceeds the max size of the output buffer,
               flush the output buffer and then write the data directly.
               In this way buffered streams will cascade harmlessly. */
            flushBuffer();
            @out.write(b, off, len);
            return;
        }
        growIfNeeded(len);
        if (len > buf.Length - count) {
            flushBuffer();
        }
        Array.Copy(b, off, buf, count, len);
        count += len;
    }

    /**
     * Flushes this buffered output stream. This forces any buffered
     * output bytes to be written out to the underlying output stream.
     *
     * @throws     IOException  if an I/O error occurs.
     * @see        java.io.FilterOutputStream#out
     */
    public override void flush()  {
        if (_lock != null) {
            lock(_lock)
            try {
                implFlush();
            } finally {
            }
        } else {
            lock (this) {
                implFlush();
            }
        }
    }

    private void implFlush() {
        flushBuffer();
        @out.flush();
    }
}

}
