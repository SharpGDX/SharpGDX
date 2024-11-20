using SharpGDX.Utils;

namespace SharpGDX.Graphics;

/// <summary>
///     Represents a mouse cursor.
/// </summary>
/// <remarks>
///     <para>
///         Create a cursor via <see cref="IGraphics.NewCursor(Pixmap, int, int)" />.
///     </para>
///     <para>
///         To set the cursor use <see cref="IGraphics.SetCursor(ICursor)" />.
///     </para>
///     <para>
///         To use one of the system cursors, call <see cref="IGraphics.SetSystemCursor(SystemCursor)" />.
///     </para>
/// </remarks>
public interface ICursor : IDisposable
{
	public enum SystemCursor
	{
		Arrow,
		Ibeam,
		Crosshair,
		Hand,
		HorizontalResize,
		VerticalResize,
		NWSEResize,
		NESWResize,
		AllResize,
		NotAllowed,
		None
	}
}