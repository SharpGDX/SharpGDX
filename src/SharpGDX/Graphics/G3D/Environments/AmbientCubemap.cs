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

public class AmbientCubemap {
	private static readonly int NUM_VALUES = 6 * 3;

	private static float clamp ( float v) {
		return v < 0f ? 0f : (v > 1f ? 1f : v);
	}

	public readonly float[] data;

	public AmbientCubemap () {
		data = new float[NUM_VALUES];
	}

	public AmbientCubemap (float[] copyFrom) {
		if (copyFrom.Length != (NUM_VALUES)) throw new GdxRuntimeException("Incorrect array size");
		data = new float[copyFrom.Length];
		Array.Copy(copyFrom, 0, data, 0, data.Length);
	}

	public AmbientCubemap ( AmbientCubemap copyFrom) 
    : this(copyFrom.data)
    {
		
	}

	public AmbientCubemap set ( float[] values) {
		for (int i = 0; i < data.Length; i++)
			data[i] = values[i];
		return this;
	}

	public AmbientCubemap set ( AmbientCubemap other) {
		return set(other.data);
	}

	public AmbientCubemap set (Color color) {
		return set(color.R, color.G, color.B);
	}

	public AmbientCubemap set (float r, float g, float b) {
		for (int idx = 0; idx < NUM_VALUES;) {
			data[idx] = r;
			data[idx + 1] = g;
			data[idx + 2] = b;
			idx += 3;
		}
		return this;
	}

	public Color getColor ( Color @out, int side) {
		side *= 3;
		return @out.Set(data[side], data[side + 1], data[side + 2], 1f);
	}

	public AmbientCubemap clear () {
		for (int i = 0; i < data.Length; i++)
			data[i] = 0f;
		return this;
	}

	public AmbientCubemap clamp () {
		for (int i = 0; i < data.Length; i++)
			data[i] = clamp(data[i]);
		return this;
	}

	public AmbientCubemap add (float r, float g, float b) {
		for (int idx = 0; idx < data.Length;) {
			data[idx++] += r;
			data[idx++] += g;
			data[idx++] += b;
		}
		return this;
	}

	public AmbientCubemap add (Color color) {
		return add(color.R, color.G, color.B);
	}

	public AmbientCubemap add ( float r,  float g,  float b,  float x,  float y,  float z) {
		 float x2 = x * x, y2 = y * y, z2 = z * z;
		float d = x2 + y2 + z2;
		if (d == 0f) return this;
		d = 1f / d * (d + 1f);
		 float rd = r * d, gd = g * d, bd = b * d;
		int idx = x > 0 ? 0 : 3;
		data[idx] += x2 * rd;
		data[idx + 1] += x2 * gd;
		data[idx + 2] += x2 * bd;
		idx = y > 0 ? 6 : 9;
		data[idx] += y2 * rd;
		data[idx + 1] += y2 * gd;
		data[idx + 2] += y2 * bd;
		idx = z > 0 ? 12 : 15;
		data[idx] += z2 * rd;
		data[idx + 1] += z2 * gd;
		data[idx + 2] += z2 * bd;
		return this;
	}

	public AmbientCubemap add ( Color color,  Vector3 direction) {
		return add(color.R, color.G, color.B, direction.x, direction.y, direction.z);
	}

	public AmbientCubemap add ( float r,  float g,  float b,  Vector3 direction) {
		return add(r, g, b, direction.x, direction.y, direction.z);
	}

	public AmbientCubemap add ( Color color,  float x,  float y,  float z) {
		return add(color.R, color.G, color.B, x, y, z);
	}

	public AmbientCubemap add ( Color color,  Vector3 point,  Vector3 target) {
		return add(color.R, color.G, color.B, target.x - point.x, target.y - point.y, target.z - point.z);
	}

	public AmbientCubemap add ( Color color,  Vector3 point,  Vector3 target,  float intensity) {
		 float t = intensity / (1f + target.dst(point));
		return add(color.R * t, color.G * t, color.B * t, target.x - point.x, target.y - point.y, target.z - point.z);
	}

	public override String ToString () {
		String result = "";
		for (int i = 0; i < data.Length; i += 3) {
			result += (data[i]).ToString() + ", " + (data[i + 1]).ToString() + ", " + (data[i + 2]).ToString() + "\n";
		}
		return result;
	}
}
