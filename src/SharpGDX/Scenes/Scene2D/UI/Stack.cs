using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A stack is a container that sizes its children to its size and positions them at 0,0 on top of each other.
 * <p>
 * The preferred and min size of the stack is the largest preferred and min size of any children. The max size of the stack is the
 * smallest max size of any children.
 * @author Nathan Sweet */
public class Stack : WidgetGroup {
	private float prefWidth, prefHeight, minWidth, minHeight, maxWidth, maxHeight;
	private bool sizeInvalid = true;

	public Stack () {
		setTransform(false);
		setWidth(150);
		setHeight(150);
		setTouchable(Touchable.childrenOnly);
	}

	public Stack (Actor[] actors) {
		foreach (Actor actor in actors)
			addActor(actor);
	}

	public override void invalidate () {
		base.invalidate();
		sizeInvalid = true;
	}

	private void computeSize () {
		sizeInvalid = false;
		prefWidth = 0;
		prefHeight = 0;
		minWidth = 0;
		minHeight = 0;
		maxWidth = 0;
		maxHeight = 0;
		SnapshotArray<Actor> children = getChildren();
		for (int i = 0, n = children.size; i < n; i++) {
			Actor child = children.get(i);
			float childMaxWidth, childMaxHeight;
			if (child is ILayout) {
				ILayout layout = (ILayout)child;
				prefWidth = Math.Max(prefWidth, layout.getPrefWidth());
				prefHeight = Math.Max(prefHeight, layout.getPrefHeight());
				minWidth = Math.Max(minWidth, layout.getMinWidth());
				minHeight = Math.Max(minHeight, layout.getMinHeight());
				childMaxWidth = layout.getMaxWidth();
				childMaxHeight = layout.getMaxHeight();
			} else {
				prefWidth = Math.Max(prefWidth, child.getWidth());
				prefHeight = Math.Max(prefHeight, child.getHeight());
				minWidth = Math.Max(minWidth, child.getWidth());
				minHeight = Math.Max(minHeight, child.getHeight());
				childMaxWidth = 0;
				childMaxHeight = 0;
			}
			if (childMaxWidth > 0) maxWidth = maxWidth == 0 ? childMaxWidth : Math.Min(maxWidth, childMaxWidth);
			if (childMaxHeight > 0) maxHeight = maxHeight == 0 ? childMaxHeight : Math.Min(maxHeight, childMaxHeight);
		}
	}

	public void add (Actor actor) {
		addActor(actor);
	}

	public override void layout () {
		if (sizeInvalid) computeSize();
		float width = getWidth(), height = getHeight();
		Array<Actor> children = getChildren();
		for (int i = 0, n = children.size; i < n; i++) {
			Actor child = children.get(i);
			child.setBounds(0, 0, width, height);
			if (child is ILayout) ((ILayout)child).validate();
		}
	}

	public override float getPrefWidth () {
		if (sizeInvalid) computeSize();
		return prefWidth;
	}

	public override float getPrefHeight () {
		if (sizeInvalid) computeSize();
		return prefHeight;
	}

	public override float getMinWidth () {
		if (sizeInvalid) computeSize();
		return minWidth;
	}

	public override float getMinHeight () {
		if (sizeInvalid) computeSize();
		return minHeight;
	}

	public override float getMaxWidth () {
		if (sizeInvalid) computeSize();
		return maxWidth;
	}

	public override float getMaxHeight () {
		if (sizeInvalid) computeSize();
		return maxHeight;
	}
}
