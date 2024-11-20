using SharpGDX.Graphics;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;

namespace SharpGDX
{
	public abstract class AbstractGraphics : IGraphics
	{
		public abstract float GetPpcY();

		public float GetDensity()
		{
			float ppiX = GetPpiX();
			return ppiX is > 0 and <= float.MaxValue ? ppiX / 160f : 1f;
		}

		public abstract bool SupportsDisplayModeChange();
		public abstract IGraphics.Monitor GetPrimaryMonitor();
		public abstract IGraphics.Monitor GetMonitor();
		public abstract IGraphics.Monitor[] GetMonitors();
		public abstract IGraphics.DisplayMode[] GetDisplayModes();
		public abstract IGraphics.DisplayMode[] GetDisplayModes(IGraphics.Monitor monitor);
		public abstract IGraphics.DisplayMode GetDisplayMode();
		public abstract IGraphics.DisplayMode GetDisplayMode(IGraphics.Monitor monitor);
		public abstract bool SetFullscreenMode(IGraphics.DisplayMode displayMode);
		public abstract bool SetWindowedMode(int width, int height);
		public abstract void SetTitle(string title);
		public abstract void SetUndecorated(bool undecorated);
		public abstract void SetResizable(bool resizable);
		public abstract void SetVSync(bool vsync);
		public abstract void SetForegroundFPS(int fps);
		public abstract IGraphics.BufferFormat GetBufferFormat();
		public abstract bool SupportsExtension(string extension);
		public abstract void SetContinuousRendering(bool isContinuous);
		public abstract bool IsContinuousRendering();
		public abstract void RequestRendering();
		public abstract bool IsFullscreen();
		public abstract ICursor NewCursor(Pixmap pixmap, int xHotspot, int yHotspot);
		public abstract void SetCursor(ICursor cursor);
		public abstract void SetSystemCursor(ICursor.SystemCursor systemCursor);

		public abstract bool IsGL30Available();
		public abstract bool IsGL31Available();
		public abstract bool IsGL32Available();
		public abstract IGL20 GetGL20();
		public abstract IGL30 GetGL30();
		public abstract IGL31 GetGL31();
		public abstract IGL32 GetGL32();
		public abstract void SetGL20(IGL20 gl20);
		public abstract void SetGL30(IGL30 gl30);
		public abstract void SetGL31(IGL31 gl31);
		public abstract void SetGL32(IGL32 gl32);
		public abstract int GetWidth();
		public abstract int GetHeight();
		public abstract int GetBackBufferWidth();
		public abstract int GetBackBufferHeight();

		public float GetBackBufferScale()
		{
			return GetBackBufferWidth() / (float)GetWidth();
		}

		public abstract int GetSafeInsetLeft();
		public abstract int GetSafeInsetTop();
		public abstract int GetSafeInsetBottom();
		public abstract int GetSafeInsetRight();
		public abstract long GetFrameId();
		public abstract float GetDeltaTime();
		public abstract int GetFramesPerSecond();
		public abstract IGraphics.GraphicsType GetType();
		public abstract GLVersion GetGLVersion();
		public abstract float GetPpiX();
		public abstract float GetPpiY();
		public abstract float GetPpcX();
	}
}