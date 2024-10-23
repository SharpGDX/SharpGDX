using SharpGDX.Audio;

namespace SharpGDX.Headless.Mock.Audio;

/// <summary>
///     The headless backend does its best to mock elements.
/// </summary>
/// <remarks>
///     This is intended to make code-sharing between server and client as simple as possible.
/// </remarks>
public class MockMusic : IMusic
{
	/// <inheritdoc cref="IMusic.Position" />
	public float Position { get; set; }

	/// <inheritdoc cref="IMusic.Volume" />
	public float Volume { get; set; }

	/// <inheritdoc cref="IMusic.IsPlaying" />
	public bool IsPlaying => false;

	public void Dispose()
	{
	}
	
	public bool IsLooping()
	{
		return false;
	}
	
	public void Pause()
	{
	}

	public void Play()
	{
	}

	public void SetLooping(bool isLooping)
	{
	}

	public void setOnCompletionListener(Action<IMusic> listener)
	{
	}

	public void SetPan(float pan, float volume)
	{
	}
	
	public void Stop()
	{
	}
}