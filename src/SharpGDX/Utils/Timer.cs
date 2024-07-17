using SharpGDX.Shims;

namespace SharpGDX.Utils
{
	/** Executes tasks in the future on the main loop thread.
 * @author Nathan Sweet */
public class Timer {
	// TimerThread access is synchronized using threadLock.
	// Timer access is synchronized using the Timer instance.
	// Task access is synchronized using the Task instance.

	static readonly Object threadLock = new Object();
	static TimerThread _thread;

	/** Timer instance singleton for general application wide usage. Static methods on {@link Timer} make convenient use of this
	 * instance. */
	static public Timer instance () {
		lock (threadLock) {
			TimerThread thread = Timer.thread();
			if (thread.instance == null) thread.instance = new Timer();
			return thread.instance;
		}
	}

	static private TimerThread thread () {
		lock (threadLock) {
			if (thread == null || _thread.files != Gdx.files) {
				if (thread != null) _thread.dispose();
				_thread = new TimerThread();
			}
			return _thread;
		}
	}

	readonly Array<Task> tasks = new (false, 8);

	public Timer () {
		start();
	}

	/** Schedules a task to occur once as soon as possible, but not sooner than the start of the next frame. */
	public Task postTask (Task task) {
		return scheduleTask(task, 0, 0, 0);
	}

	/** Schedules a task to occur once after the specified delay. */
	public Task scheduleTask (Task task, float delaySeconds) {
		return scheduleTask(task, delaySeconds, 0, 0);
	}

	/** Schedules a task to occur once after the specified delay and then repeatedly at the specified interval until cancelled. */
	public Task scheduleTask (Task task, float delaySeconds, float intervalSeconds) {
		return scheduleTask(task, delaySeconds, intervalSeconds, -1);
	}

	/** Schedules a task to occur once after the specified delay and then a number of additional times at the specified interval.
	 * @param repeatCount If negative, the task will repeat forever. */
	public Task scheduleTask (Task task, float delaySeconds, float intervalSeconds, int repeatCount) {
		throw new NotImplementedException();
			//lock (threadLock) {
			//	lock (this) {
			//		lock (task) {
			//			if (task.timer != null) throw new IllegalArgumentException("The same task may not be scheduled twice.");
			//			task.timer = this;
			//			long timeMillis = TimeUtils.nanoTime() / 1000000;
			//			long executeTimeMillis = timeMillis + (long)(delaySeconds * 1000);
			//			if (_thread.pauseTimeMillis > 0) executeTimeMillis -= timeMillis - _thread.pauseTimeMillis;
			//			task.executeTimeMillis = executeTimeMillis;
			//			task.intervalMillis = (long)(intervalSeconds * 1000);
			//			task.repeatCount = repeatCount;
			//			tasks.add(task);
			//		}
			//	}
			//	threadLock.notifyAll();
			//}
			//return task;
		}

	/** Stops the timer, tasks will not be executed and time that passes will not be applied to the task delays. */
	public void stop () {
		lock (threadLock) {
			thread().instances.removeValue(this, true);
		}
	}

	/** Starts the timer if it was stopped. */
	public void start () {
		throw new NotImplementedException();
		//	lock (threadLock) {
		//	TimerThread thread = thread();
		//	Array<Timer> instances = thread.instances;
		//	if (instances.contains(this, true)) return;
		//	instances.add(this);
		//	threadLock.notifyAll();
		//}
	}

	/** Cancels all tasks. */
	public void clear () {
		lock (this)
		{
			for (int i = 0, n = tasks.size; i < n; i++)
			{
				Task task = tasks.get(i);
				lock (task) {
					task.executeTimeMillis = 0;
					task.timer = null;
				}
			}

			tasks.clear();
		}
	}

	/** Returns true if the timer has no tasks in the queue. Note that this can change at any time. Synchronize on the timer
	 * instance to prevent tasks being added, removed, or updated. */
	public  bool isEmpty () {
		lock(this)
		return tasks.size == 0;
	}

	long update (long timeMillis, long waitMillis) {
		throw new NotImplementedException();
		//	lock (this)
		//{
		//	for (int i = 0, n = tasks.size; i < n; i++)
		//	{
		//		Task task = tasks.get(i);
		//		lock (task) {
		//			if (task.executeTimeMillis > timeMillis)
		//			{
		//				waitMillis = Math.Min(waitMillis, task.executeTimeMillis - timeMillis);
		//				continue;
		//			}

		//			if (task.repeatCount == 0)
		//			{
		//				task.timer = null;
		//				tasks.removeIndex(i);
		//				i--;
		//				n--;
		//			}
		//			else
		//			{
		//				task.executeTimeMillis = timeMillis + task.intervalMillis;
		//				waitMillis = Math.Min(waitMillis, task.intervalMillis);
		//				if (task.repeatCount > 0) task.repeatCount--;
		//			}

		//			task.app.postRunnable(task);
		//		}
		//	}

		//	return waitMillis;
		//}
	}

	/** Adds the specified delay to all tasks. */
	public void delay (long delayMillis) {
		lock (this)
		{
			for (int i = 0, n = tasks.size; i < n; i++)
			{
				Task task = tasks.get(i);
				lock (task) {
					task.executeTimeMillis += delayMillis;
				}
			}
		}
	}

