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

public interface ITiledMapRenderer : IMapRenderer {
	public void renderObjects (MapLayer layer);

	public void renderObject (MapObject obj);

	public void renderTileLayer (TiledMapTileLayer layer);

	public void renderImageLayer (TiledMapImageLayer layer);
}
