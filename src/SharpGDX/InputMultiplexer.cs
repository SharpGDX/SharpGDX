using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX;

/// <summary>
///     An <see cref="IInputProcessor" /> that delegates to an ordered list of other InputProcessors.
/// </summary>
/// <remarks>
///     Delegation for an event stops if a processor returns true, which indicates that the event was handled.
/// </remarks>
public class InputMultiplexer : IInputProcessor
{
    private readonly SnapshotArray<IInputProcessor> _processors = new(4);

    public InputMultiplexer()
    {
    }

    public InputMultiplexer(IInputProcessor[] processors)
    {
        _processors.addAll(processors);
    }

    public void AddProcessor(int index, IInputProcessor processor)
    {
        if (processor == null)
        {
            throw new NullPointerException("processor cannot be null");
        }

        _processors.insert(index, processor);
    }

    public void AddProcessor(IInputProcessor processor)
    {
        if (processor == null)
        {
            throw new NullPointerException("processor cannot be null");
        }

        _processors.Add(processor);
    }

    public void Clear()
    {
        _processors.clear();
    }

    public SnapshotArray<IInputProcessor> GetProcessors()
    {
        return _processors;
    }

    public bool KeyDown(int keycode)
    {
        object[] items = _processors.begin();

        try
        {
            for (int i = 0, n = _processors.size; i < n; i++)
            {
                if (((IInputProcessor)items[i]).KeyDown(keycode))
                {
                    return true;
                }
            }
        }
        finally
        {
            _processors.end();
        }

        return false;
    }

    public bool KeyTyped(char character)
    {
        object[] items = _processors.begin();

        try
        {
            for (int i = 0, n = _processors.size; i < n; i++)
            {
                if (((IInputProcessor)items[i]).KeyTyped(character))
                {
                    return true;
                }
            }
        }
        finally
        {
            _processors.end();
        }

        return false;
    }

    public bool KeyUp(int keycode)
    {
        object[] items = _processors.begin();

        try
        {
            for (int i = 0, n = _processors.size; i < n; i++)
            {
                if (((IInputProcessor)items[i]).KeyUp(keycode))
                {
                    return true;
                }
            }
        }
        finally
        {
            _processors.end();
        }

        return false;
    }

    public bool MouseMoved(int screenX, int screenY)
    {
        object[] items = _processors.begin();

        try
        {
            for (int i = 0, n = _processors.size; i < n; i++)
            {
                if (((IInputProcessor)items[i]).MouseMoved(screenX, screenY))
                {
                    return true;
                }
            }
        }
        finally
        {
            _processors.end();
        }

        return false;
    }

    public void RemoveProcessor(int index)
    {
        _processors.RemoveIndex(index);
    }

    public void RemoveProcessor(IInputProcessor processor)
    {
        _processors.RemoveValue(processor, true);
    }

    public bool Scrolled(float amountX, float amountY)
    {
        object[] items = _processors.begin();

        try
        {
            for (int i = 0, n = _processors.size; i < n; i++)
            {
                if (((IInputProcessor)items[i]).Scrolled(amountX, amountY))
                {
                    return true;
                }
            }
        }
        finally
        {
            _processors.end();
        }

        return false;
    }

    public void SetProcessors(IInputProcessor[] processors)
    {
        _processors.clear();
        _processors.addAll(processors);
    }

    public void SetProcessors(Array<IInputProcessor> processors)
    {
        _processors.clear();
        _processors.addAll(processors);
    }

    /// <summary>
    ///     The number of processors in this multiplexer.
    /// </summary>
    /// <returns>The number of processors in this multiplexer.</returns>
    public int Size()
    {
        return _processors.size;
    }

    public bool TouchCancelled(int screenX, int screenY, int pointer, int button)
    {
        object[] items = _processors.begin();

        try
        {
            for (int i = 0, n = _processors.size; i < n; i++)
            {
                if (((IInputProcessor)items[i]).TouchCancelled(screenX, screenY, pointer, button))
                {
                    return true;
                }
            }
        }
        finally
        {
            _processors.end();
        }

        return false;
    }

    public bool TouchDown(int screenX, int screenY, int pointer, int button)
    {
        object[] items = _processors.begin();

        try
        {
            for (int i = 0, n = _processors.size; i < n; i++)
            {
                if (((IInputProcessor)items[i]).TouchDown(screenX, screenY, pointer, button))
                {
                    return true;
                }
            }
        }
        finally
        {
            _processors.end();
        }

        return false;
    }

    public bool TouchDragged(int screenX, int screenY, int pointer)
    {
        object[] items = _processors.begin();

        try
        {
            for (int i = 0, n = _processors.size; i < n; i++)
            {
                if (((IInputProcessor)items[i]).TouchDragged(screenX, screenY, pointer))
                {
                    return true;
                }
            }
        }
        finally
        {
            _processors.end();
        }

        return false;
    }

    public bool TouchUp(int screenX, int screenY, int pointer, int button)
    {
        object[] items = _processors.begin();

        try
        {
            for (int i = 0, n = _processors.size; i < n; i++)
            {
                if (((IInputProcessor)items[i]).TouchUp(screenX, screenY, pointer, button))
                {
                    return true;
                }
            }
        }
        finally
        {
            _processors.end();
        }

        return false;
    }
}