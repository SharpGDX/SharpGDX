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
//public class StageTest : GdxTest , Input.IInputProcessor {
//	private static readonly int NUM_GROUPS = 4;
//	private static readonly int NUM_SPRITES = (int)Math.Sqrt(150 / NUM_GROUPS);
//	private static readonly float SPACING = 5;
//	ShapeRenderer renderer;
//	Stage stage;
//	Stage ui;
//	Texture texture;
//	Texture uiTexture;
//	BitmapFont font;
//
//	bool rotateSprites = false;
//	bool scaleSprites = false;
//	float angle;
//	Array<Actor> sprites = new ();
//	float scale = 1;
//	float vScale = 1;
//	Label fps;
//
//	public override void Create () {
//		texture = new Texture(Gdx.files.@internal("data/badlogicsmall.jpg"));
//		texture.setFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
//		font = new BitmapFont(Gdx.files.@internal("data/lsans-15.fnt"), false);
//
//		stage = new Stage(new ScreenViewport());
//
//		float loc = (NUM_SPRITES * (32 + SPACING) - SPACING) / 2;
//		for (int i = 0; i < NUM_GROUPS; i++) {
//			Group group = new Group();
//			group.setX((float)Math.random() * (stage.getWidth() - NUM_SPRITES * (32 + SPACING)));
//			group.setY((float)Math.random() * (stage.getHeight() - NUM_SPRITES * (32 + SPACING)));
//			group.setOrigin(loc, loc);
//
//			fillGroup(group, texture);
//			stage.addActor(group);
//		}
//
//		uiTexture = new Texture(Gdx.files.@internal("data/ui.png"));
//		uiTexture.setFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
//		ui = new Stage(new ScreenViewport());
//
//		Image blend = new Image(new TextureRegion(uiTexture, 0, 0, 64, 32));
//		blend.setAlign(Align.center);
//		blend.setScaling(Scaling.none);
//		blend.addListener(new InputListener() {
//			public boolean touchDown (InputEvent event, float x, float y, int pointer, int button) {
//				if (stage.getBatch().isBlendingEnabled())
//					stage.getBatch().disableBlending();
//				else
//					stage.getBatch().enableBlending();
//				return true;
//			}
//		});
//		blend.setY(ui.getHeight() - 64);
//
//		Image rotate = new Image(new TextureRegion(uiTexture, 64, 0, 64, 32));
//		rotate.setAlign(Align.center);
//		rotate.setScaling(Scaling.none);
//		rotate.addListener(new InputListener() {
//			public boolean touchDown (InputEvent event, float x, float y, int pointer, int button) {
//				rotateSprites = !rotateSprites;
//				return true;
//			}
//		});
//		rotate.setPosition(64, blend.getY());
//
//		Image scale = new Image(new TextureRegion(uiTexture, 64, 32, 64, 32));
//		scale.setAlign(Align.center);
//		scale.setScaling(Scaling.none);
//		scale.addListener(new InputListener() {
//			public boolean touchDown (InputEvent event, float x, float y, int pointer, int button) {
//				scaleSprites = !scaleSprites;
//				return true;
//			}
//		});
//		scale.setPosition(128, blend.getY());
//
//		{
//			Actor shapeActor = new Actor() {
//				public void drawDebug (ShapeRenderer shapes) {
//					shapes.set(ShapeRenderer.ShapeType.Filled);
//					shapes.setColor(getColor());
//					shapes.rect(getX(), getY(), getOriginX(), getOriginY(), getWidth(), getHeight(), getScaleX(), getScaleY(),
//						getRotation());
//				}
//			};
//			shapeActor.setBounds(0, 0, 100, 150);
//			shapeActor.setOrigin(50, 75);
//			shapeActor.debug();
//			sprites.add(shapeActor);
//
//			Group shapeGroup = new Group();
//			shapeGroup.setBounds(300, 300, 300, 300);
//			shapeGroup.setOrigin(50, 75);
//			shapeGroup.setTouchable(Touchable.childrenOnly);
//			shapeGroup.addActor(shapeActor);
//			stage.addActor(shapeGroup);
//		}
//
//		ui.addActor(blend);
//		ui.addActor(rotate);
//		ui.addActor(scale);
//
//		fps = new Label("fps: 0", new Label.LabelStyle(font, Color.WHITE));
//		fps.setPosition(10, 30);
//		fps.setColor(0, 1, 0, 1);
//		ui.addActor(fps);
//
//		renderer = new ShapeRenderer();
//		Gdx.input.setInputProcessor(this);
//	}
//
//	private void fillGroup (Group group, Texture texture) {
//		float advance = 32 + SPACING;
//		for (int y = 0; y < NUM_SPRITES * advance; y += advance)
//			for (int x = 0; x < NUM_SPRITES * advance; x += advance) {
//				Image img = new Image(new TextureRegion(texture));
//				img.setAlign(Align.center);
//				img.setScaling(Scaling.none);
//				img.setBounds(x, y, 32, 32);
//				img.setOrigin(16, 16);
//				group.addActor(img);
//				sprites.add(img);
//			}
//	}
//
//	private readonly Vector2 stageCoords = new Vector2();
//
//	public override void Render () {
//		GDX.GL.glViewport(0, 0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getBackBufferHeight());
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//
//		if (Gdx.input.isTouched()) {
//			stage.screenToStageCoordinates(stageCoords.set(Gdx.input.getX(), Gdx.input.getY()));
//			Actor actor = stage.hit(stageCoords.x, stageCoords.y, true);
//			if (actor != null)
//				actor.setColor((float)Math.random(), (float)Math.random(), (float)Math.random(), 0.5f + 0.5f * (float)Math.random());
//		}
//
//		Array<Actor> actors = stage.getActors();
//		int len = actors.size;
//		if (rotateSprites) {
//			for (int i = 0; i < len; i++)
//				actors.get(i).rotateBy(Gdx.graphics.getDeltaTime() * 10);
//		}
//
//		scale += vScale * Gdx.graphics.getDeltaTime();
//		if (scale > 1) {
//			scale = 1;
//			vScale = -vScale;
//		}
//		if (scale < 0.5f) {
//			scale = 0.5f;
//			vScale = -vScale;
//		}
//
//		len = sprites.size;
//		for (int i = 0; i < len; i++) {
//			Actor sprite = sprites.get(i);
//			if (rotateSprites)
//				sprite.rotateBy(-40 * Gdx.graphics.getDeltaTime());
//			else
//				sprite.setRotation(0);
//
//			if (scaleSprites) {
//				sprite.setScale(scale);
//			} else {
//				sprite.setScale(1);
//			}
//		}
//
//		stage.draw();
//
//		renderer.begin(ShapeRenderer.ShapeType.Point);
//		renderer.setColor(1, 0, 0, 1);
//		len = actors.size;
//		for (int i = 0; i < len; i++) {
//			Group group = (Group)actors.get(i);
//			renderer.point(group.getX() + group.getOriginX(), group.getY() + group.getOriginY(), 0);
//		}
//		renderer.end();
//
//		fps.setText("fps: " + Gdx.graphics.getFramesPerSecond() + ", actors " + sprites.size + ", groups " + sprites.size);
//		ui.draw();
//	}
//
//	public override bool TouchDown (int x, int y, int pointer, int button) {
//		return ui.touchDown(x, y, pointer, button);
//	}
//
//	public override void Resize (int width, int height) {
//		ui.getViewport().update(width, height, true);
//		stage.getViewport().update(width, height, true);
//	}
//
//	public override void Dispose () {
//		ui.Dispose();
//		renderer.Dispose();
//		texture.Dispose();
//		uiTexture.Dispose();
//		font.Dispose();
//	}
//}
