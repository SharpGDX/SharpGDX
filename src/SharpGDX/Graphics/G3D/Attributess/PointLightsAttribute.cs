using SharpGDX.Utils;
using SharpGDX.Graphics.G3D.Environments;

namespace SharpGDX.Graphics.G3D.Attributess;

/** An {@link Attribute} which can be used to send an {@link Array} of {@link PointLight} instances to the {@link Shader}. The
 * lights are stored by reference, the {@link #copy()} or {@link #PointLightsAttribute(PointLightsAttribute)} method will not
 * create new lights.
 * @author Xoppa */
public class PointLightsAttribute : Attribute {
	public readonly static String Alias = "pointLights";
	public readonly static long Type = register(Alias);

	public static bool @is ( long mask) {
		return (mask & Type) == mask;
	}

	public readonly Array<PointLight> lights;

	public PointLightsAttribute () 
    : base(Type)
    {
		
		lights = new Array<PointLight>(1);
	}

	public PointLightsAttribute ( PointLightsAttribute copyFrom) 
    : this()
    {
		
		lights.addAll(copyFrom.lights);
	}

	public override PointLightsAttribute copy () {
		return new PointLightsAttribute(this);
	}

	public override int GetHashCode () {
		int result = base.GetHashCode();
		foreach (PointLight light in lights)
			result = 1231 * result + (light == null ? 0 : light.GetHashCode());
		return result;
	}

	public override int CompareTo (Attribute o) {
		if (type != o.type) return type < o.type ? -1 : 1;
		return 0; // FIXME implement comparing
	}
}
