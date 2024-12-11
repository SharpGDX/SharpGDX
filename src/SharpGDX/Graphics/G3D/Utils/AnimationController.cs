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

/** Class to control one or more {@link Animation}s on a {@link ModelInstance}. Use the
 * {@link #setAnimation(String, int, float, AnimationListener)} method to change the current animation. Use the
 * {@link #animate(String, int, float, AnimationListener, float)} method to start an animation, optionally blending onto the
 * current animation. Use the {@link #queue(String, int, float, AnimationListener, float)} method to queue an animation to be
 * played when the current animation is finished. Use the {@link #action(String, int, float, AnimationListener, float)} method to
 * play a (short) animation on top of the current animation.
 * 
 * You can use multiple AnimationControllers on the same ModelInstance, as long as they don't interfere with each other (don't
 * affect the same {@link Node}s).
 * 
 * @author Xoppa */
public class AnimationController : BaseAnimationController {

	/** Listener that will be informed when an animation is looped or completed.
	 * @author Xoppa */
	public interface AnimationListener {
		/** Gets called when an animation is completed.
		 * @param animation The animation which just completed. */
		void onEnd ( AnimationDesc animation);

		/** Gets called when an animation is looped. The {@link AnimationDesc#loopCount} is updated prior to this call and can be
		 * read or written to alter the number of remaining loops.
		 * @param animation The animation which just looped. */
		void onLoop ( AnimationDesc animation);
	}

	/** Class describing how to play and {@link Animation}. You can read the values within this class to get the progress of the
	 * animation. Do not change the values. Only valid when the animation is currently played.
	 * @author Xoppa */
	public  class AnimationDesc {
		/** Listener which will be informed when the animation is looped or ended. */
		public AnimationListener listener;
		/** The animation to be applied. */
		public Animation animation;
		/** The speed at which to play the animation (can be negative), 1.0 for normal speed. */
		public float speed;
		/** The current animation time. */
		public float time;
		/** The offset within the animation (animation time = offsetTime + time) */
		public float offset;
		/** The duration of the animation */
		public float duration;
		/** The number of remaining loops, negative for continuous, zero if stopped. */
		public int loopCount;

		internal protected AnimationDesc () {
		}

		/** @param delta delta time, must be positive.
		 * @return the remaining time or -1 if still animating. */
		internal protected float update (float delta) {
			if (loopCount != 0 && animation != null) {
				int loops;
				 float diff = speed * delta;
				if (!MathUtils.isZero(duration)) {
					time += diff;
					if (speed < 0) {
						float invTime = duration - time;
						loops = (int)Math.Abs(invTime / duration);
						invTime = Math.Abs(invTime % duration);
						time = duration - invTime;
					} else {
						loops = (int)Math.Abs(time / duration);
						time = Math.Abs(time % duration);
					}
				} else
					loops = 1;
				for (int i = 0; i < loops; i++) {
					if (loopCount > 0) loopCount--;
					if (loopCount != 0 && listener != null) listener.onLoop(this);
					if (loopCount == 0) {
						 float result = ((loops - 1) - i) * duration + (diff < 0f ? duration - time : time);
						time = (diff < 0f) ? 0f : duration;
						if (listener != null) listener.onEnd(this);
						return result;
					}
				}
				return -1;
			} else
				return delta;
		}
	}

    private class AnimationPool : Pool<AnimationDesc>
    {
        protected internal override AnimationDesc newObject()
        {
            return new AnimationDesc();
        }
    }

	protected readonly Pool<AnimationDesc> animationPool = new AnimationPool();

