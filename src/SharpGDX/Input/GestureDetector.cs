﻿using SharpGDX.Utils;
using Timer = SharpGDX.Utils.Timer;
using Task = SharpGDX.Utils.Timer.Task;
using SharpGDX.Shims;
using SharpGDX.Mathematics;

namespace SharpGDX.Input
{
	/** {@link InputProcessor} implementation that detects gestures (tap, long press, fling, pan, zoom, pinch) and hands them to a
 * {@link GestureListener}.
 * @author mzechner */
public class GestureDetector : InputAdapter {
	readonly IGestureListener listener;
	private float tapRectangleWidth;
	private float tapRectangleHeight;
	private long tapCountInterval;
	private float longPressSeconds;
	private long maxFlingDelay;

	private bool inTapRectangle;
	private int tapCount;
	private long lastTapTime;
	private float lastTapX, lastTapY;
	private int lastTapButton, lastTapPointer;
	bool longPressFired;
	private bool pinching;
	private bool panning;

	private readonly VelocityTracker tracker = new VelocityTracker();
	private float tapRectangleCenterX, tapRectangleCenterY;
	private long touchDownTime;
	Vector2 pointer1 = new Vector2();
	private readonly Vector2 pointer2 = new Vector2();
	private readonly Vector2 initialPointer1 = new Vector2();
	private readonly Vector2 initialPointer2 = new Vector2();

	private readonly Task longPressTask ;

	private class LongPressTask : Task
		{
			private readonly GestureDetector _gestureDetector;

			public LongPressTask(GestureDetector gestureDetector)
			{
				_gestureDetector = gestureDetector;
			}

				public override void run()
				{
					if (!_gestureDetector.longPressFired) _gestureDetector.longPressFired = _gestureDetector.listener.longPress(_gestureDetector.pointer1.x, _gestureDetector.pointer1.y);
				}
	}

	/** Creates a new GestureDetector with default values: halfTapSquareSize=20, tapCountInterval=0.4f, longPressDuration=1.1f,
	 * maxFlingDelay=Integer.MAX_VALUE. */
	public GestureDetector (IGestureListener listener) 
	: this(20, 0.4f, 1.1f, int.MaxValue, listener)
	{
		
	}

	/** @param halfTapSquareSize half width in pixels of the square around an initial touch event, see
	 *           {@link GestureListener#tap(float, float, int, int)}.
	 * @param tapCountInterval time in seconds that must pass for two touch down/up sequences to be detected as consecutive taps.
	 * @param longPressDuration time in seconds that must pass for the detector to fire a
	 *           {@link GestureListener#longPress(float, float)} event.
	 * @param maxFlingDelay no fling event is fired when the time in seconds the finger was dragged is larger than this, see
	 *           {@link GestureListener#fling(float, float, int)} */
	public GestureDetector (float halfTapSquareSize, float tapCountInterval, float longPressDuration, float maxFlingDelay,
		IGestureListener listener) 
	: this(halfTapSquareSize, halfTapSquareSize, tapCountInterval, longPressDuration, maxFlingDelay, listener)
	{
		
	}

	/** @param halfTapRectangleWidth half width in pixels of the rectangle around an initial touch event, see
	 *           {@link GestureListener#tap(float, float, int, int)}.
	 * @param halfTapRectangleHeight half height in pixels of the rectangle around an initial touch event, see
	 *           {@link GestureListener#tap(float, float, int, int)}.
	 * @param tapCountInterval time in seconds that must pass for two touch down/up sequences to be detected as consecutive taps.
	 * @param longPressDuration time in seconds that must pass for the detector to fire a
	 *           {@link GestureListener#longPress(float, float)} event.
	 * @param maxFlingDelay no fling event is fired when the time in seconds the finger was dragged is larger than this, see
	 *           {@link GestureListener#fling(float, float, int)} */
	public GestureDetector (float halfTapRectangleWidth, float halfTapRectangleHeight, float tapCountInterval,
		float longPressDuration, float maxFlingDelay, IGestureListener listener) {
		longPressTask = new LongPressTask(this);
			if (listener == null) throw new IllegalArgumentException("listener cannot be null.");
		this.tapRectangleWidth = halfTapRectangleWidth;
		this.tapRectangleHeight = halfTapRectangleHeight;
		this.tapCountInterval = (long)(tapCountInterval * 1000000000L);
		this.longPressSeconds = longPressDuration;
		this.maxFlingDelay = (long)(maxFlingDelay * 1000000000L);
		this.listener = listener;
	}

