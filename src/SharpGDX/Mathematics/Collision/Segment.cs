using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Mathematics.Collision
{
	/** A Segment is a line in 3-space having a starting and an ending position.
 * 
 * @author mzechner */
	[Serializable]
public class Segment   {
	private static readonly long serialVersionUID = 2739667069736519602L;

	/** the starting position **/
	public readonly Vector3 a = new Vector3();

	/** the ending position **/
	public readonly Vector3 b = new Vector3();

	/** Constructs a new Segment from the two points given.
	 * 
	 * @param a the first point
	 * @param b the second point */
	public Segment (Vector3 a, Vector3 b) {
		this.a.set(a);
		this.b.set(b);
	}

	/** Constructs a new Segment from the two points given.
	 * @param aX the x-coordinate of the first point
	 * @param aY the y-coordinate of the first point
	 * @param aZ the z-coordinate of the first point
	 * @param bX the x-coordinate of the second point
	 * @param bY the y-coordinate of the second point
	 * @param bZ the z-coordinate of the second point */
	public Segment (float aX, float aY, float aZ, float bX, float bY, float bZ) {
		this.a.set(aX, aY, aZ);
		this.b.set(bX, bY, bZ);
	}

	public float len () {
		return a.dst(b);
	}

	public float len2 () {
		return a.dst2(b);
	}

	public override bool Equals (Object? o) {
		if (o == this) return true;
		if (o == null || o.GetType() != this.GetType()) return false;
		Segment s = (Segment)o;
		return this.a.Equals(s.a) && this.b.Equals(s.b);
	}

	public override int GetHashCode () {
		int prime = 71;
		int result = 1;
		result = prime * result + this.a.GetHashCode();
		result = prime * result + this.b.GetHashCode();
		return result;
	}
}
}
