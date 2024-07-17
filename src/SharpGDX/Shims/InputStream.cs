using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class InputStream: ICloseable
	{
		internal readonly Stream _stream;

		protected InputStream()
		{
		}

		public InputStream(Stream stream)
		{
			_stream = stream;
		}

		public int available()
		{
			// TODO: This seems wrong. -RP
			return (int)_stream.Length;
		}

		public virtual int read()
		{
			throw new NotImplementedException();
		}

		public virtual int read(byte[] buffer)
		{
			// TODO: I don't like this. -RP
			if (_stream.Position == _stream.Length)
			{
				return -1;
			}

			return _stream.Read(buffer);
		}

		public virtual int read(byte[] buffer, int offset, int length)
		{
			return _stream.Read(buffer, offset, length);
		}

		public long skip(long n)
		{
			long result = 0;

			while (n > 0)
			{
				n--;

				result += _stream.ReadByte();
			}

			return result;
		}

		public virtual void close()
		{
			throw new NotImplementedException();
		}
	}
}
