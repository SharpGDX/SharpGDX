using OpenTK.Windowing.GraphicsLibraryFramework;
using SharpGDX.Files;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Mathematics;
using SharpGDX.Shims;
using static SharpGDX.IGraphics;
using Monitor = SharpGDX.IGraphics.Monitor;

namespace SharpGDX.Desktop;

public class DesktopApplicationConfiguration : DesktopWindowConfiguration
{
    public enum GLEmulation
    {
        ANGLE_GLES20,
        GL20,
        GL30,
        GL31,
        GL32
    }

    public static PrintStream ErrorStream = new();

    /**
     * The maximum number of threads to use for network requests. Default is {@link Integer#MAX_VALUE}.
     */
    private int _maxNetThreads = int.MaxValue;

    internal int a = 8;
    internal int AudioDeviceBufferCount = 9;
    internal int AudioDeviceBufferSize = 512;

    internal int AudioDeviceSimultaneousSources = 16;
    internal int b = 8;

    internal bool debug;
    internal PrintStream debugStream = new();
    internal int depth = 16, stencil;
    internal int foregroundFPS;
    internal int g = 8;

    internal GLEmulation glEmulation = GLEmulation.GL20;
    internal int gles30ContextMajorVersion = 3;
    internal int gles30ContextMinorVersion = 2;

    internal HdpiMode hdpiMode = HdpiMode.Logical;

    internal int idleFPS = 60;

    internal bool IsAudioDisabled;
    internal bool pauseWhenLostFocus;

    internal bool pauseWhenMinimized = true;

    internal string preferencesDirectory = ".prefs/";
    internal FileType preferencesFileType = FileType.External;

    internal int r = 8;
    internal int samples;
    internal bool transparentFramebuffer;

    internal static DesktopApplicationConfiguration Copy(DesktopApplicationConfiguration config)
    {
        var copy = new DesktopApplicationConfiguration();
        copy.Set(config);
        return copy;
    }

    private void Set(DesktopApplicationConfiguration config)
    {
        SetWindowConfiguration(config);
        IsAudioDisabled = config.IsAudioDisabled;
        AudioDeviceSimultaneousSources = config.AudioDeviceSimultaneousSources;
        AudioDeviceBufferSize = config.AudioDeviceBufferSize;
        AudioDeviceBufferCount = config.AudioDeviceBufferCount;
        glEmulation = config.glEmulation;
        gles30ContextMajorVersion = config.gles30ContextMajorVersion;
        gles30ContextMinorVersion = config.gles30ContextMinorVersion;
        r = config.r;
        g = config.g;
        b = config.b;
        a = config.a;
        depth = config.depth;
        stencil = config.stencil;
        samples = config.samples;
        transparentFramebuffer = config.transparentFramebuffer;
        idleFPS = config.idleFPS;
        foregroundFPS = config.foregroundFPS;
        pauseWhenMinimized = config.pauseWhenMinimized;
        pauseWhenLostFocus = config.pauseWhenLostFocus;
        preferencesDirectory = config.preferencesDirectory;
        preferencesFileType = config.preferencesFileType;
        hdpiMode = config.hdpiMode;
        debug = config.debug;
        debugStream = config.debugStream;
    }

    /// <summary>
    ///     Whether the window will be visible on creation. (default true)
    /// </summary>
    /// <param name="visibility"></param>
    public override void SetInitialVisible(bool visibility)
    {
        InitialVisible = visibility;
    }

    /// <summary>
    ///     Whether to disable audio or not. If set to true, the returned audio class instances like {@link Audio} or {@link
    ///     Music} will be mock implementations.
    /// </summary>
    /// <param name="disableAudio"></param>
    public void DisableAudio(bool disableAudio)
    {
        IsAudioDisabled = disableAudio;
    }

