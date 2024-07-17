using System.Collections;
using SharpGDX.Utils.Reflect;
using SharpGDX.Shims;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Mathematics;

namespace SharpGDX.Maps.Tiled;

public class TiledMapImageLayer : MapLayer {

	private TextureRegion region;

	private float x;
	private float y;

	public TiledMapImageLayer (TextureRegion region, float x, float y) {
		this.region = region;
		this.x = x;
		this.y = y;
	}

	public TextureRegion getTextureRegion () {
		return region;
	}

	public void setTextureRegion (TextureRegion region) {
		this.region = region;
	}

	public float getX () {
		return x;
	}

	public void setX (float x) {
		this.x = x;
	}

	public float getY () {
		return y;
	}

	public void setY (float y) {
		this.y = y;
	}

}
