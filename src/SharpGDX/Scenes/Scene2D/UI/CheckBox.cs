using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A checkbox is a button that contains an image indicating the checked or unchecked state and a label.
 * @author Nathan Sweet */
public class CheckBox : TextButton {
	private Image image;
	private Cell imageCell;
	private CheckBoxStyle style;

	public CheckBox ( String? text, Skin skin) 
	: this(text, skin.get< CheckBoxStyle>(typeof(CheckBoxStyle))){
		
	}

	public CheckBox ( String? text, Skin skin, String styleName) 
	: this(text, skin.get< CheckBoxStyle>(styleName, typeof(CheckBoxStyle)))
	{
		
	}

	public CheckBox ( String? text, CheckBoxStyle style) 
	: base(text, style)
	{
		

		Label label = getLabel();
		label.setAlignment(Align.left);

		image = newImage();
		image.setDrawable(style.checkboxOff);

		clearChildren();
		imageCell = add(image);
		add(label);
		setSize(getPrefWidth(), getPrefHeight());
	}

	protected Image newImage () {
		return new Image((IDrawable)null, Scaling.none);
	}

	public override void setStyle (Button.ButtonStyle style) {
		if (!(style is CheckBoxStyle)) throw new IllegalArgumentException("style must be a CheckBoxStyle.");
		this.style = (CheckBoxStyle)style;
		base.setStyle(style);
	}

	/** Returns the checkbox's style. Modifying the returned style may not have an effect until {@link #setStyle(ButtonStyle)} is
	 * called. */
	public override CheckBoxStyle getStyle () {
		return style;
	}

	public override void draw (IBatch batch, float parentAlpha) {
		image.setDrawable(getImageDrawable());
		base.draw(batch, parentAlpha);
	}

	protected IDrawable? getImageDrawable () {
		if (isDisabled()) {
			if (_isChecked && style.checkboxOnDisabled != null) return style.checkboxOnDisabled;
			return style.checkboxOffDisabled;
		}
		bool over = isOver() && !isDisabled();
		if (_isChecked && style.checkboxOn != null)
			return over && style.checkboxOnOver != null ? style.checkboxOnOver : style.checkboxOn;
		if (over && style.checkboxOver != null) return style.checkboxOver;
		return style.checkboxOff;
	}

	public Image getImage () {
		return image;
	}

	public Cell getImageCell () {
		return imageCell;
	}

	/** The style for a select box, see {@link CheckBox}.
	 * @author Nathan Sweet */
	public class CheckBoxStyle : TextButtonStyle {
		public IDrawable checkboxOn, checkboxOff;
		public IDrawable? checkboxOnOver, checkboxOver, checkboxOnDisabled, checkboxOffDisabled;

		public CheckBoxStyle () {
		}

		public CheckBoxStyle (IDrawable checkboxOff, IDrawable checkboxOn, BitmapFont font, Color? fontColor) {
			this.checkboxOff = checkboxOff;
			this.checkboxOn = checkboxOn;
			this.font = font;
			this.fontColor = fontColor;
		}

		public CheckBoxStyle (CheckBoxStyle style) 
		: base(style)
		{
			
			checkboxOff = style.checkboxOff;
			checkboxOn = style.checkboxOn;

			checkboxOnOver = style.checkboxOnOver;
			checkboxOver = style.checkboxOver;
			checkboxOnDisabled = style.checkboxOnDisabled;
			checkboxOffDisabled = style.checkboxOffDisabled;
		}
	}
}
