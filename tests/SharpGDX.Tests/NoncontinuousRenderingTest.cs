using SharpGDX.Tests.Utils;
using SharpGDX.Utils;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Actions;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Scenes.Scene2D.UI;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Utils.Viewports;
using SharpGDX.Shims;
using SharpGDX.Mathematics;
using SharpGDX.Graphics.GLUtils;

namespace SharpGDX.Tests;

public class NoncontinuousRenderingTest : GdxTest
{
	SpriteBatch batch;
	Texture texture;
	TextureRegion region;
	Stage stage;
	Skin skin;
	BitmapFont font;
	float elapsed;
	int colorCycle;

	public override void Create()
	{
		batch = new SpriteBatch();
		texture = new Texture("data/badlogic.jpg");
		region = new TextureRegion(texture);
		stage = new Stage(new ScreenViewport(), batch);
		Gdx.Input.setInputProcessor(stage);

		skin = new Skin(Gdx.Files.Internal("data/uiskin.json"));
		skin.add("default", font = new BitmapFont(Gdx.Files.Internal("data/lsans-32.fnt"), false));

		populateTable();

		Gdx.Graphics.setContinuousRendering(false);
		Gdx.Graphics.requestRendering();
	}

	void nextColor()
	{
		lock (this)
		{
			colorCycle = (colorCycle + 1) % 3;
		}
	}

	public override void Render()
	{
		float delta = Math.Min(Gdx.Graphics.getDeltaTime(), 1 / 30f);
		elapsed += delta;
		float value = elapsed % 1f;
		value = value < 0.5f ? Interpolation.fade.apply(2 * value) : 1 - Interpolation.fade.apply(2 * value - 1);
		value = 0.2f + value * 0.8f; // avoid black

		lock (this)
		{
			switch (colorCycle)
			{
				case 0:
					Gdx.GL.glClearColor(value, 0, 0, 1);
					break;
				case 1:
					Gdx.GL.glClearColor(0, value, 0, 1);
					break;
				case 2:
					Gdx.GL.glClearColor(0, 0, value, 1);
					break;
			}
		}

		Gdx.GL.glClear(IGL20.GL_COLOR_BUFFER_BIT);

		Camera cam = stage.getCamera();
		batch.setProjectionMatrix(cam.combined);
		batch.begin();
		batch.draw(region, cam.position.x - texture.getWidth() / 2, cam.position.y - texture.getHeight() / 2,
			texture.getWidth() / 2f, texture.getHeight() / 2f, (float)texture.getWidth(), (float)texture.getHeight(), 1f, 1f,
			-((elapsed / 2f) % 1f) * 360f);
		batch.end();

		stage.act(delta);
		stage.draw();
	}

    private class Button0ChangeListener : ChangeListener
    {
        public override void changed(ChangeEvent @event, Actor actor)
        {
            bool continuous = Gdx.Graphics.isContinuousRendering();
            Gdx.Graphics.setContinuousRendering(!continuous);
        }
    }

    private class Button1ChangeListener : ChangeListener
    {
        private readonly NoncontinuousRenderingTest _test;
        private readonly string _str;

        public Button1ChangeListener(NoncontinuousRenderingTest test, string str)
        {
            _test = test;
            _str = str;
        }
        public override void changed(ChangeEvent @event, Actor actor) {
            new Thread(() =>
                {

                    try
                    {
                    Thread.Sleep(2000);
                }
                catch (ThreadInterruptedException x)
            {
            }
            _test.nextColor();
            Gdx.App.postRunnable(() =>
            {
                Gdx.App.log(_str, "Posted runnable to Gdx.app");


            });
                }).Start();

        }
}

    private class Button2ChangeListener : ChangeListener
    {
        private readonly NoncontinuousRenderingTest _test;
        private readonly string _str2;

        public Button2ChangeListener(NoncontinuousRenderingTest test, string str2)
        {
            _test = test;
            _str2 = str2;
        }
        public override void changed(ChangeEvent @event, Actor actor)
        {
            IGraphics graphics = Gdx.Graphics; // caching necessary to ensure call on this window
            new Thread(() =>
                {

                    try
                    {
                    Thread.Sleep(2000);
                }
                catch (ThreadInterruptedException ignored)
            {
            }
            _test.nextColor();
            graphics.requestRendering();
            Gdx.App.log(_str2, "Called Gdx.graphics.requestRendering()");
            
            }).Start();

        }
}

    private class Button3ChangeListener : ChangeListener
    {
        private readonly NoncontinuousRenderingTest _test;
        private readonly string _str3;

        public Button3ChangeListener(NoncontinuousRenderingTest test, string str3)
        {
            _test = test;
            _str3 = str3;
        }

        public override void changed(ChangeEvent @event, Actor actor)
        {
			SharpGDX.Utils.Timer.schedule(new Task(_test, _str3), 2f);
        }

        private class Task : SharpGDX.Utils.Timer.Task
        {
            private readonly NoncontinuousRenderingTest _test;
            private readonly string _str3;

