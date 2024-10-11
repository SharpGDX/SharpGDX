namespace SharpGDX;

/// <summary>
///     Convenience implementation of <see cref="IScreen" />.
/// </summary>
/// <remarks>
///     Derive from this and only override what you need.
/// </remarks>
/// TODO: Why wouldn't this be abstract? -RP
public class ScreenAdapter : IScreen
{
	public virtual void Dispose()
	{
	}

	public virtual void Hide()
	{
	}

	public virtual void Pause()
	{
	}

	public virtual void Render(float delta)
	{
	}

	public virtual void Resize(int width, int height)
	{
	}

	public virtual void Resume()
	{
	}

	public virtual void Show()
	{
	}
}