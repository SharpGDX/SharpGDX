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
//public class AudioRecorderTest : GdxTest {
//	short[] samples = new short[1024 * 4];
//	AudioDevice device;
//	AudioRecorder recorder;
//
//	public override void Create () {
//		device = Gdx.audio.newAudioDevice(44100, true);
//		recorder = Gdx.audio.newAudioRecorder(44100, true);
//
//		Thread t = new Thread(new Runnable() {
//
//			@Override
//			public void run () {
//				while (true) {
//					recorder.read(samples, 0, samples.length);
//					device.writeSamples(samples, 0, samples.length);
//				}
//			}
//		});
//		t.setDaemon(true);
//		t.start();
//	}
//
//	public override void Render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//	}
//
//	public override void Pause () {
//		device.Dispose();
//		recorder.Dispose();
//	}
//
//	public override void Resume () {
//		device = Gdx.audio.newAudioDevice(44100, true);
//		recorder = Gdx.audio.newAudioRecorder(44100, true);
//	}
//}
