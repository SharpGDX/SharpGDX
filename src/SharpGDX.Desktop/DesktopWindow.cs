using System.Runtime.InteropServices;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SharpGDX.Files;
using SharpGDX.Graphics;
using SharpGDX.Shims;
using SharpGDX.Utils;
using static OpenTK.Windowing.GraphicsLibraryFramework.GLFWCallbacks;


namespace SharpGDX.Desktop;

public class DesktopWindow : IDisposable
{
    private readonly IDesktopApplicationBase _application;
    private readonly DesktopApplicationConfiguration _config;
    private readonly Array<Runnable> _executedRunnables = new();
    private readonly Array<ILifecycleListener> _lifecycleListeners;
    private readonly IApplicationListener _listener;
    private readonly Array<Runnable> _runnables = new();

    private WindowFocusCallback _focusCallback;
    private bool _focused;
    private DesktopGraphics _graphics;
    private bool _iconified;
    private IDesktopInput _input;
    private bool _listenerInitialized;
    private WindowMaximizeCallback _maximizeCallback;
    private WindowRefreshCallback _refreshCallback;
    private bool _requestRendering;
    private unsafe Window* _windowHandle;
    private IDesktopWindowListener? _windowListener;

    internal bool AsyncResized;

    private WindowCloseCallback closeCallback;
    private WindowIconifyCallback iconifyCallback;

    internal DesktopWindow(IApplicationListener listener, Array<ILifecycleListener> lifecycleListeners,
        DesktopApplicationConfiguration config,
        IDesktopApplicationBase application)
    {
        _listener = listener;
        _lifecycleListeners = lifecycleListeners;
        _windowListener = config.WindowListener;
        _config = config;
        _application = application;
    }

    public unsafe void Dispose()
    {
        _listener.Pause();
        _listener.Dispose();
        DesktopCursor.dispose(this);
        _graphics.Dispose();
        _input.Dispose();

        GLFW.SetWindowFocusCallback(_windowHandle, null);
        GLFW.SetWindowIconifyCallback(_windowHandle, null);
        GLFW.SetWindowMaximizeCallback(_windowHandle, null);
        GLFW.SetWindowCloseCallback(_windowHandle, null);
        GLFW.SetDropCallback(_windowHandle, null);
        GLFW.SetWindowRefreshCallback(_windowHandle, null);

        GLFW.DestroyWindow(_windowHandle);
    }
    
    private unsafe void DropCallback(Window* windowHandle, int count, byte** names)
    {
        var files = new string?[count];

        for (var i = 0; i < count; i++)
        {
            // TODO: Do these need to be freed? -RP
            files[i] = Marshal.PtrToStringUTF8((IntPtr)names[i]);
        }

        PostRunnable(() =>
        {
            _windowListener?.FilesDropped(files);
        });
    }

