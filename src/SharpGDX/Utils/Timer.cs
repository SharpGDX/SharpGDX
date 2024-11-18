using SharpGDX.Shims;

namespace SharpGDX.Utils
{
    /// <summary>
    /// Executes tasks in the future on the main loop thread.
    /// </summary>
    /// TODO: This needs heavy testing.
    public class Timer
    {
        // TimerThread access is synchronized using threadLock.
        // Timer access is synchronized using the Timer instance.
        // Task access is synchronized using the Task instance.
        // Posted tasks are synchronized using TimerThread#postedTasks.

        static readonly Object threadLock = new Object();
        static TimerThread _thread;

        /** Timer instance singleton for general application wide usage. Static methods on {@link Timer} make convenient use of this
         * instance. */
        static public Timer instance()
        {
            lock (threadLock)
            {
                TimerThread _thread = thread();
                if (_thread.instance == null) _thread.instance = new Timer();
                return _thread.instance;
            }
        }

        static private TimerThread thread()
        {
            lock (threadLock)
            {
                if (_thread == null || _thread.files != Gdx.Files)
                {
                    if (_thread != null) _thread.Dispose();
                    _thread = new TimerThread();
                }

                return _thread;
            }
        }

        readonly SharpGDX.Utils.Array<Task> tasks = new (false, 8);
        long stopTimeMillis;

        public Timer()
        {
            start();
        }

        /** Schedules a task to occur once as soon as possible, but not sooner than the start of the next frame. */
        public Task postTask(Task task)
        {
            return scheduleTask(task, 0, 0, 0);
        }

        /** Schedules a task to occur once after the specified delay. */
        public Task scheduleTask(Task task, float delaySeconds)
        {
            return scheduleTask(task, delaySeconds, 0, 0);
        }

        /** Schedules a task to occur once after the specified delay and then repeatedly at the specified interval until cancelled. */
        public Task scheduleTask(Task task, float delaySeconds, float intervalSeconds)
        {
            return scheduleTask(task, delaySeconds, intervalSeconds, -1);
        }

        /** Schedules a task to occur once after the specified delay and then a number of additional times at the specified interval.
         * @param repeatCount If negative, the task will repeat forever. */
        public Task scheduleTask(Task task, float delaySeconds, float intervalSeconds, int repeatCount)
        {
            try
            {

                Monitor.Enter(threadLock);

                lock (this)
                {
                    lock (task)
                    {
                        if (task.timer != null)
                            throw new IllegalArgumentException("The same task may not be scheduled twice.");
                        task.timer = this;
                        long timeMillis = TimeUtils.nanoTime() / 1000000;
                        long executeTimeMillis = timeMillis + (long)(delaySeconds * 1000);
                        if (_thread.pauseTimeMillis > 0) executeTimeMillis -= timeMillis - _thread.pauseTimeMillis;
                        task.executeTimeMillis = executeTimeMillis;
                        task.intervalMillis = (long)(intervalSeconds * 1000);
                        task.repeatCount = repeatCount;
                        tasks.Add(task);
                    }
                }

                Monitor.PulseAll(threadLock);

                return task;
            }
            finally
            {
                Monitor.Exit(threadLock);
            }
        }

        /** Stops the timer if it was started. Tasks will not be executed while stopped. */
        public void stop()
        {
            lock (threadLock)
            {
                if (thread().instances.RemoveValue(this, true)) stopTimeMillis = TimeUtils.nanoTime() / 1000000;
            }
        }

        /** Starts the timer if it was stopped. Tasks are delayed by the time passed while stopped. */
        public void start()
        {
            try
            {
                Monitor.Enter(threadLock);
                TimerThread _thread = thread();
                Array<Timer> instances = _thread.instances;
                if (instances.contains(this, true)) return;
                instances.Add(this);
                if (stopTimeMillis > 0)
                {
                    delay(TimeUtils.nanoTime() / 1000000 - stopTimeMillis);
                    stopTimeMillis = 0;
                }

                Monitor.PulseAll(threadLock);
            }
            finally
            {
                Monitor.Exit(threadLock);
            }
                
        }

