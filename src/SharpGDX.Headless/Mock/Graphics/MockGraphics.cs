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

		public override bool isGL30Available()
	{
		return false;
	}

	public override bool isGL31Available()
	{
		return false;
	}

	public override bool isGL32Available()
	{
		return false;
	}

	public override GL20 getGL20()
	{
		return null;
	}

	public override void setGL20(GL20 gl20)
	{

	}

	public override GL30 getGL30()
	{
		return null;
	}

	public override void setGL30(GL30 gl30)
	{

	}

	public override GL31 getGL31()
	{
		return null;
	}

	public override void setGL31(GL31 gl31)
	{

	}

	public override GL32 getGL32()
	{
		return null;
	}

	public override void setGL32(GL32 gl32)
	{

	}

	public override int getWidth()
	{
		return 0;
	}

	public override int getHeight()
	{
		return 0;
	}

	public override int getBackBufferWidth()
	{
		return 0;
	}

	public override int getBackBufferHeight()
	{
		return 0;
	}

	public override long getFrameId()
	{
		return frameId;
	}

	public override float getDeltaTime()
	{
		return deltaTime;
	}

	public override int getFramesPerSecond()
	{
		return fps;
	}

	public override GraphicsType getType()
	{
		return GraphicsType.Mock;
	}

	public override GLVersion getGLVersion()
	{
		return glVersion;
	}

	public override float getPpiX()
	{
		return 0;
	}

	public override float getPpiY()
	{
		return 0;
	}

	public override float getPpcX()
	{
		return 0;
	}

	public override float getPpcY()
	{
		return 0;
	}

	public override bool supportsDisplayModeChange()
	{
		return false;
	}

	public override DisplayMode[] getDisplayModes()
	{
		return new DisplayMode[0];
	}

	public override DisplayMode getDisplayMode()
	{
		return null;
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

	public override bool setFullscreenMode(DisplayMode displayMode)
	{
		return false;
	}

	public override bool setWindowedMode(int width, int height)
	{
		return false;
	}

	public override void setTitle(String title)
	{

	}

	public override void setVSync(bool vsync)
	{

	}

	/** Sets the target framerate for the application. Use 0 to never sleep; negative to not call the render method at all. Default
	 * is 60.
	 *
	 * @param fps fps */
	public override void setForegroundFPS(int fps)
	{
		this.targetRenderInterval = (long)(fps <= 0 ? (fps == 0 ? 0 : -1) : ((1F / fps) * 1000000000F));
	}

	public long getTargetRenderInterval()
	{
		return targetRenderInterval;
	}

	public override BufferFormat getBufferFormat()
	{
		return null;
	}

	public override bool supportsExtension(String extension)
	{
		return false;
	}

	public override void setContinuousRendering(bool isContinuous)
	{

	}

	public override bool isContinuousRendering()
	{
		return false;
	}

	public override void requestRendering()
	{

	}

	public override bool isFullscreen()
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

	public override ICursor newCursor(Pixmap pixmap, int xHotspot, int yHotspot)
	{
		return null;
	}

	public override void setCursor(ICursor cursor)
	{
	}

	public override void setSystemCursor(SystemCursor systemCursor)
	{
	}

	public override SharpGDX.IGraphics.Monitor getPrimaryMonitor()
	{
		return null;
	}

	public override SharpGDX.IGraphics.Monitor getMonitor()
	{
		return null;
	}

	public override SharpGDX.IGraphics.Monitor[] getMonitors()
	{
		return null;
	}

	public override DisplayMode[] getDisplayModes(SharpGDX.IGraphics.Monitor monitor)
	{
		return null;
	}

	public override DisplayMode getDisplayMode(SharpGDX.IGraphics.Monitor monitor)
	{
		return null;
	}

	public override void setUndecorated(bool undecorated)
	{

	}

	public override void setResizable(bool resizable)
	{

	}
}
}
