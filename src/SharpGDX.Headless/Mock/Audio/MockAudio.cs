using SharpGDX.Audio;
using SharpGDX.Files;

namespace SharpGDX.Headless.Mock.Audio;

/// <summary>
///     The headless backend does its best to mock elements.
/// </summary>
/// <remarks>
///     This is intended to make code-sharing between server and client as simple as possible.
/// </remarks>
public class MockAudio : SharpGDX.IAudio
{
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
}