using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX.Mathematics
{
	/** @author Xoppa */
	public class BSpline<T > : IPath<T>
	where T : IVector<T>
	{
	private readonly static float d6 = 1f / 6f;

	/** Calculates the cubic b-spline value for the given position (t).
	 * @param out The Vector to set to the result.
	 * @param t The position (0<=t<=1) on the spline
	 * @param points The control points
	 * @param continuous If true the b-spline restarts at 0 when reaching 1
	 * @param tmp A temporary vector used for the calculation
	 * @return The value of out */
	public static T cubic( T @out,  float t,  T[] points,  bool continuous,
		 T tmp)
	{
		 int n = continuous ? points.Length : points.Length - 3;
		float u = t * n;
		int i = (t >= 1f) ? (n - 1) : (int)u;
		u -= i;
		return cubic(@out, i, u, points, continuous, tmp);
	}

	/** Calculates the cubic b-spline derivative for the given position (t).
	 * @param out The Vector to set to the result.
	 * @param t The position (0<=t<=1) on the spline
	 * @param points The control points
	 * @param continuous If true the b-spline restarts at 0 when reaching 1
	 * @param tmp A temporary vector used for the calculation
	 * @return The value of out */
	public static T cubic_derivative( T @out,  float t,  T[] points,  bool continuous,
		 T tmp)
	{
		 int n = continuous ? points.Length : points.Length - 3;
		float u = t * n;
		int i = (t >= 1f) ? (n - 1) : (int)u;
		u -= i;
		return cubic(@out, i, u, points, continuous, tmp);
	}

	/** Calculates the cubic b-spline value for the given span (i) at the given position (u).
	 * @param out The Vector to set to the result.
	 * @param i The span (0<=i<spanCount) spanCount = continuous ? points.length : points.length - 3 (cubic degree)
	 * @param u The position (0<=u<=1) on the span
	 * @param points The control points
	 * @param continuous If true the b-spline restarts at 0 when reaching 1
	 * @param tmp A temporary vector used for the calculation
	 * @return The value of out */
	public static T cubic( T @out,  int i,  float u,  T[] points,
		 bool continuous,  T tmp)
	{
		 int n = points.Length;
		 float dt = 1f - u;
		 float t2 = u * u;
		 float t3 = t2 * u;
		 @out.set(points[i]).scl((3f * t3 - 6f * t2 + 4f) * d6);
		if (continuous || i > 0) @out.add(tmp.set(points[(n + i - 1) % n]).scl(dt * dt * dt * d6));
		if (continuous || i < (n - 1)) @out.add(tmp.set(points[(i + 1) % n]).scl((-3f * t3 + 3f * t2 + 3f * u + 1f) * d6));
		if (continuous || i < (n - 2)) @out.add(tmp.set(points[(i + 2) % n]).scl(t3 * d6));
		return @out;
	}

	/** Calculates the cubic b-spline derivative for the given span (i) at the given position (u).
	 * @param out The Vector to set to the result.
	 * @param i The span (0<=i<spanCount) spanCount = continuous ? points.length : points.length - 3 (cubic degree)
	 * @param u The position (0<=u<=1) on the span
	 * @param points The control points
	 * @param continuous If true the b-spline restarts at 0 when reaching 1
	 * @param tmp A temporary vector used for the calculation
	 * @return The value of out */
	public static  T cubic_derivative( T @out,  int i,  float u,  T[] points,
		 bool continuous,  T tmp)
	{
		 int n = points.Length;
		 float dt = 1f - u;
		 float t2 = u * u;
		 float t3 = t2 * u;
		 @out.set(points[i]).scl(1.5f * t2 - 2 * u);
		if (continuous || i > 0) @out.add(tmp.set(points[(n + i - 1) % n]).scl(-0.5f * dt * dt));
		if (continuous || i < (n - 1)) @out.add(tmp.set(points[(i + 1) % n]).scl(-1.5f * t2 + u + 0.5f));
		if (continuous || i < (n - 2)) @out.add(tmp.set(points[(i + 2) % n]).scl(0.5f * t2));
		return @out;
	}

	/** Calculates the n-degree b-spline value for the given position (t).
	 * @param out The Vector to set to the result.
	 * @param t The position (0<=t<=1) on the spline
	 * @param points The control points
	 * @param degree The degree of the b-spline
	 * @param continuous If true the b-spline restarts at 0 when reaching 1
	 * @param tmp A temporary vector used for the calculation
	 * @return The value of out */
	public static T calculate( T @out,  float t,  T[] points,  int degree,
		 bool continuous,  T tmp)
	{
		 int n = continuous ? points.Length : points.Length - degree;
		float u = t * n;
		int i = (t >= 1f) ? (n - 1) : (int)u;
		u -= i;
		return calculate(@out, i, u, points, degree, continuous, tmp);
	}

	/** Calculates the n-degree b-spline derivative for the given position (t).
	 * @param out The Vector to set to the result.
	 * @param t The position (0<=t<=1) on the spline
	 * @param points The control points
	 * @param degree The degree of the b-spline
	 * @param continuous If true the b-spline restarts at 0 when reaching 1
	 * @param tmp A temporary vector used for the calculation
	 * @return The value of out */
	public static T derivative( T @out,  float t,  T[] points,  int degree,
		 bool continuous,  T tmp)
	{
		 int n = continuous ? points.Length : points.Length - degree;
		float u = t * n;
		int i = (t >= 1f) ? (n - 1) : (int)u;
		u -= i;
		return derivative(@out, i, u, points, degree, continuous, tmp);
	}

	/** Calculates the n-degree b-spline value for the given span (i) at the given position (u).
	 * @param out The Vector to set to the result.
	 * @param i The span (0<=i<spanCount) spanCount = continuous ? points.length : points.length - degree
	 * @param u The position (0<=u<=1) on the span
	 * @param points The control points
	 * @param degree The degree of the b-spline, only 3 is supported
	 * @param continuous If true the b-spline restarts at 0 when reaching 1
	 * @param tmp A temporary vector used for the calculation
	 * @return The value of out */
	public static T calculate( T @out,  int i,  float u,  T[] points,  int degree,
		 bool continuous,  T tmp)
	{
		switch (degree)
		{
			case 3:
				return cubic(@out, i, u, points, continuous, tmp);
		}
		throw new IllegalArgumentException();
	}

	/** Calculates the n-degree b-spline derivative for the given span (i) at the given position (u).
	 * @param out The Vector to set to the result.
	 * @param i The span (0<=i<spanCount) spanCount = continuous ? points.length : points.length - degree
	 * @param u The position (0<=u<=1) on the span
	 * @param points The control points
	 * @param degree The degree of the b-spline, only 3 is supported
	 * @param continuous If true the b-spline restarts at 0 when reaching 1
	 * @param tmp A temporary vector used for the calculation
	 * @return The value of out */
	public static  T derivative( T @out,  int i,  float u,  T[] points,  int degree,
		 bool continuous,  T tmp)
	{
		switch (degree)
		{
			case 3:
				return cubic_derivative(@out, i, u, points, continuous, tmp);
		}
		throw new IllegalArgumentException();
	}

	public T[] controlPoints;
	public Array<T> knots;
	public int degree;
	public bool continuous;
	public int spanCount;
	private T tmp;
	private T tmp2;
	private T tmp3;

	public BSpline()
	{
	}

	public BSpline( T[] controlPoints,  int degree,  bool continuous)
	{
		set(controlPoints, degree, continuous);
	}

	public BSpline<T> set( T[] controlPoints,  int degree,  bool continuous)
	{
		if (tmp == null) tmp = controlPoints[0].cpy();
		if (tmp2 == null) tmp2 = controlPoints[0].cpy();
		if (tmp3 == null) tmp3 = controlPoints[0].cpy();
		this.controlPoints = controlPoints;
		this.degree = degree;
		this.continuous = continuous;
		this.spanCount = continuous ? controlPoints.Length : controlPoints.Length - degree;
		if (knots == null)
			knots = new Array<T>(spanCount);
		else
		{
			knots.clear();
			knots.ensureCapacity(spanCount);
		}
		for (int i = 0; i < spanCount; i++)
			knots.add(calculate(controlPoints[0].cpy(), continuous ? i : (int)(i + 0.5f * degree), 0f, controlPoints, degree,
				continuous, tmp));
		return this;
	}

	public T valueAt(T @out, float t)
	{
		 int n = spanCount;
		float u = t * n;
		int i = (t >= 1f) ? (n - 1) : (int)u;
		u -= i;
		return valueAt(@out, i, u);
	}

	/** @return The value of the spline at position u of the specified span */
	public T valueAt( T @out,  int span,  float u)
	{
		return calculate(@out, continuous ? span : (span + (int)(degree * 0.5f)), u, controlPoints, degree, continuous, tmp);
	}

	public T derivativeAt( T @out,  float t)
	{
		 int n = spanCount;
		float u = t * n;
		int i = (t >= 1f) ? (n - 1) : (int)u;
		u -= i;
		return derivativeAt(@out, i, u);
	}

	/** @return The derivative of the spline at position u of the specified span */
	public T derivativeAt( T @out,  int span,  float u)
	{
		return derivative(@out, continuous ? span : (span + (int)(degree * 0.5f)), u, controlPoints, degree, continuous, tmp);
	}

	/** @return The span closest to the specified value */
	public int nearest( T @in)
	{
		return nearest(@in, 0, spanCount);
	}

	/** @return The span closest to the specified value, restricting to the specified spans. */
	public int nearest( T @in, int start,  int count)
	{
		while (start < 0)
			start += spanCount;
		int result = start % spanCount;
		float dst = @in.dst2(knots.get(result));
		for (int i = 1; i < count; i++)
		{
			 int idx = (start + i) % spanCount;
			 float d = @in.dst2(knots.get(idx));
			if (d < dst)
			{
				dst = d;
				result = idx;
			}
		}
		return result;
	}

	public float approximate(T v)
	{
		return approximate(v, nearest(v));
	}

	public float approximate( T @in, int start,  int count)
	{
		return approximate(@in, nearest(@in, start, count));
	}

	public float approximate( T @in,  int near)
	{
		int n = near;
		 T nearest = knots.get(n);
		 T previous = knots.get(n > 0 ? n - 1 : spanCount - 1);
		 T next = knots.get((n + 1) % spanCount);
		 float dstPrev2 = @in.dst2(previous);
		 float dstNext2 = @in.dst2(next);
		T P1, P2, P3;
		if (dstNext2 < dstPrev2)
		{
			P1 = nearest;
			P2 = next;
			P3 = @in;
		}
		else
		{
			P1 = previous;
			P2 = nearest;
			P3 = @in;
			n = n > 0 ? n - 1 : spanCount - 1;
		}
		float L1Sqr = P1.dst2(P2);
		float L2Sqr = P3.dst2(P2);
		float L3Sqr = P3.dst2(P1);
		float L1 = (float)Math.Sqrt(L1Sqr);
		float s = (L2Sqr + L1Sqr - L3Sqr) / (2 * L1);
		float u = MathUtils.clamp((L1 - s) / L1, 0f, 1f);
		return (n + u) / spanCount;
	}

	public float locate(T v)
	{
		// TODO Add a precise method
		return approximate(v);
	}

	public float approxLength(int samples)
	{
		float tempLength = 0;
		for (int i = 0; i < samples; ++i)
		{
			tmp2.set(tmp3);
			valueAt(tmp3, (i) / ((float)samples - 1));
			if (i > 0) tempLength += tmp2.dst(tmp3);
		}
		return tempLength;
	}
}
}
