namespace SharpGDX.Graphics;

public sealed partial class VertexAttributes
{
    /// <summary>
    ///     The usage of a vertex attribute.
    /// </summary>
    public static class Usage
    {
        public static readonly int Position = 1;
        public static readonly int ColorUnpacked = 2;
        public static readonly int ColorPacked = 4;
        public static readonly int Normal = 8;
        public static readonly int TextureCoordinates = 16;
        public static readonly int Generic = 32;
        public static readonly int BoneWeight = 64;
        public static readonly int Tangent = 128;
        public static readonly int BiNormal = 256;
    }
}