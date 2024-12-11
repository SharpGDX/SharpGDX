using System;
using static SharpGDX.Graphics.G3D.Particles.ParticleShader;
using SharpGDX.Graphics.G3D.Particles.Renderers;
using SharpGDX.Assets;
using static SharpGDX.Graphics.G3D.Particles.ParallelArray;
using SharpGDX.Graphics.G3D.Attributess;
using SharpGDX.Graphics.G3D.Shaders;
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
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.G3D.Utils;
using static SharpGDX.Graphics.VertexAttributes;

namespace SharpGDX.Graphics.G3D.Particles.Batches;

/** This class is used to render billboard particles.
 * @author Inferno */
public class BillboardParticleBatch : BufferedParticleBatch<BillboardControllerRenderData> {
	protected static readonly Vector3 TMP_V1 = new Vector3(), TMP_V2 = new Vector3(), TMP_V3 = new Vector3(), TMP_V4 = new Vector3(),
		TMP_V5 = new Vector3(), TMP_V6 = new Vector3();
	protected static readonly Matrix3 TMP_M3 = new Matrix3();
	// Attributes
	protected static readonly int sizeAndRotationUsage = 1 << 9, directionUsage = 1 << 10;
	private static readonly VertexAttributes GPU_ATTRIBUTES = new VertexAttributes(
		new VertexAttribute(Usage.Position, 3, ShaderProgram.POSITION_ATTRIBUTE),
		new VertexAttribute(Usage.TextureCoordinates, 2, ShaderProgram.TEXCOORD_ATTRIBUTE + "0"),
		new VertexAttribute(Usage.ColorUnpacked, 4, ShaderProgram.COLOR_ATTRIBUTE),
		new VertexAttribute(sizeAndRotationUsage, 4, "a_sizeAndRotation")),
		/*
		 * GPU_EXT_ATTRIBUTES = new VertexAttributes(new VertexAttribute(Usage.Position, 3, ShaderProgram.POSITION_ATTRIBUTE), new
		 * VertexAttribute(Usage.TextureCoordinates, 2, ShaderProgram.TEXCOORD_ATTRIBUTE+"0"), new VertexAttribute(Usage.Color, 4,
		 * ShaderProgram.COLOR_ATTRIBUTE), new VertexAttribute(sizeAndRotationUsage, 4, "a_sizeAndRotation"), new
		 * VertexAttribute(directionUsage, 3, "a_direction")),
		 */
		CPU_ATTRIBUTES = new VertexAttributes(new VertexAttribute(Usage.Position, 3, ShaderProgram.POSITION_ATTRIBUTE),
			new VertexAttribute(Usage.TextureCoordinates, 2, ShaderProgram.TEXCOORD_ATTRIBUTE + "0"),
			new VertexAttribute(Usage.ColorUnpacked, 4, ShaderProgram.COLOR_ATTRIBUTE));

	// Offsets
	private static readonly int GPU_POSITION_OFFSET = (short)(GPU_ATTRIBUTES.findByUsage(Usage.Position).offset / 4),
		GPU_UV_OFFSET = (short)(GPU_ATTRIBUTES.findByUsage(Usage.TextureCoordinates).offset / 4),
		GPU_SIZE_ROTATION_OFFSET = (short)(GPU_ATTRIBUTES.findByUsage(sizeAndRotationUsage).offset / 4),
		GPU_COLOR_OFFSET = (short)(GPU_ATTRIBUTES.findByUsage(Usage.ColorUnpacked).offset / 4),
		GPU_VERTEX_SIZE = GPU_ATTRIBUTES.vertexSize / 4,

		// Ext
		/*
		 * GPU_EXT_POSITION_OFFSET = (short)(GPU_EXT_ATTRIBUTES.findByUsage(Usage.Position).offset/4), GPU_EXT_UV_OFFSET =
		 * (short)(GPU_EXT_ATTRIBUTES.findByUsage(Usage.TextureCoordinates).offset/4), GPU_EXT_SIZE_ROTATION_OFFSET =
		 * (short)(GPU_EXT_ATTRIBUTES.findByUsage(sizeAndRotationUsage).offset/4), GPU_EXT_COLOR_OFFSET =
		 * (short)(GPU_EXT_ATTRIBUTES.findByUsage(Usage.Color).offset/4), GPU_EXT_DIRECTION_OFFSET =
		 * (short)(GPU_EXT_ATTRIBUTES.findByUsage(directionUsage).offset/4), GPU_EXT_VERTEX_SIZE = GPU_EXT_ATTRIBUTES.vertexSize/4,
		 */

		// Cpu
		CPU_POSITION_OFFSET = (short)(CPU_ATTRIBUTES.findByUsage(Usage.Position).offset / 4),
		CPU_UV_OFFSET = (short)(CPU_ATTRIBUTES.findByUsage(Usage.TextureCoordinates).offset / 4),
		CPU_COLOR_OFFSET = (short)(CPU_ATTRIBUTES.findByUsage(Usage.ColorUnpacked).offset / 4),
		CPU_VERTEX_SIZE = CPU_ATTRIBUTES.vertexSize / 4;
	private readonly static int MAX_PARTICLES_PER_MESH = short.MaxValue / 4, MAX_VERTICES_PER_MESH = MAX_PARTICLES_PER_MESH * 4;

