using SharpGDX.Utils;

namespace SharpGDX.Desktop.Audio;

public interface IDesktopAudio : IAudio, Disposable
{
	void Update();
}