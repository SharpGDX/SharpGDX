namespace SharpGDX.Shims;

public class BufferedInputStream : FilterInputStream {

    private static readonly int DEFAULT_BUFFER_SIZE = 8192;

    private static readonly byte[] EMPTY = new byte[0];

    /**
     * As this class is used early during bootstrap, it's motivated to use
     * Unsafe.compareAndSetReference instead of AtomicReferenceFieldUpdater
     * (or VarHandles) to reduce dependencies and improve startup time.
     */
    //private static readonly Unsafe U = Unsafe.getUnsafe();

    //private static readonly long BUF_OFFSET = U.objectFieldOffset(BufferedInputStream.class, "buf");

    // initialized to null when BufferedInputStream is sub-classed
    private readonly object? _lock;

    // initial buffer size (DEFAULT_BUFFER_SIZE or size specified to constructor)
    private readonly int initialSize;

    /**
     * The internal buffer array where the data is stored. When necessary,
     * it may be replaced by another array of
     * a different size.
     */
    /*
     * We null this out with a CAS on close(), which is necessary since
     * closes can be asynchronous. We use nullness of buf[] as primary
     * indicator that this stream is closed. (The "in" field is also
     * nulled out on close.)
     */
    protected volatile byte[] buf;

    /**
     * The index one greater than the index of the last valid byte in
     * the buffer.
     * This value is always
     * in the range {@code 0} through {@code buf.length};
     * elements {@code buf[0]} through {@code buf[count-1]}
     * contain buffered input data obtained
     * from the underlying  input stream.
     */
    protected int count;

    /**
     * The current position in the buffer. This is the index of the next
     * byte to be read from the {@code buf} array.
     * <p>
     * This value is always in the range {@code 0}
     * through {@code count}. If it is less
     * than {@code count}, then  {@code buf[pos]}
     * is the next byte to be supplied as input;
     * if it is equal to {@code count}, then
     * the  next {@code read} or {@code skip}
     * operation will require more bytes to be
     * read from the contained  input stream.
     *
     * @see     java.io.BufferedInputStream#buf
     */
    protected int pos;

    /**
     * The value of the {@code pos} field at the time the last
     * {@code mark} method was called.
     * <p>
     * This value is always
     * in the range {@code -1} through {@code pos}.
     * If there is no marked position in  the input
     * stream, this field is {@code -1}. If
     * there is a marked position in the input
     * stream,  then {@code buf[markpos]}
     * is the first byte to be supplied as input
     * after a {@code reset} operation. If
     * {@code markpos} is not {@code -1},
     * then all bytes from positions {@code buf[markpos]}
     * through  {@code buf[pos-1]} must remain
     * in the buffer array (though they may be
     * moved to  another place in the buffer array,
     * with suitable adjustments to the values
     * of {@code count},  {@code pos},
     * and {@code markpos}); they may not
     * be discarded unless and until the difference
     * between {@code pos} and {@code markpos}
     * exceeds {@code marklimit}.
     *
     * @see     java.io.BufferedInputStream#mark(int)
     * @see     java.io.BufferedInputStream#pos
     */
    protected int markpos = -1;

    /**
     * The maximum read ahead allowed after a call to the
     * {@code mark} method before subsequent calls to the
     * {@code reset} method fail.
     * Whenever the difference between {@code pos}
     * and {@code markpos} exceeds {@code marklimit},
     * then the  mark may be dropped by setting
     * {@code markpos} to {@code -1}.
     *
     * @see     java.io.BufferedInputStream#mark(int)
     * @see     java.io.BufferedInputStream#reset()
     */
    protected int marklimit;

    /**
     * Check to make sure that underlying input stream has not been
     * nulled out due to close; if not return it;
     */
    private InputStream getInIfOpen()  {
        InputStream input = @in;
        if (input == null)
            throw new IOException("Stream closed");
        return input;
    }

    /**
     * Returns the internal buffer, optionally allocating it if empty.
     * @param allocateIfEmpty true to allocate if empty
     * @throws IOException if the stream is closed (buf is null)
     */
    private byte[] getBufIfOpen(bool allocateIfEmpty)  {
        byte[] buffer = buf;
        if (allocateIfEmpty && buffer == EMPTY) {
            buf = buffer = new byte[initialSize];
            // TODO: Can this even be replicated in .NET? -RP
            //buffer = new byte[initialSize];
            // if (!U.compareAndSetReference(this, BUF_OFFSET, EMPTY, buffer)) {
            //    // re-read buf
            //    buffer = buf;
            //}
        }
        if (buffer == null) {
            throw new IOException("Stream closed");
        }
        return buffer;
    }

    /**
     * Returns the internal buffer, allocating it if empty.
     * @throws IOException if the stream is closed (buf is null)
     */
    private byte[] getBufIfOpen()  {
        return getBufIfOpen(true);
    }

