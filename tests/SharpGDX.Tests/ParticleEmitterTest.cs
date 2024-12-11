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
//public class ParticleEmitterTest : GdxTest {
//	private SpriteBatch spriteBatch;
//	ParticleEffect effect;
//	int emitterIndex;
//	Array<ParticleEmitter> emitters;
//	int particleCount = 10;
//	float fpsCounter;
//	IInputProcessor inputProcessor;
//
//	public override void Create () {
//		spriteBatch = new SpriteBatch();
//
//		effect = new ParticleEffect();
//		effect.load(Gdx.files.@internal("data/test.p"), Gdx.files.@internal("data"));
//		effect.setPosition(Gdx.graphics.getWidth() / 2, Gdx.graphics.getHeight() / 2);
//		// Of course, a ParticleEffect is normally just used, without messing around with its emitters.
//		emitters = new Array(effect.getEmitters());
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
//				// effect.setPosition(x, Gdx.graphics.getHeight() - y);
//				ParticleEmitter emitter = emitters.get(emitterIndex);
//				particleCount += 100;
//				Console.WriteLine(particleCount);
//				particleCount = Math.max(0, particleCount);
//				if (particleCount > emitter.getMaxParticleCount()) emitter.setMaxParticleCount(particleCount * 2);
//				emitter.getEmission().setHigh(particleCount / emitter.getLife().getHighMax() * 1000);
//				effect.getEmitters().clear();
//				effect.getEmitters().add(emitter);
//				return false;
//			}
//
//			public bool keyDown (int keycode) {
//				ParticleEmitter emitter = emitters.get(emitterIndex);
//				if (keycode == Input.Keys.DPAD_UP)
//					particleCount += 5;
//				else if (keycode == Input.Keys.PLUS) {
//					emitter = new ParticleEmitter(emitter);
//				} else if (keycode == Input.Keys.DPAD_DOWN)
//					particleCount -= 5;
//				else if (keycode == Input.Keys.SPACE) {
//					emitterIndex = (emitterIndex + 1) % emitters.size;
//					emitter = emitters.get(emitterIndex);
//
//					// if we've previously stopped the emitter reset it
//					if (emitter.isComplete()) emitter.reset();
//					particleCount = (int)(emitter.getEmission().getHighMax() * emitter.getLife().getHighMax() / 1000f);
//				} else if (keycode == Input.Keys.ENTER) {
//					emitter = emitters.get(emitterIndex);
//					if (emitter.isComplete())
//						emitter.reset();
//					else
//						emitter.allowCompletion();
//				} else
//					return false;
//				particleCount = Math.max(0, particleCount);
//				if (particleCount > emitter.getMaxParticleCount()) emitter.setMaxParticleCount(particleCount * 2);
//				emitter.getEmission().setHigh(particleCount / emitter.getLife().getHighMax() * 1000);
//				effect.getEmitters().clear();
//				effect.getEmitters().add(emitter);
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
//	}
//
//	public void render () {
//		spriteBatch.getProjectionMatrix().setToOrtho2D(0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		float delta = Gdx.graphics.getDeltaTime();
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		spriteBatch.begin();
//		effect.draw(spriteBatch, delta);
//		spriteBatch.end();
//		fpsCounter += delta;
//		if (fpsCounter > 3) {
//			fpsCounter = 0;
//			int activeCount = emitters.get(emitterIndex).getActiveCount();
//			Gdx.app.log("libGDX", activeCount + "/" + particleCount + " particles, FPS: " + Gdx.graphics.getFramesPerSecond());
//		}
//	}
//
//	public bool needsGL20 () {
//		return false;
//	}
//}
