using SharpGDX.Utils;
using static SharpGDX.Graphics.G2D.Sprite;
using static SharpGDX.Graphics.Mesh;
using static SharpGDX.Graphics.VertexAttributes;
using SharpGDX.Shims;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Utils.Reflect;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics.G2D;

/** A PolygonSpriteBatch is used to draw 2D polygons that reference a texture (region). The class will batch the drawing commands
 * and optimize them for processing by the GPU.
 * <p>
 * To draw something with a PolygonSpriteBatch one has to first call the {@link PolygonSpriteBatch#begin()} method which will
 * setup appropriate render states. When you are done with drawing you have to call {@link PolygonSpriteBatch#end()} which will
 * actually draw the things you specified.
 * <p>
 * All drawing commands of the PolygonSpriteBatch operate in screen coordinates. The screen coordinate system has an x-axis
 * pointing to the right, an y-axis pointing upwards and the origin is in the lower left corner of the screen. You can also
 * provide your own transformation and projection matrices if you so wish.
 * <p>
 * A PolygonSpriteBatch is managed. In case the OpenGL context is lost all OpenGL resources a PolygonSpriteBatch uses internally
 * get invalidated. A context is lost when a user switches to another application or receives an incoming call on Android. A
 * SpritPolygonSpriteBatcheBatch will be automatically reloaded after the OpenGL context is restored.
 * <p>
 * A PolygonSpriteBatch is a pretty heavy object so you should only ever have one in your program.
 * <p>
 * A PolygonSpriteBatch works with OpenGL ES 1.x and 2.0. In the case of a 2.0 context it will use its own custom shader to draw
 * all provided sprites. You can set your own custom shader via {@link #setShader(ShaderProgram)}.
 * <p>
 * A PolygonSpriteBatch has to be disposed if it is no longer used.
 * @author mzechner
 * @author Stefan Bachmann
 * @author Nathan Sweet */
public class PolygonSpriteBatch : IPolygonBatch {
	private Mesh mesh;

	private readonly float[] vertices;
	private readonly short[] triangles;
	private int vertexIndex, triangleIndex;
	private Texture lastTexture;
	private float invTexWidth = 0, invTexHeight = 0;
	private bool drawing;

	private readonly Matrix4 transformMatrix = new Matrix4();
	private readonly Matrix4 projectionMatrix = new Matrix4();
	private readonly Matrix4 combinedMatrix = new Matrix4();

	private bool blendingDisabled;
	private int blendSrcFunc = GL20.GL_SRC_ALPHA;
	private int blendDstFunc = GL20.GL_ONE_MINUS_SRC_ALPHA;
	private int blendSrcFuncAlpha = GL20.GL_SRC_ALPHA;
	private int blendDstFuncAlpha = GL20.GL_ONE_MINUS_SRC_ALPHA;

	private readonly ShaderProgram shader;
	private ShaderProgram customShader;
	private bool ownsShader;

	private readonly Color color = new Color(1, 1, 1, 1);
	float colorPacked = Color.WHITE_FLOAT_BITS;

	/** Number of render calls since the last {@link #begin()}. **/
	public int renderCalls = 0;

	/** Number of rendering calls, ever. Will not be reset unless set manually. **/
	public int totalRenderCalls = 0;

	/** The maximum number of triangles rendered in one batch so far. **/
	public int maxTrianglesInBatch = 0;

	/** Constructs a PolygonSpriteBatch with the default shader, 2000 vertices, and 4000 triangles.
	 * @see #PolygonSpriteBatch(int, int, ShaderProgram) */
	public PolygonSpriteBatch () 
	: this(2000, null)
	{
		
	}

	/** Constructs a PolygonSpriteBatch with the default shader, size vertices, and size * 2 triangles.
	 * @param size The max number of vertices and number of triangles in a single batch. Max of 32767.
	 * @see #PolygonSpriteBatch(int, int, ShaderProgram) */
	public PolygonSpriteBatch (int size) 
	: this(size, size * 2, null)
	{
		
	}

	/** Constructs a PolygonSpriteBatch with the specified shader, size vertices and size * 2 triangles.
	 * @param size The max number of vertices and number of triangles in a single batch. Max of 32767.
	 * @see #PolygonSpriteBatch(int, int, ShaderProgram) */
	public PolygonSpriteBatch (int size, ShaderProgram defaultShader) 
	: this(size, size * 2, defaultShader)
	{
		
	}

