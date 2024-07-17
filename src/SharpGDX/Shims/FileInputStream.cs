using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class FileInputStream : InputStream
	{
		public FileInputStream(File file)
			: base(System.IO.File.OpenRead(file.getCanonicalPath()))
		{

		}
	}
}
