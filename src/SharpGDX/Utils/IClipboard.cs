namespace SharpGDX.Utils;

/// <summary>
///     A very simple clipboard interface for text content.
/// </summary>
public interface IClipboard
{
	/// <summary>
	///     Gets the current content of the clipboard if it contains text.
	/// </summary>
	/// <remarks>
	///     For WebGL app, getting the system clipboard is currently not supported.
	/// </remarks>
	/// <returns>The clipboard content or <see langword="null" /></returns>
	public string? GetContents();

	/// <summary>
	///     Check if the clipboard has contents.
	/// </summary>
	/// <remarks>
	///     Recommended to use over getContents() for privacy reasons, if you only want to check if there's something on the
	///     clipboard.
	/// </remarks>
	/// <returns><see langword="true" />, if the clipboard has contents; otherwise <see langword="false" />.</returns>
	public bool HasContents();

	/// <summary>
	///     Sets the content of the system clipboard.
	/// </summary>
	/// <remarks>
	///     For WebGL app, clipboard content might not be set if user denied permission, setting clipboard is not synchronous,
	///     so you cannot rely on getting the same content just after setting it.
	/// </remarks>
	/// <param name="content">The content.</param>
	/// TODO: Should content be made nullable? -RP
	public void SetContents(string content);
}