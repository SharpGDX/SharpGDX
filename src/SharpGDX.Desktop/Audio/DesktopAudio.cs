using SharpGDX.Utils;

namespace SharpGDX.Desktop.Audio;

public interface IDesktopAudio : IAudio, IDisposable
{
	void Update();
}