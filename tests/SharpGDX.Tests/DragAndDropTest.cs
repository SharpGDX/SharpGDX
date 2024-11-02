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
//
//namespace SharpGDX.Tests;
//
//public class DragAndDropTest : GdxTest {
//	Stage stage;
//
//	public void create () {
//		stage = new Stage();
//		Gdx.input.setInputProcessor(stage);
//
//		final Skin skin = new Skin();
//		skin.add("default", new LabelStyle(new BitmapFont(), Color.WHITE));
//		skin.add("badlogic", new Texture("data/badlogic.jpg"));
//
//		Image sourceImage = new Image(skin, "badlogic");
//		sourceImage.setBounds(50, 125, 100, 100);
//		stage.addActor(sourceImage);
//
//		Image validTargetImage = new Image(skin, "badlogic");
//		validTargetImage.setBounds(200, 50, 100, 100);
//		stage.addActor(validTargetImage);
//
//		Image invalidTargetImage = new Image(skin, "badlogic");
//		invalidTargetImage.setBounds(200, 200, 100, 100);
//		stage.addActor(invalidTargetImage);
//
//		DragAndDrop dragAndDrop = new DragAndDrop();
//		dragAndDrop.addSource(new Source(sourceImage) {
//			@Null
//			public Payload dragStart (InputEvent event, float x, float y, int pointer) {
//				Payload payload = new Payload();
//				payload.setObject("Some payload!");
//
//				payload.setDragActor(getActor());
//
//				Label validLabel = new Label("Some payload!", skin);
//				validLabel.setColor(0, 1, 0, 1);
//				payload.setValidDragActor(validLabel);
//
//				Label invalidLabel = new Label("Some payload!", skin);
//				invalidLabel.setColor(1, 0, 0, 1);
//				payload.setInvalidDragActor(invalidLabel);
//
//				return payload;
//			}
//		});
//		dragAndDrop.addTarget(new Target(validTargetImage) {
//			public boolean drag (Source source, Payload payload, float x, float y, int pointer) {
//				getActor().setColor(Color.GREEN);
//				return true;
//			}
//
//			public void reset (Source source, Payload payload) {
//				getActor().setColor(Color.WHITE);
//			}
//
//			public void drop (Source source, Payload payload, float x, float y, int pointer) {
//				Console.WriteLine("Accepted: " + payload.getObject() + " " + x + ", " + y);
//			}
//		});
//		dragAndDrop.addTarget(new Target(invalidTargetImage) {
//			public boolean drag (Source source, Payload payload, float x, float y, int pointer) {
//				getActor().setColor(Color.RED);
//				return false;
//			}
//
//			public void reset (Source source, Payload payload) {
//				getActor().setColor(Color.WHITE);
//			}
//
//			public void drop (Source source, Payload payload, float x, float y, int pointer) {
//			}
//		});
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
//}
