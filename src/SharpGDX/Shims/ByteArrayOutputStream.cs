using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotImplementedException = System.NotImplementedException;

namespace SharpGDX.Shims
{
	public class ByteArrayOutputStream : OutputStream {

    /**
     * The buffer where data is stored.
     */
    protected byte[] buf;

    /**
     * The number of valid bytes in the buffer.
     */
    protected int count;

    /**
     * Creates a new {@code ByteArrayOutputStream}. The buffer capacity is
     * initially 32 bytes, though its size increases if necessary.
     */
    public ByteArrayOutputStream() 
    : this(32)
    {
        
    }

    /**
     * Creates a new {@code ByteArrayOutputStream}, with a buffer capacity of
     * the specified size, in bytes.
     *
     * @param  size   the initial size.
     * @throws IllegalArgumentException if size is negative.
     */
    public ByteArrayOutputStream(int size) {
        if (size < 0) {
            throw new IllegalArgumentException("Negative initial size: "
                                               + size);
        }
        buf = new byte[size];
    }

    /**
     * Increases the capacity if necessary to ensure that it can hold
     * at least the number of elements specified by the minimum
     * capacity argument.
     *
     * @param  minCapacity the desired minimum capacity.
     * @throws OutOfMemoryError if {@code minCapacity < 0} and
     * {@code minCapacity - buf.length > 0}.  This is interpreted as a
     * request for the unsatisfiably large capacity.
     * {@code (long) Integer.MAX_VALUE + (minCapacity - Integer.MAX_VALUE)}.
     */
    private void ensureCapacity(int minCapacity) {
        // overflow-conscious code
        int oldCapacity = buf.Length;
        int minGrowth = minCapacity - oldCapacity;
        if (minGrowth > 0)
        {
            throw new NotImplementedException();
            // TODO: buf = Arrays.copyOf(buf, ArraysSupport.newLength(oldCapacity,
            //        minGrowth, oldCapacity /* preferred growth */));
        }
    }

    /**
     * Writes the specified byte to this {@code ByteArrayOutputStream}.
     *
     * @param   b   the byte to be written.
     */
    public override void write(int b) {
        lock (this)
        {
            ensureCapacity(count + 1);
            buf[count] = (byte)b;
            count += 1;
        }
    }

    /**
     * Writes {@code len} bytes from the specified byte array
     * starting at offset {@code off} to this {@code ByteArrayOutputStream}.
     *
     * @param   b     {@inheritDoc}
     * @param   off   {@inheritDoc}
     * @param   len   {@inheritDoc}
     * @throws  NullPointerException if {@code b} is {@code null}.
     * @throws  IndexOutOfBoundsException if {@code off} is negative,
     * {@code len} is negative, or {@code len} is greater than
     * {@code b.length - off}
     */
    public override void write(byte[] b, int off, int len) {
        lock (this)
        {
            // TODO: Objects.checkFromIndexSize(off, len, b.length);
            ensureCapacity(count + len);
            Array.Copy(b, off, buf, count, len);
            count += len;
        }
    }

    /**
     * Writes the complete contents of the specified byte array
     * to this {@code ByteArrayOutputStream}.
     *
     * @apiNote
     * This method is equivalent to {@link #write(byte[],int,int)
     * write(b, 0, b.length)}.
     *
     * @param   b     the data.
     * @throws  NullPointerException if {@code b} is {@code null}.
     * @since   11
     */
    public void writeBytes(byte[] b) {
        write(b, 0, b.Length);
    }

    /**
     * Writes the complete contents of this {@code ByteArrayOutputStream} to
     * the specified output stream argument, as if by calling the output
     * stream's write method using {@code out.write(buf, 0, count)}.
     *
     * @param   out   the output stream to which to write the data.
     * @throws  NullPointerException if {@code out} is {@code null}.
     * @throws  IOException if an I/O error occurs.
     */
    public void writeTo(OutputStream @out)
    {
        // TODO: Is this even possible in .Net? -RP
        //if (Thread.currentThread().isVirtual())
        //{
        //    byte[] bytes;
        //    lock (this)
        //    {
        //        bytes = Arrays.copyOf(buf, count);
        //    }

        //    @out.write(bytes);
        //}
        //else
            lock (this)
            {
                @out.write(buf, 0, count);
            }
    }

    /**
     * Resets the {@code count} field of this {@code ByteArrayOutputStream}
     * to zero, so that all currently accumulated output in the
     * output stream is discarded. The output stream can be used again,
     * reusing the already allocated buffer space.
     *
     * @see     java.io.ByteArrayInputStream#count
     */
    public void reset() {
        lock(this)
        count = 0;
    }

