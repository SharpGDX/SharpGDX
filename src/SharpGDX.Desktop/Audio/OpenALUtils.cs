using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;
using SharpGDX.Utils;

namespace SharpGDX.Desktop.Audio
{
    public class OpenALUtils {

	/** @param channels The number of channels for the sound. Most commonly 1 (for mono) or 2 (for stereo).
	 * @param bitDepth The number of bits in each sample. Normally 16. Can also be 8, 32, 64.
	 * @return An OpenAL enum for use with {@link OpenALSound} and {@link OpenALMusic} */
	internal static ALFormat determineFormat (int channels, int bitDepth) { // @off
		ALFormat format;
		switch (channels) {
			case 1:
				switch (bitDepth) {
					case 8: format = OpenTK.Audio.OpenAL.ALFormat.Mono8; break;
					case 16: format = OpenTK.Audio.OpenAL.ALFormat.Mono16; break;
					case 32: format = OpenTK.Audio.OpenAL.ALFormat.MonoFloat32Ext; break;
					case 64: format = OpenTK.Audio.OpenAL.ALFormat.MonoDoubleExt; break;
					default: throw new GdxRuntimeException("Audio: Bit depth must be 8, 16, 32 or 64.");
				}
				break;
			case 2: // Doesn't work on mono devices (#6631)
				switch (bitDepth) {
					case 8: format = OpenTK.Audio.OpenAL.ALFormat.Stereo8; break;
					case 16: format = OpenTK.Audio.OpenAL.ALFormat.Stereo16; break;
					case 32: format = OpenTK.Audio.OpenAL.ALFormat.StereoFloat32Ext; break;
					case 64: format = OpenTK.Audio.OpenAL.ALFormat.StereoDoubleExt; break;
					default: throw new GdxRuntimeException("Audio: Bit depth must be 8, 16, 32 or 64.");
				}
				break;
			case 4: format = OpenTK.Audio.OpenAL.ALFormat.MultiQuad16Ext; break; // Works on stereo devices but not mono as above
			case 6: format = OpenTK.Audio.OpenAL.ALFormat.Multi51Chn16Ext; break;
			case 7: format = OpenTK.Audio.OpenAL.ALFormat.Multi61Chn16Ext; break;
			case 8: format = OpenTK.Audio.OpenAL.ALFormat.Multi71Chn16Ext; break;
			default: throw new GdxRuntimeException("Audio: Invalid number of channels. " +
				"Must be mono, stereo, quad, 5.1, 6.1 or 7.1.");
		}
		if (channels >= 4) {
			if (bitDepth == 8) format--; // Use 8-bit AL_FORMAT instead
			else if (bitDepth == 32) format++; // Use 32-bit AL_FORMAT instead
			else if (bitDepth != 16)
				throw new GdxRuntimeException("Audio: Bit depth must be 8, 16 or 32 when 4+ channels are present.");
		}
		return format; // @on
	}

}
}
