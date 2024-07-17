using SharpGDX.Files;
using System;
using static SharpGDX.Assets.Loaders.CubemapLoader;
using SharpGDX.Graphics.GLUtils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Assets;
using SharpGDX.Shims;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Graphics
{
	/** Wraps a standard OpenGL ES Cubemap. Must be disposed when it is no longer used.
 * @author Xoppa */
public class Cubemap : GLTexture {
	private static AssetManager assetManager;
	readonly static Map<IApplication, Array<Cubemap>> managedCubemaps = new ();

	/** Enum to identify each side of a Cubemap */
	// TODO: Should this be converted to an enum with extension methods? -RP
	public class CubemapSide {
		/** The positive X and first side of the cubemap */
		public static CubemapSide PositiveX = new(0, GL20.GL_TEXTURE_CUBE_MAP_POSITIVE_X, 0, -1, 0, 1, 0, 0);

		public static CubemapSide NegativeX = new(1, GL20.GL_TEXTURE_CUBE_MAP_NEGATIVE_X, 0, -1, 0, -1, 0, 0);

		/** The positive Y and third side of the cubemap */
		public static CubemapSide PositiveY = new(2, GL20.GL_TEXTURE_CUBE_MAP_POSITIVE_Y, 0, 0, 1, 0, 1, 0);

		/** The negative Y and fourth side of the cubemap */
		public static CubemapSide NegativeY = new(3, GL20.GL_TEXTURE_CUBE_MAP_NEGATIVE_Y, 0, 0, -1, 0, -1, 0);

		/** The positive Z and fifth side of the cubemap */
		public static CubemapSide PositiveZ = new(4, GL20.GL_TEXTURE_CUBE_MAP_POSITIVE_Z, 0, -1, 0, 0, 0, 1);
		/** The negative Z and sixth side of the cubemap */
		public static CubemapSide NegativeZ = new(5, GL20.GL_TEXTURE_CUBE_MAP_NEGATIVE_Z, 0, -1, 0, 0, 0, -1);

		static CubemapSide()
		{
			_values = new[]
			{
				PositiveX,
				NegativeX,
				PositiveY,
				NegativeY,
				PositiveZ,
				NegativeZ
			};
		}

		private static CubemapSide[] _values;

		public static CubemapSide[] values()
		{
			return _values;
		}

		/** The zero based index of the side in the cubemap */
		public readonly int index;
		/** The OpenGL target (used for glTexImage2D) of the side. */
		public readonly int glEnum;
		/** The up vector to target the side. */
		public readonly Vector3 up;
		/** The direction vector to target the side. */
		public readonly Vector3 direction;

		CubemapSide (int index, int glEnum, float upX, float upY, float upZ, float directionX, float directionY, float directionZ) {
			this.index = index;
			this.glEnum = glEnum;
			this.up = new Vector3(upX, upY, upZ);
			this.direction = new Vector3(directionX, directionY, directionZ);
		}

		/** @return The OpenGL target (used for glTexImage2D) of the side. */
		public int getGLEnum () {
			return glEnum;
		}

		/** @return The up vector of the side. */
		public Vector3 getUp (Vector3 @out) {
			return @out.set(up);
		}

		/** @return The direction vector of the side. */
		public Vector3 getDirection (Vector3 @out) {
			return @out.set(direction);
		}
	}

	protected ICubemapData data;

	/** Construct a Cubemap based on the given CubemapData. */
	public Cubemap (ICubemapData data) 
		: base(GL20.GL_TEXTURE_CUBE_MAP)
	{
		
		this.data = data;
		load(data);
		if (data.isManaged()) addManagedCubemap(Gdx.app, this);
	}

	/** Construct a Cubemap with the specified texture files for the sides, does not generate mipmaps. */
	public Cubemap (FileHandle positiveX, FileHandle negativeX, FileHandle positiveY, FileHandle negativeY, FileHandle positiveZ,
		FileHandle negativeZ) 
	: this(positiveX, negativeX, positiveY, negativeY, positiveZ, negativeZ, false)
	{
		
	}

	/** Construct a Cubemap with the specified texture files for the sides, optionally generating mipmaps. */
	public Cubemap (FileHandle positiveX, FileHandle negativeX, FileHandle positiveY, FileHandle negativeY, FileHandle positiveZ,
		FileHandle negativeZ, bool useMipMaps) 
	: this(ITextureData.Factory.LoadFromFile(positiveX, useMipMaps), ITextureData.Factory.LoadFromFile(negativeX, useMipMaps),
		ITextureData.Factory.LoadFromFile(positiveY, useMipMaps), ITextureData.Factory.LoadFromFile(negativeY, useMipMaps),
		ITextureData.Factory.LoadFromFile(positiveZ, useMipMaps), ITextureData.Factory.LoadFromFile(negativeZ, useMipMaps))
	{
		
	}

	/** Construct a Cubemap with the specified {@link Pixmap}s for the sides, does not generate mipmaps. */
	public Cubemap (Pixmap positiveX, Pixmap negativeX, Pixmap positiveY, Pixmap negativeY, Pixmap positiveZ, Pixmap negativeZ) 
	: this(positiveX, negativeX, positiveY, negativeY, positiveZ, negativeZ, false)
	{
		
	}

	/** Construct a Cubemap with the specified {@link Pixmap}s for the sides, optionally generating mipmaps. */
	public Cubemap (Pixmap positiveX, Pixmap negativeX, Pixmap positiveY, Pixmap negativeY, Pixmap positiveZ, Pixmap negativeZ,
		bool useMipMaps) 
	: this(positiveX == null ? null : new PixmapTextureData(positiveX, null, useMipMaps, false),
		negativeX == null ? null : new PixmapTextureData(negativeX, null, useMipMaps, false),
		positiveY == null ? null : new PixmapTextureData(positiveY, null, useMipMaps, false),
		negativeY == null ? null : new PixmapTextureData(negativeY, null, useMipMaps, false),
		positiveZ == null ? null : new PixmapTextureData(positiveZ, null, useMipMaps, false),
		negativeZ == null ? null : new PixmapTextureData(negativeZ, null, useMipMaps, false))
	{
		
	}

	/** Construct a Cubemap with {@link Pixmap}s for each side of the specified size. */
	public Cubemap (int width, int height, int depth, Pixmap.Format format) 
	: this(new PixmapTextureData(new Pixmap(depth, height, format), null, false, true),
		new PixmapTextureData(new Pixmap(depth, height, format), null, false, true),
		new PixmapTextureData(new Pixmap(width, depth, format), null, false, true),
		new PixmapTextureData(new Pixmap(width, depth, format), null, false, true),
		new PixmapTextureData(new Pixmap(width, height, format), null, false, true),
		new PixmapTextureData(new Pixmap(width, height, format), null, false, true))
	{
		
	}

	/** Construct a Cubemap with the specified {@link TextureData}'s for the sides */
	public Cubemap (ITextureData positiveX, ITextureData negativeX, ITextureData positiveY, ITextureData negativeY,
		ITextureData positiveZ, ITextureData negativeZ) 
	: this(new FacedCubemapData(positiveX, negativeX, positiveY, negativeY, positiveZ, negativeZ))
	{
		
	}

	/** Sets the sides of this cubemap to the specified {@link CubemapData}. */
	public void load (ICubemapData data) {
		if (!data.isPrepared()) data.prepare();
		bind();
		unsafeSetFilter(minFilter, magFilter, true);
		unsafeSetWrap(uWrap, vWrap, true);
		unsafeSetAnisotropicFilter(anisotropicFilterLevel, true);
		data.consumeCubemapData();
		Gdx.gl.glBindTexture(glTarget, 0);
	}

	public ICubemapData getCubemapData () {
		return data;
	}

	public override bool isManaged () {
		return data.isManaged();
	}

	protected override void reload () {
		if (!isManaged()) throw new GdxRuntimeException("Tried to reload an unmanaged Cubemap");
		glHandle = Gdx.gl.glGenTexture();
		load(data);
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

		/** Disposes all resources associated with the cubemap */
		public override void dispose () {
		// this is a hack. reason: we have to set the glHandle to 0 for textures that are
		// reloaded through the asset manager as we first remove (and thus dispose) the texture
		// and then reload it. the glHandle is set to 0 in invalidateAllTextures prior to
		// removal from the asset manager.
		if (glHandle == 0) return;
		delete();
		if (data.isManaged()) if (managedCubemaps.get(Gdx.app) != null) managedCubemaps.get(Gdx.app).removeValue(this, true);
	}

	private static void addManagedCubemap (IApplication app, Cubemap cubemap) {
		Array<Cubemap> managedCubemapArray = managedCubemaps.get(app);
		if (managedCubemapArray == null) managedCubemapArray = new Array<Cubemap>();
		managedCubemapArray.add(cubemap);
		managedCubemaps.put(app, managedCubemapArray);
	}

	/** Clears all managed cubemaps. This is an internal method. Do not use it! */
	public static void clearAllCubemaps (IApplication app) {
		managedCubemaps.remove(app);
	}

	/** Invalidate all managed cubemaps. This is an internal method. Do not use it! */
	public static void invalidateAllCubemaps (IApplication app) {
		Array<Cubemap> managedCubemapArray = managedCubemaps.get(app);
		if (managedCubemapArray == null) return;

		if (assetManager == null) {
			for (int i = 0; i < managedCubemapArray.size; i++) {
				Cubemap cubemap = managedCubemapArray.get(i);
				cubemap.reload();
			}
		} else {
			// first we have to make sure the AssetManager isn't loading anything anymore,
			// otherwise the ref counting trick below wouldn't work (when a cubemap is
			// currently on the task stack of the manager.)
			assetManager.finishLoading();

			// next we go through each cubemap and reload either directly or via the
			// asset manager.
			Array<Cubemap> cubemaps = new Array<Cubemap>(managedCubemapArray);
			foreach (Cubemap cubemap in cubemaps) {
				String fileName = assetManager.getAssetFileName(cubemap);
				if (fileName == null) {
					cubemap.reload();
				} else {
					// get the ref count of the cubemap, then set it to 0 so we
					// can actually remove it from the assetmanager. Also set the
					// handle to zero, otherwise we might accidentially dispose
					// already reloaded cubemaps.
					int refCount = assetManager.getReferenceCount(fileName);
					assetManager.setReferenceCount(fileName, 0);
					cubemap.glHandle = 0;

					// create the parameters, passing the reference to the cubemap as
					// well as a callback that sets the ref count.
					CubemapParameter @params = new CubemapParameter();
					@params.cubemapData = cubemap.getCubemapData();
					@params.minFilter = cubemap.getMinFilter();
					@params.magFilter = cubemap.getMagFilter();
					@params.wrapU = cubemap.getUWrap();
					@params.wrapV = cubemap.getVWrap();
					@params.cubemap = cubemap; // special parameter which will ensure that the references stay the same.
					
					// TODO: Should be cleared at some point. -RP
					@params.loadedCallback += (assetManager, fileNamename, type) =>
					{
						assetManager.setReferenceCount(fileName, refCount);
					};

					// unload the c, create a new gl handle then reload it.
					assetManager.unload(fileName);
					cubemap.glHandle = Gdx.gl.glGenTexture();
					assetManager.load(fileName, typeof(Cubemap), @params);
				}
			}
			managedCubemapArray.clear();
			managedCubemapArray.addAll(cubemaps);
		}
	}

	/** Sets the {@link AssetManager}. When the context is lost, cubemaps managed by the asset manager are reloaded by the manager
	 * on a separate thread (provided that a suitable {@link AssetLoader} is registered with the manager). Cubemaps not managed by
	 * the AssetManager are reloaded via the usual means on the rendering thread.
	 * @param manager the asset manager. */
	public static void setAssetManager (AssetManager manager) {
		Cubemap.assetManager = manager;
	}

	public static String getManagedStatus () {
		StringBuilder builder = new StringBuilder();
		builder.Append("Managed cubemap/app: { ");
		foreach (IApplication app in managedCubemaps.keySet()) {
			builder.Append(managedCubemaps.get(app).size);
			builder.Append(" ");
		}
		builder.Append("}");
		return builder.ToString();
	}

	/** @return the number of managed cubemaps currently loaded */
	public static int getNumManagedCubemaps () {
		return managedCubemaps.get(Gdx.app).size;
	}

}
}
