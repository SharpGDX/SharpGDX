using System;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Utils;


namespace SharpGDX.Graphics.G3D.Utils;

/** Class that you assign a range of texture units and binds textures for you within that range. It does some basic usage tracking
 * to avoid unnecessary bind calls.
 * @author xoppa */
public sealed class DefaultTextureBinder : TextureBinder {
	public const int ROUNDROBIN = 0;
	public const int LRU = 1;
	/** GLES only supports up to 32 textures */
	public readonly static int MAX_GLES_UNITS = 32;
	/** The index of the first exclusive texture unit */
	private readonly int offset;
	/** The amount of exclusive textures that may be used */
	private readonly int count;
	/** The textures currently exclusive bound */
	private readonly GLTexture[] textures;
	/** Texture units ordered from most to least recently used */
	private int[] unitsLRU;
	/** The method of binding to use */
	private readonly int method;
	/** Flag to indicate the current texture is reused */
	private bool reused;

	private int reuseCount = 0; // TODO remove debug code
	private int bindCount = 0; // TODO remove debug code

	/** Uses all available texture units and reuse weight of 3 */
	public DefaultTextureBinder ( int method) 
    : this(method, 0)
    {
		
	}

	/** Uses all remaining texture units and reuse weight of 3 */
	public DefaultTextureBinder ( int method,  int offset) 
    : this(method, offset, -1)
    {
		
	}

	public DefaultTextureBinder ( int method,  int offset, int count) {
		 int max = Math.Min(getMaxTextureUnits(), MAX_GLES_UNITS);
		if (count < 0) count = max - offset;
		if (offset < 0 || count < 0 || (offset + count) > max) throw new GdxRuntimeException("Illegal arguments");
		this.method = method;
		this.offset = offset;
		this.count = count;
		this.textures = new GLTexture[count];
		this.unitsLRU = (method == LRU) ? new int[count] : null;
	}

	private static int getMaxTextureUnits () {
		IntBuffer buffer = BufferUtils.newIntBuffer(16);
		GDX.GL.glGetIntegerv(IGL20.GL_MAX_TEXTURE_IMAGE_UNITS, buffer);
		return buffer.get(0);
	}

	public void begin () {
		for (int i = 0; i < count; i++) {
			textures[i] = null;
			if (unitsLRU != null) unitsLRU[i] = i;
		}
	}

	public void end () {
		/*
		 * No need to unbind and textures are set to null in begin() for(int i = 0; i < count; i++) { if (textures[i] != null) {
		 * GDX.GL.glActiveTexture(GL20.GL_TEXTURE0 + offset + i); GDX.GL.glBindTexture(GL20.GL_TEXTURE_2D, 0); textures[i] = null; }
		 * }
		 */
		GDX.GL.glActiveTexture(IGL20.GL_TEXTURE0);
	}

	public int bind ( TextureDescriptor textureDesc) {
		return bindTexture(textureDesc, false);
	}

	private readonly TextureDescriptor tempDesc = new TextureDescriptor();

	public int bind ( GLTexture texture) {
		tempDesc.set(texture, null, null, null, null);
		return bindTexture(tempDesc, false);
	}

	private int bindTexture ( TextureDescriptor textureDesc,  bool rebind) {
		 int idx, result;
		 GLTexture texture = textureDesc.texture;
		reused = false;

		switch (method) {
		case ROUNDROBIN:
			result = offset + (idx = bindTextureRoundRobin(texture));
			break;
		case LRU:
			result = offset + (idx = bindTextureLRU(texture));
			break;
		default:
			return -1;
		}

		if (reused) {
			reuseCount++;
			if (rebind)
				texture.bind(result);
			else
				GDX.GL.glActiveTexture(IGL20.GL_TEXTURE0 + result);
		} else
			bindCount++;
		texture.unsafeSetWrap(textureDesc.uWrap, textureDesc.vWrap);
		texture.unsafeSetFilter(textureDesc.minFilter, textureDesc.magFilter);
		return result;
	}

	private int currentTexture = 0;

	private  int bindTextureRoundRobin ( GLTexture texture) {
		for (int i = 0; i < count; i++) {
			 int idx = (currentTexture + i) % count;
			if (textures[idx] == texture) {
				reused = true;
				return idx;
			}
		}
		currentTexture = (currentTexture + 1) % count;
		textures[currentTexture] = texture;
		texture.bind(offset + currentTexture);
		return currentTexture;
	}

	private  int bindTextureLRU ( GLTexture texture) {
		int i;
        int idx;
		for (i = 0; i < count; i++) {
			  idx = unitsLRU[i];
			if (textures[idx] == texture) {
				reused = true;
				break;
			}
			if (textures[idx] == null) {
				break;
			}
		}
		if (i >= count) i = count - 1;
		  idx = unitsLRU[i];
		while (i > 0) {
			unitsLRU[i] = unitsLRU[i - 1];
			i--;
		}
		unitsLRU[0] = idx;
		if (!reused) {
			textures[idx] = texture;
			texture.bind(offset + idx);
		}
		return idx;
	}

	public  int getBindCount () {
		return bindCount;
	}

	public  int getReuseCount () {
		return reuseCount;
	}

	public  void resetCounts () {
		bindCount = reuseCount = 0;
	}
}
