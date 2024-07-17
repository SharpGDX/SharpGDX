namespace SharpGDX.Desktop;

/// <summary>
///     Receives notifications of various window events, such as iconification, focus loss and gain, and window close
///     events. Can be set per window via <see cref="DesktopApplicationConfiguration" /> and
///     <see cref="DesktopWindowConfiguration" />. Close events can be canceled by returning false.
/// </summary>
public interface IDesktopWindowListener
{
	/// <summary>
	///     Called when the user requested to close the window, e.g. clicking the close button or pressing the window closing
	///     keyboard shortcut.
	/// </summary>
	/// <returns>Whether the window should actually close.</returns>
	public bool CloseRequested();

	/// <summary>
	///     Called after the GLFW window is created. Before this callback is received, it's unsafe to use any
	///     <see cref="DesktopWindow" /> member functions which, for their part, involve calling GLFW functions.
	/// </summary>
	/// <remarks>
	///     <para>
	///         For the main window, this is an immediate callback from inside
	///         <see cref="DesktopApplication(IApplicationListener, DesktopApplicationConfiguration)" />.
	///     </para>
	///     <para>
	///         See <see cref="DesktopApplication.newWindow(IApplicationListener, DesktopWindowConfiguration)" />.
	///     </para>
	/// </remarks>
	/// <param name="window">window the window instance.</param>
	public void Created(DesktopWindow window);

	/// <summary>
	///     Called when external files are dropped into the window, e.g from the Desktop.
	/// </summary>
	/// <param name="files">Array with absolute paths to the files.</param>
	public void FilesDropped(string[] files);

	/// <summary>
	///     Called when the window gained focus.
	/// </summary>
	public void FocusGained();

	/// <summary>
	///     Called when the window lost focus to another window. The window's <see cref="IApplicationListener" /> will continue
	///     to be called.
	/// </summary>
	public void FocusLost();

	/// <summary>
	///     Called when the window is iconified (i.e. its minimize button was clicked), or when restored from the iconified
	///     state. When a window becomes iconified, its <see cref="IApplicationListener" /> will be paused, and when restored
	///     it
	///     will be resumed.
	/// </summary>
	/// <param name="isIconified">
	///     <see langword="true" /> if window is iconified, <see langword="false" /> if it leaves the
	///     iconified state
	/// </param>
	public void Iconified(bool isIconified);

	/// <summary>
	///     Called when the window is maximized, or restored from the maximized state.
	/// </summary>
	/// <param name="isMaximized">
	///     <see langword="true" /> if window is maximized, <see langword="false" /> if it leaves the
	///     maximized state.
	/// </param>
	public void Maximized(bool isMaximized);

	/// <summary>
	///     Called when the window content is damaged and needs to be refreshed. When this occurs,
	///     <see cref="DesktopGraphics.requestRendering()" /> is automatically called.
	/// </summary>
	public void RefreshRequested();
}