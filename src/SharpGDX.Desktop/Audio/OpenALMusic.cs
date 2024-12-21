using SharpGDX.Files;
using SharpGDX.Mathematics;
using OpenTK.Audio.OpenAL;
using SharpGDX.Audio;
using SharpGDX.Shims;
using SharpGDX.Utils;

// TODO: Not fond of the creation of arrays just to pass queue and dequeue buffers.

namespace SharpGDX.Desktop.Audio
{
	/** @author Nathan Sweet */
	public abstract class OpenALMusic : IMusic
	{
		private static readonly int bufferSize = 4096 * 10;
		private static readonly int bufferCount = 3;
		private static readonly byte[] tempBytes = new byte[bufferSize];

		// TODO: This was originally using BufferUtils.createByteBuffer, not sure if this will work.
		private static readonly ByteBuffer tempBuffer = ByteBuffer.allocate(bufferSize);

		private FloatArray renderedSecondsQueue = new FloatArray(bufferCount);

		private readonly OpenALDesktopAudio audio;
		private IntBuffer buffers;
		private int sourceID = -1;
		private ALFormat format;
		private int sampleRate;
		private bool _isLooping, _isPlaying;
		private float volume = 1;
		private float pan = 0;
		private float renderedSeconds, maxSecondsPerBuffer;

		protected readonly FileHandle file;

		private Action<IMusic>? onCompletionListener;

		public OpenALMusic(OpenALDesktopAudio audio, FileHandle file)
		{
			this.audio = audio;
			this.file = file;
			this.onCompletionListener = null;
		}

        /** Prepare our music for playback!
     * @param channels The number of channels for the music. Most commonly 1 (for mono) or 2 (for stereo).
     * @param bitDepth The number of bits in each sample. Normally 16. Can also be 8, 32, 64.
     * @param sampleRate The number of samples to be played each second. Commonly 44100; can be anything within reason. */
        protected void Setup(int channels, int bitDepth, int sampleRate)
        {
            this.format = OpenALUtils.determineFormat(channels, bitDepth);
            this.sampleRate = sampleRate;
            this.maxSecondsPerBuffer = (float)bufferSize / ((bitDepth >> 3) * channels * sampleRate);
        }

		public void Play()
		{
			if (audio.noDevice) return;
			if (sourceID == -1)
			{
				sourceID = audio.obtainSource(true);
				if (sourceID == -1) return;

				audio.music.Add(this);

				if (buffers == null)
				{
					// TODO: This was originally using BufferUtils.createIntBuffer, not sure if this will work
					buffers = IntBuffer.allocate(bufferCount);
					AL.GetError();
					AL.GenBuffers(bufferCount, buffers.array());
					var errorCode = AL.GetError();
					if (errorCode != ALError.NoError)
						throw new GdxRuntimeException("Unable to allocate audio buffers. AL Error: " + errorCode);
				}

				AL.Source(sourceID, ALSourceb.Looping, false);
				SetPan(pan, volume);

				AL.GetError();

				bool filled = false; // Check if there's anything to actually play.
				for (int i = 0; i < bufferCount; i++)
				{
					int bufferID = buffers.get(i);
					if (!fill(bufferID)) break;
					filled = true;
					// TODO: Verify
					AL.SourceQueueBuffers(sourceID, 1, new[] { bufferID });
				}

				if (!filled && onCompletionListener != null) onCompletionListener?.Invoke(this);

				if (AL.GetError() != ALError.NoError)
				{
					Stop();
					return;
				}
			}

			if (!_isPlaying)
			{
				AL.SourcePlay(sourceID);
				_isPlaying = true;
			}
		}

		public void Stop()
		{
			if (audio.noDevice) return;
			if (sourceID == -1) return;
			audio.music.RemoveValue(this, true);
			reset();
			audio.freeSource(sourceID);
			sourceID = -1;
			renderedSeconds = 0;
			renderedSecondsQueue.clear();
			_isPlaying = false;
		}

		public void Pause()
		{
			if (audio.noDevice) return;
			if (sourceID != -1) AL.SourcePause(sourceID);
			_isPlaying = false;
		}

		/// <inheritdoc cref="IMusic.IsPlaying()" />
		public bool IsPlaying()
		{
				if (audio.noDevice)
				{
					return false;
				}

				if (sourceID == -1)
				{
					return false;
				}

				return _isPlaying;
		}

        public void SetLooping(bool isLooping)
        {
            _isLooping = isLooping;
        }

        public bool IsLooping()
        {
            return _isLooping;
        }
        
		public float GetVolume()
		{
			return this.volume;
		}

        public void SetVolume(float volume)
        {
            if (volume < 0)
            {
                throw new IllegalArgumentException("volume cannot be < 0: " + volume);
            }

            this.volume = volume;

            if (audio.noDevice)
            {
                return;
            }

            if (sourceID != -1)
            {
                AL.Source(sourceID, ALSourcef.Gain, volume);
            }
        }

