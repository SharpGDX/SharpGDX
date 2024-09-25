using SharpGDX.Audio;

namespace SharpGDX.Desktop.Audio.Mock;

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

	public void dispose()
	{
	}
	
	public bool isLooping()
	{
		return false;
	}

	public bool isPlaying()
	{
		return false;
	}

	public void pause()
	{
	}

	public void play()
	{
	}

	public void setLooping(bool isLooping)
	{
	}

	public void setOnCompletionListener(IMusic.IOnCompletionListener listener)
	{
	}

	public void setPan(float pan, float volume)
	{
	}
	
	public void stop()
	{
	}
}