            public Task(NoncontinuousRenderingTest test, string str3)
            {
                _test = test;
                _str3 = str3;
            }

            public override void run()
            {
                _test.nextColor();
                Gdx.App.postRunnable(() =>
                {

                    Gdx.App.log(_str3, "Posted runnable to Gdx.app");

                });
            }
        }
}

    private class Button4ChangeListener : ChangeListener
    {
        private readonly NoncontinuousRenderingTest _test;
        private readonly Stage _stage;
        private readonly string _str4;

        public Button4ChangeListener(NoncontinuousRenderingTest test, Stage stage, string str4)
        {
            _test = test;
            _stage = stage;
            _str4 = str4;
        }

        public override void changed(ChangeEvent @event, Actor actor)
        {
            _stage.addAction(Actions.sequence(Actions.delay(2), Actions.run(() => {

                
                _test.nextColor();
                Gdx.App.log(_str4, "RunnableAction executed");
            
            })));
        }
    }

    private class Button5ChangeListener : ChangeListener
    {
        private readonly NoncontinuousRenderingTest _test;
        private readonly Stage _stage;
        private readonly string _str5;

        public Button5ChangeListener(NoncontinuousRenderingTest test, Stage stage, string str5)
        {
            _test = test;
            _stage = stage;
            _str5 = str5;
        }

        public override void changed(ChangeEvent @event, Actor actor)
        {
            IGraphics graphics = Gdx.Graphics; // caching necessary to ensure call on this window
            new Thread(() =>
                {
                for (int i = 0; i < 2; i++)
                    {
                    try
                    {
                    Thread.Sleep(2000);
                }
                catch (ThreadInterruptedException ignored)
            {
            }
            _test.nextColor();
            bool continuous = graphics.isContinuousRendering();
            graphics.setContinuousRendering(!continuous);
            Gdx.App.log(_str5, "Toggled continuous");
            }
            
            }).Start();

        }
}

    private class CheckBoxChangeListener : ChangeListener
    {
        private readonly Stage _stage;
        private readonly CheckBox _actionsRequestRendering;

        public CheckBoxChangeListener(Stage stage, CheckBox actionsRequestRendering)
        {
            _stage = stage;
            _actionsRequestRendering = actionsRequestRendering;
        }
        public override void changed(ChangeEvent @event, Actor actor)
        {
            _stage.setActionsRequestRendering(_actionsRequestRendering.isChecked());
        }
        }

    private void populateTable()
	{
		Table root = new Table();
		stage.addActor(root);
		root.setFillParent(true);
		root.pad(5);
		root.defaults().Left().Space(5);

		Button button0 = new TextButton("Toggle continuous rendering", skin, "toggle");
		button0.addListener(new Button0ChangeListener());
		root.add(button0).Row();

	String str1 = "2s sleep -> Application.postRunnable()";
		Button button1 = new TextButton(str1, skin);
	button1.addListener(new Button1ChangeListener(this, str1));
root.add(button1).Row();

String str2 = "2s sleep -> Graphics.requestRendering()";
Button button2 = new TextButton(str2, skin);
button2.addListener(new Button2ChangeListener(this, str2));
root.add(button2).Row();

String str3 = "2s Timer -> Application.postRunnable()";
Button button3 = new TextButton(str3, skin);
button3.addListener(new Button3ChangeListener(this, str3));
root.add(button3).Row();

String str4 = "2s DelayAction";
Button button4 = new TextButton(str4, skin);
button4.addListener(new Button4ChangeListener(this, stage, str4));
root.add(button4).Row();

String str5 = "(2s sleep -> toggle continuous) 2X";
Button button5 = new TextButton(str5, skin);
button5.addListener(new Button5ChangeListener(this, stage, str5));
root.add(button5).Row();

CheckBox actionsRequestRendering = new CheckBox("ActionsRequestRendering", skin);
actionsRequestRendering.setChecked(true);
actionsRequestRendering.addListener(new CheckBoxChangeListener(stage, actionsRequestRendering));
root.add(actionsRequestRendering).Row();

IDrawable knobDown = skin.newDrawable("default-slider-knob", Color.GRAY);
Slider.SliderStyle sliderStyle = skin.get<Slider.SliderStyle>("default-horizontal", typeof(Slider.SliderStyle));
sliderStyle.knobDown = knobDown;
Slider slider = new Slider(0, 100, 1, false, sliderStyle);
root.add(slider).Row();

SelectBox<Pixmap.Format> selectBox = new (skin);
selectBox.setItems(Enum.GetValues<Pixmap.Format>());
root.add(selectBox).Row();

root.add();
root.add().Grow();
	}

	public override void Resize(int width, int height)
{
	stage.getViewport().Update(width, height, true);
}

public override void Dispose()
{
	batch.Dispose();
	texture.Dispose();
	stage.Dispose();
	font.Dispose();
}
}
