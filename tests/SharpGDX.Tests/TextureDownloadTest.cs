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
//public class TextureDownloadTest : GdxTest {
//	TextureRegion image;
//	BitmapFont font;
//	SpriteBatch batch;
//
//	public override void Create () {
//		new Thread(new Runnable() {
//			/** Downloads the content of the specified url to the array. The array has to be big enough. */
//			private int download (byte[] out, String url) {
//				InputStream in = null;
//				try {
//					HttpURLConnection conn = null;
//					conn = (HttpURLConnection)new URL(url).openConnection();
//					conn.setDoInput(true);
//					conn.setDoOutput(false);
//					conn.setUseCaches(true);
//					conn.connect();
//					in = conn.getInputStream();
//					int readBytes = 0;
//					while (true) {
//						int length = in.read(out, readBytes, out.length - readBytes);
//						if (length == -1) break;
//						readBytes += length;
//					}
//					return readBytes;
//				} catch (Exception ex) {
//					return 0;
//				} finally {
//					StreamUtils.closeQuietly(in);
//				}
//			}
//
//			@Override
//			public void run () {
//				byte[] bytes = new byte[200 * 1024]; // assuming the content is not bigger than 200kb.
//				int numBytes = download(bytes, "https://www.badlogicgames.com/wordpress/wp-content/uploads/2012/01/badlogic-new.png");
//				if (numBytes != 0) {
//					// load the pixmap, make it a power of two if necessary (not needed for GL ES 2.0!)
//					Pixmap pixmap = new Pixmap(bytes, 0, numBytes);
//					final int originalWidth = pixmap.getWidth();
//					final int originalHeight = pixmap.getHeight();
//					int width = MathUtils.nextPowerOfTwo(pixmap.getWidth());
//					int height = MathUtils.nextPowerOfTwo(pixmap.getHeight());
//					final Pixmap potPixmap = new Pixmap(width, height, pixmap.getFormat());
//					potPixmap.setBlending(Blending.None);
//					potPixmap.drawPixmap(pixmap, 0, 0, 0, 0, pixmap.getWidth(), pixmap.getHeight());
//					pixmap.Dispose();
//					Gdx.app.postRunnable(new Runnable() {
//						@Override
//						public void run () {
//							image = new TextureRegion(new Texture(potPixmap), 0, 0, originalWidth, originalHeight);
//						}
//					});
//				}
//			}
//		}).start();
//
//		font = new BitmapFont();
//		batch = new SpriteBatch();
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//
//		if (image != null) {
//			batch.begin();
//			batch.draw(image, 100, 100);
//			batch.end();
//		} else {
//			batch.begin();
//			font.draw(batch, "Downloading...", 100, 100);
//			batch.end();
//		}
//	}
//}
