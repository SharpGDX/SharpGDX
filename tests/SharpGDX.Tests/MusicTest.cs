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
//using SharpGDX.Audio;
//
//namespace SharpGDX.Tests;
//
//public class MusicTest : GdxTest {
//
//	IMusic music;
//	float songDuration;
//	float currentPosition;
//
//	SpriteBatch batch;
//
//	Stage stage;
//	Label label;
//	Slider slider;
//	bool sliderUpdating = false;
//	SelectBox<Song> musicBox;
//	TextButton btLoop;
//
//	enum Song {
//		MP3, OGG, WAV, PCM8, MP3_CLOCK
//	}
//
//	float time;
//
//	public override void Create () {
//
//		batch = new SpriteBatch();
//
//		stage = new Stage(new ExtendViewport(600, 480));
//		Skin skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		Table sliderTable = new Table();
//		label = new Label("", skin);
//		slider = new Slider(0, 100, 0.1f, false, skin);
//		sliderTable.add(slider).expand();
//		sliderTable.add(label).left().width(60f);
//		slider.addListener(new ChangeListener() {
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				if (!sliderUpdating && slider.isDragging()) music.setPosition((slider.getValue() / 100f) * songDuration);
//			}
//		});
//
//		musicBox = new SelectBox<Song>(skin);
//		musicBox.setItems(Song.values());
//		musicBox.addListener(new ChangeListener() {
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				setSong(musicBox.getSelected());
//			}
//		});
//
//		btLoop = new TextButton("loop", skin, "toggle");
//		btLoop.setChecked(true);
//		btLoop.addListener(new ChangeListener() {
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				if (music != null) music.setLooping(btLoop.isChecked());
//			}
//		});
//
//		// Build buttons
//		Table controlsTable = new Table();
//		controlsTable.setSize(200f, 80f);
//		Button playButton = new ImageButton(getDrawable("data/player_play.png"));
//		Button pauseButton = new ImageButton(getDrawable("data/player_pause.png"));
//		Button stopButton = new ImageButton(getDrawable("data/player_stop.png"));
//		float buttonSize = 64f;
//		controlsTable.add(playButton).size(buttonSize);
//		controlsTable.add(pauseButton).size(buttonSize);
//		controlsTable.add(stopButton).size(buttonSize);
//		playButton.addListener(new ChangeListener() {
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				music.play();
//				time = 0;
//			}
//		});
//		pauseButton.addListener(new ChangeListener() {
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				music.pause();
//			}
//		});
//		stopButton.addListener(new ChangeListener() {
//			@Override
//			public void changed (ChangeEvent event, Actor actor) {
//				music.stop();
//			}
//		});
//
//		Table footerTable = new Table();
//		footerTable.setSize(500f, 120f);
//		footerTable.add(controlsTable);
//		footerTable.add(sliderTable).width(250f);
//
//		setSong(musicBox.getSelected());
//
//		Table table = new Table(skin);
//		table.add(musicBox);
//		table.add(btLoop);
//		table.setFillParent(true);
//		stage.addActor(table);
//		stage.addActor(footerTable);
//
//		Gdx.input.setInputProcessor(stage);
//	}
//
//	void setSong (Song song) {
//		if (music != null) {
//			music.Dispose();
//		}
//		switch (song) {
//		default:
//		case MP3_CLOCK:
//			music = Gdx.audio.newMusic(Gdx.files.@internal("data/60bpm.mp3"));
//			songDuration = 5 * 60 + 4;
//			break;
//		case MP3:
//			music = Gdx.audio.newMusic(Gdx.files.@internal("data/8.12.mp3"));
//			songDuration = 183;
//			break;
//		case OGG:
//			music = Gdx.audio.newMusic(Gdx.files.@internal("data/8.12.ogg"));
//			songDuration = 183;
//			break;
//		case WAV:
//			music = Gdx.audio.newMusic(Gdx.files.@internal("data/8.12.loop.wav"));
//			songDuration = 4;
//			break;
//		case PCM8:
//			music = Gdx.audio.newMusic(Gdx.files.@internal("data/8.12.loop-8bit.wav"));
//			songDuration = 4;
//			break;
//		}
//		music.setLooping(btLoop.isChecked());
//		music.play();
//		time = 0;
//	}
//
//	public override void Resize (int width, int height) {
//		stage.getViewport().update(width, height, true);
//	}
//
//	public override void Resume () {
//		Console.WriteLine(Gdx.graphics.getDeltaTime());
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(Color.BLACK);
//		currentPosition = music.getPosition();
//		label.setText((int)currentPosition / 60 + ":" + (int)currentPosition % 60);
//
//		sliderUpdating = true;
//		slider.setValue((currentPosition / songDuration) * 100f);
//		sliderUpdating = false;
//		stage.act();
//		stage.draw();
//
//// if(music.isPlaying()){
//// time += Gdx.graphics.getDeltaTime();
//// Console.WriteLine("realtime: " + time + " music time: " + currentPosition);
//// }
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		music.Dispose();
//	}
//
//	private Drawable getDrawable (String path) {
//		return new TextureRegionDrawable(new TextureRegion(new Texture(Gdx.files.@internal(path))));
//	}
//}
