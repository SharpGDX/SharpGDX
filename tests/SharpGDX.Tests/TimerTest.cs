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
//public class TimerTest : GdxTest {
//	static readonly int expectedTests = 10;
//
//	public override void Create () {
//		Gdx.app.log("TimerTest", "Starting tests...");
//		testOnce();
//		testRepeat();
//		testCancelPosted();
//		testCancelPostedTwice();
//		testCancelPostedReschedule();
//		testStop();
//		testStopStart();
//		testDelay();
//		testClear();
//	}
//
//	float time;
//	int testCount;
//	Task repeatTask;
//
//	public void render () {
//		if (time < 2) {
//			time += Gdx.graphics.getDeltaTime();
//			if (time >= 2) {
//				if (testCount != expectedTests)
//					throw new RuntimeException("Some tests did not run: " + testCount + " != " + expectedTests);
//				Gdx.app.log("TimerTest", "SUCCESS! " + testCount + " tests passed.");
//				repeatTask.cancel();
//			}
//		}
//	}
//
//	void testOnce () {
//		Task task = Timer.schedule(new Task() {
//			@Override
//			public void run () {
//				Gdx.app.log("TimerTest", "testOnce");
//				testCount++;
//			}
//		}, 1);
//		assertScheduled(task);
//	}
//
//	void testRepeat () {
//		repeatTask = Timer.schedule(new Task() {
//			int count;
//
//			@Override
//			public void run () {
//				Gdx.app.log("TimerTest", "testRepeat");
//				if (++count <= 2) testCount++;
//			}
//		}, 1, 1);
//		assertScheduled(repeatTask);
//	}
//
//	void testCancelPosted () {
//		Task task = Timer.schedule(new Task() {
//			@Override
//			public void run () {
//				throw new RuntimeException("Cancelled task should not run.");
//			}
//		}, 0.01f);
//		assertScheduled(task);
//		sleep(200); // Sleep so the task is posted.
//		assertNotScheduled(task);
//		task.cancel(); // The posted task should not execute.
//		assertNotScheduled(task);
//		Gdx.app.log("TimerTest", "testCancelPosted");
//		testCount++;
//	}
//
//	void testCancelPostedTwice () {
//		Task task = new Task() {
//			@Override
//			public void run () {
//				throw new RuntimeException("Cancelled task should not run.");
//			}
//		};
//		Timer.schedule(task, 0.01f);
//		assertScheduled(task);
//		sleep(200); // Sleep so the task is posted.
//		assertNotScheduled(task);
//		Timer.schedule(task, 0.01f);
//		assertScheduled(task);
//		sleep(200); // Sleep so the task is posted.
//		assertNotScheduled(task);
//		task.cancel(); // The twice posted task should not execute.
//		assertNotScheduled(task);
//		Gdx.app.log("TimerTest", "testCancelPostedTwice");
//		testCount++;
//	}
//
//	void testCancelPostedReschedule () {
//		Task task = Timer.schedule(new Task() {
//			int count;
//
//			@Override
//			public void run () {
//				if (++count != 1) throw new RuntimeException("Rescheduled task should only run once.");
//				Gdx.app.log("TimerTest", "testCancelPostedReschedule");
//				testCount++;
//			}
//		}, 0.01f);
//		assertScheduled(task);
//		sleep(200); // Sleep so the task is posted.
//		assertNotScheduled(task);
//		task.cancel(); // The posted task should not execute.
//		assertNotScheduled(task);
//		Timer.schedule(task, 0.01f); // Schedule it again.
//		assertScheduled(task);
//	}
//
//	void testStop () {
//		Timer timer = new Timer();
//		Task task = timer.scheduleTask(new Task() {
//			@Override
//			public void run () {
//				throw new RuntimeException("Stopped timer should not run tasks.");
//			}
//		}, 1);
//		assertScheduled(task);
//		timer.stop();
//		Gdx.app.log("TimerTest", "testStop");
//		testCount++;
//	}
//
//	void testStopStart () {
//		Timer timer = new Timer();
//		Task task = timer.scheduleTask(new Task() {
//			@Override
//			public void run () {
//				Gdx.app.log("TimerTest", "testStopStart");
//				testCount++;
//			}
//		}, 0.200f);
//		assertScheduled(task);
//		timer.stop();
//		sleep(300); // Sleep longer than task delay while stopped.
//		timer.start();
//		sleep(100);
//		assertScheduled(task); // Shouldn't have happened yet.
//	}
//
//	void testDelay () {
//		Timer timer = new Timer();
//		Task task = timer.scheduleTask(new Task() {
//			@Override
//			public void run () {
//				Gdx.app.log("TimerTest", "testDelay");
//				testCount++;
//			}
//		}, 0.200f);
//		assertScheduled(task);
//		timer.delay(200);
//		sleep(300); // Sleep longer than task delay.
//		assertScheduled(task); // Shouldn't have happened yet.
//	}
//
//	void testClear () {
//		Timer timer = new Timer();
//		Task task = timer.scheduleTask(new Task() {
//			@Override
//			public void run () {
//				throw new RuntimeException("Cleared timer should not run tasks.");
//			}
//		}, 0.200f);
//		assertScheduled(task);
//		timer.clear();
//		assertNotScheduled(task);
//		Gdx.app.log("TimerTest", "testClear");
//		testCount++;
//	}
//
//	void assertScheduled (Task task) {
//		if (!task.isScheduled()) throw new RuntimeException("Should be scheduled.");
//	}
//
//	void assertNotScheduled (Task task) {
//		if (task.isScheduled()) throw new RuntimeException("Should not be scheduled.");
//	}
//
//	void sleep (long millis) {
//		try {
//			Thread.sleep(millis);
//		} catch (InterruptedException ignored) {
//		}
//	}
//}
