using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class LongBuffer : Buffer
	{
		public LongBuffer(int capacity) : base(capacity)
		{
		}

		public override bool isReadOnly()
		{
			throw new NotImplementedException();
		}
	}
}
