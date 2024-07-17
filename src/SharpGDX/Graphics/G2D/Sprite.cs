using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G2D
{
	/** Holds the geometry, color, and texture information for drawing 2D sprites using {@link Batch}. A Sprite has a position and a
 * size given as width and height. The position is relative to the origin of the coordinate system specified via
 * {@link Batch#begin()} and the respective matrices. A Sprite is always rectangular and its position (x, y) are located in the
 * bottom left corner of that rectangle. A Sprite also has an origin around which rotations and scaling are performed (that is,
 * the origin is not modified by rotation and scaling). The origin is given relative to the bottom left corner of the Sprite, its
 * position.
 * @author mzechner
 * @author Nathan Sweet */
public class Sprite : TextureRegion {
	internal static readonly int VERTEX_SIZE = 2 + 1 + 2;
	internal static readonly int SPRITE_SIZE = 4 * VERTEX_SIZE;

	readonly float[] vertices = new float[SPRITE_SIZE];
	private readonly Color color = new Color(1, 1, 1, 1);
	private float x, y;
	protected float width, height;
	private float originX, originY;
	private float rotation;
	private float scaleX = 1, scaleY = 1;
	private bool dirty = true;
	private Rectangle bounds;

	/** Creates an uninitialized sprite. The sprite will need a texture region and bounds set before it can be drawn. */
	public Sprite () {
		setColor(1, 1, 1, 1);
	}

	/** Creates a sprite with width, height, and texture region equal to the size of the texture. */
	public Sprite (Texture texture) 
	: this(texture, 0, 0, texture.getWidth(), texture.getHeight())
	{
		
	}

	/** Creates a sprite with width, height, and texture region equal to the specified size. The texture region's upper left corner
	 * will be 0,0.
	 * @param srcWidth The width of the texture region. May be negative to flip the sprite when drawn.
	 * @param srcHeight The height of the texture region. May be negative to flip the sprite when drawn. */
	public Sprite (Texture texture, int srcWidth, int srcHeight) 
	: this(texture, 0, 0, srcWidth, srcHeight)
	{
		
	}

	/** Creates a sprite with width, height, and texture region equal to the specified size.
	 * @param srcWidth The width of the texture region. May be negative to flip the sprite when drawn.
	 * @param srcHeight The height of the texture region. May be negative to flip the sprite when drawn. */
	public Sprite (Texture texture, int srcX, int srcY, int srcWidth, int srcHeight) {
		if (texture == null) throw new IllegalArgumentException("texture cannot be null.");
		this.texture = texture;
		setRegion(srcX, srcY, srcWidth, srcHeight);
		setColor(1, 1, 1, 1);
		setSize(Math.Abs(srcWidth), Math.Abs(srcHeight));
		setOrigin(width / 2, height / 2);
	}

	// Note the region is copied.
	/** Creates a sprite based on a specific TextureRegion, the new sprite's region is a copy of the parameter region - altering
	 * one does not affect the other */
	public Sprite (TextureRegion region) {
		setRegion(region);
		setColor(1, 1, 1, 1);
		setSize(region.getRegionWidth(), region.getRegionHeight());
		setOrigin(width / 2, height / 2);
	}

	/** Creates a sprite with width, height, and texture region equal to the specified size, relative to specified sprite's texture
	 * region.
	 * @param srcWidth The width of the texture region. May be negative to flip the sprite when drawn.
	 * @param srcHeight The height of the texture region. May be negative to flip the sprite when drawn. */
	public Sprite (TextureRegion region, int srcX, int srcY, int srcWidth, int srcHeight) {
		setRegion(region, srcX, srcY, srcWidth, srcHeight);
		setColor(1, 1, 1, 1);
		setSize(Math.Abs(srcWidth), Math.Abs(srcHeight));
		setOrigin(width / 2, height / 2);
	}

	/** Creates a sprite that is a copy in every way of the specified sprite. */
	public Sprite (Sprite sprite) {
		set(sprite);
	}

	/** Make this sprite a copy in every way of the specified sprite */
	public void set (Sprite sprite) {
		if (sprite == null) throw new IllegalArgumentException("sprite cannot be null.");
		Array.Copy(sprite.vertices, 0, vertices, 0, SPRITE_SIZE);
		texture = sprite.texture;
		u = sprite.u;
		v = sprite.v;
		u2 = sprite.u2;
		v2 = sprite.v2;
		x = sprite.x;
		y = sprite.y;
		width = sprite.width;
		height = sprite.height;
		regionWidth = sprite.regionWidth;
		regionHeight = sprite.regionHeight;
		originX = sprite.originX;
		originY = sprite.originY;
		rotation = sprite.rotation;
		scaleX = sprite.scaleX;
		scaleY = sprite.scaleY;
		color.set(sprite.color);
		dirty = sprite.dirty;
	}

	/** Sets the position and size of the sprite when drawn, before scaling and rotation are applied. If origin, rotation, or scale
	 * are changed, it is slightly more efficient to set the bounds after those operations. */
	public virtual void setBounds (float x, float y, float width, float height) {
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;

		if (dirty) return;
		if (rotation != 0 || scaleX != 1 || scaleY != 1) {
			dirty = true;
			return;
		}

		float x2 = x + width;
		float y2 = y + height;
		float[] vertices = this.vertices;
		vertices[IBatch.X1] = x;
		vertices[IBatch.Y1] = y;

		vertices[IBatch.X2] = x;
		vertices[IBatch.Y2] = y2;

		vertices[IBatch.X3] = x2;
		vertices[IBatch.Y3] = y2;

		vertices[IBatch.X4] = x2;
		vertices[IBatch.Y4] = y;
	}

	/** Sets the size of the sprite when drawn, before scaling and rotation are applied. If origin, rotation, or scale are changed,
	 * it is slightly more efficient to set the size after those operations. If both position and size are to be changed, it is
	 * better to use {@link #setBounds(float, float, float, float)}. */
	public virtual void setSize (float width, float height) {
		this.width = width;
		this.height = height;

		if (dirty) return;
		if (rotation != 0 || scaleX != 1 || scaleY != 1) {
			dirty = true;
			return;
		}

		float x2 = x + width;
		float y2 = y + height;
		float[] vertices = this.vertices;
		vertices[IBatch.X1] = x;
		vertices[IBatch.Y1] = y;

		vertices[IBatch.X2] = x;
		vertices[IBatch.Y2] = y2;

		vertices[IBatch.X3] = x2;
		vertices[IBatch.Y3] = y2;

		vertices[IBatch.X4] = x2;
		vertices[IBatch.Y4] = y;
	}

	/** Sets the position where the sprite will be drawn. If origin, rotation, or scale are changed, it is slightly more efficient
	 * to set the position after those operations. If both position and size are to be changed, it is better to use
	 * {@link #setBounds(float, float, float, float)}. */
	public virtual void setPosition (float x, float y) {
		this.x = x;
		this.y = y;

		if (dirty) return;
		if (rotation != 0 || scaleX != 1 || scaleY != 1) {
			dirty = true;
			return;
		}

		float x2 = x + width;
		float y2 = y + height;
		float[] vertices = this.vertices;
		vertices[IBatch.X1] = x;
		vertices[IBatch.Y1] = y;

		vertices[IBatch.X2] = x;
		vertices[IBatch.Y2] = y2;

		vertices[IBatch.X3] = x2;
		vertices[IBatch.Y3] = y2;

		vertices[IBatch.X4] = x2;
		vertices[IBatch.Y4] = y;
	}

	/** Sets the position where the sprite will be drawn, relative to its current origin. */
	public void setOriginBasedPosition (float x, float y) {
		setPosition(x - this.originX, y - this.originY);
	}

	/** Sets the x position where the sprite will be drawn. If origin, rotation, or scale are changed, it is slightly more
	 * efficient to set the position after those operations. If both position and size are to be changed, it is better to use
	 * {@link #setBounds(float, float, float, float)}. */
	public virtual void setX (float x) {
		this.x = x;

		if (dirty) return;
		if (rotation != 0 || scaleX != 1 || scaleY != 1) {
			dirty = true;
			return;
		}

		float x2 = x + width;
		float[] vertices = this.vertices;
		vertices[IBatch.X1] = x;
		vertices[IBatch.X2] = x;
		vertices[IBatch.X3] = x2;
		vertices[IBatch.X4] = x2;
	}

	/** Sets the y position where the sprite will be drawn. If origin, rotation, or scale are changed, it is slightly more
	 * efficient to set the position after those operations. If both position and size are to be changed, it is better to use
	 * {@link #setBounds(float, float, float, float)}. */
	public virtual void setY (float y) {
		this.y = y;

		if (dirty) return;
		if (rotation != 0 || scaleX != 1 || scaleY != 1) {
			dirty = true;
			return;
		}

		float y2 = y + height;
		float[] vertices = this.vertices;
		vertices[IBatch.Y1] = y;
		vertices[IBatch.Y2] = y2;
		vertices[IBatch.Y3] = y2;
		vertices[IBatch.Y4] = y;
	}

	/** Sets the x position so that it is centered on the given x parameter */
	public void setCenterX (float x) {
		setX(x - width / 2);
	}

	/** Sets the y position so that it is centered on the given y parameter */
	public void setCenterY (float y) {
		setY(y - height / 2);
	}

	/** Sets the position so that the sprite is centered on (x, y) */
	public void setCenter (float x, float y) {
		setPosition(x - width / 2, y - height / 2);
	}

	/** Sets the x position relative to the current position where the sprite will be drawn. If origin, rotation, or scale are
	 * changed, it is slightly more efficient to translate after those operations. */
	public void translateX (float xAmount) {
		this.x += xAmount;

		if (dirty) return;
		if (rotation != 0 || scaleX != 1 || scaleY != 1) {
			dirty = true;
			return;
		}

		float[] vertices = this.vertices;
		vertices[IBatch.X1] += xAmount;
		vertices[IBatch.X2] += xAmount;
		vertices[IBatch.X3] += xAmount;
		vertices[IBatch.X4] += xAmount;
	}

	/** Sets the y position relative to the current position where the sprite will be drawn. If origin, rotation, or scale are
	 * changed, it is slightly more efficient to translate after those operations. */
	public void translateY (float yAmount) {
		y += yAmount;

		if (dirty) return;
		if (rotation != 0 || scaleX != 1 || scaleY != 1) {
			dirty = true;
			return;
		}

		float[] vertices = this.vertices;
		vertices[IBatch.Y1] += yAmount;
		vertices[IBatch.Y2] += yAmount;
		vertices[IBatch.Y3] += yAmount;
		vertices[IBatch.Y4] += yAmount;
	}

	/** Sets the position relative to the current position where the sprite will be drawn. If origin, rotation, or scale are
	 * changed, it is slightly more efficient to translate after those operations. */
	public void translate (float xAmount, float yAmount) {
		x += xAmount;
		y += yAmount;

		if (dirty) return;
		if (rotation != 0 || scaleX != 1 || scaleY != 1) {
			dirty = true;
			return;
		}

		float[] vertices = this.vertices;
		vertices[IBatch.X1] += xAmount;
		vertices[IBatch.Y1] += yAmount;

		vertices[IBatch.X2] += xAmount;
		vertices[IBatch.Y2] += yAmount;

		vertices[IBatch.X3] += xAmount;
		vertices[IBatch.Y3] += yAmount;

		vertices[IBatch.X4] += xAmount;
		vertices[IBatch.Y4] += yAmount;
	}

	/** Sets the color used to tint this sprite. Default is {@link Color#WHITE}. */
	public void setColor (Color tint) {
		this.color.set(tint);
		float color = tint.toFloatBits();
		float[] vertices = this.vertices;
		vertices[IBatch.C1] = color;
		vertices[IBatch.C2] = color;
		vertices[IBatch.C3] = color;
		vertices[IBatch.C4] = color;
	}

	/** Sets the alpha portion of the color used to tint this sprite. */
	public void setAlpha (float a) {
		this.color.a = a;
		float color = this.color.toFloatBits();
		vertices[IBatch.C1] = color;
		vertices[IBatch.C2] = color;
		vertices[IBatch.C3] = color;
		vertices[IBatch.C4] = color;
	}

	/** @see #setColor(Color) */
	public void setColor (float r, float g, float b, float a) {
		this.color.set(r, g, b, a);
		float color = this.color.toFloatBits();
		float[] vertices = this.vertices;
		vertices[IBatch.C1] = color;
		vertices[IBatch.C2] = color;
		vertices[IBatch.C3] = color;
		vertices[IBatch.C4] = color;
	}

	/** Sets the color of this sprite, expanding the alpha from 0-254 to 0-255.
	 * @see #setColor(Color)
	 * @see Color#toFloatBits() */
	public void setPackedColor (float packedColor) {
		Color.abgr8888ToColor(color, packedColor);
		float[] vertices = this.vertices;
		vertices[IBatch.C1] = packedColor;
		vertices[IBatch.C2] = packedColor;
		vertices[IBatch.C3] = packedColor;
		vertices[IBatch.C4] = packedColor;
	}

	/** Sets the origin in relation to the sprite's position for scaling and rotation. */
	public virtual void setOrigin (float originX, float originY) {
		this.originX = originX;
		this.originY = originY;
		dirty = true;
	}

	/** Place origin in the center of the sprite */
	public virtual void setOriginCenter () {
		this.originX = width / 2;
		this.originY = height / 2;
		dirty = true;
	}

	/** Sets the rotation of the sprite in degrees. Rotation is centered on the origin set in {@link #setOrigin(float, float)} */
	public void setRotation (float degrees) {
		this.rotation = degrees;
		dirty = true;
	}

	/** @return the rotation of the sprite in degrees */
	public float getRotation () {
		return rotation;
	}

	/** Sets the sprite's rotation in degrees relative to the current rotation. Rotation is centered on the origin set in
	 * {@link #setOrigin(float, float)} */
	public void rotate (float degrees) {
		if (degrees == 0) return;
		rotation += degrees;
		dirty = true;
	}

	/** Rotates this sprite 90 degrees in-place by rotating the texture coordinates. This rotation is unaffected by
	 * {@link #setRotation(float)} and {@link #rotate(float)}. */
	public virtual void rotate90 (bool clockwise) {
		float[] vertices = this.vertices;

		if (clockwise) {
			float temp = vertices[IBatch.V1];
			vertices[IBatch.V1] = vertices[IBatch.V4];
			vertices[IBatch.V4] = vertices[IBatch.V3];
			vertices[IBatch.V3] = vertices[IBatch.V2];
			vertices[IBatch.V2] = temp;

			temp = vertices[IBatch.U1];
			vertices[IBatch.U1] = vertices[IBatch.U4];
			vertices[IBatch.U4] = vertices[IBatch.U3];
			vertices[IBatch.U3] = vertices[IBatch.U2];
			vertices[IBatch.U2] = temp;
		} else {
			float temp = vertices[IBatch.V1];
			vertices[IBatch.V1] = vertices[IBatch.V2];
			vertices[IBatch.V2] = vertices[IBatch.V3];
			vertices[IBatch.V3] = vertices[IBatch.V4];
			vertices[IBatch.V4] = temp;

			temp = vertices[IBatch.U1];
			vertices[IBatch.U1] = vertices[IBatch.U2];
			vertices[IBatch.U2] = vertices[IBatch.U3];
			vertices[IBatch.U3] = vertices[IBatch.U4];
			vertices[IBatch.U4] = temp;
		}
	}

	/** Sets the sprite's scale for both X and Y uniformly. The sprite scales out from the origin. This will not affect the values
	 * returned by {@link #getWidth()} and {@link #getHeight()} */
	public void setScale (float scaleXY) {
		this.scaleX = scaleXY;
		this.scaleY = scaleXY;
		dirty = true;
	}

	/** Sets the sprite's scale for both X and Y. The sprite scales out from the origin. This will not affect the values returned
	 * by {@link #getWidth()} and {@link #getHeight()} */
	public void setScale (float scaleX, float scaleY) {
		this.scaleX = scaleX;
		this.scaleY = scaleY;
		dirty = true;
	}

	/** Sets the sprite's scale relative to the current scale. for example: original scale 2 -> sprite.scale(4) -> final scale 6.
	 * The sprite scales out from the origin. This will not affect the values returned by {@link #getWidth()} and
	 * {@link #getHeight()} */
	public void scale (float amount) {
		this.scaleX += amount;
		this.scaleY += amount;
		dirty = true;
	}

	/** Returns the packed vertices, colors, and texture coordinates for this sprite. */
	public float[] getVertices () {
		if (dirty) {
			dirty = false;

			float[] vertices = this.vertices;
			float localX = -originX;
			float localY = -originY;
			float localX2 = localX + width;
			float localY2 = localY + height;
			float worldOriginX = this.x - localX;
			float worldOriginY = this.y - localY;
			if (scaleX != 1 || scaleY != 1) {
				localX *= scaleX;
				localY *= scaleY;
				localX2 *= scaleX;
				localY2 *= scaleY;
			}
			if (rotation != 0) {
				 float cos = MathUtils.cosDeg(rotation);
				 float sin = MathUtils.sinDeg(rotation);
				 float localXCos = localX * cos;
				 float localXSin = localX * sin;
				 float localYCos = localY * cos;
				 float localYSin = localY * sin;
				 float localX2Cos = localX2 * cos;
				 float localX2Sin = localX2 * sin;
				 float localY2Cos = localY2 * cos;
				 float localY2Sin = localY2 * sin;

				 float x1 = localXCos - localYSin + worldOriginX;
				 float y1 = localYCos + localXSin + worldOriginY;
				vertices[IBatch.X1] = x1;
				vertices[IBatch.Y1] = y1;

				 float x2 = localXCos - localY2Sin + worldOriginX;
				 float y2 = localY2Cos + localXSin + worldOriginY;
				vertices[IBatch.X2] = x2;
				vertices[IBatch.Y2] = y2;

				 float x3 = localX2Cos - localY2Sin + worldOriginX;
				 float y3 = localY2Cos + localX2Sin + worldOriginY;
				vertices[IBatch.X3] = x3;
				vertices[IBatch.Y3] = y3;

				vertices[IBatch.X4] = x1 + (x3 - x2);
				vertices[IBatch.Y4] = y3 - (y2 - y1);
			} else {
				 float x1 = localX + worldOriginX;
				 float y1 = localY + worldOriginY;
				 float x2 = localX2 + worldOriginX;
				 float y2 = localY2 + worldOriginY;

				vertices[IBatch.X1] = x1;
				vertices[IBatch.Y1] = y1;

				vertices[IBatch.X2] = x1;
				vertices[IBatch.Y2] = y2;

				vertices[IBatch.X3] = x2;
				vertices[IBatch.Y3] = y2;

				vertices[IBatch.X4] = x2;
				vertices[IBatch.Y4] = y1;
			}
		}
		return vertices;
	}

	/** Returns the bounding axis aligned {@link Rectangle} that bounds this sprite. The rectangles x and y coordinates describe
	 * its bottom left corner. If you change the position or size of the sprite, you have to fetch the triangle again for it to be
	 * recomputed.
	 * 
	 * @return the bounding Rectangle */
	public Rectangle getBoundingRectangle () {
		 float[] vertices = getVertices();

		float minx = vertices[IBatch.X1];
		float miny = vertices[IBatch.Y1];
		float maxx = vertices[IBatch.X1];
		float maxy = vertices[IBatch.Y1];

		minx = minx > vertices[IBatch.X2] ? vertices[IBatch.X2] : minx;
		minx = minx > vertices[IBatch.X3] ? vertices[IBatch.X3] : minx;
		minx = minx > vertices[IBatch.X4] ? vertices[IBatch.X4] : minx;

		maxx = maxx < vertices[IBatch.X2] ? vertices[IBatch.X2] : maxx;
		maxx = maxx < vertices[IBatch.X3] ? vertices[IBatch.X3] : maxx;
		maxx = maxx < vertices[IBatch.X4] ? vertices[IBatch.X4] : maxx;

		miny = miny > vertices[IBatch.Y2] ? vertices[IBatch.Y2] : miny;
		miny = miny > vertices[IBatch.Y3] ? vertices[IBatch.Y3] : miny;
		miny = miny > vertices[IBatch.Y4] ? vertices[IBatch.Y4] : miny;

		maxy = maxy < vertices[IBatch.Y2] ? vertices[IBatch.Y2] : maxy;
		maxy = maxy < vertices[IBatch.Y3] ? vertices[IBatch.Y3] : maxy;
		maxy = maxy < vertices[IBatch.Y4] ? vertices[IBatch.Y4] : maxy;

		if (bounds == null) bounds = new Rectangle();
		bounds.x = minx;
		bounds.y = miny;
		bounds.width = maxx - minx;
		bounds.height = maxy - miny;
		return bounds;
	}

	public void draw (IBatch batch) {
		batch.draw(texture, getVertices(), 0, SPRITE_SIZE);
	}

	public void draw (IBatch batch, float alphaModulation) {
		float oldAlpha = getColor().a;
		setAlpha(oldAlpha * alphaModulation);
		draw(batch);
		setAlpha(oldAlpha);
	}

		public virtual float getX () {
		return x;
	}

		public virtual float getY () {
		return y;
	}

	/** @return the width of the sprite, not accounting for scale. */
	public virtual float getWidth () {
		return width;
	}

	/** @return the height of the sprite, not accounting for scale. */
	public virtual float getHeight () {
		return height;
	}

		/** The origin influences {@link #setPosition(float, float)}, {@link #setRotation(float)} and the expansion direction of
		 * scaling {@link #setScale(float, float)} */
		public virtual float getOriginX () {
		return originX;
	}

		/** The origin influences {@link #setPosition(float, float)}, {@link #setRotation(float)} and the expansion direction of
		 * scaling {@link #setScale(float, float)} */
		public virtual float getOriginY () {
		return originY;
	}

	/** X scale of the sprite, independent of size set by {@link #setSize(float, float)} */
	public float getScaleX () {
		return scaleX;
	}

	/** Y scale of the sprite, independent of size set by {@link #setSize(float, float)} */
	public float getScaleY () {
		return scaleY;
	}

	/** Returns the color of this sprite. If the returned instance is manipulated, {@link #setColor(Color)} must be called
	 * afterward. */
	public Color getColor () {
		return color;
	}

		public override void setRegion (float u, float v, float u2, float v2) {
		base.setRegion(u, v, u2, v2);

		float[] vertices = this.vertices;
		vertices[IBatch.U1] = u;
		vertices[IBatch.V1] = v2;

		vertices[IBatch.U2] = u;
		vertices[IBatch.V2] = v;

		vertices[IBatch.U3] = u2;
		vertices[IBatch.V3] = v;

		vertices[IBatch.U4] = u2;
		vertices[IBatch.V4] = v2;
	}

		public override void setU (float u) {
		base.setU(u);
		vertices[IBatch.U1] = u;
		vertices[IBatch.U2] = u;
	}

		public override void setV (float v) {
		base.setV(v);
		vertices[IBatch.V2] = v;
		vertices[IBatch.V3] = v;
	}

		public override void setU2 (float u2) {
		base.setU2(u2);
		vertices[IBatch.U3] = u2;
		vertices[IBatch.U4] = u2;
	}

		public override void setV2 (float v2) {
		base.setV2(v2);
		vertices[IBatch.V1] = v2;
		vertices[IBatch.V4] = v2;
	}

	/** Set the sprite's flip state regardless of current condition
	 * @param x the desired horizontal flip state
	 * @param y the desired vertical flip state */
	public void setFlip (bool x, bool y) {
		bool performX = false;
		bool performY = false;
		if (isFlipX() != x) {
			performX = true;
		}
		if (isFlipY() != y) {
			performY = true;
		}
		flip(performX, performY);
	}

		/** boolean parameters x,y are not setting a state, but performing a flip
		 * @param x perform horizontal flip
		 * @param y perform vertical flip */
		public override void flip (bool x, bool y) {
		base.flip(x, y);
		float[] vertices = this.vertices;
		if (x) {
			float temp = vertices[IBatch.U1];
			vertices[IBatch.U1] = vertices[IBatch.U3];
			vertices[IBatch.U3] = temp;
			temp = vertices[IBatch.U2];
			vertices[IBatch.U2] = vertices[IBatch.U4];
			vertices[IBatch.U4] = temp;
		}
		if (y) {
			float temp = vertices[IBatch.V1];
			vertices[IBatch.V1] = vertices[IBatch.V3];
			vertices[IBatch.V3] = temp;
			temp = vertices[IBatch.V2];
			vertices[IBatch.V2] = vertices[IBatch.V4];
			vertices[IBatch.V4] = temp;
		}
	}

		public override void scroll (float xAmount, float yAmount) {
		float[] vertices = this.vertices;
		if (xAmount != 0) {
			float u = (vertices[IBatch.U1] + xAmount) % 1;
			float u2 = u + width / texture.getWidth();
			this.u = u;
			this.u2 = u2;
			vertices[IBatch.U1] = u;
			vertices[IBatch.U2] = u;
			vertices[IBatch.U3] = u2;
			vertices[IBatch.U4] = u2;
		}
		if (yAmount != 0) {
			float v = (vertices[IBatch.V2] + yAmount) % 1;
			float v2 = v + height / texture.getHeight();
			this.v = v;
			this.v2 = v2;
			vertices[IBatch.V1] = v2;
			vertices[IBatch.V2] = v;
			vertices[IBatch.V3] = v;
			vertices[IBatch.V4] = v2;
		}
	}
}
}