	/** The animation currently playing. Do not alter this value. */
	public AnimationDesc current;
	/** The animation queued to be played when the {@link #current} animation is completed. Do not alter this value. */
	public AnimationDesc queued;
	/** The transition time which should be applied to the queued animation. Do not alter this value. */
	public float queuedTransitionTime;
	/** The animation which previously played. Do not alter this value. */
	public AnimationDesc previous;
	/** The current transition time. Do not alter this value. */
	public float transitionCurrentTime;
	/** The target transition time. Do not alter this value. */
	public float transitionTargetTime;
	/** Whether an action is being performed. Do not alter this value. */
	public bool inAction;
	/** When true a call to {@link #update(float)} will not be processed. */
	public bool paused;
	/** Whether to allow the same animation to be played while playing that animation. */
	public bool allowSameAnimation;

	private bool justChangedAnimation = false;

	/** Construct a new AnimationController.
	 * @param target The {@link ModelInstance} on which the animations will be performed. */
	public AnimationController ( ModelInstance target) 
    : base(target)
    {
		
	}

	private AnimationDesc obtain ( Animation anim, float offset, float duration, int loopCount, float speed,
		 AnimationListener listener) {
		if (anim == null) return null;
		 AnimationDesc result = animationPool.obtain();
		result.animation = anim;
		result.listener = listener;
		result.loopCount = loopCount;
		result.speed = speed;
		result.offset = offset;
		result.duration = duration < 0 ? (anim.duration - offset) : duration;
		result.time = speed < 0 ? result.duration : 0.0f;
		return result;
	}

	private AnimationDesc obtain ( String id, float offset, float duration, int loopCount, float speed,
		 AnimationListener listener) {
		if (id == null) return null;
		 Animation anim = target.getAnimation(id);
		if (anim == null) throw new GdxRuntimeException("Unknown animation: " + id);
		return obtain(anim, offset, duration, loopCount, speed, listener);
	}

	private AnimationDesc obtain ( AnimationDesc anim) {
		return obtain(anim.animation, anim.offset, anim.duration, anim.loopCount, anim.speed, anim.listener);
	}

	/** Update any animations currently being played.
	 * @param delta The time elapsed since last update, change this to alter the overall speed (can be negative). */
	public void update (float delta) {
		if (paused) return;
		if (previous != null && ((transitionCurrentTime += delta) >= transitionTargetTime)) {
			removeAnimation(previous.animation);
			justChangedAnimation = true;
			animationPool.free(previous);
			previous = null;
		}
		if (justChangedAnimation) {
			target.calculateTransforms();
			justChangedAnimation = false;
		}
		if (current == null || current.loopCount == 0 || current.animation == null) return;
		 float remain = current.update(delta);
		if (remain >= 0f && queued != null) {
			inAction = false;
			animate(queued, queuedTransitionTime);
			queued = null;
			if (remain > 0f) update(remain);
			return;
		}
		if (previous != null)
			applyAnimations(previous.animation, previous.offset + previous.time, current.animation, current.offset + current.time,
				transitionCurrentTime / transitionTargetTime);
		else
			applyAnimation(current.animation, current.offset + current.time);
	}