    /**
     * Throws IOException if the stream is closed (buf is null).
     */
    private void ensureOpen()  {
        if (buf == null) {
            throw new IOException("Stream closed");
        }
    }

    /**
     * Creates a {@code BufferedInputStream}
     * and saves its  argument, the input stream
     * {@code in}, for later use. An internal
     * buffer array is created and  stored in {@code buf}.
     *
     * @param   in   the underlying input stream.
     */
    public BufferedInputStream(InputStream @in) 
    : this(@in, DEFAULT_BUFFER_SIZE)
    {
        
    }

    /**
     * Creates a {@code BufferedInputStream}
     * with the specified buffer size,
     * and saves its  argument, the input stream
     * {@code in}, for later use.  An internal
     * buffer array of length  {@code size}
     * is created and stored in {@code buf}.
     *
     * @param   in     the underlying input stream.
     * @param   size   the buffer size.
     * @throws  IllegalArgumentException if {@code size <= 0}.
     */
    public BufferedInputStream(InputStream @in, int size) 
    : base(@in)
    {
        
        if (size <= 0) {
            throw new IllegalArgumentException("Buffer size <= 0");
        }
        initialSize = size;
        if (GetType() == typeof(BufferedInputStream)) {
            // use internal lock and lazily create buffer when not subclassed
            _lock = new object();
            buf = EMPTY;
        } else {
            // use monitors and eagerly create buffer when subclassed
            _lock = null;
            buf = new byte[size];
        }
    }

    /**
     * Fills the buffer with more data, taking into account
     * shuffling and other tricks for dealing with marks.
     * Assumes that it is being called by a locked method.
     * This method also assumes that all data has already been read in,
     * hence pos > count.
     */
    private void fill()  {
        byte[] buffer = getBufIfOpen();
        if (markpos == -1)
            pos = 0;            /* no mark: throw away the buffer */
        else if (pos >= buffer.Length) { /* no room left in buffer */
            if (markpos > 0) {  /* can throw away early part of the buffer */
                int sz = pos - markpos;
                Array.Copy(buffer, markpos, buffer, 0, sz);
                pos = sz;
                markpos = 0;
            } else if (buffer.Length >= marklimit) {
                markpos = -1;   /* buffer got too big, invalidate mark */
                pos = 0;        /* drop buffer contents */
            } else {            /* grow buffer */
                throw new NotImplementedException();
                // TODO: int nsz = ArraysSupport.newLength(pos,
                //        1,  /* minimum growth */
                //        pos /* preferred growth */);
                //if (nsz > marklimit)
                //    nsz = marklimit;
                //byte[] nbuf = new byte[nsz];
                //System.arraycopy(buffer, 0, nbuf, 0, pos);
                //if (!U.compareAndSetReference(this, BUF_OFFSET, buffer, nbuf)) {
                //    // Can't replace buf if there was an async close.
                //    // Note: This would need to be changed if fill()
                //    // is ever made accessible to multiple threads.
                //    // But for now, the only way CAS can fail is via close.
                //    // assert buf == null;
                //    throw new IOException("Stream closed");
                //}
               // buffer = nbuf;
            }
        }
        count = pos;
        int n = getInIfOpen().read(buffer, pos, buffer.Length - pos);
        if (n > 0)
            count = n + pos;
    }

    /**
     * See
     * the general contract of the {@code read}
     * method of {@code InputStream}.
     *
     * @return     the next byte of data, or {@code -1} if the end of the
     *             stream is reached.
     * @throws     IOException  if this input stream has been closed by
     *                          invoking its {@link #close()} method,
     *                          or an I/O error occurs.
     * @see        java.io.FilterInputStream#in
     */
    public override int read() {
        if (_lock != null) {
            lock(_lock)
            try {
                return implRead();
            } finally {
            }
        } else {
            lock (this) {
                return implRead();
            }
        }
    }

    private int implRead()  {
        if (pos >= count) {
            fill();
            if (pos >= count)
                return -1;
        }
        return getBufIfOpen()[pos++] & 0xff;
    }

    /**
     * Read bytes into a portion of an array, reading from the underlying
     * stream at most once if necessary.
     */
    private int read1(byte[] b, int off, int len)  {
        int avail = count - pos;
        if (avail <= 0) {
            /* If the requested length is at least as large as the buffer, and
               if there is no mark/reset activity, do not bother to copy the
               bytes into the local buffer.  In this way buffered streams will
               cascade harmlessly. */
            int size = Math.Max(getBufIfOpen(false).Length, initialSize);
            if (len >= size && markpos == -1) {
                return getInIfOpen().read(b, off, len);
            }
            fill();
            avail = count - pos;
            if (avail <= 0) return -1;
        }
        int cnt = (avail < len) ? avail : len;
        Array.Copy(getBufIfOpen(), pos, b, off, cnt);
        pos += cnt;
        return cnt;
    }

