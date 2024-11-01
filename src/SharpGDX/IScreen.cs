using SharpGDX.Utils;

namespace SharpGDX;

/// <summary>
///     Represents one of many application screens, such as a main menu, a settings menu, the game screen and so on.
/// </summary>
/// <remarks>
///     Note that <see cref="Dispose()" /> is not called automatically.
/// </remarks>
public interface IScreen : Disposable
{
    /// <summary>
    ///     Called when this screen should release all resources.
    /// </summary>
    /// TODO: Should this really have a 'new'? -RP
    public new void Dispose();

    /// <summary>
    ///     Called when this screen is no longer the current screen for a <see cref="Game" />.
    /// </summary>
    public void Hide();

    /// <inheritdoc cref="IApplicationListener.Pause()" />
    public void Pause();

    /// <summary>
    ///     Called when the screen should render itself.
    /// </summary>
    /// <param name="delta">The time in seconds since the last render.</param>
    public void Render(float delta);

    /// <inheritdoc cref="IApplicationListener.Resize(int, int)" />
    public void Resize(int width, int height);

    /// <inheritdoc cref="IApplicationListener.Resume()" />
    public void Resume();

    /// <summary>
    ///     Called when this screen becomes the current screen for a <see cref="Game" />.
    /// </summary>
    public void Show();
}