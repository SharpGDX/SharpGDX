using System.Text;
using SharpGDX.Shims;

namespace SharpGDX.MP3;

/**
 * The <code>Bistream</code> class is responsible for parsing an MPEG audio bitstream.
 * 
 * <b>REVIEW:</b> much of the parsing currently occurs in the various decoders. This should be moved into this class and
 * associated inner classes.
 */
public sealed class Bitstream {
	/**
	 * Synchronization control constant for the initial synchronization to the start of a frame.
	 */
	internal static byte INITIAL_SYNC = 0;

	/**
	 * Synchronization control constant for non-initial frame synchronizations.
	 */
	internal static byte STRICT_SYNC = 1;

	// max. 1730 bytes per frame: 144 * 384kbit/s / 32000 Hz + 2 Bytes CRC
	/**
	 * Maximum size of the frame buffer.
	 */
	private static readonly int BUFFER_INT_SIZE = 433;

	/**
	 * The frame buffer that holds the data for the current frame.
	 */
	private readonly int[] framebuffer = new int[BUFFER_INT_SIZE];

	/**
	 * Number of valid bytes in the frame buffer.
	 */
	private int framesize;

	/**
	 * The bytes read from the stream.
	 */
	private byte[] frame_bytes = new byte[BUFFER_INT_SIZE * 4];

	/**
	 * Index into <code>framebuffer</code> where the next bits are retrieved.
	 */
	private int wordpointer;

	/**
	 * Number (0-31, from MSB to LSB) of next bit for get_bits()
	 */
	private int bitindex;

	/**
	 * The current specified syncword
	 */
	private int syncword;

	/**
	 * Audio header position in stream.
	 */
	private int _header_pos = 0;

	private float replayGainScale;

	/**
	 *
	 */
	private bool single_ch_mode;
	// private int current_frame_number;
	// private int last_frame_number;

	private readonly int[] bitmask = {
		0, // dummy
		0x00000001, 0x00000003, 0x00000007, 0x0000000F, 0x0000001F, 0x0000003F, 0x0000007F, 0x000000FF, 0x000001FF, 0x000003FF,
		0x000007FF, 0x00000FFF, 0x00001FFF, 0x00003FFF, 0x00007FFF, 0x0000FFFF, 0x0001FFFF};

	private readonly PushbackInputStream source;

	private readonly Header header = new Header();

	private readonly byte[] syncbuf = new byte[4];

	private Crc16[] crc = new Crc16[1];

	private byte[]? rawid3v2 = null;

	private bool firstframe = true;

	/**
	 * Construct a IBitstream that reads data from a given InputStream.
	 * 
	 * @param in The InputStream to read from.
	 */
	public Bitstream (InputStream? @in) {
		if (@in == null) throw new NullPointerException("in");
		@in = new BufferedInputStream(@in);
		loadID3v2(@in);
		firstframe = true;
		// source = new PushbackInputStream(in, 1024);
		source = new PushbackInputStream(@in, BUFFER_INT_SIZE * 4);

		closeFrame();
		// current_frame_number = -1;
		// last_frame_number = -1;
	}

	/**
	 * Return position of the first audio header.
	 * @return size of ID3v2 tag frames.
	 */
	public int header_pos () {
		return _header_pos;
	}

	/**
	 * Load ID3v2 frames.
	 * @param in MP3 InputStream.
	 * @author JavaZOOM
	 */
	private void loadID3v2 (InputStream @in) {
		int size = -1;
		try {
			// Read ID3v2 header (10 bytes).
			@in.mark(10);
			size = readID3v2Header(@in);
			_header_pos = size;
		} catch (IOException e) {
		} finally {
			try {
				// Unread ID3v2 header (10 bytes).
				@in.reset();
			} catch (IOException e) {
			}
		}
		// Load ID3v2 tags.
		try {
			if (size > 0) {
				rawid3v2 = new byte[size];
				@in.read(rawid3v2, 0, rawid3v2.Length);
				parseID3v2Frames(rawid3v2);
			}
		} catch (IOException e) {
		}
	}

