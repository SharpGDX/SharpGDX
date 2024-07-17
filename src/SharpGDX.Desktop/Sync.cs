using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SharpGDX.Desktop;

internal class Sync
{
	private const long NanosInSecond = 1000L * 1000L * 1000L;

	private readonly RunningAverage _sleepDurations = new(10);

	private readonly RunningAverage _yieldDurations = new(10);

	private bool _initialized;

	private long _nextFrame;

	public void SyncTo(int fps)
	{
		if (fps <= 0)
		{
			return;
		}

		if (!_initialized)
		{
			Initialize();
		}

		try
		{
			for (long t0 = GetTime(), t1; _nextFrame - t0 > _sleepDurations.Average(); t0 = t1)
			{
				Thread.Sleep(1);
				_sleepDurations.Add((t1 = GetTime()) - t0);
			}

			_sleepDurations.DampenForLowResTicker();

			for (long t0 = GetTime(), t1; _nextFrame - t0 > _yieldDurations.Average(); t0 = t1)
			{
				Thread.Yield();
				_yieldDurations.Add((t1 = GetTime()) - t0);
			}
		}
		catch (ThreadInterruptedException)
		{
			// ignored
		}

		_nextFrame = Math.Max(_nextFrame + NanosInSecond / fps, GetTime());
	}

	private long GetTime()
	{
		return (long)(GLFW.GetTime() * NanosInSecond);
	}

	private void Initialize()
	{
		_initialized = true;

		_sleepDurations.Init(1000 * 1000);
		_yieldDurations.Init((int)(-(GetTime() - GetTime()) * 1.333));

		_nextFrame = GetTime();

		if (!OperatingSystem.IsWindows())
		{
			return;
		}

		var timerAccuracyThread = new Thread
		(() =>
			{
				try
				{
					Thread.Sleep(int.MaxValue);
				}
				catch (Exception)
				{
					// ignored
				}
			}
		)
		{
			Name = "SharpGDX Timer",
			IsBackground = true
		};

		timerAccuracyThread.Start();
	}

	private class RunningAverage(int slotCount)
	{
		private static readonly float DampenFactor = 0.9f;
		private static readonly long DampenThreshold = 10 * 1000L * 1000L;

		private readonly long[] _slots = new long[slotCount];

		private int _offset;

		public void Add(long value)
		{
			_slots[_offset++ % _slots.Length] = value;
			_offset %= _slots.Length;
		}

		public long Average()
		{
			long sum = 0;

			foreach (var slot in _slots)
			{
				sum += slot;
			}

			return sum / _slots.Length;
		}

		public void DampenForLowResTicker()
		{
			if (Average() <= DampenThreshold)
			{
				return;
			}

			for (var i = 0; i < _slots.Length; i++)
			{
				_slots[i] = (long)(_slots[i] * DampenFactor);
			}
		}

		public void Init(long value)
		{
			while (_offset < _slots.Length)
			{
				_slots[_offset++] = value;
			}
		}
	}
}