    internal unsafe void Create(Window* windowHandle)
    {
        _windowHandle = windowHandle;
        _input = _application.CreateInput(this);
        _graphics = new DesktopGraphics(this);

        GLFW.SetWindowFocusCallback
        (
            windowHandle,
            _focusCallback = (_, focused) => PostRunnable(() =>
            {
                if (_windowListener != null)
                {
                    if (focused)
                    {
                        if (_config.pauseWhenLostFocus)
                        {
                            lock (_lifecycleListeners)
                            {
                                foreach (var lifecycleListener in _lifecycleListeners)
                                {
                                    lifecycleListener.Resume();
                                }
                            }
                        }

                        _windowListener.FocusGained();
                    }
                    else
                    {
                        _windowListener.FocusLost();

                        if (_config.pauseWhenLostFocus)
                        {
                            lock (_lifecycleListeners)
                            {
                                foreach (var lifecycleListener in _lifecycleListeners)
                                {
                                    lifecycleListener.Pause();
                                }
                            }

                            _listener.Pause();
                        }
                    }

                    _focused = focused;
                }
            })
        );

        GLFW.SetWindowIconifyCallback
        (
            windowHandle,
            iconifyCallback = (_, iconified) => PostRunnable(() =>
            {
                if (_windowListener != null)
                {
                    _windowListener.Iconified(iconified);
                }

                _iconified = iconified;

                if (iconified)
                {
                    if (_config.pauseWhenMinimized)
                    {
                        lock (_lifecycleListeners)
                        {
                            foreach (var lifecycleListener in _lifecycleListeners)
                            {
                                lifecycleListener.Pause();
                            }
                        }

                        _listener.Pause();
                    }
                }
                else
                {
                    if (_config.pauseWhenMinimized)
                    {
                        lock (_lifecycleListeners)
                        {
                            foreach (var lifecycleListener in _lifecycleListeners)
                            {
                                lifecycleListener.Resume();
                            }
                        }

                        _listener.Resume();
                    }
                }
            })
        );

        GLFW.SetWindowMaximizeCallback(windowHandle, _maximizeCallback = (_, maximized) => PostRunnable(() =>
        {
            _windowListener?.Maximized(maximized);
        }));

        GLFW.SetWindowCloseCallback
        (
            windowHandle,
            closeCallback = _ => PostRunnable(() =>
            {
                if (_windowListener?.CloseRequested() == false)
                {
                    GLFW.SetWindowShouldClose(windowHandle, false);
                }
            })
        );

        GLFW.SetDropCallback(windowHandle, DropCallback);

        GLFW.SetWindowRefreshCallback
        (
            windowHandle,
            _refreshCallback = _ => PostRunnable(() => { _windowListener?.RefreshRequested(); })
        );

        _windowListener?.Created(this);
    }

    /** @return the {@link ApplicationListener} associated with this window **/
    public IApplicationListener GetListener()
    {
        return _listener;
    }

    /// <summary>
    /// Returns the <see cref="IDesktopWindowListener"/> set on this window.
    /// </summary>
    /// <returns>The <see cref="IDesktopWindowListener"/> set on this window.</returns>
    public IDesktopWindowListener? GetWindowListener()
    {
        return _windowListener;
    }

    public void SetWindowListener(IDesktopWindowListener listener)
    {
        _windowListener = listener;
    }

    /**
     * Post a {@link Runnable} to this window's event queue. Use this if you access statics like {@link Gdx#graphics} in your
     * runnable instead of {@link Application#postRunnable(Runnable)}.
     */
    public void PostRunnable(Runnable runnable)
    {
        lock (_runnables)
        {
            _runnables.Add(runnable);
        }
    }

    /** Sets the position of the window in logical coordinates. All monitors span a virtual surface together. The coordinates are
     * relative to the first monitor in the virtual surface. **/
    public unsafe void SetPosition(int x, int y)
    {
        GLFW.SetWindowPos(_windowHandle, x, y);
    }

    /**
     * @return the window position in logical coordinates. All monitors span a virtual surface together. The coordinates are
     * relative to the first monitor in the virtual surface. *
     */
    public unsafe int GetPositionX()
    {
        GLFW.GetWindowPos(_windowHandle, out var x, out var y);
        return x;
    }

    /**
     * @return the window position in logical coordinates. All monitors span a virtual surface together. The coordinates are
     * relative to the first monitor in the virtual surface. *
     */
    public unsafe int GetPositionY()
    {
        GLFW.GetWindowPos(_windowHandle, out var x, out var y);
        return y;
    }

    /**
     * Sets the visibility of the window. Invisible windows will still call their {@link ApplicationListener}
     */
    public unsafe void SetVisible(bool visible)
    {
        if (visible)
        {
            GLFW.ShowWindow(_windowHandle);
        }
        else
        {
            GLFW.HideWindow(_windowHandle);
        }
    }

    /**
     * Closes this window and pauses and disposes the associated {@link ApplicationListener}.
     */
    public unsafe void CloseWindow()
    {
        GLFW.SetWindowShouldClose(_windowHandle, true);
    }

    /**
     * Minimizes (iconifies) the window. Iconified windows do not call their {@link ApplicationListener} until the window is
     * restored.
     */
    public unsafe void IconifyWindow()
    {
        GLFW.IconifyWindow(_windowHandle);
    }

    /// <summary>
    ///     Whether the window is iconified.
    /// </summary>
    /// <returns></returns>
    public bool IsIconified()
    {
        return _iconified;
    }

