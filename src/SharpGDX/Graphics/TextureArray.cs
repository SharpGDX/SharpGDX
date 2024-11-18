using SharpGDX.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics
{
	/** Open GLES wrapper for TextureArray
 * @author Tomski */
public class TextureArray : GLTexture {

	readonly static Map<IApplication, Array<TextureArray>> managedTextureArrays = new ();

	private ITextureArrayData data;

	public TextureArray (String[] internalPaths) 
	: this(getInternalHandles(internalPaths))
	{
		
	}

	public TextureArray (FileHandle[] files) 
	: this(false, files)
	{
		
	}

	public TextureArray (bool useMipMaps, FileHandle[] files) 
	: this(useMipMaps, Pixmap.Format.RGBA8888, files)
	{
		
	}

	public TextureArray (bool useMipMaps, Pixmap.Format format, FileHandle[] files) 
	: this(ITextureArrayData.Factory.loadFromFiles(format, useMipMaps, files))
	{
		
	}

	public TextureArray (ITextureArrayData data) 
	: base(IGL30.GL_TEXTURE_2D_ARRAY, Gdx.GL.glGenTexture())
	{
		

		if (Gdx.GL30 == null) {
			throw new GdxRuntimeException("TextureArray requires a device running with GLES 3.0 compatibilty");
		}

		load(data);

		if (data.isManaged()) addManagedTexture(Gdx.App, this);
	}

	private static FileHandle[] getInternalHandles (String[] internalPaths) {
		FileHandle[] handles = new FileHandle[internalPaths.Length];
		for (int i = 0; i < internalPaths.Length; i++) {
			handles[i] = Gdx.Files.Internal(internalPaths[i]);
		}
		return handles;
	}

	private void load (ITextureArrayData data) {
		if (this.data != null && data.isManaged() != this.data.isManaged())
			throw new GdxRuntimeException("New data must have the same managed status as the old data");
		this.data = data;

		bind();
		Gdx.GL30.glTexImage3D(IGL30.GL_TEXTURE_2D_ARRAY, 0, data.getInternalFormat(), data.getWidth(), data.getHeight(),
			data.getDepth(), 0, data.getInternalFormat(), data.getGLType(), null);

		if (!data.isPrepared()) data.prepare();

		data.consumeTextureArrayData();

		setFilter(minFilter, magFilter);
		setWrap(uWrap, vWrap);
		Gdx.GL.glBindTexture(glTarget, 0);
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
		glHandle = Gdx.GL.glGenTexture();
		load(data);
	}

	private static void addManagedTexture (IApplication app, TextureArray texture) {
		Array<TextureArray> managedTextureArray = managedTextureArrays.get(app);
		if (managedTextureArray == null) managedTextureArray = new Array<TextureArray>();
		managedTextureArray.Add(texture);
		managedTextureArrays.put(app, managedTextureArray);
	}

	/** Clears all managed TextureArrays. This is an internal method. Do not use it! */
	public static void clearAllTextureArrays (IApplication app) {
		managedTextureArrays.remove(app);
	}

	/** Invalidate all managed TextureArrays. This is an internal method. Do not use it! */
	public static void invalidateAllTextureArrays (IApplication app) {
		Array<TextureArray> managedTextureArray = managedTextureArrays.get(app);
		if (managedTextureArray == null) return;

		for (int i = 0; i < managedTextureArray.size; i++) {
			TextureArray textureArray = managedTextureArray.Get(i);
			textureArray.reload();
		}
	}

	public static String getManagedStatus () {
		StringBuilder builder = new StringBuilder();
		builder.Append("Managed TextureArrays/app: { ");
		foreach (var app in managedTextureArrays.keySet()) {
			builder.Append(managedTextureArrays.get(app).size);
			builder.Append(" ");
		}
		builder.Append("}");
		return builder.ToString();
	}

	/** @return the number of managed TextureArrays currently loaded */
	public static int getNumManagedTextureArrays () {
		return managedTextureArrays.get(Gdx.App).size;
	}

}
}
