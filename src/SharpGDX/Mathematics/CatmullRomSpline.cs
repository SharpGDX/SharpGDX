namespace SharpGDX.Mathematics
{
	/** @author Xoppa */
	public class CatmullRomSpline<T > : IPath<T> 
	where T: IVector<T>
	{
	/** Calculates the catmullrom value for the given position (t).
	 * @param out The Vector to set to the result.
	 * @param t The position (0<=t<=1) on the spline
	 * @param points The control points
	 * @param continuous If true the b-spline restarts at 0 when reaching 1
	 * @param tmp A temporary vector used for the calculation
	 * @return The value of out */
	public static  T calculate( T @out,  float t,  T[] points,  bool continuous,
		 T tmp)
	{
		 int n = continuous ? points.Length : points.Length - 3;
		float u = t * n;
		int i = (t >= 1f) ? (n - 1) : (int)u;
		u -= i;
		return calculate(@out, i, u, points, continuous, tmp);
	}

	/** Calculates the catmullrom value for the given span (i) at the given position (u).
	 * @param out The Vector to set to the result.
	 * @param i The span (0<=i<spanCount) spanCount = continuous ? points.Length : points.Length - degree
	 * @param u The position (0<=u<=1) on the span
	 * @param points The control points
	 * @param continuous If true the b-spline restarts at 0 when reaching 1
	 * @param tmp A temporary vector used for the calculation
	 * @return The value of out */
	public static  T calculate( T @out,  int i,  float u,  T[] points,
		 bool continuous,  T tmp)
	{
		 int n = points.Length;
		 float u2 = u * u;
		 float u3 = u2 * u;
		@out.set(points[i]).scl(1.5f * u3 - 2.5f * u2 + 1.0f);
		if (continuous || i > 0) @out.add(tmp.set(points[(n + i - 1) % n]).scl(-0.5f * u3 + u2 - 0.5f * u));
		if (continuous || i < (n - 1)) @out.add(tmp.set(points[(i + 1) % n]).scl(-1.5f * u3 + 2f * u2 + 0.5f * u));
		if (continuous || i < (n - 2)) @out.add(tmp.set(points[(i + 2) % n]).scl(0.5f * u3 - 0.5f * u2));
		return @out;
	}

	/** Calculates the derivative of the catmullrom spline for the given position (t).
	 * @param out The Vector to set to the result.
	 * @param t The position (0<=t<=1) on the spline
	 * @param points The control points
	 * @param continuous If true the b-spline restarts at 0 when reaching 1
	 * @param tmp A temporary vector used for the calculation
	 * @return The value of out */
	public static  T derivative( T @out,  float t,  T[] points,  bool continuous,
		 T tmp)
	{
		 int n = continuous ? points.Length : points.Length - 3;
		float u = t * n;
		int i = (t >= 1f) ? (n - 1) : (int)u;
		u -= i;
		return derivative(@out, i, u, points, continuous, tmp);
	}

	/** Calculates the derivative of the catmullrom spline for the given span (i) at the given position (u).
	 * @param out The Vector to set to the result.
	 * @param i The span (0<=i<spanCount) spanCount = continuous ? points.Length : points.Length - degree
	 * @param u The position (0<=u<=1) on the span
	 * @param points The control points
	 * @param continuous If true the b-spline restarts at 0 when reaching 1
	 * @param tmp A temporary vector used for the calculation
	 * @return The value of out */
	public static  T derivative( T @out,  int i,  float u,  T[] points,
		 bool continuous,  T tmp)
	{
		/*
		 * catmull'(u) = 0.5 *((-p0 + p2) + 2 * (2*p0 - 5*p1 + 4*p2 - p3) * u + 3 * (-p0 + 3*p1 - 3*p2 + p3) * u * u)
		 */
		 int n = points.Length;
		 float u2 = u * u;
		// final float u3 = u2 * u;
		@out.set(points[i]).scl(-u * 5 + u2 * 4.5f);
		if (continuous || i > 0) @out.add(tmp.set(points[(n + i - 1) % n]).scl(-0.5f + u * 2 - u2 * 1.5f));
		if (continuous || i < (n - 1)) @out.add(tmp.set(points[(i + 1) % n]).scl(0.5f + u * 4 - u2 * 4.5f));
		if (continuous || i < (n - 2)) @out.add(tmp.set(points[(i + 2) % n]).scl(-u + u2 * 1.5f));
		return @out;
	}

	public T[] controlPoints;
	public bool continuous;
	public int spanCount;
	private T tmp;
	private T tmp2;
	private T tmp3;

	public CatmullRomSpline()
	{
	}

	public CatmullRomSpline( T[] controlPoints,  bool continuous)
	{
		set(controlPoints, continuous);
	}

	public CatmullRomSpline<T> set( T[] controlPoints,  bool continuous)
	{
		if (tmp == null) tmp = controlPoints[0].cpy();
		if (tmp2 == null) tmp2 = controlPoints[0].cpy();
		if (tmp3 == null) tmp3 = controlPoints[0].cpy();
		this.controlPoints = controlPoints;
		this.continuous = continuous;
		this.spanCount = continuous ? controlPoints.Length : controlPoints.Length - 3;
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
		return calculate(@out, continuous ? span : (span + 1), u, controlPoints, continuous, tmp);
	}

		public T derivativeAt(T @out, float t)
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
		return derivative(@out, continuous ? span : (span + 1), u, controlPoints, continuous, tmp);
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
		float dst = @in.dst2(controlPoints[result]);
		for (int i = 1; i < count; i++)
		{
			 int idx = (start + i) % spanCount;
			 float d = @in.dst2(controlPoints[idx]);
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
		 T nearest = controlPoints[n];
		 T previous = controlPoints[n > 0 ? n - 1 : spanCount - 1];
		 T next = controlPoints[(n + 1) % spanCount];
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
		float s = (L2Sqr + L1Sqr - L3Sqr) / (2f * L1);
		float u = MathUtils.clamp((L1 - s) / L1, 0f, 1f);
		return (n + u) / spanCount;
	}

		public float locate(T v)
	{
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
