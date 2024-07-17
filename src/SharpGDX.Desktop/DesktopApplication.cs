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
		private int logLevel = IApplication.LOG_INFO;
		private IApplicationLogger applicationLogger;
		private volatile bool running = true;
		private readonly Array<Runnable> runnables = new();
		private readonly Array<Runnable> executedRunnables = new();
		private readonly Array<ILifecycleListener> lifecycleListeners = new();
		private static ErrorCallback errorCallback;
		private static GLVersion glVersion;
		private static DebugProc? glDebugCallback;
		private readonly Sync sync;

		internal static void initializeGlfw()
		{
			if (errorCallback == null)
			{
				if (SharedLibraryLoader.isMac)
				{
					//loadGlfwAwtMacos();
					throw new NotImplementedException();
				}

				DesktopNativesLoader.Load();
				errorCallback = (OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode code, string description) =>
				{
					// TODO: ??? GLFWErrorCallback.createPrint(DesktopApplicationConfiguration.errorStream);

					Console.WriteLine(description);
				};

				GLFW.SetErrorCallback(errorCallback);

				if (SharedLibraryLoader.isMac)
				{
					// TODO: GLFW.InitHint(GLFW.GLFW_ANGLE_PLATFORM_TYPE, GLFW.GLFW_ANGLE_PLATFORM_TYPE_METAL);
					throw new NotImplementedException();
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

		static void loadGlfwAwtMacos()
		{
			//try {
			//	Class loader = Class.forName("com.badlogic.gdx.backends.lwjgl3.awt.GlfwAWTLoader");
			//	Method load = loader.getMethod("load");
			//	WebRequestMethods.File sharedLib = (WebRequestMethods.File)load.invoke(loader);
			//	Configuration.GLFW_LIBRARY_NAME.set(sharedLib.getAbsolutePath());
			//	Configuration.GLFW_CHECK_THREAD0.set(false);
			//} catch (ClassNotFoundException t) {
			//	return;
			//} catch (Exception t) {
			//	throw new GdxRuntimeException("Couldn't load GLFW AWT for macOS.", t);
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
			setApplicationLogger(new DesktopApplicationLogger());

			this.config = config = DesktopApplicationConfiguration.copy(config);
			if (config.title == null) config.title = listener.GetType().Name;

			Gdx.app = this;
			if (!config._disableAudio)
			{
				try
				{
					this.audio = CreateAudio(config);

				}
				catch (Exception t)
				{
					log("DesktopApplication", "Couldn't initialize audio, disabling audio", t);
					this.audio = new MockAudio();
				}
			}
			else
			{
				this.audio = new MockAudio();
			}

			Gdx.audio = audio;
			this.files = Gdx.files = createFiles();
			this.net = Gdx.net = new DesktopNet(config);
			this.clipboard = new DesktopClipboard();

			this.sync = new Sync();

			DesktopWindow window = createWindow(config, listener, null);
			if (config.glEmulation == DesktopApplicationConfiguration.GLEmulation.ANGLE_GLES20) postLoadANGLE();
			windows.add(window);
			try
			{
				loop();
				cleanupWindows();
			}
			catch (Exception t)
			{
				if (t is RuntimeException)
					throw (RuntimeException)t;
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
					window.makeCurrent();
					currentWindow = window;
					if (targetFramerate == -2) targetFramerate = window.getConfig().foregroundFPS;
					lock (lifecycleListeners)
					{
						haveWindowsRendered |= window.update();
					}

					if (window.shouldClose())
					{
						closedWindows.add(window);
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
						if (!window.getGraphics().isContinuousRendering()) window.requestRendering();
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
							ILifecycleListener l = lifecycleListeners.get(i);
							l.pause();
							l.dispose();
						}

						lifecycleListeners.clear();
					}

					closedWindow.dispose();

					windows.removeValue(closedWindow, false);
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
					lifecycleListener.pause();
					lifecycleListener.dispose();
				}
			}

			foreach (DesktopWindow window in windows)
			{
				window.dispose();
			}

			windows.clear();
		}

		protected void cleanup()
		{
			DesktopCursor.disposeSystemCursors();
			audio.dispose();
			// TODO: errorCallback.free();
			errorCallback = null;
			if (glDebugCallback != null)
			{
				// TODO: 	glDebugCallback.free();
				glDebugCallback = null;
			}

			GLFW.Terminate();
		}

		public IApplicationListener getApplicationListener()
		{
			return currentWindow.getListener();
		}

		public IGraphics getGraphics()
		{
			return currentWindow.getGraphics();
		}

		public SharpGDX.IAudio getAudio()
		{
			return audio;
		}

		public IInput getInput()
		{
			return currentWindow.getInput();
		}

		public IFiles getFiles()
		{
			return files;
		}

		public INet getNet()
		{
			return net;
		}

		public void debug(String tag, String message)
		{
			if (logLevel >= IApplication.LOG_DEBUG) getApplicationLogger().Debug(tag, message);
		}

		public void debug(String tag, String message, Exception exception)
		{
			if (logLevel >= IApplication.LOG_DEBUG) getApplicationLogger().Debug(tag, message, exception);
		}

		public void log(String tag, String message)
		{
			if (logLevel >= IApplication.LOG_INFO) getApplicationLogger().Log(tag, message);
		}

		public void log(String tag, String message, Exception exception)
		{
			if (logLevel >= IApplication.LOG_INFO) getApplicationLogger().Log(tag, message, exception);
		}

		public void error(String tag, String message)
		{
			if (logLevel >= IApplication.LOG_ERROR) getApplicationLogger().Error(tag, message);
		}

		public void error(String tag, String message, Exception exception)
		{
			if (logLevel >= IApplication.LOG_ERROR) getApplicationLogger().Error(tag, message, exception);
		}

		public void setLogLevel(int logLevel)
		{
			this.logLevel = logLevel;
		}

		public int getLogLevel()
		{
			return logLevel;
		}

		public void setApplicationLogger(IApplicationLogger applicationLogger)
		{
			this.applicationLogger = applicationLogger;
		}

		public IApplicationLogger getApplicationLogger()
		{
			return applicationLogger;
		}

		public IApplication.ApplicationType getType()
		{
			return IApplication.ApplicationType.Desktop;
		}

		public int getVersion()
		{
			return 0;
		}

		public long getJavaHeap()
		{
			return GC.GetTotalMemory(false);
		}

		public long getNativeHeap()
		{
			return getJavaHeap();
		}

		public IPreferences getPreferences(String name)
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

		public IClipboard getClipboard()
		{
			return clipboard;
		}

		public void postRunnable(Runnable runnable)
		{
			lock (runnables)
			{
				runnables.add(runnable);
			}
		}

		public void exit()
		{
			running = false;
		}

		public void addLifecycleListener(ILifecycleListener listener)
		{
			lock (lifecycleListeners)
			{
				lifecycleListeners.add(listener);
			}
		}

		public void removeLifecycleListener(ILifecycleListener listener)
		{
			lock (lifecycleListeners)
			{
				lifecycleListeners.removeValue(listener, true);
			}
		}

		public IDesktopAudio CreateAudio(DesktopApplicationConfiguration config)
		{
			return new OpenALDesktopAudio(config.audioDeviceSimultaneousSources, config.audioDeviceBufferCount,
				config.audioDeviceBufferSize);
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
			DesktopApplicationConfiguration appConfig = DesktopApplicationConfiguration.copy(this.config);
			appConfig.setWindowConfiguration(config);
			if (appConfig.title == null) appConfig.title = listener.GetType().Name;
			return createWindow(appConfig, listener, windows.get(0).getWindowPtr());
		}

		private unsafe DesktopWindow createWindow(DesktopApplicationConfiguration config, IApplicationListener listener,
			Window* sharedContext)
		{
			DesktopWindow window = new DesktopWindow(listener, config, this);
			if (sharedContext == null)
			{
				// the main window is created immediately
				createWindow(window, config, sharedContext);
			}
			else
			{
				// creation of additional windows is deferred to avoid GL context trouble
				postRunnable(() =>
				{
					createWindow(window, config, sharedContext);
					windows.add(window);
				});
			}

			return window;
		}

		private unsafe void createWindow(DesktopWindow window, DesktopApplicationConfiguration config,
			Window* sharedContext)
		{
			Window* windowHandle = createGlfwWindow(config, sharedContext);
			window.create(windowHandle);
			window.setVisible(config.initialVisible);

			for (int i = 0; i < 2; i++)
			{
				GL.ClearColor(config.initialBackgroundColor.r, config.initialBackgroundColor.g,
					config.initialBackgroundColor.b, config.initialBackgroundColor.a);
				GL.Clear(ClearBufferMask.ColorBufferBit);
				GLFW.SwapBuffers(windowHandle);
			}
		}

		private static unsafe Window* createGlfwWindow(DesktopApplicationConfiguration config,
			Window* sharedContextWindow)
		{
			GLFW.DefaultWindowHints();
			GLFW.WindowHint(WindowHintBool.Visible, false);
			GLFW.WindowHint(WindowHintBool.Resizable, config.windowResizable);
			GLFW.WindowHint(WindowHintBool.Maximized, config.windowMaximized);
			GLFW.WindowHint(WindowHintBool.AutoIconify, config.autoIconify);

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

			if (config.fullscreenMode != null)
			{
				GLFW.WindowHint(WindowHintInt.RefreshRate, config.fullscreenMode.refreshRate);
				windowHandle = GLFW.CreateWindow(config.fullscreenMode.width, config.fullscreenMode.height,
					config.title,
					config.fullscreenMode.getMonitor(), sharedContextWindow);
			}
			else
			{
				GLFW.WindowHint(WindowHintBool.Decorated, config.windowDecorated);
				windowHandle = GLFW.CreateWindow(config.windowWidth, config.windowHeight, config.title, null,
					sharedContextWindow);
			}

			if (windowHandle == null)
			{
				throw new GdxRuntimeException("Couldn't create window");
			}

			DesktopWindow.setSizeLimits(windowHandle, config.windowMinWidth, config.windowMinHeight,
				config.windowMaxWidth,
				config.windowMaxHeight);
			if (config.fullscreenMode == null)
			{
				if (config.windowX == -1 && config.windowY == -1)
				{
					// i.e., center the window
					int windowWidth = Math.Max(config.windowWidth, config.windowMinWidth);
					int windowHeight = Math.Max(config.windowHeight, config.windowMinHeight);
					if (config.windowMaxWidth > -1) windowWidth = Math.Min(windowWidth, config.windowMaxWidth);
					if (config.windowMaxHeight > -1) windowHeight = Math.Min(windowHeight, config.windowMaxHeight);

					Monitor* monitorHandle = GLFW.GetPrimaryMonitor();
					if (config.windowMaximized && config.maximizedMonitor != null)
					{
						monitorHandle = config.maximizedMonitor.monitorHandle;
					}

					GridPoint2 newPos = DesktopApplicationConfiguration.calculateCenteredWindowPosition(
						DesktopApplicationConfiguration.toDesktopMonitor(monitorHandle), windowWidth, windowHeight);
					GLFW.SetWindowPos(windowHandle, newPos.x, newPos.y);
				}
				else
				{
					GLFW.SetWindowPos(windowHandle, config.windowX, config.windowY);
				}

				if (config.windowMaximized)
				{
					GLFW.MaximizeWindow(windowHandle);
				}
			}

			if (config.windowIconPaths != null)
			{
				//DesktopWindow.setIcon(windowHandle, config.windowIconPaths, config.windowIconFileType);
				throw new NotImplementedException();
			}

			GLFW.MakeContextCurrent(windowHandle);
			GLFW.SwapInterval(config.vSyncEnabled ? 1 : 0);
			if (config.glEmulation == DesktopApplicationConfiguration.GLEmulation.ANGLE_GLES20)
			{
				try
				{
					//Class gles = Class.forName("org.lwjgl.opengles.GLES");
					//gles.getMethod("createCapabilities").invoke(gles);
					//// TODO: Remove once https://github.com/LWJGL/lwjgl3/issues/931 is fixed
					//ThreadLocalUtil.setFunctionMissingAddresses(0);
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
				//	+ GL11.glGetString(GL11.GL_VERSION) + ", FBO extension: false\n" + glVersion.getDebugVersionString());
			}

			if (config.debug)
			{
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