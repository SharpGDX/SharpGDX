using System.Text;
using SharpGDX.Shims;

namespace SharpGDX.MP3;

/**
 * The <code>DecoderException</code> represents the class of errors that can occur when decoding MPEG audio.
 * 
 * @author MDM
 */
public class DecoderException : JavaLayerException  {
	private int errorcode = MP3Decoder.UNKNOWN_ERROR;

	public DecoderException (String msg, Exception t) 
    : base(msg, t)
    {
		
	}

	public DecoderException (int errorcode, Exception t) 
    : this(getErrorString(errorcode), t)
    {
		
		this.errorcode = errorcode;
	}

	public int getErrorCode () {
		return errorcode;
	}

	static public String getErrorString (int errorcode) {
		// REVIEW: use resource file to map error codes
		// to locale-sensitive strings.

		return "Decoder errorcode " + (errorcode).ToString("X");
	}

}
