namespace SharpGDX;

/// <summary>
///     Convenience implementation of <see cref="IApplicationListener" />.
/// </summary>
/// <remarks>
///     Derive from this and only override what you need.
/// </remarks>
public abstract class ApplicationAdapter : IApplicationListener
{
    /// <inheritdoc cref="IApplicationListener.Create()" />
    public virtual void Create()
    {
    }

    /// <inheritdoc cref="IApplicationListener.Dispose()" />
    public virtual void Dispose()
    {
    }

    /// <inheritdoc cref="IApplicationListener.Pause()" />
    public virtual void Pause()
    {
    }

    /// <inheritdoc cref="IApplicationListener.Render()" />
    public virtual void Render()
    {
    }

    /// <inheritdoc cref="IApplicationListener.Resize(int, int)" />
    public virtual void Resize(int width, int height)
    {
    }

    /// <inheritdoc cref="IApplicationListener.Resume()" />
    public virtual void Resume()
    {
    }
}