using System;
using SharpGDX.Files;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGDX.Graphics.G3D.Shaders;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Utils;


namespace SharpGDX.Graphics.G3D.Utils;

public class DepthShaderProvider : BaseShaderProvider {
	public readonly DepthShader.Config config;

	public DepthShaderProvider ( DepthShader.Config? config) {
		this.config = (config == null) ? new DepthShader.Config() : config;
	}

	public DepthShaderProvider ( String vertexShader,  String fragmentShader) 
    : this(new DepthShader.Config(vertexShader, fragmentShader))
    {
		
	}

	public DepthShaderProvider ( FileHandle vertexShader,  FileHandle fragmentShader) 
    : this(vertexShader.readString(), fragmentShader.readString())
    {
		
	}

	public DepthShaderProvider () 
    : this(null)
    {
		
	}

	protected override Shader createShader ( Renderable renderable) {
		return new DepthShader(renderable, config);
	}
}
