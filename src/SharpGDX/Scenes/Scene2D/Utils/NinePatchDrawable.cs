using System;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	/** Drawable for a {@link NinePatch}.
 * <p>
 * The drawable sizes are set when the ninepatch is set, but they are separate values. Eg, {@link Drawable#getLeftWidth()} could
 * be set to more than {@link NinePatch#getLeftWidth()} in order to provide more space on the left than actually exists in the
 * ninepatch.
 * <p>
 * The min size is set to the ninepatch total size by default. It could be set to the left+right and top+bottom, excluding the
 * middle size, to allow the drawable to be sized down as small as possible.
 * @author Nathan Sweet */
public class NinePatchDrawable : BaseDrawable , ITransformDrawable {
	private NinePatch patch;

	/** Creates an uninitialized NinePatchDrawable. The ninepatch must be {@link #setPatch(NinePatch) set} before use. */
	public NinePatchDrawable () {
	}

	public NinePatchDrawable (NinePatch patch) {
		setPatch(patch);
	}

	public NinePatchDrawable (NinePatchDrawable drawable) 
	: base(drawable)
	{
		
		this.patch = drawable.patch;
	}

		public override void draw (IBatch batch, float x, float y, float width, float height) {
		patch.draw(batch, x, y, width, height);
	}

	public void draw (IBatch batch, float x, float y, float originX, float originY, float width, float height, float scaleX,
		float scaleY, float rotation) {
		patch.draw(batch, x, y, originX, originY, width, height, scaleX, scaleY, rotation);
	}

	/** Sets this drawable's ninepatch and set the min width, min height, top height, right width, bottom height, and left width to
	 * the patch's padding. */
	public void setPatch (NinePatch patch) {
		this.patch = patch;
		if (patch != null) {
			setMinWidth(patch.getTotalWidth());
			setMinHeight(patch.getTotalHeight());
			setTopHeight(patch.getPadTop());
			setRightWidth(patch.getPadRight());
			setBottomHeight(patch.getPadBottom());
			setLeftWidth(patch.getPadLeft());
		}
	}

	public NinePatch getPatch () {
		return patch;
	}

	/** Creates a new drawable that renders the same as this drawable tinted the specified color. */
	public NinePatchDrawable tint (Color tint) {
		NinePatchDrawable drawable = new NinePatchDrawable(this);
		drawable.patch = new NinePatch(drawable.getPatch(), tint);
		return drawable;
	}
}
}
