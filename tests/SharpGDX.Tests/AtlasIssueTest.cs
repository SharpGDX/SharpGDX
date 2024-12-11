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
//public class AtlasIssueTest : GdxTest {
//	SpriteBatch batch;
//	Sprite sprite;
//	TextureAtlas atlas;
//	BitmapFont font;
//
//	public void create () {
//		batch = new SpriteBatch();
//		batch.setProjectionMatrix(new Matrix4().setToOrtho2D(0, 0, 855, 480));
//		atlas = new TextureAtlas(Gdx.files.@internal("data/issue_pack"), Gdx.files.@internal("data/"));
//		sprite = atlas.createSprite("map");
//		font = new BitmapFont(Gdx.files.@internal("data/font.fnt"), Gdx.files.@internal("data/font.png"), false);
//		GDX.GL.glClearColor(0, 1, 0, 1);
//	}
//
//	public void render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		sprite.draw(batch);
//		font.draw(batch, "fps:" + Gdx.graphics.getFramesPerSecond(), 26, 65);
//		batch.end();
//	}
//
//	public boolean needsGL20 () {
//		return false;
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		atlas.Dispose();
//		font.Dispose();
//	}
//}
