using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D.UI;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A tooltip that shows a label.
 * @author Nathan Sweet */
public class TextTooltip : Tooltip<Label> {
	public TextTooltip (String? text, Skin skin) 
	: this(text, TooltipManager.getInstance(), skin.get<TextTooltipStyle>(typeof(TextTooltipStyle)))
	{
		
	}

	public TextTooltip (String? text, Skin skin, String styleName) 
	: this(text, TooltipManager.getInstance(), skin.get<TextTooltipStyle>(styleName, typeof(TextTooltipStyle)))
	{
		
	}

	public TextTooltip (String? text, TextTooltipStyle style) 
	: this(text, TooltipManager.getInstance(), style)
	{
		
	}

	public TextTooltip (String? text, TooltipManager manager, Skin skin) 
	: this(text, manager, skin.get<TextTooltipStyle>(typeof(TextTooltipStyle)))
	{
		
	}

	public TextTooltip (String? text, TooltipManager manager, Skin skin, String styleName) 
	: this(text, manager, skin.get<TextTooltipStyle>(styleName, typeof(TextTooltipStyle)))
	{
		
	}

	public TextTooltip (String? text, TooltipManager manager, TextTooltipStyle style) 
	: base(null, manager)
	{
		

		container.setActor(newLabel(text, style.label));

		setStyle(style);
	}

	protected Label newLabel (String text, Label.LabelStyle style) {
		return new Label(text, style);
	}

	public void setStyle (TextTooltipStyle style) {
		if (style == null) throw new NullPointerException("style cannot be null");
		container.setBackground(style.background);
		container.maxWidth(style.wrapWidth);

		bool wrap = style.wrapWidth != 0;
		container.fill(wrap);

		Label label = container.getActor();
		label.setStyle(style.label);
		label.setWrap(wrap);
	}

	/** The style for a text tooltip, see {@link TextTooltip}.
	 * @author Nathan Sweet */
	public class TextTooltipStyle {
		public Label.LabelStyle label;
		public IDrawable? background;
		/** 0 means don't wrap. */
		public float wrapWidth;

		public TextTooltipStyle () {
		}

		public TextTooltipStyle (Label.LabelStyle label, IDrawable? background) {
			this.label = label;
			this.background = background;
		}

		public TextTooltipStyle (TextTooltipStyle style) {
			label = new Label.LabelStyle(style.label);
			background = style.background;
			wrapWidth = style.wrapWidth;
		}
	}
}