    /// <summary>
    ///     Sets the maximum number of threads to use for network requests.
    /// </summary>
    /// <param name="maxNetThreads"></param>
    public void SetMaxNetThreads(int maxNetThreads)
    {
        _maxNetThreads = maxNetThreads;
    }

    /**
     * Sets the audio device configuration.
     *
     * @param simultaneousSources the maximum number of sources that can be played simultaniously (default 16)
     * @param bufferSize the audio device buffer size in samples (default 512)
     * @param bufferCount the audio device buffer count (default 9)
     */
    public void SetAudioConfig(int simultaneousSources, int bufferSize, int bufferCount)
    {
        AudioDeviceSimultaneousSources = simultaneousSources;
        AudioDeviceBufferSize = bufferSize;
        AudioDeviceBufferCount = bufferCount;
    }

    /**
     * Sets which OpenGL version to use to emulate OpenGL ES. If the given major/minor version is not supported, the backend falls
     * back to OpenGL ES 2.0 emulation through OpenGL 2.0. The default parameters for major and minor should be 3 and 2
     * respectively to be compatible with Mac OS X. Specifying major version 4 and minor version 2 will ensure that all OpenGL ES
     * 3.0 features are supported. Note however that Mac OS X does only support 3.2.
     *
     * @see
     * <a href="http://legacy.lwjgl.org/javadoc/org/lwjgl/opengl/ContextAttribs.html"> LWJGL OSX ContextAttribs note</a>
     * @param glVersion which OpenGL ES emulation version to use
     * @param gles3MajorVersion OpenGL ES major version, use 3 as default
     * @param gles3MinorVersion OpenGL ES minor version, use 2 as default
     */
    public void SetOpenGLEmulation(GLEmulation glVersion, int gles3MajorVersion, int gles3MinorVersion)
    {
        glEmulation = glVersion;
        gles30ContextMajorVersion = gles3MajorVersion;
        gles30ContextMinorVersion = gles3MinorVersion;
    }

