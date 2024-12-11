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
//public class SpriteBatchRotationTest : GdxTest {
//	SpriteBatch spriteBatch;
//	Texture texture;
//	// Font font;
//	float angle = 0;
//	float scale = 1;
//	float vScale = 1;
//	IntBuffer pixelBuffer;
//
//	public override void Render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		spriteBatch.begin();
//		spriteBatch.draw(texture, 16, 10, 16, 16, 32, 32, 1, 1, 0, 0, 0, texture.getWidth(), texture.getHeight(), false, false);
//		spriteBatch.draw(texture, 64, 10, 32, 32, 0, 0, texture.getWidth(), texture.getHeight(), false, false);
//		spriteBatch.draw(texture, 112, 10, 0, 0, texture.getWidth(), texture.getHeight());
//
//		spriteBatch.draw(texture, 16, 58, 16, 16, 32, 32, 1, 1, angle, 0, 0, texture.getWidth(), texture.getHeight(), false, false);
//		spriteBatch.draw(texture, 64, 58, 16, 16, 32, 32, scale, scale, 0, 0, 0, texture.getWidth(), texture.getHeight(), false,
//			false);
//		spriteBatch.draw(texture, 112, 58, 16, 16, 32, 32, scale, scale, angle, 0, 0, texture.getWidth(), texture.getHeight(),
//			false, false);
//		spriteBatch.draw(texture, 160, 58, 0, 0, 32, 32, scale, scale, angle, 0, 0, texture.getWidth(), texture.getHeight(), false,
//			false);
//
//		spriteBatch.end();
//		angle += 20 * Gdx.graphics.getDeltaTime();
//		scale += vScale * Gdx.graphics.getDeltaTime();
//		if (scale > 2) {
//			vScale = -vScale;
//			scale = 2;
//		}
//		if (scale < 0) {
//			vScale = -vScale;
//			scale = 0;
//		}
//
//	}
//
//	public override void Create () {
//		spriteBatch = new SpriteBatch();
//		texture = new Texture(Gdx.files.@internal("data/test.png"));
//	}
//}