	private class RenderablePool : Pool<Renderable> {
        private readonly BillboardParticleBatch _billboardParticleBatch;

        public RenderablePool (BillboardParticleBatch billboardParticleBatch)
        {
            _billboardParticleBatch = billboardParticleBatch;
        }

		protected internal override Renderable newObject () {
			return _billboardParticleBatch.allocRenderable();
		}
	}

	public class Config {
		public Config () {
		}

		public Config (bool useGPU, AlignMode mode) {
			this.useGPU = useGPU;
			this.mode = mode;
		}

		internal bool useGPU;
		internal AlignMode mode;
	}

	private RenderablePool renderablePool;
	private Array<Renderable> renderables;
	private float[] vertices;
	private short[] indices;
	private int currentVertexSize = 0;
	private VertexAttributes currentAttributes;
	protected bool useGPU = false;
	protected AlignMode mode = AlignMode.Screen;
	protected Texture texture;
	protected BlendingAttribute blendingAttribute;
	protected DepthTestAttribute depthTestAttribute;
	Shader shader;

	/** Create a new BillboardParticleBatch
	 * @param mode
	 * @param useGPU Allow to use GPU instead of CPU
	 * @param capacity Max particle displayed
	 * @param blendingAttribute Blending attribute used by the batch
	 * @param depthTestAttribute DepthTest attribute used by the batch */
	public BillboardParticleBatch (AlignMode mode, bool useGPU, int capacity, BlendingAttribute blendingAttribute,
		DepthTestAttribute depthTestAttribute) 
    : base(typeof(BillboardControllerRenderData))
    {
		
		renderables = new Array<Renderable>();
		renderablePool = new RenderablePool(this);
		this.blendingAttribute = blendingAttribute;
		this.depthTestAttribute = depthTestAttribute;

		if (this.blendingAttribute == null)
			this.blendingAttribute = new BlendingAttribute(IGL20.GL_ONE, IGL20.GL_ONE_MINUS_SRC_ALPHA, 1f);
		if (this.depthTestAttribute == null) this.depthTestAttribute = new DepthTestAttribute(IGL20.GL_LEQUAL, false);

		allocIndices();
		initRenderData();
		ensureCapacity(capacity);
		setUseGpu(useGPU);
		setAlignMode(mode);
	}

	public BillboardParticleBatch (AlignMode mode, bool useGPU, int capacity) 
    : this(mode, useGPU, capacity, null, null)
    {
		
	}

	public BillboardParticleBatch () 
    : this(AlignMode.Screen, false, 100)
    {
		
	}

	public BillboardParticleBatch (int capacity) 
    : this(AlignMode.Screen, false, capacity)
    {
		
	}

	protected override void allocParticlesData (int capacity) {
		vertices = new float[currentVertexSize * 4 * capacity];
		allocRenderables(capacity);
	}

	protected Renderable allocRenderable () {
		Renderable renderable = new Renderable();
		renderable.meshPart.primitiveType = IGL20.GL_TRIANGLES;
		renderable.meshPart.offset = 0;
		renderable.material = new Material(this.blendingAttribute, this.depthTestAttribute,
			TextureAttribute.createDiffuse(texture));
		renderable.meshPart.mesh = new Mesh(false, MAX_VERTICES_PER_MESH, MAX_PARTICLES_PER_MESH * 6, currentAttributes);
		renderable.meshPart.mesh.setIndices(indices);
		renderable.shader = shader;
		return renderable;
	}

	private void allocIndices () {
		int indicesCount = MAX_PARTICLES_PER_MESH * 6;
		indices = new short[indicesCount];
		for (int i = 0, vertex = 0; i < indicesCount; i += 6, vertex += 4) {
			indices[i] = (short)vertex;
			indices[i + 1] = (short)(vertex + 1);
			indices[i + 2] = (short)(vertex + 2);
			indices[i + 3] = (short)(vertex + 2);
			indices[i + 4] = (short)(vertex + 3);
			indices[i + 5] = (short)vertex;
		}
	}

	private void allocRenderables (int capacity) {
		// Free old meshes
		int meshCount = MathUtils.ceil(capacity / MAX_PARTICLES_PER_MESH), free = renderablePool.getFree();
		if (free < meshCount) {
			for (int i = 0, left = meshCount - free; i < left; ++i)
				renderablePool.free(renderablePool.newObject());
		}
	}

	protected Shader getShader (Renderable renderable) {
		Shader shader = useGPU ? new ParticleShader(renderable, new ParticleShader.Config(mode)) : new DefaultShader(renderable);
		shader.init();
		return shader;
	}

	private void allocShader () {
		Renderable newRenderable = allocRenderable();
		shader = newRenderable.shader = getShader(newRenderable);
		renderablePool.free(newRenderable);
	}

	private void clearRenderablesPool () {
		renderablePool.freeAll(renderables);
		for (int i = 0, free = renderablePool.getFree(); i < free; ++i) {
			Renderable renderable = renderablePool.obtain();
			renderable.meshPart.mesh.Dispose();
		}
		renderables.clear();
	}

