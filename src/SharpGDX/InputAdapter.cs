using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX
{
	/** An adapter class for {@link InputProcessor}. You can derive from this and only override what you are interested in.
 *
 * @author mzechner */
	public class InputAdapter : IInputProcessor
	{
	public virtual bool KeyDown(int keycode)
	{
		return false;
	}

	public virtual bool KeyUp(int keycode)
	{
		return false;
	}

	public virtual bool KeyTyped(char character)
	{
		return false;
	}

	public virtual bool TouchDown(int screenX, int screenY, int pointer, int button)
	{
		return false;
	}

	public virtual bool TouchUp(int screenX, int screenY, int pointer, int button)
	{
		return false;
	}

	public virtual bool TouchCancelled(int screenX, int screenY, int pointer, int button)
	{
		return false;
	}

	public virtual bool TouchDragged(int screenX, int screenY, int pointer)
	{
		return false;
	}

	public virtual bool MouseMoved(int screenX, int screenY)
	{
		return false;
	}

	public virtual bool Scrolled(float amountX, float amountY)
	{
		return false;
	}
	}
}
