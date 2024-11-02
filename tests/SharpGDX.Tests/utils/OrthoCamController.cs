//using SharpGDX.Graphics;
//using SharpGDX.Mathematics;
//
//namespace SharpGDX.Tests.Utils;
//
//public class OrthoCamController : InputAdapter {
//	readonly OrthographicCamera camera;
//	readonly Vector3 curr = new Vector3();
//	readonly Vector3 last = new Vector3(-1, -1, -1);
//	readonly Vector3 delta = new Vector3();
//
//	public OrthoCamController (OrthographicCamera camera) {
//		this.camera = camera;
//	}
//
//	public override bool TouchDragged (int x, int y, int pointer) {
//		camera.unproject(curr.set(x, y, 0));
//		if (!(last.x == -1 && last.y == -1 && last.z == -1)) {
//			camera.unproject(delta.set(last.x, last.y, 0));
//			delta.sub(curr);
//			camera.position.add(delta.x, delta.y, 0);
//		}
//		last.set(x, y, 0);
//		return false;
//	}
//
//	public override bool TouchUp (int x, int y, int pointer, int button) {
//		last.set(-1, -1, -1);
//		return false;
//	}
//}
