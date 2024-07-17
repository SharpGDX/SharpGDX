using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A text input field with multiple lines. */
public class TextArea : TextField {
	/** Array storing lines breaks positions **/
	IntArray linesBreak;

	/** Last text processed. This attribute is used to avoid unnecessary computations while calculating offsets **/
	private String lastText;

	/** Current line for the cursor **/
	int cursorLine;

	/** Index of the first line showed by the text area **/
	int firstLineShowing;

	/** Number of lines showed by the text area **/
	private int linesShowing;

	/** Variable to maintain the x offset of the cursor when moving up and down. If it's set to -1, the offset is reset **/
	float moveOffset;

	private float prefRows;

	public TextArea (String text, Skin skin) 
	: base(text, skin)
	{
		
	}

	public TextArea (String text, Skin skin, String styleName) 
	: base(text, skin, styleName)
	{
		
	}

	public TextArea (String text, TextFieldStyle style) 
	: base(text, style)
	{
		
	}

	protected override void initialize () {
		base.initialize();
		writeEnters = true;
		linesBreak = new IntArray();
		cursorLine = 0;
		firstLineShowing = 0;
		moveOffset = -1;
		linesShowing = 0;
	}

	protected override int letterUnderCursor (float x) {
		if (linesBreak.size > 0) {
			if (cursorLine * 2 >= linesBreak.size) {
				return text.Length;
			} else {
				float[] glyphPositions = this.glyphPositions.items;
				int start = linesBreak.items[cursorLine * 2];
				x += glyphPositions[start];
				int end = linesBreak.items[cursorLine * 2 + 1];
				int i = start;
				for (; i < end; i++)
					if (glyphPositions[i] > x) break;
				if (i > 0 && glyphPositions[i] - x <= x - glyphPositions[i - 1]) return i;
				return Math.Max(0, i - 1);
			}
		} else {
			return 0;
		}
	}

	public override void setStyle (TextFieldStyle style) {
		// same as super(), just different textHeight. no super() so we don't do same work twice
		if (style == null) throw new IllegalArgumentException("style cannot be null.");
		this.style = style;

		// no extra descent to fake line height
		textHeight = style.font.getCapHeight() - style.font.getDescent();
		if (text != null) updateDisplayText();
		invalidateHierarchy();
	}

	/** Sets the preferred number of rows (lines) for this text area. Used to calculate preferred height */
	public void setPrefRows (float prefRows) {
		this.prefRows = prefRows;
	}

	public override float getPrefHeight () {
		if (prefRows <= 0) {
			return base.getPrefHeight();
		} else {
			// without ceil we might end up with one less row then expected
			// due to how linesShowing is calculated in #sizeChanged and #getHeight() returning rounded value
			float prefHeight = MathUtils.ceil(style.font.getLineHeight() * prefRows);
			if (style.background != null) {
				prefHeight = Math.Max(prefHeight + style.background.getBottomHeight() + style.background.getTopHeight(),
					style.background.getMinHeight());
			}
			return prefHeight;
		}
	}

	/** Returns total number of lines that the text occupies **/
	public int getLines () {
		return linesBreak.size / 2 + (newLineAtEnd() ? 1 : 0);
	}

	/** Returns if there's a new line at then end of the text **/
	public bool newLineAtEnd () {
		return text.Length != 0
			&& (text[text.Length - 1] == NEWLINE || text[text.Length - 1] == CARRIAGE_RETURN);
	}

	/** Moves the cursor to the given number line **/
	public void moveCursorLine (int line) {
		if (line < 0) {
			cursorLine = 0;
			cursor = 0;
			moveOffset = -1;
		} else if (line >= getLines()) {
			int newLine = getLines() - 1;
			cursor = text.Length;
			if (line > getLines() || newLine == cursorLine) {
				moveOffset = -1;
			}
			cursorLine = newLine;
		} else if (line != cursorLine) {
			if (moveOffset < 0) {
				moveOffset = linesBreak.size <= cursorLine * 2 ? 0
					: glyphPositions.get(cursor) - glyphPositions.get(linesBreak.get(cursorLine * 2));
			}
			cursorLine = line;
			cursor = cursorLine * 2 >= linesBreak.size ? text.Length : linesBreak.get(cursorLine * 2);
			while (cursor < text.Length && cursor <= linesBreak.get(cursorLine * 2 + 1) - 1
				&& glyphPositions.get(cursor) - glyphPositions.get(linesBreak.get(cursorLine * 2)) < moveOffset) {
				cursor++;
			}
			showCursor();
		}
	}

