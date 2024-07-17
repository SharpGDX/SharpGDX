using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/** Ugly hack to get gwt internal stuff into nio, see StringByteBuffer in nio */

	public interface StringToByteBuffer
	{
		ByteBuffer stringToByteBuffer(String s);
	}
}
