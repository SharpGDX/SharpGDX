using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;

namespace SharpGDX.Scenes.Scene2D.UI;

/** An {@link Actor} that participates in layout and provides a minimum, preferred, and maximum size.
 * <p>
 * The default preferred size of a widget is 0 and this is almost always overridden by a subclass. The default minimum size
 * returns the preferred size, so a subclass may choose to return 0 if it wants to allow itself to be sized smaller. The default
 * maximum size is 0, which means no maximum size.
 * <p>
 * See {@link Layout} for details on how a widget should participate in layout. A widget's mutator methods should call
 * {@link #invalidate()} or {@link #invalidateHierarchy()} as needed.
 * @author mzechner
 * @author Nathan Sweet */
public class Widget : Actor , ILayout {
	private bool _needsLayout = true;
	private bool fillParent;
	private bool layoutEnabled = true;

	public virtual float getMinWidth () {
		return getPrefWidth();
	}

	public virtual float getMinHeight () {
		return getPrefHeight();
	}

	public virtual float getPrefWidth () {
		return 0;
	}

	public virtual float getPrefHeight () {
		return 0;
	}

	public float getMaxWidth () {
		return 0;
	}

	public float getMaxHeight () {
		return 0;
	}

	public void setLayoutEnabled (bool enabled) {
		layoutEnabled = enabled;
		if (enabled) invalidateHierarchy();
	}

	public void validate () {
		if (!layoutEnabled) return;

		Group parent = getParent();
		if (fillParent && parent != null) {
			float parentWidth, parentHeight;
			Stage stage = getStage();
			if (stage != null && parent == stage.getRoot()) {
				parentWidth = stage.getWidth();
				parentHeight = stage.getHeight();
			} else {
				parentWidth = parent.getWidth();
				parentHeight = parent.getHeight();
			}
			setSize(parentWidth, parentHeight);
		}

		if (!_needsLayout) return;
		_needsLayout = false;
		layout();
	}

	/** Returns true if the widget's layout has been {@link #invalidate() invalidated}. */
	public bool needsLayout () {
		return _needsLayout;
	}

	public virtual void invalidate () {
		_needsLayout = true;
	}

	public void invalidateHierarchy () {
		if (!layoutEnabled) return;
		invalidate();
		Group parent = getParent();
		if (parent is ILayout) ((ILayout)parent).invalidateHierarchy();
	}

	protected override void sizeChanged () {
		invalidate();
	}

	public void pack () {
		setSize(getPrefWidth(), getPrefHeight());
		validate();
	}

	public void setFillParent (bool fillParent) {
		this.fillParent = fillParent;
	}

	/** If this method is overridden, the super method or {@link #validate()} should be called to ensure the widget is laid out. */
	public override void draw (IBatch batch, float parentAlpha) {
		validate();
	}

	public virtual void layout () {
	}
}
