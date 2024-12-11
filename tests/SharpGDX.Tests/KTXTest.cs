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
///** Simple test and example for the KTX/ZKTX file format
// * @author Vincent Bousquet */
//public class KTXTest : GdxTest {
//
//	// 3D texture cubemap example
//	private PerspectiveCamera perspectiveCamera;
//	private CameraInputController inputController;
//	private ModelBatch modelBatch;
//	private Model model;
//	private ModelInstance instance;
//	private Environment environment;
//	private Cubemap cubemap;
//
//	// 2D texture alpha ETC1 example
//	private OrthographicCamera orthoCamera;
//	private Texture image;
//	private SpriteBatch batch;
//	private ShaderProgram etc1aShader;
//
//	// animation
//	private float time;
//
//	public override void Create () {
//
//		// Cubemap test
//
//		String cubemapVS = "" //
//			+ "attribute vec3 a_position;\n"//
//			+ "uniform mat4 u_projViewTrans;\n"//
//			+ "uniform mat4 u_worldTrans;\n"//
//			+ "\n"//
//			+ "varying vec3 v_cubeMapUV;\n"//
//			+ "\n"//
//			+ "void main() {\n"//
//			+ "   vec4 g_position = vec4(a_position, 1.0);\n"//
//			+ "   g_position = u_worldTrans * g_position;\n"//
//			+ "   v_cubeMapUV = normalize(g_position.xyz);\n"//
//			+ "   gl_Position = u_projViewTrans * g_position;\n"//
//			+ "}";
//		String cubemapFS = ""//
//			+ "#ifdef GL_ES\n"//
//			+ "precision mediump float;\n"//
//			+ "#endif\n"//
//			+ "uniform samplerCube u_environmentCubemap;\n"//
//			+ "varying vec3 v_cubeMapUV;\n"//
//			+ "void main() {\n" //
//			+ "	gl_FragColor = vec4(textureCube(u_environmentCubemap, v_cubeMapUV).rgb, 1.0);\n" //
//			+ "}\n";
//		modelBatch = new ModelBatch(new DefaultShaderProvider(new Config(cubemapVS, cubemapFS)));
//
//		cubemap = new Cubemap(new KTXTextureData(Gdx.files.@internal("data/cubemap.zktx"), true));
//		cubemap.setFilter(Texture.TextureFilter.MipMapLinearLinear, Texture.TextureFilter.Linear);
//
//		environment = new Environment();
//		environment.set(new ColorAttribute(ColorAttribute.AmbientLight, 0.1f, 0.1f, 0.1f, 1.f));
//		environment.add(new DirectionalLight().set(0.8f, 0.8f, 0.8f, -0.5f, -1.0f, -0.8f));
//		environment.set(new CubemapAttribute(CubemapAttribute.EnvironmentMap, cubemap));
//
//		perspectiveCamera = new PerspectiveCamera(67, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		perspectiveCamera.position.set(10f, 10f, 10f);
//		perspectiveCamera.lookAt(0, 0, 0);
//		perspectiveCamera.near = 0.1f;
//		perspectiveCamera.far = 300f;
//		perspectiveCamera.update();
//
//		ModelBuilder modelBuilder = new ModelBuilder();
//		model = modelBuilder.createBox(5f, 5f, 5f, new Material(ColorAttribute.createDiffuse(Color.GREEN)),
//			Usage.Position | Usage.Normal);
//		instance = new ModelInstance(model);
//
//		Gdx.input.setInputProcessor(new InputMultiplexer(this, inputController = new CameraInputController(perspectiveCamera)));
//
//		// 2D texture test
//		String etc1aVS = "" //
//			+ "uniform mat4 u_projTrans;\n"//
//			+ "\n"//
//			+ "attribute vec4 a_position;\n"//
//			+ "attribute vec2 a_texCoord0;\n"//
//			+ "attribute vec4 a_color;\n"//
//			+ "\n"//
//			+ "varying vec4 v_color;\n"//
//			+ "varying vec2 v_texCoord;\n"//
//			+ "\n"//
//			+ "void main() {\n"//
//			+ "   gl_Position = u_projTrans * a_position;\n"//
//			+ "   v_texCoord = a_texCoord0;\n"//
//			+ "   v_color = a_color;\n"//
//			+ "}\n";//
//		String etc1aFS = ""//
//			+ "#ifdef GL_ES\n"//
//			+ "precision mediump float;\n"//
//			+ "#endif\n"//
//			+ "uniform sampler2D u_texture;\n"//
//			+ "\n"//
//			+ "varying vec4 v_color;\n"//
//			+ "varying vec2 v_texCoord;\n"//
//			+ "\n"//
//			+ "void main() {\n"//
//			+ "   vec3 col = texture2D(u_texture, v_texCoord.st).rgb;\n"//
//			+ "   float alpha = texture2D(u_texture, v_texCoord.st + vec2(0.0, 0.5)).r;\n"//
//			+ "   gl_FragColor = vec4(col, alpha) * v_color;\n"//
//			+ "}\n";//
//		etc1aShader = new ShaderProgram(etc1aVS, etc1aFS);
//		orthoCamera = new OrthographicCamera(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		image = new Texture("data/egg.zktx");
//		batch = new SpriteBatch(100, etc1aShader);
//
//	}
//
//	public override void Render () {
//		time += Gdx.graphics.getDeltaTime();
//		inputController.update();
//		int gw = Gdx.graphics.getWidth(), gh = Gdx.graphics.getHeight();
//		int pw = gw > gh ? gw / 2 : gw, ph = gw > gh ? gh : gh / 2;
//
//		ScreenUtils.clear(0, 0, 0, 1, true);
//
//		// cubemap
//		GDX.GL.glViewport(gw - pw, gh - ph, pw, ph);
//		perspectiveCamera.viewportWidth = pw;
//		perspectiveCamera.viewportHeight = ph;
//		perspectiveCamera.update();
//		modelBatch.begin(perspectiveCamera);
//		modelBatch.render(instance, environment);
//		modelBatch.end();
//
//		// 2D texture with alpha & ETC1
//		GDX.GL.glViewport(0, 0, pw, ph);
//		orthoCamera.viewportWidth = pw;
//		orthoCamera.viewportHeight = ph;
//		orthoCamera.update();
//		batch.setProjectionMatrix(orthoCamera.combined);
//		batch.begin();
//		float s = 0.1f + 0.5f * (1 + MathUtils.sinDeg(time * 90.0f));
//		float w = s * image.getWidth(), h = s * image.getHeight() / 2, x = -w / 2, y = -h / 2;
//		batch.setShader(null);
//		batch.disableBlending();
//		batch.draw(image, -pw / 2, -ph / 2, pw, ph, 0, 1, 1, 0);
//		batch.setShader(etc1aShader);
//		batch.enableBlending();
//		batch.draw(image, x, y, w, h, 0, 0.5f, 1, 0);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		modelBatch.Dispose();
//		model.Dispose();
//		cubemap.Dispose();
//		image.Dispose();
//		batch.Dispose();
//		etc1aShader.Dispose();
//	}
//
//	public boolean needsGL20 () {
//		return true;
//	}
//
//	public void resume () {
//	}
//
//	public void resize (int width, int height) {
//	}
//
//	public void pause () {
//	}
//
//}