	/** Updates the current line, checking the cursor position in the text **/
	void updateCurrentLine () {
		int index = calculateCurrentLineIndex(cursor);
		int line = index / 2;
		// Special case when cursor moves to the beginning of the line from the end of another and a word
		// wider than the box
		if (index % 2 == 0 || index + 1 >= linesBreak.size || cursor != linesBreak.items[index]
			|| linesBreak.items[index + 1] != linesBreak.items[index]) {
			if (line < linesBreak.size / 2 || text.Length == 0 || text[text.Length - 1] == NEWLINE
				|| text[text.Length - 1] == CARRIAGE_RETURN) {
				cursorLine = line;
			}
		}
		updateFirstLineShowing(); // fix for drag-selecting text out of the TextArea's bounds
	}

	/** Scroll the text area to show the line of the cursor **/
	void showCursor () {
		updateCurrentLine();
		updateFirstLineShowing();
	}

	void updateFirstLineShowing () {
		if (cursorLine != firstLineShowing) {
			int step = cursorLine >= firstLineShowing ? 1 : -1;
			while (firstLineShowing > cursorLine || firstLineShowing + linesShowing - 1 < cursorLine) {
				firstLineShowing += step;
			}
		}
	}

	/** Calculates the text area line for the given cursor position **/
	private int calculateCurrentLineIndex (int cursor) {
		int index = 0;
		while (index < linesBreak.size && cursor > linesBreak.items[index]) {
			index++;
		}
		return index;
	}

	// OVERRIDE from TextField

	protected override void sizeChanged () {
		lastText = null; // Cause calculateOffsets to recalculate the line breaks.

		// The number of lines showed must be updated whenever the height is updated
		BitmapFont font = style.font;
		IDrawable background = style.background;
		float availableHeight = getHeight() - (background == null ? 0 : background.getBottomHeight() + background.getTopHeight());
		linesShowing = (int)Math.Floor(availableHeight / font.getLineHeight());
	}

	protected override float getTextY (BitmapFont font, IDrawable? background) {
		float textY = getHeight();
		if (background != null) {
			textY = textY - background.getTopHeight();
		}
		if (font.usesIntegerPositions()) textY = (int)textY;
		return textY;
	}

	protected override void drawSelection (IDrawable selection, IBatch batch, BitmapFont font, float x, float y) {
		int i = firstLineShowing * 2;
		float offsetY = 0;
		int minIndex = Math.Min(cursor, selectionStart);
		int maxIndex = Math.Max(cursor, selectionStart);
		BitmapFont.BitmapFontData fontData = font.getData();
		float lineHeight = style.font.getLineHeight();
		while (i + 1 < linesBreak.size && i < (firstLineShowing + linesShowing) * 2) {

			int lineStart = linesBreak.get(i);
			int lineEnd = linesBreak.get(i + 1);

			if (!((minIndex < lineStart && minIndex < lineEnd && maxIndex < lineStart && maxIndex < lineEnd)
				|| (minIndex > lineStart && minIndex > lineEnd && maxIndex > lineStart && maxIndex > lineEnd))) {

				int start = Math.Max(lineStart, minIndex);
				int end = Math.Min(lineEnd, maxIndex);

				float fontLineOffsetX = 0;
				float fontLineOffsetWidth = 0;
				// we can't use fontOffset as it is valid only for first glyph/line in the text
				// we will grab first character in this line and calculate proper offset for this line
				BitmapFont.Glyph lineFirst = fontData.getGlyph(displayText[lineStart]);
				if (lineFirst != null) {
					// see BitmapFontData.getGlyphs()#852 for offset calculation
					// if selection starts when line starts we want to offset width instead of moving the start as it looks better
					if (start == lineStart) {
						fontLineOffsetWidth = lineFirst.fixedWidth ? 0 : -lineFirst.xoffset * fontData.scaleX - fontData.padLeft;
					} else {
						fontLineOffsetX = lineFirst.fixedWidth ? 0 : -lineFirst.xoffset * fontData.scaleX - fontData.padLeft;
					}
				}
				float selectionX = glyphPositions.get(start) - glyphPositions.get(lineStart);
				float selectionWidth = glyphPositions.get(end) - glyphPositions.get(start);
				selection.draw(batch, x + selectionX + fontLineOffsetX, y - lineHeight - offsetY,
					selectionWidth + fontLineOffsetWidth, font.getLineHeight());
			}

			offsetY += font.getLineHeight();
			i += 2;
		}
	}

