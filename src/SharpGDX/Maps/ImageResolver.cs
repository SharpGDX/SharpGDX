using SharpGDX.Shims;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.GLUtils;

namespace SharpGDX.Maps;

/** Resolves an image by a string, wrapper around a Map or AssetManager to load maps either directly or via AssetManager.
 * @author mzechner */
public interface ImageResolver {
	/** @param name
	 * @return the Texture for the given image name or null. */
	public TextureRegion getImage (String name);

	public  class DirectImageResolver : ImageResolver {
		private readonly ObjectMap<String, Texture> images;

		public DirectImageResolver (ObjectMap<String, Texture> images) {
			this.images = images;
		}

		public TextureRegion getImage (String name) {
			return new TextureRegion(images.get(name));
		}
	}

	public  class AssetManagerImageResolver : ImageResolver {
		private readonly AssetManager assetManager;

		public AssetManagerImageResolver (AssetManager assetManager) {
			this.assetManager = assetManager;
		}

		public TextureRegion getImage (String name) {
			return new TextureRegion(assetManager.get< Texture>(name, typeof(Texture)));
		}
	}

	public  class TextureAtlasImageResolver : ImageResolver {
		private readonly TextureAtlas atlas;

		public TextureAtlasImageResolver (TextureAtlas atlas) {
			this.atlas = atlas;
		}

		public TextureRegion getImage (String name) {
			return atlas.findRegion(name);
		}
	}
}