	/** Constructs a new PolygonSpriteBatch. Sets the projection matrix to an orthographic projection with y-axis point upwards,
	 * x-axis point to the right and the origin being in the bottom left corner of the screen. The projection will be pixel perfect
	 * with respect to the current screen resolution.
	 * <p>
	 * The defaultShader specifies the shader to use. Note that the names for uniforms for this default shader are different than
	 * the ones expect for shaders set with {@link #setShader(ShaderProgram)}. See {@link SpriteBatch#createDefaultShader()}.
	 * @param maxVertices The max number of vertices in a single batch. Max of 32767.
	 * @param maxTriangles The max number of triangles in a single batch.
	 * @param defaultShader The default shader to use. This is not owned by the PolygonSpriteBatch and must be disposed separately.
	 *           May be null to use the default shader. */
	public PolygonSpriteBatch (int maxVertices, int maxTriangles, ShaderProgram defaultShader) {
		// 32767 is max vertex index.
		if (maxVertices > 32767)
			throw new IllegalArgumentException("Can't have more than 32767 vertices per batch: " + maxVertices);

		Mesh.VertexDataType vertexDataType = Mesh.VertexDataType.VertexArray;
		if (Gdx.gl30 != null) {
			vertexDataType = VertexDataType.VertexBufferObjectWithVAO;
		}
		mesh = new Mesh(vertexDataType, false, maxVertices, maxTriangles * 3,
			new []{
			new VertexAttribute(Usage.Position, 2, ShaderProgram.POSITION_ATTRIBUTE),
			new VertexAttribute(Usage.ColorPacked, 4, ShaderProgram.COLOR_ATTRIBUTE),
			new VertexAttribute(Usage.TextureCoordinates, 2, ShaderProgram.TEXCOORD_ATTRIBUTE + "0")
			});

		vertices = new float[maxVertices * VERTEX_SIZE];
		triangles = new short[maxTriangles * 3];

		if (defaultShader == null) {
			shader = SpriteBatch.createDefaultShader();
			ownsShader = true;
		} else
			shader = defaultShader;

		projectionMatrix.setToOrtho2D(0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
	}

	public void begin () {
		if (drawing) throw new IllegalStateException("PolygonSpriteBatch.end must be called before begin.");
		renderCalls = 0;

		Gdx.gl.glDepthMask(false);
		if (customShader != null)
			customShader.bind();
		else
			shader.bind();
		setupMatrices();

		drawing = true;
	}

	public void end () {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before end.");
		if (vertexIndex > 0) flush();
		lastTexture = null;
		drawing = false;

		GL20 gl = Gdx.gl;
		gl.glDepthMask(true);
		if (isBlendingEnabled()) gl.glDisable(GL20.GL_BLEND);
	}

	public void setColor (Color tint) {
		color.set(tint);
		colorPacked = tint.toFloatBits();
	}

	public void setColor (float r, float g, float b, float a) {
		color.set(r, g, b, a);
		colorPacked = color.toFloatBits();
	}

	public void setPackedColor (float packedColor) {
		Color.abgr8888ToColor(color, packedColor);
		colorPacked = packedColor;
	}

	public Color getColor () {
		return color;
	}

	public float getPackedColor () {
		return colorPacked;
	}

	public void draw (PolygonRegion region, float x, float y) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 short[] regionTriangles = region.triangles;
		 int regionTrianglesLength = regionTriangles.Length;
		 float[] regionVertices = region.vertices;
		 int regionVerticesLength = regionVertices.Length;

		 Texture texture = region.region.texture;
		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + regionTrianglesLength > triangles.Length
			|| this.vertexIndex + regionVerticesLength * VERTEX_SIZE / 2 > this.vertices.Length) flush();

		int triangleIndex = this.triangleIndex;
		int vertexIndex = this.vertexIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;

		for (int i = 0; i < regionTrianglesLength; i++)
			triangles[triangleIndex++] = (short)(regionTriangles[i] + startVertex);
		this.triangleIndex = triangleIndex;

		 float[] vertices = this.vertices;
		 float color = this.colorPacked;
		 float[] textureCoords = region.textureCoords;

