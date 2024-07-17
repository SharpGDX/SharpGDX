using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class UnsupportedOperationException : Exception
	{
		public UnsupportedOperationException() { }

		public UnsupportedOperationException(string message) : base(message) { }
	}
}
