using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders;

// TODO: Split into two files

public interface IAssetLoader
{
	/** Returns the assets this asset requires to be loaded first. This method may be called on a thread other than the GL thread.
 * @param fileName name of the asset to load
 * @param file the resolved file to load
 * @param parameter parameters for loading the asset
 * @return other assets that the asset depends on and need to be loaded first or null if there are no dependencies. */
	Array<IAssetDescriptor>
		getDependencies(String fileName, FileHandle file, IAssetLoaderParameters parameter);

	/** @param fileName file name to resolve
	 * @return handle to the file, as resolved by the {@link FileHandleResolver} set on the loader */
	FileHandle resolve(String fileName);
}

/** Abstract base class for asset loaders.
 * @author mzechner
 * 
 * @param <T> the class of the asset the loader supports
 * @param <P> the class of the loading parameters the loader supports. */
public abstract class AssetLoader<T, P > : IAssetLoader
where P: AssetLoaderParameters<T>
{
	/** {@link FileHandleResolver} used to map from plain asset names to {@link FileHandle} instances **/
	private IFileHandleResolver resolver;

	/** Constructor, sets the {@link FileHandleResolver} to use to resolve the file associated with the asset name.
	 * @param resolver */
	public AssetLoader (IFileHandleResolver resolver) {
		this.resolver = resolver;
	}

	/** @param fileName file name to resolve
	 * @return handle to the file, as resolved by the {@link FileHandleResolver} set on the loader */
	public FileHandle resolve (String fileName) {
		return resolver.Resolve(fileName);
	}

	/** Returns the assets this asset requires to be loaded first. This method may be called on a thread other than the GL thread.
	 * @param fileName name of the asset to load
	 * @param file the resolved file to load
	 * @param parameter parameters for loading the asset
	 * @return other assets that the asset depends on and need to be loaded first or null if there are no dependencies. */
	public abstract Array<AssetDescriptor<T>>? getDependencies (String fileName, FileHandle file, P parameter);
	
	Array<IAssetDescriptor> IAssetLoader.getDependencies(string fileName, FileHandle file,
		IAssetLoaderParameters parameter)
	{
		// TODO: Better way to do this while still using Array<T>?
		return new Array<IAssetDescriptor>(getDependencies(fileName, file, (P)parameter).OfType<IAssetDescriptor>().ToArray());
	}
}
