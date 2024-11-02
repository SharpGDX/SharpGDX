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
//public class NinePatchTest : GdxTest {
//	/** A string name for the type of test, and the NinePatch being tested. */
//	private  class TestPatch {
//		public readonly String name;
//		public readonly NinePatch ninePatch;
//
//		internal TestPatch (String n) {
//			this.name = n;
//			this.ninePatch = NinePatchTest.newNinePatch();
//		}
//
//		internal TestPatch (String n, NinePatch np) {
//			this.name = n;
//			this.ninePatch = np;
//		}
//	}
//
//	private OrthographicCamera camera;
//	private SpriteBatch b;
//	private Array<TestPatch> ninePatches = new Array<TestPatch>(10);
//
//	private readonly long start = TimeUtils.currentTimeMillis();
//
//	public override void Create () {
//		TestPatch tp;
//
//		// Create all the NinePatches to test
//		ninePatches.add(new TestPatch("default"));
//
//		tp = new TestPatch("20px width");
//		int bWidth = 20;
//		tp.ninePatch.setLeftWidth(bWidth);
//		tp.ninePatch.setRightWidth(bWidth);
//		tp.ninePatch.setTopHeight(bWidth);
//		tp.ninePatch.setBottomHeight(bWidth);
//		ninePatches.add(tp);
//
//		tp = new TestPatch("fat left");
//		tp.ninePatch.setLeftWidth(3 * tp.ninePatch.getRightWidth());
//		ninePatches.add(tp);
//
//		tp = new TestPatch("fat top");
//		tp.ninePatch.setTopHeight(3 * tp.ninePatch.getBottomHeight());
//		ninePatches.add(tp);
//
//		tp = new TestPatch("degenerate", newDegenerateNinePatch());
//		ninePatches.add(tp);
//
//		tp = new TestPatch("upper-left quad", newULQuadPatch());
//		ninePatches.add(tp);
//
//		tp = new TestPatch("no middle row", newMidlessPatch());
//		ninePatches.add(tp);
//
//		b = new SpriteBatch();
//	}
//
//	// Make a new 'pixmapSize' square texture region with 'patchSize' patches in it. Each patch is a different color.
//	static TextureRegion newPatchPix (int patchSize, int pixmapSize) {
//		int pixmapDim = MathUtils.nextPowerOfTwo(pixmapSize);
//
//		Pixmap p = new Pixmap(pixmapDim, pixmapDim, Pixmap.Format.RGBA8888);
//		p.setColor(1, 1, 1, 0);
//		p.fill();
//
//		for (int x = 0; x < pixmapSize; x += patchSize) {
//			for (int y = 0; y < pixmapSize; y += patchSize) {
//				p.setColor(x / (float)pixmapSize, y / (float)pixmapSize, 1.0f, 1.0f);
//				p.fillRectangle(x, y, patchSize, patchSize);
//			}
//		}
//
//		return new TextureRegion(new Texture(p), pixmapSize, pixmapSize);
//	}
//
//	// Make a degenerate NinePatch
//	static NinePatch newDegenerateNinePatch () {
//		 int patchSize = 8;
//		 int pixmapSize = patchSize * 3;
//		TextureRegion tr = newPatchPix(patchSize, pixmapSize);
//		return new NinePatch(tr);
//	}
//
//	// Make a basic NinePatch with different colors in each of the nine patches
//	static NinePatch newNinePatch () {
//		 int patchSize = 8;
//		 int pixmapSize = patchSize * 3;
//		TextureRegion tr = newPatchPix(patchSize, pixmapSize);
//
//		return new NinePatch(tr, patchSize, patchSize, patchSize, patchSize);
//	}
//
//	// Make a upper-left "quad" patch (only 4 patches defined in the top-left corner of the ninepatch)
//	static NinePatch newULQuadPatch () {
//		 int patchSize = 8;
//		 int pixmapSize = patchSize * 2;
//		TextureRegion tr = newPatchPix(patchSize, pixmapSize);
//
//		return new NinePatch(tr, patchSize, 0, patchSize, 0);
//	}
//
//	// Make a ninepatch with no middle band, just top three and bottom three.
//	static NinePatch newMidlessPatch () {
//		 int patchSize = 8;
//		 int fullPatchHeight = patchSize * 2;
//		 int fullPatchWidth = patchSize * 3;
//		 int pixmapDim = MathUtils.nextPowerOfTwo(Math.Max(fullPatchWidth, fullPatchHeight));
//
//		Pixmap testPatch = new Pixmap(pixmapDim, pixmapDim, Pixmap.Format.RGBA8888);
//		testPatch.setColor(1, 1, 1, 0);
//		testPatch.fill();
//
//		for (int x = 0; x < fullPatchWidth; x += patchSize) {
//			for (int y = 0; y < fullPatchHeight; y += patchSize) {
//				testPatch.setColor(x / (float)fullPatchWidth, y / (float)fullPatchHeight, 1.0f, 1.0f);
//				testPatch.fillRectangle(x, y, patchSize, patchSize);
//			}
//		}
//
//		return new NinePatch(new TextureRegion(new Texture(testPatch), fullPatchWidth, fullPatchHeight), patchSize, patchSize,
//			patchSize, patchSize);
//	}
//
//	private float timePassed = 0;
//	private readonly Color filterColor = new Color();
//	private readonly Color oldColor = new Color();
//
//	public override void Render () {
//		int screenWidth = Gdx.graphics.getWidth();
//		 int screenHeight = Gdx.graphics.getHeight();
//
//		ScreenUtils.clear(0, 0, 0, 0);
//
//		timePassed += Gdx.graphics.getDeltaTime();
//
//		b.begin();
//		 int sz = ninePatches.size;
//		 int XGAP = 10;
//		 int pheight = (int)((screenHeight * 0.5f) / ((sz + 1) / 2));
//		int x = XGAP;
//		int y = 10;
//
//		// Test that batch color is applied to NinePatch
//		if (timePassed < 2) {
//			b.setColor(1, 1, 1, Interpolation.sine.apply(timePassed / 2f));
//		}
//
//		// Test that the various nine patches render
//		for (int i = 0; i < sz; i += 2) {
//			int pwidth = (int)(0.44f * screenWidth);
//
//			 NinePatch np1 = ninePatches.get(i).ninePatch;
//			np1.draw(b, x, y, pwidth, pheight);
//
//			if (i + 1 < sz) {
//				 NinePatch np2 = ninePatches.get(i + 1).ninePatch;
//				 int x2 = x + pwidth + XGAP;
//				 int pwidth2 = screenWidth - XGAP - x2;
//
//				np2.draw(b, x2, y, pwidth2, pheight);
//			}
//
//			y += pheight + 2;
//		}
//
//		// Dim a np by setting its color. Also test sending same np to batch twice
//		NinePatch np = ninePatches.get(0).ninePatch;
//		oldColor.set(np.getColor());
//		filterColor.set(0.3f, 0.3f, 0.3f, 1.0f);
//		np.setColor(filterColor);
//		np.draw(b, x, y, 100, 30);
//		np.setColor(oldColor);
//
//		b.end();
//	}
//
//	public override void Resize (int width, int height) {
//		float ratio = ((float)Gdx.graphics.getWidth() / (float)Gdx.graphics.getHeight());
//		int h = 10;
//		int w = (int)(h * ratio);
//		camera = new OrthographicCamera(w, h);
//	}
//}
