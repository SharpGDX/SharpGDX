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
//public class SpriteCacheTest : GdxTest , Input.IInputProcessor {
//	int SPRITES = 400 / 2;
//
//	long startTime = TimeUtils.nanoTime();
//	int frames = 0;
//
//	Texture texture;
//	Texture texture2;
//	SpriteCache spriteCache;
//	int normalCacheID, spriteCacheID;
//	int renderMethod = 0;
//
//	private float[] sprites;
//	private float[] sprites2;
//
//	public override void Render () {
//		if (renderMethod == 0) renderNormal();
//		;
//		if (renderMethod == 1) renderSprites();
//	}
//
//	private void renderNormal () {
//		ScreenUtils.clear(0.7f, 0.7f, 0.7f, 1);
//
//		float begin = 0;
//		float end = 0;
//		float draw1 = 0;
//
//		long start = TimeUtils.nanoTime();
//		spriteCache.begin();
//		begin = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//		spriteCache.draw(normalCacheID);
//		draw1 = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//		spriteCache.end();
//		end = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		if (TimeUtils.nanoTime() - startTime > 1000000000) {
//// Gdx.app.log( "SpriteBatch", "fps: " + frames + ", render calls: " + spriteBatch.renderCalls + ", " + begin + ", " + draw1 +
//// ", " + draw2 + ", " + drawText + ", " + end );
//			frames = 0;
//			startTime = TimeUtils.nanoTime();
//		}
//		frames++;
//	}
//
//	private void renderSprites () {
//		ScreenUtils.clear(0.7f, 0.7f, 0.7f, 1);
//
//		float begin = 0;
//		float end = 0;
//		float draw1 = 0;
//		float draw2 = 0;
//		float drawText = 0;
//
//		long start = TimeUtils.nanoTime();
//		spriteCache.begin();
//		begin = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//		spriteCache.draw(spriteCacheID);
//		draw1 = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//		spriteCache.end();
//		end = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		if (TimeUtils.nanoTime() - startTime > 1000000000) {
//// Gdx.app.log( "SpriteBatch", "fps: " + frames + ", render calls: " + spriteBatch.renderCalls + ", " + begin + ", " + draw1 +
//// ", " + draw2 + ", " + drawText + ", " + end );
//			frames = 0;
//			startTime = TimeUtils.nanoTime();
//		}
//		frames++;
//	}
//
//	public override void Create () {
//		spriteCache = new SpriteCache(1000, true);
//
//		texture = new Texture(Gdx.files.@internal("data/badlogicsmall.jpg"));
//		texture.setFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
//
//		Pixmap pixmap = new Pixmap(32, 32, Pixmap.Format.RGBA8888);
//		pixmap.setColor(1, 1, 0, 0.5f);
//		pixmap.fill();
//		texture2 = new Texture(pixmap);
//		pixmap.Dispose();
//
//		sprites = new float[SPRITES * 6];
//		sprites2 = new float[SPRITES * 6];
//		Sprite[] sprites3 = new Sprite[SPRITES * 2];
//
//		for (int i = 0; i < sprites.Length; i += 6) {
//			sprites[i] = (int)(Math.random() * (Gdx.graphics.getWidth() - 32));
//			sprites[i + 1] = (int)(Math.random() * (Gdx.graphics.getHeight() - 32));
//			sprites[i + 2] = 0;
//			sprites[i + 3] = 0;
//			sprites[i + 4] = 32;
//			sprites[i + 5] = 32;
//			sprites2[i] = (int)(Math.random() * (Gdx.graphics.getWidth() - 32));
//			sprites2[i + 1] = (int)(Math.random() * (Gdx.graphics.getHeight() - 32));
//			sprites2[i + 2] = 0;
//			sprites2[i + 3] = 0;
//			sprites2[i + 4] = 32;
//			sprites2[i + 5] = 32;
//		}
//
//		for (int i = 0; i < SPRITES * 2; i++) {
//			int x = (int)(Math.random() * (Gdx.graphics.getWidth() - 32));
//			int y = (int)(Math.random() * (Gdx.graphics.getHeight() - 32));
//
//			if (i >= SPRITES)
//				sprites3[i] = new Sprite(texture2, 32, 32);
//			else
//				sprites3[i] = new Sprite(texture, 32, 32);
//			sprites3[i].setPosition(x, y);
//			sprites3[i].setOrigin(16, 16);
//		}
//
//		float scale = 1;
//		float angle = 15;
//
//		spriteCache.beginCache();
//		for (int i = 0; i < sprites2.Length; i += 6)
//			spriteCache.add(texture2, sprites2[i], sprites2[i + 1], 16, 16, 32, 32, scale, scale, angle, 0, 0, 32, 32, false, false);
//		for (int i = 0; i < sprites.Length; i += 6)
//			spriteCache.add(texture, sprites[i], sprites[i + 1], 16, 16, 32, 32, scale, scale, angle, 0, 0, 32, 32, false, false);
//		normalCacheID = spriteCache.endCache();
//
//		angle = -15;
//
//		spriteCache.beginCache();
//		for (int i = SPRITES; i < SPRITES << 1; i++) {
//			sprites3[i].setRotation(angle);
//			sprites3[i].setScale(scale);
//			spriteCache.add(sprites3[i]);
//		}
//		for (int i = 0; i < SPRITES; i++) {
//			sprites3[i].setRotation(angle);
//			sprites3[i].setScale(scale);
//			spriteCache.add(sprites3[i]);
//		}
//		spriteCacheID = spriteCache.endCache();
//
//		Gdx.input.setInputProcessor(this);
//	}
//
//	public override bool KeyDown (int keycode) {
//		if (keycode != Input.Keys.SPACE) return false;
//		float scale = MathUtils.random(0.75f, 1.25f);
//		float angle = MathUtils.random(1, 360);
//		spriteCache.beginCache(normalCacheID);
//		for (int i = 0; i < sprites2.Length; i += 6)
//			spriteCache.add(texture2, sprites2[i], sprites2[i + 1], 16, 16, 32, 32, scale, scale, angle, 0, 0, 32, 32, false, false);
//		for (int i = 0; i < sprites.Length; i += 6)
//			spriteCache.add(texture, sprites[i], sprites[i + 1], 16, 16, 32, 32, scale, scale, angle, 0, 0, 32, 32, false, false);
//		spriteCache.endCache();
//		return false;
//	}
//
//    public override bool TouchUp (int x, int y, int pointer, int button) {
//		renderMethod = (renderMethod + 1) % 2;
//		return false;
//	}
//
//	public override void Dispose () {
//		texture.Dispose();
//		texture2.Dispose();
//		spriteCache.Dispose();
//	}
//}