		public void SetPan(float pan, float volume)
		{
			this.volume = volume;
			this.pan = pan;
			if (audio.noDevice) return;
			if (sourceID == -1) return;
			AL.Source(sourceID, ALSource3f.Position, MathUtils.cos((pan - 1) * MathUtils.HALF_PI), 0,
				MathUtils.sin((pan + 1) * MathUtils.HALF_PI));
			AL.Source(sourceID, ALSourcef.Gain, volume);
		}

		public float Position
		{
			get
			{
				if (audio.noDevice) return 0;
				if (sourceID == -1) return 0;
				AL.GetSource(sourceID, ALSourcef.SecOffset, out var offset);
				return renderedSeconds + offset;
			}
			set
			{
				if (audio.noDevice) return;
				if (sourceID == -1) return;
				bool wasPlaying = _isPlaying;
				_isPlaying = false;
				AL.SourceStop(sourceID);
				AL.SourceUnqueueBuffers(sourceID, 1, buffers.array());
				while (renderedSecondsQueue.size > 0)
				{
					renderedSeconds = renderedSecondsQueue.pop();
				}

				if (value <= renderedSeconds)
				{
					reset();
					renderedSeconds = 0;
				}

				while (renderedSeconds < (value - maxSecondsPerBuffer))
				{
					int length = read(tempBytes);
					if (length <= 0) break;
					float currentBufferSeconds = maxSecondsPerBuffer * (float)length / (float)bufferSize;
					renderedSeconds += currentBufferSeconds;
				}

				renderedSecondsQueue.add(renderedSeconds);
				bool filled = false;
				for (int i = 0; i < bufferCount; i++)
				{
					int bufferID = buffers.get(i);
					if (!fill(bufferID)) break;
					filled = true;
					AL.SourceQueueBuffers(sourceID, 1, new[] { bufferID });
				}

				renderedSecondsQueue.pop();
				if (!filled)
				{
					Stop();
					if (onCompletionListener != null) onCompletionListener?.Invoke(this);
				}

				AL.Source(sourceID, ALSourcef.SecOffset, value - renderedSeconds);
				if (wasPlaying)
				{
					AL.SourcePlay(sourceID);
					_isPlaying = true;
				}
			}
		}

		/** Fills as much of the buffer as possible and returns the number of bytes filled. Returns <= 0 to indicate the end of the
		 * stream. */
		public abstract int read(byte[] buffer);

		/** Resets the stream to the beginning. */
		public abstract void reset();

		/** By default, does just the same as reset(). Used to add special behaviour in Ogg.Music. */
		protected void loop()
		{
			reset();
		}

		public int getChannels()
		{
			return format == ALFormat.Stereo16 ? 2 : 1;
		}

		public int getRate()
		{
			return sampleRate;
		}

		public void update()
		{
			if (audio.noDevice) return;
			if (sourceID == -1) return;

			bool end = false;
			AL.GetSource(sourceID, ALGetSourcei.BuffersProcessed, out var buffers);
			while (buffers-- > 0)
			{
				// TODO: Verify
				var bufferIds = new int[1];
				AL.SourceUnqueueBuffers(sourceID, 1, bufferIds);
				int bufferID = bufferIds[0];

				if (bufferID == (int)ALError.InvalidValue) break;
				if (renderedSecondsQueue.size > 0) renderedSeconds = renderedSecondsQueue.pop();
				if (end) continue;
				if (fill(bufferID))
					AL.SourceQueueBuffers(sourceID, 1, new[] { bufferID });
				else
					end = true;
			}

			AL.GetSource(sourceID, ALGetSourcei.BuffersQueued, out var queued);

			if (end && queued == 0)
			{
				Stop();
				if (onCompletionListener != null) onCompletionListener?.Invoke(this);
			}

			// A buffer underflow will cause the source to stop.
			AL.GetSource(sourceID, ALGetSourcei.SourceState, out var state);
			if (_isPlaying && state != (int)ALSourceState.Playing) AL.SourcePlay(sourceID);
		}

		private bool fill(int bufferID)
		{
			tempBuffer.clear();
			int length = read(tempBytes);
			if (length <= 0)
			{
				if (_isLooping)
				{
					loop();
					length = read(tempBytes);
					if (length <= 0) return false;
					if (renderedSecondsQueue.size > 0)
					{
						renderedSecondsQueue.set(0, 0);
					}
				}
				else
					return false;
			}

			float previousLoadedSeconds = renderedSecondsQueue.size > 0 ? renderedSecondsQueue.first() : 0;
			float currentBufferSeconds = maxSecondsPerBuffer * (float)length / (float)bufferSize;
			renderedSecondsQueue.insert(0, previousLoadedSeconds + currentBufferSeconds);

			tempBuffer.put(tempBytes, 0, length).flip();
			AL.BufferData(bufferID, format, tempBuffer.array(), sampleRate);

			return true;
		}

		public void Dispose()
		{
			Stop();
			if (audio.noDevice) return;
			if (buffers == null) return;
			AL.DeleteBuffers(1, buffers.array());
			buffers = null;
			onCompletionListener = null;
		}

		public void SetOnCompletionListener(Action<IMusic> listener)
		{
			onCompletionListener = listener;
		}

		public int getSourceId()
		{
			return sourceID;
		}
	}
}