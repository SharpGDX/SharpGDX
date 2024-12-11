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


namespace SharpGDX.Graphics.G3D.Utils;

/** Base class for applying one or more {@link Animation}s to a {@link ModelInstance}. This class only applies the actual
 * {@link Node} transformations, it does not manage animations or keep track of animation states. See {@link AnimationController}
 * for an implementation of this class which does manage animations.
 * 
 * @author Xoppa */
public class BaseAnimationController {
	public sealed class Transform : IPoolable {
		public readonly Vector3 translation = new Vector3();
		public readonly Quaternion rotation = new Quaternion();
		public readonly Vector3 scale = new Vector3(1, 1, 1);

		public Transform () {
		}

		public Transform idt () {
			translation.Set(0, 0, 0);
			rotation.idt();
			scale.Set(1, 1, 1);
			return this;
		}

		public Transform set ( Vector3 t,  Quaternion r,  Vector3 s) {
			translation.Set(t);
			rotation.set(r);
			scale.Set(s);
			return this;
		}

		public Transform set ( Transform other) {
			return set(other.translation, other.rotation, other.scale);
		}

		public Transform lerp ( Transform target,  float alpha) {
			return lerp(target.translation, target.rotation, target.scale, alpha);
		}

		public Transform lerp ( Vector3 targetT,  Quaternion targetR,  Vector3 targetS,  float alpha) {
			translation.lerp(targetT, alpha);
			rotation.slerp(targetR, alpha);
			scale.lerp(targetS, alpha);
			return this;
		}

		public Matrix4 toMatrix4 ( Matrix4 @out) {
			return @out.set(translation, rotation, scale);
		}

		public void Reset () {
			idt();
		}

		public override String ToString () {
			return translation.ToString() + " - " + rotation.ToString() + " - " + scale.ToString();
		}
	}

    private class TransformPool : Pool<Transform>
    {
        internal protected override Transform newObject()
        {
            return new Transform();
        }
    }

	private readonly Pool<Transform> transformPool = new TransformPool();
	private readonly static ObjectMap<Node, Transform> transforms = new ObjectMap<Node, Transform>();
	private bool applying = false;
	/** The {@link ModelInstance} on which the animations are being performed. */
	public readonly ModelInstance target;

	/** Construct a new BaseAnimationController.
	 * @param target The {@link ModelInstance} on which the animations are being performed. */
	public BaseAnimationController (ModelInstance target) {
		this.target = target;
	}

	/** Begin applying multiple animations to the instance, must followed by one or more calls to {
	 * {@link #apply(Animation, float, float)} and finally {{@link #end()}. */
	protected void begin () {
		if (applying) throw new GdxRuntimeException("You must call end() after each call to being()");
		applying = true;
	}

	/** Apply an animation, must be called between {{@link #begin()} and {{@link #end()}.
	 * @param weight The blend weight of this animation relative to the previous applied animations. */
	protected void apply ( Animation animation,  float time,  float weight) {
		if (!applying) throw new GdxRuntimeException("You must call begin() before adding an animation");
		applyAnimation(transforms, transformPool, weight, animation, time);
	}

	/** End applying multiple animations to the instance and update it to reflect the changes. */
	protected void end () {
		if (!applying) throw new GdxRuntimeException("You must call begin() first");
		foreach (var entry in transforms.entries()) {
			entry.value.toMatrix4(entry.key.localTransform);
			transformPool.free(entry.value);
		}
		transforms.clear();
		target.calculateTransforms();
		applying = false;
	}

	/** Apply a single animation to the {@link ModelInstance} and update the it to reflect the changes. */
	protected void applyAnimation ( Animation animation,  float time) {
		if (applying) throw new GdxRuntimeException("Call end() first");
		applyAnimation(null, null, 1.0f, animation, time);
		target.calculateTransforms();
	}

	/** Apply two animations, blending the second onto to first using weight. */
	protected void applyAnimations ( Animation anim1,  float time1,  Animation anim2,  float time2,
		 float weight) {
		if (anim2 == null || weight == 0.0f)
			applyAnimation(anim1, time1);
		else if (anim1 == null || weight == 1.0f)
			applyAnimation(anim2, time2);
		else if (applying)
			throw new GdxRuntimeException("Call end() first");
		else {
			begin();
			apply(anim1, time1, 1.0f);
			apply(anim2, time2, weight);
			end();
		}
	}

	private readonly static Transform tmpT = new Transform();

	/** Find first key frame index just before a given time
	 * @param arr Key frames ordered by time ascending
	 * @param time Time to search
	 * @return key frame index, 0 if time is out of key frames time range */
	 static  int getFirstKeyframeIndexAtTime<T>( Array<NodeKeyframe<T>> arr,  float time) {
		 int lastIndex = arr.size - 1;

		// edges cases : time out of range always return first index
		if (lastIndex <= 0 || time < arr.Get(0).keytime || time > arr.Get(lastIndex).keytime) {
			return 0;
		}

		// binary search
		int minIndex = 0;
		int maxIndex = lastIndex;

		while (minIndex < maxIndex) {
			int i = (minIndex + maxIndex) / 2;
			if (time > arr.Get(i + 1).keytime) {
				minIndex = i + 1;
			} else if (time < arr.Get(i).keytime) {
				maxIndex = i - 1;
			} else {
				return i;
			}
		}
		return minIndex;
	}