	/** Sets vertex attributes and size */
	public void setVertexData () {
		if (useGPU) {
			currentAttributes = GPU_ATTRIBUTES;
			currentVertexSize = GPU_VERTEX_SIZE;
			/*
			 * if(mode == AlignMode.ParticleDirection){ currentAttributes = GPU_EXT_ATTRIBUTES; currentVertexSize =
			 * GPU_EXT_VERTEX_SIZE; } else{ currentAttributes = GPU_ATTRIBUTES; currentVertexSize = GPU_VERTEX_SIZE; }
			 */
		} else {
			currentAttributes = CPU_ATTRIBUTES;
			currentVertexSize = CPU_VERTEX_SIZE;
		}
	}

	/** Allocates all the require rendering resources like Renderables,Shaders,Meshes according to the current batch
	 * configuration. */
	private void initRenderData () {
		setVertexData();
		clearRenderablesPool();
		allocShader();
		resetCapacity();
	}

	/** Sets the current align mode. It will reallocate internal data, use only when necessary. */
	public void setAlignMode (AlignMode mode) {
		if (mode != this.mode) {
			this.mode = mode;
			if (useGPU) {
				initRenderData();
				allocRenderables(bufferedParticlesCount);
			}
		}
	}

	public AlignMode getAlignMode () {
		return mode;
	}

	/** Sets the current align mode. It will reallocate internal data, use only when necessary. */
	public void setUseGpu (bool useGPU) {
		if (this.useGPU != useGPU) {
			this.useGPU = useGPU;
			initRenderData();
			allocRenderables(bufferedParticlesCount);
		}
	}

	public bool isUseGPU () {
		return useGPU;
	}

	public void setTexture (Texture texture) {
		renderablePool.freeAll(renderables);
		renderables.clear();
		for (int i = 0, free = renderablePool.getFree(); i < free; ++i) {
			Renderable renderable = renderablePool.obtain();
			TextureAttribute attribute = (TextureAttribute)renderable.material.get(TextureAttribute.Diffuse);
			attribute.textureDescription.texture = texture;
		}
		this.texture = texture;
	}

	public Texture getTexture () {
		return texture;
	}

	public BlendingAttribute getBlendingAttribute () {
		return blendingAttribute;
	}

	public override void begin () {
		base.begin();
		renderablePool.freeAll(renderables);
		renderables.clear();
	}

	// GPU
	// Required + Color + Rotation
	private static void putVertex (float[] vertices, int offset, float x, float y, float z, float u, float v, float scaleX,
		float scaleY, float cosRotation, float sinRotation, float r, float g, float b, float a) {
		// Position
		vertices[offset + GPU_POSITION_OFFSET] = x;
		vertices[offset + GPU_POSITION_OFFSET + 1] = y;
		vertices[offset + GPU_POSITION_OFFSET + 2] = z;
		// UV
		vertices[offset + GPU_UV_OFFSET] = u;
		vertices[offset + GPU_UV_OFFSET + 1] = v;
		// Scale
		vertices[offset + GPU_SIZE_ROTATION_OFFSET] = scaleX;
		vertices[offset + GPU_SIZE_ROTATION_OFFSET + 1] = scaleY;
		vertices[offset + GPU_SIZE_ROTATION_OFFSET + 2] = cosRotation;
		vertices[offset + GPU_SIZE_ROTATION_OFFSET + 3] = sinRotation;
		// Color
		vertices[offset + GPU_COLOR_OFFSET] = r;
		vertices[offset + GPU_COLOR_OFFSET + 1] = g;
		vertices[offset + GPU_COLOR_OFFSET + 2] = b;
		vertices[offset + GPU_COLOR_OFFSET + 3] = a;
	}

	/*
	 * //Required + Color + Rotation + Direction private static void putVertex( float[] vertices, int offset, float x, float y,
	 * float z, float u, float v, float scaleX, float scaleY, float cosRotation, float sinRotation, float r, float g, float b,
	 * float a, Vector3 direction) { //Position vertices[offset + GPU_EXT_POSITION_OFFSET] = x; vertices[offset +
	 * GPU_EXT_POSITION_OFFSET+1] = y; vertices[offset + GPU_EXT_POSITION_OFFSET+2] = z; //UV vertices[offset + GPU_EXT_UV_OFFSET]
	 * = u; vertices[offset + GPU_EXT_UV_OFFSET+1] = v; //Scale vertices[offset + GPU_EXT_SIZE_ROTATION_OFFSET] = scaleX;
	 * vertices[offset + GPU_EXT_SIZE_ROTATION_OFFSET+1] = scaleY; vertices[offset + GPU_EXT_SIZE_ROTATION_OFFSET+2] = cosRotation;
	 * vertices[offset + GPU_EXT_SIZE_ROTATION_OFFSET+3] = sinRotation; //Color vertices[offset + GPU_EXT_COLOR_OFFSET] = r;
	 * vertices[offset + GPU_EXT_COLOR_OFFSET+1] = g; vertices[offset + GPU_EXT_COLOR_OFFSET+2] = b; vertices[offset +
	 * GPU_EXT_COLOR_OFFSET+3] = a; //Direction vertices[offset + GPU_EXT_DIRECTION_OFFSET] = direction.x; vertices[offset +
	 * GPU_EXT_DIRECTION_OFFSET +1] = direction.y; vertices[offset + GPU_EXT_DIRECTION_OFFSET +2] = direction.z; }
	 */

