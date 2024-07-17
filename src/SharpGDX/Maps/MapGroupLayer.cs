using SharpGDX.Shims;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.GLUtils;

namespace SharpGDX.Maps;

/** Map layer containing a set of MapLayers, objects and properties */
public class MapGroupLayer : MapLayer {

	private MapLayers layers = new MapLayers();

	/** @return the {@link MapLayers} owned by this group */
	public MapLayers getLayers () {
		return layers;
	}

	public override void invalidateRenderOffset () {
		base.invalidateRenderOffset();
		for (int i = 0; i < layers.size(); i++) {
			MapLayer child = layers.get(i);
			child.invalidateRenderOffset();
		}
	}
}