	/** Schedules a task on {@link #instance}.
	 * @see #postTask(Task) */
	static public Task post (Task task) {
		return instance().postTask(task);
	}

	/** Schedules a task on {@link #instance}.
	 * @see #scheduleTask(Task, float) */
	static public Task schedule (Task task, float delaySeconds) {
		return instance().scheduleTask(task, delaySeconds);
	}

	/** Schedules a task on {@link #instance}.
	 * @see #scheduleTask(Task, float, float) */
	static public Task schedule (Task task, float delaySeconds, float intervalSeconds) {
		return instance().scheduleTask(task, delaySeconds, intervalSeconds);
	}

	/** Schedules a task on {@link #instance}.
	 * @see #scheduleTask(Task, float, float, int) */
	static public Task schedule (Task task, float delaySeconds, float intervalSeconds, int repeatCount) {
		return instance().scheduleTask(task, delaySeconds, intervalSeconds, repeatCount);
	}

	/** Runnable that can be scheduled on a {@link Timer}.
	 * @author Nathan Sweet */
	abstract public class Task // TODO: : Runnable 
	{
		internal readonly IApplication app;
		internal long executeTimeMillis, intervalMillis;
		internal int repeatCount;
		internal volatile Timer timer;

		public Task () {
			app = Gdx.app; // Store which app to postRunnable (eg for multiple LwjglAWTCanvas).
			if (app == null) throw new IllegalStateException("Gdx.app not available.");
		}

		/** If this is the last time the task will be ran or the task is first cancelled, it may be scheduled again in this
		 * method. */
		abstract public void run ();

		/** Cancels the task. It will not be executed until it is scheduled again. This method can be called at any time. */
		public void cancel () {
			Timer timer = this.timer;
			if (timer != null) {
				lock (timer) {
					lock (this) {
						executeTimeMillis = 0;
						this.timer = null;
						timer.tasks.removeValue(this, true);
					}
				}
			} else {
				lock (this) {
					executeTimeMillis = 0;
					this.timer = null;
				}
			}
		}

		/** Returns true if this task is scheduled to be executed in the future by a timer. The execution time may be reached at any
		 * time after calling this method, which may change the scheduled state. To prevent the scheduled state from changing,
		 * synchronize on this task object, eg:
		 * 
		 * <pre>
		 * synchronized (task) {
		 * 	if (!task.isScheduled()) { ... }
		 * }
		 * </pre>
		 */
		public bool isScheduled () {
			return timer != null;
		}

		/** Returns the time in milliseconds when this task will be executed next. */
		public long getExecuteTimeMillis () {
			lock (this)
			{
				return executeTimeMillis;
			}
		}
	}

	/** Manages a single thread for updating timers. Uses libgdx application events to pause, resume, and dispose the thread.
	 * @author Nathan Sweet */
	 class TimerThread : // TODO:  Runnable,
		ILifecycleListener {
		internal readonly IFiles files;
		readonly IApplication app;
		internal readonly Array<Timer> instances = new(1);
		internal Timer instance;
		internal long pauseTimeMillis;

		public TimerThread () {
			throw new NotImplementedException();
				//files = Gdx.files;
				//app = Gdx.app;
				//app.addLifecycleListener(this);
				//resume();

				//Thread thread = new Thread(this, "Timer");
				//thread.IsBackground =(true);
				//thread.Start();
			}

		public void run () {
			throw new NotImplementedException();
				//while (true) {
				//	lock (threadLock) {
				//		if (_thread != this || files != Gdx.files) break;

				//		long waitMillis = 5000;
				//		if (pauseTimeMillis == 0) {
				//			long timeMillis = TimeUtils.nanoTime() / 1000000;
				//			for (int i = 0, n = instances.size; i < n; i++) {
				//				try {
				//					waitMillis = instances.get(i).update(timeMillis, waitMillis);
				//				} catch (Exception ex) {
				//					throw new GdxRuntimeException("Task failed: " + instances.get(i).GetType().Name, ex);
				//				}
				//			}
				//		}

				//		if (_thread != this || files != Gdx.files) break;

				//		try {
				//			if (waitMillis > 0) threadLock.wait(waitMillis);
				//		} catch (ThreadInterruptedException ignored) {
				//		}
				//	}
				//}
				//dispose();
			}

		public void resume () {
			throw new NotImplementedException();
				//lock (threadLock) {
				//	long delayMillis = TimeUtils.nanoTime() / 1000000 - pauseTimeMillis;
				//	for (int i = 0, n = instances.size; i < n; i++)
				//		instances.get(i).delay(delayMillis);
				//	pauseTimeMillis = 0;
				//	threadLock.notifyAll();
				//}
			}

		public void pause () {
			throw new NotImplementedException();
				//lock (threadLock) {
				//	pauseTimeMillis = TimeUtils.nanoTime() / 1000000;
				//	threadLock.notifyAll();
				//}
			}

		public void dispose () { // OK to call multiple times.
			throw new NotImplementedException();
				//lock (threadLock) {
				//	if (_thread == this) _thread = null;
				//	instances.clear();
				//	threadLock.notifyAll();
				//}
				//app.removeLifecycleListener(this);
			}
	}
}
}
