﻿namespace SharpGDX.Mathematics
{
	/** A point in a 2D grid, with integer x and y coordinates
 * 
 * @author badlogic */
	[Serializable]
	public class GridPoint2 
	{
	private static readonly long serialVersionUID = -4019969926331717380L;

	public int x;
	public int y;

	/** Constructs a new 2D grid point. */
	public GridPoint2()
	{
	}

	/** Constructs a new 2D grid point.
	 * 
	 * @param x X coordinate
	 * @param y Y coordinate */
	public GridPoint2(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	/** Copy constructor
	 * 
	 * @param point The 2D grid point to make a copy of. */
	public GridPoint2(GridPoint2 point)
	{
		this.x = point.x;
		this.y = point.y;
	}

	/** Sets the coordinates of this 2D grid point to that of another.
	 * 
	 * @param point The 2D grid point to copy the coordinates of.
	 * 
	 * @return this 2D grid point for chaining. */
	public GridPoint2 set(GridPoint2 point)
	{
		this.x = point.x;
		this.y = point.y;
		return this;
	}

	/** Sets the coordinates of this 2D grid point.
	 * 
	 * @param x X coordinate
	 * @param y Y coordinate
	 * 
	 * @return this 2D grid point for chaining. */
	public GridPoint2 set(int x, int y)
	{
		this.x = x;
		this.y = y;
		return this;
	}

	/** @param other The other point
	 * @return the squared distance between this point and the other point. */
	public float dst2(GridPoint2 other)
	{
		int xd = other.x - x;
		int yd = other.y - y;

		return xd * xd + yd * yd;
	}

	/** @param x The x-coordinate of the other point
	 * @param y The y-coordinate of the other point
	 * @return the squared distance between this point and the other point. */
	public float dst2(int x, int y)
	{
		int xd = x - this.x;
		int yd = y - this.y;

		return xd * xd + yd * yd;
	}

	/** @param other The other point
	 * @return the distance between this point and the other vector. */
	public float dst(GridPoint2 other)
	{
		int xd = other.x - x;
		int yd = other.y - y;

		return (float)Math.Sqrt(xd * xd + yd * yd);
	}

	/** @param x The x-coordinate of the other point
	 * @param y The y-coordinate of the other point
	 * @return the distance between this point and the other point. */
	public float dst(int x, int y)
	{
		int xd = x - this.x;
		int yd = y - this.y;

		return (float)Math.Sqrt(xd * xd + yd * yd);
	}

	/** Adds another 2D grid point to this point.
	 *
	 * @param other The other point
	 * @return this 2d grid point for chaining. */
	public GridPoint2 add(GridPoint2 other)
	{
		x += other.x;
		y += other.y;
		return this;
	}

	/** Adds another 2D grid point to this point.
	 *
	 * @param x The x-coordinate of the other point
	 * @param y The y-coordinate of the other point
	 * @return this 2d grid point for chaining. */
	public GridPoint2 add(int x, int y)
	{
		this.x += x;
		this.y += y;
		return this;
	}

	/** Subtracts another 2D grid point from this point.
	 *
	 * @param other The other point
	 * @return this 2d grid point for chaining. */
	public GridPoint2 sub(GridPoint2 other)
	{
		x -= other.x;
		y -= other.y;
		return this;
	}

	/** Subtracts another 2D grid point from this point.
	 *
	 * @param x The x-coordinate of the other point
	 * @param y The y-coordinate of the other point
	 * @return this 2d grid point for chaining. */
	public GridPoint2 sub(int x, int y)
	{
		this.x -= x;
		this.y -= y;
		return this;
	}

	/** @return a copy of this grid point */
	public GridPoint2 cpy()
	{
		return new GridPoint2(this);
	}

	public override bool Equals(Object? o)
	{
		if (this == o) return true;
		if (o == null || o.GetType() != this.GetType()) return false;
		GridPoint2 g = (GridPoint2)o;
		return this.x == g.x && this.y == g.y;
	}

	public override int GetHashCode()
	{
		int prime = 53;
		int result = 1;
		result = prime * result + this.x;
		result = prime * result + this.y;
		return result;
	}

	public override String ToString()
	{
		return "(" + x + ", " + y + ")";
	}
}
}
