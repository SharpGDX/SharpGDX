using System.Diagnostics;

namespace SharpGDX.Utils;

public static class TimeUtils
{
	public static long nanoTime()
	{
		var nano = 10000L * Stopwatch.GetTimestamp();

		nano /= TimeSpan.TicksPerMillisecond;
		nano *= 100L;

		return nano;
	}

	public static long currentTimeMillis()
	{
		return (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds;
	}

	public static long millis()
	{
		return Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
	}
}