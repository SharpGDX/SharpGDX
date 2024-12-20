﻿using SharpGDX.Files;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using System;
using SharpGDX.Assets;
using SharpGDX.Shims;
using SharpGDX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Assets.Loaders;

namespace SharpGDX.Graphics
{
	/** A Texture wraps a standard OpenGL ES texture.
 * <p>
 * A Texture can be managed. If the OpenGL context is lost all managed textures get invalidated. This happens when a user switches
 * to another application or receives an incoming call. Managed textures get reloaded automatically.
 * <p>
 * A Texture has to be bound via the {@link Texture#bind()} method in order for it to be applied to geometry. The texture will be
 * bound to the currently active texture unit specified via {@link GL20#glActiveTexture(int)}.
 * <p>
 * You can draw {@link Pixmap}s to a texture at any time. The changes will be automatically uploaded to texture memory. This is of
 * course not extremely fast so use it with care. It also only works with unmanaged textures.
 * <p>
 * A Texture must be disposed when it is no longer used
 * @author badlogicgames@gmail.com */
public class Texture : GLTexture {
	private static AssetManager assetManager;
	readonly static Map<IApplication, Array<Texture>> managedTextures = new();
		
	public enum TextureFilter {
		/** Fetch the nearest texel that best maps to the pixel on screen. */
		Nearest=(IGL20.GL_NEAREST),

		/** Fetch four nearest texels that best maps to the pixel on screen. */
		Linear=(IGL20.GL_LINEAR),

		/** @see TextureFilter#MipMapLinearLinear */
		MipMap=(IGL20.GL_LINEAR_MIPMAP_LINEAR),

		/** Fetch the best fitting image from the mip map chain based on the pixel/texel ratio and then sample the texels with a
		 * nearest filter. */
		MipMapNearestNearest=(IGL20.GL_NEAREST_MIPMAP_NEAREST),

		/** Fetch the best fitting image from the mip map chain based on the pixel/texel ratio and then sample the texels with a
		 * linear filter. */
		MipMapLinearNearest=(IGL20.GL_LINEAR_MIPMAP_NEAREST),

		/** Fetch the two best fitting images from the mip map chain and then sample the nearest texel from each of the two images,
		 * combining them to the final output pixel. */
		MipMapNearestLinear=(IGL20.GL_NEAREST_MIPMAP_LINEAR),

		/** Fetch the two best fitting images from the mip map chain and then sample the four nearest texels from each of the two
		 * images, combining them to the final output pixel. */
		MipMapLinearLinear=(IGL20.GL_LINEAR_MIPMAP_LINEAR)
		
	}
        
	public static class TextureFilterUtils
	{
		public static bool isMipMap(TextureFilter filter)
		{
			var glEnum = (int)filter;
			return glEnum != IGL20.GL_NEAREST && glEnum != IGL20.GL_LINEAR;
		}

		public static int getGLEnum(TextureFilter filter)
		{
			return (int)filter;
		}
		}

	public enum TextureWrap {
		MirroredRepeat=(IGL20.GL_MIRRORED_REPEAT),
		ClampToEdge=(IGL20.GL_CLAMP_TO_EDGE),
		Repeat=(IGL20.GL_REPEAT)
		
	}

	public static class TextureWrapUtils
	{
		public static int getGLEnum(TextureWrap wrap)
		{
			return (int)wrap;
		}
	}

	ITextureData data;

    protected Texture()
	: base(0, 0)
        {
        
    }

        public Texture (String internalPath) 
	: this(GDX.Files.Internal(internalPath))
	{
		
	}

	public Texture (FileHandle file) 
	: this(file, null, false)
	{
		
	}

	public Texture (FileHandle file, bool useMipMaps) 
	: this(file, null, useMipMaps)
	{
		
	}

	public Texture (FileHandle file, Pixmap.Format? format, bool useMipMaps) 
	: this(ITextureData.Factory.LoadFromFile(file, format, useMipMaps))
	{
		
	}

	public Texture (Pixmap pixmap) 
	: this(new PixmapTextureData(pixmap, null, false, false))
	{
		
	}

	public Texture (Pixmap pixmap, bool useMipMaps) 
	: this(new PixmapTextureData(pixmap, null, useMipMaps, false))
	{
		
	}

	public Texture (Pixmap pixmap, Pixmap.Format? format, bool useMipMaps) 
	: this(new PixmapTextureData(pixmap, format, useMipMaps, false))
	{
		
	}

	public Texture (int width, int height, Pixmap.Format? format) 
	: this(new PixmapTextureData(new Pixmap(width, height, format), null, false, true))
	{
		
	}

	public Texture (ITextureData data) 
	: this(IGL20.GL_TEXTURE_2D, GDX.GL.glGenTexture(), data)
	{
		
	}

	protected Texture (int glTarget, int glHandle, ITextureData data) 
	: base(glTarget, glHandle)
	{
		
		load(data);
		if (data.isManaged()) addManagedTexture(GDX.App, this);
	}

	public void load (ITextureData data) {
		if (this.data != null && data.isManaged() != this.data.isManaged())
			throw new GdxRuntimeException("New data must have the same managed status as the old data");
		this.data = data;

		if (!data.isPrepared()) data.prepare();

		bind();
		uploadImageData(IGL20.GL_TEXTURE_2D, data);

		unsafeSetFilter(minFilter, magFilter, true);
		unsafeSetWrap(uWrap, vWrap, true);
		unsafeSetAnisotropicFilter(anisotropicFilterLevel, true);
		GDX.GL.glBindTexture(glTarget, 0);
	}

