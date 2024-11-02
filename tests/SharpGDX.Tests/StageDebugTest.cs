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
///** @author Daniel Holderbaum */
//public class StageDebugTest : GdxTest {
//	static TextureRegion textureRegion;
//
//	private Stage stage;
//	private Stage stage1;
//	private Stage stage2;
//
//	class DebugActor : Actor {
//		@Override
//		public void draw (IBatch batch, float parentAlpha) {
//			batch.draw(textureRegion, getX(), getY(), getOriginX(), getOriginY(), getWidth(), getHeight(), getScaleX(), getScaleY(),
//				getRotation());
//		}
//	}
//
//	public override void Create () {
//		textureRegion = new TextureRegion(new Texture("data/badlogic.jpg"));
//
//		Gdx.input.setInputProcessor(this);
//
//		stage1 = new Stage();
//		stage1.getCamera().position.set(100, 100, 0);
//
//		Group group = new Group();
//		// group.setBounds(0, 0, 10, 10);
//		// group.setOrigin(25, 50);
//		group.setRotation(10);
//		group.setScale(1.2f);
//		stage1.addActor(group);
//
//		DebugActor actor = new DebugActor();
//		actor.setBounds(300, 140, 50, 100);
//		actor.setOrigin(25, 50);
//		actor.setRotation(-45);
//		actor.setScale(2f);
//		actor.addAction(forever(rotateBy(360, 8f)));
//		group.addActor(actor);
//
//		group.debugAll();
//
//		stage2 = new Stage();
//		Skin skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		TextButton shortButton = new TextButton("Button short", skin);
//		shortButton.debug();
//
//		TextButton longButton = new TextButton("Button loooooooooong", skin);
//		longButton.debug();
//
//		Table root = new Table(skin);
//		root.setFillParent(true);
//		root.setBackground(skin.getDrawable("default-pane"));
//		root.defaults().Space(6);
//		root.setTransform(true);
//		root.rotateBy(10);
//		root.setScale(1.3f, 1);
//		root.debug();
//		stage2.addActor(root);
//
//		root.add(shortButton).Pad(5);
//		root.add(longButton).Row();
//		root.add("Colspan").Colspan(2).Row();
//
//		switchStage();
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT);
//		stage.act();
//		stage.draw();
//	}
//
//	public override bool TouchDown (int screenX, int screenY, int pointer, int button) {
//		switchStage();
//		return false;
//	}
//
//	public override void Resize (int width, int height) {
//		stage1.getViewport().update(width, height, true);
//		stage2.getViewport().update(width, height, true);
//	}
//
//	private void switchStage () {
//		if (stage != stage2) {
//			stage = stage2;
//		} else {
//			stage = stage1;
//		}
//	}
//
//}
