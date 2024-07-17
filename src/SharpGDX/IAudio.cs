using SharpGDX.Audio;
using SharpGDX.Files;

namespace SharpGDX
{
	/** This interface encapsulates the creation and management of audio resources. It allows you to get direct access to the audio
 * hardware via the {@link AudioDevice} and {@link AudioRecorder} interfaces, create sound effects via the {@link Sound} interface
 * and play music streams via the {@link Music} interface.
 * 
 * <p>
 * All resources created via this interface have to be disposed as soon as they are no longer used.
 * </p>
 * 
 * <p>
 * Note that all {@link Music} instances will be automatically paused when the {@link ApplicationListener#pause()} method is
 * called, and automatically resumed when the {@link ApplicationListener#resume()} method is called.
 * </p>
 * 
 * @author mzechner */
	public interface IAudio
	{
		/** Creates a new {@link AudioDevice} either in mono or stereo mode. The AudioDevice has to be disposed via its
		 * {@link AudioDevice#dispose()} method when it is no longer used.
		 * 
		 * @param samplingRate the sampling rate.
		 * @param isMono whether the AudioDevice should be in mono or stereo mode
		 * @return the AudioDevice
		 * 
		 * @throws GdxRuntimeException in case the device could not be created */
		public IAudioDevice newAudioDevice(int samplingRate, bool isMono);

		/** Creates a new {@link AudioRecorder}. The AudioRecorder has to be disposed after it is no longer used.
		 * 
		 * @param samplingRate the sampling rate in Hertz
		 * @param isMono whether the recorder records in mono or stereo
		 * @return the AudioRecorder
		 * 
		 * @throws GdxRuntimeException in case the recorder could not be created */
		public IAudioRecorder newAudioRecorder(int samplingRate, bool isMono);

		/**
		 * <p>
		 * Creates a new {@link Sound} which is used to play back audio effects such as gun shots or explosions. The Sound's audio data
		 * is retrieved from the file specified via the {@link FileHandle}. Note that the complete audio data is loaded into RAM. You
		 * should therefore not load big audio files with this methods. The current upper limit for decoded audio is 1 MB.
		 * </p>
		 * 
		 * <p>
		 * Currently supported formats are WAV, MP3 and OGG.
		 * </p>
		 * 
		 * <p>
		 * The Sound has to be disposed if it is no longer used via the {@link Sound#dispose()} method.
		 * </p>
		 * 
		 * @return the new Sound
		 * @throws GdxRuntimeException in case the sound could not be loaded */
		public ISound newSound(FileHandle fileHandle);

		/** Creates a new {@link Music} instance which is used to play back a music stream from a file. Currently supported formats are
		 * WAV, MP3 and OGG. The Music instance has to be disposed if it is no longer used via the {@link Music#dispose()} method.
		 * Music instances are automatically paused when {@link ApplicationListener#pause()} is called and resumed when
		 * {@link ApplicationListener#resume()} is called.
		 * 
		 * @param file the FileHandle
		 * @return the new Music or null if the Music could not be loaded
		 * @throws GdxRuntimeException in case the music could not be loaded */
		public IMusic newMusic(FileHandle file);

		/** Sets a new OutputDevice. The identifier can be retrieved from {@link Audio#getAvailableOutputDevices()}. If null is passed,
		 * it will switch to auto.
		 *
		 * @param deviceIdentifier device identifier to switch to, or null for auto */
		public bool switchOutputDevice(String? deviceIdentifier);

		/** This function returns a list of fully qualified Output device names. This function is only implemented on desktop. On all
		 * other platforms it will return a empty array. It will also return a empty array on error. The names returned need os
		 * dependent preprocessing before exposing to a user.
		 *
		 * @return A array of available output devices */
		public String[] getAvailableOutputDevices();
	}
}