	/** Used internally to reload after context loss. Creates a new GL handle then calls {@link #load(TextureData)}. Use this only
	 * if you know what you do! */
	protected override void reload () {
		if (!isManaged()) throw new GdxRuntimeException("Tried to reload unmanaged Texture");
		glHandle = GDX.GL.glGenTexture();
		load(data);
	}

        /** Draws the given {@link Pixmap} to the texture at position x, y. No clipping is performed so you have to make sure that you
         * draw only inside the texture region. Note that this will only draw to mipmap level 0!
         *
         * @param pixmap The Pixmap
         * @param x The x coordinate in pixels
         * @param y The y coordinate in pixels */
        public void draw (Pixmap pixmap, int x, int y) {
		if (data.isManaged()) throw new GdxRuntimeException("can't draw to a managed texture");

		bind();
		GDX.GL.glTexSubImage2D(glTarget, 0, x, y, pixmap.GetWidth(), pixmap.GetHeight(), pixmap.GetGLFormat(), pixmap.GetGLType(),
			pixmap.GetPixels());
	}

	public override int getWidth () {
		return data.getWidth();
	}

	public override int getHeight () {
		return data.getHeight();
	}

		public override int getDepth () {
		return 0;
	}

	public ITextureData getTextureData () {
		return data;
	}

		/** @return whether this texture is managed or not. */
		public override bool isManaged () {
		return data.isManaged();
	}

		/** Disposes all resources associated with the texture */
		public override void Dispose () {
		// this is a hack. reason: we have to set the glHandle to 0 for textures that are
		// reloaded through the asset manager as we first remove (and thus dispose) the texture
		// and then reload it. the glHandle is set to 0 in invalidateAllTextures prior to
		// removal from the asset manager.
		if (glHandle == 0) return;
		delete();
		if (data.isManaged()) if (managedTextures.get(GDX.App) != null) managedTextures.get(GDX.App).RemoveValue(this, true);
	}

		public override String ToString () {
		if (data is FileTextureData) return data.ToString();
		return base.ToString();
	}

	private static void addManagedTexture (IApplication app, Texture texture) {
		Array<Texture> managedTextureArray = managedTextures.get(app);
		if (managedTextureArray == null) managedTextureArray = new Array<Texture>();
		managedTextureArray.Add(texture);
		managedTextures.put(app, managedTextureArray);
	}

	/** Clears all managed textures. This is an internal method. Do not use it! */
	public static void clearAllTextures (IApplication app) {
		managedTextures.remove(app);
	}

	/** Invalidate all managed textures. This is an internal method. Do not use it! */
	public static void invalidateAllTextures (IApplication app) {
		Array<Texture> managedTextureArray = managedTextures.get(app);
		if (managedTextureArray == null) return;

		if (assetManager == null) {
			for (int i = 0; i < managedTextureArray.size; i++) {
				Texture texture = managedTextureArray.Get(i);
				texture.reload();
			}
		} else {
			// first we have to make sure the AssetManager isn't loading anything anymore,
			// otherwise the ref counting trick below wouldn't work (when a texture is
			// currently on the task stack of the manager.)
			assetManager.finishLoading();

			// next we go through each texture and reload either directly or via the
			// asset manager.
			Array<Texture> textures = new Array<Texture>(managedTextureArray);
			foreach (Texture texture in textures) {
				String fileName = assetManager.getAssetFileName(texture);
				if (fileName == null) {
					texture.reload();
				} else {
					// get the ref count of the texture, then set it to 0 so we
					// can actually remove it from the assetmanager. Also set the
					// handle to zero, otherwise we might accidentially dispose
					// already reloaded textures.
					int refCount = assetManager.getReferenceCount(fileName);
					assetManager.setReferenceCount(fileName, 0);
					texture.glHandle = 0;

					// create the parameters, passing the reference to the texture as
					// well as a callback that sets the ref count.
					TextureLoader.TextureParameter @params = new TextureLoader.TextureParameter();
					@params.textureData = texture.getTextureData();
					@params.minFilter = texture.getMinFilter();
					@params.magFilter = texture.getMagFilter();
					@params.wrapU = texture.getUWrap();
					@params.wrapV = texture.getVWrap();
					@params.genMipMaps = texture.data.useMipMaps(); // not sure about this?
					@params.texture = texture; // special parameter which will ensure that the references stay the same.

					// TODO: Should this be done differently?
					@params.loadedCallback = (assetManager, fileName, _) =>
					{
						assetManager.setReferenceCount(fileName, refCount);
					};

					// unload the texture, create a new gl handle then reload it.
					assetManager.unload(fileName);
					texture.glHandle = GDX.GL.glGenTexture();
					assetManager.load<Texture>(fileName, typeof(Texture), @params);
				}
			}
			managedTextureArray.clear();
			managedTextureArray.addAll(textures);
		}
	}

	/** Sets the {@link AssetManager}. When the context is lost, textures managed by the asset manager are reloaded by the manager
	 * on a separate thread (provided that a suitable {@link AssetLoader} is registered with the manager). Textures not managed by
	 * the AssetManager are reloaded via the usual means on the rendering thread.
	 * @param manager the asset manager. */
	public static void setAssetManager (AssetManager manager) {
		Texture.assetManager = manager;
	}

	public static String getManagedStatus () {
		StringBuilder builder = new StringBuilder();
		builder.Append("Managed textures/app: { ");
		foreach (IApplication app in managedTextures.keySet()) {
			builder.Append(managedTextures.get(app).size);
			builder.Append(" ");
		}
		builder.Append("}");
		return builder.ToString();
	}

	/** @return the number of managed textures currently loaded */
	public static int getNumManagedTextures () {
		return managedTextures.get(GDX.App).size;
	}
}
}
