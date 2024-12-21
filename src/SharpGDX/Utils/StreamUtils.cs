using SharpGDX.Shims;
using System.Runtime.InteropServices;
using System;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Utils
{
    /** Provides utility methods to copy streams. */
    public sealed class StreamUtils
    {
        public static readonly int DEFAULT_BUFFER_SIZE = 4096;
        public static readonly byte[] EMPTY_BYTES = new byte[0];

        /** Allocates a {@value #DEFAULT_BUFFER_SIZE} byte[] for use as a temporary buffer and calls
         * {@link #copyStream(InputStream, OutputStream, byte[])}. */
        public static void copyStream(InputStream input, OutputStream output) 
        {
            copyStream(input, output, new byte[DEFAULT_BUFFER_SIZE]);
	}

	/** Allocates a byte[] of the specified size for use as a temporary buffer and calls
	 * {@link #copyStream(InputStream, OutputStream, byte[])}. */
	public static void copyStream(InputStream input, OutputStream output, int bufferSize) 
        {
            copyStream(input, output, new byte[bufferSize]);
	}

/** Copy the data from an {@link InputStream} to an {@link OutputStream}, using the specified byte[] as a temporary buffer. The
 * stream is not closed. */
public static void copyStream(InputStream input, OutputStream output, byte[] buffer) 
{
		int bytesRead;
		while ((bytesRead = input.read(buffer)) != -1) {
        output.write(buffer, 0, bytesRead);
    }
}

/** Allocates a {@value #DEFAULT_BUFFER_SIZE} byte[] for use as a temporary buffer and calls
 * {@link #copyStream(InputStream, OutputStream, byte[])}. */
public static void copyStream(InputStream input, ByteBuffer output) 
{
    copyStream(input, output, new byte[DEFAULT_BUFFER_SIZE]);
	}

	/** Allocates a byte[] of the specified size for use as a temporary buffer and calls
	 * {@link #copyStream(InputStream, ByteBuffer, byte[])}. */
	public static void copyStream(InputStream input, ByteBuffer output, int bufferSize) 
{
    copyStream(input, output, new byte[bufferSize]);
	}

	/** Copy the data from an {@link InputStream} to a {@link ByteBuffer}, using the specified byte[] as a temporary buffer. The
	 * buffer's limit is increased by the number of bytes copied, the position is left unchanged. The stream is not closed.
	 * @param output Must be a direct Buffer with native byte order and the buffer MUST be large enough to hold all the bytes in
	 *           the stream. No error checking is performed.
	 * @return the number of bytes copied. */
	public static int copyStream(InputStream input, ByteBuffer output, byte[] buffer) 
{
		int startPosition = output.position(), total = 0, bytesRead;
		while ((bytesRead = input.read(buffer)) != -1) {
        BufferUtils.copy(buffer, 0, output, bytesRead);
        total += bytesRead;
        ((Buffer)output).position(startPosition + total);
    }
		((Buffer)output).position(startPosition);
		return total;
}

/** Copy the data from an {@link InputStream} to a byte array. The stream is not closed. */
public static byte[] copyStreamToByteArray(InputStream input) 
{
		return copyStreamToByteArray(input, input.available());
}

/** Copy the data from an {@link InputStream} to a byte array. The stream is not closed.
 * @param estimatedSize Used to allocate the output byte[] to possibly avoid an array copy. */
public static byte[] copyStreamToByteArray(InputStream input, int estimatedSize) 
{
    ByteArrayOutputStream baos = new OptimizedByteArrayOutputStream(Math.Max(0, estimatedSize));
copyStream(input, baos);
return baos.toByteArray();
	}

	/** Calls {@link #copyStreamToString(InputStream, int, String)} using the input's {@link InputStream#available() available}
	 * size and the platform's default charset. */
	public static String copyStreamToString(InputStream input) 
{
		return copyStreamToString(input, input.available(), null);
}

/** Calls {@link #copyStreamToString(InputStream, int, String)} using the platform's default charset. */
public static String copyStreamToString(InputStream input, int estimatedSize) 
{
		return copyStreamToString(input, estimatedSize, null);
}

/** Copy the data from an {@link InputStream} to a string using the specified charset.
 * @param estimatedSize Used to allocate the output buffer to possibly avoid an array copy.
 * @param charset May be null to use the platform's default charset. */
public static String copyStreamToString(InputStream input, int estimatedSize, String? charset)
{
    throw new NotImplementedException();
// TODO:   InputStreamReader reader = charset == null ? new InputStreamReader(input) : new InputStreamReader(input, charset);
//StringWriter writer = new StringWriter(Math.Max(0, estimatedSize));
//char[] buffer = new char[DEFAULT_BUFFER_SIZE];
//int charsRead;
//while ((charsRead = reader.read(buffer)) != -1)
//{
//    writer.write(buffer, 0, charsRead);
//}
//return writer.toString();
//	}
}

/** Close and ignore all errors. */
    public static void closeQuietly(ICloseable c)
        {
            if (c != null)
            {
                try
                {
                    c.close();
                }
                catch (Exception ignored)
                {
                }
            }
        }

/// <summary>
/// A ByteArrayOutputStream which avoids copying of the byte array if possible.
/// </summary>
public class OptimizedByteArrayOutputStream : ByteArrayOutputStream
{

        public OptimizedByteArrayOutputStream (int initialSize)
        : base(initialSize)
    {
    
}

        public override byte[] toByteArray()
{
    lock (this)
    {
        if (count == buf.Length) return buf;
        return base.toByteArray();
    }
}

public byte[] getBuffer()
{
    return buf;
}
	}
}
}