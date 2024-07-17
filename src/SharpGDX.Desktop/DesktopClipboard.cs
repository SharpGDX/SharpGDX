using OpenTK.Windowing.GraphicsLibraryFramework;
using SharpGDX.Utils;

namespace SharpGDX.Desktop;

/// <summary>
///     Clipboard implementation for desktop that uses the system clipboard via GLFW.
/// </summary>
public class DesktopClipboard : IClipboard
{
	/// <inheritdoc cref="IClipboard.GetContents()" />
	public unsafe string? GetContents()
	{
		return GLFW.GetClipboardString(((DesktopGraphics)Gdx.graphics).getWindow().getWindowPtr());
	}

	/// <inheritdoc cref="IClipboard.HasContents" />
	public bool HasContents()
	{
		return !string.IsNullOrEmpty(GetContents());
	}

	/// <inheritdoc cref="IClipboard.SetContents" />
	public unsafe void SetContents(string content)
	{
		GLFW.SetClipboardString(((DesktopGraphics)Gdx.graphics).getWindow().getWindowPtr(), content);
	}
}