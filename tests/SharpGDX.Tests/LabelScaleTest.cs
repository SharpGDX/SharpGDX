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
//public class LabelScaleTest : GdxTest {
//	Skin skin;
//	Stage stage;
//	SpriteBatch batch;
//	Actor root;
//
//	public override void Create () {
//		batch = new SpriteBatch();
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//
//		Table table = new Table();
//		stage.addActor(table);
//		table.setPosition(200, 65);
//
//		Label label1 = new Label("This text is scaled 2x.", skin);
//		label1.setFontScale(2);
//		Label label2 = new Label(
//			"This text is scaled. This text is scaled. This text is scaled. This text is scaled. This text is scaled. ", skin);
//		label2.setWrap(true);
//		label2.setFontScale(0.75f, 0.75f);
//
//		table.debug();
//		table.add(label1);
//		table.row();
//		table.add(label2).fill();
//		table.pack();
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		skin.Dispose();
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//
//		stage.act(Math.min(Gdx.graphics.getDeltaTime(), 1 / 30f));
//		stage.draw();
//	}
//
//	public override void Resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//	}
//}
