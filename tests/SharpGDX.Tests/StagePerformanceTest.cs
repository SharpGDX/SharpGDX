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
//public class StagePerformanceTest : GdxTest {
//
//	Texture texture;
//	TextureRegion[] regions;
//	Stage stage;
//	SpriteBatch batch;
//	BitmapFont font;
//	Sprite[] sprites;
//	bool useStage = true;
//
//	public override void Create () {
//		batch = new SpriteBatch();
//		font = new BitmapFont();
//		stage = new Stage(new ScalingViewport(Scaling.fit, 24, 12));
//		regions = new TextureRegion[8 * 8];
//		sprites = new Sprite[24 * 12];
//
//		texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"));
//		for (int y = 0; y < 8; y++) {
//			for (int x = 0; x < 8; x++) {
//				regions[x + y * 8] = new TextureRegion(texture, x * 32, y * 32, 32, 32);
//			}
//		}
//
//		Random rand = new Random();
//		for (int y = 0, i = 0; y < 12; y++) {
//			for (int x = 0; x < 24; x++) {
//				Image img = new Image(regions[rand.nextInt(8 * 8)]);
//				img.setBounds(x, y, 1, 1);
//				stage.addActor(img);
//				sprites[i] = new Sprite(regions[rand.nextInt(8 * 8)]);
//				sprites[i].setPosition(x, y);
//				sprites[i].setSize(1, 1);
//				i++;
//			}
//		}
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		if (useStage) {
//			stage.act(Gdx.graphics.getDeltaTime());
//			stage.getBatch().disableBlending();
//			Group root = stage.getRoot();
//			Array<Actor> actors = root.getChildren();
//// for(int i = 0; i < actors.size(); i++) {
//// actors.get(i).rotation += 45 * Gdx.graphics.getDeltaTime();
//// }
//			stage.draw();
//		} else {
//			batch.getProjectionMatrix().setToOrtho2D(0, 0, 24, 12);
//			batch.getTransformMatrix().idt();
//			batch.disableBlending();
//			batch.begin();
//			for (int i = 0; i < sprites.Length; i++) {
//// sprites[i].rotate(45 * Gdx.graphics.getDeltaTime());
//				sprites[i].draw(batch);
//			}
//			batch.end();
//		}
//
//		batch.getProjectionMatrix().setToOrtho2D(0, 0, 480, 320);
//		batch.enableBlending();
//		batch.begin();
//		font.setColor(0, 0, 1, 1);
//		font.getData().setScale(2);
//		font.draw(batch, "fps: " + Gdx.graphics.getFramesPerSecond() + (useStage ? ", stage" : "sprite"), 10, 40);
//		batch.end();
//
//		if (Gdx.input.justTouched()) {
//			useStage = !useStage;
//		}
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		batch.Dispose();
//		font.Dispose();
//		texture.Dispose();
//	}
//}
