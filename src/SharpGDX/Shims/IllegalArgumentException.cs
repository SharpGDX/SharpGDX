using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class IllegalArgumentException : ArgumentException
	{
		public IllegalArgumentException() { }
		public IllegalArgumentException(string message) : base(message) { }
	}
}
