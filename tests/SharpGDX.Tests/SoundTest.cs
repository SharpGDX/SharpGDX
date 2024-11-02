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
//using SharpGDX.Audio;
//
//namespace SharpGDX.Tests;
//
//public class SoundTest : GdxTest {
//
//	private static readonly String[] FILENAMES = {"shotgun.ogg", "shotgun-8bit.wav", "shotgun-32float.wav", "shotgun-64float.wav",
//		"quadraphonic.ogg", "quadraphonic.wav", "bubblepop.ogg", "bubblepop-stereo-left-only.wav"};
//
//	ISound sound;
//	float volume = 0.5f;
//	long soundId = 0;
//	Stage ui;
//	Skin skin;
//
//	public override void Create () {
//
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		ui = new Stage(new FitViewport(640, 400));
//		SelectBox<String> soundSelector = new SelectBox<String>(skin);
//		soundSelector.setItems(FILENAMES);
//		setSound(soundSelector.getSelected());
//
//		TextButton play = new TextButton("Play", skin);
//		TextButton stop = new TextButton("Stop", skin);
//		TextButton loop = new TextButton("Loop", skin);
//		Slider pitch = new Slider(0.1f, 4, 0.1f, false, skin);
//		pitch.setValue(1);
//		Label pitchValue = new Label("1.0", skin);
//		Slider volume = new Slider(0.1f, 1, 0.1f, false, skin);
//		volume.setValue(1);
//		Label volumeValue = new Label("1.0", skin);
//		Table table = new Table();
//		Slider pan = new Slider(-1f, 1f, 0.1f, false, skin);
//		pan.setValue(0);
//		Label panValue = new Label("0.0", skin);
//		table.setFillParent(true);
//
//		table.align(Align.center | Align.top);
//		table.add(soundSelector).colspan(3).row();
//		table.columnDefaults(0).expandX().right().uniformX();
//		table.columnDefaults(2).expandX().left().uniformX();
//		table.add(play);
//		table.add(loop).Left();
//		table.add(stop).Left();
//		table.row();
//		table.add(new Label("Pitch", skin));
//		table.add(pitch);
//		table.add(pitchValue);
//		table.row();
//		table.add(new Label("Volume", skin));
//		table.add(volume);
//		table.add(volumeValue);
//		table.row();
//		table.add(new Label("Pan", skin));
//		table.add(pan);
//		table.add(panValue);
//		ui.addActor(table);
//
//		soundSelector.addListener(new ChangeListener() {
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				setSound(soundSelector.getSelected());
//			}
//		});
//
//		play.addListener(new ClickListener() {
//			public void clicked (InputEvent event, float x, float y) {
//				soundId = sound.play(volume.getValue());
//				sound.setPitch(soundId, pitch.getValue());
//				sound.setPan(soundId, pan.getValue(), volume.getValue());
//			}
//		});
//
//		loop.addListener(new ClickListener() {
//			public void clicked (InputEvent event, float x, float y) {
//				if (soundId == 0) {
//					soundId = sound.loop(volume.getValue());
//					sound.setPitch(soundId, pitch.getValue());
//					sound.setPan(soundId, pan.getValue(), volume.getValue());
//				} else {
//					sound.setLooping(soundId, true);
//				}
//			}
//		});
//		stop.addListener(new ClickListener() {
//			public void clicked (InputEvent event, float x, float y) {
//				sound.stop(soundId);
//				soundId = 0;
//			}
//		});
//		pitch.addListener(new ChangeListener() {
//			public void changed (ChangeEvent event, Actor actor) {
//				sound.setPitch(soundId, pitch.getValue());
//				pitchValue.setText("" + pitch.getValue());
//			}
//		});
//		volume.addListener(new ChangeListener() {
//			public void changed (ChangeEvent event, Actor actor) {
//				sound.setVolume(soundId, volume.getValue());
//				volumeValue.setText("" + volume.getValue());
//			}
//		});
//		pan.addListener(new ChangeListener() {
//			public void changed (ChangeEvent event, Actor actor) {
//				sound.setPan(soundId, pan.getValue(), volume.getValue());
//				panValue.setText("" + pan.getValue());
//			}
//		});
//		Gdx.input.setInputProcessor(ui);
//	}
//
//	protected void setSound (String fileName) {
//		if (sound != null) sound.Dispose();
//		sound = Gdx.audio.NewSound(Gdx.files.@internal("data").child(fileName));
//	}
//
//	public override void Resize (int width, int height) {
//		ui.getViewport().update(width, height, true);
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		ui.act(Gdx.graphics.getDeltaTime());
//		ui.draw();
//	}
//
//	public override void Dispose () {
//		ui.Dispose();
//		skin.Dispose();
//		sound.Dispose();
//	}
//}