	// CPU
	// Required
	private static void putVertex (float[] vertices, int offset, Vector3 p, float u, float v, float r, float g, float b, float a) {
		// Position
		vertices[offset + CPU_POSITION_OFFSET] = p.x;
		vertices[offset + CPU_POSITION_OFFSET + 1] = p.y;
		vertices[offset + CPU_POSITION_OFFSET + 2] = p.z;
		// UV
		vertices[offset + CPU_UV_OFFSET] = u;
		vertices[offset + CPU_UV_OFFSET + 1] = v;
		// Color
		vertices[offset + CPU_COLOR_OFFSET] = r;
		vertices[offset + CPU_COLOR_OFFSET + 1] = g;
		vertices[offset + CPU_COLOR_OFFSET + 2] = b;
		vertices[offset + CPU_COLOR_OFFSET + 3] = a;
	}

	private void fillVerticesGPU (int[] particlesOffset) {
		int tp = 0;
		foreach (BillboardControllerRenderData data in renderData) {
			FloatChannel scaleChannel = data.scaleChannel;
			FloatChannel regionChannel = data.regionChannel;
			FloatChannel positionChannel = data.positionChannel;
			FloatChannel colorChannel = data.colorChannel;
			FloatChannel rotationChannel = data.rotationChannel;
			for (int p = 0, c = data.controller.particles.size; p < c; ++p, ++tp) {
				int baseOffset = particlesOffset[tp] * currentVertexSize * 4;
				float scale = scaleChannel.data[p * scaleChannel.strideSize];
				int regionOffset = p * regionChannel.strideSize;
				int positionOffset = p * positionChannel.strideSize;
				int colorOffset = p * colorChannel.strideSize;
				int rotationOffset = p * rotationChannel.strideSize;
				float px = positionChannel.data[positionOffset + ParticleChannels.XOffset],
					py = positionChannel.data[positionOffset + ParticleChannels.YOffset],
					pz = positionChannel.data[positionOffset + ParticleChannels.ZOffset];
				float u = regionChannel.data[regionOffset + ParticleChannels.UOffset];
				float v = regionChannel.data[regionOffset + ParticleChannels.VOffset];
				float u2 = regionChannel.data[regionOffset + ParticleChannels.U2Offset];
				float v2 = regionChannel.data[regionOffset + ParticleChannels.V2Offset];
				float sx = regionChannel.data[regionOffset + ParticleChannels.HalfWidthOffset] * scale,
					sy = regionChannel.data[regionOffset + ParticleChannels.HalfHeightOffset] * scale;
				float r = colorChannel.data[colorOffset + ParticleChannels.RedOffset];
				float g = colorChannel.data[colorOffset + ParticleChannels.GreenOffset];
				float b = colorChannel.data[colorOffset + ParticleChannels.BlueOffset];
				float a = colorChannel.data[colorOffset + ParticleChannels.AlphaOffset];
				float cosRotation = rotationChannel.data[rotationOffset + ParticleChannels.CosineOffset];
				float sinRotation = rotationChannel.data[rotationOffset + ParticleChannels.SineOffset];

				// bottom left, bottom right, top right, top left
				putVertex(vertices, baseOffset, px, py, pz, u, v2, -sx, -sy, cosRotation, sinRotation, r, g, b, a);
				baseOffset += currentVertexSize;
				putVertex(vertices, baseOffset, px, py, pz, u2, v2, sx, -sy, cosRotation, sinRotation, r, g, b, a);
				baseOffset += currentVertexSize;
				putVertex(vertices, baseOffset, px, py, pz, u2, v, sx, sy, cosRotation, sinRotation, r, g, b, a);
				baseOffset += currentVertexSize;
				putVertex(vertices, baseOffset, px, py, pz, u, v, -sx, sy, cosRotation, sinRotation, r, g, b, a);
				baseOffset += currentVertexSize;
			}
		}
	}

