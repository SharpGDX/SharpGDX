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
//public class GroupCullingTest : GdxTest {
//	static private readonly int count = 100;
//
//	private Stage stage;
//	private Skin skin;
//	private Table root;
//	private Label drawnLabel;
//	int drawn;
//
//	public void create () {
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//
//		root = new Table();
//		root.setFillParent(true);
//		stage.addActor(root);
//
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		Table labels = new Table();
//		root.add(new ScrollPane(labels, skin)).expand().fill();
//		root.row();
//		root.add(drawnLabel = new Label("", skin));
//
//		for (int i = 0; i < count; i++) {
//			labels.add(new Label("Label: " + i, skin) {
//				public void draw (Batch batch, float parentAlpha) {
//					super.draw(batch, parentAlpha);
//					drawn++;
//				}
//			});
//			labels.row();
//		}
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		skin.Dispose();
//	}
//
//	public void resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//		root.invalidate();
//	}
//
//	public void render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		drawn = 0;
//		stage.act(Gdx.graphics.getDeltaTime());
//		stage.draw();
//		drawnLabel.setText("Drawn: " + drawn + "/" + count);
//		drawnLabel.invalidateHierarchy();
//	}
//
//	public boolean needsGL20 () {
//		return false;
//	}
//}
