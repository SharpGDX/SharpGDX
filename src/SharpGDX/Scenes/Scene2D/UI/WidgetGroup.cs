using System;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Utils;
using SharpGDX.Shims;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.UI
{
	/** A {@link Group} that participates in layout and provides a minimum, preferred, and maximum size.
 * <p>
 * The default preferred size of a widget group is 0 and this is almost always overridden by a subclass. The default minimum size
 * returns the preferred size, so a subclass may choose to return 0 for minimum size if it wants to allow itself to be sized
 * smaller than the preferred size. The default maximum size is 0, which means no maximum size.
 * <p>
 * See {@link Layout} for details on how a widget group should participate in layout. A widget group's mutator methods should call
 * {@link #invalidate()} or {@link #invalidateHierarchy()} as needed. By default, invalidateHierarchy is called when child widgets
 * are added and removed.
 * @author Nathan Sweet */
public class WidgetGroup : Group , ILayout {
	private bool _needsLayout = true;
	private bool fillParent;
	private bool layoutEnabled = true;

	public WidgetGroup () {
	}

	/** Creates a new widget group containing the specified actors. */
	public WidgetGroup (Actor[] actors) {
		foreach (Actor actor in actors)
			addActor(actor);
	}

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

	public virtual float getMaxWidth () {
		return 0;
	}

	public virtual float getMaxHeight () {
		return 0;
	}

	public void setLayoutEnabled (bool enabled) {
		layoutEnabled = enabled;
		setLayoutEnabled(this, enabled);
	}

	private void setLayoutEnabled (Group parent, bool enabled) {
		SnapshotArray<Actor> children = parent.getChildren();
		for (int i = 0, n = children.size; i < n; i++) {
			Actor actor = children.get(i);
			if (actor is ILayout)
				((ILayout)actor).setLayoutEnabled(enabled);
			else if (actor is Group) //
				setLayoutEnabled((Group)actor, enabled);
		}
	}

	public void validate () {
		if (!layoutEnabled) return;

		Group parent = getParent();
		if (fillParent && parent != null) {
			Stage stage = getStage();
			if (stage != null && parent == stage.getRoot())
				setSize(stage.getWidth(), stage.getHeight());
			else
				setSize(parent.getWidth(), parent.getHeight());
		}

		if (!_needsLayout) return;
		_needsLayout = false;
		layout();

		// Widgets may call invalidateHierarchy during layout (eg, a wrapped label). The root-most widget group retries layout a
		// reasonable number of times.
		if (_needsLayout) {
			if (parent is WidgetGroup) return; // The parent widget will layout again.
			for (int i = 0; i < 5; i++) {
				_needsLayout = false;
				layout();
				if (!_needsLayout) break;
			}
		}
	}

	/** Returns true if the widget's layout has been {@link #invalidate() invalidated}. */
	public bool needsLayout () {
		return _needsLayout;
	}

		public virtual void invalidate () {
		_needsLayout = true;
	}

	public void invalidateHierarchy () {
		invalidate();
		Group parent = getParent();
		if (parent is ILayout) ((ILayout)parent).invalidateHierarchy();
	}

		protected override void childrenChanged () {
		invalidateHierarchy();
	}

		protected override void sizeChanged () {
		invalidate();
	}

	public void pack () {
		setSize(getPrefWidth(), getPrefHeight());
		validate();
		// Validating the layout may change the pref size. Eg, a wrapped label doesn't know its pref height until it knows its
		// width, so it calls invalidateHierarchy() in layout() if its pref height has changed.
		setSize(getPrefWidth(), getPrefHeight());
		validate();
	}

	public void setFillParent (bool fillParent) {
		this.fillParent = fillParent;
	}

	public virtual void layout () {
	}

		/** If this method is overridden, the super method or {@link #validate()} should be called to ensure the widget group is laid
		 * out. */
		public override Actor hit (float x, float y, bool touchable) {
		validate();
		return base.hit(x, y, touchable);
	}

		/** If this method is overridden, the super method or {@link #validate()} should be called to ensure the widget group is laid
		 * out. */
		public override void draw (IBatch batch, float parentAlpha) {
		validate();
		base.draw(batch, parentAlpha);
	}
}
}
