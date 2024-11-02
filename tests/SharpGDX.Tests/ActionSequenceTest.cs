//using SharpGDX.Tests.Utils;
//using SharpGDX.Scenes.Scene2D;
//using SharpGDX.Scenes.Scene2D.Utils;
//using SharpGDX.Scenes.Scene2D.UI;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Utils.Viewports;
//using SharpGDX.Shims;
//
//namespace SharpGDX.Tests;
//
//public class ActionSequenceTest : GdxTest , Runnable {
//
//	Image img;
//	Image img2;
//	Image img3;
//	Stage stage;
//	Texture texture;
//
//	public override void Create () {
//		stage = new Stage();
//		texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"), false);
//		texture.setFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
//		img = new Image(new TextureRegion(texture));
//		img.setSize(100, 100);
//		img.setOrigin(50, 50);
//		img.setPosition(100, 100);
//
//		img2 = new Image(new TextureRegion(texture));
//		img2.setSize(100, 100);
//		img2.setOrigin(50, 50);
//		img2.setPosition(100, 100);
//
//		img3 = new Image(new TextureRegion(texture));
//		img3.setSize(100, 100);
//		img3.setOrigin(50, 50);
//		img3.setPosition(100, 100);
//
//		stage.addActor(img);
//		stage.addActor(img2);
//		stage.addActor(img3);
//
//		img.addAction(sequence());
//		img2.addAction(parallel(sequence(), moveBy(100, 0, 1)));
//		img3.addAction(sequence(parallel(moveBy(100, 200, 2)), Actions.run(this)));
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		stage.act(Math.min(Gdx.graphics.getDeltaTime(), 1 / 30f));
//		stage.draw();
//	}
//
//	@Override
//	public void run () {
//		Console.WriteLine("completed");
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		texture.Dispose();
//	}
//}
