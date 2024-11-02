//using SharpGDX.Tests.Utils;
//using SharpGDX.Utils;
//using SharpGDX.Scenes.Scene2D;
//using SharpGDX.Scenes.Scene2D.Utils;
//using SharpGDX.Scenes.Scene2D.UI;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Utils.Viewports;
//using SharpGDX.Shims;
//using System.Text;
//using SharpGDX.Mathematics;
//using SharpGDX.Graphics.GLUtils;
//
//namespace SharpGDX.Tests;
//
//@GdxTestConfig
//public class SpriteBatchPerformanceTest : GdxTest {
//
//	private Texture texture;
//	private SpriteBatch spriteBatch;
//	private WindowedMean counter = new WindowedMean(10000);
//	private StringBuilder stringBuilder = new StringBuilder();
//
//	private BitmapFont bitmapFont;
//
//	public override void Create () {
//		texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"));
//		spriteBatch = new SpriteBatch(8191);
//		bitmapFont = new BitmapFont();
//	}
//
//	public override void Render () {
//		Gdx.gl20.glViewport(0, 0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getBackBufferHeight());
//		Gdx.gl20.glClearColor(0.2f, 0.2f, 0.2f, 1);
//		Gdx.gl20.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		spriteBatch.begin();
//
//		// Accelerate the draws
//		for (int j = 0; j < 100; j++) {
//
//			// fill the batch
//			for (int i = 0; i < 8190; i++) {
//				spriteBatch.draw(texture, 0, 0, 1, 1);
//			}
//
//			long beforeFlush = TimeUtils.nanoTime();
//
//			spriteBatch.flush();
//			Gdx.gl.glFlush();
//			long afterFlush = TimeUtils.nanoTime();
//
//			counter.addValue(afterFlush - beforeFlush);
//
//		}
//
//		spriteBatch.end();
//
//		spriteBatch.begin();
//		stringBuilder.SetLength(0);
//
//		if (counter.hasEnoughData()) {
//			stringBuilder.Append("Mean Time ms: ");
//			stringBuilder.Append(counter.getMean() / 1e6);
//		} else {
//			stringBuilder.Append("Please wait, collecting data...");
//		}
//
//		bitmapFont.draw(spriteBatch, stringBuilder, 0, 200);
//		spriteBatch.end();
//	}
//
//	public override void Dispose () {
//		texture.Dispose();
//		spriteBatch.Dispose();
//	}
//
//}
