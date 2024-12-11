////using SharpGDX.Tests.Utils;
////using SharpGDX.Utils;
////using SharpGDX.Scenes.Scene2D;
////using SharpGDX.Scenes.Scene2D.Utils;
////using SharpGDX.Scenes.Scene2D.UI;
////using SharpGDX.Graphics;
////using SharpGDX.Graphics.G2D;
////using SharpGDX.Utils.Viewports;
////using SharpGDX.Shims;
////using SharpGDX.Graphics.GLUtils;
////using System.Text;
////using SharpGDX.Mathematics;
////
////namespace SharpGDX.Tests;
////
////@GdxTestConfig(requireGL30 = true)
////public class VBOWithVAOPerformanceTest : GdxTest {
////
////	ShaderProgram shader;
////	Texture texture;
////	Matrix4 matrix = new Matrix4();
////
////	Mesh oldVBOWithVAOMesh;
////	Mesh newVBOWithVAOMesh;
////
////	SpriteBatch batch;
////	BitmapFont bitmapFont;
////	StringBuilder stringBuilder;
////
////	WindowedMean newCounter = new WindowedMean(100);
////	WindowedMean oldCounter = new WindowedMean(100);
////
////	WindowedMean newCounterStress = new WindowedMean(100);
////	WindowedMean oldCounterStress = new WindowedMean(100);
////
////	public override void Create () {
////		if (Gdx.gl30 == null) {
////			throw new GdxRuntimeException("GLES 3.0 profile required for this test");
////		}
////		String vertexShader = "attribute vec4 a_position;    \n" + "attribute vec4 a_color;\n" + "attribute vec2 a_texCoord0;\n"
////			+ "uniform mat4 u_worldView;\n" + "varying vec4 v_color;" + "varying vec2 v_texCoords;"
////			+ "void main()                  \n" + "{                            \n" + "   v_color = a_color; \n"
////			+ "   v_texCoords = a_texCoord0; \n" + "   gl_Position =  u_worldView * a_position;  \n"
////			+ "}                            \n";
////		String fragmentShader = "#ifdef GL_ES\n" + "precision mediump float;\n" + "#endif\n" + "varying vec4 v_color;\n"
////			+ "varying vec2 v_texCoords;\n" + "uniform sampler2D u_texture;\n" + "void main()                                  \n"
////			+ "{                                            \n" + "  gl_FragColor = v_color * texture2D(u_texture, v_texCoords);\n"
////			+ "}";
////
////		shader = new ShaderProgram(vertexShader, fragmentShader);
////		if (shader.isCompiled() == false) {
////			Gdx.app.log("ShaderTest", shader.getLog());
////			Gdx.app.exit();
////		}
////
////		int numSprites = 1000;
////		int maxIndices = numSprites * 6;
////		int maxVertices = numSprites * 6;
////
////		VertexAttribute[] vertexAttributes = new VertexAttribute[] {VertexAttribute.Position(), VertexAttribute.ColorUnpacked(),
////			VertexAttribute.TexCoords(0)};
////
////		VertexBufferObjectWithVAO newVBOWithVAO = new VertexBufferObjectWithVAO(false, maxVertices, vertexAttributes);
////		OldVertexBufferObjectWithVAO oldVBOWithVAO = new OldVertexBufferObjectWithVAO(false, maxVertices, vertexAttributes);
////
////		IndexBufferObjectSubData newIndices = new IndexBufferObjectSubData(false, maxIndices);
////		IndexBufferObjectSubData oldIndices = new IndexBufferObjectSubData(false, maxIndices);
////
////		newVBOWithVAOMesh = new Mesh(newVBOWithVAO, newIndices, false) {};
////		oldVBOWithVAOMesh = new Mesh(oldVBOWithVAO, oldIndices, false) {};
////
////		float[] vertexArray = new float[maxVertices * 9];
////		int index = 0;
////		int stride = 9 * 6;
////		for (int i = 0; i < numSprites; i++) {
////			addRandomSprite(vertexArray, index);
////			index += stride;
////		}
////		short[] indexArray = new short[maxIndices];
////		for (short i = 0; i < maxIndices; i++) {
////			indexArray[i] = i;
////		}
////
////		newVBOWithVAOMesh.setVertices(vertexArray);
////		newVBOWithVAOMesh.setIndices(indexArray);
////
////		oldVBOWithVAOMesh.setVertices(vertexArray);
////		oldVBOWithVAOMesh.setIndices(indexArray);
////
////		texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"));
////
////		batch = new SpriteBatch();
////		bitmapFont = new BitmapFont();
////		stringBuilder = new StringBuilder();
////	}
////
////	private void addRandomSprite (float[] vertArray, int currentIndex) {
////		float width = MathUtils.random(0.05f, 0.2f);
////		float height = MathUtils.random(0.05f, 0.2f);
////		float x = MathUtils.random(-1f, 1f);
////		float y = MathUtils.random(-1f, 1f);
////		float r = MathUtils.random();
////		float g = MathUtils.random();
////		float b = MathUtils.random();
////		float a = MathUtils.random();
////
////		vertArray[currentIndex++] = x;
////		vertArray[currentIndex++] = y;
////		vertArray[currentIndex++] = 0;
////		vertArray[currentIndex++] = r;
////		vertArray[currentIndex++] = g;
////		vertArray[currentIndex++] = b;
////		vertArray[currentIndex++] = a;
////		vertArray[currentIndex++] = 0;
////		vertArray[currentIndex++] = 1;
////
////		vertArray[currentIndex++] = x + width;
////		vertArray[currentIndex++] = y;
////		vertArray[currentIndex++] = 0;
////		vertArray[currentIndex++] = r;
////		vertArray[currentIndex++] = g;
////		vertArray[currentIndex++] = b;
////		vertArray[currentIndex++] = a;
////		vertArray[currentIndex++] = 1;
////		vertArray[currentIndex++] = 1;
////
////		vertArray[currentIndex++] = x + width;
////		vertArray[currentIndex++] = y + height;
////		vertArray[currentIndex++] = 0;
////		vertArray[currentIndex++] = r;
////		vertArray[currentIndex++] = g;
////		vertArray[currentIndex++] = b;
////		vertArray[currentIndex++] = a;
////		vertArray[currentIndex++] = 1;
////		vertArray[currentIndex++] = 0;
////
////		vertArray[currentIndex++] = x + width;
////		vertArray[currentIndex++] = y + height;
////		vertArray[currentIndex++] = 0;
////		vertArray[currentIndex++] = r;
////		vertArray[currentIndex++] = g;
////		vertArray[currentIndex++] = b;
////		vertArray[currentIndex++] = a;
////		vertArray[currentIndex++] = 1;
////		vertArray[currentIndex++] = 0;
////
////		vertArray[currentIndex++] = x;
////		vertArray[currentIndex++] = y + height;
////		vertArray[currentIndex++] = 0;
////		vertArray[currentIndex++] = r;
////		vertArray[currentIndex++] = g;
////		vertArray[currentIndex++] = b;
////		vertArray[currentIndex++] = a;
////		vertArray[currentIndex++] = 0;
////		vertArray[currentIndex++] = 0;
////
////		vertArray[currentIndex++] = x;
////		vertArray[currentIndex++] = y;
////		vertArray[currentIndex++] = 0;
////		vertArray[currentIndex++] = r;
////		vertArray[currentIndex++] = g;
////		vertArray[currentIndex++] = b;
////		vertArray[currentIndex++] = a;
////		vertArray[currentIndex++] = 0;
////		vertArray[currentIndex++] = 1;
////	}
////
////	public override void Render () {
////		Gdx.gl20.glViewport(0, 0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getBackBufferHeight());
////		Gdx.gl20.glClearColor(0.2f, 0.2f, 0.2f, 1);
////		Gdx.gl20.glClear(GL20.GL_COLOR_BUFFER_BIT);
////		Gdx.gl20.glEnable(GL20.GL_TEXTURE_2D);
////		Gdx.gl20.glEnable(GL20.GL_BLEND);
////		Gdx.gl20.glBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
////
////		texture.bind();
////		shader.bind();
////		shader.setUniformMatrix("u_worldView", matrix);
////		shader.setUniformi("u_texture", 0);
////
////		long beforeOld = TimeUtils.nanoTime();
////		oldVBOWithVAOMesh.render(shader, GL20.GL_TRIANGLES);
////		GDX.GL.glFlush();
////		oldCounter.addValue((TimeUtils.nanoTime() - beforeOld));
////
////		Gdx.gl20.glClear(GL20.GL_COLOR_BUFFER_BIT);
////
////		texture.bind();
////		shader.bind();
////		shader.setUniformMatrix("u_worldView", matrix);
////		shader.setUniformi("u_texture", 0);
////
////		long beforeNew = TimeUtils.nanoTime();
////		newVBOWithVAOMesh.render(shader, GL20.GL_TRIANGLES);
////		GDX.GL.glFlush();
////		newCounter.addValue((TimeUtils.nanoTime() - beforeNew));
////
////		Gdx.gl20.glClear(GL20.GL_COLOR_BUFFER_BIT);
////
////		texture.bind();
////		shader.bind();
////		shader.setUniformMatrix("u_worldView", matrix);
////		shader.setUniformi("u_texture", 0);
////
////		long beforeOldStress = TimeUtils.nanoTime();
////		for (int i = 0; i < 100; i++)
////			oldVBOWithVAOMesh.render(shader, GL20.GL_TRIANGLES);
////		GDX.GL.glFlush();
////		oldCounterStress.addValue((TimeUtils.nanoTime() - beforeOldStress));
////
////		Gdx.gl20.glClear(GL20.GL_COLOR_BUFFER_BIT);
////
////		texture.bind();
////		shader.bind();
////		shader.setUniformMatrix("u_worldView", matrix);
////		shader.setUniformi("u_texture", 0);
////
////		long beforeNewStress = TimeUtils.nanoTime();
////		for (int i = 0; i < 100; i++)
////			newVBOWithVAOMesh.render(shader, GL20.GL_TRIANGLES);
////		GDX.GL.glFlush();
////		newCounterStress.addValue((TimeUtils.nanoTime() - beforeNewStress));
////
////		batch.begin();
////		stringBuilder.SetLength(0);
////		stringBuilder.Append("O Mean Time: ");
////		stringBuilder.Append(oldCounter.getMean());
////		bitmapFont.draw(batch, stringBuilder, 0, 200);
////		stringBuilder.SetLength(0);
////		stringBuilder.Append("N Mean Time: ");
////		stringBuilder.Append(newCounter.getMean());
////		bitmapFont.draw(batch, stringBuilder, 0, 200 - 20);
////
////		float oldMean = oldCounter.getMean();
////		float newMean = newCounter.getMean();
////
////		float meanedAverage = newMean / oldMean;
////		stringBuilder.setLength(0);
////		stringBuilder.Append("New VBO time as a percentage of Old Time: ");
////		stringBuilder.Append(meanedAverage);
////		bitmapFont.draw(batch, stringBuilder, 0, 200 - 40);
////
////		stringBuilder.setLength(0);
////		stringBuilder.Append("Stress: O Mean Time: ");
////		stringBuilder.Append(oldCounterStress.getMean());
////		bitmapFont.draw(batch, stringBuilder, 0, 200 - 80);
////		stringBuilder.setLength(0);
////		stringBuilder.Append("Stress: N Mean Time: ");
////		stringBuilder.Append(newCounterStress.getMean());
////		bitmapFont.draw(batch, stringBuilder, 0, 200 - 100);
////
////		float oldMeanStress = oldCounterStress.getMean();
////		float newMeanStress = newCounterStress.getMean();
////
////		float meanedStressAverage = newMeanStress / oldMeanStress;
////		stringBuilder.setLength(0);
////		stringBuilder.Append("Stress: New VBO time as a percentage of Old Time: ");
////		stringBuilder.Append(meanedStressAverage);
////		bitmapFont.draw(batch, stringBuilder, 0, 200 - 120);
////
////		batch.end();
////	}
////
////	public override void Dispose () {
////		oldVBOWithVAOMesh.Dispose();
////		newVBOWithVAOMesh.Dispose();
////		texture.Dispose();
////		shader.Dispose();
////	}
////
////	private class OldVertexBufferObjectWithVAO : IVertexData {
////		readonly static IntBuffer tmpHandle = BufferUtils.newIntBuffer(1);
////
////        readonly VertexAttributes attributes;
////        readonly FloatBuffer buffer;
////        readonly ByteBuffer byteBuffer;
////		int bufferHandle;
////        readonly bool isStatic;
////        readonly int usage;
////		bool isDirty = false;
////		bool isBound = false;
////		bool vaoDirty = true;
////		int vaoHandle = -1;
////
////		public OldVertexBufferObjectWithVAO (bool isStatic, int numVertices, VertexAttribute[] attributes) {
////			this(isStatic, numVertices, new VertexAttributes(attributes));
////		}
////
////		public OldVertexBufferObjectWithVAO (bool isStatic, int numVertices, VertexAttributes attributes) {
////			this.isStatic = isStatic;
////			this.attributes = attributes;
////
////			byteBuffer = BufferUtils.newUnsafeByteBuffer(this.attributes.vertexSize * numVertices);
////			buffer = byteBuffer.asFloatBuffer();
////			((Buffer)buffer).flip();
////			((Buffer)byteBuffer).flip();
////			bufferHandle = Gdx.gl20.glGenBuffer();
////			usage = isStatic ? GL20.GL_STATIC_DRAW : GL20.GL_DYNAMIC_DRAW;
////		}
////
////		@Override
////		public VertexAttributes getAttributes () {
////			return attributes;
////		}
////
////		@Override
////		public int getNumVertices () {
////			return buffer.limit() * 4 / attributes.vertexSize;
////		}
////
////		@Override
////		public int getNumMaxVertices () {
////			return byteBuffer.capacity() / attributes.vertexSize;
////		}
////
////		@Deprecated
////		@Override
////		public FloatBuffer getBuffer () {
////			isDirty = true;
////			return buffer;
////		}
////
////		@Override
////		public FloatBuffer getBuffer (bool forWriting) {
////			isDirty |= forWriting;
////			return buffer;
////		}
////
////		private void bufferChanged () {
////			if (isBound) {
////				Gdx.gl20.glBufferData(GL20.GL_ARRAY_BUFFER, byteBuffer.limit(), byteBuffer, usage);
////				isDirty = false;
////			}
////		}
////
////		@Override
////		public void setVertices (float[] vertices, int offset, int count) {
////			isDirty = true;
////			BufferUtils.copy(vertices, byteBuffer, count, offset);
////			((Buffer)buffer).position(0);
////			((Buffer)buffer).limit(count);
////			bufferChanged();
////		}
////
////		@Override
////		public void updateVertices (int targetOffset, float[] vertices, int sourceOffset, int count) {
////			isDirty = true;
////			final int pos = byteBuffer.position();
////			((Buffer)byteBuffer).position(targetOffset * 4);
////			BufferUtils.copy(vertices, sourceOffset, count, byteBuffer);
////			((Buffer)byteBuffer).position(pos);
////			((Buffer)buffer).position(0);
////			bufferChanged();
////		}
////
////		@Override
////		public void bind (ShaderProgram shader) {
////			bind(shader, null);
////		}
////
////		@Override
////		public void bind (ShaderProgram shader, int[] locations) {
////			GL30 gl = Gdx.gl30;
////			if (vaoDirty || !gl.glIsVertexArray(vaoHandle)) {
////				// initialize the VAO with our vertex attributes and buffer:
////				((Buffer)tmpHandle).clear();
////				gl.glGenVertexArrays(1, tmpHandle);
////				vaoHandle = tmpHandle.get(0);
////				gl.glBindVertexArray(vaoHandle);
////				vaoDirty = false;
////
////			} else {
////				// else simply bind the VAO.
////				gl.glBindVertexArray(vaoHandle);
////			}
////
////			bindAttributes(shader, locations);
////
////			// if our data has changed upload it:
////			bindData(gl);
////
////			isBound = true;
////		}
////
////		private void bindAttributes (ShaderProgram shader, int[] locations) {
////			final GL20 gl = Gdx.gl20;
////			gl.glBindBuffer(GL20.GL_ARRAY_BUFFER, bufferHandle);
////			final int numAttributes = attributes.size();
////			if (locations == null) {
////				for (int i = 0; i < numAttributes; i++) {
////					final VertexAttribute attribute = attributes.get(i);
////					final int location = shader.getAttributeLocation(attribute.alias);
////					if (location < 0) continue;
////					shader.enableVertexAttribute(location);
////
////					shader.setVertexAttribute(location, attribute.numComponents, attribute.type, attribute.normalized,
////						attributes.vertexSize, attribute.offset);
////				}
////
////			} else {
////				for (int i = 0; i < numAttributes; i++) {
////					final VertexAttribute attribute = attributes.get(i);
////					final int location = locations[i];
////					if (location < 0) continue;
////					shader.enableVertexAttribute(location);
////
////					shader.setVertexAttribute(location, attribute.numComponents, attribute.type, attribute.normalized,
////						attributes.vertexSize, attribute.offset);
////				}
////			}
////		}
////
////		private void bindData (GL20 gl) {
////			if (isDirty) {
////				gl.glBindBuffer(GL20.GL_ARRAY_BUFFER, bufferHandle);
////				((Buffer)byteBuffer).limit(buffer.limit() * 4);
////				gl.glBufferData(GL20.GL_ARRAY_BUFFER, byteBuffer.limit(), byteBuffer, usage);
////				isDirty = false;
////			}
////		}
////
////		@Override
////		public void unbind (final ShaderProgram shader) {
////			unbind(shader, null);
////		}
////
////		@Override
////		public void unbind (final ShaderProgram shader, final int[] locations) {
////			GL30 gl = Gdx.gl30;
////			gl.glBindVertexArray(0);
////			isBound = false;
////		}
////
////		@Override
////		public void invalidate () {
////			bufferHandle = Gdx.gl20.glGenBuffer();
////			isDirty = true;
////			vaoDirty = true;
////		}
////
////		@Override
////		public void dispose () {
////			GL30 gl = Gdx.gl30;
////
////			gl.glBindBuffer(GL20.GL_ARRAY_BUFFER, 0);
////			gl.glDeleteBuffer(bufferHandle);
////			bufferHandle = 0;
////			BufferUtils.disposeUnsafeByteBuffer(byteBuffer);
////
////			if (gl.glIsVertexArray(vaoHandle)) {
////				((Buffer)tmpHandle).clear();
////				tmpHandle.put(vaoHandle);
////				((Buffer)tmpHandle).flip();
////				gl.glDeleteVertexArrays(1, tmpHandle);
////			}
////		}
////	}
////}
