//using SharpGDX.Tests.Utils;
//using SharpGDX.Input;
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
//public class ParticleEmitterChangeSpriteTest : GdxTest {
//	private SpriteBatch spriteBatch;
//	ParticleEffect effect;
//	int emitterIndex = 0;
//	Array<ParticleEmitter> emitters;
//	TextureAtlas atlas;
//	Array<Sprite> sprites;
//	int currentSprite = 0;
//	float fpsCounter;
//	IInputProcessor inputProcessor;
//
//	public override void Create () {
//		spriteBatch = new SpriteBatch();
//
//		atlas = new TextureAtlas("data/particles.atlas");
//
//		int spriteCount = atlas.getRegions().size;
//		sprites = new Array<Sprite>(spriteCount);
//		foreach (TextureRegion region in atlas.getRegions()) {
//			sprites.add(new Sprite(region));
//		}
//
//		effect = new ParticleEffect();
//		effect.load(Gdx.files.@internal("data/test.p"), Gdx.files.@internal("data"));
//		effect.setPosition(Gdx.graphics.getWidth() / 2, Gdx.graphics.getHeight() / 2);
//		// Of course, a ParticleEffect is normally just used, without messing around with its emitters.
//		emitters = new (effect.getEmitters());
//		effect.getEmitters().clear();
//		effect.getEmitters().add(emitters.get(0));
//
//		inputProcessor = new InputAdapter() {
//
//			public boolean touchDragged (int x, int y, int pointer) {
//				effect.setPosition(x, Gdx.graphics.getHeight() - y);
//				return false;
//			}
//
//			public bool touchDown (int x, int y, int pointer, int newParam) {
//				ParticleEmitter emitter = emitters.get(emitterIndex);
//				currentSprite = (currentSprite + 1) % sprites.size;
//				emitter.setSprites(new Array<Sprite>(new Sprite[] {sprites.get(currentSprite)}));
//				return false;
//			}
//		};
//
//		Gdx.input.setInputProcessor(inputProcessor);
//	}
//
//	public override void Dispose () {
//		spriteBatch.Dispose();
//		effect.Dispose();
//		atlas.Dispose();
//	}
//
//	public void render () {
//		spriteBatch.getProjectionMatrix().setToOrtho2D(0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		float delta = Gdx.graphics.getDeltaTime();
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		spriteBatch.begin();
//		effect.draw(spriteBatch, delta);
//		spriteBatch.end();
//		fpsCounter += delta;
//		if (fpsCounter > 3) {
//			fpsCounter = 0;
//			Gdx.app.log("libgdx", "current sprite: " + currentSprite + ", FPS: " + Gdx.graphics.getFramesPerSecond());
//		}
//	}
//
//	public bool needsGL20 () {
//		return false;
//	}
//}
