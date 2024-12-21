using System.Text;
using SharpGDX.Shims;

namespace SharpGDX.MP3;

/**
 * Base Class for audio output.
 */
public class OutputBuffer {
	public static readonly int BUFFERSIZE = 2 * 1152; // max. 2 * 1152 samples per frame
	private static readonly int MAXCHANNELS = 2; // max. number of channels

	private float? replayGainScale;
	private int channels;
	private byte[] buffer;
	private int[] channelPointer;
	private bool isBigEndian;

	public OutputBuffer (int channels, bool isBigEndian) {
		this.channels = channels;
		this.isBigEndian = isBigEndian;
		buffer = new byte[BUFFERSIZE * channels];
		channelPointer = new int[channels];
		reset();
	}

	/**
	 * Takes a 16 Bit PCM sample.
	 */
	private void append (int channel, short value) {
		byte firstByte;
		byte secondByte;
		if (isBigEndian) {
			firstByte = (byte)(value >>> 8 & 0xFF);
			secondByte = (byte)(value & 0xFF);
		} else {
			firstByte = (byte)(value & 0xFF);
			secondByte = (byte)(value >>> 8 & 0xFF);
		}
		buffer[channelPointer[channel]] = firstByte;
		buffer[channelPointer[channel] + 1] = secondByte;
		channelPointer[channel] += channels * 2;
	}

	/**
	 * Takes 32 PCM samples.
	 */
	public void appendSamples (int channel, float[] f) {
		short s;
		if (replayGainScale != null) {
			for (int i = 0; i < 32;) {
				s = clip(f[i++] * replayGainScale.Value);
				append(channel, s);
			}
		} else {
			for (int i = 0; i < 32;) {
				s = clip(f[i++]);
				append(channel, s);
			}
		}
	}

	public byte[] getBuffer () {
		return buffer;
	}

	public int reset () {
		try {
			int index = channels - 1;
			return channelPointer[index] - index * 2;
		} finally {
			// Points to byte location, implicitely assuming 16 bit samples.
			for (int i = 0; i < channels; i++)
				channelPointer[i] = i * 2;
		}
	}

	public void setReplayGainScale (float replayGainScale) {
		this.replayGainScale = replayGainScale;
	}

	public bool isStereo () {
		return channelPointer[1] == 2;
	}

	// Clip to 16 bits.
	private short clip (float sample) {
		// TODO: Is this casting at the correct time? -RP
		return (short)(sample > 32767.0f ? 32767 : sample < -32768.0f ? -32768 : (short)sample);
	}

}
