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
//public class FramebufferToTextureTest : GdxTest {
//
//	TextureRegion fbTexture;
//	Texture texture;
//	Model mesh;
//	ModelInstance modelInstance;
//	ModelBatch modelBatch;
//	PerspectiveCamera cam;
//	SpriteBatch batch;
//	BitmapFont font;
//	Color clearColor = new Color(0.2f, 0.2f, 0.2f, 1);
//	float angle = 0;
//
//	public override void Create () {
//		texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"), true);
//		texture.setFilter(Texture.TextureFilter.MipMap, Texture.TextureFilter.Linear);
//		ObjLoader objLoader = new ObjLoader();
//		mesh = objLoader.loadModel(Gdx.files.@internal("data/cube.obj"));
//		mesh.materials.get(0).set(new TextureAttribute(TextureAttribute.Diffuse, texture));
//		modelInstance = new ModelInstance(mesh);
//		modelBatch = new ModelBatch();
//
//		cam = new PerspectiveCamera(67, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		cam.position.set(3, 3, 3);
//		cam.direction.set(-1, -1, -1);
//		batch = new SpriteBatch();
//		font = new BitmapFont();
//	}
//
//	public override void Render () {
//		GDX.GL.glViewport(0, 0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getBackBufferHeight());
//		GDX.GL.glClearColor(clearColor.r, clearColor.g, clearColor.b, clearColor.a);
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT);
//		GDX.GL.glEnable(GL20.GL_DEPTH_TEST);
//
//		cam.update();
//
//		modelInstance.transform.rotate(Vector3.Y, 45 * Gdx.graphics.getDeltaTime());
//		modelBatch.begin(cam);
//		modelBatch.render(modelInstance);
//		modelBatch.end();
//
//		if (Gdx.input.justTouched() || fbTexture == null) {
//			if (fbTexture != null) fbTexture.getTexture().Dispose();
//			fbTexture = ScreenUtils.getFrameBufferTexture();
//		}
//
//		batch.begin();
//		if (fbTexture != null) {
//			batch.draw(fbTexture, 0, 0, 100, 100);
//		}
//		font.draw(batch, "Touch screen to take a snapshot", 10, 40);
//		batch.end();
//	}
//
//	public override void Pause () {
//		fbTexture = null;
//	}
//}
