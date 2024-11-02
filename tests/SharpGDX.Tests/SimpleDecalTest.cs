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
//public class SimpleDecalTest : GdxTest {
//	private static readonly int NUM_DECALS = 3;
//	DecalBatch batch;
//	Array<Decal> decals = new Array<Decal>();
//	PerspectiveCamera camera;
//	PerspectiveCamController controller;
//	FPSLogger logger = new FPSLogger();
//
//	public void create () {
//		float width = Gdx.graphics.getWidth();
//		float height = Gdx.graphics.getHeight();
//
//		camera = new PerspectiveCamera(45, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		camera.near = 1;
//		camera.far = 300;
//		camera.position.set(0, 0, 5);
//		controller = new PerspectiveCamController(camera);
//
//		Gdx.input.setInputProcessor(controller);
//		batch = new DecalBatch(new CameraGroupStrategy(camera));
//
//		TextureRegion[] textures = {new TextureRegion(new Texture(Gdx.files.@internal("data/egg.png"))),
//			new TextureRegion(new Texture(Gdx.files.@internal("data/sys.png"))),
//			new TextureRegion(new Texture(Gdx.files.@internal("data/badlogic.jpg")))};
//
//		Decal decal = Decal.newDecal(1, 1, textures[1]);
//		decal.setPosition(0, 0, 0);
//		decals.add(decal);
//
//		decal = Decal.newDecal(1, 1, textures[0], true);
//		decal.setPosition(0.5f, 0.5f, 1);
//		decals.add(decal);
//
//		decal = Decal.newDecal(1, 1, textures[0], true);
//		decal.setPosition(1, 1, -1);
//		decals.add(decal);
//
//		decal = Decal.newDecal(1, 1, textures[2]);
//		decal.setPosition(1.5f, 1.5f, -2);
//		decals.add(decal);
//
//		decal = Decal.newDecal(1, 1, textures[1]);
//		decal.setPosition(2, 2, -1.5f);
//		decals.add(decal);
//	}
//
//	Vector3 dir = new Vector3();
//	private boolean billboard = true;
//
//	public void render () {
//		ScreenUtils.clear(Color.DARK_GRAY, true);
//		Gdx.gl.glEnable(GL20.GL_DEPTH_TEST);
//
//		camera.update();
//		for (int i = 0; i < decals.size; i++) {
//			Decal decal = decals.get(i);
//			if (billboard) {
//				// billboarding for ortho cam :)
//// dir.set(-camera.direction.x, -camera.direction.y, -camera.direction.z);
//// decal.setRotation(dir, Vector3.Y);
//
//				// billboarding for perspective cam
//				decal.lookAt(camera.position, camera.up);
//			}
//			batch.add(decal);
//		}
//		batch.flush();
//		logger.log();
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//	}
//}
