//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Scenes.Scene2D;
//using SharpGDX.Scenes.Scene2D.Actions;
//using SharpGDX.Scenes.Scene2D.UI;
//using SharpGDX.Tests.Utils;
//
//namespace SharpGDX.Tests;
//
//public class ActionTest : GdxTest
//{
//    private Stage stage;
//    private Texture texture;
//
//    public override void Create()
//    {
//        stage = new Stage();
//        texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"), false);
//        texture.setFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
//        var img = new Image(new TextureRegion(texture));
//        img.setSize(100, 100);
//        img.setOrigin(50, 50);
//        img.setPosition(100, 100);
//
//        // img.addAction(forever(sequence(delay(1.0f), new Action() {
//        // public boolean act (float delta) {
//        // Console.WriteLine(1);
//        // img.clearActions();
//        // return true;
//        // }
//        // })));
//
//        img.addAction(Actions.moveBy(100, 0, 2));
//        img.addAction(Actions.after(Actions.scaleTo(2, 2, 2)));
//
//        stage.addActor(img);
//    }
//
//    public override void Dispose()
//    {
//        stage.Dispose();
//        texture.Dispose();
//    }
//
//    public override void Render()
//    {
//        Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//        stage.act(Math.Min(Gdx.graphics.getDeltaTime(), 1 / 30f));
//        stage.draw();
//    }
//}