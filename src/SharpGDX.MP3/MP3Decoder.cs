using System.Text;
using SharpGDX.Shims;

namespace SharpGDX.MP3;


/**
 * The <code>Decoder</code> class encapsulates the details of decoding an MPEG audio frame.
 * 
 * @author MDM
 * @version 0.0.7 12/12/99
 * @since 0.0.5
 */
public class MP3Decoder {
	/**
	 * The Bistream from which the MPEG audio frames are read.
	 */
	// private Bitstream stream;
	/**
	 * The Obuffer instance that will receive the decoded PCM samples.
	 */
	private OutputBuffer output;

	/**
	 * Synthesis filter for the left channel.
	 */
	private SynthesisFilter filter1;

	/**
	 * Sythesis filter for the right channel.
	 */
	private SynthesisFilter filter2;

	private LayerIIIDecoder? l3decoder;
	private LayerIIDecoder? l2decoder;
	private LayerIDecoder? l1decoder;

	private int outputFrequency;
	private int outputChannels;

	private bool initialized;

	/**
	 * Creates a new <code>Decoder</code> instance with default parameters.
	 */

	public MP3Decoder () {
	}

	/**
	 * Decodes one frame from an MPEG audio bitstream.
	 * 
	 * @param header The header describing the frame to decode.
	 * @param bitstream The bistream that provides the bits for te body of the frame.
	 * 
	 * @return A SampleBuffer containing the decoded samples.
	 */
	public OutputBuffer decodeFrame (Header header, Bitstream stream)  {
		if (!initialized) initialize(header);

		int layer = header.layer();

		FrameDecoder decoder = retrieveDecoder(header, stream, layer);

		decoder.decodeFrame();

		return output;
	}

	/**
	 * Changes the output buffer. This will take effect the next time decodeFrame() is called.
	 */
	public void setOutputBuffer (OutputBuffer @out) {
		output = @out;
	}

	/**
	 * Retrieves the sample frequency of the PCM samples output by this decoder. This typically corresponds to the sample rate
	 * encoded in the MPEG audio stream.
	 * 
	 * @param the sample rate (in Hz) of the samples written to the output buffer when decoding.
	 */
	public int getOutputFrequency () {
		return outputFrequency;
	}

	/**
	 * Retrieves the number of channels of PCM samples output by this decoder. This usually corresponds to the number of channels
	 * in the MPEG audio stream, although it may differ.
	 * 
	 * @return The number of output channels in the decoded samples: 1 for mono, or 2 for stereo.
	 * 
	 */
	public int getOutputChannels () {
		return outputChannels;
	}

	protected DecoderException newDecoderException (int errorcode) {
		return new DecoderException(errorcode, null);
	}

	protected DecoderException newDecoderException (int errorcode, Exception throwable) {
		return new DecoderException(errorcode, throwable);
	}

	protected FrameDecoder retrieveDecoder (Header header, Bitstream stream, int layer)  {
		FrameDecoder decoder = null;

		// REVIEW: allow channel output selection type
		// (LEFT, RIGHT, BOTH, DOWNMIX)
		switch (layer) {
		case 3:
			if (l3decoder == null)
				l3decoder = new LayerIIIDecoder(stream, header, filter1, filter2, output, OutputChannels.BOTH_CHANNELS);

			decoder = l3decoder;
			break;
		case 2:
			if (l2decoder == null) {
				l2decoder = new LayerIIDecoder();
				l2decoder.create(stream, header, filter1, filter2, output, OutputChannels.BOTH_CHANNELS);
			}
			decoder = l2decoder;
			break;
		case 1:
			if (l1decoder == null) {
				l1decoder = new LayerIDecoder();
				l1decoder.create(stream, header, filter1, filter2, output, OutputChannels.BOTH_CHANNELS);
			}
			decoder = l1decoder;
			break;
		}

		if (decoder == null) throw newDecoderException(UNSUPPORTED_LAYER, null);

		return decoder;
	}

	private void initialize (Header header)  {

		// REVIEW: allow customizable scale factor
		float scalefactor = 32700.0f;

		int mode = header.mode();
		header.layer();
		int channels = mode == Header.SINGLE_CHANNEL ? 1 : 2;

		// set up output buffer if not set up by client.
		if (output == null) throw new RuntimeException("Output buffer was not set.");

		filter1 = new SynthesisFilter(0, scalefactor, null);

		// REVIEW: allow mono output for stereo
		if (channels == 2) filter2 = new SynthesisFilter(1, scalefactor, null);

		outputChannels = channels;
		outputFrequency = header.frequency();

		initialized = true;
	}

	/**
	 * The first decoder error code. See the {@link DecoderErrors DecoderErrors} interface for other decoder error codes.
	 */
	static public readonly int DECODER_ERROR = 0x200;

	static public readonly int UNKNOWN_ERROR = DECODER_ERROR + 0;

	/**
	 * Layer not supported by the decoder.
	 */
	static public readonly int UNSUPPORTED_LAYER = DECODER_ERROR + 1;

	/**
	 * Illegal allocation in subband layer. Indicates a corrupt stream.
	 */
	static public readonly int ILLEGAL_SUBBAND_ALLOCATION = DECODER_ERROR + 2;
}
