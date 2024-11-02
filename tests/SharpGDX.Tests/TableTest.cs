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
//public class TableTest : GdxTest {
//	Skin skin;
//	Stage stage;
//	Texture texture;
//	Table root;
//
//	public override void Create () {
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"));
//		TextureRegion region = new TextureRegion(texture);
//
//		NinePatch patch = skin.getPatch("default-round");
//
//		Label label = new Label("This is some text.", skin);
//
//		root = new Table() {
//			public void draw (Batch batch, float parentAlpha) {
//				super.draw(batch, parentAlpha);
//			}
//		};
//		stage.addActor(root);
//		// root.setTransform(true);
//
//		Table table = new Table();
//		table.setTransform(true);
//		table.setPosition(100, 100);
//		table.setOrigin(0, 0);
//		table.setRotation(45);
//		table.setScaleY(2);
//		table.add(label);
//		table.add(new TextButton("Text Button", skin));
//		table.pack();
//		// table.debug();
//		table.addListener(new ClickListener() {
//			public void clicked (InputEvent event, float x, float y) {
//				Console.WriteLine("click!");
//			}
//		});
//// root.addActor(table);
//
//		TextButton button = new TextButton("Text Button", skin);
//		Table table2 = new Table();
//		// table2.debug()
//		table2.add(button);
//		table2.setTransform(true);
//		table2.setScaleX(1.5f);
//		table2.setOrigin(table2.getPrefWidth() / 2, table2.getPrefHeight() / 2);
//
//		// Test colspan with expandX.
//		// root.setPosition(10, 10);
//		root.debug();
//		root.setFillParent(true);
//		root.add(new Label("meow meow meow meow meow meow meow meow meow meow meow meow", skin)).colspan(3).expandX();
//		root.add(new TextButton("Text Button", skin));
//		root.row();
//		root.add(new TextButton("Text Button", skin));
//		root.add(new TextButton("Toggle Button", skin.get("toggle", TextButton.TextButtonStyle.class)));
//		root.add(new CheckBox("meow meow meow meow meow meow meow meow", skin));
//		// root.pack();
//		// root.add(new Button(new Image(region), skin));
//		// root.add(new LabelButton("Toggley", skin.getStyle("toggle", LabelButtonStyle.class)));
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//
//		stage.act(Math.Min(Gdx.graphics.getDeltaTime(), 1 / 30f));
//		stage.draw();
//	}
//
//	public override void Resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//// root.width = width;
//// root.height = height;
//// root.invalidate();
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		texture.Dispose();
//		skin.Dispose();
//	}
//}
