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
//public class ScrollPaneWithDynamicScrolling : GdxTest {
//	private Stage stage;
//	private Table container;
//	Label dynamicLabel;
//	ScrollPane scrollPane;
//	int count;
//
//	public void create () {
//		stage = new Stage();
//		Skin skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		Gdx.input.setInputProcessor(stage);
//
//		dynamicLabel = new Label("Chat box begin here", skin);
//
//		float chatWidth = 200;
//		float controlHeight = 300;
//
//		scrollPane = new ScrollPane(dynamicLabel, skin);
//
//		Table main = new Table();
//
//		main.setFillParent(true);
//
//		TextButton btAdd = new TextButton("Add text and scroll down", skin);
//
//		main.add(btAdd).Row();
//		main.add(scrollPane).Size(200, 100);
//
//		stage.addActor(main);
//
//		stage.setDebugAll(true);
//
//		btAdd.addListener(new ChangeListener() {
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				dynamicLabel.setText(dynamicLabel.getText() + "\nline " + count++);
//				scrollPane.scrollTo(0, 0, 0, 0);
//			}
//		});
//
//	}
//
//	public void render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		stage.act(Gdx.graphics.getDeltaTime());
//		stage.draw();
//	}
//
//	public void resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//	}
//
//	public void dispose () {
//		stage.Dispose();
//	}
//
//	public bool needsGL20 () {
//		return false;
//	}
//}
