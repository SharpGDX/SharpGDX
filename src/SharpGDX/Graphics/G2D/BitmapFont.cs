using System.Text.RegularExpressions;
using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Graphics.G2D
{
	/** Renders bitmap fonts. The font consists of 2 files: an image file or {@link TextureRegion} containing the glyphs and a file in
 * the AngleCode BMFont text format that describes where each glyph is on the image.
 * <p>
 * Text is drawn using a {@link Batch}. Text can be cached in a {@link BitmapFontCache} for faster rendering of static text, which
 * saves needing to compute the location of each glyph each frame.
 * <p>
 * * The texture for a BitmapFont loaded from a file is managed. {@link #dispose()} must be called to free the texture when no
 * longer needed. A BitmapFont loaded using a {@link TextureRegion} is managed if the region's texture is managed. Disposing the
 * BitmapFont disposes the region's texture, which may not be desirable if the texture is still being used elsewhere.
 * <p>
 * The code was originally based on Matthias Mann's TWL BitmapFont class. Thanks for sharing, Matthias! :)
 * @author Nathan Sweet
 * @author Matthias Mann */
public class BitmapFont : Disposable {
	static private readonly int LOG2_PAGE_SIZE = 9;
	static private readonly int PAGE_SIZE = 1 << LOG2_PAGE_SIZE;
	static private readonly int PAGES = 0x10000 / PAGE_SIZE;

	internal readonly BitmapFontData data;
	internal Array<TextureRegion> regions;
	private readonly BitmapFontCache cache;
	private bool flipped;
	protected bool integer;
	private bool _ownsTexture;

	/** Creates a BitmapFont using the default 15pt Liberation Sans font included in the libgdx JAR file. This is convenient to
	 * easily display text without bothering without generating a bitmap font yourself. */
	public BitmapFont () 
	: this(Gdx.files.classpath("SharpGDX.lsans-15.fnt"), Gdx.files.classpath("SharpGDX.lsans-15.png"),
		false, true)
	{
		
	}

	/** Creates a BitmapFont using the default 15pt Liberation Sans font included in the libgdx JAR file. This is convenient to
	 * easily display text without bothering without generating a bitmap font yourself.
	 * @param flip If true, the glyphs will be flipped for use with a perspective where 0,0 is the upper left corner. */
	public BitmapFont (bool flip) 
	: this(Gdx.files.classpath("SharpGDX.lsans-15.fnt"), Gdx.files.classpath("SharpGDX.lsans-15.png"),
		flip, true)
	{
		
	}

	/** Creates a BitmapFont with the glyphs relative to the specified region. If the region is null, the glyph textures are loaded
	 * from the image file given in the font file. The {@link #dispose()} method will not dispose the region's texture in this
	 * case!
	 * <p>
	 * The font data is not flipped.
	 * @param fontFile the font definition file
	 * @param region The texture region containing the glyphs. The glyphs must be relative to the lower left corner (ie, the region
	 *           should not be flipped). If the region is null the glyph images are loaded from the image path in the font file. */
	public BitmapFont (FileHandle fontFile, TextureRegion region) 
	: this(fontFile, region, false)
	{
		
	}

	/** Creates a BitmapFont with the glyphs relative to the specified region. If the region is null, the glyph textures are loaded
	 * from the image file given in the font file. The {@link #dispose()} method will not dispose the region's texture in this
	 * case!
	 * @param region The texture region containing the glyphs. The glyphs must be relative to the lower left corner (ie, the region
	 *           should not be flipped). If the region is null the glyph images are loaded from the image path in the font file.
	 * @param flip If true, the glyphs will be flipped for use with a perspective where 0,0 is the upper left corner. */
	public BitmapFont (FileHandle fontFile, TextureRegion region, bool flip) 
	: this(new BitmapFontData(fontFile, flip), region, true)
	{
		
	}

	/** Creates a BitmapFont from a BMFont file. The image file name is read from the BMFont file and the image is loaded from the
	 * same directory. The font data is not flipped. */
	public BitmapFont (FileHandle fontFile) 
	: this(fontFile, false)
	{
		
	}

	/** Creates a BitmapFont from a BMFont file. The image file name is read from the BMFont file and the image is loaded from the
	 * same directory.
	 * @param flip If true, the glyphs will be flipped for use with a perspective where 0,0 is the upper left corner. */
	public BitmapFont (FileHandle fontFile, bool flip) 
	: this(new BitmapFontData(fontFile, flip), (TextureRegion)null, true)
	{
		
	}

	/** Creates a BitmapFont from a BMFont file, using the specified image for glyphs. Any image specified in the BMFont file is
	 * ignored.
	 * @param flip If true, the glyphs will be flipped for use with a perspective where 0,0 is the upper left corner. */
	public BitmapFont (FileHandle fontFile, FileHandle imageFile, bool flip) 
	: this(fontFile, imageFile, flip, true)
	{
		
	}

	/** Creates a BitmapFont from a BMFont file, using the specified image for glyphs. Any image specified in the BMFont file is
	 * ignored.
	 * @param flip If true, the glyphs will be flipped for use with a perspective where 0,0 is the upper left corner.
	 * @param integer If true, rendering positions will be at integer values to avoid filtering artifacts. */
	public BitmapFont (FileHandle fontFile, FileHandle imageFile, bool flip, bool integer) 
	: this(new BitmapFontData(fontFile, flip), new TextureRegion(new Texture(imageFile, false)), integer)
	{
		
		_ownsTexture = true;
	}

	/** Constructs a new BitmapFont from the given {@link BitmapFontData} and {@link TextureRegion}. If the TextureRegion is null,
	 * the image path(s) will be read from the BitmapFontData. The dispose() method will not dispose the texture of the region(s)
	 * if the region is != null.
	 * <p>
	 * Passing a single TextureRegion assumes that your font only needs a single texture page. If you need to support multiple
	 * pages, either let the Font read the images themselves (by specifying null as the TextureRegion), or by specifying each page
	 * manually with the TextureRegion[] constructor.
	 * @param integer If true, rendering positions will be at integer values to avoid filtering artifacts. */
	public BitmapFont (BitmapFontData data, TextureRegion region, bool integer) 
	: this(data, region != null ? Array<TextureRegion>.with(new []{ region }) : null, integer)
	{
		
	}

	/** Constructs a new BitmapFont from the given {@link BitmapFontData} and array of {@link TextureRegion}. If the TextureRegion
	 * is null or empty, the image path(s) will be read from the BitmapFontData. The dispose() method will not dispose the texture
	 * of the region(s) if the regions array is != null and not empty.
	 * @param integer If true, rendering positions will be at integer values to avoid filtering artifacts. */
	public BitmapFont (BitmapFontData data, Array<TextureRegion> pageRegions, bool integer) {
		this.flipped = data.flipped;
		this.data = data;
		this.integer = integer;

		if (pageRegions == null || pageRegions.size == 0) {
			if (data.imagePaths == null)
				throw new IllegalArgumentException("If no regions are specified, the font data must have an images path.");

			// Load each path.
			int n = data.imagePaths.Length;
			regions = new (n);
			for (int i = 0; i < n; i++) {
				FileHandle file;
				if (data.fontFile == null)
					file = Gdx.files.@internal(data.imagePaths[i]);
				else
					file = Gdx.files.getFileHandle(data.imagePaths[i], data.fontFile.type());
				regions.add(new TextureRegion(new Texture(file, false)));
			}
			_ownsTexture = true;
		} else {
			regions = pageRegions;
			_ownsTexture = false;
		}

		cache = newFontCache();

		load(data);
	}

	protected virtual void load (BitmapFontData data) {
		foreach (Glyph[] page in data.glyphs) {
			if (page == null) continue;
			foreach (Glyph glyph in page)
				if (glyph != null)
				{
					data.setGlyphRegion(glyph, regions.get(glyph.page));
				}
		}
		if (data.missingGlyph != null) data.setGlyphRegion(data.missingGlyph, regions.get(data.missingGlyph.page));
	}

	/** Draws text at the specified position.
	 * @see BitmapFontCache#addText(CharSequence, float, float) */
	public GlyphLayout draw (IBatch batch, string str, float x, float y) {
		cache.clear();
		GlyphLayout layout = cache.addText(str, x, y);
		cache.draw(batch);
		return layout;
	}

	/** Draws text at the specified position.
	 * @see BitmapFontCache#addText(CharSequence, float, float, int, int, float, int, boolean, String) */
	public GlyphLayout draw (IBatch batch, string str, float x, float y, float targetWidth, int halign, bool wrap) {
		cache.clear();
		GlyphLayout layout = cache.addText(str, x, y, targetWidth, halign, wrap);
		cache.draw(batch);
		return layout;
	}

	/** Draws text at the specified position.
	 * @see BitmapFontCache#addText(CharSequence, float, float, int, int, float, int, boolean, String) */
	public GlyphLayout draw (IBatch batch, string str, float x, float y, int start, int end, float targetWidth, int halign,
		bool wrap) {
		cache.clear();
		GlyphLayout layout = cache.addText(str, x, y, start, end, targetWidth, halign, wrap);
		cache.draw(batch);
		return layout;
	}

	/** Draws text at the specified position.
	 * @see BitmapFontCache#addText(CharSequence, float, float, int, int, float, int, boolean, String) */
	public GlyphLayout draw (IBatch batch, string str, float x, float y, int start, int end, float targetWidth, int halign,
		bool wrap, String truncate) {
		cache.clear();
		GlyphLayout layout = cache.addText(str, x, y, start, end, targetWidth, halign, wrap, truncate);
		cache.draw(batch);
		return layout;
	}

	/** Draws text at the specified position.
	 * @see BitmapFontCache#addText(CharSequence, float, float, int, int, float, int, boolean, String) */
	public void draw (IBatch batch, GlyphLayout layout, float x, float y) {
		cache.clear();
		cache.addText(layout, x, y);
		cache.draw(batch);
	}

	/** Returns the color of text drawn with this font. */
	public Color getColor () {
		return cache.getColor();
	}

	/** A convenience method for setting the font color. The color can also be set by modifying {@link #getColor()}. */
	public void setColor (Color color) {
		cache.getColor().set(color);
	}

	/** A convenience method for setting the font color. The color can also be set by modifying {@link #getColor()}. */
	public void setColor (float r, float g, float b, float a) {
		cache.getColor().set(r, g, b, a);
	}

	public float getScaleX () {
		return data.scaleX;
	}

	public float getScaleY () {
		return data.scaleY;
	}

	/** Returns the first texture region. This is included for backwards compatibility, and for convenience since most fonts only
	 * use one texture page. For multi-page fonts, use {@link #getRegions()}.
	 * @return the first texture region */
	public TextureRegion getRegion () {
		return regions.first();
	}

	/** Returns the array of TextureRegions that represents each texture page of glyphs.
	 * @return the array of texture regions; modifying it may produce undesirable results */
	public Array<TextureRegion> getRegions () {
		return regions;
	}

	/** Returns the texture page at the given index.
	 * @return the texture page at the given index */
	public TextureRegion getRegion (int index) {
		return regions.get(index);
	}

	/** Returns the line height, which is the distance from one line of text to the next. */
	public float getLineHeight () {
		return data.lineHeight;
	}

	/** Returns the x-advance of the space character. */
	public float getSpaceXadvance () {
		return data.spaceXadvance;
	}

	/** Returns the x-height, which is the distance from the top of most lowercase characters to the baseline. */
	public float getXHeight () {
		return data.xHeight;
	}

	/** Returns the cap height, which is the distance from the top of most uppercase characters to the baseline. Since the drawing
	 * position is the cap height of the first line, the cap height can be used to get the location of the baseline. */
	public float getCapHeight () {
		return data.capHeight;
	}

	/** Returns the ascent, which is the distance from the cap height to the top of the tallest glyph. */
	public float getAscent () {
		return data.ascent;
	}

	/** Returns the descent, which is the distance from the bottom of the glyph that extends the lowest to the baseline. This
	 * number is negative. */
	public float getDescent () {
		return data.descent;
	}

	/** Returns true if this BitmapFont has been flipped for use with a y-down coordinate system. */
	public bool isFlipped () {
		return flipped;
	}

	/** Disposes the texture used by this BitmapFont's region IF this BitmapFont created the texture. */
	public void dispose () {
		if (_ownsTexture) {
			for (int i = 0; i < regions.size; i++)
				regions.get(i).getTexture().dispose();
		}
	}

	/** Makes the specified glyphs fixed width. This can be useful to make the numbers in a font fixed width. Eg, when horizontally
	 * centering a score or loading percentage text, it will not jump around as different numbers are shown. */
	public void setFixedWidthGlyphs (string glyphs) {
		BitmapFontData data = this.data;
		int maxAdvance = 0;
		for (int index = 0, end = glyphs.Length; index < end; index++) {
			Glyph g = data.getGlyph(glyphs[index]);
			if (g != null && g.xadvance > maxAdvance) maxAdvance = g.xadvance;
		}
		for (int index = 0, end = glyphs.Length; index < end; index++) {
			Glyph g = data.getGlyph(glyphs[index]);
			if (g == null) continue;
			g.xoffset += (maxAdvance - g.xadvance) / 2;
			g.xadvance = maxAdvance;
			g.kerning = null;
			g.fixedWidth = true;
		}
	}

	/** Specifies whether to use integer positions. Default is to use them so filtering doesn't kick in as badly. */
	public void setUseIntegerPositions (bool integer) {
		this.integer = integer;
		cache.setUseIntegerPositions(integer);
	}

	/** Checks whether this font uses integer positions for drawing. */
	public bool usesIntegerPositions () {
		return integer;
	}

	/** For expert usage -- returns the BitmapFontCache used by this font, for rendering to a sprite batch. This can be used, for
	 * example, to manipulate glyph colors within a specific index.
	 * @return the bitmap font cache used by this font */
	public BitmapFontCache getCache () {
		return cache;
	}

	/** Gets the underlying {@link BitmapFontData} for this BitmapFont. */
	public BitmapFontData getData () {
		return data;
	}

	/** @return whether the texture is owned by the font, font disposes the texture itself if true */
	public bool ownsTexture () {
		return _ownsTexture;
	}

	/** Sets whether the font owns the texture. In case it does, the font will also dispose of the texture when {@link #dispose()}
	 * is called. Use with care!
	 * @param ownsTexture whether the font owns the texture */
	public void setOwnsTexture (bool ownsTexture) {
		this._ownsTexture = ownsTexture;
	}

	/** Creates a new BitmapFontCache for this font. Using this method allows the font to provide the BitmapFontCache
	 * implementation to customize rendering.
	 * <p>
	 * Note this method is called by the BitmapFont constructors. If a subclass overrides this method, it will be called before the
	 * subclass constructors. */
	public virtual BitmapFontCache newFontCache () {
		return new BitmapFontCache(this, integer);
	}

	public override String ToString () {
		return data.name != null ? data.name : base.ToString();
	}

	/** Represents a single character in a font page. */
	public class Glyph {
		public int id;
		public int srcX;
		public int srcY;
		public int width, height;
		public float u, v, u2, v2;
		public int xoffset, yoffset;
		public int xadvance;
		public sbyte[][] kerning;
		public bool fixedWidth;

		/** The index to the texture page that holds this glyph. */
		public int page = 0;

		public int getKerning (char ch) {
			if (kerning != null) {
				sbyte[] page = kerning[ch >>> LOG2_PAGE_SIZE];
				if (page != null) return page[ch & PAGE_SIZE - 1];
			}
			return 0;
		}

		public void setKerning(int ch, int value)
		{
			if (kerning == null) kerning = new sbyte[PAGES][];
			sbyte[] page = kerning[ch >>> LOG2_PAGE_SIZE];
			if (page == null) kerning[ch >>> LOG2_PAGE_SIZE] = page = new sbyte[PAGE_SIZE];
			page[ch & PAGE_SIZE - 1] = (sbyte)value;
		}

		public override String ToString () {
			return ((char)id).ToString();
		}
	}

	static int indexOf (string text, char ch, int start) {
		int n = text.Length;
		for (; start < n; start++)
			if (text[start] == ch) return start;
		return n;
	}

	/** Backing data for a {@link BitmapFont}. */
	 public class BitmapFontData {
		/** The name of the font, or null. */
		public String name;
		/** An array of the image paths, for multiple texture pages. */
		public String[] imagePaths;
		public FileHandle fontFile;
		public bool flipped;
		public float padTop, padRight, padBottom, padLeft;
		/** The distance from one line of text to the next. To set this value, use {@link #setLineHeight(float)}. */
		public float lineHeight;
		/** The distance from the top of most uppercase characters to the baseline. Since the drawing position is the cap height of
		 * the first line, the cap height can be used to get the location of the baseline. */
		public float capHeight = 1;
		/** The distance from the cap height to the top of the tallest glyph. */
		public float ascent;
		/** The distance from the bottom of the glyph that extends the lowest to the baseline. This number is negative. */
		public float descent;
		/** The distance to move down when \n is encountered. */
		public float down;
		/** Multiplier for the line height of blank lines. down * blankLineHeight is used as the distance to move down for a blank
		 * line. */
		public float blankLineScale = 1;
		public float scaleX = 1, scaleY = 1;
		public bool markupEnabled;
		/** The amount to add to the glyph X position when drawing a cursor between glyphs. This field is not set by the BMFont
		 * file, it needs to be set manually depending on how the glyphs are rendered on the backing textures. */
		public float cursorX;

		public readonly Glyph[][] glyphs = new Glyph[PAGES][];
		/** The glyph to display for characters not in the font. May be null. */
		public Glyph missingGlyph;

		/** The width of the space character. */
		public float spaceXadvance;
		/** The x-height, which is the distance from the top of most lowercase characters to the baseline. */
		public float xHeight = 1;

		/** Additional characters besides whitespace where text is wrapped. Eg, a hypen (-). */
		public char[] breakChars;
		public char[] xChars = {'x', 'e', 'a', 'o', 'n', 's', 'r', 'c', 'u', 'm', 'v', 'w', 'z'};
		public char[] capChars = {'M', 'N', 'B', 'D', 'C', 'E', 'F', 'K', 'A', 'G', 'H', 'I', 'J', 'L', 'O', 'P', 'Q', 'R', 'S',
			'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

		/** Creates an empty BitmapFontData for configuration before calling {@link #load(FileHandle, boolean)}, to subclass, or to
		 * populate yourself, e.g. using stb-truetype or FreeType. */
		public BitmapFontData () {
		}

		public BitmapFontData (FileHandle fontFile, bool flip) {
			this.fontFile = fontFile;
			this.flipped = flip;
			load(fontFile, flip);
		}

		public void load (FileHandle fontFile, bool flip)
		{
				if (imagePaths != null) throw new IllegalStateException("Already loaded.");

				name = fontFile.nameWithoutExtension();

				BufferedReader reader = new BufferedReader(new InputStreamReader(fontFile.read()), 512);
				try
				{
					String line = reader.readLine(); // info
					if (line == null) throw new GdxRuntimeException("File is empty.");

					line = line.Substring(line.IndexOf("padding=") + 8);
					String[] padding = line.Substring(0, line.IndexOf(' ')).Split(",", 4);
					if (padding.Length != 4) throw new GdxRuntimeException("Invalid padding.");
					padTop = int.Parse(padding[0]);
					padRight = int.Parse(padding[1]);
					padBottom = int.Parse(padding[2]);
					padLeft = int.Parse(padding[3]);
					float padY = padTop + padBottom;

					line = reader.readLine();
					if (line == null) throw new GdxRuntimeException("Missing common header.");
					String[] common = line.Split(" ", 9); // At most we want the 6th element; i.e. "page=N"

					// At least lineHeight and base are required.
					if (common.Length < 3) throw new GdxRuntimeException("Invalid common header.");

					if (!common[1].StartsWith("lineHeight=")) throw new GdxRuntimeException("Missing: lineHeight");
					lineHeight = int.Parse(common[1].Substring(11));

					if (!common[2].StartsWith("base=")) throw new GdxRuntimeException("Missing: base");
					float baseLine = int.Parse(common[2].Substring(5));

					int pageCount = 1;
					if (common.Length >= 6 && common[5] != null && common[5].StartsWith("pages="))
					{
						try
						{
							pageCount = Math.Max(1, int.Parse(common[5].Substring(6)));
						}
						catch (FormatException ignored)
						{ // Use one page.
						}
					}

					imagePaths = new String[pageCount];

					// Read each page definition.
					for (int p = 0; p < pageCount; p++)
					{
						// Read each "page" info line.
						line = reader.readLine();
						if (line == null) throw new GdxRuntimeException("Missing additional page definitions.");

						// Expect ID to mean "index".
						var matcher = new Regex(".*id=(\\d+)", RegexOptions.Compiled).Match(line);
						if (matcher.Success)
						{
							// TODO: Should probably be 0
							String id = matcher.Groups[1].ToString();
							try
							{
								int pageID = int.Parse(id);
								if (pageID != p) throw new GdxRuntimeException("Page IDs must be indices starting at 0: " + id);
							}
							catch (FormatException ex)
							{
								throw new GdxRuntimeException("Invalid page id: " + id, ex);
							}
						}

						matcher = new Regex(".*file=\"?([^\"]+)\"?", RegexOptions.Compiled).Match(line);
						if (!matcher.Success) throw new GdxRuntimeException("Missing: file");
						String fileName = matcher.Groups[1].ToString();

						imagePaths[p] = fontFile.parent().child(fileName).path().Replace("\\\\", "/");
					}
					descent = 0;

					while (true)
					{
						line = reader.readLine();
						if (line == null) break; // EOF
						if (line.StartsWith("kernings ")) break; // Starting kernings block.
						if (line.StartsWith("metrics ")) break; // Starting metrics block.
						if (!line.StartsWith("char ")) continue;

						Glyph glyph = new Glyph();

						var tokens = Regex.Matches(line, "(?<==)(-?\\d+)");
						//var tokens = line.Split("=");
						//tokens.MoveNext();
						//tokens.MoveNext();
						//tokens.nextToken();
						//tokens.nextToken();
						int ch = int.Parse(tokens[0].Value);
						
						if (ch <= 0)
							missingGlyph = glyph;
						else if (ch <= char.MaxValue)
							setGlyph(ch, glyph);
						else
							continue;
						glyph.id = ch;
						//tokens.nextToken();
						glyph.srcX = int.Parse(tokens[1].Value);
						//tokens.nextToken();
						glyph.srcY = int.Parse(tokens[2].Value);
						//tokens.nextToken();
						glyph.width = int.Parse(tokens[3].Value);
						//tokens.nextToken();
						glyph.height = int.Parse(tokens[4].Value);
						//tokens.nextToken();
						glyph.xoffset = int.Parse(tokens[5].Value);
						//tokens.nextToken();
						if (flip)
							glyph.yoffset = int.Parse(tokens[6].Value);
						else
							glyph.yoffset = -(glyph.height + int.Parse(tokens[6].Value));
						//tokens.nextToken();
						glyph.xadvance = int.Parse(tokens[7].Value);
						
						// Check for page safely, it could be omitted or invalid.
						//if (tokens.hasMoreTokens()) tokens.nextToken();
						//if (tokens.hasMoreTokens())

						if(tokens.Count >= 10)
						{
							try
							{
								// TODO: Verify that 9 is correct
								glyph.page = int.Parse(tokens[9].Value);
							}
							catch (FormatException ignored)
							{
							}
						}

						if (glyph.width > 0 && glyph.height > 0) descent = Math.Min(baseLine + glyph.yoffset, descent);
					}
					descent += padBottom;

					while (true)
					{
						line = reader.readLine();
						if (line == null) break;
						if (!line.StartsWith("kerning ")) break;

						var tokens = Regex.Matches(line, "(?<==)(-?\\d+)");
						//tokens.nextToken();
					//	tokens.nextToken();
						int first = int.Parse(tokens[0].Value);
						//tokens.nextToken();
						int second = int.Parse(tokens[1].Value);
						if (first < 0 || first > char.MaxValue || second < 0 || second > char.MaxValue) continue;
						Glyph glyph = getGlyph((char)first);
						//tokens.nextToken();
						int amount = int.Parse(tokens[2].Value);
						
						if (glyph != null)
						{ // Kernings may exist for glyph pairs not contained in the font.
							glyph.setKerning(second, amount);
						}
					}

					bool hasMetricsOverride = false;
					float overrideAscent = 0;
					float overrideDescent = 0;
					float overrideDown = 0;
					float overrideCapHeight = 0;
					float overrideLineHeight = 0;
					float overrideSpaceXAdvance = 0;
					float overrideXHeight = 0;

					// Metrics override
					if (line != null && line.StartsWith("metrics "))
					{

						hasMetricsOverride = true;

					var tokens = Regex.Matches(line, "(?<==)(-?\\d+)");
					//	tokens.nextToken();
					//	tokens.nextToken();
						overrideAscent = float.Parse(tokens[0].Value);
					//	tokens.nextToken();
						overrideDescent = float.Parse(tokens[1].Value);
					//	tokens.nextToken();
						overrideDown = float.Parse(tokens[2].Value);
					//	tokens.nextToken();
						overrideCapHeight = float.Parse(tokens[3].Value);
					//	tokens.nextToken();
						overrideLineHeight = float.Parse(tokens[4].Value);
					//	tokens.nextToken();
						overrideSpaceXAdvance = float.Parse(tokens[5].Value);
					//	tokens.nextToken();
						overrideXHeight = float.Parse(tokens[6].Value);
					}
					
					Glyph spaceGlyph = getGlyph(' ');
					if (spaceGlyph == null)
					{
						spaceGlyph = new Glyph();
						spaceGlyph.id = ' ';
						Glyph xadvanceGlyph = getGlyph('l');
						if (xadvanceGlyph == null) xadvanceGlyph = getFirstGlyph();
						spaceGlyph.xadvance = xadvanceGlyph.xadvance;
						setGlyph(' ', spaceGlyph);
					}
					if (spaceGlyph.width == 0)
					{
						spaceGlyph.width = (int)(padLeft + spaceGlyph.xadvance + padRight);
						spaceGlyph.xoffset = (int)-padLeft;
					}
					spaceXadvance = spaceGlyph.xadvance;
					
					Glyph xGlyph = null;
					foreach (char xChar in xChars)
					{
						xGlyph = getGlyph(xChar);
						if (xGlyph != null) break;
					}
					if (xGlyph == null) xGlyph = getFirstGlyph();
					xHeight = xGlyph.height - padY;

					Glyph capGlyph = null;
					foreach (char capChar in capChars)
					{
						capGlyph = getGlyph(capChar);
						if (capGlyph != null) break;
					}
					if (capGlyph == null)
					{
						foreach (Glyph[] page in this.glyphs)
						{
							if (page == null) continue;
							foreach (Glyph glyph in page)
							{
								if (glyph == null || glyph.height == 0 || glyph.width == 0) continue;
								capHeight = Math.Max(capHeight, glyph.height);
							}
						}
					}
					else
						capHeight = capGlyph.height;
					capHeight -= padY;

					ascent = baseLine - capHeight;
					down = -lineHeight;
					if (flip)
					{
						ascent = -ascent;
						down = -down;
					}

					if (hasMetricsOverride)
					{
						this.ascent = overrideAscent;
						this.descent = overrideDescent;
						this.down = overrideDown;
						this.capHeight = overrideCapHeight;
						this.lineHeight = overrideLineHeight;
						this.spaceXadvance = overrideSpaceXAdvance;
						this.xHeight = overrideXHeight;
					}

				}
				catch (Exception ex)
				{
					throw new GdxRuntimeException("Error loading font file: " + fontFile, ex);
				}
				finally
				{
					StreamUtils.closeQuietly(reader);
				}
			}

		public void setGlyphRegion (Glyph glyph, TextureRegion region)
		{
				Texture texture = region.getTexture();
				float invTexWidth = 1.0f / texture.getWidth();
				float invTexHeight = 1.0f / texture.getHeight();

				float offsetX = 0, offsetY = 0;
				float u = region.u;
				float v = region.v;
				float regionWidth = region.getRegionWidth();
				float regionHeight = region.getRegionHeight();
				if (region is TextureAtlas.AtlasRegion)
				{
					// Compensate for whitespace stripped from left and top edges.
					TextureAtlas.AtlasRegion atlasRegion = (TextureAtlas.AtlasRegion)region;
					offsetX = atlasRegion.offsetX;
					offsetY = atlasRegion.originalHeight - atlasRegion.packedHeight - atlasRegion.offsetY;
				}

				float x = glyph.srcX;
				float x2 = glyph.srcX + glyph.width;
				float y = glyph.srcY;
				float y2 = glyph.srcY + glyph.height;

				// Shift glyph for left and top edge stripped whitespace. Clip glyph for right and bottom edge stripped whitespace.
				// Note if the font region has padding, whitespace stripping must not be used.
				if (offsetX > 0)
				{
					x -= offsetX;
					if (x < 0)
					{
						glyph.width = (int)(glyph.width+ x);
						glyph.xoffset = (int)(glyph.xoffset - x);
						x = 0;
					}
					x2 -= offsetX;
					if (x2 > regionWidth)
					{
						glyph.width = (int)(glyph.width - x2 - regionWidth);
						x2 = regionWidth;
					}
				}
				if (offsetY > 0)
				{
					y -= offsetY;
					if (y < 0)
					{
						glyph.height =(int)(glyph.height+ y);
						if (glyph.height < 0) glyph.height = 0;
						y = 0;
					}
					y2 -= offsetY;
					if (y2 > regionHeight)
					{
						float amount = y2 - regionHeight;
						glyph.height = (int)(glyph.height - amount);
						glyph.yoffset = (int)(glyph.yoffset + amount);
						y2 = regionHeight;
					}
				}

				glyph.u = u + x * invTexWidth;
				glyph.u2 = u + x2 * invTexWidth;
				if (flipped)
				{
					glyph.v = v + y * invTexHeight;
					glyph.v2 = v + y2 * invTexHeight;
				}
				else
				{
					glyph.v2 = v + y * invTexHeight;
					glyph.v = v + y2 * invTexHeight;
				}
			}

		/** Sets the line height, which is the distance from one line of text to the next. */
		public void setLineHeight (float height) {
			lineHeight = height * scaleY;
			down = flipped ? lineHeight : -lineHeight;
		}

		public void setGlyph (int ch, Glyph glyph) {
			Glyph[] page = glyphs[ch / PAGE_SIZE];
			if (page == null) glyphs[ch / PAGE_SIZE] = page = new Glyph[PAGE_SIZE];
			page[ch & PAGE_SIZE - 1] = glyph;
		}

		public Glyph getFirstGlyph () {
			foreach (Glyph[] page in this.glyphs) {
				if (page == null) continue;
				foreach (Glyph glyph in page) {
					if (glyph == null || glyph.height == 0 || glyph.width == 0) continue;
					return glyph;
				}
			}
			throw new GdxRuntimeException("No glyphs found.");
		}

		/** Returns true if the font has the glyph, or if the font has a {@link #missingGlyph}. */
		public bool hasGlyph (char ch) {
			if (missingGlyph != null) return true;
			return getGlyph(ch) != null;
		}

		/** Returns the glyph for the specified character, or null if no such glyph exists. Note that
		 * {@link #getGlyphs(GlyphRun, CharSequence, int, int, Glyph)} should be be used to shape a string of characters into a list
		 * of glyphs. */
		public Glyph getGlyph (char ch) {
			Glyph[] page = glyphs[ch / PAGE_SIZE];
			if (page != null) return page[ch & PAGE_SIZE - 1];
			return null;
		}

		/** Using the specified string, populates the glyphs and positions of the specified glyph run.
		 * @param str Characters to convert to glyphs. Will not contain newline or color tags. May contain "[[" for an escaped left
		 *           square bracket.
		 * @param lastGlyph The glyph immediately before this run, or null if this is run is the first on a line of text. Used tp
		 *           apply kerning between the specified glyph and the first glyph in this run. */
		public void getGlyphs (GlyphLayout.GlyphRun run, string str, int start, int end, Glyph lastGlyph) {
			int max = end - start;
			if (max == 0) return;
			bool markupEnabled = this.markupEnabled;
			float scaleX = this.scaleX;
			Array<Glyph> glyphs = run.glyphs;
			FloatArray xAdvances = run.xAdvances;

			// Guess at number of glyphs needed.
			glyphs.ensureCapacity(max);
			run.xAdvances.ensureCapacity(max + 1);

			do {
				char ch = str[start++];
					
				if (ch == '\r') continue; // Ignore.
				Glyph glyph = getGlyph(ch);
					
				if (glyph == null) {
					if (missingGlyph == null) continue;
					glyph = missingGlyph;
				}
				glyphs.add(glyph);
				xAdvances.add(lastGlyph == null // First glyph on line, adjust the position so it isn't drawn left of 0.
					? (glyph.fixedWidth ? 0 : -glyph.xoffset * scaleX - padLeft)
					: (lastGlyph.xadvance + lastGlyph.getKerning(ch)) * scaleX);
				lastGlyph = glyph;

					// "[[" is an escaped left square bracket, skip second character.
					if (markupEnabled && ch == '[' && start < end && str[start] == '[') start++;
			} while (start < end);
			if (lastGlyph != null) {
				float lastGlyphWidth = lastGlyph.fixedWidth ? lastGlyph.xadvance * scaleX
					: (lastGlyph.width + lastGlyph.xoffset) * scaleX - padRight;
				xAdvances.add(lastGlyphWidth);
			}
		}

		/** Returns the first valid glyph index to use to wrap to the next line, starting at the specified start index and
		 * (typically) moving toward the beginning of the glyphs array. */
		public int getWrapIndex (Array<Glyph> glyphs, int start) {
			int i = start - 1;
			Object[] glyphsItems = glyphs.items;
			char ch = (char)((Glyph)glyphsItems[i]).id;
			if (isWhitespace(ch)) return i;
			if (isBreakChar(ch)) i--;
			for (; i > 0; i--) {
				ch = (char)((Glyph)glyphsItems[i]).id;
				if (isWhitespace(ch) || isBreakChar(ch)) return i + 1;
			}
			return 0;
		}

		public bool isBreakChar (char c) {
			if (breakChars == null) return false;
			foreach (char br in breakChars)
				if (c == br) return true;
			return false;
		}

		public bool isWhitespace (char c) {
			switch (c) {
			case '\n':
			case '\r':
			case '\t':
			case ' ':
				return true;
			default:
				return false;
			}
		}

		/** Returns the image path for the texture page at the given index (the "id" in the BMFont file). */
		public String getImagePath (int index) {
			return imagePaths[index];
		}

		public String[] getImagePaths () {
			return imagePaths;
		}

		public FileHandle getFontFile () {
			return fontFile;
		}

		/** Scales the font by the specified amounts on both axes
		 * <p>
		 * Note that smoother scaling can be achieved if the texture backing the BitmapFont is using {@link TextureFilter#Linear}.
		 * The default is Nearest, so use a BitmapFont constructor that takes a {@link TextureRegion}.
		 * @throws IllegalArgumentException if scaleX or scaleY is zero. */
		public void setScale (float scaleX, float scaleY) {
			if (scaleX == 0) throw new IllegalArgumentException("scaleX cannot be 0.");
			if (scaleY == 0) throw new IllegalArgumentException("scaleY cannot be 0.");
			float x = scaleX / this.scaleX;
			float y = scaleY / this.scaleY;
			lineHeight *= y;
			spaceXadvance *= x;
			xHeight *= y;
			capHeight *= y;
			ascent *= y;
			descent *= y;
			down *= y;
			padLeft *= x;
			padRight *= x;
			padTop *= y;
			padBottom *= y;
			this.scaleX = scaleX;
			this.scaleY = scaleY;
		}

		/** Scales the font by the specified amount in both directions.
		 * @see #setScale(float, float)
		 * @throws IllegalArgumentException if scaleX or scaleY is zero. */
		public void setScale (float scaleXY) {
			setScale(scaleXY, scaleXY);
		}

		/** Sets the font's scale relative to the current scale.
		 * @see #setScale(float, float)
		 * @throws IllegalArgumentException if the resulting scale is zero. */
		public void scale (float amount) {
			setScale(scaleX + amount, scaleY + amount);
		}

		public override String ToString () {
			return name != null ? name : base.ToString();
		}
	}
}
}
