using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders.Resolvers;

public class ClasspathFileHandleResolver : IFileHandleResolver {
	public FileHandle Resolve (String fileName) {
		return GDX.Files.Classpath(fileName);
	}
}
