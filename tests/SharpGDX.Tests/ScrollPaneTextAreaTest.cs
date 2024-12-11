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
//public class ScrollPaneTextAreaTest : GdxTest {
//	Stage stage;
//	TextArea textArea;
//	ScrollPane scrollPane;
//	Skin skin;
//
//	public override void Create () {
//		stage = new Stage(new ScreenViewport());
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		Gdx.input.setInputProcessor(stage);
//
//		Table container = new Table();
//		stage.addActor(container);
//
//		container.setFillParent(true);
//		container.pad(10).defaults().ExpandX().FillX().Space(4);
//
//		textArea = new TextArea(">>> FIRST LINE <<<\n" + "Scrolling to the bottom of the area you should see the last line.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n"
//			+ "Scrolling to the top of the area you should see the first line.\n" + ">>> LAST LINE <<<", skin) {
//			public float getPrefHeight () {
//				return getLines() * getStyle().font.getLineHeight();
//			}
//		};
//
//		scrollPane = new ScrollPane(textArea, skin);
//		scrollPane.setFadeScrollBars(false);
//		scrollPane.setFlickScroll(false);
//
//		container.row().height(350);
//		container.add(scrollPane);
//
//		container.debugAll();
//	}
//
//	public override void Render () {
//		if (textArea.getHeight() != textArea.getPrefHeight()) {
//			scrollPane.invalidate();
//			scrollPane.scrollTo(0, textArea.getHeight() - textArea.getCursorY(), 0, textArea.getStyle().font.getLineHeight());
//		}
//
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
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
//		skin.Dispose();
//	}
//}
