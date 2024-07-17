namespace SharpGDX.Headless;

/// <summary>
///     Default implementation of <see cref="IApplicationLogger" /> for headless.
/// </summary>
public class HeadlessApplicationLogger : IApplicationLogger
{
	public void Debug(string tag, string message)
	{
		Console.WriteLine("[" + tag + "] " + message);
	}

	public void Debug(string tag, string message, Exception exception)
	{
		Console.WriteLine("[" + tag + "] " + message);
		Console.WriteLine(exception.StackTrace);
	}

	public void Error(string tag, string message)
	{
		Console.WriteLine("[" + tag + "] " + message);
	}

	public void Error(string tag, string message, Exception exception)
	{
		Console.WriteLine("[" + tag + "] " + message);
		Console.WriteLine(exception.StackTrace);
	}

	public void Log(string tag, string message)
	{
		Console.WriteLine("[" + tag + "] " + message);
	}

	public void Log(string tag, string message, Exception exception)
	{
		Console.WriteLine("[" + tag + "] " + message);
		Console.WriteLine(exception.StackTrace);
	}
}