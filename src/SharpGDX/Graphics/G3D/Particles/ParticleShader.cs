using System;
using SharpGDX.Graphics.G3D.Utils;
using SharpGDX.Graphics.G3D.Attributess;
using SharpGDX.Utils.Reflect;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G3D.Models.Data;
using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Graphics.G3D.Shaders;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Environments;

namespace SharpGDX.Graphics.G3D.Particles;

/** This is a custom shader to render the particles. Usually is not required, because the {@link DefaultShader} will be used
 * instead. This shader will be used when dealing with billboards using GPU mode or point sprites.
 * @author inferno */
public class ParticleShader : BaseShader {
	public enum ParticleType {
		Billboard, Point
	}

	public enum AlignMode {
		Screen, ViewPoint// , ParticleDirection
	}

	public class Config {
		/** The uber vertex shader to use, null to use the default vertex shader. */
		public String vertexShader = null;
		/** The uber fragment shader to use, null to use the default fragment shader. */
		public String fragmentShader = null;
		public bool ignoreUnimplemented = true;
		/** Set to 0 to disable culling */
		public int defaultCullFace = -1;
		/** Set to 0 to disable depth test */
		public int defaultDepthFunc = -1;
		public AlignMode align = AlignMode.Screen;
		public ParticleType type = ParticleType.Billboard;

		public Config () {
		}

		public Config (AlignMode align, ParticleType type) {
			this.align = align;
			this.type = type;
		}

		public Config (AlignMode align) {
			this.align = align;
		}

		public Config (ParticleType type) {
			this.type = type;
		}

		public Config ( String vertexShader,  String fragmentShader) {
			this.vertexShader = vertexShader;
			this.fragmentShader = fragmentShader;
		}
	}

	private static String defaultVertexShader = null;

	public static String getDefaultVertexShader () {
		if (defaultVertexShader == null)
			defaultVertexShader = GDX.Files.Classpath("com/badlogic/gdx/graphics/g3d/particles/particles.vertex.glsl").readString();
		return defaultVertexShader;
	}

	private static String defaultFragmentShader = null;

	public static String getDefaultFragmentShader () {
		if (defaultFragmentShader == null) defaultFragmentShader = GDX.Files
			.Classpath("com/badlogic/gdx/graphics/g3d/particles/particles.fragment.glsl").readString();
		return defaultFragmentShader;
	}

	protected static long implementedFlags = BlendingAttribute.Type | TextureAttribute.Diffuse;

	static readonly Vector3 TMP_VECTOR3 = new Vector3();

	public static class Inputs {
		public readonly static Uniform cameraRight = new Uniform("u_cameraRight");
		public readonly static Uniform cameraInvDirection = new Uniform("u_cameraInvDirection");
		public readonly static Uniform screenWidth = new Uniform("u_screenWidth");
		public readonly static Uniform regionSize = new Uniform("u_regionSize");
	}

    public static class Setters {
        private class CameraRightSetter : Setter
        {
            public bool isGlobal(BaseShader shader, int inputID)
            {
                return true;
            }

