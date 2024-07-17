using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Shims;
using SharpGDX.Utils;
using static SharpGDX.Graphics.Texture;

namespace SharpGDX.Graphics
{
	/** Open GLES wrapper for Texture3D
 * @author mgsx */
public class Texture3D : GLTexture {

	readonly static Map<IApplication, Array<Texture3D>> managedTexture3Ds = new ();

	private ITexture3DData data;

	protected TextureWrap rWrap = TextureWrap.ClampToEdge;

	public Texture3D (int width, int height, int depth, int glFormat, int glInternalFormat, int glType) 
	: this(new CustomTexture3DData(width, height, depth, 0, glFormat, glInternalFormat, glType))
	{
		
	}

	public Texture3D (ITexture3DData data) 
	: base(GL30.GL_TEXTURE_3D, Gdx.gl.glGenTexture())
	{
		

		if (Gdx.gl30 == null) {
			throw new GdxRuntimeException("Texture3D requires a device running with GLES 3.0 compatibilty");
		}

		load(data);

		if (data.isManaged()) addManagedTexture(Gdx.app, this);
	}

	private void load (ITexture3DData data) {
		if (this.data != null && data.isManaged() != this.data.isManaged())
			throw new GdxRuntimeException("New data must have the same managed status as the old data");
		this.data = data;

		bind();

		if (!data.isPrepared()) data.prepare();

		data.consume3DData();

		setFilter(minFilter, magFilter);
		setWrap(uWrap, vWrap, rWrap);

		Gdx.gl.glBindTexture(glTarget, 0);
	}

	public ITexture3DData getData () {
		return data;
	}

	public void upload () {
		bind();
		data.consume3DData();
	}

	public override int getWidth () {
		return data.getWidth();
	}

	public override int getHeight () {
		return data.getHeight();
	}

	public override int getDepth () {
		return data.getDepth();
	}

	public override bool isManaged () {
		return data.isManaged();
	}

	protected override void reload () {
		if (!isManaged()) throw new GdxRuntimeException("Tried to reload an unmanaged TextureArray");
		glHandle = Gdx.gl.glGenTexture();
		load(data);
	}

	private static void addManagedTexture (IApplication app, Texture3D texture) {
		Array<Texture3D> managedTextureArray = managedTexture3Ds.get(app);
		if (managedTextureArray == null) managedTextureArray = new Array<Texture3D>();
		managedTextureArray.add(texture);
		managedTexture3Ds.put(app, managedTextureArray);
	}

	/** Clears all managed TextureArrays. This is an internal method. Do not use it! */
	public static void clearAllTextureArrays (IApplication app) {
		managedTexture3Ds.remove(app);
	}

	/** Invalidate all managed TextureArrays. This is an internal method. Do not use it! */
	public static void invalidateAllTextureArrays (IApplication app) {
		Array<Texture3D> managedTextureArray = managedTexture3Ds.get(app);
		if (managedTextureArray == null) return;

		for (int i = 0; i < managedTextureArray.size; i++) {
			Texture3D textureArray = managedTextureArray.get(i);
			textureArray.reload();
		}
	}

	public static String getManagedStatus () {
		StringBuilder builder = new StringBuilder();
		builder.Append("Managed TextureArrays/app: { ");
		foreach (IApplication app in managedTexture3Ds.keySet()) {
			builder.Append(managedTexture3Ds.get(app).size);
			builder.Append(" ");
		}
		builder.Append("}");
		return builder.ToString();
	}

	/** @return the number of managed Texture3D currently loaded */
	public static int getNumManagedTextures3D () {
		return managedTexture3Ds.get(Gdx.app).size;
	}

	public void setWrap (TextureWrap u, TextureWrap v, TextureWrap r) {
		this.rWrap = r;
		base.setWrap(u, v);
		Gdx.gl.glTexParameteri(glTarget, GL30.GL_TEXTURE_WRAP_R, TextureWrapUtils.getGLEnum(r));
	}

	public void unsafeSetWrap (TextureWrap u, TextureWrap v, TextureWrap r, bool force) {
		unsafeSetWrap(u, v, force);
		if (r != null && (force || rWrap != r)) {
			Gdx.gl.glTexParameteri(glTarget, GL30.GL_TEXTURE_WRAP_R, TextureWrapUtils.getGLEnum(u));
			rWrap = r;
		}
	}

	public void unsafeSetWrap (TextureWrap u, TextureWrap v, TextureWrap r) {
		unsafeSetWrap(u, v, r, false);
	}

}
}
