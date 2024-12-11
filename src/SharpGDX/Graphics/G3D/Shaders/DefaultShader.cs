using System;
using static SharpGDX.Graphics.VertexAttributes;
using SharpGDX.Graphics.G3D.Attributess;
using SharpGDX.Graphics.G3D.Environments;
using SharpGDX.Graphics.G3D;
using SharpGDX.Graphics.G3D.Attributess;
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

public class DefaultShader : BaseShader {
	public class Config {
		/** The uber vertex shader to use, null to use the default vertex shader. */
		public String vertexShader = null;
		/** The uber fragment shader to use, null to use the default fragment shader. */
		public String fragmentShader = null;
		/** The number of directional lights to use */
		public int numDirectionalLights = 2;
		/** The number of point lights to use */
		public int numPointLights = 5;
		/** The number of spot lights to use */
		public int numSpotLights = 0;
		/** The number of bones to use */
		public int numBones = 12;
		/** The number of bone weights to use (up to 8 with default vertex shader), default is 4. */
		public int numBoneWeights = 4;
		/** */
		public bool ignoreUnimplemented = true;
		/** Set to 0 to disable culling, -1 to inherit from {@link DefaultShader#defaultCullFace} */
		public int defaultCullFace = -1;
		/** Set to 0 to disable depth test, -1 to inherit from {@link DefaultShader#defaultDepthFunc} */
		public int defaultDepthFunc = -1;

		public Config () {
		}

		public Config ( String vertexShader,  String fragmentShader) {
			this.vertexShader = vertexShader;
			this.fragmentShader = fragmentShader;
		}
	}

	public static class Inputs {
		public readonly static Uniform projTrans = new Uniform("u_projTrans");
		public readonly static Uniform viewTrans = new Uniform("u_viewTrans");
		public readonly static Uniform projViewTrans = new Uniform("u_projViewTrans");
		public readonly static Uniform cameraPosition = new Uniform("u_cameraPosition");
		public readonly static Uniform cameraDirection = new Uniform("u_cameraDirection");
		public readonly static Uniform cameraUp = new Uniform("u_cameraUp");
		public readonly static Uniform cameraNearFar = new Uniform("u_cameraNearFar");

		public readonly static Uniform worldTrans = new Uniform("u_worldTrans");
		public readonly static Uniform viewWorldTrans = new Uniform("u_viewWorldTrans");
		public readonly static Uniform projViewWorldTrans = new Uniform("u_projViewWorldTrans");
		public readonly static Uniform normalMatrix = new Uniform("u_normalMatrix");
		public readonly static Uniform bones = new Uniform("u_bones");

		public readonly static Uniform shininess = new Uniform("u_shininess", FloatAttribute.Shininess);
		public readonly static Uniform opacity = new Uniform("u_opacity", BlendingAttribute.Type);
		public readonly static Uniform diffuseColor = new Uniform("u_diffuseColor", ColorAttribute.Diffuse);
		public readonly static Uniform diffuseTexture = new Uniform("u_diffuseTexture", TextureAttribute.Diffuse);
		public readonly static Uniform diffuseUVTransform = new Uniform("u_diffuseUVTransform", TextureAttribute.Diffuse);
		public readonly static Uniform specularColor = new Uniform("u_specularColor", ColorAttribute.Specular);
		public readonly static Uniform specularTexture = new Uniform("u_specularTexture", TextureAttribute.Specular);
		public readonly static Uniform specularUVTransform = new Uniform("u_specularUVTransform", TextureAttribute.Specular);
		public readonly static Uniform emissiveColor = new Uniform("u_emissiveColor", ColorAttribute.Emissive);
		public readonly static Uniform emissiveTexture = new Uniform("u_emissiveTexture", TextureAttribute.Emissive);
		public readonly static Uniform emissiveUVTransform = new Uniform("u_emissiveUVTransform", TextureAttribute.Emissive);
		public readonly static Uniform reflectionColor = new Uniform("u_reflectionColor", ColorAttribute.Reflection);
		public readonly static Uniform reflectionTexture = new Uniform("u_reflectionTexture", TextureAttribute.Reflection);
		public readonly static Uniform reflectionUVTransform = new Uniform("u_reflectionUVTransform", TextureAttribute.Reflection);
		public readonly static Uniform normalTexture = new Uniform("u_normalTexture", TextureAttribute.Normal);
		public readonly static Uniform normalUVTransform = new Uniform("u_normalUVTransform", TextureAttribute.Normal);
		public readonly static Uniform ambientTexture = new Uniform("u_ambientTexture", TextureAttribute.Ambient);
		public readonly static Uniform ambientUVTransform = new Uniform("u_ambientUVTransform", TextureAttribute.Ambient);
		public readonly static Uniform alphaTest = new Uniform("u_alphaTest");

		public readonly static Uniform ambientCube = new Uniform("u_ambientCubemap");
		public readonly static Uniform dirLights = new Uniform("u_dirLights");
		public readonly static Uniform pointLights = new Uniform("u_pointLights");
		public readonly static Uniform spotLights = new Uniform("u_spotLights");
		public readonly static Uniform environmentCubemap = new Uniform("u_environmentCubemap");
	}

	public class Setters {

