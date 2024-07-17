using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Monitor = SharpGDX.IGraphics.Monitor;
using SharpGDX.Mathematics;
using SharpGDX.Shims;
using SharpGDX.Utils;
using static SharpGDX.IGraphics;

namespace SharpGDX.Desktop
{
	public class DesktopGraphics : AbstractGraphics, Disposable
	{
		readonly DesktopWindow window;
		GL20 gl20;
		private GL30 gl30;
		private GL31 gl31;
		private GL32 gl32;
		private GLVersion glVersion;
		private volatile int backBufferWidth;
		private volatile int backBufferHeight;
		private volatile int logicalWidth;
		private volatile int logicalHeight;
		private volatile bool isContinuous = true;
		private BufferFormat bufferFormat;
		private long lastFrameTime = -1;
		private float deltaTime;
		private bool _resetDeltaTime = false;
		private long frameId;
		private long frameCounterStart = 0;
		private int frames;
		private int fps;
		private int windowPosXBeforeFullscreen;
		private int windowPosYBeforeFullscreen;
		private int windowWidthBeforeFullscreen;
		private int windowHeightBeforeFullscreen;
		private DisplayMode displayModeBeforeFullscreen = null;

		// TODO: this was originally BufferUtils.createIntBuffer, not sure if this works
		IntBuffer tmpBuffer = IntBuffer.allocate(1);
		IntBuffer tmpBuffer2 = IntBuffer.allocate(1);

		private volatile bool posted;

		private GLFWCallbacks.FramebufferSizeCallback _framebufferSizeCallback;

		private unsafe void resizeCallback(Window* windowHandle, int width, int height)
		{
			// TODO: Implement, this might really only be a Java(lwjgl) thing.
			// TODO: This was used to ensure that glfwInit was called on the first thread of the JVM process.
			//if (Configuration.GLFW_CHECK_THREAD0.get(true))
			//{
			//	renderWindow(windowHandle, width, height);
			//}
			//else
			{
				if (posted) return;
				posted = true;
				Gdx.app.postRunnable(() =>
				{

					posted = false;
					renderWindow(windowHandle, width, height);

				});
			}
		}

		private unsafe void renderWindow(Window* windowHandle, int width, int height)
		{
			updateFramebufferInfo();
			if (!window.isListenerInitialized())
			{
				return;
			}

			window.makeCurrent();
			gl20.glViewport(0, 0, backBufferWidth, backBufferHeight);
			window.getListener().Resize(getWidth(), getHeight());
			window.getListener().Render();
			GLFW.SwapBuffers(windowHandle);
		}

		public unsafe DesktopGraphics(DesktopWindow window)
		{
			this.window = window;
			if (window.getConfig().glEmulation == DesktopApplicationConfiguration.GLEmulation.GL32)
			{
				this.gl20 = this.gl30 = this.gl31 = this.gl32 = new DesktopGL32();
			}
			else if (window.getConfig().glEmulation == DesktopApplicationConfiguration.GLEmulation.GL31)
			{
				this.gl20 = this.gl30 = this.gl31 = new DesktopGL31();
			}
			else if (window.getConfig().glEmulation == DesktopApplicationConfiguration.GLEmulation.GL30)
			{
				this.gl20 = this.gl30 = new DesktopGL30();
			}
			else
			{
				try
				{
					this.gl20 = window.getConfig().glEmulation == DesktopApplicationConfiguration.GLEmulation.GL20
						? new DesktopGL20()
						: throw new NotImplementedException(); // TODO: (GL20)Class.forName("com.badlogic.gdx.backends.lwjgl3.angle.Lwjgl3GLES20").newInstance();
				}
				catch (Exception t)
				{
					throw new GdxRuntimeException("Couldn't instantiate GLES20.", t);
				}

				this.gl30 = null;
			}

			updateFramebufferInfo();
			initiateGL();

			// TODO: Clear this in the dispose method
			GLFW.SetFramebufferSizeCallback(window.getWindowPtr(), _framebufferSizeCallback = resizeCallback);
		}

		private void initiateGL()
		{
			String versionString = gl20.glGetString(GL11.GL_VERSION);
			String vendorString = gl20.glGetString(GL11.GL_VENDOR);
			String rendererString = gl20.glGetString(GL11.GL_RENDERER);
			glVersion = new GLVersion(IApplication.ApplicationType.Desktop, versionString, vendorString,
				rendererString);
			if (supportsCubeMapSeamless())
			{
				enableCubeMapSeamless(true);
			}
		}

		/** @return whether cubemap seamless feature is supported. */
		public bool supportsCubeMapSeamless()
		{
			return glVersion.IsVersionEqualToOrHigher(3, 2) || supportsExtension("GL_ARB_seamless_cube_map");
		}

