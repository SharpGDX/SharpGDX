using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Scenes.Scene2D.UI;
using System.Text;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A text label, with optional word wrapping.
 * <p>
 * The preferred size of the label is determined by the actual text bounds, unless {@link #setWrap(boolean) word wrap} is enabled.
 * @author Nathan Sweet */
public class Label : Widget {
	static private readonly Color tempColor = new Color();
	static private readonly GlyphLayout prefSizeLayout = new GlyphLayout();

	private LabelStyle style;
	private readonly GlyphLayout _layout = new GlyphLayout();
	private float prefWidth, prefHeight;
	private readonly StringBuilder text = new StringBuilder();
	private int intValue = int.MinValue;
	private BitmapFontCache cache;
	private int labelAlign = Align.left;
	private int lineAlign = Align.left;
	private bool wrap;
	private float lastPrefHeight;
	private bool prefSizeInvalid = true;
	private float fontScaleX = 1, fontScaleY = 1;
	private bool fontScaleChanged = false;
	private  String? ellipsis;

	public Label (String? text, Skin skin) 
	: this(text, skin.get<LabelStyle>(typeof(LabelStyle)))
	{
		
	}

	public Label (String? text, Skin skin, String styleName) 
	: this(text, skin.get<LabelStyle>(styleName, typeof(LabelStyle)))
	{
		
	}

	/** Creates a label, using a {@link LabelStyle} that has a BitmapFont with the specified name from the skin and the specified
	 * color. */
	public Label (String? text, Skin skin, String fontName, Color color)
	:this(text, new LabelStyle(skin.getFont(fontName), color)){
		
	}

/** Creates a label, using a {@link LabelStyle} that has a BitmapFont with the specified name and the specified color from the
 * skin. */
	public Label (String? text, Skin skin, String fontName, String colorName)
	:this(text, new LabelStyle(skin.getFont(fontName), skin.getColor(colorName))){
}
public Label (String? text, LabelStyle style) {
		if (text != null) this.text.Append(text);
		setStyle(style);
		if (text != null && text.Length > 0) setSize(getPrefWidth(), getPrefHeight());
	}

	public void setStyle (LabelStyle style) {
		if (style == null) throw new IllegalArgumentException("style cannot be null.");
		if (style.font == null) throw new IllegalArgumentException("Missing LabelStyle font.");
		this.style = style;

		cache = style.font.newFontCache();
		invalidateHierarchy();
	}

	/** Returns the label's style. Modifying the returned style may not have an effect until {@link #setStyle(LabelStyle)} is
	 * called. */
	public LabelStyle getStyle () {
		return style;
	}

	/** Sets the text to the specified integer value. If the text is already equivalent to the specified value, a string is not
	 * allocated.
	 * @return true if the text was changed. */
	public bool setText (int value) {
		if (this.intValue == value) return false;
		text.Clear();
		text.Append(value);
		intValue = value;
		invalidateHierarchy();
		return true;
	}

	/** @param newText If null, "" will be used. */
	public void setText(String? newText)
	{
		if (newText == null)
		{
			if (text.Length == 0) return;
			text.Clear();
		}
		else if (newText is StringBuilder)
		{
			// TODO: WTF?
			//if (text.Equals(newText)) return;
			//text.Clear();
			//text.Append((StringBuilder)newText);
			throw new NotImplementedException();
		}
		else
		{
			if (textEquals(newText)) return;
			text.Clear();
			text.Append(newText);
		}

		intValue = int.MinValue;
		invalidateHierarchy();
	}

	public bool textEquals (String other) {
		int length = text.Length;
		// TODO: Don't love this. -RP
		char[] chars = text.ToString().ToCharArray();
		if (length != other.Length) return false;
		for (int i = 0; i < length; i++)
			if (chars[i] != other[i]) return false;
		return true;
	}

	public StringBuilder getText () {
		// TODO: Should this really return a StringBuilder? -RP
		return text;
	}

	public override void invalidate () {
		base.invalidate();
		prefSizeInvalid = true;
	}

	private void scaleAndComputePrefSize () {
		BitmapFont font = cache.getFont();
		float oldScaleX = font.getScaleX();
		float oldScaleY = font.getScaleY();
		if (fontScaleChanged) font.getData().setScale(fontScaleX, fontScaleY);

		computePrefSize(Label.prefSizeLayout);

		if (fontScaleChanged) font.getData().setScale(oldScaleX, oldScaleY);
	}

	protected void computePrefSize (GlyphLayout layout) {
		prefSizeInvalid = false;
		if (wrap && ellipsis == null) {
			float width = getWidth();
			if (style.background != null) {
				width = Math.Max(width, style.background.getMinWidth()) - style.background.getLeftWidth()
					- style.background.getRightWidth();
			}
			layout.setText(cache.getFont(), text.ToString(), Color.WHITE, width, Align.left, true);
		} else
			layout.setText(cache.getFont(), text.ToString());
		prefWidth = layout.width;
		prefHeight = layout.height;
	}

	public override void layout () {
		BitmapFont font = cache.getFont();
		float oldScaleX = font.getScaleX();
		float oldScaleY = font.getScaleY();
		if (fontScaleChanged) font.getData().setScale(fontScaleX, fontScaleY);

		bool wrap = this.wrap && ellipsis == null;
		if (wrap) {
			float prefHeight = getPrefHeight();
			if (prefHeight != lastPrefHeight) {
				lastPrefHeight = prefHeight;
				invalidateHierarchy();
			}
		}

		float width = getWidth(), height = getHeight();
		IDrawable background = style.background;
		float x = 0, y = 0;
		if (background != null) {
			x = background.getLeftWidth();
			y = background.getBottomHeight();
			width -= background.getLeftWidth() + background.getRightWidth();
			height -= background.getBottomHeight() + background.getTopHeight();
		}

		GlyphLayout layout = this._layout;
		float textWidth, textHeight;

		// TODO: Better integrate this, cannot call IndexOf() on a StringBuilder. -RP
		var stringText = text.ToString();

		if (wrap || stringText.IndexOf("\n") != -1) {
			// If the text can span multiple lines, determine the text's actual size so it can be aligned within the label.
			layout.setText(font, stringText, 0, stringText.Length, Color.WHITE, width, lineAlign, wrap, ellipsis);
			textWidth = layout.width;
			textHeight = layout.height;

			if ((labelAlign & Align.left) == 0) {
				if ((labelAlign & Align.right) != 0)
					x += width - textWidth;
				else
					x += (width - textWidth) / 2;
			}
		} else {
			textWidth = width;
			textHeight = font.getData().capHeight;
		}

		if ((labelAlign & Align.top) != 0) {
			y += cache.getFont().isFlipped() ? 0 : height - textHeight;
			y += style.font.getDescent();
		} else if ((labelAlign & Align.bottom) != 0) {
			y += cache.getFont().isFlipped() ? height - textHeight : 0;
			y -= style.font.getDescent();
		} else {
			y += (height - textHeight) / 2;
		}
		if (!cache.getFont().isFlipped()) y += textHeight;

		layout.setText(font, stringText, 0, stringText.Length, Color.WHITE, textWidth, lineAlign, wrap, ellipsis);
		cache.setText(layout, x, y);

		if (fontScaleChanged) font.getData().setScale(oldScaleX, oldScaleY);
	}

	public override void draw (IBatch batch, float parentAlpha) {
		validate();
		Color color = tempColor.set(getColor());
		color.a *= parentAlpha;
		if (style.background != null) {
			batch.setColor(color.r, color.g, color.b, color.a);
			style.background.draw(batch, getX(), getY(), getWidth(), getHeight());
		}
		if (style.fontColor != null) color.mul(style.fontColor);
		cache.tint(color);
		cache.setPosition(getX(), getY());
		cache.draw(batch);
	}

	public override float getPrefWidth () {
		if (wrap) return 0;
		if (prefSizeInvalid) scaleAndComputePrefSize();
		float width = prefWidth;
		IDrawable background = style.background;
		if (background != null)
			width = Math.Max(width + background.getLeftWidth() + background.getRightWidth(), background.getMinWidth());
		return width;
	}

	public override float getPrefHeight () {
		if (prefSizeInvalid) scaleAndComputePrefSize();
		float descentScaleCorrection = 1;
		if (fontScaleChanged) descentScaleCorrection = fontScaleY / style.font.getScaleY();
		float height = prefHeight - style.font.getDescent() * descentScaleCorrection * 2;
		IDrawable background = style.background;
		if (background != null)
			height = Math.Max(height + background.getTopHeight() + background.getBottomHeight(), background.getMinHeight());
		return height;
	}

	public GlyphLayout getGlyphLayout () {
		return _layout;
	}

	/** If false, the text will only wrap where it contains newlines (\n). The preferred size of the label will be the text bounds.
	 * If true, the text will word wrap using the width of the label. The preferred width of the label will be 0, it is expected
	 * that something external will set the width of the label. Wrapping will not occur when ellipsis is enabled. Default is false.
	 * <p>
	 * When wrap is enabled, the label's preferred height depends on the width of the label. In some cases the parent of the label
	 * will need to layout twice: once to set the width of the label and a second time to adjust to the label's new preferred
	 * height. */
	public void setWrap (bool wrap) {
		this.wrap = wrap;
		invalidateHierarchy();
	}

	public bool getWrap () {
		return wrap;
	}

	public int getLabelAlign () {
		return labelAlign;
	}

	public int getLineAlign () {
		return lineAlign;
	}

	/** @param alignment Aligns all the text within the label (default left center) and each line of text horizontally (default
	 *           left).
	 * @see Align */
	public void setAlignment (int alignment) {
		setAlignment(alignment, alignment);
	}

	/** @param labelAlign Aligns all the text within the label (default left center).
	 * @param lineAlign Aligns each line of text horizontally (default left).
	 * @see Align */
	public void setAlignment (int labelAlign, int lineAlign) {
		this.labelAlign = labelAlign;

		if ((lineAlign & Align.left) != 0)
			this.lineAlign = Align.left;
		else if ((lineAlign & Align.right) != 0)
			this.lineAlign = Align.right;
		else
			this.lineAlign = Align.center;

		invalidate();
	}

	public void setFontScale (float fontScale) {
		setFontScale(fontScale, fontScale);
	}

	public void setFontScale (float fontScaleX, float fontScaleY) {
		fontScaleChanged = true;
		this.fontScaleX = fontScaleX;
		this.fontScaleY = fontScaleY;
		invalidateHierarchy();
	}

	public float getFontScaleX () {
		return fontScaleX;
	}

	public void setFontScaleX (float fontScaleX) {
		setFontScale(fontScaleX, fontScaleY);
	}

	public float getFontScaleY () {
		return fontScaleY;
	}

	public void setFontScaleY (float fontScaleY) {
		setFontScale(fontScaleX, fontScaleY);
	}

	/** When non-null the text will be truncated "..." if it does not fit within the width of the label. Wrapping will not occur
	 * when ellipsis is enabled. Default is false. */
	public void setEllipsis (String? ellipsis) {
		this.ellipsis = ellipsis;
	}

	/** When true the text will be truncated "..." if it does not fit within the width of the label. Wrapping will not occur when
	 * ellipsis is true. Default is false. */
	public void setEllipsis (bool ellipsis) {
		if (ellipsis)
			this.ellipsis = "...";
		else
			this.ellipsis = null;
	}

	/** Allows subclasses to access the cache in {@link #draw(Batch, float)}. */
	protected BitmapFontCache getBitmapFontCache () {
		return cache;
	}

	public override String ToString() {
		String name = getName();
		if (name != null) return name;
		String className = GetType().Name;
		int dotIndex = className.LastIndexOf('.');
		if (dotIndex != -1) className = className.Substring(dotIndex + 1);
		return (className.IndexOf('$') != -1 ? "Label " : "") + className + ": " + text;
	}

	/** The style for a label, see {@link Label}.
	 * @author Nathan Sweet */
	public class LabelStyle {
		public BitmapFont font;
		public Color? fontColor;
		public IDrawable? background;

		public LabelStyle () {
		}

		public LabelStyle (BitmapFont font,  Color? fontColor) {
			this.font = font;
			this.fontColor = fontColor;
		}

		public LabelStyle (LabelStyle style) {
			font = style.font;
			if (style.fontColor != null) fontColor = new Color(style.fontColor);
			background = style.background;
		}
	}
}