	/*
	 * private void fillVerticesToParticleDirectionGPU (int[] particlesOffset) { int tp=0; for(BillboardControllerRenderData data :
	 * renderData){ FloatChannel scaleChannel = data.scaleChannel; FloatChannel regionChannel = data.regionChannel; FloatChannel
	 * positionChannel = data.positionChannel; FloatChannel colorChannel = data.colorChannel; FloatChannel rotationChannel =
	 * data.rotationChannel;
	 * 
	 * for(int p=0, c = data.controller.particles.size; p < c; ++p, ++tp){ int baseOffset =
	 * particlesOffset[tp]*currentVertexSize*4; float scale = scaleChannel.data[p* scaleChannel.strideSize]; int regionOffset =
	 * p*regionChannel.strideSize; int positionOffset = p*positionChannel.strideSize; int colorOffset = p*colorChannel.strideSize;
	 * int rotationOffset = p*rotationChannel.strideSize; int velocityOffset = p* velocityChannel.strideSize; float px =
	 * positionChannel.data[positionOffset + ParticleChannels.XOffset], py = positionChannel.data[positionOffset +
	 * ParticleChannels.YOffset], pz = positionChannel.data[positionOffset + ParticleChannels.ZOffset]; float u =
	 * regionChannel.data[regionOffset +ParticleChannels.UOffset]; float v = regionChannel.data[regionOffset
	 * +ParticleChannels.VOffset]; float u2 = regionChannel.data[regionOffset +ParticleChannels.U2Offset]; float v2 =
	 * regionChannel.data[regionOffset +ParticleChannels.V2Offset]; float sx = regionChannel.data[regionOffset
	 * +ParticleChannels.HalfWidthOffset] * scale, sy = regionChannel.data[regionOffset+ParticleChannels.HalfHeightOffset] * scale;
	 * float r = colorChannel.data[colorOffset +ParticleChannels.RedOffset]; float g = colorChannel.data[colorOffset
	 * +ParticleChannels.GreenOffset]; float b = colorChannel.data[colorOffset +ParticleChannels.BlueOffset]; float a =
	 * colorChannel.data[colorOffset +ParticleChannels.AlphaOffset]; float cosRotation = rotationChannel.data[rotationOffset
	 * +ParticleChannels.CosineOffset]; float sinRotation = rotationChannel.data[rotationOffset +ParticleChannels.SineOffset];
	 * float vx = velocityChannel.data[velocityOffset + ParticleChannels.XOffset], vy = velocityChannel.data[velocityOffset +
	 * ParticleChannels.YOffset], vz = velocityChannel.data[velocityOffset + ParticleChannels.ZOffset];
	 * 
	 * //bottom left, bottom right, top right, top left TMP_V1.set(vx, vy, vz).nor(); putVertex(vertices, baseOffset, px, py, pz,
	 * u, v2, -sx, -sy, cosRotation, sinRotation, r, g, b, a, TMP_V1); baseOffset += currentVertexSize; putVertex(vertices,
	 * baseOffset, px, py, pz, u2, v2, sx, -sy, cosRotation, sinRotation, r, g, b, a, TMP_V1); baseOffset += currentVertexSize;
	 * putVertex(vertices, baseOffset, px, py, pz, u2, v, sx, sy, cosRotation, sinRotation, r, g, b, a, TMP_V1); baseOffset +=
	 * currentVertexSize; putVertex(vertices, baseOffset, px, py, pz, u, v, -sx, sy, cosRotation, sinRotation, r, g, b, a, TMP_V1);
	 * } } }
	 * 
	 * private void fillVerticesToParticleDirectionCPU (int[] particlesOffset) { int tp=0; for(ParticleController controller :
	 * renderData){ FloatChannel scaleChannel = controller.particles.getChannel(ParticleChannels.Scale); FloatChannel regionChannel
	 * = controller.particles.getChannel(ParticleChannels.TextureRegion); FloatChannel positionChannel =
	 * controller.particles.getChannel(ParticleChannels.Position); FloatChannel colorChannel =
	 * controller.particles.getChannel(ParticleChannels.Color); FloatChannel rotationChannel =
	 * controller.particles.getChannel(ParticleChannels.Rotation2D); FloatChannel velocityChannel =
	 * controller.particles.getChannel(ParticleChannels.Accelleration);
	 * 
	 * for(int p=0, c = controller.particles.size; p < c; ++p, ++tp){ int baseOffset = particlesOffset[tp]*currentVertexSize*4;
	 * float scale = scaleChannel.data[p* scaleChannel.strideSize]; int regionOffset = p*regionChannel.strideSize; int
	 * positionOffset = p*positionChannel.strideSize; int colorOffset = p*colorChannel.strideSize; int rotationOffset =
	 * p*rotationChannel.strideSize; int velocityOffset = p* velocityChannel.strideSize; float px =
	 * positionChannel.data[positionOffset + ParticleChannels.XOffset], py = positionChannel.data[positionOffset +
	 * ParticleChannels.YOffset], pz = positionChannel.data[positionOffset + ParticleChannels.ZOffset]; float u =
	 * regionChannel.data[regionOffset +ParticleChannels.UOffset]; float v = regionChannel.data[regionOffset
	 * +ParticleChannels.VOffset]; float u2 = regionChannel.data[regionOffset +ParticleChannels.U2Offset]; float v2 =
	 * regionChannel.data[regionOffset +ParticleChannels.V2Offset]; float sx = regionChannel.data[regionOffset
	 * +ParticleChannels.HalfWidthOffset] * scale, sy = regionChannel.data[regionOffset+ParticleChannels.HalfHeightOffset] * scale;
	 * float r = colorChannel.data[colorOffset +ParticleChannels.RedOffset]; float g = colorChannel.data[colorOffset
	 * +ParticleChannels.GreenOffset]; float b = colorChannel.data[colorOffset +ParticleChannels.BlueOffset]; float a =
	 * colorChannel.data[colorOffset +ParticleChannels.AlphaOffset]; float cosRotation = rotationChannel.data[rotationOffset
	 * +ParticleChannels.CosineOffset]; float sinRotation = rotationChannel.data[rotationOffset +ParticleChannels.SineOffset];
	 * float vx = velocityChannel.data[velocityOffset + ParticleChannels.XOffset], vy = velocityChannel.data[velocityOffset +
	 * ParticleChannels.YOffset], vz = velocityChannel.data[velocityOffset + ParticleChannels.ZOffset]; Vector3 up =
	 * TMP_V1.set(vx,vy,vz).nor(), look = TMP_V3.set(camera.position).sub(px,py,pz).nor(), //normal right =
	 * TMP_V2.set(up).crs(look).nor(); //tangent look.set(right).crs(up).nor(); right.scl(sx); up.scl(sy);
	 * 
	 * if(cosRotation != 1){ TMP_M3.setToRotation(look, cosRotation, sinRotation); putVertex(vertices, baseOffset,
	 * TMP_V6.set(-TMP_V1.x-TMP_V2.x, -TMP_V1.y-TMP_V2.y, -TMP_V1.z-TMP_V2.z).mul(TMP_M3).add(px, py, pz), u, v2, r, g, b, a);
	 * baseOffset += currentVertexSize; putVertex(vertices, baseOffset,TMP_V6.set(TMP_V1.x-TMP_V2.x, TMP_V1.y-TMP_V2.y,
	 * TMP_V1.z-TMP_V2.z).mul(TMP_M3).add(px, py, pz), u2, v2, r, g, b, a); baseOffset += currentVertexSize; putVertex(vertices,
	 * baseOffset,TMP_V6.set(TMP_V1.x+TMP_V2.x, TMP_V1.y+TMP_V2.y, TMP_V1.z+TMP_V2.z).mul(TMP_M3).add(px, py, pz), u2, v, r, g, b,
	 * a); baseOffset += currentVertexSize; putVertex(vertices, baseOffset, TMP_V6.set(-TMP_V1.x+TMP_V2.x, -TMP_V1.y+TMP_V2.y,
	 * -TMP_V1.z+TMP_V2.z).mul(TMP_M3).add(px, py, pz), u, v, r, g, b, a); } else { putVertex(vertices,
	 * baseOffset,TMP_V6.set(-TMP_V1.x-TMP_V2.x+px, -TMP_V1.y-TMP_V2.y+py, -TMP_V1.z-TMP_V2.z+pz), u, v2, r, g, b, a); baseOffset
	 * += currentVertexSize; putVertex(vertices, baseOffset,TMP_V6.set(TMP_V1.x-TMP_V2.x+px, TMP_V1.y-TMP_V2.y+py,
	 * TMP_V1.z-TMP_V2.z+pz), u2, v2, r, g, b, a); baseOffset += currentVertexSize; putVertex(vertices,
	 * baseOffset,TMP_V6.set(TMP_V1.x+TMP_V2.x+px, TMP_V1.y+TMP_V2.y+py, TMP_V1.z+TMP_V2.z+pz), u2, v, r, g, b, a); baseOffset +=
	 * currentVertexSize; putVertex(vertices, baseOffset, TMP_V6.set(-TMP_V1.x+TMP_V2.x+px, -TMP_V1.y+TMP_V2.y+py,
	 * -TMP_V1.z+TMP_V2.z+pz), u, v, r, g, b, a); } } } }
	 */