		/** Enable or disable cubemap seamless feature. Default is true if supported. Should only be called if this feature is
		 * supported. (see {@link #supportsCubeMapSeamless()})
		 * @param enable */
		public void enableCubeMapSeamless(bool enable)
		{
			if (enable)
			{
				gl20.glEnable(GL32.GL_TEXTURE_CUBE_MAP_SEAMLESS);
			}
			else
			{
				gl20.glDisable(GL32.GL_TEXTURE_CUBE_MAP_SEAMLESS);
			}
		}

		public DesktopWindow getWindow()
		{
			return window;
		}

		private unsafe void updateFramebufferInfo()
		{
			GLFW.GetFramebufferSize(window.getWindowPtr(), out backBufferWidth, out backBufferHeight);
			GLFW.GetWindowSize(window.getWindowPtr(), out logicalWidth, out logicalHeight);
			DesktopApplicationConfiguration config = window.getConfig();
			bufferFormat = new BufferFormat(config.r, config.g, config.b, config.a, config.depth, config.stencil,
				config.samples,
				false);
		}

		internal void update()
		{
			long time = TimeUtils.nanoTime();
			if (lastFrameTime == -1) lastFrameTime = time;
			if (_resetDeltaTime)
			{
				_resetDeltaTime = false;
				deltaTime = 0;
			}
			else
				deltaTime = (time - lastFrameTime) / 1000000000.0f;

			lastFrameTime = time;

			if (time - frameCounterStart >= 1000000000)
			{
				fps = frames;
				frames = 0;
				frameCounterStart = time;
			}

			frames++;
			frameId++;
		}

		public override bool isGL30Available()
		{
			return gl30 != null;
		}

		public override bool isGL31Available()
		{
			return gl31 != null;
		}

		public override bool isGL32Available()
		{
			return gl32 != null;
		}

		public override GL20 getGL20()
		{
			return gl20;
		}

		public override GL30 getGL30()
		{
			return gl30;
		}

		public override GL31 getGL31()
		{
			return gl31;
		}

		public override GL32 getGL32()
		{
			return gl32;
		}

		public override void setGL20(GL20 gl20)
		{
			this.gl20 = gl20;
		}

		public override void setGL30(GL30 gl30)
		{
			this.gl30 = gl30;
		}

		public override void setGL31(GL31 gl31)
		{
			this.gl31 = gl31;
		}

		public override void setGL32(GL32 gl32)
		{
			this.gl32 = gl32;
		}

		public override int getWidth()
		{
			if (window.getConfig().hdpiMode == HdpiMode.Pixels)
			{
				return backBufferWidth;
			}
			else
			{
				return logicalWidth;
			}
		}

		public override int getHeight()
		{
			if (window.getConfig().hdpiMode == HdpiMode.Pixels)
			{
				return backBufferHeight;
			}
			else
			{
				return logicalHeight;
			}
		}

		public override int getBackBufferWidth()
		{
			return backBufferWidth;
		}

		public override int getBackBufferHeight()
		{
			return backBufferHeight;
		}

		public int getLogicalWidth()
		{
			return logicalWidth;
		}

		public int getLogicalHeight()
		{
			return logicalHeight;
		}

		public override long getFrameId()
		{
			return frameId;
		}

		public override float getDeltaTime()
		{
			return deltaTime;
		}

		public void resetDeltaTime()
		{
			_resetDeltaTime = true;
		}

		public override int getFramesPerSecond()
		{
			return fps;
		}

		public override GraphicsType getType()
		{
			return GraphicsType.OpenGL;
		}

		public override GLVersion getGLVersion()
		{
			return glVersion;
		}

		public override float getPpiX()
		{
			return getPpcX() * 2.54f;
		}

		public override float getPpiY()
		{
			return getPpcY() * 2.54f;
		}

		public override unsafe float getPpcX()
		{
			DesktopMonitor monitor = (DesktopMonitor)getMonitor();
			GLFW.GetMonitorPhysicalSize(monitor.monitorHandle, out var sizeX, out var _);
			DisplayMode mode = getDisplayMode();
			return mode.width / (float)sizeX * 10;
		}

		public override unsafe float getPpcY()
		{
			DesktopMonitor monitor = (DesktopMonitor)getMonitor();
			GLFW.GetMonitorPhysicalSize(monitor.monitorHandle, out var _, out var sizeY);
			DisplayMode mode = getDisplayMode();
			return mode.height / (float)sizeY * 10;
		}

		public override bool supportsDisplayModeChange()
		{
			return true;
		}

		public override unsafe Monitor getPrimaryMonitor()
		{
			return DesktopApplicationConfiguration.toDesktopMonitor(GLFW.GetPrimaryMonitor());
		}

