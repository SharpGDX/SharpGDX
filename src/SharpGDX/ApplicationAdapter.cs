namespace SharpGDX;

/// <summary>
///     Convenience implementation of <see cref="IApplicationListener" />.
/// </summary>
/// <remarks>
///     Derive from this and only override what you need.
/// </remarks>
public abstract class ApplicationAdapter : IApplicationListener
{
	public virtual void Create()
	{
	}

	public virtual void Dispose()
	{
	}

	public virtual void Pause()
	{
	}

	public virtual void Render()
	{
	}

	public virtual void Resize(int width, int height)
	{
	}

	public virtual void Resume()
	{
	}
}