	/**
	 * Parse ID3v2 tag header to find out size of ID3v2 frames.
	 * @param in MP3 InputStream
	 * @return size of ID3v2 frames + header
	 * @throws IOException
	 * @author JavaZOOM
	 */
	private int readID3v2Header (InputStream @in) {
		byte[] id3header = new byte[4];
		int size = -10;
		@in.read(id3header, 0, 3);
		// Look for ID3v2
		if (id3header[0] == 'I' && id3header[1] == 'D' && id3header[2] == '3') {
			@in.read(id3header, 0, 3);
			@in.read(id3header, 0, 4);
			size = (id3header[0] << 21) + (id3header[1] << 14) + (id3header[2] << 7) + id3header[3];
		}
		return size + 10;
	}

	/**
	 * Return raw ID3v2 frames + header.
	 * @return ID3v2 InputStream or null if ID3v2 frames are not available.
	 */
	public InputStream getRawID3v2 () {
		if (rawid3v2 == null)
			return null;
		else {
			ByteArrayInputStream bain = new ByteArrayInputStream(rawid3v2);
			return bain;
		}
	}

	private void parseID3v2Frames (byte[]? bframes) {
		if (bframes == null) return;
		// TODO: Is this the right encoding? -RP
        var x = Encoding.ASCII.GetString(bframes, 0, 3);

        if (!"ID3".Equals(Encoding.ASCII.GetString(bframes, 0, 3))) return;
		int v2version = (int)(bframes[3] & 0xFF);
		if (v2version < 2 || v2version > 4) {
			return;
		}
		try {
			float? replayGain = null, replayGainPeak = null;
			int size;
			String value = null;
			for (int i = 10; i < bframes.Length && bframes[i] > 0; i += size) {
				if (v2version == 3 || v2version == 4) {
                    // ID3v2.3 & ID3v2.4
                    // TODO: Is this the right encoding? -RP
                    String code = Encoding.ASCII.GetString(bframes, i, 4);
					size = (int)(bframes[i + 4] << 24 & 0xFF000000 | bframes[i + 5] << 16 & 0x00FF0000 | bframes[i + 6] << 8
						& 0x0000FF00 | bframes[i + 7] & 0x000000FF);
					i += 10;
					if (code.Equals("TXXX")) {
						value = parseText(bframes, i, size, 1);
						String[] values = value.Split("\0");
						if (values.Length == 2) {
							String name = values[0];
							value = values[1];
							if (name.Equals("replaygain_track_peak")) {
								replayGainPeak = float.Parse(value);
								if (replayGain != null) break;
							} else if (name.Equals("replaygain_track_gain")) {
								replayGain = float.Parse(value.Replace(" dB", "")) + 3;
								if (replayGainPeak != null) break;
							}
						}
					}
				} else {
                    // ID3v2.2
                    // TODO: Is this the right encoding? -RP
                    String scode = Encoding.ASCII.GetString(bframes, i, 3);
					size = (int)0x00000000 + (bframes[i + 3] << 16) + (bframes[i + 4] << 8) + bframes[i + 5];
					i += 6;
					if (scode.Equals("TXXX")) {
						value = parseText(bframes, i, size, 1);
						String[] values = value.Split("\0");
						if (values.Length == 2) {
							String name = values[0];
							value = values[1];
							if (name.Equals("replaygain_track_peak")) {
								replayGainPeak = float.Parse(value);
								if (replayGain != null) break;
							} else if (name.Equals("replaygain_track_gain")) {
								replayGain = float.Parse(value.Replace(" dB", "")) + 3;
								if (replayGainPeak != null) break;
							}
						}
					}
				}
			}
			if (replayGain != null && replayGainPeak != null) {
				replayGainScale = (float)Math.Pow(10, replayGain.Value / 20f);
				// If scale * peak > 1 then reduce scale (preamp) to prevent clipping.
				replayGainScale = Math.Min(1 / replayGainPeak.Value, replayGainScale);
			}
		} catch (RuntimeException ignored) {
		}
	}

