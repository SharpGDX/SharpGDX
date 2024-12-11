//using SharpGDX.Tests.Utils;
//using SharpGDX.Utils;
//using SharpGDX.Scenes.Scene2D;
//using SharpGDX.Scenes.Scene2D.Utils;
//using SharpGDX.Scenes.Scene2D.UI;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Utils.Viewports;
//using SharpGDX.Shims;
//using SharpGDX.Mathematics;
//using SharpGDX.Graphics.GLUtils;
//
//namespace SharpGDX.Tests;
//
///** Shows how to pull up the softkeyboard and process input from it.
// * @author mzechner */
//public class SoftKeyboardTest : GdxTest {
//	SpriteBatch batch;
//	BitmapFont font;
//	SimpleCharSequence textBuffer;
//
//	public override void Create () {
//		// we want to render the input, so we need
//		// a sprite batch and a font
//		batch = new SpriteBatch();
//		font = new BitmapFont();
//		textBuffer = new SimpleCharSequence();
//
//		// we register an InputAdapter to listen for the keyboard
//		// input. The on-screen keyboard might only generate
//		// "key typed" events, depending on the backend.
//		Gdx.input.setInputProcessor(new InputAdapter() {
//			@Override
//			public boolean keyTyped (char character) {
//				// convert \r to \n
//				if (character == '\r') character = '\n';
//
//				// if we get \b, we remove the last inserted character
//				if (character == '\b' && textBuffer.length() > 0) {
//					textBuffer.delete();
//				}
//
//				// else we just insert the character
//				textBuffer.add(character);
//				return true;
//			}
//		});
//	}
//
//	public override void Render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		font.draw(batch, textBuffer, 0, Gdx.graphics.getHeight() - 20);
//		batch.end();
//
//		// bring up the keyboard if we touch the screen
//		if (Gdx.input.justTouched()) {
//			Gdx.input.setOnscreenKeyboardVisible(true);
//			textBuffer = new SimpleCharSequence();
//		}
//	}
//
//	/** Let's create a very simple {@link CharSequence} implementation that can handle common text input operations.
//	 * @author mzechner */
//	public class SimpleCharSequence : CharSequence {
//		CharArray chars = new CharArray();
//		int cursor = -1;
//
//		public void add (char c) {
//			cursor++;
//			if (cursor == -1)
//				chars.add(c);
//			else
//				chars.insert(cursor, c);
//		}
//
//		public void delete () {
//			if (chars.size == 0) return;
//			chars.removeIndex(cursor - 1);
//			cursor--;
//		}
//
//		@Override
//		public char charAt (int index) {
//			return chars.get(index);
//		}
//
//		@Override
//		public int length () {
//			return chars.size;
//		}
//
//		@Override
//		public CharSequence subSequence (int arg0, int arg1) {
//			return null;
//		}
//	}
//}
