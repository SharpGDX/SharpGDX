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
//public class DecalTest : GdxTest {
//	public static final int TARGET_FPS = 40;
//	public static final int INITIAL_RENDERED = 100;
//	private boolean willItBlend_that_is_the_question = true;
//	Texture egg;
//	Texture wheel;
//	LinkedList<Decal> toRender = new LinkedList<Decal>();
//	DecalBatch batch;
//	float timePassed = 0;
//	int frames = 0;
//	Camera cam;
//	WindowedMean fps = new WindowedMean(5);
//	int idx = 0;
//	float w;
//	float h;
//
//	public override void Create () {
//		Gdx.gl.glEnable(GL20.GL_DEPTH_TEST);
//		Gdx.gl.glDepthFunc(GL20.GL_LESS);
//
//		egg = new Texture(Gdx.files.@internal("data/egg.png"));
//		egg.setFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
//		egg.setWrap(Texture.TextureWrap.ClampToEdge, Texture.TextureWrap.ClampToEdge);
//		wheel = new Texture(Gdx.files.@internal("data/sys.png"));
//		wheel.setFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
//		wheel.setWrap(Texture.TextureWrap.ClampToEdge, Texture.TextureWrap.ClampToEdge);
//
//		w = Gdx.graphics.getWidth() / 0.8f;
//		h = Gdx.graphics.getHeight() / 0.8f;
//		for (int i = 0; i < INITIAL_RENDERED; i++) {
//			toRender.add(makeDecal());
//		}
//		cam = new OrthographicCamera(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		cam.near = 0.1f;
//		cam.far = 10f;
//		cam.position.set(0, 0, 0.1f);
//		cam.direction.set(0, 0, -1f);
//		batch = new DecalBatch(new CameraGroupStrategy(cam));
//
//		Gdx.gl.glClearColor(1, 1, 0, 1);
//	}
//
//	public override void Dispose () {
//		egg.Dispose();
//		wheel.Dispose();
//		batch.Dispose();
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT);
//
//		float elapsed = Gdx.graphics.getDeltaTime();
//		float scale = timePassed > 0.5 ? 1 - timePassed / 2 : 0.5f + timePassed / 2;
//
//		for (Decal decal : toRender) {
//			decal.rotateZ(elapsed * 45);
//			decal.setScale(scale);
//			batch.add(decal);
//		}
//		batch.flush();
//
//		timePassed += elapsed;
//		frames++;
//		if (timePassed > 1.0f) {
//			Console.WriteLine("DecalPerformanceTest2 fps: " + frames + " at spritecount: " + toRender.size());
//			fps.addValue(frames);
//			if (fps.hasEnoughData()) {
//				float factor = fps.getMean() / (float)TARGET_FPS;
//				int target = (int)(toRender.size() * factor);
//				if (fps.getMean() > TARGET_FPS) {
//					int start = toRender.size();
//					for (int i = start; toRender.size() < target; i++) {
//						toRender.add(makeDecal());
//					}
//					fps.clear();
//				} else {
//					while (toRender.size() > target) {
//						toRender.removeLast();
//					}
//					fps.clear();
//				}
//			}
//			timePassed = 0;
//			frames = 0;
//		}
//	}
//
//	public override void Resize (int width, int height) {
//		w = Gdx.graphics.getWidth() / 0.8f;
//		h = Gdx.graphics.getHeight() / 0.8f;
//		cam = new OrthographicCamera(width, height);
//		cam.near = 0.1f;
//		cam.far = 10f;
//		cam.position.set(0, 0, 0.1f);
//		cam.direction.set(0, 0, -1f);
//	}
//
//	private Decal makeDecal () {
//		Decal sprite = null;
//		switch (idx % 2) {
//		case 0:
//			sprite = Decal.newDecal(new TextureRegion(egg), willItBlend_that_is_the_question);
//			break;
//		case 1:
//			sprite = Decal.newDecal(new TextureRegion(wheel));
//			break;
//		}
//		sprite.setPosition(-w / 2 + (float)Math.random() * w, h / 2 - (float)Math.random() * h, (float)-Math.random() * 10);
//		idx++;
//		return sprite;
//	}
//}
