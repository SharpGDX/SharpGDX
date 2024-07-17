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
	/** Drawable for a {@link Sprite}.
 * @author Nathan Sweet */
public class SpriteDrawable : BaseDrawable , ITransformDrawable {
	private Sprite sprite;

	/** Creates an uninitialized SpriteDrawable. The sprite must be set before use. */
	public SpriteDrawable () {
	}

	public SpriteDrawable (Sprite sprite) {
		setSprite(sprite);
	}

	public SpriteDrawable (SpriteDrawable drawable) 
	: base(drawable)
	{
		
		setSprite(drawable.sprite);
	}

		public override void draw (IBatch batch, float x, float y, float width, float height) {
		Color spriteColor = sprite.getColor();
		float oldColor = spriteColor.toFloatBits();
		sprite.setColor(spriteColor.mul(batch.getColor()));

		sprite.setRotation(0);
		sprite.setScale(1, 1);
		sprite.setBounds(x, y, width, height);
		sprite.draw(batch);

		sprite.setPackedColor(oldColor);
	}

	public void draw (IBatch batch, float x, float y, float originX, float originY, float width, float height, float scaleX,
		float scaleY, float rotation) {

		Color spriteColor = sprite.getColor();
		float oldColor = spriteColor.toFloatBits();
		sprite.setColor(spriteColor.mul(batch.getColor()));

		sprite.setOrigin(originX, originY);
		sprite.setRotation(rotation);
		sprite.setScale(scaleX, scaleY);
		sprite.setBounds(x, y, width, height);
		sprite.draw(batch);

		sprite.setPackedColor(oldColor);
	}

	public void setSprite (Sprite sprite) {
		this.sprite = sprite;
		setMinWidth(sprite.getWidth());
		setMinHeight(sprite.getHeight());
	}

	public Sprite getSprite () {
		return sprite;
	}

	/** Creates a new drawable that renders the same as this drawable tinted the specified color. */
	public SpriteDrawable tint (Color tint) {
		Sprite newSprite;
		if (sprite is TextureAtlas.AtlasSprite)
			newSprite = new TextureAtlas.AtlasSprite((TextureAtlas.AtlasSprite)sprite);
		else
			newSprite = new Sprite(sprite);
		newSprite.setColor(tint);
		newSprite.setSize(getMinWidth(), getMinHeight());
		SpriteDrawable drawable = new SpriteDrawable(newSprite);
		drawable.setLeftWidth(getLeftWidth());
		drawable.setRightWidth(getRightWidth());
		drawable.setTopHeight(getTopHeight());
		drawable.setBottomHeight(getBottomHeight());
		return drawable;
	}
}
}
