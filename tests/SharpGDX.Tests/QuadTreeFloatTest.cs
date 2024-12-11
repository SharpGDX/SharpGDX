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
//public class QuadTreeFloatTest : GdxTest {
//	QuadTreeFloat q = new QuadTreeFloat();
//	ShapeRenderer shapes;
//	FloatArray results = new FloatArray(false, 16);
//
//	public void create () {
//		shapes = new ShapeRenderer();
//		q.setBounds(10, 10, 400, 400);
//		for (int i = 0; i < 100; i++) {
//			float x = MathUtils.random(10, 400);
//			float y = MathUtils.random(10, 400);
//			q.add(x + y * 410, x, y);
//		}
//		for (int i = 0; i < q.maxDepth; i++)
//			q.add(100 + 100 * 410, 100, 100);
//	}
//
//	public void render () {
//		float radius = 50, x = Gdx.input.getX(), y = Gdx.graphics.getHeight() - Gdx.input.getY();
//		results.clear();
//		q.query(x, y, radius, results);
//
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		shapes.begin(ShapeRenderer.ShapeType.Line);
//		draw(q);
//		shapes.setColor(Color.GREEN);
//		shapes.circle(x, y, radius, 64);
//		for (int i = 0, n = results.size; i < n; i += 4) {
//			float value = results.get(i);
//			float valueX = value % 410;
//			float valueY = value / 410;
//			shapes.circle(valueX, valueY, 10);
//		}
//		shapes.end();
//	}
//
//	void draw (QuadTreeFloat q) {
//		shapes.setColor(Color.WHITE);
//		shapes.rect(q.x, q.y, q.width, q.height);
//		if (q.values != null) {
//			shapes.setColor(Color.RED);
//			for (int i = 1, n = q.count; i < n; i += 3)
//				shapes.x(q.values[i], q.values[i + 1], 7);
//		}
//		if (q.nw != null) draw(q.nw);
//		if (q.sw != null) draw(q.sw);
//		if (q.ne != null) draw(q.ne);
//		if (q.se != null) draw(q.se);
//	}
//}
