using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders.Resolvers;

public class InternalFileHandleResolver : IFileHandleResolver {
	public FileHandle Resolve (String fileName) {
		return Gdx.files.@internal(fileName);
	}
}
