using SharpGDX.Audio;

namespace SharpGDX.Headless.Mock.Audio;

/// <summary>
///     The headless backend does its best to mock elements.
/// </summary>
/// <remarks>
///     This is intended to make code-sharing between server and client as simple as possible.
/// </remarks>
public class MockAudioDevice : IAudioDevice
{
	public void dispose()
	{
	}

	public int GetLatency()
	{
		return 0;
	}

	public bool IsMono()
	{
		return false;
	}

	public void Pause()
	{
	}

	public void Resume()
	{
	}

	public void SetVolume(float volume)
	{
	}

	public void WriteSamples(short[] samples, int offset, int numSamples)
	{
	}

	public void WriteSamples(float[] samples, int offset, int numSamples)
	{
	}
}