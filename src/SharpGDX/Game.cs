namespace SharpGDX;

/// <summary>
///     An <see cref="IApplicationListener" /> that delegates to a <see cref="IScreen" />.
/// </summary>
/// <remarks>
///     <para>
///         This allows an application to easily have multiple screens.
///     </para>
///     <para>
///         Screens are not disposed automatically. You must handle whether you want to keep screens around or dispose of
///         them when another screen is set.
///     </para>
/// </remarks>
public abstract class Game : IApplicationListener
{
	protected IScreen? Screen;

	/// <inheritdoc cref="IApplicationListener.Create()" />
	public abstract void Create();

	/// <inheritdoc cref="IApplicationListener.Dispose()" />
	public virtual void Dispose()
	{
		Screen?.Hide();
	}

	/// <summary>
	///     Returns the currently active <see cref="IScreen" />.
	/// </summary>
	/// <returns>The currently active <see cref="IScreen" /></returns>
	public IScreen? GetScreen()
	{
		return Screen;
	}

	/// <inheritdoc cref="IApplicationListener.Pause()" />
	public virtual void Pause()
	{
		if (Screen != null)
		{
			Screen.Pause();
		}
	}

	/// <inheritdoc cref="IApplicationListener.Render()" />
	public virtual void Render()
	{
		Screen?.Render(Gdx.graphics.getDeltaTime());
	}

	/// <inheritdoc cref="IApplicationListener.Resize(int, int)" />
	public virtual void Resize(int width, int height)
	{
		Screen?.Resize(width, height);
	}

	/// <inheritdoc cref="IApplicationListener.Resume()" />
	public virtual void Resume()
	{
		Screen?.Resume();
	}

	/// <summary>
	///     Sets the current screen.
	/// </summary>
	/// <remarks>
	///     <see cref="IScreen.Hide()" /> is called on any old screen, and <see cref="IScreen.Show()" /> is called on the new
	///     screen, if any.
	/// </remarks>
	/// <param name="screen">The screen.</param>
	public void SetScreen(IScreen? screen)
	{
		Screen?.Hide();

		Screen = screen;

		Screen?.Show();
		Screen?.Resize(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
	}
}