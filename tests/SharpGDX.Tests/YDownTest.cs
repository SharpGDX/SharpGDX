//using SharpGDX.Tests.Utils;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Scenes.Scene2D;
//
//namespace SharpGDX.Tests;
//
///** A simple example of how to use a y-down coordinate system.
// * @author mzechner */
//public class YDownTest : GdxTest {
//	SpriteBatch batch;
//	BitmapFont font;
//	TextureRegion region;
//	Sprite sprite;
//	TextureAtlas atlas;
//	Stage stage;
//	MyActor image;
//	OrthographicCamera camera;
//
//	public override void Create () {
//		// a bitmap font to draw some text, note that we
//		// pass true to the constructor, which flips glyphs on y
//		font = new BitmapFont(Gdx.files.@internal("data/lsans-15.fnt"), true);
//
//		// a texture region, note the flipping on y again
//		region = new TextureRegion(new Texture("data/badlogic.jpg"));
//		region.flip(false, true);
//
//		// a texture atlas, note the boolean
//		atlas = new TextureAtlas(Gdx.files.@internal("data/pack.atlas"), true);
//
//		// a sprite, created from a region in the atlas
//		sprite = atlas.createSprite("badlogicsmall");
//		sprite.setPosition(0, 0);
//
//		// a sprite batch with which we want to render
//		batch = new SpriteBatch();
//
//		// a camera, note the setToOrtho call, which will set the y-axis
//		// to point downwards
//		camera = new OrthographicCamera();
//		camera.setToOrtho(true);
//
//		// a stage which uses our y-down camera and a simple actor (see MyActor below),
//		// which uses the flipped region. The key here is to
//		// set our y-down camera on the stage, the rest is just for demo purposes.
//		stage = new Stage();
//		stage.getViewport().setCamera(camera);
//		image = new MyActor(region);
//		image.setPosition(100, 100);
//		stage.addActor(image);
//
//		// finally we write up the stage as the input process and call it a day.
//		Gdx.input.setInputProcessor(stage);
//	}
//
//	public override void Resize (int width, int height) {
//		// handling resizing is simple, just set the camera to ortho again
//		camera.setToOrtho(true, width, height);
//	}
//
//	public override void Render () {
//		// clear the screen, update the camera and make the sprite batch
//		// use its matrices.
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		camera.update();
//		batch.setProjectionMatrix(camera.combined);
//
//		// render all the things, we render in a y-down
//		// cartesian coordinate system
//		batch.begin();
//		// drawing a region, x and y will be the top left corner of the region, would be bottom left
//		// with y-up.
//		batch.draw(region, 20, 100);
//		// drawing text, x and y will be the top left corner for text, same as with y-up
//		font.draw(batch, "This is a test", 270, 100);
//		// drawing regions from an atlas, x and y will be the top left corner.
//		// you shouldn't call findRegion every frame, cache the result.
//		batch.draw(atlas.findRegion("badlogicsmall"), 360, 100);
//		// drawing a sprite created from an atlas, FIXME wut?! AtlasSprite#setPosition seems to be wrong
//		sprite.setColor(Color.RED);
//		sprite.draw(batch);
//		// finally we draw our current touch/mouse coordinates
//		font.draw(batch, Gdx.input.getX() + ", " + Gdx.input.getY(), 0, 0);
//		batch.end();
//
//		// tell the stage to act and draw itself
//		stage.act(Gdx.graphics.getDeltaTime());
//		stage.draw();
//	}
//
//	/** A very simple actor implementation that does not obey rotation/scale/origin set on the actor. Allows dragging of the actor.
//	 * @author mzechner */
//	public class MyActor : Actor {
//		TextureRegion region;
//		float lastX;
//		float lastY;
//
//		public MyActor (TextureRegion region) {
//			this.region = region;
//			setWidth(region.getRegionWidth());
//			setHeight(region.getRegionHeight());
//
//			addListener(new InputListener() {
//				public boolean touchDown (InputEvent event, float x, float y, int pointer, int button) {
//					// we only care for the first finger to make things easier
//					if (pointer != 0) return false;
//
//					// record the coordinates the finger went down on. they
//					// are given relative to the actor's upper left corner (0, 0)
//					lastX = x;
//					lastY = y;
//					return true;
//				}
//
//				public void touchDragged (InputEvent event, float x, float y, int pointer) {
//					// we only care for the first finger to make things easier
//					if (pointer != 0) return;
//
//					// adjust the actor's position by (current mouse position - last mouse position)
//					// in the actor's coordinate system.
//					moveBy(x - lastX, y - lastY);
//
//					// save the current mouse position as the basis for the next drag event.
//					// we adjust by the same delta so next time drag is called, lastX/lastY
//					// are in the actor's local coordinate system automatically.
//					lastX = x - (x - lastX);
//					lastY = y - (y - lastY);
//				}
//			});
//		}
//
//		@Override
//		public void draw (Batch batch, float parentAlpha) {
//			batch.draw(region, getX(), getY());
//		}
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		font.Dispose();
//		atlas.Dispose();
//		region.getTexture().Dispose();
//		stage.Dispose();
//	}
//}
