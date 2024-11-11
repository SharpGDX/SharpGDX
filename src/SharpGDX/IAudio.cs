using SharpGDX.Audio;
using SharpGDX.Files;

namespace SharpGDX;

/// <summary>
///     This interface encapsulates the creation and management of audio resources. It allows you to get direct access to
///     the audio hardware via the <see cref="IAudioDevice" /> and <see cref="IAudioRecorder" /> interfaces, create sound
///     effects via the <see cref="ISound" /> interface and play music streams via the <see cref="IMusic" /> interface.
/// </summary>
/// <remarks>
///     <para>
///         All resources created via this interface have to be disposed as soon as they are no longer used.
///     </para>
///     <para>
///         Note that all <see cref="IMusic" /> instances will be automatically paused when the
///         <see cref="IApplicationListener.Pause()" /> method is called, and automatically resumed when the
///         <see cref="IApplicationListener.Resume()" /> method is called.
///     </para>
/// </remarks>
public interface IAudio
{
    /// <summary>
    ///     This function returns a list of fully qualified Output device names. This function is only implemented on desktop
    ///     and web.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Note that on gwt the GwtApplicationConfiguration#fetchAvailableOutputDevices attribute needs to be set to true
    ///         for asking the user for permission! On all other platforms it will return an empty array. It will also return
    ///         an empty array on error.
    ///     </para>
    ///     <para>
    ///         The names returned need os dependent preprocessing before exposing to a user.
    ///     </para>
    /// </remarks>
    /// <returns>An array of available output devices.</returns>
    public string[] GetAvailableOutputDevices();

    /// <summary>
    ///     Creates a new <see cref="IAudioDevice" /> either in mono or stereo mode.
    /// </summary>
    /// <remarks>
    ///     The AudioDevice has to be disposed via its <see cref="IAudioDevice.Dispose()" /> method when it is no longer used.
    /// </remarks>
    /// <param name="samplingRate">The sampling rate.</param>
    /// <param name="isMono">Whether the AudioDevice should be in mono or stereo mode.</param>
    /// <returns>The AudioDevice.</returns>
    /// <exception cref="Utils.GdxRuntimeException">In case the device could not be created.</exception>
    public IAudioDevice NewAudioDevice(int samplingRate, bool isMono);

    /// <summary>
    ///     Creates a new <see cref="IAudioRecorder" />.
    /// </summary>
    /// <remarks>
    ///     The AudioRecorder has to be disposed after it is no longer used.
    /// </remarks>
    /// <param name="samplingRate">The sampling rate in Hertz.</param>
    /// <param name="isMono">Whether the recorder records in mono or stereo.</param>
    /// <returns>The IAudioRecorder.</returns>
    /// <exception cref="Utils.GdxRuntimeException">In case the recorder could not be created.</exception>
    public IAudioRecorder NewAudioRecorder(int samplingRate, bool isMono);

    /// <summary>
    ///     Creates a new <see cref="IMusic" /> instance which is used to play back a music stream from a file.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Currently supported formats are WAV, MP3 and OGG. The Music instance has to be disposed if it is no longer used
    ///         via the <see cref="IMusic.Dispose()" /> method.
    ///     </para>
    ///     <para>
    ///         Music instances are automatically paused when <see cref="IApplicationListener.Pause()" /> is called and resumed
    ///         when <see cref="IApplicationListener.Resume()" /> is called.
    ///     </para>
    /// </remarks>
    /// <param name="file">The FileHandle.</param>
    /// <returns>The new Music or null if the Music could not be loaded.</returns>
    /// <exception cref="Utils.GdxRuntimeException">In case the music could not be loaded.</exception>
    public IMusic NewMusic(FileHandle file);

    /// <summary>
    ///     Creates a new <see cref="ISound" /> which is used to play back audio effects such as gun shots or explosions.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The Sound's audio data is retrieved from the file specified via the <see cref="FileHandle" />.
    ///     </para>
    ///     <para>
    ///         Note that the complete audio data is loaded into RAM. You should therefore not load big audio files with this
    ///         method. The current upper limit for decoded audio is 1 MB.
    ///     </para>
    ///     <para>
    ///         Currently supported formats are WAV, MP3 and OGG.
    ///     </para>
    ///     <para>
    ///         The Sound has to be disposed if it is no longer used via the <see cref="ISound.Dispose()" /> method.
    ///     </para>
    /// </remarks>
    /// <param name="fileHandle">The FileHandle.</param>
    /// <returns>The new Sound.</returns>
    /// <exception cref="Utils.GdxRuntimeException">In case the sound could not be loaded.</exception>
    public ISound NewSound(FileHandle fileHandle);

    /// <summary>
    ///     Sets a new OutputDevice. The identifier can be retrieved from <see cref="IAudio.GetAvailableOutputDevices()" />.
    /// </summary>
    /// <remarks>
    ///     If null is passed, it will switch to auto.
    /// </remarks>
    /// <param name="deviceIdentifier">Device identifier to switch to, or null for auto.</param>
    /// <returns></returns>
    public bool SwitchOutputDevice(string? deviceIdentifier);
}