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
//public class GroupFadeTest : GdxTest {
//
//	Texture texture;
//	Stage stage;
//
//	public override void Create () {
//		texture = new Texture(Gdx.files.@internal("data/badlogicsmall.jpg"));
//		stage = new Stage();
//
//		for (int i = 0; i < 100; i++) {
//			Image img = new Image(new TextureRegion(texture));
//			img.setX((float)Math.random() * 480);
//			img.setY((float)Math.random() * 320);
//			img.getColor().a = (float)Math.random() * 0.5f + 0.5f;
//			stage.addActor(img);
//		}
//
//		stage.getRoot().addAction(forever(sequence(fadeOut(3), fadeIn(3))));
//	}
//
//	public override void Render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		stage.act(Gdx.graphics.getDeltaTime());
//		stage.draw();
//	}
//
//	public override void Dispose () {
//		texture.Dispose();
//		stage.Dispose();
//	}
//}
