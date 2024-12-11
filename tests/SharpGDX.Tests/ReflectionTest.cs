//using SharpGDX.Tests.Utils;
//using SharpGDX.Utils;
//using SharpGDX.Scenes.Scene2D;
//using SharpGDX.Scenes.Scene2D.Utils;
//using SharpGDX.Scenes.Scene2D.UI;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Utils.Viewports;
//using SharpGDX.Shims;
//using SharpGDX.Utils.Reflect;
//using SharpGDX.Mathematics;
//using SharpGDX.Graphics.GLUtils;
//
//namespace SharpGDX.Tests;
//
///** Performs some tests with {@link ClassReflection} and prints the results on the screen.
// * @author hneuer */
//public class ReflectionTest : GdxTest {
//	String message = "";
//	BitmapFont font;
//	SpriteBatch batch;
//
//	public override void Create () {
//		font = new BitmapFont();
//		batch = new SpriteBatch();
//
//		try {
//			Vector2 fromDefaultConstructor = ClassReflection.newInstance(typeof(Vector2));
//			println("From default constructor: " + fromDefaultConstructor);
//
//			Method mSet = ClassReflection.getMethod(typeof(Vector2), "set", typeof(float), typeof(float));
//			mSet.invoke(fromDefaultConstructor, 10, 11);
//			println("Set to 10/11: " + fromDefaultConstructor);
//
//			Constructor copyConstroctor = ClassReflection.getConstructor(typeof(Vector2), typeof(Vector2));
//			Vector2 fromCopyConstructor = (Vector2)copyConstroctor.newInstance(fromDefaultConstructor);
//			println("From copy constructor: " + fromCopyConstructor);
//
//			Method mMul = ClassReflection.getMethod(Vector2.class, "scl", float.class);
//			println("Multiplied by 2; " + mMul.invoke(fromCopyConstructor, 2));
//
//			Method mNor = ClassReflection.getMethod(Vector2.class, "nor");
//			println("Normalized: " + mNor.invoke(fromCopyConstructor));
//
//			Vector2 fieldCopy = new Vector2();
//			Field fx = ClassReflection.getField(Vector2.class, "x");
//			Field fy = ClassReflection.getField(Vector2.class, "y");
//			fx.set(fieldCopy, fx.get(fromCopyConstructor));
//			fy.set(fieldCopy, fy.get(fromCopyConstructor));
//			println("Copied field by field: " + fieldCopy);
//
//			Json json = new Json();
//			String jsonString = json.toJson(fromCopyConstructor);
//			Vector2 fromJson = json.fromJson(typeof(Vector2), jsonString);
//			println("JSON serialized: " + jsonString);
//			println("JSON deserialized: " + fromJson);
//			fromJson.x += 1;
//			fromJson.y += 1;
//			println("JSON deserialized + 1/1: " + fromJson);
//
//			Object array = ArrayReflection.newInstance(typeof(int), 5);
//			ArrayReflection.set(array, 0, 42);
//			println("Array int: length=" + ArrayReflection.getLength(array) + ", access=" + ArrayReflection.get(array, 0));
//
//			array = ArrayReflection.newInstance(typeof(string), 5);
//			ArrayReflection.set(array, 0, "test string");
//			println("Array String: length=" + ArrayReflection.getLength(array) + ", access=" + ArrayReflection.get(array, 0));
//		} catch (Exception e) {
//			message = "FAILED: " + e.Message + "\n";
//			message += e.GetType();
//		}
//	}
//
//	private void println (String line) {
//		message += line + "\n";
//	}
//
//	public override void Render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		font.draw(batch, message, 20, Gdx.graphics.getHeight() - 20);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		font.Dispose();
//	}
//}
