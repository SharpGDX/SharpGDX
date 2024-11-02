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
//public class BitmapFontAtlasRegionTest : GdxTest {
//	private SpriteBatch batch;
//	private AssetManager assets;
//
//	private BitmapFont[] fonts;
//	private String[] testStrings;
//
//	private static final String FONT_1 = "data/default.fnt";
//	private static final String FONT_2 = "data/font.fnt";
//	private static final String FONT_3 = "data/verdana39.fnt";
//	private static final String ATLAS = "data/atlased-fonts.txt";
//
//	public override void Create () {
//		this.batch = new SpriteBatch();
//		this.assets = new AssetManager();
//
//		BitmapFontParameter params = new BitmapFontParameter();
//		params.atlasName = ATLAS;
//
//		this.assets.load(FONT_1, BitmapFont.class, params);
//		this.assets.load(FONT_2, BitmapFont.class, params);
//		this.assets.load(FONT_3, BitmapFont.class, params);
//		this.assets.finishLoading();
//
//		this.fonts = new BitmapFont[3];
//		this.fonts[0] = assets.get(FONT_1);
//		this.fonts[1] = assets.get(FONT_2);
//		this.fonts[2] = assets.get(FONT_3);
//
//		this.fonts[0].setColor(Color.RED);
//		this.fonts[1].setColor(Color.BLUE);
//		this.fonts[2].setColor(Color.GREEN);
//		this.testStrings = new String[] {"I'm loaded from an atlas!", "I, too, am loaded from an atlas", "I'm with stupid ^"};
//
//		Gdx.gl.glClearColor(1, 1, 1, 1);
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		batch.begin();
//
//		for (int i = 0; i < fonts.length; ++i) {
//			fonts[i].draw(batch, testStrings[i], 16, 16 + 48 * i);
//		}
//
//		batch.end();
//	}
//
//	public override void Dispose () {
//		Array<String> loaded = this.assets.getAssetNames();
//
//		this.assets.Dispose();
//		this.batch.Dispose();
//
//		String name = ClassReflection.getSimpleName(this.getClass());
//		for (int i = 0; i < loaded.size; ++i) {
//			String asset = loaded.get(i);
//			if (this.assets.isLoaded(asset)) {
//				Gdx.app.error(name, asset + " not properly disposed of!");
//			} else {
//				Gdx.app.log(name, asset + " disposed of OK");
//			}
//		}
//	}
//}
