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
//using System.Text;
//using SharpGDX.Graphics.GLUtils;
//
//namespace SharpGDX.Tests;
//
//// test for TextField#textHeight calculation change
//public class TextAreaTest3 : GdxTest {
//	private Stage stage;
//	private Skin skin;
//	TextField textField;
//	TextArea textArea;
//	private TextField.TextFieldStyle styleDefault;
//	private TextField.TextFieldStyle styleLSans15;
//	private TextField.TextFieldStyle styleLSans32;
//	private TextField.TextFieldStyle styleFont;
//
//	public override void Create () {
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		// default font in the skin has line height == text height, so its impossible to see updated selection/cursor rendering
//		styleDefault = skin.get(TextField.TextFieldStyle.class);
//		// nearest so its easier to see whats going on
//		styleDefault.font.getRegion().getTexture().setFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
//		styleDefault.font.getData().setLineHeight(styleDefault.font.getData().lineHeight * 2);
//		printMetrics("default", styleDefault.font);
//
//		styleLSans15 = new TextField.TextFieldStyle(styleDefault);
//		styleLSans15.font = new BitmapFont(Gdx.files.@internal("data/lsans-15.fnt"), Gdx.files.@internal("data/lsans-15_00.png"),
//			false);
//		printMetrics("lsans15", styleLSans15.font);
//
//		styleLSans32 = new TextField.TextFieldStyle(styleDefault);
//		styleLSans32.font = new BitmapFont(Gdx.files.@internal("data/lsans-32.fnt"), Gdx.files.@internal("data/lsans-32.png"), false);
//		printMetrics("lsans32", styleLSans32.font);
//
//		styleFont = new TextField.TextFieldStyle(styleDefault);
//		styleFont.font = new BitmapFont(Gdx.files.@internal("data/font.fnt"), Gdx.files.@internal("data/font.png"), false);
//		printMetrics("font", styleFont.font);
//
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//		// easier to test this with proper layout
//		Table root = new Table();
//		root.setFillParent(true);
//		stage.addActor(root);
//
//		Table styleSelector = new Table();
//		styleSelector.defaults().Pad(10);
//		root.add(styleSelector).Row();
//
//		// | is the tallest char
//		textField = new TextField("| Text field", styleDefault);
//		root.add(textField).GrowX().Pad(20, 100, 20, 100).Row();
//
//		StringBuilder sb = new StringBuilder("| Text Area\nEssentially, a text field\nwith\nmultiple\nlines.\n");
//		// we need a bunch of lines to demonstrate that prefHeight is way too large
//		for (int i = 0; i < 30; i++) {
//			sb.Append(
//				"It can even handle very loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong lines.\n");
//		}
//		textArea = new TextArea(sb.ToString(), styleLSans32);
//		// we need a container that will allow the TextArea to be as tall as it wants
//		// without the fix, text area height wont match text height depending on the fonts line height
//		ScrollPane pane = new ScrollPane(textArea, skin);
//		pane.setScrollingDisabled(true, false);
//		pane.setScrollbarsVisible(true);
//		pane.setFadeScrollBars(false);
//		root.add(pane).Grow().Pad(20, 100, 20, 100);
//
//		// after we init widgets
//		ButtonGroup<TextButton> group = new ();
//		styleSelector.add(newStyleButton("Default", styleDefault, group));
//		styleSelector.add(newStyleButton("LSans 15", styleLSans15, group));
//		styleSelector.add(newStyleButton("LSans 32", styleLSans32, group));
//		styleSelector.add(newStyleButton("Font", styleFont, group));
//		group.setMaxCheckCount(1);
//		group.setMinCheckCount(1);
//		group.setChecked("LSans 32");
//	}
//
//	private void printMetrics (String name, BitmapFont font) {
//		BitmapFont.BitmapFontData data = font.getData();
//		float textHeight = data.capHeight - data.descent;
//		float textFieldHeight = data.capHeight - data.descent * 2;
//		Gdx.app.log(name, "line height = " + data.lineHeight + ", text height = " + textHeight + ", text field height = "
//			+ textFieldHeight + ", cap height = " + data.capHeight + ", descent = " + data.descent);
//	}
//
//	private Button newStyleButton (String label, TextField.TextFieldStyle style, ButtonGroup<TextButton> group) {
//		TextButton button = new TextButton(label, skin, "toggle");
//		button.addListener(new ChangeListener() {
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				textField.setStyle(style);
//				textArea.setStyle(style);
//			}
//		});
//		group.add(button);
//		return button;
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//		stage.act();
//		stage.draw();
//		// getLines() does not return correct count before first draw happens and there is no other way to trigger update
//		textArea.setPrefRows(textArea.getLines());
//		// change does not propagate on its own, number of lines does not change so calling this each frame is not necessary
//		textArea.invalidateHierarchy();
//	}
//
//	public override void Resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		skin.Dispose();
//		styleLSans15.font.getRegion().getTexture().Dispose();
//		styleLSans32.font.getRegion().getTexture().Dispose();
//		styleFont.font.getRegion().getTexture().Dispose();
//	}
//}