	/** Set the active animation, replacing any current animation.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc setAnimation ( String id) {
		return setAnimation(id, 1, 1.0f, null);
	}

	/** Set the active animation, replacing any current animation.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param loopCount The number of times to loop the animation, zero to play the animation only once, negative to continuously
	 *           loop the animation.
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc setAnimation ( String id, int loopCount) {
		return setAnimation(id, loopCount, 1.0f, null);
	}

	/** Set the active animation, replacing any current animation.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param listener The {@link AnimationListener} which will be informed when the animation is looped or completed.
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc setAnimation ( String id,  AnimationListener listener) {
		return setAnimation(id, 1, 1.0f, listener);
	}

	/** Set the active animation, replacing any current animation.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param loopCount The number of times to play the animation, 1 to play the animation only once, negative to continuously loop
	 *           the animation.
	 * @param listener The {@link AnimationListener} which will be informed when the animation is looped or completed.
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc setAnimation ( String id, int loopCount,  AnimationListener listener) {
		return setAnimation(id, loopCount, 1.0f, listener);
	}

	/** Set the active animation, replacing any current animation.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param loopCount The number of times to play the animation, 1 to play the animation only once, negative to continuously loop
	 *           the animation.
	 * @param speed The speed at which the animation should be played. Default is 1.0f. A value of 2.0f will play the animation at
	 *           twice the normal speed, a value of 0.5f will play the animation at half the normal speed, etc. This value can be
	 *           negative, causing the animation to played in reverse. This value cannot be zero.
	 * @param listener The {@link AnimationListener} which will be informed when the animation is looped or completed.
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc setAnimation ( String id, int loopCount, float speed,  AnimationListener listener) {
		return setAnimation(id, 0f, -1f, loopCount, speed, listener);
	}

	/** Set the active animation, replacing any current animation.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param offset The offset in seconds to the start of the animation.
	 * @param duration The duration in seconds of the animation (or negative to play till the end of the animation).
	 * @param loopCount The number of times to play the animation, 1 to play the animation only once, negative to continuously loop
	 *           the animation.
	 * @param speed The speed at which the animation should be played. Default is 1.0f. A value of 2.0f will play the animation at
	 *           twice the normal speed, a value of 0.5f will play the animation at half the normal speed, etc. This value can be
	 *           negative, causing the animation to played in reverse. This value cannot be zero.
	 * @param listener The {@link AnimationListener} which will be informed when the animation is looped or completed.
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc setAnimation ( String id, float offset, float duration, int loopCount, float speed,
		 AnimationListener listener) {
		return setAnimation(obtain(id, offset, duration, loopCount, speed, listener));
	}

	/** Set the active animation, replacing any current animation. */
	protected AnimationDesc setAnimation ( Animation anim, float offset, float duration, int loopCount, float speed,
		 AnimationListener listener) {
		return setAnimation(obtain(anim, offset, duration, loopCount, speed, listener));
	}

	/** Set the active animation, replacing any current animation. */
	protected AnimationDesc setAnimation ( AnimationDesc anim) {
		if (current == null)
			current = anim;
		else {
			if (!allowSameAnimation && anim != null && current.animation == anim.animation)
				anim.time = current.time;
			else
				removeAnimation(current.animation);
			animationPool.free(current);
			current = anim;
		}
		justChangedAnimation = true;
		return anim;
	}

	/** Changes the current animation by blending the new on top of the old during the transition time.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param transitionTime The time to transition the new animation on top of the currently playing animation (if any).
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc animate ( String id, float transitionTime) {
		return animate(id, 1, 1.0f, null, transitionTime);
	}

	/** Changes the current animation by blending the new on top of the old during the transition time.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param listener The {@link AnimationListener} which will be informed when the animation is looped or completed.
	 * @param transitionTime The time to transition the new animation on top of the currently playing animation (if any).
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc animate ( String id,  AnimationListener listener, float transitionTime) {
		return animate(id, 1, 1.0f, listener, transitionTime);
	}

	/** Changes the current animation by blending the new on top of the old during the transition time.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param loopCount The number of times to loop the animation, zero to play the animation only once, negative to continuously
	 *           loop the animation.
	 * @param listener The {@link AnimationListener} which will be informed when the animation is looped or completed.
	 * @param transitionTime The time to transition the new animation on top of the currently playing animation (if any).
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc animate ( String id, int loopCount,  AnimationListener listener, float transitionTime) {
		return animate(id, loopCount, 1.0f, listener, transitionTime);
	}

	/** Changes the current animation by blending the new on top of the old during the transition time.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param loopCount The number of times to play the animation, 1 to play the animation only once, negative to continuously loop
	 *           the animation.
	 * @param speed The speed at which the animation should be played. Default is 1.0f. A value of 2.0f will play the animation at
	 *           twice the normal speed, a value of 0.5f will play the animation at half the normal speed, etc. This value can be
	 *           negative, causing the animation to played in reverse. This value cannot be zero.
	 * @param listener The {@link AnimationListener} which will be informed when the animation is looped or completed.
	 * @param transitionTime The time to transition the new animation on top of the currently playing animation (if any).
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc animate ( String id, int loopCount, float speed,  AnimationListener listener,
		float transitionTime) {
		return animate(id, 0f, -1f, loopCount, speed, listener, transitionTime);
	}

	/** Changes the current animation by blending the new on top of the old during the transition time.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param offset The offset in seconds to the start of the animation.
	 * @param duration The duration in seconds of the animation (or negative to play till the end of the animation).
	 * @param loopCount The number of times to play the animation, 1 to play the animation only once, negative to continuously loop
	 *           the animation.
	 * @param speed The speed at which the animation should be played. Default is 1.0f. A value of 2.0f will play the animation at
	 *           twice the normal speed, a value of 0.5f will play the animation at half the normal speed, etc. This value can be
	 *           negative, causing the animation to played in reverse. This value cannot be zero.
	 * @param listener The {@link AnimationListener} which will be informed when the animation is looped or completed.
	 * @param transitionTime The time to transition the new animation on top of the currently playing animation (if any).
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc animate ( String id, float offset, float duration, int loopCount, float speed,
		 AnimationListener listener, float transitionTime) {
		return animate(obtain(id, offset, duration, loopCount, speed, listener), transitionTime);
	}

	/** Changes the current animation by blending the new on top of the old during the transition time. */
	protected AnimationDesc animate ( Animation anim, float offset, float duration, int loopCount, float speed,
		 AnimationListener listener, float transitionTime) {
		return animate(obtain(anim, offset, duration, loopCount, speed, listener), transitionTime);
	}

