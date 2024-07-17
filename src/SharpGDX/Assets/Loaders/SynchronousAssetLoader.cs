using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders;

// TODO: Split into two files

internal interface ISynchronousAssetLoader : IAssetLoader
{
	public object load(AssetManager assetManager, String fileName, FileHandle file, IAssetLoaderParameters parameter);
}

public abstract class SynchronousAssetLoader<T, P> : AssetLoader<T, P>, ISynchronousAssetLoader
	where P : AssetLoaderParameters<T>
{
	public SynchronousAssetLoader(IFileHandleResolver resolver)
		: base(resolver)
	{

	}

	public abstract T load(AssetManager assetManager, String fileName, FileHandle file, P parameter);

	object ISynchronousAssetLoader.load(AssetManager assetManager, string fileName, FileHandle file, IAssetLoaderParameters parameter)
	{
		return load(assetManager, fileName, file, (P)parameter);
	}
}