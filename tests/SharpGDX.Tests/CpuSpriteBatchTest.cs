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
//public class CpuSpriteBatchTest : GdxTest {
//
//	private static class DrawableActor : Actor {
//		private final TransformDrawable drawable;
//
//		public DrawableActor (TransformDrawable drawable) {
//			this.drawable = drawable;
//			setSize(drawable.getMinWidth(), drawable.getMinHeight());
//		}
//
//		public void draw (Batch batch, float parentAlpha) {
//			Color color = getColor();
//			batch.setColor(color.r, color.g, color.b, parentAlpha);
//			drawable.draw(batch, getX(), getY(), getOriginX(), getOriginY(), getWidth(), getHeight(), getScaleX(), getScaleY(),
//				getRotation());
//		}
//	}
//
//	private static final int NUM_GROUPS = 1000;
//
//	private Stage stage;
//	private Texture texture;
//
//	private long sampleStartTime;
//	private long sampleFrames;
//
//	public void create () {
//		Batch batch = new CpuSpriteBatch();
//		// batch = new SpriteBatch();
//
//		stage = new Stage(new ExtendViewport(500, 500), batch);
//
//		Gdx.input.setInputProcessor(stage);
//
//		texture = new Texture("data/bobargb8888-32x32.png");
//		texture.setFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
//
//		TextureRegionDrawable drawable = new TextureRegionDrawable(new TextureRegion(texture));
//
//		for (int i = 0; i < NUM_GROUPS; i++) {
//			Group group = createActorGroup(drawable);
//			stage.addActor(group);
//		}
//	}
//
//	private Group createActorGroup (TextureRegionDrawable bob) {
//		Actor main = new DrawableActor(bob);
//		main.setPosition(0, 0, Align.center);
//
//		Actor hat = new DrawableActor(bob) {
//			@Override
//			public void act (float delta) {
//				rotateBy(delta * -300);
//			}
//		};
//		hat.setOrigin(Align.center);
//		hat.setScale(0.5f);
//		hat.setPosition(0, 21, Align.center);
//
//		Group group = new Group() {
//			@Override
//			public void act (float delta) {
//				rotateBy(delta * 120);
//				setScale(0.9f + 0.2f * MathUtils.cos(MathUtils.degreesToRadians * getRotation()));
//				super.act(delta);
//			}
//		};
//		group.addActor(main);
//		group.addActor(hat);
//		// group.setTransform(false);
//
//		float margin = 35;
//		float x = MathUtils.random(margin, stage.getWidth() - margin);
//		float y = MathUtils.random(margin, stage.getHeight() - margin);
//		group.setPosition(x, y);
//		group.setRotation(MathUtils.random(0, 360));
//
//		return group;
//	}
//
//	public void render () {
//		ScreenUtils.clear(0.5f, 0.5f, 0.5f, 1);
//
//		stage.act(Gdx.graphics.getDeltaTime());
//		stage.draw();
//
//		long now = TimeUtils.nanoTime();
//		sampleFrames++;
//
//		if (now - sampleStartTime > 1000000000) {
//			if (sampleStartTime != 0) {
//				int renderCalls = ((SpriteBatch)stage.getBatch()).renderCalls;
//				Gdx.app.log("CpuSpriteBatch", "FPS: " + sampleFrames + ", render calls: " + renderCalls);
//			}
//			sampleStartTime = now;
//			sampleFrames = 0;
//		}
//	}
//
//	public void resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//	}
//
//	public void dispose () {
//		stage.Dispose();
//		texture.Dispose();
//	}
//}
