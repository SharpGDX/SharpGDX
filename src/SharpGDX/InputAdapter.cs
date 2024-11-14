namespace SharpGDX;

/// <summary>
///     An adapter class for <see cref="IInputProcessor" />.
/// </summary>
/// <remarks>
///     You can derive from this and only override what you are interested in.
/// </remarks>
public class InputAdapter : IInputProcessor
{
    /// <inheritdoc cref="IInputProcessor.KeyDown(int)" />
    public virtual bool KeyDown(int keycode)
    {
        return false;
    }

    /// <inheritdoc cref="IInputProcessor.KeyTyped(char)" />
    public virtual bool KeyTyped(char character)
    {
        return false;
    }

    /// <inheritdoc cref="IInputProcessor.KeyUp(int)" />
    public virtual bool KeyUp(int keycode)
    {
        return false;
    }

    /// <inheritdoc cref="IInputProcessor.MouseMoved(int, int)" />
    public virtual bool MouseMoved(int screenX, int screenY)
    {
        return false;
    }

    /// <inheritdoc cref="IInputProcessor.Scrolled(float, float)" />
    public virtual bool Scrolled(float amountX, float amountY)
    {
        return false;
    }

    /// <inheritdoc cref="IInputProcessor.TouchCancelled(int, int, int, int)" />
    public virtual bool TouchCancelled(int screenX, int screenY, int pointer, int button)
    {
        return false;
    }

    /// <inheritdoc cref="IInputProcessor.TouchDown(int, int, int, int)" />
    public virtual bool TouchDown(int screenX, int screenY, int pointer, int button)
    {
        return false;
    }

    /// <inheritdoc cref="IInputProcessor.TouchDragged(int, int, int)" />
    public virtual bool TouchDragged(int screenX, int screenY, int pointer)
    {
        return false;
    }

    /// <inheritdoc cref="IInputProcessor.TouchUp(int, int, int, int)" />
    public virtual bool TouchUp(int screenX, int screenY, int pointer, int button)
    {
        return false;
    }
}