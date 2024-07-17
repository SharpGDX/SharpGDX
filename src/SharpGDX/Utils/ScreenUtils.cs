using System;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Shims;
using SharpGDX.Mathematics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	/** Class with static helper methods related to currently bound OpenGL frame buffer, including access to the current OpenGL
 * FrameBuffer. These methods can be used to get the entire screen content or a portion thereof.
 *
 * @author espitz */
public static class ScreenUtils {

	/** Clears the color buffers with the specified Color.
	 * @param color Color to clear the color buffers with. */
	public static void clear (Color color) {
		clear(color.r, color.g, color.b, color.a, false);
	}

	/** Clears the color buffers with the specified color. */
	public static void clear (float r, float g, float b, float a) {
		clear(r, g, b, a, false);
	}

	/** Clears the color buffers and optionally the depth buffer.
	 * @param color Color to clear the color buffers with.
	 * @param clearDepth Clears the depth buffer if true. */
	public static void clear (Color color, bool clearDepth) {
		clear(color.r, color.g, color.b, color.a, clearDepth);
	}

	/** Clears the color buffers and optionally the depth buffer.
	 * @param clearDepth Clears the depth buffer if true. */
	public static void clear (float r, float g, float b, float a, bool clearDepth) {
		Gdx.gl.glClearColor(r, g, b, a);
		int mask = GL20.GL_COLOR_BUFFER_BIT;
		if (clearDepth) mask = mask | GL20.GL_DEPTH_BUFFER_BIT;
		Gdx.gl.glClear(mask);
	}

	/** Returns the current framebuffer contents as a {@link TextureRegion} with a width and height equal to the current screen
	 * size. The base {@link Texture} always has {@link MathUtils#nextPowerOfTwo} dimensions and RGBA8888 {@link Format}. It can be
	 * accessed via {@link TextureRegion#getTexture}. The texture is not managed and has to be reloaded manually on a context loss.
	 * The returned TextureRegion is flipped along the Y axis by default. */
	public static TextureRegion getFrameBufferTexture () {
		 int w = Gdx.graphics.getBackBufferWidth();
		 int h = Gdx.graphics.getBackBufferHeight();
		return getFrameBufferTexture(0, 0, w, h);
	}

	/** Returns a portion of the current framebuffer contents specified by x, y, width and height as a {@link TextureRegion} with
	 * the same dimensions. The base {@link Texture} always has {@link MathUtils#nextPowerOfTwo} dimensions and RGBA8888
	 * {@link Format}. It can be accessed via {@link TextureRegion#getTexture}. This texture is not managed and has to be reloaded
	 * manually on a context loss. If the width and height specified are larger than the framebuffer dimensions, the Texture will
	 * be padded accordingly. Pixels that fall outside of the current screen will have RGBA values of 0.
	 *
	 * @param x the x position of the framebuffer contents to capture
	 * @param y the y position of the framebuffer contents to capture
	 * @param w the width of the framebuffer contents to capture
	 * @param h the height of the framebuffer contents to capture */
	public static TextureRegion getFrameBufferTexture (int x, int y, int w, int h) {
		 int potW = MathUtils.nextPowerOfTwo(w);
		 int potH = MathUtils.nextPowerOfTwo(h);

		 Pixmap pixmap = Pixmap.createFromFrameBuffer(x, y, w, h);
		 Pixmap potPixmap = new Pixmap(potW, potH, Pixmap.Format.RGBA8888);
		potPixmap.setBlending(Pixmap.Blending.None);
		potPixmap.drawPixmap(pixmap, 0, 0);
		Texture texture = new Texture(potPixmap);
		TextureRegion textureRegion = new TextureRegion(texture, 0, h, w, -h);
		potPixmap.dispose();
		pixmap.dispose();

		return textureRegion;
	}
		
	/** Returns the current framebuffer contents as a byte[] array with a length equal to screen width * height * 4. The byte[]
	 * will always contain RGBA8888 data. Because of differences in screen and image origins the framebuffer contents should be
	 * flipped along the Y axis if you intend save them to disk as a bitmap. Flipping is not a cheap operation, so use this
	 * functionality wisely.
	 *
	 * @param flipY whether to flip pixels along Y axis */
	public static byte[] getFrameBufferPixels (bool flipY) {
		 int w = Gdx.graphics.getBackBufferWidth();
		 int h = Gdx.graphics.getBackBufferHeight();
		return getFrameBufferPixels(0, 0, w, h, flipY);
	}

	/** Returns a portion of the current framebuffer contents specified by x, y, width and height, as a byte[] array with a length
	 * equal to the specified width * height * 4. The byte[] will always contain RGBA8888 data. If the width and height specified
	 * are larger than the framebuffer dimensions, the Texture will be padded accordingly. Pixels that fall outside of the current
	 * screen will have RGBA values of 0. Because of differences in screen and image origins the framebuffer contents should be
	 * flipped along the Y axis if you intend save them to disk as a bitmap. Flipping is not a cheap operation, so use this
	 * functionality wisely.
	 *
	 * @param flipY whether to flip pixels along Y axis */
	public static byte[] getFrameBufferPixels (int x, int y, int w, int h, bool flipY) {
		Gdx.gl.glPixelStorei(GL20.GL_PACK_ALIGNMENT, 1);
		 ByteBuffer pixels = BufferUtils.newByteBuffer(w * h * 4);
		Gdx.gl.glReadPixels(x, y, w, h, GL20.GL_RGBA, GL20.GL_UNSIGNED_BYTE, pixels);
		 int numBytes = w * h * 4;
		byte[] lines = new byte[numBytes];
		if (flipY) {
			 int numBytesPerLine = w * 4;
			for (int i = 0; i < h; i++) {
				((Buffer)pixels).position((h - i - 1) * numBytesPerLine);
				pixels.get(lines, i * numBytesPerLine, numBytesPerLine);
			}
		} else {
			((Buffer)pixels).clear();
			pixels.get(lines);
		}
		return lines;

	}
}
}
