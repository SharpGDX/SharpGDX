using System.Net;
using SharpGDX.Graphics.GLUtils;
using Monitor = OpenTK.Windowing.GraphicsLibraryFramework.Monitor;
using static OpenTK.Windowing.GraphicsLibraryFramework.GLFWCallbacks;
using System.Runtime.InteropServices;
using OpenTK.Windowing.GraphicsLibraryFramework;
using File = SharpGDX.Shims.File;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Desktop.Audio;
using SharpGDX.Desktop.Audio.Mock;
using SharpGDX.Mathematics;
using OpenTK.Graphics.OpenGL4;
using SharpGDX.Input;
using SharpGDX.Scenes.Scene2D.UI;
using static SharpGDX.Desktop.DesktopApplicationConfiguration;
using Window = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace SharpGDX.Desktop
{
	public class DesktopApplication : IDesktopApplicationBase
	{
		private readonly DesktopApplicationConfiguration config;
		readonly Array<DesktopWindow> windows = new();
		private volatile DesktopWindow currentWindow;
		private IDesktopAudio audio;
		private readonly IFiles files;
		private readonly INet net;
		private readonly ObjectMap<String, IPreferences> preferences = new();
		private readonly DesktopClipboard clipboard;
		private int logLevel = IApplication.LogInfo;
		private IApplicationLogger applicationLogger;
		private volatile bool running = true;
		private readonly Array<Runnable> runnables = new();
		private readonly Array<Runnable> executedRunnables = new();
		private readonly Array<ILifecycleListener> lifecycleListeners = new();
		private static ErrorCallback? errorCallback;
		private static GLVersion glVersion;
		private static DebugProc? glDebugCallback;
		private readonly Sync sync;

		internal static void initializeGlfw()
		{
			if (errorCallback == null)
			{
				DesktopNativesLoader.Load();
				errorCallback = (OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode code, string description) =>
				{
					// TODO: ??? GLFWErrorCallback.createPrint(DesktopApplicationConfiguration.errorStream);

					Console.WriteLine(description);
				};

				GLFW.SetErrorCallback(errorCallback);

                if (SharedLibraryLoader.isMac)
                {
                    throw new NotImplementedException();
                    //GLFW.glfwInitHint(GLFW.GLFW_ANGLE_PLATFORM_TYPE, GLFW.GLFW_ANGLE_PLATFORM_TYPE_METAL);
                }

                GLFW.InitHint(InitHintBool.JoystickHatButtons, false);
				if (!GLFW.Init())
				{
					throw new GdxRuntimeException("Unable to initialize GLFW");
				}
			}
		}

		static void loadANGLE()
		{
			//try {
			//	Class angleLoader = Class.forName("com.badlogic.gdx.backends.lwjgl3.angle.ANGLELoader");
			//	Method load = angleLoader.getMethod("load");
			//	load.invoke(angleLoader);
			//} catch (ClassNotFoundException t) {
			//	return;
			//} catch (Throwable t) {
			//	throw new GdxRuntimeException("Couldn't load ANGLE.", t);
			//}
			throw new NotImplementedException();
		}

		static void postLoadANGLE()
		{
			//try {
			//	Class angleLoader = Class.forName("com.badlogic.gdx.backends.lwjgl3.angle.ANGLELoader");
			//	Method load = angleLoader.getMethod("postGlfwInit");
			//	load.invoke(angleLoader);
			//} catch (ClassNotFoundException t) {
			//	return;
			//} catch (Exception t) {
			//	throw new GdxRuntimeException("Couldn't load ANGLE.", t);
			//}
			throw new NotImplementedException();
		}
        
		public DesktopApplication(IApplicationListener listener)
			: this(listener, new DesktopApplicationConfiguration())
		{

		}

		public unsafe DesktopApplication(IApplicationListener listener, DesktopApplicationConfiguration config)
		{
			if (config.glEmulation == DesktopApplicationConfiguration.GLEmulation.ANGLE_GLES20) loadANGLE();
			initializeGlfw();
			SetApplicationLogger(new DesktopApplicationLogger());

			this.config = config = DesktopApplicationConfiguration.Copy(config);
			if (config.Title == null) config.Title = listener.GetType().Name;

			GDX.App = this;
			if (!config.IsAudioDisabled)
			{
				try
				{
					this.audio = CreateAudio(config);

				}
				catch (Exception t)
				{
					Log("DesktopApplication", "Couldn't initialize audio, disabling audio", t);
					this.audio = new MockAudio();
				}
			}
			else
			{
				this.audio = new MockAudio();
			}

			GDX.Audio = audio;
			this.files = GDX.Files = createFiles();
			this.net = GDX.Net = new DesktopNet(config);
			this.clipboard = new DesktopClipboard();

			this.sync = new Sync();

			DesktopWindow window = createWindow(config, listener, null);
			if (config.glEmulation == DesktopApplicationConfiguration.GLEmulation.ANGLE_GLES20) postLoadANGLE();
			windows.Add(window);
			try
			{
				loop();
				cleanupWindows();
			}
			catch (Exception t)
			{
				if (t is RuntimeException exception)
					throw exception;
				else
					throw new GdxRuntimeException(t);
			}
			finally
			{
				cleanup();
			}
		}

		protected void loop()
		{
			Array<DesktopWindow> closedWindows = new Array<DesktopWindow>();
			while (running && windows.size > 0)
			{
				// FIXME put it on a separate thread
				audio.Update();

				bool haveWindowsRendered = false;
				closedWindows.clear();
				int targetFramerate = -2;
				foreach (DesktopWindow window in windows)
				{
                    if (currentWindow != window)
                    {
                        window.makeCurrent();
                        currentWindow = window;
                    }

                    if (targetFramerate == -2)
                    {
                        targetFramerate = window.getConfig().foregroundFPS;
                    }
					lock (lifecycleListeners)
					{
						haveWindowsRendered |= window.update();
					}

					if (window.shouldClose())
					{
						closedWindows.Add(window);
					}
				}

				GLFW.PollEvents();

				bool shouldRequestRendering;
				lock (runnables)
				{
					shouldRequestRendering = runnables.size > 0;
					executedRunnables.clear();
					executedRunnables.addAll(runnables);
					runnables.clear();
				}

				foreach (Runnable runnable in executedRunnables)
				{
					runnable.Invoke();
				}

				if (shouldRequestRendering)
				{
					// Must follow Runnables execution so changes done by Runnables are reflected
					// in the following render.
					foreach (DesktopWindow window in windows)
					{
						if (!window.getGraphics().IsContinuousRendering()) window.requestRendering();
					}
				}

				foreach (DesktopWindow closedWindow in closedWindows)
				{
					if (windows.size == 1)
					{
						// Lifecycle listener methods have to be called before ApplicationListener methods. The
						// application will be disposed when _all_ windows have been disposed, which is the case,
						// when there is only 1 window left, which is in the process of being disposed.
						for (int i = lifecycleListeners.size - 1; i >= 0; i--)
						{
							ILifecycleListener l = lifecycleListeners.Get(i);
							l.Pause();
							l.Dispose();
						}

						lifecycleListeners.clear();
					}

					closedWindow.Dispose();

					windows.RemoveValue(closedWindow, false);
				}

				if (!haveWindowsRendered)
				{
					// Sleep a few milliseconds in case no rendering was requested
					// with continuous rendering disabled.
					try
					{
						Thread.Sleep(1000 / config.idleFPS);
					}
					catch (ThreadInterruptedException e)
					{
						// ignore
					}
				}
				else if (targetFramerate > 0)
				{
					sync.SyncTo(targetFramerate); // sleep as needed to meet the target framerate
				}
			}
		}

		protected void cleanupWindows()
		{
			lock (lifecycleListeners)
			{
				foreach (ILifecycleListener lifecycleListener in lifecycleListeners)
				{
					lifecycleListener.Pause();
					lifecycleListener.Dispose();
				}
			}

			foreach (DesktopWindow window in windows)
			{
				window.Dispose();
			}

			windows.clear();
		}

		protected void cleanup()
		{
			DesktopCursor.disposeSystemCursors();
			audio.Dispose();
			// TODO: errorCallback.free();
			errorCallback = null;
			if (glDebugCallback != null)
			{
				// TODO: 	glDebugCallback.free();
				glDebugCallback = null;
			}

			GLFW.Terminate();
		}

		public IApplicationListener GetApplicationListener()
		{
			return currentWindow.getListener();
		}

		public IGraphics GetGraphics()
		{
			return currentWindow.getGraphics();
		}

		public SharpGDX.IAudio GetAudio()
		{
			return audio;
		}

		public IInput GetInput()
		{
			return currentWindow.getInput();
		}

		public IFiles GetFiles()
		{
			return files;
		}

		public INet GetNet()
		{
			return net;
		}

		public void Debug(String tag, String message)
		{
			if (logLevel >= IApplication.LogDebug) GetApplicationLogger().Debug(tag, message);
		}

		public void Debug(String tag, String message, Exception exception)
		{
			if (logLevel >= IApplication.LogDebug) GetApplicationLogger().Debug(tag, message, exception);
		}

		public void Log(String tag, String message)
		{
			if (logLevel >= IApplication.LogInfo) GetApplicationLogger().Log(tag, message);
		}

		public void Log(String tag, String message, Exception exception)
		{
			if (logLevel >= IApplication.LogInfo) GetApplicationLogger().Log(tag, message, exception);
		}

		public void Error(String tag, String message)
		{
			if (logLevel >= IApplication.LogError) GetApplicationLogger().Error(tag, message);
		}

		public void Error(String tag, String message, Exception exception)
		{
			if (logLevel >= IApplication.LogError) GetApplicationLogger().Error(tag, message, exception);
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

		public IApplication.ApplicationType GetType()
		{
			return IApplication.ApplicationType.Desktop;
		}

		public int GetVersion()
		{
			return 0;
		}

		public long GetJavaHeap()
		{
			return GC.GetTotalMemory(false);
		}

		public long GetNativeHeap()
		{
			return GetJavaHeap();
		}

		public IPreferences GetPreferences(String name)
		{
			if (preferences.containsKey(name))
			{
				return preferences.get(name);
			}
			else
			{
				IPreferences prefs = new DesktopPreferences(
					new DesktopFileHandle(new File(config.preferencesDirectory, name), config.preferencesFileType));
				preferences.put(name, prefs);
				
				return prefs;
			}
		}

		public IClipboard GetClipboard()
		{
			return clipboard;
		}

		public void PostRunnable(Runnable runnable)
		{
			lock (runnables)
			{
				runnables.Add(runnable);
			}
		}

		public void Exit()
		{
			running = false;
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

		public IDesktopAudio CreateAudio(DesktopApplicationConfiguration config)
		{
			return new OpenALDesktopAudio(config.AudioDeviceSimultaneousSources, config.AudioDeviceBufferCount,
				config.AudioDeviceBufferSize);
		}

		public IDesktopInput CreateInput(DesktopWindow window)
		{
			return new DefaultDesktopInput(window);
		}

		protected IFiles createFiles()
		{
			return new DesktopFiles();
		}

		/** Creates a new {@link DesktopWindow} using the provided listener and {@link DesktopWindowConfiguration}.
		 *
		 * This function only just instantiates a {@link DesktopWindow} and returns immediately. The actual window creation is postponed
		 * with {@link Application#postRunnable(Runnable)} until after all existing windows are updated. */
		public unsafe DesktopWindow newWindow(IApplicationListener listener, DesktopWindowConfiguration config)
		{
			DesktopApplicationConfiguration appConfig = DesktopApplicationConfiguration.Copy(this.config);
			appConfig.SetWindowConfiguration(config);
			if (appConfig.Title == null) appConfig.Title = listener.GetType().Name;
			return createWindow(appConfig, listener, windows.Get(0).getWindowPtr());
		}

		private unsafe DesktopWindow createWindow(DesktopApplicationConfiguration config, IApplicationListener listener,
        Window* sharedContext)
		{
            DesktopWindow window = new DesktopWindow(listener, lifecycleListeners, config, this);
            if (sharedContext == null)
			{
				// the main window is created immediately
				createWindow(window, config, sharedContext);
			}
			else
			{
				// creation of additional windows is deferred to avoid GL context trouble
				PostRunnable(() =>
				{
					createWindow(window, config, sharedContext);
					windows.Add(window);
				});
			}

			return window;
		}

		private unsafe void createWindow(DesktopWindow window, DesktopApplicationConfiguration config,
			Window* sharedContext)
		{
			Window* windowHandle = createGlfwWindow(config, sharedContext);
			window.create(windowHandle);
			window.setVisible(config.InitialVisible);

			for (int i = 0; i < 2; i++)
			{
                window.getGraphics().gl20.glClearColor(config.InitialBackgroundColor.R, config.InitialBackgroundColor.G,
                    config.InitialBackgroundColor.B, config.InitialBackgroundColor.A);
                window.getGraphics().gl20.glClear(GL11.GL_COLOR_BUFFER_BIT);
                GLFW.SwapBuffers(windowHandle);
			}

            if (currentWindow != null)
            {
                // the call above to createGlfwWindow switches the OpenGL context to the newly created window,
                // ensure that the invariant "currentWindow is the window with the current active OpenGL context" holds
                currentWindow.makeCurrent();
            }
        }

		private static unsafe Window* createGlfwWindow(DesktopApplicationConfiguration config,
			Window* sharedContextWindow)
		{
			GLFW.DefaultWindowHints();
			GLFW.WindowHint(WindowHintBool.Visible, false);
			GLFW.WindowHint(WindowHintBool.Resizable, config.WindowResizable);
			GLFW.WindowHint(WindowHintBool.Maximized, config.WindowMaximized);
			GLFW.WindowHint(WindowHintBool.AutoIconify, config.AutoIconify);

			GLFW.WindowHint(WindowHintInt.RedBits, config.r);
			GLFW.WindowHint(WindowHintInt.GreenBits, config.g);
			GLFW.WindowHint(WindowHintInt.BlueBits, config.b);
			GLFW.WindowHint(WindowHintInt.AlphaBits, config.a);
			GLFW.WindowHint(WindowHintInt.StencilBits, config.stencil);
			GLFW.WindowHint(WindowHintInt.DepthBits, config.depth);
			GLFW.WindowHint(WindowHintInt.Samples, config.samples);

			if (config.glEmulation == DesktopApplicationConfiguration.GLEmulation.GL30
			    || config.glEmulation == DesktopApplicationConfiguration.GLEmulation.GL31
			    || config.glEmulation == DesktopApplicationConfiguration.GLEmulation.GL32)
			{
				GLFW.WindowHint(WindowHintInt.ContextVersionMajor, config.gles30ContextMajorVersion);
				GLFW.WindowHint(WindowHintInt.ContextVersionMinor, config.gles30ContextMinorVersion);
				if (SharedLibraryLoader.isMac)
				{
					// hints mandatory on OS X for GL 3.2+ context creation, but fail on Windows if the
					// WGL_ARB_create_context extension is not available
					// see: http://www.glfw.org/docs/latest/compat.html
					GLFW.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
					GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
				}
			}
			else
			{
				if (config.glEmulation == DesktopApplicationConfiguration.GLEmulation.ANGLE_GLES20)
				{
					GLFW.WindowHint(WindowHintContextApi.ContextCreationApi, ContextApi.EglContextApi);
					GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlEsApi);
					GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 2);
					GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 0);
				}
			}

			if (config.transparentFramebuffer)
			{
				GLFW.WindowHint(WindowHintBool.TransparentFramebuffer, true);
			}

			if (config.debug)
			{
				GLFW.WindowHint(WindowHintBool.OpenGLDebugContext, true);
			}

			Window* windowHandle;

			if (config.FullscreenMode != null)
			{
				GLFW.WindowHint(WindowHintInt.RefreshRate, config.FullscreenMode.RefreshRate);
				windowHandle = GLFW.CreateWindow(config.FullscreenMode.Width, config.FullscreenMode.Height,
					config.Title,
					config.FullscreenMode.getMonitor(), sharedContextWindow);
			}
			else
			{
				GLFW.WindowHint(WindowHintBool.Decorated, config.WindowDecorated);
				windowHandle = GLFW.CreateWindow(config.WindowWidth, config.WindowHeight, config.Title, null,
					sharedContextWindow);
			}

			if (windowHandle == null)
			{
				throw new GdxRuntimeException("Couldn't create window");
			}

			DesktopWindow.setSizeLimits(windowHandle, config.WindowMinWidth, config.WindowMinHeight,
				config.WindowMaxWidth,
				config.WindowMaxHeight);
			if (config.FullscreenMode == null)
			{
				if (config.WindowX == -1 && config.WindowY == -1)
				{
					// i.e., center the window
					int windowWidth = Math.Max(config.WindowWidth, config.WindowMinWidth);
					int windowHeight = Math.Max(config.WindowHeight, config.WindowMinHeight);
					if (config.WindowMaxWidth > -1) windowWidth = Math.Min(windowWidth, config.WindowMaxWidth);
					if (config.WindowMaxHeight > -1) windowHeight = Math.Min(windowHeight, config.WindowMaxHeight);

					Monitor* monitorHandle = GLFW.GetPrimaryMonitor();
					if (config.WindowMaximized && config.MaximizedMonitor != null)
					{
						monitorHandle = config.MaximizedMonitor.monitorHandle;
					}

					GridPoint2 newPos = DesktopApplicationConfiguration.CalculateCenteredWindowPosition(
						DesktopApplicationConfiguration.ToDesktopMonitor(monitorHandle), windowWidth, windowHeight);
					GLFW.SetWindowPos(windowHandle, newPos.x, newPos.y);
				}
				else
				{
					GLFW.SetWindowPos(windowHandle, config.WindowX, config.WindowY);
				}

				if (config.WindowMaximized)
				{
					GLFW.MaximizeWindow(windowHandle);
				}
			}

			if (config.WindowIconPaths != null)
			{
				DesktopWindow.setIcon(windowHandle, config.WindowIconPaths, config.WindowIconFileType);
			}

			GLFW.MakeContextCurrent(windowHandle);
			GLFW.SwapInterval(config.VSyncEnabled ? 1 : 0);
			if (config.glEmulation == DesktopApplicationConfiguration.GLEmulation.ANGLE_GLES20)
			{
				try
				{
					//Class gles = Class.forName("org.lwjgl.opengles.GLES");
					//gles.getMethod("createCapabilities").invoke(gles);
					throw new NotImplementedException();
				}
				catch (Exception e)
				{
					throw new GdxRuntimeException("Couldn't initialize GLES", e);
				}
			}
			else
			{
				// TODO: Should this just be a new binding context?
				GL.LoadBindings(new GLFWBindingsContext());
			}

			initiateGL(config.glEmulation == DesktopApplicationConfiguration.GLEmulation.ANGLE_GLES20);
			if (!glVersion.IsVersionEqualToOrHigher(2, 0))
			{
				throw new NotImplementedException();
				//throw new GdxRuntimeException("OpenGL 2.0 or higher with the FBO extension is required. OpenGL version: "
				//                              + GL11.glGetString(GL11.GL_VERSION) + "\n" + glVersion.getDebugVersionString());

			}

			if (config.glEmulation != DesktopApplicationConfiguration.GLEmulation.ANGLE_GLES20 && !supportsFBO())
			{
				throw new NotImplementedException();
                //throw new GdxRuntimeException("OpenGL 2.0 or higher with the FBO extension is required. OpenGL version: "
                //	+ glVersion.getVersionString() + ", FBO extension: false\n" + glVersion.getDebugVersionString());
            }

            if (config.debug)
			{
                if (config.glEmulation == GLEmulation.ANGLE_GLES20)
                {
                    throw new IllegalStateException(
                        "ANGLE currently can't be used with with Lwjgl3ApplicationConfiguration#enableGLDebugOutput");
                }

                // TODO: Remove
                GL.Enable(EnableCap.DebugOutputSynchronous);
				GL.DebugMessageCallback(glDebugCallback = GLDebugCallback, IntPtr.Zero);
				setGLDebugMessageControl(GLDebugMessageSeverity.NOTIFICATION, false);
			}

			return windowHandle;
		}

		private static void GLDebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity,
			int length, IntPtr message, IntPtr userparam)
		{
			Console.WriteLine(Marshal.PtrToStringUTF8(message));
		}


		private static void initiateGL(bool useGLES20)
		{
			if (!useGLES20)
			{
				String versionString = GL.GetString(StringName.Version);
				String vendorString = GL.GetString(StringName.Vendor);
				String rendererString = GL.GetString(StringName.Renderer);
				glVersion = new GLVersion(IApplication.ApplicationType.Desktop, versionString, vendorString,
					rendererString);
			}
			else
			{
				try
				{
					//Class gles = Class.forName("org.lwjgl.opengles.GLES20");
					//Method getString = gles.getMethod("glGetString", int.class);
					//String versionString = (String)getString.invoke(gles, GL11.GL_VERSION);
					//String vendorString = (String)getString.invoke(gles, GL11.GL_VENDOR);
					//String rendererString = (String)getString.invoke(gles, GL11.GL_RENDERER);
					//glVersion = new GLVersion(Application.ApplicationType.Desktop, versionString, vendorString, rendererString);
					throw new NotImplementedException();
				}
				catch (Exception e)
				{
					throw new GdxRuntimeException("Couldn't get GLES version string.", e);
				}
			}
		}

		private static bool supportsFBO()
		{
			// FBO is in core since OpenGL 3.0, see https://www.opengl.org/wiki/Framebuffer_Object
			return glVersion.IsVersionEqualToOrHigher(3, 0) || GLFW.ExtensionSupported("GL_EXT_framebuffer_object")
			                                                || GLFW.ExtensionSupported("GL_ARB_framebuffer_object");
		}

		public enum GLDebugMessageSeverity
		{
			HIGH = DebugSeverityControl.DebugSeverityHigh,
			MEDIUM = DebugSeverityControl.DebugSeverityMedium,
			LOW = DebugSeverityControl.DebugSeverityLow,
			NOTIFICATION = DebugSeverityControl.DebugSeverityNotification


			//HIGH(GL.GL_DEBUG_SEVERITY_HIGH, KHRDebug.GL_DEBUG_SEVERITY_HIGH, ARBDebugOutput.GL_DEBUG_SEVERITY_HIGH_ARB,
			//	AMDDebugOutput.GL_DEBUG_SEVERITY_HIGH_AMD), MEDIUM(GL.GL_DEBUG_SEVERITY_MEDIUM, KHRDebug.GL_DEBUG_SEVERITY_MEDIUM,
			//		ARBDebugOutput.GL_DEBUG_SEVERITY_MEDIUM_ARB, AMDDebugOutput.GL_DEBUG_SEVERITY_MEDIUM_AMD), LOW(
			//			GL43.GL_DEBUG_SEVERITY_LOW, KHRDebug.GL_DEBUG_SEVERITY_LOW, ARBDebugOutput.GL_DEBUG_SEVERITY_LOW_ARB,
			//			AMDDebugOutput.GL_DEBUG_SEVERITY_LOW_AMD), NOTIFICATION(GL.GL_DEBUG_SEVERITY_NOTIFICATION,
			//				KHRDebug.GL_DEBUG_SEVERITY_NOTIFICATION, -1, -1);

			//final int gl43, khr, arb, amd;

			//GLDebugMessageSeverity(int gl43, int khr, int arb, int amd)
			//{
			//	this.gl43 = gl43;
			//	this.khr = khr;
			//	this.arb = arb;
			//	this.amd = amd;
			//}
		}

		/** Enables or disables GL debug messages for the specified severity level. Returns false if the severity level could not be
		 * set (e.g. the NOTIFICATION level is not supported by the ARB and AMD extensions).
		 *
		 * See {@link DesktopApplicationConfiguration#enableGLDebugOutput(bool, PrintStream)} */
		public static bool setGLDebugMessageControl(GLDebugMessageSeverity severity, bool enabled)
		{
			// TODO: GLCapabilities caps = GL.getCapabilities();
			int GL_DONT_CARE = 0x1100; // not defined anywhere yet

			//if (caps.OpenGL43)
			//{
			// TODO: GL.glDebugMessageControl(GL_DONT_CARE, GL_DONT_CARE, severity.gl43, (IntBuffer)null, enabled);
			GL.DebugMessageControl(DebugSourceControl.DontCare, DebugTypeControl.DontCare,
				(DebugSeverityControl)severity, 0, (int[]?)null, enabled);
			return true;
			//	}

			//if (caps.GL_KHR_debug)
			//{
			//	KHRDebug.glDebugMessageControl(GL_DONT_CARE, GL_DONT_CARE, severity.khr, (IntBuffer)null, enabled);
			//	return true;
			//}

			//if (caps.GL_ARB_debug_output && severity.arb != -1)
			//{
			//	ARBDebugOutput.glDebugMessageControlARB(GL_DONT_CARE, GL_DONT_CARE, severity.arb, (IntBuffer)null, enabled);
			//	return true;
			//}

			//if (caps.GL_AMD_debug_output && severity.amd != -1)
			//{
			//	AMDDebugOutput.glDebugMessageEnableAMD(GL_DONT_CARE, severity.amd, (IntBuffer)null, enabled);
			//	return true;
			//}

			return false;
		}

	}
}