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
///** @author xoppa */
//public class BulletTestCollection : GdxTest , Input.IInputProcessor, Input.GestureDetector.IGestureListener {
//	protected final BulletTest[] tests = {new BasicBulletTest(), new ShootTest(), new BasicShapesTest(), new KinematicTest(),
//		new ConstraintsTest(), new MeshShapeTest(), new GimpactTest(), new ConvexHullTest(), new ConvexHullDistanceTest(),
//		new RayCastTest(), new RayPickRagdollTest(), new InternalTickTest(), new CollisionWorldTest(), new CollisionTest(),
//		new FrustumCullingTest(), new CollisionDispatcherTest(), new ContactCallbackTest(), new ContactCallbackTest2(),
//		new ContactCacheTest(), new SoftBodyTest(), new SoftMeshTest(), new VehicleTest(), new VehicleFilterTest(),
//		new CharacterTest(), new ImportTest(), new TriangleRaycastTest(), new OcclusionCullingTest(), new PairCacheTest(),
//		new HeightFieldTest()};
//
//	protected int testIndex = 0;
//
//	private IApplication app = null;
//
//	private BitmapFont font;
//	private Stage hud;
//	private Label fpsLabel;
//	private Label titleLabel;
//	private Label instructLabel;
//	private int loading = 0;
//	private CameraInputController cameraController;
//
//	public override void Render () {
//		if ((loading > 0) && (++loading > 2)) loadnext();
//
//		tests[testIndex].render();
//		fpsLabel.setText(tests[testIndex].performance);
//		hud.draw();
//	}
//
//	public override void Create () {
//		if (app == null) {
//			app = Gdx.app;
//			tests[testIndex].create();
//		}
//
//		cameraController = new CameraInputController(tests[testIndex].camera);
//		cameraController.activateKey = Keys.CONTROL_LEFT;
//		cameraController.autoUpdate = false;
//		cameraController.forwardTarget = false;
//		cameraController.translateTarget = false;
//		Gdx.input.setInputProcessor(new InputMultiplexer(cameraController, this, new GestureDetector(this)));
//
//		font = new BitmapFont(Gdx.files.@internal("data/lsans-15.fnt"), false);
//		hud = new Stage();
//		hud.addActor(fpsLabel = new Label(" ", new Label.LabelStyle(font, Color.WHITE)));
//		fpsLabel.setPosition(0, 0);
//		hud.addActor(titleLabel = new Label(tests[testIndex].getClass().getSimpleName(), new Label.LabelStyle(font, Color.WHITE)));
//		titleLabel.setY(hud.getHeight() - titleLabel.getHeight());
//		hud.addActor(instructLabel = new Label("A\nB\nC\nD\nE\nF", new Label.LabelStyle(font, Color.WHITE)));
//		instructLabel.setY(titleLabel.getY() - instructLabel.getHeight());
//		instructLabel.setAlignment(Align.top | Align.left);
//		instructLabel.setText(tests[testIndex].instructions);
//	}
//
//	public override void Resize (int width, int height) {
//		hud.getViewport().update(width, height, true);
//	}
//
//	public override void Dispose () {
//		tests[testIndex].Dispose();
//		app = null;
//	}
//
//	public void next () {
//		titleLabel.setText("Loading...");
//		loading = 1;
//	}
//
//	public void loadnext () {
//		app.log("TestCollection", "disposing test '" + tests[testIndex].getClass().getName() + "'");
//		tests[testIndex].Dispose();
//		// This would be a good time for GC to kick in.
//		System.gc();
//		testIndex++;
//		if (testIndex >= tests.length) testIndex = 0;
//		tests[testIndex].create();
//		cameraController.camera = tests[testIndex].camera;
//		app.log("TestCollection", "created test '" + tests[testIndex].getClass().getName() + "'");
//
//		titleLabel.setText(tests[testIndex].getClass().getSimpleName());
//		instructLabel.setText(tests[testIndex].instructions);
//		loading = 0;
//	}
//
//	public override bool KeyDown (int keycode) {
//		return tests[testIndex].keyDown(keycode);
//	}
//
//	public override bool KeyTyped (char character) {
//		return tests[testIndex].keyTyped(character);
//	}
//
//	public override bool KeyUp (int keycode) {
//		boolean result = tests[testIndex].keyUp(keycode);
//		if ((result == false) && (keycode == Keys.SPACE || keycode == Keys.MENU)) {
//			next();
//			result = true;
//		}
//		return result;
//	}
//
//	public override bool TouchDown (int x, int y, int pointer, int button) {
//		return tests[testIndex].touchDown(x, y, pointer, button);
//	}
//
//	public override bool TouchDragged (int x, int y, int pointer) {
//		return tests[testIndex].touchDragged(x, y, pointer);
//	}
//
//	public override bool TouchUp (int x, int y, int pointer, int button) {
//		return tests[testIndex].touchUp(x, y, pointer, button);
//	}
//
//	public override bool MouseMoved (int x, int y) {
//		return tests[testIndex].mouseMoved(x, y);
//	}
//
//	public override bool Scrolled (float amountX, float amountY) {
//		return tests[testIndex].scrolled(amountX, amountY);
//	}
//
//	public override bool TouchDown (float x, float y, int pointer, int button) {
//		return tests[testIndex].touchDown(x, y, pointer, button);
//	}
//
//	@Override
//	public boolean tap (float x, float y, int count, int button) {
//		return tests[testIndex].tap(x, y, count, button);
//	}
//
//	@Override
//	public boolean longPress (float x, float y) {
//		return tests[testIndex].longPress(x, y);
//	}
//
//	@Override
//	public boolean fling (float velocityX, float velocityY, int button) {
//		if (tests[testIndex].fling(velocityX, velocityY, button) == false) next();
//		return true;
//	}
//
//	@Override
//	public boolean pan (float x, float y, float deltaX, float deltaY) {
//		return tests[testIndex].pan(x, y, deltaX, deltaY);
//	}
//
//	@Override
//	public boolean panStop (float x, float y, int pointer, int button) {
//		return tests[testIndex].panStop(x, y, pointer, button);
//	}
//
//	@Override
//	public boolean zoom (float originalDistance, float currentDistance) {
//		return tests[testIndex].zoom(originalDistance, currentDistance);
//	}
//
//	@Override
//	public boolean pinch (Vector2 initialFirstPointer, Vector2 initialSecondPointer, Vector2 firstPointer, Vector2 secondPointer) {
//		return tests[testIndex].pinch(initialFirstPointer, initialSecondPointer, firstPointer, secondPointer);
//	}
//
//	@Override
//	public void pinchStop () {
//	}
//}
