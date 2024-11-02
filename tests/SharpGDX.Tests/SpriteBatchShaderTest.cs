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
//public class SpriteBatchShaderTest : GdxTest {
//	int SPRITES = 400;
//
//	long startTime = TimeUtils.nanoTime();
//	int frames = 0;
//
//	Texture texture;
//	Texture texture2;
//// Font font;
//	SpriteBatch spriteBatch;
//	int[] coords = new int[SPRITES * 2];
//	int[] coords2 = new int[SPRITES * 2];
//
//	Color col = new Color(1, 1, 1, 0.6f);
//
//	Mesh mesh;
//	float[] vertices = new float[SPRITES * 6 * (2 + 2 + 4)];
//
//	public override void Render () {
//		GL20 gl = Gdx.gl20;
//		gl.glClearColor(0.7f, 0.7f, 0.7f, 1);
//		gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		float begin = 0;
//		float end = 0;
//		float draw1 = 0;
//		float draw2 = 0;
//		float drawText = 0;
//
//		long start = TimeUtils.nanoTime();
//		spriteBatch.begin();
//		begin = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		int len = coords.Length;
//		start = TimeUtils.nanoTime();
//		for (int i = 0; i < len; i += 2)
//			spriteBatch.draw(texture, coords[i], coords[i + 1], 0, 0, 32, 32);
//		draw1 = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//		spriteBatch.setColor(col);
//		for (int i = 0; i < coords2.Length; i += 2)
//			spriteBatch.draw(texture2, coords2[i], coords2[i + 1], 0, 0, 32, 32);
//		draw2 = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//// spriteBatch.drawText(font, "Question?", 100, 300, Color.RED);
//// spriteBatch.drawText(font, "and another this is a test", 200, 100, Color.WHITE);
//// spriteBatch.drawText(font, "all hail and another this is a test", 200, 200, Color.WHITE);
//		drawText = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//		spriteBatch.end();
//		end = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		if (TimeUtils.nanoTime() - startTime > 1000000000) {
//			Gdx.app.log("SpriteBatch", "fps: " + frames + ", render calls: " + spriteBatch.renderCalls + ", " + begin + ", " + draw1
//				+ ", " + draw2 + ", " + drawText + ", " + end);
//			frames = 0;
//			startTime = TimeUtils.nanoTime();
//		}
//		frames++;
//	}
//
//	public override void Create () {
//		spriteBatch = new SpriteBatch();
//		texture = new Texture(Gdx.files.@internal("data/badlogicsmall.jpg"));
//
//		Pixmap pixmap = new Pixmap(32, 32, Pixmap.Format.RGB565);
//		pixmap.setColor(1, 1, 0, 0.7f);
//		pixmap.fill();
//		texture2 = new Texture(pixmap);
//		pixmap.Dispose();
//
//		for (int i = 0; i < coords.Length; i += 2) {
//			coords[i] = (int)(Math.random() * Gdx.graphics.getWidth());
//			coords[i + 1] = (int)(Math.random() * Gdx.graphics.getHeight());
//			coords2[i] = (int)(Math.random() * Gdx.graphics.getWidth());
//			coords2[i + 1] = (int)(Math.random() * Gdx.graphics.getHeight());
//		}
//	}
//
//	public override void Dispose () {
//		spriteBatch.Dispose();
//		texture.Dispose();
//		texture2.Dispose();
//	}
//}