    /// <summary>
    ///     De-minimizes (de-iconifies) and de-maximizes the window.
    /// </summary>
    public unsafe void RestoreWindow()
    {
        GLFW.RestoreWindow(_windowHandle);
    }

    /**
     * Maximizes the window.
     */
    public unsafe void MaximizeWindow()
    {
        GLFW.MaximizeWindow(_windowHandle);
    }

    /// <summary>
    ///     Brings the window to front and sets input focus.
    /// </summary>
    /// <remarks>
    ///     The window should already be visible and not iconified.
    /// </remarks>
    public unsafe void FocusWindow()
    {
        GLFW.FocusWindow(_windowHandle);
    }

    public bool IsFocused()
    {
        return _focused;
    }

    /**
     * Sets the icon that will be used in the window's title bar. Has no effect in macOS, which doesn't use window icons.
     * @param image One or more images. The one closest to the system's desired size will be scaled. Good sizes include 16x16,
     * 32x32 and 48x48. Pixmap format {@link com.badlogic.gdx.graphics.Pixmap.Format#RGBA8888 RGBA8888} is preferred so
     * the images will not have to be copied and converted. The chosen image is copied, and the provided Pixmaps are not
     * disposed.
     */
    public unsafe void SetIcon(Pixmap[] image)
    {
        SetIcon(_windowHandle, image);
    }

    internal static unsafe void SetIcon(Window* windowHandle, string[] imagePaths, FileType imageFileType)
    {
        if (SharedLibraryLoader.isMac)
        {
            return;
        }

        var pixmaps = new Pixmap[imagePaths.Length];
        for (var i = 0; i < imagePaths.Length; i++)
        {
            pixmaps[i] = new Pixmap(GDX.Files.GetFileHandle(imagePaths[i], imageFileType));
        }

        SetIcon(windowHandle, pixmaps);

        foreach (var pixmap in pixmaps)
        {
            pixmap.Dispose();
        }
    }

    private static unsafe void SetIcon(Window* windowHandle, Pixmap[] images)
    {
        if (SharedLibraryLoader.isMac)
        {
            return;
        }

        Span<GCHandle> handles = stackalloc GCHandle[images.Length];
        Span<Image> glfwImages = stackalloc Image[images.Length];

        Pixmap?[] tmpPixmaps = new Pixmap[images.Length];

        for (var i = 0; i < images.Length; i++)
        {
            var pixmap = images[i];

            if (pixmap.GetFormat() != Pixmap.Format.RGBA8888)
            {
                var rgba = new Pixmap(pixmap.GetWidth(), pixmap.GetHeight(), Pixmap.Format.RGBA8888);
                rgba.SetBlending(Pixmap.Blending.None);
                rgba.DrawPixmap(pixmap, 0, 0);
                tmpPixmaps[i] = rgba;
                pixmap = rgba;
            }

            handles[i] = GCHandle.Alloc(pixmap.GetPixels().array(), GCHandleType.Pinned);
            var addrOfPinnedObject = (byte*)handles[i].AddrOfPinnedObject();

            glfwImages[i] = new Image(pixmap.GetWidth(), pixmap.GetHeight(), addrOfPinnedObject);
        }

        GLFW.SetWindowIcon(windowHandle, glfwImages);

        foreach (var handle in handles)
        {
            handle.Free();
        }

        foreach (var pixmap in tmpPixmaps)
        {
            if (pixmap != null)
            {
                pixmap.Dispose();
            }
        }
    }

    public unsafe void SetTitle(string title)
    {
        GLFW.SetWindowTitle(_windowHandle, title);
    }

    /**
     * Sets minimum and maximum size limits for the window. If the window is full screen or not resizable, these limits are
     * ignored. Use -1 to indicate an unrestricted dimension.
     */
    public unsafe void SetSizeLimits(int minWidth, int minHeight, int maxWidth, int maxHeight)
    {
        SetSizeLimits(_windowHandle, minWidth, minHeight, maxWidth, maxHeight);
    }