    /**
     * Reads bytes from this byte-input stream into the specified byte array,
     * starting at the given offset.
     *
     * <p> This method implements the general contract of the corresponding
     * {@link InputStream#read(byte[], int, int) read} method of
     * the {@link InputStream} class.  As an additional
     * convenience, it attempts to read as many bytes as possible by repeatedly
     * invoking the {@code read} method of the underlying stream.  This
     * iterated {@code read} continues until one of the following
     * conditions becomes true: <ul>
     *
     *   <li> The specified number of bytes have been read,
     *
     *   <li> The {@code read} method of the underlying stream returns
     *   {@code -1}, indicating end-of-file, or
     *
     *   <li> The {@code available} method of the underlying stream
     *   returns zero, indicating that further input requests would block.
     *
     * </ul> If the first {@code read} on the underlying stream returns
     * {@code -1} to indicate end-of-file then this method returns
     * {@code -1}.  Otherwise, this method returns the number of bytes
     * actually read.
     *
     * <p> Subclasses of this class are encouraged, but not required, to
     * attempt to read as many bytes as possible in the same fashion.
     *
     * @param      b     destination buffer.
     * @param      off   offset at which to start storing bytes.
     * @param      len   maximum number of bytes to read.
     * @return     the number of bytes read, or {@code -1} if the end of
     *             the stream has been reached.
     * @throws     IOException  if this input stream has been closed by
     *                          invoking its {@link #close()} method,
     *                          or an I/O error occurs.
     * @throws     IndexOutOfBoundsException {@inheritDoc}
     */
    public override int read(byte[] b, int off, int len)  {
        if (_lock != null) {
            lock(_lock)
            try {
                return implRead(b, off, len);
            } finally {
            }
        } else {
            lock (this) {
                return implRead(b, off, len);
            }
        }
    }

    private int implRead(byte[] b, int off, int len)  {
        ensureOpen();
        if ((off | len | (off + len) | (b.Length - (off + len))) < 0) {
            throw new IndexOutOfBoundsException();
        } else if (len == 0) {
            return 0;
        }

        int n = 0;
        for (;;) {
            int nread = read1(b, off + n, len - n);
            if (nread <= 0)
                return (n == 0) ? nread : n;
            n += nread;
            if (n >= len)
                return n;
            // if not closed but no bytes available, return
            InputStream? input = @in;
            if (input != null && input.available() <= 0)
                return n;
        }
    }

    /**
     * See the general contract of the {@code skip}
     * method of {@code InputStream}.
     *
     * @throws IOException  if this input stream has been closed by
     *                      invoking its {@link #close()} method,
     *                      {@code in.skip(n)} throws an IOException,
     *                      or an I/O error occurs.
     */
    public override long skip(long n) {
        if (_lock != null) {
            lock(_lock)
            try {
                return implSkip(n);
            } finally {
            }
        } else {
            lock (this) {
                return implSkip(n);
            }
        }
    }

    private long implSkip(long n)  {
        ensureOpen();
        if (n <= 0) {
            return 0;
        }
        long avail = count - pos;

        if (avail <= 0) {
            // If no mark position set then don't keep in buffer
            if (markpos == -1)
                return getInIfOpen().skip(n);

            // Fill in buffer to save bytes for reset
            fill();
            avail = count - pos;
            if (avail <= 0)
                return 0;
        }

        long skipped = (avail < n) ? avail : n;
        pos += (int)skipped;
        return skipped;
    }

    /**
     * Returns an estimate of the number of bytes that can be read (or
     * skipped over) from this input stream without blocking by the next
     * invocation of a method for this input stream. The next invocation might be
     * the same thread or another thread.  A single read or skip of this
     * many bytes will not block, but may read or skip fewer bytes.
     * <p>
     * This method returns the sum of the number of bytes remaining to be read in
     * the buffer ({@code count - pos}) and the result of calling the
     * {@link java.io.FilterInputStream#in in}{@code .available()}.
     *
     * @return     an estimate of the number of bytes that can be read (or skipped
     *             over) from this input stream without blocking.
     * @throws     IOException  if this input stream has been closed by
     *                          invoking its {@link #close()} method,
     *                          or an I/O error occurs.
     */
    public override int available() {
        if (_lock != null) {
            lock(_lock)
            try {
                return implAvailable();
            } finally {
            }
        } else {
            lock (this) {
                return implAvailable();
            }
        }
    }

    private int implAvailable() {
        int n = count - pos;
        int avail = getInIfOpen().available();
        return n > (int.MaxValue - avail)
                    ? int.MaxValue
                    : n + avail;
    }

