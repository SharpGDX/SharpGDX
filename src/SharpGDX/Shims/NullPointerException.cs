using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class NullPointerException : Exception
	{
		public NullPointerException() { }
		public NullPointerException(string message) : base(message) { }
	}
}