	protected override void drawText (IBatch batch, BitmapFont font, float x, float y) {
		float offsetY = -(style.font.getLineHeight() - textHeight) / 2;
		for (int i = firstLineShowing * 2; i < (firstLineShowing + linesShowing) * 2 && i < linesBreak.size; i += 2) {
			font.draw(batch, displayText, x, y + offsetY, linesBreak.items[i], linesBreak.items[i + 1], 0, Align.left, false);
			offsetY -= font.getLineHeight();
		}
	}

	protected override void drawCursor (IDrawable cursorPatch, IBatch batch, BitmapFont font, float x, float y) {
		cursorPatch.draw(batch, x + getCursorX(), y + getCursorY(), cursorPatch.getMinWidth(), font.getLineHeight());
	}

	protected override void calculateOffsets () {
		base.calculateOffsets();
		if (!this.text.Equals(lastText)) {
			this.lastText = text;
			BitmapFont font = style.font;
			float maxWidthLine = this.getWidth()
				- (style.background != null ? style.background.getLeftWidth() + style.background.getRightWidth() : 0);
			linesBreak.clear();
			int lineStart = 0;
			int lastSpace = 0;
			char lastCharacter;
			Pool<GlyphLayout> layoutPool = Pools.get<GlyphLayout>(typeof(GlyphLayout));
			GlyphLayout layout = layoutPool.obtain();
			for (int i = 0; i < text.Length; i++) {
				lastCharacter = text[i];
				if (lastCharacter == CARRIAGE_RETURN || lastCharacter == NEWLINE) {
					linesBreak.add(lineStart);
					linesBreak.add(i);
					lineStart = i + 1;
				} else {
					lastSpace = (continueCursor(i, 0) ? lastSpace : i);
					// TODO: Verify, was originally layout.setText(font, text.SubSequence(lineStart, i + 1));. -RP
					layout.setText(font, text.Substring(lineStart, i + 1));
					if (layout.width > maxWidthLine) {
						if (lineStart >= lastSpace) {
							lastSpace = i - 1;
						}
						linesBreak.add(lineStart);
						linesBreak.add(lastSpace + 1);
						lineStart = lastSpace + 1;
						lastSpace = lineStart;
					}
				}
			}
			layoutPool.free(layout);
			// Add last line
			if (lineStart < text.Length) {
				linesBreak.add(lineStart);
				linesBreak.add(text.Length);
			}
			showCursor();
		}
	}

	protected override InputListener createInputListener () {
		return new TextAreaListener(this);
	}

	public override void setSelection(int selectionStart, int selectionEnd) {
		base.setSelection(selectionStart, selectionEnd);
		updateCurrentLine();
	}

	protected override void moveCursor (bool forward, bool jump) {
		int count = forward ? 1 : -1;
		int index = (cursorLine * 2) + count;
		if (index >= 0 && index + 1 < linesBreak.size && linesBreak.items[index] == cursor
			&& linesBreak.items[index + 1] == cursor) {
			cursorLine += count;
			if (jump) {
				base.moveCursor(forward, jump);
			}
			showCursor();
		} else {
			base.moveCursor(forward, jump);
		}
		updateCurrentLine();

	}

	protected override bool continueCursor (int index, int offset) {
		int pos = calculateCurrentLineIndex(index + offset);
		return base.continueCursor(index, offset) && (pos < 0 || pos >= linesBreak.size - 2 || (linesBreak.items[pos + 1] != index)
			|| (linesBreak.items[pos + 1] == linesBreak.items[pos + 2]));
	}

	public int getCursorLine () {
		return cursorLine;
	}

	public int getFirstLineShowing () {
		return firstLineShowing;
	}

	public int getLinesShowing () {
		return linesShowing;
	}