    /**
     * Creates a newly allocated byte array. Its size is the current
     * size of this output stream and the valid contents of the buffer
     * have been copied into it.
     *
     * @return  the current contents of this output stream, as a byte array.
     * @see     java.io.ByteArrayOutputStream#size()
     */
    public virtual byte[] toByteArray() {
        lock(this)
        return Arrays.copyOf(buf, count);
    }

    /**
     * Returns the current size of the buffer.
     *
     * @return  the value of the {@code count} field, which is the number
     *          of valid bytes in this output stream.
     * @see     java.io.ByteArrayOutputStream#count
     */
    public  int size() {
        lock(this)
        return count;
    }

    /**
     * Converts the buffer's contents into a string decoding bytes using the
     * default charset. The length of the new {@code String}
     * is a function of the charset, and hence may not be equal to the
     * size of the buffer.
     *
     * <p> This method always replaces malformed-input and unmappable-character
     * sequences with the default replacement string for the
     * default charset. The {@linkplain java.nio.charset.CharsetDecoder}
     * class should be used when more control over the decoding process is
     * required.
     *
     * @see Charset#defaultCharset()
     * @return String decoded from the buffer's contents.
     * @since  1.1
     */
    public override String ToString() {
        // TODO: What encoding should this use?
        lock(this)
        return Encoding.Default.GetString(buf, 0, count);
    }

    /**
     * Converts the buffer's contents into a string by decoding the bytes using
     * the named {@link Charset charset}.
     *
     * <p> This method is equivalent to {@code #toString(charset)} that takes a
     * {@link Charset charset}.
     *
     * <p> An invocation of this method of the form
     *
     * {@snippet lang=java :
     *     ByteArrayOutputStream b;
     *     b.toString("UTF-8")
     * }
     *
     * behaves in exactly the same way as the expression
     *
     * {@snippet lang=java :
     *     ByteArrayOutputStream b;
     *     b.toString(StandardCharsets.UTF_8)
     * }
     *
     *
     * @param  charsetName  the name of a supported
     *         {@link Charset charset}
     * @return String decoded from the buffer's contents.
     * @throws UnsupportedEncodingException
     *         If the named charset is not supported
     * @since  1.1
     */
    public String ToString(String charsetName)
    {
        lock (this)
            return Encoding.GetEncoding(charsetName).GetString(buf, 0, count);
    }

    /**
     * Converts the buffer's contents into a string by decoding the bytes using
     * the specified {@link Charset charset}. The length of the new
     * {@code String} is a function of the charset, and hence may not be equal
     * to the length of the byte array.
     *
     * <p> This method always replaces malformed-input and unmappable-character
     * sequences with the charset's default replacement string. The {@link
     * java.nio.charset.CharsetDecoder} class should be used when more control
     * over the decoding process is required.
     *
     * @param      charset  the {@linkplain Charset charset}
     *             to be used to decode the {@code bytes}
     * @return     String decoded from the buffer's contents.
     * @since      10
     */
    public  String ToString(Encoding charset) {
        lock(this)
        return charset.GetString(buf, 0, count);
    }

    /**
     * Creates a newly allocated string. Its size is the current size of
     * the output stream and the valid contents of the buffer have been
     * copied into it. Each character <i>c</i> in the resulting string is
     * constructed from the corresponding element <i>b</i> in the byte
     * array such that:
     * {@snippet lang=java :
     *     c == (char)(((hibyte & 0xff) << 8) | (b & 0xff))
     * }
     *
     * @deprecated This method does not properly convert bytes into characters.
     * As of JDK&nbsp;1.1, the preferred way to do this is via the
     * {@link #toString(String charsetName)} or {@link #toString(Charset charset)}
     * method, which takes an encoding-name or charset argument,
     * or the {@code toString()} method, which uses the default charset.
     *
     * @param      hibyte    the high byte of each resulting Unicode character.
     * @return     the current contents of the output stream, as a string.
     * @see        java.io.ByteArrayOutputStream#size()
     * @see        java.io.ByteArrayOutputStream#toString(String)
     * @see        java.io.ByteArrayOutputStream#toString()
     * @see        Charset#defaultCharset()
     */
    [Obsolete]
    public String ToString(int hibyte) {
        //lock(this)
        //return new String(buf, hibyte, 0, count);
        throw new NotImplementedException();
    }

    /**
     * Closing a {@code ByteArrayOutputStream} has no effect. The methods in
     * this class can be called after the stream has been closed without
     * generating an {@code IOException}.
     */
    public override void close() {
    }

}

}