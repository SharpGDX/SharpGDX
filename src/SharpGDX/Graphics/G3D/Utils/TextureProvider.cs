using System;
using SharpGDX.Assets;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D.Utils;

/** Used by {@link Model} to load textures from {@link ModelData}.
 * @author badlogic */
public interface TextureProvider {
	public Texture load (String fileName);

	public class FileTextureProvider : TextureProvider {
		private Texture.TextureFilter minFilter, magFilter;
		private Texture.TextureWrap uWrap, vWrap;
		private bool useMipMaps;

		public FileTextureProvider () {
			minFilter = magFilter = Texture.TextureFilter.Linear;
			uWrap = vWrap = Texture.TextureWrap.Repeat;
			useMipMaps = false;
		}

		public FileTextureProvider (Texture.TextureFilter minFilter, Texture.TextureFilter magFilter, Texture.TextureWrap uWrap,
			Texture.TextureWrap vWrap, bool useMipMaps) {
			this.minFilter = minFilter;
			this.magFilter = magFilter;
			this.uWrap = uWrap;
			this.vWrap = vWrap;
			this.useMipMaps = useMipMaps;
		}

		public Texture load (String fileName) {
			Texture result = new Texture(GDX.Files.Internal(fileName), useMipMaps);
			result.setFilter(minFilter, magFilter);
			result.setWrap(uWrap, vWrap);
			return result;
		}
	}

	public class AssetTextureProvider : TextureProvider {
		public readonly AssetManager assetManager;

		public AssetTextureProvider (AssetManager assetManager) {
			this.assetManager = assetManager;
		}

		public Texture load (String fileName) {
			return assetManager.get<Texture>(fileName, typeof(Texture));
		}
	}
}
