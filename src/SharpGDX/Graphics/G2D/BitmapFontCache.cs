using SharpGDX.Shims;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Graphics.G2D
{
	/** Caches glyph geometry for a BitmapFont, providing a fast way to render static text. This saves needing to compute the glyph
 * geometry each frame.
 * @author Nathan Sweet
 * @author davebaol
 * @author Alexander Dorokhov */
	public class BitmapFontCache
	{
		static private readonly Color TempColor = new Color(1, 1, 1, 1);

		private readonly BitmapFont _font;
		private bool _integer;
		private readonly Array<GlyphLayout> _layouts = new();
		private readonly Array<GlyphLayout> _pooledLayouts = new();
		private int _glyphCount;
		private float _x, _y;
		private readonly Color _color = new Color(1, 1, 1, 1);
		private float _currentTint;

		/** Vertex data per page. */
		private float[][] _pageVertices;

		/** Number of vertex data entries per page. */
		private int[] _idx;

		/** For each page, an array with a value for each glyph from that page, where the value is the index of the character in the
		 * full text being cached. */
		private IntArray[] _pageGlyphIndices;

		/** Used internally to ensure a correct capacity for multi-page font vertex data. */
		private int[] _tempGlyphCount;

		public BitmapFontCache(BitmapFont font)
			: this(font, font.usesIntegerPositions())
		{

		}

		/** @param integer If true, rendering positions will be at integer values to avoid filtering artifacts. */
		public BitmapFontCache(BitmapFont font, bool integer)
		{
			this._font = font;
			this._integer = integer;

			int pageCount = font.regions.size;
			if (pageCount == 0)
				throw new IllegalArgumentException("The specified font must contain at least one texture page.");

			_pageVertices = new float[pageCount][];
			_idx = new int[pageCount];
			if (pageCount > 1)
			{
				// Contains the indices of the glyph in the cache as they are added.
				_pageGlyphIndices = new IntArray[pageCount];
				for (int i = 0, n = _pageGlyphIndices.Length; i < n; i++)
					_pageGlyphIndices[i] = new IntArray();
			}

			_tempGlyphCount = new int[pageCount];
		}

		/** Sets the position of the text, relative to the position when the cached text was created.
		 * @param x The x coordinate
		 * @param y The y coordinate */
		public void setPosition(float x, float y)
		{
			translate(x - this._x, y - this._y);
		}

		/** Sets the position of the text, relative to its current position.
		 * @param xAmount The amount in x to move the text
		 * @param yAmount The amount in y to move the text */
		public void translate(float xAmount, float yAmount)
		{
			if (xAmount == 0 && yAmount == 0) return;
			if (_integer)
			{
				xAmount = (float)Math.Round(xAmount);
				yAmount = (float)Math.Round(yAmount);
			}

			_x += xAmount;
			_y += yAmount;

			float[][] pageVertices = this._pageVertices;
			for (int i = 0, n = pageVertices.Length; i < n; i++)
			{
				float[] vertices = pageVertices[i];
				for (int ii = 0, nn = _idx[i]; ii < nn; ii += 5)
				{
					vertices[ii] += xAmount;
					vertices[ii + 1] += yAmount;
				}
			}
		}

		/** Tints all text currently in the cache. Does not affect subsequently added text. */
		public void tint(Color tint)
		{
			float newTint = tint.toFloatBits();
			if (_currentTint == newTint) return;
			_currentTint = newTint;

			float[][] pageVertices = this._pageVertices;
			Color tempColor = TempColor;
			int[] tempGlyphCount = this._tempGlyphCount;
			Array.Fill(tempGlyphCount, 0);

			for (int i = 0, n = _layouts.size; i < n; i++)
			{
				GlyphLayout layout = _layouts.get(i);
				IntArray colors = layout.colors;
				int colorsIndex = 0, nextColorGlyphIndex = 0, glyphIndex = 0;
				float lastColorFloatBits = 0;
				for (int ii = 0, nn = layout.runs.size; ii < nn; ii++)
				{
					GlyphLayout.GlyphRun run = layout.runs.get(ii);
					Object[] glyphs = run.glyphs.items;
					for (int iii = 0, nnn = run.glyphs.size; iii < nnn; iii++)
					{
						if (glyphIndex++ == nextColorGlyphIndex)
						{
							Color.abgr8888ToColor(tempColor, colors.get(++colorsIndex));
							lastColorFloatBits = tempColor.mul(tint).toFloatBits();
							nextColorGlyphIndex = ++colorsIndex < colors.size ? colors.get(colorsIndex) : -1;
						}

						int page = ((BitmapFont.Glyph)glyphs[iii]).page;
						int offset = tempGlyphCount[page] * 20 + 2;
						tempGlyphCount[page]++;
						float[] vertices = pageVertices[page];
						vertices[offset] = lastColorFloatBits;
						vertices[offset + 5] = lastColorFloatBits;
						vertices[offset + 10] = lastColorFloatBits;
						vertices[offset + 15] = lastColorFloatBits;
					}
				}
			}
		}

		/** Sets the alpha component of all text currently in the cache. Does not affect subsequently added text. */
		public void setAlphas(float alpha)
		{
			int alphaBits = ((int)(254 * alpha)) << 24;
			float prev = 0, newColor = 0;
			for (int j = 0, length = _pageVertices.Length; j < length; j++)
			{
				float[] vertices = _pageVertices[j];
				for (int i = 2, n = _idx[j]; i < n; i += 5)
				{
					float c = vertices[i];
					if (c == prev && i != 2)
					{
						vertices[i] = newColor;
					}
					else
					{
						prev = c;
						int rgba = NumberUtils.floatToIntColor(c);
						rgba = (rgba & 0x00FFFFFF) | alphaBits;
						newColor = NumberUtils.intToFloatColor(rgba);
						vertices[i] = newColor;
					}
				}
			}
		}

		/** Sets the color of all text currently in the cache. Does not affect subsequently added text. */
		public void setColors(float color)
		{
			for (int j = 0, length = _pageVertices.Length; j < length; j++)
			{
				float[] vertices = _pageVertices[j];
				for (int i = 2, n = _idx[j]; i < n; i += 5)
					vertices[i] = color;
			}
		}

		/** Sets the color of all text currently in the cache. Does not affect subsequently added text. */
		public void setColors(Color tint)
		{
			setColors(tint.toFloatBits());
		}

		/** Sets the color of all text currently in the cache. Does not affect subsequently added text. */
		public void setColors(float r, float g, float b, float a)
		{
			int intBits = ((int)(255 * a) << 24) | ((int)(255 * b) << 16) | ((int)(255 * g) << 8) | ((int)(255 * r));
			setColors(NumberUtils.intToFloatColor(intBits));
		}

		/** Sets the color of the specified characters. This may only be called after {@link #setText(CharSequence, float, float)} and
		 * is reset every time setText is called. */
		public void setColors(Color tint, int start, int end)
		{
			setColors(tint.toFloatBits(), start, end);
		}

		/** Sets the color of the specified characters. This may only be called after {@link #setText(CharSequence, float, float)} and
		 * is reset every time setText is called. */
		public void setColors(float color, int start, int end)
		{
			if (_pageVertices.Length == 1)
			{
				// One page.
				float[] vertices = _pageVertices[0];
				for (int i = start * 20 + 2, n = Math.Min(end * 20, _idx[0]); i < n; i += 5)
					vertices[i] = color;
				return;
			}

			int pageCount = _pageVertices.Length;
			for (int i = 0; i < pageCount; i++)
			{
				float[] vertices = _pageVertices[i];
				IntArray glyphIndices = _pageGlyphIndices[i];
				// Loop through the indices and determine whether the glyph is inside begin/end.
				for (int j = 0, n = glyphIndices.size; j < n; j++)
				{
					int glyphIndex = glyphIndices.items[j];

					// Break early if the glyph is out of bounds.
					if (glyphIndex >= end) break;

					// If inside start and end, change its colour.
					if (glyphIndex >= start)
					{
						// && glyphIndex < end
						int offset = j * 20 + 2;
						vertices[offset] = color;
						vertices[offset + 5] = color;
						vertices[offset + 10] = color;
						vertices[offset + 15] = color;
					}
				}
			}
		}

		/** Returns the color used for subsequently added text. Modifying the color affects text subsequently added to the cache, but
		 * does not affect existing text currently in the cache. */
		public Color getColor()
		{
			return _color;
		}

		/** A convenience method for setting the cache color. The color can also be set by modifying {@link #getColor()}. */
		public void setColor(Color color)
		{
			this._color.set(color);
		}

		/** A convenience method for setting the cache color. The color can also be set by modifying {@link #getColor()}. */
		public void setColor(float r, float g, float b, float a)
		{
			_color.set(r, g, b, a);
		}

		public virtual void draw(IBatch spriteBatch)
		{
			Array<TextureRegion> regions = _font.getRegions();
			for (int j = 0, n = _pageVertices.Length; j < n; j++)
			{
				if (_idx[j] > 0)
				{
					// ignore if this texture has no glyphs
					float[] vertices = _pageVertices[j];
					spriteBatch.draw(regions.get(j).getTexture(), vertices, 0, _idx[j]);
				}
			}
		}

		public virtual void draw(IBatch spriteBatch, int start, int end)
		{
			if (_pageVertices.Length == 1)
			{
				// 1 page.
				spriteBatch.draw(_font.getRegion().getTexture(), _pageVertices[0], start * 20, (end - start) * 20);
				return;
			}

			// Determine vertex offset and count to render for each page. Some pages might not need to be rendered at all.
			Array<TextureRegion> regions = _font.getRegions();
			for (int i = 0, pageCount = _pageVertices.Length; i < pageCount; i++)
			{
				int offset = -1, count = 0;

				// For each set of glyph indices, determine where to begin within the start/end bounds.
				IntArray glyphIndices = _pageGlyphIndices[i];
				for (int ii = 0, n = glyphIndices.size; ii < n; ii++)
				{
					int glyphIndex = glyphIndices.get(ii);

					// Break early if the glyph is out of bounds.
					if (glyphIndex >= end) break;

					// Determine if this glyph is within bounds. Use the first match of that for the offset.
					if (offset == -1 && glyphIndex >= start) offset = ii;

					// Determine the vertex count by counting glyphs within bounds.
					if (glyphIndex >= start) count++;
				}

				// Page doesn't need to be rendered.
				if (offset == -1 || count == 0) continue;

				// Render the page vertex data with the offset and count.
				spriteBatch.draw(regions.get(i).getTexture(), _pageVertices[i], offset * 20, count * 20);
			}
		}

		public void draw(IBatch spriteBatch, float alphaModulation)
		{
			if (alphaModulation == 1)
			{
				draw(spriteBatch);
				return;
			}

			Color color = getColor();
			float oldAlpha = color.a;
			color.a *= alphaModulation;
			setColors(color);
			draw(spriteBatch);
			color.a = oldAlpha;
			setColors(color);
		}

		/** Removes all glyphs in the cache. */
		public void clear()
		{
			_x = 0;
			_y = 0;
			Pools.freeAll(_pooledLayouts, true);
			_pooledLayouts.clear();
			_layouts.clear();
			for (int i = 0, n = _idx.Length; i < n; i++)
			{
				if (_pageGlyphIndices != null) _pageGlyphIndices[i].clear();
				_idx[i] = 0;
			}
		}

		private void requireGlyphs(GlyphLayout layout)
		{
			if (_pageVertices.Length == 1)
			{
				// Simple if we just have one page.
				requirePageGlyphs(0, layout.glyphCount);
			}
			else
			{
				int[] tempGlyphCount = this._tempGlyphCount;
				Array.Fill(tempGlyphCount, 0);
				// Determine # of glyphs in each page.
				for (int i = 0, n = layout.runs.size; i < n; i++)
				{
					Array<BitmapFont.Glyph> glyphs = layout.runs.get(i).glyphs;
					Object[] glyphItems = glyphs.items;
					for (int ii = 0, nn = glyphs.size; ii < nn; ii++)
						tempGlyphCount[((BitmapFont.Glyph)glyphItems[ii]).page]++;
				}

				// Require that many for each page.
				for (int i = 0, n = tempGlyphCount.Length; i < n; i++)
					requirePageGlyphs(i, tempGlyphCount[i]);
			}
		}

		private void requirePageGlyphs(int page, int glyphCount)
		{
			if (_pageGlyphIndices != null)
			{
				if (glyphCount > _pageGlyphIndices[page].items.Length)
					_pageGlyphIndices[page].ensureCapacity(glyphCount - _pageGlyphIndices[page].size);
			}

			int vertexCount = _idx[page] + glyphCount * 20;
			float[] vertices = _pageVertices[page];
			if (vertices == null)
			{
				_pageVertices[page] = new float[vertexCount];
			}
			else if (vertices.Length < vertexCount)
			{
				float[] newVertices = new float[vertexCount];
				Array.Copy(vertices, 0, newVertices, 0, _idx[page]);
				_pageVertices[page] = newVertices;
			}
		}

		private void setPageCount(int pageCount)
		{
			float[][] newPageVertices = new float[pageCount][];
			Array.Copy(_pageVertices, 0, newPageVertices, 0, _pageVertices.Length);
			_pageVertices = newPageVertices;

			int[] newIdx = new int[pageCount];
			Array.Copy(_idx, 0, newIdx, 0, _idx.Length);
			_idx = newIdx;

			IntArray[] newPageGlyphIndices = new IntArray[pageCount];
			int pageGlyphIndicesLength = 0;
			if (_pageGlyphIndices != null)
			{
				pageGlyphIndicesLength = _pageGlyphIndices.Length;
				Array.Copy(_pageGlyphIndices, 0, newPageGlyphIndices, 0, _pageGlyphIndices.Length);
			}

			for (int i = pageGlyphIndicesLength; i < pageCount; i++)
				newPageGlyphIndices[i] = new IntArray();
			_pageGlyphIndices = newPageGlyphIndices;

			_tempGlyphCount = new int[pageCount];
		}

		private void addToCache(GlyphLayout layout, float x, float y)
		{
			int runCount = layout.runs.size;
			if (runCount == 0) return;

			// Check if the number of font pages has changed.
			if (_pageVertices.Length < _font.regions.size) setPageCount(_font.regions.size);

			_layouts.add(layout);
			requireGlyphs(layout);

			IntArray colors = layout.colors;
			int colorsIndex = 0, nextColorGlyphIndex = 0, glyphIndex = 0;
			float lastColorFloatBits = 0;
			for (int i = 0; i < runCount; i++)
			{
				GlyphLayout.GlyphRun run = layout.runs.get(i);
				Object[] glyphs = run.glyphs.items;
				float[] xAdvances = run.xAdvances.items;
				float gx = x + run.x, gy = y + run.y;
				for (int ii = 0, nn = run.glyphs.size; ii < nn; ii++)
				{
					if (glyphIndex++ == nextColorGlyphIndex)
					{
						lastColorFloatBits = NumberUtils.intToFloatColor(colors.get(++colorsIndex));
						nextColorGlyphIndex = ++colorsIndex < colors.size ? colors.get(colorsIndex) : -1;
					}

					gx += xAdvances[ii];
					addGlyph((BitmapFont.Glyph)glyphs[ii], gx, gy, lastColorFloatBits);
				}
			}

			_currentTint = Color.WHITE_FLOAT_BITS; // Cached glyphs have changed, reset the current tint.
		}

		private void addGlyph(BitmapFont.Glyph glyph, float x, float y, float color)
		{
			float scaleX = _font.data.scaleX, scaleY = _font.data.scaleY;
			x += glyph.xoffset * scaleX;
			y += glyph.yoffset * scaleY;
			float width = glyph.width * scaleX, height = glyph.height * scaleY;
			float u = glyph.u, u2 = glyph.u2, v = glyph.v, v2 = glyph.v2;

			if (_integer)
			{
				x = (float)Math.Round(x);
				y = (float)Math.Round(y);
				width = (float)Math.Round(width);
				height = (float)Math.Round(height);
			}

			float x2 = x + width, y2 = y + height;

			int page = glyph.page;
			int idx = this._idx[page];
			this._idx[page] += 20;

			if (_pageGlyphIndices != null) _pageGlyphIndices[page].add(_glyphCount++);

			float[] vertices = _pageVertices[page];
			vertices[idx++] = x;
			vertices[idx++] = y;
			vertices[idx++] = color;
			vertices[idx++] = u;
			vertices[idx++] = v;

			vertices[idx++] = x;
			vertices[idx++] = y2;
			vertices[idx++] = color;
			vertices[idx++] = u;
			vertices[idx++] = v2;

			vertices[idx++] = x2;
			vertices[idx++] = y2;
			vertices[idx++] = color;
			vertices[idx++] = u2;
			vertices[idx++] = v2;

			vertices[idx++] = x2;
			vertices[idx++] = y;
			vertices[idx++] = color;
			vertices[idx++] = u2;
			vertices[idx] = v;
		}

		/** Clears any cached glyphs and adds glyphs for the specified text.
		 * @see #addText(CharSequence, float, float, int, int, float, int, boolean, String) */
		public GlyphLayout setText(string str, float x, float y)
		{
			clear();
			return addText(str, x, y, 0, str.Length, 0, Align.left, false);
		}

		/** Clears any cached glyphs and adds glyphs for the specified text.
		 * @see #addText(CharSequence, float, float, int, int, float, int, boolean, String) */
		public GlyphLayout setText(string str, float x, float y, float targetWidth, int halign, bool wrap)
		{
			clear();
			return addText(str, x, y, 0, str.Length, targetWidth, halign, wrap);
		}

		/** Clears any cached glyphs and adds glyphs for the specified text.
		 * @see #addText(CharSequence, float, float, int, int, float, int, boolean, String) */
		public GlyphLayout setText(string str, float x, float y, int start, int end, float targetWidth, int halign,
			bool wrap)
		{
			clear();
			return addText(str, x, y, start, end, targetWidth, halign, wrap);
		}

		/** Clears any cached glyphs and adds glyphs for the specified text.
		 * @see #addText(CharSequence, float, float, int, int, float, int, boolean, String) */
		public GlyphLayout setText(string str, float x, float y, int start, int end, float targetWidth, int halign,
			bool wrap, String truncate)
		{
			clear();
			return addText(str, x, y, start, end, targetWidth, halign, wrap, truncate);
		}

		/** Clears any cached glyphs and adds the specified glyphs.
		 * @see #addText(CharSequence, float, float, int, int, float, int, boolean, String) */
		public void setText(GlyphLayout layout, float x, float y)
		{
			clear();
			addText(layout, x, y);
		}

		/** Adds glyphs for the specified text.
		 * @see #addText(CharSequence, float, float, int, int, float, int, boolean, String) */
		public GlyphLayout addText(string str, float x, float y)
		{
			return addText(str, x, y, 0, str.Length, 0, Align.left, false, null);
		}

		/** Adds glyphs for the specified text.
		 * @see #addText(CharSequence, float, float, int, int, float, int, boolean, String) */
		public GlyphLayout addText(string str, float x, float y, float targetWidth, int halign, bool wrap)
		{
			return addText(str, x, y, 0, str.Length, targetWidth, halign, wrap, null);
		}

		/** Adds glyphs for the specified text.
		 * @see #addText(CharSequence, float, float, int, int, float, int, boolean, String) */
		public GlyphLayout addText(string str, float x, float y, int start, int end, float targetWidth, int halign,
			bool wrap)
		{
			return addText(str, x, y, start, end, targetWidth, halign, wrap, null);
		}

		/** Adds glyphs for the the specified text.
		 * @param x The x position for the left most character.
		 * @param y The y position for the top of most capital letters in the font (the {@link BitmapFontData#capHeight cap height}).
		 * @param start The first character of the string to draw.
		 * @param end The last character of the string to draw (exclusive).
		 * @param targetWidth The width of the area the text will be drawn, for wrapping or truncation.
		 * @param halign Horizontal alignment of the text, see {@link Align}.
		 * @param wrap If true, the text will be wrapped within targetWidth.
		 * @param truncate If not null, the text will be truncated within targetWidth with this string appended. May be an empty
		 *           string.
		 * @return The glyph layout for the cached string (the layout's height is the distance from y to the baseline). */
		public GlyphLayout addText(string str, float x, float y, int start, int end, float targetWidth, int halign,
			bool wrap, String truncate)
		{
			GlyphLayout layout = Pools.obtain<GlyphLayout>(typeof(GlyphLayout));
			_pooledLayouts.add(layout);
			layout.setText(_font, str, start, end, _color, targetWidth, halign, wrap, truncate);
			addText(layout, x, y);
			return layout;
		}

		/** Adds the specified glyphs.
		 * @param layout The cache keeps the layout until cleared or new text is set. The layout should not be modified before then. */
		public void addText(GlyphLayout layout, float x, float y)
		{
			addToCache(layout, x, y + _font.data.ascent);
		}

		/** Returns the x position of the cached string, relative to the position when the string was cached. */
		public float getX()
		{
			return _x;
		}

		/** Returns the y position of the cached string, relative to the position when the string was cached. */
		public float getY()
		{
			return _y;
		}

		public BitmapFont getFont()
		{
			return _font;
		}

		/** Specifies whether to use integer positions or not. Default is to use them so filtering doesn't kick in as badly.
		 * @param use */
		public void setUseIntegerPositions(bool use)
		{
			this._integer = use;
		}

		/** @return whether this font uses integer positions for drawing. */
		public bool usesIntegerPositions()
		{
			return _integer;
		}

		public float[] getVertices()
		{
			return getVertices(0);
		}

		public float[] getVertices(int page)
		{
			return _pageVertices[page];
		}

		public int getVertexCount(int page)
		{
			return _idx[page];
		}

		public Array<GlyphLayout> getLayouts()
		{
			return _layouts;
		}
	}
}