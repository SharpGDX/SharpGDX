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
//public class PixmapTest : GdxTest {
//	Pixmap pixmap;
//	Texture texture;
//	SpriteBatch batch;
//	TextureRegion region;
//	Pixmap pixmapCustom;
//	Texture textureCustom;
//	TextureRegion regionCustom;
//
//	public override void Create () {
//		// Create an empty dynamic pixmap
//		pixmap = new Pixmap(800, 480, Pixmap.Format.RGBA8888);
//		pixmapCustom = new Pixmap(256, 256, Pixmap.Format.RGBA8888);
//
//		ByteBuffer buffer = BufferUtils.newByteBuffer(pixmapCustom.getWidth() * pixmapCustom.getHeight() * 4);
//		for (int y = 0; y < pixmapCustom.getHeight(); y++) {
//			for (int x = 0; x < pixmapCustom.getWidth(); x++) {
//				buffer.put((byte)x);
//				buffer.put((byte)y);
//				buffer.put((byte)0);
//				buffer.put((byte)255);
//			}
//		}
//		buffer.flip();
//		pixmapCustom.setPixels(buffer);
//		textureCustom = new Texture(pixmapCustom);
//		regionCustom = new TextureRegion(textureCustom);
//
//		// Create a texture to contain the pixmap
//		texture = new Texture(1024, 1024, Pixmap.Format.RGBA8888); // Pixmap.Format.RGBA8888);
//		texture.setFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Linear);
//		texture.setWrap(Texture.TextureWrap.ClampToEdge, Texture.TextureWrap.ClampToEdge);
//
//		pixmap.setColor(1.0f, 0.0f, 0.0f, 1.0f); // Red
//		pixmap.drawLine(0, 0, 100, 100);
//
//		pixmap.setColor(0.0f, 0.0f, 1.0f, 1.0f); // Blue
//		pixmap.drawLine(100, 100, 200, 0);
//
//		pixmap.setColor(0.0f, 1.0f, 0.0f, 1.0f); // Green
//		pixmap.drawLine(100, 0, 100, 100);
//
//		pixmap.setColor(1.0f, 1.0f, 1.0f, 1.0f); // White
//		pixmap.drawCircle(400, 300, 100);
//
//		// Blit the composited overlay to a texture
//		texture.draw(pixmap, 0, 0);
//		region = new TextureRegion(texture, 0, 0, 800, 480);
//		batch = new SpriteBatch();
//
//        {
//            Pixmap pixmap = new Pixmap(512, 1024, Pixmap.Format.RGBA8888);
//            for (int y = 0; y < pixmap.getHeight(); y++)
//            {
//                // 1024
//                for (int x = 0; x < pixmap.getWidth(); x++)
//                {
//                    // 512
//                    pixmap.getPixel(x, y);
//                }
//            }
//
//            pixmap.Dispose();
//        }
//    }
//
//	public override void Render () {
//		ScreenUtils.clear(0.6f, 0.6f, 0.6f, 1);
//		batch.begin();
//		batch.draw(region, 0, 0);
//		batch.draw(regionCustom, 0, 0);
//		batch.end();
//	}
//}
