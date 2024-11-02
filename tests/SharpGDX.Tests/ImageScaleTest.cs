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
//public class ImageScaleTest : GdxTest {
//	Stage stage;
//	Texture texture;
//
//	public void create () {
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//
//		texture = new Texture("data/group-debug.png");
//		Image image = new Image(texture);
//		image.setScaling(Scaling.fit);
//		image.setBounds(100, 100, 400, 200);
//		stage.addActor(image);
//
//		Image image2 = new Image(texture);
//		image2.setScaling(Scaling.fit);
//		image.setBounds(100, 100, 400, 200);
//		image2.setOrigin(200, 100);
//		image2.setScale(0.5f);
//		stage.addActor(image2);
//
//	}
//
//	public void render () {
//		ScreenUtils.clear(0, 0, 0, 1);
//		stage.draw();
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		texture.Dispose();
//	}
//
//	public void resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//	}
//}
