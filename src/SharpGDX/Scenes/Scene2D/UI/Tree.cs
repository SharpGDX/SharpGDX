//using SharpGDX.Mathematics;
//using SharpGDX.Shims;
//using SharpGDX.Scenes.Scene2D.Utils;
//using SharpGDX.Utils;

//namespace SharpGDX.Scenes.Scene2D.UI;

///** A tree widget where each node has an icon, actor, and child nodes.
// * <p>
// * The preferred size of the tree is determined by the preferred size of the actors for the expanded nodes.
// * <p>
// * {@link ChangeEvent} is fired when the selected node changes.
// * @param <N> The type of nodes in the tree.
// * @param <V> The type of values for each node.
// * @author Nathan Sweet */
//public class Tree<N, V> : WidgetGroup
//where N :Tree<N,V>.Node
//{
//	static private readonly Vector2 tmp = new Vector2();

//	TreeStyle style;
//	readonly Array<N> rootNodes = new();
//	readonly Selection<N> selection;
//	float ySpacing = 4, iconSpacingLeft = 2, iconSpacingRight = 2, paddingLeft, paddingRight, indentSpacing;
//	private float prefWidth, prefHeight;
//	private bool sizeInvalid = true;
//	private N foundNode, overNode;
//	N rangeStart;
//	private ClickListener clickListener;

//	public Tree (Skin skin) 
//	: this(skin.get<TreeStyle>(typeof(TreeStyle)))
//	{
		
//	}

//	public Tree (Skin skin, String styleName) 
//	: this(skin.get<TreeStyle>(styleName, typeof(TreeStyle)))
//	{
		
//	}

//	public Tree (TreeStyle style) {
//		selection = new Selection<N>() {
//			protected void changed () {
//				switch (size()) {
//				case 0:
//					rangeStart = null;
//					break;
//				case 1:
//					rangeStart = first();
//					break;
//				}
//			}
//		};
//		selection.setActor(this);
//		selection.setMultiple(true);
//		setStyle(style);
//		initialize();
//	}

//	private void initialize () {
//		addListener(clickListener = new ClickListener() {
//			public void clicked (InputEvent event, float x, float y) {
//				N node = getNodeAt(y);
//				if (node == null) return;
//				if (node != getNodeAt(getTouchDownY())) return;
//				if (selection.getMultiple() && selection.notEmpty() && UIUtils.shift()) {
//					// Select range (shift).
//					if (rangeStart == null) rangeStart = node;
//					N rangeStart = Tree.this.rangeStart;
//					if (!UIUtils.ctrl()) selection.clear();
//					float start = rangeStart.actor.getY(), end = node.actor.getY();
//					if (start > end)
//						selectNodes(rootNodes, end, start);
//					else {
//						selectNodes(rootNodes, start, end);
//						selection.items().orderedItems().reverse();
//					}

//					selection.fireChangeEvent();
//					Tree.this.rangeStart = rangeStart;
//					return;
//				}
//				if (node.children.size > 0 && (!selection.getMultiple() || !UIUtils.ctrl())) {
//					// Toggle expanded if left of icon.
//					float rowX = node.actor.getX();
//					if (node.icon != null) rowX -= iconSpacingRight + node.icon.getMinWidth();
//					if (x < rowX) {
//						node.setExpanded(!node.expanded);
//						return;
//					}
//				}
//				if (!node.isSelectable()) return;
//				selection.choose(node);
//				if (!selection.isEmpty()) rangeStart = node;
//			}

//			public boolean mouseMoved (InputEvent event, float x, float y) {
//				setOverNode(getNodeAt(y));
//				return false;
//			}

//			public void enter (InputEvent event, float x, float y, int pointer, Actor fromActor) {
//				base.enter(event, x, y, pointer, fromActor);
//				setOverNode(getNodeAt(y));
//			}

//			public void exit (InputEvent event, float x, float y, int pointer, Actor? toActor) {
//				base.exit(event, x, y, pointer, toActor);
//				if (toActor == null || !toActor.isDescendantOf(Tree.this)) setOverNode(null);
//			}
//		});
//	}

//	public void setStyle (TreeStyle style) {
//		this.style = style;

//		// Reasonable default.
//		if (indentSpacing == 0) indentSpacing = plusMinusWidth();
//	}

//	public void add (N node) {
//		insert(rootNodes.size, node);
//	}

