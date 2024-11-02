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
///** Cycles viewports while rendering a stage with a root Table for the layout. */
//public class ViewportTest1 : GdxTest {
//	Array<Viewport> viewports;
//	Array<String> names;
//	Stage stage;
//	Label label;
//
//	public void create () {
//		stage = new Stage();
//		Skin skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		label = new Label("", skin);
//
//		Table root = new Table(skin);
//		root.setFillParent(true);
//		root.setBackground(skin.getDrawable("default-pane"));
//		root.debug().defaults().Space(6);
//		root.add(new TextButton("Button 1", skin));
//		root.add(new TextButton("Button 2", skin)).row();
//		root.add("Press spacebar to change the viewport:").colspan(2).row();
//		root.add(label).colspan(2);
//		stage.addActor(root);
//
//		viewports = getViewports(stage.getCamera());
//		names = getViewportNames();
//
//		stage.setViewport(viewports.first());
//		label.setText(names.first());
//
//		Gdx.input.setInputProcessor(new InputMultiplexer(new InputAdapter() {
//			public boolean keyDown (int keycode) {
//				if (keycode == Input.Keys.SPACE) {
//					int index = (viewports.indexOf(stage.getViewport(), true) + 1) % viewports.size;
//					label.setText(names.get(index));
//					Viewport viewport = viewports.get(index);
//					stage.setViewport(viewport);
//					resize(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//				}
//				return false;
//			}
//		}, stage));
//	}
//
//	public void render () {
//		stage.act();
//
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
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
//	static public Array<String> getViewportNames () {
//		Array<String> names = new Array();
//		names.add("StretchViewport");
//		names.add("FillViewport");
//		names.add("FitViewport");
//		names.add("ExtendViewport: no max");
//		names.add("ExtendViewport: max");
//		names.add("ScreenViewport: 1:1");
//		names.add("ScreenViewport: 0.75:1");
//		names.add("ScalingViewport: none");
//		return names;
//	}
//
//	static public Array<Viewport> getViewports (Camera camera) {
//		int minWorldWidth = 640;
//		int minWorldHeight = 480;
//		int maxWorldWidth = 800;
//		int maxWorldHeight = 480;
//
//		Array<Viewport> viewports = new Array();
//		viewports.add(new StretchViewport(minWorldWidth, minWorldHeight, camera));
//		viewports.add(new FillViewport(minWorldWidth, minWorldHeight, camera));
//		viewports.add(new FitViewport(minWorldWidth, minWorldHeight, camera));
//		viewports.add(new ExtendViewport(minWorldWidth, minWorldHeight, camera));
//		viewports.add(new ExtendViewport(minWorldWidth, minWorldHeight, maxWorldWidth, maxWorldHeight, camera));
//		viewports.add(new ScreenViewport(camera));
//
//		ScreenViewport screenViewport = new ScreenViewport(camera);
//		screenViewport.setUnitsPerPixel(0.75f);
//		viewports.add(screenViewport);
//
//		viewports.add(new ScalingViewport(Scaling.none, minWorldWidth, minWorldHeight, camera));
//		return viewports;
//	}
//}
