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
//
//namespace SharpGDX.Tests;
//
//public class SystemCursorTest : GdxTest {
//	private Stage stage;
//	private Skin skin;
//
//	public override void Create () {
//		base.Create();
//		stage = new Stage(new ScreenViewport());
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		Gdx.input.setInputProcessor(stage);
//
//		Table table = new Table();
//		table.setFillParent(true);
//		stage.addActor(table);
//
//		foreach (ICursor.SystemCursor cursor in ICursor.SystemCursor.values()) {
//			TextButton button = new TextButton(cursor.name(), skin);
//			button.addListener(new ChangeListener() {
//				@Override
//				public void changed (ChangeEvent event, Actor actor) {
//					Gdx.graphics.setSystemCursor(cursor);
//				}
//			});
//			table.add(button).row();
//		}
//	}
//
//	public override void Render () {
//        base.Render();
//		ScreenUtils.clear(Color.BLACK);
//		stage.act();
//		stage.draw();
//	}
//
//	public override void Resize (int width, int height) {
//        base.Resize(width, height);
//		stage.getViewport().update(width, height, true);
//	}
//
//	public override void Dispose () {
//		base.Dispose();
//		stage.Dispose();
//		skin.Dispose();
//	}
//}
