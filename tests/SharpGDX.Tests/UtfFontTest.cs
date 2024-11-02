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
//
//namespace SharpGDX.Tests;
//
///** See <a href="https://github.com/libgdx/libgdx/issues/1315">#1315</a>
// * @author badlogic */
//public class UtfFontTest : GdxTest {
//	BitmapFont font;
//	SpriteBatch batch;
//
//	public override void Create () {
//		batch = new SpriteBatch();
//		font = new BitmapFont(Gdx.files.@internal("data/utf-font.fnt"));
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//
//		// https://github.com/libgdx/libgdx/pull/6501#issuecomment-821749417
//		font.draw(batch, "\u0089\u0065\u0089\u0074", 20, 400);// Missing chars should print 'et'
//
//		font.draw(batch, "ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØŒŠþÙÚÛÜÝŸ", 20, 300);
//		font.draw(batch, "àáâãäåæçèéêëìíîïðñòóôõöøœšÞùúûüýÿ", 20, 200);
//		font.draw(batch, "⌘¢ß¥£™©®ª×÷±²³¼½¾µ¿¶·¸º°¯§…¤¦≠¬ˆ¨‰", 20, 100);
//		batch.end();
//	}
//}
