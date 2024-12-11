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

/** Note that the default shader doesn't support spot lights, you'll have to supply your own shader to use this class.
 * @author realitix */
public class SpotLight : BaseLight {
	public readonly Vector3 position = new Vector3();
	public readonly Vector3 direction = new Vector3();
	public float intensity;
	public float cutoffAngle;
	public float exponent;

	public SpotLight setPosition (float positionX, float positionY, float positionZ) {
		this.position.Set(positionX, positionY, positionZ);
		return this;
	}

	public SpotLight setPosition (Vector3 position) {
		this.position.Set(position);
		return this;
	}

	public SpotLight setDirection (float directionX, float directionY, float directionZ) {
		this.direction.Set(directionX, directionY, directionZ);
		return this;
	}

	public SpotLight setDirection (Vector3 direction) {
		this.direction.Set(direction);
		return this;
	}

	public SpotLight setIntensity (float intensity) {
		this.intensity = intensity;
		return this;
	}

	public SpotLight setCutoffAngle (float cutoffAngle) {
		this.cutoffAngle = cutoffAngle;
		return this;
	}

	public SpotLight setExponent (float exponent) {
		this.exponent = exponent;
		return this;
	}

	public SpotLight set ( SpotLight copyFrom) {
		return set(copyFrom.color, copyFrom.position, copyFrom.direction, copyFrom.intensity, copyFrom.cutoffAngle,
			copyFrom.exponent);
	}

	public SpotLight set ( Color color,  Vector3 position,  Vector3 direction,  float intensity,
		float cutoffAngle, float exponent) {
		if (color != null) this.color.Set(color);
		if (position != null) this.position.Set(position);
		if (direction != null) this.direction.Set(direction).nor();
		this.intensity = intensity;
		this.cutoffAngle = cutoffAngle;
		this.exponent = exponent;
		return this;
	}

	public SpotLight set ( float r,  float g,  float b,  Vector3 position,  Vector3 direction,
		 float intensity,  float cutoffAngle,  float exponent) {
		this.color.Set(r, g, b, 1f);
		if (position != null) this.position.Set(position);
		if (direction != null) this.direction.Set(direction).nor();
		this.intensity = intensity;
		this.cutoffAngle = cutoffAngle;
		this.exponent = exponent;
		return this;
	}

	public SpotLight set ( Color color,  float posX,  float posY,  float posZ,  float dirX,
		 float dirY,  float dirZ,  float intensity,  float cutoffAngle,  float exponent) {
		if (color != null) this.color.Set(color);
		this.position.Set(posX, posY, posZ);
		this.direction.Set(dirX, dirY, dirZ).nor();
		this.intensity = intensity;
		this.cutoffAngle = cutoffAngle;
		this.exponent = exponent;
		return this;
	}

	public SpotLight set ( float r,  float g,  float b,  float posX,  float posY,  float posZ,
		 float dirX,  float dirY,  float dirZ,  float intensity,  float cutoffAngle,
		 float exponent) {
		this.color.Set(r, g, b, 1f);
		this.position.Set(posX, posY, posZ);
		this.direction.Set(dirX, dirY, dirZ).nor();
		this.intensity = intensity;
		this.cutoffAngle = cutoffAngle;
		this.exponent = exponent;
		return this;
	}

	public SpotLight setTarget ( Vector3 target) {
		direction.Set(target).sub(position).nor();
		return this;
	}

	public override bool Equals (Object? obj) {
		return (obj is SpotLight) && Equals((SpotLight)obj);
	}

	public bool Equals (SpotLight other) {
		return (other != null && (other == this || (color.Equals(other.color) && position.Equals(other.position)
			&& direction.Equals(other.direction) && MathUtils.isEqual(intensity, other.intensity)
			&& MathUtils.isEqual(cutoffAngle, other.cutoffAngle) && MathUtils.isEqual(exponent, other.exponent))));
	}
}