	private String parseText (byte[] bframes, int offset, int size, int skip) {
		String value = null;
		try {
			String[] ENC_TYPES = {"ISO-8859-1", "UTF16", "UTF-16BE", "UTF-8"};
            // TODO: This `should` be correct in terms of selecting the correct encoding. This also matches what Java's new String(byte[], int, int) says it does. -RP
            value = Encoding.GetEncoding(ENC_TYPES[bframes[offset]]).GetString(bframes, offset + skip, size - skip);
        } catch (UnsupportedEncodingException e) {
		}
		return value;
	}

	public float getReplayGainScale () {
		return replayGainScale;
	}

	/**
	 * Close the Bitstream.
	 * @throws BitstreamException
	 */
	public void close ()  {
		try {
			source.close();
		} catch (IOException ex) {
			throw newBitstreamException(STREAM_ERROR, ex);
		}
	}

	/**
	 * Reads and parses the next frame from the input source.
	 * @return the Header describing details of the frame read, or null if the end of the stream has been reached.
	 */
	public Header readFrame ()  {
		Header? result = null;
		try {
			result = readNextFrame();
			// E.B, Parse VBR (if any) first frame.
			if (firstframe == true) {
				result.parseVBR(frame_bytes);
				firstframe = false;
			}
		} catch (BitstreamException ex) {
			if (ex.getErrorCode() == INVALIDFRAME)
				// Try to skip this frame.
				// System.out.println("INVALIDFRAME");
				try {
					closeFrame();
					result = readNextFrame();
				} catch (BitstreamException e) {
					if (e.getErrorCode() != STREAM_EOF) // wrap original exception so stack trace is maintained.
						throw newBitstreamException(e.getErrorCode(), e);
				}
			else if (ex.getErrorCode() != STREAM_EOF) // wrap original exception so stack trace is maintained.
				throw newBitstreamException(ex.getErrorCode(), ex);
		}
		return result;
	}

	/**
	 * Read next MP3 frame.
	 * @return MP3 frame header.
	 * @throws BitstreamException
	 */
	private Header readNextFrame ()  {
		if (framesize == -1) nextFrame();
		return header;
	}

	/**
	 * Read next MP3 frame.
	 * @throws BitstreamException
	 */
	private void nextFrame ()  {
		// entire frame is read by the header class.
		header.read_header(this, crc);
	}

	/**
	 * Unreads the bytes read from the frame.
	 * @throws BitstreamException
	 */
	// REVIEW: add new error codes for this.
	public void unreadFrame ()  {
		if (wordpointer == -1 && bitindex == -1 && framesize > 0) try {
			source.unread(frame_bytes, 0, framesize);
		} catch (IOException ex) {
			throw newBitstreamException(STREAM_ERROR);
		}
	}

	/**
	 * Close MP3 frame.
	 */
	public void closeFrame () {
		framesize = -1;
		wordpointer = -1;
		bitindex = -1;
	}

	/**
	 * Determines if the next 4 bytes of the stream represent a frame header.
	 */
	public bool isSyncCurrentPosition (int syncmode)  {
		int read = readBytes(syncbuf, 0, 4);
        // TODO: Is this cast correct? -RP
        int headerstring = (int)(syncbuf[0] << 24 & 0xFF000000 | syncbuf[1] << 16 & 0x00FF0000 | syncbuf[2] << 8 & 0x0000FF00
			| syncbuf[3] << 0 & 0x000000FF);

		try {
			source.unread(syncbuf, 0, read);
		} catch (IOException ex) {
		}

		bool sync = false;
		switch (read) {
		case 0:
			sync = true;
			break;
		case 4:
			sync = isSyncMark(headerstring, syncmode, syncword);
			break;
		}

		return sync;
	}

	// REVIEW: this class should provide inner classes to
	// parse the frame contents. Eventually, readBits will
	// be removed.
	public int readBits (int n) {
		return get_bits(n);
	}

	public int readCheckedBits (int n) {
		// REVIEW: implement CRC check.
		return get_bits(n);
	}

	internal protected BitstreamException newBitstreamException (int errorcode) {
		return new BitstreamException(errorcode, null);
	}

	protected BitstreamException newBitstreamException (int errorcode, Exception throwable) {
		return new BitstreamException(errorcode, throwable);
	}

	/**
	 * Get next 32 bits from bitstream. They are stored in the headerstring. syncmod allows Synchro flag ID The returned value is
	 * False at the end of stream.
	 */

