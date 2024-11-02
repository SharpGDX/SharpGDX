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
//public class IntegerBitmapFontTest : GdxTest {
//
//	BitmapFont font;
//	BitmapFontCache singleLineCacheNonInteger;
//	BitmapFontCache multiLineCacheNonInteger;
//	BitmapFontCache singleLineCache;
//	BitmapFontCache multiLineCache;
//	SpriteBatch batch;
//
//	public void create () {
//		TextureAtlas textureAtlas = new TextureAtlas("data/pack.atlas");
//		font = new BitmapFont(Gdx.files.@internal("data/verdana39.fnt"), textureAtlas.findRegion("verdana39"), false);
//		singleLineCache = new BitmapFontCache(font, true);
//		multiLineCache = new BitmapFontCache(font, true);
//		singleLineCacheNonInteger = new BitmapFontCache(font, false);
//		multiLineCacheNonInteger = new BitmapFontCache(font, false);
//		batch = new SpriteBatch();
//		fillCaches();
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		font.Dispose();
//	}
//
//	public void render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		font.setUseIntegerPositions(false);
//		font.setColor(1, 0, 0, 1);
//		singleLineCacheNonInteger.draw(batch);
//		multiLineCacheNonInteger.draw(batch);
//		drawTexts();
//		font.setUseIntegerPositions(true);
//		font.setColor(1, 1, 1, 1);
//		singleLineCache.draw(batch);
//		multiLineCache.draw(batch);
//		drawTexts();
//		batch.end();
//	}
//
//	private void fillCaches () {
//		String text = "This is a TEST\nxahsdhwekjhasd23���$%$%/%&";
//		singleLineCache.setColor(0, 0, 1, 1);
//		singleLineCache.setText(text, 10.2f, 30.5f);
//		multiLineCache.setColor(0, 0, 1, 1);
//		multiLineCache.setText(text, 10.5f, 180.5f, 200, Align.center, false);
//		singleLineCacheNonInteger.setColor(0, 1, 0, 1);
//		singleLineCacheNonInteger.setText(text, 10.2f, 30.5f);
//		multiLineCacheNonInteger.setColor(0, 1, 0, 1);
//		multiLineCacheNonInteger.setText(text, 10.5f, 180.5f, 200, Align.center, false);
//	}
//
//	private void drawTexts () {
//		String text = "This is a TEST\nxahsdhwekjhasd23���$%$%/%&";
//		font.draw(batch, text, 10.2f, 30.5f);
//		font.draw(batch, text, 10.5f, 120.5f);
//		font.draw(batch, text, 10.5f, 180.5f, 200, Align.center, false);
//	}
//}
