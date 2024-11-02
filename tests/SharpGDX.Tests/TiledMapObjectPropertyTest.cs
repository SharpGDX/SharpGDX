//using SharpGDX.Tests.Utils;
//using SharpGDX.Maps.Objects;
//using SharpGDX.Utils;
//using SharpGDX.Scenes.Scene2D;
//using SharpGDX.Scenes.Scene2D.Utils;
//using SharpGDX.Scenes.Scene2D.UI;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Utils.Viewports;
//using SharpGDX.Maps.Tiled;
//using SharpGDX.Maps;
//using System.Text;
//using SharpGDX.Shims;
//using SharpGDX.Mathematics;
//using SharpGDX.Graphics.GLUtils;
//
//namespace SharpGDX.Tests;
//
//public class TiledMapObjectPropertyTest : GdxTest {
//
//	private TiledMap map;
//	private SpriteBatch batch;
//	private ShapeRenderer shapeRenderer;
//	private OrthographicCamera camera;
//	private TiledMapRenderer mapRenderer;
//	private Array<MapObject> objects;
//
//	private bool error;
//
//	public override void Create () {
//		try {
//			TmxMapLoader loader = new TmxMapLoader();
//			// run multiple times to ensure reloading map works correctly
//			for (int i = 0; i < 3; i++) {
//				Gdx.app.log("-------------------------------------", "Running test " + (i + 1) + "/3\n");
//
//				StringBuilder builder = new StringBuilder();
//				builder.Append("Expected results:\n").Append("- Object with id 1 should have \"object\" props:\n")
//					.Append("\t- Points_To_ID_1 = id: 1\n").Append("\t- Points_To_ID_2 = id: 2\n")
//					.Append("\t- Points_To_ID_5 = id: 5\n").Append("- Object with id 2 should have \"object\" props:\n")
//					.Append("\t- Points_To_ID_3 = id: 3\n").Append("\t- Points_To_ID_4 = id: 4\n")
//					.Append("- Object with id 3 should have \"object\" props:\n").Append("\t- Points_To_ID_2 = id: 2\n")
//					.Append("- Object with id 4 should have \"object\" props:\n").Append("\t- Points_To_ID_1 = id: 1\n")
//					.Append("- Objects with id's 5 and 6 should have \"object\" props:\n").Append("\t- Placeholder = 0\n");
//				Gdx.app.log("TiledMapObjectPropertyTest", builder.ToString());
//
//				float w = Gdx.graphics.getWidth();
//				float h = Gdx.graphics.getHeight();
//
//				camera = new OrthographicCamera();
//				camera.setToOrtho(false, (w / h) * 512, 512);
//				camera.zoom = .5f;
//				camera.update();
//
//				OrthoCamController cameraController = new OrthoCamController(camera);
//				Gdx.input.setInputProcessor(cameraController);
//
//				map = loader.load("data/maps/tiled-objects/test-object-properties.tmx");
//
//				batch = new SpriteBatch();
//				shapeRenderer = new ShapeRenderer();
//				mapRenderer = new OrthogonalTiledMapRenderer(map);
//
//				MapObjects objects1 = map.getLayers().get("Objects 1").getObjects();
//				MapObjects objects2 = map.getLayers().get("Objects 2").getObjects();
//				objects = new ();
//				foreach (MapObject obj in objects1) {
//					objects.add(obj);
//				}
//				foreach (MapObject obj in objects2) {
//					objects.add(obj);
//				}
//
//				IntMap<MapObject> idToObject = loader.getIdToObject();
//
//				builder.Clear();
//				builder.Append("\nidToObject: {");
//				foreach (var entry in idToObject) {
//					builder.Append("\n\t").Append(entry.key).Append(" -> ").Append(entry.value);
//				}
//				builder.Append("\n}\n");
//				Gdx.app.log("TiledMapObjectPropertyTest", builder.ToString());
//
//				foreach (MapObject object1 in objects) {
//					int id = object1.getProperties().get("id", Integer.class);
//					MapObject object2 = idToObject.get(id);
//					if (object1 != object2) {
//						throw new RuntimeException(
//							"Error! Object with id " + id + " " + "is not the same object as the one in the idToObject map!");
//					}
//
//					MapProperties props = object1.getProperties();
//					switch (id) {
//					case 1:
//						test(props, 2, idToObject);
//						test(props, 5, idToObject);
//						test(props, 1, idToObject);
//						break;
//					case 2:
//						test(props, 3, idToObject);
//						test(props, 4, idToObject);
//						break;
//					case 3:
//						test(props, 2, idToObject);
//						break;
//					case 4:
//						test(props, 1, idToObject);
//						break;
//					case 5:
//					case 6:
//						var propKeysIterator = props.getKeys();
//						ObjectSet<String> propKeys = new();
//						while (propKeysIterator.hasNext()) {
//							propKeys.add(propKeysIterator.next());
//						}
//						if (propKeys.size != 6) {
//							throw new RuntimeException("Object with id " + id + " should " + "have six keys " + "but has " + propKeys);
//						}
//					}
//				}
//
//				builder.Clear();
//				builder.Append("Actual results:\n");
//				foreach (var entry in idToObject.entries()) {
//					int id = entry.key;
//					MapProperties props = entry.value.getProperties();
//
//					builder.Append("- Object with id ").Append(id).append(" has \"object\" props:\n");
//
//					var propKeysIterator = props.getKeys();
//					var propValuesIterator = props.getValues();
//
//					while (propKeysIterator.hasNext() && propValuesIterator.hasNext()) {
//						Object value = propValuesIterator.next();
//						String key = propKeysIterator.next();
//						if (!key.Contains("Points_To_ID_") && !key.Contains("Placeholder")) {
//							continue;
//						}
//
//						if (value is MapObject) {
//							MapObject obj = (MapObject)value;
//							int objectId = obj.getProperties().get("id", Integer.class);
//							value = "id: " + objectId + ", object: " + obj;
//						}
//
//						builder.Append("\t\t").Append(key).Append(" -> ").Append(value).Append("\n");
//					}
//				}
//				Gdx.app.log("TiledMapObjectPropertyTest", builder.ToString());
//			}
//		} catch (Exception e) {
//			Gdx.app.error("TiledMapObjectPropertyTest", "Failed to run test!", e);
//			e.printStackTrace();
//			error = true;
//		}
//	}
//
//	public override void Render () {
//		if (error) {
//			Gdx.app.error("TiledMapObjectPropertyTest", "Failed to run test!");
//			Gdx.app.exit();
//		}
//
//		ScreenUtils.clear(0.55f, 0.55f, 0.55f, 1f);
//		camera.update();
//		mapRenderer.setView(camera);
//		mapRenderer.render();
//
//		shapeRenderer.setProjectionMatrix(camera.combined);
//		batch.setProjectionMatrix(camera.combined);
//
//		shapeRenderer.setColor(Color.BLUE);
//		Gdx.gl20.glLineWidth(2);
//		foreach (MapObject obj in objects) {
//			if (!obj.isVisible()) continue;
//			if (obj is RectangleMapObject) {
//				shapeRenderer.begin(ShapeRenderer.ShapeType.Filled);
//				Rectangle rectangle = ((RectangleMapObject)obj).getRectangle();
//				shapeRenderer.rect(rectangle.x, rectangle.y, rectangle.width, rectangle.height);
//				shapeRenderer.end();
//			}
//		}
//	}
//
//	public override void Dispose () {
//		map.Dispose();
//		shapeRenderer.Dispose();
//	}
//
//	private void test (MapProperties props, int idToObjProp1, IntMap<MapObject> idToObjectMap) {
//		String key = "Points_To_ID_" + idToObjProp1;
//		if (!props.containsKey(key)) {
//			throw new GdxRuntimeException("Missing property: " + key);
//		}
//
//		MapObject other1 = idToObjectMap.get(idToObjProp1);
//		MapObject other2 = props.get(key, MapObject.class);
//
//		if (other1 != other2) {
//			throw new GdxRuntimeException("Property " + key + " does not point to the correct object");
//		}
//	}
//}
