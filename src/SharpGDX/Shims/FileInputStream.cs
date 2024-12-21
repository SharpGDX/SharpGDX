using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
    //public class FileInputStream : InputStream
    //{
    //	public FileInputStream(File file)
    //		: base(System.IO.File.OpenRead(file.getCanonicalPath()))
    //	{

    //	}
    //}

    public class FileInputStream : InputStream
    {
        private static readonly int DEFAULT_BUFFER_SIZE = 8192;

        /**
         * Flag set by jdk.internal.event.JFRTracing to indicate if
         * file reads should be traced by JFR.
         */
        private static bool jfrTracing;

        /* File Descriptor - handle to the open file */
        private //readonly
                FileStream fd;

        /**
         * The path of the referenced file
         * (null if the stream is created with a file descriptor)
         */
        private readonly String path;

        private volatile FileChannel channel;

        private readonly Object closeLock = new Object();

        private volatile bool closed;

        /**
         * Creates a {@code FileInputStream} by
         * opening a connection to an actual file,
         * the file named by the path name {@code name}
         * in the file system.  A new {@code FileDescriptor}
         * object is created to represent this file
         * connection.
         * <p>
         * First, if there is a security
         * manager, its {@code checkRead} method
         * is called with the {@code name} argument
         * as its argument.
         * <p>
         * If the named file does not exist, is a directory rather than a regular
         * file, or for some other reason cannot be opened for reading then a
         * {@code FileNotFoundException} is thrown.
         *
         * @param      name   the system-dependent file name.
         * @throws     FileNotFoundException  if the file does not exist,
         *             is a directory rather than a regular file,
         *             or for some other reason cannot be opened for
         *             reading.
         * @throws     SecurityException      if a security manager exists and its
         *             {@code checkRead} method denies read access
         *             to the file.
         * @see        java.lang.SecurityManager#checkRead(java.lang.String)
         */
        public FileInputStream(String name)
            : this(name != null ? new File(name) : null)
        {

        }

        /**
         * Creates a {@code FileInputStream} by
         * opening a connection to an actual file,
         * the file named by the {@code File}
         * object {@code file} in the file system.
         * A new {@code FileDescriptor} object
         * is created to represent this file connection.
         * <p>
         * First, if there is a security manager,
         * its {@code checkRead} method  is called
         * with the path represented by the {@code file}
         * argument as its argument.
         * <p>
         * If the named file does not exist, is a directory rather than a regular
         * file, or for some other reason cannot be opened for reading then a
         * {@code FileNotFoundException} is thrown.
         *
         * @param      file   the file to be opened for reading.
         * @throws     FileNotFoundException  if the file does not exist,
         *             is a directory rather than a regular file,
         *             or for some other reason cannot be opened for
         *             reading.
         * @throws     SecurityException      if a security manager exists and its
         *             {@code checkRead} method denies read access to the file.
         * @see        java.io.File#getPath()
         * @see        java.lang.SecurityManager#checkRead(java.lang.String)
         */
        // TODO: @SuppressWarnings("this-escape")
        public FileInputStream(File file)
        {
            // TODO: This should be file.getPath(). -RP
            String name = (file != null ? file.getCanonicalPath() : null);
            // TODO: @SuppressWarnings("removal")
            // TODO: SecurityManager security = System.getSecurityManager();
            //if (security != null) {
            //    security.checkRead(name);
            //}
            if (name == null)
            {
                throw new NullPointerException();
            }

            // TODO: if (file.isInvalid())
            //{
            //    throw new FileNotFoundException("Invalid file path");
            //}

            // TODO: fd = new FileDescriptor();
            // TODO: fd.attach(this);
            path = name;
            open(name);
            // TODO: FileCleanable.register(fd); // open set the fd, register the cleanup
        }

        /**
         * Creates a {@code FileInputStream} by using the file descriptor
         * {@code fdObj}, which represents an existing connection to an
         * actual file in the file system.
         * <p>
         * If there is a security manager, its {@code checkRead} method is
         * called with the file descriptor {@code fdObj} as its argument to
         * see if it's ok to read the file descriptor. If read access is denied
         * to the file descriptor a {@code SecurityException} is thrown.
         * <p>
         * If {@code fdObj} is null then a {@code NullPointerException}
         * is thrown.
         * <p>
         * This constructor does not throw an exception if {@code fdObj}
         * is {@link java.io.FileDescriptor#valid() invalid}.
         * However, if the methods are invoked on the resulting stream to attempt
         * I/O on the stream, an {@code IOException} is thrown.
         *
         * @param      fdObj   the file descriptor to be opened for reading.
         * @throws     SecurityException      if a security manager exists and its
         *             {@code checkRead} method denies read access to the
         *             file descriptor.
         * @see        SecurityManager#checkRead(java.io.FileDescriptor)
         */
        // TODO: @SuppressWarnings("this-escape")
        //public FileInputStream(FileDescriptor fdObj)
        //{
        //    // TODO: @SuppressWarnings("removal")
        //    SecurityManager security = System.getSecurityManager();
        //    if (fdObj == null)
        //    {
        //        throw new NullPointerException();
        //    }

        //    if (security != null)
        //    {
        //        security.checkRead(fdObj);
        //    }

        //    fd = fdObj;
        //    path = null;

        //    /*
        //     * FileDescriptor is being shared by streams.
        //     * Register this stream with FileDescriptor tracker.
        //     */
        //    fd.attach(this);
        //}

        /**
         * Opens the specified file for reading.
         * @param name the name of the file
         */
        //private extern void open0(String name);

        // wrap native call to allow instrumentation
        /**
         * Opens the specified file for reading.
         * @param name the name of the file
         */
        private void open(String name)
        {
            //open0(name);
            fd = new FileStream(name, FileMode.Open);
        }

        /**
         * Reads a byte of data from this input stream. This method blocks
         * if no input is yet available.
         *
         * @return     the next byte of data, or {@code -1} if the end of the
         *             file is reached.
         * @throws     IOException {@inheritDoc}
         */
        public override int read()
        {
            //if (jfrTracing && FileReadEvent.enabled())
            //{
            //    return traceRead0();
            //}

            //return read0();
            return fd.ReadByte();
        }

        //private extern int read0();

        //private int traceRead0()
        //{
        //    int result = 0;
        //    bool endOfFile = false;
        //    long bytesRead = 0;
        //    long start = 0;
        //    try
        //    {
        //        start = FileReadEvent.timestamp();
        //        result = read0();
        //        if (result < 0)
        //        {
        //            endOfFile = true;
        //        }
        //        else
        //        {
        //            bytesRead = 1;
        //        }
        //    }
        //    finally
        //    {
        //        long duration = FileReadEvent.timestamp() - start;
        //        if (FileReadEvent.shouldCommit(duration))
        //        {
        //            FileReadEvent.commit(start, duration, path, bytesRead, endOfFile);
        //        }
        //    }

        //    return result;
        //}

        /**
         * Reads a subarray as a sequence of bytes.
         * @param     b the data to be written
         * @param     off the start offset in the data
         * @param     len the number of bytes that are written
         * @throws    IOException If an I/O error has occurred.
         */
        //private extern int readBytes(byte[] b, int off, int len);

        //private int traceReadBytes(byte[] b, int off, int len)
        //{
        //    int bytesRead = 0;
        //   // long start = 0;
        //    try
        //    {
        //        //start = FileReadEvent.timestamp();
        //        bytesRead = fd.Read(b, off, len);// readBytes(b, off, len);
        //    }
        //    finally
        //    {
        //        //long duration = FileReadEvent.timestamp() - start;
        //        //if (FileReadEvent.shouldCommit(duration))
        //        //{
        //        //    if (bytesRead < 0)
        //        //    {
        //        //        FileReadEvent.commit(start, duration, path, 0L, true);
        //        //    }
        //        //    else
        //        //    {
        //        //        FileReadEvent.commit(start, duration, path, bytesRead, false);
        //        //    }
        //        //}
        //    }

        //    return bytesRead;
        //}

        /**
         * Reads up to {@code b.length} bytes of data from this input
         * stream into an array of bytes. This method blocks until some input
         * is available.
         *
         * @param      b   {@inheritDoc}
         * @return     the total number of bytes read into the buffer, or
         *             {@code -1} if there is no more data because the end of
         *             the file has been reached.
         * @throws     IOException  if an I/O error occurs.
         */
        public override int read(byte[] b)
        {
            //if (jfrTracing && FileReadEvent.enabled())
            //{
            //    return traceReadBytes(b, 0, b.Length);
            //}

            // TODO: I don't like this. -RP
            if (fd.Position == fd.Length)
            {
                return -1;
            }

            return fd.Read(b, 0, b.Length);// readBytes(b, 0, b.Length);
        }

        /**
         * Reads up to {@code len} bytes of data from this input stream
         * into an array of bytes. If {@code len} is not zero, the method
         * blocks until some input is available; otherwise, no
         * bytes are read and {@code 0} is returned.
         *
         * @param      b     {@inheritDoc}
         * @param      off   {@inheritDoc}
         * @param      len   {@inheritDoc}
         * @return     {@inheritDoc}
         * @throws     NullPointerException {@inheritDoc}
         * @throws     IndexOutOfBoundsException {@inheritDoc}
         * @throws     IOException  if an I/O error occurs.
         */
        public override int read(byte[] b, int off, int len)
        {
            //if (jfrTracing && FileReadEvent.enabled())
            //{
            //    return traceReadBytes(b, off, len);
            //}

            return fd.Read(b, off, len);// readBytes(b, off, len);
        }

        public override byte[] readAllBytes()
        {
            throw new NotImplementedException();
            //long length = this.length();
            //long position = this.position();
            //long size = length - position;

            //if (length <= 0 || size <= 0)
            //    return base.readAllBytes();

            //if (size > (long)int.MaxValue)
            //{
            //    String msg =
            //        String.Format("Required array size too large for %s: %d = %d - %d",
            //            path, size, length, position);
            //    throw new OutOfMemoryException(msg);
            //}

            //int capacity = (int)size;
            //byte[] buf = new byte[capacity];

            //int nread = 0;
            //int n;
            //for (;;)
            //{
            //    // read to EOF which may read more or less than initial size, e.g.,
            //    // file is truncated while we are reading
            //    while ((n = read(buf, nread, capacity - nread)) > 0)
            //        nread += n;

            //    // if last call to read() returned -1, we are done; otherwise,
            //    // try to read one more byte and if that fails we're done too
            //    if (n < 0 || (n = read()) < 0)
            //        break;

            //    // one more byte was read; need to allocate a larger buffer
            //    capacity = Math.Max(ArraysSupport.newLength(capacity,
            //            1, // min growth
            //            capacity), // pref growth
            //        DEFAULT_BUFFER_SIZE);
            //    buf = Arrays.copyOf(buf, capacity);
            //    buf[nread++] = (byte)n;
            //}

            //return (capacity == nread) ? buf : Arrays.copyOf(buf, nread);
        }

        public override byte[] readNBytes(int len)
        {
            if (len < 0)
                throw new IllegalArgumentException("len < 0");
            if (len == 0)
                return new byte[0];

            long length = this.length();
            long position = this.position();
            long size = length - position;

            if (length <= 0 || size <= 0)
                return base.readNBytes(len);

            int capacity = (int)Math.Min(len, size);
            byte[] buf = new byte[capacity];

            int remaining = capacity;
            int nread = 0;
            int n;
            do
            {
                n = read(buf, nread, remaining);
                if (n > 0)
                {
                    nread += n;
                    remaining -= n;
                }
                else if (n == 0)
                {
                    // Block until a byte is read or EOF is detected
                    byte b = (byte)read();
                    if (b == -1)
                        break;
                    buf[nread++] = b;
                    remaining--;
                }
            } while (n >= 0 && remaining > 0);

            return (capacity == nread) ? buf : Arrays.copyOf(buf, nread);
        }

        /**
         * {@inheritDoc}
         */
        public override long transferTo(OutputStream @out)
        {
            throw new NotImplementedException();
            //long transferred = 0L;
            //if (@out is FileOutputStream fos) {
            //    FileChannel fc = getChannel();
            //    long pos = fc.position();
            //    transferred = fc.transferTo(pos, Long.MAX_VALUE, fos.getChannel());
            //    long newPos = pos + transferred;
            //    fc.position(newPos);
            //    if (newPos >= fc.size()) {
            //        return transferred;
            //    }
            //}
            //try {
            //    return Math.addExact(transferred, super.transferTo(out));
            //} catch (ArithmeticException ignore) {
            //    return Long.MAX_VALUE;
            //}
        }

        private long length()
        {
            return fd.Length;
        }

        //private extern long length0();

        private long position()
        {
            return fd.Position;
        }

        //private extern long position0();

        /**
         * Skips over and discards {@code n} bytes of data from the
         * input stream.
         *
         * <p>The {@code skip} method may, for a variety of
         * reasons, end up skipping over some smaller number of bytes,
         * possibly {@code 0}. If {@code n} is negative, the method
         * will try to skip backwards. In case the backing file does not support
         * backward skip at its current position, an {@code IOException} is
         * thrown. The actual number of bytes skipped is returned. If it skips
         * forwards, it returns a positive value. If it skips backwards, it
         * returns a negative value.
         *
         * <p>This method may skip more bytes than what are remaining in the
         * backing file. This produces no exception and the number of bytes skipped
         * may include some number of bytes that were beyond the EOF of the
         * backing file. Attempting to read from the stream after skipping past
         * the end will result in -1 indicating the end of the file.
         *
         * @param      n   {@inheritDoc}
         * @return     the actual number of bytes skipped.
         * @throws     IOException  if n is negative, if the stream does not
         *             support seek, or if an I/O error occurs.
         */
        public override long skip(long n)
        {
            return skip0(n);
        }

        private extern long skip0(long n);

        /**
         * Returns an estimate of the number of remaining bytes that can be read (or
         * skipped over) from this input stream without blocking by the next
         * invocation of a method for this input stream. Returns 0 when the file
         * position is beyond EOF. The next invocation might be the same thread
         * or another thread. A single read or skip of this many bytes will not
         * block, but may read or skip fewer bytes.
         *
         * <p> In some cases, a non-blocking read (or skip) may appear to be
         * blocked when it is merely slow, for example when reading large
         * files over slow networks.
         *
         * @return     an estimate of the number of remaining bytes that can be read
         *             (or skipped over) from this input stream without blocking.
         * @throws     IOException  if this file input stream has been closed by calling
         *             {@code close} or an I/O error occurs.
         */
        public override int available()
        {
            // TODO: This is a hack, what would this be in .NET? -RP
            var result = (int)(fd.Length - fd.Position);
            return result;//. available0();
        }


        /**
         * Closes this file input stream and releases any system resources
         * associated with the stream.
         *
         * <p> If this stream has an associated channel then the channel is closed
         * as well.
         *
         * @apiNote
         * Overriding {@link #close} to perform cleanup actions is reliable
         * only when called directly or when called by try-with-resources.
         *
         * @implSpec
         * Subclasses requiring that resource cleanup take place after a stream becomes
         * unreachable should use the {@link java.lang.ref.Cleaner} mechanism.
         *
         * <p>
         * If this stream has an associated channel then this method will close the
         * channel, which in turn will close this stream. Subclasses that override
         * this method should be prepared to handle possible reentrant invocation.
         *
         * @throws     IOException  {@inheritDoc}
         */
        public override void close()
        {
            if (closed)
            {
                return;
            }

            lock (closeLock)
            {
                if (closed)
                {
                    return;
                }

                closed = true;
            }

            // TODO: FileChannel fc = channel;
            //if (fc != null)
            //{
            //    // possible race with getChannel(), benign since
            //    // FileChannel.close is final and idempotent
            //    fc.close();
            //}

            //fd.closeAll(new Closeable()
            //{
 
            //    public void close() throws IOException {
            //    fd.close();
            //}
            //});

            // TODO: Should we dispose?
            fd.Close();
        }

        /**
         * Returns the {@code FileDescriptor}
         * object  that represents the connection to
         * the actual file in the file system being
         * used by this {@code FileInputStream}.
         *
         * @return     the file descriptor object associated with this stream.
         * @throws     IOException  if an I/O error occurs.
         * @see        java.io.FileDescriptor
         */
        //public FileDescriptor getFD()
        //{
        //    if (fd != null)
        //    {
        //        return fd;
        //    }

        //    throw new IOException();
        //}

        /**
         * Returns the unique {@link java.nio.channels.FileChannel FileChannel}
         * object associated with this file input stream.
         *
         * <p> The initial {@link java.nio.channels.FileChannel#position()
         * position} of the returned channel will be equal to the
         * number of bytes read from the file so far.  Reading bytes from this
         * stream will increment the channel's position.  Changing the channel's
         * position, either explicitly or by reading, will change this stream's
         * file position.
         *
         * @return  the file channel associated with this file input stream
         *
         * @since 1.4
         */
        //public FileChannel getChannel()
        //{
        //    FileChannel fc = this.channel;
        //    if (fc == null)
        //    {
        //        lock (this)
        //        {
        //            fc = this.channel;
        //            if (fc == null)
        //            {
        //                fc = FileChannelImpl.open(fd, path, true, false, false, false, this);
        //                this.channel = fc;
        //                if (closed)
        //                {
        //                    try
        //                    {
        //                        // possible race with close(), benign since
        //                        // FileChannel.close is final and idempotent
        //                        fc.close();
        //                    }
        //                    catch (IOException ioe)
        //                    {
        //                        throw new InternalError(ioe); // should not happen
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return fc;
        //}

        //private static extern void initIDs();

        //static FileInputStream()
        //{
        //    initIDs();
        //}
    }
}