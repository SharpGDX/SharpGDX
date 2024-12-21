﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public abstract class OutputStream : ICloseable, Flushable {
    /**
     * Constructor for subclasses to call.
     */
    public OutputStream() {}

    /**
     * Returns a new {@code OutputStream} which discards all bytes.  The
     * returned stream is initially open.  The stream is closed by calling
     * the {@code close()} method.  Subsequent calls to {@code close()} have
     * no effect.
     *
     * <p> While the stream is open, the {@code write(int)}, {@code
     * write(byte[])}, and {@code write(byte[], int, int)} methods do nothing.
     * After the stream has been closed, these methods all throw {@code
     * IOException}.
     *
     * <p> The {@code flush()} method does nothing.
     *
     * @return an {@code OutputStream} which discards all bytes
     *
     * @since 11
     */
    public static OutputStream nullOutputStream()
    {
        throw new NotImplementedException();
        //return new OutputStream() {
        //    private volatile bool closed;

        //    private void ensureOpen() throws IOException {
        //        if (closed) {
        //            throw new IOException("Stream closed");
        //        }
        //    }

        //    @Override
        //    public void write(int b) throws IOException {
        //        ensureOpen();
        //    }

        //    @Override
        //    public void write(byte[] b, int off, int len) throws IOException {
        //        Objects.checkFromIndexSize(off, len, b.length);
        //        ensureOpen();
        //    }

        //    @Override
        //    public void close() {
        //        closed = true;
        //    }
        //};
    }

    /**
     * Writes the specified byte to this output stream. The general
     * contract for {@code write} is that one byte is written
     * to the output stream. The byte to be written is the eight
     * low-order bits of the argument {@code b}. The 24
     * high-order bits of {@code b} are ignored.
     *
     * @param      b   the {@code byte}.
     * @throws     IOException  if an I/O error occurs. In particular,
     *             an {@code IOException} may be thrown if the
     *             output stream has been closed.
     */
    public abstract void write(int b) ;

    /**
     * Writes {@code b.length} bytes from the specified byte array
     * to this output stream. The general contract for {@code write(b)}
     * is that it should have exactly the same effect as the call
     * {@code write(b, 0, b.length)}.
     *
     * @param      b   the data.
     * @throws     IOException  if an I/O error occurs.
     * @see        java.io.OutputStream#write(byte[], int, int)
     */
    public virtual void write(byte[] b)  {
        write(b, 0, b.Length);
    }

    /**
     * Writes {@code len} bytes from the specified byte array
     * starting at offset {@code off} to this output stream.
     * The general contract for {@code write(b, off, len)} is that
     * some of the bytes in the array {@code b} are written to the
     * output stream in order; element {@code b[off]} is the first
     * byte written and {@code b[off+len-1]} is the last byte written
     * by this operation.
     *
     * <p>
     * If {@code b} is {@code null}, a
     * {@code NullPointerException} is thrown.
     * <p>
     * If {@code off} is negative, or {@code len} is negative, or
     * {@code off+len} is greater than the length of the array
     * {@code b}, then an {@code IndexOutOfBoundsException} is thrown.
     *
     * @implSpec
     * The {@code write} method of {@code OutputStream} calls
     * the write method of one argument on each of the bytes to be
     * written out.
     *
     * @apiNote
     * Subclasses are encouraged to override this method and
     * provide a more efficient implementation.
     *
     * @param      b     the data.
     * @param      off   the start offset in the data.
     * @param      len   the number of bytes to write.
     * @throws     IOException  if an I/O error occurs. In particular,
     *             an {@code IOException} is thrown if the output
     *             stream is closed.
     * @throws     IndexOutOfBoundsException If {@code off} is negative,
     *             {@code len} is negative, or {@code len} is greater than
     *             {@code b.length - off}
     */
    public virtual void write(byte[] b, int off, int len)  {
        // TODO: Objects.checkFromIndexSize(off, len, b.Length);
        // len == 0 condition implicitly handled by loop bounds
        for (int i = 0 ; i < len ; i++) {
            write(b[off + i]);
        }
    }

    /**
     * Flushes this output stream and forces any buffered output bytes
     * to be written out. The general contract of {@code flush} is
     * that calling it is an indication that, if any bytes previously
     * written have been buffered by the implementation of the output
     * stream, such bytes should immediately be written to their
     * intended destination.
     * <p>
     * If the intended destination of this stream is an abstraction provided by
     * the underlying operating system, for example a file, then flushing the
     * stream guarantees only that bytes previously written to the stream are
     * passed to the operating system for writing; it does not guarantee that
     * they are actually written to a physical device such as a disk drive.
     *
     * @implSpec
     * The {@code flush} method of {@code OutputStream} does nothing.
     *
     * @throws     IOException  if an I/O error occurs.
     */
    public virtual void flush()  {
    }

    /**
     * Closes this output stream and releases any system resources
     * associated with this stream. The general contract of {@code close}
     * is that it closes the output stream. A closed stream cannot perform
     * output operations and cannot be reopened.
     *
     * @implSpec
     * The {@code close} method of {@code OutputStream} does nothing.
     *
     * @throws     IOException  if an I/O error occurs.
     */
    public virtual void close()  {
    }

}

}
