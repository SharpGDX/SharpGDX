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
//public class TextureDataTest : GdxTest {
//	private SpriteBatch spriteBatch;
//	private Texture texture;
//
//	public void create () {
//		spriteBatch = new SpriteBatch();
//// texture = new Texture(new PixmapTextureData(new Pixmap(Gdx.files.@internal("data/t8890.png")), null, false, true));
//		texture = new Texture(new FileTextureData(Gdx.files.@internal("data/t8890.png"), null, null, false));
//	}
//
//	public void render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		spriteBatch.begin();
//		spriteBatch.draw(texture, 100, 100);
//		spriteBatch.end();
//	}
//
//	public bool needsGL20 () {
//		return false;
//	}
//}
