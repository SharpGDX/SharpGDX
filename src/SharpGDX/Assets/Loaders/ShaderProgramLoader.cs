using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders;

/** {@link AssetLoader} for {@link ShaderProgram} instances loaded from text files. If the file suffix is ".vert", it is assumed
 * to be a vertex shader, and a fragment shader is found using the same file name with a ".frag" suffix. And vice versa if the
 * file suffix is ".frag". These default suffixes can be changed in the ShaderProgramLoader constructor.
 * <p>
 * For all other file suffixes, the same file is used for both (and therefore should internally distinguish between the programs
 * using preprocessor directives and {@link ShaderProgram#prependVertexCode} and {@link ShaderProgram#prependFragmentCode}).
 * <p>
 * The above default behavior for finding the files can be overridden by explicitly setting the file names in a
 * {@link ShaderProgramParameter}. The parameter can also be used to prepend code to the programs.
 * @author cypherdare */
public class ShaderProgramLoader : AsynchronousAssetLoader<ShaderProgram, ShaderProgramLoader.ShaderProgramParameter> {

	private String vertexFileSuffix = ".vert";
	private String fragmentFileSuffix = ".frag";

	public ShaderProgramLoader (IFileHandleResolver resolver) 
	:base(resolver)
	{
		
	}

	public ShaderProgramLoader (IFileHandleResolver resolver, String vertexFileSuffix, String fragmentFileSuffix) 
	: base(resolver)
	{
		
		this.vertexFileSuffix = vertexFileSuffix;
		this.fragmentFileSuffix = fragmentFileSuffix;
	}

	public override Array<AssetDescriptor<ShaderProgram>> getDependencies (String fileName, FileHandle file, ShaderProgramParameter parameter) {
		return null;
	}

	public override void loadAsync (AssetManager manager, String fileName, FileHandle file, ShaderProgramParameter parameter) {
	}

	public override ShaderProgram loadSync (AssetManager manager, String fileName, FileHandle file, ShaderProgramParameter parameter) {
		String vertFileName = null, fragFileName = null;
		if (parameter != null) {
			if (parameter.vertexFile != null) vertFileName = parameter.vertexFile;
			if (parameter.fragmentFile != null) fragFileName = parameter.fragmentFile;
		}
		if (vertFileName == null && fileName.EndsWith(fragmentFileSuffix)) {
			vertFileName = fileName.Substring(0, fileName.Length - fragmentFileSuffix.Length) + vertexFileSuffix;
		}
		if (fragFileName == null && fileName.EndsWith(vertexFileSuffix)) {
			fragFileName = fileName.Substring(0, fileName.Length - vertexFileSuffix.Length) + fragmentFileSuffix;
		}
		FileHandle vertexFile = vertFileName == null ? file : resolve(vertFileName);
		FileHandle fragmentFile = fragFileName == null ? file : resolve(fragFileName);
		String vertexCode = vertexFile.readString();
		String fragmentCode = vertexFile.Equals(fragmentFile) ? vertexCode : fragmentFile.readString();
		if (parameter != null) {
			if (parameter.prependVertexCode != null) vertexCode = parameter.prependVertexCode + vertexCode;
			if (parameter.prependFragmentCode != null) fragmentCode = parameter.prependFragmentCode + fragmentCode;
		}

		ShaderProgram shaderProgram = new ShaderProgram(vertexCode, fragmentCode);
		if ((parameter == null || parameter.logOnCompileFailure) && !shaderProgram.isCompiled()) {
			manager.getLogger().error("ShaderProgram " + fileName + " failed to compile:\n" + shaderProgram.getLog());
		}

		return shaderProgram;
	}

	public class ShaderProgramParameter : AssetLoaderParameters<ShaderProgram> {
		/** File name to be used for the vertex program instead of the default determined by the file name used to submit this asset
		 * to AssetManager. */
		public String vertexFile;
		/** File name to be used for the fragment program instead of the default determined by the file name used to submit this
		 * asset to AssetManager. */
		public String fragmentFile;
		/** Whether to log (at the error level) the shader's log if it fails to compile. Default true. */
		public bool logOnCompileFailure = true;
		/** Code that is always added to the vertex shader code. This is added as-is, and you should include a newline (`\n`) if
		 * needed. {@linkplain ShaderProgram#prependVertexCode} is placed before this code. */
		public String prependVertexCode;
		/** Code that is always added to the fragment shader code. This is added as-is, and you should include a newline (`\n`) if
		 * needed. {@linkplain ShaderProgram#prependFragmentCode} is placed before this code. */
		public String prependFragmentCode;
	}
}
