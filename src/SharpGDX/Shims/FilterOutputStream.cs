using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
    public class FilterOutputStream : OutputStream {
    /**
     * The underlying output stream to be filtered.
     */
    protected OutputStream @out;

    /**
     * Whether the stream is closed; implicitly initialized to false.
     */
    private volatile bool closed;

    /**
     * Object used to prevent a race on the 'closed' instance variable.
     */
    private readonly Object closeLock = new Object();

    /**
     * Creates an output stream filter built on top of the specified
     * underlying output stream.
     *
     * @param   out   the underlying output stream to be assigned to
     *                the field {@code this.out} for later use, or
     *                {@code null} if this instance is to be
     *                created without an underlying stream.
     */
    public FilterOutputStream(OutputStream @out) {
        this.@out = @out;
    }

    /**
     * Writes the specified {@code byte} to this output stream.
     * <p>
     * The {@code write} method of {@code FilterOutputStream}
     * calls the {@code write} method of its underlying output stream,
     * that is, it performs {@code out.write(b)}.
     * <p>
     * Implements the abstract {@code write} method of {@code OutputStream}.
     *
     * @param      b   {@inheritDoc}
     * @throws     IOException  if an I/O error occurs.
     */
    public override void write(int b)  {
        @out.write(b);
    }

    /**
     * Writes {@code b.length} bytes to this output stream.
     * @implSpec
     * The {@code write} method of {@code FilterOutputStream}
     * calls its {@code write} method of three arguments with the
     * arguments {@code b}, {@code 0}, and
     * {@code b.length}.
     * @implNote
     * Note that this method does <em>not</em> call the one-argument
     * {@code write} method of its underlying output stream with
     * the single argument {@code b}.
     *
     * @param      b   the data to be written.
     * @throws     IOException  {@inheritDoc}
     * @see        java.io.FilterOutputStream#write(byte[], int, int)
     */
    public override void write(byte[] b)  {
        write(b, 0, b.Length);
    }

    /**
     * Writes {@code len} bytes from the specified
     * {@code byte} array starting at offset {@code off} to
     * this output stream.
     * @implSpec
     * The {@code write} method of {@code FilterOutputStream}
     * calls the {@code write} method of one argument on each
     * {@code byte} to output.
     * @implNote
     * Note that this method does not call the {@code write} method
     * of its underlying output stream with the same arguments. Subclasses
     * of {@code FilterOutputStream} should provide a more efficient
     * implementation of this method.
     *
     * @param      b     {@inheritDoc}
     * @param      off   {@inheritDoc}
     * @param      len   {@inheritDoc}
     * @throws     IOException  if an I/O error occurs.
     * @throws     IndexOutOfBoundsException {@inheritDoc}
     * @see        java.io.FilterOutputStream#write(int)
     */
    public override void write(byte[] b, int off, int len) {
        // TODO: Objects.checkFromIndexSize(off, len, b.length);

        for (int i = 0 ; i < len ; i++) {
            write(b[off + i]);
        }
    }

    /**
     * Flushes this output stream and forces any buffered output bytes
     * to be written out to the stream.
     * @implSpec
     * The {@code flush} method of {@code FilterOutputStream}
     * calls the {@code flush} method of its underlying output stream.
     *
     * @throws     IOException  {@inheritDoc}
     * @see        java.io.FilterOutputStream#out
     */
    public override void flush()  {
        @out.flush();
    }

    /**
     * Closes this output stream and releases any system resources
     * associated with the stream.
     * @implSpec
     * When not already closed, the {@code close} method of {@code
     * FilterOutputStream} calls its {@code flush} method, and then
     * calls the {@code close} method of its underlying output stream.
     *
     * @throws     IOException  if an I/O error occurs.
     * @see        java.io.FilterOutputStream#flush()
     * @see        java.io.FilterOutputStream#out
     */
    public override void close() {
        if (closed) {
            return;
        }
        lock (closeLock) {
            if (closed) {
                return;
            }
            closed = true;
        }

        Exception? flushException = null;
        try {
            flush();
        } catch (Exception e) {
            flushException = e;
            throw e;
        } finally {
            if (flushException == null) {
                @out.close();
            } else {
                try {
                    @out.close();
                } catch (Exception closeException) {
                    if (flushException != closeException)
                    {
                        throw new NotImplementedException();
                        // TODO: closeException.addSuppressed(flushException);
                    }
                    throw closeException;
                }
            }
        }
    }
}

}
