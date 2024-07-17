using SharpGDX.Files;
using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders;

/** {@link AssetLoader} to load {@link ParticleEffect} instances. Passing a {@link ParticleEffectParameter} to
 * {@link AssetManager#load(String, Class, AssetLoaderParameters)} allows to specify an atlas file or an image directory to be
 * used for the effect's images. Per default images are loaded from the directory in which the effect file is found. */
public class ParticleEffectLoader : SynchronousAssetLoader<ParticleEffect, ParticleEffectLoader.ParticleEffectParameter>
{
	public ParticleEffectLoader(IFileHandleResolver resolver)
	: base(resolver)
	{
		
	}

	
	public override ParticleEffect load(AssetManager am, String fileName, FileHandle file, ParticleEffectParameter param)
	{
		ParticleEffect effect = new ParticleEffect();
		if (param is { atlasFile: not null })
			effect.load(file, am.get< TextureAtlas>(param.atlasFile, typeof(TextureAtlas)), param.atlasPrefix);
		else if (param is { imagesDir: not null })
			effect.load(file, param.imagesDir);
		else
			effect.load(file, file.parent());
		return effect;
	}

	public override Array<AssetDescriptor<ParticleEffect>> getDependencies(String fileName, FileHandle file, ParticleEffectParameter param)
	{
		throw new NotImplementedException();
	//Array<AssetDescriptor> deps = null;
	//if (param != null && param.atlasFile != null)
	//{
	//	deps = new ();
	//	deps.add(new AssetDescriptor<TextureAtlas>(param.atlasFile, typeof(TextureAtlas)));
	//	}
	//	return deps;
	}

	/** Parameter to be passed to {@link AssetManager#load(String, Class, AssetLoaderParameters)} if additional configuration is
	 * necessary for the {@link ParticleEffect}. */
	public  class ParticleEffectParameter : AssetLoaderParameters<ParticleEffect> {
		/** Atlas file name. */
		public String atlasFile;
/** Optional prefix to image names **/
public String atlasPrefix;
/** Image directory. */
public FileHandle imagesDir;
	}
}
