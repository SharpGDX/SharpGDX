using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	/** Provides bit flag constants for alignment.
 * @author Nathan Sweet */
public class Align {
	static public readonly int center = 1 << 0;
	static public readonly int top = 1 << 1;
	static public readonly int bottom = 1 << 2;
	static public readonly int left = 1 << 3;
	static public readonly int right = 1 << 4;

	static public readonly int topLeft = top | left;
	static public readonly int topRight = top | right;
	static public readonly int bottomLeft = bottom | left;
	static public readonly int bottomRight = bottom | right;

	static public  bool isLeft (int align) {
		return (align & left) != 0;
	}

	static public bool isRight (int align) {
		return (align & right) != 0;
	}

	static public bool isTop (int align) {
		return (align & top) != 0;
	}

	static public bool isBottom (int align) {
		return (align & bottom) != 0;
	}

	static public bool isCenterVertical (int align) {
		return (align & top) == 0 && (align & bottom) == 0;
	}

	static public bool isCenterHorizontal (int align) {
		return (align & left) == 0 && (align & right) == 0;
	}

	static public String toString (int align) {
		StringBuilder buffer = new StringBuilder(13);
		if ((align & top) != 0)
			buffer.Append("top,");
		else if ((align & bottom) != 0)
			buffer.Append("bottom,");
		else
			buffer.Append("center,");
		if ((align & left) != 0)
			buffer.Append("left");
		else if ((align & right) != 0)
			buffer.Append("right");
		else
			buffer.Append("center");
		return buffer.ToString();
	}
}
}
