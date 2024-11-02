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
//public class AudioChangeDeviceTest : GdxTest {
//
//	private Stage stage;
//	private Skin skin;
//	private Sound sound;
//
//	public override void Create () {
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		final SelectBox<String> selectBox = new SelectBox<>(skin);
//		List<String> tmp = new ArrayList<>(Arrays.asList(Gdx.audio.getAvailableOutputDevices()));
//		tmp.add(0, "Auto");
//		selectBox.setItems(tmp.toArray(new String[0]));
//		sound = Gdx.audio.newSound(Gdx.files.@internal("data").child("bubblepop-stereo-left-only.wav"));
//		sound.loop();
//		selectBox.addListener(new ChangeListener() {
//
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				if (selectBox.getSelected().equals("Auto")) {
//					Gdx.app.getAudio().switchOutputDevice(null);
//					return;
//				}
//				Gdx.app.getAudio().switchOutputDevice(selectBox.getSelected());
//			}
//		});
//		selectBox.setWidth(200);
//		selectBox.setPosition(200, 200);
//
//		stage.addActor(selectBox);
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		stage.act(Gdx.graphics.getDeltaTime());
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
//		sound.stop();
//		sound.Dispose();
//	}
//}
