using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders.Resolvers;

public class ExternalFileHandleResolver : IFileHandleResolver {
	public FileHandle Resolve (String fileName) {
		return Gdx.files.external(fileName);
	}
}
