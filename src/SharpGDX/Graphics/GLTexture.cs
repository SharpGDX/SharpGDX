using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Shims;

namespace SharpGDX.Graphics
{
/** Class representing an OpenGL texture by its target and handle. Keeps track of its state like the TextureFilter and
 * TextureWrap. Also provides some (protected) static methods to create TextureData and upload image data.
 * @author badlogic, Xoppa */
public abstract class GLTexture : Disposable {
	/** The target of this texture, used when binding the texture, e.g. GL_TEXTURE_2D */
	public readonly int glTarget;
	protected int glHandle;
	protected Texture.TextureFilter minFilter = Texture.TextureFilter.Nearest;
	protected Texture.TextureFilter magFilter = Texture.TextureFilter.Nearest;
	protected Texture.TextureWrap uWrap = Texture.TextureWrap.ClampToEdge;
	protected Texture.TextureWrap vWrap = Texture.TextureWrap.ClampToEdge;
	protected float anisotropicFilterLevel = 1.0f;
	private static float maxAnisotropicFilterLevel = 0;

	/** @return the width of the texture in pixels */
	public abstract int getWidth ();

	/** @return the height of the texture in pixels */
	public abstract int getHeight ();

	/** @return the depth of the texture in pixels */
	public abstract int getDepth ();

	/** Generates a new OpenGL texture with the specified target. */
	public GLTexture (int glTarget) 
	: this(glTarget, Gdx.gl.glGenTexture())
	{
		
	}

	public GLTexture (int glTarget, int glHandle) {
		this.glTarget = glTarget;
		this.glHandle = glHandle;
	}

	/** @return whether this texture is managed or not. */
	public abstract bool isManaged ();

	protected abstract void reload ();

	/** Binds this texture. The texture will be bound to the currently active texture unit specified via
	 * {@link GL20#glActiveTexture(int)}. */
	public void bind () {
		Gdx.gl.glBindTexture(glTarget, glHandle);
	}

	/** Binds the texture to the given texture unit. Sets the currently active texture unit via {@link GL20#glActiveTexture(int)}.
	 * @param unit the unit (0 to MAX_TEXTURE_UNITS). */
	public void bind (int unit) {
		Gdx.gl.glActiveTexture(GL20.GL_TEXTURE0 + unit);
		Gdx.gl.glBindTexture(glTarget, glHandle);
	}

	/** @return The {@link Texture.TextureFilter} used for minification. */
	public Texture.TextureFilter getMinFilter () {
		return minFilter;
	}

	/** @return The {@link Texture.TextureFilter} used for magnification. */
	public Texture.TextureFilter getMagFilter () {
		return magFilter;
	}

	/** @return The {@link Texture.TextureWrap} used for horizontal (U) texture coordinates. */
	public Texture.TextureWrap getUWrap () {
		return uWrap;
	}

	/** @return The {@link Texture.TextureWrap} used for vertical (V) texture coordinates. */
	public Texture.TextureWrap getVWrap () {
		return vWrap;
	}

	/** @return The OpenGL handle for this texture. */
	public int getTextureObjectHandle () {
		return glHandle;
	}

	/** Sets the {@link TextureWrap} for this texture on the u and v axis. Assumes the texture is bound and active!
	 * @param u the u wrap
	 * @param v the v wrap */
	public void unsafeSetWrap (Texture.TextureWrap u, Texture.TextureWrap v) {
		unsafeSetWrap(u, v, false);
	}

	/** Sets the {@link TextureWrap} for this texture on the u and v axis. Assumes the texture is bound and active!
	 * @param u the u wrap
	 * @param v the v wrap
	 * @param force True to always set the values, even if they are the same as the current values. */
	public void unsafeSetWrap (Texture.TextureWrap u, Texture.TextureWrap v, bool force) {
		if (u != null && (force || uWrap != u)) {
			Gdx.gl.glTexParameteri(glTarget, GL20.GL_TEXTURE_WRAP_S, Texture.TextureWrapUtils.getGLEnum(u));
			uWrap = u;
		}
		if (v != null && (force || vWrap != v)) {
			Gdx.gl.glTexParameteri(glTarget, GL20.GL_TEXTURE_WRAP_T, Texture.TextureWrapUtils.getGLEnum(v));
			vWrap = v;
		}
	}

