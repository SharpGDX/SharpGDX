using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders;

/** Interface for classes the can map a file name to a {@link FileHandle}. Used to allow the {@link AssetManager} to load
 * resources from anywhere or implement caching strategies.
 * @author mzechner */
public interface IFileHandleResolver {
	public FileHandle Resolve (String fileName);
}
