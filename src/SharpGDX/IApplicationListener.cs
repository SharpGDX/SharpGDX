namespace SharpGDX;

/// <summary>
///     An <see cref="IApplicationListener" /> is called when the {@link Application} is created, resumed, rendering,
///     paused or destroyed.
/// </summary>
/// <remarks>
///     <para>
///         All methods are called in a thread that has the OpenGL context current. You can thus safely create and
///         manipulate graphics resources.
///     </para>
///     <para>
///         The <see cref="IApplicationListener" /> interface follows the standard Android activity life-cycle and is
///         emulated on the desktop accordingly.
///     </para>
/// </remarks>
public interface IApplicationListener
{
    /// <summary>
    ///     Called when the <see cref="IApplication" /> is first created.
    /// </summary>
    public void Create();

    /// <summary>
    ///     Called when the <see cref="IApplication" /> is destroyed. Preceded by a call to <see cref="Pause()" />.
    /// </summary>
    public void Dispose();

    /// <summary>
    ///     Called when the <see cref="IApplication" /> is paused, usually when it's not active or visible on-screen.
    /// </summary>
    /// <remarks>
    ///     An Application is also paused before it is destroyed.
    /// </remarks>
    public void Pause();

    /// <summary>
    ///     Called when the <see cref="IApplication" /> should render itself.
    /// </summary>
    public void Render();

    /// <summary>
    ///     Called when the <see cref="IApplication" /> is resized.
    /// </summary>
    /// <remarks>
    ///     This can happen at any point during a non-paused state but will never happen before a call to
    ///     <see cref="Create()" />.
    /// </remarks>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void Resize(int width, int height);

    /// <summary>
    ///     Called when the <see cref="IApplication" /> is resumed from a paused state, usually when it regains focus.
    /// </summary>
    public void Resume();
}