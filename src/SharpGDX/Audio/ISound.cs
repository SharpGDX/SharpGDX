using SharpGDX.Utils;

namespace SharpGDX.Audio;

/// <summary>
///     A Sound is a short audio clip that can be played numerous times in parallel.
/// </summary>
/// <remarks>
///     <para>It's completely loaded into memory so only load small audio files.</para>
///     <para>Call the <see cref="Dispose()" /> method when you're done using the Sound.</para>
///     <para>Sound instances are created via a call to <see cref="IAudio.NewSound(Files.FileHandle)" />.</para>
///     <para>
///         Calling the <see cref="Play()" /> or <see cref="Play(float)" /> method will return a long which is an id to
///         that instance of the sound. You can use this id to modify the playback of that sound instance.
///     </para>
///     <para><b>Note</b>: any values provided will not be clamped, it is the developer's responsibility to do so</para>
/// </remarks>
public interface ISound : Disposable
{
	/// <summary>
	///     Releases all the resources.
	/// </summary>
	public void Dispose();

	/// <summary>
	///     Plays the sound, looping.
	/// </summary>
	/// <remarks>
	///     If the sound is already playing, it will be played again, concurrently.
	/// </remarks>
	/// <returns>The id of the sound instance if successful, or -1 on failure.</returns>
	public long Loop();

	/// <summary>
	///     Plays the sound, looping.
	/// </summary>
	/// <remarks>
	///     If the sound is already playing, it will be played again, concurrently.
	/// </remarks>
	/// <param name="volume">The volume in the range [0, 1].</param>
	/// <returns>The id of the sound instance if successful, or -1 on failure.</returns>
	public long Loop(float volume);

	/// <summary>
	///     Plays the sound, looping.
	/// </summary>
	/// <remarks>
	/// <para>
	///     If the sound is already playing, it will be played again, concurrently.
	/// </para>
	/// <para>
	///Note that (with the exception of the web backend) panning only works for mono sounds, not for stereo sounds!
    /// </para>
    /// </remarks>
    /// <param name="volume">The volume in the range [0, 1].</param>
    /// <param name="pitch">
    ///     <para>
    ///         The pitch multiplier.
    ///     </para>
    ///     <para>
    ///         1 == default, &gt;1 == faster, &lt;1 == slower, the value has to be between 0.5 and 2.0.
    ///     </para>
    /// </param>
    /// <param name="pan">Panning in the range -1 (full left) to 1 (full right). 0 is center position.</param>
    /// <returns>The id of the sound instance if successful, or -1 on failure.</returns>
    public long Loop(float volume, float pitch, float pan);

	/// <summary>
	///     Pauses all instances of this sound.
	/// </summary>
	public void Pause();

	/// <summary>
	///     Pauses the sound instance with the given id as returned by .
	/// </summary>
	/// <remarks>
	///     If the sound is no longer playing, this has no effect.
	/// </remarks>
	/// <param name="soundId">The sound id.</param>
	public void Pause(long soundId);

	/// <summary>
	///     Plays the sound.
	/// </summary>
	/// <remarks>
	///     If the sound is already playing, it will be played again, concurrently.
	/// </remarks>
	/// <returns>The id of the sound instance if successful, or -1 on failure.</returns>
	public long Play();

	/// <summary>
	///     Plays the sound.
	/// </summary>
	/// <remarks>
	///     If the sound is already playing, it will be played again, concurrently.
	/// </remarks>
	/// <param name="volume">The volume in the range [0,1].</param>
	/// <returns>The id of the sound instance if successful, or -1 on failure.</returns>
	public long Play(float volume);

	/// <summary>
	///     Plays the sound.
	/// </summary>
	/// <remarks>
	/// <para>
	///     If the sound is already playing, it will be played again, concurrently.
	/// </para>
	/// <para>
	/// Note that (with the exception of the web backend) panning only works for mono sounds, not for stereo sounds!
    /// </para>
    /// </remarks>
    /// <param name="volume">The volume in the range [0,1].</param>
    /// <param name="pitch">
    ///     The pitch multiplier, 1 == default, &gt;1 == faster, &lt;1 == slower, the value has to be between 0.5 and 2.0.
    /// </param>
    /// <param name="pan">Panning in the range -1 (full left) to 1 (full right). 0 is center position.</param>
    /// <returns>The id of the sound instance if successful, or -1 on failure.</returns>
    public long Play(float volume, float pitch, float pan);

	/// <summary>
	///     Resumes all paused instances of this sound.
	/// </summary>
	public void Resume();

	/// <summary>
	///     Resumes the sound instance with the given id as returned by <see cref="Play()" /> or <see cref="Play(float)" />.
	/// </summary>
	/// <remarks>
	///     If the sound is no longer playing, this has no effect.
	/// </remarks>
	/// <param name="soundId">The sound id.</param>
	public void Resume(long soundId);

	/// <summary>
	///     Sets the sound instance with the given id to be looping.
	/// </summary>
	/// <remarks>
	///     If the sound is no longer playing, this has no effect.
	/// </remarks>
	/// <param name="soundId">The sound id.</param>
	/// <param name="looping">Whether to loop or not.</param>
	public void SetLooping(long soundId, bool looping);

	/// <summary>
	///     Sets the panning and volume of the sound instance with the given id as returned by <see cref="Play()" /> or
	///     <see cref="Play(float)" />.
	/// </summary>
	/// <remarks>
	///     If the sound is no longer playing, this has no effect.
	/// </remarks>
	/// <param name="soundId">The sound id.</param>
	/// <param name="pan">Panning in the range -1 (full left) to 1 (full right). 0 is center position.</param>
	/// <param name="volume">The volume in the range 0 (silent) to 1 (max volume).</param>
	public void SetPan(long soundId, float pan, float volume);

	/// <summary>
	///     Changes the pitch multiplier of the sound instance with the given id as returned by <see cref="Play()" /> or
	///     <see cref="Play(float)" />.
	/// </summary>
	/// <remarks>
	///     If the sound is no longer playing, this has no effect.
	/// </remarks>
	/// <param name="soundId">The sound id.</param>
	/// <param name="pitch">
	///     The pitch multiplier, 1 == default, &gt;1 == faster, &lt;1 == slower, the value has to be between 0.5 and 2.0.
	/// </param>
	public void SetPitch(long soundId, float pitch);

	/// <summary>
	///     Changes the volume of the sound instance with the given id as returned by <see cref="Play()" /> or
	///     <see cref="Play(float)" />.
	/// </summary>
	/// <remarks>
	///     If the sound is no longer playing, this has no effect.
	/// </remarks>
	/// <param name="soundId">The sound id.</param>
	/// <param name="volume">The volume in the range 0 (silent) to 1 (max volume).</param>
	public void SetVolume(long soundId, float volume);

	/// <summary>
	///     Stops playing all instances of this sound.
	/// </summary>
	public void Stop();

	/// <summary>
	///     Stops the sound instance with the given id as returned by <see cref="Play()" /> or <see cref="Play(float)" />.
	/// </summary>
	/// <remarks>
	///     If the sound is no longer playing, this has no effect.
	/// </remarks>
	/// <param name="soundId">The sound id.</param>
	public void Stop(long soundId);
}