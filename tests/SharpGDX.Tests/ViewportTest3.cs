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
///** Cycles viewports while rendering with SpriteBatch. */
//public class ViewportTest3 : GdxTest {
//	Array<Viewport> viewports;
//	Viewport viewport;
//	Array<String> names;
//	String name;
//
//	private PerspectiveCamera camera;
//	public Environment environment;
//	public DirectionalLight shadowLight;
//	public ModelBuilder modelBuilder;
//	public ModelBatch modelBatch;
//	public ModelInstance boxInstance;
//
//	public void create () {
//		modelBatch = new ModelBatch();
//		modelBuilder = new ModelBuilder();
//
//		environment = new Environment();
//		environment.set(new ColorAttribute(ColorAttribute.AmbientLight, 0.3f, 0.3f, 0.3f, 1.f));
//		shadowLight = new DirectionalLight();
//		shadowLight.set(0.8f, 0.8f, 0.8f, -0.5f, -1f, 0.7f);
//		environment.add(shadowLight);
//
//		modelBatch = new ModelBatch();
//
//		camera = new PerspectiveCamera();
//		camera.fieldOfView = 67;
//		camera.near = 0.1f;
//		camera.far = 300f;
//		camera.position.set(0, 0, 100);
//		camera.lookAt(0, 0, 0);
//
//		viewports = ViewportTest1.getViewports(camera);
//		viewport = viewports.first();
//
//		names = ViewportTest1.getViewportNames();
//		name = names.first();
//
//		ModelBuilder modelBuilder = new ModelBuilder();
//		Model boxModel = modelBuilder.createBox(50f, 50f, 50f, new Material(ColorAttribute.createDiffuse(Color.GREEN)),
//			Usage.Position | Usage.Normal);
//		boxInstance = new ModelInstance(boxModel);
//		boxInstance.transform.rotate(1, 0, 0, 30);
//		boxInstance.transform.rotate(0, 1, 0, 30);
//
//		Gdx.input.setInputProcessor(new InputAdapter() {
//			public boolean keyDown (int keycode) {
//				if (keycode == Input.Keys.SPACE) {
//					int index = (viewports.indexOf(viewport, true) + 1) % viewports.size;
//					name = names.get(index);
//					viewport = viewports.get(index);
//					resize(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//				}
//				return false;
//			}
//		});
//	}
//
//	public void render () {
//		ScreenUtils.clear(0, 0, 0, 1, true);
//
//		modelBatch.begin(camera);
//		modelBatch.render(boxInstance, environment);
//		modelBatch.end();
//	}
//
//	public void resize (int width, int height) {
//		Console.WriteLine(name);
//		viewport.update(width, height);
//	}
//}
