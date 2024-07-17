using SharpGDX.Mathematics;
using SharpGDX.Shims;
using SharpGDX.Utils;
using OpenTK.Audio.OpenAL;
using SharpGDX.Audio;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Desktop.Audio
{
	/** @author Nathan Sweet */
	public class OpenALAudioDevice : IAudioDevice
	{
		private static readonly int bytesPerSample = 2;

		private readonly OpenALDesktopAudio audio;
		private readonly int channels;
		private IntBuffer? buffers;
		private int sourceID = -1;
		private ALFormat format;
		private int sampleRate;
		private bool _isPlaying;
		private float volume = 1;
		private float renderedSeconds, secondsPerBuffer;
		private byte[]? bytes;
		private readonly int bufferSize;
		private readonly int bufferCount;
		private readonly ByteBuffer tempBuffer;

		public OpenALAudioDevice(OpenALDesktopAudio audio, int sampleRate, bool isMono, int bufferSize, int bufferCount)
		{
			this.audio = audio;
			channels = isMono ? 1 : 2;
			this.bufferSize = bufferSize;
			this.bufferCount = bufferCount;
			this.format = channels > 1 ? ALFormat.Stereo16 : ALFormat.Mono16;
			this.sampleRate = sampleRate;
			secondsPerBuffer = (float)bufferSize / bytesPerSample / channels / sampleRate;

			// TODO: This appeared to use the lwjgl BufferUtils.createByteBuffer, not sure if this is technically the same thing.
			tempBuffer = ByteBuffer.allocate(bufferSize);
		}

		public void WriteSamples(short[] samples, int offset, int numSamples)
		{
			if (bytes == null || bytes.Length < numSamples * 2) bytes = new byte[numSamples * 2];
			int end = Math.Min(offset + numSamples, samples.Length);
			for (int i = offset, ii = 0; i < end; i++)
			{
				short sample = samples[i];
				bytes[ii++] = (byte)(sample & 0xFF);
				bytes[ii++] = (byte)((sample >> 8) & 0xFF);
			}

			writeSamples(bytes, 0, numSamples * 2);
		}

		public void WriteSamples(float[] samples, int offset, int numSamples)
		{
			if (bytes == null || bytes.Length < numSamples * 2) bytes = new byte[numSamples * 2];
			int end = Math.Min(offset + numSamples, samples.Length);
			for (int i = offset, ii = 0; i < end; i++)
			{
				float floatSample = samples[i];
				floatSample = MathUtils.clamp(floatSample, -1f, 1f);
				int intSample = (int)(floatSample * 32767);
				bytes[ii++] = (byte)(intSample & 0xFF);
				bytes[ii++] = (byte)((intSample >> 8) & 0xFF);
			}

			writeSamples(bytes, 0, numSamples * 2);
		}

		public void writeSamples(byte[] data, int offset, int length)
		{
			if (length < 0) throw new IllegalArgumentException("length cannot be < 0.");

			if (sourceID == -1)
			{
				sourceID = audio.obtainSource(true);
				if (sourceID == -1) return;
				if (buffers == null)
				{
					// TODO: This appeared to use the lwjgl BufferUtils.createIntBuffer, not sure if this is technically the same thing.
					buffers = IntBuffer.allocate(bufferCount);
					AL.GetError();
					AL.GenBuffers(buffers.remaining(), buffers.array());
					if (AL.GetError() != ALError.NoError) throw new GdxRuntimeException("Unable to allocate audio buffers.");
				}

				AL.Source(sourceID, ALSourceb.Looping, false);
				AL.Source(sourceID, ALSourcef.Gain, volume);
				// Fill initial buffers.
				for (int i = 0; i < bufferCount; i++)
				{
					int bufferID = buffers.get(i);
					int written = Math.Min(bufferSize, length);
					((Buffer)tempBuffer).clear();
					((Buffer)tempBuffer.put(data, offset, written)).flip();
					AL.BufferData(bufferID, format, tempBuffer.array(), sampleRate);
					AL.SourceQueueBuffers(sourceID, 1, new []{ bufferID });
					length -= written;
					offset += written;
				}

				AL.SourcePlay(sourceID);
				_isPlaying = true;
			}

			while (length > 0)
			{
				int written = fillBuffer(data, offset, length);
				length -= written;
				offset += written;
			}
		}

		/** Blocks until some of the data could be buffered. */
		private int fillBuffer(byte[] data, int offset, int length)
		{
			int written = Math.Min(bufferSize, length);

			outer:
			while (true)
			{
				AL.GetSource(sourceID, ALGetSourcei.BuffersProcessed, out int buffers);
				while (buffers-- > 0)
				{
					// TODO: Verify it fills the bufferID
					int bufferID = 0;
					// TODO: This needs to be reformatted. -RP
					AL.SourceUnqueueBuffers(sourceID, 1,new []{ bufferID});
					if (bufferID == (int)ALError.InvalidValue) break;
					renderedSeconds += secondsPerBuffer;

					((Buffer)tempBuffer).clear();
					((Buffer)tempBuffer.put(data, offset, written)).flip();

					AL.BufferData(bufferID, format, tempBuffer.array(), sampleRate);

					// TODO: Why?
					int buffer = 0;
					AL.SourceQueueBuffers(sourceID, bufferID, new[] { buffer});
					goto endOfOuter;
				}

				// Wait for buffer to be free.
				try
				{
					Thread.Sleep((int)(1000 * secondsPerBuffer));
				}
				catch (ThreadInterruptedException ignored)
				{
				}
			}

			endOfOuter:

			AL.GetSource(sourceID, ALGetSourcei.SourceState, out var state);
			// A buffer underflow will cause the source to stop.
			if (!_isPlaying || state != (int)ALSourceState.Playing)
			{
				AL.SourcePlay(sourceID);
				_isPlaying = true;
			}

			return written;
		}

		public void stop()
		{
			if (sourceID == -1) return;
			audio.freeSource(sourceID);
			sourceID = -1;
			renderedSeconds = 0;
			_isPlaying = false;
		}

		public bool isPlaying()
		{
			if (sourceID == -1) return false;
			return _isPlaying;
		}

		public void SetVolume(float volume)
		{
			this.volume = volume;
			if (sourceID != -1) AL.Source(sourceID, ALSourcef.Gain, volume);
		}

		public float getPosition()
		{
			if (sourceID == -1) return 0;

			AL.GetSource(sourceID, ALSourcef.SecOffset, out var offset);

			return renderedSeconds + offset;
		}

		public void setPosition(float position)
		{
			renderedSeconds = position;
		}

		public int getChannels()
		{
			return format == ALFormat.Stereo16 ? 2 : 1;
		}

		public int getRate()
		{
			return sampleRate;
		}

		public void dispose()
		{
			if (buffers == null) return;
			if (sourceID != -1)
			{
				audio.freeSource(sourceID);
				sourceID = -1;
			}

			// TODO: Verify
			AL.DeleteBuffers(buffers.remaining(), buffers.array());
			buffers = null;
		}

		public bool IsMono()
		{
			return channels == 1;
		}

		public int GetLatency()
		{
			return (int)((float)bufferSize / bytesPerSample / channels * bufferCount);
		}

		public void Pause()
		{
			// A buffer underflow will cause the source to stop.
		}

		public void Resume()
		{
			// Automatically resumes when samples are written
		}
	}
}