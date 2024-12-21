using System.Text;
using SharpGDX.Shims;

namespace SharpGDX.MP3;

/**
 * Implementations of FrameDecoder are responsible for decoding an MPEG audio frame.
 * 
 */
// REVIEW: the interface currently is too thin. There should be
// methods to specify the output buffer, the synthesis filters and
// possibly other objects used by the decoder.
public interface FrameDecoder {
	/**
	 * Decodes one frame of MPEG audio.
	 */
	public void decodeFrame ();

}
