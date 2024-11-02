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
//using SharpGDX.Input;
//
//namespace SharpGDX.Tests;
//
//public class SpriteBatchTest : GdxTest , IInputProcessor {
//	int SPRITES = 100 / 2;
//
//	long startTime = TimeUtils.nanoTime();
//	int frames = 0;
//
//	Texture texture;
//	Texture texture2;
//// Font font;
//	SpriteBatch spriteBatch;
//	float[] sprites = new float[SPRITES * 6];
//	float[] sprites2 = new float[SPRITES * 6];
//	Sprite[] sprites3 = new Sprite[SPRITES * 2];
//	float angle = 0;
//	float ROTATION_SPEED = 20;
//	float scale = 1;
//	float SCALE_SPEED = -1;
//	int renderMethod = 0;
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
//		float draw2 = 0;
//		float drawText = 0;
//
//		angle += ROTATION_SPEED * Gdx.graphics.getDeltaTime();
//		scale += SCALE_SPEED * Gdx.graphics.getDeltaTime();
//		if (scale < 0.5f) {
//			scale = 0.5f;
//			SCALE_SPEED = 1;
//		}
//		if (scale > 1.0f) {
//			scale = 1.0f;
//			SCALE_SPEED = -1;
//		}
//
//		long start = TimeUtils.nanoTime();
//		spriteBatch.begin();
//		begin = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//		for (int i = 0; i < sprites.Length; i += 6)
//			spriteBatch.draw(texture, sprites[i], sprites[i + 1], 16, 16, 32, 32, scale, scale, angle, 0, 0, 32, 32, false, false);
//		draw1 = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//		for (int i = 0; i < sprites2.Length; i += 6)
//			spriteBatch.draw(texture2, sprites2[i], sprites2[i + 1], 16, 16, 32, 32, scale, scale, angle, 0, 0, 32, 32, false,
//				false);
//		draw2 = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//// spriteBatch.drawText(font, "Question?", 100, 300, Color.RED);
//// spriteBatch.drawText(font, "and another this is a test", 200, 100, Color.WHITE);
//// spriteBatch.drawText(font, "all hail and another this is a test", 200, 200, Color.WHITE);
//// spriteBatch.drawText(font, "normal fps: " + Gdx.graphics.getFramesPerSecond(), 10, 30, Color.RED);
//		drawText = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//		spriteBatch.end();
//		end = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		if (TimeUtils.nanoTime() - startTime > 1000000000) {
//			Gdx.app.log("SpriteBatch", "fps: " + frames + ", render calls: " + spriteBatch.renderCalls + ", " + begin + ", " + draw1
//				+ ", " + draw2 + ", " + drawText + ", " + end);
//			frames = 0;
//			startTime = TimeUtils.nanoTime();
//		}
//		frames++;
//
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
//		spriteBatch.begin();
//		begin = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		float angleInc = ROTATION_SPEED * Gdx.graphics.getDeltaTime();
//		scale += SCALE_SPEED * Gdx.graphics.getDeltaTime();
//		if (scale < 0.5f) {
//			scale = 0.5f;
//			SCALE_SPEED = 1;
//		}
//		if (scale > 1.0f) {
//			scale = 1.0f;
//			SCALE_SPEED = -1;
//		}
//
//		start = TimeUtils.nanoTime();
//		for (int i = 0; i < SPRITES; i++) {
//			if (angleInc != 0) sprites3[i].rotate(angleInc); // this is aids
//			if (scale != 1) sprites3[i].setScale(scale); // this is aids
//			sprites3[i].draw(spriteBatch);
//		}
//		draw1 = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//		for (int i = SPRITES; i < SPRITES << 1; i++) {
//			if (angleInc != 0) sprites3[i].rotate(angleInc); // this is aids
//			if (scale != 1) sprites3[i].setScale(scale); // this is aids
//			sprites3[i].draw(spriteBatch);
//		}
//		draw2 = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//// spriteBatch.drawText(font, "Question?", 100, 300, Color.RED);
//// spriteBatch.drawText(font, "and another this is a test", 200, 100, Color.WHITE);
//// spriteBatch.drawText(font, "all hail and another this is a test", 200, 200, Color.WHITE);
//// spriteBatch.drawText(font, "Sprite fps: " + Gdx.graphics.getFramesPerSecond(), 10, 30, Color.RED);
//		drawText = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		start = TimeUtils.nanoTime();
//		spriteBatch.end();
//		end = (TimeUtils.nanoTime() - start) / 1000000000.0f;
//
//		if (TimeUtils.nanoTime() - startTime > 1000000000) {
//			Gdx.app.log("SpriteBatch", "fps: " + frames + ", render calls: " + spriteBatch.renderCalls + ", " + begin + ", " + draw1
//				+ ", " + draw2 + ", " + drawText + ", " + end);
//			frames = 0;
//			startTime = TimeUtils.nanoTime();
//		}
//		frames++;
//	}
//
//	public override void Create () {
//		spriteBatch = new SpriteBatch(1000);
//
//		Pixmap pixmap = new Pixmap(Gdx.files.@internal("data/badlogicsmall.jpg"));
//		texture = new Texture(32, 32, Pixmap.Format.RGB565);
//		texture.setFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
//		texture.draw(pixmap, 0, 0);
//		pixmap.Dispose();
//
//		pixmap = new Pixmap(32, 32, Pixmap.Format.RGBA8888);
//		pixmap.setColor(1, 1, 0, 0.5f);
//		pixmap.fill();
//		texture2 = new Texture(pixmap);
//		pixmap.Dispose();
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
//		Gdx.input.setInputProcessor(this);
//	}
//
//	public override void Resize (int width, int height) {
//		Gdx.app.log("SpriteBatchTest", "resized: " + width + ", " + height);
//	}
//
//	public override bool KeyDown (int keycode) {
//		return false;
//	}
//
//	public override bool KeyTyped (char character) {
//		return false;
//	}
//
//	public override bool KeyUp (int keycode) {
//		return false;
//	}
//
//	public override bool TouchDown (int x, int y, int pointer, int newParam) {
//		return false;
//	}
//
//    public override bool TouchDragged (int x, int y, int pointer) {
//		return false;
//	}
//
//    public override bool TouchUp (int x, int y, int pointer, int button) {
//		renderMethod = (renderMethod + 1) % 2;
//		return false;
//	}
//
//    public override bool MouseMoved (int x, int y) {
//		return false;
//	}
//
//    public override bool Scrolled (float amountX, float amountY) {
//		return false;
//	}
//
//}
