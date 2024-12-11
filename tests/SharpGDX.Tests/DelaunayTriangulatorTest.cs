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
///** @author Nathan Sweet */
//public class DelaunayTriangulatorTest : GdxTest {
//	private ShapeRenderer renderer;
//	FloatArray points = new FloatArray();
//	ShortArray triangles;
//	DelaunayTriangulator trianglulator = new DelaunayTriangulator();
//	long seed = MathUtils.random.nextLong();
//
//	public void create () {
//		renderer = new ShapeRenderer();
//
//		triangulate();
//		Console.WriteLine(seed);
//
//		Gdx.input.setInputProcessor(new InputAdapter() {
//			public boolean touchDown (int screenX, int screenY, int pointer, int button) {
//				seed = MathUtils.random.nextLong();
//				Console.WriteLine(seed);
//				triangulate();
//				return true;
//			}
//
//			public boolean mouseMoved (int screenX, int screenY) {
//				triangulate();
//				return false;
//			}
//		});
//	}
//
//	void triangulate () {
//		// seed = 4139368480425561099l;
//		// seed = 6559652580366669361l;
//		MathUtils.random.setSeed(seed);
//
//		int pointCount = 100;
//		points.clear();
//		for (int i = 0; i < pointCount; i++) {
//			float value;
//			do {
//				value = MathUtils.random(10, 400);
//			} while (points.contains(value));
//			points.add(value);
//			do {
//				value = MathUtils.random(10, 400);
//			} while (points.contains(value));
//			points.add(value);
//		}
//		points.add(Gdx.input.getX());
//		points.add(Gdx.graphics.getHeight() - Gdx.input.getY());
//
//		triangles = trianglulator.computeTriangles(points, false);
//	}
//
//	public void render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		renderer.setColor(Color.RED);
//		renderer.begin(ShapeRenderer.ShapeType.Filled);
//		for (int i = 0; i < points.size; i += 2)
//			renderer.circle(points.get(i), points.get(i + 1), 4, 12);
//		renderer.end();
//
//		renderer.setColor(Color.WHITE);
//		renderer.begin(ShapeRenderer.ShapeType.Line);
//		for (int i = 0; i < triangles.size; i += 3) {
//			int p1 = triangles.get(i) * 2;
//			int p2 = triangles.get(i + 1) * 2;
//			int p3 = triangles.get(i + 2) * 2;
//			renderer.triangle( //
//				points.get(p1), points.get(p1 + 1), //
//				points.get(p2), points.get(p2 + 1), //
//				points.get(p3), points.get(p3 + 1));
//		}
//		renderer.end();
//	}
//
//	public void resize (int width, int height) {
//		renderer.getProjectionMatrix().setToOrtho2D(0, 0, width, height);
//		renderer.updateMatrices();
//	}
//}
