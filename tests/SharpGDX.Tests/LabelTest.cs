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
//public class LabelTest : GdxTest {
//	Skin skin;
//	Stage stage;
//	SpriteBatch batch;
//	Actor root;
//	ShapeRenderer renderer;
//
//	float scale = 1;
//
//	public override void Create () {
//		batch = new SpriteBatch();
//		renderer = new ShapeRenderer();
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		skin.getAtlas().getTextures().iterator().next().setFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
//		skin.getFont("default-font").getData().markupEnabled = true;
//		skin.getFont("default-font").getData().setScale(scale);
//		stage = new Stage(new ScreenViewport());
//		Gdx.input.setInputProcessor(stage);
//
//		// skin.getFont("default-font").getData().getGlyph('T').xoffset = -20;
//		skin.getFont("default-font").getData().getGlyph('B').setKerning('B', -5);
//
//		Label label;
//
//		Table table = new Table().debug();
//
//		table.add(new Label("This is regular text.", skin)).row();
//		table.add(new Label("This is regular text\nwith a newline.", skin)).row();
//
//		label = new Label("This is [RED]regular text\n\nwith newlines,\naligned bottom, right.", skin);
//		label.setColor(Color.GREEN);
//		label.setAlignment(Align.bottom | Align.right);
//		table.add(label).minWidth(200 * scale).minHeight(110 * scale).fill().row();
//
//		label = new Label("This is regular text with NO newlines, wrap enabled and aligned bottom, right.", skin);
//		label.setWrap(true);
//		label.setAlignment(Align.bottom | Align.right);
//		table.add(label).minWidth(200 * scale).minHeight(110 * scale).fill().row();
//
//		label = new Label("This is regular text with\n\nnewlines, wrap\nenabled and aligned bottom, right.", skin);
//		label.setWrap(true);
//		label.setAlignment(Align.bottom | Align.right);
//		table.add(label).minWidth(200 * scale).minHeight(110 * scale).fill().row();
//
//		table.setPosition(50, 40 + 25 * scale);
//		table.pack();
//		stage.addActor(table);
//
//		//
//
//		table = new Table().debug();
//		stage.addActor(table);
//
//		table.add(new Label("This is regular text.", skin)).minWidth(200 * scale).row();
//
//		// The color markup text should match the uncolored text exactly.
//		label = new Label("AAA BBB CCC DDD EEE", skin);
//		table.add(label).align(Align.left).row();
//
//		label = new Label("AAA B[RED]B[]B CCC DDD EEE", skin);
//		table.add(label).align(Align.left).row();
//
//		label = new Label("[RED]AAA [BLUE]BBB [RED]CCC [BLUE]DDD [RED]EEE", skin);
//		table.add(label).align(Align.left).row();
//
//		label = new Label("AAA BBB CCC DDD EEE", skin);
//		label.setWrap(true);
//		table.add(label).align(Align.left).width(150 * scale).row();
//
//		label = new Label("[RED]AAA [BLUE]BBB [RED]CCC [BLUE]DDD [RED]EEE", skin);
//		label.setWrap(true);
//		table.add(label).align(Align.left).width(150 * scale).row();
//
//		table.setPosition(50 + 250 * scale, 40 + 25 * scale);
//		table.pack();
//		stage.addActor(table);
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		skin.Dispose();
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//
//		stage.act(Math.min(Gdx.graphics.getDeltaTime(), 1 / 30f));
//		stage.draw();
//
//		float x = 40, y = 15 + 20 * scale;
//
//		BitmapFont font = skin.getFont("default-font");
//		batch.begin();
//		font.draw(batch, "The quick brown fox jumped over the lazy cow.", x, y);
//		batch.end();
//
//		drawLine(x, y - font.getDescent(), x + 1000, y - font.getDescent());
//		drawLine(x, y - font.getCapHeight() + font.getDescent(), x + 1000, y - font.getCapHeight() + font.getDescent());
//	}
//
//	public void drawLine (float x1, float y1, float x2, float y2) {
//		renderer.setProjectionMatrix(batch.getProjectionMatrix());
//		renderer.begin(ShapeRenderer.ShapeType.Line);
//		renderer.line(x1, y1, x2, y2);
//		renderer.end();
//	}
//
//	public override void Resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//		batch.getProjectionMatrix().setToOrtho2D(0, 0, width, height);
//	}
//}
