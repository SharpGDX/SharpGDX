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
//public class EdgeDetectionTest : GdxTest {
//
//	FPSLogger logger;
//	// ShaderProgram shader;
//	Model scene;
//	ModelInstance sceneInstance;
//	ModelBatch modelBatch;
//	FrameBuffer fbo;
//	PerspectiveCamera cam;
//	Matrix4 matrix = new Matrix4();
//	float angle = 0;
//	TextureRegion fboRegion;
//	SpriteBatch batch;
//	ShaderProgram batchShader;
//
//	float[] filter = {0, 0.25f, 0, 0.25f, -1f, 0.6f, 0, 0.25f, 0,};
//
//	float[] offsets = new float[18];
//
//	public void create () {
//		ShaderProgram.pedantic = false;
//		/*
//		 * shader = new ShaderProgram(Gdx.files.@internal("data/shaders/default.vert").readString(), Gdx.files.@internal(
//		 * "data/shaders/depthtocolor.frag").readString()); if (!shader.isCompiled()) { Gdx.app.log("EdgeDetectionTest",
//		 * "couldn't compile scene shader: " + shader.getLog()); }
//		 */
//		batchShader = new ShaderProgram(Gdx.files.@internal("data/shaders/batch.vert").readString(),
//			Gdx.files.@internal("data/shaders/convolution.frag").readString());
//		if (!batchShader.isCompiled()) {
//			Gdx.app.log("EdgeDetectionTest", "couldn't compile post-processing shader: " + batchShader.getLog());
//		}
//
//		ObjLoader objLoader = new ObjLoader();
//		scene = objLoader.loadModel(Gdx.files.@internal("data/scene.obj"));
//		sceneInstance = new ModelInstance(scene);
//		modelBatch = new ModelBatch();
//		fbo = new FrameBuffer(Format.RGB565, Gdx.graphics.getWidth(), Gdx.graphics.getHeight(), true);
//		cam = new PerspectiveCamera(67, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		cam.position.set(0, 0, 10);
//		cam.lookAt(0, 0, 0);
//		cam.far = 30;
//		batch = new SpriteBatch();
//		batch.setShader(batchShader);
//		fboRegion = new TextureRegion(fbo.getColorBufferTexture());
//		fboRegion.flip(false, true);
//		logger = new FPSLogger();
//		calculateOffsets();
//	}
//
//	public override void Dispose () {
//		batchShader.Dispose();
//		scene.Dispose();
//		fbo.Dispose();
//		batch.Dispose();
//	}
//
//	private void calculateOffsets () {
//		int idx = 0;
//		for (int y = -1; y <= 1; y++) {
//			for (int x = -1; x <= 1; x++) {
//				offsets[idx++] = x / (float)Gdx.graphics.getWidth();
//				offsets[idx++] = y / (float)Gdx.graphics.getHeight();
//			}
//		}
//		Console.WriteLine(Arrays.toString(offsets));
//	}
//
//	public void render () {
//		angle += 45 * Gdx.graphics.getDeltaTime();
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT);
//
//		cam.update();
//		matrix.setToRotation(0, 1, 0, angle);
//		cam.combined.mul(matrix);
//
//		fbo.begin();
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT);
//		GDX.GL.glEnable(GL20.GL_DEPTH_TEST);
//		modelBatch.begin(cam);
//		modelBatch.render(sceneInstance);
//		modelBatch.end();
//		fbo.end();
//
//		batch.begin();
//		batch.disableBlending();
//		batchShader.setUniformi("u_filterSize", filter.length);
//		batchShader.setUniform1fv("u_filter", filter, 0, filter.length);
//		batchShader.setUniform2fv("u_offsets", offsets, 0, offsets.length);
//		batch.draw(fboRegion, 0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		batch.end();
//		logger.log();
//	}
//}
