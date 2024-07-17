using SharpGDX.Mathematics;

// TODO: This could probably be done with a delegate/Func<> instead of all these nested classes.
namespace SharpGDX.Utils
{
	/** Various scaling types for fitting one rectangle into another.
 * @author Nathan Sweet */
	public abstract class Scaling
	{
		protected static readonly Vector2 temp = new Vector2();

		/** Returns the size of the source scaled to the target. Note the same Vector2 instance is always returned and should never be
		 * cached. */
		public abstract Vector2 apply(float sourceWidth, float sourceHeight, float targetWidth, float targetHeight);

		/** Scales the source to fit the target while keeping the same aspect ratio. This may cause the source to be smaller than the
		 * target in one direction. */
		public static readonly Scaling fit = new FitScaling() { };

		private class FitScaling : Scaling
		{
			public override Vector2 apply(float sourceWidth, float sourceHeight, float targetWidth, float targetHeight)
			{
				float targetRatio = targetHeight / targetWidth;
				float sourceRatio = sourceHeight / sourceWidth;
				float scale = targetRatio > sourceRatio ? targetWidth / sourceWidth : targetHeight / sourceHeight;
				temp.x = sourceWidth * scale;
				temp.y = sourceHeight * scale;
				return temp;
			}
		}

		/** Scales the source to fit the target while keeping the same aspect ratio, but the source is not scaled at all if smaller in
		 * both directions. This may cause the source to be smaller than the target in one or both directions. */
		public static readonly Scaling contain = new ContainScaling()
		{

		};

		private class ContainScaling : Scaling
		{
			public override Vector2 apply(float sourceWidth, float sourceHeight, float targetWidth, float targetHeight)
			{
				float targetRatio = targetHeight / targetWidth;
				float sourceRatio = sourceHeight / sourceWidth;
				float scale = targetRatio > sourceRatio ? targetWidth / sourceWidth : targetHeight / sourceHeight;
				if (scale > 1) scale = 1;
				temp.x = sourceWidth * scale;
				temp.y = sourceHeight * scale;
				return temp;
			}
		}

		/** Scales the source to fill the target while keeping the same aspect ratio. This may cause the source to be larger than the
		 * target in one direction. */
		public static readonly Scaling fill = new FillScaling()
		{

		};

		private class FillScaling : Scaling
		{
			public override Vector2 apply(float sourceWidth, float sourceHeight, float targetWidth, float targetHeight)
			{
				float targetRatio = targetHeight / targetWidth;
				float sourceRatio = sourceHeight / sourceWidth;
				float scale = targetRatio < sourceRatio ? targetWidth / sourceWidth : targetHeight / sourceHeight;
				temp.x = sourceWidth * scale;
				temp.y = sourceHeight * scale;
				return temp;
			}
		}

		/** Scales the source to fill the target in the x direction while keeping the same aspect ratio. This may cause the source to
		 * be smaller or larger than the target in the y direction. */
		public static readonly Scaling fillX = new FillXScaling()
		{

		};

		private class FillXScaling : Scaling
		{
			public override Vector2 apply(float sourceWidth, float sourceHeight, float targetWidth, float targetHeight)
			{
				float scale = targetWidth / sourceWidth;
				temp.x = sourceWidth * scale;
				temp.y = sourceHeight * scale;
				return temp;
			}
		}

		/** Scales the source to fill the target in the y direction while keeping the same aspect ratio. This may cause the source to
		 * be smaller or larger than the target in the x direction. */
		public static readonly Scaling fillY = new FillYScaling()
		{

		};

		private class FillYScaling : Scaling
		{
			public override Vector2 apply(float sourceWidth, float sourceHeight, float targetWidth, float targetHeight)
			{
				float scale = targetHeight / sourceHeight;
				temp.x = sourceWidth * scale;
				temp.y = sourceHeight * scale;
				return temp;
			}
		}

		/** Scales the source to fill the target. This may cause the source to not keep the same aspect ratio. */
		public static readonly Scaling stretch = new StretchScaling()
		{

		};

		private class StretchScaling : Scaling
		{
			public override Vector2 apply(float sourceWidth, float sourceHeight, float targetWidth, float targetHeight)
			{
				temp.x = targetWidth;
				temp.y = targetHeight;
				return temp;
			}
		}

		/** Scales the source to fill the target in the x direction, without changing the y direction. This may cause the source to not
		 * keep the same aspect ratio. */
		public static readonly Scaling stretchX = new StretchXScaling()
		{

		};

		private class StretchXScaling : Scaling
		{
			public override Vector2 apply(float sourceWidth, float sourceHeight, float targetWidth, float targetHeight)
			{
				temp.x = targetWidth;
				temp.y = sourceHeight;
				return temp;
			}
		}

		/** Scales the source to fill the target in the y direction, without changing the x direction. This may cause the source to not
		 * keep the same aspect ratio. */
		public static readonly Scaling stretchY = new StretchYScaling()
		{

		};

		private class StretchYScaling : Scaling
		{
			public override Vector2 apply(float sourceWidth, float sourceHeight, float targetWidth, float targetHeight)
			{
				temp.x = sourceWidth;
				temp.y = targetHeight;
				return temp;
			}
		}

		/** The source is not scaled. */
		public static readonly Scaling none = new NoneScaling()
		{

		};

		private class NoneScaling : Scaling
		{
			public override Vector2 apply(float sourceWidth, float sourceHeight, float targetWidth, float targetHeight)
			{
				temp.x = sourceWidth;
				temp.y = sourceHeight;
				return temp;
			}
		}
	}
}