	public override bool TouchDown (int x, int y, int pointer, int button) {
		return touchDown((float)x, (float)y, pointer, button);
	}

	public bool touchDown (float x, float y, int pointer, int button) {
		if (pointer > 1) return false;

		if (pointer == 0) {
			pointer1.Set(x, y);
			touchDownTime = GDX.Input.GetCurrentEventTime();
			tracker.start(x, y, touchDownTime);
			if (GDX.Input.IsTouched(1)) {
				// Start pinch.
				inTapRectangle = false;
				pinching = true;
				initialPointer1.Set(pointer1);
				initialPointer2.Set(pointer2);
				longPressTask.cancel();
			} else {
				// Normal touch down.
				inTapRectangle = true;
				pinching = false;
				longPressFired = false;
				tapRectangleCenterX = x;
				tapRectangleCenterY = y;
				if (!longPressTask.isScheduled()) Timer.schedule(longPressTask, longPressSeconds);
			}
		} else {
			// Start pinch.
			pointer2.Set(x, y);
			inTapRectangle = false;
			pinching = true;
			initialPointer1.Set(pointer1);
			initialPointer2.Set(pointer2);
			longPressTask.cancel();
		}
		return listener.touchDown(x, y, pointer, button);
	}

	public override bool TouchDragged (int x, int y, int pointer) {
		return touchDragged((float)x, (float)y, pointer);
	}

	public bool touchDragged (float x, float y, int pointer) {
		if (pointer > 1) return false;
		if (longPressFired) return false;

		if (pointer == 0)
			pointer1.Set(x, y);
		else
			pointer2.Set(x, y);

		// handle pinch zoom
		if (pinching) {
			bool result = listener.pinch(initialPointer1, initialPointer2, pointer1, pointer2);
			return listener.zoom(initialPointer1.dst(initialPointer2), pointer1.dst(pointer2)) || result;
		}

		// update tracker
		tracker.update(x, y, GDX.Input.GetCurrentEventTime());

		// check if we are still tapping.
		if (inTapRectangle && !isWithinTapRectangle(x, y, tapRectangleCenterX, tapRectangleCenterY)) {
			longPressTask.cancel();
			inTapRectangle = false;
		}

		// if we have left the tap square, we are panning
		if (!inTapRectangle) {
			panning = true;
			return listener.pan(x, y, tracker.deltaX, tracker.deltaY);
		}

		return false;
	}

	public override bool TouchUp (int x, int y, int pointer, int button) {
		return touchUp((float)x, (float)y, pointer, button);
	}

	public bool touchUp (float x, float y, int pointer, int button) {
		if (pointer > 1) return false;

		// check if we are still tapping.
		if (inTapRectangle && !isWithinTapRectangle(x, y, tapRectangleCenterX, tapRectangleCenterY)) inTapRectangle = false;

		bool wasPanning = panning;
		panning = false;

		longPressTask.cancel();
		if (longPressFired) return false;

		if (inTapRectangle) {
			// handle taps
			if (lastTapButton != button || lastTapPointer != pointer || TimeUtils.nanoTime() - lastTapTime > tapCountInterval
				|| !isWithinTapRectangle(x, y, lastTapX, lastTapY)) tapCount = 0;
			tapCount++;
			lastTapTime = TimeUtils.nanoTime();
			lastTapX = x;
			lastTapY = y;
			lastTapButton = button;
			lastTapPointer = pointer;
			touchDownTime = 0;
			return listener.tap(x, y, tapCount, button);
		}

		if (pinching) {
			// handle pinch end
			pinching = false;
			listener.pinchStop();
			panning = true;
			// we are in pan mode again, reset velocity tracker
			if (pointer == 0) {
				// first pointer has lifted off, set up panning to use the second pointer...
				tracker.start(pointer2.x, pointer2.y, GDX.Input.GetCurrentEventTime());
			} else {
				// second pointer has lifted off, set up panning to use the first pointer...
				tracker.start(pointer1.x, pointer1.y, GDX.Input.GetCurrentEventTime());
			}
			return false;
		}

		// handle no longer panning
		bool handled = false;
		if (wasPanning && !panning) handled = listener.panStop(x, y, pointer, button);

		// handle fling
		long time = GDX.Input.GetCurrentEventTime();
		if (time - touchDownTime <= maxFlingDelay) {
			tracker.update(x, y, time);
			handled = listener.fling(tracker.getVelocityX(), tracker.getVelocityY(), button) || handled;
		}
		touchDownTime = 0;
		return handled;
	}

