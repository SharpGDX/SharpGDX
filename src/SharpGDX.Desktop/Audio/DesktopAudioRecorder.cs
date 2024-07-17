using SharpGDX.Audio;

namespace SharpGDX.Desktop.Audio
{
	public class JavaSoundAudioRecorder : IAudioRecorder
	{
	//private TargetDataLine line;
	//private byte[] buffer = new byte[1024 * 4];

	public JavaSoundAudioRecorder(int samplingRate, bool isMono)
	{
		//try
		//{
		//	AudioFormat format = new AudioFormat(Encoding.PCM_SIGNED, samplingRate, 16, isMono ? 1 : 2, isMono ? 2 : 4, samplingRate,
		//		false);
		//	line = AudioSystem.getTargetDataLine(format);
		//	line.open(format, buffer.length);
		//	line.start();
		//}
		//catch (Exception ex)
		//{
		//	throw new GdxRuntimeException("Error creating JavaSoundAudioRecorder.", ex);
		//}
	}

	public void Read(short[] samples, int offset, int numSamples)
	{
		//if (buffer.length < numSamples * 2) buffer = new byte[numSamples * 2];

		//int toRead = numSamples * 2;
		//int read = 0;
		//while (read != toRead)
		//	read += line.read(buffer, read, toRead - read);

		//for (int i = 0, j = 0; i < numSamples * 2; i += 2, j++)
		//	samples[offset + j] = (short)((buffer[i + 1] << 8) | (buffer[i] & 0xff));
	}

	public void dispose()
	{
		//line.close();
	}
	}
}
