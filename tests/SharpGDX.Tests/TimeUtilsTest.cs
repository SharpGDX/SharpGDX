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
///** Test utility functions in TimeUtils.java
// * @author Jon Renner */
//public class TimeUtilsTest : GdxTest {
//	readonly long oneMilliInNanos = 1000000;
//
//	public override void Create () {
//		// test nanos -> millis -> nanos
//		long now = TimeUtils.nanoTime();
//		long nowConvertToMillis = TimeUtils.nanosToMillis(now);
//		long nowConvertBackToNanos = TimeUtils.millisToNanos(nowConvertToMillis);
//
//		assertEpsilonEqual(now, nowConvertBackToNanos, "Nano -> Millis conversion");
//
//		// test millis -> nanos -> millis
//		long millis = TimeUtils.millis();
//		long millisToNanos = TimeUtils.millisToNanos(millis);
//		long nanosToMillis = TimeUtils.nanosToMillis(millisToNanos);
//
//		assertAbsoluteEqual(millis, nanosToMillis, "Millis -> Nanos conversion");
//
//		// test comparison for 1 sec
//		long oneSecondMillis = 1000;
//		long oneSecondNanos = 1000000000;
//
//		assertAbsoluteEqual(oneSecondMillis, TimeUtils.nanosToMillis(oneSecondNanos), "One Second Comparison, Nano -> Millis");
//		assertAbsoluteEqual(TimeUtils.millisToNanos(oneSecondMillis), oneSecondNanos, "One Second Comparison, Millis -> Nanos");
//	}
//
//	public override void Render () {
//
//	}
//
//	private void failTest (String testName) {
//		throw new GdxRuntimeException("FAILED TEST: [" + testName + "]");
//	}
//
//	private void assertAbsoluteEqual (long a, long b, String testName) {
//		// because of precision loss in conversion, epsilon = 1 ms worth of nanos
//		Console.WriteLine("Compare " + a + " to " + b);
//		if (a != b) {
//			failTest(testName + " - NOT EQUAL");
//		} else {
//			Console.WriteLine("TEST PASSED: " + testName);
//		}
//	}
//
//	private void assertEpsilonEqual (long a, long b, String testName) {
//		Console.WriteLine("Compare " + a + " to " + b);
//		if (Math.abs(a - b) > oneMilliInNanos) {
//			failTest(testName + " - NOT EQUAL");
//		} else {
//			Console.WriteLine("TEST PASSED: " + testName);
//		}
//	}
//
//}
