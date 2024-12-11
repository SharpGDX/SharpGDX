using System;
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
using SharpGDX.Graphics.G3D.Environments;

namespace SharpGDX.Graphics.G3D.Attributess;

/** An {@link Attribute} which can be used to send an {@link Array} of {@link SpotLight} instances to the {@link Shader}. The
 * lights are stored by reference, the {@link #copy()} or {@link #SpotLightsAttribute(SpotLightsAttribute)} method will not create
 * new lights.
 * @author Xoppa */
public class SpotLightsAttribute : Attribute {
	public readonly static String Alias = "spotLights";
	public readonly static long Type = register(Alias);

	public static bool @is (long mask) {
		return (mask & Type) == mask;
	}

	public readonly Array<SpotLight> lights;

	public SpotLightsAttribute () 
    : base(Type)
    {
		
		lights = new Array<SpotLight>(1);
	}

	public SpotLightsAttribute ( SpotLightsAttribute copyFrom) 
    : this()
    {
		
		lights.addAll(copyFrom.lights);
	}

	public override SpotLightsAttribute copy () {
		return new SpotLightsAttribute(this);
	}

	public override int GetHashCode () {
		int result = base.GetHashCode();
		foreach (SpotLight light in lights)
			result = 1237 * result + (light == null ? 0 : light.GetHashCode());
		return result;
	}

	public override int CompareTo (Attribute o) {
		if (type != o.type) return type < o.type ? -1 : 1;
		return 0; // FIXME implement comparing
	}
}
