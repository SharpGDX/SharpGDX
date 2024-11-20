using SharpGDX.Headless.Mock.Audio;
using SharpGDX.Headless.Mock.Graphics;
using SharpGDX.Headless.Mock.Input;
using SharpGDX.Headless;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Input;
using static SharpGDX.IApplication;

namespace SharpGDX.Headless
{
	/** a headless implementation of a GDX Application primarily intended to be used in servers
 * @author Jon Renner */
	public class HeadlessApplication : IApplication
	{
		protected readonly IApplicationListener listener;
		protected Thread mainLoopThread;
		protected readonly HeadlessFiles files;
		protected readonly HeadlessNet net;
		protected readonly MockAudio audio;
		protected readonly MockInput input;
		protected readonly MockGraphics graphics;
		protected bool running = true;
		protected readonly Array<Runnable> runnables = new Array<Runnable>();
		protected readonly Array<Runnable> executedRunnables = new Array<Runnable>();
		protected readonly Array<ILifecycleListener> lifecycleListeners = new Array<ILifecycleListener>();
		protected int logLevel = LogInfo;
		protected IApplicationLogger applicationLogger;
		private String preferencesdir;

		public HeadlessApplication(IApplicationListener listener)
			: this(listener, null)
		{
		}

		public HeadlessApplication(IApplicationListener listener, HeadlessApplicationConfiguration config)
		{
			if (config == null) config = new HeadlessApplicationConfiguration();

			HeadlessNativesLoader.Load();
			SetApplicationLogger(new HeadlessApplicationLogger());
			this.listener = listener;
			this.files = new HeadlessFiles();
			this.net = new HeadlessNet(config);
			// the following elements are not applicable for headless applications
			// they are only implemented as mock objects
			this.graphics = new MockGraphics();
			this.graphics.SetForegroundFPS(config.updatesPerSecond);
			this.audio = new MockAudio();
			this.input = new MockInput();

			this.preferencesdir = config.preferencesDirectory;

			Gdx.App = this;
			Gdx.Files = files;
			Gdx.Net = net;
			Gdx.Audio = audio;
			Gdx.Graphics = graphics;
			Gdx.Input = input;

			initialize();
		}

		private void initialize()
		{
			mainLoopThread = new Thread(() =>
			{

				try
				{
					this.mainLoop();
				}
				catch (Exception t)
				{
					if (t is RuntimeException)
						throw (RuntimeException)t;
					else
						throw new GdxRuntimeException(t);
				}

			})
			{
				Name = "HeadlessApplication"
			};
			mainLoopThread.Start();
		}

		protected void mainLoop()
		{
			Array<ILifecycleListener> lifecycleListeners = this.lifecycleListeners;

			listener.Create();

			// unlike LwjglApplication, a headless application will eat up CPU in this while loop
			// it is up to the implementation to call Thread.sleep as necessary
			long t = TimeUtils.nanoTime() + graphics.getTargetRenderInterval();
			if (graphics.getTargetRenderInterval() >= 0f)
			{
				while (running)
				{
					long n = TimeUtils.nanoTime();
					if (t > n)
					{
						try
						{
							long sleep = t - n;

							// TODO: The original call is Thread.sleep(long, int). C# can't sleep that precisely, and I doubt Java can either.
							Thread.Sleep((int)(sleep / 1000000));//, (int)(sleep % 1000000));
						}
						catch (ThreadInterruptedException e)
						{
						}

						t = t + graphics.getTargetRenderInterval();
					}
					else
						t = n + graphics.getTargetRenderInterval();

					executeRunnables();
					graphics.incrementFrameId();
					listener.Render();
					graphics.updateTime();

					// If one of the runnables set running to false, for example after an exit().
					if (!running) break;
				}
			}

			lock (lifecycleListeners)
			{
				foreach (ILifecycleListener listener in lifecycleListeners)
				{
					listener.Pause();
					listener.Dispose();
				}
			}

			listener.Pause();
			listener.Dispose();
		}

		public bool executeRunnables()
		{
			lock (runnables)
			{
				for (int i = runnables.size - 1; i >= 0; i--)
					executedRunnables.Add(runnables.Get(i));
				runnables.clear();
			}

			if (executedRunnables.size == 0) return false;
			for (int i = executedRunnables.size - 1; i >= 0; i--)
				executedRunnables.RemoveIndex(i).Invoke();
			return true;
		}

		public IApplicationListener GetApplicationListener()
		{
			return listener;
		}

		public IGraphics GetGraphics()
		{
			return graphics;
		}

		public IAudio GetAudio()
		{
			return audio;
		}

		public IInput GetInput()
		{
			return input;
		}

		public IFiles GetFiles()
		{
			return files;
		}

		public INet GetNet()
		{
			return net;
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

		ObjectMap<String, IPreferences> preferences = new ObjectMap<String, IPreferences>();

		public IPreferences GetPreferences(String name)
		{
			if (preferences.containsKey(name))
			{
				return preferences.get(name);
			}
			else
			{
				IPreferences prefs = new HeadlessPreferences(name, this.preferencesdir);
				preferences.put(name, prefs);
				return prefs;
			}
		}

		public IClipboard GetClipboard()
		{
			// no clipboards for headless apps
			return null;
		}

		public void PostRunnable(Runnable runnable)
		{
			lock (runnables)
			{
				runnables.Add(runnable);
			}
		}

		public void Debug(String tag, String message)
		{
			if (logLevel >= LogDebug) GetApplicationLogger().Debug(tag, message);
		}

		public void Debug(String tag, String message, Exception exception)
		{
			if (logLevel >= LogDebug) GetApplicationLogger().Debug(tag, message, exception);
		}

		public void Log(String tag, String message)
		{
			if (logLevel >= LogInfo) GetApplicationLogger().Log(tag, message);
		}

		public void Log(String tag, String message, Exception exception)
		{
			if (logLevel >= LogInfo) GetApplicationLogger().Log(tag, message, exception);
		}

		public void Error(String tag, String message)
		{
			if (logLevel >= LogError) GetApplicationLogger().Error(tag, message);
		}

		public void Error(String tag, String message, Exception exception)
		{
			if (logLevel >= LogError) GetApplicationLogger().Error(tag, message, exception);
		}

		public void SetLogLevel(int logLevel)
		{
			this.logLevel = logLevel;
		}

		public int GetLogLevel()
		{
			return logLevel;
		}

		public void SetApplicationLogger(IApplicationLogger applicationLogger)
		{
			this.applicationLogger = applicationLogger;
		}

		public IApplicationLogger GetApplicationLogger()
		{
			return applicationLogger;
		}

		public void Exit()
		{
			PostRunnable(() =>
			{

				running = false;

			});
		}

		public void AddLifecycleListener(ILifecycleListener listener)
		{
			lock (lifecycleListeners)
			{
				lifecycleListeners.Add(listener);
			}
		}

		public void RemoveLifecycleListener(ILifecycleListener listener)
		{
			lock (lifecycleListeners)
			{
				lifecycleListeners.RemoveValue(listener, true);
			}
		}
	}
}