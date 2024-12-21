using System.Text;
using SharpGDX.Shims;

namespace SharpGDX.MP3;

/**
 * Class for extracting information from a frame header.
 */
public sealed class Header {
	public static readonly int[][] frequencies = { new int[]{22050, 24000, 16000, 1}, new int[] { 44100, 48000, 32000, 1}, new int[] { 11025, 12000, 8000, 1}};
	// SZD:
	// MPEG25

	/**
	 * Constant for MPEG-2 LSF version
	 */
	public const int MPEG2_LSF = 0;
	public const int MPEG25_LSF = 2; // SZD

	/**
	 * Constant for MPEG-1 version
	 */
	public const int MPEG1 = 1;

	public const int STEREO = 0;
	public const int JOINT_STEREO = 1;
	public const int DUAL_CHANNEL = 2;
	public const int SINGLE_CHANNEL = 3;
	public const int FOURTYFOUR_POINT_ONE = 0;
	public const int FOURTYEIGHT = 1;
	public const int THIRTYTWO = 2;

	private int h_layer, h_protection_bit, h_bitrate_index, h_padding_bit, h_mode_extension;
	private int h_version;
	private int h_mode;
	private int h_sample_frequency;
	private int h_number_of_subbands, h_intensity_stereo_bound;
	private bool h_copyright, h_original;
	private double[] h_vbr_time_per_frame = {-1, 384, 1152, 1152};
	private bool h_vbr;
	private int h_vbr_frames;
	private int h_vbr_scale;
	private int h_vbr_bytes;
	private byte[] h_vbr_toc;

	private byte syncmode = Bitstream.INITIAL_SYNC;
	private Crc16? crc;

	public short checksum;
	public int framesize;
	public int nSlots;

	private int _headerstring = -1;

internal	Header () {
	}

	public String toString () {
		StringBuilder buffer = new StringBuilder(200);
		buffer.Append("Layer ");
		buffer.Append(layer_string());
		buffer.Append(" frame ");
		buffer.Append(mode_string());
		buffer.Append(' ');
		buffer.Append(version_string());
		if (!checksums()) buffer.Append(" no");
		buffer.Append(" checksums");
		buffer.Append(' ');
		buffer.Append(sample_frequency_string());
		buffer.Append(',');
		buffer.Append(' ');
		buffer.Append(bitrate_string());

		String s = buffer.ToString();
		return s;
	}