		for (int i = 0; i < regionVerticesLength; i += 2) {
			vertices[vertexIndex++] = regionVertices[i] + x;
			vertices[vertexIndex++] = regionVertices[i + 1] + y;
			vertices[vertexIndex++] = color;
			vertices[vertexIndex++] = textureCoords[i];
			vertices[vertexIndex++] = textureCoords[i + 1];
		}
		this.vertexIndex = vertexIndex;
	}

	public void draw (PolygonRegion region, float x, float y, float width, float height) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 short[] regionTriangles = region.triangles;
		 int regionTrianglesLength = regionTriangles.Length;
		 float[] regionVertices = region.vertices;
		 int regionVerticesLength = regionVertices.Length;
		 TextureRegion textureRegion = region.region;

		 Texture texture = textureRegion.texture;
		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + regionTrianglesLength > triangles.Length
		         || this.vertexIndex + regionVerticesLength * VERTEX_SIZE / 2 > this.vertices.Length) flush();

		int triangleIndex = this.triangleIndex;
		int vertexIndex = this.vertexIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;

		for (int i = 0, n = regionTriangles.Length; i < n; i++)
			triangles[triangleIndex++] = (short)(regionTriangles[i] + startVertex);
		this.triangleIndex = triangleIndex;

		 float[] vertices = this.vertices;
		 float color = this.colorPacked;
		 float[] textureCoords = region.textureCoords;
		 float sX = width / textureRegion.regionWidth;
		 float sY = height / textureRegion.regionHeight;

		for (int i = 0; i < regionVerticesLength; i += 2) {
			vertices[vertexIndex++] = regionVertices[i] * sX + x;
			vertices[vertexIndex++] = regionVertices[i + 1] * sY + y;
			vertices[vertexIndex++] = color;
			vertices[vertexIndex++] = textureCoords[i];
			vertices[vertexIndex++] = textureCoords[i + 1];
		}
		this.vertexIndex = vertexIndex;
	}

	public void draw (PolygonRegion region, float x, float y, float originX, float originY, float width, float height,
		float scaleX, float scaleY, float rotation) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 short[] regionTriangles = region.triangles;
		 int regionTrianglesLength = regionTriangles.Length;
		 float[] regionVertices = region.vertices;
		 int regionVerticesLength = regionVertices.Length;
		 TextureRegion textureRegion = region.region;

		Texture texture = textureRegion.texture;
		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + regionTrianglesLength > triangles.Length
		         || this.vertexIndex + regionVerticesLength * VERTEX_SIZE / 2 > this.vertices.Length) flush();

		int triangleIndex = this.triangleIndex;
		int vertexIndex = this.vertexIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;

		for (int i = 0; i < regionTrianglesLength; i++)
			triangles[triangleIndex++] = (short)(regionTriangles[i] + startVertex);
		this.triangleIndex = triangleIndex;

		 float[] vertices = this.vertices;
		 float color = this.colorPacked;
		 float[] textureCoords = region.textureCoords;

		 float worldOriginX = x + originX;
		 float worldOriginY = y + originY;
		 float sX = width / textureRegion.regionWidth;
		 float sY = height / textureRegion.regionHeight;
		 float cos = MathUtils.cosDeg(rotation);
		 float sin = MathUtils.sinDeg(rotation);

		float fx, fy;
		for (int i = 0; i < regionVerticesLength; i += 2) {
			fx = (regionVertices[i] * sX - originX) * scaleX;
			fy = (regionVertices[i + 1] * sY - originY) * scaleY;
			vertices[vertexIndex++] = cos * fx - sin * fy + worldOriginX;
			vertices[vertexIndex++] = sin * fx + cos * fy + worldOriginY;
			vertices[vertexIndex++] = color;
			vertices[vertexIndex++] = textureCoords[i];
			vertices[vertexIndex++] = textureCoords[i + 1];
		}
		this.vertexIndex = vertexIndex;
	}

	public void draw (Texture texture, float[] polygonVertices, int verticesOffset, int verticesCount, short[] polygonTriangles,
		int trianglesOffset, int trianglesCount) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 float[] vertices = this.vertices;

		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + trianglesCount > triangles.Length || this.vertexIndex + verticesCount > vertices.Length) //
			flush();

		int triangleIndex = this.triangleIndex;
		 int vertexIndex = this.vertexIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;

		for (int i = trianglesOffset, n = i + trianglesCount; i < n; i++)
			triangles[triangleIndex++] = (short)(polygonTriangles[i] + startVertex);
		this.triangleIndex = triangleIndex;

		Array.Copy(polygonVertices, verticesOffset, vertices, vertexIndex, verticesCount);
		this.vertexIndex += verticesCount;
	}

	public void draw (Texture texture, float x, float y, float originX, float originY, float width, float height, float scaleX,
		float scaleY, float rotation, int srcX, int srcY, int srcWidth, int srcHeight, bool flipX, bool flipY) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 float[] vertices = this.vertices;

		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + 6 > triangles.Length || vertexIndex + SPRITE_SIZE > vertices.Length) //
			flush();

		int triangleIndex = this.triangleIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;
		triangles[triangleIndex++] = (short)startVertex;
		triangles[triangleIndex++] = (short)(startVertex + 1);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 3);
		triangles[triangleIndex++] = (short)startVertex;
		this.triangleIndex = triangleIndex;

		// bottom left and top right corner points relative to origin
		 float worldOriginX = x + originX;
		 float worldOriginY = y + originY;
		float fx = -originX;
		float fy = -originY;
		float fx2 = width - originX;
		float fy2 = height - originY;

		// scale
		if (scaleX != 1 || scaleY != 1) {
			fx *= scaleX;
			fy *= scaleY;
			fx2 *= scaleX;
			fy2 *= scaleY;
		}

		// construct corner points, start from top left and go counter clockwise
		 float p1x = fx;
		 float p1y = fy;
		 float p2x = fx;
		 float p2y = fy2;
		 float p3x = fx2;
		 float p3y = fy2;
		 float p4x = fx2;
		 float p4y = fy;

		float x1;
		float y1;
		float x2;
		float y2;
		float x3;
		float y3;
		float x4;
		float y4;

		// rotate
		if (rotation != 0) {
			 float cos = MathUtils.cosDeg(rotation);
			 float sin = MathUtils.sinDeg(rotation);

			x1 = cos * p1x - sin * p1y;
			y1 = sin * p1x + cos * p1y;

			x2 = cos * p2x - sin * p2y;
			y2 = sin * p2x + cos * p2y;

			x3 = cos * p3x - sin * p3y;
			y3 = sin * p3x + cos * p3y;

			x4 = x1 + (x3 - x2);
			y4 = y3 - (y2 - y1);
		} else {
			x1 = p1x;
			y1 = p1y;

			x2 = p2x;
			y2 = p2y;

			x3 = p3x;
			y3 = p3y;

			x4 = p4x;
			y4 = p4y;
		}

		x1 += worldOriginX;
		y1 += worldOriginY;
		x2 += worldOriginX;
		y2 += worldOriginY;
		x3 += worldOriginX;
		y3 += worldOriginY;
		x4 += worldOriginX;
		y4 += worldOriginY;

		float u = srcX * invTexWidth;
		float v = (srcY + srcHeight) * invTexHeight;
		float u2 = (srcX + srcWidth) * invTexWidth;
		float v2 = srcY * invTexHeight;

		if (flipX) {
			float tmp = u;
			u = u2;
			u2 = tmp;
		}

		if (flipY) {
			float tmp = v;
			v = v2;
			v2 = tmp;
		}

		float color = this.colorPacked;
		int idx = this.vertexIndex;
		vertices[idx++] = x1;
		vertices[idx++] = y1;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v;

		vertices[idx++] = x2;
		vertices[idx++] = y2;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v2;

		vertices[idx++] = x3;
		vertices[idx++] = y3;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v2;

		vertices[idx++] = x4;
		vertices[idx++] = y4;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v;
		this.vertexIndex = idx;
	}

	public void draw (Texture texture, float x, float y, float width, float height, int srcX, int srcY, int srcWidth,
		int srcHeight, bool flipX, bool flipY) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 float[] vertices = this.vertices;

		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + 6 > triangles.Length || vertexIndex + SPRITE_SIZE > vertices.Length) //
			flush();

		int triangleIndex = this.triangleIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;
		triangles[triangleIndex++] = (short)startVertex;
		triangles[triangleIndex++] = (short)(startVertex + 1);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 3);
		triangles[triangleIndex++] = (short)startVertex;
		this.triangleIndex = triangleIndex;

		float u = srcX * invTexWidth;
		float v = (srcY + srcHeight) * invTexHeight;
		float u2 = (srcX + srcWidth) * invTexWidth;
		float v2 = srcY * invTexHeight;
		 float fx2 = x + width;
		 float fy2 = y + height;

		if (flipX) {
			float tmp = u;
			u = u2;
			u2 = tmp;
		}

		if (flipY) {
			float tmp = v;
			v = v2;
			v2 = tmp;
		}

		float color = this.colorPacked;
		int idx = this.vertexIndex;
		vertices[idx++] = x;
		vertices[idx++] = y;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v;

		vertices[idx++] = x;
		vertices[idx++] = fy2;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v2;

		vertices[idx++] = fx2;
		vertices[idx++] = fy2;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v2;

		vertices[idx++] = fx2;
		vertices[idx++] = y;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v;
		this.vertexIndex = idx;
	}

	public void draw (Texture texture, float x, float y, int srcX, int srcY, int srcWidth, int srcHeight) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 float[] vertices = this.vertices;

		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + 6 > triangles.Length || vertexIndex + SPRITE_SIZE > vertices.Length) //
			flush();

		int triangleIndex = this.triangleIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;
		triangles[triangleIndex++] = (short)startVertex;
		triangles[triangleIndex++] = (short)(startVertex + 1);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 3);
		triangles[triangleIndex++] = (short)startVertex;
		this.triangleIndex = triangleIndex;

		 float u = srcX * invTexWidth;
		 float v = (srcY + srcHeight) * invTexHeight;
		 float u2 = (srcX + srcWidth) * invTexWidth;
		 float v2 = srcY * invTexHeight;
		 float fx2 = x + srcWidth;
		 float fy2 = y + srcHeight;

		float color = this.colorPacked;
		int idx = this.vertexIndex;
		vertices[idx++] = x;
		vertices[idx++] = y;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v;

		vertices[idx++] = x;
		vertices[idx++] = fy2;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v2;

		vertices[idx++] = fx2;
		vertices[idx++] = fy2;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v2;

		vertices[idx++] = fx2;
		vertices[idx++] = y;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v;
		this.vertexIndex = idx;
	}

	public void draw (Texture texture, float x, float y, float width, float height, float u, float v, float u2, float v2) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 float[] vertices = this.vertices;

		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + 6 > triangles.Length || vertexIndex + SPRITE_SIZE > vertices.Length) //
			flush();

		int triangleIndex = this.triangleIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;
		triangles[triangleIndex++] = (short)startVertex;
		triangles[triangleIndex++] = (short)(startVertex + 1);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 3);
		triangles[triangleIndex++] = (short)startVertex;
		this.triangleIndex = triangleIndex;

		 float fx2 = x + width;
		 float fy2 = y + height;

		float color = this.colorPacked;
		int idx = this.vertexIndex;
		vertices[idx++] = x;
		vertices[idx++] = y;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v;

		vertices[idx++] = x;
		vertices[idx++] = fy2;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v2;

		vertices[idx++] = fx2;
		vertices[idx++] = fy2;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v2;

		vertices[idx++] = fx2;
		vertices[idx++] = y;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v;
		this.vertexIndex = idx;
	}

	public void draw (Texture texture, float x, float y) {
		draw(texture, x, y, texture.getWidth(), texture.getHeight());
	}

	public void draw (Texture texture, float x, float y, float width, float height) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 float[] vertices = this.vertices;

		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + 6 > triangles.Length || vertexIndex + SPRITE_SIZE > vertices.Length) //
			flush();

		int triangleIndex = this.triangleIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;
		triangles[triangleIndex++] = (short)startVertex;
		triangles[triangleIndex++] = (short)(startVertex + 1);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 3);
		triangles[triangleIndex++] = (short)startVertex;
		this.triangleIndex = triangleIndex;

		 float fx2 = x + width;
		 float fy2 = y + height;
		 float u = 0;
		 float v = 1;
		 float u2 = 1;
		 float v2 = 0;

		float color = this.colorPacked;
		int idx = this.vertexIndex;
		vertices[idx++] = x;
		vertices[idx++] = y;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v;

		vertices[idx++] = x;
		vertices[idx++] = fy2;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v2;

		vertices[idx++] = fx2;
		vertices[idx++] = fy2;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v2;

		vertices[idx++] = fx2;
		vertices[idx++] = y;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v;
		this.vertexIndex = idx;
	}

	public void draw (Texture texture, float[] spriteVertices, int offset, int count) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 float[] vertices = this.vertices;

		int triangleCount = count / SPRITE_SIZE * 6;
		int batch;
		if (texture != lastTexture) {
			switchTexture(texture);
			batch = Math.Min(Math.Min(count, vertices.Length - (vertices.Length % SPRITE_SIZE)), triangles.Length / 6 * SPRITE_SIZE);
			triangleCount = batch / SPRITE_SIZE * 6;
		} else if (this.triangleIndex + triangleCount > triangles.Length || this.vertexIndex + count > vertices.Length) {
			flush();
			batch = Math.Min(Math.Min(count, vertices.Length - (vertices.Length % SPRITE_SIZE)), triangles.Length / 6 * SPRITE_SIZE);
			triangleCount = batch / SPRITE_SIZE * 6;
		} else
			batch = count;

		int vertexIndex = this.vertexIndex;
		short vertex = (short)(vertexIndex / VERTEX_SIZE);
		int triangleIndex = this.triangleIndex;
		for (int n = triangleIndex + triangleCount; triangleIndex < n; triangleIndex += 6, vertex += 4) {
			triangles[triangleIndex] = vertex;
			triangles[triangleIndex + 1] = (short)(vertex + 1);
			triangles[triangleIndex + 2] = (short)(vertex + 2);
			triangles[triangleIndex + 3] = (short)(vertex + 2);
			triangles[triangleIndex + 4] = (short)(vertex + 3);
			triangles[triangleIndex + 5] = vertex;
		}

		while (true) {
			Array.Copy(spriteVertices, offset, vertices, vertexIndex, batch);
			this.vertexIndex = vertexIndex + batch;
			this.triangleIndex = triangleIndex;
			count -= batch;
			if (count == 0) break;
			offset += batch;
			flush();
			vertexIndex = 0;
			if (batch > count) {
				batch = Math.Min(count, triangles.Length / 6 * SPRITE_SIZE);
				triangleIndex = batch / SPRITE_SIZE * 6;
			}
		}
	}

	public void draw (TextureRegion region, float x, float y) {
		draw(region, x, y, region.getRegionWidth(), region.getRegionHeight());
	}

	public void draw (TextureRegion region, float x, float y, float width, float height) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 float[] vertices = this.vertices;

		Texture texture = region.texture;
		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + 6 > triangles.Length || vertexIndex + SPRITE_SIZE > vertices.Length) //
			flush();

		int triangleIndex = this.triangleIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;
		triangles[triangleIndex++] = (short)startVertex;
		triangles[triangleIndex++] = (short)(startVertex + 1);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 3);
		triangles[triangleIndex++] = (short)startVertex;
		this.triangleIndex = triangleIndex;

		 float fx2 = x + width;
		 float fy2 = y + height;
		 float u = region.u;
		 float v = region.v2;
		 float u2 = region.u2;
		 float v2 = region.v;

		float color = this.colorPacked;
		int idx = this.vertexIndex;
		vertices[idx++] = x;
		vertices[idx++] = y;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v;

		vertices[idx++] = x;
		vertices[idx++] = fy2;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v2;

		vertices[idx++] = fx2;
		vertices[idx++] = fy2;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v2;

		vertices[idx++] = fx2;
		vertices[idx++] = y;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v;
		this.vertexIndex = idx;
	}

	public void draw (TextureRegion region, float x, float y, float originX, float originY, float width, float height,
		float scaleX, float scaleY, float rotation) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 float[] vertices = this.vertices;

		Texture texture = region.texture;
		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + 6 > triangles.Length || vertexIndex + SPRITE_SIZE > vertices.Length) //
			flush();

		int triangleIndex = this.triangleIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;
		triangles[triangleIndex++] = (short)startVertex;
		triangles[triangleIndex++] = (short)(startVertex + 1);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 3);
		triangles[triangleIndex++] = (short)startVertex;
		this.triangleIndex = triangleIndex;

		// bottom left and top right corner points relative to origin
		 float worldOriginX = x + originX;
		 float worldOriginY = y + originY;
		float fx = -originX;
		float fy = -originY;
		float fx2 = width - originX;
		float fy2 = height - originY;

		// scale
		if (scaleX != 1 || scaleY != 1) {
			fx *= scaleX;
			fy *= scaleY;
			fx2 *= scaleX;
			fy2 *= scaleY;
		}

		// construct corner points, start from top left and go counter clockwise
		 float p1x = fx;
		 float p1y = fy;
		 float p2x = fx;
		 float p2y = fy2;
		 float p3x = fx2;
		 float p3y = fy2;
		 float p4x = fx2;
		 float p4y = fy;

		float x1;
		float y1;
		float x2;
		float y2;
		float x3;
		float y3;
		float x4;
		float y4;

		// rotate
		if (rotation != 0) {
			 float cos = MathUtils.cosDeg(rotation);
			 float sin = MathUtils.sinDeg(rotation);

			x1 = cos * p1x - sin * p1y;
			y1 = sin * p1x + cos * p1y;

			x2 = cos * p2x - sin * p2y;
			y2 = sin * p2x + cos * p2y;

			x3 = cos * p3x - sin * p3y;
			y3 = sin * p3x + cos * p3y;

			x4 = x1 + (x3 - x2);
			y4 = y3 - (y2 - y1);
		} else {
			x1 = p1x;
			y1 = p1y;

			x2 = p2x;
			y2 = p2y;

			x3 = p3x;
			y3 = p3y;

			x4 = p4x;
			y4 = p4y;
		}

		x1 += worldOriginX;
		y1 += worldOriginY;
		x2 += worldOriginX;
		y2 += worldOriginY;
		x3 += worldOriginX;
		y3 += worldOriginY;
		x4 += worldOriginX;
		y4 += worldOriginY;

		 float u = region.u;
		 float v = region.v2;
		 float u2 = region.u2;
		 float v2 = region.v;

		float color = this.colorPacked;
		int idx = this.vertexIndex;
		vertices[idx++] = x1;
		vertices[idx++] = y1;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v;

		vertices[idx++] = x2;
		vertices[idx++] = y2;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v2;

		vertices[idx++] = x3;
		vertices[idx++] = y3;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v2;

		vertices[idx++] = x4;
		vertices[idx++] = y4;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v;
		this.vertexIndex = idx;
	}

	public void draw (TextureRegion region, float x, float y, float originX, float originY, float width, float height,
		float scaleX, float scaleY, float rotation, bool clockwise) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 float[] vertices = this.vertices;

		Texture texture = region.texture;
		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + 6 > triangles.Length || vertexIndex + SPRITE_SIZE > vertices.Length) //
			flush();

		int triangleIndex = this.triangleIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;
		triangles[triangleIndex++] = (short)startVertex;
		triangles[triangleIndex++] = (short)(startVertex + 1);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 3);
		triangles[triangleIndex++] = (short)startVertex;
		this.triangleIndex = triangleIndex;

		// bottom left and top right corner points relative to origin
		 float worldOriginX = x + originX;
		 float worldOriginY = y + originY;
		float fx = -originX;
		float fy = -originY;
		float fx2 = width - originX;
		float fy2 = height - originY;

		// scale
		if (scaleX != 1 || scaleY != 1) {
			fx *= scaleX;
			fy *= scaleY;
			fx2 *= scaleX;
			fy2 *= scaleY;
		}

		// construct corner points, start from top left and go counter clockwise
		 float p1x = fx;
		 float p1y = fy;
		 float p2x = fx;
		 float p2y = fy2;
		 float p3x = fx2;
		 float p3y = fy2;
		 float p4x = fx2;
		 float p4y = fy;

		float x1;
		float y1;
		float x2;
		float y2;
		float x3;
		float y3;
		float x4;
		float y4;

		// rotate
		if (rotation != 0) {
			 float cos = MathUtils.cosDeg(rotation);
			 float sin = MathUtils.sinDeg(rotation);

			x1 = cos * p1x - sin * p1y;
			y1 = sin * p1x + cos * p1y;

			x2 = cos * p2x - sin * p2y;
			y2 = sin * p2x + cos * p2y;

			x3 = cos * p3x - sin * p3y;
			y3 = sin * p3x + cos * p3y;

			x4 = x1 + (x3 - x2);
			y4 = y3 - (y2 - y1);
		} else {
			x1 = p1x;
			y1 = p1y;

			x2 = p2x;
			y2 = p2y;

			x3 = p3x;
			y3 = p3y;

			x4 = p4x;
			y4 = p4y;
		}

		x1 += worldOriginX;
		y1 += worldOriginY;
		x2 += worldOriginX;
		y2 += worldOriginY;
		x3 += worldOriginX;
		y3 += worldOriginY;
		x4 += worldOriginX;
		y4 += worldOriginY;

		float u1, v1, u2, v2, u3, v3, u4, v4;
		if (clockwise) {
			u1 = region.u2;
			v1 = region.v2;
			u2 = region.u;
			v2 = region.v2;
			u3 = region.u;
			v3 = region.v;
			u4 = region.u2;
			v4 = region.v;
		} else {
			u1 = region.u;
			v1 = region.v;
			u2 = region.u2;
			v2 = region.v;
			u3 = region.u2;
			v3 = region.v2;
			u4 = region.u;
			v4 = region.v2;
		}

		float color = this.colorPacked;
		int idx = this.vertexIndex;
		vertices[idx++] = x1;
		vertices[idx++] = y1;
		vertices[idx++] = color;
		vertices[idx++] = u1;
		vertices[idx++] = v1;

		vertices[idx++] = x2;
		vertices[idx++] = y2;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v2;

		vertices[idx++] = x3;
		vertices[idx++] = y3;
		vertices[idx++] = color;
		vertices[idx++] = u3;
		vertices[idx++] = v3;

		vertices[idx++] = x4;
		vertices[idx++] = y4;
		vertices[idx++] = color;
		vertices[idx++] = u4;
		vertices[idx++] = v4;
		this.vertexIndex = idx;
	}

	public void draw (TextureRegion region, float width, float height, Affine2 transform) {
		if (!drawing) throw new IllegalStateException("PolygonSpriteBatch.begin must be called before draw.");

		 short[] triangles = this.triangles;
		 float[] vertices = this.vertices;

		Texture texture = region.texture;
		if (texture != lastTexture)
			switchTexture(texture);
		else if (this.triangleIndex + 6 > triangles.Length || vertexIndex + SPRITE_SIZE > vertices.Length) //
			flush();

		int triangleIndex = this.triangleIndex;
		 int startVertex = vertexIndex / VERTEX_SIZE;
		triangles[triangleIndex++] = (short)startVertex;
		triangles[triangleIndex++] = (short)(startVertex + 1);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 2);
		triangles[triangleIndex++] = (short)(startVertex + 3);
		triangles[triangleIndex++] = (short)startVertex;
		this.triangleIndex = triangleIndex;

		// construct corner points
		float x1 = transform.m02;
		float y1 = transform.m12;
		float x2 = transform.m01 * height + transform.m02;
		float y2 = transform.m11 * height + transform.m12;
		float x3 = transform.m00 * width + transform.m01 * height + transform.m02;
		float y3 = transform.m10 * width + transform.m11 * height + transform.m12;
		float x4 = transform.m00 * width + transform.m02;
		float y4 = transform.m10 * width + transform.m12;

		float u = region.u;
		float v = region.v2;
		float u2 = region.u2;
		float v2 = region.v;

		float color = this.colorPacked;
		int idx = vertexIndex;
		vertices[idx++] = x1;
		vertices[idx++] = y1;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v;

		vertices[idx++] = x2;
		vertices[idx++] = y2;
		vertices[idx++] = color;
		vertices[idx++] = u;
		vertices[idx++] = v2;

		vertices[idx++] = x3;
		vertices[idx++] = y3;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v2;

		vertices[idx++] = x4;
		vertices[idx++] = y4;
		vertices[idx++] = color;
		vertices[idx++] = u2;
		vertices[idx++] = v;
		vertexIndex = idx;
	}

	public void flush () {
		if (vertexIndex == 0) return;

		renderCalls++;
		totalRenderCalls++;
		int trianglesInBatch = triangleIndex;
		if (trianglesInBatch > maxTrianglesInBatch) maxTrianglesInBatch = trianglesInBatch;

		lastTexture.bind();
		Mesh mesh = this.mesh;
		mesh.setVertices(vertices, 0, vertexIndex);
		mesh.setIndices(triangles, 0, trianglesInBatch);
		if (blendingDisabled) {
			Gdx.gl.glDisable(GL20.GL_BLEND);
		} else {
			Gdx.gl.glEnable(GL20.GL_BLEND);
			if (blendSrcFunc != -1) Gdx.gl.glBlendFuncSeparate(blendSrcFunc, blendDstFunc, blendSrcFuncAlpha, blendDstFuncAlpha);
		}

		mesh.render(customShader != null ? customShader : shader, GL20.GL_TRIANGLES, 0, trianglesInBatch);

		vertexIndex = 0;
		triangleIndex = 0;
	}

	public void disableBlending () {
		flush();
		blendingDisabled = true;
	}

	public void enableBlending () {
		flush();
		blendingDisabled = false;
	}

	public void setBlendFunction (int srcFunc, int dstFunc) {
		setBlendFunctionSeparate(srcFunc, dstFunc, srcFunc, dstFunc);
	}

	public void setBlendFunctionSeparate (int srcFuncColor, int dstFuncColor, int srcFuncAlpha, int dstFuncAlpha) {
		if (blendSrcFunc == srcFuncColor && blendDstFunc == dstFuncColor && blendSrcFuncAlpha == srcFuncAlpha
			&& blendDstFuncAlpha == dstFuncAlpha) return;
		flush();
		blendSrcFunc = srcFuncColor;
		blendDstFunc = dstFuncColor;
		blendSrcFuncAlpha = srcFuncAlpha;
		blendDstFuncAlpha = dstFuncAlpha;
	}

	public int getBlendSrcFunc () {
		return blendSrcFunc;
	}

	public int getBlendDstFunc () {
		return blendDstFunc;
	}

	public int getBlendSrcFuncAlpha () {
		return blendSrcFuncAlpha;
	}

	public int getBlendDstFuncAlpha () {
		return blendDstFuncAlpha;
	}

	public void dispose () {
		mesh.dispose();
		if (ownsShader && shader != null) shader.dispose();
	}

	public Matrix4 getProjectionMatrix () {
		return projectionMatrix;
	}

	public Matrix4 getTransformMatrix () {
		return transformMatrix;
	}

	public void setProjectionMatrix (Matrix4 projection) {
		if (drawing) flush();
		projectionMatrix.set(projection);
		if (drawing) setupMatrices();
	}

	public void setTransformMatrix (Matrix4 transform) {
		if (drawing) flush();
		transformMatrix.set(transform);
		if (drawing) setupMatrices();
	}

	protected void setupMatrices () {
		combinedMatrix.set(projectionMatrix).mul(transformMatrix);
		if (customShader != null) {
			customShader.setUniformMatrix("u_projTrans", combinedMatrix);
			customShader.setUniformi("u_texture", 0);
		} else {
			shader.setUniformMatrix("u_projTrans", combinedMatrix);
			shader.setUniformi("u_texture", 0);
		}
	}

	private void switchTexture (Texture texture) {
		flush();
		lastTexture = texture;
		invTexWidth = 1.0f / texture.getWidth();
		invTexHeight = 1.0f / texture.getHeight();
	}

	public void setShader (ShaderProgram shader) {
		if (drawing) {
			flush();
		}
		customShader = shader;
		if (drawing) {
			if (customShader != null)
				customShader.bind();
			else
				this.shader.bind();
			setupMatrices();
		}
	}

	public ShaderProgram getShader () {
		if (customShader == null) {
			return shader;
		}
		return customShader;
	}

	public bool isBlendingEnabled () {
		return !blendingDisabled;
	}

	public bool isDrawing () {
		return drawing;
	}
}
