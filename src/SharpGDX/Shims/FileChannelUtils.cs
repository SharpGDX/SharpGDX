using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	internal static class FileChannelUtils
	{
		public static ByteBuffer map(this FileChannel channel, FileChannel.MapMode model, int offset, long length)
		{
			throw new NotImplementedException();
		}
	}
}