	private void fillVerticesToViewPointCPU (int[] particlesOffset) {
		int tp = 0;
		foreach (BillboardControllerRenderData data in renderData) {
			FloatChannel scaleChannel = data.scaleChannel;
			FloatChannel regionChannel = data.regionChannel;
			FloatChannel positionChannel = data.positionChannel;
			FloatChannel colorChannel = data.colorChannel;
			FloatChannel rotationChannel = data.rotationChannel;

			for (int p = 0, c = data.controller.particles.size; p < c; ++p, ++tp) {
				int baseOffset = particlesOffset[tp] * currentVertexSize * 4;
				float scale = scaleChannel.data[p * scaleChannel.strideSize];
				int regionOffset = p * regionChannel.strideSize;
				int positionOffset = p * positionChannel.strideSize;
				int colorOffset = p * colorChannel.strideSize;
				int rotationOffset = p * rotationChannel.strideSize;
				float px = positionChannel.data[positionOffset + ParticleChannels.XOffset],
					py = positionChannel.data[positionOffset + ParticleChannels.YOffset],
					pz = positionChannel.data[positionOffset + ParticleChannels.ZOffset];
				float u = regionChannel.data[regionOffset + ParticleChannels.UOffset];
				float v = regionChannel.data[regionOffset + ParticleChannels.VOffset];
				float u2 = regionChannel.data[regionOffset + ParticleChannels.U2Offset];
				float v2 = regionChannel.data[regionOffset + ParticleChannels.V2Offset];
				float sx = regionChannel.data[regionOffset + ParticleChannels.HalfWidthOffset] * scale,
					sy = regionChannel.data[regionOffset + ParticleChannels.HalfHeightOffset] * scale;
				float r = colorChannel.data[colorOffset + ParticleChannels.RedOffset];
				float g = colorChannel.data[colorOffset + ParticleChannels.GreenOffset];
				float b = colorChannel.data[colorOffset + ParticleChannels.BlueOffset];
				float a = colorChannel.data[colorOffset + ParticleChannels.AlphaOffset];
				float cosRotation = rotationChannel.data[rotationOffset + ParticleChannels.CosineOffset];
				float sinRotation = rotationChannel.data[rotationOffset + ParticleChannels.SineOffset];
				Vector3 look = TMP_V3.Set(camera.position).sub(px, py, pz).nor(), // normal
					right = TMP_V1.Set(camera.up).crs(look).nor(), // tangent
					up = TMP_V2.Set(look).crs(right);
				right.scl(sx);
				up.scl(sy);

				if (cosRotation != 1) {
					TMP_M3.setToRotation(look, cosRotation, sinRotation);
					putVertex(vertices, baseOffset,
						TMP_V6.Set(-TMP_V1.x - TMP_V2.x, -TMP_V1.y - TMP_V2.y, -TMP_V1.z - TMP_V2.z).mul(TMP_M3).add(px, py, pz), u, v2,
						r, g, b, a);
					baseOffset += currentVertexSize;
					putVertex(vertices, baseOffset,
						TMP_V6.Set(TMP_V1.x - TMP_V2.x, TMP_V1.y - TMP_V2.y, TMP_V1.z - TMP_V2.z).mul(TMP_M3).add(px, py, pz), u2, v2,
						r, g, b, a);
					baseOffset += currentVertexSize;
					putVertex(vertices, baseOffset,
						TMP_V6.Set(TMP_V1.x + TMP_V2.x, TMP_V1.y + TMP_V2.y, TMP_V1.z + TMP_V2.z).mul(TMP_M3).add(px, py, pz), u2, v, r,
						g, b, a);
					baseOffset += currentVertexSize;
					putVertex(vertices, baseOffset,
						TMP_V6.Set(-TMP_V1.x + TMP_V2.x, -TMP_V1.y + TMP_V2.y, -TMP_V1.z + TMP_V2.z).mul(TMP_M3).add(px, py, pz), u, v,
						r, g, b, a);
				} else {
					putVertex(vertices, baseOffset,
						TMP_V6.Set(-TMP_V1.x - TMP_V2.x + px, -TMP_V1.y - TMP_V2.y + py, -TMP_V1.z - TMP_V2.z + pz), u, v2, r, g, b, a);
					baseOffset += currentVertexSize;
					putVertex(vertices, baseOffset,
						TMP_V6.Set(TMP_V1.x - TMP_V2.x + px, TMP_V1.y - TMP_V2.y + py, TMP_V1.z - TMP_V2.z + pz), u2, v2, r, g, b, a);
					baseOffset += currentVertexSize;
					putVertex(vertices, baseOffset,
						TMP_V6.Set(TMP_V1.x + TMP_V2.x + px, TMP_V1.y + TMP_V2.y + py, TMP_V1.z + TMP_V2.z + pz), u2, v, r, g, b, a);
					baseOffset += currentVertexSize;
					putVertex(vertices, baseOffset,
						TMP_V6.Set(-TMP_V1.x + TMP_V2.x + px, -TMP_V1.y + TMP_V2.y + py, -TMP_V1.z + TMP_V2.z + pz), u, v, r, g, b, a);
				}
			}
		}
	}

