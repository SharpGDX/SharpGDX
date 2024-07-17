using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A button with a child {@link Image} and {@link Label}.
 * @see ImageButton
 * @see TextButton
 * @see Button
 * @author Nathan Sweet */
public class ImageTextButton : Button {
	private readonly Image image;
	private Label label;
	private ImageTextButtonStyle style;

	public ImageTextButton (String? text, Skin skin) 
	: this(text, skin.get<ImageTextButtonStyle>(typeof(ImageTextButtonStyle)))
	{
		
		setSkin(skin);
	}

	public ImageTextButton (String? text, Skin skin, String styleName) 
	: this(text, skin.get<ImageTextButtonStyle>(styleName, typeof(ImageTextButtonStyle)))
	{
		
		setSkin(skin);
	}

	public ImageTextButton (String? text, ImageTextButtonStyle style) 
	: base(style)
	{
		
		this.style = style;

		defaults().Space(3);

		image = newImage();

		label = newLabel(text, new Label.LabelStyle(style.font, style.fontColor));
		label.setAlignment(Align.center);

		add(image);
		add(label);

		setStyle(style);

		setSize(getPrefWidth(), getPrefHeight());
	}

	protected Image newImage () {
		return new Image((IDrawable)null, Scaling.fit);
	}

	protected Label newLabel (String text, Label.LabelStyle style) {
		return new Label(text, style);
	}

	public override void setStyle (ButtonStyle style) {
		if (!(style is ImageTextButtonStyle)) throw new IllegalArgumentException("style must be a ImageTextButtonStyle.");
		this.style = (ImageTextButtonStyle)style;
		base.setStyle(style);

		if (image != null) updateImage();

		if (label != null) {
			ImageTextButtonStyle textButtonStyle = (ImageTextButtonStyle)style;
			Label.LabelStyle labelStyle = label.getStyle();
			labelStyle.font = textButtonStyle.font;
			labelStyle.fontColor = getFontColor();
			label.setStyle(labelStyle);
		}
	}

	public override ImageTextButtonStyle getStyle () {
		return style;
	}

	/** Returns the appropriate image drawable from the style based on the current button state. */
	protected IDrawable? getImageDrawable () {
		if (isDisabled() && style.imageDisabled != null) return style.imageDisabled;
		if (isPressed()) {
			if (isChecked() && style.imageCheckedDown != null) return style.imageCheckedDown;
			if (style.imageDown != null) return style.imageDown;
		}
		if (isOver()) {
			if (isChecked()) {
				if (style.imageCheckedOver != null) return style.imageCheckedOver;
			} else {
				if (style.imageOver != null) return style.imageOver;
			}
		}
		if (isChecked()) {
			if (style.imageChecked != null) return style.imageChecked;
			if (isOver() && style.imageOver != null) return style.imageOver;
		}
		return style.imageUp;
	}

	/** Sets the image drawable based on the current button state. The default implementation sets the image drawable using
	 * {@link #getImageDrawable()}. */
	protected void updateImage () {
		image.setDrawable(getImageDrawable());
	}

	/** Returns the appropriate label font color from the style based on the current button state. */
	protected Color? getFontColor () {
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
		updateImage();
		label.getStyle().fontColor = getFontColor();
		base.draw(batch, parentAlpha);
	}

	public Image getImage () {
		return image;
	}

	public Cell getImageCell () {
		return getCell(image);
	}

	public void setLabel (Label label) {
		getLabelCell().SetActor(label);
		this.label = label;
	}

	public Label getLabel () {
		return label;
	}

	public Cell getLabelCell () {
		return getCell(label);
	}

	public void setText (string text) {
		label.setText(text);
	}

	public String getText () {
		return label.getText().ToString();
	}

	public override String ToString () {
		String name = getName();
		if (name != null) return name;
		String className = GetType().Name;
		int dotIndex = className.LastIndexOf('.');
		if (dotIndex != -1) className = className.Substring(dotIndex + 1);
		return (className.IndexOf('$') != -1 ? "ImageTextButton " : "") + className + ": " + image.getDrawable() + " "
			+ label.getText();
	}

	/** The style for an image text button, see {@link ImageTextButton}.
	 * @author Nathan Sweet */
	public class ImageTextButtonStyle : TextButton.TextButtonStyle {
		public IDrawable? imageUp, imageDown, imageOver, imageDisabled;
		public IDrawable? imageChecked, imageCheckedDown, imageCheckedOver;

		public ImageTextButtonStyle () {
		}

		public ImageTextButtonStyle (IDrawable? up, IDrawable? down, IDrawable? @checked, BitmapFont font) 
		: base(up, down, @checked, font)
		{
			
		}

		public ImageTextButtonStyle (ImageTextButtonStyle style) 
		: base(style)
		{
			
			imageUp = style.imageUp;
			imageDown = style.imageDown;
			imageOver = style.imageOver;
			imageDisabled = style.imageDisabled;

			imageChecked = style.imageChecked;
			imageCheckedDown = style.imageCheckedDown;
			imageCheckedOver = style.imageCheckedOver;
		}

		public ImageTextButtonStyle (TextButton.TextButtonStyle style) 
		: base(style)
		{
			
		}
	}
}
