using SharpGDX.Utils;

namespace SharpGDX.Graphics;

/// <summary>
///     Represents a mouse cursor.
/// </summary>
/// <remarks>
///     <para>
///         Create a cursor via <see cref="IGraphics.newCursor(Pixmap, int, int)" />.
///     </para>
///     <para>
///         To set the cursor use <see cref="IGraphics.setCursor(ICursor)" />.
///     </para>
///     <para>
///         To use one of the system cursors, call <see cref="IGraphics.setSystemCursor(SystemCursor)" />.
///     </para>
/// </remarks>
public interface ICursor : Disposable
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