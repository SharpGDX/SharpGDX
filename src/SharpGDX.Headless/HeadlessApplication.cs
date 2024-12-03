using SharpGDX.Headless.Mock.Audio;
using SharpGDX.Headless.Mock.Graphics;
using SharpGDX.Headless.Mock.Input;
using SharpGDX.Shims;
using SharpGDX.Utils;
using static SharpGDX.IApplication;

namespace SharpGDX.Headless;

/// <summary>
///     A headless implementation of a SharpGDX Application; primarily intended to be used in servers.
/// </summary>
public class HeadlessApplication : IApplication
{
    private readonly ObjectMap<string, IPreferences> _preferences = new();
    private readonly string _preferencesDir;

    protected readonly MockAudio Audio;
    protected readonly Array<Runnable> ExecutedRunnables = [];
    protected readonly HeadlessFiles Files;
    protected readonly MockGraphics Graphics;
    protected readonly MockInput Input;
    protected readonly Array<ILifecycleListener> LifecycleListeners = [];
    protected readonly IApplicationListener Listener;
    protected readonly HeadlessNet Net;
    protected readonly Array<Runnable> Runnables = [];
    protected IApplicationLogger ApplicationLogger;
    protected int LogLevel = LogInfo;
    protected Thread MainLoopThread;
    protected bool Running = true;

    public HeadlessApplication(IApplicationListener listener)
        : this(listener, null)
    {
    }

    public HeadlessApplication(IApplicationListener listener, HeadlessApplicationConfiguration? config)
    {
        config ??= new HeadlessApplicationConfiguration();

        HeadlessNativesLoader.Load();
        SetApplicationLogger(new HeadlessApplicationLogger());
        Listener = listener;
        Files = new HeadlessFiles();
        Net = new HeadlessNet(config);
        Graphics = new MockGraphics();
        Graphics.SetForegroundFPS(config.updatesPerSecond);
        Audio = new MockAudio();
        Input = new MockInput();

        _preferencesDir = config.preferencesDirectory;

        GDX.App = this;
        GDX.Files = Files;
        GDX.Net = Net;
        GDX.Audio = Audio;
        GDX.Graphics = Graphics;
        GDX.Input = Input;

        Initialize();
    }

    public IApplicationListener GetApplicationListener()
    {
        return Listener;
    }

    public IGraphics GetGraphics()
    {
        return Graphics;
    }

    public IAudio GetAudio()
    {
        return Audio;
    }

    public IInput GetInput()
    {
        return Input;
    }

    public IFiles GetFiles()
    {
        return Files;
    }

    public INet GetNet()
    {
        return Net;
    }

    public ApplicationType GetType()
    {
        return ApplicationType.HeadlessDesktop;
    }

    public int GetVersion()
    {
        return 0;
    }

    public long GetJavaHeap()
    {
        // TODO: return Runtime.getRuntime().totalMemory() - Runtime.getRuntime().freeMemory();
        throw new NotImplementedException();
    }

    public long GetNativeHeap()
    {
        return GetJavaHeap();
    }

    public IPreferences? GetPreferences(string name)
    {
        if (_preferences.containsKey(name))
        {
            return _preferences.get(name);
        }

        IPreferences preferences = new HeadlessPreferences(name, _preferencesDir);
        _preferences.put(name, preferences);

        return preferences;
    }

    public IClipboard? GetClipboard()
    {
        return null;
    }

    public void PostRunnable(Runnable runnable)
    {
        lock (Runnables)
        {
            Runnables.Add(runnable);
        }
    }

    public void Debug(string tag, string message)
    {
        if (LogLevel >= LogDebug)
        {
            GetApplicationLogger().Debug(tag, message);
        }
    }

    public void Debug(string tag, string message, Exception exception)
    {
        if (LogLevel >= LogDebug)
        {
            GetApplicationLogger().Debug(tag, message, exception);
        }
    }

    public void Log(string tag, string message)
    {
        if (LogLevel >= LogInfo)
        {
            GetApplicationLogger().Log(tag, message);
        }
    }

    public void Log(string tag, string message, Exception exception)
    {
        if (LogLevel >= LogInfo)
        {
            GetApplicationLogger().Log(tag, message, exception);
        }
    }

    public void Error(string tag, string message)
    {
        if (LogLevel >= LogError)
        {
            GetApplicationLogger().Error(tag, message);
        }
    }

    public void Error(string tag, string message, Exception exception)
    {
        if (LogLevel >= LogError)
        {
            GetApplicationLogger().Error(tag, message, exception);
        }
    }

    public void SetLogLevel(int logLevel)
    {
        LogLevel = logLevel;
    }

    public int GetLogLevel()
    {
        return LogLevel;
    }

    public void SetApplicationLogger(IApplicationLogger applicationLogger)
    {
        ApplicationLogger = applicationLogger;
    }

    public IApplicationLogger GetApplicationLogger()
    {
        return ApplicationLogger;
    }

    public void Exit()
    {
        PostRunnable(() => { Running = false; });
    }

    public void AddLifecycleListener(ILifecycleListener listener)
    {
        lock (LifecycleListeners)
        {
            LifecycleListeners.Add(listener);
        }
    }

    public void RemoveLifecycleListener(ILifecycleListener listener)
    {
        lock (LifecycleListeners)
        {
            LifecycleListeners.RemoveValue(listener, true);
        }
    }

    private void Initialize()
    {
        MainLoopThread = new Thread(() =>
        {
            try
            {
                MainLoop();
            }
            catch (Exception t)
            {
                if (t is RuntimeException re)
                {
                    throw re;
                }

                throw new GdxRuntimeException(t);
            }
        })
        {
            Name = "HeadlessApplication"
        };
        MainLoopThread.Start();
    }

    protected void MainLoop()
    {
        Listener.Create();

        var t = TimeUtils.nanoTime() + Graphics.getTargetRenderInterval();

        if (Graphics.getTargetRenderInterval() >= 0f)
        {
            while (Running)
            {
                var n = TimeUtils.nanoTime();

                if (t > n)
                {
                    try
                    {
                        var sleep = t - n;

                        // TODO: The original call is Thread.sleep(long, int). C# can't sleep that precisely, and I doubt Java can either.
                        Thread.Sleep((int)(sleep / 1000000)); //, (int)(sleep % 1000000));
                    }
                    catch (ThreadInterruptedException e)
                    {
                    }

                    t = t + Graphics.getTargetRenderInterval();
                }
                else
                {
                    t = n + Graphics.getTargetRenderInterval();
                }

                ExecuteRunnables();
                Graphics.incrementFrameId();
                Listener.Render();
                Graphics.updateTime();

                // If one of the runnables set running to false, for example after an exit().
                if (!Running)
                {
                    break;
                }
            }
        }

        lock (LifecycleListeners)
        {
            foreach (var listener in LifecycleListeners)
            {
                listener.Pause();
                listener.Dispose();
            }
        }

        Listener.Pause();
        Listener.Dispose();
    }

    public bool ExecuteRunnables()
    {
        lock (Runnables)
        {
            for (var i = Runnables.size - 1; i >= 0; i--)
            {
                ExecutedRunnables.Add(Runnables.Get(i));
            }

            Runnables.clear();
        }

        if (ExecutedRunnables.size == 0)
        {
            return false;
        }

        for (var i = ExecutedRunnables.size - 1; i >= 0; i--)
        {
            ExecutedRunnables.RemoveIndex(i).Invoke();
        }

        return true;
    }
}