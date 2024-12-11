using SharpGDX.Utils;

namespace SharpGDX.Graphics.G3D.Attributess;

public class ColorAttribute : Attribute {
	public readonly static String DiffuseAlias = "diffuseColor";
	public readonly static long Diffuse = register(DiffuseAlias);
	public readonly static String SpecularAlias = "specularColor";
	public readonly static long Specular = register(SpecularAlias);
	public readonly static String AmbientAlias = "ambientColor";
	public static readonly long Ambient = register(AmbientAlias);
	public readonly static String EmissiveAlias = "emissiveColor";
	public static readonly long Emissive = register(EmissiveAlias);
	public readonly static String ReflectionAlias = "reflectionColor";
	public static readonly long Reflection = register(ReflectionAlias);
	public readonly static String AmbientLightAlias = "ambientLightColor";
	public static readonly long AmbientLight = register(AmbientLightAlias);
	public readonly static String FogAlias = "fogColor";
	public static readonly long Fog = register(FogAlias);

	protected static long Mask = Ambient | Diffuse | Specular | Emissive | Reflection | AmbientLight | Fog;

	public static bool @is ( long mask) {
		return (mask & Mask) != 0;
	}

	public static ColorAttribute createAmbient (Color color) {
		return new ColorAttribute(Ambient, color);
	}

	public  static ColorAttribute createAmbient (float r, float g, float b, float a) {
		return new ColorAttribute(Ambient, r, g, b, a);
	}

	public  static ColorAttribute createDiffuse ( Color color) {
		return new ColorAttribute(Diffuse, color);
	}

	public  static ColorAttribute createDiffuse (float r, float g, float b, float a) {
		return new ColorAttribute(Diffuse, r, g, b, a);
	}

	public  static ColorAttribute createSpecular ( Color color) {
		return new ColorAttribute(Specular, color);
	}

	public  static ColorAttribute createSpecular (float r, float g, float b, float a) {
		return new ColorAttribute(Specular, r, g, b, a);
	}

	public  static ColorAttribute createReflection ( Color color) {
		return new ColorAttribute(Reflection, color);
	}

	public  static ColorAttribute createReflection (float r, float g, float b, float a) {
		return new ColorAttribute(Reflection, r, g, b, a);
	}

	public  static ColorAttribute createEmissive ( Color color) {
		return new ColorAttribute(Emissive, color);
	}

	public  static ColorAttribute createEmissive (float r, float g, float b, float a) {
		return new ColorAttribute(Emissive, r, g, b, a);
	}

	public  static ColorAttribute createAmbientLight ( Color color) {
		return new ColorAttribute(AmbientLight, color);
	}

	public  static ColorAttribute createAmbientLight (float r, float g, float b, float a) {
		return new ColorAttribute(AmbientLight, r, g, b, a);
	}

	public  static ColorAttribute createFog ( Color color) {
		return new ColorAttribute(Fog, color);
	}

	public  static ColorAttribute createFog (float r, float g, float b, float a) {
		return new ColorAttribute(Fog, r, g, b, a);
	}

	public readonly Color color = new Color();

	public ColorAttribute ( long type) 
    : base(type)
    {
		
		if (!@is(type)) throw new GdxRuntimeException("Invalid type specified");
	}

	public ColorAttribute ( long type,  Color color) 
    : this(type)
    {
		
		if (color != null) this.color.Set(color);
	}

	public ColorAttribute ( long type, float r, float g, float b, float a) 
    : this(type)
    {
		
		this.color.Set(r, g, b, a);
	}

	public ColorAttribute ( ColorAttribute copyFrom) 
    : this(copyFrom.type, copyFrom.color)
    {
		
	}

	public override Attribute copy () {
		return new ColorAttribute(this);
	}

	public override int GetHashCode () {
		int result = base.GetHashCode();
		result = 953 * result + color.ToIntBits();
		return result;
	}

	public override int CompareTo (Attribute o) {
		if (type != o.type) return (int)(type - o.type);
		return ((ColorAttribute)o).color.ToIntBits() - color.ToIntBits();
	}
}
