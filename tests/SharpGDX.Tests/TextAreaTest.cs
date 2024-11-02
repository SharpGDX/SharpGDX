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
//public class TextAreaTest : GdxTest {
//	private Stage stage;
//	private Skin skin;
//
//	public override void Create () {
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		skin.getFont("default-font").setFixedWidthGlyphs("0123456789");
//		TextArea textArea = new TextArea("Text Area\n1111111111\n0123456789\nEssentially, a text field\nwith\nmultiple\nlines.\n"
//			+ "It can even handle very loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong lines.",
//			skin);
//		textArea.setX(10);
//		textArea.setY(10);
//		textArea.setWidth(200);
//		textArea.setHeight(200);
//
//		TextField textField = new TextField("Text field", skin);
//		textField.setX(10);
//		textField.setY(220);
//		textField.setWidth(200);
//		textField.setHeight(30);
//		stage.addActor(textArea);
//		stage.addActor(textField);
//
//		Gdx.input.setCatchKey(Input.Keys.TAB, true);
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
