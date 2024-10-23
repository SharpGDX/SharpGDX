using SharpGDX.Audio;

namespace SharpGDX.Headless.Mock.Audio;

/// <summary>
///     The headless backend does its best to mock elements.
/// </summary>
/// <remarks>
///     This is intended to make code-sharing between server and client as simple as possible.
/// </remarks>
public class MockSound : ISound
{
	public void Dispose()
	{
	}

	public long Loop()
	{
		return 0;
	}

	public long Loop(float volume)
	{
		return 0;
	}

	public long Loop(float volume, float pitch, float pan)
	{
		return 0;
	}

	public void Pause()
	{
	}

	public void Pause(long soundId)
	{
	}

	public long Play()
	{
		return 0;
	}

	public long Play(float volume)
	{
		return 0;
	}

	public long Play(float volume, float pitch, float pan)
	{
		return 0;
	}

	public void Resume()
	{
	}

	public void Resume(long soundId)
	{
	}

	public void SetLooping(long soundId, bool looping)
	{
	}

	public void SetPan(long soundId, float pan, float volume)
	{
	}

	public void SetPitch(long soundId, float pitch)
	{
	}

	public void SetVolume(long soundId, float volume)
	{
	}

	public void Stop()
	{
	}

	public void Stop(long soundId)
	{
	}
}