        private class ProjTransSetter : GlobalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, shader.camera.projection);
            }
        }

		public readonly static Setter projTrans = new ProjTransSetter();

        private class ViewTransSetter : GlobalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, shader.camera.view);
            }
        }

		public readonly static Setter viewTrans = new ViewTransSetter() {
			
		};

        private class ProjViewTransSetter : GlobalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, shader.camera.Combined);
            }
        }

		public readonly static Setter projViewTrans = new ProjViewTransSetter() {
			
		};

        private class CameraPositionSetter : GlobalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, shader.camera.position.x, shader.camera.position.y, shader.camera.position.z,
                    1.1881f / (shader.camera.far * shader.camera.far));
            }
        }

		public readonly static Setter cameraPosition = new CameraPositionSetter() {
			
		};

        private class CameraDirectionSetter : GlobalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, shader.camera.direction);
            }
        }

		public readonly static Setter cameraDirection = new CameraDirectionSetter() {
			
		};

        private class CameraUpSetter : GlobalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, shader.camera.up);
            }
        }

		public readonly static Setter cameraUp = new CameraUpSetter() {
			
		};

        private class CameraNearFarSetter : GlobalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, shader.camera.near, shader.camera.far);
            }
        }

		public readonly static Setter cameraNearFar = new CameraNearFarSetter() {
			
		};

        private class WorldTransSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, renderable.worldTransform);
            }
        }

		public readonly static Setter worldTrans = new WorldTransSetter() {
			
		};

        private class ViewWorldTransSetter : LocalSetter
        {
            readonly Matrix4 temp = new Matrix4();

            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, temp.set(shader.camera.view).mul(renderable.worldTransform));
            }
        }

		public readonly static Setter viewWorldTrans = new ViewWorldTransSetter() {
            
		};

        private class ProjViewWorldTransSetter : LocalSetter
        {
            readonly Matrix4 temp = new Matrix4();

            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, temp.set(shader.camera.Combined).mul(renderable.worldTransform));
            }
        }

		public readonly static Setter projViewWorldTrans = new ProjViewWorldTransSetter() {
            
		};

        private class NormalMatrixSetter : LocalSetter
        {
            private readonly Matrix3 tmpM = new Matrix3();

            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, tmpM.set(renderable.worldTransform).inv().transpose());
            }
        }

		public readonly static Setter normalMatrix = new NormalMatrixSetter() {
			
		};

		public class Bones : LocalSetter {
			private readonly static Matrix4 idtMatrix = new Matrix4();
			public readonly float[] bones;

			public Bones (int numBones) {
				this.bones = new float[numBones * 16];
			}

			public override void set (BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes) {
				for (int i = 0; i < bones.Length; i += 16) {
					int idx = i / 16;
					if (renderable.bones == null || idx >= renderable.bones.Length || renderable.bones[idx] == null)
						Array.Copy(idtMatrix.val, 0, bones, i, 16);
					else
						Array.Copy(renderable.bones[idx].val, 0, bones, i, 16);
				}
				shader.program.setUniformMatrix4fv(shader.loc(inputID), bones, 0, bones.Length);
			}
		}

        private class ShininessSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, ((FloatAttribute)(combinedAttributes.get(FloatAttribute.Shininess))).value);
            }
        }

		public readonly static Setter shininess = new ShininessSetter();

        private class DiffuseColorSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, ((ColorAttribute)(combinedAttributes.get(ColorAttribute.Diffuse))).color);
            }
        }

		public readonly static Setter diffuseColor = new DiffuseColorSetter() ;

        private class DiffuseTextureSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                int unit = shader.context.textureBinder
                    .bind(((TextureAttribute)(combinedAttributes.get(TextureAttribute.Diffuse))).textureDescription);
                shader.set(inputID, unit);
            }
        }

		public readonly static Setter diffuseTexture = new DiffuseTextureSetter();

        private class DiffuseUVTransformSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                TextureAttribute ta = (TextureAttribute)(combinedAttributes.get(TextureAttribute.Diffuse));
                shader.set(inputID, ta.offsetU, ta.offsetV, ta.scaleU, ta.scaleV);
            }
        }

		public readonly static Setter diffuseUVTransform = new DiffuseUVTransformSetter();

        private class SpecularColorSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, ((ColorAttribute)(combinedAttributes.get(ColorAttribute.Specular))).color);
            }
        }

		public readonly static Setter specularColor = new SpecularColorSetter();

        private class SpecularTextureSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                int unit = shader.context.textureBinder
                    .bind((TextureDescriptor)((TextureAttribute)(combinedAttributes.get(TextureAttribute.Specular))).textureDescription);
                shader.set(inputID, unit);
            }
        }

		public readonly static Setter specularTexture = new SpecularTextureSetter() ;

        private class SpecularUVTransform : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                TextureAttribute ta = (TextureAttribute)(combinedAttributes.get(TextureAttribute.Specular));
                shader.set(inputID, ta.offsetU, ta.offsetV, ta.scaleU, ta.scaleV);
            }
        }

		public readonly static Setter specularUVTransform = new SpecularUVTransform();

        private class EmissiveColorSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, ((ColorAttribute)(combinedAttributes.get(ColorAttribute.Emissive))).color);
            }
        }

		public readonly static Setter emissiveColor = new EmissiveColorSetter() ;

        private class EmissiveTextureSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                int unit = shader.context.textureBinder
                    .bind((TextureDescriptor)((TextureAttribute)(combinedAttributes.get(TextureAttribute.Emissive))).textureDescription);
                shader.set(inputID, unit);
            }
        }

		public readonly static Setter emissiveTexture = new EmissiveTextureSetter();

        private class EmissiveUVTranformSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                TextureAttribute ta = (TextureAttribute)(combinedAttributes.get(TextureAttribute.Emissive));
                shader.set(inputID, ta.offsetU, ta.offsetV, ta.scaleU, ta.scaleV);
            }
        }

		public readonly static Setter emissiveUVTransform = new EmissiveUVTranformSetter();

        private class ReflectionColorSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                shader.set(inputID, ((ColorAttribute)(combinedAttributes.get(ColorAttribute.Reflection))).color);
            }
        }

		public readonly static Setter reflectionColor = new ReflectionColorSetter();

        private class ReflectionTextureSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                int unit = shader.context.textureBinder
                    .bind((TextureDescriptor)((TextureAttribute)(combinedAttributes.get(TextureAttribute.Reflection))).textureDescription);
                shader.set(inputID, unit);
            }
        }

		public readonly static Setter reflectionTexture = new ReflectionTextureSetter();

        private class ReflectionUVTransformSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                 TextureAttribute ta = (TextureAttribute)(combinedAttributes.get(TextureAttribute.Reflection));
                shader.set(inputID, ta.offsetU, ta.offsetV, ta.scaleU, ta.scaleV);
            }
        }

		public readonly static Setter reflectionUVTransform = new ReflectionUVTransformSetter();

        private class NormalTextureSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                int unit = shader.context.textureBinder
                    .bind((TextureDescriptor)((TextureAttribute)(combinedAttributes.get(TextureAttribute.Normal))).textureDescription);
                shader.set(inputID, unit);
            }
        }

		public readonly static Setter normalTexture = new NormalTextureSetter() ;

        private class NormalUVTransformSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                TextureAttribute ta = (TextureAttribute)(combinedAttributes.get(TextureAttribute.Normal));
                shader.set(inputID, ta.offsetU, ta.offsetV, ta.scaleU, ta.scaleV);
            }
        }

		public readonly static Setter normalUVTransform = new NormalUVTransformSetter();

        private class AmbientTextureSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                 int unit = shader.context.textureBinder
                    .bind((TextureDescriptor)((TextureAttribute)(combinedAttributes.get(TextureAttribute.Ambient))).textureDescription);
                shader.set(inputID, unit);
            }
        }

		public readonly static Setter ambientTexture = new AmbientTextureSetter();

        private class AmbientUVTransformSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                TextureAttribute ta = (TextureAttribute)(combinedAttributes.get(TextureAttribute.Ambient));
                shader.set(inputID, ta.offsetU, ta.offsetV, ta.scaleU, ta.scaleV);
            }
        }

		public readonly static Setter ambientUVTransform = new AmbientUVTransformSetter();

		public class ACubemap : LocalSetter {
			private readonly static float[] ones = {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
			private readonly AmbientCubemap cacheAmbientCubemap = new AmbientCubemap();
			private readonly static Vector3 tmpV1 = new Vector3();
			public readonly int dirLightsOffset;
			public readonly int pointLightsOffset;

			public ACubemap ( int dirLightsOffset, int pointLightsOffset) {
				this.dirLightsOffset = dirLightsOffset;
				this.pointLightsOffset = pointLightsOffset;
			}

			public override void set (BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes) {
				if (renderable.environment == null)
					shader.program.setUniform3fv(shader.loc(inputID), ones, 0, ones.Length);
				else {
					renderable.worldTransform.getTranslation(tmpV1);
					if (combinedAttributes.has(ColorAttribute.AmbientLight))
						cacheAmbientCubemap.set(((ColorAttribute)combinedAttributes.get(ColorAttribute.AmbientLight)).color);

					if (combinedAttributes.has(DirectionalLightsAttribute.Type)) {
						Array<DirectionalLight> lights = ((DirectionalLightsAttribute)combinedAttributes
							.get(DirectionalLightsAttribute.Type)).lights;
						for (int i = dirLightsOffset; i < lights.size; i++)
							cacheAmbientCubemap.add(lights.Get(i).color, lights.Get(i).direction);
					}

					if (combinedAttributes.has(PointLightsAttribute.Type)) {
						Array<PointLight> lights = ((PointLightsAttribute)combinedAttributes.get(PointLightsAttribute.Type)).lights;
						for (int i = pointLightsOffset; i < lights.size; i++)
							cacheAmbientCubemap.add(lights.Get(i).color, lights.Get(i).position, tmpV1, lights.Get(i).intensity);
					}

					cacheAmbientCubemap.clamp();
					shader.program.setUniform3fv(shader.loc(inputID), cacheAmbientCubemap.data, 0, cacheAmbientCubemap.data.Length);
				}
			}
		}

        private class EnvironmentCubemapSetter : LocalSetter
        {
            public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
            {
                if (combinedAttributes.has(CubemapAttribute.EnvironmentMap))
                {
                    shader.set(inputID, shader.context.textureBinder
                        .bind(((CubemapAttribute)combinedAttributes.get(CubemapAttribute.EnvironmentMap)).textureDescription));
                }
            }
        }

		public readonly static Setter environmentCubemap = new EnvironmentCubemapSetter();
	}

	private static String defaultVertexShader = null;

	public static String getDefaultVertexShader () {
		if (defaultVertexShader == null)
			defaultVertexShader = GDX.Files.Classpath("com/badlogic/gdx/graphics/g3d/shaders/default.vertex.glsl").readString();
		return defaultVertexShader;
	}

	private static String defaultFragmentShader = null;

	public static String getDefaultFragmentShader () {
		if (defaultFragmentShader == null)
			defaultFragmentShader = GDX.Files.Classpath("com/badlogic/gdx/graphics/g3d/shaders/default.fragment.glsl").readString();
		return defaultFragmentShader;
	}

	protected static long implementedFlags = BlendingAttribute.Type | TextureAttribute.Diffuse | ColorAttribute.Diffuse
		| ColorAttribute.Specular | FloatAttribute.Shininess;

	/** @deprecated Replaced by {@link Config#defaultCullFace} Set to 0 to disable culling */
	[Obsolete] public static int defaultCullFace = IGL20.GL_BACK;
	/** @deprecated Replaced by {@link Config#defaultDepthFunc} Set to 0 to disable depth test */
	[Obsolete] public static int defaultDepthFunc = IGL20.GL_LEQUAL;

	// Global uniforms
	public readonly int u_projTrans;
	public readonly int u_viewTrans;
	public readonly int u_projViewTrans;
	public readonly int u_cameraPosition;
	public readonly int u_cameraDirection;
	public readonly int u_cameraUp;
	public readonly int u_cameraNearFar;
	public readonly int u_time;
	// Object uniforms
	public readonly int u_worldTrans;
	public readonly int u_viewWorldTrans;
	public readonly int u_projViewWorldTrans;
	public readonly int u_normalMatrix;
	public readonly int u_bones;
	// Material uniforms
	public readonly int u_shininess;
	public readonly int u_opacity;
	public readonly int u_diffuseColor;
	public readonly int u_diffuseTexture;
	public readonly int u_diffuseUVTransform;
	public readonly int u_specularColor;
	public readonly int u_specularTexture;
	public readonly int u_specularUVTransform;
	public readonly int u_emissiveColor;
	public readonly int u_emissiveTexture;
	public readonly int u_emissiveUVTransform;
	public readonly int u_reflectionColor;
	public readonly int u_reflectionTexture;
	public readonly int u_reflectionUVTransform;
	public readonly int u_normalTexture;
	public readonly int u_normalUVTransform;
	public readonly int u_ambientTexture;
	public readonly int u_ambientUVTransform;
	public readonly int u_alphaTest;
	// Lighting uniforms
	protected readonly int u_ambientCubemap;
	protected readonly int u_environmentCubemap;
	protected readonly int u_dirLights0color ;
	protected readonly int u_dirLights0direction ;
	protected readonly int u_dirLights1color ;
	protected readonly int u_pointLights0color ;
	protected readonly int u_pointLights0position ;
	protected readonly int u_pointLights0intensity ;
	protected readonly int u_pointLights1color ;
	protected readonly int u_spotLights0color ;
	protected readonly int u_spotLights0position ;
	protected readonly int u_spotLights0intensity ;
	protected readonly int u_spotLights0direction ;
	protected readonly int u_spotLights0cutoffAngle ;
	protected readonly int u_spotLights0exponent ;
	protected readonly int u_spotLights1color ;
	protected readonly int u_fogColor ;
	protected readonly int u_shadowMapProjViewTrans ;
	protected readonly int u_shadowTexture ;
	protected readonly int u_shadowPCFOffset ;
	// FIXME Cache vertex attribute locations...

	protected int dirLightsLoc;
	protected int dirLightsColorOffset;
	protected int dirLightsDirectionOffset;
	protected int dirLightsSize;
	protected int pointLightsLoc;
	protected int pointLightsColorOffset;
	protected int pointLightsPositionOffset;
	protected int pointLightsIntensityOffset;
	protected int pointLightsSize;
	protected int spotLightsLoc;
	protected int spotLightsColorOffset;
	protected int spotLightsPositionOffset;
	protected int spotLightsDirectionOffset;
	protected int spotLightsIntensityOffset;
	protected int spotLightsCutoffAngleOffset;
	protected int spotLightsExponentOffset;
	protected int spotLightsSize;

	protected readonly bool lighting;
	protected readonly bool environmentCubemap;
	protected readonly bool shadowMap;
	protected readonly AmbientCubemap ambientCubemap = new AmbientCubemap();
	protected readonly DirectionalLight[] directionalLights;
	protected readonly PointLight[] pointLights;
	protected readonly SpotLight[] spotLights;

	/** The renderable used to create this shader, invalid after the call to init */
	private Renderable renderable;
	/** The attributes that this shader supports */
	protected readonly long attributesMask;
	private readonly long vertexMask;
	private readonly int textureCoordinates;
	private int[] boneWeightsLocations;
	protected readonly Config config;
	/** Attributes which are not required but always supported. */
	private readonly static long optionalAttributes = IntAttribute.CullFace | DepthTestAttribute.Type;

	public DefaultShader ( Renderable renderable) 
    : this(renderable, new Config())
    {
		
	}

	public DefaultShader ( Renderable renderable,  Config config) 
    : this(renderable, config, createPrefix(renderable, config))
    {
		
	}

	public DefaultShader ( Renderable renderable,  Config config,  String prefix) 
    : this(renderable, config, prefix, config.vertexShader != null ? config.vertexShader : getDefaultVertexShader(),
        config.fragmentShader != null ? config.fragmentShader : getDefaultFragmentShader())
    {
		
	}

	public DefaultShader ( Renderable renderable,  Config config,  String prefix,  String vertexShader,
		 String fragmentShader) 
    : this(renderable, config, new ShaderProgram(prefix + vertexShader, prefix + fragmentShader))
    {
		
	}

	public DefaultShader ( Renderable renderable,  Config config,  ShaderProgram shaderProgram) {


     u_dirLights0color = register(new Uniform("u_dirLights[0].color"));
     u_dirLights0direction = register(new Uniform("u_dirLights[0].direction"));
     u_dirLights1color = register(new Uniform("u_dirLights[1].color"));
     u_pointLights0color = register(new Uniform("u_pointLights[0].color"));
     u_pointLights0position = register(new Uniform("u_pointLights[0].position"));
     u_pointLights0intensity = register(new Uniform("u_pointLights[0].intensity"));
     u_pointLights1color = register(new Uniform("u_pointLights[1].color"));
     u_spotLights0color = register(new Uniform("u_spotLights[0].color"));
     u_spotLights0position = register(new Uniform("u_spotLights[0].position"));
     u_spotLights0intensity = register(new Uniform("u_spotLights[0].intensity"));
     u_spotLights0direction = register(new Uniform("u_spotLights[0].direction"));
     u_spotLights0cutoffAngle = register(new Uniform("u_spotLights[0].cutoffAngle"));
     u_spotLights0exponent = register(new Uniform("u_spotLights[0].exponent"));
     u_spotLights1color = register(new Uniform("u_spotLights[1].color"));
     u_fogColor = register(new Uniform("u_fogColor"));
     u_shadowMapProjViewTrans = register(new Uniform("u_shadowMapProjViewTrans"));
     u_shadowTexture = register(new Uniform("u_shadowTexture"));
     u_shadowPCFOffset = register(new Uniform("u_shadowPCFOffset"));




Attributes attributes = combineAttributes(renderable);
		this.config = config;
		this.program = shaderProgram;
		this.lighting = renderable.environment != null;
		this.environmentCubemap = attributes.has(CubemapAttribute.EnvironmentMap)
			|| (lighting && attributes.has(CubemapAttribute.EnvironmentMap));
		this.shadowMap = lighting && renderable.environment.shadowMap != null;
		this.renderable = renderable;
		attributesMask = attributes.getMask() | optionalAttributes;
		vertexMask = renderable.meshPart.mesh.getVertexAttributes().GetMaskWithSizePacked();
		textureCoordinates = renderable.meshPart.mesh.getVertexAttributes().GetTextureCoordinates();

		this.directionalLights = new DirectionalLight[lighting && config.numDirectionalLights > 0 ? config.numDirectionalLights
			: 0];
		for (int i = 0; i < directionalLights.Length; i++)
			directionalLights[i] = new DirectionalLight();
		this.pointLights = new PointLight[lighting && config.numPointLights > 0 ? config.numPointLights : 0];
		for (int i = 0; i < pointLights.Length; i++)
			pointLights[i] = new PointLight();
		this.spotLights = new SpotLight[lighting && config.numSpotLights > 0 ? config.numSpotLights : 0];
		for (int i = 0; i < spotLights.Length; i++)
			spotLights[i] = new SpotLight();

		if (!config.ignoreUnimplemented && (implementedFlags & attributesMask) != attributesMask)
			throw new GdxRuntimeException("Some attributes not implemented yet (" + attributesMask + ")");

		if (renderable.bones != null && renderable.bones.Length > config.numBones) {
			throw new GdxRuntimeException("too many bones: " + renderable.bones.Length + ", max configured: " + config.numBones);
		}

		int boneWeights = renderable.meshPart.mesh.getVertexAttributes().GetBoneWeights();
		if (boneWeights > config.numBoneWeights) {
			throw new GdxRuntimeException("too many bone weights: " + boneWeights + ", max configured: " + config.numBoneWeights);
		}
		if (renderable.bones != null) {
			boneWeightsLocations = new int[config.numBoneWeights];
		}

		// Global uniforms
		u_projTrans = register(Inputs.projTrans, Setters.projTrans);
		u_viewTrans = register(Inputs.viewTrans, Setters.viewTrans);
		u_projViewTrans = register(Inputs.projViewTrans, Setters.projViewTrans);
		u_cameraPosition = register(Inputs.cameraPosition, Setters.cameraPosition);
		u_cameraDirection = register(Inputs.cameraDirection, Setters.cameraDirection);
		u_cameraUp = register(Inputs.cameraUp, Setters.cameraUp);
		u_cameraNearFar = register(Inputs.cameraNearFar, Setters.cameraNearFar);
		u_time = register(new Uniform("u_time"));
		// Object uniforms
		u_worldTrans = register(Inputs.worldTrans, Setters.worldTrans);
		u_viewWorldTrans = register(Inputs.viewWorldTrans, Setters.viewWorldTrans);
		u_projViewWorldTrans = register(Inputs.projViewWorldTrans, Setters.projViewWorldTrans);
		u_normalMatrix = register(Inputs.normalMatrix, Setters.normalMatrix);
		u_bones = (renderable.bones != null && config.numBones > 0) ? register(Inputs.bones, new Setters.Bones(config.numBones))
			: -1;

		u_shininess = register(Inputs.shininess, Setters.shininess);
		u_opacity = register(Inputs.opacity);
		u_diffuseColor = register(Inputs.diffuseColor, Setters.diffuseColor);
		u_diffuseTexture = register(Inputs.diffuseTexture, Setters.diffuseTexture);
		u_diffuseUVTransform = register(Inputs.diffuseUVTransform, Setters.diffuseUVTransform);
		u_specularColor = register(Inputs.specularColor, Setters.specularColor);
		u_specularTexture = register(Inputs.specularTexture, Setters.specularTexture);
		u_specularUVTransform = register(Inputs.specularUVTransform, Setters.specularUVTransform);
		u_emissiveColor = register(Inputs.emissiveColor, Setters.emissiveColor);
		u_emissiveTexture = register(Inputs.emissiveTexture, Setters.emissiveTexture);
		u_emissiveUVTransform = register(Inputs.emissiveUVTransform, Setters.emissiveUVTransform);
		u_reflectionColor = register(Inputs.reflectionColor, Setters.reflectionColor);
		u_reflectionTexture = register(Inputs.reflectionTexture, Setters.reflectionTexture);
		u_reflectionUVTransform = register(Inputs.reflectionUVTransform, Setters.reflectionUVTransform);
		u_normalTexture = register(Inputs.normalTexture, Setters.normalTexture);
		u_normalUVTransform = register(Inputs.normalUVTransform, Setters.normalUVTransform);
		u_ambientTexture = register(Inputs.ambientTexture, Setters.ambientTexture);
		u_ambientUVTransform = register(Inputs.ambientUVTransform, Setters.ambientUVTransform);
		u_alphaTest = register(Inputs.alphaTest);

		u_ambientCubemap = lighting
			? register(Inputs.ambientCube, new Setters.ACubemap(config.numDirectionalLights, config.numPointLights))
			: -1;
		u_environmentCubemap = environmentCubemap ? register(Inputs.environmentCubemap, Setters.environmentCubemap) : -1;
	}

	public override void init () {
		ShaderProgram program = this.program;
		this.program = null;
		init(program, renderable);
		renderable = null;

		dirLightsLoc = loc(u_dirLights0color);
		dirLightsColorOffset = loc(u_dirLights0color) - dirLightsLoc;
		dirLightsDirectionOffset = loc(u_dirLights0direction) - dirLightsLoc;
		dirLightsSize = loc(u_dirLights1color) - dirLightsLoc;
		if (dirLightsSize < 0) dirLightsSize = 0;

		pointLightsLoc = loc(u_pointLights0color);
		pointLightsColorOffset = loc(u_pointLights0color) - pointLightsLoc;
		pointLightsPositionOffset = loc(u_pointLights0position) - pointLightsLoc;
		pointLightsIntensityOffset = has(u_pointLights0intensity) ? loc(u_pointLights0intensity) - pointLightsLoc : -1;
		pointLightsSize = loc(u_pointLights1color) - pointLightsLoc;
		if (pointLightsSize < 0) pointLightsSize = 0;

		spotLightsLoc = loc(u_spotLights0color);
		spotLightsColorOffset = loc(u_spotLights0color) - spotLightsLoc;
		spotLightsPositionOffset = loc(u_spotLights0position) - spotLightsLoc;
		spotLightsDirectionOffset = loc(u_spotLights0direction) - spotLightsLoc;
		spotLightsIntensityOffset = has(u_spotLights0intensity) ? loc(u_spotLights0intensity) - spotLightsLoc : -1;
		spotLightsCutoffAngleOffset = loc(u_spotLights0cutoffAngle) - spotLightsLoc;
		spotLightsExponentOffset = loc(u_spotLights0exponent) - spotLightsLoc;
		spotLightsSize = loc(u_spotLights1color) - spotLightsLoc;
		if (spotLightsSize < 0) spotLightsSize = 0;

		if (boneWeightsLocations != null) {
			for (int i = 0; i < boneWeightsLocations.Length; i++) {
				boneWeightsLocations[i] = program.getAttributeLocation(ShaderProgram.BONEWEIGHT_ATTRIBUTE + i);
			}
		}
	}

	private static  bool and ( long mask,  long flag) {
		return (mask & flag) == flag;
	}

	private static  bool or ( long mask,  long flag) {
		return (mask & flag) != 0;
	}

	private readonly static Attributes tmpAttributes = new Attributes();

	// TODO: Perhaps move responsibility for combining attributes to RenderableProvider?
	private static  Attributes combineAttributes ( Renderable renderable) {
		tmpAttributes.clear();
		if (renderable.environment != null) tmpAttributes.set(renderable.environment);
		if (renderable.material != null) tmpAttributes.set(renderable.material);
		return tmpAttributes;
	}

	private static  long combineAttributeMasks ( Renderable renderable) {
		long mask = 0;
		if (renderable.environment != null) mask |= renderable.environment.getMask();
		if (renderable.material != null) mask |= renderable.material.getMask();
		return mask;
	}

	public static String createPrefix ( Renderable renderable,  Config config) {
		 Attributes attributes = combineAttributes(renderable);
		String prefix = "";
		 long attributesMask = attributes.getMask();
		 long vertexMask = renderable.meshPart.mesh.getVertexAttributes().GetMask();
		if (and(vertexMask, Usage.Position)) prefix += "#define positionFlag\n";
		if (or(vertexMask, Usage.ColorUnpacked | Usage.ColorPacked)) prefix += "#define colorFlag\n";
		if (and(vertexMask, Usage.BiNormal)) prefix += "#define binormalFlag\n";
		if (and(vertexMask, Usage.Tangent)) prefix += "#define tangentFlag\n";
		if (and(vertexMask, Usage.Normal)) prefix += "#define normalFlag\n";
		if (and(vertexMask, Usage.Normal) || and(vertexMask, Usage.Tangent | Usage.BiNormal)) {
			if (renderable.environment != null) {
				prefix += "#define lightingFlag\n";
				prefix += "#define ambientCubemapFlag\n";
				prefix += "#define numDirectionalLights " + config.numDirectionalLights + "\n";
				prefix += "#define numPointLights " + config.numPointLights + "\n";
				prefix += "#define numSpotLights " + config.numSpotLights + "\n";
				if (attributes.has(ColorAttribute.Fog)) {
					prefix += "#define fogFlag\n";
				}
				if (renderable.environment.shadowMap != null) prefix += "#define shadowMapFlag\n";
				if (attributes.has(CubemapAttribute.EnvironmentMap)) prefix += "#define environmentCubemapFlag\n";
			}
		}
		 int n = renderable.meshPart.mesh.getVertexAttributes().Size();
		for (int i = 0; i < n; i++) {
			 VertexAttribute attr = renderable.meshPart.mesh.getVertexAttributes().Get(i);
			if (attr.usage == Usage.TextureCoordinates) prefix += "#define texCoord" + attr.unit + "Flag\n";
		}
		if (renderable.bones != null) {
			for (int i = 0; i < config.numBoneWeights; i++) {
				prefix += "#define boneWeight" + i + "Flag\n";
			}
		}
		if ((attributesMask & BlendingAttribute.Type) == BlendingAttribute.Type)
			prefix += "#define " + BlendingAttribute.Alias + "Flag\n";
		if ((attributesMask & TextureAttribute.Diffuse) == TextureAttribute.Diffuse) {
			prefix += "#define " + TextureAttribute.DiffuseAlias + "Flag\n";
			prefix += "#define " + TextureAttribute.DiffuseAlias + "Coord texCoord0\n"; // FIXME implement UV mapping
		}
		if ((attributesMask & TextureAttribute.Specular) == TextureAttribute.Specular) {
			prefix += "#define " + TextureAttribute.SpecularAlias + "Flag\n";
			prefix += "#define " + TextureAttribute.SpecularAlias + "Coord texCoord0\n"; // FIXME implement UV mapping
		}
		if ((attributesMask & TextureAttribute.Normal) == TextureAttribute.Normal) {
			prefix += "#define " + TextureAttribute.NormalAlias + "Flag\n";
			prefix += "#define " + TextureAttribute.NormalAlias + "Coord texCoord0\n"; // FIXME implement UV mapping
		}
		if ((attributesMask & TextureAttribute.Emissive) == TextureAttribute.Emissive) {
			prefix += "#define " + TextureAttribute.EmissiveAlias + "Flag\n";
			prefix += "#define " + TextureAttribute.EmissiveAlias + "Coord texCoord0\n"; // FIXME implement UV mapping
		}
		if ((attributesMask & TextureAttribute.Reflection) == TextureAttribute.Reflection) {
			prefix += "#define " + TextureAttribute.ReflectionAlias + "Flag\n";
			prefix += "#define " + TextureAttribute.ReflectionAlias + "Coord texCoord0\n"; // FIXME implement UV mapping
		}
		if ((attributesMask & TextureAttribute.Ambient) == TextureAttribute.Ambient) {
			prefix += "#define " + TextureAttribute.AmbientAlias + "Flag\n";
			prefix += "#define " + TextureAttribute.AmbientAlias + "Coord texCoord0\n"; // FIXME implement UV mapping
		}
		if ((attributesMask & ColorAttribute.Diffuse) == ColorAttribute.Diffuse)
			prefix += "#define " + ColorAttribute.DiffuseAlias + "Flag\n";
		if ((attributesMask & ColorAttribute.Specular) == ColorAttribute.Specular)
			prefix += "#define " + ColorAttribute.SpecularAlias + "Flag\n";
		if ((attributesMask & ColorAttribute.Emissive) == ColorAttribute.Emissive)
			prefix += "#define " + ColorAttribute.EmissiveAlias + "Flag\n";
		if ((attributesMask & ColorAttribute.Reflection) == ColorAttribute.Reflection)
			prefix += "#define " + ColorAttribute.ReflectionAlias + "Flag\n";
		if ((attributesMask & FloatAttribute.Shininess) == FloatAttribute.Shininess)
			prefix += "#define " + FloatAttribute.ShininessAlias + "Flag\n";
		if ((attributesMask & FloatAttribute.AlphaTest) == FloatAttribute.AlphaTest)
			prefix += "#define " + FloatAttribute.AlphaTestAlias + "Flag\n";
		if (renderable.bones != null && config.numBones > 0) prefix += "#define numBones " + config.numBones + "\n";
		return prefix;
	}

	public override bool canRender ( Renderable renderable) {
		if (renderable.bones != null) {
			if (renderable.bones.Length > config.numBones) return false;
			if (renderable.meshPart.mesh.getVertexAttributes().GetBoneWeights() > config.numBoneWeights) return false;
		}
		if (renderable.meshPart.mesh.getVertexAttributes().GetTextureCoordinates() != textureCoordinates) return false;
		 long renderableMask = combineAttributeMasks(renderable);
		return (attributesMask == (renderableMask | optionalAttributes))
			&& (vertexMask == renderable.meshPart.mesh.getVertexAttributes().GetMaskWithSizePacked())
			&& (renderable.environment != null) == lighting;
	}

	public override int compareTo (Shader other) {
		if (other == null) return -1;
		if (other == this) return 0;
		return 0; // FIXME compare shaders on their impact on performance
	}

	public override bool Equals (Object? obj) {
		return (obj is DefaultShader) && Equals((DefaultShader)obj);
	}

	public bool Equals (DefaultShader obj) {
		return (obj == this);
	}

	private readonly Matrix3 normalMatrix = new Matrix3();
	private float time;
	private bool lightsSet;

	public override void begin ( Camera camera,  RenderContext context) {
		base.begin(camera, context);

		foreach ( DirectionalLight dirLight in directionalLights)
			dirLight.set(0, 0, 0, 0, -1, 0);
        foreach ( PointLight pointLight in pointLights)
			pointLight.set(0, 0, 0, 0, 0, 0, 0);
        foreach ( SpotLight spotLight in spotLights)
			spotLight.set(0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0);
		lightsSet = false;

		if (has(u_time)) set(u_time, time += GDX.Graphics.GetDeltaTime());

		// set generic vertex attribute value for all bone weights in case a mesh has missing attributes.
		if (boneWeightsLocations != null) {
			foreach (int location in boneWeightsLocations) {
				if (location >= 0) {
					GDX.GL.glVertexAttrib2f(location, 0, 0);
				}
			}
		}
	}

	public override void render (Renderable renderable, Attributes combinedAttributes) {
		if (!combinedAttributes.has(BlendingAttribute.Type))
			context.setBlending(false, IGL20.GL_SRC_ALPHA, IGL20.GL_ONE_MINUS_SRC_ALPHA);
		bindMaterial(combinedAttributes);
		if (lighting) bindLights(renderable, combinedAttributes);
		base.render(renderable, combinedAttributes);
	}

	public override void end () {
		base.end();
	}

	protected void bindMaterial (Attributes attributes) {
		int cullFace = config.defaultCullFace == -1 ? defaultCullFace : config.defaultCullFace;
		int depthFunc = config.defaultDepthFunc == -1 ? defaultDepthFunc : config.defaultDepthFunc;
		float depthRangeNear = 0f;
		float depthRangeFar = 1f;
		bool depthMask = true;

		foreach ( Attribute attr in attributes) {
			 long t = attr.type;
			if (BlendingAttribute.@is(t)) {
				context.setBlending(true, ((BlendingAttribute)attr).sourceFunction, ((BlendingAttribute)attr).destFunction);
				set(u_opacity, ((BlendingAttribute)attr).opacity);
			} else if ((t & IntAttribute.CullFace) == IntAttribute.CullFace)
				cullFace = ((IntAttribute)attr).value;
			else if ((t & FloatAttribute.AlphaTest) == FloatAttribute.AlphaTest)
				set(u_alphaTest, ((FloatAttribute)attr).value);
			else if ((t & DepthTestAttribute.Type) == DepthTestAttribute.Type) {
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

	private readonly Vector3 tmpV1 = new Vector3();

	protected void bindLights ( Renderable renderable,  Attributes attributes) {
		 Environment lights = renderable.environment;
		 DirectionalLightsAttribute dla = attributes.get<DirectionalLightsAttribute>(typeof(DirectionalLightsAttribute), DirectionalLightsAttribute.Type);
		 Array<DirectionalLight> dirs = dla == null ? null : dla.lights;
		 PointLightsAttribute pla = attributes.get< PointLightsAttribute>(typeof(PointLightsAttribute), PointLightsAttribute.Type);
		 Array<PointLight> points = pla == null ? null : pla.lights;
		 SpotLightsAttribute sla = attributes.get< SpotLightsAttribute>(typeof(SpotLightsAttribute), SpotLightsAttribute.Type);
		 Array<SpotLight> spots = sla == null ? null : sla.lights;

		if (dirLightsLoc >= 0) {
			for (int i = 0; i < directionalLights.Length; i++) {
				if (dirs == null || i >= dirs.size) {
					if (lightsSet && directionalLights[i].color.R == 0f && directionalLights[i].color.G == 0f
						&& directionalLights[i].color.B == 0f) continue;
					directionalLights[i].color.Set(0, 0, 0, 1);
				} else if (lightsSet && directionalLights[i].Equals(dirs.Get(i)))
					continue;
				else
					directionalLights[i].set(dirs.Get(i));

				int idx = dirLightsLoc + i * dirLightsSize;
				program.setUniformf(idx + dirLightsColorOffset, directionalLights[i].color.R, directionalLights[i].color.G,
					directionalLights[i].color.B);
				program.setUniformf(idx + dirLightsDirectionOffset, directionalLights[i].direction.x,
					directionalLights[i].direction.y, directionalLights[i].direction.z);
				if (dirLightsSize <= 0) break;
			}
		}

		if (pointLightsLoc >= 0) {
			for (int i = 0; i < pointLights.Length; i++) {
				if (points == null || i >= points.size) {
					if (lightsSet && pointLights[i].intensity == 0f) continue;
					pointLights[i].intensity = 0f;
				} else if (lightsSet && pointLights[i].Equals(points.Get(i)))
					continue;
				else
					pointLights[i].set(points.Get(i));

				int idx = pointLightsLoc + i * pointLightsSize;
				program.setUniformf(idx + pointLightsColorOffset, pointLights[i].color.R * pointLights[i].intensity,
					pointLights[i].color.G * pointLights[i].intensity, pointLights[i].color.B * pointLights[i].intensity);
				program.setUniformf(idx + pointLightsPositionOffset, pointLights[i].position.x, pointLights[i].position.y,
					pointLights[i].position.z);
				if (pointLightsIntensityOffset >= 0) program.setUniformf(idx + pointLightsIntensityOffset, pointLights[i].intensity);
				if (pointLightsSize <= 0) break;
			}
		}

		if (spotLightsLoc >= 0) {
			for (int i = 0; i < spotLights.Length; i++) {
				if (spots == null || i >= spots.size) {
					if (lightsSet && spotLights[i].intensity == 0f) continue;
					spotLights[i].intensity = 0f;
				} else if (lightsSet && spotLights[i].Equals(spots.Get(i)))
					continue;
				else
					spotLights[i].set(spots.Get(i));

				int idx = spotLightsLoc + i * spotLightsSize;
				program.setUniformf(idx + spotLightsColorOffset, spotLights[i].color.R * spotLights[i].intensity,
					spotLights[i].color.G * spotLights[i].intensity, spotLights[i].color.B * spotLights[i].intensity);
				program.setUniformf(idx + spotLightsPositionOffset, spotLights[i].position);
				program.setUniformf(idx + spotLightsDirectionOffset, spotLights[i].direction);
				program.setUniformf(idx + spotLightsCutoffAngleOffset, spotLights[i].cutoffAngle);
				program.setUniformf(idx + spotLightsExponentOffset, spotLights[i].exponent);
				if (spotLightsIntensityOffset >= 0) program.setUniformf(idx + spotLightsIntensityOffset, spotLights[i].intensity);
				if (spotLightsSize <= 0) break;
			}
		}

		if (attributes.has(ColorAttribute.Fog)) {
			set(u_fogColor, ((ColorAttribute)attributes.get(ColorAttribute.Fog)).color);
		}

		if (lights != null && lights.shadowMap != null) {
			set(u_shadowMapProjViewTrans, lights.shadowMap.getProjViewTrans());
			set(u_shadowTexture, lights.shadowMap.getDepthMap());
			set(u_shadowPCFOffset, 1.0f / (2f * lights.shadowMap.getDepthMap().texture.getWidth()));
		}

		lightsSet = true;
	}

	public override void Dispose () {
		program.Dispose();
		base.Dispose();
	}

	public int getDefaultCullFace () {
		return config.defaultCullFace == -1 ? defaultCullFace : config.defaultCullFace;
	}

	public void setDefaultCullFace (int cullFace) {
		config.defaultCullFace = cullFace;
	}

	public int getDefaultDepthFunc () {
		return config.defaultDepthFunc == -1 ? defaultDepthFunc : config.defaultDepthFunc;
	}

	public void setDefaultDepthFunc (int depthFunc) {
		config.defaultDepthFunc = depthFunc;
	}
}
