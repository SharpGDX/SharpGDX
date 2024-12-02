namespace SharpGDX.Audio;

/// <summary>
///     A Music instance represents a streamed audio file.
/// </summary>
/// <remarks>
///     <para>
///         The interface supports pausing, resuming and so on.
///     </para>
///     <para>
///         Music instances are created via <see cref="IAudio.NewMusic(Files.FileHandle)" />.
///     </para>
///     <para>
///         Music instances are automatically paused and resumed when an <see cref="IApplication" /> is paused or resumed.
///         See <see cref="IApplicationListener" />.
///     </para>
///     <para>
///         When you are done with using the Music instance you have to dispose it via the
///         <see cref="IDisposable.Dispose()" /> method.
///     </para>
///     <para>
///         <b>Note</b>: any values provided will not be clamped, it is the developer's responsibility to do so.
///     </para>
/// </remarks>
public interface IMusic : IDisposable
{
    /// <summary>
    ///     The volume of this music stream.
    /// </summary>
    /// <returns>The volume of this music stream.</returns>
    public float GetVolume();

    /**
     * @return whether the music stream is playing.
     */
    public bool IsLooping();

    /// <summary>
    ///     Whether this music stream is playing.
    /// </summary>
    /// <returns>Whether this music stream is playing.</returns>
    public bool IsPlaying();

    /// <summary>
    ///     Pauses the playback.
    /// </summary>
    /// <remarks>
    ///     If the music stream has not been started yet or has finished playing a call to this method will be ignored.
    /// </remarks>
    public void Pause();

    /// <summary>
    ///     Starts the playback of the music stream.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         In case the stream was paused this will resume the playback.
    ///     </para>
    ///     <para>
    ///         In case the music stream is finished playing this will restart the playback.
    ///     </para>
    /// </remarks>
    public void Play();

    /// <summary>
    ///     Sets whether the music stream is looping.
    /// </summary>
    /// <remarks>
    ///     This can be called at any time, whether the stream is playing.
    /// </remarks>
    /// <param name="isLooping">Whether to loop the stream.</param>
    public void SetLooping(bool isLooping);

    /// <summary>
    ///     Register a callback to be invoked when the end of a music stream has been reached during playback.
    /// </summary>
    /// <param name="listener">The callback that will be run.</param>
    public void SetOnCompletionListener(Action<IMusic> listener);

    /**
     * Sets the panning and volume of this music stream.
     * @param pan panning in the range -1 (full left) to 1 (full right). 0 is center position.
     * @param volume the volume in the range [0,1].
     */
    public void SetPan(float pan, float volume);

    /// <summary>
    ///     Sets the volume of this music stream.
    /// </summary>
    /// <remarks>
    ///     The volume must be given in the range [0,1] with 0 being silent and 1 being the maximum volume.
    /// </remarks>
    /// <param name="volume">The volume.</param>
    public void SetVolume(float volume);

    /// <summary>
    ///     Stops a playing or paused IMusic instance.
    /// </summary>
    /// <remarks>
    ///     Next time <see cref="Play()" /> is invoked the IMusic will start from the beginning.
    /// </remarks>
    public void Stop();
}