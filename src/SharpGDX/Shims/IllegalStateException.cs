using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class IllegalStateException : InvalidOperationException
	{
		public IllegalStateException() { }
		public IllegalStateException(string  message) : base(message) { }
	}
}
