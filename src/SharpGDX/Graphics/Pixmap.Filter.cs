namespace SharpGDX.Graphics;

public partial class Pixmap
{
    /// <summary>
    ///     Filters to be used with {@link Pixmap#drawPixmap(Pixmap, int, int, int, int, int, int, int, int)}.
    /// </summary>
    public enum Filter
    {
        NearestNeighbour,
        BiLinear
    }
}