	private void fillVerticesToScreenCPU (int[] particlesOffset) {
		Vector3 look = TMP_V3.Set(camera.direction).scl(-1), // normal
			right = TMP_V4.Set(camera.up).crs(look).nor(), // tangent
			up = camera.up;

		int tp = 0;
		foreach (BillboardControllerRenderData data in renderData) {
			FloatChannel scaleChannel = data.scaleChannel;
			FloatChannel regionChannel = data.regionChannel;
			FloatChannel positionChannel = data.positionChannel;
			FloatChannel colorChannel = data.colorChannel;
			FloatChannel rotationChannel = data.rotationChannel;

			for (int p = 0, c = data.controller.particles.size; p < c; ++p, ++tp) {
				int baseOffset = particlesOffset[tp] * currentVertexSize * 4;
				float scale = scaleChannel.data[p * scaleChannel.strideSize];
				int regionOffset = p * regionChannel.strideSize;
				int positionOffset = p * positionChannel.strideSize;
				int colorOffset = p * colorChannel.strideSize;
				int rotationOffset = p * rotationChannel.strideSize;
				float px = positionChannel.data[positionOffset + ParticleChannels.XOffset],
					py = positionChannel.data[positionOffset + ParticleChannels.YOffset],
					pz = positionChannel.data[positionOffset + ParticleChannels.ZOffset];
				float u = regionChannel.data[regionOffset + ParticleChannels.UOffset];
				float v = regionChannel.data[regionOffset + ParticleChannels.VOffset];
				float u2 = regionChannel.data[regionOffset + ParticleChannels.U2Offset];
				float v2 = regionChannel.data[regionOffset + ParticleChannels.V2Offset];
				float sx = regionChannel.data[regionOffset + ParticleChannels.HalfWidthOffset] * scale,
					sy = regionChannel.data[regionOffset + ParticleChannels.HalfHeightOffset] * scale;
				float r = colorChannel.data[colorOffset + ParticleChannels.RedOffset];
				float g = colorChannel.data[colorOffset + ParticleChannels.GreenOffset];
				float b = colorChannel.data[colorOffset + ParticleChannels.BlueOffset];
				float a = colorChannel.data[colorOffset + ParticleChannels.AlphaOffset];
				float cosRotation = rotationChannel.data[rotationOffset + ParticleChannels.CosineOffset];
				float sinRotation = rotationChannel.data[rotationOffset + ParticleChannels.SineOffset];
				TMP_V1.Set(right).scl(sx);
				TMP_V2.Set(up).scl(sy);

				if (cosRotation != 1) {
					TMP_M3.setToRotation(look, cosRotation, sinRotation);
					putVertex(vertices, baseOffset,
						TMP_V6.Set(-TMP_V1.x - TMP_V2.x, -TMP_V1.y - TMP_V2.y, -TMP_V1.z - TMP_V2.z).mul(TMP_M3).add(px, py, pz), u, v2,
						r, g, b, a);
					baseOffset += currentVertexSize;
					putVertex(vertices, baseOffset,
						TMP_V6.Set(TMP_V1.x - TMP_V2.x, TMP_V1.y - TMP_V2.y, TMP_V1.z - TMP_V2.z).mul(TMP_M3).add(px, py, pz), u2, v2,
						r, g, b, a);
					baseOffset += currentVertexSize;
					putVertex(vertices, baseOffset,
						TMP_V6.Set(TMP_V1.x + TMP_V2.x, TMP_V1.y + TMP_V2.y, TMP_V1.z + TMP_V2.z).mul(TMP_M3).add(px, py, pz), u2, v, r,
						g, b, a);
					baseOffset += currentVertexSize;
					putVertex(vertices, baseOffset,
						TMP_V6.Set(-TMP_V1.x + TMP_V2.x, -TMP_V1.y + TMP_V2.y, -TMP_V1.z + TMP_V2.z).mul(TMP_M3).add(px, py, pz), u, v,
						r, g, b, a);
				} else {
					putVertex(vertices, baseOffset,
						TMP_V6.Set(-TMP_V1.x - TMP_V2.x + px, -TMP_V1.y - TMP_V2.y + py, -TMP_V1.z - TMP_V2.z + pz), u, v2, r, g, b, a);
					baseOffset += currentVertexSize;
					putVertex(vertices, baseOffset,
						TMP_V6.Set(TMP_V1.x - TMP_V2.x + px, TMP_V1.y - TMP_V2.y + py, TMP_V1.z - TMP_V2.z + pz), u2, v2, r, g, b, a);
					baseOffset += currentVertexSize;
					putVertex(vertices, baseOffset,
						TMP_V6.Set(TMP_V1.x + TMP_V2.x + px, TMP_V1.y + TMP_V2.y + py, TMP_V1.z + TMP_V2.z + pz), u2, v, r, g, b, a);
					baseOffset += currentVertexSize;
					putVertex(vertices, baseOffset,
						TMP_V6.Set(-TMP_V1.x + TMP_V2.x + px, -TMP_V1.y + TMP_V2.y + py, -TMP_V1.z + TMP_V2.z + pz), u, v, r, g, b, a);
				}
			}
		}
	}

