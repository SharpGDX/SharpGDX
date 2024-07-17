using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class NoSuchElementException : Exception
	{
		public NoSuchElementException() { }
		public NoSuchElementException(string message) : base(message) { }
	}
}
