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
///** Demonstrates how to do simple z-sorting of sprites
// * @author mzechner */
//public class SortedSpriteTest : GdxTest {
//	/** Sprite based class that adds a z-coordinate for depth sorting. Note that allt he constructors were auto-generated in
//	 * Eclipse (alt + shift + s, c).
//	 * @author mzechner */
//	public class MySprite : Sprite {
//		public float z;
//
//		public MySprite () 
//        : base()
//        {
//			
//		}
//
//		public MySprite (Sprite sprite) 
//        : base(sprite)
//        {
//            
//		}
//
//		public MySprite (Texture texture, int srcX, int srcY, int srcWidth, int srcHeight) 
//        : base(texture, srcX, srcY, srcWidth, srcHeight)
//        {
//            
//		}
//
//		public MySprite (Texture texture, int srcWidth, int srcHeight) 
//        : base(texture, srcWidth, srcHeight)
//        {
//            
//		}
//
//		public MySprite (Texture texture) 
//        : base(texture)
//        {
//            
//		}
//
//		public MySprite (TextureRegion region, int srcX, int srcY, int srcWidth, int srcHeight) 
//        : base(region, srcX, srcY, srcWidth, srcHeight)
//        {
//            
//		}
//
//		public MySprite (TextureRegion region) 
//        : base(region)
//        {
//            
//		}
//	}
//
//	/** Comparator used for sorting, sorts in ascending order (biggset z to smallest z).
//	 * @author mzechner */
//	public class MySpriteComparator : IComparer<MySprite> {
//		public int Compare (MySprite sprite1, MySprite sprite2) {
//			return (sprite2.z - sprite1.z) > 0 ? 1 : -1;
//		}
//	}
//
//	/** spritebatch used for rendering **/
//	SpriteBatch batch;
//	/** the texture used by the sprites **/
//	Texture texture;
//	/** array of sprites **/
//	Array<MySprite> sprites = new Array<MySprite>();
//	/** a comparator, we keep it around so the GC shuts up **/
//	MySpriteComparator comparator = new MySpriteComparator();
//
//	public override void Create () {
//		// create the SpriteBatch
//		batch = new SpriteBatch();
//
//		// load a texture, usually you dispose of this
//		// eventually.
//		texture = new Texture("data/badlogicsmall.jpg");
//
//		// create 100 sprites, tinted red, from dark to light.
//		// red color component is also used as z-value so we
//		// can see that the sorting works.
//		for (int i = 0; i < 100; i++) {
//			// create the sprite and set a random position
//			MySprite sprite = new MySprite(texture);
//			sprite.setPosition(MathUtils.random() * Gdx.graphics.getWidth(), MathUtils.random() * Gdx.graphics.getHeight());
//
//			// create a random z coordinate in the range 0-1
//			sprite.z = MathUtils.random();
//
//			// set the tinting color to the z coordinate as well
//			// for visual inspection
//			sprite.setColor(sprite.z, 0, 0, 1);
//
//			// add the sprite to the array
//			sprites.add(sprite);
//		}
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//
//		// sort the sprites (not necessary if we know
//		// the are already sorted).
//		sprites.sort(comparator);
//
//		// draw the sprites
//		batch.begin();
//		foreach (MySprite sprite in sprites) {
//			sprite.draw(batch);
//		}
//		batch.end();
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		texture.Dispose();
//	}
//}
