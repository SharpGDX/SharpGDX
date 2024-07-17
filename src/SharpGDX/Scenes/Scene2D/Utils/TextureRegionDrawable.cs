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
	/** Drawable for a {@link TextureRegion}.
 * @author Nathan Sweet */
public class TextureRegionDrawable : BaseDrawable , ITransformDrawable {
	private TextureRegion? region;

	/** Creates an uninitialized TextureRegionDrawable. The texture region must be set before use. */
	public TextureRegionDrawable () {
	}

	public TextureRegionDrawable (Texture texture) {
		setRegion(new TextureRegion(texture));
	}

	public TextureRegionDrawable (TextureRegion region) {
		setRegion(region);
	}

	public TextureRegionDrawable (TextureRegionDrawable drawable) 
	: base(drawable)
	{
		
		setRegion(drawable.region);
	}

		public override void draw (IBatch batch, float x, float y, float width, float height) {
		batch.draw(region, x, y, width, height);
	}

	public virtual void draw (IBatch batch, float x, float y, float originX, float originY, float width, float height, float scaleX,
		float scaleY, float rotation) {
		batch.draw(region, x, y, originX, originY, width, height, scaleX, scaleY, rotation);
	}

	public void setRegion (TextureRegion? region) {
		this.region = region;
		if (region != null) {
			setMinWidth(region.getRegionWidth());
			setMinHeight(region.getRegionHeight());
		}
	}

	public TextureRegion getRegion () {
		return region;
	}

	/** Creates a new drawable that renders the same as this drawable tinted the specified color. */
	public virtual IDrawable tint (Color tint) {
		Sprite sprite;
		if (region is TextureAtlas.AtlasRegion)
			sprite = new TextureAtlas.AtlasSprite((TextureAtlas.AtlasRegion)region);
		else
			sprite = new Sprite(region);
		sprite.setColor(tint);
		sprite.setSize(getMinWidth(), getMinHeight());
		SpriteDrawable drawable = new SpriteDrawable(sprite);
		drawable.setLeftWidth(getLeftWidth());
		drawable.setRightWidth(getRightWidth());
		drawable.setTopHeight(getTopHeight());
		drawable.setBottomHeight(getBottomHeight());
		return drawable;
	}
}
}
