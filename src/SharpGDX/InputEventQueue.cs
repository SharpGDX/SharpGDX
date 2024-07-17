using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX
{
    /** Queues events that are later passed to an {@link InputProcessor}.
 * @author Nathan Sweet */
    public class InputEventQueue
	{
		private const int SKIP = -1;
		private const int KEY_DOWN = 0;
		private const int KEY_UP = 1;
		private const int KEY_TYPED = 2;
		private const int TOUCH_DOWN = 3;
		private const int TOUCH_UP = 4;
		private const int TOUCH_DRAGGED = 5;
		private const int MOUSE_MOVED = 6;
		private const int SCROLLED = 7;

		private readonly IntArray queue = new IntArray();
		private readonly IntArray processingQueue = new IntArray();
		private long currentEventTime;

		public void drain(IInputProcessor? processor)
		{
			lock(this) {
				if (processor == null)
				{
					queue.clear();
					return;
				}
				processingQueue.addAll(queue);
				queue.clear();
			}
			int[] q = processingQueue.items;
			for (int i = 0, n = processingQueue.size; i < n;)
			{
				int type = q[i++];
				currentEventTime = (long)q[i++] << 32 | q[i++] & 0xFFFFFFFFL;
				switch (type)
				{
					case SKIP:
						i += q[i];
						break;
					case KEY_DOWN:
						processor.KeyDown(q[i++]);
						break;
					case KEY_UP:
						processor.KeyUp(q[i++]);
						break;
					case KEY_TYPED:
						processor.KeyTyped((char)q[i++]);
						break;
					case TOUCH_DOWN:
						processor.TouchDown(q[i++], q[i++], q[i++], q[i++]);
						break;
					case TOUCH_UP:
						processor.TouchUp(q[i++], q[i++], q[i++], q[i++]);
						break;
					case TOUCH_DRAGGED:
						processor.TouchDragged(q[i++], q[i++], q[i++]);
						break;
					case MOUSE_MOVED:
						processor.MouseMoved(q[i++], q[i++]);
						break;
					case SCROLLED:
						processor.Scrolled(NumberUtils.intBitsToFloat(q[i++]), NumberUtils.intBitsToFloat(q[i++]));
						break;
					default:
						throw new RuntimeException();
				}
			}
			processingQueue.clear();
		}

		private int next(int nextType, int i)
		{
			lock (this)
			{
				int[] q = queue.items;
				for (int n = queue.size; i < n;)
				{
					int type = q[i];
					if (type == nextType) return i;
					i += 3;
					switch (type)
					{
						case SKIP:
							i += q[i];
							break;
						case KEY_DOWN:
							i++;
							break;
						case KEY_UP:
							i++;
							break;
						case KEY_TYPED:
							i++;
							break;
						case TOUCH_DOWN:
							i += 4;
							break;
						case TOUCH_UP:
							i += 4;
							break;
						case TOUCH_DRAGGED:
							i += 3;
							break;
						case MOUSE_MOVED:
							i += 2;
							break;
						case SCROLLED:
							i += 2;
							break;
						default:
							throw new RuntimeException();
					}
				}

				return -1;
			}
		}

		private void queueTime(long time)
		{
			queue.add((int)(time >> 32));
			queue.add((int)time);
		}

		public bool keyDown(int keycode, long time)
		{
			lock (this)
			{
				queue.add(KEY_DOWN);
				queueTime(time);
				queue.add(keycode);
				return false;
			}
		}

		public bool keyUp(int keycode, long time)
		{
			lock (this)
			{
				queue.add(KEY_UP);
				queueTime(time);
				queue.add(keycode);
				return false;
			}
		}

		public bool keyTyped(char character, long time)
		{
			lock (this)
			{
				queue.add(KEY_TYPED);
				queueTime(time);
				queue.add(character);
				return false;
			}
		}

		public bool touchDown(int screenX, int screenY, int pointer, int button, long time)
		{
			lock (this)
			{
				queue.add(TOUCH_DOWN);
				queueTime(time);
				queue.add(screenX);
				queue.add(screenY);
				queue.add(pointer);
				queue.add(button);
				return false;
			}
		}

		public bool touchUp(int screenX, int screenY, int pointer, int button, long time)
		{
			lock (this)
			{
				queue.add(TOUCH_UP);
				queueTime(time);
				queue.add(screenX);
				queue.add(screenY);
				queue.add(pointer);
				queue.add(button);
				return false;
			}
		}

		public bool touchDragged(int screenX, int screenY, int pointer, long time)
		{
			lock (this)
			{
				// Skip any queued touch dragged events for the same pointer.
				for (int i = next(TOUCH_DRAGGED, 0); i >= 0; i = next(TOUCH_DRAGGED, i + 6))
				{
					if (queue.get(i + 5) == pointer)
					{
						queue.set(i, SKIP);
						queue.set(i + 3, 3);
					}
				}

				queue.add(TOUCH_DRAGGED);
				queueTime(time);
				queue.add(screenX);
				queue.add(screenY);
				queue.add(pointer);
				return false;
			}
		}

		public bool mouseMoved(int screenX, int screenY, long time)
		{
			lock (this)
			{
				// Skip any queued mouse moved events.
				for (int i = next(MOUSE_MOVED, 0); i >= 0; i = next(MOUSE_MOVED, i + 5))
				{
					queue.set(i, SKIP);
					queue.set(i + 3, 2);
				}

				queue.add(MOUSE_MOVED);
				queueTime(time);
				queue.add(screenX);
				queue.add(screenY);
				return false;
			}
		}

		public bool scrolled(float amountX, float amountY, long time)
		{
			lock (this)
			{
				queue.add(SCROLLED);
				queueTime(time);
				queue.add(NumberUtils.floatToIntBits(amountX));
				queue.add(NumberUtils.floatToIntBits(amountY));
				return false;
			}
		}

		public long getCurrentEventTime()
		{
			return currentEventTime;
		}
	}
}