	/**
	 * Read a 32-bit header from the bitstream.
	 */
	internal void read_header (Bitstream stream, Crc16?[] crcp)  {
		int headerstring;
		int channel_bitrate;
		bool sync = false;
		do {
			headerstring = stream.syncHeader(syncmode);
			_headerstring = headerstring; // E.B
			if (syncmode == Bitstream.INITIAL_SYNC) {
				h_version = headerstring >>> 19 & 1;
				if ((headerstring >>> 20 & 1) == 0) // SZD: MPEG2.5 detection
					if (h_version == MPEG2_LSF)
						h_version = MPEG25_LSF;
					else
						throw stream.newBitstreamException(Bitstream.UNKNOWN_ERROR);
				if ((h_sample_frequency = headerstring >>> 10 & 3) == 3) throw stream.newBitstreamException(Bitstream.UNKNOWN_ERROR);
			}
			h_layer = 4 - (headerstring >>> 17) & 3;
			h_protection_bit = headerstring >>> 16 & 1;
			h_bitrate_index = headerstring >>> 12 & 0xF;
			h_padding_bit = headerstring >>> 9 & 1;
			h_mode = headerstring >>> 6 & 3;
			h_mode_extension = headerstring >>> 4 & 3;
			if (h_mode == JOINT_STEREO)
				h_intensity_stereo_bound = (h_mode_extension << 2) + 4;
			else
				h_intensity_stereo_bound = 0; // should never be used
			if ((headerstring >>> 3 & 1) == 1) h_copyright = true;
			if ((headerstring >>> 2 & 1) == 1) h_original = true;
			// calculate number of subbands:
			if (h_layer == 1)
				h_number_of_subbands = 32;
			else {
				channel_bitrate = h_bitrate_index;
				// calculate bitrate per channel:
				if (h_mode != SINGLE_CHANNEL) if (channel_bitrate == 4)
					channel_bitrate = 1;
				else
					channel_bitrate -= 4;
				if (channel_bitrate == 1 || channel_bitrate == 2)
					if (h_sample_frequency == THIRTYTWO)
						h_number_of_subbands = 12;
					else
						h_number_of_subbands = 8;
				else if (h_sample_frequency == FOURTYEIGHT || channel_bitrate >= 3 && channel_bitrate <= 5)
					h_number_of_subbands = 27;
				else
					h_number_of_subbands = 30;
			}
			if (h_intensity_stereo_bound > h_number_of_subbands) h_intensity_stereo_bound = h_number_of_subbands;
			// calculate framesize and nSlots
			calculate_framesize();
			// read framedata:
			int framesizeloaded = stream.read_frame_data(framesize);
			if (framesize >= 0 && framesizeloaded != framesize) // Data loaded does not match to expected framesize,
				// it might be an ID3v1 TAG. (Fix 11/17/04).
				throw stream.newBitstreamException(Bitstream.INVALIDFRAME);
			if (stream.isSyncCurrentPosition(syncmode)) {
				if (syncmode == Bitstream.INITIAL_SYNC) {
					syncmode = Bitstream.STRICT_SYNC;
                    // TODO: Is this cast correct? -RP
                    stream.set_syncword((int)(headerstring & 0xFFF80CC0));
				}
				sync = true;
			} else
				stream.unreadFrame();
		} while (!sync);
		stream.parse_frame();
		if (h_protection_bit == 0) {
			// frame contains a crc checksum
			checksum = (short)stream.get_bits(16);
			if (crc == null) crc = new Crc16();
			crc.add_bits(headerstring, 16);
			crcp[0] = crc;
		} else
			crcp[0] = null;
		if (h_sample_frequency == FOURTYFOUR_POINT_ONE) {
			/*
			 * if (offset == null) { int max = max_number_of_frames(stream); offset = new int[max]; for(int i=0; i<max; i++)
			 * offset[i] = 0; } // E.B : Investigate more int cf = stream.current_frame(); int lf = stream.last_frame(); if ((cf > 0)
			 * && (cf == lf)) { offset[cf] = offset[cf-1] + h_padding_bit; } else { offset[0] = h_padding_bit; }
			 */
		}
	}