		public override unsafe Monitor getMonitor()
		{
			Monitor[] monitors = getMonitors();
			Monitor result = monitors[0];

			GLFW.GetWindowPos(window.getWindowPtr(), out var windowX, out var windowY);
			GLFW.GetWindowSize(window.getWindowPtr(), out var windowWidth, out var windowHeight);
			int overlap;
			int bestOverlap = 0;

			foreach (Monitor monitor in monitors)
			{
				DisplayMode mode = getDisplayMode(monitor);

				overlap = Math.Max(0,
					          Math.Min(windowX + windowWidth, monitor.virtualX + mode.width) -
					          Math.Max(windowX, monitor.virtualX))
				          * Math.Max(0,
					          Math.Min(windowY + windowHeight, monitor.virtualY + mode.height) -
					          Math.Max(windowY, monitor.virtualY));

				if (bestOverlap < overlap)
				{
					bestOverlap = overlap;
					result = monitor;
				}
			}

			return result;
		}

		public override Monitor[] getMonitors()
		{
			throw new NotImplementedException();
			//PointerBuffer glfwMonitors = GLFW.glfwGetMonitors();
			//Monitor[] monitors = new Monitor[glfwMonitors.limit()];
			//for (int i = 0; i < glfwMonitors.limit(); i++)
			//{
			//	monitors[i] = DesktopApplicationConfiguration.toDesktopMonitor(glfwMonitors.get(i));
			//}
			//return monitors;
		}

		public override DisplayMode[] getDisplayModes()
		{
			return DesktopApplicationConfiguration.getDisplayModes(getMonitor());
		}

		public override DisplayMode[] getDisplayModes(Monitor monitor)
		{
			return DesktopApplicationConfiguration.getDisplayModes(monitor);
		}

		public override DisplayMode getDisplayMode()
		{
			return DesktopApplicationConfiguration.getDisplayMode(getMonitor());
		}

		public override DisplayMode getDisplayMode(Monitor monitor)
		{
			return DesktopApplicationConfiguration.getDisplayMode(monitor);
		}

		public override int getSafeInsetLeft()
		{
			return 0;
		}

		public override int getSafeInsetTop()
		{
			return 0;
		}

		public override int getSafeInsetBottom()
		{
			return 0;
		}

		public override int getSafeInsetRight()
		{
			return 0;
		}

		public override unsafe bool setFullscreenMode(DisplayMode displayMode)
		{
			window.getInput().ResetPollingStates();
			DesktopDisplayMode newMode = (DesktopDisplayMode)displayMode;
			if (isFullscreen())
			{
				DesktopDisplayMode currentMode = (DesktopDisplayMode)getDisplayMode();
				if (currentMode.getMonitor() == newMode.getMonitor() && currentMode.refreshRate == newMode.refreshRate)
				{
					// same monitor and refresh rate
					GLFW.SetWindowSize(window.getWindowPtr(), newMode.width, newMode.height);
				}
				else
				{
					// different monitor and/or refresh rate
					GLFW.SetWindowMonitor(window.getWindowPtr(), newMode.getMonitor(), 0, 0, newMode.width,
						newMode.height,
						newMode.refreshRate);
				}
			}
			else
			{
				// store window position so we can restore it when switching from fullscreen to windowed later
				storeCurrentWindowPositionAndDisplayMode();

				// switch from windowed to fullscreen
				GLFW.SetWindowMonitor(window.getWindowPtr(), newMode.getMonitor(), 0, 0, newMode.width, newMode.height,
					newMode.refreshRate);
			}

			updateFramebufferInfo();

			setVSync(window.getConfig().vSyncEnabled);

			return true;
		}

		private void storeCurrentWindowPositionAndDisplayMode()
		{
			windowPosXBeforeFullscreen = window.getPositionX();
			windowPosYBeforeFullscreen = window.getPositionY();
			windowWidthBeforeFullscreen = logicalWidth;
			windowHeightBeforeFullscreen = logicalHeight;
			displayModeBeforeFullscreen = getDisplayMode();
		}

