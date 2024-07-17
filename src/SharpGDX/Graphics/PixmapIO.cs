using SharpGDX.Files;
using SharpGDX.Shims;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics
{
	/** Writes Pixmaps to various formats.
 * @author mzechner
 * @author Nathan Sweet */
public class PixmapIO {
	/** Writes the {@link Pixmap} to the given file using a custom compression scheme. First three integers define the width,
	 * height and format, remaining bytes are zlib compressed pixels. To be able to load the Pixmap to a Texture, use ".cim" as the
	 * file suffix. Throws a GdxRuntimeException in case the Pixmap couldn't be written to the file.
	 * @param file the file to write the Pixmap to */
	static public void writeCIM (FileHandle file, Pixmap pixmap) {
		CIM.write(file, pixmap);
	}

	/** Reads the {@link Pixmap} from the given file, assuming the Pixmap was written with the
	 * {@link PixmapIO#writeCIM(FileHandle, Pixmap)} method. Throws a GdxRuntimeException in case the file couldn't be read.
	 * @param file the file to read the Pixmap from */
	static public Pixmap readCIM (FileHandle file) {
		return CIM.read(file);
	}

	/** Writes the pixmap as a PNG. See {@link PNG} to write out multiple PNGs with minimal allocation.
	 * @param compression sets the deflate compression level. Default is {@link Deflater#DEFAULT_COMPRESSION}
	 * @param flipY flips the Pixmap vertically if true */
	static public void writePNG (FileHandle file, Pixmap pixmap, int compression, bool flipY) {
		// TODO:
		throw new NotImplementedException();
			//try {
			//	PNG writer = new PNG((int)(pixmap.getWidth() * pixmap.getHeight() * 1.5f)); // Guess at deflated size.
			//	try {
			//		writer.setFlipY(flipY);
			//		writer.setCompression(compression);
			//		writer.write(file, pixmap);
			//	} finally {
			//		writer.dispose();
			//	}
			//} catch (IOException ex) {
			//	throw new GdxRuntimeException("Error writing PNG: " + file, ex);
			//}
		}

	/** Writes the pixmap as a PNG with compression. See {@link PNG} to configure the compression level, more efficiently flip the
	 * pixmap vertically, and to write out multiple PNGs with minimal allocation. */
	static public void writePNG (FileHandle file, Pixmap pixmap) {
		// TODO:
		throw new NotImplementedException();
			//writePNG(file, pixmap, Deflater.DEFAULT_COMPRESSION, false);
		}

	/** @author mzechner */
	private class CIM {
		static private readonly int BUFFER_SIZE = 32000;
		static private readonly byte[] writeBuffer = new byte[BUFFER_SIZE];
		static private readonly byte[] readBuffer = new byte[BUFFER_SIZE];

		static public void write (FileHandle file, Pixmap pixmap) {
			// TODO:
			throw new NotImplementedException();
				//DataOutputStream @out = null;

				//try {
				//	DeflaterOutputStream deflaterOutputStream = new DeflaterOutputStream(file.write(false));
				//	@out = new DataOutputStream(deflaterOutputStream);
				//	@out.writeInt(pixmap.getWidth());
				//	@out.writeInt(pixmap.getHeight());
				//	@out.writeInt(Pixmap.FormatUtils.toGdx2DPixmapFormat(pixmap.getFormat()));

				//	ByteBuffer pixelBuf = pixmap.getPixels();
				//	((Buffer)pixelBuf).position(0);
				//	((Buffer)pixelBuf).limit(pixelBuf.capacity());

				//	int remainingBytes = pixelBuf.capacity() % BUFFER_SIZE;
				//	int iterations = pixelBuf.capacity() / BUFFER_SIZE;

				//	lock (writeBuffer) {
				//		for (int i = 0; i < iterations; i++) {
				//			pixelBuf.get(writeBuffer);
				//			@out.write(writeBuffer);
				//		}

				//		pixelBuf.get(writeBuffer, 0, remainingBytes);
				//		@out.write(writeBuffer, 0, remainingBytes);
				//	}

				//	((Buffer)pixelBuf).position(0);
				//	((Buffer)pixelBuf).limit(pixelBuf.capacity());
				//} catch (Exception e) {
				//	throw new GdxRuntimeException("Couldn't write Pixmap to file '" + file + "'", e);
				//} finally {
				//	StreamUtils.closeQuietly(@out);
				//}
			}

		static public Pixmap read (FileHandle file) {
			// TODO:
			throw new NotImplementedException();
				//	DataInputStream @in = null;

				//	try {
				//		@in = new DataInputStream(new InflaterInputStream(new BufferedInputStream(file.read())));
				//		int width = @in.readInt();
				//		int height = @in.readInt();
				//		Pixmap.Format format = Pixmap.FormatUtils.fromGdx2DPixmapFormat(@in.readInt());
				//		Pixmap pixmap = new Pixmap(width, height, format);

				//		ByteBuffer pixelBuf = pixmap.getPixels();
				//		((Buffer)pixelBuf).position(0);
				//		((Buffer)pixelBuf).limit(pixelBuf.capacity());

				//		lock (readBuffer) {
				//			int readBytes = 0;
				//			while ((readBytes = @in.read(readBuffer)) > 0) {
				//				pixelBuf.put(readBuffer, 0, readBytes);
				//			}
				//		}

				//		((Buffer)pixelBuf).position(0);
				//		((Buffer)pixelBuf).limit(pixelBuf.capacity());
				//		return pixmap;
				//	} catch (Exception e) {
				//		throw new GdxRuntimeException("Couldn't read Pixmap from file '" + file + "'", e);
				//	} finally {
				//		StreamUtils.closeQuietly(@in);
				//	}
				//}
			}

	/** PNG encoder with compression. An instance can be reused to encode multiple PNGs with minimal allocation.
	 *
	 * <pre>
	 * Copyright (c) 2007 Matthias Mann - www.matthiasmann.de
	 * Copyright (c) 2014 Nathan Sweet
	 *
	 * Permission is hereby granted, free of charge, to any person obtaining a copy
	 * of this software and associated documentation files (the "Software"), to deal
	 * in the Software without restriction, including without limitation the rights
	 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	 * copies of the Software, and to permit persons to whom the Software is
	 * furnished to do so, subject to the following conditions:
	 *
	 * The above copyright notice and this permission notice shall be included in
	 * all copies or substantial portions of the Software.
	 *
	 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	 * THE SOFTWARE.
	 * </pre>
	 * 
	 * @author Matthias Mann
	 * @author Nathan Sweet */
	public class PNG : Disposable {
		static private readonly byte[] SIGNATURE = {(byte)137, 80, 78, 71, 13, 10, 26, 10};
		static private readonly int IHDR = 0x49484452, IDAT = 0x49444154, IEND = 0x49454E44;
		static private readonly byte COLOR_ARGB = 6;
		static private readonly byte COMPRESSION_DEFLATE = 0;
		static private readonly byte FILTER_NONE = 0;
		static private readonly byte INTERLACE_NONE = 0;
		static private readonly byte PAETH = 4;

		//private readonly ChunkBuffer buffer;
		private readonly Deflater deflater;
		//private ByteArray lineOutBytes, curLineBytes, prevLineBytes;
		private bool flipY = true;
		private int lastLineLen;

		public PNG () 
			: this(128 * 128)
		{
			
		}

		public PNG (int initialBufferSize) {
			// TODO:
			throw new NotImplementedException();
				//buffer = new ChunkBuffer(initialBufferSize);
				//deflater = new Deflater();

			}

		/** If true, the resulting PNG is flipped vertically. Default is true. */
		public void setFlipY (bool flipY) {
			this.flipY = flipY;
		}

		/** Sets the deflate compression level. Default is {@link Deflater#DEFAULT_COMPRESSION}. */
		public void setCompression (int level) {
			// TODO:
			throw new NotImplementedException();
				//deflater.setLevel(level);
			}

		public void write (FileHandle file, Pixmap pixmap) // TODO: throws IOException
{
			OutputStream output = file.write(false);
			try {
				write(output, pixmap);
			} finally {
				StreamUtils.closeQuietly(output);
			}
		}

		/** Writes the pixmap to the stream without closing the stream. */
		public void write (OutputStream output, Pixmap pixmap)  // TODO:throws IOException 
			{
				// TODO:
				throw new NotImplementedException();
				//DeflaterOutputStream deflaterOutput = new DeflaterOutputStream(buffer, deflater);
				//DataOutputStream dataOutput = new DataOutputStream(output);
				//dataOutput.write(SIGNATURE);

				//buffer.writeInt(IHDR);
				//buffer.writeInt(pixmap.getWidth());
				//buffer.writeInt(pixmap.getHeight());
				//buffer.writeByte(8); // 8 bits per component.
				//buffer.writeByte(COLOR_ARGB);
				//buffer.writeByte(COMPRESSION_DEFLATE);
				//buffer.writeByte(FILTER_NONE);
				//buffer.writeByte(INTERLACE_NONE);
				//buffer.endChunk(dataOutput);

				//buffer.writeInt(IDAT);
				//deflater.reset();

				//int lineLen = pixmap.getWidth() * 4;
				//byte[] lineOut, curLine, prevLine;
				//if (lineOutBytes == null) {
				//	lineOut = (lineOutBytes = new ByteArray(lineLen)).items;
				//	curLine = (curLineBytes = new ByteArray(lineLen)).items;
				//	prevLine = (prevLineBytes = new ByteArray(lineLen)).items;
				//} else {
				//	lineOut = lineOutBytes.ensureCapacity(lineLen);
				//	curLine = curLineBytes.ensureCapacity(lineLen);
				//	prevLine = prevLineBytes.ensureCapacity(lineLen);
				//	for (int i = 0, n = lastLineLen; i < n; i++)
				//		prevLine[i] = 0;
				//}
				//lastLineLen = lineLen;

				//ByteBuffer pixels = pixmap.getPixels();
				//int oldPosition = pixels.position();
				//bool rgba8888 = pixmap.getFormat() == Pixmap.Format.RGBA8888;
				//for (int y = 0, h = pixmap.getHeight(); y < h; y++) {
				//	int py = flipY ? (h - y - 1) : y;
				//	if (rgba8888) {
				//		((Buffer)pixels).position(py * lineLen);
				//		pixels.get(curLine, 0, lineLen);
				//	} else {
				//		for (int px = 0, x = 0; px < pixmap.getWidth(); px++) {
				//			int pixel = pixmap.getPixel(px, py);
				//			curLine[x++] = (byte)((pixel >> 24) & 0xff);
				//			curLine[x++] = (byte)((pixel >> 16) & 0xff);
				//			curLine[x++] = (byte)((pixel >> 8) & 0xff);
				//			curLine[x++] = (byte)(pixel & 0xff);
				//		}
				//	}

				//	lineOut[0] = (byte)(curLine[0] - prevLine[0]);
				//	lineOut[1] = (byte)(curLine[1] - prevLine[1]);
				//	lineOut[2] = (byte)(curLine[2] - prevLine[2]);
				//	lineOut[3] = (byte)(curLine[3] - prevLine[3]);

				//	for (int x = 4; x < lineLen; x++) {
				//		int a = curLine[x - 4] & 0xff;
				//		int b = prevLine[x] & 0xff;
				//		int c = prevLine[x - 4] & 0xff;
				//		int p = a + b - c;
				//		int pa = p - a;
				//		if (pa < 0) pa = -pa;
				//		int pb = p - b;
				//		if (pb < 0) pb = -pb;
				//		int pc = p - c;
				//		if (pc < 0) pc = -pc;
				//		if (pa <= pb && pa <= pc)
				//			c = a;
				//		else if (pb <= pc) //
				//			c = b;
				//		lineOut[x] = (byte)(curLine[x] - c);
				//	}

				//	deflaterOutput.write(PAETH);
				//	deflaterOutput.write(lineOut, 0, lineLen);

				//	byte[] temp = curLine;
				//	curLine = prevLine;
				//	prevLine = temp;
				//}
				//((Buffer)pixels).position(oldPosition);
				//deflaterOutput.finish();
				//buffer.endChunk(dataOutput);

				//buffer.writeInt(IEND);
				//buffer.endChunk(dataOutput);

				//output.flush();
			}

		/** Disposal will happen automatically in {@link #finalize()} but can be done explicitly if desired. */
		// TODO: @SuppressWarnings("javadoc")
		public void dispose () {
			// TODO:
			throw new NotImplementedException();
				//deflater.end();
			}

		//class ChunkBuffer : DataOutputStream {
		//	readonly ByteArrayOutputStream buffer;
		//	readonly CRC32 crc;

		//	internal ChunkBuffer (int initialSize) 
		//	: this(new ByteArrayOutputStream(initialSize), new CRC32())
		//	{
				
		//	}

		//	private ChunkBuffer (ByteArrayOutputStream buffer, CRC32 crc) 
		//	: base(new CheckedOutputStream(buffer, crc))
		//	{
				
		//		this.buffer = buffer;
		//		this.crc = crc;
		//	}

		//	public void endChunk (DataOutputStream target) // TODO: throws IOException 
		//	{
		//		flush();
		//		target.writeInt(buffer.size() - 4);
		//		buffer.writeTo(target);
		//		target.writeInt((int)crc.getValue());
		//		buffer.reset();
		//		crc.reset();
		//	}
		}
	}
}
}

