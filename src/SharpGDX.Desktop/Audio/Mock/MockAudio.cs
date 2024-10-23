using SharpGDX.Audio;
using SharpGDX.Files;
namespace SharpGDX.Desktop.Audio.Mock;

/// <summary>
///     The headless backend does its best to mock elements.
/// </summary>
/// <remarks>
///     This is intended to make code-sharing between server and client as simple as possible.
/// </remarks>
public class MockAudio : IDesktopAudio
{
	public void Dispose()
	{
	}

	public string[] GetAvailableOutputDevices()
	{
		return [];
	}

	public IAudioDevice NewAudioDevice(int samplingRate, bool isMono)
	{
		return new MockAudioDevice();
	}

	public IAudioRecorder NewAudioRecorder(int samplingRate, bool isMono)
	{
		return new MockAudioRecorder();
	}

	public IMusic NewMusic(FileHandle file)
	{
		return new MockMusic();
	}

	public ISound NewSound(FileHandle fileHandle)
	{
		return new MockSound();
	}

	public bool SwitchOutputDevice(string? deviceIdentifier)
	{
		return true;
	}

	public void Update()
	{
	}
}