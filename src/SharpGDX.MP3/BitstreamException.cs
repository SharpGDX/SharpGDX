using System.Text;
using SharpGDX.Shims;

namespace SharpGDX.MP3;

/**
 * Instances of <code>BitstreamException</code> are thrown when operations on a <code>Bitstream</code> fail.
 * <p>
 * The exception provides details of the exception condition in two ways:
 * <ol>
 * <li>
 * as an error-code describing the nature of the error</li><br>
 * </br>
 * <li>
 * as the <code>Throwable</code> instance, if any, that was thrown indicating that an exceptional condition has occurred.</li>
 * </ol>
 * </p>
 * 
 * @since 0.0.6
 * @author MDM 12/12/99
 */

public class BitstreamException : JavaLayerException {
	private int errorcode = Bitstream.UNKNOWN_ERROR;

	public BitstreamException (String msg, Exception t) 
    : base(msg, t)
    {
		
	}

	public BitstreamException (int errorcode, Exception t) 
    : this(getErrorString(errorcode), t)
    {
		
		this.errorcode = errorcode;
	}

	public int getErrorCode () {
		return errorcode;
	}

	static public String getErrorString (int errorcode) {
		// REVIEW: use resource bundle to map error codes
		// to locale-sensitive strings.

		return "Bitstream errorcode " +(errorcode).ToString("X");
	}

}
