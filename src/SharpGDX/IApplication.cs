using SharpGDX.Input;
using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX;

/// <summary>
///     An <see cref="IApplication" /> is the main entry point of a project, providing a set of modules for graphics,
///     audio, input and file i/o, as well as functionality for logging.
/// </summary>
public partial interface IApplication
{
    public static readonly int LogDebug = 3;
    public static readonly int LogError = 1;
    public static readonly int LogInfo = 2;
    public static readonly int LogNone = 0;

    /// <summary>
    ///     Adds a new <see cref="ILifecycleListener" /> to the application.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This can be used by extensions to hook into the lifecycle more easily.
    ///     </para>
    ///     <para>
    ///         The <see cref="IApplicationListener" /> methods are sufficient for application level development.
    ///     </para>
    /// </remarks>
    /// <param name="listener"></param>
    public void AddLifecycleListener(ILifecycleListener listener);

    /// <summary>
    ///     Logs a debug message to the console or logcat.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="message"></param>
    public void Debug(string tag, string message);

    /// <summary>
    ///     Logs a debug message to the console or logcat.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public void Debug(string tag, string message, Exception exception);

    /// <summary>
    ///     Logs an error message to the console or logcat.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="message"></param>
    public void Error(string tag, string message);

    /// <summary>
    ///     Logs an error message to the console or logcat.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public void Error(string tag, string message, Exception exception);

    /// <summary>
    ///     Schedule an exit from the application.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         On android, this will cause a call to <see cref="IApplicationListener.Pause()" /> and
    ///         <see cref="IApplicationListener.Dispose()" /> some time in the future, it will not immediately finish your
    ///         application.
    ///     </para>
    ///     <para>
    ///         On iOS this should be avoided in production as it breaks Apples guidelines.
    ///     </para>
    /// </remarks>
    public void Exit();

    /// <summary>
    ///     Gets the <see cref="IApplicationListener" /> instance.
    /// </summary>
    /// <returns>The <see cref="IApplicationListener" /> instance.</returns>
    public IApplicationListener GetApplicationListener();

    /// <summary>
    ///     Gets the current <see cref="IApplicationLogger" />.
    /// </summary>
    /// <returns>The current <see cref="IApplicationLogger" />.</returns>
    public IApplicationLogger GetApplicationLogger();

    /// <summary>
    ///     Gets the <see cref="IAudio" /> instance.
    /// </summary>
    /// <returns>The <see cref="IAudio" /> instance.</returns>
    public IAudio GetAudio();

    public IClipboard GetClipboard();

    /// <summary>
    ///     Gets the <see cref="IFiles" /> instance.
    /// </summary>
    /// <returns>The <see cref="IFiles" /> instance.</returns>
    public IFiles GetFiles();

    /// <summary>
    ///     Gets the <see cref="IGraphics" /> instance.
    /// </summary>
    /// <returns>The <see cref="IGraphics" /> instance.</returns>
    public IGraphics GetGraphics();

    /// <summary>
    ///     Gets the <see cref="IInput" /> instance.
    /// </summary>
    /// <returns>The <see cref="IInput" /> instance.</returns>
    public IInput GetInput();

    /// <summary>
    ///     Gets the Java heap memory use in bytes.
    /// </summary>
    /// <returns>The Java heap memory use in bytes.</returns>
    public long GetJavaHeap();

    /// <summary>
    ///     Gets the log level.
    /// </summary>
    /// <returns>The log level.</returns>
    public int GetLogLevel();

    /// <summary>
    ///     Gets the Native heap memory use in bytes.
    /// </summary>
    /// <returns>The Native heap memory use in bytes.</returns>
    public long GetNativeHeap();

    /// <summary>
    ///     Gets the <see cref="INet" /> instance.
    /// </summary>
    /// <returns>The <see cref="INet" /> instance</returns>
    public INet GetNet();

    /// <summary>
    ///     Returns the <see cref="IPreferences" /> instance of this Application. It can be used to store application settings
    ///     across runs.
    /// </summary>
    /// <param name="name">The name of the preferences, must be usable as a file name.</param>
    /// <returns>The preferences.</returns>
    public IPreferences GetPreferences(string name);

    /// <summary>
    ///     Gets what <see cref="ApplicationType" /> this application has, e.g. Android or Desktop.
    /// </summary>
    /// <returns>What <see cref="ApplicationType" /> this application has, e.g. Android or Desktop.</returns>
    public ApplicationType GetType();

    /// <summary>
    ///     Gets the Android API level on Android, the major OS version on iOS (5, 6, 7, ...), or 0 on the desktop.
    /// </summary>
    /// <returns>The Android API level on Android, the major OS version on iOS (5, 6, 7, ...), or 0 on the desktop.</returns>
    public int GetVersion();

    /// <summary>
    ///     Logs a message to the console or logcat.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="message"></param>
    public void Log(string tag, string message);

    /// <summary>
    ///     Logs a message to the console or logcat.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public void Log(string tag, string message, Exception exception);

    /// <summary>
    ///     Posts a <see cref="Runnable" /> on the main loop thread.
    /// </summary>
    /// <remarks>
    ///     In a multi-window application, the <see cref="Gdx.Graphics" /> and <see cref="Gdx.Input" /> values may be
    ///     unpredictable
    ///     at the time the Runnable is executed. If graphics or input are needed, they can be copied to a variable to be used
    ///     in the Runnable.
    /// </remarks>
    /// <param name="runnable">The runnable.</param>
    public void PostRunnable(Runnable runnable);

    /// <summary>
    ///     Removes the <see cref="ILifecycleListener" />.
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveLifecycleListener(ILifecycleListener listener);

    /// <summary>
    ///     Sets the current Application logger.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Calls to <see cref="Log(string, string)" /> are delegated to this <see cref="IApplicationLogger" />.
    ///     </para>
    /// </remarks>
    /// <param name="applicationLogger"></param>
    public void SetApplicationLogger(IApplicationLogger applicationLogger);

    /// <summary>
    ///     Sets the log level.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="LogNone" /> will mute all log output. <see cref="LogError" /> will only let error messages through.
    ///         <see cref="LogInfo" /> will let all non-debug messages through, and <see cref="LogDebug" /> will let all
    ///         messages through.
    ///     </para>
    /// </remarks>
    /// <param name="logLevel">
    ///     <see cref="LogNone" />, <see cref="LogError" />, <see cref="LogInfo" />, <see cref="LogDebug" />.
    /// </param>
    public void SetLogLevel(int logLevel);
}