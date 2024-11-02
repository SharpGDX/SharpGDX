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
///** This tests both {@link Actor#parentToLocalCoordinates(Vector2)} and {@link Actor#localToParentCoordinates(Vector2)}. */
//public class GroupTest : GdxTest {
//	Stage stage;
//	SpriteBatch batch;
//	BitmapFont font;
//	ShapeRenderer renderer;
//	TextureRegion region;
//	TestGroup group1;
//	TestGroup group2;
//	HorizontalGroup horiz, horizWrap;
//	VerticalGroup vert, vertWrap;
//
//	public void create () {
//		batch = new SpriteBatch();
//		font = new BitmapFont();
//		renderer = new ShapeRenderer();
//
//		stage = new Stage(new ScreenViewport());
//		Gdx.input.setInputProcessor(stage);
//
//		region = new TextureRegion(new Texture(Gdx.files.@internal("data/group-debug.png")));
//
//		group2 = new TestGroup("group2");
//		group2.setTransform(true);
//		stage.addActor(group2);
//
//		group1 = new TestGroup("group1");
//		group1.setTransform(true);
//		group2.addActor(group1);
//
//		LabelStyle style = new LabelStyle();
//		style.font = new BitmapFont();
//
//		Texture texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"));
//
//		horiz = new HorizontalGroup().pad(10, 20, 30, 40).top().space(5).reverse();
//		for (int i = 1; i <= 15; i++) {
//			horiz.addActor(new Label(i + ",", style));
//			if (i == 7) horiz.addActor(new Container(new Image(texture)).size(10));
//		}
//		horiz.addActor(new Container(new Image(texture)).fill().prefSize(30));
//		horiz.debug();
//		horiz.setPosition(10, 10);
//		horiz.pack();
//		stage.addActor(horiz);
//
//		horizWrap = new HorizontalGroup().wrap().pad(10, 20, 30, 40).right().rowBottom().space(5).wrapSpace(15).reverse();
//		for (int i = 1; i <= 15; i++) {
//			horizWrap.addActor(new Label(i + ",", style));
//			if (i == 7) horizWrap.addActor(new Container(new Image(texture)).prefSize(10).fill());
//		}
//		horizWrap.addActor(new Container(new Image(texture)).prefSize(30));
//		horizWrap.debug();
//		horizWrap.setBounds(10, 85, 150, 40);
//		stage.addActor(horizWrap);
//
//		vert = new VerticalGroup().pad(10, 20, 30, 40).top().space(5).reverse();
//		for (int i = 1; i <= 8; i++) {
//			vert.addActor(new Label(i + ",", style));
//			if (i == 4) vert.addActor(new Container(new Image(texture)).size(10));
//		}
//		vert.addActor(new Container(new Image(texture)).size(30));
//		vert.debug();
//		vert.setPosition(515, 10);
//		vert.pack();
//		stage.addActor(vert);
//
//		vertWrap = new VerticalGroup().wrap().pad(10, 20, 30, 40).bottom().columnRight().space(5).wrapSpace(15).reverse();
//		for (int i = 1; i <= 8; i++) {
//			vertWrap.addActor(new Label(i + ",", style));
//			if (i == 4) vertWrap.addActor(new Container(new Image(texture)).prefSize(10).fill());
//		}
//		vertWrap.addActor(new Container(new Image(texture)).prefSize(30));
//		vertWrap.debug();
//		vertWrap.setBounds(610, 10, 150, 40);
//		stage.addActor(vertWrap);
//	}
//
//	public void render () {
//
//		horiz.setVisible(true);
//		horiz.setWidth(Gdx.input.getX() - horiz.getX());
//		// horiz.setWidth(200);
//		horiz.setHeight(100);
//		horiz.fill();
//		horiz.expand();
//		horiz.invalidate();
//
//		horizWrap.setVisible(true);
//		horizWrap.fill();
//		horizWrap.expand();
//		horizWrap.setWidth(Gdx.input.getX() - horizWrap.getX());
//		// horizWrap.setHeight(horizWrap.getPrefHeight());
//		horizWrap.setHeight(200);
//
//		vert.setHeight(Gdx.graphics.getHeight() - Gdx.input.getY() - vert.getY());
//		// vert.setWidth(200);
//		vertWrap.setHeight(Gdx.graphics.getHeight() - Gdx.input.getY() - vertWrap.getY());
//// vertWrap.setWidth(vertWrap.getPrefWidth());
//		vertWrap.setWidth(200);
//
//		// Vary the transforms to exercise the different code paths.
//		group2.setBounds(150, 150, 150, 150);
//		group2.setRotation(45);
//		group2.setOrigin(150, 150);
//		group2.setScale(1.25f);
//
//		group1.setBounds(150, 150, 50, 50);
//		group1.setRotation(45);
//		group1.setOrigin(25, 25);
//		group1.setScale(1.3f);
//
//		ScreenUtils.clear(0, 0, 0, 1);
//		stage.draw();
//
//		renderer.setProjectionMatrix(batch.getProjectionMatrix());
//		renderer.begin(ShapeRenderer.ShapeType.Filled);
//		if (MathUtils.randomBoolean()) { // So we see when they are drawn on top of each other (which should be always).
//			renderer.setColor(Color.GREEN);
//			renderer.circle(group1.toScreenCoordinates.x, Gdx.graphics.getHeight() - group1.toScreenCoordinates.y, 5);
//			renderer.setColor(Color.RED);
//			renderer.circle(group1.localToParentCoordinates.x, Gdx.graphics.getHeight() - group1.localToParentCoordinates.y, 5);
//		} else {
//			renderer.setColor(Color.RED);
//			renderer.circle(group1.localToParentCoordinates.x, Gdx.graphics.getHeight() - group1.localToParentCoordinates.y, 5);
//			renderer.setColor(Color.GREEN);
//			renderer.circle(group1.toScreenCoordinates.x, Gdx.graphics.getHeight() - group1.toScreenCoordinates.y, 5);
//		}
//		renderer.end();
//	}
//
//	public void resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//	}
//
//	public boolean needsGL20 () {
//		return false;
//	}
//
//	class TestGroup : Group {
//		private String name;
//		Vector2 toScreenCoordinates = new Vector2();
//		Vector2 localToParentCoordinates = new Vector2();
//		float testX = 25;
//		float testY = 25;
//
//		public TestGroup (String name) {
//			this.name = name;
//
//			addListener(new InputListener() {
//				public boolean mouseMoved (InputEvent event, float x, float y) {
//					// These come from Actor#parentToLocalCoordinates.
//					testX = x;
//					testY = y;
//					return true;
//				}
//			});
//		}
//
//		public void draw (Batch batch, float parentAlpha) {
//			// Use Stage#toScreenCoordinates, which we know is correct.
//			toScreenCoordinates.set(testX, testY).sub(getOriginX(), getOriginY()).scl(getScaleX(), getScaleY())
//				.rotateDeg(getRotation()).add(getOriginX(), getOriginY()).add(getX(), getY());
//			getStage().toScreenCoordinates(toScreenCoordinates, batch.getTransformMatrix());
//
//			// Do the same as toScreenCoordinates via Actor#localToParentCoordinates.
//			localToAscendantCoordinates(null, localToParentCoordinates.set(testX, testY));
//			getStage().stageToScreenCoordinates(localToParentCoordinates);
//
//			// Console.WriteLine(name + " " + toScreenCoordinates + " " + localToParentCoordinates);
//
//			batch.setColor(getColor());
//			batch.draw(region, getX(), getY(), getOriginX(), getOriginY(), getWidth(), getHeight(), getScaleX(), getScaleY(),
//				getRotation());
//			super.draw(batch, parentAlpha);
//		}
//	}
//}
