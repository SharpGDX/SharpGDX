namespace SharpGDX.Mathematics
{
	public interface IShape2D
	{

		/// <summary>
		/// Returns whether the given point is contained within the shape.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		bool contains(Vector2 point);

		/// <summary>
		/// Returns whether a point with the given coordinates is contained within the shape.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		bool contains(float x, float y);

	}
}
