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
//public class VibratorTest : GdxTest {
//
//	Stage stage;
//	SpriteBatch batch;
//	Skin skin;
//
//	public override void Create () {
//		batch = new SpriteBatch();
//		stage = new Stage();
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		Gdx.input.setInputProcessor(stage);
//
//		// Create a table that fills the screen. Everything else will go inside this table.
//		Table table = new Table();
//		table.setFillParent(true);
//		stage.addActor(table);
//
//		final CheckBox fallbackCheckbox = new CheckBox("Fallback", skin);
//		final Button button = getButton("Vibrate");
//		button.addListener(new ChangeListener() {
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				Gdx.input.vibrate(50);
//			}
//		});
//		final Button buttonVibrateAmplitude = getButton("Vibrate \n Amplitude \n Random");
//		buttonVibrateAmplitude.addListener(new ChangeListener() {
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				int randomLength = MathUtils.random(10, 200);
//				int randomAmplitude = MathUtils.random(0, 255);
//				Gdx.input.vibrate(randomLength, randomAmplitude, fallbackCheckbox.isChecked());
//				Gdx.app.log("VibratorTest", "Length: " + randomLength + "ms, Amplitude: " + randomAmplitude);
//			}
//		});
//		final Button buttonVibrateType = getButton("Vibrate \n Type \n Random");
//		buttonVibrateType.addListener(new ChangeListener() {
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				Input.VibrationType vibrationType = Input.VibrationType.values()[MathUtils.random(0,
//					Input.VibrationType.values().length - 1)];
//				Gdx.input.vibrate(vibrationType);
//				Gdx.app.log("VibratorTest", "VibrationType: " + vibrationType.name());
//			}
//		});
//
//		table.defaults().pad(20f);
//		table.add(button).size(120f);
//		table.add(buttonVibrateAmplitude).size(120f);
//		table.add(buttonVibrateType).size(120f);
//		table.row();
//		table.add(fallbackCheckbox).colspan(3).height(120f);
//
//	}
//
//	private Button getButton (String text) {
//		final Button button = new Button(skin);
//		Label label = new Label(text, skin);
//		button.add(label);
//		return button;
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		stage.act(Math.min(Gdx.graphics.getDeltaTime(), 1 / 30f));
//		stage.draw();
//	}
//
//	public override void Resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		skin.Dispose();
//	}
//}
