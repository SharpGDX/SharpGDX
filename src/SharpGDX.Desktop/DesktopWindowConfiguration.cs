using static SharpGDX.IFiles;
using SharpGDX.Graphics;
using static SharpGDX.IGraphics;

namespace SharpGDX.Desktop
{
	public class DesktopWindowConfiguration
	{
		internal int windowX = -1;
		internal int windowY = -1;
		internal int windowWidth = 640;
		internal int windowHeight = 480;
		internal int windowMinWidth = -1, windowMinHeight = -1, windowMaxWidth = -1, windowMaxHeight = -1;
		internal bool windowResizable = true;
		internal bool windowDecorated = true;
		internal bool windowMaximized = false;
		internal DesktopGraphics.DesktopMonitor maximizedMonitor;
		internal bool autoIconify = true;
		internal FileType windowIconFileType;
		internal String[]? windowIconPaths;
		internal IDesktopWindowListener windowListener;
		internal DesktopGraphics.DesktopDisplayMode fullscreenMode;
		internal String title;
		internal Color initialBackgroundColor = Color.BLACK;
		internal bool initialVisible = true;
		internal bool vSyncEnabled = true;

		internal void setWindowConfiguration(DesktopWindowConfiguration config)
		{
			windowX = config.windowX;
			windowY = config.windowY;
			windowWidth = config.windowWidth;
			windowHeight = config.windowHeight;
			windowMinWidth = config.windowMinWidth;
			windowMinHeight = config.windowMinHeight;
			windowMaxWidth = config.windowMaxWidth;
			windowMaxHeight = config.windowMaxHeight;
			windowResizable = config.windowResizable;
			windowDecorated = config.windowDecorated;
			windowMaximized = config.windowMaximized;
			maximizedMonitor = config.maximizedMonitor;
			autoIconify = config.autoIconify;
			windowIconFileType = config.windowIconFileType;
			if (config.windowIconPaths != null)
			{
				windowIconPaths = new string[config.windowIconPaths.Length];
				Array.Copy(config.windowIconPaths, windowIconPaths, config.windowIconPaths.Length);
			}
			windowListener = config.windowListener;
			fullscreenMode = config.fullscreenMode;
			title = config.title;
			initialBackgroundColor = config.initialBackgroundColor;
			initialVisible = config.initialVisible;
			vSyncEnabled = config.vSyncEnabled;
		}

		/** @param visibility whether the window will be visible on creation. (default true) */
		public virtual void setInitialVisible(bool visibility)
		{
			this.initialVisible = visibility;
		}

		/** Sets the app to use windowed mode.
		 * 
		 * @param width the width of the window (default 640)
		 * @param height the height of the window (default 480) */
		public void setWindowedMode(int width, int height)
		{
			this.windowWidth = width;
			this.windowHeight = height;
		}

		/** @param resizable whether the windowed mode window is resizable (default true) */
		public void setResizable(bool resizable)
		{
			this.windowResizable = resizable;
		}

		/** @param decorated whether the windowed mode window is decorated, i.e. displaying the title bars (default true) */
		public void setDecorated(bool decorated)
		{
			this.windowDecorated = decorated;
		}

		/** @param maximized whether the window starts maximized. Ignored if the window is full screen. (default false) */
		public void setMaximized(bool maximized)
		{
			this.windowMaximized = maximized;
		}

		/** @param monitor what monitor the window should maximize to */
		public void setMaximizedMonitor(IGraphics.Monitor monitor)
		{
			this.maximizedMonitor = (DesktopGraphics.DesktopMonitor)monitor;
		}

		/** @param autoIconify whether the window should automatically iconify and restore previous video mode on input focus loss.
		 *           (default true) Does nothing in windowed mode. */
		public void setAutoIconify(bool autoIconify)
		{
			this.autoIconify = autoIconify;
		}

		/** Sets the position of the window in windowed mode. Default -1 for both coordinates for centered on primary monitor. */
		public void setWindowPosition(int x, int y)
		{
			windowX = x;
			windowY = y;
		}

		/** Sets minimum and maximum size limits for the window. If the window is full screen or not resizable, these limits are
		 * ignored. The default for all four parameters is -1, which means unrestricted. */
		public void setWindowSizeLimits(int minWidth, int minHeight, int maxWidth, int maxHeight)
		{
			windowMinWidth = minWidth;
			windowMinHeight = minHeight;
			windowMaxWidth = maxWidth;
			windowMaxHeight = maxHeight;
		}

		/** Sets the icon that will be used in the window's title bar. Has no effect in macOS, which doesn't use window icons.
		 * @param filePaths One or more {@linkplain FileType#Internal internal} image paths. Must be JPEG, PNG, or BMP format. The one
		 *           closest to the system's desired size will be scaled. Good sizes include 16x16, 32x32 and 48x48. */
		public void setWindowIcon(String[]filePaths)
		{
			setWindowIcon(FileType.Internal, filePaths);
		}

		/** Sets the icon that will be used in the window's title bar. Has no effect in macOS, which doesn't use window icons.
		 * @param fileType The type of file handle the paths are relative to.
		 * @param filePaths One or more image paths, relative to the given {@linkplain FileType}. Must be JPEG, PNG, or BMP format. The
		 *           one closest to the system's desired size will be scaled. Good sizes include 16x16, 32x32 and 48x48. */
		public void setWindowIcon(FileType fileType, String[]filePaths)
		{
			windowIconFileType = fileType;
			windowIconPaths = filePaths;
		}

		/** Sets the {@link DesktopWindowListener} which will be informed about iconficiation, focus loss and window close events. */
		public void setWindowListener(IDesktopWindowListener windowListener)
		{
			this.windowListener = windowListener;
		}

		/** Sets the app to use fullscreen mode. Use the static methods like {@link DesktopApplicationConfiguration#getDisplayMode()} on
		 * this class to enumerate connected monitors and their fullscreen display modes. */
		public void setFullscreenMode(DisplayMode mode)
		{
			this.fullscreenMode = (DesktopGraphics.DesktopDisplayMode)mode;
		}

		/** Sets the window title. If null, the application listener's class name is used. */
		public void setTitle(String title)
		{
			this.title = title;
		}

		/** Sets the initial background color. Defaults to black. */
		public void setInitialBackgroundColor(Color color)
		{
			initialBackgroundColor = color;
		}

		/** Sets whether to use vsync. This setting can be changed anytime at runtime via {@link Graphics#setVSync(boolean)}.
		 *
		 * For multi-window applications, only one (the main) window should enable vsync. Otherwise, every window will wait for the
		 * vertical blank on swap individually, effectively cutting the frame rate to (refreshRate / numberOfWindows). */
		public void useVsync(bool vsync)
		{
			this.vSyncEnabled = vsync;
		}
	}
}