	protected override void flush (int[] offsets) {

		// fill vertices
		if (useGPU) {
			// if(mode != AlignMode.ParticleDirection)
			fillVerticesGPU(offsets);
			// else
			// fillVerticesToParticleDirectionGPU(offsets);
		} else {
			if (mode == AlignMode.Screen)
				fillVerticesToScreenCPU(offsets);
			else if (mode == AlignMode.ViewPoint) fillVerticesToViewPointCPU(offsets);
			// else
			// fillVerticesToParticleDirectionCPU(offsets);
		}

		// send vertices to meshes
		int addedVertexCount = 0;
		int vCount = bufferedParticlesCount * 4;
		for (int v = 0; v < vCount; v += addedVertexCount) {
			addedVertexCount = Math.Min(vCount - v, MAX_VERTICES_PER_MESH);
			Renderable renderable = renderablePool.obtain();
			renderable.meshPart.size = (addedVertexCount / 4) * 6;
			renderable.meshPart.mesh.setVertices(vertices, currentVertexSize * v, currentVertexSize * addedVertexCount);
			renderable.meshPart.update();
			renderables.Add(renderable);
		}
	}

	public override void getRenderables (Array<Renderable> renderables, Pool<Renderable> pool) {
		foreach (Renderable renderable in this.renderables)
			renderables.Add(pool.obtain().set(renderable));
	}

    public override void save (AssetManager manager, ResourceData<ParticleEffect> resources) {
        ResourceData<ParticleEffect> .SaveData data = resources.createSaveData("billboardBatch");
		data.save("cfg", new Config(useGPU, mode));
		data.saveAsset<Texture>(manager.getAssetFileName(texture), typeof(Texture));
	}

    public override void load (AssetManager manager, ResourceData<ParticleEffect> resources) {
        ResourceData<ParticleEffect> .SaveData data = resources.getSaveData("billboardBatch");
		if (data != null) {
			setTexture((Texture)manager.get<Texture>(data.loadAsset()));
			Config cfg = (Config)data.load<Config>("cfg");
			setUseGpu(cfg.useGPU);
			setAlignMode(cfg.mode);
		}
	}
}
