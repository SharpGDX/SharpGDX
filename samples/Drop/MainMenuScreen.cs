using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX;
using SharpGDX.Graphics;
using SharpGDX.Utils;

namespace Drop
{
	public class MainMenuScreen : IScreen
	{

	readonly Drop game;
	OrthographicCamera camera;

	public MainMenuScreen(Drop gam)
	{
		game = gam;

		camera = new OrthographicCamera();
		camera.setToOrtho(false, 800, 480);
	}

	public void Render(float delta)
	{
		ScreenUtils.clear(0, 0, 0.2f, 1);

		camera.update();
		game.batch.setProjectionMatrix(camera.combined);

		game.batch.begin();
		game.font.draw(game.batch, "Welcome to Drop!!! ", 100, 150);
		game.font.draw(game.batch, "Tap anywhere to begin!", 100, 100);

			game.batch.end();

		if (Gdx.input.isTouched())
		{
			game.SetScreen(new GameScreen(game));
			Dispose();
		}
	}

	public void Resize(int width, int height)
	{
	}

	public void Show()
	{
	}

	public void Hide()
	{
	}

	public void Pause()
	{
	}

	public void Resume()
	{
	}

	public void Dispose()
	{
	}
	}
}
