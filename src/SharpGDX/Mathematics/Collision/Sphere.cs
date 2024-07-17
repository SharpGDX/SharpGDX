using System;
using SharpGDX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Mathematics.Collision
{
	/** Encapsulates a 3D sphere with a center and a radius
 * 
 * @author badlogicgames@gmail.com */
	[Serializable]
public class Sphere   {
	private static readonly long serialVersionUID = -6487336868908521596L;
	/** the radius of the sphere **/
	public float radius;
	/** the center of the sphere **/
	public readonly Vector3 center;

	private static readonly float PI_4_3 = MathUtils.PI * 4f / 3f;

	/** Constructs a sphere with the given center and radius
	 * @param center The center
	 * @param radius The radius */
	public Sphere (Vector3 center, float radius) {
		this.center = new Vector3(center);
		this.radius = radius;
	}

	/** @param sphere the other sphere
	 * @return whether this and the other sphere overlap */
	public bool overlaps (Sphere sphere) {
		return center.dst2(sphere.center) < (radius + sphere.radius) * (radius + sphere.radius);
	}

	public override int GetHashCode () {
		int prime = 71;
		int result = 1;
		result = prime * result + this.center.GetHashCode();
		result = prime * result + NumberUtils.floatToRawIntBits(this.radius);
		return result;
	}

	public override bool Equals (Object? o) {
		if (this == o) return true;
		if (o == null || o.GetType() != this.GetType()) return false;
		Sphere s = (Sphere)o;
		return this.radius == s.radius && this.center.Equals(s.center);
	}

	public float volume () {
		return PI_4_3 * this.radius * this.radius * this.radius;
	}

	public float surfaceArea () {
		return 4 * MathUtils.PI * this.radius * this.radius;
	}
}
}
