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
//public class BitmapFontTest : GdxTest {
//	private Stage stage;
//	private SpriteBatch spriteBatch;
//	private BitmapFont font;
//	private ShapeRenderer renderer;
//	private BitmapFont multiPageFont;
//	private BitmapFont smallFont;
//	private GlyphLayout layout;
//	private Label label;
//
//	public override void Create () {
//		spriteBatch = new SpriteBatch();
//		// font = new BitmapFont(Gdx.files.@internal("data/verdana39.fnt"), false);
//		font = new BitmapFont(Gdx.files.@internal("data/lsans-32-pad.fnt"), false);
//		smallFont = new BitmapFont(); // uses LSans 15, the default
//		// font = new FreeTypeFontGenerator(Gdx.files.@internal("data/lsans.ttf")).generateFont(new FreeTypeFontParameter());
//		font.getData().markupEnabled = true;
//		font.getData().breakChars = new char[] {'-'};
//
//		multiPageFont = new BitmapFont(Gdx.files.@internal("data/multipagefont.fnt"));
//
//		// Add user defined color
//		Colors.put("PERU", Color.valueOf("CD853F"));
//
//		renderer = new ShapeRenderer();
//		renderer.setProjectionMatrix(spriteBatch.getProjectionMatrix());
//
//		stage = new Stage(new ScreenViewport());
//
//		Skin skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		BitmapFont labelFont = skin.get("default-font", BitmapFont.class);
//		labelFont.getData().markupEnabled = true;
//
//		// Notice that the last [] has been deliberately added to test the effect of excessive pop operations.
//		// They are silently ignored, as expected.
//		label = new Label("<<[BLUE]M[RED]u[YELLOW]l[GREEN]t[OLIVE]ic[]o[]l[]o[]r[]*[MAROON]Label[][] [Unknown Color]>>", skin);
//		label.setPosition(100, 200);
//		stage.addActor(label);
//
//		Window window = new Window("[RED]Multicolor[GREEN] Title", skin);
//		window.setPosition(400, 300);
//		window.pack();
//		stage.addActor(window);
//
//		layout = new GlyphLayout();
//	}
//
//	public override void Render () {
//		// red.a = (red.a + Gdx.graphics.getDeltaTime() * 0.1f) % 1;
//
//		int viewHeight = Gdx.graphics.getHeight();
//
//		ScreenUtils.clear(0, 0, 0, 1);
//
//		// Test wrapping or truncation with the font directly.
//		if (true) {
//			// BitmapFont font = label.getStyle().font;
//			BitmapFont font = this.font;
//			font.getData().markupEnabled = true;
//			font.getRegion().getTexture().setFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
//
//			font.getData().setScale(2f);
//			renderer.begin(ShapeRenderer.ShapeType.Line);
//			renderer.setColor(0, 1, 0, 1);
//			float w = Gdx.input.getX() - 10;
//			// w = 855;
//			renderer.rect(10, 10, w, 500);
//			renderer.end();
//
//			spriteBatch.begin();
//			String text = "your new";
//			// text = "How quickly da[RED]ft jumping zebras vex.";
//			// text = "Another font wrap is-sue, this time with multiple whitespace characters.";
//			// text = "test with AGWlWi AGWlWi issue";
//			// text = "AA BB \nEE"; // When wrapping after BB, there should not be a blank line before EE.
//			text = "[BLUE]A[]A BB [#00f000]EE[] T [GREEN]e[]   \r\r[PINK]\n\nV[][YELLOW]a bb[] ([CYAN]5[]FFFurz)\nV[PURPLE]a[]\nVa\n[PURPLE]V[]a";
//			if (true) { // Test wrap.
//				layout.setText(font, text, 0, text.length(), font.getColor(), w, Align.center, true, null);
//			} else { // Test truncation.
//				layout.setText(font, text, 0, text.length(), font.getColor(), w, Align.center, false, "...");
//			}
//			float meowy = (500 / 2 + layout.height / 2 + 5);
//			font.draw(spriteBatch, layout, 10, 10 + meowy);
//			spriteBatch.end();
//
//			GDX.GL.glEnable(GL20.GL_BLEND);
//			GDX.GL.glBlendFunc(GL20.GL_ONE, GL20.GL_ONE);
//			renderer.begin(ShapeRenderer.ShapeType.Line);
//			float c = 0.8f;
//
//			// GlyphLayout bounds
//			if (true) {
//				renderer.setColor(c, c, c, 1);
//				renderer.rect(10 + 0.5f * (w - layout.width), 10 + meowy, layout.width, -layout.height);
//			}
//			// GlyphRun bounds
//			for (int i = 0, n = layout.runs.size; i < n; i++) {
//				if (i % 3 == 0)
//					renderer.setColor(c, 0, c, 1);
//				else if (i % 2 == 0)
//					renderer.setColor(0, c, c, 1);
//				else
//					renderer.setColor(c, c, 0, 1);
//				GlyphRun r = layout.runs.get(i);
//				renderer.rect(10 + r.x, 10 + meowy + r.y, r.width, -font.getLineHeight());
//			}
//			renderer.end();
//			font.getData().setScale(1f);
//			return;
//		}
//
//		// Test wrapping with label.
//		if (false) {
//			label.debug();
//			label.getStyle().font = font;
//			label.setStyle(label.getStyle());
//			label.setText("How quickly [RED]daft[] jumping zebras vex.");
//			label.setWrap(true);
//// label.setEllipsis(true);
//			label.setAlignment(Align.center, Align.right);
//			label.setWidth(Gdx.input.getX() - label.getX());
//			label.setHeight(label.getPrefHeight());
//		} else {
//			// Test various font features.
//			spriteBatch.begin();
//
//			String text = "Sphinx of black quartz, judge my vow.";
//			font.setColor(Color.RED);
//
//			float x = 100, y = 20;
//			float alignmentWidth;
//
//			if (false) {
//				alignmentWidth = 0;
//				font.draw(spriteBatch, text, x, viewHeight - y, alignmentWidth, Align.right, false);
//			}
//
//			if (true) {
//				alignmentWidth = 280;
//				font.draw(spriteBatch, text, x, viewHeight - y, alignmentWidth, Align.right, true);
//			}
//
//			font.draw(spriteBatch, "[", 50, 60, 100, Align.left, true);
//			font.getData().markupEnabled = true;
//			font.draw(spriteBatch, "[", 100, 60, 100, Align.left, true);
//			font.getData().markupEnabled = false;
//
//			// 'R' and 'p' are in different pages
//			String txt2 = "this font uses " + multiPageFont.getRegions().size + " texture pages: RpRpRpRpRpNM";
//			spriteBatch.renderCalls = 0;
//
//			// regular draw function
//			multiPageFont.setColor(Color.BLUE);
//			multiPageFont.draw(spriteBatch, txt2, 10, 100);
//
//			// expert usage.. drawing with bitmap font cache
//			BitmapFontCache cache = multiPageFont.getCache();
//			cache.clear();
//			cache.setColor(Color.BLACK);
//			cache.setText(txt2, 10, 50);
//			cache.setColors(Color.PINK, 3, 6);
//			cache.setColors(Color.ORANGE, 9, 12);
//			cache.setColors(Color.GREEN, 16, txt2.length());
//			cache.draw(spriteBatch, 5, txt2.length() - 5);
//
//			cache.clear();
//			cache.setColor(Color.BLACK);
//			float textX = 10;
//			textX += cache.setText("[black] ", textX, 150).width;
//			multiPageFont.getData().markupEnabled = true;
//			textX += cache.addText("[[[PINK]pink[]] ", textX, 150).width;
//			textX += cache.addText("[PERU][[peru] ", textX, 150).width;
//			cache.setColor(Color.GREEN);
//			textX += cache.addText("green ", textX, 150).width;
//			textX += cache.addText("[#A52A2A]br[#A52A2ADF]ow[#A52A2ABF]n f[#A52A2A9F]ad[#A52A2A7F]in[#A52A2A5F]g o[#A52A2A3F]ut ",
//				textX, 150).width;
//			multiPageFont.getData().markupEnabled = false;
//
//			cache.draw(spriteBatch);
//
//			// tinting
//			cache.tint(new Color(1f, 1f, 1f, 0.3f));
//			cache.translate(0f, 40f);
//			cache.draw(spriteBatch);
//
//			cache = smallFont.getCache();
//			// String neeeds to be pretty long to trigger the crash described in #5834; fixed now
//			final String trogdor = "TROGDOR! TROGDOR! Trogdor was a man! Or maybe he was a... Dragon-Man!";
//			cache.clear();
//			cache.addText(trogdor, 24, 37, 500, Align.center, true);
//			cache.setColors(Color.FOREST, 0, trogdor.length());
//			cache.draw(spriteBatch);
//
//			spriteBatch.end();
//			// Console.WriteLine(spriteBatch.renderCalls);
//
//			renderer.begin(ShapeRenderer.ShapeType.Line);
//			renderer.setColor(Color.BLACK);
//			renderer.rect(x, viewHeight - y - 200, alignmentWidth, 200);
//			renderer.end();
//		}
//
//		stage.act(Gdx.graphics.getDeltaTime());
//		stage.draw();
//	}
//
//	public void resize (int width, int height) {
//		spriteBatch.getProjectionMatrix().setToOrtho2D(0, 0, width, height);
//		renderer.setProjectionMatrix(spriteBatch.getProjectionMatrix());
//		stage.getViewport().update(width, height, true);
//	}
//
//	public override void Dispose () {
//		spriteBatch.Dispose();
//		renderer.Dispose();
//		font.Dispose();
//
//		// Restore predefined colors
//		Colors.reset();
//	}
//}
