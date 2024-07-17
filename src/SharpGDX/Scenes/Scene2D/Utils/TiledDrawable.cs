using SharpGDX;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	/** Draws a {@link TextureRegion} repeatedly to fill the area, instead of stretching it.
 * @author Nathan Sweet
 * @author Thomas Creutzenberg */
public class TiledDrawable : TextureRegionDrawable {
	private readonly Color color = new Color(1, 1, 1, 1);
	private float scale = 1;
	private int align = Align.bottomLeft;

	public TiledDrawable () {
	}

	public TiledDrawable (TextureRegion region) 
	: base(region)
	{
		
	}

	public TiledDrawable (TextureRegionDrawable drawable) 
	: base(drawable)
	{
		
	}

		public override void draw (IBatch batch, float x, float y, float width, float height) {
		float oldColor = batch.getPackedColor();
		batch.setColor(batch.getColor().mul(color));

		draw(batch, getRegion(), x, y, width, height, scale, align);

		batch.setPackedColor(oldColor);
	}

	public static void draw (IBatch batch, TextureRegion textureRegion, float x, float y, float width, float height, float scale,
		int align) {
		 float regionWidth = textureRegion.getRegionWidth() * scale;
		 float regionHeight = textureRegion.getRegionHeight() * scale;

		 Texture texture = textureRegion.getTexture();
		 float textureWidth = texture.getWidth() * scale;
		 float textureHeight = texture.getHeight() * scale;
		 float u = textureRegion.getU();
		 float v = textureRegion.getV();
		 float u2 = textureRegion.getU2();
		 float v2 = textureRegion.getV2();

		int fullX = (int)(width / regionWidth);
		 float leftPartialWidth;
		 float rightPartialWidth;
		if (Align.isLeft(align)) {
			leftPartialWidth = 0f;
			rightPartialWidth = width - (regionWidth * fullX);
		} else if (Align.isRight(align)) {
			leftPartialWidth = width - (regionWidth * fullX);
			rightPartialWidth = 0f;
		} else {
			if (fullX != 0) {
				fullX = fullX % 2 == 1 ? fullX : fullX - 1;
				 float leftRight = 0.5f * (width - (regionWidth * fullX));
				leftPartialWidth = leftRight;
				rightPartialWidth = leftRight;
			} else {
				leftPartialWidth = 0f;
				rightPartialWidth = 0f;
			}
		}
		int fullY = (int)(height / regionHeight);
		 float topPartialHeight;
		 float bottomPartialHeight;
		if (Align.isTop(align)) {
			topPartialHeight = 0f;
			bottomPartialHeight = height - (regionHeight * fullY);
		} else if (Align.isBottom(align)) {
			topPartialHeight = height - (regionHeight * fullY);
			bottomPartialHeight = 0f;
		} else {
			if (fullY != 0) {
				fullY = fullY % 2 == 1 ? fullY : fullY - 1;
				 float topBottom = 0.5f * (height - (regionHeight * fullY));
				topPartialHeight = topBottom;
				bottomPartialHeight = topBottom;
			} else {
				topPartialHeight = 0f;
				bottomPartialHeight = 0f;
			}
		}

		float drawX = x;
		float drawY = y;

		// Left edge
		if (leftPartialWidth > 0f) {
			 float leftEdgeU = u2 - (leftPartialWidth / textureWidth);

			// Left bottom partial
			if (bottomPartialHeight > 0f) {
				 float leftBottomV = v + (bottomPartialHeight / textureHeight);
				batch.draw(texture, drawX, drawY, leftPartialWidth, bottomPartialHeight, leftEdgeU, leftBottomV, u2, v);
				drawY += bottomPartialHeight;
			}

			// Left center partials
			if (fullY == 0 && Align.isCenterVertical(align)) {
				 float vOffset = 0.5f * (v2 - v) * (1f - (height / regionHeight));
				 float leftCenterV = v2 - vOffset;
				 float leftCenterV2 = v + vOffset;
				batch.draw(texture, drawX, drawY, leftPartialWidth, height, leftEdgeU, leftCenterV, u2, leftCenterV2);
				drawY += height;
			} else {
				for (int i = 0; i < fullY; i++) {
					batch.draw(texture, drawX, drawY, leftPartialWidth, regionHeight, leftEdgeU, v2, u2, v);
					drawY += regionHeight;
				}
			}

			// Left top partial
			if (topPartialHeight > 0f) {
				 float leftTopV = v2 - (topPartialHeight / textureHeight);
				batch.draw(texture, drawX, drawY, leftPartialWidth, topPartialHeight, leftEdgeU, v2, u2, leftTopV);
			}
		}

		// Center full texture regions
		{
			// Center bottom partials
			if (bottomPartialHeight > 0f) {
				drawX = x + leftPartialWidth;
				drawY = y;

				 float centerBottomV = v + (bottomPartialHeight / textureHeight);

				if (fullX == 0 && Align.isCenterHorizontal(align)) {
					 float uOffset = 0.5f * (u2 - u) * (1f - (width / regionWidth));
					 float centerBottomU = u + uOffset;
					 float centerBottomU2 = u2 - uOffset;
					batch.draw(texture, drawX, drawY, width, bottomPartialHeight, centerBottomU, centerBottomV, centerBottomU2, v);
					drawX += width;
				} else {
					for (int i = 0; i < fullX; i++) {
						batch.draw(texture, drawX, drawY, regionWidth, bottomPartialHeight, u, centerBottomV, u2, v);
						drawX += regionWidth;
					}
				}
			}

			// Center full texture regions
			{
				drawX = x + leftPartialWidth;

				 int originalFullX = fullX;
				 int originalFullY = fullY;

				float centerCenterDrawWidth = regionWidth;
				float centerCenterDrawHeight = regionHeight;
				float centerCenterU = u;
				float centerCenterU2 = u2;
				float centerCenterV = v2;
				float centerCenterV2 = v;
				if (fullX == 0 && Align.isCenterHorizontal(align)) {
					fullX = 1;
					centerCenterDrawWidth = width;
					 float uOffset = 0.5f * (u2 - u) * (1f - (width / regionWidth));
					centerCenterU = u + uOffset;
					centerCenterU2 = u2 - uOffset;
				}
				if (fullY == 0 && Align.isCenterVertical(align)) {
					fullY = 1;
					centerCenterDrawHeight = height;
					 float vOffset = 0.5f * (v2 - v) * (1f - (height / regionHeight));
					centerCenterV = v2 - vOffset;
					centerCenterV2 = v + vOffset;
				}
				for (int i = 0; i < fullX; i++) {
					drawY = y + bottomPartialHeight;
					for (int ii = 0; ii < fullY; ii++) {
						batch.draw(texture, drawX, drawY, centerCenterDrawWidth, centerCenterDrawHeight, centerCenterU, centerCenterV,
							centerCenterU2, centerCenterV2);
						drawY += centerCenterDrawHeight;
					}
					drawX += centerCenterDrawWidth;
				}

				fullX = originalFullX;
				fullY = originalFullY;
			}

			// Center top partials
			if (topPartialHeight > 0f) {
				drawX = x + leftPartialWidth;

				 float centerTopV = v2 - (topPartialHeight / textureHeight);

				if (fullX == 0 && Align.isCenterHorizontal(align)) {
					 float uOffset = 0.5f * (u2 - u) * (1f - (width / regionWidth));
					 float centerTopU = u + uOffset;
					 float centerTopU2 = u2 - uOffset;
					batch.draw(texture, drawX, drawY, width, topPartialHeight, centerTopU, v2, centerTopU2, centerTopV);
					drawX += width;
				} else {
					for (int i = 0; i < fullX; i++) {
						batch.draw(texture, drawX, drawY, regionWidth, topPartialHeight, u, v2, u2, centerTopV);
						drawX += regionWidth;
					}
				}
			}
		}

		// Right edge
		if (rightPartialWidth > 0f) {
			drawY = y;

			 float rightEdgeU2 = u + (rightPartialWidth / textureWidth);

			// Right bottom partial
			if (bottomPartialHeight > 0f) {
				 float rightBottomV = v + (bottomPartialHeight / textureHeight);
				batch.draw(texture, drawX, drawY, rightPartialWidth, bottomPartialHeight, u, rightBottomV, rightEdgeU2, v);
				drawY += bottomPartialHeight;
			}

			// Right center partials
			if (fullY == 0 && Align.isCenterVertical(align)) {
				 float vOffset = 0.5f * (v2 - v) * (1f - (height / regionHeight));
				 float rightCenterV = v2 - vOffset;
				 float rightCenterV2 = v + vOffset;
				batch.draw(texture, drawX, drawY, rightPartialWidth, height, u, rightCenterV, rightEdgeU2, rightCenterV2);
				drawY += height;
			} else {
				for (int i = 0; i < fullY; i++) {
					batch.draw(texture, drawX, drawY, rightPartialWidth, regionHeight, u, v2, rightEdgeU2, v);
					drawY += regionHeight;
				}
			}

			// Right top partial
			if (topPartialHeight > 0f) {
				 float rightTopV = v2 - (topPartialHeight / textureHeight);
				batch.draw(texture, drawX, drawY, rightPartialWidth, topPartialHeight, u, v2, rightEdgeU2, rightTopV);
			}
		}
	}

		public override void draw (IBatch batch, float x, float y, float originX, float originY, float width, float height, float scaleX,
		float scaleY, float rotation) {
		throw new UnsupportedOperationException();
	}

	public Color getColor () {
		return color;
	}

	public void setScale (float scale) {
		this.scale = scale;
	}

	public float getScale () {
		return scale;
	}

	public int getAlign () {
		return align;
	}

	public void setAlign (int align) {
		this.align = align;
	}

		public override TiledDrawable tint (Color tint) {
		TiledDrawable drawable = new TiledDrawable(this);
		drawable.color.set(tint);
		drawable.setLeftWidth(getLeftWidth());
		drawable.setRightWidth(getRightWidth());
		drawable.setTopHeight(getTopHeight());
		drawable.setBottomHeight(getBottomHeight());
		return drawable;
	}
}
}
