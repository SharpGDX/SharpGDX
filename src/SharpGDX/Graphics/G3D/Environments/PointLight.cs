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

public class PointLight : BaseLight {
	public readonly Vector3 position = new Vector3();
	public float intensity;

	public PointLight setPosition (float positionX, float positionY, float positionZ) {
		this.position.Set(positionX, positionY, positionZ);
		return this;
	}

	public PointLight setPosition (Vector3 position) {
		this.position.Set(position);
		return this;
	}

	public PointLight setIntensity (float intensity) {
		this.intensity = intensity;
		return this;
	}

	public PointLight set ( PointLight copyFrom) {
		return set(copyFrom.color, copyFrom.position, copyFrom.intensity);
	}

	public PointLight set ( Color color,  Vector3 position,  float intensity) {
		if (color != null) this.color.Set(color);
		if (position != null) this.position.Set(position);
		this.intensity = intensity;
		return this;
	}

	public PointLight set ( float r,  float g,  float b,  Vector3 position,  float intensity) {
		this.color.Set(r, g, b, 1f);
		if (position != null) this.position.Set(position);
		this.intensity = intensity;
		return this;
	}

	public PointLight set ( Color color,  float x,  float y,  float z,  float intensity) {
		if (color != null) this.color.Set(color);
		this.position.Set(x, y, z);
		this.intensity = intensity;
		return this;
	}

	public PointLight set ( float r,  float g,  float b,  float x,  float y,  float z,
		 float intensity) {
		this.color.Set(r, g, b, 1f);
		this.position.Set(x, y, z);
		this.intensity = intensity;
		return this;
	}

	public override bool Equals (Object? obj) {
		return (obj is PointLight) && Equals((PointLight)obj);
	}

	public bool Equals (PointLight other) {
		return (other != null
			&& (other == this || (color.Equals(other.color) && position.Equals(other.position) && intensity == other.intensity)));
	}
}
