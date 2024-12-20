﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics
{
	/** A camera with orthographic projection.
 * 
 * @author mzechner */
public class OrthographicCamera : Camera {
	/** the zoom of the camera **/
	public float zoom = 1;

	public OrthographicCamera () {
		this.near = 0;
	}

	/** Constructs a new OrthographicCamera, using the given viewport width and height. For pixel perfect 2D rendering just supply
	 * the screen size, for other unit scales (e.g. meters for box2d) proceed accordingly. The camera will show the region
	 * [-viewportWidth/2, -(viewportHeight/2-1)] - [(viewportWidth/2-1), viewportHeight/2]
	 * @param viewportWidth the viewport width
	 * @param viewportHeight the viewport height */
	public OrthographicCamera (float viewportWidth, float viewportHeight) {
		this.viewportWidth = viewportWidth;
		this.viewportHeight = viewportHeight;
		this.near = 0;
		update();
	}

	private readonly Vector3 tmp = new Vector3();

		public override void update () {
		update(true);
	}

	public override void update (bool updateFrustum) {
		projection.setToOrtho(zoom * -viewportWidth / 2, zoom * (viewportWidth / 2), zoom * -(viewportHeight / 2),
			zoom * viewportHeight / 2, near, far);
        view.setToLookAt(direction, up);
        view.translate(-position.x, -position.y, -position.z);
            Combined.set(projection);
		Matrix4.mul(Combined.val, view.val);

		if (updateFrustum) {
			invProjectionView.set(Combined);
			Matrix4.inv(invProjectionView.val);
			frustum.update(invProjectionView);
		}
	}

	/** Sets this camera to an orthographic projection using a viewport fitting the screen resolution, centered at
	 * (Gdx.graphics.getWidth()/2, Gdx.graphics.getHeight()/2), with the y-axis pointing up or down.
	 * @param yDown whether y should be pointing down */
	public void setToOrtho (bool yDown) {
		setToOrtho(yDown, GDX.Graphics.GetWidth(), GDX.Graphics.GetHeight());
	}

	/** Sets this camera to an orthographic projection, centered at (viewportWidth/2, viewportHeight/2), with the y-axis pointing
	 * up or down.
	 * @param yDown whether y should be pointing down.
	 * @param viewportWidth
	 * @param viewportHeight */
	public void setToOrtho (bool yDown, float viewportWidth, float viewportHeight) {
		if (yDown) {
			up.Set(0, -1, 0);
			direction.Set(0, 0, 1);
		} else {
			up.Set(0, 1, 0);
			direction.Set(0, 0, -1);
		}
		position.Set(zoom * viewportWidth / 2.0f, zoom * viewportHeight / 2.0f, 0);
		this.viewportWidth = viewportWidth;
		this.viewportHeight = viewportHeight;
		update();
	}

	/** Rotates the camera by the given angle around the direction vector. The direction and up vector will not be orthogonalized.
	 * @param angle */
	public void rotate (float angle) {
		rotate(direction, angle);
	}

	/** Moves the camera by the given amount on each axis.
	 * @param x the displacement on the x-axis
	 * @param y the displacement on the y-axis */
	public void translate (float x, float y) {
		translate(x, y, 0);
	}

	/** Moves the camera by the given vector.
	 * @param vec the displacement vector */
	public void translate (Vector2 vec) {
		translate(vec.x, vec.y, 0);
	}
}
}
