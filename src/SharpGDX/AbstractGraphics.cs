using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;

namespace SharpGDX;

public abstract class AbstractGraphics : IGraphics
{
    /// <inheritdoc cref="IGraphics.GetPpcY()" />
    public abstract float GetPpcY();

    /// <inheritdoc cref="IGraphics.GetDensity()" />
    public float GetDensity()
    {
        var ppiX = GetPpiX();

        return ppiX is > 0 and <= float.MaxValue ? ppiX / 160f : 1f;
    }

    /// <inheritdoc cref="IGraphics.SupportsDisplayModeChange()" />
    public abstract bool SupportsDisplayModeChange();

    /// <inheritdoc cref="IGraphics.GetPrimaryMonitor()" />
    public abstract IGraphics.Monitor GetPrimaryMonitor();

    /// <inheritdoc cref="IGraphics.GetMonitor()" />
    public abstract IGraphics.Monitor GetMonitor();

    /// <inheritdoc cref="IGraphics.GetMonitors()" />
    public abstract IGraphics.Monitor[] GetMonitors();

    /// <inheritdoc cref="IGraphics.GetDisplayModes()" />
    public abstract IGraphics.DisplayMode[] GetDisplayModes();

    /// <inheritdoc cref="IGraphics.GetDisplayModes()" />
    public abstract IGraphics.DisplayMode[] GetDisplayModes(IGraphics.Monitor monitor);

    /// <inheritdoc cref="IGraphics.GetDisplayMode()" />
    public abstract IGraphics.DisplayMode GetDisplayMode();

    /// <inheritdoc cref="IGraphics.GetDisplayMode()" />
    public abstract IGraphics.DisplayMode GetDisplayMode(IGraphics.Monitor monitor);

    /// <inheritdoc cref="IGraphics.SetFullscreenMode(IGraphics.DisplayMode)" />
    public abstract bool SetFullscreenMode(IGraphics.DisplayMode displayMode);

    /// <inheritdoc cref="IGraphics.SetWindowedMode(int, int)" />
    public abstract bool SetWindowedMode(int width, int height);

    /// <inheritdoc cref="IGraphics.SetTitle(string)" />
    public abstract void SetTitle(string title);

    /// <inheritdoc cref="IGraphics.SetUndecorated(bool)" />
    public abstract void SetUndecorated(bool undecorated);

    /// <inheritdoc cref="IGraphics.SetResizable(bool)" />
    public abstract void SetResizable(bool resizable);

    /// <inheritdoc cref="IGraphics.SetVSync(bool)" />
    public abstract void SetVSync(bool vsync);

    /// <inheritdoc cref="IGraphics.SetForegroundFPS(int)" />
    public abstract void SetForegroundFPS(int fps);

    /// <inheritdoc cref="IGraphics.GetBufferFormat()" />
    public abstract IGraphics.BufferFormat GetBufferFormat();

    /// <inheritdoc cref="IGraphics.SupportsExtension(string)" />
    public abstract bool SupportsExtension(string extension);

    /// <inheritdoc cref="IGraphics.SetContinuousRendering(bool)" />
    public abstract void SetContinuousRendering(bool isContinuous);

    /// <inheritdoc cref="IGraphics.IsContinuousRendering()" />
    public abstract bool IsContinuousRendering();

    /// <inheritdoc cref="IGraphics.RequestRendering()" />
    public abstract void RequestRendering();

    /// <inheritdoc cref="IGraphics.IsFullscreen()" />
    public abstract bool IsFullscreen();

    /// <inheritdoc cref="IGraphics.NewCursor(Pixmap, int, int)" />
    public abstract ICursor NewCursor(Pixmap pixmap, int xHotspot, int yHotspot);

    /// <inheritdoc cref="IGraphics.SetCursor(ICursor)" />
    public abstract void SetCursor(ICursor cursor);

    /// <inheritdoc cref="IGraphics.SetSystemCursor(ICursor.SystemCursor)" />
    public abstract void SetSystemCursor(ICursor.SystemCursor systemCursor);

    /// <inheritdoc cref="IGraphics.IsGL30Available()" />
    public abstract bool IsGL30Available();

    /// <inheritdoc cref="IGraphics.IsGL31Available()" />
    public abstract bool IsGL31Available();

    /// <inheritdoc cref="IGraphics.IsGL32Available()" />
    public abstract bool IsGL32Available();

    /// <inheritdoc cref="IGraphics.GetGL20()" />
    public abstract IGL20 GetGL20();

    /// <inheritdoc cref="IGraphics.GetGL30()" />
    public abstract IGL30 GetGL30();

    /// <inheritdoc cref="IGraphics.GetGL31()" />
    public abstract IGL31 GetGL31();

    /// <inheritdoc cref="IGraphics.GetGL32()" />
    public abstract IGL32 GetGL32();

    /// <inheritdoc cref="IGraphics.SetGL20(IGL20)" />
    public abstract void SetGL20(IGL20 gl20);

    /// <inheritdoc cref="IGraphics.SetGL30(IGL30)" />
    public abstract void SetGL30(IGL30 gl30);

    /// <inheritdoc cref="IGraphics.SetGL31(IGL31)" />
    public abstract void SetGL31(IGL31 gl31);

    /// <inheritdoc cref="IGraphics.SetGL32(IGL32)" />
    public abstract void SetGL32(IGL32 gl32);

    /// <inheritdoc cref="IGraphics.GetWidth()" />
    public abstract int GetWidth();

    /// <inheritdoc cref="IGraphics.GetHeight()" />
    public abstract int GetHeight();

    /// <inheritdoc cref="IGraphics.GetBackBufferWidth()" />
    public abstract int GetBackBufferWidth();

    /// <inheritdoc cref="IGraphics.GetBackBufferHeight()" />
    public abstract int GetBackBufferHeight();

    /// <inheritdoc cref="IGraphics.GetBackBufferScale()" />
    public float GetBackBufferScale()
    {
        return GetBackBufferWidth() / (float)GetWidth();
    }

    /// <inheritdoc cref="IGraphics.GetSafeInsetLeft()" />
    public abstract int GetSafeInsetLeft();

    /// <inheritdoc cref="IGraphics.GetSafeInsetTop()" />
    public abstract int GetSafeInsetTop();

    /// <inheritdoc cref="IGraphics.GetSafeInsetBottom()" />
    public abstract int GetSafeInsetBottom();

    /// <inheritdoc cref="IGraphics.GetSafeInsetRight()" />
    public abstract int GetSafeInsetRight();

    /// <inheritdoc cref="IGraphics.GetFrameId()" />
    public abstract long GetFrameId();

    /// <inheritdoc cref="IGraphics.GetDeltaTime()" />
    public abstract float GetDeltaTime();

    /// <inheritdoc cref="IGraphics.GetFramesPerSecond()" />
    public abstract int GetFramesPerSecond();

    /// <inheritdoc cref="IGraphics.GetType()" />
    public new abstract IGraphics.GraphicsType GetType();

    /// <inheritdoc cref="IGraphics.GetGLVersion()" />
    public abstract GLVersion GetGLVersion();

    /// <inheritdoc cref="IGraphics.GetPpiX()" />
    public abstract float GetPpiX();

    /// <inheritdoc cref="IGraphics.GetPpiY()" />
    public abstract float GetPpiY();

    /// <inheritdoc cref="IGraphics.GetPpcX()" />
    public abstract float GetPpcX();
}