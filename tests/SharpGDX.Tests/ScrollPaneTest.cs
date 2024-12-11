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
//public class ScrollPaneTest : GdxTest {
//	private Stage stage;
//	private Table container;
//
//	public override void Create () {
//		stage = new Stage();
//		Skin skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		Gdx.input.setInputProcessor(stage);
//
//		// Gdx.graphics.setVSync(false);
//
//		container = new Table();
//		stage.addActor(container);
//		container.setFillParent(true);
//
//		Table table = new Table();
//		// table.debug();
//
//		ScrollPane scroll = new ScrollPane(table, skin);
//
//		InputListener stopTouchDown = new InputListener() {
//			public boolean touchDown (InputEvent event, float x, float y, int pointer, int button) {
//				event.stop();
//				return false;
//			}
//		};
//
//		table.pad(10).defaults().expandX().space(4);
//		for (int i = 0; i < 100; i++) {
//			table.row();
//			table.add(new Label(i + "uno", skin)).expandX().fillX();
//
//			TextButton button = new TextButton(i + "dos", skin);
//			table.add(button);
//			button.addListener(new ClickListener() {
//				public void clicked (InputEvent event, float x, float y) {
//					Console.WriteLine("click " + x + ", " + y);
//				}
//			});
//
//			Slider slider = new Slider(0, 100, 1, false, skin);
//			slider.addListener(stopTouchDown); // Stops touchDown events from propagating to the FlickScrollPane.
//			table.add(slider);
//
//			table.add(new Label(i + "tres long0 long1 long2 long3 long4 long5 long6 long7 long8 long9 long10 long11 long12", skin));
//		}
//
//		final TextButton flickButton = new TextButton("Flick Scroll", skin.get("toggle", TextButtonStyle.class));
//		flickButton.setChecked(true);
//		flickButton.addListener(new ChangeListener() {
//			public void changed (ChangeEvent event, Actor actor) {
//				scroll.setFlickScroll(flickButton.isChecked());
//			}
//		});
//
//		final TextButton fadeButton = new TextButton("Fade Scrollbars", skin.get("toggle", TextButtonStyle.class));
//		fadeButton.setChecked(true);
//		fadeButton.addListener(new ChangeListener() {
//			public void changed (ChangeEvent event, Actor actor) {
//				scroll.setFadeScrollBars(fadeButton.isChecked());
//			}
//		});
//
//		final TextButton smoothButton = new TextButton("Smooth Scrolling", skin.get("toggle", TextButtonStyle.class));
//		smoothButton.setChecked(true);
//		smoothButton.addListener(new ChangeListener() {
//			public void changed (ChangeEvent event, Actor actor) {
//				scroll.setSmoothScrolling(smoothButton.isChecked());
//			}
//		});
//
//		final TextButton onTopButton = new TextButton("Scrollbars On Top", skin.get("toggle", TextButtonStyle.class));
//		onTopButton.addListener(new ChangeListener() {
//			public void changed (ChangeEvent event, Actor actor) {
//				scroll.setScrollbarsOnTop(onTopButton.isChecked());
//			}
//		});
//
//		container.add(scroll).expand().fill().colspan(4);
//		container.row().space(10).padBottom(10);
//		container.add(flickButton).right().expandX();
//		container.add(onTopButton);
//		container.add(smoothButton);
//		container.add(fadeButton).left().expandX();
//	}
//
//	public void render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		stage.act(Gdx.graphics.getDeltaTime());
//		stage.draw();
//	}
//
//	public void resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//
//		// GDX.GL.glViewport(100, 100, width - 200, height - 200);
//		// stage.setViewport(800, 600, false, 100, 100, width - 200, height - 200);
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//	}
//
//	public bool needsGL20 () {
//		return false;
//	}
//}
