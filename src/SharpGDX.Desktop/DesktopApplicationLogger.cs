namespace SharpGDX.Desktop;

/// <summary>
///     Default implementation of <see cref="IApplicationLogger" /> for headless.
/// </summary>
public class DesktopApplicationLogger : IApplicationLogger
{
	/// <inheritdoc cref="IApplicationLogger.Debug(string, string)" />
	public void Debug(string tag, string message)
	{
		Console.WriteLine("[" + tag + "] " + message);
	}

	/// <inheritdoc cref="IApplicationLogger.Debug(string, string, Exception)" />
	public void Debug(string tag, string message, Exception exception)
	{
		Console.WriteLine("[" + tag + "] " + message);
		Console.WriteLine(exception.StackTrace);
	}

	/// <inheritdoc cref="IApplicationLogger.Error(string, string)" />
	public void Error(string tag, string message)
	{
		Console.WriteLine("[" + tag + "] " + message);
	}

	/// <inheritdoc cref="IApplicationLogger.Error(string, string, Exception)" />
	public void Error(string tag, string message, Exception exception)
	{
		Console.WriteLine("[" + tag + "] " + message);
		Console.WriteLine(exception.StackTrace);
	}

	/// <inheritdoc cref="IApplicationLogger.Log(string, string)" />
	public void Log(string tag, string message)
	{
		Console.WriteLine("[" + tag + "] " + message);
	}

	/// <inheritdoc cref="IApplicationLogger.Log(string, string, Exception)" />
	public void Log(string tag, string message, Exception exception)
	{
		Console.WriteLine("[" + tag + "] " + message);
		Console.WriteLine(exception.StackTrace);
	}
}