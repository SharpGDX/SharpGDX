//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Tests.Utils;
//
//namespace SharpGDX.Tests;
//
//public class AccelerometerTest : GdxTest
//{
//    private SpriteBatch batch;
//    private BitmapFont font;
//
//    public override void Create()
//    {
//        font = new BitmapFont(Gdx.files.@internal("data/lsans-15.fnt"), false);
//        batch = new SpriteBatch();
//    }
//
//    public override void Dispose()
//    {
//        font.Dispose();
//        batch.Dispose();
//    }
//
//    public override void Render()
//    {
//        Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//        batch.begin();
//        font.draw(batch,
//            "accel: [" + Gdx.input.getAccelerometerX() + "," + Gdx.input.getAccelerometerY() + "," +
//            Gdx.input.getAccelerometerZ()
//            + "]\n" + "gyros: [" + Gdx.input.getGyroscopeX() + "," + Gdx.input.getGyroscopeY() + "," +
//            Gdx.input.getGyroscopeZ()
//            + "]\n" + "orientation: " + Gdx.input.getNativeOrientation() + "\n" + "rotation: " +
//            Gdx.input.getRotation() + "\n"
//            + "wh: " + Gdx.graphics.getDisplayMode() + "\n",
//            0, 100);
//        batch.end();
//    }
//}