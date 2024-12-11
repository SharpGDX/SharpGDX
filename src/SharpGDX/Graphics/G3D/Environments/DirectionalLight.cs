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

public class DirectionalLight : BaseLight {
	public readonly Vector3 direction = new Vector3();

	public DirectionalLight setDirection (float directionX, float directionY, float directionZ) {
		this.direction.Set(directionX, directionY, directionZ);
		return this;
	}

	public DirectionalLight setDirection (Vector3 direction) {
		this.direction.Set(direction);
		return this;
	}

	public DirectionalLight set ( DirectionalLight copyFrom) {
		return set(copyFrom.color, copyFrom.direction);
	}

	public DirectionalLight set ( Color color,  Vector3 direction) {
		if (color != null) this.color.Set(color);
		if (direction != null) this.direction.Set(direction).nor();
		return this;
	}

	public DirectionalLight set ( float r,  float g,  float b,  Vector3 direction) {
		this.color.Set(r, g, b, 1f);
		if (direction != null) this.direction.Set(direction).nor();
		return this;
	}

	public DirectionalLight set ( Color color,  float dirX,  float dirY,  float dirZ) {
		if (color != null) this.color.Set(color);
		this.direction.Set(dirX, dirY, dirZ).nor();
		return this;
	}

	public DirectionalLight set ( float r,  float g,  float b,  float dirX,  float dirY,
		 float dirZ) {
		this.color.Set(r, g, b, 1f);
		this.direction.Set(dirX, dirY, dirZ).nor();
		return this;
	}

	public override bool Equals (Object? arg0) {
		return (arg0 is DirectionalLight) && Equals((DirectionalLight)arg0);
	}

	public bool Equals ( DirectionalLight other) {
		return (other != null) && ((other == this) || ((color.Equals(other.color) && direction.Equals(other.direction))));
	}
}