	/** Sets the {@link TextureWrap} for this texture on the u and v axis. This will bind this texture!
	 * @param u the u wrap
	 * @param v the v wrap */
	public void setWrap (Texture.TextureWrap u, Texture.TextureWrap v) {
		this.uWrap = u;
		this.vWrap = v;
		bind();
		Gdx.gl.glTexParameteri(glTarget, GL20.GL_TEXTURE_WRAP_S, Texture.TextureWrapUtils.getGLEnum(u));
		Gdx.gl.glTexParameteri(glTarget, GL20.GL_TEXTURE_WRAP_T, Texture.TextureWrapUtils.getGLEnum(v));
	}

	/** Sets the {@link TextureFilter} for this texture for minification and magnification. Assumes the texture is bound and
	 * active!
	 * @param minFilter the minification filter
	 * @param magFilter the magnification filter */
	public void unsafeSetFilter (Texture.TextureFilter minFilter, Texture.TextureFilter magFilter) {
		unsafeSetFilter(minFilter, magFilter, false);
	}

	/** Sets the {@link TextureFilter} for this texture for minification and magnification. Assumes the texture is bound and
	 * active!
	 * @param minFilter the minification filter
	 * @param magFilter the magnification filter
	 * @param force True to always set the values, even if they are the same as the current values. */
	public void unsafeSetFilter (Texture.TextureFilter minFilter, Texture.TextureFilter magFilter, bool force) {
		if (minFilter != null && (force || this.minFilter != minFilter)) {
			Gdx.gl.glTexParameteri(glTarget, GL20.GL_TEXTURE_MIN_FILTER, Texture.TextureFilterUtils.getGLEnum(minFilter));
			this.minFilter = minFilter;
		}
		if (magFilter != null && (force || this.magFilter != magFilter)) {
			Gdx.gl.glTexParameteri(glTarget, GL20.GL_TEXTURE_MAG_FILTER, Texture.TextureFilterUtils.getGLEnum(magFilter));
			this.magFilter = magFilter;
		}
	}

	/** Sets the {@link TextureFilter} for this texture for minification and magnification. This will bind this texture!
	 * @param minFilter the minification filter
	 * @param magFilter the magnification filter */
	public void setFilter (Texture.TextureFilter minFilter, Texture.TextureFilter magFilter) {
		this.minFilter = minFilter;
		this.magFilter = magFilter;
		bind();
		Gdx.gl.glTexParameteri(glTarget, GL20.GL_TEXTURE_MIN_FILTER, Texture.TextureFilterUtils.getGLEnum(minFilter));
		Gdx.gl.glTexParameteri(glTarget, GL20.GL_TEXTURE_MAG_FILTER, Texture.TextureFilterUtils.getGLEnum(magFilter));
	}

	/** Sets the anisotropic filter level for the texture. Assumes the texture is bound and active!
	 *
	 * @param level The desired level of filtering. The maximum level supported by the device up to this value will be used.
	 * @return The actual level set, which may be lower than the provided value due to device limitations. */
	public float unsafeSetAnisotropicFilter (float level) {
		return unsafeSetAnisotropicFilter(level, false);
	}

	/** Sets the anisotropic filter level for the texture. Assumes the texture is bound and active!
	 *
	 * @param level The desired level of filtering. The maximum level supported by the device up to this value will be used.
	 * @param force True to always set the value, even if it is the same as the current values.
	 * @return The actual level set, which may be lower than the provided value due to device limitations. */
	public float unsafeSetAnisotropicFilter (float level, bool force) {
		float max = getMaxAnisotropicFilterLevel();
		if (max == 1f) return 1f;
		level = Math.Min(level, max);
		if (!force && MathUtils.isEqual(level, anisotropicFilterLevel, 0.1f)) return anisotropicFilterLevel;
		Gdx.gl20.glTexParameterf(GL20.GL_TEXTURE_2D, GL20.GL_TEXTURE_MAX_ANISOTROPY_EXT, level);
		return anisotropicFilterLevel = level;
	}