		public override unsafe bool setWindowedMode(int width, int height)
		{
			window.getInput().ResetPollingStates();
			if (!isFullscreen())
			{
				GridPoint2 newPos = null;
				bool centerWindow = false;
				if (width != logicalWidth || height != logicalHeight)
				{
					centerWindow = true; // recenter the window since its size changed
					newPos = DesktopApplicationConfiguration.calculateCenteredWindowPosition(
						(DesktopMonitor)getMonitor(), width, height);
				}

				GLFW.SetWindowSize(window.getWindowPtr(), width, height);
				if (centerWindow)
				{
					window.setPosition(newPos.x,
						newPos.y); // on macOS the centering has to happen _after_ the new window size was set
				}
			}
			else
			{
				// if we were in fullscreen mode, we should consider restoring a previous display mode
				if (displayModeBeforeFullscreen == null)
				{
					storeCurrentWindowPositionAndDisplayMode();
				}

				if (width != windowWidthBeforeFullscreen || height != windowHeightBeforeFullscreen)
				{
					// center the window since its size
					// changed
					GridPoint2 newPos = DesktopApplicationConfiguration.calculateCenteredWindowPosition(
						(DesktopMonitor)getMonitor(), width,
						height);
					GLFW.SetWindowMonitor(window.getWindowPtr(), null, newPos.x, newPos.y, width, height,
						displayModeBeforeFullscreen.refreshRate);
				}
				else
				{
					// restore previous position
					GLFW.SetWindowMonitor(window.getWindowPtr(), null, windowPosXBeforeFullscreen,
						windowPosYBeforeFullscreen, width,
						height, displayModeBeforeFullscreen.refreshRate);
				}
			}

			updateFramebufferInfo();
			return true;
		}

		public override unsafe void setTitle(String title)
		{
			if (title == null)
			{
				title = "";
			}

			GLFW.SetWindowTitle(window.getWindowPtr(), title);
		}

		public override unsafe void setUndecorated(bool undecorated)
		{
			getWindow().getConfig().setDecorated(!undecorated);
			GLFW.SetWindowAttrib(window.getWindowPtr(), WindowAttribute.Decorated, !undecorated);
		}

		public override unsafe void setResizable(bool resizable)
		{
			getWindow().getConfig().setResizable(resizable);
			GLFW.SetWindowAttrib(window.getWindowPtr(), WindowAttribute.Resizable, resizable);
		}

		public override void setVSync(bool vsync)
		{
			getWindow().getConfig().vSyncEnabled = vsync;
			GLFW.SwapInterval(vsync ? 1 : 0);
		}

		/** Sets the target framerate for the application, when using continuous rendering. Must be positive. The cpu sleeps as needed.
		 * Use 0 to never sleep. If there are multiple windows, the value for the first window created is used for all. Default is 0.
		 *
		 * @param fps fps */
		public override void setForegroundFPS(int fps)
		{
			getWindow().getConfig().foregroundFPS = fps;
		}

		public override BufferFormat getBufferFormat()
		{
			return bufferFormat;
		}

		public override bool supportsExtension(String extension)
		{
			return GLFW.ExtensionSupported(extension);
		}

		public override void setContinuousRendering(bool isContinuous)
		{
			this.isContinuous = isContinuous;
		}

		public override bool isContinuousRendering()
		{
			return isContinuous;
		}

		public override void requestRendering()
		{
			window.requestRendering();
		}

		public override unsafe bool isFullscreen()
		{
			return GLFW.GetWindowMonitor(window.getWindowPtr()) != null;
		}

		public override ICursor newCursor(Pixmap pixmap, int xHotspot, int yHotspot)
		{
			return new DesktopCursor(getWindow(), pixmap, xHotspot, yHotspot);
		}

		public override unsafe void setCursor(ICursor cursor)
		{
			GLFW.SetCursor(getWindow().getWindowPtr(), ((DesktopCursor)cursor).glfwCursor);
		}

		public override unsafe void setSystemCursor(ICursor.SystemCursor systemCursor)
		{
			DesktopCursor.setSystemCursor(getWindow().getWindowPtr(), systemCursor);
		}

		public void dispose()
		{
			// TODO: Set to null
			//this.resizeCallback.free();
		}

		internal class DesktopDisplayMode : DisplayMode
		{
			readonly unsafe OpenTK.Windowing.GraphicsLibraryFramework.Monitor* monitorHandle;

			internal unsafe DesktopDisplayMode(OpenTK.Windowing.GraphicsLibraryFramework.Monitor* monitor, int width,
				int height, int refreshRate, int bitsPerPixel)
				: base(width, height, refreshRate, bitsPerPixel)
			{

				this.monitorHandle = monitor;
			}

			public unsafe OpenTK.Windowing.GraphicsLibraryFramework.Monitor* getMonitor()
			{
				return monitorHandle;
			}
		}

		public class DesktopMonitor : Monitor
		{
			internal unsafe readonly OpenTK.Windowing.GraphicsLibraryFramework.Monitor* monitorHandle;

			internal unsafe DesktopMonitor(OpenTK.Windowing.GraphicsLibraryFramework.Monitor* monitor, int virtualX,
				int virtualY, String name)
				: base(virtualX, virtualY, name)
			{

				this.monitorHandle = monitor;
			}

			public unsafe OpenTK.Windowing.GraphicsLibraryFramework.Monitor* getMonitorHandle()
			{
				return monitorHandle;
			}
		}
	}
}