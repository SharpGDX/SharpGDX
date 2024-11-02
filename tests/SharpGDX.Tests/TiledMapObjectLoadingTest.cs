//using SharpGDX.Tests.Utils;
//using SharpGDX.Utils;
//using SharpGDX.Scenes.Scene2D;
//using SharpGDX.Scenes.Scene2D.Utils;
//using SharpGDX.Scenes.Scene2D.UI;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Utils.Viewports;
//using SharpGDX.Shims;
//using SharpGDX.Maps.Tiled;
//using SharpGDX.Maps.Tiled.Tiles;
//using SharpGDX.Maps;
//using SharpGDX.Maps.Objects;
//using SharpGDX.Mathematics;
//using SharpGDX.Graphics.GLUtils;
//
//namespace SharpGDX.Tests;
//
//public class TiledMapObjectLoadingTest : GdxTest {
//
//	private TiledMap map;
//	private ShapeRenderer shapeRenderer;
//	private OrthographicCamera camera;
//	private OrthoCamController cameraController;
//	private BitmapFont font;
//	private SpriteBatch batch;
//	private String loadingStatus;
//
//	public override void Create () {
//		float w = Gdx.graphics.getWidth();
//		float h = Gdx.graphics.getHeight();
//
//		camera = new OrthographicCamera();
//		camera.setToOrtho(false, (w / h) * 512, 512);
//		camera.zoom = .5f;
//		camera.update();
//
//		cameraController = new OrthoCamController(camera);
//		Gdx.input.setInputProcessor(cameraController);
//
//		font = new BitmapFont();
//		batch = new SpriteBatch();
//		map = new TmxMapLoader().load("data/maps/tiled-objects/test-load-mapobjects.tmx");
//		MapProperties properties = map.getProperties();
//		shapeRenderer = new ShapeRenderer();
//
//		// Test get objects by type (adding circle manually because it doesn't exists in Tiledmap editor)
//
//		loadingStatus = "loading status:\n";
//		MapLayer layer = map.getLayers().get("Objects");
//		MapObjects mapObjects = layer.getObjects();
//
//		mapObjects.add(new CircleMapObject(280, 400, 50));
//
//		loadingStatus += "- MapObject : " + mapObjects.getByType(MapObject.class).size + "\n";
//		loadingStatus += "- CircleMapObject : " + mapObjects.getByType(CircleMapObject.class).size + "\n";
//		loadingStatus += "- EllipseMapObject : " + mapObjects.getByType(EllipseMapObject.class).size + "\n";
//		loadingStatus += "- PolygonMapObject : " + mapObjects.getByType(PolygonMapObject.class).size + "\n";
//		loadingStatus += "- PolylineMapObject : " + mapObjects.getByType(PolylineMapObject.class).size + "\n";
//		loadingStatus += "- RectangleMapObject : " + mapObjects.getByType(RectangleMapObject.class).size + "\n";
//		loadingStatus += "- TextureMapObject : " + mapObjects.getByType(TextureMapObject.class).size + "\n";
//		loadingStatus += "- TiledMapTileMapObject : " + mapObjects.getByType(TiledMapTileMapObject.class).size + "\n";
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.55f, 0.55f, 0.55f, 1f);
//		camera.update();
//		shapeRenderer.setProjectionMatrix(camera.combined);
//		batch.setProjectionMatrix(camera.combined);
//		shapeRenderer.setColor(Color.BLUE);
//		Gdx.gl20.glLineWidth(2);
//		MapLayer layer = map.getLayers().get("Objects");
//		AnimatedTiledMapTile.updateAnimationBaseTime();
//		foreach (MapObject mapObject in layer.getObjects()) {
//			if (!mapObject.isVisible()) continue;
//			if (mapObject is TiledMapTileMapObject) {
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
//			} else if (mapObject is EllipseMapObject) {
//				shapeRenderer.begin(ShapeRenderer.ShapeType.Filled);
//				Ellipse ellipse = ((EllipseMapObject)mapObject).getEllipse();
//				shapeRenderer.ellipse(ellipse.x, ellipse.y, ellipse.width, ellipse.height);
//				shapeRenderer.end();
//			} else if (mapObject is CircleMapObject) {
//				shapeRenderer.begin(ShapeRenderer.ShapeType.Filled);
//				Circle circle = ((CircleMapObject)mapObject).getCircle();
//				shapeRenderer.circle(circle.x, circle.y, circle.radius);
//				shapeRenderer.end();
//			} else if (mapObject is RectangleMapObject) {
//				shapeRenderer.begin(ShapeRenderer.ShapeType.Filled);
//				Rectangle rectangle = ((RectangleMapObject)mapObject).getRectangle();
//				shapeRenderer.rect(rectangle.x, rectangle.y, rectangle.width, rectangle.height);
//				shapeRenderer.end();
//			} else if (mapObject is PolygonMapObject) {
//				shapeRenderer.begin(ShapeRenderer.ShapeType.Line);
//				Polygon polygon = ((PolygonMapObject)mapObject).getPolygon();
//				shapeRenderer.polygon(polygon.getTransformedVertices());
//				shapeRenderer.end();
//			} else if (mapObject is PolylineMapObject) {
//				shapeRenderer.begin(ShapeRenderer.ShapeType.Line);
//				Polyline polyline = ((PolylineMapObject)mapObject).getPolyline();
//				shapeRenderer.polyline(polyline.getTransformedVertices());
//				shapeRenderer.end();
//			}
//		}
//		batch.begin();
//		font.draw(batch, "FPS: " + Gdx.graphics.getFramesPerSecond() + "\n" + loadingStatus, 20, 500);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		map.Dispose();
//		shapeRenderer.Dispose();
//	}
//}