        /** Cancels all tasks. */
        public void clear()
        {
            lock (threadLock)
            {
                TimerThread _thread = thread();
                lock (this)
                {
                    lock (_thread.postedTasks)
                    {
                        for (int i = 0, n = tasks.size; i < n; i++)
                        {
                            Task task = tasks.Get(i);
                            _thread.removePostedTask(task);
                            task.reset();
                        }
                    }

                    tasks.clear();
                }
            }
        }

        /** Returns true if the timer has no tasks in the queue. Note that this can change at any time. Synchronize on the timer
         * instance to prevent tasks being added, removed, or updated. */
        public bool isEmpty()
        {
            lock (this)
            {
                return tasks.size == 0;
            }
        }

        long update(TimerThread _thread, long timeMillis, long waitMillis)
        {
            lock (this)
            {
                for (int i = 0, n = tasks.size; i < n; i++)
                {
                    Task task = tasks.Get(i);
                    lock (task)
                    {
                        if (task.executeTimeMillis > timeMillis)
                        {
                            waitMillis = Math.Min(waitMillis, task.executeTimeMillis - timeMillis);
                            continue;
                        }

                        if (task.repeatCount == 0)
                        {
                            task.timer = null;
                            tasks.RemoveIndex(i);
                            i--;
                            n--;
                        }
                        else
                        {
                            task.executeTimeMillis = timeMillis + task.intervalMillis;
                            waitMillis = Math.Min(waitMillis, task.intervalMillis);
                            if (task.repeatCount > 0) task.repeatCount--;
                        }

                        _thread.addPostedTask(task);
                    }
                }

                return waitMillis;
            }
        }

        /** Adds the specified delay to all tasks. */
        public void delay(long delayMillis)
        {
            lock (this)
            {
                for (int i = 0, n = tasks.size; i < n; i++)
                {
                    Task task = tasks.Get(i);
                    lock (task)
                    {
                        task.executeTimeMillis += delayMillis;
                    }
                }
            }
        }

        /** Schedules a task on {@link #instance}.
         * @see #postTask(Task) */
        static public Task post(Task task)
        {
            return instance().postTask(task);
        }

        /** Schedules a task on {@link #instance}.
         * @see #scheduleTask(Task, float) */
        static public Task schedule(Task task, float delaySeconds)
        {
            return instance().scheduleTask(task, delaySeconds);
        }

        /** Schedules a task on {@link #instance}.
         * @see #scheduleTask(Task, float, float) */
        static public Task schedule(Task task, float delaySeconds, float intervalSeconds)
        {
            return instance().scheduleTask(task, delaySeconds, intervalSeconds);
        }

        /** Schedules a task on {@link #instance}.
         * @see #scheduleTask(Task, float, float, int) */
        static public Task schedule(Task task, float delaySeconds, float intervalSeconds, int repeatCount)
        {
            return instance().scheduleTask(task, delaySeconds, intervalSeconds, repeatCount);
        }

        /** Runnable that can be scheduled on a {@link Timer}.
         * @author Nathan Sweet */
        abstract public class Task
        {
            internal readonly IApplication app;

            internal long executeTimeMillis, intervalMillis;
            internal int repeatCount;
            internal volatile Timer timer;

            public Task()
            {
                app = Gdx.App; // Store which app to postRunnable (eg for multiple LwjglAWTCanvas).
                if (app == null) throw new IllegalStateException("Gdx.app not available.");
            }

            /** If this is the last time the task will be ran or the task is first cancelled, it may be scheduled again in this
             * method. */
            abstract public void run();

            /** Cancels the task. It will not be executed until it is scheduled again. This method can be called at any time. */
            public void cancel()
            {
                lock (threadLock)
                {
                    thread().removePostedTask(this);
                    Timer timer = this.timer;
                    if (timer != null)
                    {
                        lock (timer)
                        {
                            timer.tasks.RemoveValue(this, true);
                            reset();
                        }
                    }
                    else
                        reset();
                }
            }

