namespace SharpGDX;

/// <summary>
///     The IApplicationLogger provides an interface for a SharpGDX Application to log messages and exceptions.
/// </summary>
/// <remarks>
///     A default implementations is provided for each backend. Custom implementations can be provided and set using
///     <see cref="IApplication.setApplicationLogger(IApplicationLogger)" />.
/// </remarks>
public interface IApplicationLogger
{
	/// <summary>
	///     Logs a debug message with a tag.
	/// </summary>
	/// <param name="tag">The tag.</param>
	/// <param name="message">The message.</param>
	public void Debug(string tag, string message);

	/// <summary>
	///     Logs a debug message and exception with a tag.
	/// </summary>
	/// <param name="tag">The tag.</param>
	/// <param name="message">The message.</param>
	/// <param name="exception">The exception</param>
	public void Debug(string tag, string message, Exception exception);

	/// <summary>
	///     Logs an error message with a tag.
	/// </summary>
	/// <param name="tag">The tag.</param>
	/// <param name="message">The message.</param>
	public void Error(string tag, string message);

	/// <summary>
	///     Logs an error message and exception with a tag.
	/// </summary>
	/// <param name="tag">The tag.</param>
	/// <param name="message">The message.</param>
	/// <param name="exception">The exception</param>
	public void Error(string tag, string message, Exception exception);

	/// <summary>
	///     Logs a message with a tag
	/// </summary>
	/// <param name="tag">The tag.</param>
	/// <param name="message">The message.</param>
	public void Log(string tag, string message);

	/// <summary>
	///     Logs a message and exception with a tag.
	/// </summary>
	/// <param name="tag">The tag.</param>
	/// <param name="message">The message.</param>
	/// <param name="exception">The exception</param>
	public void Log(string tag, string message, Exception exception);
}