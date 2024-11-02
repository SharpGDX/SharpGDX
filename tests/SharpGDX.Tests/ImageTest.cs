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
//public class ImageTest : GdxTest {
//	Skin skin;
//	Stage ui;
//	Table root;
//	TextureRegion image2;
//
//	public override void Create () {
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		image2 = new TextureRegion(new Texture(Gdx.files.@internal("data/badlogic.jpg")));
//		ui = new Stage();
//		Gdx.input.setInputProcessor(ui);
//
//		root = new Table();
//		root.setSize(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		ui.addActor(root);
//		root.debug();
//
//		Image image = new Image(image2);
//		image.setScaling(Scaling.fill);
//		root.add(image).width(image2.getRegionWidth()).height(image2.getRegionHeight());
//	}
//
//	public override void Dispose () {
//		ui.Dispose();
//		skin.Dispose();
//		image2.getTexture().Dispose();
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//		ui.act(Math.min(Gdx.graphics.getDeltaTime(), 1 / 30f));
//		ui.draw();
//	}
//
//	public override void Resize (int width, int height) {
//		ui.getViewport().update(width, height, true);
//	}
//}
