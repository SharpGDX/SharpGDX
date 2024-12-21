using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
    public class PushbackInputStream : FilterInputStream {

    // initialized to null when PushbackInputStream is sub-classed
    private readonly object? closeLock;

    /**
     * The pushback buffer.
     * @since   1.1
     */
    protected byte[] buf;

    /**
     * The position within the pushback buffer from which the next byte will
     * be read.  When the buffer is empty, {@code pos} is equal to
     * {@code buf.length}; when the buffer is full, {@code pos} is
     * equal to zero.
     *
     * @since   1.1
     */
    protected int pos;

    /**
     * Check to make sure that this stream has not been closed
     */
    private void ensureOpen()  {
        if (@in == null)
            throw new IOException("Stream closed");
    }

    /**
     * Creates a {@code PushbackInputStream}
     * with a pushback buffer of the specified {@code size},
     * and saves its argument, the input stream
     * {@code in}, for later use. Initially,
     * the pushback buffer is empty.
     *
     * @param  in    the input stream from which bytes will be read.
     * @param  size  the size of the pushback buffer.
     * @throws IllegalArgumentException if {@code size <= 0}
     * @since  1.1
     */
    public PushbackInputStream(InputStream @in, int size) 
    : base(@in)
    {
        
        if (size <= 0) {
            throw new IllegalArgumentException("size <= 0");
        }
        this.buf = new byte[size];
        this.pos = size;

        // use monitors when PushbackInputStream is sub-classed
        if (GetType() == typeof(PushbackInputStream)) {
            closeLock = new object();
        } else {
            closeLock = null;
        }
    }

    /**
     * Creates a {@code PushbackInputStream}
     * with a 1-byte pushback buffer, and saves its argument, the input stream
     * {@code in}, for later use. Initially,
     * the pushback buffer is empty.
     *
     * @param   in   the input stream from which bytes will be read.
     */
    public PushbackInputStream(InputStream @in) 
    : this(@in, 1)
    {
        
    }

    /**
     * Reads the next byte of data from this input stream. The value
     * byte is returned as an {@code int} in the range
     * {@code 0} to {@code 255}. If no byte is available
     * because the end of the stream has been reached, the value
     * {@code -1} is returned. This method blocks until input data
     * is available, the end of the stream is detected, or an exception
     * is thrown.
     *
     * <p> This method returns the most recently pushed-back byte, if there is
     * one, and otherwise calls the {@code read} method of its underlying
     * input stream and returns whatever value that method returns.
     *
     * @return     the next byte of data, or {@code -1} if the end of the
     *             stream has been reached.
     * @throws     IOException  if this input stream has been closed by
     *             invoking its {@link #close()} method,
     *             or an I/O error occurs.
     * @see        java.io.InputStream#read()
     */
    public override int read()  {
        ensureOpen();
        if (pos < buf.Length) {
            return buf[pos++] & 0xff;
        }
        return base.read();
    }

    /**
     * Reads up to {@code len} bytes of data from this input stream into
     * an array of bytes.  This method first reads any pushed-back bytes; after
     * that, if fewer than {@code len} bytes have been read then it
     * reads from the underlying input stream. If {@code len} is not zero, the method
     * blocks until at least 1 byte of input is available; otherwise, no
     * bytes are read and {@code 0} is returned.
     *
     * @param      b     the buffer into which the data is read.
     * @param      off   the start offset in the destination array {@code b}
     * @param      len   the maximum number of bytes read.
     * @return     the total number of bytes read into the buffer, or
     *             {@code -1} if there is no more data because the end of
     *             the stream has been reached.
     * @throws     NullPointerException If {@code b} is {@code null}.
     * @throws     IndexOutOfBoundsException If {@code off} is negative,
     *             {@code len} is negative, or {@code len} is greater than
     *             {@code b.length - off}
     * @throws     IOException  if this input stream has been closed by
     *             invoking its {@link #close()} method,
     *             or an I/O error occurs.
     * @see        java.io.InputStream#read(byte[], int, int)
     */
    public override int read(byte[] b, int off, int len)  {
        ensureOpen();
        if (b == null) {
            throw new NullPointerException();
        }
        // TODO: Objects.checkFromIndexSize(off, len, b.length);
        if (len == 0) {
            return 0;
        }

        int avail = buf.Length - pos;
        if (avail > 0) {
            if (len < avail) {
                avail = len;
            }
            Array.Copy(buf, pos, b, off, avail);
            pos += avail;
            off += avail;
            len -= avail;
        }
        if (len > 0) {
            len = base.read(b, off, len);
            if (len == -1) {
                return avail == 0 ? -1 : avail;
            }
            return avail + len;
        }
        return avail;
    }

    /**
     * Pushes back a byte by copying it to the front of the pushback buffer.
     * After this method returns, the next byte to be read will have the value
     * {@code (byte)b}.
     *
     * @param      b   the {@code int} value whose low-order
     *                  byte is to be pushed back.
     * @throws    IOException If there is not enough room in the pushback
     *            buffer for the byte, or this input stream has been closed by
     *            invoking its {@link #close()} method.
     */
    public void unread(int b)  {
        ensureOpen();
        if (pos == 0) {
            throw new IOException("Push back buffer is full");
        }
        buf[--pos] = (byte)b;
    }

    /**
     * Pushes back a portion of an array of bytes by copying it to the front
     * of the pushback buffer.  After this method returns, the next byte to be
     * read will have the value {@code b[off]}, the byte after that will
     * have the value {@code b[off+1]}, and so forth.
     *
     * @param     b the byte array to push back.
     * @param     off the start offset of the data.
     * @param     len the number of bytes to push back.
     * @throws    NullPointerException If {@code b} is {@code null}.
     * @throws    IOException If there is not enough room in the pushback
     *            buffer for the specified number of bytes,
     *            or this input stream has been closed by
     *            invoking its {@link #close()} method.
     * @since     1.1
     */
    public void unread(byte[] b, int off, int len)  {
        ensureOpen();
        if (len > pos) {
            throw new IOException("Push back buffer is full");
        }
        pos -= len;
        Array.Copy(b, off, buf, pos, len);
    }

    /**
     * Pushes back an array of bytes by copying it to the front of the
     * pushback buffer.  After this method returns, the next byte to be read
     * will have the value {@code b[0]}, the byte after that will have the
     * value {@code b[1]}, and so forth.
     *
     * @param     b the byte array to push back
     * @throws    NullPointerException If {@code b} is {@code null}.
     * @throws    IOException If there is not enough room in the pushback
     *            buffer for the specified number of bytes,
     *            or this input stream has been closed by
     *            invoking its {@link #close()} method.
     * @since     1.1
     */
    public void unread(byte[] b)  {
        unread(b, 0, b.Length);
    }

    /**
     * Returns an estimate of the number of bytes that can be read (or
     * skipped over) from this input stream without blocking by the next
     * invocation of a method for this input stream. The next invocation might be
     * the same thread or another thread.  A single read or skip of this
     * many bytes will not block, but may read or skip fewer bytes.
     *
     * <p> The method returns the sum of the number of bytes that have been
     * pushed back and the value returned by {@link
     * java.io.FilterInputStream#available available}.
     *
     * @return     the number of bytes that can be read (or skipped over) from
     *             the input stream without blocking.
     * @throws     IOException  if this input stream has been closed by
     *             invoking its {@link #close()} method,
     *             or an I/O error occurs.
     * @see        java.io.FilterInputStream#in
     * @see        java.io.InputStream#available()
     */
    public override int available()  {
        ensureOpen();
        int n = buf.Length - pos;
        int avail = base.available();
        return n > (int.MaxValue - avail)
                    ? int.MaxValue
                    : n + avail;
    }

    /**
     * Skips over and discards {@code n} bytes of data from this
     * input stream. The {@code skip} method may, for a variety of
     * reasons, end up skipping over some smaller number of bytes,
     * possibly zero.  If {@code n} is negative, no bytes are skipped.
     *
     * <p> The {@code skip} method of {@code PushbackInputStream}
     * first skips over the bytes in the pushback buffer, if any.  It then
     * calls the {@code skip} method of the underlying input stream if
     * more bytes need to be skipped.  The actual number of bytes skipped
     * is returned.
     *
     * @param      n  {@inheritDoc}
     * @return     {@inheritDoc}
     * @throws     IOException  if the stream has been closed by
     *             invoking its {@link #close()} method,
     *             {@code in.skip(n)} throws an IOException,
     *             or an I/O error occurs.
     * @see        java.io.FilterInputStream#in
     * @see        java.io.InputStream#skip(long n)
     * @since      1.2
     */
    public override long skip(long n)  {
        ensureOpen();
        if (n <= 0) {
            return 0;
        }

        long pskip = buf.Length - pos;
        if (pskip > 0) {
            if (n < pskip) {
                pskip = n;
            }
            pos += (int) pskip;
            n -= pskip;
        }
        if (n > 0) {
            pskip += base.skip(n);
        }
        return pskip;
    }

    /**
     * Tests if this input stream supports the {@code mark} and
     * {@code reset} methods, which it does not.
     *
     * @return   {@code false}, since this class does not support the
     *           {@code mark} and {@code reset} methods.
     * @see      java.io.InputStream#mark(int)
     * @see      java.io.InputStream#reset()
     */
    public override bool markSupported() {
        return false;
    }

    /**
     * Marks the current position in this input stream.
     *
     * <p> The {@code mark} method of {@code PushbackInputStream}
     * does nothing.
     *
     * @param   readlimit   the maximum limit of bytes that can be read before
     *                      the mark position becomes invalid.
     * @see     java.io.InputStream#reset()
     */
    public override void mark(int readlimit) {
    }

    /**
     * Repositions this stream to the position at the time the
     * {@code mark} method was last called on this input stream.
     *
     * <p> The method {@code reset} for class
     * {@code PushbackInputStream} does nothing except throw an
     * {@code IOException}.
     *
     * @throws  IOException  if this method is invoked.
     * @see     java.io.InputStream#mark(int)
     * @see     java.io.IOException
     */
    public override void reset() {
        throw new IOException("mark/reset not supported");
    }

    /**
     * Closes this input stream and releases any system resources
     * associated with the stream.
     * Once the stream has been closed, further read(), unread(),
     * available(), reset(), or skip() invocations will throw an IOException.
     * Closing a previously closed stream has no effect.
     *
     * @throws     IOException  if an I/O error occurs.
     */
    public override void close()  {
        if (closeLock != null) {
            lock (closeLock)
            {
                implClose();
            }
        } else {
            lock (this) {
                implClose();
            }
        }
    }

    private void implClose()  {
        if (@in != null) {
            @in.close();
            @in = null;
            buf = null;
        }
    }

    public override long transferTo(OutputStream @out)
    {
        throw new NotImplementedException();
        // TODO: Objects.requireNonNull(@out, "out");
        //ensureOpen();
        //if (GetType() == typeof(PushbackInputStream)) {
        //    int avail = buf.Length - pos;
        //    if (avail > 0) {
        //        // Prevent poisoning and leaking of buf
        //        byte[] buffer = Arrays.copyOfRange(buf, pos, buf.Length);
        //        @out.write(buffer);
        //        pos = buffer.Length;
        //    }
        //    try {
        //        return Math.addExact(avail, @in.transferTo(@out));
        //    } catch (ArithmeticException ignore) {
        //        return long.MaxValue;
        //    }
        //} else {
        //    return base.transferTo(@out);
        //}
    }

}

}