            public void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, TMP_VECTOR3.Set(shader.camera.direction).crs(shader.camera.up).nor());
            }
        }

        public readonly static Setter cameraRight = new CameraRightSetter();

        private class CameraUpSetter : Setter
        {
            public bool isGlobal(BaseShader shader, int inputID)
            {
                return true;
            }

            public void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, TMP_VECTOR3.Set(shader.camera.up).nor());
            }
}

		public readonly static Setter cameraUp = new CameraUpSetter();

        private class CameraInvDirectionSetter : Setter
        {
            public bool isGlobal(BaseShader shader, int inputID)
            {
                return true;
            }

            public void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID,
                    TMP_VECTOR3.Set(-shader.camera.direction.x, -shader.camera.direction.y, -shader.camera.direction.z).nor());
            }
        }
        
        public readonly static Setter cameraInvDirection = new CameraInvDirectionSetter();

        private class CameraPositionSetter : Setter
        {
            public bool isGlobal(BaseShader shader, int inputID)
            {
                return true;
            }

            public void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, shader.camera.position);
            }
        }

		public readonly static Setter cameraPosition = new CameraPositionSetter();

        private class ScreenWidthSetter : Setter
        {
            public bool isGlobal(BaseShader shader, int inputID)
            {
                return true;
            }

            public void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, (float)GDX.Graphics.GetWidth());
            }
        }

		public readonly static Setter screenWidth = new ScreenWidthSetter();

        private class WorldViewTransSetter : Setter
        {
            private readonly Matrix4 temp = new Matrix4();

            public bool isGlobal(BaseShader shader, int inputID)
            {
                return false;
            }

            public void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, temp.set(shader.camera.view).mul(renderable.worldTransform));
            }
        }

		public readonly static Setter worldViewTrans = new WorldViewTransSetter() ;
	}

	/** The renderable used to create this shader, invalid after the call to init */
	private Renderable renderable;
	private long materialMask;
	private long vertexMask;
	protected readonly Config config;
	/** Material attributes which are not required but always supported. */
	private readonly static long optionalAttributes = IntAttribute.CullFace | DepthTestAttribute.Type;

	public ParticleShader ( Renderable renderable) 
    : this(renderable, new Config())
    {
		
	}

	public ParticleShader ( Renderable renderable,  Config config) 
    : this(renderable, config, createPrefix(renderable, config))
    {
		
	}

	public ParticleShader ( Renderable renderable,  Config config,  String prefix) 
    : this(renderable, config, prefix, config.vertexShader != null ? config.vertexShader : getDefaultVertexShader(),
        config.fragmentShader != null ? config.fragmentShader : getDefaultFragmentShader())
    {
		
	}

	public ParticleShader ( Renderable renderable,  Config config,  String prefix,  String vertexShader,
		 String fragmentShader) 
    : this(renderable, config, new ShaderProgram(prefix + vertexShader, prefix + fragmentShader))
    {
		
	}

	public ParticleShader ( Renderable renderable,  Config config,  ShaderProgram shaderProgram) {
		this.config = config;
		this.program = shaderProgram;
		this.renderable = renderable;
		materialMask = renderable.material.getMask() | optionalAttributes;
		vertexMask = renderable.meshPart.mesh.getVertexAttributes().GetMask();

		if (!config.ignoreUnimplemented && (implementedFlags & materialMask) != materialMask)
			throw new GdxRuntimeException("Some attributes not implemented yet (" + materialMask + ")");

		// Global uniforms
		register(DefaultShader.Inputs.viewTrans, DefaultShader.Setters.viewTrans);
		register(DefaultShader.Inputs.projViewTrans, DefaultShader.Setters.projViewTrans);
		register(DefaultShader.Inputs.projTrans, DefaultShader.Setters.projTrans);
		register(Inputs.screenWidth, Setters.screenWidth);
		register(DefaultShader.Inputs.cameraUp, Setters.cameraUp);
		register(Inputs.cameraRight, Setters.cameraRight);
		register(Inputs.cameraInvDirection, Setters.cameraInvDirection);
		register(DefaultShader.Inputs.cameraPosition, Setters.cameraPosition);

		// Object uniforms
		register(DefaultShader.Inputs.diffuseTexture, DefaultShader.Setters.diffuseTexture);
	}

	public override void init () {
		ShaderProgram program = this.program;
		this.program = null;
		init(program, renderable);
		renderable = null;
	}

	public static String createPrefix ( Renderable renderable,  Config config) {
		String prefix = "";
		if (GDX.App.GetType() == IApplication.ApplicationType.Desktop)
			prefix += "#version 120\n";
		else
			prefix += "#version 100\n";
		if (config.type == ParticleType.Billboard) {
			prefix += "#define billboard\n";
			if (config.align == AlignMode.Screen)
				prefix += "#define screenFacing\n";
			else if (config.align == AlignMode.ViewPoint) prefix += "#define viewPointFacing\n";
			// else if(config.align == AlignMode.ParticleDirection)
			// prefix += "#define paticleDirectionFacing\n";
		}
		return prefix;
	}

    public override bool canRender ( Renderable renderable) {
		return (materialMask == (renderable.material.getMask() | optionalAttributes))
			&& (vertexMask == renderable.meshPart.mesh.getVertexAttributes().GetMask());
	}

    public override int compareTo (Shader other) {
		if (other == null) return -1;
		if (other == this) return 0;
		return 0; // FIXME compare shaders on their impact on performance
	}

	public override bool Equals (Object? obj) {
		return (obj is ParticleShader) && Equals((ParticleShader)obj);
	}

	public bool Equals (ParticleShader obj) {
		return (obj == this);
	}

	public override void begin ( Camera camera,  RenderContext context) {
		base.begin(camera, context);
	}

	public override void render ( Renderable renderable) {
		if (!renderable.material.has(BlendingAttribute.Type))
			context.setBlending(false, IGL20.GL_SRC_ALPHA, IGL20.GL_ONE_MINUS_SRC_ALPHA);
		bindMaterial(renderable);
		base.render(renderable);
	}

	public override void end () {
		currentMaterial = null;
		base.end();
	}

	Material currentMaterial;

	protected void bindMaterial ( Renderable renderable) {
		if (currentMaterial == renderable.material) return;

		int cullFace = config.defaultCullFace == -1 ? IGL20.GL_BACK : config.defaultCullFace;
		int depthFunc = config.defaultDepthFunc == -1 ? IGL20.GL_LEQUAL : config.defaultDepthFunc;
		float depthRangeNear = 0f;
		float depthRangeFar = 1f;
		bool depthMask = true;

		currentMaterial = renderable.material;
		foreach ( Attribute attr in currentMaterial) {
			 long t = attr.type;
			if (BlendingAttribute.@is(t)) {
				context.setBlending(true, ((BlendingAttribute)attr).sourceFunction, ((BlendingAttribute)attr).destFunction);
			} else if ((t & DepthTestAttribute.Type) == DepthTestAttribute.Type) {
				DepthTestAttribute dta = (DepthTestAttribute)attr;
				depthFunc = dta.depthFunc;
				depthRangeNear = dta.depthRangeNear;
				depthRangeFar = dta.depthRangeFar;
				depthMask = dta.depthMask;
			} else if (!config.ignoreUnimplemented) throw new GdxRuntimeException("Unknown material attribute: " + attr.ToString());
		}

		context.setCullFace(cullFace);
		context.setDepthTest(depthFunc, depthRangeNear, depthRangeFar);
		context.setDepthMask(depthMask);
	}

	public override void Dispose () {
		program.Dispose();
		base.Dispose();
	}

	public int getDefaultCullFace () {
		return config.defaultCullFace == -1 ? IGL20.GL_BACK : config.defaultCullFace;
	}

	public void setDefaultCullFace (int cullFace) {
		config.defaultCullFace = cullFace;
	}

	public int getDefaultDepthFunc () {
		return config.defaultDepthFunc == -1 ? IGL20.GL_LEQUAL : config.defaultDepthFunc;
	}

	public void setDefaultDepthFunc (int depthFunc) {
		config.defaultDepthFunc = depthFunc;
	}
}
