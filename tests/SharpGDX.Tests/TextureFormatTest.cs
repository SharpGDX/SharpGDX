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
//using SharpGDX.Files;
//
//namespace SharpGDX.Tests;
//
//public class TextureFormatTest : GdxTest {
//
//	Texture[] nonMipMapped = new Texture[6];
//	Texture[] mipMapped = new Texture[6];
//	SpriteBatch batch;
//
//	public override void Create () {
//		FileHandle file = Gdx.files.@internal("data/bobargb8888-32x32.png");
//		nonMipMapped[0] = new Texture(file, Pixmap.Format.Alpha, false);
//		nonMipMapped[1] = new Texture(file, Pixmap.Format.LuminanceAlpha, false);
//		nonMipMapped[2] = new Texture(file, Pixmap.Format.RGB888, false);
//		nonMipMapped[3] = new Texture(file, Pixmap.Format.RGB565, false);
//		nonMipMapped[4] = new Texture(file, Pixmap.Format.RGBA8888, false);
//		nonMipMapped[5] = new Texture(file, Pixmap.Format.RGBA4444, false);
//
//		mipMapped[0] = new Texture(file, Pixmap.Format.Alpha, true);
//		mipMapped[1] = new Texture(file, Pixmap.Format.LuminanceAlpha, true);
//		mipMapped[2] = new Texture(file, Pixmap.Format.RGB888, true);
//		mipMapped[3] = new Texture(file, Pixmap.Format.RGB565, true);
//		mipMapped[4] = new Texture(file, Pixmap.Format.RGBA8888, true);
//		mipMapped[5] = new Texture(file, Pixmap.Format.RGBA4444, true);
//
//		batch = new SpriteBatch();
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//		batch.begin();
//		for (int i = 0; i < 6; i++) {
//			batch.draw(nonMipMapped[i], i * 32, 0);
//		}
//		for (int i = 0; i < 6; i++) {
//			batch.draw(mipMapped[i], i * 32, 32);
//		}
//		batch.end();
//	}
//}
