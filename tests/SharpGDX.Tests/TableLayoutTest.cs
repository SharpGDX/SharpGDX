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
//public class TableLayoutTest : GdxTest {
//	Stage stage;
//
//	public override void Create () {
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//		Skin skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		Label nameLabel = new Label("Name:", skin);
//		TextField nameText = new TextField("", skin);
//		Label addressLabel = new Label("Address:", skin);
//		TextField addressText = new TextField("", skin);
//
//		Table table = new Table();
//		stage.addActor(table);
//		table.setSize(260, 195);
//		table.setPosition(190, 142);
//		// table.align(Align.right | Align.bottom);
//
//		table.debug();
//
//		TextureRegion upRegion = skin.getRegion("default-slider-knob");
//		TextureRegion downRegion = skin.getRegion("default-slider-knob");
//		BitmapFont buttonFont = skin.getFont("default-font");
//
//		TextButton button = new TextButton("Button 1", skin);
//		button.addListener(new InputListener() {
//			public boolean touchDown (InputEvent event, float x, float y, int pointer, int button) {
//				Console.WriteLine("touchDown 1");
//				return false;
//			}
//		});
//		table.add(button);
//		// table.setTouchable(Touchable.disabled);
//
//		Table table2 = new Table();
//		stage.addActor(table2);
//		table2.setFillParent(true);
//		table2.bottom();
//
//		TextButton button2 = new TextButton("Button 2", skin);
//		button2.addListener(new ChangeListener() {
//			public void changed (ChangeEvent event, Actor actor) {
//				Console.WriteLine("2!");
//			}
//		});
//		button2.addListener(new InputListener() {
//			public boolean touchDown (InputEvent event, float x, float y, int pointer, int button) {
//				Console.WriteLine("touchDown 2");
//				return false;
//			}
//		});
//		table2.add(button2);
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		stage.act(Gdx.graphics.getDeltaTime());
//		stage.draw();
//	}
//
//	public override void Resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//	}
//}
