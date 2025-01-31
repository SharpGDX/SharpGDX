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
//public class Box2DTest : GdxTest implements InputProcessor {
//	/** the camera **/
//	private com.badlogic.gdx.graphics.OrthographicCamera camera;
//
//	/** the immediate mode renderer to output our debug drawings **/
//	private ShapeRenderer renderer;
//
//	/** box2d debug renderer **/
//	private Box2DDebugRenderer debugRenderer;
//
//	/** a spritebatch and a font for text rendering and a Texture to draw our boxes **/
//	private SpriteBatch batch;
//	private BitmapFont font;
//	private TextureRegion textureRegion;
//
//	/** our box2D world **/
//	private World world;
//
//	/** our boxes **/
//	private ArrayList<Body> boxes = new ArrayList<Body>();
//
//	/** our ground box **/
//	Body groundBody;
//
//	/** our mouse joint **/
//	private MouseJoint mouseJoint = null;
//
//	/** a hit body **/
//	Body hitBody = null;
//
//	public override void Create () {
//		// setup the camera. In Box2D we operate on a
//		// meter scale, pixels won't do it. So we use
//		// an orthographic camera with a viewport of
//		// 48 meters in width and 32 meters in height.
//		// We also position the camera so that it
//		// looks at (0,16) (that's where the middle of the
//		// screen will be located).
//		camera = new OrthographicCamera(48, 32);
//		camera.position.set(0, 16, 0);
//
//		// next we setup the immediate mode renderer
//		renderer = new ShapeRenderer();
//
//		// next we create the box2d debug renderer
//		debugRenderer = new Box2DDebugRenderer();
//
//		// next we create a SpriteBatch and a font
//		batch = new SpriteBatch();
//		font = new BitmapFont(Gdx.files.@internal("data/lsans-15.fnt"), false);
//		font.setColor(Color.RED);
//		textureRegion = new TextureRegion(new Texture(Gdx.files.@internal("data/badlogicsmall.jpg")));
//
//		// next we create out physics world.
//		createPhysicsWorld();
//
//		// register ourselfs as an InputProcessor
//		Gdx.input.setInputProcessor(this);
//	}
//
//	private void createPhysicsWorld () {
//		// we instantiate a new World with a proper gravity vector
//		// and tell it to sleep when possible.
//		world = new World(new Vector2(0, -10), true);
//
//		float[] vertices = {-0.07421887f, -0.16276085f, -0.12109375f, -0.22786504f, -0.157552f, -0.7122401f, 0.04296875f,
//			-0.7122401f, 0.110677004f, -0.6419276f, 0.13151026f, -0.49869835f, 0.08984375f, -0.3190109f};
//
//		PolygonShape shape = new PolygonShape();
//		shape.set(vertices);
//
//		// next we create a static ground platform. This platform
//		// is not moveable and will not react to any influences from
//		// outside. It will however influence other bodies. First we
//		// create a PolygonShape that holds the form of the platform.
//		// it will be 100 meters wide and 2 meters high, centered
//		// around the origin
//		PolygonShape groundPoly = new PolygonShape();
//		groundPoly.setAsBox(50, 1);
//
//		// next we create the body for the ground platform. It's
//		// simply a static body.
//		BodyDef groundBodyDef = new BodyDef();
//		groundBodyDef.type = BodyType.StaticBody;
//		groundBody = world.createBody(groundBodyDef);
//
//		// finally we add a fixture to the body using the polygon
//		// defined above. Note that we have to dispose PolygonShapes
//		// and CircleShapes once they are no longer used. This is the
//		// only time you have to care explicitly for memory management.
//		FixtureDef fixtureDef = new FixtureDef();
//		fixtureDef.shape = groundPoly;
//		fixtureDef.filter.groupIndex = 0;
//		groundBody.createFixture(fixtureDef);
//		groundPoly.Dispose();
//
//		// We also create a simple ChainShape we put above our
//		// ground polygon for extra funkyness.
//		ChainShape chainShape = new ChainShape();
//		chainShape.createLoop(new Vector2[] {new Vector2(-10, 10), new Vector2(-10, 5), new Vector2(10, 5), new Vector2(10, 11),});
//		BodyDef chainBodyDef = new BodyDef();
//		chainBodyDef.type = BodyType.StaticBody;
//		Body chainBody = world.createBody(chainBodyDef);
//		chainBody.createFixture(chainShape, 0);
//		chainShape.Dispose();
//
//		createBoxes();
//
//		Array<Fixture> fixtures = new Array<Fixture>();
//		world.getFixtures(fixtures);
//
//		// You can savely ignore the rest of this method :)
//		world.setContactListener(new ContactListener() {
//			@Override
//			public void beginContact (Contact contact) {
//// Console.WriteLine("begin contact");
//			}
//
//			@Override
//			public void endContact (Contact contact) {
//// Console.WriteLine("end contact");
//			}
//
//			@Override
//			public void preSolve (Contact contact, Manifold oldManifold) {
//// Manifold.ManifoldType type = oldManifold.getType();
//// Vector2 localPoint = oldManifold.getLocalPoint();
//// Vector2 localNormal = oldManifold.getLocalNormal();
//// int pointCount = oldManifold.getPointCount();
//// ManifoldPoint[] points = oldManifold.getPoints();
//// Console.WriteLine("pre solve, " + type +
//// ", point: " + localPoint +
//// ", local normal: " + localNormal +
//// ", #points: " + pointCount +
//// ", [" + points[0] + ", " + points[1] + "]");
//			}
//
//			@Override
//			public void postSolve (Contact contact, ContactImpulse impulse) {
//// float[] ni = impulse.getNormalImpulses();
//// float[] ti = impulse.getTangentImpulses();
//// Console.WriteLine("post solve, normal impulses: " + ni[0] + ", " + ni[1] + ", tangent impulses: " + ti[0] + ", " + ti[1]);
//			}
//		});
//	}
//
//	private void createBoxes () {
//		// next we create 50 boxes at random locations above the ground
//		// body. First we create a nice polygon representing a box 2 meters
//		// wide and high.
//		PolygonShape boxPoly = new PolygonShape();
//		boxPoly.setAsBox(1, 1);
//
//		// next we create the 50 box bodies using the PolygonShape we just
//		// defined. This process is similar to the one we used for the ground
//		// body. Note that we reuse the polygon for each body fixture.
//		for (int i = 0; i < 20; i++) {
//			// Create the BodyDef, set a random position above the
//			// ground and create a new body
//			BodyDef boxBodyDef = new BodyDef();
//			boxBodyDef.type = BodyType.DynamicBody;
//			boxBodyDef.position.x = -24 + (float)(Math.random() * 48);
//			boxBodyDef.position.y = 10 + (float)(Math.random() * 100);
//			Body boxBody = world.createBody(boxBodyDef);
//
//			boxBody.createFixture(boxPoly, 1);
//
//			// add the box to our list of boxes
//			boxes.add(boxBody);
//		}
//
//		// we are done, all that's left is disposing the boxPoly
//		boxPoly.Dispose();
//	}
//
//	public override void Render () {
//		// first we update the world. For simplicity
//		// we use the delta time provided by the Graphics
//		// instance. Normally you'll want to fix the time
//		// step.
//		long start = TimeUtils.nanoTime();
//		world.step(Gdx.graphics.getDeltaTime(), 8, 3);
//		float updateTime = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		// next we clear the color buffer and set the camera
//		// matrices
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		camera.update();
//
//		// next we render the ground body
//		renderBox(groundBody, 50, 1);
//
//		// next we render each box via the SpriteBatch.
//		// for this we have to set the projection matrix of the
//		// spritebatch to the camera's combined matrix. This will
//		// make the spritebatch work in world coordinates
//		batch.getProjectionMatrix().set(camera.combined);
//		batch.begin();
//		for (int i = 0; i < boxes.size(); i++) {
//			Body box = boxes.get(i);
//			Vector2 position = box.getPosition(); // that's the box's center position
//			float angle = MathUtils.radiansToDegrees * box.getAngle(); // the rotation angle around the center
//			batch.draw(textureRegion, position.x - 1, position.y - 1, // the bottom left corner of the box, unrotated
//				1f, 1f, // the rotation center relative to the bottom left corner of the box
//				2, 2, // the width and height of the box
//				1, 1, // the scale on the x- and y-axis
//				angle); // the rotation angle
//		}
//		batch.end();
//
//		// next we use the debug renderer. Note that we
//		// simply apply the camera again and then call
//		// the renderer. the camera.apply() call is actually
//		// not needed as the opengl matrices are already set
//		// by the spritebatch which in turn uses the camera matrices :)
//		debugRenderer.render(world, camera.combined);
//
//		// finally we render all contact points
//		renderer.setProjectionMatrix(camera.combined);
//		renderer.begin(ShapeRenderer.ShapeType.Point);
//		renderer.setColor(0, 1, 0, 1);
//		for (int i = 0; i < world.getContactCount(); i++) {
//			Contact contact = world.getContactList().get(i);
//			// we only render the contact if it actually touches
//			if (contact.isTouching()) {
//				// get the world manifold from which we get the
//				// contact points. A manifold can have 0, 1 or 2
//				// contact points.
//				WorldManifold manifold = contact.getWorldManifold();
//				int numContactPoints = manifold.getNumberOfContactPoints();
//				for (int j = 0; j < numContactPoints; j++) {
//					Vector2 point = manifold.getPoints()[j];
//					renderer.point(point.x, point.y, 0);
//				}
//			}
//		}
//		renderer.end();
//
//		// finally we render the time it took to update the world
//		// for this we have to set the projection matrix again, so
//		// we work in pixel coordinates
//		batch.getProjectionMatrix().setToOrtho2D(0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		batch.begin();
//		font.draw(batch, "fps: " + Gdx.graphics.getFramesPerSecond() + " update time: " + updateTime, 0, 20);
//		batch.end();
//	}
//
//	Matrix4 transform = new Matrix4();
//
//	private void renderBox (Body body, float halfWidth, float halfHeight) {
//		// get the bodies center and angle in world coordinates
//		Vector2 pos = body.getWorldCenter();
//		float angle = body.getAngle();
//
//		// set the translation and rotation matrix
//		transform.setToTranslation(pos.x, pos.y, 0);
//		transform.rotate(0, 0, 1, (float)Math.toDegrees(angle));
//
//		// render the box
//		renderer.begin(ShapeRenderer.ShapeType.Line);
//		renderer.setTransformMatrix(transform);
//		renderer.setColor(1, 1, 1, 1);
//		renderer.rect(-halfWidth, -halfHeight, halfWidth * 2, halfHeight * 2);
//		renderer.end();
//	}
//
//	/** we instantiate this vector and the callback here so we don't irritate the GC **/
//	Vector3 testPoint = new Vector3();
//	QueryCallback callback = new QueryCallback() {
//		@Override
//		public boolean reportFixture (Fixture fixture) {
//			// if the hit fixture's body is the ground body
//			// we ignore it
//			if (fixture.getBody() == groundBody) return true;
//
//			// if the hit point is inside the fixture of the body
//			// we report it
//			if (fixture.testPoint(testPoint.x, testPoint.y)) {
//				hitBody = fixture.getBody();
//				return false;
//			} else
//				return true;
//		}
//	};
//
//	public override bool TouchDown (int x, int y, int pointer, int newParam) {
//		// translate the mouse coordinates to world coordinates
//		testPoint.set(x, y, 0);
//		camera.unproject(testPoint);
//
//		// ask the world which bodies are within the given
//		// bounding box around the mouse pointer
//		hitBody = null;
//		world.QueryAABB(callback, testPoint.x - 0.1f, testPoint.y - 0.1f, testPoint.x + 0.1f, testPoint.y + 0.1f);
//
//		// if we hit something we create a new mouse joint
//		// and attach it to the hit body.
//		if (hitBody != null) {
//			MouseJointDef def = new MouseJointDef();
//			def.bodyA = groundBody;
//			def.bodyB = hitBody;
//			def.collideConnected = true;
//			def.target.set(testPoint.x, testPoint.y);
//			def.maxForce = 1000.0f * hitBody.getMass();
//
//			mouseJoint = (MouseJoint)world.createJoint(def);
//			hitBody.setAwake(true);
//		} else {
//			for (Body box : boxes)
//				world.destroyBody(box);
//			boxes.clear();
//			createBoxes();
//		}
//
//		return false;
//	}
//
//	/** another temporary vector **/
//	Vector2 target = new Vector2();
//
//	public override bool TouchDragged (int x, int y, int pointer) {
//		// if a mouse joint exists we simply update
//		// the target of the joint based on the new
//		// mouse coordinates
//		if (mouseJoint != null) {
//			camera.unproject(testPoint.set(x, y, 0));
//			mouseJoint.setTarget(target.set(testPoint.x, testPoint.y));
//		}
//		return false;
//	}
//
//	public override bool TouchUp (int x, int y, int pointer, int button) {
//		// if a mouse joint exists we simply destroy it
//		if (mouseJoint != null) {
//			world.destroyJoint(mouseJoint);
//			mouseJoint = null;
//		}
//		return false;
//	}
//
//	public override void Dispose () {
//		world.Dispose();
//		renderer.Dispose();
//		debugRenderer.Dispose();
//		font.Dispose();
//		textureRegion.getTexture().Dispose();
//	}
//}
