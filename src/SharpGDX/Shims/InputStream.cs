namespace SharpGDX.Shims;

public abstract class InputStream : ICloseable
{
    private static readonly int MAX_SKIP_BUFFER_SIZE = 2048;

    private static readonly int DEFAULT_BUFFER_SIZE = 16384;

    private static readonly int MAX_BUFFER_SIZE = int.MaxValue - 8;
    
    public virtual void close()
    {
    }

    public virtual void mark(int readlimit)
    {
    }

    public abstract int read();

    public virtual int read(byte[] b)
    {
        return read(b, 0, b.Length);
    }

    public virtual int read(byte[] b, int off, int len)
    {
        // TODO: Objects.checkFromIndexSize(off, len, b.Length);
        if (len == 0)
        {
            return 0;
        }

        var c = read();
        if (c == -1)
        {
            return -1;
        }

        b[off] = (byte)c;

        var i = 1;
        try
        {
            for (; i < len; i++)
            {
                c = read();
                if (c == -1)
                {
                    break;
                }

                b[off + i] = (byte)c;
            }
        }
        catch (IOException ee)
        {
        }

        return i;
    }

    public virtual byte[] readAllBytes()
    {
        return readNBytes(int.MaxValue);
    }

    public virtual int readNBytes(byte[] b, int off, int len)
    {
        // TODO: Objects.checkFromIndexSize(off, len, b.Length);

        var n = 0;
        while (n < len)
        {
            var count = read(b, off + n, len - n);
            if (count < 0)
            {
                break;
            }

            n += count;
        }

        return n;
    }

    public virtual byte[] readNBytes(int len)
    {
        throw new NotImplementedException();
//        if (len< 0) {
//            throw new IllegalArgumentException("len < 0");
//        }

//        List<byte[]> bufs = null;
//    byte[] result = null;
//    int total = 0;
//    int remaining = len;
//    int n;
//        do {
//            byte[] buf = new byte[Math.Min(remaining, DEFAULT_BUFFER_SIZE)];
//    int nread = 0;

//            // read to EOF which may read more or less than buffer size
//            while ((n = read(buf, nread,
//                    Math.Min(buf.Length - nread, remaining))) > 0) {
//                nread += n;
//                remaining -= n;
//            }

//            if (nread > 0)
//{
//    if (MAX_BUFFER_SIZE - total < nread)
//    {
//        throw new OutOfMemoryError("Required array size too large");
//    }
//    if (nread < buf.length)
//    {
//        buf = Arrays.copyOfRange(buf, 0, nread);
//    }
//    total += nread;
//    if (result == null)
//    {
//        result = buf;
//    }
//    else
//    {
//        if (bufs == null)
//        {
//            bufs = new List<>();
//            bufs.add(result);
//        }
//        bufs.add(buf);
//    }
//}
//            // if the last call to read returned -1 or the number of bytes
//            // requested have been read then break
//        } while (n >= 0 && remaining > 0) ;

//if (bufs == null)
//{
//    if (result == null)
//    {
//        return new byte[0];
//    }
//    return result.length == total ?
//        result : Arrays.copyOf(result, total);
//}

//result = new byte[total];
//int offset = 0;
//remaining = total;
//for (byte[] b : bufs)
//{
//    int count = Math.min(b.length, remaining);
//    System.arraycopy(b, 0, result, offset, count);
//    offset += count;
//    remaining -= count;
//}

//return result;
    }

    public virtual long skip(long n)
    {
        var remaining = n;
        int nr;

        if (n <= 0)
        {
            return 0;
        }

        var size = (int)Math.Min(MAX_SKIP_BUFFER_SIZE, remaining);
        var skipBuffer = new byte[size];
        while (remaining > 0)
        {
            nr = read(skipBuffer, 0, (int)Math.Min(size, remaining));
            if (nr < 0)
            {
                break;
            }

            remaining -= nr;
        }

        return n - remaining;
    }

    public virtual void reset()
    {
        throw new IOException("mark/reset not supported");
    }

    public virtual bool markSupported()
    {
        return false;
    }

    public virtual long transferTo(OutputStream @out)
    {
        throw new NotImplementedException();
//        Objects.requireNonNull(@out, "out");
//    long transferred = 0;
//    byte[] buffer = new byte[DEFAULT_BUFFER_SIZE];
//    int read;
//        while ((read = this.read(buffer, 0, DEFAULT_BUFFER_SIZE)) >= 0) {
//       @out.write(buffer, 0, read);
//        if (transferred<long.MaxValue) {
//            try {
//                transferred = Math.addExact(transferred, read);
//            } catch (ArithmeticException ignore) {
//                transferred = Long.MAX_VALUE;
//            }
//        }
//    }
//return transferred;
    }

    public void skipNBytes(long n)
    {
        while (n > 0)
        {
            var ns = skip(n);
            if (ns > 0 && ns <= n)
            {
                // adjust number to skip
                n -= ns;
            }
            else if (ns == 0)
            {
                // no bytes skipped
                // read one byte to check for EOS
                if (read() == -1)
                {
                    throw new EOFException();
                }

                // one byte read so decrement number to skip
                n--;
            }
            else
            {
                // skipped negative or too many bytes
                throw new IOException("Unable to skip exactly");
            }
        }
    }

    public virtual int available()
    {
        return 0;
    }
}