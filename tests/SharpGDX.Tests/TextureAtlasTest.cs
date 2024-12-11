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
//public class TextureAtlasTest : GdxTest {
//	SpriteBatch batch;
//	Sprite badlogic, badlogicSmall, star;
//	TextureAtlas atlas;
//	TextureAtlas jumpAtlas;
//	Animation<TextureRegion> jumpAnimation;
//	BitmapFont font;
//	float time = 0;
//	ShapeRenderer renderer;
//
//	public void create () {
//		batch = new SpriteBatch();
//		renderer = new ShapeRenderer();
//
//		atlas = new TextureAtlas(Gdx.files.@internal("data/pack.atlas"));
//		jumpAtlas = new TextureAtlas(Gdx.files.@internal("data/jump.txt"));
//
//		jumpAnimation = new Animation<TextureRegion>(0.25f, jumpAtlas.findRegions("ALIEN_JUMP_"));
//
//		badlogic = atlas.createSprite("badlogicslice");
//		badlogic.setPosition(50, 50);
//
//		// badlogicSmall = atlas.createSprite("badlogicsmall");
//		badlogicSmall = atlas.createSprite("badlogicsmall-rotated");
//		badlogicSmall.setPosition(10, 10);
//
//		TextureAtlas.AtlasRegion region = atlas.findRegion("badlogicsmall");
//		Console.WriteLine("badlogicSmall original size: " + region.originalWidth + ", " + region.originalHeight);
//		Console.WriteLine("badlogicSmall packed size: " + region.packedWidth + ", " + region.packedHeight);
//
//		star = atlas.createSprite("particle-star");
//		star.setPosition(10, 70);
//
//		font = new BitmapFont(Gdx.files.@internal("data/font.fnt"), atlas.findRegion("font"), false);
//
//		GDX.GL.glClearColor(0, 1, 0, 1);
//
//		Gdx.input.setInputProcessor(new InputAdapter() {
//			public boolean keyUp (int keycode) {
//				if (keycode == Keys.UP) {
//					badlogicSmall.flip(false, true);
//				} else if (keycode == Input.Keys.RIGHT) {
//					badlogicSmall.flip(true, false);
//				} else if (keycode == Input.Keys.LEFT) {
//					badlogicSmall.setSize(512, 512);
//				} else if (keycode == Input.Keys.DOWN) {
//					badlogicSmall.rotate90(true);
//				}
//				return base.KeyUp(keycode);
//			}
//		});
//	}
//
//	public void render () {
//		time += Gdx.graphics.getDeltaTime();
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		renderer.begin(ShapeRenderer.ShapeType.Line);
//		renderer.rect(10, 10, 256, 256);
//		renderer.end();
//
//		batch.begin();
//		// badlogic.draw(batch);
//		// star.draw(batch);
//		// font.draw(batch, "This font was packed!", 26, 65);
//		badlogicSmall.draw(batch);
//		// batch.draw(jumpAnimation.getKeyFrame(time, true), 100, 100);
//		batch.end();
//	}
//
//	public void dispose () {
//		atlas.Dispose();
//		jumpAtlas.Dispose();
//		batch.Dispose();
//		font.Dispose();
//	}
//}
