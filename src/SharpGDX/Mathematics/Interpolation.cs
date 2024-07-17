using System;
using SharpGDX.Shims;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Mathematics
{
	/** Takes a linear value in the range of 0-1 and outputs a (usually) non-linear, interpolated value.
 * @author Nathan Sweet */
public abstract class Interpolation {
	/** @param a Alpha value between 0 and 1. */
	abstract public float apply (float a);

	/** @param a Alpha value between 0 and 1. */
	public float apply (float start, float end, float a) {
		return start + (end - start) * apply(a);
	}

	//

	static public readonly Interpolation linear = new Linear() {
		
	};

	private sealed class Linear : Interpolation
	{
		public override float apply(float a)
		{
			return a;
		}
		}

	/** Aka "smoothstep". */
	static public readonly Interpolation smooth = new Smooth()
	{

	};

	private sealed class Smooth : Interpolation
	{
		public override float apply(float a)
		{
			return a * a * (3 - 2 * a);
		}

		}

	static public readonly Interpolation smooth2 = new Smooth2()
	{

	};

	private sealed class Smooth2 : Interpolation
	{
		public override float apply(float a)
		{
			a = a * a * (3 - 2 * a);
			return a * a * (3 - 2 * a);
		}
		}

	/** By Ken Perlin. */
	static public readonly Interpolation smoother = new Smoother() {
		
	};

	private sealed class Smoother : Interpolation
	{
		public override float apply(float a)
		{
			return a * a * a * (a * (a * 6 - 15) + 10);
		}
		}

	static public readonly Interpolation fade = smoother;

	//

	static public readonly Pow pow2 = new Pow(2);
	/** Slow, then fast. */
	static public readonly PowIn pow2In = new PowIn(2);
	static public readonly PowIn slowFast = pow2In;
	/** Fast, then slow. */
	static public readonly PowOut pow2Out = new PowOut(2);
	static public readonly PowOut fastSlow = pow2Out;
	static public readonly Interpolation pow2InInverse = new Pow2InInverse() {
		
	};

	private sealed class Pow2InInverse : Interpolation
	{
		public override float apply(float a)
		{
			if (a < MathUtils.FLOAT_ROUNDING_ERROR) return 0;
			return (float)Math.Sqrt(a);
		}
		}
	static public readonly Interpolation pow2OutInverse = new Pow2OutInverse() {
		
	};

	private sealed class Pow2OutInverse : Interpolation
	{
		public override float apply(float a)
		{
			if (a < MathUtils.FLOAT_ROUNDING_ERROR) return 0;
			if (a > 1) return 1;
			return 1 - (float)Math.Sqrt(-(a - 1));
		}
		}

	static public readonly Pow pow3 = new Pow(3);
	static public readonly PowIn pow3In = new PowIn(3);
	static public readonly PowOut pow3Out = new PowOut(3);
	static public readonly Interpolation pow3InInverse = new Pow3InInverse() {
		
	};

	private sealed class Pow3InInverse : Interpolation
	{
		public override float apply(float a)
		{
			return (float)Math.Cbrt(a);
		}
		}

	static public readonly Interpolation pow3OutInverse = new Pow3OutInverse() {
		
	};

	private sealed class Pow3OutInverse : Interpolation
	{
		public override float apply(float a)
		{
			return 1 - (float)Math.Cbrt(-(a - 1));
		}
		}

	static public readonly Pow pow4 = new Pow(4);
	static public readonly PowIn pow4In = new PowIn(4);
	static public readonly PowOut pow4Out = new PowOut(4);

	static public readonly Pow pow5 = new Pow(5);
	static public readonly PowIn pow5In = new PowIn(5);
	static public readonly PowOut pow5Out = new PowOut(5);

	static public readonly Interpolation sine = new Sine() {
		
	};

	private sealed class Sine : Interpolation
	{
		public override float apply(float a)
		{
			return (1 - MathUtils.cos(a * MathUtils.PI)) / 2;
		}
		}

	static public readonly Interpolation sineIn = new SineIn() {
		
	};

	private sealed class SineIn : Interpolation
	{
		public override float apply(float a)
		{
			return 1 - MathUtils.cos(a * MathUtils.HALF_PI);
		}
		}

	static public readonly Interpolation sineOut = new SineOut() {
		
	};

	private sealed class SineOut : Interpolation
	{
		public override float apply(float a)
		{
			return MathUtils.sin(a * MathUtils.HALF_PI);
		}
		}

	static public readonly Exp exp10 = new Exp(2, 10);
	static public readonly ExpIn exp10In = new ExpIn(2, 10);
	static public readonly ExpOut exp10Out = new ExpOut(2, 10);

	static public readonly Exp exp5 = new Exp(2, 5);
	static public readonly ExpIn exp5In = new ExpIn(2, 5);
	static public readonly ExpOut exp5Out = new ExpOut(2, 5);

	static public readonly Interpolation circle = new Circle() {
		
	};

	private sealed class Circle : Interpolation
	{
		public override float apply(float a)
		{
			if (a <= 0.5f)
			{
				a *= 2;
				return (1 - (float)Math.Sqrt(1 - a * a)) / 2;
			}
			a--;
			a *= 2;
			return ((float)Math.Sqrt(1 - a * a) + 1) / 2;
		}
		}

	static public readonly Interpolation circleIn = new CircleIn() {
		
	};

	private sealed class CircleIn : Interpolation
	{
		public override float apply(float a)
		{
			return 1 - (float)Math.Sqrt(1 - a * a);
		}
		}

	static public readonly Interpolation circleOut = new CircleOut() {
		
	};

	private sealed class CircleOut : Interpolation
	{
		public override float apply(float a)
		{
			a--;
			return (float)Math.Sqrt(1 - a * a);
		}
		}

	static public readonly Elastic elastic = new Elastic(2, 10, 7, 1);
	static public readonly ElasticIn elasticIn = new ElasticIn(2, 10, 6, 1);
	static public readonly ElasticOut elasticOut = new ElasticOut(2, 10, 7, 1);

	static public readonly Swing swing = new Swing(1.5f);
	static public readonly SwingIn swingIn = new SwingIn(2f);
	static public readonly SwingOut swingOut = new SwingOut(2f);

	static public readonly Bounce bounce = new Bounce(4);
	static public readonly BounceIn bounceIn = new BounceIn(4);
	static public readonly BounceOut bounceOut = new BounceOut(4);

	//

	 public class Pow : Interpolation {
		protected readonly int power;

		public Pow (int power) {
			this.power = power;
		}

		public override float apply (float a) {
			if (a <= 0.5f) return (float)Math.Pow(a * 2, power) / 2;
			return (float)Math.Pow((a - 1) * 2, power) / (power % 2 == 0 ? -2 : 2) + 1;
		}
	}

	 public class PowIn : Pow {
		public PowIn (int power) 
		: base(power)
		{
			
		}

			public override float apply (float a) {
			return (float)Math.Pow(a, power);
		}
	}

	 public class PowOut : Pow {
		public PowOut (int power) : base(power)
		{
			
		}

			public override float apply (float a) {
			return (float)Math.Pow(a - 1, power) * (power % 2 == 0 ? -1 : 1) + 1;
		}
	}

	//

	 public class Exp : Interpolation {
		 protected readonly float value, power, min, scale;

		public Exp (float value, float power) {
			this.value = value;
			this.power = power;
			min = (float)Math.Pow(value, -power);
			scale = 1 / (1 - min);
		}

		public override float apply (float a) {
			if (a <= 0.5f) return ((float)Math.Pow(value, power * (a * 2 - 1)) - min) * scale / 2;
			return (2 - ((float)Math.Pow(value, -power * (a * 2 - 1)) - min) * scale) / 2;
		}
	};

	 public class ExpIn : Exp {
		public ExpIn (float value, float power) 
		: base(value, power)
		{
			
		}

			public override float apply (float a) {
			return ((float)Math.Pow(value, power * (a - 1)) - min) * scale;
		}
	}

	 public class ExpOut : Exp {
		public ExpOut (float value, float power) 
			: base(value, power)
		{
			
		}

			public override float apply (float a) {
			return 1 - ((float)Math.Pow(value, -power * a) - min) * scale;
		}
	}

	//

	 public class Elastic : Interpolation {
		 protected readonly float value, power, scale, bounces;

		public Elastic (float value, float power, int bounces, float scale) {
			this.value = value;
			this.power = power;
			this.scale = scale;
			this.bounces = bounces * MathUtils.PI * (bounces % 2 == 0 ? 1 : -1);
		}

		public override float apply (float a) {
			if (a <= 0.5f) {
				a *= 2;
				return (float)Math.Pow(value, power * (a - 1)) * MathUtils.sin(a * bounces) * scale / 2;
			}
			a = 1 - a;
			a *= 2;
			return 1 - (float)Math.Pow(value, power * (a - 1)) * MathUtils.sin((a) * bounces) * scale / 2;
		}
	}

	 public class ElasticIn : Elastic {
		public ElasticIn (float value, float power, int bounces, float scale) 
		: base(value, power, bounces, scale)
		{
			
		}

			public override float apply (float a) {
			if (a >= 0.99) return 1;
			return (float)Math.Pow(value, power * (a - 1)) * MathUtils.sin(a * bounces) * scale;
		}
	}

	 public class ElasticOut : Elastic {
		public ElasticOut (float value, float power, int bounces, float scale) 
		: base(value, power, bounces, scale)
		{
			
		}

			public override float apply (float a) {
			if (a == 0) return 0;
			a = 1 - a;
			return (1 - (float)Math.Pow(value, power * (a - 1)) * MathUtils.sin(a * bounces) * scale);
		}
	}

	//

	 public class Bounce : BounceOut {
		public Bounce (float[] widths, float[] heights) 
		: base(widths, heights)
		{
			
		}

		public Bounce (int bounces) 
		: base(bounces)
		{
			
		}

		private float @out (float a) {
			float test = a + widths[0] / 2;
			if (test < widths[0]) return test / (widths[0] / 2) - 1;
			return base.apply(a);
		}

			public override float apply (float a) {
			if (a <= 0.5f) return (1 - @out(1 - a * 2)) / 2;
			return @out(a * 2 - 1) / 2 + 0.5f;
		}
	}

	 public class BounceOut : Interpolation {
		 protected readonly float[] widths, heights;

		public BounceOut (float[] widths, float[] heights) {
			if (widths.Length != heights.Length)
				throw new IllegalArgumentException("Must be the same number of widths and heights.");
			this.widths = widths;
			this.heights = heights;
		}

		public BounceOut (int bounces) {
			if (bounces < 2 || bounces > 5) throw new IllegalArgumentException("bounces cannot be < 2 or > 5: " + bounces);
			widths = new float[bounces];
			heights = new float[bounces];
			heights[0] = 1;
			switch (bounces) {
			case 2:
				widths[0] = 0.6f;
				widths[1] = 0.4f;
				heights[1] = 0.33f;
				break;
			case 3:
				widths[0] = 0.4f;
				widths[1] = 0.4f;
				widths[2] = 0.2f;
				heights[1] = 0.33f;
				heights[2] = 0.1f;
				break;
			case 4:
				widths[0] = 0.34f;
				widths[1] = 0.34f;
				widths[2] = 0.2f;
				widths[3] = 0.15f;
				heights[1] = 0.26f;
				heights[2] = 0.11f;
				heights[3] = 0.03f;
				break;
			case 5:
				widths[0] = 0.3f;
				widths[1] = 0.3f;
				widths[2] = 0.2f;
				widths[3] = 0.1f;
				widths[4] = 0.1f;
				heights[1] = 0.45f;
				heights[2] = 0.3f;
				heights[3] = 0.15f;
				heights[4] = 0.06f;
				break;
			}
			widths[0] *= 2;
		}

		public override float apply (float a) {
			if (a == 1) return 1;
			a += widths[0] / 2;
			float width = 0, height = 0;
			for (int i = 0, n = widths.Length; i < n; i++) {
				width = widths[i];
				if (a <= width) {
					height = heights[i];
					break;
				}
				a -= width;
			}
			a /= width;
			float z = 4 / width * height * a;
			return 1 - (z - z * a) * width;
		}
	}

	 public class BounceIn : BounceOut {
		public BounceIn (float[] widths, float[] heights) 
		: base(widths, heights)
		{
			
		}

		public BounceIn (int bounces) 
		: base(bounces)
		{
			
		}

			public override float apply (float a) {
			return 1 - base.apply(1 - a);
		}
	}

	//

	 public class Swing : Interpolation {
		private readonly float scale;

		public Swing (float scale) {
			this.scale = scale * 2;
		}

			public override float apply (float a) {
			if (a <= 0.5f) {
				a *= 2;
				return a * a * ((scale + 1) * a - scale) / 2;
			}
			a--;
			a *= 2;
			return a * a * ((scale + 1) * a + scale) / 2 + 1;
		}
	}

	public class SwingOut : Interpolation {
		private readonly float scale;

		public SwingOut (float scale) {
			this.scale = scale;
		}

			public override float apply (float a) {
			a--;
			return a * a * ((scale + 1) * a + scale) + 1;
		}
	}

	 public class SwingIn : Interpolation {
		private readonly float scale;

		public SwingIn (float scale) {
			this.scale = scale;
		}

		public override float apply (float a) {
			return a * a * ((scale + 1) * a - scale);
		}
	}
}
}
