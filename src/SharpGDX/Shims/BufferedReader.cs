using SharpGDX.Shims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class BufferedReader : Reader
	{
		private readonly Reader @in;
		private readonly char[] buffer;
		private int position;
		private int limit;

		public BufferedReader(Reader @in, int bufferSize)
		{
			this.@in = @in;
			this.buffer = new char[bufferSize];
		}

		public BufferedReader(Reader @in)
			: this(@in, 8192)
		{

		}

		private void fill() // TODO: throws IOException
		{
			position = 0;
			limit = @in.read(buffer);
		}

		public String readLine() // TODO: throws IOException
		{
			StringBuilder sb = new StringBuilder();
			while (true)
			{
				if (position >= limit)
				{
					fill();
				}

				if (position >= limit)
				{
					return sb.Length == 0 ? null : sb.ToString();
				}

				for (int i = position; i < limit; ++i)
				{
					if (buffer[i] == '\r')
					{
						sb.Append(buffer, position, i - position);
						position = i + 1;
						if (i + 1 < limit)
						{
							if (buffer[i + 1] == '\n')
							{
								position = i + 2;
							}
						}
						else
						{
							fill();
							if (buffer[position] == '\n')
							{
								position += 1;
							}
						}

						return sb.ToString();
					}
					else if (buffer[i] == '\n')
					{
						sb.Append(buffer, position, i - position);
						position = i + 1;
						return sb.ToString();
					}
				}

				sb.Append(buffer, position, limit - position);
				position = limit;
			}
		}

		public override int read(char[] b, int offset, int length) // TODO: throws IOException
		{
			int count = 0;

			if (position >= limit && length < buffer.Length)
			{
				fill();
			}

			if (position < limit)
			{
				int remaining = limit - position;
				if (remaining > length)
				{
					remaining = length;
				}

				Array.Copy(buffer, position, b, offset, remaining);

				count += remaining;
				position += remaining;
				offset += remaining;
				length -= remaining;
			}

			if (length > 0)
			{
				int c = @in.read(b, offset, length);
				if (c == -1)
				{
					if (count == 0)
					{
						count = -1;
					}
				}
				else
				{
					count += c;
				}
			}

			return count;
		}

		public override void close() // TODO: throws IOException
		{
			@in.close();
		}
	}
}