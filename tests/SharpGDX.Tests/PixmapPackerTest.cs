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
//public class PixmapPackerTest : GdxTest {
//
//	OrthographicCamera camera;
//	SpriteBatch batch;
//	ShapeRenderer shapeRenderer;
//
//	TextureAtlas atlas;
//
//	int pageToShow = 0;
//	Array<TextureRegion> textureRegions;
//
//	NinePatch ninePatch;
//	NinePatch officialPatch;
//
//	Animation<TextureRegion> animation;
//
//	Skin skin;
//
//	float stateTime = 0;
//
//	public override void Create () {
//		batch = new SpriteBatch();
//		shapeRenderer = new ShapeRenderer();
//
//		camera = new OrthographicCamera(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		camera.position.set(Gdx.graphics.getWidth() / 2, Gdx.graphics.getHeight() / 2, 0);
//		camera.update();
//
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//
//		Pixmap pixmap1 = new Pixmap(Gdx.files.@internal("data/badlogic.jpg"));
//		Pixmap pixmap2 = new Pixmap(Gdx.files.@internal("data/particle-fire.png"));
//		Pixmap pixmap3 = new Pixmap(Gdx.files.@internal("data/isotile.png"));
//		Pixmap pixmap4 = new Pixmap(Gdx.files.@internal("data/textfield.9.png"));
//		Pixmap pixmap5 = new Pixmap(Gdx.files.@internal("data/badlogic-with-whitespace.png"));
//
//		PixmapPacker packer = new PixmapPacker(1024, 1024, Pixmap.Format.RGBA8888, 8, false, true, true,
//			new PixmapPacker.GuillotineStrategy());
//		packer.setTransparentColor(Color.PINK);
//		for (int count = 1; count <= 3; ++count) {
//			packer.pack("badlogic " + count, pixmap1);
//			packer.pack("fire " + count, pixmap2);
//			packer.pack("isotile " + count, pixmap3);
//			packer.pack("textfield-" + count + ".9", pixmap4);
//			packer.pack("badlogic-whitespace " + count, pixmap5);
//		}
//
//		atlas = packer.generateTextureAtlas(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest, false);
//		Gdx.app.log("PixmapPackerTest", "Number of initial textures: " + atlas.getTextures().size);
//
//		packer.setPackToTexture(true);
//
//		for (int count = 4; count <= 10; ++count) {
//			packer.pack("badlogic " + count, pixmap1);
//			packer.pack("fire " + count, pixmap2);
//			packer.pack("isotile " + count, pixmap3);
//			packer.pack("textfield-" + count + ".9", pixmap4);
//			packer.pack("badlogic-whitespace -" + count, pixmap5);
//		}
//
//		packer.pack("badlogic-anim_0", pixmap1);
//		packer.pack("badlogic-anim_1", pixmap5);
//		packer.pack("badlogic-anim_2", pixmap1);
//		packer.pack("badlogic-anim_3", pixmap5);
//
//		pixmap1.Dispose();
//		pixmap2.Dispose();
//		pixmap3.Dispose();
//		pixmap4.Dispose();
//		pixmap5.Dispose();
//
//		packer.updateTextureAtlas(atlas, Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest, false);
//		animation = new Animation<TextureRegion>(0.33f, atlas.findRegions("badlogic-anim"), Animation.PlayMode.LOOP);
//
//		textureRegions = new Array<TextureRegion>();
//		packer.updateTextureRegions(textureRegions, Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest, false);
//		Gdx.app.log("PixmapPackerTest", "Number of updated textures: " + atlas.getTextures().size);
//		Gdx.input.setInputProcessor(new InputAdapter() {
//			@Override
//			public boolean keyDown (int keycode) {
//				if (keycode >= Input.Keys.NUM_0 && keycode <= Input.Keys.NUM_9) {
//					int number = keycode - Input.Keys.NUM_0;
//					if (number < textureRegions.size) {
//						pageToShow = number;
//					}
//				}
//				return base.KeyDown(keycode);
//			}
//		});
//
//		ninePatch = atlas.createPatch("textfield-1");
//		officialPatch = skin.getPatch("textfield");
//		officialPatch.getTexture().setFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
//
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//		int size = Math.Min(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		int quarterSize = (int)(size / 4f);
//		batch.begin();
//		batch.draw(textureRegions.get(pageToShow), 0, 0, size, size);
//		ninePatch.draw(batch, 10, 10, quarterSize, quarterSize);
//		officialPatch.draw(batch, (int)(size * 0.25f + 20), 10, quarterSize, quarterSize);
//		batch.draw(animation.getKeyFrame(stateTime), 30 + (quarterSize * 2), 10, quarterSize, quarterSize);
//		batch.end();
//		shapeRenderer.begin(ShapeRenderer.ShapeType.Line);
//		shapeRenderer.setColor(Color.GREEN);
//		shapeRenderer.rect(0, 0, size, size);
//		shapeRenderer.end();
//
//		stateTime += Gdx.graphics.getDeltaTime();
//	}
//
//	public override void Dispose () {
//		atlas.Dispose();
//		skin.Dispose();
//	}
//}
