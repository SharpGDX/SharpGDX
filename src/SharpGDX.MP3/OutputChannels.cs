using System.Text;
using SharpGDX.Shims;

namespace SharpGDX.MP3;

/**
 * A Type-safe representation of the the supported output channel constants.
 * 
 * This class is immutable and, hence, is thread safe.
 * 
 * @author Mat McGowan 12/12/99
 * @since 0.0.7
 */
public class OutputChannels {
	/**
	 * Flag to indicate output should include both channels.
	 */
	public const int BOTH_CHANNELS = 0;

	/**
	 * Flag to indicate output should include the left channel only.
	 */
	public const int LEFT_CHANNEL = 1;

	/**
	 * Flag to indicate output should include the right channel only.
	 */
	public const int RIGHT_CHANNEL = 2;

	/**
	 * Flag to indicate output is mono.
	 */
	public const int DOWNMIX_CHANNELS = 3;

	public static readonly OutputChannels LEFT = new OutputChannels(LEFT_CHANNEL);
	public static readonly OutputChannels RIGHT = new OutputChannels(RIGHT_CHANNEL);
	public static readonly OutputChannels BOTH = new OutputChannels(BOTH_CHANNELS);
	public static readonly OutputChannels DOWNMIX = new OutputChannels(DOWNMIX_CHANNELS);

	private/* final */int outputChannels;

	/**
	 * Creates an <code>OutputChannels</code> instance corresponding to the given channel code.
	 * 
	 * @param code one of the OutputChannels channel code constants.
	 * 
	 * @throws IllegalArgumentException if code is not a valid channel code.
	 */
	static public OutputChannels fromInt (int code) {
		switch (code) {
		case LEFT_CHANNEL:
			return LEFT;
		case RIGHT_CHANNEL:
			return RIGHT;
		case BOTH_CHANNELS:
			return BOTH;
		case DOWNMIX_CHANNELS:
			return DOWNMIX;
		default:
			throw new IllegalArgumentException("Invalid channel code: " + code);
		}
	}

	private OutputChannels (int channels) {
		outputChannels = channels;

		if (channels < 0 || channels > 3) throw new IllegalArgumentException("channels");
	}

	/**
	 * Retrieves the code representing the desired output channels. Will be one of LEFT_CHANNEL, RIGHT_CHANNEL, BOTH_CHANNELS or
	 * DOWNMIX_CHANNELS.
	 * 
	 * @return the channel code represented by this instance.
	 */
	public int getChannelsOutputCode () {
		return outputChannels;
	}

	/**
	 * Retrieves the number of output channels represented by this channel output type.
	 * 
	 * @return The number of output channels for this channel output type. This will be 2 for BOTH_CHANNELS only, and 1 for all
	 *         other types.
	 */
	public int getChannelCount () {
		int count = outputChannels == BOTH_CHANNELS ? 2 : 1;
		return count;
	}

	public override bool Equals (Object? o) {
		bool equals = false;

		if (o is OutputChannels) {
			OutputChannels oc = (OutputChannels)o;
			equals = oc.outputChannels == outputChannels;
		}

		return equals;
	}

	public override int GetHashCode () {
		return outputChannels;
	}

}
