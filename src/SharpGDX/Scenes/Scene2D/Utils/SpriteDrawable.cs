﻿using System;
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
		Color spriteColor = sprite.GetColor();
        float oldColor = sprite.GetPackedColor();
		sprite.SetColor(spriteColor.Mul(batch.GetColor()));

		sprite.SetRotation(0);
		sprite.SetScale(1, 1);
		sprite.SetBounds(x, y, width, height);
		sprite.Draw(batch);

		sprite.SetPackedColor(oldColor);
	}

	public void draw (IBatch batch, float x, float y, float originX, float originY, float width, float height, float scaleX,
		float scaleY, float rotation) {

		Color spriteColor = sprite.GetColor();
		float oldColor = sprite.GetPackedColor();
		sprite.SetColor(spriteColor.Mul(batch.GetColor()));

		sprite.SetOrigin(originX, originY);
		sprite.SetRotation(rotation);
		sprite.SetScale(scaleX, scaleY);
		sprite.SetBounds(x, y, width, height);
		sprite.Draw(batch);

		sprite.SetPackedColor(oldColor);
	}

	public void setSprite (Sprite sprite) {
		this.sprite = sprite;
		setMinWidth(sprite.GetWidth());
		setMinHeight(sprite.GetHeight());
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
		newSprite.SetColor(tint);
		newSprite.SetSize(getMinWidth(), getMinHeight());
		SpriteDrawable drawable = new SpriteDrawable(newSprite);
		drawable.setLeftWidth(getLeftWidth());
		drawable.setRightWidth(getRightWidth());
		drawable.setTopHeight(getTopHeight());
		drawable.setBottomHeight(getBottomHeight());
		return drawable;
	}
}
}