            internal void reset()
            {
                lock (this)
                {
                    executeTimeMillis = 0;
                    this.timer = null;
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
            public bool isScheduled()
            {
                return timer != null;
            }

            /** Returns the time in milliseconds when this task will be executed next. */
            public long getExecuteTimeMillis()
            {
                lock (this)
                {
                    return executeTimeMillis;
                }
            }
        }

        /** Manages a single thread for updating timers. Uses libgdx application events to pause, resume, and dispose the thread.
         * @author Nathan Sweet */
        class TimerThread : ILifecycleListener
        {
            internal readonly IFiles files;
            readonly IApplication app;
            internal readonly Array<Timer> instances = new (1);
            internal Timer instance;
            internal long pauseTimeMillis;

            internal readonly Array<Task> postedTasks = new (2);
            readonly Array<Task> runTasks = new (2);
            private readonly Runnable _runPostedTasks;

            public TimerThread()
            {
                _runPostedTasks = runPostedTasks;

                files = Gdx.Files;
                app = Gdx.App;
                app.AddLifecycleListener(this);
                Resume();

                Thread _thread = new Thread(run)
                {
                    Name = "Timer",
                    IsBackground = true
                };

                _thread.Start();
            }

            public void run()
            {
                while (true)
                {
                    try
                    {
                        Monitor.Enter(threadLock);
                        
                            if (_thread != this || files != Gdx.Files) break;

                            long waitMillis = 5000;
                            if (pauseTimeMillis == 0)
                            {
                                long timeMillis = TimeUtils.nanoTime() / 1000000;
                                for (int i = 0, n = instances.size; i < n; i++)
                                {
                                    try
                                    {
                                        waitMillis = instances.Get(i).update(this, timeMillis, waitMillis);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new GdxRuntimeException(
                                            "Task failed: " + instances.Get(i).GetType().Name, ex);
                                    }
                                }
                            }

                            if (_thread != this || files != Gdx.Files) break;

                            try
                            {
                                if (waitMillis > 0) Monitor.Wait(threadLock, TimeSpan.FromMilliseconds(waitMillis));
                            }
                            catch (ThreadInterruptedException ignored)
                            {
                            }
                    }
                    finally
                    {
                        Monitor.Exit(threadLock);
                    }
                    
                }

                Dispose();
            }

            void runPostedTasks()
            {
                lock (postedTasks)
                {
                    runTasks.addAll(postedTasks);
                    postedTasks.clear();
                }

                Object[] items = runTasks.items;
                for (int i = 0, n = runTasks.size; i < n; i++)
                    ((Task)items[i]).run();
                runTasks.clear();
            }

            internal void addPostedTask(Task task)
            {
                lock (postedTasks)
                {
                    if (postedTasks.isEmpty()) task.app.PostRunnable(_runPostedTasks);
                    postedTasks.Add(task);
                }
            }

            internal void removePostedTask(Task task)
            {
                lock (postedTasks)
                {
                    Object[] items = postedTasks.items;
                    for (int i = postedTasks.size - 1; i >= 0; i--)
                        if (items[i] == task)
                            postedTasks.RemoveIndex(i);
                }
            }

            public void Resume()
            {
                try
                {
                    Monitor.Enter(threadLock);
                    long delayMillis = TimeUtils.nanoTime() / 1000000 - pauseTimeMillis;
                    for (int i = 0, n = instances.size; i < n; i++)
                        instances.Get(i).delay(delayMillis);
                    pauseTimeMillis = 0;
                    Monitor.PulseAll(threadLock);
                }
                finally
                {
                    Monitor.Exit(threadLock);
                }
            }

            public void Pause()
            {
                try
                {
                    Monitor.Enter(threadLock);
                    pauseTimeMillis = TimeUtils.nanoTime() / 1000000;
                    Monitor.PulseAll(threadLock);
                }
                finally
                {
                    Monitor.Exit(threadLock);
                }
            }

            public void Dispose()
            {
                // OK to call multiple times.
                try
                {
                    Monitor.Enter(threadLock);
                    lock (postedTasks)
                    {
                        postedTasks.clear();
                    }

                    if (_thread == this) _thread = null;
                    instances.clear();
                    Monitor.PulseAll(threadLock);
                }
                finally
                {
                    Monitor.Exit(threadLock);
                }

                app.RemoveLifecycleListener(this);
            }
        }
    }
}

