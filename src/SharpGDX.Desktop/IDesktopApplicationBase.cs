using SharpGDX.Desktop.Audio;

namespace SharpGDX.Desktop;

public interface IDesktopApplicationBase : IApplication
{
	public IDesktopAudio CreateAudio(DesktopApplicationConfiguration config);

	public IDesktopInput CreateInput(DesktopWindow window);
}