//	public void insert (int index, N node) {
//		if (node.parent != null) {
//			node.parent.remove(node);
//			node.parent = null;
//		} else {
//			int existingIndex = rootNodes.indexOf(node, true);
//			if (existingIndex != -1) {
//				if (existingIndex == index) return;
//				if (existingIndex < index) index--;
//				rootNodes.removeIndex(existingIndex);
//				int actorIndex = node.actor.getZIndex();
//				if (actorIndex != -1) node.removeFromTree(this, actorIndex);
//			}
//		}

//		rootNodes.insert(index, node);

//		int actorIndex;
//		if (index == 0)
//			actorIndex = 0;
//		else if (index < rootNodes.size - 1)
//			actorIndex = rootNodes.get(index + 1).actor.getZIndex();
//		else {
//			N before = rootNodes.get(index - 1);
//			actorIndex = before.actor.getZIndex() + before.countActors();
//		}
//		node.addToTree(this, actorIndex);
//	}

//	public void remove (N node) {
//		if (node.parent != null) {
//			node.parent.remove(node);
//			return;
//		}
//		if (!rootNodes.removeValue(node, true)) return;
//		int actorIndex = node.actor.getZIndex();
//		if (actorIndex != -1) node.removeFromTree(this, actorIndex);
//	}

//	/** Removes all tree nodes. */
//	public void clearChildren (boolean unfocus) {
//		base.clearChildren(unfocus);
//		setOverNode(null);
//		rootNodes.clear();
//		selection.clear();
//	}

//	public void invalidate () {
//		base.invalidate();
//		sizeInvalid = true;
//	}

//	private float plusMinusWidth () {
//		float width = Math.Max(style.plus.getMinWidth(), style.minus.getMinWidth());
//		if (style.plusOver != null) width = Math.Max(width, style.plusOver.getMinWidth());
//		if (style.minusOver != null) width = Math.Max(width, style.minusOver.getMinWidth());
//		return width;
//	}

//	private void computeSize () {
//		sizeInvalid = false;
//		prefWidth = plusMinusWidth();
//		prefHeight = 0;
//		computeSize(rootNodes, 0, prefWidth);
//		prefWidth += paddingLeft + paddingRight;
//	}

//	private void computeSize (Array<N> nodes, float indent, float plusMinusWidth) {
//		float ySpacing = this.ySpacing;
//		float spacing = iconSpacingLeft + iconSpacingRight;
//		for (int i = 0, n = nodes.size; i < n; i++) {
//			N node = nodes.get(i);
//			float rowWidth = indent + plusMinusWidth;
//			Actor actor = node.actor;
//			if (actor is Layout) {
//				Layout layout = (Layout)actor;
//				rowWidth += layout.getPrefWidth();
//				node.height = layout.getPrefHeight();
//			} else {
//				rowWidth += actor.getWidth();
//				node.height = actor.getHeight();
//			}
//			if (node.icon != null) {
//				rowWidth += spacing + node.icon.getMinWidth();
//				node.height = Math.Max(node.height, node.icon.getMinHeight());
//			}
//			prefWidth = Math.Max(prefWidth, rowWidth);
//			prefHeight += node.height + ySpacing;
//			if (node.expanded) computeSize(node.children, indent + indentSpacing, plusMinusWidth);
//		}
//	}

//	public void layout () {
//		if (sizeInvalid) computeSize();
//		layout(rootNodes, paddingLeft, getHeight() - ySpacing / 2, plusMinusWidth());
//	}

//	private float layout (Array<N> nodes, float indent, float y, float plusMinusWidth) {
//		float ySpacing = this.ySpacing;
//		float iconSpacingLeft = this.iconSpacingLeft;
//		float spacing = iconSpacingLeft + iconSpacingRight;
//		for (int i = 0, n = nodes.size; i < n; i++) {
//			N node = nodes.get(i);
//			float x = indent + plusMinusWidth;
//			if (node.icon != null)
//				x += spacing + node.icon.getMinWidth();
//			else
//				x += iconSpacingLeft;
//			if (node.actor is Layout) ((Layout)node.actor).pack();
//			y -= node.getHeight();
//			node.actor.setPosition(x, y);
//			y -= ySpacing;
//			if (node.expanded) y = layout(node.children, indent + indentSpacing, y, plusMinusWidth);
//		}
//		return y;
//	}

