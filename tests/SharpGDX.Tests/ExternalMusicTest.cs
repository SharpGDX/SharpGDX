//using SharpGDX.Audio;
//using SharpGDX.Tests.Utils;
//
//namespace SharpGDX.Tests;
//
///**
// * Tests playing back audio from the external storage.
// * @author mzechner
// */
//public class ExternalMusicTest : GdxTest
//{
//    private IMusic music;
//
//    public override void Create()
//    {
//        // copy an internal mp3 to the external storage
//        var src = Gdx.files.@internal("data/8.12.mp3");
//        var dst = Gdx.files.external("8.12.mp3");
//        src.copyTo(dst);
//
//        // create a music instance and start playback
//        music = Gdx.audio.NewMusic(dst);
//        music.Play();
//    }
//
//    public override void Dispose()
//    {
//        music.Stop();
//        music.Dispose();
//        // delete the copy on the external storage
//        Gdx.files.external("8.12.mp3").delete();
//    }
//}