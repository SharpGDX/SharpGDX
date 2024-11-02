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
//public class TreeTest : GdxTest {
//	Stage stage;
//	Skin skin;
//	Tree<Node, String> tree;
//	private Label label;
//
//	class Node : Tree.Node<Node, String, TextButton> {
//		public Node (String text) {
//			super(new TextButton(text, skin));
//			setValue(text);
//		}
//	}
//
//	public void create () {
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		label = new Label("", skin);
//		stage.addActor(label);
//
//		Table table = new Table();
//		table.setFillParent(true);
//		stage.addActor(table);
//
//		tree = new Tree(skin);
//		tree.setPadding(10);
//		tree.setIndentSpacing(25);
//		tree.setIconSpacing(5, 0);
//		final Node moo1 = new Node("moo1 (add to moo2)");
//		final Node moo2 = new Node("moo2 (moo3 to bottom)");
//		final Node moo3 = new Node("moo3");
//		final Node moo4 = new Node("moo4");
//		final Node moo5 = new Node("moo5 (remove moo4)");
//		tree.add(moo1);
//		tree.add(moo2);
//		moo2.add(moo3);
//		moo3.add(moo4);
//		tree.add(moo5);
//
//		moo1.getActor().addListener(new ClickListener() {
//			public void clicked (InputEvent event, float x, float y) {
//				Console.WriteLine(moo1.getActor().getText() + ", " + moo1.getValue() + ", " + moo1.getValue().length());
//				Node node = new Node("added " + moo2.getChildren().size);
//				node.add(new Node("1"));
//				node.add(new Node("2"));
//				node.setExpanded(MathUtils.randomBoolean());
//				moo2.insert(MathUtils.randomBoolean() ? moo2.getChildren().size : MathUtils.random(0, moo2.getChildren().size), node);
//			}
//		});
//		moo2.getActor().addListener(new ClickListener() {
//			public void clicked (InputEvent event, float x, float y) {
//				moo2.getChildren().removeValue(moo3, true);
//				moo2.getChildren().add(moo3);
//				moo2.updateChildren();
//			}
//		});
//		moo5.getActor().addListener(new ClickListener() {
//			public void clicked (InputEvent event, float x, float y) {
//				Node node = tree.findNode("moo4");
//				if (node != null) node.remove();
//			}
//		});
//
//		table.add(tree).fill().expand();
//	}
//
//	public void render () {
//		// Console.WriteLine(meow.getValue());
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		stage.act(Gdx.graphics.getDeltaTime());
//		stage.draw();
//
//		label.setText(tree.toString());
//		label.pack();
//		label.setPosition(0, 0, Align.bottomLeft);
//	}
//
//	public void resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//	}
//
//	public void dispose () {
//		stage.Dispose();
//	}
//}
