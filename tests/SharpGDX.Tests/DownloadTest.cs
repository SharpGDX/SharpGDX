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
//public class DownloadTest : GdxTest {
//	Texture texture;
//	SpriteBatch batch;
//	Queue<String> urls = new Queue<>();
//
//	public override void Create () {
//		urls.addLast("https://www.google.at/images/srpr/logo11w.png");
//		urls.addLast("https://placekitten.com/200/300");
//		urls.addLast("https://i.imgur.com/snfjsWx.png");
//
//		batch = new SpriteBatch();
//		Pixmap.downloadFromUrl(urls.removeFirst(), new Pixmap.DownloadPixmapResponseListener() {
//			@Override
//			public void downloadComplete (Pixmap pixmap) {
//				texture = new Texture(new PixmapTextureData(pixmap, pixmap.getFormat(), false, false, true));
//			}
//
//			@Override
//			public void downloadFailed (Throwable t) {
//				Gdx.app.log("EmptyDownloadTest", "Failed, trying next", t);
//				if (urls.notEmpty()) {
//					Pixmap.downloadFromUrl(urls.removeFirst(), this);
//				}
//			}
//		});
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		if (texture != null) batch.draw(texture, 0, 0);
//		batch.end();
//	}
//}
