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
//public class BitmapFontDistanceFieldTest : GdxTest {
//
//	private static final String TEXT = "Ta";
//	private static final Color COLOR = Color.BLACK;
//	private static final float[] SCALES = {0.25f, 0.5f, 1, 2, 4};
//
//	private static class DistanceFieldShader : ShaderProgram {
//		public DistanceFieldShader () {
//			super(Gdx.files.@internal("data/shaders/distancefield.vert"), Gdx.files.@internal("data/shaders/distancefield.frag"));
//			if (!isCompiled()) {
//				throw new RuntimeException("Shader compilation failed:\n" + getLog());
//			}
//		}
//
//		/** @param smoothing a value between 0 and 1 */
//		public void setSmoothing (float smoothing) {
//			float delta = 0.5f * MathUtils.clamp(smoothing, 0, 1);
//			setUniformf("u_lower", 0.5f - delta);
//			setUniformf("u_upper", 0.5f + delta);
//		}
//	}
//
//	private OrthographicCamera camera;
//	private SpriteBatch spriteBatch;
//
//	private Texture regularTexture;
//	private Texture distanceFieldTexture;
//	private BitmapFont descriptionFont;
//	private BitmapFont regularFont;
//	private BitmapFont distanceFieldFont;
//	private DistanceFieldShader distanceFieldShader;
//	private GlyphLayout layout = new GlyphLayout();
//
//	public override void Create () {
//		camera = new OrthographicCamera();
//		spriteBatch = new SpriteBatch();
//
//		descriptionFont = new BitmapFont(Gdx.files.@internal("data/lsans-15.fnt"), true);
//		descriptionFont.setColor(Color.RED);
//
//		regularTexture = new Texture(Gdx.files.@internal("data/verdana39.png"), true);
//		regularFont = new BitmapFont(Gdx.files.@internal("data/verdana39.fnt"), new TextureRegion(regularTexture), true);
//		regularFont.setColor(COLOR);
//
//		distanceFieldTexture = new Texture(Gdx.files.@internal("data/verdana39distancefield.png"), true);
//		distanceFieldFont = new BitmapFont(Gdx.files.@internal("data/verdana39distancefield.fnt"),
//			new TextureRegion(distanceFieldTexture), true);
//		distanceFieldFont.setColor(COLOR);
//
//		distanceFieldShader = new DistanceFieldShader();
//		ShaderProgram.pedantic = false; // Useful when debugging this test
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(1, 1, 1, 1);
//
//		spriteBatch.begin();
//
//		int x = 10;
//		x += drawFont(regularFont, "Regular font\nNearest filter", false, false, 0, x);
//		x += drawFont(regularFont, "Regular font\nLinear filter", true, false, 0, x);
//		x += drawFont(regularFont, "Regular font\nCustom shader", true, true, 1.0f, x);
//		x += drawFont(distanceFieldFont, "Distance field\nCustom shader", true, true, 1 / 8f, x);
//		x += drawFont(distanceFieldFont, "Distance field\nShowing distance field", false, false, 0, x);
//
//		spriteBatch.end();
//	}
//
//	private int drawFont (BitmapFont font, String description, boolean linearFiltering, boolean useShader, float smoothing,
//		int x) {
//		int y = 10;
//		float maxWidth = 0;
//
//		spriteBatch.setShader(null);
//		descriptionFont.draw(spriteBatch, description, x, y);
//		spriteBatch.flush();
//		y += 10 + 2 * descriptionFont.getLineHeight();
//
//		// set filters for each page
//		TextureFilter minFilter = linearFiltering ? TextureFilter.MipMapLinearNearest : TextureFilter.Nearest;
//		TextureFilter magFilter = linearFiltering ? TextureFilter.Linear : TextureFilter.Nearest;
//		for (int i = 0; i < font.getRegions().size; i++) {
//			font.getRegion(i).getTexture().setFilter(minFilter, magFilter);
//		}
//
//		if (useShader) {
//			spriteBatch.setShader(distanceFieldShader);
//		} else {
//			spriteBatch.setShader(null);
//		}
//
//		for (float scale : SCALES) {
//			font.getData().setScale(scale);
//			layout.setText(font, TEXT);
//			maxWidth = Math.max(maxWidth, layout.width);
//			if (useShader) {
//				distanceFieldShader.setSmoothing(smoothing / scale);
//			}
//			font.draw(spriteBatch, layout, x, y);
//			y += font.getLineHeight();
//			spriteBatch.flush();
//		}
//		return (int)Math.ceil(maxWidth);
//	}
//
//	private float getBaselineShift (float shift) {
//		return shift;
//	}
//
//	public override void Resize (int width, int height) {
//		super.resize(width, height);
//		camera.setToOrtho(true, width, height);
//		spriteBatch.setTransformMatrix(camera.view);
//		spriteBatch.setProjectionMatrix(camera.projection);
//	}
//
//	public override void Dispose () {
//		spriteBatch.Dispose();
//		regularTexture.Dispose();
//		distanceFieldTexture.Dispose();
//		descriptionFont.Dispose();
//		regularFont.Dispose();
//		distanceFieldFont.Dispose();
//		distanceFieldShader.Dispose();
//	}
//}
