using System;
using SharpGDX.Graphics.G2D;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Utils;

namespace SharpGDX.Graphics.G3D.Attributess;

public class TextureAttribute : Attribute {
	public readonly static String DiffuseAlias = "diffuseTexture";
	public readonly static long Diffuse = register(DiffuseAlias);
	public readonly static String SpecularAlias = "specularTexture";
	public readonly static long Specular = register(SpecularAlias);
	public readonly static String BumpAlias = "bumpTexture";
	public readonly static long Bump = register(BumpAlias);
	public readonly static String NormalAlias = "normalTexture";
	public readonly static long Normal = register(NormalAlias);
	public readonly static String AmbientAlias = "ambientTexture";
	public readonly static long Ambient = register(AmbientAlias);
	public readonly static String EmissiveAlias = "emissiveTexture";
	public readonly static long Emissive = register(EmissiveAlias);
	public readonly static String ReflectionAlias = "reflectionTexture";
	public readonly static long Reflection = register(ReflectionAlias);

	protected static long Mask = Diffuse | Specular | Bump | Normal | Ambient | Emissive | Reflection;

	public static bool @is (long mask) {
		return (mask & Mask) != 0;
	}

	public static TextureAttribute createDiffuse ( Texture texture) {
		return new TextureAttribute(Diffuse, texture);
	}

	public static TextureAttribute createDiffuse ( TextureRegion region) {
		return new TextureAttribute(Diffuse, region);
	}

	public static TextureAttribute createSpecular ( Texture texture) {
		return new TextureAttribute(Specular, texture);
	}

	public static TextureAttribute createSpecular ( TextureRegion region) {
		return new TextureAttribute(Specular, region);
	}

	public static TextureAttribute createNormal ( Texture texture) {
		return new TextureAttribute(Normal, texture);
	}

	public static TextureAttribute createNormal ( TextureRegion region) {
		return new TextureAttribute(Normal, region);
	}

	public static TextureAttribute createBump ( Texture texture) {
		return new TextureAttribute(Bump, texture);
	}

	public static TextureAttribute createBump ( TextureRegion region) {
		return new TextureAttribute(Bump, region);
	}

	public static TextureAttribute createAmbient ( Texture texture) {
		return new TextureAttribute(Ambient, texture);
	}

	public static TextureAttribute createAmbient ( TextureRegion region) {
		return new TextureAttribute(Ambient, region);
	}

	public static TextureAttribute createEmissive ( Texture texture) {
		return new TextureAttribute(Emissive, texture);
	}

	public static TextureAttribute createEmissive ( TextureRegion region) {
		return new TextureAttribute(Emissive, region);
	}

	public static TextureAttribute createReflection ( Texture texture) {
		return new TextureAttribute(Reflection, texture);
	}

	public static TextureAttribute createReflection ( TextureRegion region) {
		return new TextureAttribute(Reflection, region);
	}

	public readonly TextureDescriptor textureDescription;
	public float offsetU = 0;
	public float offsetV = 0;
	public float scaleU = 1;
	public float scaleV = 1;
	/** The index of the texture coordinate vertex attribute to use for this TextureAttribute. Whether this value is used, depends
	 * on the shader and {@link Attribute#type} value. For basic (model specific) types (e.g. {@link #Diffuse}, {@link #Normal},
	 * etc.), this value is usually ignored and the first texture coordinate vertex attribute is used. */
	public int uvIndex = 0;

	public TextureAttribute ( long type) 
    : base(type)
    {
		
		if (!@is(type)) throw new GdxRuntimeException("Invalid type specified");
		textureDescription = new TextureDescriptor();
	}

	public TextureAttribute ( long type,  TextureDescriptor textureDescription) 
    : this(type)
    {
		
		this.textureDescription.set(textureDescription);
	}

	public  TextureAttribute ( long type,  TextureDescriptor textureDescription, float offsetU,
		float offsetV, float scaleU, float scaleV, int uvIndex) 
    : this(type, textureDescription)
    {
		
		this.offsetU = offsetU;
		this.offsetV = offsetV;
		this.scaleU = scaleU;
		this.scaleV = scaleV;
		this.uvIndex = uvIndex;
	}

	public  TextureAttribute ( long type,  TextureDescriptor textureDescription, float offsetU,
		float offsetV, float scaleU, float scaleV) 
    : this(type, textureDescription, offsetU, offsetV, scaleU, scaleV, 0)
    {
		
	}

	public TextureAttribute ( long type,  Texture texture) 
    : this(type)
    {
		
		textureDescription.texture = texture;
	}

	public TextureAttribute ( long type,  TextureRegion region) 
    : this(type)
    {
		
		set(region);
	}

	public TextureAttribute ( TextureAttribute copyFrom) 
    : this(copyFrom.type, copyFrom.textureDescription, copyFrom.offsetU, copyFrom.offsetV, copyFrom.scaleU, copyFrom.scaleV,
        copyFrom.uvIndex)
    {
		
	}

	public void set ( TextureRegion region) {
		textureDescription.texture = region.getTexture();
		offsetU = region.getU();
		offsetV = region.getV();
		scaleU = region.getU2() - offsetU;
		scaleV = region.getV2() - offsetV;
	}

	public override Attribute copy () {
		return new TextureAttribute(this);
	}

	public override int GetHashCode () {
		int result = base.GetHashCode();
		result = 991 * result + textureDescription.GetHashCode();
		result = 991 * result + NumberUtils.floatToRawIntBits(offsetU);
		result = 991 * result + NumberUtils.floatToRawIntBits(offsetV);
		result = 991 * result + NumberUtils.floatToRawIntBits(scaleU);
		result = 991 * result + NumberUtils.floatToRawIntBits(scaleV);
		result = 991 * result + uvIndex;
		return result;
	}

	public override int CompareTo (Attribute o) {
		if (type != o.type) return type < o.type ? -1 : 1;
		TextureAttribute other = (TextureAttribute)o;
		 int c = textureDescription.CompareTo(other.textureDescription);
		if (c != 0) return c;
		if (uvIndex != other.uvIndex) return uvIndex - other.uvIndex;
		if (!MathUtils.isEqual(scaleU, other.scaleU)) return scaleU > other.scaleU ? 1 : -1;
		if (!MathUtils.isEqual(scaleV, other.scaleV)) return scaleV > other.scaleV ? 1 : -1;
		if (!MathUtils.isEqual(offsetU, other.offsetU)) return offsetU > other.offsetU ? 1 : -1;
		if (!MathUtils.isEqual(offsetV, other.offsetV)) return offsetV > other.offsetV ? 1 : -1;
		return 0;
	}
}
