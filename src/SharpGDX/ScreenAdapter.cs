namespace SharpGDX;

/// <summary>
///     Convenience implementation of <see cref="IScreen" />.
/// </summary>
/// <remarks>
///     Derive from this and only override what you need.
/// </remarks>
public class ScreenAdapter : IScreen
{
	public void Dispose()
	{
	}

	public void Hide()
	{
	}

	public void Pause()
	{
	}

	public void Render(float delta)
	{
	}

	public void Resize(int width, int height)
	{
	}

	public void Resume()
	{
	}

	public void Show()
	{
	}
}