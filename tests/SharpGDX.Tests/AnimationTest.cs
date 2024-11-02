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
//public class AnimationTest : GdxTest {
//
//	class Caveman {
//		static readonly float VELOCITY = 20;
//		public readonly Vector2 pos;
//		public readonly bool headsLeft;
//		public float stateTime;
//
//		public Caveman (float x, float y, bool headsLeft) {
//			pos = new Vector2().set(x, y);
//			this.headsLeft = headsLeft;
//			this.stateTime = (float)Math.random();
//		}
//
//		public void update (float deltaTime) {
//			stateTime += deltaTime;
//			pos.x = pos.x + (headsLeft ? -VELOCITY * deltaTime : VELOCITY * deltaTime);
//			if (pos.x < -64) pos.x = Gdx.graphics.getWidth();
//			if (pos.x > Gdx.graphics.getWidth() + 64) pos.x = -64;
//		}
//	}
//
//	Animation<TextureRegion> leftWalk;
//	Animation<TextureRegion> rightWalk;
//	Caveman[] cavemen;
//	Texture texture;
//	SpriteBatch batch;
//	FPSLogger fpsLog;
//
//	public override void Create () {
//		texture = new Texture(Gdx.files.@internal("data/walkanim.png"));
//		TextureRegion[] leftWalkFrames = TextureRegion.split(texture, 64, 64)[0];
//		Array<TextureRegion> rightWalkFrames = new Array(TextureRegion.class);
//		for (int i = 0; i < leftWalkFrames.length; i++) {
//			TextureRegion frame = new TextureRegion(leftWalkFrames[i]);
//			frame.flip(true, false);
//			rightWalkFrames.add(frame);
//		}
//		leftWalk = new Animation<TextureRegion>(0.25f, leftWalkFrames);
//		rightWalk = new Animation<TextureRegion>(0.25f, rightWalkFrames);
//
//		TextureRegion[] rightRegions = rightWalk.getKeyFrames(); // testing backing array type
//		TextureRegion firstRightRegion = rightRegions[0];
//		Gdx.app.log("AnimationTest",
//			"First right walk region is " + firstRightRegion.getRegionWidth() + "x" + firstRightRegion.getRegionHeight());
//
//		cavemen = new Caveman[100];
//		for (int i = 0; i < 100; i++) {
//			cavemen[i] = new Caveman((float)Math.random() * Gdx.graphics.getWidth(), (float)Math.random() * Gdx.graphics.getHeight(),
//				Math.random() > 0.5 ? true : false);
//		}
//		batch = new SpriteBatch();
//		fpsLog = new FPSLogger();
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.1f, 0f, 0.25f, 1f);
//		batch.begin();
//		for (int i = 0; i < cavemen.length; i++) {
//			Caveman caveman = cavemen[i];
//			TextureRegion frame = caveman.headsLeft ? leftWalk.getKeyFrame(caveman.stateTime, true)
//				: rightWalk.getKeyFrame(caveman.stateTime, true);
//			batch.draw(frame, caveman.pos.x, caveman.pos.y);
//		}
//		batch.end();
//
//		for (int i = 0; i < cavemen.length; i++) {
//			cavemen[i].update(Gdx.graphics.getDeltaTime());
//		}
//
//		fpsLog.log();
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		texture.Dispose();
//	}
//}
