using SharpGDX.Graphics.G3D.Attributess;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Graphics.G3D.Utils;


namespace SharpGDX.Graphics.G3D.Particles.Utils;

public class DefaultRenderableSorter : RenderableSorter, IComparer<Renderable> {
	private Camera camera;
	private readonly Vector3 tmpV1 = new Vector3();
	private readonly Vector3 tmpV2 = new Vector3();

	public void sort ( Camera camera,  Array<Renderable> renderables) {
		this.camera = camera;
		renderables.sort(this);
	}

	private Vector3 getTranslation (Matrix4 worldTransform, Vector3 center, Vector3 output) {
		if (center.isZero())
			worldTransform.getTranslation(output);
		else if (!worldTransform.hasRotationOrScaling())
			worldTransform.getTranslation(output).add(center);
		else
			output.Set(center).mul(worldTransform);
		return output;
	}

	public int Compare ( Renderable o1,  Renderable o2) {
		 bool b1 = o1.material.has(BlendingAttribute.Type)
			&& ((BlendingAttribute)o1.material.get(BlendingAttribute.Type)).blended;
		 bool b2 = o2.material.has(BlendingAttribute.Type)
			&& ((BlendingAttribute)o2.material.get(BlendingAttribute.Type)).blended;
		if (b1 != b2) return b1 ? 1 : -1;
		// FIXME implement better sorting algorithm
		// final boolean same = o1.shader == o2.shader && o1.mesh == o2.mesh && (o1.lights == null) == (o2.lights == null) &&
		// o1.material.equals(o2.material);
		getTranslation(o1.worldTransform, o1.meshPart.center, tmpV1);
		getTranslation(o2.worldTransform, o2.meshPart.center, tmpV2);
		 float dst = (int)(1000f * camera.position.dst2(tmpV1)) - (int)(1000f * camera.position.dst2(tmpV2));
		 int result = dst < 0 ? -1 : (dst > 0 ? 1 : 0);
		return b1 ? -result : result;
	}
}
