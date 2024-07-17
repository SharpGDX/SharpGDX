using OpenTK.Windowing.GraphicsLibraryFramework;
using SharpGDX.Utils;

namespace SharpGDX.Desktop;

public interface IDesktopInput : IInput, Disposable
{
	public void PrepareNext();

	public void ResetPollingStates();

	public void Update();

	// TODO: Really don't want to expose this, marked internal for now. -RP
	internal unsafe void WindowHandleChanged(Window* windowHandle);
}