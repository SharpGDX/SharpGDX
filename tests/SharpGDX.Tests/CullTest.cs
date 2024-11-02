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
//
//namespace SharpGDX.Tests;
//
//public class CullTest : GdxTest implements ApplicationListener {
//
//	Model sphere;
//	Camera cam;
//	SpriteBatch batch;
//	ModelBatch modelBatch;
//	BitmapFont font;
//	ModelInstance[] instances = new ModelInstance[100];
//	final Vector3 pos = new Vector3();
//
//	public override void Create () {
//		ModelBuilder builder = new ModelBuilder();
//		sphere = builder.createSphere(2f, 2f, 2f, 16, 16, new Material(new ColorAttribute(ColorAttribute.Diffuse, Color.WHITE)),
//			Usage.Position | Usage.Normal);
//		// cam = new PerspectiveCamera(45, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		cam = new OrthographicCamera(45, 45 * (Gdx.graphics.getWidth() / (float)Gdx.graphics.getHeight()));
//
//		cam.near = 1;
//		cam.far = 200;
//
//		Random rand = new Random();
//		for (int i = 0; i < instances.length; i++) {
//			pos.set(rand.nextFloat() * 100 - rand.nextFloat() * 100, rand.nextFloat() * 100 - rand.nextFloat() * 100,
//				rand.nextFloat() * -100 - 3);
//			instances[i] = new ModelInstance(sphere, pos);
//		}
//		modelBatch = new ModelBatch();
//
//		batch = new SpriteBatch();
//		font = new BitmapFont();
//		// Gdx.graphics.setVSync(true);
//		// Gdx.app.log("CullTest", "" + Gdx.graphics.getBufferFormat().toString());
//	}
//
//	public override void Render () {
//		GL20 gl = Gdx.gl20;
//
//		gl.glClearColor(0, 0, 0, 0);
//		gl.glClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT);
//		gl.glEnable(GL20.GL_DEPTH_TEST);
//
//		cam.update();
//		modelBatch.begin(cam);
//
//		int visible = 0;
//		for (int i = 0; i < instances.length; i++) {
//			instances[i].transform.getTranslation(pos);
//			if (cam.frustum.sphereInFrustum(pos, 1)) {
//				((ColorAttribute)instances[i].materials.get(0).get(ColorAttribute.Diffuse)).color.set(Color.WHITE);
//				visible++;
//			} else {
//				((ColorAttribute)instances[i].materials.get(0).get(ColorAttribute.Diffuse)).color.set(Color.RED);
//			}
//			modelBatch.render(instances[i]);
//		}
//		modelBatch.end();
//
//		if (Gdx.input.isKeyPressed(Keys.A)) cam.rotate(20 * Gdx.graphics.getDeltaTime(), 0, 1, 0);
//		if (Gdx.input.isKeyPressed(Keys.D)) cam.rotate(-20 * Gdx.graphics.getDeltaTime(), 0, 1, 0);
//
//		gl.glDisable(GL20.GL_DEPTH_TEST);
//		batch.begin();
//		font.draw(batch, "visible: " + visible + "/100" + ", fps: " + Gdx.graphics.getFramesPerSecond(), 0, 20);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		font.Dispose();
//		sphere.Dispose();
//	}
//}
