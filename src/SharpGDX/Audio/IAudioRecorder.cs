namespace SharpGDX.Audio;

/// <summary>
///     An AudioRecorder allows to record input from an audio device.
/// </summary>
/// <remarks>
///     <para>
///         It has a sampling rate and is either stereo or mono.
///     </para>
///     <para>
///         Samples are returned in signed 16-bit PCM format. Stereo samples are interleaved in the order left channel,
///         right channel.
///     </para>
///     <para>
///         The AudioRecorder has to be disposed if no longer needed via the <see cref="IDisposable.Dispose()" />.
///     </para>
/// </remarks>
public interface IAudioRecorder : IDisposable
{
    /// <summary>
    ///     Reads in numSamples samples into the array samples starting at offset. If the recorder is in stereo you have to
    ///     multiply numSamples by 2.
    /// </summary>
    /// <param name="samples">The array to write the samples to.</param>
    /// <param name="offset">The offset into the array.</param>
    /// <param name="numSamples">The number of samples to be read.</param>
    public void Read(short[] samples, int offset, int numSamples);
}