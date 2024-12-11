using System;
using static SharpGDX.IInput;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.G3D.Utils;


namespace SharpGDX.Graphics.G3D.Utils;

/** Takes a {@link Camera} instance and controls it via w,a,s,d and mouse panning.
 * @author badlogic */
public class FirstPersonCameraController : InputAdapter {
	protected readonly Camera camera;
	protected readonly IntIntMap keys = new IntIntMap();
	public int strafeLeftKey = Keys.A;
	public int strafeRightKey = Keys.D;
	public int forwardKey = Keys.W;
	public int backwardKey = Keys.S;
	public int upKey = Keys.Q;
	public int downKey = Keys.E;
	public bool autoUpdate = true;
	protected float velocity = 5;
	protected float degreesPerPixel = 0.5f;
	protected readonly Vector3 tmp = new Vector3();

	public FirstPersonCameraController (Camera camera) {
		this.camera = camera;
	}

	public override bool KeyDown (int keycode) {
		keys.put(keycode, keycode);
		return true;
	}

	public override bool KeyUp (int keycode) {
		keys.remove(keycode, 0);
		return true;
	}

	/** Sets the velocity in units per second for moving forward, backward and strafing left/right.
	 * @param velocity the velocity in units per second */
	public void setVelocity (float velocity) {
		this.velocity = velocity;
	}

	/** Sets how many degrees to rotate per pixel the mouse moved.
	 * @param degreesPerPixel */
	public void setDegreesPerPixel (float degreesPerPixel) {
		this.degreesPerPixel = degreesPerPixel;
	}

	public override bool TouchDragged (int screenX, int screenY, int pointer) {
		float deltaX = -GDX.Input.GetDeltaX() * degreesPerPixel;
		float deltaY = -GDX.Input.GetDeltaY() * degreesPerPixel;
		camera.direction.rotate(camera.up, deltaX);
		tmp.Set(camera.direction).crs(camera.up).nor();
		camera.direction.rotate(tmp, deltaY);
		return true;
	}

	public void update () {
		update(GDX.Graphics.GetDeltaTime());
	}

	public void update (float deltaTime) {
		if (keys.containsKey(forwardKey)) {
			tmp.Set(camera.direction).nor().scl(deltaTime * velocity);
			camera.position.add(tmp);
		}
		if (keys.containsKey(backwardKey)) {
			tmp.Set(camera.direction).nor().scl(-deltaTime * velocity);
			camera.position.add(tmp);
		}
		if (keys.containsKey(strafeLeftKey)) {
			tmp.Set(camera.direction).crs(camera.up).nor().scl(-deltaTime * velocity);
			camera.position.add(tmp);
		}
		if (keys.containsKey(strafeRightKey)) {
			tmp.Set(camera.direction).crs(camera.up).nor().scl(deltaTime * velocity);
			camera.position.add(tmp);
		}
		if (keys.containsKey(upKey)) {
			tmp.Set(camera.up).nor().scl(deltaTime * velocity);
			camera.position.add(tmp);
		}
		if (keys.containsKey(downKey)) {
			tmp.Set(camera.up).nor().scl(-deltaTime * velocity);
			camera.position.add(tmp);
		}
		if (autoUpdate) camera.update(true);
	}
}
