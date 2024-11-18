using SharpGDX.Graphics.GLUtils;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics.G2D
{
	/** A Batch is used to draw 2D rectangles that reference a texture (region). The class will batch the drawing commands and
 * optimize them for processing by the GPU.
 * <p>
 * To draw something with a Batch one has to first call the {@link Batch#begin()} method which will setup appropriate render
 * states. When you are done with drawing you have to call {@link Batch#end()} which will actually draw the things you specified.
 * <p>
 * All drawing commands of the Batch operate in screen coordinates. The screen coordinate system has an x-axis pointing to the
 * right, an y-axis pointing upwards and the origin is in the lower left corner of the screen. You can also provide your own
 * transformation and projection matrices if you so wish.
 * <p>
 * A Batch is managed. In case the OpenGL context is lost all OpenGL resources a Batch uses internally get invalidated. A context
 * is lost when a user switches to another application or receives an incoming call on Android. A Batch will be automatically
 * reloaded after the OpenGL context is restored.
 * <p>
 * A Batch is a pretty heavy object so you should only ever have one in your program.
 * <p>
 * A Batch works with OpenGL ES 2.0. It will use its own custom shader to draw all provided sprites. You can set your own custom
 * shader via {@link #setShader(ShaderProgram)}.
 * <p>
 * A Batch has to be disposed if it is no longer used.
 * @author mzechner
 * @author Nathan Sweet */
public interface IBatch : IDisposable {
	/** Sets up the Batch for drawing. This will disable depth buffer writing. It enables blending and texturing. If you have more
	 * texture units enabled than the first one you have to disable them before calling this. Uses a screen coordinate system by
	 * default where everything is given in pixels. You can specify your own projection and modelview matrices via
	 * {@link #setProjectionMatrix(Matrix4)} and {@link #setTransformMatrix(Matrix4)}. */
	public void Begin ();

        /// <summary>
        /// Finishes off rendering.
        /// </summary>
        /// <remarks>
        ///<para>
        /// Enables depth writes, disables blending and texturing.
        /// </para>
        /// <para>
        ///Must always be called after a call to <see cref="Begin()"/>.
        /// </para>
        /// </remarks>
        public void End ();

        /// <summary>
        /// Sets the color used to tint images when they are added to the Batch.
        /// </summary>
        /// <remarks>
        ///Default is <see cref="Color.White"/>.
        /// </remarks>
        /// <param name="tint"></param>
        public void SetColor (Color tint);

	/** @see #setColor(Color) */
	public void SetColor (float r, float g, float b, float a);

        /// <summary>
        /// Gets the rendering color of this Batch.
        /// </summary>
        /// <remarks>
        ///If the returned instance is manipulated, <see cref="SetColor(Color)"/> must be called afterward.
        /// </remarks>
        /// <returns>The rendering color of this Batch.</returns>
        public Color GetColor ();

        /// <summary>
        /// Sets the rendering color of this Batch, expanding the alpha from 0-254 to 0-255.
        /// </summary>
        /// <remarks>
        ///<para>
        ///See <see cref="SetColor(Color)"/>
        /// </para>
        /// <para>
        ///<see cref="Color.ToFloatBits()"/>
        /// </para>
        /// </remarks>
        /// <param name="packedColor"></param>
        public void SetPackedColor (float packedColor);

        /// <summary>
        /// Gets the rendering color of this Batch in vertex format (alpha compressed to 0-254).
        /// </summary>
        /// <remarks>
        ///See <see cref="Color.ToFloatBits()"/>
        /// </remarks>
        /// <returns>The rendering color of this Batch in vertex format (alpha compressed to 0-254).</returns>
        public float GetPackedColor ();

        /** Draws a rectangle with the bottom left corner at x,y having the given width and height in pixels. The rectangle is offset
         * by originX, originY relative to the origin. Scale specifies the scaling factor by which the rectangle should be scaled
         * around originX, originY. Rotation specifies the angle of counter clockwise rotation of the rectangle around originX,
         * originY. The portion of the {@link Texture} given by srcX, srcY and srcWidth, srcHeight is used. These coordinates and sizes
         * are given in texels. FlipX and flipY specify whether the texture portion should be flipped horizontally or vertically.
         * @param x the x-coordinate in screen space
         * @param y the y-coordinate in screen space
         * @param originX the x-coordinate of the scaling and rotation origin relative to the screen space coordinates
         * @param originY the y-coordinate of the scaling and rotation origin relative to the screen space coordinates
         * @param width the width in pixels
         * @param height the height in pixels
         * @param scaleX the scale of the rectangle around originX/originY in x
         * @param scaleY the scale of the rectangle around originX/originY in y
         * @param rotation the angle of counter clockwise rotation of the rectangle around originX/originY, in degrees
         * @param srcX the x-coordinate in texel space
         * @param srcY the y-coordinate in texel space
         * @param srcWidth the source with in texels
         * @param srcHeight the source height in texels
         * @param flipX whether to flip the sprite horizontally
         * @param flipY whether to flip the sprite vertically */
        public void Draw (Texture texture, float x, float y, float originX, float originY, float width, float height, float scaleX,
		float scaleY, float rotation, int srcX, int srcY, int srcWidth, int srcHeight, bool flipX, bool flipY);

	/** Draws a rectangle with the bottom left corner at x,y having the given width and height in pixels. The portion of the
	 * {@link Texture} given by srcX, srcY and srcWidth, srcHeight is used. These coordinates and sizes are given in texels. FlipX
	 * and flipY specify whether the texture portion should be flipped horizontally or vertically.
	 * @param x the x-coordinate in screen space
	 * @param y the y-coordinate in screen space
	 * @param width the width in pixels
	 * @param height the height in pixels
	 * @param srcX the x-coordinate in texel space
	 * @param srcY the y-coordinate in texel space
	 * @param srcWidth the source with in texels
	 * @param srcHeight the source height in texels
	 * @param flipX whether to flip the sprite horizontally
	 * @param flipY whether to flip the sprite vertically */
	public void Draw (Texture texture, float x, float y, float width, float height, int srcX, int srcY, int srcWidth,
		int srcHeight, bool flipX, bool flipY);

	/** Draws a rectangle with the bottom left corner at x,y having the given width and height in pixels. The portion of the
	 * {@link Texture} given by srcX, srcY and srcWidth, srcHeight are used. These coordinates and sizes are given in texels.
	 * @param x the x-coordinate in screen space
	 * @param y the y-coordinate in screen space
	 * @param srcX the x-coordinate in texel space
	 * @param srcY the y-coordinate in texel space
	 * @param srcWidth the source with in texels
	 * @param srcHeight the source height in texels */
	public void Draw (Texture texture, float x, float y, int srcX, int srcY, int srcWidth, int srcHeight);

	/** Draws a rectangle with the bottom left corner at x,y having the given width and height in pixels. The portion of the
	 * {@link Texture} given by u, v and u2, v2 are used. These coordinates and sizes are given in texture size percentage. The
	 * rectangle will have the given tint {@link Color}.
	 * @param x the x-coordinate in screen space
	 * @param y the y-coordinate in screen space
	 * @param width the width in pixels
	 * @param height the height in pixels */
	public void Draw (Texture texture, float x, float y, float width, float height, float u, float v, float u2, float v2);

	/** Draws a rectangle with the bottom left corner at x,y having the width and height of the texture.
	 * @param x the x-coordinate in screen space
	 * @param y the y-coordinate in screen space */
	public void Draw (Texture texture, float x, float y);

	/** Draws a rectangle with the bottom left corner at x,y and stretching the region to cover the given width and height. */
	public void Draw (Texture texture, float x, float y, float width, float height);

	/** Draws a rectangle using the given vertices. There must be 4 vertices, each made up of 5 elements in this order: x, y,
	 * color, u, v. The {@link #getColor()} from the Batch is not applied. */
	public void Draw (Texture texture, float[] spriteVertices, int offset, int count);

	/** Draws a rectangle with the bottom left corner at x,y having the width and height of the region. */
	public void Draw (TextureRegion region, float x, float y);

	/** Draws a rectangle with the bottom left corner at x,y and stretching the region to cover the given width and height. */
	public void Draw (TextureRegion region, float x, float y, float width, float height);

        /** Draws a rectangle with the bottom left corner at x,y and stretching the region to cover the given width and height. The
         * rectangle is offset by originX, originY relative to the origin. Scale specifies the scaling factor by which the rectangle
         * should be scaled around originX, originY. Rotation specifies the angle of counter clockwise rotation of the rectangle around
         * originX, originY.
         * @param rotation rotation in degrees */
        public void Draw (TextureRegion region, float x, float y, float originX, float originY, float width, float height,
		float scaleX, float scaleY, float rotation);

        /** Draws a rectangle with the texture coordinates rotated 90 degrees. The bottom left corner at x,y and stretching the region
         * to cover the given width and height. The rectangle is offset by originX, originY relative to the origin. Scale specifies the
         * scaling factor by which the rectangle should be scaled around originX, originY. Rotation specifies the angle of counter
         * clockwise rotation of the rectangle around originX, originY.
         * @param rotation rotation in degrees
         * @param clockwise If true, the texture coordinates are rotated 90 degrees clockwise. If false, they are rotated 90 degrees
         *           counter clockwise. */
        public void Draw (TextureRegion region, float x, float y, float originX, float originY, float width, float height,
		float scaleX, float scaleY, float rotation, bool clockwise);

	/** Draws a rectangle transformed by the given matrix. */
	public void Draw (TextureRegion region, float width, float height, Affine2 transform);

	/** Causes any pending sprites to be rendered, without ending the Batch. */
	public void Flush ();

	/** Disables blending for drawing sprites. Calling this within {@link #begin()}/{@link #end()} will flush the batch. */
	public void DisableBlending ();

	/** Enables blending for drawing sprites. Calling this within {@link #begin()}/{@link #end()} will flush the batch. */
	public void EnableBlending ();

	/** Sets the blending function to be used when rendering sprites.
	 * @param srcFunc the source function, e.g. GL20.GL_SRC_ALPHA. If set to -1, Batch won't change the blending function.
	 * @param dstFunc the destination function, e.g. GL20.GL_ONE_MINUS_SRC_ALPHA */
	public void SetBlendFunction (int srcFunc, int dstFunc);

	/** Sets separate (color/alpha) blending function to be used when rendering sprites.
	 * @param srcFuncColor the source color function, e.g. GL20.GL_SRC_ALPHA. If set to -1, Batch won't change the blending
	 *           function.
	 * @param dstFuncColor the destination color function, e.g. GL20.GL_ONE_MINUS_SRC_ALPHA.
	 * @param srcFuncAlpha the source alpha function, e.g. GL20.GL_SRC_ALPHA.
	 * @param dstFuncAlpha the destination alpha function, e.g. GL20.GL_ONE_MINUS_SRC_ALPHA. */
	public void SetBlendFunctionSeparate (int srcFuncColor, int dstFuncColor, int srcFuncAlpha, int dstFuncAlpha);

	public int GetBlendSrcFunc ();

	public int GetBlendDstFunc ();

	public int GetBlendSrcFuncAlpha ();

	public int GetBlendDstFuncAlpha ();

	/** Returns the current projection matrix. Changing this within {@link #begin()}/{@link #end()} results in undefined
	 * behaviour. */
	public Matrix4 GetProjectionMatrix ();

	/** Returns the current transform matrix. Changing this within {@link #begin()}/{@link #end()} results in undefined
	 * behaviour. */
	public Matrix4 GetTransformMatrix ();

	/** Sets the projection matrix to be used by this Batch. If this is called inside a {@link #begin()}/{@link #end()} block, the
	 * current batch is flushed to the gpu. */
	public void SetProjectionMatrix (Matrix4 projection);

	/** Sets the transform matrix to be used by this Batch. */
	public void SetTransformMatrix (Matrix4 transform);

	/** Sets the shader to be used in a GLES 2.0 environment. Vertex position attribute is called "a_position", the texture
	 * coordinates attribute is called "a_texCoord0", the color attribute is called "a_color". See
	 * {@link ShaderProgram#POSITION_ATTRIBUTE}, {@link ShaderProgram#COLOR_ATTRIBUTE} and {@link ShaderProgram#TEXCOORD_ATTRIBUTE}
	 * which gets "0" appended to indicate the use of the first texture unit. The combined transform and projection matrx is
	 * uploaded via a mat4 uniform called "u_projTrans". The texture sampler is passed via a uniform called "u_texture".
	 * <p>
	 * Call this method with a null argument to use the default shader.
	 * <p>
	 * This method will flush the batch before setting the new shader, you can call it in between {@link #begin()} and
	 * {@link #end()}.
	 * @param shader the {@link ShaderProgram} or null to use the default shader. */
	public void SetShader (ShaderProgram shader);

	/** @return the current {@link ShaderProgram} set by {@link #setShader(ShaderProgram)} or the defaultShader */
	public ShaderProgram GetShader ();

	/** @return true if blending for sprites is enabled */
	public bool IsBlendingEnabled ();

	/** @return true if currently between begin and end. */
	public bool IsDrawing ();

	public const int X1 = 0;
	public const int Y1 = 1;
	public const int C1 = 2;
	public const int U1 = 3;
	public const int V1 = 4;
	public const int X2 = 5;
	public const int Y2 = 6;
	public const int C2 = 7;
	public const int U2 = 8;
	public const int V2 = 9;
	public const int X3 = 10;
	public const int Y3 = 11;
	public const int C3 = 12;
	public const int U3 = 13;
	public const int V3 = 14;
	public const int X4 = 15;
	public const int Y4 = 16;
	public const int C4 = 17;
	public const int U4 = 18;
	public const int V4 = 19;
}
}
