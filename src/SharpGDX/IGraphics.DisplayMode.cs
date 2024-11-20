namespace SharpGDX;

public partial interface IGraphics
{
    /// <summary>
    ///     Describes a fullscreen display mode.
    /// </summary>
    public class DisplayMode
    {
        /// <summary>
        ///     The number of bits per pixel, may exclude alpha.
        /// </summary>
        public readonly int BitsPerPixel;

        /// <summary>
        ///     The height in physical pixels.
        /// </summary>
        public readonly int Height;

        /// <summary>
        ///     The refresh rate in Hertz.
        /// </summary>
        public readonly int RefreshRate;

        /// <summary>
        ///     The width in physical pixels.
        /// </summary>
        public readonly int Width;

        protected DisplayMode(int width, int height, int refreshRate, int bitsPerPixel)
        {
            Width = width;
            Height = height;
            RefreshRate = refreshRate;
            BitsPerPixel = bitsPerPixel;
        }

        public override string ToString()
        {
            return $"{Width}x{Height}, bpp: {BitsPerPixel}, hz: {RefreshRate}";
        }
    }
}