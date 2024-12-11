using System;
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
using static SharpGDX.IInput;
using SharpGDX.Input;

namespace SharpGDX.Graphics.G3D.Utils;

public class CameraInputController : GestureDetector {
	/** The button for rotating the camera. */
	public int rotateButton = Buttons.Left;
	/** The angle to rotate when moved the full width or height of the screen. */
	public float rotateAngle = 360f;
	/** The button for translating the camera along the up/right plane */
	public int translateButton = Buttons.Right;
	/** The units to translate the camera when moved the full width or height of the screen. */
	public float translateUnits = 10f; // FIXME auto calculate this based on the target
	/** The button for translating the camera along the direction axis */
	public int forwardButton = Buttons.Middle;
	/** The key which must be pressed to activate rotate, translate and forward or 0 to always activate. */
	public int activateKey = 0;
	/** Indicates if the activateKey is currently being pressed. */
	protected bool activatePressed;
	/** Whether scrolling requires the activeKey to be pressed (false) or always allow scrolling (true). */
	public bool alwaysScroll = true;
	/** The weight for each scrolled amount. */
	public float scrollFactor = -0.1f;
	/** World units per screen size */
	public float pinchZoomFactor = 10f;
	/** Whether to update the camera after it has been changed. */
	public bool autoUpdate = true;
	/** The target to rotate around. */
	public Vector3 target = new Vector3();
	/** Whether to update the target on translation */
	public bool translateTarget = true;
	/** Whether to update the target on forward */
	public bool forwardTarget = true;
	/** Whether to update the target on scroll */
	public bool scrollTarget = false;
	public int forwardKey = Keys.W;
	protected bool forwardPressed;
	public int backwardKey = Keys.S;
	protected bool backwardPressed;
	public int rotateRightKey = Keys.A;
	protected bool rotateRightPressed;
	public int rotateLeftKey = Keys.D;
	protected bool rotateLeftPressed;
	protected bool controlsInverted;
	/** The camera. */
	public Camera camera;
	/** The current (first) button being pressed. */
	protected int button = -1;

	private float startX, startY;
	private readonly Vector3 tmpV1 = new Vector3();
	private readonly Vector3 tmpV2 = new Vector3();

	protected class CameraGestureListener : GestureAdapter {
		public CameraInputController controller;
		private float previousZoom;

        public override bool touchDown (float x, float y, int pointer, int button) {
			previousZoom = 0;
			return false;
		}

        public override bool tap (float x, float y, int count, int button) {
			return false;
		}

        public override bool longPress (float x, float y) {
			return false;
		}

        public override bool fling (float velocityX, float velocityY, int button) {
			return false;
		}

        public override bool pan (float x, float y, float deltaX, float deltaY) {
			return false;
		}

		public override bool zoom (float initialDistance, float distance) {
			float newZoom = distance - initialDistance;
			float amount = newZoom - previousZoom;
			previousZoom = newZoom;
			float w = GDX.Graphics.GetWidth(), h = GDX.Graphics.GetHeight();
			return controller.pinchZoom(amount / ((w > h) ? h : w));
		}

		public override bool pinch (Vector2 initialPointer1, Vector2 initialPointer2, Vector2 pointer1, Vector2 pointer2) {
			return false;
		}
	};

	protected readonly CameraGestureListener gestureListener;

	protected CameraInputController ( CameraGestureListener gestureListener,  Camera camera) 
    : base(gestureListener)
    {
		
		this.gestureListener = gestureListener;
		this.gestureListener.controller = this;
		this.camera = camera;
	}

	public CameraInputController ( Camera camera) 
    : this(new CameraGestureListener(), camera)
    {
		
	}

	public void update () {
		if (rotateRightPressed || rotateLeftPressed || forwardPressed || backwardPressed) {
			 float delta = GDX.Graphics.GetDeltaTime();
			if (rotateRightPressed) camera.rotate(camera.up, -delta * rotateAngle);
			if (rotateLeftPressed) camera.rotate(camera.up, delta * rotateAngle);
			if (forwardPressed) {
				camera.translate(tmpV1.Set(camera.direction).scl(delta * translateUnits));
				if (forwardTarget) target.add(tmpV1);
			}
			if (backwardPressed) {
				camera.translate(tmpV1.Set(camera.direction).scl(-delta * translateUnits));
				if (forwardTarget) target.add(tmpV1);
			}
			if (autoUpdate) camera.update();
		}
	}

