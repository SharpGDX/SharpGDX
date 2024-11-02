//using SharpGDX.Graphics;
//using SharpGDX.Mathematics;
//
//namespace SharpGDX.Tests.Utils;
//
//public class PerspectiveCamController : InputAdapter
//{
//    private readonly static Vector3 tmpV = new Vector3();
//
//    public PerspectiveCamera cam;
//
//    enum TransformMode
//    {
//        Rotate,
//        Translate,
//        Zoom,
//        None
//    }
//
//    Vector3 lookAt = new Vector3();
//    TransformMode mode = TransformMode.Translate;
//    bool translated = false;
//
//    public PerspectiveCamController(PerspectiveCamera cam)
//    {
//        this.cam = cam;
//    }
//
//    public override bool TouchDown(int x, int y, int pointer, int button)
//    {
//        mode = TransformMode.Rotate;
//        last.set(x, y);
//        tCurr.set(x, y);
//        return true;
//    }
//
//    public override bool TouchUp(int x, int y, int pointer, int button)
//    {
//        mode = TransformMode.None;
//        return true;
//    }
//
//    Vector2 tCurr = new Vector2();
//    Vector2 last = new Vector2();
//    Vector2 delta = new Vector2();
//    Vector2 currWindow = new Vector2();
//    Vector2 lastWindow = new Vector2();
//    Vector3 curr3 = new Vector3();
//    Vector3 delta3 = new Vector3();
//    Plane lookAtPlane = new Plane(new Vector3(0, 1, 0), 0);
//    Matrix4 rotMatrix = new Matrix4();
//    Vector3 xAxis = new Vector3(1, 0, 0);
//    Vector3 yAxis = new Vector3(0, 1, 0);
//    Vector3 point = new Vector3();
//
//    public override bool TouchDragged(int x, int y, int pointer)
//    {
//        if (pointer != 0) return false;
//        delta.set(x, y).sub(last);
//
//        if (mode == TransformMode.Rotate)
//        {
//            point.set(cam.position).sub(lookAt);
//
//            if (tmpV.set(point).nor().dot(yAxis) < 0.9999f)
//            {
//                xAxis.set(cam.direction).crs(yAxis).nor();
//                rotMatrix.setToRotation(xAxis, delta.y / 5);
//                point.mul(rotMatrix);
//            }
//
//            rotMatrix.setToRotation(yAxis, -delta.x / 5);
//            point.mul(rotMatrix);
//
//            cam.position.set(point.add(lookAt));
//            cam.lookAt(lookAt.x, lookAt.y, lookAt.z);
//        }
//
//        if (mode == TransformMode.Zoom)
//        {
//            cam.FieldOfView -= -delta.y / 10;
//        }
//
//        if (mode == TransformMode.Translate)
//        {
//            tCurr.set(x, y);
//            translated = true;
//        }
//
//        cam.update();
//        last.set(x, y);
//        return true;
//    }
//
//    public override bool Scrolled(float amountX, float amountY)
//    {
//        cam.FieldOfView -= -amountY * Gdx.graphics.getDeltaTime() * 100;
//        cam.update();
//        return true;
//    }
//}