	/**
	 * Parse frame to extract optionnal VBR frame.
	 * @param firstframe
	 * @author E.B (javalayer@javazoom.net)
	 */
	internal void parseVBR (byte[] firstframe) {
		// Trying Xing header.
		String xing = "Xing";
		byte[] tmp = new byte[4];
		int offset = 0;
		// Compute "Xing" offset depending on MPEG version and channels.
		if (h_version == MPEG1) {
			if (h_mode == SINGLE_CHANNEL)
				offset = 21 - 4;
			else
				offset = 36 - 4;
		} else if (h_mode == SINGLE_CHANNEL)
			offset = 13 - 4;
		else
			offset = 21 - 4;
		try {
			Array.Copy(firstframe, offset, tmp, 0, 4);
			// Is "Xing" ?
			// TODO: Is this the correct encoding? -RP
			if (xing.Equals(Encoding.ASCII.GetString(tmp))) {
				// Yes.
				h_vbr = true;
				h_vbr_frames = -1;
				h_vbr_bytes = -1;
				h_vbr_scale = -1;
				h_vbr_toc = new byte[100];
                
                int length = 4;
				// Read flags.
				byte[] flags = new byte[4];
				Array.Copy(firstframe, offset + length, flags, 0, flags.Length);
				length += flags.Length;
				// Read number of frames (if available).
				if ((flags[3] & (byte)(1 << 0)) != 0) {
					Array.Copy(firstframe, offset + length, tmp, 0, tmp.Length);
					// TODO: This cast to an int is just a brute force, no clue what should really happen here.
					h_vbr_frames = (int)(tmp[0] << 24 & 0xFF000000 | tmp[1] << 16 & 0x00FF0000 | tmp[2] << 8 & 0x0000FF00 | tmp[3]
						& 0x000000FF);
					length += 4;
				}
				// Read size (if available).
				if ((flags[3] & (byte)(1 << 1)) != 0) {
					Array.Copy(firstframe, offset + length, tmp, 0, tmp.Length);
                    // TODO: This cast to an int is just a brute force, no clue what should really happen here.
                    h_vbr_bytes = (int)(tmp[0] << 24 & 0xFF000000 | tmp[1] << 16 & 0x00FF0000 | tmp[2] << 8 & 0x0000FF00 | tmp[3]
						& 0x000000FF);
					length += 4;
				}
				// Read TOC (if available).
				if ((flags[3] & (byte)(1 << 2)) != 0) {
					Array.Copy(firstframe, offset + length, h_vbr_toc, 0, h_vbr_toc.Length);
					length += h_vbr_toc.Length;
				}
				// Read scale (if available).
				if ((flags[3] & (byte)(1 << 3)) != 0) {
					Array.Copy(firstframe, offset + length, tmp, 0, tmp.Length);
                    // TODO: This cast to an int is just a brute force, no clue what should really happen here.
                    h_vbr_scale = (int)(tmp[0] << 24 & 0xFF000000 | tmp[1] << 16 & 0x00FF0000 | tmp[2] << 8 & 0x0000FF00 | tmp[3]
						& 0x000000FF);
					length += 4;
				}
				// System.out.println("VBR:"+xing+" Frames:"+ h_vbr_frames +" Size:"+h_vbr_bytes);
			}
		} catch (ArrayIndexOutOfBoundsException e) {
			throw new BitstreamException("XingVBRHeader Corrupted", e);
		}

		// Trying VBRI header.
		String vbri = "VBRI";
		offset = 36 - 4;
		try {
			Array.Copy(firstframe, offset, tmp, 0, 4);
            // Is "VBRI" ?
            // TODO: Is this the correct encoding? -RP
            if (vbri.Equals(Encoding.ASCII.GetString(tmp))) {
				// Yes.
				h_vbr = true;
				h_vbr_frames = -1;
				h_vbr_bytes = -1;
				h_vbr_scale = -1;
				h_vbr_toc = new byte[100];
				// Bytes.
				int length = 4 + 6;
				Array.Copy(firstframe, offset + length, tmp, 0, tmp.Length);
                // TODO: This cast to an int is just a brute force, no clue what should really happen here.
                h_vbr_bytes = (int)(tmp[0] << 24 & 0xFF000000 | tmp[1] << 16 & 0x00FF0000 | tmp[2] << 8 & 0x0000FF00 | tmp[3] & 0x000000FF);
				length += 4;
				// Frames.
				Array.Copy(firstframe, offset + length, tmp, 0, tmp.Length);
                // TODO: This cast to an int is just a brute force, no clue what should really happen here.
                h_vbr_frames = (int)(tmp[0] << 24 & 0xFF000000 | tmp[1] << 16 & 0x00FF0000 | tmp[2] << 8 & 0x0000FF00 | tmp[3] & 0x000000FF);
				length += 4;
				// System.out.println("VBR:"+vbri+" Frames:"+ h_vbr_frames +" Size:"+h_vbr_bytes);
				// TOC
				// TODO
			}
		} catch (ArrayIndexOutOfBoundsException e) {
			throw new BitstreamException("VBRIVBRHeader Corrupted", e);
		}
	}

	// Functions to query header contents:
	/**
	 * Returns version.
	 */
	public int version () {
		return h_version;
	}

	/**
	 * Returns Layer ID.
	 */
	public int layer () {
		return h_layer;
	}

	/**
	 * Returns bitrate index.
	 */
	public int bitrate_index () {
		return h_bitrate_index;
	}

	/**
	 * Returns Sample Frequency.
	 */
	public int sample_frequency () {
		return h_sample_frequency;
	}

	/**
	 * Returns Frequency.
	 */
	public int frequency () {
		return frequencies[h_version][h_sample_frequency];
	}

	/**
	 * Returns Mode.
	 */
	public int mode () {
		return h_mode;
	}