//	public void draw (Batch batch, float parentAlpha) {
//		drawBackground(batch, parentAlpha);
//		Color color = getColor();
//		float a = color.a * parentAlpha;
//		batch.setColor(color.r, color.g, color.b, a);
//		drawIcons(batch, color.r, color.g, color.b, a, null, rootNodes, paddingLeft, plusMinusWidth());
//		base.draw(batch, parentAlpha); // Draw node actors.
//	}

//	/** Called to draw the background. Default implementation draws the style background drawable. */
//	protected void drawBackground (Batch batch, float parentAlpha) {
//		if (style.background != null) {
//			Color color = getColor();
//			batch.setColor(color.r, color.g, color.b, color.a * parentAlpha);
//			style.background.draw(batch, getX(), getY(), getWidth(), getHeight());
//		}
//	}

//	/** Draws selection, icons, and expand icons.
//	 * @param parent null for the root nodes.
//	 * @return The Y position of the last visible actor for the nodes. */
//	protected float drawIcons (Batch batch, float r, float g, float b, float a,  N? parent, Array<N> nodes, float indent,
//		float plusMinusWidth) {

//		Rectangle cullingArea = getCullingArea();
//		float cullBottom = 0, cullTop = 0;
//		if (cullingArea != null) {
//			cullBottom = cullingArea.y;
//			cullTop = cullBottom + cullingArea.height;
//		}
//		TreeStyle style = this.style;
//		float x = getX(), y = getY(), expandX = x + indent, iconX = expandX + plusMinusWidth + iconSpacingLeft, actorY = 0;
//		for (int i = 0, n = nodes.size; i < n; i++) {
//			N node = nodes.get(i);
//			Actor actor = node.actor;
//			actorY = actor.getY();
//			float height = node.height;
//			if (cullingArea == null || (actorY + height >= cullBottom && actorY <= cullTop)) {
//				if (selection.contains(node) && style.selection != null) {
//					drawSelection(node, style.selection, batch, x, y + actorY - ySpacing / 2, getWidth(), height + ySpacing);
//				} else if (node == overNode && style.over != null) {
//					drawOver(node, style.over, batch, x, y + actorY - ySpacing / 2, getWidth(), height + ySpacing);
//				}

//				if (node.icon != null) {
//					float iconY = y + actorY + (float)Math.Round((height - node.icon.getMinHeight()) / 2);
//					Color actorColor = actor.getColor();
//					batch.setColor(actorColor.r, actorColor.g, actorColor.b, actorColor.a * a);
//					drawIcon(node, node.icon, batch, iconX, iconY);
//					batch.setColor(r, g, b, a);
//				}

//				if (node.children.size > 0) {
//					Drawable expandIcon = getExpandIcon(node, iconX);
//					float iconY = y + actorY + (float)Math.Round((height - expandIcon.getMinHeight()) / 2);
//					drawExpandIcon(node, expandIcon, batch, expandX, iconY);
//				}
//			} else if (actorY < cullBottom) //
//				break;
//			if (node.expanded && node.children.size > 0)
//				drawIcons(batch, r, g, b, a, node, node.children, indent + indentSpacing, plusMinusWidth);
//		}
//		return actorY;
//	}

//	protected void drawSelection (N node, Drawable selection, Batch batch, float x, float y, float width, float height) {
//		selection.draw(batch, x, y, width, height);
//	}

//	protected void drawOver (N node, Drawable over, Batch batch, float x, float y, float width, float height) {
//		over.draw(batch, x, y, width, height);
//	}

//	protected void drawExpandIcon (N node, Drawable expandIcon, Batch batch, float x, float y) {
//		expandIcon.draw(batch, x, y, expandIcon.getMinWidth(), expandIcon.getMinHeight());
//	}

//	protected void drawIcon (N node, Drawable icon, Batch batch, float x, float y) {
//		icon.draw(batch, x, y, icon.getMinWidth(), icon.getMinHeight());
//	}

