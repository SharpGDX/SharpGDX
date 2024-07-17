using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders.Resolvers;

/** {@link FileHandleResolver} that adds a prefix to the filename before passing it to the base resolver. Can be used e.g. to use
 * a given subfolder from the base resolver. The prefix is added as is, you have to include any trailing '/' character if needed.
 * @author Xoppa */
public class PrefixFileHandleResolver : IFileHandleResolver {
	private String prefix;
	private IFileHandleResolver baseResolver;

	public PrefixFileHandleResolver (IFileHandleResolver baseResolver, String prefix) {
		this.baseResolver = baseResolver;
		this.prefix = prefix;
	}

	public void setBaseResolver (IFileHandleResolver baseResolver) {
		this.baseResolver = baseResolver;
	}

	public IFileHandleResolver getBaseResolver () {
		return baseResolver;
	}

	public void setPrefix (String prefix) {
		this.prefix = prefix;
	}

	public String getPrefix () {
		return prefix;
	}

	public FileHandle Resolve (String fileName) {
		return baseResolver.Resolve(prefix + fileName);
	}
}
