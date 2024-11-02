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
//public class MeshShaderTest : GdxTest {
//	ShaderProgram shader;
//	Mesh mesh, meshCustomVA;
//	Texture texture;
//	Matrix4 matrix = new Matrix4();
//
//	public override void Create () {
//		String vertexShader = "attribute vec4 a_position;    \n" + "attribute vec4 a_color;\n" + "attribute vec2 a_texCoord0;\n"
//			+ "uniform mat4 u_worldView;\n" + "varying vec4 v_color;" + "varying vec2 v_texCoords;"
//			+ "void main()                  \n" + "{                            \n" + "   v_color = vec4(1, 1, 1, 1); \n"
//			+ "   v_texCoords = a_texCoord0; \n" + "   gl_Position =  u_worldView * a_position;  \n"
//			+ "}                            \n";
//		String fragmentShader = "#ifdef GL_ES\n" + "precision mediump float;\n" + "#endif\n" + "varying vec4 v_color;\n"
//			+ "varying vec2 v_texCoords;\n" + "uniform sampler2D u_texture;\n" + "void main()                                  \n"
//			+ "{                                            \n" + "  gl_FragColor = v_color * texture2D(u_texture, v_texCoords);\n"
//			+ "}";
//
//		shader = new ShaderProgram(vertexShader, fragmentShader);
//		if (shader.isCompiled() == false) {
//			Gdx.app.log("ShaderTest", shader.getLog());
//			Gdx.app.exit();
//		}
//
//		mesh = new Mesh(true, 4, 6, VertexAttribute.Position(), VertexAttribute.ColorUnpacked(), VertexAttribute.TexCoords(0));
//		mesh.setVertices(new float[] {-0.5f, -0.5f, 0, 1, 1, 1, 1, 0, 1, 0.5f, -0.5f, 0, 1, 1, 1, 1, 1, 1, 0.5f, 0.5f, 0, 1, 1, 1,
//			1, 1, 0, -0.5f, 0.5f, 0, 1, 1, 1, 1, 0, 0});
//		mesh.setIndices(new short[] {0, 1, 2, 2, 3, 0});
//
//		// Mesh with texCoords wearing a pair of shorts. :)
//		meshCustomVA = new Mesh(true, 4, 6, VertexAttribute.Position(), VertexAttribute.ColorPacked(), new VertexAttribute(
//			Usage.TextureCoordinates, 2, GL20.GL_UNSIGNED_SHORT, true, ShaderProgram.TEXCOORD_ATTRIBUTE + "0", 0));
//		meshCustomVA.setVertices(new float[] {-0.5f, -0.5f, 0, Color.WHITE_FLOAT_BITS, toSingleFloat(0, 1), 0.5f, -0.5f, 0,
//			Color.WHITE_FLOAT_BITS, toSingleFloat(1, 1), 0.5f, 0.5f, 0, Color.WHITE_FLOAT_BITS, toSingleFloat(1, 0), -0.5f, 0.5f, 0,
//			Color.WHITE_FLOAT_BITS, toSingleFloat(0, 0)});
//		meshCustomVA.setIndices(new short[] {0, 1, 2, 2, 3, 0});
//
//		texture = new Texture(Gdx.files.@internal("data/bobrgb888-32x32.png"));
//	}
//
//	private static float toSingleFloat (float u, float v) {
//		int vu = ((int)(v * 0xffff) << 16) | (int)(u * 0xffff);
//		return NumberUtils.intToFloatColor(vu); // v will lose some precision due to masking
//	}
//
//	Vector3 axis = new Vector3(0, 0, 1);
//	float angle = 0;
//
//	public override void Render () {
//		angle += Gdx.graphics.getDeltaTime() * 45;
//		matrix.setToRotation(axis, angle);
//
//		Mesh meshToDraw = Gdx.input.isButtonPressed(0) ? meshCustomVA : mesh;
//
//		Gdx.gl20.glViewport(0, 0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getBackBufferHeight());
//		Gdx.gl20.glClearColor(0.2f, 0.2f, 0.2f, 1);
//		Gdx.gl20.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		Gdx.gl20.glEnable(GL20.GL_TEXTURE_2D);
//		Gdx.gl20.glEnable(GL20.GL_BLEND);
//		Gdx.gl20.glBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
//		texture.bind();
//		shader.bind();
//		shader.setUniformMatrix("u_worldView", matrix);
//		shader.setUniformi("u_texture", 0);
//		meshToDraw.render(shader, GL20.GL_TRIANGLES);
//	}
//
//	public override void Dispose () {
//		mesh.Dispose();
//		texture.Dispose();
//		shader.Dispose();
//	}
//}