	private int touched;
	private bool multiTouch;

	public override bool TouchDown (int screenX, int screenY, int pointer, int button) {
		touched |= (1 << pointer);
		multiTouch = !MathUtils.isPowerOfTwo(touched);
		if (multiTouch)
			this.button = -1;
		else if (this.button < 0 && (activateKey == 0 || activatePressed)) {
			startX = screenX;
			startY = screenY;
			this.button = button;
		}
		return base.TouchDown(screenX, screenY, pointer, button) || (activateKey == 0 || activatePressed);
	}

	public override bool TouchUp (int screenX, int screenY, int pointer, int button) {
		touched &= -1 ^ (1 << pointer);
		multiTouch = !MathUtils.isPowerOfTwo(touched);
		if (button == this.button) this.button = -1;
		return base.TouchUp(screenX, screenY, pointer, button) || activatePressed;
	}

	/** Sets the CameraInputControllers' control inversion.
	 * @param invertControls Whether or not to invert the controls */
	public void setInvertedControls (bool invertControls) {
		if (this.controlsInverted != invertControls) {
			// Flip the rotation angle
			this.rotateAngle = -this.rotateAngle;
		}
		this.controlsInverted = invertControls;
	}
	
	protected bool process (float deltaX, float deltaY, int button) {
		if (button == rotateButton) {
			tmpV1.Set(camera.direction).crs(camera.up).y = 0f;
			camera.rotateAround(target, tmpV1.nor(), deltaY * rotateAngle);
			camera.rotateAround(target, Vector3.Y, deltaX * -rotateAngle);
		} else if (button == translateButton) {
			camera.translate(tmpV1.Set(camera.direction).crs(camera.up).nor().scl(-deltaX * translateUnits));
			camera.translate(tmpV2.Set(camera.up).scl(-deltaY * translateUnits));
			if (translateTarget) target.add(tmpV1).add(tmpV2);
		} else if (button == forwardButton) {
			camera.translate(tmpV1.Set(camera.direction).scl(deltaY * translateUnits));
			if (forwardTarget) target.add(tmpV1);
		}
		if (autoUpdate) camera.update();
		return true;
	}

	public override bool TouchDragged (int screenX, int screenY, int pointer) {
		bool result = base.TouchDragged(screenX, screenY, pointer);
		if (result || this.button < 0) return result;
		 float deltaX = (screenX - startX) / GDX.Graphics.GetWidth();
		 float deltaY = (startY - screenY) / GDX.Graphics.GetHeight();
		startX = screenX;
		startY = screenY;
		return process(deltaX, deltaY, button);
	}

	public override bool Scrolled (float amountX, float amountY) {
		return zoom(amountY * scrollFactor * translateUnits);
	}

	public bool zoom (float amount) {
		if (!alwaysScroll && activateKey != 0 && !activatePressed) return false;
		camera.translate(tmpV1.Set(camera.direction).scl(amount));
		if (scrollTarget) target.add(tmpV1);
		if (autoUpdate) camera.update();
		return true;
	}

	protected bool pinchZoom (float amount) {
		return zoom(pinchZoomFactor * amount);
	}

	public override bool KeyDown (int keycode) {
		if (keycode == activateKey) activatePressed = true;
		if (keycode == forwardKey)
			forwardPressed = true;
		else if (keycode == backwardKey)
			backwardPressed = true;
		else if (keycode == rotateRightKey)
			rotateRightPressed = true;
		else if (keycode == rotateLeftKey) rotateLeftPressed = true;
		return false;
	}

    public override bool KeyUp (int keycode) {
		if (keycode == activateKey) {
			activatePressed = false;
			button = -1;
		}
		if (keycode == forwardKey)
			forwardPressed = false;
		else if (keycode == backwardKey)
			backwardPressed = false;
		else if (keycode == rotateRightKey)
			rotateRightPressed = false;
		else if (keycode == rotateLeftKey) rotateLeftPressed = false;
		return false;
	}
}
