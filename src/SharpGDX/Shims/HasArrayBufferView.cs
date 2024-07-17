using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public interface HasArrayBufferView
	{

		//public ArrayBufferView getTypedArray();

		public int getElementSize();
	}
}
