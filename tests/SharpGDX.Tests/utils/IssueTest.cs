//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//
//namespace SharpGDX.Tests.Utils;
//
///// <summary>
/////     Placeholder class that's wired up with all backends to quickly test out issues...
///// </summary>
//public class IssueTest : GdxTest
//{
//    private SpriteBatch batch;
//    private Texture img;
//    private Texture img2;
//
//    public override void Create()
//    {
//        batch = new SpriteBatch();
//        img = new Texture("data/issue/bark.png");
//        img2 = new Texture("data/issue/leaf.png");
//    }
//
//    public override void Render()
//    {
//        GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//        batch.begin();
//        batch.draw(img, 0, 0);
//        batch.draw(img2, 512, 0);
//        batch.end();
//    }
//}