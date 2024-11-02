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
//using SharpGDX.Maps;
//using SharpGDX.Maps.Objects;
//using SharpGDX.Maps.Tiled;
//using SharpGDX.Maps.Tiled.Renderers;
//using SharpGDX.Assets.Loaders;
//using SharpGDX.Assets;
//using SharpGDX.Assets.Loaders.Resolvers;
//
//namespace SharpGDX.Tests;
//
//public class TiledMapAnimationLoadingTest : GdxTest {
//
//	private TiledMap map;
//	private OrthographicCamera camera;
//	private OrthoCamController cameraController;
//	private BitmapFont font;
//	private SpriteBatch batch;
//
//	public override void Create () {
//		float w = Gdx.graphics.getWidth();
//		float h = Gdx.graphics.getHeight();
//
//		camera = new OrthographicCamera();
//		camera.setToOrtho(false, (w / h) * 512, 512);
//		camera.zoom = 1f;
//		camera.update();
//
//		cameraController = new OrthoCamController(camera);
//		Gdx.input.setInputProcessor(cameraController);
//
//		font = new BitmapFont();
//		batch = new SpriteBatch();
//		map = new TmxMapLoader().load("data/maps/tiled-animations/test-load-animations.tmx");
//
//		MapLayer layer = map.getLayers().get("Objects");
//		MapObjects mapObjects = layer.getObjects();
//
//		mapObjects.add(new CircleMapObject(280, 400, 50));
//
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.55f, 0.55f, 0.55f, 1f);
//		camera.update();
//		batch.setProjectionMatrix(camera.combined);
//		MapLayer layer = map.getLayers().get("Objects");
//		AnimatedTiledMapTile.updateAnimationBaseTime();
//		for (MapObject mapObject : layer.getObjects()) {
//			if (!mapObject.isVisible()) continue;
//			if (mapObject instanceof TiledMapTileMapObject) {
//				batch.begin();
//				TiledMapTileMapObject tmtObject = (TiledMapTileMapObject)mapObject;
//				TextureRegion textureRegion = tmtObject.getTile().getTextureRegion();
//				// TilEd rotation is clockwise, we need counter-clockwise.
//				float rotation = -tmtObject.getRotation();
//				float scaleX = tmtObject.getScaleX();
//				float scaleY = tmtObject.getScaleY();
//				float xPos = tmtObject.getX();
//				float yPos = tmtObject.getY();
//				textureRegion.flip(tmtObject.isFlipHorizontally(), tmtObject.isFlipVertically());
//				batch.draw(textureRegion, xPos, yPos, tmtObject.getOriginX() * scaleX, tmtObject.getOriginY() * scaleY,
//					textureRegion.getRegionWidth() * scaleX, textureRegion.getRegionHeight() * scaleY, 1f, 1f, rotation);
//				// We flip back to the original state.
//				textureRegion.flip(tmtObject.isFlipHorizontally(), tmtObject.isFlipVertically());
//				batch.end();
//			}
//		}
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		map.Dispose();
//	}
//}
