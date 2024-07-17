using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX;
using SharpGDX.Audio;
using SharpGDX.Desktop.Audio;
using SharpGDX.Graphics;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace Drop
{
	public class GameScreen : IScreen {

	readonly Drop game;

	Texture dropImage;
	Texture bucketImage;
	ISound dropSound;
	IMusic rainMusic;
	OrthographicCamera camera;
	Rectangle bucket;
	List<Rectangle> raindrops;
	long lastDropTime;
	int dropsGathered;

	public GameScreen(Drop gam) {
		this.game = gam;

		// load the images for the droplet and the bucket, 64x64 pixels each
		dropImage = new Texture(Gdx.files.@internal("assets/droplet.png"));
		bucketImage = new Texture(Gdx.files.@internal("assets/bucket.png"));

		// load the drop sound effect and the rain background "music"
		dropSound = Gdx.audio.newSound(Gdx.files.@internal("assets/drop.wav"));
		rainMusic = Gdx.audio.newMusic(Gdx.files.@internal("assets/rain.wav"));
		rainMusic.setLooping(true);

		// create the camera and the SpriteBatch
		camera = new OrthographicCamera();
		camera.setToOrtho(false, 800, 480);

		// create a Rectangle to logically represent the bucket
		bucket = new Rectangle();
		bucket.x = 800 / 2 - 64 / 2; // center the bucket horizontally
		bucket.y = 20; // bottom left corner of the bucket is 20 pixels above
						// the bottom screen edge
		bucket.width = 64;
		bucket.height = 64;

		// create the raindrops array and spawn the first raindrop
		raindrops = new List<Rectangle>();
		spawnRaindrop();

	}

	private void spawnRaindrop() {
		Rectangle raindrop = new Rectangle();
		raindrop.x = MathUtils.random(0, 800 - 64);
		raindrop.y = 480;
		raindrop.width = 64;
		raindrop.height = 64;
		raindrops.Add(raindrop);
		lastDropTime = TimeUtils.nanoTime();
	}

	public void Render(float delta)
	{
		// clear the screen with a dark blue color. The
		// arguments to clear are the red, green
		// blue and alpha component in the range [0,1]
		// of the color to be used to clear the screen.
		ScreenUtils.clear(0, 0, 0.2f, 1);

		// tell the camera to update its matrices.
		camera.update();

		// tell the SpriteBatch to render in the
		// coordinate system specified by the camera.
		game.batch.setProjectionMatrix(camera.combined);

		// begin a new batch and draw the bucket and
		// all drops
		game.batch.begin();
		game.font.draw(game.batch, "Drops Collected: " + dropsGathered, 0, 480);
		game.batch.draw(bucketImage, bucket.x, bucket.y);
		foreach (Rectangle raindrop in raindrops)
		{
			game.batch.draw(dropImage, raindrop.x, raindrop.y);
		}

		game.batch.end();

		// process user input
		if (Gdx.input.isTouched())
		{
			Vector3 touchPos = new Vector3();
			touchPos.set(Gdx.input.getX(), Gdx.input.getY(), 0);
			camera.unproject(touchPos);
			bucket.x = touchPos.x - 64 / 2;
		}

		if (Gdx.input.isKeyPressed(IInput.Keys.LEFT))
			bucket.x -= 200 * Gdx.graphics.getDeltaTime();
		if (Gdx.input.isKeyPressed(IInput.Keys.RIGHT))
			bucket.x += 200 * Gdx.graphics.getDeltaTime();

		// make sure the bucket stays within the screen bounds
		if (bucket.x < 0)
			bucket.x = 0;
		if (bucket.x > 800 - 64)
			bucket.x = 800 - 64;

		// check if we need to create a new raindrop
		if (TimeUtils.nanoTime() - lastDropTime > 1000000000)
			spawnRaindrop();

		// move the raindrops, remove any that are beneath the bottom edge of
		// the screen or that hit the bucket. In the later case we play back
		// a sound effect as well.
		for (var i = raindrops.Count - 1; i >= 0; i--)
		{
			var raindrop = raindrops[i];
			raindrop.y -= 200 * Gdx.graphics.getDeltaTime();
			if (raindrop.y + 64 < 0)
				raindrops.RemoveAt(i);
			if (raindrop.overlaps(bucket))
			{
				dropsGathered++;
				dropSound.play();
				raindrops.RemoveAt(i);
			}
		}
	}

	public void Resize(int width, int height) {
	}

	public void Show() {
		// start the playback of the background music
		// when the screen is shown
		rainMusic.play();
	}

	public void Hide() {
	}

	public void Pause() {
	}

	public void Resume() {
	}

	public void Dispose() {
		dropImage.dispose();
		bucketImage.dispose();
		dropSound.dispose();
		rainMusic.dispose();
	}

}
}