//	/** Returns the drawable for the expand icon. The default implementation returns {@link TreeStyle#plusOver} or
//	 * {@link TreeStyle#minusOver} on the desktop if the node is the {@link #getOverNode() over node}, the mouse is left of
//	 * <code>iconX</code>, and clicking would expand the node.
//	 * @param iconX The X coordinate of the over node's icon. */
//	protected Drawable getExpandIcon (N node, float iconX) {
//		if (node == overNode //
//			&& Gdx.app.getType() == Application.ApplicationType.Desktop //
//			&& (!selection.getMultiple() || (!UIUtils.ctrl() && !UIUtils.shift())) //
//		) {
//			float mouseX = screenToLocalCoordinates(tmp.set(Gdx.input.getX(), 0)).x + getX();
//			if (mouseX >= 0 && mouseX < iconX) {
//				Drawable icon = node.expanded ? style.minusOver : style.plusOver;
//				if (icon != null) return icon;
//			}
//		}
//		return node.expanded ? style.minus : style.plus;
//	}

//	/** @return May be null. */
//	public N? getNodeAt (float y) {
//		foundNode = null;
//		getNodeAt(rootNodes, y, getHeight());
//		try {
//			return foundNode;
//		} finally {
//			foundNode = null;
//		}
//	}

//	private float getNodeAt (Array<N> nodes, float y, float rowY) {
//		for (int i = 0, n = nodes.size; i < n; i++) {
//			N node = nodes.get(i);
//			float height = node.height;
//			rowY -= node.getHeight() - height; // Node subclass may increase getHeight.
//			if (y >= rowY - height - ySpacing && y < rowY) {
//				foundNode = node;
//				return -1;
//			}
//			rowY -= height + ySpacing;
//			if (node.expanded) {
//				rowY = getNodeAt(node.children, y, rowY);
//				if (rowY == -1) return -1;
//			}
//		}
//		return rowY;
//	}

//	void selectNodes (Array<N> nodes, float low, float high) {
//		for (int i = 0, n = nodes.size; i < n; i++) {
//			N node = nodes.get(i);
//			if (node.actor.getY() < low) break;
//			if (!node.isSelectable()) continue;
//			if (node.actor.getY() <= high) selection.add(node);
//			if (node.expanded) selectNodes(node.children, low, high);
//		}
//	}

//	public Selection<N> getSelection () {
//		return selection;
//	}

//	/** Returns the first selected node, or null. */
//	public  N? getSelectedNode () {
//		return selection.first();
//	}

//	/** Returns the first selected value, or null. */
//	public V? getSelectedValue () {
//		N node = selection.first();
//		return node == null ? null : (V)node.getValue();
//	}

//	public TreeStyle getStyle () {
//		return style;
//	}

//	/** If the order of the root nodes is changed, {@link #updateRootNodes()} must be called to ensure the nodes' actors are in the
//	 * correct order. */
//	public Array<N> getRootNodes () {
//		return rootNodes;
//	}

//	/** Updates the order of the actors in the tree for all root nodes and all child nodes. This is useful after changing the order
//	 * of {@link #getRootNodes()}.
//	 * @see Node#updateChildren() */
//	public void updateRootNodes () {
//		for (int i = 0, n = rootNodes.size; i < n; i++) {
//			N node = rootNodes.get(i);
//			int actorIndex = node.actor.getZIndex();
//			if (actorIndex != -1) node.removeFromTree(this, actorIndex);
//		}
//		for (int i = 0, n = rootNodes.size, actorIndex = 0; i < n; i++)
//			actorIndex += rootNodes.get(i).addToTree(this, actorIndex);
//	}

//	/** @return May be null. */
//	public  N? getOverNode () {
//		return overNode;
//	}

//	/** @return May be null. */
//	public V? getOverValue () {
//		if (overNode == null) return null;
//		return (V)overNode.getValue();
//	}

//	/** @param overNode May be null. */
//	public void setOverNode ( N? overNode) {
//		this.overNode = overNode;
//	}

//	/** Sets the amount of horizontal space between the nodes and the left/right edges of the tree. */
//	public void setPadding (float padding) {
//		paddingLeft = padding;
//		paddingRight = padding;
//	}

//	/** Sets the amount of horizontal space between the nodes and the left/right edges of the tree. */
//	public void setPadding (float left, float right) {
//		this.paddingLeft = left;
//		this.paddingRight = right;
//	}

//	public void setIndentSpacing (float indentSpacing) {
//		this.indentSpacing = indentSpacing;
//	}

//	/** Returns the amount of horizontal space for indentation level. */
//	public float getIndentSpacing () {
//		return indentSpacing;
//	}

