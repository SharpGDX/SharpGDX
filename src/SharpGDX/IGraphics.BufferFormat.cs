namespace SharpGDX;

public partial interface IGraphics
{
    /// <summary>
    ///     Class describing the bits per pixel, depth buffer precision, stencil precision and number of MSAA samples.
    /// </summary>
    class BufferFormat(int r, int g, int b, int a, int depth, int stencil, int samples, bool coverageSampling)
    {
        /// <summary>
        ///     Whether coverage sampling antialiasing is used. in that case you have to clear the coverage buffer as well!
        /// </summary>
        public readonly bool CoverageSampling = coverageSampling;

        /// <summary>
        ///     Number of bits for depth and stencil buffer.
        /// </summary>
        public readonly int Depth = depth, Stencil = stencil;

        /// <summary>
        ///     Number of bits per color channel.
        /// </summary>
        public readonly int R = r, G = g, B = b, A = a;

        /// <summary>
        ///     Number of samples for multi-sample antialiasing (MSAA).
        /// </summary>
        public readonly int Samples = samples;

        public override string ToString()
        {
            return
                $"r: {R}, g: {G}, b: {B}, a: {A}, depth: {Depth}, stencil: {Stencil}, num samples: {Samples}, coverage sampling: {CoverageSampling}";
        }
    }
}