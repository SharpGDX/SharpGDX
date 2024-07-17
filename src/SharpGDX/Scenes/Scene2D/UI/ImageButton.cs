using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A button with a child {@link Image} to display an image. This is useful when the button must be larger than the image and the
 * image centered on the button. If the image is the size of the button, a {@link Button} without any children can be used, where
 * the {@link Button.ButtonStyle#up}, {@link Button.ButtonStyle#down}, and {@link Button.ButtonStyle#checked} nine patches define
 * the image.
 * @author Nathan Sweet */
public class ImageButton : Button {
	private readonly Image image;
	private ImageButtonStyle style;

	public ImageButton (Skin skin) 
	: this(skin.get<ImageButtonStyle>(typeof(ImageButtonStyle))){
		
		setSkin(skin);
	}

	public ImageButton (Skin skin, String styleName) 
	: this(skin.get<ImageButtonStyle>(styleName, typeof(ImageButtonStyle))){
		
		setSkin(skin);
	}

	public ImageButton (ImageButtonStyle style)
	:base(style){
		
		image = newImage();
		add(image);
		setStyle(style);
		setSize(getPrefWidth(), getPrefHeight());
	}

	public ImageButton (IDrawable? imageUp)
	:this(new ImageButtonStyle(null, null, null, imageUp, null, null)){
		
	}

	public ImageButton (IDrawable? imageUp, IDrawable? imageDown)
	:this(new ImageButtonStyle(null, null, null, imageUp, imageDown, null)){
		
	}

	public ImageButton (IDrawable? imageUp, IDrawable? imageDown, IDrawable? imageChecked)
	:this(new ImageButtonStyle(null, null, null, imageUp, imageDown, imageChecked)){
		
	}

	protected Image newImage () {
		return new Image((IDrawable)null, Scaling.fit);
	}

	public override void setStyle (ButtonStyle style) {
		if (!(style is ImageButtonStyle)) throw new IllegalArgumentException("style must be an ImageButtonStyle.");
		this.style = (ImageButtonStyle)style;
		base.setStyle(style);

		if (image != null) updateImage();
	}

	public override ImageButtonStyle getStyle () {
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

	public override void draw (IBatch batch, float parentAlpha) {
		updateImage();
		base.draw(batch, parentAlpha);
	}

	public Image getImage () {
		return image;
	}

	public Cell<Image> getImageCell () {
		return getCell<Image>(image);
	}

	public override String ToString () {
		String name = getName();
		if (name != null) return name;
		String className = GetType().Name;
		int dotIndex = className.LastIndexOf('.');
		if (dotIndex != -1) className = className.Substring(dotIndex + 1);
		return (className.IndexOf('$') != -1 ? "ImageButton " : "") + className + ": " + image.getDrawable();
	}

	/** The style for an image button, see {@link ImageButton}.
	 * @author Nathan Sweet */
	 public class ImageButtonStyle : ButtonStyle {
		public IDrawable? imageUp, imageDown, imageOver, imageDisabled;
		public IDrawable? imageChecked, imageCheckedDown, imageCheckedOver;

		public ImageButtonStyle () {
		}

		public ImageButtonStyle (IDrawable? up, IDrawable? down, IDrawable? @checked, IDrawable? imageUp,
			IDrawable? imageDown, IDrawable? imageChecked) 
		: base(up, down, @checked)
		{
			
			this.imageUp = imageUp;
			this.imageDown = imageDown;
			this.imageChecked = imageChecked;
		}

		public ImageButtonStyle (ImageButtonStyle style) 
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

		public ImageButtonStyle (ButtonStyle style) 
		: base(style)
		{
			
		}
	}
}