//	/** Sets the amount of vertical space between nodes. */
//	public void setYSpacing (float ySpacing) {
//		this.ySpacing = ySpacing;
//	}

//	public float getYSpacing () {
//		return ySpacing;
//	}

//	/** Sets the amount of horizontal space left and right of the node's icon. If a node has no icon, the left spacing is used
//	 * between the plus/minus drawable and the node's actor. */
//	public void setIconSpacing (float left, float right) {
//		this.iconSpacingLeft = left;
//		this.iconSpacingRight = right;
//	}

//	public float getPrefWidth () {
//		if (sizeInvalid) computeSize();
//		return prefWidth;
//	}

//	public float getPrefHeight () {
//		if (sizeInvalid) computeSize();
//		return prefHeight;
//	}

//	public void findExpandedValues (Array<V> values) {
//		findExpandedValues(rootNodes, values);
//	}

//	public void restoreExpandedValues (Array<V> values) {
//		for (int i = 0, n = values.size; i < n; i++) {
//			N node = findNode(values.get(i));
//			if (node != null) {
//				node.setExpanded(true);
//				node.expandTo();
//			}
//		}
//	}

//	static bool findExpandedValues (Array<Node> nodes, Array values) {
//		bool expanded = false;
//		for (int i = 0, n = nodes.size; i < n; i++) {
//			Node node = nodes.get(i);
//			if (node.expanded && !findExpandedValues(node.children, values)) values.add(node.value);
//		}
//		return expanded;
//	}

//	/** Returns the node with the specified value, or null. */
//	public N? findNode (V value) {
//		if (value == null) throw new IllegalArgumentException("value cannot be null.");
//		return (N)findNode(rootNodes, value);
//	}

//	static Node? findNode (Array<Node> nodes, Object value) {
//		for (int i = 0, n = nodes.size; i < n; i++) {
//			Node node = nodes.get(i);
//			if (value.Equals(node.value)) return node;
//		}
//		for (int i = 0, n = nodes.size; i < n; i++) {
//			Node node = nodes.get(i);
//			Node found = findNode(node.children, value);
//			if (found != null) return found;
//		}
//		return null;
//	}

//	public void collapseAll () {
//		collapseAll(rootNodes);
//	}

//	static void collapseAll (Array<Node> nodes) {
//		for (int i = 0, n = nodes.size; i < n; i++) {
//			Node node = nodes.get(i);
//			node.setExpanded(false);
//			collapseAll(node.children);
//		}
//	}

//	public void expandAll () {
//		expandAll(rootNodes);
//	}

//	static void expandAll (Array<Node> nodes) {
//		for (int i = 0, n = nodes.size; i < n; i++)
//			nodes.get(i).expandAll();
//	}

//	/** Returns the click listener the tree uses for clicking on nodes and the over node. */
//	public ClickListener getClickListener () {
//		return clickListener;
//	}

//	/** A {@link Tree} node which has an actor and value.
//	 * <p>
//	 * A subclass can be used so the generic type parameters don't need to be specified repeatedly.
//	 * @param <N> The type for the node's parent and child nodes.
//	 * @param <V> The type for the node's value.
//	 * @param <A> The type for the node's actor.
//	 * @author Nathan Sweet */
//	abstract public class Node<N , V, A >
//	where N : Node
//	where A: Actor{
//		A actor;
//		N parent;
//		Array<N> children = new(0);
//		bool selectable = true;
//		bool expanded;
//		Drawable icon;
//		float height;
//		V value;

//		public Node (A actor) {
//			if (actor == null) throw new IllegalArgumentException("actor cannot be null.");
//			this.actor = actor;
//		}

//		/** Creates a node without an actor. An actor must be set using {@link #setActor(Actor)} before this node can be used. */
//		public Node () {
//		}

//		public void setExpanded (bool expanded) {
//			if (expanded == this.expanded) return;
//			this.expanded = expanded;
//			if (children.size == 0) return;
//			Tree tree = getTree();
//			if (tree == null) return;
//			Object[] children = this.children.items;
//			int actorIndex = actor.getZIndex() + 1;
//			if (expanded) {
//				for (int i = 0, n = this.children.size; i < n; i++)
//					actorIndex += ((N)children[i]).addToTree(tree, actorIndex);
//			} else {
//				for (int i = 0, n = this.children.size; i < n; i++)
//					((N)children[i]).removeFromTree(tree, actorIndex);
//			}
//		}

