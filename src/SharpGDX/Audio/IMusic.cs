﻿using SharpGDX.Utils;

namespace SharpGDX.Audio
{
	/**
 * <p>
 * A Music instance represents a streamed audio file. The interface supports pausing, resuming and so on. When you are done with
 * using the Music instance you have to dispose it via the {@link #dispose()} method.
 * </p>
 *
 * <p>
 * Music instances are created via {@link Audio#newMusic(FileHandle)}.
 * </p>
 *
 * <p>
 * Music instances are automatically paused and resumed when an {@link Application} is paused or resumed. See
 * {@link ApplicationListener}.
 * </p>
 *
 * <p>
 * <b>Note</b>: any values provided will not be clamped, it is the developer's responsibility to do so
 * </p>
 *
 * @author mzechner */
	public interface IMusic : Disposable
	{
		/** Starts the play back of the music stream. In case the stream was paused this will resume the play back. In case the music
		 * stream is finished playing this will restart the play back. */
		public void Play();

		/** Pauses the play back. If the music stream has not been started yet or has finished playing a call to this method will be
		 * ignored. */
		public void Pause();

		/** Stops a playing or paused Music instance. Next time play() is invoked the Music will start from the beginning. */
		public void Stop();

		/// <summary>
		/// Gets whether this music stream is playing.
		/// </summary>
		public bool IsPlaying { get; }

		/** Sets whether the music stream is looping. This can be called at any time, whether the stream is playing.
		 *
		 * @param isLooping whether to loop the stream */
		public void SetLooping(bool isLooping);

		/** @return whether the music stream is playing. */
		public bool IsLooping();

		/// <summary>
		/// Gets or sets the volume of the music stream.
		/// </summary>
		/// <remarks>
		/// The volume must be given in the range [0,1] with 0 being silent and 1 being the maximum volume.
		/// </remarks>
		public float Volume { get; set; }

		/** Sets the panning and volume of this music stream.
		 * @param pan panning in the range -1 (full left) to 1 (full right). 0 is center position.
		 * @param volume the volume in the range [0,1]. */
		public void SetPan(float pan, float volume);

		/// <summary>
		/// Gets or sets the playback position in seconds.
		/// </summary>
		public float Position { get; set; }

		/// <summary>
		/// Needs to be called when the IMusic is no longer needed.
		/// </summary>
		public void Dispose();

		/** Register a callback to be invoked when the end of a music stream has been reached during playback.
		 *
		 * @param listener the callback that will be run. */
		public void setOnCompletionListener(Action<IMusic> listener);
	}
}