    /**
     * Sets the bit depth of the color, depth and stencil buffer as well as multi-sampling.
     *
     * @param r red bits (default 8)
     * @param g green bits (default 8)
     * @param b blue bits (default 8)
     * @param a alpha bits (default 8)
     * @param depth depth bits (default 16)
     * @param stencil stencil bits (default 0)
     * @param samples MSAA samples (default 0)
     */
    public void SetBackBufferConfig(int r, int g, int b, int a, int depth, int stencil, int samples)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
        this.depth = depth;
        this.stencil = stencil;
        this.samples = samples;
    }

    /**
     * Set transparent window hint. Results may vary on different OS and GPUs. Usage with the ANGLE backend is less consistent.
     * @param transparentFrameBuffer
     */
    public void SetTransparentFrameBuffer(bool transparentFramebuffer)
    {
        this.transparentFramebuffer = transparentFramebuffer;
    }

    /**
     * Sets the polling rate during idle time in non-continuous rendering mode. Must be positive. Default is 60.
     */
    public void SetIdleFps(int fps)
    {
        idleFPS = fps;
    }

    /**
     * Sets the target framerate for the application. The CPU sleeps as needed. Must be positive. Use 0 to never sleep. Default is
     * 0.
     */
    public void SetForegroundFPS(int fps)
    {
        foregroundFPS = fps;
    }

    /** Sets whether to pause the application {@link ApplicationListener#pause()} and fire
 * {@link LifecycleListener#pause()}/{@link LifecycleListener#resume()} events on when window is minimized/restored. **/
    public void SetPauseWhenMinimized(bool pauseWhenMinimized)
    {
        this.pauseWhenMinimized = pauseWhenMinimized;
    }

    /** Sets whether to pause the application {@link ApplicationListener#pause()} and fire
     * {@link LifecycleListener#pause()}/{@link LifecycleListener#resume()} events on when window loses/gains focus. **/
    public void SetPauseWhenLostFocus(bool pauseWhenLostFocus)
    {
        this.pauseWhenLostFocus = pauseWhenLostFocus;
    }


    /**
     * Sets the directory where {@link Preferences} will be stored, as well as the file type to be used to store them. Defaults to
     * "$USER_HOME/.prefs/" and {@link FileType#External}.
     */
    public void SetPreferencesConfig(string preferencesDirectory, FileType preferencesFileType)
    {
        this.preferencesDirectory = preferencesDirectory;
        this.preferencesFileType = preferencesFileType;
    }

    /**
     * Defines how HDPI monitors are handled. Operating systems may have a per-monitor HDPI scale setting. The operating system
     * may report window width/height and mouse coordinates in a logical coordinate system at a lower resolution than the actual
     * physical resolution. This setting allows you to specify whether you want to work in logical or raw pixel units. See
     * {@link HdpiMode} for more information. Note that some OpenGL functions like {@link GL20#glViewport(int, int, int, int)} and
     * {@link GL20#glScissor(int, int, int, int)} require raw pixel units. Use {@link HdpiUtils} to help with the conversion if
     * HdpiMode is set to {@link HdpiMode#Logical}. Defaults to {@link HdpiMode#Logical}.
     */
    public void SetHdpiMode(HdpiMode mode)
    {
        hdpiMode = mode;
    }

    /**
     * Enables use of OpenGL debug message callbacks. If not supported by the core GL driver (since GL 4.3), this uses the
     * KHR_debug, ARB_debug_output or AMD_debug_output extension if available. By default, debug messages with NOTIFICATION
     * severity are disabled to avoid log spam.
     *
     * You can call with {@link System#err} to output to the "standard" error output stream.
     *
     * Use {@link DesktopApplication#setGLDebugMessageControl(DesktopApplication.GLDebugMessageSeverity, boolean)} to enable or
     * disable other severity debug levels.
     */
    public void EnableGLDebugOutput(bool enable, PrintStream debugOutputStream)
    {
        debug = enable;
        debugStream = debugOutputStream;
    }

    /**
     * @return the currently active {@link DisplayMode} of the primary monitor
     */
    public static unsafe DisplayMode GetDisplayMode()
    {
        DesktopApplication.initializeGlfw();
        var videoMode = GLFW.GetVideoMode(GLFW.GetPrimaryMonitor());
        return new DesktopGraphics.DesktopDisplayMode(GLFW.GetPrimaryMonitor(), videoMode->Width, videoMode->Height,
            videoMode->RefreshRate, videoMode->RedBits + videoMode->GreenBits + videoMode->BlueBits);
    }

    /**
     * @return the currently active {@link DisplayMode} of the given monitor
     */
    public static unsafe DisplayMode GetDisplayMode(Monitor monitor)
    {
        DesktopApplication.initializeGlfw();
        var videoMode = GLFW.GetVideoMode(((DesktopGraphics.DesktopMonitor)monitor).monitorHandle);
        return new DesktopGraphics.DesktopDisplayMode(((DesktopGraphics.DesktopMonitor)monitor).monitorHandle,
            videoMode->Width, videoMode->Height,
            videoMode->RefreshRate, videoMode->RedBits + videoMode->GreenBits + videoMode->BlueBits);
    }

    /// <summary>
    /// </summary>
    /// <returns>the available {@link DisplayMode}s of the primary monitor</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static unsafe DisplayMode[] GetDisplayModes()
    {
        DesktopApplication.initializeGlfw();
        var videoModes = GLFW.GetVideoModes(GLFW.GetPrimaryMonitor());
        var result = new DisplayMode[videoModes.Length];
        for (var i = 0; i < result.Length; i++) throw new NotImplementedException();
        //GLFW.GLFWVidMode videoMode = videoModes.get(i);
        //result[i] = new DesktopGraphics.DesktopDisplayMode(GLFW.glfwGetPrimaryMonitor(), videoMode.Width, videoMode.Height,
        //	videoMode.RefreshRate, videoMode.RedBits + videoMode.GreenBits + videoMode.BlueBits);
        return result;
    }

    /// <summary>
    /// </summary>
    /// <param name="monitor"></param>
    /// <returns>the available {@link DisplayMode}s of the given {@link Monitor}</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static unsafe DisplayMode[] GetDisplayModes(Monitor monitor)
    {
        DesktopApplication.initializeGlfw();
        var videoModes = GLFW.GetVideoModes(((DesktopGraphics.DesktopMonitor)monitor).monitorHandle);
        var result = new DisplayMode[videoModes.Length];
        for (var i = 0; i < result.Length; i++) throw new NotImplementedException();
        //GLFW.GLFWVidMode videoMode = videoModes.get(i);
        //result[i] = new DesktopGraphics.DesktopDisplayMode(((DesktopGraphics.DesktopMonitor)monitor).monitorHandle, videoMode.Width,
        //	videoMode.Height, videoMode.RefreshRate, videoMode.RedBits + videoMode.GreenBits + videoMode.BlueBits);
        return result;
    }

    /// <summary>
    /// </summary>
    /// <returns>the primary {@link Monitor}</returns>
    public static unsafe Monitor GetPrimaryMonitor()
    {
        DesktopApplication.initializeGlfw();
        return ToDesktopMonitor(GLFW.GetPrimaryMonitor());
    }

    /// <summary>
    ///     the connected {@link Monitor}s
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static Monitor[] GetMonitors()
    {
        throw new NotImplementedException();
        //DesktopApplication.initializeGlfw();
        //PointerBuffer glfwMonitors = GLFW.glfwGetMonitors();
        //Monitor[] monitors = new Monitor[glfwMonitors.limit()];
        //for (int i = 0; i < glfwMonitors.limit(); i++)
        //{
        //	monitors[i] = toDesktopMonitor(glfwMonitors.get(i));
        //}
        //return monitors;
    }

    internal static unsafe DesktopGraphics.DesktopMonitor ToDesktopMonitor(
        OpenTK.Windowing.GraphicsLibraryFramework.Monitor* glfwMonitor)
    {
        // TODO: This was originally BufferUtils.createIntBuffer, not sure if this will work.
        var tmp = IntBuffer.allocate(1);
        var tmp2 = IntBuffer.allocate(1);
        GLFW.GetMonitorPos(glfwMonitor, out var virtualX, out var virtualY);
        var name = GLFW.GetMonitorName(glfwMonitor);
        return new DesktopGraphics.DesktopMonitor(glfwMonitor, virtualX, virtualY, name);
    }

    internal static unsafe GridPoint2 CalculateCenteredWindowPosition(DesktopGraphics.DesktopMonitor monitor,
        int newWidth, int newHeight)
    {
        // TODO: This was originally BufferUtils.createIntBuffer, not sure if this will work.


        var displayMode = GetDisplayMode(monitor);

        GLFW.GetMonitorWorkarea(monitor.monitorHandle, out var xPos, out var yPos, out var width, out var height);
        var workAreaWidth = width;
        var workAreaHeight = height;

        int minX, minY, maxX, maxY;

        // If the new width is greater than the working area, we have to ignore stuff like the taskbar for centering and use the
        // whole monitor's size
        if (newWidth > workAreaWidth)
        {
            minX = monitor.VirtualX;
            maxX = displayMode.Width;
        }
        else
        {
            minX = xPos;
            maxX = workAreaWidth;
        }

        // The same is true for height
        if (newHeight > workAreaHeight)
        {
            minY = monitor.VirtualY;
            maxY = displayMode.Height;
        }
        else
        {
            minY = yPos;
            maxY = workAreaHeight;
        }

        return new GridPoint2
        (
            Math.Max(minX, minX + (maxX - newWidth) / 2),
            Math.Max(minY, minY + (maxY - newHeight) / 2)
        );
    }
}