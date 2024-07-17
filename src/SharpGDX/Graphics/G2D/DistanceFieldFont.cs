using SharpGDX.Files;
using SharpGDX.Utils;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Shims;

namespace SharpGDX.Graphics.G2D
{
	/** Renders bitmap fonts using distance field textures, see the
 * <a href="https://libgdx.com/wiki/graphics/2d/fonts/distance-field-fonts">Distance Field Fonts wiki article</a> for usage.
 * Initialize the SpriteBatch with the {@link #createDistanceFieldShader()} shader.
 * <p>
 * Attention: The batch is flushed before and after each string is rendered.
 * @author Florian Falkner */
public class DistanceFieldFont : BitmapFont {
	private float distanceFieldSmoothing;

	public DistanceFieldFont (BitmapFontData data, Array<TextureRegion> pageRegions, bool integer) 
	: base(data, pageRegions, integer)
	{
		
	}

	public DistanceFieldFont (BitmapFontData data, TextureRegion region, bool integer) 
	: base(data, region, integer)
	{
		
	}

	public DistanceFieldFont (FileHandle fontFile, bool flip) 
	: base(fontFile, flip)
	{
		
	}

	public DistanceFieldFont (FileHandle fontFile, FileHandle imageFile, bool flip, bool integer) 
	: base(fontFile, imageFile, flip, integer)
	{
		
	}

	public DistanceFieldFont (FileHandle fontFile, FileHandle imageFile, bool flip) 
	: base(fontFile, imageFile, flip)
	{
		
	}

	public DistanceFieldFont (FileHandle fontFile, TextureRegion region, bool flip) 
	: base(fontFile, region, flip)
	{
		
	}

	public DistanceFieldFont (FileHandle fontFile, TextureRegion region) 
	: base(fontFile, region)
	{
		
	}

	public DistanceFieldFont (FileHandle fontFile) 
	: base(fontFile)
	{
		
	}

	protected override void load (BitmapFontData data) {
		base.load(data);

		// Distance field font rendering requires font texture to be filtered linear.
		 Array<TextureRegion> regions = getRegions();
		foreach (TextureRegion region in regions)
			region.getTexture().setFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
	}

	public override BitmapFontCache newFontCache () {
		return new DistanceFieldFontCache(this, integer);
	}

	/** @return The distance field smoothing factor for this font. */
	public float getDistanceFieldSmoothing () {
		return distanceFieldSmoothing;
	}

	/** @param distanceFieldSmoothing Set the distance field smoothing factor for this font. SpriteBatch needs to have this shader
	 *           set for rendering distance field fonts. */
	public void setDistanceFieldSmoothing (float distanceFieldSmoothing) {
		this.distanceFieldSmoothing = distanceFieldSmoothing;
	}

	/** Returns a new instance of the distance field shader, see https://libgdx.com/wiki/graphics/2d/fonts/distance-field-fonts if
	 * the u_smoothing uniform > 0.0. Otherwise the same code as the default SpriteBatch shader is used. */
	static public ShaderProgram createDistanceFieldShader () {
		String vertexShader = "attribute vec4 " + ShaderProgram.POSITION_ATTRIBUTE + ";\n" //
			+ "attribute vec4 " + ShaderProgram.COLOR_ATTRIBUTE + ";\n" //
			+ "attribute vec2 " + ShaderProgram.TEXCOORD_ATTRIBUTE + "0;\n" //
			+ "uniform mat4 u_projTrans;\n" //
			+ "varying vec4 v_color;\n" //
			+ "varying vec2 v_texCoords;\n" //
			+ "\n" //
			+ "void main() {\n" //
			+ "	v_color = " + ShaderProgram.COLOR_ATTRIBUTE + ";\n" //
			+ "	v_color.a = v_color.a * (255.0/254.0);\n" //
			+ "	v_texCoords = " + ShaderProgram.TEXCOORD_ATTRIBUTE + "0;\n" //
			+ "	gl_Position =  u_projTrans * " + ShaderProgram.POSITION_ATTRIBUTE + ";\n" //
			+ "}\n";

		String fragmentShader = "#ifdef GL_ES\n" //
			+ "	precision mediump float;\n" //
			+ "	precision mediump int;\n" //
			+ "#endif\n" //
			+ "\n" //
			+ "uniform sampler2D u_texture;\n" //
			+ "uniform float u_smoothing;\n" //
			+ "varying vec4 v_color;\n" //
			+ "varying vec2 v_texCoords;\n" //
			+ "\n" //
			+ "void main() {\n" //
			+ "	if (u_smoothing > 0.0) {\n" //
			+ "		float smoothing = 0.25 / u_smoothing;\n" //
			+ "		float distance = texture2D(u_texture, v_texCoords).a;\n" //
			+ "		float alpha = smoothstep(0.5 - smoothing, 0.5 + smoothing, distance);\n" //
			+ "		gl_FragColor = vec4(v_color.rgb, alpha * v_color.a);\n" //
			+ "	} else {\n" //
			+ "		gl_FragColor = v_color * texture2D(u_texture, v_texCoords);\n" //
			+ "	}\n" //
			+ "}\n";

		ShaderProgram shader = new ShaderProgram(vertexShader, fragmentShader);
		if (!shader.isCompiled()) throw new IllegalArgumentException("Error compiling distance field shader: " + shader.getLog());
		return shader;
	}

	/** Provides a font cache that uses distance field shader for rendering fonts. Attention: breaks batching because uniform is
	 * needed for smoothing factor, so a flush is performed before and after every font rendering.
	 * @author Florian Falkner */
	 private class DistanceFieldFontCache : BitmapFontCache {
		public DistanceFieldFontCache (DistanceFieldFont font) 
		: base(font, font.usesIntegerPositions())
		{
			
		}

		public DistanceFieldFontCache (DistanceFieldFont font, bool integer) 
		: base(font, integer)
		{
			
		}

		private float getSmoothingFactor () {
			 DistanceFieldFont font = (DistanceFieldFont)getFont();
			return font.getDistanceFieldSmoothing() * font.getScaleX();
		}

		private void setSmoothingUniform (IBatch spriteBatch, float smoothing) {
			spriteBatch.flush();
			spriteBatch.getShader().setUniformf("u_smoothing", smoothing);
		}

		public override void draw (IBatch spriteBatch) {
			setSmoothingUniform(spriteBatch, getSmoothingFactor());
			base.draw(spriteBatch);
			setSmoothingUniform(spriteBatch, 0);
		}

		public override void draw (IBatch spriteBatch, int start, int end) {
			setSmoothingUniform(spriteBatch, getSmoothingFactor());
			base.draw(spriteBatch, start, end);
			setSmoothingUniform(spriteBatch, 0);
		}
	}
}
}
