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
///** This is a simple demonstration of how to perform VERY basic culling on hierarchies of stage actors that do not scale or
// * rotate. It is not a general solution as it assumes that actors and groups are only translated (moved, change their x/y
// * coordinates). NOTE: This has been obsoleted by {@link Cullable}.
// * 
// * @author mzechner */
//public class SimpleStageCullingTest : GdxTest {
//
//	/** We need to extend a base actor class so we can add the culling in the render method. We also add a method to get the stage
//	 * coordinates of the actor so we can cull it against the camera's view volume.
//	 * 
//	 * @author mzechner */
//	private class CullableActor : Image {
//		/** the camera to test against **/
//		readonly OrthographicCamera camera;
//		/** whether we are visible or not, used for counting visible actors **/
//		bool visible = false;
//
//		public CullableActor (String name, Texture texture, OrthographicCamera camera) 
//        : base(new TextureRegion(texture))
//        {
//			
//			setAlign(Align.center);
//			setScaling(Scaling.none);
//			this.camera = camera;
//		}
//
//		public override void draw (IBatch batch, float parentAlpha) {
//			// if this actor is not within the view of the camera we don't draw it.
//			if (isCulled()) return;
//
//			// otherwise we draw via the super class method
//			base.draw(batch, parentAlpha);
//		}
//
//		/** static helper Rectangles **/
//		Rectangle actorRect = new Rectangle();
//		Rectangle camRect = new Rectangle();
//
//		private bool isCulled () {
//			// we start by setting the stage coordinates to this
//			// actors coordinates which are relative to its parent
//			// Group.
//			float stageX = getX();
//			float stageY = getY();
//
//			// now we go up the hierarchy and add all the parents'
//			// coordinates to this actors coordinates. Note that
//			// this assumes that neither this actor nor any of its
//			// parents are rotated or scaled!
//			Actor parent = this.getParent();
//			while (parent != null) {
//				stageX += parent.getX();
//				stageY += parent.getY();
//				parent = parent.getParent();
//			}
//
//			// now we check if the rectangle of this actor in screen
//			// coordinates is in the rectangle spanned by the camera's
//			// view. This assumes that the camera has no zoom and is
//			// not rotated!
//			actorRect.set(stageX, stageY, getWidth(), getHeight());
//			camRect.set(camera.position.x - camera.viewportWidth / 2.0f, camera.position.y - camera.viewportHeight / 2.0f,
//				camera.viewportWidth, camera.viewportHeight);
//			visible = camRect.overlaps(actorRect);
//			return !visible;
//		}
//	}
//
//	OrthoCamController camController;
//	Stage stage;
//	Texture texture;
//	SpriteBatch batch;
//	BitmapFont font;
//
//	public override void Create () {
//		// create a stage and a camera controller so we can pan the view.
//		stage = new Stage();
//		;
//		camController = new OrthoCamController((OrthographicCamera)stage.getCamera()); // we know it's an ortho cam at this point!
//		Gdx.input.setInputProcessor(camController);
//
//		// load a dummy texture
//		texture = new Texture(Gdx.files.@internal("data/badlogicsmall.jpg"));
//
//		// populate the stage with some actors and groups.
//		for (int i = 0; i < 5000; i++) {
//			Actor img = new CullableActor("img" + i, texture, (OrthographicCamera)stage.getCamera());
//			img.setX((float)Math.random() * 480 * 10);
//			img.setY((float)Math.random() * 320 * 10);
//			stage.addActor(img);
//		}
//
//		// we also want to output the number of visible actors, so we need a SpriteBatch and a BitmapFont
//		batch = new SpriteBatch();
//		font = new BitmapFont(Gdx.files.@internal("data/lsans-15.fnt"), false);
//	}
//
//	public void render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		stage.draw();
//
//		// check how many actors are visible.
//		Array<Actor> actors = stage.getActors();
//		int numVisible = 0;
//		for (int i = 0; i < actors.size; i++) {
//			numVisible += ((CullableActor)actors.get(i)).visible ? 1 : 0;
//		}
//
//		batch.begin();
//		font.draw(batch, "Visible: " + numVisible + ", fps: " + Gdx.graphics.getFramesPerSecond(), 20, 30);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		stage.Dispose();
//		texture.Dispose();
//		batch.Dispose();
//		font.Dispose();
//	}
//}
