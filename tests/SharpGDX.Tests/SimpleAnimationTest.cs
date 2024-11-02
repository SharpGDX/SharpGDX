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
//public class SimpleAnimationTest : GdxTest {
//	private Animation<TextureRegion> currentWalk;
//	private float currentFrameTime;
//	private Vector2 position;
//
//	private Texture texture;
//
//	private Animation<TextureRegion> downWalk;
//	private Animation<TextureRegion> leftWalk;
//	private Animation<TextureRegion> rightWalk;
//	private Animation<TextureRegion> upWalk;
//
//	private SpriteBatch spriteBatch;
//
//	private static readonly float ANIMATION_SPEED = 0.2f;
//
//	public override void Create () {
//		Gdx.input.setInputProcessor(this);
//		texture = new Texture(Gdx.files.@internal("data/animation.png"));
//		TextureRegion[][] regions = TextureRegion.split(texture, 32, 48);
//		TextureRegion[] downWalkReg = regions[0];
//		TextureRegion[] leftWalkReg = regions[1];
//		TextureRegion[] rightWalkReg = regions[2];
//		TextureRegion[] upWalkReg = regions[3];
//		downWalk = new Animation<TextureRegion>(ANIMATION_SPEED, downWalkReg);
//		leftWalk = new Animation<TextureRegion>(ANIMATION_SPEED, leftWalkReg);
//		rightWalk = new Animation<TextureRegion>(ANIMATION_SPEED, rightWalkReg);
//		upWalk = new Animation<TextureRegion>(ANIMATION_SPEED, upWalkReg);
//
//		currentWalk = leftWalk;
//		currentFrameTime = 0.0f;
//
//		spriteBatch = new SpriteBatch();
//		position = new Vector2();
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		currentFrameTime += Gdx.graphics.getDeltaTime();
//
//		spriteBatch.begin();
//		TextureRegion frame = currentWalk.getKeyFrame(currentFrameTime, true);
//		spriteBatch.draw(frame, position.x, position.y);
//		spriteBatch.end();
//	}
//
//	public  override bool TouchDown (int x, int y, int pointer, int button) {
//		position.x = x;
//		position.y = Gdx.graphics.getHeight() - y;
//		return true;
//	}
//
//	public override void Dispose () {
//		spriteBatch.Dispose();
//		texture.Dispose();
//	}
//}
