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
//public class AudioDeviceTest : GdxTest {
//	Thread thread;
//	boolean stop = false;
//	Stage ui;
//	Skin skin;
//	float wavePanValue = 0;
//
//	public override void Create () {
//
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		ui = new Stage(new FitViewport(640, 400));
//
//		Table table = new Table(skin);
//		final Slider pan = new Slider(-1f, 1f, 0.1f, false, skin);
//		pan.setValue(0);
//		final Label panValue = new Label("0.0", skin);
//		table.setFillParent(true);
//		table.add("Pan");
//		table.add(pan);
//		table.add(panValue).width(100);
//
//		ui.addActor(table);
//
//		Gdx.input.setInputProcessor(ui);
//
//		pan.addListener(new ChangeListener() {
//			public void changed (ChangeEvent event, Actor actor) {
//				wavePanValue = pan.getValue();
//				panValue.setText("" + pan.getValue());
//			}
//		});
//
//		if (thread == null) {
//			final int samplingFrequency = 44100;
//			final AudioDevice device = Gdx.app.getAudio().newAudioDevice(samplingFrequency, false);
//			thread = new Thread(new Runnable() {
//				@Override
//				public void run () {
//					final float waveFrequency = 440;
//					float samples[] = new float[1024];
//					long playedFrames = 0;
//					while (!stop) {
//						for (int i = 0; i < samples.length; i += 2) {
//							float time = (float)playedFrames / (float)samplingFrequency;
//							float wave = (float)Math.sin(time * waveFrequency * Math.PI * 2.0);
//							float pan = wavePanValue * .5f + .5f;
//							samples[i] = wave * (1 - pan);
//							samples[i + 1] = wave * pan;
//							playedFrames++;
//						}
//
//						device.writeSamples(samples, 0, samples.length);
//					}
//
//					device.Dispose();
//				}
//			});
//			thread.start();
//		}
//	}
//
//	public override void Resize (int width, int height) {
//		ui.getViewport().update(width, height, true);
//	}
//
//	public override void Render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		ui.act(Gdx.graphics.getDeltaTime());
//		ui.draw();
//	}
//
//	public override void Dispose () {
//		ui.Dispose();
//		skin.Dispose();
//		stop = true;
//		try {
//			thread.join();
//		} catch (InterruptedException e) {
//			e.printStackTrace();
//		}
//	}
//}
