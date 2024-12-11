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
using SharpGDX.Graphics.G3D.Utils;


namespace SharpGDX.Graphics.G3D.Environments;

public class SphericalHarmonics {
	// <kalle_h> last term is no x*x * y*y but x*x - y*y
	private readonly static float[] coeff = {0.282095f, 0.488603f, 0.488603f, 0.488603f, 1.092548f, 1.092548f, 1.092548f, 0.315392f,
		0.546274f};

	private  static float clamp ( float v) {
		return v < 0f ? 0f : (v > 1f ? 1f : v);
	}

	public readonly float[] data;

	public SphericalHarmonics () {
		data = new float[9 * 3];
	}

	public SphericalHarmonics ( float[] copyFrom) {
		if (copyFrom.Length != (9 * 3)) throw new GdxRuntimeException("Incorrect array size");
		// TODO: this cast is kind of crap, but /shrug. -RP
		data = (float[])copyFrom.Clone();
	}

	public SphericalHarmonics set ( float[] values) {
		for (int i = 0; i < data.Length; i++)
			data[i] = values[i];
		return this;
	}

	public SphericalHarmonics set ( AmbientCubemap other) {
		return set(other.data);
	}

	public SphericalHarmonics set ( Color color) {
		return set(color.R, color.G, color.B);
	}

	public SphericalHarmonics set (float r, float g, float b) {
		for (int idx = 0; idx < data.Length;) {
			data[idx++] = r;
			data[idx++] = g;
			data[idx++] = b;
		}
		return this;
	}
}
