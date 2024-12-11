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
//public class TouchpadTest : GdxTest {
//	Stage stage;
//	Touchpad touchpad;
//
//	public override void Create () {
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//
//		Skin skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		touchpad = new Touchpad(20, skin);
//		touchpad.setBounds(15, 15, 100, 100);
//		stage.addActor(touchpad);
//	}
//
//	public override void Render () {
//		// Console.WriteLine(touchpad.getKnobPercentX() + " " + touchpad.getKnobPercentY());
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		stage.act(Gdx.graphics.getDeltaTime());
//		stage.draw();
//	}
//
//	public override void Resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//	}
//}
