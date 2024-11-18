namespace SharpGDX.Tests.Desktop;
using SharpGDX.Tests.Utils;
using SharpGDX.Graphics;
using SharpGDX.Utils;

using SharpGDX.Scenes.Scene2D.UI;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Desktop;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Shims;
using SharpGDX.Utils.Viewports;

public class DesktopTestStarter {

	static CommandLineOptions options;

	/** Runs libgdx tests.
	 * 
	 * some options can be passed, see {@link CommandLineOptions}
	 * 
	 * @param argv command line arguments */
	public static void main (String[] argv) {
		options = new CommandLineOptions(argv);

		DesktopApplicationConfiguration config = new DesktopApplicationConfiguration();
		config.setWindowedMode(640, 480);

		if (options.gl30 || options.gl31 || options.gl32) {
			ShaderProgram.prependVertexCode = "#version 140\n#define varying out\n#define attribute in\n";
			ShaderProgram.prependFragmentCode = "#version 140\n#define varying in\n#define texture2D texture\n#define gl_FragColor fragColor\nout vec4 fragColor;\n";
		}

		if (options.gl32) {
			config.setOpenGLEmulation(DesktopApplicationConfiguration.GLEmulation.GL32, 4, 6);
		} else if (options.gl31) {
			config.setOpenGLEmulation(DesktopApplicationConfiguration.GLEmulation.GL31, 4, 5);
		} else if (options.gl30) {
			if (SharedLibraryLoader.isMac) {
				config.setOpenGLEmulation(DesktopApplicationConfiguration.GLEmulation.GL30, 3, 2);
			} else {
				config.setOpenGLEmulation(DesktopApplicationConfiguration.GLEmulation.GL30, 4, 3);
			}
		} else if (options.angle) {
			config.setOpenGLEmulation(DesktopApplicationConfiguration.GLEmulation.ANGLE_GLES20, 0, 0);
			// Use CPU sync if ANGLE is enabled on macOS, otherwise the framerate gets halfed
			// by each new open window.
			if (SharedLibraryLoader.isMac) {
				config.useVsync(false);
				config.setForegroundFPS(60);
			}
		}

		if (options.startupTestName != null) {
			IApplicationListener test = GdxTests.newTest(options.startupTestName);
			if (test != null) {
				new DesktopApplication(test, config);
				return;
			}
			// Otherwise, fall back to showing the list
		}
		new DesktopApplication(new TestChooser(), config);
	}

	class TestChooser : ApplicationAdapter {
		private Stage stage;
		private Skin skin;
		TextButton lastClickedTestButton;

		public override void Create () {
			Console.WriteLine("OpenGL renderer: " + Gdx.Graphics.getGLVersion().GetRendererString());
			Console.WriteLine("OpenGL vendor: " + Gdx.Graphics.getGLVersion().GetVendorString());

			IPreferences prefs = Gdx.App.GetPreferences("lwjgl3-tests");

			stage = new Stage(new ScreenViewport());
			Gdx.Input.SetInputProcessor(stage);
			skin = new Skin(Gdx.Files.Internal("data/uiskin.json"));

			Table container = new Table();
			stage.addActor(container);
			container.setFillParent(true);

			Table table = new Table();

			ScrollPane scroll = new ScrollPane(table, skin);
			scroll.setSmoothScrolling(false);
			scroll.setFadeScrollBars(false);
			stage.setScrollFocus(scroll);

			int tableSpace = 4;
			table.pad(10).defaults().ExpandX().Space(tableSpace);
			foreach (String testName in GdxTests.getNames()) {
				TextButton testButton = new TextButton(testName, skin);
				testButton.setDisabled(!options.isTestCompatible(testName));
				testButton.setName(testName);
				table.add(testButton).FillX();
				table.row();
				testButton.addListener(new TestButtonChangeListener(prefs, this, testName, testButton) {
					
				});
			}

			container.add(scroll).Expand().Fill();
			container.row();

			lastClickedTestButton = (TextButton)table.findActor<TextButton>(prefs.GetString("LastTest"));
			if (lastClickedTestButton != null) {
				lastClickedTestButton.setColor(Color.Cyan);
				scroll.layout();
				float scrollY = lastClickedTestButton.getY() + scroll.getScrollHeight() / 2 + lastClickedTestButton.getHeight() / 2
					+ tableSpace * 2 + 20;
				scroll.scrollTo(0, scrollY, 0, 0, false, false);

				// Since ScrollPane takes some time for scrolling to a position, we just "fake" time
				stage.act(1f);
				stage.act(1f);
				stage.draw();
			}
		}

        private class TestButtonChangeListener : ChangeListener
        {
            private readonly IPreferences _prefs;
            private readonly TestChooser _testChooser;
            private readonly string _testName;
            private readonly TextButton _testButton;

            public TestButtonChangeListener(IPreferences prefs, TestChooser testChooser, string testName, TextButton testButton)
            {
                _prefs = prefs;
                _testChooser = testChooser;
                _testName = testName;
                _testButton = testButton;
            }

            public override void changed(ChangeEvent @event, Actor actor) {
                IApplicationListener test = GdxTests.newTest(_testName);
                DesktopWindowConfiguration winConfig = new DesktopWindowConfiguration();
                winConfig.setTitle(_testName);
                winConfig.setWindowedMode(640, 480);
                winConfig.setWindowPosition(((DesktopGraphics)Gdx.Graphics).getWindow().getPositionX() + 40,
                    ((DesktopGraphics)Gdx.Graphics).getWindow().getPositionY() + 40);
                winConfig.useVsync(false);
                ((DesktopApplication)Gdx.App).newWindow(new GdxTestWrapper(test, options.logGLErrors), winConfig);
                Console.WriteLine("Started test: " + _testName);
                _prefs.PutString("LastTest", _testName);
                _prefs.Flush();
                if (_testButton != _testChooser.lastClickedTestButton)
                {
                    _testButton.setColor(Color.Cyan);
                    if (_testChooser.lastClickedTestButton != null)
                    {
                        _testChooser.lastClickedTestButton.setColor(Color.White);
                    }
                    _testChooser.lastClickedTestButton = _testButton;
                }
            }
}

		public override void Render () {
			ScreenUtils.Clear(0, 0, 0, 1);
			stage.act();
			stage.draw();
		}

		public override void Resize (int width, int height) {
			stage.getViewport().Update(width, height, true);
		}

		public override void Dispose () {
			skin.Dispose();
			stage.Dispose();
		}
	}
}