	/** Changes the current animation by blending the new on top of the old during the transition time. */
	protected AnimationDesc animate ( AnimationDesc anim, float transitionTime) {
		if (current == null || current.loopCount == 0)
			current = anim;
		else if (inAction)
			queue(anim, transitionTime);
		else if (!allowSameAnimation && anim != null && current.animation == anim.animation) {
			anim.time = current.time;
			animationPool.free(current);
			current = anim;
		} else {
			if (previous != null) {
				removeAnimation(previous.animation);
				animationPool.free(previous);
			}
			previous = current;
			current = anim;
			transitionCurrentTime = 0f;
			transitionTargetTime = transitionTime;
		}
		return anim;
	}

	/** Queue an animation to be applied when the {@link #current} animation is finished. If the current animation is continuously
	 * looping it will be synchronized on next loop.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param loopCount The number of times to play the animation, 1 to play the animation only once, negative to continuously loop
	 *           the animation.
	 * @param speed The speed at which the animation should be played. Default is 1.0f. A value of 2.0f will play the animation at
	 *           twice the normal speed, a value of 0.5f will play the animation at half the normal speed, etc. This value can be
	 *           negative, causing the animation to played in reverse. This value cannot be zero.
	 * @param listener The {@link AnimationListener} which will be informed when the animation is looped or completed.
	 * @param transitionTime The time to transition the new animation on top of the currently playing animation (if any).
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc queue ( String id, int loopCount, float speed,  AnimationListener listener,
		float transitionTime) {
		return queue(id, 0f, -1f, loopCount, speed, listener, transitionTime);
	}

	/** Queue an animation to be applied when the {@link #current} animation is finished. If the current animation is continuously
	 * looping it will be synchronized on next loop.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param offset The offset in seconds to the start of the animation.
	 * @param duration The duration in seconds of the animation (or negative to play till the end of the animation).
	 * @param loopCount The number of times to play the animation, 1 to play the animation only once, negative to continuously loop
	 *           the animation.
	 * @param speed The speed at which the animation should be played. Default is 1.0f. A value of 2.0f will play the animation at
	 *           twice the normal speed, a value of 0.5f will play the animation at half the normal speed, etc. This value can be
	 *           negative, causing the animation to played in reverse. This value cannot be zero.
	 * @param listener The {@link AnimationListener} which will be informed when the animation is looped or completed.
	 * @param transitionTime The time to transition the new animation on top of the currently playing animation (if any).
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc queue ( String id, float offset, float duration, int loopCount, float speed,
		 AnimationListener listener, float transitionTime) {
		return queue(obtain(id, offset, duration, loopCount, speed, listener), transitionTime);
	}

	/** Queue an animation to be applied when the current is finished. If current is continuous it will be synced on next loop. */
	protected AnimationDesc queue ( Animation anim, float offset, float duration, int loopCount, float speed,
		 AnimationListener listener, float transitionTime) {
		return queue(obtain(anim, offset, duration, loopCount, speed, listener), transitionTime);
	}