	public float getCursorX () {
		float textOffset = 0;
		BitmapFont.BitmapFontData fontData = style.font.getData();
		if (!(cursor >= glyphPositions.size || cursorLine * 2 >= linesBreak.size)) {
			int lineStart = linesBreak.items[cursorLine * 2];
			float glyphOffset = 0;
			BitmapFont.Glyph lineFirst = fontData.getGlyph(displayText[lineStart]);
			if (lineFirst != null) {
				// BitmapFontData.getGlyphs()#852
				glyphOffset = lineFirst.fixedWidth ? 0 : -lineFirst.xoffset * fontData.scaleX - fontData.padLeft;
			}
			textOffset = glyphPositions.get(cursor) - glyphPositions.get(lineStart) + glyphOffset;
		}
		return textOffset + fontData.cursorX;
	}

	public float getCursorY () {
		BitmapFont font = style.font;
		return -(cursorLine - firstLineShowing + 1) * font.getLineHeight();
	}

	/** Input listener for the text area **/
	public class TextAreaListener : TextFieldClickListener {
		private readonly TextArea _textArea;

		public TextAreaListener(TextArea textArea)
		:base(textArea)
		{
			_textArea = textArea;
		}

		protected override void setCursorPosition (float x, float y) {
			_textArea.moveOffset = -1;

			IDrawable background = _textArea.style.background;
			BitmapFont font = _textArea.style.font;

			float height = _textArea.getHeight();

			if (background != null) {
				height -= background.getTopHeight();
				x -= background.getLeftWidth();
			}
			x = Math.Max(0, x);
			if (background != null) {
				y -= background.getTopHeight();
			}

			_textArea.cursorLine = (int)Math.Floor((height - y) / font.getLineHeight()) + _textArea.firstLineShowing;
			_textArea.cursorLine = Math.Max(0, Math.Min(_textArea.cursorLine, _textArea.getLines() - 1));

			base.setCursorPosition(x, y);
			_textArea.updateCurrentLine();
		}

		public override bool keyDown (InputEvent @event, int keycode) {
			bool result = base.keyDown(@event, keycode);
			if (_textArea.hasKeyboardFocus()) {
				bool repeat = false;
				bool shift = Gdx.input.isKeyPressed(IInput.Keys.SHIFT_LEFT) || Gdx.input.isKeyPressed(IInput.Keys.SHIFT_RIGHT);
				if (keycode == IInput.Keys.DOWN) {
					if (shift) {
						if (!_textArea.hasSelection) {
							_textArea.selectionStart = _textArea.cursor;
							_textArea.hasSelection = true;
						}
					} else {
						_textArea.clearSelection();
					}
					_textArea.moveCursorLine(_textArea.cursorLine + 1);
					repeat = true;

				} else if (keycode == IInput.Keys.UP) {
					if (shift) {
						if (!_textArea.hasSelection) {
							_textArea.selectionStart = _textArea.cursor;
							_textArea.hasSelection = true;
						}
					} else {
						_textArea.clearSelection();
					}
					_textArea.moveCursorLine(_textArea.cursorLine - 1);
					repeat = true;

				} else {
					_textArea.moveOffset = -1;
				}
				if (repeat) {
					scheduleKeyRepeatTask(keycode);
				}
				_textArea.showCursor();
				return true;
			}
			return result;
		}

		protected override bool checkFocusTraversal (char character) {
			return _textArea.focusTraversal && character == TAB;
		}

		public override bool keyTyped (InputEvent @event, char character) {
			bool result = base.keyTyped(@event, character);
			_textArea.showCursor();
			return result;
		}

		protected override void goHome (bool jump) {
			if (jump) {
				_textArea.cursor = 0;
			} else if (_textArea.cursorLine * 2 < _textArea.linesBreak.size) {
				_textArea.cursor = _textArea.linesBreak.get(_textArea.cursorLine * 2);
			}
		}

		protected override void goEnd (bool jump) {
			if (jump || _textArea.cursorLine >= _textArea.getLines()) {
				_textArea.cursor = _textArea.text.Length;
			} else if (_textArea.cursorLine * 2 + 1 < _textArea.linesBreak.size) {
				_textArea.cursor = _textArea.linesBreak.get(_textArea.cursorLine * 2 + 1);
			}
		}
	}
}
