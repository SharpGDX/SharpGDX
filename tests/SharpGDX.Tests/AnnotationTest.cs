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
//
//namespace SharpGDX.Tests;
//
///** Performs some tests with {@link Annotation} and prints the results on the screen.
// * @author dludwig */
//public class AnnotationTest : GdxTest {
//	String message = "";
//	BitmapFont font;
//	SpriteBatch batch;
//
//	public enum TestEnum {
//		EnumA, EnumB
//	}
//
//	/** Sample annotation. It is required to annotate annotations themselves with {@link RetentionPolicy}.RUNTIME to become
//	 * available to the {@link ClassReflection} package. */
//	@Retention(RetentionPolicy.RUNTIME)
//	static public @interface TestAnnotation {
//		// String parameter, no default
//		String name();
//
//		// Array of integers, with default values
//		int[] values() default {1, 2, 3};
//
//		// Enumeration, with default value
//		TestEnum someEnum() default TestEnum.EnumA;
//	}
//
//	/** Sample usage of class and field annotations. */
//	@TestAnnotation(name = "MyAnnotatedClass", someEnum = TestEnum.EnumB)
//	static public class AnnotatedClass {
//		public int unanottatedField;
//
//		public int unannotatedMethod () {
//			return 0;
//		}
//
//		@TestAnnotation(name = "MyAnnotatedField", values = {4, 5}) public int annotatedValue;
//
//		@TestAnnotation(name = "MyAnnotatedMethod", values = {6, 7})
//		public int annotatedMethod () {
//			return 0;
//		};
//	}
//
//	@Retention(RetentionPolicy.RUNTIME)
//	@Inherited
//	static public @interface TestInheritAnnotation {
//	}
//
//	@TestInheritAnnotation
//	static public class InheritClassA {
//	}
//
//	@TestAnnotation(name = "MyInheritClassB")
//	static public class InheritClassB : InheritClassA {
//	}
//
//	public override void Create () {
//		font = new BitmapFont();
//		batch = new SpriteBatch();
//
//		try {
//			Annotation annotation = ClassReflection.getDeclaredAnnotation(AnnotatedClass.class, TestAnnotation.class);
//			if (annotation != null) {
//				TestAnnotation annotationInstance = annotation.getAnnotation(TestAnnotation.class);
//				println("Class annotation:\n name=" + annotationInstance.name() + ",\n values="
//					+ Arrays.toString(annotationInstance.values()) + ",\n enum=" + annotationInstance.someEnum().toString());
//			} else {
//				println("ERROR: Class annotation not found.");
//			}
//
//			Field field = ClassReflection.getDeclaredField(AnnotatedClass.class, "annotatedValue");
//			if (field != null) {
//				Annotation[] annotations = field.getDeclaredAnnotations();
//				for (Annotation a : annotations) {
//					if (a.getAnnotationType().equals(TestAnnotation.class)) {
//						TestAnnotation annotationInstance = a.getAnnotation(TestAnnotation.class);
//						println("Field annotation:\n name=" + annotationInstance.name() + ",\n values="
//							+ Arrays.toString(annotationInstance.values()) + ",\n enum=" + annotationInstance.someEnum().toString());
//						break;
//					}
//				}
//			} else {
//				println("ERROR: Field 'annotatedValue' not found.");
//			}
//
//			Method method = ClassReflection.getDeclaredMethod(AnnotatedClass.class, "annotatedMethod");
//			if (method != null) {
//				Annotation[] annotations = method.getDeclaredAnnotations();
//				for (Annotation a : annotations) {
//					if (a.getAnnotationType().equals(TestAnnotation.class)) {
//						TestAnnotation annotationInstance = a.getAnnotation(TestAnnotation.class);
//						println("Method annotation:\n name=" + annotationInstance.name() + ",\n values="
//							+ Arrays.toString(annotationInstance.values()) + ",\n enum=" + annotationInstance.someEnum().toString());
//						break;
//					}
//				}
//			} else {
//				println("ERROR: Method 'annotatedMethod' not found.");
//			}
//
//			println("Class annotations w/@Inherit:");
//			Annotation[] annotations = ClassReflection.getAnnotations(InheritClassB.class);
//			for (Annotation a : annotations) {
//				println(" name=" + a.getAnnotationType().getSimpleName());
//			}
//			if (!ClassReflection.isAnnotationPresent(InheritClassB.class, TestInheritAnnotation.class)) {
//				println("ERROR: Inherited class annotation not found.");
//			}
//		} catch (Exception e) {
//			println("FAILED: " + e.getMessage());
//			message += e.getClass();
//		}
//	}
//
//	private void println (String line) {
//		message += line + "\n";
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		font.draw(batch, message, 20, Gdx.graphics.getHeight() - 20);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		font.Dispose();
//	}
//
//}
