using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A button with a child {@link Label} to display text.
 * @author Nathan Sweet */
public class TextButton : Button {
	private Label label;
	private TextButtonStyle style;

	public TextButton (String? text, Skin skin) 
	: this(text, skin.get<TextButtonStyle>(typeof(TextButtonStyle)))
	{
		
		setSkin(skin);
	}

	public TextButton (String? text, Skin skin, String styleName) 
	: this(text, skin.get<TextButtonStyle>(styleName, typeof(TextButtonStyle)))
	{
		
		setSkin(skin);
	}

	public TextButton (String? text, TextButtonStyle style)
	{
		setStyle(style);
		label = newLabel(text, new Label.LabelStyle(style.font, style.fontColor));
		label.setAlignment(Align.center);
		add(label).Expand().Fill();
		setSize(getPrefWidth(), getPrefHeight());
	}

	protected Label newLabel (String text, Label.LabelStyle style) {
		return new Label(text, style);
	}

	public override void setStyle (ButtonStyle style) {
		if (style == null) throw new NullPointerException("style cannot be null");
		if (!(style is TextButtonStyle)) throw new IllegalArgumentException("style must be a TextButtonStyle.");
		this.style = (TextButtonStyle)style;
		base.setStyle(style);

		if (label != null) {
			TextButtonStyle textButtonStyle = (TextButtonStyle)style;
			Label.LabelStyle labelStyle = label.getStyle();
			labelStyle.font = textButtonStyle.font;
			labelStyle.fontColor = textButtonStyle.fontColor;
			label.setStyle(labelStyle);
		}
	}

	public override TextButtonStyle getStyle () {
		return style;
	}

	/** Returns the appropriate label font color from the style based on the current button state. */
	protected  Color? getFontColor () {
		if (isDisabled() && style.disabledFontColor != null) return style.disabledFontColor;
		if (isPressed()) {
			if (isChecked() && style.checkedDownFontColor != null) return style.checkedDownFontColor;
			if (style.downFontColor != null) return style.downFontColor;
		}
		if (isOver()) {
			if (isChecked()) {
				if (style.checkedOverFontColor != null) return style.checkedOverFontColor;
			} else {
				if (style.overFontColor != null) return style.overFontColor;
			}
		}
		bool focused = hasKeyboardFocus();
		if (isChecked()) {
			if (focused && style.checkedFocusedFontColor != null) return style.checkedFocusedFontColor;
			if (style.checkedFontColor != null) return style.checkedFontColor;
			if (isOver() && style.overFontColor != null) return style.overFontColor;
		}
		if (focused && style.focusedFontColor != null) return style.focusedFontColor;
		return style.fontColor;
	}

	public override void draw (IBatch batch, float parentAlpha) {
		label.getStyle().fontColor = getFontColor();
		base.draw(batch, parentAlpha);
	}

	public void setLabel (Label label) {
		if (label == null) throw new IllegalArgumentException("label cannot be null.");
		getLabelCell().SetActor(label);
		this.label = label;
	}

	public Label getLabel () {
		return label;
	}

	public Cell<Label> getLabelCell () {
		return getCell(label);
	}

	public void setText (String? text) {
		label.setText(text);
	}

	public string getText () {
		// TODO: Should Label.getText() really return a StringBuilder?
		return label.getText().ToString();
	}

	public override String ToString () {
		String name = getName();
		if (name != null) return name;
		String className = GetType().Name;
		int dotIndex = className.LastIndexOf('.');
		if (dotIndex != -1) className = className.Substring(dotIndex + 1);
		return (className.IndexOf('$') != -1 ? "TextButton " : "") + className + ": " + label.getText();
	}

	/** The style for a text button, see {@link TextButton}.
	 * @author Nathan Sweet */
	 public class TextButtonStyle : Button.ButtonStyle {
		public BitmapFont font;
		public  Color? fontColor, downFontColor, overFontColor, focusedFontColor, disabledFontColor;
		public  Color? checkedFontColor, checkedDownFontColor, checkedOverFontColor, checkedFocusedFontColor;

		public TextButtonStyle () {
		}

		public TextButtonStyle (IDrawable? up, IDrawable? down, IDrawable? @checked,  BitmapFont? font) 
		: base(up, down, @checked)
		{
			
			this.font = font;
		}

		public TextButtonStyle (TextButtonStyle style) 
		: base(style)
		{
			
			font = style.font;

			if (style.fontColor != null) fontColor = new Color(style.fontColor);
			if (style.downFontColor != null) downFontColor = new Color(style.downFontColor);
			if (style.overFontColor != null) overFontColor = new Color(style.overFontColor);
			if (style.focusedFontColor != null) focusedFontColor = new Color(style.focusedFontColor);
			if (style.disabledFontColor != null) disabledFontColor = new Color(style.disabledFontColor);

			if (style.checkedFontColor != null) checkedFontColor = new Color(style.checkedFontColor);
			if (style.checkedDownFontColor != null) checkedDownFontColor = new Color(style.checkedDownFontColor);
			if (style.checkedOverFontColor != null) checkedOverFontColor = new Color(style.checkedOverFontColor);
			if (style.checkedFocusedFontColor != null) checkedFocusedFontColor = new Color(style.checkedFocusedFontColor);
		}
	}
}
