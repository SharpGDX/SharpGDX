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
//using SharpGDX.Maps;
//using SharpGDX.Maps.Objects;
//using SharpGDX.Maps.Tiled;
//using SharpGDX.Maps.Tiled.Renderers;
//using SharpGDX.Assets.Loaders;
//using SharpGDX.Assets;
//using SharpGDX.Assets.Loaders.Resolvers;
//
//namespace SharpGDX.Tests;
//
//public class TiledDrawableTest : GdxTest {
//
//	private static readonly float SCALE_CHANGE = 0.25f;
//
//	private Stage stage;
//	private IBatch batch;
//	private BitmapFont font;
//	private TextureAtlas atlas;
//	private TiledDrawable tiledDrawable;
//
//	public override void Create () {
//		stage = new Stage();
//		batch = new SpriteBatch();
//		font = new BitmapFont(Gdx.files.@internal("data/lsans-15.fnt"), false);
//
//		// Must be a texture atlas so uv is not just 0 and 1
//		atlas = new TextureAtlas(Gdx.files.@internal("data/testAtlas.atlas"));
//		tiledDrawable = new TiledDrawable(atlas.findRegion("tileTester"));
//
//		Gdx.input.setInputProcessor(this);
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//
//		IBatch batch = stage.getBatch();
//		batch.begin();
//
//		font.draw(batch,
//			"Scale: " + tiledDrawable.getScale() + "  (to change scale press: 'A' -" + SCALE_CHANGE + ", 'D' +" + SCALE_CHANGE + ")",
//			8, 20);
//
//		 float leftSpacingX = 40;
//		 float spacingX = 80;
//		 float bottomSpacing = 60;
//		 float spacingY = 40;
//		float inputX = Gdx.input.getX();
//		float inputY = Gdx.graphics.getHeight() - Gdx.input.getY();
//
//		 float clusterWidth = Math.Max(13, (inputX - leftSpacingX - (2 * spacingX)) / 3f);
//		 float clusterHeight = Math.Max(13, (inputY - bottomSpacing - (2 * spacingY)) / 3f);
//
//		 float leftX = leftSpacingX;
//		 float centerX = leftSpacingX + spacingX + clusterWidth;
//		 float rightX = leftSpacingX + (2 * spacingX) + (2 * clusterWidth);
//		 float topY = bottomSpacing + (2 * spacingY) + (2 * clusterHeight);
//		 float centerY = bottomSpacing + spacingY + clusterHeight;
//		 float bottomY = bottomSpacing;
//
//		drawTiledDrawableCluster(batch, leftX, topY, clusterWidth, clusterHeight, Align.topLeft);
//		drawTiledDrawableCluster(batch, centerX, topY, clusterWidth, clusterHeight, Align.top);
//		drawTiledDrawableCluster(batch, rightX, topY, clusterWidth, clusterHeight, Align.topRight);
//
//		drawTiledDrawableCluster(batch, leftX, centerY, clusterWidth, clusterHeight, Align.left);
//		drawTiledDrawableCluster(batch, centerX, centerY, clusterWidth, clusterHeight, Align.center);
//		drawTiledDrawableCluster(batch, rightX, centerY, clusterWidth, clusterHeight, Align.right);
//
//		drawTiledDrawableCluster(batch, leftX, bottomY, clusterWidth, clusterHeight, Align.bottomLeft);
//		drawTiledDrawableCluster(batch, centerX, bottomY, clusterWidth, clusterHeight, Align.bottom);
//		drawTiledDrawableCluster(batch, rightX, bottomY, clusterWidth, clusterHeight, Align.bottomRight);
//
//		batch.end();
//	}
//
//	private void drawTiledDrawableCluster (IBatch batch, float x, float y, float clusterWidth, float clusterHeight,
//		int align) {
//		tiledDrawable.setAlign(align);
//		tiledDrawable.draw(batch, x, y, clusterWidth, clusterHeight);
//		font.draw(batch, Align.toString(align), x, y - 5);
//	}
//
//	public override bool KeyDown (int keycode) {
//		if (keycode == Input.Keys.A) {
//			tiledDrawable.setScale(Math.Max(SCALE_CHANGE, tiledDrawable.getScale() - SCALE_CHANGE));
//		} else if (keycode == Input.Keys.D) {
//			tiledDrawable.setScale(tiledDrawable.getScale() + SCALE_CHANGE);
//		}
//		return true;
//	}
//
//	public override void Resize (int width, int height) {
//		batch.getProjectionMatrix().setToOrtho2D(0, 0, width, height);
//		stage.getViewport().update(width, height, true);
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		font.Dispose();
//		atlas.Dispose();
//	}
//}
