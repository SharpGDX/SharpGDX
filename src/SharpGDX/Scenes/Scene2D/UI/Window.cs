using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Scenes.Scene2D;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A table that can be dragged and act as a modal window. The top padding is used as the window's title height.
 * <p>
 * The preferred size of a window is the preferred size of the title text and the children as laid out by the table. After adding
 * children to the window, it can be convenient to call {@link #pack()} to size the window to the size of the children.
 * @author Nathan Sweet */
public class Window : Table {
	static private readonly Vector2 tmpPosition = new Vector2();
	static private readonly Vector2 tmpSize = new Vector2();
	static private readonly int MOVE = 1 << 5;

	private WindowStyle style;
	protected bool _isMovable = true, _isModal, _isResizable;
	int resizeBorder = 8;
	bool _keepWithinStage = true;
	Label titleLabel;
	Table titleTable;
	bool drawTitleTable;

	protected int edge;
	protected bool dragging;

	public Window (String title, Skin skin) 
	: this(title, skin.get< WindowStyle>(typeof(WindowStyle)))
	{
		
		setSkin(skin);
	}

	public Window (String title, Skin skin, String styleName) 
	: this(title, skin.get<WindowStyle>(styleName, typeof(WindowStyle)))
	{
		
		setSkin(skin);
	}

	private class WindowTable : Table
	{
		private readonly Window _window;

		public WindowTable(Window window)
		{
			_window = window;
		}

		public override void draw(IBatch batch, float parentAlpha)
		{
			if (_window.drawTitleTable) base.draw(batch, parentAlpha);
		}
	}

	private class CaptureInputListener : InputListener
	{
		private readonly Window _window;

		public CaptureInputListener(Window window)
		{
			_window = window;
		}

		public override bool touchDown(InputEvent @event, float x, float y, int pointer, int button)
		{
			_window.toFront();
			return false;
		}
	}

	public Window (String title, WindowStyle style) {
		if (title == null) throw new IllegalArgumentException("title cannot be null.");
		setTouchable(Touchable.enabled);
		setClip(true);

		titleLabel = newLabel(title, new Label.LabelStyle(style.titleFont, style.titleFontColor));
		titleLabel.setEllipsis(true);

		titleTable = new WindowTable(this);
		titleTable.add(titleLabel).ExpandX().FillX().MinWidth(0);
		addActor(titleTable);

		setStyle(style);
		setWidth(150);
		setHeight(150);

		addCaptureListener(new CaptureInputListener(this));
		addListener(new WindowInputListener(this) );
	}

	private class WindowInputListener : InputListener
{
	private readonly Window _window;
	float startX, startY, lastX, lastY;

	public WindowInputListener(Window window)
	{
		_window = window;
	}

	private void updateEdge(float x, float y)
	{
		float border = _window.resizeBorder / 2f;
		float width = _window.getWidth(), height = _window.getHeight();
		float padTop = _window.getPadTop(), padLeft = _window.getPadLeft(), padBottom = _window.getPadBottom(), padRight = _window.getPadRight();
		float left = padLeft, right = width - padRight, bottom = padBottom;
		_window.edge = 0;
		if (_window._isResizable && x >= left - border && x <= right + border && y >= bottom - border)
		{
			if (x < left + border) _window.edge |= Align.left;
			if (x > right - border) _window.edge |= Align.right;
			if (y < bottom + border) _window.edge |= Align.bottom;
			if (_window.edge != 0) border += 25;
			if (x < left + border) _window.edge |= Align.left;
			if (x > right - border) _window.edge |= Align.right;
			if (y < bottom + border) _window.edge |= Align.bottom;
		}
		if (_window._isMovable && _window.edge == 0 && y <= height && y >= height - padTop && x >= left && x <= right) _window.edge = MOVE;
	}

		public override bool touchDown(InputEvent @event, float x, float y, int pointer, int button) {
				if (button == 0) {
					updateEdge(x, y);
					_window.dragging = _window.edge != 0;
					startX = x;
					startY = y;
					lastX = x - _window.getWidth();
	lastY = y - _window.getHeight();
}
				return _window.edge != 0 || _window._isModal;
			}

		public override void touchUp(InputEvent @event, float x, float y, int pointer, int button)
{
	_window.dragging = false;
}

		public override void touchDragged(InputEvent @event, float x, float y, int pointer)
{
	if (!_window.dragging) return;
	float width = _window.getWidth(), height = _window.getHeight();
	float windowX = _window.getX(), windowY = _window.getY();

	float minWidth = _window.getMinWidth(), maxWidth = _window.getMaxWidth();
	float minHeight = _window.getMinHeight(), maxHeight = _window.getMaxHeight();
	Stage stage = _window.getStage();
	bool clampPosition = _window._keepWithinStage && stage != null && _window.getParent() == stage.getRoot();

	if ((_window.edge & MOVE) != 0)
	{
		float amountX = x - startX, amountY = y - startY;
		windowX += amountX;
		windowY += amountY;
	}
	if ((_window.edge & Align.left) != 0)
	{
		float amountX = x - startX;
		if (width - amountX < minWidth) amountX = -(minWidth - width);
		if (clampPosition && windowX + amountX < 0) amountX = -windowX;
		width -= amountX;
		windowX += amountX;
	}
	if ((_window.edge & Align.bottom) != 0)
	{
		float amountY = y - startY;
		if (height - amountY < minHeight) amountY = -(minHeight - height);
		if (clampPosition && windowY + amountY < 0) amountY = -windowY;
		height -= amountY;
		windowY += amountY;
	}
	if ((_window.edge & Align.right) != 0)
	{
		float amountX = x - lastX - width;
		if (width + amountX < minWidth) amountX = minWidth - width;
		if (clampPosition && windowX + width + amountX > stage.getWidth()) amountX = stage.getWidth() - windowX - width;
		width += amountX;
	}
	if ((_window.edge & Align.top) != 0)
	{
		float amountY = y - lastY - height;
		if (height + amountY < minHeight) amountY = minHeight - height;
		if (clampPosition && windowY + height + amountY > stage.getHeight())
			amountY = stage.getHeight() - windowY - height;
		height += amountY;
	}
	_window.setBounds((float)Math.Round(windowX), (float)Math.Round(windowY), (float)Math.Round(width), (float)Math.Round(height));
}

		public override bool mouseMoved(InputEvent @event, float x, float y)
{
	updateEdge(x, y);
	return _window._isModal;
}

		public bool scrolled(InputEvent @event, float x, float y, int amount)
{
	return _window._isModal;
}

		public override bool keyDown(InputEvent @event, int keycode)
{
	return _window._isModal;
}

		public override bool keyUp(InputEvent @event, int keycode)
{
	return _window._isModal;
}

		public override bool keyTyped(InputEvent @event, char character)
{
	return _window._isModal;
}
		}

	protected Label newLabel (String text, Label.LabelStyle style) {
		return new Label(text, style);
	}

	public void setStyle (WindowStyle style) {
		if (style == null) throw new IllegalArgumentException("style cannot be null.");
		this.style = style;

		setBackground(style.background);
		titleLabel.setStyle(new Label.LabelStyle(style.titleFont, style.titleFontColor));
		invalidateHierarchy();
	}

	/** Returns the window's style. Modifying the returned style may not have an effect until {@link #setStyle(WindowStyle)} is
	 * called. */
	public WindowStyle getStyle () {
		return style;
	}

	public void keepWithinStage () {
		if (!_keepWithinStage) return;
		Stage stage = getStage();
		if (stage == null) return;
		Camera camera = stage.getCamera();
		if (camera is OrthographicCamera) {
			OrthographicCamera orthographicCamera = (OrthographicCamera)camera;
			float parentWidth = stage.getWidth();
			float parentHeight = stage.getHeight();
			if (getX(Align.right) - camera.position.x > parentWidth / 2 / orthographicCamera.zoom)
				setPosition(camera.position.x + parentWidth / 2 / orthographicCamera.zoom, getY(Align.right), Align.right);
			if (getX(Align.left) - camera.position.x < -parentWidth / 2 / orthographicCamera.zoom)
				setPosition(camera.position.x - parentWidth / 2 / orthographicCamera.zoom, getY(Align.left), Align.left);
			if (getY(Align.top) - camera.position.y > parentHeight / 2 / orthographicCamera.zoom)
				setPosition(getX(Align.top), camera.position.y + parentHeight / 2 / orthographicCamera.zoom, Align.top);
			if (getY(Align.bottom) - camera.position.y < -parentHeight / 2 / orthographicCamera.zoom)
				setPosition(getX(Align.bottom), camera.position.y - parentHeight / 2 / orthographicCamera.zoom, Align.bottom);
		} else if (getParent() == stage.getRoot()) {
			float parentWidth = stage.getWidth();
			float parentHeight = stage.getHeight();
			if (getX() < 0) setX(0);
			if (getRight() > parentWidth) setX(parentWidth - getWidth());
			if (getY() < 0) setY(0);
			if (getTop() > parentHeight) setY(parentHeight - getHeight());
		}
	}

	public override void draw (IBatch batch, float parentAlpha) {
		Stage stage = getStage();
		if (stage != null) {
			if (stage.getKeyboardFocus() == null) stage.setKeyboardFocus(this);

			keepWithinStage();

			if (style.stageBackground != null) {
				stageToLocalCoordinates(tmpPosition.set(0, 0));
				stageToLocalCoordinates(tmpSize.set(stage.getWidth(), stage.getHeight()));
				drawStageBackground(batch, parentAlpha, getX() + tmpPosition.x, getY() + tmpPosition.y, getX() + tmpSize.x,
					getY() + tmpSize.y);
			}
		}
		base.draw(batch, parentAlpha);
	}

	protected void drawStageBackground (IBatch batch, float parentAlpha, float x, float y, float width, float height) {
		Color color = getColor();
		batch.setColor(color.r, color.g, color.b, color.a * parentAlpha);
		style.stageBackground.draw(batch, x, y, width, height);
	}

	protected override void drawBackground (IBatch batch, float parentAlpha, float x, float y) {
		base.drawBackground(batch, parentAlpha, x, y);

		// Manually draw the title table before clipping is done.
		titleTable.getColor().a = getColor().a;
		float padTop = getPadTop(), padLeft = getPadLeft();
		titleTable.setSize(getWidth() - padLeft - getPadRight(), padTop);
		titleTable.setPosition(padLeft, getHeight() - padTop);
		drawTitleTable = true;
		titleTable.draw(batch, parentAlpha);
		drawTitleTable = false; // Avoid drawing the title table again in drawChildren.
	}

	public override Actor? hit (float x, float y, bool touchable) {
		if (!isVisible()) return null;
		Actor hit = base.hit(x, y, touchable);
		if (hit == null && _isModal && (!touchable || getTouchable() == Touchable.enabled)) return this;
		float height = getHeight();
		if (hit == null || hit == this) return hit;
		if (y <= height && y >= height - getPadTop() && x >= 0 && x <= getWidth()) {
			// Hit the title bar, don't use the hit child if it is in the Window's table.
			Actor current = hit;
			while (current.getParent() != this)
				current = current.getParent();
			if (getCell(current) != null) return this;
		}
		return hit;
	}

	public bool isMovable () {
		return _isMovable;
	}

	public void setMovable (bool isMovable) {
		this._isMovable = isMovable;
	}

	public bool isModal () {
		return _isModal;
	}

	public void setModal (bool isModal) {
		this._isModal = isModal;
	}

	public void setKeepWithinStage (bool keepWithinStage) {
		this._keepWithinStage = keepWithinStage;
	}

	public bool isResizable () {
		return _isResizable;
	}

	public void setResizable (bool isResizable) {
		this._isResizable = isResizable;
	}

	public void setResizeBorder (int resizeBorder) {
		this.resizeBorder = resizeBorder;
	}

	public bool isDragging () {
		return dragging;
	}

	public override float getPrefWidth () {
		return Math.Max(base.getPrefWidth(), titleTable.getPrefWidth() + getPadLeft() + getPadRight());
	}

	public Table getTitleTable () {
		return titleTable;
	}

	public Label getTitleLabel () {
		return titleLabel;
	}

	/** The style for a window, see {@link Window}.
	 * @author Nathan Sweet */
	public class WindowStyle {
		public IDrawable? background;
		public BitmapFont titleFont;
		public Color? titleFontColor = new Color(1, 1, 1, 1);
		public IDrawable? stageBackground;

		public WindowStyle () {
		}

		public WindowStyle (BitmapFont titleFont, Color titleFontColor, IDrawable? background) {
			this.titleFont = titleFont;
			this.titleFontColor.set(titleFontColor);
			this.background = background;
		}

		public WindowStyle (WindowStyle style) {
			titleFont = style.titleFont;
			if (style.titleFontColor != null) titleFontColor = new Color(style.titleFontColor);
			background = style.background;
			stageBackground = style.stageBackground;
		}
	}
}
