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
//public class Gdx2DTest : GdxTest {
//	SpriteBatch batch;
//	List<Sprite> sprites;
//
//	Texture textureFromPixmap (Gdx2DPixmap pixmap) {
//		Texture texture = new Texture(pixmap.getWidth(), pixmap.getHeight(), Format.RGB565);
//		texture.bind();
//		Gdx.gl.glTexImage2D(GL20.GL_TEXTURE_2D, 0, pixmap.getGLInternalFormat(), pixmap.getWidth(), pixmap.getHeight(), 0,
//			pixmap.getGLFormat(), pixmap.getGLType(), pixmap.getPixels());
//		return texture;
//	}
//
//	void drawToPixmap (Gdx2DPixmap pixmap) {
//		pixmap.clear(Color.rgba8888(1, 0, 0, 0.1f));
//		pixmap.setPixel(16, 16, Color.rgba8888(0, 0, 1, 1));
//		int clearColor = 0;
//		int pixelColor = 0;
//		switch (pixmap.getFormat()) {
//		case Gdx2DPixmap.GDX2D_FORMAT_ALPHA:
//			clearColor = Color.rgba8888(1, 1, 1, 0.1f);
//			pixelColor = Color.rgba8888(1, 1, 1, 1);
//			break;
//		case Gdx2DPixmap.GDX2D_FORMAT_LUMINANCE_ALPHA:
//			clearColor = 0x36363619; // Color.rgba8888(1, 1, 1, 0.1f);
//			pixelColor = 0xffffff12;
//			break;
//		case Gdx2DPixmap.GDX2D_FORMAT_RGB565:
//			clearColor = Color.rgba8888(1, 0, 0, 1);
//			pixelColor = Color.rgba8888(0, 0, 1, 1);
//			break;
//		case Gdx2DPixmap.GDX2D_FORMAT_RGB888:
//			clearColor = Color.rgba8888(1, 0, 0, 1);
//			pixelColor = Color.rgba8888(0, 0, 1, 1);
//			break;
//		case Gdx2DPixmap.GDX2D_FORMAT_RGBA4444:
//			clearColor = 0xff000011;
//			pixelColor = Color.rgba8888(0, 0, 1, 1);
//			break;
//		case Gdx2DPixmap.GDX2D_FORMAT_RGBA8888:
//			clearColor = Color.rgba8888(1, 0, 0, 0.1f);
//			pixelColor = Color.rgba8888(0, 0, 1, 1);
//
//		}
//		if (pixmap.getPixel(15, 16) != clearColor) throw new RuntimeException("error clear: " + pixmap.getFormatString());
//		if (pixmap.getPixel(16, 16) != pixelColor) throw new RuntimeException("error pixel: " + pixmap.getFormatString());
//		pixmap.drawLine(0, 0, 31, 31, Color.rgba8888(1, 1, 1, 1));
//		pixmap.drawRect(10, 10, 5, 7, Color.rgba8888(1, 1, 0, 0.5f));
//		pixmap.fillRect(20, 10, 5, 7, Color.rgba8888(0, 1, 1, 0.5f));
//		pixmap.drawCircle(16, 16, 10, Color.rgba8888(1, 0, 1, 1));
//		pixmap.fillCircle(16, 16, 6, Color.rgba8888(0, 1, 0, 0.5f));
//		pixmap.fillTriangle(16, 16, 18, 18, 20, 14, Color.rgba8888(0, 0.5f, 0, 0.5f));
//		pixmap.drawLine(0, -1, 0, 0, Color.rgba8888(1, 1, 0, 1));
//		pixmap.drawLine(41, -10, 31, 0, Color.rgba8888(1, 1, 0, 1));
//		pixmap.drawLine(10, 41, 0, 31, Color.rgba8888(0, 1, 1, 1));
//		pixmap.drawLine(41, 41, 31, 31, Color.rgba8888(0, 1, 1, 1));
//
//		pixmap.drawRect(-10, -10, 20, 20, Color.rgba8888(0, 1, 1, 1));
//		pixmap.drawRect(21, -10, 20, 20, Color.rgba8888(0, 1, 1, 1));
//		pixmap.drawRect(-10, 21, 20, 20, Color.rgba8888(0, 1, 1, 1));
//		pixmap.drawRect(21, 21, 20, 20, Color.rgba8888(0, 1, 1, 1));
//
//		pixmap.fillRect(-10, -10, 20, 20, Color.rgba8888(0, 1, 1, 0.5f));
//		pixmap.fillRect(21, -10, 20, 20, Color.rgba8888(0, 1, 1, 0.5f));
//		pixmap.fillRect(-10, 21, 20, 20, Color.rgba8888(0, 1, 1, 0.5f));
//		pixmap.fillRect(21, 21, 20, 20, Color.rgba8888(0, 1, 1, 0.5f));
//	}
//
//	Gdx2DPixmap[] testPixmaps () {
//		int[] formats = {Gdx2DPixmap.GDX2D_FORMAT_ALPHA, Gdx2DPixmap.GDX2D_FORMAT_LUMINANCE_ALPHA, Gdx2DPixmap.GDX2D_FORMAT_RGB565,
//			Gdx2DPixmap.GDX2D_FORMAT_RGB888, Gdx2DPixmap.GDX2D_FORMAT_RGBA4444, Gdx2DPixmap.GDX2D_FORMAT_RGBA8888};
//
//		Gdx2DPixmap[] pixmaps = new Gdx2DPixmap[formats.length];
//		for (int i = 0; i < pixmaps.length; i++) {
//			Gdx2DPixmap pixmap = new Gdx2DPixmap(64, 32, formats[i]);
//			drawToPixmap(pixmap);
//			pixmaps[i] = pixmap;
//		}
//		return pixmaps;
//	}
//
//	public override void Create () {
//		batch = new SpriteBatch();
//		sprites = new ArrayList<Sprite>();
//		Gdx2DPixmap[] pixmaps = testPixmaps();
//
//		Gdx2DPixmap composite = new Gdx2DPixmap(512, 256, Gdx2DPixmap.GDX2D_FORMAT_RGBA8888);
//		composite.clear(0);
//		composite.setBlend(Gdx2DPixmap.GDX2D_BLEND_NONE);
//		for (int i = 0; i < pixmaps.length; i++) {
//			composite.setScale(Gdx2DPixmap.GDX2D_SCALE_NEAREST);
//			composite.drawPixmap(pixmaps[i], 0, 0, 32, 32, i * 64, 0, 64, 64);
//			composite.drawPixmap(pixmaps[i], 0, 0, 32, 32, i * 64, 64, 16, 16);
//			composite.drawPixmap(pixmaps[i], 0, 0, 32, 32, i * 64, 0, 64, 64);
//			composite.drawPixmap(pixmaps[i], 0, 0, 32, 32, i * 64, 64, 16, 16);
//			composite.setScale(Gdx2DPixmap.GDX2D_SCALE_LINEAR);
//			composite.drawPixmap(pixmaps[i], 0, 0, 32, 32, i * 64, 100, 64, 64);
//			composite.drawPixmap(pixmaps[i], 0, 0, 32, 32, i * 64, 164, 16, 16);
//			composite.drawPixmap(pixmaps[i], 0, 0, 32, 32, i * 64, 100, 64, 64);
//			composite.drawPixmap(pixmaps[i], 0, 0, 32, 32, i * 64, 164, 16, 16);
//			Sprite sprite = new Sprite(textureFromPixmap(pixmaps[i]));
//			sprite.setPosition(10 + i * 32, 10);
//			sprites.add(sprite);
//		}
//
//		Sprite sprite = new Sprite(textureFromPixmap(composite));
//		sprite.setPosition(10, 50);
//		sprites.add(sprite);
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//		batch.begin();
//		for (int i = 0; i < sprites.size(); i++) {
//			sprites.get(i).draw(batch);
//		}
//		batch.end();
//	}
//}
