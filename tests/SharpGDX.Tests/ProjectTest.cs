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
//public class ProjectTest : GdxTest {
//
//	Model sphere;
//	Camera cam;
//	SpriteBatch batch;
//	BitmapFont font;
//	ModelInstance[] instances = new ModelInstance[100];
//	ModelBatch modelBatch;
//	Vector3 tmp = new Vector3();
//	TextureRegion logo;
//
//	public override void Create () {
//		ObjLoader objLoader = new ObjLoader();
//		sphere = objLoader.loadModel(Gdx.files.@internal("data/sphere.obj"));
//		sphere.materials.get(0).set(new ColorAttribute(ColorAttribute.Diffuse, Color.WHITE));
//		cam = new PerspectiveCamera(45, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		cam.far = 200;
//		Random rand = new Random();
//		for (int i = 0; i < instances.length; i++) {
//			instances[i] = new ModelInstance(sphere, rand.nextFloat() * 100 - rand.nextFloat() * 100,
//				rand.nextFloat() * 100 - rand.nextFloat() * 100, rand.nextFloat() * -100 - 3);
//		}
//		batch = new SpriteBatch();
//		font = new BitmapFont();
//		logo = new TextureRegion(new Texture(Gdx.files.@internal("data/badlogicsmall.jpg")));
//		modelBatch = new ModelBatch();
//	}
//
//	public override void Render () {
//
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT);
//		GDX.GL.glEnable(GL20.GL_DEPTH_TEST);
//
//		cam.update();
//
//		modelBatch.begin(cam);
//
//		int visible = 0;
//		for (int i = 0; i < instances.length; i++) {
//			instances[i].transform.getTranslation(tmp);
//			if (cam.frustum.sphereInFrustum(tmp, 1)) {
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
//		GDX.GL.glDisable(GL20.GL_DEPTH_TEST);
//		batch.begin();
//		for (int i = 0; i < instances.length; i++) {
//			instances[i].transform.getTranslation(tmp);
//			cam.project(tmp);
//			if (tmp.z < 0) continue;
//			batch.draw(logo, tmp.x, tmp.y);
//		}
//		batch.end();
//	}
//}
