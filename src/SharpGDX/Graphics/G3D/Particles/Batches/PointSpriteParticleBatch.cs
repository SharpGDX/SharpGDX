using System;
using static SharpGDX.Graphics.G3D.Particles.ParticleShader;
using SharpGDX.Graphics.G3D.Particles.Renderers;
using SharpGDX.Graphics.G3D.Attributess;
using SharpGDX.Assets;
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


namespace SharpGDX.Graphics.G3D.Particles.Batches;

/** This class is used to draw particles as point sprites.
 * @author Inferno */
public class PointSpriteParticleBatch : BufferedParticleBatch<PointSpriteControllerRenderData> {
	private static bool pointSpritesEnabled = false;
	protected static readonly Vector3 TMP_V1 = new Vector3();
	protected static readonly int sizeAndRotationUsage = 1 << 9;
	protected static readonly VertexAttributes CPU_ATTRIBUTES = new VertexAttributes(
		new VertexAttribute(VertexAttributes.Usage.Position, 3, ShaderProgram.POSITION_ATTRIBUTE),
		new VertexAttribute(VertexAttributes.Usage.ColorUnpacked, 4, ShaderProgram.COLOR_ATTRIBUTE),
		new VertexAttribute(VertexAttributes.Usage.TextureCoordinates, 4, "a_region"),
		new VertexAttribute(sizeAndRotationUsage, 3, "a_sizeAndRotation"));
	protected static readonly int CPU_VERTEX_SIZE = (short)(CPU_ATTRIBUTES.vertexSize / 4),
		CPU_POSITION_OFFSET = (short)(CPU_ATTRIBUTES.findByUsage(VertexAttributes.Usage.Position).offset / 4),
		CPU_COLOR_OFFSET = (short)(CPU_ATTRIBUTES.findByUsage(VertexAttributes.Usage.ColorUnpacked).offset / 4),
		CPU_REGION_OFFSET = (short)(CPU_ATTRIBUTES.findByUsage(VertexAttributes.Usage.TextureCoordinates).offset / 4),
		CPU_SIZE_AND_ROTATION_OFFSET = (short)(CPU_ATTRIBUTES.findByUsage(sizeAndRotationUsage).offset / 4);

	private static void enablePointSprites () {
		GDX.GL.glEnable(IGL20.GL_VERTEX_PROGRAM_POINT_SIZE);
		if (GDX.App.GetType() == IApplication.ApplicationType.Desktop) {
			GDX.GL.glEnable(0x8861); // GL_POINT_OES
		}
		pointSpritesEnabled = true;
	}

	private float[] vertices;
	Renderable renderable;
	protected BlendingAttribute blendingAttribute;
	protected DepthTestAttribute depthTestAttribute;

	public PointSpriteParticleBatch () 
    : this(1000)
    {
		
	}

	public PointSpriteParticleBatch (int capacity) 
    : this(capacity, new ParticleShader.Config(ParticleType.Point))
    {
		
	}

	public PointSpriteParticleBatch (int capacity, ParticleShader.Config shaderConfig) 
    : this(capacity, shaderConfig, null, null)
    {
		
	}

	public PointSpriteParticleBatch (int capacity, ParticleShader.Config shaderConfig, BlendingAttribute blendingAttribute,
		DepthTestAttribute depthTestAttribute) 
    : base(typeof(PointSpriteControllerRenderData))
    {
		

		if (!pointSpritesEnabled) enablePointSprites();

		this.blendingAttribute = blendingAttribute;
		this.depthTestAttribute = depthTestAttribute;

		if (this.blendingAttribute == null)
			this.blendingAttribute = new BlendingAttribute(IGL20.GL_ONE, IGL20.GL_ONE_MINUS_SRC_ALPHA, 1f);
		if (this.depthTestAttribute == null) this.depthTestAttribute = new DepthTestAttribute(IGL20.GL_LEQUAL, false);

		allocRenderable();
		ensureCapacity(capacity);
		renderable.shader = new ParticleShader(renderable, shaderConfig);
		renderable.shader.init();
	}

	protected override void allocParticlesData (int capacity) {
		vertices = new float[capacity * CPU_VERTEX_SIZE];
		if (renderable.meshPart.mesh != null) renderable.meshPart.mesh.Dispose();
		renderable.meshPart.mesh = new Mesh(false, capacity, 0, CPU_ATTRIBUTES);
	}

	protected void allocRenderable () {
		renderable = new Renderable();
		renderable.meshPart.primitiveType = IGL20.GL_POINTS;
		renderable.meshPart.offset = 0;
		renderable.material = new Material(blendingAttribute, depthTestAttribute, TextureAttribute.createDiffuse((Texture)null));
	}

