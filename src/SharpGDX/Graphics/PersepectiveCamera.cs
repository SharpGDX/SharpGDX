using SharpGDX.Mathematics;

namespace SharpGDX.Graphics;

/// <summary>
///     A Camera with perspective projection.
/// </summary>
public class PerspectiveCamera : Camera
{
	/// <summary>
	///     The field of view of the height, in degrees.
	/// </summary>
	public float FieldOfView = 67;

	private readonly Vector3 tmp = new();

	public PerspectiveCamera()
	{
	}

	/// <summary>
	///     Constructs a new {@link PerspectiveCamera} with the given field of view and viewport size.
	/// </summary>
	/// <remarks>
	///     The aspect ratio is derived from the viewport size.
	/// </remarks>
	/// <param name="fieldOfViewY">
	///     The field of view of the height, in degrees.
	///     <remarks>
	///         <para>
	///             The field of view for the width will be calculated according to The aspect ratio.
	///         </para>
	///     </remarks>
	/// </param>
	/// <param name="viewportWidth">The viewport width.</param>
	/// <param name="viewportHeight">The viewport height.</param>
	public PerspectiveCamera(float fieldOfViewY, float viewportWidth, float viewportHeight)
	{
		FieldOfView = fieldOfViewY;
		this.viewportWidth = viewportWidth;
		this.viewportHeight = viewportHeight;
		update();
	}

	public override void update()
	{
		update(true);
	}

	public override void update(bool updateFrustum)
	{
		var aspect = viewportWidth / viewportHeight;
		projection.setToProjection(Math.Abs(near), Math.Abs(far), FieldOfView, aspect);
		view.setToLookAt(position, tmp.set(position).add(direction), up);
		combined.set(projection);
		Matrix4.mul(combined.val, view.val);

		if (!updateFrustum)
		{
			return;
		}

		invProjectionView.set(combined);
		Matrix4.inv(invProjectionView.val);
		frustum.update(invProjectionView);
	}
}