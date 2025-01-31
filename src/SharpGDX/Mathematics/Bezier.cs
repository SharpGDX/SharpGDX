﻿using SharpGDX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Mathematics
{
	/** Implementation of the Bezier curve.
 * @author Xoppa */
	public class Bezier<T > : IPath<T> 
	where T: IVector<T>
	{
	// TODO implement Serializable

		/** Simple linear interpolation
		 * @param out The {@link Vector} to set to the result.
		 * @param t The location (ranging 0..1) on the line.
		 * @param p0 The start point.
		 * @param p1 The end point.
		 * @param tmp A temporary vector to be used by the calculation.
		 * @return The value specified by out for chaining */
	public static T linear( T @out,  float t,  T p0,  T p1,  T tmp)
	{
		// B1(t) = p0 + (p1-p0)*t
		return @out.Set(p0).scl(1f - t).add(tmp.Set(p1).scl(t)); // Could just use lerp...
	}

	/** Simple linear interpolation derivative
	 * @param out The {@link Vector} to set to the result.
	 * @param t The location (ranging 0..1) on the line.
	 * @param p0 The start point.
	 * @param p1 The end point.
	 * @param tmp A temporary vector to be used by the calculation.
	 * @return The value specified by out for chaining */
	public static T linear_derivative( T @out,  float t,  T p0,  T p1,  T tmp)
	{
		// B1'(t) = p1-p0
		return @out.Set(p1).sub(p0);
	}

	/** Quadratic Bezier curve
	 * @param out The {@link Vector} to set to the result.
	 * @param t The location (ranging 0..1) on the curve.
	 * @param p0 The first bezier point.
	 * @param p1 The second bezier point.
	 * @param p2 The third bezier point.
	 * @param tmp A temporary vector to be used by the calculation.
	 * @return The value specified by out for chaining */
	public static T quadratic( T @out,  float t,  T p0,  T p1,  T p2,  T tmp)
	{
		// B2(t) = (1 - t) * (1 - t) * p0 + 2 * (1-t) * t * p1 + t*t*p2
		 float dt = 1f - t;
		return @out.Set(p0).scl(dt * dt).add(tmp.Set(p1).scl(2 * dt * t)).add(tmp.Set(p2).scl(t * t));
	}

	/** Quadratic Bezier curve derivative
	 * @param out The {@link Vector} to set to the result.
	 * @param t The location (ranging 0..1) on the curve.
	 * @param p0 The first bezier point.
	 * @param p1 The second bezier point.
	 * @param p2 The third bezier point.
	 * @param tmp A temporary vector to be used by the calculation.
	 * @return The value specified by out for chaining */
	public static T quadratic_derivative( T @out,  float t,  T p0,  T p1,  T p2,
		 T tmp)
	{
		// B2'(t) = 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1)
		 float dt = 1f - t;
		return @out.Set(p1).sub(p0).scl(2).scl(1 - t).add(tmp.Set(p2).sub(p1).scl(t).scl(2));
	}

	/** Cubic Bezier curve
	 * @param out The {@link Vector} to set to the result.
	 * @param t The location (ranging 0..1) on the curve.
	 * @param p0 The first bezier point.
	 * @param p1 The second bezier point.
	 * @param p2 The third bezier point.
	 * @param p3 The fourth bezier point.
	 * @param tmp A temporary vector to be used by the calculation.
	 * @return The value specified by out for chaining */
	public static T cubic( T @out,  float t,  T p0,  T p1,  T p2,  T p3,
		 T tmp)
	{
		// B3(t) = (1-t) * (1-t) * (1-t) * p0 + 3 * (1-t) * (1-t) * t * p1 + 3 * (1-t) * t * t * p2 + t * t * t * p3
		 float dt = 1f - t;
		 float dt2 = dt * dt;
		 float t2 = t * t;
		return @out.Set(p0).scl(dt2 * dt).add(tmp.Set(p1).scl(3 * dt2 * t)).add(tmp.Set(p2).scl(3 * dt * t2))
			.add(tmp.Set(p3).scl(t2 * t));
	}

	/** Cubic Bezier curve derivative
	 * @param out The {@link Vector} to set to the result.
	 * @param t The location (ranging 0..1) on the curve.
	 * @param p0 The first bezier point.
	 * @param p1 The second bezier point.
	 * @param p2 The third bezier point.
	 * @param p3 The fourth bezier point.
	 * @param tmp A temporary vector to be used by the calculation.
	 * @return The value specified by out for chaining */
	public static T cubic_derivative( T @out,  float t,  T p0,  T p1,  T p2,
		 T p3,  T tmp)
	{
		// B3'(t) = 3 * (1-t) * (1-t) * (p1 - p0) + 6 * (1 - t) * t * (p2 - p1) + 3 * t * t * (p3 - p2)
		 float dt = 1f - t;
		 float dt2 = dt * dt;
		 float t2 = t * t;
		return @out.Set(p1).sub(p0).scl(dt2 * 3).add(tmp.Set(p2).sub(p1).scl(dt * t * 6)).add(tmp.Set(p3).sub(p2).scl(t2 * 3));
	}

	public Array<T> points = new Array<T>();
	private T tmp;
	private T tmp2;
	private T tmp3;

	public Bezier()
	{
	}

	public Bezier( T[] points)
	{
		set(points);
	}

	public Bezier( T[] points,  int offset,  int length)
	{
		set(points, offset, length);
	}

	public Bezier( Array<T> points,  int offset,  int length)
	{
		set(points, offset, length);
	}

	public Bezier<T> set( T[] points)
	{
		return set(points, 0, points.Length);
	}

	public Bezier<T> set( T[] points,  int offset,  int length)
	{
		if (length < 2 || length > 4)
			throw new GdxRuntimeException("Only first, second and third degree Bezier curves are supported.");
		if (tmp == null) tmp = points[0].cpy();
		if (tmp2 == null) tmp2 = points[0].cpy();
		if (tmp3 == null) tmp3 = points[0].cpy();
		this.points.clear();
		this.points.addAll(points, offset, length);
		return this;
	}

	public Bezier<T> set( Array<T> points,  int offset,  int length)
	{
		if (length < 2 || length > 4)
			throw new GdxRuntimeException("Only first, second and third degree Bezier curves are supported.");
		if (tmp == null) tmp = points.Get(0).cpy();
		if (tmp2 == null) tmp2 = points.Get(0).cpy();
		if (tmp3 == null) tmp3 = points.Get(0).cpy();
		this.points.clear();
		this.points.addAll(points, offset, length);
		return this;
	}

	public T valueAt( T @out,  float t)
	{
		 int n = points.size;
		if (n == 2)
			linear(@out, t, points.Get(0), points.Get(1), tmp);
		else if (n == 3)
			quadratic(@out, t, points.Get(0), points.Get(1), points.Get(2), tmp);
		else if (n == 4) cubic(@out, t, points.Get(0), points.Get(1), points.Get(2), points.Get(3), tmp);
		return @out;
	}

	public T derivativeAt( T @out,  float t)
	{
		 int n = points.size;
		if (n == 2)
			linear_derivative(@out, t, points.Get(0), points.Get(1), tmp);
		else if (n == 3)
			quadratic_derivative(@out, t, points.Get(0), points.Get(1), points.Get(2), tmp);
		else if (n == 4) cubic_derivative(@out, t, points.Get(0), points.Get(1), points.Get(2), points.Get(3), tmp);
		return @out;
	}

	public float approximate( T v)
	{
		// TODO: make a real approximate method
		T p1 = points.Get(0);
		T p2 = points.Get(points.size - 1);
		T p3 = v;
		float l1Sqr = p1.dst2(p2);
		float l2Sqr = p3.dst2(p2);
		float l3Sqr = p3.dst2(p1);
		float l1 = (float)Math.Sqrt(l1Sqr);
		float s = (l2Sqr + l1Sqr - l3Sqr) / (2 * l1);
		return MathUtils.Clamp((l1 - s) / l1, 0f, 1f);
	}

	public float locate(T v)
	{
		// TODO implement a precise method
		return approximate(v);
	}

	public float approxLength(int samples)
	{
		float tempLength = 0;
		for (int i = 0; i < samples; ++i)
		{
			tmp2.Set(tmp3);
			valueAt(tmp3, (i) / ((float)samples - 1));
			if (i > 0) tempLength += tmp2.dst(tmp3);
		}
		return tempLength;
	}
}
}
