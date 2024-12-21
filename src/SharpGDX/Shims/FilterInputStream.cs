namespace SharpGDX.Shims;

public class FilterInputStream : InputStream
{
    protected volatile InputStream @in;

    protected FilterInputStream(InputStream @in)
    {
        this.@in = @in;
    }

    public override int read()
    {
        return @in.read();
    }

    public override int read(byte[] b)
    {
        return read(b, 0, b.Length);
    }

    public override int read(byte[] b, int off, int len)
    {
        return @in.read(b, off, len);
    }

    public override long skip(long n)
    {
        return @in.skip(n);
    }

    public override int available()
    {
        return @in.available();
    }

    public override void close()
    {
        @in.close();
    }

    public override void mark(int readlimit)
    {
        @in.mark(readlimit);
    }

    public override void reset()
    {
        @in.reset();
    }

    public override bool markSupported()
    {
        return @in.markSupported();
    }
}