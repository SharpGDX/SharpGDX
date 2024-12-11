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
//public class ComplexActionTest : GdxTest {
//
//	Stage stage;
//	Texture texture;
//
//	public override void Create () {
//		stage = new Stage();
//
//		Action complexAction = forever(sequence(parallel(rotateBy(180, 2), scaleTo(1.4f, 1.4f, 2), alpha(0.7f, 2)),
//			parallel(rotateBy(180, 2), scaleTo(1.0f, 1.0f, 2), alpha(1.0f, 2))));
//
//		texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"), false);
//		texture.setFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
//
//		final Image img1 = new Image(new TextureRegion(texture));
//		img1.setSize(100, 100);
//		img1.setOrigin(50, 50);
//		img1.setPosition(50, 50);
//
//		final Image img2 = new Image(new TextureRegion(texture));
//		img2.setSize(50, 50);
//		img2.setOrigin(50, 50);
//		img2.setPosition(150, 150);
//
//		stage.addActor(img1);
//		stage.addActor(img2);
//
//		img1.addAction(complexAction);
//		// img2.action(complexAction.copy());
//	}
//
//	public override void Render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		stage.act(Math.min(Gdx.graphics.getDeltaTime(), 1 / 30f));
//		stage.draw();
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		texture.Dispose();
//	}
//}
