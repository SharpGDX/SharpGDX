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
//public class Box2DTestCollection : GdxTest implements InputProcessor, GestureListener {
//	private final Box2DTest[] tests = {new DebugRendererTest(), new CollisionFiltering(), new Chain(), new Bridge(),
//		new SphereStack(), new Cantilever(), new ApplyForce(), new ContinuousTest(), new Prismatic(), new CharacterCollision(),
//		new BodyTypes(), new SimpleTest(), new Pyramid(), new OneSidedPlatform(), new VerticalStack(), new VaryingRestitution(),
//		new ConveyorBelt()};
//
//	private int testIndex = 0;
//
//	private Application app = null;
//
//	public override void Render () {
//		tests[testIndex].render();
//	}
//
//	public override void Create () {
//		if (this.app == null) {
//			this.app = Gdx.app;
//			Box2DTest test = tests[testIndex];
//			test.create();
//		}
//
//		InputMultiplexer multiplexer = new InputMultiplexer();
//		multiplexer.addProcessor(this);
//		multiplexer.addProcessor(new GestureDetector(this));
//		Gdx.input.setInputProcessor(multiplexer);
//	}
//
//	public override void Dispose () {
//		tests[testIndex].Dispose();
//	}
//
//	public override bool KeyDown (int keycode) {
//		tests[testIndex].keyDown(keycode);
//
//		return false;
//	}
//
//	public override bool KeyTyped (char character) {
//		tests[testIndex].keyTyped(character);
//		return false;
//	}
//
//	public override bool KeyUp (int keycode) {
//		tests[testIndex].keyUp(keycode);
//		return false;
//	}
//
//	public override bool TouchDown (int x, int y, int pointer, int button) {
//		tests[testIndex].touchDown(x, y, pointer, button);
//		return false;
//	}
//
//	public override bool TouchDragged (int x, int y, int pointer) {
//		tests[testIndex].touchDragged(x, y, pointer);
//		return false;
//	}
//
//	public override bool TouchUp (int x, int y, int pointer, int button) {
//		tests[testIndex].touchUp(x, y, pointer, button);
//		return false;
//	}
//
//	public override bool MouseMoved (int x, int y) {
//		return false;
//	}
//
//	public override bool Scrolled (float amountX, float amountY) {
//		return false;
//	}
//
//	public override bool TouchDown (float x, float y, int pointer, int button) {
//		return false;
//	}
//
//	@Override
//	public boolean tap (float x, float y, int count, int button) {
//		app.log("TestCollection", "disposing test '" + tests[testIndex].getClass().getName());
//		tests[testIndex].Dispose();
//		testIndex++;
//		if (testIndex >= tests.length) testIndex = 0;
//		Box2DTest test = tests[testIndex];
//		test.create();
//		app.log("TestCollection", "created test '" + tests[testIndex].getClass().getName());
//		return false;
//	}
//
//	@Override
//	public boolean longPress (float x, float y) {
//		return false;
//	}
//
//	@Override
//	public boolean fling (float velocityX, float velocityY, int button) {
//		return false;
//	}
//
//	@Override
//	public boolean pan (float x, float y, float deltaX, float deltaY) {
//		return false;
//	}
//
//	@Override
//	public boolean panStop (float x, float y, int pointer, int button) {
//		return false;
//	}
//
//	@Override
//	public boolean zoom (float originalDistance, float currentDistance) {
//		return false;
//	}
//
//	@Override
//	public boolean pinch (Vector2 initialFirstPointer, Vector2 initialSecondPointer, Vector2 firstPointer, Vector2 secondPointer) {
//		return false;
//	}
//
//	@Override
//	public void pinchStop () {
//	}
//}
