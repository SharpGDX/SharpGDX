using SharpGDX.Files;
using SharpGDX.Graphics;
using static SharpGDX.IGraphics;

namespace SharpGDX.Desktop;

public class DesktopWindowConfiguration
{
    public   string                             Title;
    public   int                                WindowHeight = 480;
    public   int                                WindowWidth  = 640;
    internal bool                               AutoIconify  = true;
    internal DesktopGraphics.DesktopDisplayMode FullscreenMode;
    internal Color                              InitialBackgroundColor = Color.Black;
    internal bool                               InitialVisible         = true;
    internal DesktopGraphics.DesktopMonitor     MaximizedMonitor;
    internal bool                               VSyncEnabled    = true;
    internal bool                               WindowDecorated = true;
    internal FileType                           WindowIconFileType;
    internal string[]?                          WindowIconPaths;
    internal IDesktopWindowListener             WindowListener;
    internal int                                WindowMaxHeight = -1;
    internal bool                               WindowMaximized;
    internal int                                WindowMaxWidth  = -1;
    internal int                                WindowMinHeight = -1;
    internal int                                WindowMinWidth  = -1;
    internal bool                               WindowResizable = true;
    internal int                                WindowX         = -1;
    internal int                                WindowY         = -1;

    /// <summary>
    /// </summary>
    /// <remarks>
    ///     (default true) Does nothing in windowed mode.
    /// </remarks>
    /// <param name="autoIconify">
    ///     Whether the window should automatically iconify and restore previous video mode on input
    ///     focus loss.
    /// </param>
    public void SetAutoIconify(bool autoIconify)
    {
        AutoIconify = autoIconify;
    }

    /// <summary>
    /// </summary>
    /// <param name="decorated">Whether the windowed mode window is decorated, i.e. displaying the title bars (default true)</param>
    public void SetDecorated(bool decorated)
    {
        WindowDecorated = decorated;
    }

    /// <summary>
    ///     Sets the app to use fullscreen mode.
    /// </summary>
    /// <remarks>
    ///     Use the static methods like <see cref="DesktopApplicationConfiguration.GetDisplayMode()" /> on this class to
    ///     enumerate connected monitors and their fullscreen display modes.
    /// </remarks>
    /// <param name="mode"></param>
    public void SetFullscreenMode(DisplayMode mode)
    {
        FullscreenMode = (DesktopGraphics.DesktopDisplayMode)mode;
    }

    /// <summary>
    ///     Sets the initial background color.
    /// </summary>
    /// <remarks>
    ///     Defaults to black.
    /// </remarks>
    /// <param name="color"></param>
    public void SetInitialBackgroundColor(Color color)
    {
        InitialBackgroundColor = color;
    }

    /// <summary>
    /// </summary>
    /// <param name="visibility">Whether the window will be visible on creation. (default true)</param>
    public virtual void SetInitialVisible(bool visibility)
    {
        InitialVisible = visibility;
    }

    /// <summary>
    /// </summary>
    /// <remarks>
    ///     Ignored if the window is full screen. (default false)
    /// </remarks>
    /// <param name="maximized">Whether the window starts maximized.</param>
    public void SetMaximized(bool maximized)
    {
        WindowMaximized = maximized;
    }

    /// <summary>
    /// </summary>
    /// <param name="monitor">What monitor the window should maximize to.</param>
    public void SetMaximizedMonitor(IGraphics.Monitor monitor)
    {
        MaximizedMonitor = (DesktopGraphics.DesktopMonitor)monitor;
    }

    /// <summary>
    /// </summary>
    /// <param name="resizable">Whether the windowed mode window is resizable (default true)</param>
    public void SetResizable(bool resizable)
    {
        WindowResizable = resizable;
    }

    /// <summary>
    ///     Sets the window title.
    /// </summary>
    /// <remarks>
    ///     If null, the application listener's class name is used.
    /// </remarks>
    /// <param name="title"></param>
    public void SetTitle(string title)
    {
        Title = title;
    }

    /// <summary>
    ///     Sets the app to use windowed mode.
    /// </summary>
    /// <param name="width">The width of the window (default 640).</param>
    /// <param name="height">The height of the window (default 480).</param>
    public void SetWindowedMode(int width, int height)
    {
        WindowWidth = width;
        WindowHeight = height;
    }

