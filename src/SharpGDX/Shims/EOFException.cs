using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class EOFException : Exception
	{
		public EOFException() { }

		public EOFException (string message) : base(message) { }
	}
}
