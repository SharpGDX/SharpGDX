using SharpGDX.Utils;
using SharpGDX.Graphics.G3D.Utils;

namespace SharpGDX.Graphics.G3D.Attributess;

public class CubemapAttribute : Attribute {
	public readonly static String EnvironmentMapAlias = "environmentCubemap";
	public readonly static long EnvironmentMap = register(EnvironmentMapAlias);

	protected static long Mask = EnvironmentMap;

	public static bool @is ( long mask) {
		return (mask & Mask) != 0;
	}

	public readonly TextureDescriptor textureDescription;

	public CubemapAttribute ( long type) 
    : base(type)
    {
		
		if (!@is(type)) throw new GdxRuntimeException("Invalid type specified");
		textureDescription = new TextureDescriptor();
	}

	public CubemapAttribute ( long type,  TextureDescriptor textureDescription) 
    : this(type)
    {
		
		this.textureDescription.set(textureDescription);
	}

	public CubemapAttribute ( long type,  Cubemap texture) 
    : this(type)
    {
		
		textureDescription.texture = texture;
	}

	public CubemapAttribute ( CubemapAttribute copyFrom) 
    : this(copyFrom.type, copyFrom.textureDescription)
    {
		
	}

	public override Attribute copy () {
		return new CubemapAttribute(this);
	}

	public override int GetHashCode () {
		int result = base.GetHashCode();
		result = 967 * result + textureDescription.GetHashCode();
		return result;
	}

	public override int CompareTo (Attribute o) {
		if (type != o.type) return (int)(type - o.type);
		return textureDescription.CompareTo(((CubemapAttribute)o).textureDescription);
	}
}