	/** Queue an animation to be applied when the current is finished. If current is continuous it will be synced on next loop. */
	protected AnimationDesc queue ( AnimationDesc anim, float transitionTime) {
		if (current == null || current.loopCount == 0)
			animate(anim, transitionTime);
		else {
			if (queued != null) animationPool.free(queued);
			queued = anim;
			queuedTransitionTime = transitionTime;
			if (current.loopCount < 0) current.loopCount = 1;
		}
		return anim;
	}

	/** Apply an action animation on top of the current animation.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param loopCount The number of times to play the animation, 1 to play the animation only once, negative to continuously loop
	 *           the animation.
	 * @param speed The speed at which the animation should be played. Default is 1.0f. A value of 2.0f will play the animation at
	 *           twice the normal speed, a value of 0.5f will play the animation at half the normal speed, etc. This value can be
	 *           negative, causing the animation to played in reverse. This value cannot be zero.
	 * @param listener The {@link AnimationListener} which will be informed when the animation is looped or completed.
	 * @param transitionTime The time to transition the new animation on top of the currently playing animation (if any).
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc action ( String id, int loopCount, float speed,  AnimationListener listener,
		float transitionTime) {
		return action(id, 0, -1f, loopCount, speed, listener, transitionTime);
	}

	/** Apply an action animation on top of the current animation.
	 * @param id The ID of the {@link Animation} within the {@link ModelInstance}.
	 * @param offset The offset in seconds to the start of the animation.
	 * @param duration The duration in seconds of the animation (or negative to play till the end of the animation).
	 * @param loopCount The number of times to play the animation, 1 to play the animation only once, negative to continuously loop
	 *           the animation.
	 * @param speed The speed at which the animation should be played. Default is 1.0f. A value of 2.0f will play the animation at
	 *           twice the normal speed, a value of 0.5f will play the animation at half the normal speed, etc. This value can be
	 *           negative, causing the animation to played in reverse. This value cannot be zero.
	 * @param listener The {@link AnimationListener} which will be informed when the animation is looped or completed.
	 * @param transitionTime The time to transition the new animation on top of the currently playing animation (if any).
	 * @return The {@link AnimationDesc} which can be read to get the progress of the animation. Will be invalid when the animation
	 *         is completed. */
	public AnimationDesc action ( String id, float offset, float duration, int loopCount, float speed,
		 AnimationListener listener, float transitionTime) {
		return action(obtain(id, offset, duration, loopCount, speed, listener), transitionTime);
	}

	/** Apply an action animation on top of the current animation. */
	protected AnimationDesc action ( Animation anim, float offset, float duration, int loopCount, float speed,
		 AnimationListener listener, float transitionTime) {
		return action(obtain(anim, offset, duration, loopCount, speed, listener), transitionTime);
	}

	/** Apply an action animation on top of the current animation. */
	protected AnimationDesc action ( AnimationDesc anim, float transitionTime) {
		if (anim.loopCount < 0) throw new GdxRuntimeException("An action cannot be continuous");
		if (current == null || current.loopCount == 0)
			animate(anim, transitionTime);
		else {
			AnimationDesc toQueue = inAction ? null : obtain(current);
			inAction = false;
			animate(anim, transitionTime);
			inAction = true;
			if (toQueue != null) queue(toQueue, transitionTime);
		}
		return anim;
	}
}