	private  static Vector3 getTranslationAtTime ( NodeAnimation nodeAnim,  float time,  Vector3 @out) {
		if (nodeAnim.translation == null) return @out.Set(nodeAnim.node.translation);
		if (nodeAnim.translation.size == 1) return @out.Set(nodeAnim.translation.Get(0).value);

		int index = getFirstKeyframeIndexAtTime(nodeAnim.translation, time);
		 var firstKeyframe = nodeAnim.translation.Get(index);
		@out.Set((Vector3)firstKeyframe.value);

		if (++index < nodeAnim.translation.size) {
			 NodeKeyframe<Vector3> secondKeyframe = nodeAnim.translation.Get(index);
			 float t = (time - firstKeyframe.keytime) / (secondKeyframe.keytime - firstKeyframe.keytime);
			@out.lerp(secondKeyframe.value, t);
		}
		return @out;
	}

	private  static Quaternion getRotationAtTime ( NodeAnimation nodeAnim,  float time,  Quaternion @out) {
		if (nodeAnim.rotation == null) return @out.set(nodeAnim.node.rotation);
		if (nodeAnim.rotation.size == 1) return @out.set(nodeAnim.rotation.Get(0).value);

		int index = getFirstKeyframeIndexAtTime(nodeAnim.rotation, time);
		 var firstKeyframe = nodeAnim.rotation.Get(index);
		@out.set((Quaternion)firstKeyframe.value);

		if (++index < nodeAnim.rotation.size) {
			 NodeKeyframe<Quaternion> secondKeyframe = nodeAnim.rotation.Get(index);
			 float t = (time - firstKeyframe.keytime) / (secondKeyframe.keytime - firstKeyframe.keytime);
			@out.slerp(secondKeyframe.value, t);
		}
		return @out;
	}

	private  static Vector3 getScalingAtTime ( NodeAnimation nodeAnim,  float time,  Vector3 @out) {
		if (nodeAnim.scaling == null) return @out.Set(nodeAnim.node.scale);
		if (nodeAnim.scaling.size == 1) return @out.Set(nodeAnim.scaling.Get(0).value);

		int index = getFirstKeyframeIndexAtTime(nodeAnim.scaling, time);
		 var firstKeyframe = nodeAnim.scaling.Get(index);
		@out.Set((Vector3)firstKeyframe.value);

		if (++index < nodeAnim.scaling.size) {
			 NodeKeyframe<Vector3> secondKeyframe = nodeAnim.scaling.Get(index);
			 float t = (time - firstKeyframe.keytime) / (secondKeyframe.keytime - firstKeyframe.keytime);
			@out.lerp(secondKeyframe.value, t);
		}
		return @out;
	}

	private  static Transform getNodeAnimationTransform ( NodeAnimation nodeAnim,  float time) {
		 Transform transform = tmpT;
		getTranslationAtTime(nodeAnim, time, transform.translation);
		getRotationAtTime(nodeAnim, time, transform.rotation);
		getScalingAtTime(nodeAnim, time, transform.scale);
		return transform;
	}

	private  static void applyNodeAnimationDirectly ( NodeAnimation nodeAnim,  float time) {
		 Node node = nodeAnim.node;
		node.isAnimated = true;
		 Transform transform = getNodeAnimationTransform(nodeAnim, time);
		transform.toMatrix4(node.localTransform);
	}

	private  static void applyNodeAnimationBlending ( NodeAnimation nodeAnim,  ObjectMap<Node, Transform> @out,
		 Pool<Transform> pool,  float alpha,  float time) {

		 Node node = nodeAnim.node;
		node.isAnimated = true;
		 Transform transform = getNodeAnimationTransform(nodeAnim, time);

		Transform t = @out.get(node, null);
		if (t != null) {
			if (alpha > 0.999999f)
				t.set(transform);
			else
				t.lerp(transform, alpha);
		} else {
			if (alpha > 0.999999f)
				@out.put(node, pool.obtain().set(transform));
			else
				@out.put(node, pool.obtain().set(node.translation, node.rotation, node.scale).lerp(transform, alpha));
		}
	}

	/** Helper method to apply one animation to either an objectmap for blending or directly to the bones. */
	protected static void applyAnimation ( ObjectMap<Node, Transform> @out,  Pool<Transform> pool,  float alpha,
		 Animation animation,  float time) {

		if (@out == null) {
            foreach ( NodeAnimation nodeAnim in animation.nodeAnimations)
				applyNodeAnimationDirectly(nodeAnim, time);
		} else {
            foreach ( Node node in @out.keys())
				node.isAnimated = false;
            foreach ( NodeAnimation nodeAnim in animation.nodeAnimations)
				applyNodeAnimationBlending(nodeAnim, @out, pool, alpha, time);
			foreach ( var e in @out.entries()) {
				if (!e.key.isAnimated) {
					e.key.isAnimated = true;
					e.value.lerp(e.key.translation, e.key.rotation, e.key.scale, alpha);
				}
			}
		}
	}

	/** Remove the specified animation, by marking the affected nodes as not animated. When switching animation, this should be
	 * call prior to applyAnimation(s). */
	protected void removeAnimation ( Animation animation) {
        foreach ( NodeAnimation nodeAnim in animation.nodeAnimations) {
			nodeAnim.node.isAnimated = false;
		}
	}
}
