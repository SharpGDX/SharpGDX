using SharpGDX.Utils;
using SharpGDX.Graphics.GLUtils;
using static SharpGDX.Graphics.ICursor;
using SharpGDX.Graphics;
using static SharpGDX.IGraphics;

namespace SharpGDX.Headless.Mock.Graphics
{
	/** The headless backend does its best to mock elements. This is intended to make code-sharing between server and client as simple
 * as possible. */
	public class MockGraphics : AbstractGraphics
	{
	long frameId = -1;
	float deltaTime = 0;
	long frameStart = 0;
	int frames = 0;
	int fps;
	long lastTime = TimeUtils.nanoTime();
	long targetRenderInterval;
		GLVersion glVersion = new GLVersion(IApplication.ApplicationType.HeadlessDesktop, "", "", "");

		public override bool IsGL30Available()
	{
		return false;
	}

	public override bool IsGL31Available()
	{
		return false;
	}

	public override bool IsGL32Available()
	{
		return false;
	}

	public override IGL20 GetGL20()
	{
		return null;
	}

	public override void SetGL20(IGL20 gl20)
	{

	}

	public override IGL30 GetGL30()
	{
		return null;
	}

	public override void SetGL30(IGL30 gl30)
	{

	}

	public override IGL31 GetGL31()
	{
		return null;
	}

	public override void SetGL31(IGL31 gl31)
	{

	}

	public override IGL32 GetGL32()
	{
		return null;
	}

	public override void SetGL32(IGL32 gl32)
	{

	}

	public override int GetWidth()
	{
		return 0;
	}

	public override int GetHeight()
	{
		return 0;
	}

	public override int GetBackBufferWidth()
	{
		return 0;
	}

	public override int GetBackBufferHeight()
	{
		return 0;
	}

	public override long GetFrameId()
	{
		return frameId;
	}

	public override float GetDeltaTime()
	{
		return deltaTime;
	}

	public override int GetFramesPerSecond()
	{
		return fps;
	}

	public override GraphicsType GetType()
	{
		return GraphicsType.Mock;
	}

	public override GLVersion GetGLVersion()
	{
		return glVersion;
	}

	public override float GetPpiX()
	{
		return 0;
	}

	public override float GetPpiY()
	{
		return 0;
	}

	public override float GetPpcX()
	{
		return 0;
	}

	public override float GetPpcY()
	{
		return 0;
	}

	public override bool SupportsDisplayModeChange()
	{
		return false;
	}

	public override DisplayMode[] GetDisplayModes()
	{
		return new DisplayMode[0];
	}

	public override DisplayMode GetDisplayMode()
	{
		return null;
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

	public override bool SetFullscreenMode(DisplayMode displayMode)
	{
		return false;
	}

	public override bool SetWindowedMode(int width, int height)
	{
		return false;
	}

	public override void SetTitle(String title)
	{

	}

	public override void SetVSync(bool vsync)
	{

	}

	/** Sets the target framerate for the application. Use 0 to never sleep; negative to not call the render method at all. Default
	 * is 60.
	 *
	 * @param fps fps */
	public override void SetForegroundFPS(int fps)
	{
		this.targetRenderInterval = (long)(fps <= 0 ? (fps == 0 ? 0 : -1) : ((1F / fps) * 1000000000F));
	}

	public long getTargetRenderInterval()
	{
		return targetRenderInterval;
	}

	public override BufferFormat GetBufferFormat()
	{
		return null;
	}

	public override bool SupportsExtension(String extension)
	{
		return false;
	}

	public override void SetContinuousRendering(bool isContinuous)
	{

	}

	public override bool IsContinuousRendering()
	{
		return false;
	}

	public override void RequestRendering()
	{

	}

	public override bool IsFullscreen()
	{
		return false;
	}

	public void updateTime()
	{
		long time = TimeUtils.nanoTime();
		deltaTime = (time - lastTime) / 1000000000.0f;
		lastTime = time;

		if (time - frameStart >= 1000000000)
		{
			fps = frames;
			frames = 0;
			frameStart = time;
		}
		frames++;
	}

	public void incrementFrameId()
	{
		frameId++;
	}

	public override ICursor NewCursor(Pixmap pixmap, int xHotspot, int yHotspot)
	{
		return null;
	}

	public override void SetCursor(ICursor cursor)
	{
	}

	public override void SetSystemCursor(SystemCursor systemCursor)
	{
	}

	public override SharpGDX.IGraphics.Monitor GetPrimaryMonitor()
	{
		return null;
	}

	public override SharpGDX.IGraphics.Monitor GetMonitor()
	{
		return null;
	}

	public override SharpGDX.IGraphics.Monitor[] GetMonitors()
	{
		return null;
	}

	public override DisplayMode[] GetDisplayModes(SharpGDX.IGraphics.Monitor monitor)
	{
		return null;
	}

	public override DisplayMode GetDisplayMode(SharpGDX.IGraphics.Monitor monitor)
	{
		return null;
	}

	public override void SetUndecorated(bool undecorated)
	{

	}

	public override void SetResizable(bool resizable)
	{

	}
}
}
