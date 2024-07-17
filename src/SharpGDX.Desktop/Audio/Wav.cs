using SharpGDX.Files;
using SharpGDX.Utils;
using SharpGDX.Shims;

namespace SharpGDX.Desktop.Audio
{
	public class Wav
	{
		public class Music : OpenALMusic
		{
			private WavInputStream input;

			public Music(OpenALDesktopAudio audio, FileHandle file)
				: base(audio, file)
			{

				input = new WavInputStream(file);
				if (audio.noDevice) return;
				setup(input.channels, input.sampleRate);
			}

			public override int read(byte[] buffer)
			{
				if (input == null)
				{
					input = new WavInputStream(file);
					setup(input.channels, input.sampleRate);
				}

				try
				{
					return input.read(buffer);
				}
				catch (IOException ex)
				{
					throw new GdxRuntimeException("Error reading WAV file: " + file, ex);
				}
			}

			public override void reset()
			{
				StreamUtils.closeQuietly(input);
				input = null;
			}
		}

		public class Sound : OpenALSound
		{
			public Sound(OpenALDesktopAudio audio, FileHandle file)
				: base(audio)
			{

				if (audio.noDevice) return;

				WavInputStream input = null;
				try
				{
					input = new WavInputStream(file);
					setup(StreamUtils.copyStreamToByteArray(input, input.dataRemaining), input.channels,
						input.sampleRate);
				}
				catch (IOException ex)
				{
					throw new GdxRuntimeException("Error reading WAV file: " + file, ex);
				}
				finally
				{
					StreamUtils.closeQuietly(input);
				}
			}
		}

		/** @author Nathan Sweet */
		public class WavInputStream : FilterInputStream
		{

			public int channels, sampleRate, dataRemaining;

			public WavInputStream(FileHandle file)
				: base(file.read())
			{

				try
				{
					if (read() != 'R' || read() != 'I' || read() != 'F' || read() != 'F')
						throw new GdxRuntimeException("RIFF header not found: " + file);

					skipFully(4);

					if (read() != 'W' || read() != 'A' || read() != 'V' || read() != 'E')
						throw new GdxRuntimeException("Invalid wave file header: " + file);

					int fmtChunkLength = seekToChunk('f', 'm', 't', ' ');

					// http://www-mmsp.ece.mcgill.ca/Documents/AudioFormats/WAVE/WAVE.html
					// http://soundfile.sapp.org/doc/WaveFormat/
					int type = read() & 0xff | (read() & 0xff) << 8;
					if (type != 1)
					{
						String name;
						switch (type)
						{
							case 0x0002:
								name = "ADPCM";
								break;
							case 0x0003:
								name = "IEEE float";
								break;
							case 0x0006:
								name = "8-bit ITU-T G.711 A-law";
								break;
							case 0x0007:
								name = "8-bit ITU-T G.711 u-law";
								break;
							case 0xFFFE:
								name = "Extensible";
								break;
							default:
								name = "Unknown";
								break;
						}

						throw new GdxRuntimeException("WAV files must be PCM, unsupported format: " + name + " (" +
						                              type + ")");
					}

					channels = read() & 0xff | (read() & 0xff) << 8;
					if (channels != 1 && channels != 2)
						throw new GdxRuntimeException("WAV files must have 1 or 2 channels: " + channels);
					sampleRate = read() & 0xff | (read() & 0xff) << 8 | (read() & 0xff) << 16 | (read() & 0xff) << 24;
					skipFully(6);

					int bitsPerSample = read() & 0xff | (read() & 0xff) << 8;
					if (bitsPerSample != 16)
						throw new GdxRuntimeException("WAV files must have 16 bits per sample: " + bitsPerSample);

					skipFully(fmtChunkLength - 16);

					dataRemaining = seekToChunk('d', 'a', 't', 'a');
				}
				catch (Exception ex)
				{
					StreamUtils.closeQuietly(this);
					throw new GdxRuntimeException("Error reading WAV file: " + file, ex);
				}
			}

			private int seekToChunk(char c1, char c2, char c3, char c4) // TODO: throws IOException
			{
				while (true)
				{
					bool found = read() == c1;
					found &= read() == c2;
					found &= read() == c3;
					found &= read() == c4;
					int chunkLength = read() & 0xff | (read() & 0xff) << 8 | (read() & 0xff) << 16 |
					                  (read() & 0xff) << 24;
					if (chunkLength == -1) throw new IOException("Chunk not found: " + c1 + c2 + c3 + c4);
					if (found) return chunkLength;
					skipFully(chunkLength);
				}
			}

			private void skipFully(int count) // TODO: throws IOException
			{
				while (count > 0)
				{
					long skipped = @in.skip(count);
					if (skipped <= 0) throw new EOFException("Unable to skip.");
					// TODO: How was this not casting in Java?
					count -= (int)skipped;
				}
			}

			public override int read(byte[] buffer) // TODO: throws IOException
			{
				if (dataRemaining == 0) return -1;
				int offset = 0;
				do
				{
					int length = Math.Min(base.read(buffer, offset, buffer.Length - offset), dataRemaining);
					if (length == -1)
					{
						if (offset > 0) return offset;
						return -1;
					}

					offset += length;
					dataRemaining -= length;

					if (dataRemaining <= 0)
					{
						var s = 1;
					}

				} while (offset < buffer.Length);

				{
					return offset;
				}
			}
		}
	}
}
