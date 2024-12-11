using System;
using SharpGDX.Graphics.GLUtils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D.Utils.ShapeBuilders;

/** FrustumShapeBuilder builds camera or frustum.
 * 
 * @author realitix */
public class FrustumShapeBuilder : BaseShapeBuilder {

	/** Build camera with default colors
	 * @param builder MeshPartBuilder
	 * @param camera Camera */
	public static void build (MeshPartBuilder builder, Camera camera) {
		build(builder, camera, tmpColor0.Set(1, 0.66f, 0, 1), tmpColor1.Set(1, 0, 0, 1), tmpColor2.Set(0, 0.66f, 1, 1),
			tmpColor3.Set(1, 1, 1, 1), tmpColor4.Set(0.2f, 0.2f, 0.2f, 1));
	}

	/** Build Camera with custom colors
	 * @param builder
	 * @param camera
	 * @param frustumColor
	 * @param coneColor
	 * @param upColor
	 * @param targetColor
	 * @param crossColor */
	public static void build (MeshPartBuilder builder, Camera camera, Color frustumColor, Color coneColor, Color upColor,
		Color targetColor, Color crossColor) {
		Vector3[] planePoints = camera.frustum.planePoints;

		// Frustum
		build(builder, camera.frustum, frustumColor, crossColor);

		// Cones (camera position to near plane)
		builder.line(planePoints[0], coneColor, camera.position, coneColor);
		builder.line(planePoints[1], coneColor, camera.position, coneColor);
		builder.line(planePoints[2], coneColor, camera.position, coneColor);
		builder.line(planePoints[3], coneColor, camera.position, coneColor);

		// Target line
		builder.line(camera.position, targetColor, centerPoint(planePoints[4], planePoints[5], planePoints[6]), targetColor);

		// Up triangle
		float halfNearSize = tmpV0.Set(planePoints[1]).sub(planePoints[0]).scl(0.5f).len();
		Vector3 centerNear = centerPoint(planePoints[0], planePoints[1], planePoints[2]);
		tmpV0.Set(camera.up).scl(halfNearSize * 2);
		centerNear.add(tmpV0);

		builder.line(centerNear, upColor, planePoints[2], upColor);
		builder.line(planePoints[2], upColor, planePoints[3], upColor);
		builder.line(planePoints[3], upColor, centerNear, upColor);
	}

	/** Build Frustum with custom colors
	 * @param builder
	 * @param frustum
	 * @param frustumColor
	 * @param crossColor */
	public static void build (MeshPartBuilder builder, Frustum frustum, Color frustumColor, Color crossColor) {
		Vector3[] planePoints = frustum.planePoints;

		// Near
		builder.line(planePoints[0], frustumColor, planePoints[1], frustumColor);
		builder.line(planePoints[1], frustumColor, planePoints[2], frustumColor);
		builder.line(planePoints[2], frustumColor, planePoints[3], frustumColor);
		builder.line(planePoints[3], frustumColor, planePoints[0], frustumColor);

		// Far
		builder.line(planePoints[4], frustumColor, planePoints[5], frustumColor);
		builder.line(planePoints[5], frustumColor, planePoints[6], frustumColor);
		builder.line(planePoints[6], frustumColor, planePoints[7], frustumColor);
		builder.line(planePoints[7], frustumColor, planePoints[4], frustumColor);

		// Sides
		builder.line(planePoints[0], frustumColor, planePoints[4], frustumColor);
		builder.line(planePoints[1], frustumColor, planePoints[5], frustumColor);
		builder.line(planePoints[2], frustumColor, planePoints[6], frustumColor);
		builder.line(planePoints[3], frustumColor, planePoints[7], frustumColor);

		// Cross near
		builder.line(middlePoint(planePoints[1], planePoints[0]), crossColor, middlePoint(planePoints[3], planePoints[2]),
			crossColor);
		builder.line(middlePoint(planePoints[2], planePoints[1]), crossColor, middlePoint(planePoints[3], planePoints[0]),
			crossColor);

		// Cross far
		builder.line(middlePoint(planePoints[5], planePoints[4]), crossColor, middlePoint(planePoints[7], planePoints[6]),
			crossColor);
		builder.line(middlePoint(planePoints[6], planePoints[5]), crossColor, middlePoint(planePoints[7], planePoints[4]),
			crossColor);
	}

	/** Return middle point's segment
	 * @param point0 First segment's point
	 * @param point1 Second segment's point
	 * @return the middle point */
	private static Vector3 middlePoint (Vector3 point0, Vector3 point1) {
		tmpV0.Set(point1).sub(point0).scl(0.5f);
		return tmpV1.Set(point0).add(tmpV0);
	}

	/** Return center point's rectangle
	 * @param point0
	 * @param point1
	 * @param point2
	 * @return the center point */
	private static Vector3 centerPoint (Vector3 point0, Vector3 point1, Vector3 point2) {
		tmpV0.Set(point1).sub(point0).scl(0.5f);
		tmpV1.Set(point0).add(tmpV0);
		tmpV0.Set(point2).sub(point1).scl(0.5f);
		return tmpV1.add(tmpV0);
	}
}
