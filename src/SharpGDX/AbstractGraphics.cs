using SharpGDX.Graphics;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;

namespace SharpGDX
{
	public abstract class AbstractGraphics : IGraphics
	{
		public abstract float getPpcY();

		public float getDensity()
		{
			float ppiX = getPpiX();
			return ppiX is > 0 and <= float.MaxValue ? ppiX / 160f : 1f;
		}

		public abstract bool supportsDisplayModeChange();
		public abstract IGraphics.Monitor getPrimaryMonitor();
		public abstract IGraphics.Monitor getMonitor();
		public abstract IGraphics.Monitor[] getMonitors();
		public abstract IGraphics.DisplayMode[] getDisplayModes();
		public abstract IGraphics.DisplayMode[] getDisplayModes(IGraphics.Monitor monitor);
		public abstract IGraphics.DisplayMode getDisplayMode();
		public abstract IGraphics.DisplayMode getDisplayMode(IGraphics.Monitor monitor);
		public abstract bool setFullscreenMode(IGraphics.DisplayMode displayMode);
		public abstract bool setWindowedMode(int width, int height);
		public abstract void setTitle(string title);
		public abstract void setUndecorated(bool undecorated);
		public abstract void setResizable(bool resizable);
		public abstract void setVSync(bool vsync);
		public abstract void setForegroundFPS(int fps);
		public abstract IGraphics.BufferFormat getBufferFormat();
		public abstract bool supportsExtension(string extension);
		public abstract void setContinuousRendering(bool isContinuous);
		public abstract bool isContinuousRendering();
		public abstract void requestRendering();
		public abstract bool isFullscreen();
		public abstract ICursor newCursor(Pixmap pixmap, int xHotspot, int yHotspot);
		public abstract void setCursor(ICursor cursor);
		public abstract void setSystemCursor(ICursor.SystemCursor systemCursor);

		public abstract bool isGL30Available();
		public abstract bool isGL31Available();
		public abstract bool isGL32Available();
		public abstract GL20 getGL20();
		public abstract GL30 getGL30();
		public abstract GL31 getGL31();
		public abstract GL32 getGL32();
		public abstract void setGL20(GL20 gl20);
		public abstract void setGL30(GL30 gl30);
		public abstract void setGL31(GL31 gl31);
		public abstract void setGL32(GL32 gl32);
		public abstract int getWidth();
		public abstract int getHeight();
		public abstract int getBackBufferWidth();
		public abstract int getBackBufferHeight();

		public float getBackBufferScale()
		{
			return getBackBufferWidth() / (float)getWidth();
		}

		public abstract int getSafeInsetLeft();
		public abstract int getSafeInsetTop();
		public abstract int getSafeInsetBottom();
		public abstract int getSafeInsetRight();
		public abstract long getFrameId();
		public abstract float getDeltaTime();
		public abstract int getFramesPerSecond();
		public abstract IGraphics.GraphicsType getType();
		public abstract GLVersion getGLVersion();
		public abstract float getPpiX();
		public abstract float getPpiY();
		public abstract float getPpcX();
	}
}