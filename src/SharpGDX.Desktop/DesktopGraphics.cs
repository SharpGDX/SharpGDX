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
	public class DesktopGraphics : AbstractGraphics, IDisposable
	{
		readonly DesktopWindow window;
		internal IGL20 gl20;
		private IGL30 gl30;
		private IGL31 gl31;
		private IGL32 gl32;
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
        
		private GLFWCallbacks.FramebufferSizeCallback _framebufferSizeCallback;

		private unsafe void resizeCallback(Window* windowHandle, int width, int height)
		{
            if (!"glfw_async".Equals(Configuration.GLFW_LIBRARY_NAME))
            {
                updateFramebufferInfo();
                if (!window.IsListenerInitialized())
                {
                    return;
                }

                window.MakeCurrent();
                gl20.glViewport(0, 0, backBufferWidth, backBufferHeight);
                window.GetListener().Resize(GetWidth(), GetHeight());
                update();
                window.GetListener().Render();
                GLFW.SwapBuffers(windowHandle);
            }
            else
			{
                window.AsyncResized = true;
            }
		}

		public unsafe DesktopGraphics(DesktopWindow window)
		{
			this.window = window;
			if (window.GetConfig().glEmulation == DesktopApplicationConfiguration.GLEmulation.GL32)
			{
				this.gl20 = this.gl30 = this.gl31 = this.gl32 = new DesktopGL32();
			}
			else if (window.GetConfig().glEmulation == DesktopApplicationConfiguration.GLEmulation.GL31)
			{
				this.gl20 = this.gl30 = this.gl31 = new DesktopGL31();
			}
			else if (window.GetConfig().glEmulation == DesktopApplicationConfiguration.GLEmulation.GL30)
			{
				this.gl20 = this.gl30 = new DesktopGL30();
			}
			else
			{
				try
				{
					this.gl20 = window.GetConfig().glEmulation == DesktopApplicationConfiguration.GLEmulation.GL20
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
			GLFW.SetFramebufferSizeCallback(window.GetWindowPtr(), _framebufferSizeCallback = resizeCallback);
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
			return glVersion.IsVersionEqualToOrHigher(3, 2) || SupportsExtension("GL_ARB_seamless_cube_map");
		}

		/** Enable or disable cubemap seamless feature. Default is true if supported. Should only be called if this feature is
		 * supported. (see {@link #supportsCubeMapSeamless()})
		 * @param enable */
		public void enableCubeMapSeamless(bool enable)
		{
			if (enable)
			{
				gl20.glEnable(IGL32.GL_TEXTURE_CUBE_MAP_SEAMLESS);
			}
			else
			{
				gl20.glDisable(IGL32.GL_TEXTURE_CUBE_MAP_SEAMLESS);
			}
		}

		public DesktopWindow getWindow()
		{
			return window;
		}

		internal unsafe void updateFramebufferInfo()
		{
			GLFW.GetFramebufferSize(window.GetWindowPtr(), out backBufferWidth, out backBufferHeight);
			GLFW.GetWindowSize(window.GetWindowPtr(), out logicalWidth, out logicalHeight);
			DesktopApplicationConfiguration config = window.GetConfig();
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

		public override bool IsGL30Available()
		{
			return gl30 != null;
		}

		public override bool IsGL31Available()
		{
			return gl31 != null;
		}

		public override bool IsGL32Available()
		{
			return gl32 != null;
		}

		public override IGL20 GetGL20()
		{
			return gl20;
		}

		public override IGL30 GetGL30()
		{
			return gl30;
		}

		public override IGL31 GetGL31()
		{
			return gl31;
		}

		public override IGL32 GetGL32()
		{
			return gl32;
		}

		public override void SetGL20(IGL20 gl20)
		{
			this.gl20 = gl20;
		}

		public override void SetGL30(IGL30 gl30)
		{
			this.gl30 = gl30;
		}

		public override void SetGL31(IGL31 gl31)
		{
			this.gl31 = gl31;
		}

		public override void SetGL32(IGL32 gl32)
		{
			this.gl32 = gl32;
		}

		public override int GetWidth()
		{
			if (window.GetConfig().hdpiMode == HdpiMode.Pixels)
			{
				return backBufferWidth;
			}
			else
			{
				return logicalWidth;
			}
		}

		public override int GetHeight()
		{
			if (window.GetConfig().hdpiMode == HdpiMode.Pixels)
			{
				return backBufferHeight;
			}
			else
			{
				return logicalHeight;
			}
		}

		public override int GetBackBufferWidth()
		{
			return backBufferWidth;
		}

		public override int GetBackBufferHeight()
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

		public override long GetFrameId()
		{
			return frameId;
		}

		public override float GetDeltaTime()
		{
			return deltaTime;
		}

		public void resetDeltaTime()
		{
			_resetDeltaTime = true;
		}

		public override int GetFramesPerSecond()
		{
			return fps;
		}

		public override GraphicsType GetType()
		{
			return GraphicsType.OpenGL;
		}

		public override GLVersion GetGLVersion()
		{
			return glVersion;
		}

		public override float GetPpiX()
		{
			return GetPpcX() * 2.54f;
		}

		public override float GetPpiY()
		{
			return GetPpcY() * 2.54f;
		}

		public override unsafe float GetPpcX()
		{
			DesktopMonitor monitor = (DesktopMonitor)GetMonitor();
			GLFW.GetMonitorPhysicalSize(monitor.monitorHandle, out var sizeX, out var _);
			DisplayMode mode = GetDisplayMode();
			return mode.Width / (float)sizeX * 10;
		}

		public override unsafe float GetPpcY()
		{
			DesktopMonitor monitor = (DesktopMonitor)GetMonitor();
			GLFW.GetMonitorPhysicalSize(monitor.monitorHandle, out var _, out var sizeY);
			DisplayMode mode = GetDisplayMode();
			return mode.Height / (float)sizeY * 10;
		}

		public override bool SupportsDisplayModeChange()
		{
			return true;
		}

		public override unsafe Monitor GetPrimaryMonitor()
		{
			return DesktopApplicationConfiguration.ToDesktopMonitor(GLFW.GetPrimaryMonitor());
		}

		public override unsafe Monitor GetMonitor()
		{
			Monitor[] monitors = GetMonitors();
			Monitor result = monitors[0];

			GLFW.GetWindowPos(window.GetWindowPtr(), out var windowX, out var windowY);
			GLFW.GetWindowSize(window.GetWindowPtr(), out var windowWidth, out var windowHeight);
			int overlap;
			int bestOverlap = 0;

			foreach (Monitor monitor in monitors)
			{
				DisplayMode mode = GetDisplayMode(monitor);

				overlap = Math.Max(0,
					          Math.Min(windowX + windowWidth, monitor.VirtualX + mode.Width) -
					          Math.Max(windowX, monitor.VirtualX))
				          * Math.Max(0,
					          Math.Min(windowY + windowHeight, monitor.VirtualY + mode.Height) -
					          Math.Max(windowY, monitor.VirtualY));

				if (bestOverlap < overlap)
				{
					bestOverlap = overlap;
					result = monitor;
				}
			}

			return result;
		}

		public override Monitor[] GetMonitors()
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

		public override DisplayMode[] GetDisplayModes()
		{
			return DesktopApplicationConfiguration.GetDisplayModes(GetMonitor());
		}

		public override DisplayMode[] GetDisplayModes(Monitor monitor)
		{
			return DesktopApplicationConfiguration.GetDisplayModes(monitor);
		}

		public override DisplayMode GetDisplayMode()
		{
			return DesktopApplicationConfiguration.GetDisplayMode(GetMonitor());
		}

		public override DisplayMode GetDisplayMode(Monitor monitor)
		{
			return DesktopApplicationConfiguration.GetDisplayMode(monitor);
		}

		public override int GetSafeInsetLeft()
		{
			return 0;
		}

		public override int GetSafeInsetTop()
		{
			return 0;
		}

		public override int GetSafeInsetBottom()
		{
			return 0;
		}

		public override int GetSafeInsetRight()
		{
			return 0;
		}

		public override unsafe bool SetFullscreenMode(DisplayMode displayMode)
		{
			window.GetInput().ResetPollingStates();
			DesktopDisplayMode newMode = (DesktopDisplayMode)displayMode;
			if (IsFullscreen())
			{
				DesktopDisplayMode currentMode = (DesktopDisplayMode)GetDisplayMode();
				if (currentMode.getMonitor() == newMode.getMonitor() && currentMode.RefreshRate == newMode.RefreshRate)
				{
					// same monitor and refresh rate
					GLFW.SetWindowSize(window.GetWindowPtr(), newMode.Width, newMode.Height);
				}
				else
				{
					// different monitor and/or refresh rate
					GLFW.SetWindowMonitor(window.GetWindowPtr(), newMode.getMonitor(), 0, 0, newMode.Width,
						newMode.Height,
						newMode.RefreshRate);
				}
			}
			else
			{
				// store window position so we can restore it when switching from fullscreen to windowed later
				storeCurrentWindowPositionAndDisplayMode();

				// switch from windowed to fullscreen
				GLFW.SetWindowMonitor(window.GetWindowPtr(), newMode.getMonitor(), 0, 0, newMode.Width, newMode.Height,
					newMode.RefreshRate);
			}

			updateFramebufferInfo();

			SetVSync(window.GetConfig().VSyncEnabled);

			return true;
		}

		private void storeCurrentWindowPositionAndDisplayMode()
		{
			windowPosXBeforeFullscreen = window.GetPositionX();
			windowPosYBeforeFullscreen = window.GetPositionY();
			windowWidthBeforeFullscreen = logicalWidth;
			windowHeightBeforeFullscreen = logicalHeight;
			displayModeBeforeFullscreen = GetDisplayMode();
		}

		public override unsafe bool SetWindowedMode(int width, int height)
		{
			window.GetInput().ResetPollingStates();
			if (!IsFullscreen())
			{
				GridPoint2 newPos = null;
				bool centerWindow = false;
				if (width != logicalWidth || height != logicalHeight)
				{
					centerWindow = true; // recenter the window since its size changed
					newPos = DesktopApplicationConfiguration.CalculateCenteredWindowPosition(
						(DesktopMonitor)GetMonitor(), width, height);
				}

				GLFW.SetWindowSize(window.GetWindowPtr(), width, height);
				if (centerWindow)
				{
					window.SetPosition(newPos.x,
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
					GridPoint2 newPos = DesktopApplicationConfiguration.CalculateCenteredWindowPosition(
						(DesktopMonitor)GetMonitor(), width,
						height);
					GLFW.SetWindowMonitor(window.GetWindowPtr(), null, newPos.x, newPos.y, width, height,
						displayModeBeforeFullscreen.RefreshRate);
				}
				else
				{
					// restore previous position
					GLFW.SetWindowMonitor(window.GetWindowPtr(), null, windowPosXBeforeFullscreen,
						windowPosYBeforeFullscreen, width,
						height, displayModeBeforeFullscreen.RefreshRate);
				}
			}

			updateFramebufferInfo();
			return true;
		}

		public override unsafe void SetTitle(String title)
		{
			if (title == null)
			{
				title = "";
			}

			GLFW.SetWindowTitle(window.GetWindowPtr(), title);
		}

		public override unsafe void SetUndecorated(bool undecorated)
		{
			getWindow().GetConfig().SetDecorated(!undecorated);
			GLFW.SetWindowAttrib(window.GetWindowPtr(), WindowAttribute.Decorated, !undecorated);
		}

		public override unsafe void SetResizable(bool resizable)
		{
			getWindow().GetConfig().SetResizable(resizable);
			GLFW.SetWindowAttrib(window.GetWindowPtr(), WindowAttribute.Resizable, resizable);
		}

		public override void SetVSync(bool vsync)
		{
			getWindow().GetConfig().VSyncEnabled = vsync;
			GLFW.SwapInterval(vsync ? 1 : 0);
		}

		/** Sets the target framerate for the application, when using continuous rendering. Must be positive. The cpu sleeps as needed.
		 * Use 0 to never sleep. If there are multiple windows, the value for the first window created is used for all. Default is 0.
		 *
		 * @param fps fps */
		public override void SetForegroundFPS(int fps)
		{
			getWindow().GetConfig().foregroundFPS = fps;
		}

		public override BufferFormat GetBufferFormat()
		{
			return bufferFormat;
		}

		public override bool SupportsExtension(String extension)
		{
			return GLFW.ExtensionSupported(extension);
		}

		public override void SetContinuousRendering(bool isContinuous)
		{
			this.isContinuous = isContinuous;
		}

		public override bool IsContinuousRendering()
		{
			return isContinuous;
		}

		public override void RequestRendering()
		{
			window.RequestRendering();
		}

		public override unsafe bool IsFullscreen()
		{
			return GLFW.GetWindowMonitor(window.GetWindowPtr()) != null;
		}

		public override ICursor NewCursor(Pixmap pixmap, int xHotspot, int yHotspot)
		{
			return new DesktopCursor(getWindow(), pixmap, xHotspot, yHotspot);
		}

		public override unsafe void SetCursor(ICursor cursor)
		{
			GLFW.SetCursor(getWindow().GetWindowPtr(), ((DesktopCursor)cursor).glfwCursor);
		}

		public override unsafe void SetSystemCursor(ICursor.SystemCursor systemCursor)
		{
			DesktopCursor.setSystemCursor(getWindow().GetWindowPtr(), systemCursor);
		}

		public void Dispose()
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