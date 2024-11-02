//using SharpGDX.Tests.Utils;
//using SharpGDX.Utils;
//using SharpGDX.Scenes.Scene2D;
//using SharpGDX.Scenes.Scene2D.Utils;
//using SharpGDX.Scenes.Scene2D.UI;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Utils.Viewports;
//using SharpGDX.Shims;
//using SharpGDX.Mathematics;
//using SharpGDX.Graphics.GLUtils;
//
//namespace SharpGDX.Tests;
//
///** Draws a triangle and a trapezoid. The trapezoid is intersection between two triangles, one stencil and the triangle shown on
// * left. */
//public class FrameBufferTest : GdxTest {
//	FrameBuffer stencilFrameBuffer;
//	FrameBuffer frameBuffer;
//	Mesh mesh;
//
//	Mesh stencilMesh;
//	ShaderProgram meshShader;
//	Texture texture;
//	SpriteBatch spriteBatch;
//
//	public override void Render () {
//		frameBuffer.begin();
//		Gdx.gl20.glViewport(0, 0, frameBuffer.getWidth(), frameBuffer.getHeight());
//		Gdx.gl20.glClearColor(0f, 1f, 0f, 1);
//		Gdx.gl20.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		texture.bind();
//		meshShader.bind();
//		meshShader.setUniformi("u_texture", 0);
//		mesh.render(meshShader, GL20.GL_TRIANGLES);
//		frameBuffer.end();
//
//		stencilFrameBuffer.begin();
//		Gdx.gl20.glViewport(0, 0, frameBuffer.getWidth(), frameBuffer.getHeight());
//		Gdx.gl20.glClearColor(1f, 1f, 0f, 1);
//		Gdx.gl20.glClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_STENCIL_BUFFER_BIT);
//
//		Gdx.gl20.glEnable(GL20.GL_STENCIL_TEST);
//
//		Gdx.gl20.glColorMask(false, false, false, false);
//		Gdx.gl20.glDepthMask(false);
//		Gdx.gl20.glStencilFunc(GL20.GL_NEVER, 1, 0xFF);
//		Gdx.gl20.glStencilOp(GL20.GL_REPLACE, GL20.GL_KEEP, GL20.GL_KEEP);
//
//		Gdx.gl20.glStencilMask(0xFF);
//		Gdx.gl20.glClear(GL20.GL_STENCIL_BUFFER_BIT);
//
//		meshShader.bind();
//		stencilMesh.render(meshShader, GL20.GL_TRIANGLES);
//
//		Gdx.gl20.glColorMask(true, true, true, true);
//		Gdx.gl20.glDepthMask(true);
//		Gdx.gl20.glStencilMask(0x00);
//		Gdx.gl20.glStencilFunc(GL20.GL_EQUAL, 1, 0xFF);
//
//		meshShader.bind();
//		mesh.render(meshShader, GL20.GL_TRIANGLES);
//
//		Gdx.gl20.glDisable(GL20.GL_STENCIL_TEST);
//		stencilFrameBuffer.end();
//
//		Gdx.gl20.glViewport(0, 0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getBackBufferHeight());
//		Gdx.gl20.glClearColor(0.2f, 0.2f, 0.2f, 1);
//		Gdx.gl20.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		spriteBatch.begin();
//		spriteBatch.draw(frameBuffer.getColorBufferTexture(), 0, 0, 256, 256, 0, 0, frameBuffer.getColorBufferTexture().getWidth(),
//			frameBuffer.getColorBufferTexture().getHeight(), false, true);
//
//		spriteBatch.draw(stencilFrameBuffer.getColorBufferTexture(), 256, 256, 256, 256, 0, 0,
//			frameBuffer.getColorBufferTexture().getWidth(), frameBuffer.getColorBufferTexture().getHeight(), false, true);
//		spriteBatch.end();
//	}
//
//	public override void Create () {
//		mesh = new Mesh(true, 3, 0, new VertexAttribute(Usage.Position, 3, "a_Position"),
//			new VertexAttribute(Usage.ColorPacked, 4, "a_Color"), new VertexAttribute(Usage.TextureCoordinates, 2, "a_texCoords"));
//		float c1 = Color.toFloatBits(255, 0, 0, 255);
//		float c2 = Color.toFloatBits(255, 0, 0, 255);
//		float c3 = Color.toFloatBits(0, 0, 255, 255);
//
//		mesh.setVertices(new float[] {-0.5f, -0.5f, 0, c1, 0, 0, 0.5f, -0.5f, 0, c2, 1, 0, 0, 0.5f, 0, c3, 0.5f, 1});
//
//		stencilMesh = new Mesh(true, 3, 0, new VertexAttribute(Usage.Position, 3, "a_Position"),
//			new VertexAttribute(Usage.ColorPacked, 4, "a_Color"), new VertexAttribute(Usage.TextureCoordinates, 2, "a_texCoords"));
//		stencilMesh.setVertices(new float[] {-0.5f, 0.5f, 0, c1, 0, 0, 0.5f, 0.5f, 0, c2, 1, 0, 0, -0.5f, 0, c3, 0.5f, 1});
//
//		texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"));
//
//		spriteBatch = new SpriteBatch();
//		frameBuffer = new FrameBuffer(Format.RGB565, 128, 128, false);
//		stencilFrameBuffer = new FrameBuffer(Format.RGB565, 128, 128, false, true);
//		createShader(Gdx.graphics);
//	}
//
//	private void createShader (Graphics graphics) {
//		String vertexShader = "attribute vec4 a_Position;    \n" + "attribute vec4 a_Color;\n" + "attribute vec2 a_texCoords;\n"
//			+ "varying vec4 v_Color;" + "varying vec2 v_texCoords; \n" +
//
//			"void main()                  \n" + "{                            \n" + "   v_Color = a_Color;"
//			+ "   v_texCoords = a_texCoords;\n" + "   gl_Position =   a_Position;  \n" + "}                            \n";
//		String fragmentShader = "#ifdef GL_ES\n" + "precision mediump float;\n" + "#endif\n" + "varying vec4 v_Color;\n"
//			+ "varying vec2 v_texCoords; \n" + "uniform sampler2D u_texture;\n" +
//
//			"void main()                                  \n" + "{                                            \n"
//			+ "  gl_FragColor = v_Color * texture2D(u_texture, v_texCoords);\n" + "}";
//
//		meshShader = new ShaderProgram(vertexShader, fragmentShader);
//		if (meshShader.isCompiled() == false) throw new IllegalStateException(meshShader.getLog());
//	}
//
//	public override void Dispose () {
//		mesh.Dispose();
//		texture.Dispose();
//		frameBuffer.Dispose();
//		stencilFrameBuffer.Dispose();
//		stencilMesh.Dispose();
//		spriteBatch.Dispose();
//		meshShader.Dispose();
//	}
//
//}
