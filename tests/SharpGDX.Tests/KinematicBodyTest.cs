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
//public class KinematicBodyTest : GdxTest {
//
//	OrthographicCamera cam;
//	World world;
//	Box2DDebugRenderer renderer;
//
//	public void create () {
//		cam = new OrthographicCamera(48, 32);
//		cam.position.set(0, 15, 0);
//		renderer = new Box2DDebugRenderer();
//
//		world = new World(new Vector2(0, -10), true);
//		Body body = world.createBody(new BodyDef());
//		CircleShape shape = new CircleShape();
//		shape.setRadius(1f);
//		MassData mass = new MassData();
//		mass.mass = 1f;
//		body.setMassData(mass);
//		body.setFixedRotation(true);
//		body.setType(BodyType.KinematicBody);
//		body.createFixture(shape, 1);
//		body.setBullet(true);
//		body.setTransform(new Vector2(0, 0), body.getAngle());
//		body.setLinearVelocity(new Vector2(50f, 0));
//	}
//
//	public void render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		world.step(Math.min(0.032f, Gdx.graphics.getDeltaTime()), 3, 4);
//		cam.update();
//		renderer.render(world, cam.combined);
//	}
//
//	public override void Dispose () {
//		world.Dispose();
//		renderer.Dispose();
//	}
//}
