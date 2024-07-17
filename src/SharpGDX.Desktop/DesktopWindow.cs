using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static OpenTK.Windowing.GraphicsLibraryFramework.GLFWCallbacks;
using SharpGDX;
using SharpGDX.Desktop;
using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX.Desktop
{
	public class DesktopWindow : Disposable {
	private unsafe Window* windowHandle;
	readonly IApplicationListener listener;
	readonly IDesktopApplicationBase application;
	private bool listenerInitialized = false;
	IDesktopWindowListener windowListener;
	private DesktopGraphics graphics;
	private IDesktopInput input;
	private readonly DesktopApplicationConfiguration config;
	private readonly Array<Runnable> runnables = new ();
	private readonly Array<Runnable> executedRunnables = new();
	private readonly IntBuffer tmpBuffer;
	private readonly IntBuffer tmpBuffer2;
	bool iconified = false;
	bool focused = false;
	private bool _requestRendering = false;

	private WindowFocusCallback _focusCallback;

	private WindowIconifyCallback iconifyCallback;
	private WindowMaximizeCallback _maximizeCallback;
		
		private WindowCloseCallback closeCallback;
	

	private unsafe void dropCallback (Window* windowHandle, int count, byte** names) {
			String[] files = new String[count];
			for (int i = 0; i < count; i++) {
				// TODO: Do these need to be freed?
				files[i] = Marshal.PtrToStringUTF8((IntPtr)names[i]);
			}
			postRunnable(() => {
				
					if (windowListener != null) {
						windowListener.FilesDropped(files);
					}
			});
		}

		private WindowRefreshCallback refreshCallback;

	internal DesktopWindow (IApplicationListener listener, DesktopApplicationConfiguration config, IDesktopApplicationBase application) {
		this.listener = listener;
		this.windowListener = config.windowListener;
		this.config = config;
		this.application = application;

			// TODO: This was originally BufferUtils.createIntBuffer, not sure if this is right.
			this.tmpBuffer = IntBuffer.allocate(1);
		this.tmpBuffer2 = IntBuffer.allocate(1);
	}

	internal unsafe void create (Window* windowHandle) {
		this.windowHandle = windowHandle;
		this.input = application.CreateInput(this);
		this.graphics = new DesktopGraphics(this);

		GLFW.SetWindowFocusCallback
		(
			windowHandle,
			_focusCallback = (_, focused) => postRunnable(() =>
			{

				if (windowListener != null)
				{
					if (focused)
					{
						windowListener.FocusGained();
					}
					else
					{
						windowListener.FocusLost();
					}

					this.focused = focused;
				}
			})
		);

		GLFW.SetWindowIconifyCallback
		(
			windowHandle,
			iconifyCallback = (_, iconified) => postRunnable(() =>
			{

				if (windowListener != null)
				{
					windowListener.Iconified(iconified);
				}

				this.iconified = iconified;
				if (iconified)
				{
					listener.Pause();
				}
				else
				{
					listener.Resume();
				}
			})
		);

		GLFW.SetWindowMaximizeCallback(windowHandle, _maximizeCallback = (window, maximized) => postRunnable(() =>
		{
			if (windowListener != null)
			{
				windowListener.Maximized(maximized);
			}

		}));

		GLFW.SetWindowCloseCallback
		(
			windowHandle,
			closeCallback = (_) => postRunnable(() =>
			{

				if (windowListener != null)
				{
					if (!windowListener.CloseRequested())
					{
						GLFW.SetWindowShouldClose(windowHandle, false);
					}
				}

			})
		);

		GLFW.SetDropCallback(windowHandle, dropCallback);

		GLFW.SetWindowRefreshCallback
		(
			windowHandle,
			refreshCallback = (_) => postRunnable(() =>
			{
				if (windowListener != null)
				{
					windowListener.RefreshRequested();
				}

			})
		);

		if (windowListener != null) {
			windowListener.Created(this);
		}
	}

	/** @return the {@link ApplicationListener} associated with this window **/
	public IApplicationListener getListener () {
		return listener;
	}

		/** @return the {@link DesktopWindowListener} set on this window **/
		public IDesktopWindowListener getWindowListener () {
		return windowListener;
	}

	public void setWindowListener (IDesktopWindowListener listener) {
		this.windowListener = listener;
	}

	/** Post a {@link Runnable} to this window's event queue. Use this if you access statics like {@link Gdx#graphics} in your
	 * runnable instead of {@link Application#postRunnable(Runnable)}. */
	public void postRunnable (Runnable runnable) {
		lock (runnables) {
			runnables.add(runnable);
		}
	}

	/** Sets the position of the window in logical coordinates. All monitors span a virtual surface together. The coordinates are
	 * relative to the first monitor in the virtual surface. **/
	public unsafe void setPosition (int x, int y) {
		GLFW.SetWindowPos(windowHandle, x, y);
	}

	/** @return the window position in logical coordinates. All monitors span a virtual surface together. The coordinates are
	 *         relative to the first monitor in the virtual surface. **/
	public unsafe int getPositionX () {
		GLFW.GetWindowPos(windowHandle, out var x, out var y);
		return x;
	}

	/** @return the window position in logical coordinates. All monitors span a virtual surface together. The coordinates are
	 *         relative to the first monitor in the virtual surface. **/
	public unsafe int getPositionY () {
		GLFW.GetWindowPos(windowHandle, out var x, out var y);
		return y;
	}

	/** Sets the visibility of the window. Invisible windows will still call their {@link ApplicationListener} */
	public unsafe void setVisible (bool visible) {
		if (visible) {
			GLFW.ShowWindow(windowHandle);
		} else {
			GLFW.HideWindow(windowHandle);
		}
	}

	/** Closes this window and pauses and disposes the associated {@link ApplicationListener}. */
	public unsafe void closeWindow () {
		GLFW.SetWindowShouldClose(windowHandle, true);
	}

	/** Minimizes (iconifies) the window. Iconified windows do not call their {@link ApplicationListener} until the window is
	 * restored. */
	public unsafe void iconifyWindow () {
		GLFW.IconifyWindow(windowHandle);
	}

	/** Whether the window is iconfieid */
	public bool isIconified () {
		return iconified;
	}

	/** De-minimizes (de-iconifies) and de-maximizes the window. */
	public unsafe void restoreWindow () {
		GLFW.RestoreWindow(windowHandle);
	}

	/** Maximizes the window. */
	public unsafe void maximizeWindow () {
		GLFW.MaximizeWindow(windowHandle);
	}

	/** Brings the window to front and sets input focus. The window should already be visible and not iconified. */
	public unsafe void focusWindow () {
		GLFW.FocusWindow(windowHandle);
	}

	public bool isFocused () {
		return focused;
	}

	/** Sets the icon that will be used in the window's title bar. Has no effect in macOS, which doesn't use window icons.
	 * @param image One or more images. The one closest to the system's desired size will be scaled. Good sizes include 16x16,
	 *           32x32 and 48x48. Pixmap format {@link com.badlogic.gdx.graphics.Pixmap.Format#RGBA8888 RGBA8888} is preferred so
	 *           the images will not have to be copied and converted. The chosen image is copied, and the provided Pixmaps are not
	 *           disposed. */
	public unsafe void setIcon (Pixmap[] image) {
		setIcon(windowHandle, image);
	}

	private static unsafe void setIcon (Window* windowHandle, String[] imagePaths, IFiles.FileType imageFileType) {
		if (SharedLibraryLoader.isMac) return;

		Pixmap[] pixmaps = new Pixmap[imagePaths.Length];
		for (int i = 0; i < imagePaths.Length; i++) {
			pixmaps[i] = new Pixmap(Gdx.files.getFileHandle(imagePaths[i], imageFileType));
		}

		setIcon(windowHandle, pixmaps);

		foreach (Pixmap pixmap in pixmaps) {
			pixmap.dispose();
		}
	}

	private static unsafe void setIcon (Window* windowHandle, Pixmap[] images) {
		//if (SharedLibraryLoader.isMac) return;

		//GLFWImage.Buffer buffer = GLFWImage.malloc(images.Length);
		//Pixmap[] tmpPixmaps = new Pixmap[images.length];

		//for (int i = 0; i < images.length; i++) {
		//	Pixmap pixmap = images[i];

		//	if (pixmap.getFormat() != Pixmap.Format.RGBA8888) {
		//		Pixmap rgba = new Pixmap(pixmap.getWidth(), pixmap.getHeight(), Pixmap.Format.RGBA8888);
		//		rgba.setBlending(Pixmap.Blending.None);
		//		rgba.drawPixmap(pixmap, 0, 0);
		//		tmpPixmaps[i] = rgba;
		//		pixmap = rgba;
		//	}

		//	GLFWImage icon = GLFWImage.malloc();
		//	icon.set(pixmap.getWidth(), pixmap.getHeight(), pixmap.getPixels());
		//	buffer.put(icon);

		//	icon.free();
		//}

		//buffer.position(0);
		//GLFW.glfwSetWindowIcon(windowHandle, buffer);

		//buffer.free();
		//foreach (Pixmap pixmap in tmpPixmaps) {
		//	if (pixmap != null) {
		//		pixmap.dispose();
		//	}
		//}

	}

	public unsafe void setTitle (string title) {
		GLFW.SetWindowTitle(windowHandle, title);
	}

	/** Sets minimum and maximum size limits for the window. If the window is full screen or not resizable, these limits are
	 * ignored. Use -1 to indicate an unrestricted dimension. */
	public unsafe void setSizeLimits (int minWidth, int minHeight, int maxWidth, int maxHeight) {
		setSizeLimits(windowHandle, minWidth, minHeight, maxWidth, maxHeight);
	}

	internal static unsafe void setSizeLimits (Window* windowHandle, int minWidth, int minHeight, int maxWidth, int maxHeight) {
		GLFW.SetWindowSizeLimits(windowHandle, minWidth > -1 ? minWidth : GLFW.DontCare,
			minHeight > -1 ? minHeight : GLFW.DontCare, maxWidth > -1 ? maxWidth : GLFW.DontCare,
			maxHeight > -1 ? maxHeight : GLFW.DontCare);
	}

	internal DesktopGraphics getGraphics () {
		return graphics;
	}

	internal IDesktopInput getInput () {
		return input;
	}



	public unsafe long getWindowHandle () {
		// TODO: This should be an IntPtr and should be marshaled.
		return (long)windowHandle;
	}

	internal unsafe Window* getWindowPtr()
	{
		return windowHandle;
	}

	private unsafe void windowHandleChanged (Window* windowHandle) {
		this.windowHandle = windowHandle;
		input.WindowHandleChanged(windowHandle);
	}

	internal unsafe bool update () {
		if (!listenerInitialized) {
			initializeListener();
		}
		lock (runnables) {
			executedRunnables.addAll(runnables);
			runnables.clear();
		}
		foreach (Runnable runnable in executedRunnables) {
			runnable.Invoke();
		}
		bool shouldRender = executedRunnables.size > 0 || graphics.isContinuousRendering();
		executedRunnables.clear();

		if (!iconified) input.Update();

		lock (this) {
			shouldRender |= _requestRendering && !iconified;
			_requestRendering = false;
		}

		if (shouldRender) {
			graphics.update();
			listener.Render();
			GLFW.SwapBuffers(windowHandle);
		}

		if (!iconified) input.PrepareNext();

		return shouldRender;
	}

	internal void requestRendering () {
		lock (this) {
			this._requestRendering = true;
		}
	}

	internal unsafe bool shouldClose () {
		return GLFW.WindowShouldClose(windowHandle);
	}

	internal DesktopApplicationConfiguration getConfig () {
		return config;
	}

	internal bool isListenerInitialized () {
		return listenerInitialized;
	}

	void initializeListener () {
		if (!listenerInitialized) {
			listener.Create();
			listener.Resize(graphics.getWidth(), graphics.getHeight());
			listenerInitialized = true;
		}
	}

	internal unsafe void makeCurrent () {
		Gdx.graphics = graphics;
		Gdx.gl32 = graphics.getGL32();
		Gdx.gl31 = Gdx.gl32 != null ? Gdx.gl32 : graphics.getGL31();
		Gdx.gl30 = Gdx.gl31 != null ? Gdx.gl31 : graphics.getGL30();
		Gdx.gl20 = Gdx.gl30 != null ? Gdx.gl30 : graphics.getGL20();
		Gdx.gl = Gdx.gl20;
		Gdx.input = input;

		GLFW.MakeContextCurrent(windowHandle);
	}

	public unsafe void dispose () {
		listener.Pause();
		listener.Dispose();
		DesktopCursor.dispose(this);
		graphics.dispose();
		input.dispose();

		GLFW.SetWindowFocusCallback(windowHandle, null);
		GLFW.SetWindowIconifyCallback(windowHandle, null);
		GLFW.SetWindowMaximizeCallback(windowHandle, null);
		GLFW.SetWindowCloseCallback(windowHandle, null);
		GLFW.SetDropCallback(windowHandle, null);
		GLFW.SetWindowRefreshCallback(windowHandle, null);
			
		GLFW.DestroyWindow(windowHandle);
		}

	public override unsafe int GetHashCode () {
		 int prime = 31;
		int result = 1;
		// TODO: Not sure that this cast works
		result = prime * result + (int)((long)windowHandle ^ ((long)windowHandle >>> 32));
		return result;
	}

	public override unsafe bool Equals (Object? obj) {
		if (this == obj) return true;
		if (obj == null) return false;
		if (GetType() != obj.GetType()) return false;
		DesktopWindow other = (DesktopWindow)obj;
		if (windowHandle != other.windowHandle) return false;
		return true;
	}

	public unsafe void flash () {
		GLFW.RequestWindowAttention(windowHandle);
	}
}
}