	/**
	 * Returns Protection bit.
	 */
	public bool checksums () {
		if (h_protection_bit == 0)
			return true;
		else
			return false;
	}

	/**
	 * Returns Copyright.
	 */
	public bool copyright () {
		return h_copyright;
	}

	/**
	 * Returns Original.
	 */
	public bool original () {
		return h_original;
	}

	/**
	 * Return VBR.
	 * @return true if VBR header is found
	 */
	public bool vbr () {
		return h_vbr;
	}

	/**
	 * Return VBR scale.
	 * @return scale of -1 if not available
	 */
	public int vbr_scale () {
		return h_vbr_scale;
	}

	/**
	 * Return VBR TOC.
	 * @return vbr toc ot null if not available
	 */
	public byte[] vbr_toc () {
		return h_vbr_toc;
	}

	/**
	 * Returns Checksum flag. Compares computed checksum with stream checksum.
	 */
	public bool checksum_ok () {
		return checksum == crc.checksum();
	}

	// Seeking and layer III stuff
	/**
	 * Returns Layer III Padding bit.
	 */
	public bool padding () {
		if (h_padding_bit == 0)
			return false;
		else
			return true;
	}

	/**
	 * Returns Slots.
	 */
	public int slots () {
		return nSlots;
	}

	/**
	 * Returns Mode Extension.
	 */
	public int mode_extension () {
		return h_mode_extension;
	}

	private static readonly int[][][] bitrates = {
		new int[][]{
            new int[]{0 /* free format */, 32000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000, 160000, 176000, 192000, 224000,
				256000, 0},
            new int[]{0 /* free format */, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000,
				160000, 0},
            new int[]{0 /* free format */, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000,
				160000, 0}},

        new int[][]{
            new int[]{0 /* free format */, 32000, 64000, 96000, 128000, 160000, 192000, 224000, 256000, 288000, 320000, 352000, 384000,
				416000, 448000, 0},
            new int[]{0 /* free format */, 32000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 160000, 192000, 224000, 256000, 320000,
				384000, 0},
            new int[]{0 /* free format */, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 160000, 192000, 224000, 256000,
				320000, 0}},
		// SZD: MPEG2.5
        new int[][]{
            new int[]{0 /* free format */, 32000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000, 160000, 176000, 192000, 224000,
				256000, 0},
            new int[]{0 /* free format */, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000,
				160000, 0},
            new int[]{0 /* free format */, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000,
				160000, 0}},

	};

	// E.B -> private to public
	/**
	 * Calculate Frame size. Calculates framesize in bytes excluding header size.
	 */
	public int calculate_framesize () {

		if (h_layer == 1) {
			framesize = 12 * bitrates[h_version][0][h_bitrate_index] / frequencies[h_version][h_sample_frequency];
			if (h_padding_bit != 0) framesize++;
			framesize <<= 2; // one slot is 4 bytes long
			nSlots = 0;
		} else {
			framesize = 144 * bitrates[h_version][h_layer - 1][h_bitrate_index] / frequencies[h_version][h_sample_frequency];
			if (h_version == MPEG2_LSF || h_version == MPEG25_LSF) framesize >>= 1; // SZD
			if (h_padding_bit != 0) framesize++;
			// Layer III slots
			if (h_layer == 3) {
				if (h_version == MPEG1)
					nSlots = framesize - (h_mode == SINGLE_CHANNEL ? 17 : 32) // side info size
						- (h_protection_bit != 0 ? 0 : 2) // CRC size
						- 4; // header size
				else
					nSlots = framesize - (h_mode == SINGLE_CHANNEL ? 9 : 17) // side info size
						- (h_protection_bit != 0 ? 0 : 2) // CRC size
						- 4; // header size
			} else
				nSlots = 0;
		}
		framesize -= 4; // subtract header size
		return framesize;
	}

	/**
	 * Returns the maximum number of frames in the stream.
	 * @param streamsize
	 * @return number of frames
	 */
	public int max_number_of_frames (int streamsize) // E.B
	{
		if (h_vbr == true)
			return h_vbr_frames;
		else if (framesize + 4 - h_padding_bit == 0)
			return 0;
		else
			return streamsize / (framesize + 4 - h_padding_bit);
	}

