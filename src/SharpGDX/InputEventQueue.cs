using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX;

/// <summary>
///     Queues events that are later passed to an <see cref="IInputProcessor" />.
/// </summary>
public class InputEventQueue
{
    // TODO: Fix casing. -RP
    private const int KEY_DOWN = 0;
    private const int KEY_TYPED = 2;
    private const int KEY_UP = 1;
    private const int MOUSE_MOVED = 6;
    private const int SCROLLED = 7;
    private const int SKIP = -1;
    private const int TOUCH_DOWN = 3;
    private const int TOUCH_DRAGGED = 5;
    private const int TOUCH_UP = 4;

    private readonly IntArray _processingQueue = new();
    private readonly IntArray _queue = new();
    private long _currentEventTime;

    public void Drain(IInputProcessor? processor)
    {
        lock (this)
        {
            if (processor == null)
            {
                _queue.clear();
                return;
            }

            _processingQueue.addAll(_queue);
            _queue.clear();
        }

        var q = _processingQueue.items;

        for (int i = 0, n = _processingQueue.size; i < n;)
        {
            var type = q[i++];
            _currentEventTime = ((long)q[i++] << 32) | (q[i++] & 0xFFFFFFFFL);
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

        _processingQueue.clear();
    }

    public long GetCurrentEventTime()
    {
        return _currentEventTime;
    }

    public bool KeyDown(int keycode, long time)
    {
        lock (this)
        {
            _queue.add(KEY_DOWN);
            QueueTime(time);
            _queue.add(keycode);
            return false;
        }
    }

    public bool KeyTyped(char character, long time)
    {
        lock (this)
        {
            _queue.add(KEY_TYPED);
            QueueTime(time);
            _queue.add(character);
            return false;
        }
    }

    public bool KeyUp(int keycode, long time)
    {
        lock (this)
        {
            _queue.add(KEY_UP);
            QueueTime(time);
            _queue.add(keycode);
            return false;
        }
    }

    public bool MouseMoved(int screenX, int screenY, long time)
    {
        lock (this)
        {
            // Skip any queued mouse moved events.
            for (var i = Next(MOUSE_MOVED, 0); i >= 0; i = Next(MOUSE_MOVED, i + 5))
            {
                _queue.set(i, SKIP);
                _queue.set(i + 3, 2);
            }

            _queue.add(MOUSE_MOVED);
            QueueTime(time);
            _queue.add(screenX);
            _queue.add(screenY);
            return false;
        }
    }

    public bool Scrolled(float amountX, float amountY, long time)
    {
        lock (this)
        {
            _queue.add(SCROLLED);

            QueueTime(time);

            _queue.add(NumberUtils.floatToIntBits(amountX));
            _queue.add(NumberUtils.floatToIntBits(amountY));
            return false;
        }
    }

    public bool TouchDown(int screenX, int screenY, int pointer, int button, long time)
    {
        lock (this)
        {
            _queue.add(TOUCH_DOWN);

            QueueTime(time);

            _queue.add(screenX);
            _queue.add(screenY);
            _queue.add(pointer);
            _queue.add(button);
            return false;
        }
    }

    public bool TouchDragged(int screenX, int screenY, int pointer, long time)
    {
        lock (this)
        {
            // Skip any queued touch dragged events for the same pointer.
            for (var i = Next(TOUCH_DRAGGED, 0); i >= 0; i = Next(TOUCH_DRAGGED, i + 6))
            {
                if (_queue.get(i + 5) == pointer)
                {
                    _queue.set(i, SKIP);
                    _queue.set(i + 3, 3);
                }
            }

            _queue.add(TOUCH_DRAGGED);

            QueueTime(time);

            _queue.add(screenX);
            _queue.add(screenY);
            _queue.add(pointer);
            return false;
        }
    }

    public bool TouchUp(int screenX, int screenY, int pointer, int button, long time)
    {
        lock (this)
        {
            _queue.add(TOUCH_UP);

            QueueTime(time);

            _queue.add(screenX);
            _queue.add(screenY);
            _queue.add(pointer);
            _queue.add(button);
            return false;
        }
    }

    private int Next(int nextType, int i)
    {
        lock (this)
        {
            var q = _queue.items;

            for (var n = _queue.size; i < n;)
            {
                var type = q[i];

                if (type == nextType)
                {
                    return i;
                }

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

    private void QueueTime(long time)
    {
        _queue.add((int)(time >> 32));
        _queue.add((int)time);
    }
}