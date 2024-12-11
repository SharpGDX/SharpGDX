using SharpGDX.Files;
using SharpGDX.Graphics.G3D.Shaders;

namespace SharpGDX.Graphics.G3D.Utils;

public class DefaultShaderProvider : BaseShaderProvider {
	public readonly DefaultShader.Config config;

	public DefaultShaderProvider ( DefaultShader.Config config) {
		this.config = (config == null) ? new DefaultShader.Config() : config;
	}

	public DefaultShaderProvider ( String vertexShader,  String fragmentShader) 
    : this(new DefaultShader.Config(vertexShader, fragmentShader))
    {
		
	}

	public DefaultShaderProvider ( FileHandle vertexShader,  FileHandle fragmentShader) 
    : this(vertexShader.readString(), fragmentShader.readString())
    {
		
	}

	public DefaultShaderProvider () 
    : this(null)
    {
		
	}

	protected override Shader createShader ( Renderable renderable) {
		return new DefaultShader(renderable, config);
	}
}