	public override bool TouchCancelled (int screenX, int screenY, int pointer, int button) {
		cancel();
		return base.TouchCancelled(screenX, screenY, pointer, button);
	}

	/** No further gesture events will be triggered for the current touch, if any. */
	public void cancel () {
		longPressTask.cancel();
		longPressFired = true;
	}

	/** @return whether the user touched the screen long enough to trigger a long press event. */
	public bool isLongPressed () {
		return isLongPressed(longPressSeconds);
	}

	/** @param duration
	 * @return whether the user touched the screen for as much or more than the given duration. */
	public bool isLongPressed (float duration) {
		if (touchDownTime == 0) return false;
		return TimeUtils.nanoTime() - touchDownTime > (long)(duration * 1000000000L);
	}

	public bool isPanning () {
		return panning;
	}

	public void reset () {
        longPressTask.cancel();
            touchDownTime = 0;
		panning = false;
		inTapRectangle = false;
		tracker.lastTime = 0;
	}

	private bool isWithinTapRectangle (float x, float y, float centerX, float centerY) {
		return Math.Abs(x - centerX) < tapRectangleWidth && Math.Abs(y - centerY) < tapRectangleHeight;
	}

	/** The tap square will no longer be used for the current touch. */
	public void invalidateTapSquare () {
		inTapRectangle = false;
	}

	public void setTapSquareSize (float halfTapSquareSize) {
		setTapRectangleSize(halfTapSquareSize, halfTapSquareSize);
	}

	public void setTapRectangleSize (float halfTapRectangleWidth, float halfTapRectangleHeight) {
		this.tapRectangleWidth = halfTapRectangleWidth;
		this.tapRectangleHeight = halfTapRectangleHeight;
	}

	/** @param tapCountInterval time in seconds that must pass for two touch down/up sequences to be detected as consecutive
	 *           taps. */
	public void setTapCountInterval (float tapCountInterval) {
		this.tapCountInterval = (long)(tapCountInterval * 1000000000L);
	}

	public void setLongPressSeconds (float longPressSeconds) {
		this.longPressSeconds = longPressSeconds;
	}

	public void setMaxFlingDelay (long maxFlingDelay) {
		this.maxFlingDelay = maxFlingDelay;
	}

	/** Register an instance of this class with a {@link GestureDetector} to receive gestures such as taps, long presses, flings,
	 * panning or pinch zooming. Each method returns a boolean indicating if the event should be handed to the next listener (false
	 * to hand it to the next listener, true otherwise).
	 * @author mzechner */
	public  interface IGestureListener {
		/** @see InputProcessor#touchDown(int, int, int, int) */
		public bool touchDown (float x, float y, int pointer, int button);

		/** Called when a tap occured. A tap happens if a touch went down on the screen and was lifted again without moving outside
		 * of the tap square. The tap square is a rectangular area around the initial touch position as specified on construction
		 * time of the {@link GestureDetector}.
		 * @param count the number of taps. */
		public bool tap (float x, float y, int count, int button);

		public bool longPress (float x, float y);

		/** Called when the user dragged a finger over the screen and lifted it. Reports the last known velocity of the finger in
		 * pixels per second.
		 * @param velocityX velocity on x in seconds
		 * @param velocityY velocity on y in seconds */
		public bool fling (float velocityX, float velocityY, int button);

		/** Called when the user drags a finger over the screen.
		 * @param deltaX the difference in pixels to the last drag event on x.
		 * @param deltaY the difference in pixels to the last drag event on y. */
		public bool pan (float x, float y, float deltaX, float deltaY);

