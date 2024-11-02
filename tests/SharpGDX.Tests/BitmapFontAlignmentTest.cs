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
///** Shows how to align single line, wrapped, and multi line text within a rectangle. */
//public class BitmapFontAlignmentTest : GdxTest {
//	private SpriteBatch spriteBatch;
//	private Texture texture;
//	private BitmapFont font;
//	private BitmapFontCache cache;
//	private Sprite logoSprite;
//	int renderMode;
//	GlyphLayout layout;
//
//	public override void Create () {
//		Gdx.input.setInputProcessor(new InputAdapter() {
//			public boolean touchDown (int x, int y, int pointer, int newParam) {
//				renderMode = (renderMode + 1) % 6;
//				return false;
//			}
//		});
//
//		spriteBatch = new SpriteBatch();
//		texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"));
//		logoSprite = new Sprite(texture);
//		logoSprite.setColor(1, 1, 1, 0.6f);
//		logoSprite.setBounds(50, 100, 400, 100);
//
//		font = new BitmapFont(Gdx.files.getFileHandle("data/verdana39.fnt", FileType.Internal),
//			Gdx.files.getFileHandle("data/verdana39.png", FileType.Internal), false);
//		cache = font.newFontCache();
//		layout = new GlyphLayout();
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.7f, 0, 0, 1);
//		spriteBatch.begin();
//		logoSprite.draw(spriteBatch);
//		switch (renderMode) {
//		case 0:
//			renderSingleLine();
//			break;
//		case 1:
//			renderSingleLineCached();
//			break;
//		case 2:
//			renderWrapped();
//			break;
//		case 3:
//			renderWrappedCached();
//			break;
//		case 4:
//			renderMultiLine();
//			break;
//		case 5:
//			renderMultiLineCached();
//			break;
//		}
//		spriteBatch.end();
//	}
//
//	private void renderSingleLine () {
//		String text = "Single Line";
//		float x = logoSprite.getX();
//		float y = logoSprite.getY();
//		float width = logoSprite.getWidth();
//		float height = logoSprite.getHeight();
//
//		layout.setText(font, text);
//		x += width / 2 - layout.width / 2;
//		y += height / 2 + layout.height / 2;
//
//		font.draw(spriteBatch, text, x, y);
//	}
//
//	private void renderSingleLineCached () {
//		String text = "Single Line Cached";
//		float x = logoSprite.getX();
//		float y = logoSprite.getY();
//		float width = logoSprite.getWidth();
//		float height = logoSprite.getHeight();
//
//		// Obviously you wouldn't set the cache text every frame in real code.
//		GlyphLayout layout = cache.setText(text, 0, 0);
//		cache.setColors(Color.BLUE, 1, 4);
//
//		x += width / 2 - layout.width / 2;
//		y += height / 2 + layout.height / 2;
//		cache.setPosition(x, y);
//
//		cache.draw(spriteBatch);
//	}
//
//	private void renderWrapped () {
//		String text = "Wrapped Wrapped Wrapped Wrapped";
//		float x = logoSprite.getX();
//		float y = logoSprite.getY();
//		float width = logoSprite.getWidth();
//		float height = logoSprite.getHeight();
//
//		layout.setText(font, text, Color.WHITE, width, Align.left, true);
//		x += width / 2 - layout.width / 2;
//		y += height / 2 + layout.height / 2;
//
//		font.draw(spriteBatch, text, x, y, width, Align.left, true);
//
//		// More efficient to draw the layout used for bounds:
//		// font.draw(spriteBatch, layout, x, y);
//
//		// Note that wrapped text can be aligned:
//		// font.draw(spriteBatch, text, x, y, width, Align.center, true);
//	}
//
//	private void renderWrappedCached () {
//		String text = "Wrapped Cached Wrapped Cached";
//		float x = logoSprite.getX();
//		float y = logoSprite.getY();
//		float width = logoSprite.getWidth();
//		float height = logoSprite.getHeight();
//
//		// Obviously you wouldn't set the cache text every frame in real code.
//		GlyphLayout layout = cache.setText(text, 0, 0, width, Align.left, true);
//
//		// Note that wrapped text can be aligned:
//		// cache.setWrappedText(text, 0, 0, width, HAlignment.CENTER);
//
//		x += width / 2 - layout.width / 2;
//		y += height / 2 + layout.height / 2;
//		cache.setPosition(x, y);
//
//		cache.draw(spriteBatch);
//	}
//
//	private void renderMultiLine () {
//		String text = "Multi\nLine";
//		float x = logoSprite.getX();
//		float y = logoSprite.getY();
//		float width = logoSprite.getWidth();
//		float height = logoSprite.getHeight();
//
//		layout.setText(font, text);
//		x += width / 2 - layout.width / 2;
//		y += height / 2 + layout.height / 2;
//
//		font.draw(spriteBatch, text, x, y);
//
//		// Note that multi line text can be aligned:
//		// font.draw(spriteBatch, text, x, y, width, Align.center, false);
//	}
//
//	private void renderMultiLineCached () {
//		String text = "Multi Line\nCached";
//		int lines = 2;
//		float x = logoSprite.getX();
//		float y = logoSprite.getY();
//		float width = logoSprite.getWidth();
//		float height = logoSprite.getHeight();
//
//		// Obviously you wouldn't set the cache text every frame in real code.
//		GlyphLayout layout = cache.setText(text, 0, 0);
//
//		// Note that multi line text can be aligned:
//		// cache.setText(text, 0, 0, width, Align.center, false);
//
//		x += width / 2 - layout.width / 2;
//		y += height / 2 + layout.height / 2;
//		cache.setPosition(x, y);
//
//		cache.draw(spriteBatch);
//	}
//
//	public override void Dispose () {
//		spriteBatch.Dispose();
//		font.Dispose();
//		texture.Dispose();
//	}
//}