//		/** Called to add the actor to the tree when the node's parent is expanded.
//		 * @return The number of node actors added to the tree. */
//		protected int addToTree (Tree<N, V> tree, int actorIndex) {
//			tree.addActorAt(actorIndex, actor);
//			if (!expanded) return 1;
//			int childIndex = actorIndex + 1;
//			Object[] children = this.children.items;
//			for (int i = 0, n = this.children.size; i < n; i++)
//				childIndex += ((N)children[i]).addToTree(tree, childIndex);
//			return childIndex - actorIndex;
//		}

//		/** Called to remove the actor from the tree, eg when the node is removed or the node's parent is collapsed. */
//		protected void removeFromTree (Tree<N, V> tree, int actorIndex) {
//			Actor removeActorAt = tree.removeActorAt(actorIndex, true);
//			// assert removeActorAt != actor; // If false, either 1) there's a bug, or 2) the children were modified.
//			if (!expanded) return;
//			Object[] children = this.children.items;
//			for (int i = 0, n = this.children.size; i < n; i++)
//				((N)children[i]).removeFromTree(tree, actorIndex);
//		}

//		public void add (N node) {
//			insert(children.size, node);
//		}

//		public void addAll (Array<N> nodes) {
//			for (int i = 0, n = nodes.size; i < n; i++)
//				insert(children.size, nodes.get(i));
//		}

//		public void insert (int childIndex, N node) {
//			node.parent = this;
//			children.insert(childIndex, node);
//			if (!expanded) return;
//			Tree tree = getTree();
//			if (tree != null) {
//				int actorIndex;
//				if (childIndex == 0)
//					actorIndex = actor.getZIndex() + 1;
//				else if (childIndex < children.size - 1)
//					actorIndex = children.get(childIndex + 1).actor.getZIndex();
//				else {
//					N before = children.get(childIndex - 1);
//					actorIndex = before.actor.getZIndex() + before.countActors();
//				}
//				node.addToTree(tree, actorIndex);
//			}
//		}

//		int countActors () {
//			if (!expanded) return 1;
//			int count = 1;
//			Object[] children = this.children.items;
//			for (int i = 0, n = this.children.size; i < n; i++)
//				count += ((N)children[i]).countActors();
//			return count;
//		}

//		/** Remove this node from its parent. */
//		public void remove () {
//			Tree tree = getTree();
//			if (tree != null)
//				tree.remove(this);
//			else if (parent != null) //
//				parent.remove(this);
//		}

//		/** Remove the specified child node from this node. Does nothing if the node is not a child of this node. */
//		public void remove (N node) {
//			if (!children.removeValue(node, true)) return;
//			if (!expanded) return;
//			Tree tree = getTree();
//			if (tree != null) node.removeFromTree(tree, node.actor.getZIndex());
//		}

//		/** Removes all children from this node. */
//		public void clearChildren () {
//			if (expanded) {
//				Tree tree = getTree();
//				if (tree != null) {
//					int actorIndex = actor.getZIndex() + 1;
//					Object[] children = this.children.items;
//					for (int i = 0, n = this.children.size; i < n; i++)
//						((N)children[i]).removeFromTree(tree, actorIndex);
//				}
//			}
//			children.clear();
//		}

//		/** Returns the tree this node's actor is currently in, or null. The actor is only in the tree when all of its parent nodes
//		 * are expanded. */
//		public Tree<N, V>? getTree () {
//			Group parent = actor.getParent();
//			if (parent is Tree) return (Tree)parent;
//			return null;
//		}

//		public void setActor (A newActor) {
//			if (actor != null) {
//				Tree<N, V> tree = getTree();
//				if (tree != null) {
//					int index = actor.getZIndex();
//					tree.removeActorAt(index, true);
//					tree.addActorAt(index, newActor);
//				}
//			}
//			actor = newActor;
//		}

//		public A getActor () {
//			return actor;
//		}

//		public bool isExpanded () {
//			return expanded;
//		}

//		/** If the children order is changed, {@link #updateChildren()} must be called to ensure the node's actors are in the
//		 * correct order. That is not necessary if this node is not in the tree or is not expanded, because then the child node's
//		 * actors are not in the tree. */
//		public Array<N> getChildren () {
//			return children;
//		}

//		public bool hasChildren () {
//			return children.size > 0;
//		}