		/** Called when no longer panning. */
		public bool panStop (float x, float y, int pointer, int button);

		/** Called when the user performs a pinch zoom gesture. The original distance is the distance in pixels when the gesture
		 * started.
		 * @param initialDistance distance between fingers when the gesture started.
		 * @param distance current distance between fingers. */
		public bool zoom (float initialDistance, float distance);

		/** Called when a user performs a pinch zoom gesture. Reports the initial positions of the two involved fingers and their
		 * current positions.
		 * @param initialPointer1
		 * @param initialPointer2
		 * @param pointer1
		 * @param pointer2 */
		public bool pinch (Vector2 initialPointer1, Vector2 initialPointer2, Vector2 pointer1, Vector2 pointer2);

		/** Called when no longer pinching. */
		public void pinchStop ();
	}

	/** Derrive from this if you only want to implement a subset of {@link GestureListener}.
	 * @author mzechner */
	public class GestureAdapter : IGestureListener {
		public virtual bool touchDown (float x, float y, int pointer, int button) {
			return false;
		}

		public virtual bool tap (float x, float y, int count, int button) {
			return false;
		}

		public virtual bool longPress (float x, float y) {
			return false;
		}

		public virtual bool fling (float velocityX, float velocityY, int button) {
			return false;
		}

		public virtual bool pan (float x, float y, float deltaX, float deltaY) {
			return false;
		}

		public virtual bool panStop (float x, float y, int pointer, int button) {
			return false;
		}

		public virtual bool zoom (float initialDistance, float distance) {
			return false;
		}

		public virtual bool pinch (Vector2 initialPointer1, Vector2 initialPointer2, Vector2 pointer1, Vector2 pointer2) {
			return false;
		}

		public virtual void pinchStop () {
		}
	}

	 class VelocityTracker {
		const int sampleSize = 10;
		float lastX, lastY;
		internal float deltaX, deltaY;
		internal long lastTime;
		int numSamples;
		float[] meanX = new float[sampleSize];
		float[] meanY = new float[sampleSize];
		long[] meanTime = new long[sampleSize];

		public void start (float x, float y, long timeStamp) {
			lastX = x;
			lastY = y;
			deltaX = 0;
			deltaY = 0;
			numSamples = 0;
			for (int i = 0; i < sampleSize; i++) {
				meanX[i] = 0;
				meanY[i] = 0;
				meanTime[i] = 0;
			}
			lastTime = timeStamp;
		}

		public void update (float x, float y, long currTime) {
			deltaX = x - lastX;
			deltaY = y - lastY;
			lastX = x;
			lastY = y;
			long deltaTime = currTime - lastTime;
			lastTime = currTime;
			int index = numSamples % sampleSize;
			meanX[index] = deltaX;
			meanY[index] = deltaY;
			meanTime[index] = deltaTime;
			numSamples++;
		}

		public float getVelocityX () {
			float meanX = getAverage(this.meanX, numSamples);
			float meanTime = getAverage(this.meanTime, numSamples) / 1000000000.0f;
			if (meanTime == 0) return 0;
			return meanX / meanTime;
		}

		public float getVelocityY () {
			float meanY = getAverage(this.meanY, numSamples);
			float meanTime = getAverage(this.meanTime, numSamples) / 1000000000.0f;
			if (meanTime == 0) return 0;
			return meanY / meanTime;
		}

		private float getAverage (float[] values, int numSamples) {
			numSamples = Math.Min(sampleSize, numSamples);
			float sum = 0;
			for (int i = 0; i < numSamples; i++) {
				sum += values[i];
			}
			return sum / numSamples;
		}

		private long getAverage (long[] values, int numSamples) {
			numSamples = Math.Min(sampleSize, numSamples);
			long sum = 0;
			for (int i = 0; i < numSamples; i++) {
				sum += values[i];
			}
			if (numSamples == 0) return 0;
			return sum / numSamples;
		}

		private float getSum (float[] values, int numSamples) {
			numSamples = Math.Min(sampleSize, numSamples);
			float sum = 0;
			for (int i = 0; i < numSamples; i++) {
				sum += values[i];
			}
			if (numSamples == 0) return 0;
			return sum;
		}
	}
}
}
