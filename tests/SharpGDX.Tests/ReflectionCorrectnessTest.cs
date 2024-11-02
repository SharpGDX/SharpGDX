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
//public class ReflectionCorrectnessTest : GdxTest {
//
//	// Trigger generation of reflection information
//	public AbstractInterfaceStatic abstractInterfaceStatic;
//	public AbstractAnnotationStatic abstractAnnotationStatic;
//	public StaticEnum staticEnum;
//
//	public AbstractInterfaceStatic[] AbstractInterfaceStatic () {
//		return null;
//	}
//
//	public AbstractAnnotationStatic[] AbstractAnnotationStatic () {
//		return null;
//	}
//
//	public StaticEnum[] StaticEnum () {
//		return null;
//	}
//
//	public static class Expectation {
//		public Expectation mArray () {
//			isArray = true;
//			isAbstract = true; // Arrays are _always_ abstract
//			return this;
//		}
//
//		public Expectation mEnum () {
//			isEnum = true;
//			return this;
//		}
//
//		public Expectation mInterface () {
//			isInterface = true;
//			isAbstract = true; // Interfaces are implicitly abstract
//			return this;
//		}
//
//		public Expectation mPrim () {
//			isPrimitive = true;
//			isAbstract = true; // Primitives are _always_ abstract
//			return this;
//		}
//
//		public Expectation mAnnot () {
//			isAnnotation = true;
//			return this;
//		}
//
//		public Expectation mStatic () {
//			isStatic = true;
//			return this;
//		}
//
//		public Expectation mAbstract () {
//			isAbstract = true;
//			return this;
//		}
//
//		public Expectation mCompType (Class c) {
//			componentType = c;
//			return this;
//		}
//
//		public boolean isArray, isEnum, isInterface, isPrimitive, isAnnotation, isStatic, isAbstract;
//		public Class componentType;
//	}
//
//	public void create () {
//		testIntClass();
//		testIntArrayClass();
//		testJavaLangIntegerClass();
//		testJavaLangStringClass();
//		testJavaLangStringArrayClass();
//		testCustomInterfaceClass();
//		testCustomInterfaceArrayClass();
//		testCustomAnnotationClass();
//		testCustomAnnotationArrayClass();
//		testCustomEnumClass();
//		testCustomEnumArrayClass();
//		testScene2DTouchableEnum();
//		testScene2DTouchableArrayEnum();
//	}
//
//	public void testIntClass () {
//		Class clazz = int.class;
//		Expectation e = new Expectation().mPrim();
//		doTest(clazz, e);
//	}
//
//	public void testIntArrayClass () {
//		Class clazz = int[].class;
//		Class componentClazz = int.class;
//		Expectation e = new Expectation().mArray().mCompType(componentClazz);
//		doTest(clazz, e);
//	}
//
//	public void testJavaLangIntegerClass () {
//		Class clazz = Integer.class;
//		Expectation e = new Expectation();
//		doTest(clazz, e);
//	}
//
//	public void testJavaLangStringClass () {
//		Class clazz = String.class;
//		Expectation e = new Expectation();
//		doTest(clazz, e);
//	}
//
//	public void testJavaLangStringArrayClass () {
//		Class clazz = String[].class;
//		Class componentClazz = String.class;
//		Expectation e = new Expectation().mArray().mCompType(componentClazz);
//		doTest(clazz, e);
//	}
//
//	public void testCustomInterfaceClass () {
//		Class clazz = com.badlogic.gdx.tests.ReflectionCorrectnessTest.AbstractInterfaceStatic.class;
//		Expectation e = new Expectation().mInterface().mStatic();
//		doTest(clazz, e);
//	}
//
//	public void testCustomInterfaceArrayClass () {
//		Class clazz = com.badlogic.gdx.tests.ReflectionCorrectnessTest.AbstractInterfaceStatic[].class;
//		Class componentClazz = com.badlogic.gdx.tests.ReflectionCorrectnessTest.AbstractInterfaceStatic.class;
//		Expectation e = new Expectation().mArray().mCompType(componentClazz);
//		doTest(clazz, e);
//	}
//
//	public void testCustomAnnotationClass () {
//		Class clazz = com.badlogic.gdx.tests.ReflectionCorrectnessTest.AbstractAnnotationStatic.class;
//		Expectation e = new Expectation().mInterface().mAnnot().mStatic();
//		doTest(clazz, e);
//	}
//
//	public void testCustomAnnotationArrayClass () {
//		Class clazz = com.badlogic.gdx.tests.ReflectionCorrectnessTest.AbstractAnnotationStatic[].class;
//		Class componentClazz = com.badlogic.gdx.tests.ReflectionCorrectnessTest.AbstractAnnotationStatic.class;
//		Expectation e = new Expectation().mArray().mCompType(componentClazz);
//		doTest(clazz, e);
//	}
//
//	public void testCustomEnumClass () {
//		Class clazz = com.badlogic.gdx.tests.ReflectionCorrectnessTest.StaticEnum.class;
//		Expectation e = new Expectation().mEnum().mStatic();
//		doTest(clazz, e);
//	}
//
//	public void testCustomEnumArrayClass () {
//		Class clazz = com.badlogic.gdx.tests.ReflectionCorrectnessTest.StaticEnum[].class;
//		Class componentClazz = com.badlogic.gdx.tests.ReflectionCorrectnessTest.StaticEnum.class;
//		Expectation e = new Expectation().mArray().mCompType(componentClazz);
//		doTest(clazz, e);
//	}
//
//	private void testScene2DTouchableEnum () {
//		Class clazz = com.badlogic.gdx.scenes.scene2d.Touchable.class;
//		Expectation e = new Expectation().mEnum();
//		doTest(clazz, e);
//	}
//
//	private void testScene2DTouchableArrayEnum () {
//		Class clazz = com.badlogic.gdx.scenes.scene2d.Touchable[].class;
//		Class componentClazz = com.badlogic.gdx.scenes.scene2d.Touchable.class;
//		Expectation e = new Expectation().mArray().mCompType(componentClazz);
//		doTest(clazz, e);
//	}
//
//	public void expectResult (boolean expected, boolean actual, String message) {
//		if (expected != actual) {
//			throw new AssertionError("Expected that " + message + " is " + expected + " but is " + actual);
//		}
//	}
//
//	public void expectResult (Class expected, Class actual, String message) {
//		if (expected != actual) {
//			throw new AssertionError("Expected that " + message + " is " + expected + " but is " + actual);
//		}
//	}
//
//	public void doTest (final Class c, Expectation e) {
//
//		Gdx.app.log("ClassReflectionTest", "Name of class: " + c.getName());
//
//		boolean isArray = ClassReflection.isArray(c);
//		expectResult(e.isArray, isArray, "value of is Array");
//
//		boolean isEnum = ClassReflection.isEnum(c);
//		expectResult(e.isEnum, isEnum, "value of is Enum");
//
//		boolean isInterface = ClassReflection.isInterface(c);
//		expectResult(e.isInterface, isInterface, "value of is Interface");
//
//		boolean isPrimitive = ClassReflection.isPrimitive(c);
//		expectResult(e.isPrimitive, isPrimitive, "value of is Primitive");
//
//		boolean isAnnotation = ClassReflection.isAnnotation(c);
//		expectResult(e.isAnnotation, isAnnotation, "value of is Annotation");
//
//		boolean isStaticClass = ClassReflection.isStaticClass(c);
//		expectResult(e.isStatic, isStaticClass, "value of is Static Class");
//
//		boolean isAbstract = ClassReflection.isAbstract(c);
//		expectResult(e.isAbstract, isAbstract, "value of is Abstract Class");
//
//		Class componentType = ClassReflection.getComponentType(c);
//		expectResult(e.componentType, componentType, "component type of Array");
//
//	}
//
//	public static abstract interface AbstractInterfaceStatic {
//	}
//
//	public static abstract @interface AbstractAnnotationStatic {
//	}
//
//	public static enum StaticEnum {
//	}
//
//}