	internal int syncHeader (byte syncmode)  {
		bool sync;
		int headerstring;
		// read additional 2 bytes
		int bytesRead = readBytes(syncbuf, 0, 3);

		if (bytesRead != 3) throw newBitstreamException(STREAM_EOF, null);

		headerstring = syncbuf[0] << 16 & 0x00FF0000 | syncbuf[1] << 8 & 0x0000FF00 | syncbuf[2] << 0 & 0x000000FF;

		do {
			headerstring <<= 8;

			if (readBytes(syncbuf, 3, 1) != 1) throw newBitstreamException(STREAM_EOF, null);

			headerstring |= syncbuf[3] & 0x000000FF;

			sync = isSyncMark(headerstring, syncmode, syncword);
		} while (!sync);

		// current_frame_number++;
		// if (last_frame_number < current_frame_number) last_frame_number = current_frame_number;

		return headerstring;
	}

	public bool isSyncMark (int headerstring, int syncmode, int word) {
		bool sync = false;

		if (syncmode == INITIAL_SYNC) // sync = ((headerstring & 0xFFF00000) == 0xFFF00000);
        {
            sync = (headerstring & 0xFFE00000) == 0xFFE00000; // SZD: MPEG 2.5
}
        else
        {
            sync = (int)(headerstring & 0xFFF80C00) == word && (headerstring & 0x000000C0) == 0x000000C0 == single_ch_mode;
        }

		// filter out invalid sample rate
		if (sync) sync = (headerstring >>> 10 & 3) != 3;
		// filter out invalid layer
		if (sync) sync = (headerstring >>> 17 & 3) != 0;
		// filter out invalid version
		if (sync) sync = (headerstring >>> 19 & 3) != 1;

		return sync;
	}

	/**
	 * Reads the data for the next frame. The frame is not parsed until parse frame is called.
	 */
	internal int read_frame_data (int bytesize)  {
		int numread = 0;
		numread = readFully(frame_bytes, 0, bytesize);
		framesize = bytesize;
		wordpointer = -1;
		bitindex = -1;
		return numread;
	}

	/**
	 * Parses the data previously read with read_frame_data().
	 */
	internal void parse_frame ()  {
		// Convert Bytes read to int
		int b = 0;
		byte[] byteread = frame_bytes;
		int bytesize = framesize;

		// Check ID3v1 TAG (True only if last frame).
		// for (int t=0;t<(byteread.length)-2;t++)
		// {
		// if ((byteread[t]=='T') && (byteread[t+1]=='A') && (byteread[t+2]=='G'))
		// {
		// System.out.println("ID3v1 detected at offset "+t);
		// throw newBitstreamException(INVALIDFRAME, null);
		// }
		// }

		for (int k = 0; k < bytesize; k = k + 4) {
			byte b0 = 0;
			byte b1 = 0;
			byte b2 = 0;
			byte b3 = 0;
			b0 = byteread[k];
			if (k + 1 < bytesize) b1 = byteread[k + 1];
			if (k + 2 < bytesize) b2 = byteread[k + 2];
			if (k + 3 < bytesize) b3 = byteread[k + 3];
            // TODO: Is this cast correct? -RP
            framebuffer[b++] = (int)(b0 << 24 & 0xFF000000 | b1 << 16 & 0x00FF0000 | b2 << 8 & 0x0000FF00 | b3 & 0x000000FF);
		}
		wordpointer = 0;
		bitindex = 0;
	}

