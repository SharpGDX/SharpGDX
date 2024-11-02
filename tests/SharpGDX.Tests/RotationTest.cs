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
//public class RotationTest : GdxTest {
//
//	Texture texture;
//	TextureRegion region;
//	SpriteBatch batch;
//
//	public override void Create () {
//		texture = new Texture(Gdx.files.@internal("data/black_marked_0.png"));
//		region = new TextureRegion(texture);
//		batch = new SpriteBatch();
//		batch.getTransformMatrix().setToTranslation(30.5f, 30.5f, 0);
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		batch.draw(texture, 0, 0);
//		batch.draw(region, 128, 0, 64, 64, 128, 128, 1, 1, 90);
//		batch.draw(region, 128, 128, 64, 64, 128, 128, 1, 1, 180);
//		batch.draw(region, 0, 128, 64, 64, 128, 128, 1, 1, 270);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		texture.Dispose();
//		batch.Dispose();
//	}
//}
