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

namespace SharpGDX.Maps.Objects;

/** @brief Represents {@link Polyline} map objects */
public class PolylineMapObject : MapObject {

	private Polyline polyline;

	/** @return polyline shape */
	public Polyline getPolyline () {
		return polyline;
	}

	/** @param polyline new object's polyline shape */
	public void setPolyline (Polyline polyline) {
		this.polyline = polyline;
	}

	/** Creates empty polyline */
	public PolylineMapObject () 
	: this(new float[0])
	{
		
	}

	/** @param vertices polyline defining vertices */
	public PolylineMapObject (float[] vertices) {
		polyline = new Polyline(vertices);
	}

	/** @param polyline the polyline */
	public PolylineMapObject (Polyline polyline) {
		this.polyline = polyline;
	}

}
