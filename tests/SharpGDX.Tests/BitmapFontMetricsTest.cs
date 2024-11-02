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
//public class BitmapFontMetricsTest : GdxTest {
//	private SpriteBatch spriteBatch;
//	private TextureAtlas atlas;
//	private BitmapFont font, smallFont;
//	private ShapeRenderer renderer;
//
//	public override void Create () {
//		spriteBatch = new SpriteBatch();
//		atlas = new TextureAtlas("data/pack.atlas");
//		smallFont = new BitmapFont();
//		font = new BitmapFont(Gdx.files.@internal("data/verdana39.fnt"), atlas.findRegion("verdana39"), false);
//		font = new BitmapFont(Gdx.files.@internal("data/lsans-32-pad.fnt"), false);
//		renderer = new ShapeRenderer();
//		renderer.setProjectionMatrix(spriteBatch.getProjectionMatrix());
//	}
//
//	public override void Render () {
//		// red.a = (red.a + Gdx.graphics.getDeltaTime() * 0.1f) % 1;
//
//		int viewHeight = Gdx.graphics.getHeight();
//
//		ScreenUtils.clear(1, 1, 1, 1);
//		spriteBatch.begin();
//
//		// String text = "Sphinx of black quartz, judge my vow.";
//		String text = "Sphinx of black quartz.";
//		font.setColor(Color.RED);
//
//		float x = 100, y = 100;
//		float alignmentWidth;
//
//		smallFont.setColor(Color.BLACK);
//		smallFont.draw(spriteBatch, "draw position", 20, viewHeight - 0);
//		smallFont.setColor(Color.BLUE);
//		smallFont.draw(spriteBatch, "bounds", 20, viewHeight - 20);
//		smallFont.setColor(Color.MAGENTA);
//		smallFont.draw(spriteBatch, "baseline", 20, viewHeight - 40);
//		smallFont.setColor(Color.GREEN);
//		smallFont.draw(spriteBatch, "x height", 20, viewHeight - 60);
//		smallFont.setColor(Color.CYAN);
//		smallFont.draw(spriteBatch, "ascent", 20, viewHeight - 80);
//		smallFont.setColor(Color.RED);
//		smallFont.draw(spriteBatch, "descent", 20, viewHeight - 100);
//		smallFont.setColor(Color.ORANGE);
//		smallFont.draw(spriteBatch, "line height", 20, viewHeight - 120);
//		smallFont.setColor(Color.LIGHT_GRAY);
//		smallFont.draw(spriteBatch, "cap height", 20, viewHeight - 140);
//
//		font.setColor(Color.BLACK);
//		GlyphLayout layout = font.draw(spriteBatch, text, x, y);
//
//		spriteBatch.end();
//
//		renderer.begin(ShapeRenderer.ShapeType.Filled);
//		renderer.setColor(Color.BLACK);
//		renderer.rect(x - 3, y - 3, 6, 6);
//		renderer.end();
//
//		float baseline = y - font.getCapHeight();
//		renderer.begin(ShapeRenderer.ShapeType.Line);
//		renderer.setColor(Color.LIGHT_GRAY);
//		renderer.line(0, y, 9999, y);
//		renderer.setColor(Color.MAGENTA);
//		renderer.line(0, baseline, 9999, baseline);
//		renderer.setColor(Color.GREEN);
//		renderer.line(0, baseline + font.getXHeight(), 9999, baseline + font.getXHeight());
//		renderer.setColor(Color.CYAN);
//		renderer.line(0, y + font.getAscent(), 9999, y + font.getAscent());
//		renderer.setColor(Color.RED);
//		renderer.line(0, baseline + font.getDescent(), 9999, baseline + font.getDescent());
//		renderer.setColor(Color.ORANGE);
//		renderer.line(0, y - font.getLineHeight(), 9999, y - font.getLineHeight());
//		renderer.end();
//
//		renderer.begin(ShapeRenderer.ShapeType.Line);
//		renderer.setColor(Color.BLUE);
//		renderer.rect(x, y, layout.width, -layout.height);
//		renderer.end();
//	}
//
//	public override void Dispose () {
//		spriteBatch.Dispose();
//		renderer.Dispose();
//		font.Dispose();
//		atlas.Dispose();
//	}
//}
