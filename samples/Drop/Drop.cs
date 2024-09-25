using SharpGDX;
using SharpGDX.Graphics.G2D;
using SharpGDX.Mathematics;

namespace Drop;

public class Drop : Game
{
	internal SpriteBatch? Batch { get; private set; }
	internal BitmapFont? Font { get; private set; }

	public override void Create()
	{
		Batch = new SpriteBatch();
		Font = new BitmapFont();

		SetScreen(new MainMenuScreen(this));
	}

	public override void Dispose()
	{
		Batch?.dispose();
		Font?.dispose();
	}
}