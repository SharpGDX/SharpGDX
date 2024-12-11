using SharpGDX.Mathematics;

namespace SharpGDX.Graphics.G3D.Models.Data;

public class ModelTexture
{
    public const int USAGE_UNKNOWN = 0;
    public const int USAGE_NONE = 1;
    public const  int USAGE_DIFFUSE = 2;
    public const int USAGE_EMISSIVE = 3;
    public const int USAGE_AMBIENT = 4;
    public const int USAGE_SPECULAR = 5;
    public const int USAGE_SHININESS = 6;
    public const int USAGE_NORMAL = 7;
    public const int USAGE_BUMP = 8;
    public const int USAGE_TRANSPARENCY = 9;
    public const int USAGE_REFLECTION = 10;
    public string fileName;

    public string id;
    public int usage;
    public Vector2 uvScaling;
    public Vector2 uvTranslation;
}