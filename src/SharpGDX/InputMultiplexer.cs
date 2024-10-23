using SharpGDX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Input;
using SharpGDX.Shims;

namespace SharpGDX
{
	/** An {@link InputProcessor} that delegates to an ordered list of other InputProcessors. Delegation for an event stops if a
 * processor returns true, which indicates that the event was handled.
 * @author Nathan Sweet */
	public class InputMultiplexer : IInputProcessor
	{
	private SnapshotArray<IInputProcessor> processors = new (4);

	public InputMultiplexer()
	{
	}

	public InputMultiplexer(IInputProcessor[]processors)
	{
		this.processors.addAll(processors);
	}

	public void AddProcessor(int index, IInputProcessor processor)
	{
		if (processor == null) throw new NullPointerException("processor cannot be null");
		processors.insert(index, processor);
	}

	public void RemoveProcessor(int index)
	{
		processors.removeIndex(index);
	}

	public void AddProcessor(IInputProcessor processor)
	{
		if (processor == null) throw new NullPointerException("processor cannot be null");
		processors.add(processor);
	}

	public void RemoveProcessor(IInputProcessor processor)
	{
		processors.removeValue(processor, true);
	}

	/** @return the number of processors in this multiplexer */
	public int Size()
	{
		return processors.size;
	}

	public void Clear()
	{
		processors.clear();
	}

	public void SetProcessors(IInputProcessor[]processors)
	{
		this.processors.clear();
		this.processors.addAll(processors);
	}

	public void SetProcessors(Array<IInputProcessor> processors)
	{
		this.processors.clear();
		this.processors.addAll(processors);
	}

	public SnapshotArray<IInputProcessor> GetProcessors()
	{
		return processors;
	}

	public bool KeyDown(int keycode)
	{
		Object[] items = processors.begin();
		try
		{
			for (int i = 0, n = processors.size; i < n; i++)
				if (((IInputProcessor)items[i]).KeyDown(keycode)) return true;
		}
		finally
		{
			processors.end();
		}
		return false;
	}

	public bool KeyUp(int keycode)
	{
		Object[] items = processors.begin();
		try
		{
			for (int i = 0, n = processors.size; i < n; i++)
				if (((IInputProcessor)items[i]).KeyUp(keycode)) return true;
		}
		finally
		{
			processors.end();
		}
		return false;
	}

	public bool KeyTyped(char character)
	{
		Object[] items = processors.begin();
		try
		{
			for (int i = 0, n = processors.size; i < n; i++)
				if (((IInputProcessor)items[i]).KeyTyped(character)) return true;
		}
		finally
		{
			processors.end();
		}
		return false;
	}

	public bool TouchDown(int screenX, int screenY, int pointer, int button)
	{
		Object[] items = processors.begin();
		try
		{
			for (int i = 0, n = processors.size; i < n; i++)
				if (((IInputProcessor)items[i]).TouchDown(screenX, screenY, pointer, button)) return true;
		}
		finally
		{
			processors.end();
		}
		return false;
	}

	public bool TouchUp(int screenX, int screenY, int pointer, int button)
	{
		Object[] items = processors.begin();
		try
		{
			for (int i = 0, n = processors.size; i < n; i++)
				if (((IInputProcessor)items[i]).TouchUp(screenX, screenY, pointer, button)) return true;
		}
		finally
		{
			processors.end();
		}
		return false;
	}

	public bool TouchCancelled(int screenX, int screenY, int pointer, int button)
	{
		Object[] items = processors.begin();
		try
		{
			for (int i = 0, n = processors.size; i < n; i++)
				if (((IInputProcessor)items[i]).TouchCancelled(screenX, screenY, pointer, button)) return true;
		}
		finally
		{
			processors.end();
		}
		return false;
	}

	public bool TouchDragged(int screenX, int screenY, int pointer)
	{
		Object[] items = processors.begin();
		try
		{
			for (int i = 0, n = processors.size; i < n; i++)
				if (((IInputProcessor)items[i]).TouchDragged(screenX, screenY, pointer)) return true;
		}
		finally
		{
			processors.end();
		}
		return false;
	}

	public bool MouseMoved(int screenX, int screenY)
	{
		Object[] items = processors.begin();
		try
		{
			for (int i = 0, n = processors.size; i < n; i++)
				if (((IInputProcessor)items[i]).MouseMoved(screenX, screenY)) return true;
		}
		finally
		{
			processors.end();
		}
		return false;
	}

	public bool Scrolled(float amountX, float amountY)
	{
		Object[] items = processors.begin();
		try
		{
			for (int i = 0, n = processors.size; i < n; i++)
				if (((IInputProcessor)items[i]).Scrolled(amountX, amountY)) return true;
		}
		finally
		{
			processors.end();
		}
		return false;
	}
}
}
