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
//public class TextAreaTest2 : GdxTest {
//	private Stage stage;
//	private Skin skin;
//
//	public override void Create () {
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		// Create a string that perfectly fills the float array used in the textarea float array
//		FloatArray dummyArray = new FloatArray();
//		String limit = "";
//		// Minus one, because TextField adds a magic char
//		for (int i = 0; i < dummyArray.items.Length - 1; i++) {
//			limit += "a";
//		}
//
//		TextArea textArea = new TextArea(limit, skin);
//		textArea.setX(10);
//		textArea.setY(10);
//		textArea.setWidth(200);
//		textArea.setHeight(200);
//
//		stage.addActor(textArea);
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
