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
//public class TextButtonTest : GdxTest {
//	private Stage stage;
//	private Skin skin;
//
//	public override void Create () {
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		for (int i = 0; i < 1; i++) {
//			TextButton t = new TextButton("Button" + i, skin);
//			t.setX(MathUtils.random(0, Gdx.graphics.getWidth()));
//			t.setY(MathUtils.random(0, Gdx.graphics.getHeight()));
//			t.setWidth(MathUtils.random(50, 200));
//			t.setHeight(MathUtils.random(0, 100));
//			stage.addActor(t);
//		}
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//		stage.draw();
//		Gdx.app.log("X", "FPS: " + Gdx.graphics.getFramesPerSecond());
//		SpriteBatch spriteBatch = (SpriteBatch)stage.getBatch();
//		Gdx.app.log("X", "render calls: " + spriteBatch.totalRenderCalls);
//		spriteBatch.totalRenderCalls = 0;
//	}
//
//	public override void Resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		skin.Dispose();
//	}
//}