    /// <summary>
    ///     Sets the icon that will be used in the window's title bar.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Has no effect in macOS, which doesn't use window icons.
    ///     </para>
    /// </remarks>
    /// <param name="filePaths">
    ///     One or more <see cref="FileType" /> image paths. Must be JPEG, PNG, or BMP format. The one
    ///     closest to the system's desired size will be scaled. Good sizes include 16x16, 32x32 and 48x48.
    /// </param>
    public void SetWindowIcon(params string[] filePaths)
    {
        SetWindowIcon(FileType.Internal, filePaths);
    }

    /// <summary>
    ///     Sets the icon that will be used in the window's title bar.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Has no effect in macOS, which doesn't use window icons.
    ///     </para>
    /// </remarks>
    /// <param name="fileType">The type of file handle the paths are relative to.</param>
    /// <param name="filePaths">
    ///     One or more image paths, relative to the given <see cref="FileType" />.Must be JPEG, PNG, or
    ///     BMP format. The one closest to the system's desired size will be scaled. Good sizes include 16x16, 32x32 and 48x48.
    /// </param>
    public void SetWindowIcon(FileType fileType, params string[] filePaths)
    {
        WindowIconFileType = fileType;
        WindowIconPaths = filePaths;
    }

    /// <summary>
    ///     Sets the <see cref="IDesktopWindowListener" /> which will be informed about iconficiation, focus loss and window
    ///     close events.
    /// </summary>
    /// <param name="windowListener"></param>
    public void SetWindowListener(IDesktopWindowListener windowListener)
    {
        WindowListener = windowListener;
    }

    /// <summary>
    ///     Sets the position of the window in windowed mode.
    /// </summary>
    /// <remarks>
    ///     Default -1 for both coordinates for centered on primary monitor.
    /// </remarks>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetWindowPosition(int x, int y)
    {
        WindowX = x;
        WindowY = y;
    }

    /// <summary>
    ///     Sets minimum and maximum size limits for the window.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If the window is full screen or not resizable, these limits are ignored.
    ///     </para>
    ///     <para>
    ///         The default for all four parameters is -1, which means unrestricted.
    ///     </para>
    /// </remarks>
    /// <param name="minWidth"></param>
    /// <param name="minHeight"></param>
    /// <param name="maxWidth"></param>
    /// <param name="maxHeight"></param>
    public void SetWindowSizeLimits(int minWidth, int minHeight, int maxWidth, int maxHeight)
    {
        WindowMinWidth = minWidth;
        WindowMinHeight = minHeight;
        WindowMaxWidth = maxWidth;
        WindowMaxHeight = maxHeight;
    }

    /// <summary>
    ///     Sets whether to use vsync.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This setting can be changed anytime at runtime via <see cref="IGraphics.SetVSync(bool)" />.
    ///     </para>
    ///     <para>
    ///         For multi-window applications, only one (the main) window should enable vsync. Otherwise, every window will
    ///         wait for the vertical blank on swap individually, effectively cutting the frame rate to (refreshRate /
    ///         numberOfWindows).
    ///     </para>
    /// </remarks>
    /// <param name="vsync"></param>
    public void UseVsync(bool vsync)
    {
        VSyncEnabled = vsync;
    }

    internal void SetWindowConfiguration(DesktopWindowConfiguration config)
    {
        WindowX = config.WindowX;
        WindowY = config.WindowY;
        WindowWidth = config.WindowWidth;
        WindowHeight = config.WindowHeight;
        WindowMinWidth = config.WindowMinWidth;
        WindowMinHeight = config.WindowMinHeight;
        WindowMaxWidth = config.WindowMaxWidth;
        WindowMaxHeight = config.WindowMaxHeight;
        WindowResizable = config.WindowResizable;
        WindowDecorated = config.WindowDecorated;
        WindowMaximized = config.WindowMaximized;
        MaximizedMonitor = config.MaximizedMonitor;
        AutoIconify = config.AutoIconify;
        WindowIconFileType = config.WindowIconFileType;

        if (config.WindowIconPaths != null)
        {
            WindowIconPaths = new string[config.WindowIconPaths.Length];
            Array.Copy(config.WindowIconPaths, WindowIconPaths, config.WindowIconPaths.Length);
        }

        WindowListener = config.WindowListener;
        FullscreenMode = config.FullscreenMode;
        Title = config.Title;
        InitialBackgroundColor = config.InitialBackgroundColor;
        InitialVisible = config.InitialVisible;
        VSyncEnabled = config.VSyncEnabled;
    }
}