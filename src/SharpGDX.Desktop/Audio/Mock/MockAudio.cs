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
	public void dispose()
	{
	}

	public string[] getAvailableOutputDevices()
	{
		return [];
	}

	public IAudioDevice newAudioDevice(int samplingRate, bool isMono)
	{
		return new MockAudioDevice();
	}

	public IAudioRecorder newAudioRecorder(int samplingRate, bool isMono)
	{
		return new MockAudioRecorder();
	}

	public IMusic newMusic(FileHandle file)
	{
		return new MockMusic();
	}

	public ISound newSound(FileHandle fileHandle)
	{
		return new MockSound();
	}

	public bool switchOutputDevice(string? deviceIdentifier)
	{
		return true;
	}

	public void Update()
	{
	}
}