//		/** Updates the order of the actors in the tree for this node and all child nodes. This is useful after changing the order
//		 * of {@link #getChildren()}.
//		 * @see Tree#updateRootNodes() */
//		public void updateChildren () {
//			if (!expanded) return;
//			Tree tree = getTree();
//			if (tree == null) return;
//			Object[] children = this.children.items;
//			int n = this.children.size;
//			int actorIndex = actor.getZIndex() + 1;
//			for (int i = 0; i < n; i++)
//				((N)children[i]).removeFromTree(tree, actorIndex);
//			for (int i = 0; i < n; i++)
//				actorIndex += ((N)children[i]).addToTree(tree, actorIndex);
//		}

//		/** @return May be null. */
//		public N? getParent () {
//			return parent;
//		}

//		/** Sets an icon that will be drawn to the left of the actor. */
//		public void setIcon (Drawable? icon) {
//			this.icon = icon;
//		}

//		public  V? getValue () {
//			return value;
//		}

//		/** Sets an application specific value for this node. */
//		public void setValue ( V? value) {
//			this.value = value;
//		}

//		public Drawable? getIcon () {
//			return icon;
//		}

//		public int getLevel () {
//			int level = 0;
//			Node current = this;
//			do {
//				level++;
//				current = current.getParent();
//			} while (current != null);
//			return level;
//		}

//		/** Returns this node or the child node with the specified value, or null. */
//		public  N? findNode (V value) {
//			if (value == null) throw new IllegalArgumentException("value cannot be null.");
//			if (value.Equals(this.value)) return (N)this;
//			return (N)Tree.findNode(children, value);
//		}

//		/** Collapses all nodes under and including this node. */
//		public void collapseAll () {
//			setExpanded(false);
//			Tree.collapseAll(children);
//		}

//		/** Expands all nodes under and including this node. */
//		public void expandAll () {
//			setExpanded(true);
//			if (children.size > 0) Tree.expandAll(children);
//		}

//		/** Expands all parent nodes of this node. */
//		public void expandTo () {
//			Node node = parent;
//			while (node != null) {
//				node.setExpanded(true);
//				node = node.parent;
//			}
//		}

//		public bool isSelectable () {
//			return selectable;
//		}

//		public void setSelectable (bool selectable) {
//			this.selectable = selectable;
//		}

//		public void findExpandedValues (Array<V> values) {
//			if (expanded && !Tree.findExpandedValues(children, values)) values.add(value);
//		}

//		public void restoreExpandedValues (Array<V> values) {
//			for (int i = 0, n = values.size; i < n; i++) {
//				N node = findNode(values.get(i));
//				if (node != null) {
//					node.setExpanded(true);
//					node.expandTo();
//				}
//			}
//		}

//		/** Returns the height of the node as calculated for layout. A subclass may override and increase the returned height to
//		 * create a blank space in the tree above the node, eg for a separator. */
//		public float getHeight () {
//			return height;
//		}

//		/** Returns true if the specified node is this node or an ascendant of this node. */
//		public bool isAscendantOf (N node) {
//			if (node == null) throw new IllegalArgumentException("node cannot be null.");
//			Node current = node;
//			do {
//				if (current == this) return true;
//				current = current.parent;
//			} while (current != null);
//			return false;
//		}

//		/** Returns true if the specified node is this node or an descendant of this node. */
//		public bool isDescendantOf (N node) {
//			if (node == null) throw new IllegalArgumentException("node cannot be null.");
//			Node parent = this;
//			do {
//				if (parent == node) return true;
//				parent = parent.parent;
//			} while (parent != null);
//			return false;
//		}
//	}

//	/** The style for a {@link Tree}.
//	 * @author Nathan Sweet */
//	public class TreeStyle {
//		public Drawable plus, minus;
//		public Drawable? plusOver, minusOver;
//		public Drawable? over, selection, background;

//		public TreeStyle () {
//		}

//		public TreeStyle (Drawable plus, Drawable minus, Drawable? selection) {
//			this.plus = plus;
//			this.minus = minus;
//			this.selection = selection;
//		}

//		public TreeStyle (TreeStyle style) {
//			plus = style.plus;
//			minus = style.minus;

//			plusOver = style.plusOver;
//			minusOver = style.minusOver;

//			over = style.over;
//			selection = style.selection;
//			background = style.background;
//		}
//	}
//}
