using SharpGDX;
using SharpGDX.Audio;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace Drop;

public class Drop : Game
{

internal SpriteBatch batch;
internal BitmapFont font;

public override void Create()
{
	batch = new SpriteBatch();
	// Use LibGDX's default Arial font.
	font = new BitmapFont();
	this.SetScreen(new MainMenuScreen(this));
}

public override void Render()
{
	base.Render(); // important!
}

public override void Dispose()
{
	batch.dispose();
	font.dispose();
}

}