    /**
     * See the general contract of the {@code mark}
     * method of {@code InputStream}.
     *
     * @param   readlimit   the maximum limit of bytes that can be read before
     *                      the mark position becomes invalid.
     * @see     java.io.BufferedInputStream#reset()
     */
    public override void mark(int readlimit) {
        if (_lock != null) {
            lock(_lock)
            try {
                implMark(readlimit);
            } finally {
            }
        } else {
            lock (this) {
                implMark(readlimit);
            }
        }
    }

    private void implMark(int readlimit) {
        marklimit = readlimit;
        markpos = pos;
    }

    /**
     * See the general contract of the {@code reset}
     * method of {@code InputStream}.
     * <p>
     * If {@code markpos} is {@code -1}
     * (no mark has been set or the mark has been
     * invalidated), an {@code IOException}
     * is thrown. Otherwise, {@code pos} is
     * set equal to {@code markpos}.
     *
     * @throws     IOException  if this stream has not been marked or,
     *                  if the mark has been invalidated, or the stream
     *                  has been closed by invoking its {@link #close()}
     *                  method, or an I/O error occurs.
     * @see        java.io.BufferedInputStream#mark(int)
     */
    public override void reset()  {
        if (_lock != null) {
            lock(_lock)
            try {
                implReset();
            } finally {
            }
        } else {
            lock (this) {
                implReset();
            }
        }
    }

    private void implReset()  {
        ensureOpen();
        if (markpos < 0)
            throw new IOException("Resetting to invalid mark");
        pos = markpos;
    }

    /**
     * Tests if this input stream supports the {@code mark}
     * and {@code reset} methods. The {@code markSupported}
     * method of {@code BufferedInputStream} returns
     * {@code true}.
     *
     * @return  a {@code boolean} indicating if this stream type supports
     *          the {@code mark} and {@code reset} methods.
     * @see     java.io.InputStream#mark(int)
     * @see     java.io.InputStream#reset()
     */
    public override bool markSupported() {
        return true;
    }

    /**
     * Closes this input stream and releases any system resources
     * associated with the stream.
     * Once the stream has been closed, further read(), available(), reset(),
     * or skip() invocations will throw an IOException.
     * Closing a previously closed stream has no effect.
     *
     * @throws     IOException  if an I/O error occurs.
     */
    public override void close()  {
        byte[] buffer;
        while ( (buffer = buf) != null)
        {
            throw new NotImplementedException();
            // TODO: if (U.compareAndSetReference(this, BUF_OFFSET, buffer, null)) {
            //    InputStream input = in;
            //    in = null;
            //    if (input != null)
            //        input.close();
            //    return;
            //}
            // Else retry in case a new buf was CASed in fill()
        }
    }

    public override long transferTo(OutputStream @out)  {
        throw new NotImplementedException();
        //Objects.requireNonNull(out, "out");
        //if (lock != null) {
        //    lock.lock();
        //    try {
        //        return implTransferTo(out);
        //    } finally {
        //        lock.unlock();
        //    }
        //} else {
        //    synchronized (this) {
        //        return implTransferTo(out);
        //    }
        //}
    }

    // TODO: private long implTransferTo(OutputStream out)  {
    //    if (getClass() == BufferedInputStream.class && markpos == -1) {
    //        int avail = count - pos;
    //        if (avail > 0) {
    //            if (isTrusted(out)) {
    //                out.write(getBufIfOpen(), pos, avail);
    //            } else {
    //                // Prevent poisoning and leaking of buf
    //                byte[] buffer = Arrays.copyOfRange(getBufIfOpen(), pos, count);
    //                out.write(buffer);
    //            }
    //            pos = count;
    //        }
    //        try {
    //            return Math.addExact(avail, getInIfOpen().transferTo(out));
    //        } catch (ArithmeticException ignore) {
    //            return Long.MAX_VALUE;
    //        }
    //    } else {
    //        return super.transferTo(out);
    //    }
    //}

    /**
     * Returns true if this class satisfies the following conditions:
     * <ul>
     * <li>does not retain a reference to the {@code byte[]}</li>
     * <li>does not leak a reference to the {@code byte[]} to non-trusted classes</li>
     * <li>does not modify the contents of the {@code byte[]}</li>
     * <li>{@code write()} method does not read the contents outside of the offset/length bounds</li>
     * </ul>
     *
     * @return true if this class is trusted
     */
    // TODO: private static bool isTrusted(OutputStream os) {
    //    var clazz = os.getClass();
    //    return clazz == ByteArrayOutputStream.class
    //            || clazz == FileOutputStream.class
    //            || clazz == PipedOutputStream.class;
    //}

}
