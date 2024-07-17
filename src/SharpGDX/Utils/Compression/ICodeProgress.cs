using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils.Compression
{
	public interface ICodeProgress
	{
		public void SetProgress(long inSize, long outSize);
	}
}
