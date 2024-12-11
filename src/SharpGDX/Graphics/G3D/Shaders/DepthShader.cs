using System;
using SharpGDX.Graphics.G3D.Attributess;
using SharpGDX.Files;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Utils;


namespace SharpGDX.Graphics.G3D.Shaders;

public class DepthShader : DefaultShader {
	public  class Config : DefaultShader.Config {
		public bool depthBufferOnly = false;
		public float defaultAlphaTest = 0.5f;

		public Config () 
        : base()
        {
			
			defaultCullFace = IGL20.GL_FRONT;
		}

		public Config (String vertexShader, String fragmentShader) 
        : base(vertexShader, fragmentShader)
        {
			
		}
	}

	private static String defaultVertexShader = null;

	public  static String getDefaultVertexShader () {
		if (defaultVertexShader == null)
			defaultVertexShader = GDX.Files.Classpath("com/badlogic/gdx/graphics/g3d/shaders/depth.vertex.glsl").readString();
		return defaultVertexShader;
	}

	private static String defaultFragmentShader = null;

	public static String getDefaultFragmentShader () {
		if (defaultFragmentShader == null)
			defaultFragmentShader = GDX.Files.Classpath("com/badlogic/gdx/graphics/g3d/shaders/depth.fragment.glsl").readString();
		return defaultFragmentShader;
	}

	public static String createPrefix ( Renderable renderable,  Config config) {
		String prefix = DefaultShader.createPrefix(renderable, config);
		if (!config.depthBufferOnly) prefix += "#define PackedDepthFlag\n";
		return prefix;
	}

	public readonly int numBones;
	private readonly FloatAttribute alphaTestAttribute;

	public DepthShader ( Renderable renderable) 
    : this(renderable, new Config())
    {
		
	}

	public DepthShader ( Renderable renderable,  Config config) 
    : this(renderable, config, createPrefix(renderable, config))
    {
		
	}

	public DepthShader ( Renderable renderable,  Config config,  String prefix) 
    : this(renderable, config, prefix, config.vertexShader != null ? config.vertexShader : getDefaultVertexShader(),
        config.fragmentShader != null ? config.fragmentShader : getDefaultFragmentShader())
    {
		
	}

	public DepthShader ( Renderable renderable,  Config config,  String prefix,  String vertexShader,
		 String fragmentShader) 
    : this(renderable, config, new ShaderProgram(prefix + vertexShader, prefix + fragmentShader))
    {
		
	}

	public DepthShader ( Renderable renderable,  Config config,  ShaderProgram shaderProgram) 
    : base(renderable, config, shaderProgram)
    {
		
		Attributes attributes = combineAttributes(renderable);

		if (renderable.bones != null && renderable.bones.Length > config.numBones) {
			throw new GdxRuntimeException("too many bones: " + renderable.bones.Length + ", max configured: " + config.numBones);
		}

		this.numBones = renderable.bones == null ? 0 : config.numBones;
		int boneWeights = renderable.meshPart.mesh.getVertexAttributes().GetBoneWeights();
		if (boneWeights > config.numBoneWeights) {
			throw new GdxRuntimeException("too many bone weights: " + boneWeights + ", max configured: " + config.numBoneWeights);
		}
		alphaTestAttribute = new FloatAttribute(FloatAttribute.AlphaTest, config.defaultAlphaTest);
	}

	public override void begin (Camera camera, RenderContext context) {
		base.begin(camera, context);
		// Gdx.gl20.glEnable(GL20.GL_POLYGON_OFFSET_FILL);
		// Gdx.gl20.glPolygonOffset(2.f, 100.f);
	}

	public override void end () {
		base.end();
		// Gdx.gl20.glDisable(GL20.GL_POLYGON_OFFSET_FILL);
	}

	// TODO: Ensure that this is overriding. -RP
	public override bool canRender (Renderable renderable) {
		if (renderable.bones != null) {
			if (renderable.bones.Length > config.numBones) return false;
			if (renderable.meshPart.mesh.getVertexAttributes().GetBoneWeights() > config.numBoneWeights) return false;
		}
		Attributes attributes = combineAttributes(renderable);
		if (attributes.has(BlendingAttribute.Type)) {
			if ((attributesMask & BlendingAttribute.Type) != BlendingAttribute.Type) return false;
			if (attributes
				.has(TextureAttribute.Diffuse) != ((attributesMask & TextureAttribute.Diffuse) == TextureAttribute.Diffuse))
				return false;
		}
		return (renderable.bones != null) == (numBones > 0);
	}

    // TODO: Ensure that this is overriding. -RP
public override void render (Renderable renderable, Attributes combinedAttributes) {
		if (combinedAttributes.has(BlendingAttribute.Type)) {
			BlendingAttribute blending = (BlendingAttribute)combinedAttributes.get(BlendingAttribute.Type);
			combinedAttributes.remove(BlendingAttribute.Type);
			 bool hasAlphaTest = combinedAttributes.has(FloatAttribute.AlphaTest);
			if (!hasAlphaTest) combinedAttributes.set(alphaTestAttribute);
			if (blending.opacity >= ((FloatAttribute)combinedAttributes.get(FloatAttribute.AlphaTest)).value)
				base.render(renderable, combinedAttributes);
			if (!hasAlphaTest) combinedAttributes.remove(FloatAttribute.AlphaTest);
			combinedAttributes.set(blending);
		} else
			base.render(renderable, combinedAttributes);
	}

	private readonly static Attributes tmpAttributes = new Attributes();

	// TODO: Move responsibility for combining attributes to RenderableProvider
	private static  Attributes combineAttributes ( Renderable renderable) {
		tmpAttributes.clear();
		if (renderable.environment != null) tmpAttributes.set(renderable.environment);
		if (renderable.material != null) tmpAttributes.set(renderable.material);
		return tmpAttributes;
	}
}
