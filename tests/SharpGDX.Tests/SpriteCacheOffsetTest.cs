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
//public class SpriteCacheOffsetTest : GdxTest , Input.IInputProcessor {
//	private int tileMapWidth = 10;
//	private int tileMapHeight = 5;
//	private int tileSize = 32;
//	private SpriteCache cache;
//	private Texture texture;
//
//	public void create () {
//		texture = new Texture(Gdx.files.@internal("data/badlogicsmall.jpg"));
//		Sprite sprite = new Sprite(texture);
//		sprite.setSize(tileSize, tileSize);
//
//		cache = new SpriteCache(1000, false);
//		for (int y = 0; y < tileMapHeight; y++) {
//			cache.beginCache();
//			for (int x = 0; x < tileMapWidth; x++) {
//				sprite.setPosition(x * tileSize, y * tileSize);
//				cache.add(sprite);
//			}
//			cache.endCache();
//			sprite.rotate90(true);
//		}
//	}
//
//	public void render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		cache.begin();
//		for (int y = 1; y < tileMapHeight - 1; y++)
//			cache.draw(y, 1, tileMapWidth - 2);
//		cache.end();
//	}
//
//	public override void Dispose () {
//		cache.Dispose();
//		texture.Dispose();
//	}
//}