	/**
	 * Returns the maximum number of frames in the stream.
	 * @param streamsize
	 * @return number of frames
	 */
	public int min_number_of_frames (int streamsize) // E.B
	{
		if (h_vbr == true)
			return h_vbr_frames;
		else if (framesize + 5 - h_padding_bit == 0)
			return 0;
		else
			return streamsize / (framesize + 5 - h_padding_bit);
	}

	/**
	 * Returns ms/frame.
	 * @return milliseconds per frame
	 */
	public float ms_per_frame () // E.B
	{
		if (h_vbr == true) {
			double tpf = h_vbr_time_per_frame[layer()] / frequency();
			if (h_version == MPEG2_LSF || h_version == MPEG25_LSF) tpf /= 2;
			return (float)(tpf * 1000);
		} else {
			float[][] ms_per_frame_array = { new float[]{8.707483f, 8.0f, 12.0f}, new float[] { 26.12245f, 24.0f, 36.0f}, new float[] { 26.12245f, 24.0f, 36.0f}};
			return ms_per_frame_array[h_layer - 1][h_sample_frequency];
		}
	}

	/**
	 * Returns total ms.
	 * @param streamsize
	 * @return total milliseconds
	 */
	public float total_ms (int streamsize) // E.B
	{
		return max_number_of_frames(streamsize) * ms_per_frame();
	}

	/**
	 * Returns synchronized header.
	 */
	public int getSyncHeader () // E.B
	{
		return _headerstring;
	}

	// functions which return header informations as strings:
	/**
	 * Return Layer version.
	 */
	public String layer_string () {
		switch (h_layer) {
		case 1:
			return "I";
		case 2:
			return "II";
		case 3:
			return "III";
		}
		return null;
	}

	// E.B -> private to public
	private static readonly String[][][] bitrate_str = {
        new string[][]{
			new string[]{"free format", "32 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s", "96 kbit/s", "112 kbit/s",
				"128 kbit/s", "144 kbit/s", "160 kbit/s", "176 kbit/s", "192 kbit/s", "224 kbit/s", "256 kbit/s", "forbidden"},
            new string[]{"free format", "8 kbit/s", "16 kbit/s", "24 kbit/s", "32 kbit/s", "40 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s",
				"80 kbit/s", "96 kbit/s", "112 kbit/s", "128 kbit/s", "144 kbit/s", "160 kbit/s", "forbidden"},
            new string[]{"free format", "8 kbit/s", "16 kbit/s", "24 kbit/s", "32 kbit/s", "40 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s",
				"80 kbit/s", "96 kbit/s", "112 kbit/s", "128 kbit/s", "144 kbit/s", "160 kbit/s", "forbidden"}},

        new string[][]{
            new string[]{"free format", "32 kbit/s", "64 kbit/s", "96 kbit/s", "128 kbit/s", "160 kbit/s", "192 kbit/s", "224 kbit/s",
				"256 kbit/s", "288 kbit/s", "320 kbit/s", "352 kbit/s", "384 kbit/s", "416 kbit/s", "448 kbit/s", "forbidden"},
            new string[]{"free format", "32 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s", "96 kbit/s", "112 kbit/s",
				"128 kbit/s", "160 kbit/s", "192 kbit/s", "224 kbit/s", "256 kbit/s", "320 kbit/s", "384 kbit/s", "forbidden"},
            new string[]{"free format", "32 kbit/s", "40 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s", "96 kbit/s", "112 kbit/s",
				"128 kbit/s", "160 kbit/s", "192 kbit/s", "224 kbit/s", "256 kbit/s", "320 kbit/s", "forbidden"}},
		// SZD: MPEG2.5
        new string[][]{
            new string[]{"free format", "32 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s", "96 kbit/s", "112 kbit/s",
				"128 kbit/s", "144 kbit/s", "160 kbit/s", "176 kbit/s", "192 kbit/s", "224 kbit/s", "256 kbit/s", "forbidden"},
            new string[]{"free format", "8 kbit/s", "16 kbit/s", "24 kbit/s", "32 kbit/s", "40 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s",
				"80 kbit/s", "96 kbit/s", "112 kbit/s", "128 kbit/s", "144 kbit/s", "160 kbit/s", "forbidden"},
            new string[]{"free format", "8 kbit/s", "16 kbit/s", "24 kbit/s", "32 kbit/s", "40 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s",
				"80 kbit/s", "96 kbit/s", "112 kbit/s", "128 kbit/s", "144 kbit/s", "160 kbit/s", "forbidden"}},};

