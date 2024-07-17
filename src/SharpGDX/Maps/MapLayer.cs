using SharpGDX.Shims;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.GLUtils;

namespace SharpGDX.Maps;

/** Map layer containing a set of objects and properties */
public class MapLayer {
	private String name = "";
	private float opacity = 1.0f;
	private bool visible = true;
	private float offsetX;
	private float offsetY;
	private float renderOffsetX;
	private float renderOffsetY;
	private float parallaxX = 1;
	private float parallaxY = 1;
	private bool renderOffsetDirty = true;
	private MapLayer parent;
	private MapObjects objects = new MapObjects();
	private MapProperties properties = new MapProperties();

	/** @return layer's name */
	public String getName () {
		return name;
	}

	/** @param name new name for the layer */
	public void setName (String name) {
		this.name = name;
	}

	/** @return layer's opacity */
	public float getOpacity () {
		return opacity;
	}

	/** @param opacity new opacity for the layer */
	public void setOpacity (float opacity) {
		this.opacity = opacity;
	}

	/** @return layer's x offset */
	public float getOffsetX () {
		return offsetX;
	}

	/** @param offsetX new x offset for the layer */
	public void setOffsetX (float offsetX) {
		this.offsetX = offsetX;
		invalidateRenderOffset();
	}

	/** @return layer's y offset */
	public float getOffsetY () {
		return offsetY;
	}

	/** @param offsetY new y offset for the layer */
	public void setOffsetY (float offsetY) {
		this.offsetY = offsetY;
		invalidateRenderOffset();
	}

	/** @return layer's parallax scrolling factor for x-axis */
	public float getParallaxX () {
		return parallaxX;
	}

	public void setParallaxX (float parallaxX) {
		this.parallaxX = parallaxX;
	}

	/** @return layer's parallax scrolling factor for y-axis */
	public float getParallaxY () {
		return parallaxY;
	}

	public void setParallaxY (float parallaxY) {
		this.parallaxY = parallaxY;
	}

	/** @return the layer's x render offset, this takes into consideration all parent layers' offsets */
	public float getRenderOffsetX () {
		if (renderOffsetDirty) calculateRenderOffsets();
		return renderOffsetX;
	}

	/** @return the layer's y render offset, this takes into consideration all parent layers' offsets */
	public float getRenderOffsetY () {
		if (renderOffsetDirty) calculateRenderOffsets();
		return renderOffsetY;
	}

	/** set the renderOffsetDirty state to true, when this layer or any parents' offset has changed **/
	public virtual void invalidateRenderOffset () {
		renderOffsetDirty = true;
	}

	/** @return the layer's parent {@link MapLayer}, or null if the layer does not have a parent **/
	public MapLayer getParent () {
		return parent;
	}

	/** @param parent the layer's new parent {@MapLayer}, internal use only **/
	public void setParent (MapLayer parent) {
		if (parent == this) throw new GdxRuntimeException("Can't set self as the parent");
		this.parent = parent;
	}

	/** @return collection of objects contained in the layer */
	public MapObjects getObjects () {
		return objects;
	}

	/** @return whether the layer is visible or not */
	public bool isVisible () {
		return visible;
	}

	/** @param visible toggles layer's visibility */
	public void setVisible (bool visible) {
		this.visible = visible;
	}

	/** @return layer's set of properties */
	public MapProperties getProperties () {
		return properties;
	}

	protected void calculateRenderOffsets () {
		if (parent != null) {
			parent.calculateRenderOffsets();
			renderOffsetX = parent.getRenderOffsetX() + offsetX;
			renderOffsetY = parent.getRenderOffsetY() + offsetY;
		} else {
			renderOffsetX = offsetX;
			renderOffsetY = offsetY;
		}
		renderOffsetDirty = false;
	}
}