	/**
	 * Read bits from buffer into the lower bits of an unsigned int. The LSB contains the latest read bit of the stream. (1 <=
	 * number_of_bits <= 16)
	 */
	public int get_bits (int number_of_bits) {
		int returnvalue = 0;
		int sum = bitindex + number_of_bits;

		// E.B
		// There is a problem here, wordpointer could be -1 ?!
		if (wordpointer < 0) wordpointer = 0;
		// E.B : End.

		if (sum <= 32) {
			// all bits contained in *wordpointer
			returnvalue = framebuffer[wordpointer] >>> 32 - sum & bitmask[number_of_bits];
			// returnvalue = (wordpointer[0] >> (32 - sum)) & bitmask[number_of_bits];
			if ((bitindex += number_of_bits) == 32) {
				bitindex = 0;
				wordpointer++; // added by me!
			}
			return returnvalue;
		}

		// E.B : Check that ?
		// ((short[])&returnvalue)[0] = ((short[])wordpointer + 1)[0];
		// wordpointer++; // Added by me!
		// ((short[])&returnvalue + 1)[0] = ((short[])wordpointer)[0];
		int Right = framebuffer[wordpointer] & 0x0000FFFF;
		wordpointer++;
		// TODO: Is this cast correct? -RP
		int Left = (int)(framebuffer[wordpointer] & 0xFFFF0000);
        // TODO: Is this cast correct? -RP
        returnvalue = (int)(Right << 16 & 0xFFFF0000 | Left >>> 16 & 0x0000FFFF);

		returnvalue >>>= 48 - sum; // returnvalue >>= 16 - (number_of_bits - (32 - bitindex))
		returnvalue &= bitmask[number_of_bits];
		bitindex = sum - 32;
		return returnvalue;
	}

	/**
	 * Set the word we want to sync the header to. In Big-Endian byte order
	 */
	internal void set_syncword (int syncword0) {
        // TODO: Is this cast correct? -RP
        syncword = (int)(syncword0 & 0xFFFFFF3F);
		single_ch_mode = (syncword0 & 0x000000C0) == 0x000000C0;
	}

	/**
	 * Reads the exact number of bytes from the source input stream into a byte array.
	 * 
	 * @param b The byte array to read the specified number of bytes into.
	 * @param offs The index in the array where the first byte read should be stored.
	 * @param len the number of bytes to read.
	 * 
	 * @exception BitstreamException is thrown if the specified number of bytes could not be read from the stream.
	 */
	private int readFully (byte[] b, int offs, int len)  {
		int nRead = 0;
		try {
			while (len > 0) {
				int bytesread = source.read(b, offs, len);
				if (bytesread == -1) {
					while (len-- > 0)
						b[offs++] = 0;
					break;
					// throw newBitstreamException(UNEXPECTED_EOF, new EOFException());
				}
				nRead = nRead + bytesread;
				offs += bytesread;
				len -= bytesread;
			}
		} catch (IOException ex) {
			throw newBitstreamException(STREAM_ERROR, ex);
		}
		return nRead;
	}

	/**
	 * Simlar to readFully, but doesn't throw exception when EOF is reached.
	 */
	private int readBytes (byte[] b, int offs, int len)  {
		int totalBytesRead = 0;
		try {
			while (len > 0) {
				int bytesread = source.read(b, offs, len);
				if (bytesread == -1) break;
				totalBytesRead += bytesread;
				offs += bytesread;
				len -= bytesread;
			}
		} catch (IOException ex) {
			throw newBitstreamException(STREAM_ERROR, ex);
		}
		return totalBytesRead;
	}

	/**
	 * The first bitstream error code. See the {@link DecoderErrors DecoderErrors} interface for other bitstream error codes.
	 */
	static public readonly int BITSTREAM_ERROR = 0x100;

	/**
	 * An undeterminable error occurred.
	 */
	static public readonly int UNKNOWN_ERROR = BITSTREAM_ERROR + 0;

	/**
	 * The header describes an unknown sample rate.
	 */
	static public readonly int UNKNOWN_SAMPLE_RATE = BITSTREAM_ERROR + 1;

	/**
	 * A problem occurred reading from the stream.
	 */
	static public readonly int STREAM_ERROR = BITSTREAM_ERROR + 2;

	/**
	 * The end of the stream was reached prematurely.
	 */
	static public readonly int UNEXPECTED_EOF = BITSTREAM_ERROR + 3;

	/**
	 * The end of the stream was reached.
	 */
	static public readonly int STREAM_EOF = BITSTREAM_ERROR + 4;

	/**
	 * Frame data are missing.
	 */
	static public readonly int INVALIDFRAME = BITSTREAM_ERROR + 5;

	/**
	 * 
	 */
	static public readonly int BITSTREAM_LAST = 0x1ff;
}
