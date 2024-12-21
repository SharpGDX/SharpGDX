using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
    public class ClassNotFoundException : Exception
    {

    }

    public class InvalidClassException : Exception
    {
        public InvalidClassException(string message) : base(message)
        {

        }

    }

    public class InvalidObjectException : Exception
    {
        public InvalidObjectException(string message) : base(message)
        {

        }
    }

    public class ExceptionInInitializerError : Exception
    {
        public ExceptionInInitializerError(Exception exception) : base("Some message", exception){}
    }

    public class ObjectInputStream : InputStream
    {
        internal readonly Stream _stream;

        protected ObjectInputStream()
        {
        }

        public ObjectInputStream(InputStream stream)
        {
        }

        public object readObject()
        {
            throw new NotImplementedException();
        }

        public ObjectInputStream(Stream stream)
        {
            _stream = stream;
        }

        public override int available()
        {
            // TODO: This seems wrong. -RP
            return (int)_stream.Length;
        }

        public override int read()
        {
            throw new NotImplementedException();
        }

        public override int read(byte[] buffer)
        {
            // TODO: I don't like this. -RP
            if (_stream.Position == _stream.Length)
            {
                return -1;
            }

            return _stream.Read(buffer);
        }

        public override int read(byte[] buffer, int offset, int length)
        {
            return _stream.Read(buffer, offset, length);
        }

        public override long skip(long n)
        {
            long result = 0;

            while (n > 0)
            {
                n--;

                result += _stream.ReadByte();
            }

            return result;
        }

        public override void close()
        {
            throw new NotImplementedException();
        }
    }
}
