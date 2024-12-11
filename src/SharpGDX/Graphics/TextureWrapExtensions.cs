using static SharpGDX.Graphics.Texture;

namespace SharpGDX.Graphics;

public static class TextureWrapExtensions
{
    public static int getGLEnum(this TextureWrap wrap)
    {
        return (int)wrap;
    }
}