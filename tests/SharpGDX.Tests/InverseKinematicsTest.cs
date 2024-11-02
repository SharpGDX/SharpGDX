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
//public class InverseKinematicsTest : GdxTest {
//
//	static class Bone {
//		final float len;
//		final Vector3 position = new Vector3();
//		final Vector3 inertia = new Vector3();
//
//		public String name;
//
//		public Bone (String name, float x, float y, float len) {
//			this.name = name;
//			this.position.set(x, y, 0);
//			this.len = len;
//		}
//
//		public String toString () {
//			return "bone " + name + ": " + position + ", " + len;
//		}
//	}
//
//	static final float GRAVITY = 0;
//	OrthographicCamera camera;
//	ShapeRenderer renderer;
//	Bone[] bones;
//	Vector3 globalCoords = new Vector3();
//	Vector3 endPoint = new Vector3();
//	Vector2 diff = new Vector2();
//
//	public override void Create () {
//		float aspect = Gdx.graphics.getWidth() / (float)Gdx.graphics.getHeight();
//		camera = new OrthographicCamera(15 * aspect, 15);
//		camera.update();
//		renderer = new ShapeRenderer();
//		renderer.setProjectionMatrix(camera.combined);
//
//		bones = new Bone[] {new Bone("bone0", 0, 0, 0), new Bone("bone1", 0, 2, 2), new Bone("bone2", 0, 4, 2),
//			new Bone("bone3", 0, 6, 2), new Bone("end", 0, 8, 2)};
//		globalCoords.set(bones[0].position);
//	}
//
//	public override void Dispose () {
//		renderer.Dispose();
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		camera.update();
//		renderer.setProjectionMatrix(camera.combined);
//
//		if (Gdx.input.isTouched()) camera.unproject(globalCoords.set(Gdx.input.getX(), Gdx.input.getY(), 0));
//		solveFakeIK(globalCoords);
//		renderBones();
//	}
//
//	private void renderBones () {
//		renderer.begin(ShapeRenderer.ShapeType.Line);
//		renderer.setColor(0, 1, 0, 1);
//		for (int i = 0; i < bones.length - 1; i++) {
//			renderer.line(bones[i].position.x, bones[i].position.y, bones[i + 1].position.x, bones[i + 1].position.y);
//		}
//		renderer.end();
//
//		renderer.begin(ShapeRenderer.ShapeType.Point);
//		renderer.setColor(1, 0, 0, 1);
//		for (int i = 0; i < bones.length; i++) {
//			renderer.point(bones[i].position.x, bones[i].position.y, 0);
//		}
//		renderer.end();
//	}
//
//	public void solveFakeIK (Vector3 target) {
//		float gravity = Gdx.graphics.getDeltaTime() * GRAVITY;
//
//		endPoint.set(target);
//		bones[0].position.set(endPoint);
//
//		for (int i = 0; i < bones.length - 1; i++) {
//			Bone bone = bones[i];
//			endPoint.set(bone.position);
//
//			diff.set(endPoint.x, endPoint.y).sub(bones[i + 1].position.x, bones[i + 1].position.y);
//			diff.add(0, gravity);
//			diff.add(bones[i + 1].inertia.x, bones[i + 1].inertia.y);
//			diff.nor().scl(bones[i + 1].len);
//
//			float x = endPoint.x - diff.x;
//			float y = endPoint.y - diff.y;
//			float delta = Gdx.graphics.getDeltaTime();
//			bones[i + 1].inertia.add((bones[i + 1].position.x - x) * delta, (bones[i + 1].position.y - y) * delta, 0).scl(0.99f);
//			bones[i + 1].position.set(x, y, 0);
//		}
//	}
//}