	public void setTexture (Texture texture) {
		TextureAttribute attribute = (TextureAttribute)renderable.material.get(TextureAttribute.Diffuse);
		attribute.textureDescription.texture = texture;
	}

	public Texture getTexture () {
		TextureAttribute attribute = (TextureAttribute)renderable.material.get(TextureAttribute.Diffuse);
        // TODO: this cast is kind of crap, but /shrug. -RP
        return (Texture)attribute.textureDescription.texture;
	}

	public BlendingAttribute getBlendingAttribute () {
		return blendingAttribute;
	}

	protected override void flush (int[] offsets) {
		int tp = 0;
		foreach (PointSpriteControllerRenderData data in renderData) {
            ParallelArray.FloatChannel scaleChannel = data.scaleChannel;
			ParallelArray.FloatChannel regionChannel = data.regionChannel;
            ParallelArray.FloatChannel positionChannel = data.positionChannel;
            ParallelArray.FloatChannel colorChannel = data.colorChannel;
            ParallelArray.FloatChannel rotationChannel = data.rotationChannel;

			for (int p = 0; p < data.controller.particles.size; ++p, ++tp) {
				int offset = offsets[tp] * CPU_VERTEX_SIZE;
				int regionOffset = p * regionChannel.strideSize;
				int positionOffset = p * positionChannel.strideSize;
				int colorOffset = p * colorChannel.strideSize;
				int rotationOffset = p * rotationChannel.strideSize;

				vertices[offset + CPU_POSITION_OFFSET] = positionChannel.data[positionOffset + ParticleChannels.XOffset];
				vertices[offset + CPU_POSITION_OFFSET + 1] = positionChannel.data[positionOffset + ParticleChannels.YOffset];
				vertices[offset + CPU_POSITION_OFFSET + 2] = positionChannel.data[positionOffset + ParticleChannels.ZOffset];
				vertices[offset + CPU_COLOR_OFFSET] = colorChannel.data[colorOffset + ParticleChannels.RedOffset];
				vertices[offset + CPU_COLOR_OFFSET + 1] = colorChannel.data[colorOffset + ParticleChannels.GreenOffset];
				vertices[offset + CPU_COLOR_OFFSET + 2] = colorChannel.data[colorOffset + ParticleChannels.BlueOffset];
				vertices[offset + CPU_COLOR_OFFSET + 3] = colorChannel.data[colorOffset + ParticleChannels.AlphaOffset];
				vertices[offset + CPU_SIZE_AND_ROTATION_OFFSET] = scaleChannel.data[p * scaleChannel.strideSize];
				vertices[offset + CPU_SIZE_AND_ROTATION_OFFSET + 1] = rotationChannel.data[rotationOffset
					+ ParticleChannels.CosineOffset];
				vertices[offset + CPU_SIZE_AND_ROTATION_OFFSET + 2] = rotationChannel.data[rotationOffset
					+ ParticleChannels.SineOffset];
				vertices[offset + CPU_REGION_OFFSET] = regionChannel.data[regionOffset + ParticleChannels.UOffset];
				vertices[offset + CPU_REGION_OFFSET + 1] = regionChannel.data[regionOffset + ParticleChannels.VOffset];
				vertices[offset + CPU_REGION_OFFSET + 2] = regionChannel.data[regionOffset + ParticleChannels.U2Offset];
				vertices[offset + CPU_REGION_OFFSET + 3] = regionChannel.data[regionOffset + ParticleChannels.V2Offset];
			}
		}

		renderable.meshPart.size = bufferedParticlesCount;
		renderable.meshPart.mesh.setVertices(vertices, 0, bufferedParticlesCount * CPU_VERTEX_SIZE);
		renderable.meshPart.update();
	}

    public override void getRenderables (Array<Renderable> renderables, Pool<Renderable> pool) {
		if (bufferedParticlesCount > 0) renderables.Add(pool.obtain().set(renderable));
	}

    public override void save (AssetManager manager, ResourceData<ParticleEffect> resources) {
		ResourceData<ParticleEffect>.SaveData data = resources.createSaveData("pointSpriteBatch");
		data.saveAsset<Texture>(manager.getAssetFileName(getTexture()), typeof(Texture));
	}

    public override void load (AssetManager manager, ResourceData<ParticleEffect> resources) {
		ResourceData<ParticleEffect> .SaveData data = resources.getSaveData("pointSpriteBatch");
		if (data != null) setTexture((Texture)manager.get<Texture>(data.loadAsset()));
	}
}