    internal static unsafe void SetSizeLimits
    (
        Window* windowHandle,
        int minWidth,
        int minHeight,
        int maxWidth,
        int maxHeight
    )
    {
        GLFW.SetWindowSizeLimits(windowHandle, minWidth > -1 ? minWidth : GLFW.DontCare,
            minHeight > -1 ? minHeight : GLFW.DontCare, maxWidth > -1 ? maxWidth : GLFW.DontCare,
            maxHeight > -1 ? maxHeight : GLFW.DontCare);
    }

    internal DesktopGraphics GetGraphics()
    {
        return _graphics;
    }

    internal IDesktopInput GetInput()
    {
        return _input;
    }

    public unsafe long GetWindowHandle()
    {
        // TODO: This should be an IntPtr and should be marshaled.
        return (long)_windowHandle;
    }

    internal unsafe Window* GetWindowPtr()
    {
        return _windowHandle;
    }

    private unsafe void WindowHandleChanged(Window* windowHandle)
    {
        _windowHandle = windowHandle;
        _input.WindowHandleChanged(windowHandle);
    }

    internal unsafe bool Update()
    {
        if (!_listenerInitialized)
        {
            InitializeListener();
        }

        lock (_runnables)
        {
            _executedRunnables.addAll(_runnables);
            _runnables.clear();
        }

        foreach (var runnable in _executedRunnables)
        {
            runnable.Invoke();
        }

        var shouldRender = _executedRunnables.size > 0 || _graphics.IsContinuousRendering();
        _executedRunnables.clear();

        if (!_iconified)
        {
            _input.Update();
        }

        lock (this)
        {
            shouldRender |= _requestRendering && !_iconified;
            _requestRendering = false;
        }

        // In case glfw_async is used, we need to resize outside the GLFW
        if (AsyncResized)
        {
            AsyncResized = false;
            _graphics.updateFramebufferInfo();
            _graphics.gl20.glViewport(0, 0, _graphics.GetBackBufferWidth(), _graphics.GetBackBufferHeight());
            _listener.Resize(_graphics.GetWidth(), _graphics.GetHeight());
            _graphics.update();
            _listener.Render();
            GLFW.SwapBuffers(_windowHandle);
            return true;
        }

        if (shouldRender)
        {
            _graphics.update();
            _listener.Render();
            GLFW.SwapBuffers(_windowHandle);
        }

        if (!_iconified)
        {
            _input.PrepareNext();
        }

        return shouldRender;
    }

    internal void RequestRendering()
    {
        lock (this)
        {
            _requestRendering = true;
        }
    }

    internal unsafe bool ShouldClose()
    {
        return GLFW.WindowShouldClose(_windowHandle);
    }

    internal DesktopApplicationConfiguration GetConfig()
    {
        return _config;
    }

    internal bool IsListenerInitialized()
    {
        return _listenerInitialized;
    }

    private void InitializeListener()
    {
        if (!_listenerInitialized)
        {
            _listener.Create();
            _listener.Resize(_graphics.GetWidth(), _graphics.GetHeight());
            _listenerInitialized = true;
        }
    }

    internal unsafe void MakeCurrent()
    {
        GDX.Graphics = _graphics;
        GDX.GL32 = _graphics.GetGL32();
        GDX.GL31 = GDX.GL32 != null ? GDX.GL32 : _graphics.GetGL31();
        GDX.GL30 = GDX.GL31 != null ? GDX.GL31 : _graphics.GetGL30();
        GDX.GL20 = GDX.GL30 != null ? GDX.GL30 : _graphics.GetGL20();
        GDX.GL = GDX.GL20;
        GDX.Input = _input;

        GLFW.MakeContextCurrent(_windowHandle);
    }

    public override unsafe int GetHashCode()
    {
        var prime = 31;
        var result = 1;
        // TODO: Not sure that this cast works
        result = prime * result + (int)((long)_windowHandle ^ ((long)_windowHandle >>> 32));
        return result;
    }

    public override unsafe bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }

        if (obj == null)
        {
            return false;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        var other = (DesktopWindow)obj;
        if (_windowHandle != other._windowHandle)
        {
            return false;
        }

        return true;
    }

    public unsafe void Flash()
    {
        GLFW.RequestWindowAttention(_windowHandle);
    }
}