	/** Sets the anisotropic filter level for the texture. This will bind the texture!
	 *
	 * @param level The desired level of filtering. The maximum level supported by the device up to this value will be used.
	 * @return The actual level set, which may be lower than the provided value due to device limitations. */
	public float setAnisotropicFilter (float level) {
		float max = getMaxAnisotropicFilterLevel();
		if (max == 1f) return 1f;
		level = Math.Min(level, max);
		if (MathUtils.isEqual(level, anisotropicFilterLevel, 0.1f)) return level;
		bind();
		Gdx.gl20.glTexParameterf(GL20.GL_TEXTURE_2D, GL20.GL_TEXTURE_MAX_ANISOTROPY_EXT, level);
		return anisotropicFilterLevel = level;
	}

	/** @return The currently set anisotropic filtering level for the texture, or 1.0f if none has been set. */
	public float getAnisotropicFilter () {
		return anisotropicFilterLevel;
	}

	/** @return The maximum supported anisotropic filtering level supported by the device. */
	public static float getMaxAnisotropicFilterLevel () {
		if (maxAnisotropicFilterLevel > 0) return maxAnisotropicFilterLevel;
		if (Gdx.graphics.supportsExtension("GL_EXT_texture_filter_anisotropic")) {
			FloatBuffer buffer = BufferUtils.newFloatBuffer(16);
			((Buffer)buffer).position(0);
			((Buffer)buffer).limit(buffer.capacity());
			Gdx.gl20.glGetFloatv(GL20.GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT, buffer);
			return maxAnisotropicFilterLevel = buffer.get(0);
		}
		return maxAnisotropicFilterLevel = 1f;
	}

	/** Destroys the OpenGL Texture as specified by the glHandle. */
	protected void delete () {
		if (glHandle != 0) {
			Gdx.gl.glDeleteTexture(glHandle);
			glHandle = 0;
		}
	}

	public virtual void dispose () {
		delete();
	}

	protected static void uploadImageData (int target, ITextureData data) {
		uploadImageData(target, data, 0);
	}

	public static void uploadImageData (int target, ITextureData data, int miplevel) {
		if (data == null) {
			// FIXME: remove texture on target?
			return;
		}

		if (!data.isPrepared()) data.prepare();

		ITextureData.TextureDataType type = data.getType();
		if (type == ITextureData.TextureDataType.Custom) {
			data.consumeCustomData(target);
			return;
		}

		Pixmap pixmap = data.consumePixmap();
		bool disposePixmap = data.disposePixmap();
		if (data.getFormat() != pixmap.getFormat()) {
			Pixmap tmp = new Pixmap(pixmap.getWidth(), pixmap.getHeight(), data.getFormat());
			tmp.setBlending(Pixmap.Blending.None);
			tmp.drawPixmap(pixmap, 0, 0, 0, 0, pixmap.getWidth(), pixmap.getHeight());
			if (data.disposePixmap()) {
				pixmap.dispose();
			}
			pixmap = tmp;
			disposePixmap = true;
		}

		Gdx.gl.glPixelStorei(GL20.GL_UNPACK_ALIGNMENT, 1);
		if (data.useMipMaps()) {
			MipMapGenerator.generateMipMap(target, pixmap, pixmap.getWidth(), pixmap.getHeight());
		} else {
			// TODO: Testing -RP
			var array = pixmap.getPixels().array();
			Gdx.gl.glTexImage2D(target, miplevel, pixmap.getGLInternalFormat(), pixmap.getWidth(), pixmap.getHeight(), 0,
				pixmap.getGLFormat(), pixmap.getGLType(), array);

			pixmap.getPixels().put(array);
		}
		if (disposePixmap) pixmap.dispose();
	}
}
}
