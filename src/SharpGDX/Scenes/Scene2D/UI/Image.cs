using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** Displays a {@link Drawable}, scaled various way within the widgets bounds. The preferred size is the min size of the drawable.
 * Only when using a {@link TextureRegionDrawable} will the actor's scale, rotation, and origin be used when drawing.
 * @author Nathan Sweet */
public class Image : Widget {
	private Scaling scaling;
	private int align = Align.center;
	private float imageX, imageY, imageWidth, imageHeight;
	private IDrawable? drawable;

	/** Creates an image with no drawable, stretched, and aligned center. */
	public Image () 
	: this((IDrawable?)null)
	{
		
	}

	/** Creates an image stretched, and aligned center.
	 * @param patch May be null. */
	public Image (NinePatch? patch) 
	: this(new NinePatchDrawable(patch), Scaling.stretch, Align.center)
	{
		
	}

	/** Creates an image stretched, and aligned center.
	 * @param region May be null. */
	public Image (TextureRegion? region) 
	: this(new TextureRegionDrawable(region), Scaling.stretch, Align.center)
	{
		
	}

	/** Creates an image stretched, and aligned center. */
	public Image (Texture texture) 
	: this(new TextureRegionDrawable(new TextureRegion(texture)))
	{
		
	}

	/** Creates an image stretched, and aligned center. */
	public Image (Skin skin, String drawableName) 
	: this(skin.getDrawable(drawableName), Scaling.stretch, Align.center)
	{
		
	}

	/** Creates an image stretched, and aligned center.
	 * @param drawable May be null. */
	public Image (IDrawable? drawable) 
	: this(drawable, Scaling.stretch, Align.center)
	{
		
	}

	/** Creates an image aligned center.
	 * @param drawable May be null. */
	public Image (IDrawable? drawable, Scaling scaling) 
	: this(drawable, scaling, Align.center)
	{
		
	}

	/** @param drawable May be null. */
	public Image (IDrawable? drawable, Scaling scaling, int align) {
		setDrawable(drawable);
		this.scaling = scaling;
		this.align = align;
		setSize(getPrefWidth(), getPrefHeight());
	}

	public override void layout () {
		if (drawable == null) return;

		float regionWidth = drawable.getMinWidth();
		float regionHeight = drawable.getMinHeight();
		float width = getWidth();
		float height = getHeight();

		Vector2 size = scaling.apply(regionWidth, regionHeight, width, height);
		imageWidth = size.x;
		imageHeight = size.y;

		if ((align & Align.left) != 0)
			imageX = 0;
		else if ((align & Align.right) != 0)
			imageX = (int)(width - imageWidth);
		else
			imageX = (int)(width / 2 - imageWidth / 2);

		if ((align & Align.top) != 0)
			imageY = (int)(height - imageHeight);
		else if ((align & Align.bottom) != 0)
			imageY = 0;
		else
			imageY = (int)(height / 2 - imageHeight / 2);
	}

	public override void draw (IBatch batch, float parentAlpha) {
		validate();

		Color color = getColor();
		batch.setColor(color.r, color.g, color.b, color.a * parentAlpha);

		float x = getX();
		float y = getY();
		float scaleX = getScaleX();
		float scaleY = getScaleY();

		if (drawable is ITransformDrawable) {
			float rotation = getRotation();
			if (scaleX != 1 || scaleY != 1 || rotation != 0) {
				((ITransformDrawable)drawable).draw(batch, x + imageX, y + imageY, getOriginX() - imageX, getOriginY() - imageY,
					imageWidth, imageHeight, scaleX, scaleY, rotation);
				return;
			}
		}
		if (drawable != null) drawable.draw(batch, x + imageX, y + imageY, imageWidth * scaleX, imageHeight * scaleY);
	}

	public void setDrawable (Skin skin, String drawableName) {
		setDrawable(skin.getDrawable(drawableName));
	}

	/** Sets a new drawable for the image. The image's pref size is the drawable's min size. If using the image actor's size rather
	 * than the pref size, {@link #pack()} can be used to size the image to its pref size.
	 * @param drawable May be null. */
	public void setDrawable (IDrawable? drawable) {
		if (this.drawable == drawable) return;
		if (drawable != null) {
			if (getPrefWidth() != drawable.getMinWidth() || getPrefHeight() != drawable.getMinHeight()) invalidateHierarchy();
		} else
			invalidateHierarchy();
		this.drawable = drawable;
	}

	/** @return May be null. */
	public IDrawable? getDrawable () {
		return drawable;
	}

	public void setScaling (Scaling scaling) {
		if (scaling == null) throw new IllegalArgumentException("scaling cannot be null.");
		this.scaling = scaling;
		invalidate();
	}

	public void setAlign (int align) {
		this.align = align;
		invalidate();
	}

	public int getAlign () {
		return align;
	}

	public override float getMinWidth () {
		return 0;
	}

	public override float getMinHeight () {
		return 0;
	}

	public override float getPrefWidth () {
		if (drawable != null) return drawable.getMinWidth();
		return 0;
	}

	public override float getPrefHeight () {
		if (drawable != null) return drawable.getMinHeight();
		return 0;
	}

	public float getImageX () {
		return imageX;
	}

	public float getImageY () {
		return imageY;
	}

	public float getImageWidth () {
		return imageWidth;
	}

	public float getImageHeight () {
		return imageHeight;
	}

	public override String ToString () {
		String name = getName();
		if (name != null) return name;
		String className = GetType().Name;
		int dotIndex = className.LastIndexOf('.');
		if (dotIndex != -1) className = className.Substring(dotIndex + 1);
		return (className.IndexOf('$') != -1 ? "Image " : "") + className + ": " + drawable;
	}
}
