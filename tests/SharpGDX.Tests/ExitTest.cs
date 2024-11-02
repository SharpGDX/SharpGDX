//using SharpGDX.Tests.Utils;
//
//namespace SharpGDX.Tests;
//
//public class ExitTest : GdxTest
//{
//    public override void Dispose()
//    {
//        Gdx.app.log("ExitTest", "disposed");
//    }
//
//    public override void Pause()
//    {
//        Gdx.app.log("ExitTest", "paused");
//    }
//
//    public override void Render()
//    {
//        if (Gdx.input.justTouched())
//        {
//            Gdx.app.exit();
//        }
//    }
//}