using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
    public class ByteArrayInputStream : InputStream {
    private static readonly int MAX_TRANSFER_SIZE = 128*1024;

    protected byte[] buf;
    protected int pos;
    protected int _mark = 0;
    protected int count;

    public ByteArrayInputStream(byte[] buf) {
        this.buf = buf;
        this.pos = 0;
        this.count = buf.Length;
    }

    public ByteArrayInputStream(byte[] buf, int offset, int length) {
        this.buf = buf;
        this.pos = offset;
        this.count = Math.Min(offset + length, buf.Length);
        this._mark = offset;
    }

    public override int read() {
        lock (this)
        {
            return (pos < count) ? (buf[pos++] & 0xff) : -1;
        }
    }

    public override int read(byte[] b, int off, int len) {
        lock (this)
        {
            // TODO: Objects.checkFromIndexSize(off, len, b.Length);

            if (pos >= count)
            {
                return -1;
            }

            int avail = count - pos;
            if (len > avail)
            {
                len = avail;
            }

            if (len <= 0)
            {
                return 0;
            }

            Array.Copy(buf, pos, b, off, len);
            pos += len;
            return len;
        }
    }

    
    public override byte[] readAllBytes() {
        byte[] result = Arrays.copyOfRange(buf, pos, count);
        pos = count;
        return result;
    }

    public override int readNBytes(byte[] b, int off, int len) {
        int n = read(b, off, len);
        return n == -1 ? 0 : n;
    }

    public override long transferTo(OutputStream @out)
    {
        lock (this)
        {
            int len = count - pos;
            if (len > 0)
            {
                // 'tmpbuf' is null if and only if 'out' is trusted
                byte[] tmpbuf;
                Type outClass = @out.GetType();
                if (outClass == typeof(ByteArrayOutputStream) ||
                outClass == typeof(FileOutputStream)// ||
                // TODO: outClass == typeof(PipedOutputStream)
                )
                tmpbuf = null;
                else
                tmpbuf = new byte[Math.Min(len, MAX_TRANSFER_SIZE)];

                int nwritten = 0;
                while (nwritten < len)
                {
                    int nbyte = Math.Min(len - nwritten, MAX_TRANSFER_SIZE);
                    // if 'out' is not trusted, transfer via a temporary buffer
                    if (tmpbuf != null)
                    {
                        Array.Copy(buf, pos, tmpbuf, 0, nbyte);
                           @out.write(tmpbuf, 0, nbyte);
                    }
                    else
                        @out.write(buf, pos, nbyte);
                    pos += nbyte;
                    nwritten += nbyte;
                }

                System.Diagnostics.Debug.Assert(pos == count);
            }

            return len;
        }
    }

    /**
     * Skips {@code n} bytes of input from this input stream. Fewer
     * bytes might be skipped if the end of the input stream is reached.
     * The actual number {@code k}
     * of bytes to be skipped is equal to the smaller
     * of {@code n} and  {@code count-pos}.
     * The value {@code k} is added into {@code pos}
     * and {@code k} is returned.
     *
     * @param   n   {@inheritDoc}
     * @return  the actual number of bytes skipped.
     */
    public override long skip(long n) {
        lock (this)
        {
            long k = count - pos;
            if (n < k)
            {
                k = n < 0 ? 0 : n;
            }

            pos += (int)k;
            return k;
        }
    }

    /**
     * Returns the number of remaining bytes that can be read (or skipped over)
     * from this input stream.
     * <p>
     * The value returned is {@code count - pos},
     * which is the number of bytes remaining to be read from the input buffer.
     *
     * @return  the number of remaining bytes that can be read (or skipped
     *          over) from this input stream without blocking.
     */
    public override int available() {
        lock(this)
        return count - pos;
    }

    /**
     * Tests if this {@code InputStream} supports mark/reset.
     * @implSpec
     * The {@code markSupported} method of {@code ByteArrayInputStream}
     * always returns {@code true}.
     * @return true
     * @since   1.1
     */
    public override bool markSupported() {
        return true;
    }

    /**
     * Set the current marked position in the stream.
     * ByteArrayInputStream objects are marked at position zero by
     * default when constructed.  They may be marked at another
     * position within the buffer by this method.
     * <p>
     * If no mark has been set, then the value of the mark is the
     * offset passed to the constructor (or 0 if the offset was not
     * supplied).
     *
     * <p> Note: The {@code readAheadLimit} for this class
     *  has no meaning.
     *
     * @since   1.1
     */
    public override void mark(int readAheadLimit) {
        _mark = pos;
    }

    /**
     * Resets the buffer to the marked position.  The marked position
     * is 0 unless another position was marked or an offset was specified
     * in the constructor.
     */
    public override void reset() {
        lock (this)
        {
            pos = _mark;
        }
    }

    /**
     * Closing a {@code ByteArrayInputStream} has no effect. The methods in
     * this class can be called after the stream has been closed without
     * generating an {@code IOException}.
     */
    public override void close()  {
    }
}
}