	/**
	 * Return Bitrate.
	 * @return bitrate in bps
	 */
	public String bitrate_string () {
		if (h_vbr == true)
			return (bitrate() / 1000) + " kb/s";
		else
			return bitrate_str[h_version][h_layer - 1][h_bitrate_index];
	}

	/**
	 * Return Bitrate.
	 * @return bitrate in bps and average bitrate for VBR header
	 */
	public int bitrate () {
		if (h_vbr == true)
			return (int)(h_vbr_bytes * 8 / (ms_per_frame() * h_vbr_frames)) * 1000;
		else
			return bitrates[h_version][h_layer - 1][h_bitrate_index];
	}

	/**
	 * Return Instant Bitrate. Bitrate for VBR is not constant.
	 * @return bitrate in bps
	 */
	public int bitrate_instant () {
		return bitrates[h_version][h_layer - 1][h_bitrate_index];
	}

	/**
	 * Returns Frequency
	 * @return frequency string in kHz
	 */
	public String sample_frequency_string () {
		switch (h_sample_frequency) {
		case THIRTYTWO:
			if (h_version == MPEG1)
				return "32 kHz";
			else if (h_version == MPEG2_LSF)
				return "16 kHz";
			else
				// SZD
				return "8 kHz";
		case FOURTYFOUR_POINT_ONE:
			if (h_version == MPEG1)
				return "44.1 kHz";
			else if (h_version == MPEG2_LSF)
				return "22.05 kHz";
			else
				// SZD
				return "11.025 kHz";
		case FOURTYEIGHT:
			if (h_version == MPEG1)
				return "48 kHz";
			else if (h_version == MPEG2_LSF)
				return "24 kHz";
			else
				// SZD
				return "12 kHz";
		}
		return null;
	}

	public int getSampleRate () {
		switch (h_sample_frequency) {
		case THIRTYTWO:
			if (h_version == MPEG1)
				return 32000;
			else if (h_version == MPEG2_LSF)
				return 16000;
			else
				// SZD
				return 8000;
		case FOURTYFOUR_POINT_ONE:
			if (h_version == MPEG1)
				return 44100;
			else if (h_version == MPEG2_LSF)
				return 22050;
			else
				// SZD
				return 11025;
		case FOURTYEIGHT:
			if (h_version == MPEG1)
				return 48000;
			else if (h_version == MPEG2_LSF)
				return 24000;
			else
				// SZD
				return 12000;
		}
		return 0;
	}

	/**
	 * Returns Mode.
	 */
	public String mode_string () {
		switch (h_mode) {
		case STEREO:
			return "Stereo";
		case JOINT_STEREO:
			return "Joint stereo";
		case DUAL_CHANNEL:
			return "Dual channel";
		case SINGLE_CHANNEL:
			return "Single channel";
		}
		return null;
	}

	/**
	 * Returns Version.
	 * @return MPEG-1 or MPEG-2 LSF or MPEG-2.5 LSF
	 */
	public String version_string () {
		switch (h_version) {
		case MPEG1:
			return "MPEG-1";
		case MPEG2_LSF:
			return "MPEG-2 LSF";
		case MPEG25_LSF: // SZD
			return "MPEG-2.5 LSF";
		}
		return null;
	}

	/**
	 * Returns the number of subbands in the current frame.
	 * @return number of subbands
	 */
	public int number_of_subbands () {
		return h_number_of_subbands;
	}

	/**
	 * Returns Intensity Stereo. (Layer II joint stereo only). Returns the number of subbands which are in stereo mode, subbands
	 * above that limit are in intensity stereo mode.
	 * @return intensity
	 */
	public int intensity_stereo_bound () {
		return h_intensity_stereo_bound;
	}
}
