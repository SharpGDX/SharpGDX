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
    public void Dispose()
    {
    }

    public float GetPosition()
    {
        return 0;
    }

    public float GetVolume()
    {
        return 0;
    }

    public bool IsLooping()
    {
        return false;
    }

    public bool IsPlaying()
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

    public void SetOnCompletionListener(Action<IMusic> listener)
    {
        throw new NotImplementedException();
    }

    public void SetPan(float pan, float volume)
    {
    }

    public void SetPosition(float position)
    {
    }

    public void SetVolume(float volume)
    {
    }

    public void Stop()
    {
    }
}