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
//public class FilterPerformanceTest : GdxTest {
//	SpriteBatch batch;
//	Sprite sprite;
//	Sprite sprite2;
//	TextureAtlas atlas;
//	Texture texture;
//	Matrix4 sceneMatrix;
//	Matrix4 textMatrix;
//	BitmapFont font;
//	int mode = 0;
//	String modeString = "";
//	int[] filters = {GL20.GL_NEAREST, GL20.GL_LINEAR, GL20.GL_NEAREST_MIPMAP_NEAREST, GL20.GL_LINEAR_MIPMAP_NEAREST,
//		GL20.GL_LINEAR_MIPMAP_LINEAR};
//	String[] filterNames = {"nearest", "linear", "nearest mipmap nearest", "linear mipmap nearest", "linear mipmap linear"};
//
//	void setTextureFilter (int filter) {
//		atlas.findRegion("map").getTexture().bind();
//		Gdx.gl.glTexParameterf(GL20.GL_TEXTURE_2D, GL20.GL_TEXTURE_MIN_FILTER, filters[filter]);
//		texture.bind();
//		Gdx.gl.glTexParameterf(GL20.GL_TEXTURE_2D, GL20.GL_TEXTURE_MIN_FILTER, filters[filter]);
//	}
//
//	void setModeString () {
//		modeString = (mode % 2 == 0 ? "Sprite" : "Atlas") + " " + filterNames[mode / 2];
//	}
//
//	public void create () {
//		batch = new SpriteBatch();
//		sceneMatrix = new Matrix4().setToOrtho2D(0, 0, 480, 320);
//		textMatrix = new Matrix4().setToOrtho2D(0, 0, 480, 320);
//
//		atlas = new TextureAtlas(Gdx.files.@internal("data/issue_pack"), Gdx.files.@internal("data/"));
//		texture = new Texture(Gdx.files.@internal("data/resource1.jpg"), true);
//		texture.setFilter(Texture.TextureFilter.MipMap, Texture.TextureFilter.Nearest);
//		setTextureFilter(0);
//		setModeString();
//
//		sprite = atlas.createSprite("map");
//		sprite2 = new Sprite(texture, 0, 0, 855, 480);
//		font = new BitmapFont(Gdx.files.@internal("data/font.fnt"), Gdx.files.@internal("data/font.png"), false);
//
//		Gdx.input.setInputProcessor(new InputAdapter() {
//			public boolean touchDown (int x, int y, int pointer, int newParam) {
//				mode++;
//				if (mode == filters.length * 2) mode = 0;
//				setTextureFilter(mode / 2);
//				setModeString();
//				return false;
//			}
//		});
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		atlas.Dispose();
//		texture.Dispose();
//		font.Dispose();
//	}
//
//	public void render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		batch.setProjectionMatrix(sceneMatrix);
//		batch.begin();
//		renderSprite();
//		batch.end();
//
//		batch.setProjectionMatrix(textMatrix);
//		batch.begin();
//		font.draw(batch, modeString + " fps:" + Gdx.graphics.getFramesPerSecond(), 26, 65);
//		batch.end();
//	}
//
//	public void renderSprite () {
//		batch.disableBlending();
//		if (mode % 2 == 0)
//			sprite2.draw(batch);
//		else
//			sprite.draw(batch);
//		batch.enableBlending();
//	}
//}
