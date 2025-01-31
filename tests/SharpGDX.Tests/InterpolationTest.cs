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
//public class InterpolationTest : GdxTest {
//	Stage stage;
//	private Skin skin;
//	private Table table;
//	List<String> list;
//	String interpolationNames[], selectedInterpolation;
//	private ShapeRenderer renderer;
//	float graphSize, steps, time = 0, duration = 2.5f;
//	Vector2 startPosition = new Vector2(), targetPosition = new Vector2(), position = new Vector2();
//
//	/** resets {@link #startPosition} and {@link #targetPosition} */
//	void resetPositions () {
//		startPosition.set(stage.getWidth() - stage.getWidth() / 5f, stage.getHeight() - stage.getHeight() / 5f);
//		targetPosition.set(startPosition.x, stage.getHeight() / 5f);
//	}
//
//	/** @return the {@link #position} with the {@link #selectedInterpolation interpolation} applied */
//	Vector2 getPosition (float time) {
//		position.set(targetPosition);
//		position.sub(startPosition);
//		position.scl(getInterpolation(selectedInterpolation).apply(time / duration));
//		position.add(startPosition);
//		return position;
//	}
//
//	/** @return the {@link #selectedInterpolation selected} interpolation */
//	private Interpolation getInterpolation (String name) {
//		try {
//			return (Interpolation)ClassReflection.getField(Interpolation.class, name).get(null);
//		} catch (Exception e) {
//			throw new RuntimeException(e);
//		}
//	}
//
//	public override void Create () {
//		GDX.GL.glClearColor(.3f, .3f, .3f, 1);
//		renderer = new ShapeRenderer();
//
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		stage = new Stage(new ScreenViewport());
//		resetPositions();
//
//		Field[] interpolationFields = ClassReflection.getFields(Interpolation.class);
//
//		// see how many fields are actually interpolations (for safety; other fields may be added with future)
//		int interpolationMembers = 0;
//		for (int i = 0; i < interpolationFields.length; i++)
//			if (ClassReflection.isAssignableFrom(Interpolation.class, interpolationFields[i].getDeclaringClass()))
//				interpolationMembers++;
//
//		// get interpolation names
//		interpolationNames = new String[interpolationMembers];
//		for (int i = 0; i < interpolationFields.length; i++)
//			if (ClassReflection.isAssignableFrom(Interpolation.class, interpolationFields[i].getDeclaringClass()))
//				interpolationNames[i] = interpolationFields[i].getName();
//		selectedInterpolation = interpolationNames[0];
//
//		list = new List(skin);
//		list.setItems(interpolationNames);
//		list.addListener(new ChangeListener() {
//			public void changed (ChangeEvent event, Actor actor) {
//				selectedInterpolation = list.getSelected();
//				time = 0;
//				resetPositions();
//			}
//		});
//
//		ScrollPane scroll = new ScrollPane(list, skin);
//		scroll.setFadeScrollBars(false);
//		scroll.setScrollingDisabled(true, false);
//
//		table = new Table();
//		table.setFillParent(true);
//		table.add(scroll).expandX().left().width(100);
//		stage.addActor(table);
//
//		Gdx.input.setInputProcessor(new InputMultiplexer(new InputAdapter() {
//			public boolean scrolled (float amountX, float amountY) {
//				if (!Gdx.input.isKeyPressed(Keys.CONTROL_LEFT)) return false;
//				duration -= amountY / 15f;
//				duration = MathUtils.clamp(duration, 0, Float.POSITIVE_INFINITY);
//				return true;
//			}
//
//		}, stage, new InputAdapter() {
//			public boolean touchDown (int screenX, int screenY, int pointer, int button) {
//				if (!Float.isNaN(time)) // if "walking" was interrupted by this touch down event
//					startPosition.set(getPosition(time)); // set startPosition to the current position
//				targetPosition.set(stage.screenToStageCoordinates(targetPosition.set(screenX, screenY)));
//				time = 0;
//				return true;
//			}
//
//		}));
//	}
//
//	public void render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		float bottomLeftX = Gdx.graphics.getWidth() / 2 - graphSize / 2, bottomLeftY = Gdx.graphics.getHeight() / 2 - graphSize / 2;
//
//		// only show up to two decimals
//		String text = String.valueOf(duration);
//		if (text.length() > 4) text = text.substring(0, text.lastIndexOf('.') + 3);
//		text = "duration: " + text + " s (ctrl + scroll to change)";
//		stage.getBatch().begin();
//		list.getStyle().font.draw(stage.getBatch(), text, bottomLeftX + graphSize / 2,
//			bottomLeftY + graphSize + list.getStyle().font.getLineHeight(), 0, Align.center, false);
//		stage.getBatch().end();
//
//		renderer.begin(ShapeRenderer.ShapeType.Line);
//		renderer.rect(bottomLeftX, bottomLeftY, graphSize, graphSize); // graph bounds
//		float lastX = bottomLeftX, lastY = bottomLeftY;
//		for (float step = 0; step <= steps; step++) {
//			Interpolation interpolation = getInterpolation(selectedInterpolation);
//			float percent = step / steps;
//			float x = bottomLeftX + graphSize * percent, y = bottomLeftY + graphSize * interpolation.apply(percent);
//			renderer.line(lastX, lastY, x, y);
//			lastX = x;
//			lastY = y;
//		}
//		time += Gdx.graphics.getDeltaTime();
//		if (time > duration) {
//			time = Float.NaN; // stop "walking"
//			startPosition.set(targetPosition); // set startPosition to targetPosition for next click
//		}
//		// draw time marker
//		renderer.line(bottomLeftX + graphSize * time / duration, bottomLeftY, bottomLeftX + graphSize * time / duration,
//			bottomLeftY + graphSize);
//		// draw path
//		renderer.setColor(Color.GRAY);
//		renderer.line(startPosition, targetPosition);
//		renderer.setColor(Color.WHITE);
//		renderer.end();
//
//		// draw the position
//		renderer.begin(ShapeRenderer.ShapeType.Filled);
//		if (!Float.isNaN(time)) // don't mess up position if time is NaN
//			getPosition(time);
//		renderer.circle(position.x, position.y, 7);
//		renderer.end();
//
//		stage.act();
//		stage.draw();
//	}
//
//	public void resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//		table.invalidateHierarchy();
//
//		renderer.setProjectionMatrix(stage.getViewport().getCamera().combined);
//
//		graphSize = 0.75f * Math.min(stage.getViewport().getWorldWidth(), stage.getViewport().getWorldHeight());
//		steps = graphSize * 0.5f;
//	}
//
//	public void dispose () {
//		stage.Dispose();
//		skin.Dispose();
//	}
//}
