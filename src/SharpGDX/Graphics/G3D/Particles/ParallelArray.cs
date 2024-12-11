using System;
using SharpGDX.Utils.Reflect;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G3D.Models.Data;
using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Environments;

namespace SharpGDX.Graphics.G3D.Particles;

/** This class represents an group of elements like an array, but the properties of the elements are stored as separate arrays.
 * These arrays are called {@link Channel} and are represented by {@link ChannelDescriptor}. It's not necessary to store primitive
 * types in the channels but doing so will "exploit" data locality in the JVM, which is ensured for primitive types. Use
 * {@link FloatChannel}, {@link IntChannel}, {@link ObjectChannel} to store the data.
 * @author inferno */
public class ParallelArray {

	/** This class describes the content of a {@link Channel} */
	public class ChannelDescriptor {
		public int id;
		public Type type;
		public int count;

		public ChannelDescriptor (int id, Type type, int count) {
			this.id = id;
			this.type = type;
			this.count = count;
		}
	}

	/** This class represents a container of values for all the elements for a given property */
	public abstract class Channel {
		public int id;
		public Object data;
		public int strideSize;

		public Channel (int id, Object data, int strideSize) {
			this.id = id;
			this.strideSize = strideSize;
			this.data = data;
		}

		public abstract void add (int index, Object[] objects);

		public abstract void swap (int i, int k);

		protected internal abstract void setCapacity (int requiredCapacity);
	}

	/** This interface is used to provide custom initialization of the {@link Channel} data */
	public  interface ChannelInitializer<T>
	where T: Channel{
		public void init (T channel);
	}

	public class FloatChannel : Channel {
        private readonly ParallelArray _array;
        public float[] data;

		public FloatChannel (ParallelArray array, int id, int strideSize, int size) 
        : base(id, new float[size * strideSize], strideSize)
        {
            _array = array;

            this.data = (float[])base.data;
        }

		public override void add (int index, Object[] objects) {
			for (int i = strideSize * _array.size, c = i + strideSize, k = 0; i < c; ++i, ++k) {
				data[i] = (float)objects[k];
			}
		}

		public override void swap (int i, int k) {
			float t;
			i = strideSize * i;
			k = strideSize * k;
			for (int c = i + strideSize; i < c; ++i, ++k) {
				t = data[i];
				data[i] = data[k];
				data[k] = t;
			}
		}

        protected internal override void setCapacity (int requiredCapacity) {
			float[] newData = new float[strideSize * requiredCapacity];
			Array.Copy(data, 0, newData, 0, Math.Min(data.Length, newData.Length));
			base.data = data = newData;
		}
	}

	public class IntChannel : Channel {
        private readonly ParallelArray _array;
        public int[] data;

		public IntChannel (ParallelArray array, int id, int strideSize, int size) 
        : base(id, new int[size * strideSize], strideSize)
        {
            _array = array;

            this.data = (int[])base.data;
        }

		public override void add (int index, Object[] objects) {
			for (int i = strideSize * _array.size, c = i + strideSize, k = 0; i < c; ++i, ++k) {
				data[i] = (int)objects[k];
			}
		}

		public override void swap (int i, int k) {
			int t;
			i = strideSize * i;
			k = strideSize * k;
			for (int c = i + strideSize; i < c; ++i, ++k) {
				t = data[i];
				data[i] = data[k];
				data[k] = t;
			}
		}

		protected internal override void setCapacity (int requiredCapacity) {
			int[] newData = new int[strideSize * requiredCapacity];
			Array.Copy(data, 0, newData, 0, Math.Min(data.Length, newData.Length));
			base.data = data = newData;
		}
	}

    // TODO: @SuppressWarnings("unchecked")
    public class ObjectChannel<T> : Channel {
        private readonly ParallelArray _array;
        Type componentType;
		public T[] data;

		public ObjectChannel (ParallelArray array, int id, int strideSize, int size, Type type) 
        : base(id, ArrayReflection.newInstance(type, size * strideSize), strideSize)
        {
            _array = array;
            componentType = type;
			this.data = (T[])base.data;
		}

		public override void add (int index, Object[] objects)
        {
            for (int i = strideSize * _array.size, c = i + strideSize, k = 0; i < c; ++i, ++k) {
				this.data[i] = (T)objects[k];
			}
		}

		public override void swap (int i, int k) {
			T t;
			i = strideSize * i;
			k = strideSize * k;
			for (int c = i + strideSize; i < c; ++i, ++k) {
				t = data[i];
				data[i] = data[k];
				data[k] = t;
			}
		}

        protected internal override void setCapacity (int requiredCapacity) {
			T[] newData = (T[])ArrayReflection.newInstance(componentType, strideSize * requiredCapacity);
			Array.Copy(data, 0, newData, 0, Math.Min(data.Length, newData.Length));
			base.data = data = newData;
		}
	}

	/** the channels added to the array */
	Array<Channel> arrays;
	/** the maximum amount of elements that this array can hold */
	public int capacity;
	/** the current amount of defined elements, do not change manually unless you know what you are doing. */
	public int size;

	public ParallelArray (int capacity) {
		arrays = new Array<Channel>(false, 2, typeof(Channel));
		this.capacity = capacity;
		size = 0;
	}

	/** Adds and returns a channel described by the channel descriptor parameter. If a channel with the same id already exists, no
	 * allocation is performed and that channel is returned. */
	public T addChannel <T>(ChannelDescriptor channelDescriptor)
        where T : Channel
    {
		return addChannel<T>(channelDescriptor, null);
	}

	/** Adds and returns a channel described by the channel descriptor parameter. If a channel with the same id already exists, no
	 * allocation is performed and that channel is returned. Otherwise a new channel is allocated and initialized with the
	 * initializer. */
	public  T addChannel <T>(ChannelDescriptor channelDescriptor, ChannelInitializer<T> initializer)
        where T : Channel
    {
		T channel = getChannel<T>(channelDescriptor);
		if (channel == null) {
			channel = allocateChannel<T>(channelDescriptor);
			if (initializer != null) initializer.init(channel);
			arrays.Add(channel);
		}
		return channel;
	}

// TODO: @SuppressWarnings({"unchecked", "rawtypes"})
private T allocateChannel<T> (ChannelDescriptor channelDescriptor)
where T: Channel{
		if (channelDescriptor.type == typeof(float)) {
			return (T)(Channel)new FloatChannel(this, channelDescriptor.id, channelDescriptor.count, capacity);
		} else if (channelDescriptor.type == typeof(int)) {
			return (T)(Channel)new IntChannel(this, channelDescriptor.id, channelDescriptor.count, capacity);
		} else
        {
            throw new NotImplementedException();
			// TODO: Will probably need to resort to reflection... -RP
            // TODO: return (T)(Channel)new ObjectChannel(this, channelDescriptor.id, channelDescriptor.count, capacity, channelDescriptor.type);
        }
	}

	/** Removes the channel with the given id */
	// TODO: Why the T?
	public  void removeArray<T>(int id) {
		arrays.RemoveIndex(findIndex(id));
	}

	private int findIndex (int id) {
		for (int i = 0; i < arrays.size; ++i) {
			Channel array = arrays.items[i];
			if (array.id == id) return i;
		}
		return -1;
	}

	/** Adds an element considering the values in the same order as the current channels in the array. The n_th value must have the
	 * same type and stride of the given channel at position n */
	public void addElement (Object[] values) {
		/* FIXME make it grow... */
		if (size == capacity) throw new GdxRuntimeException("Capacity reached, cannot add other elements");

		int k = 0;
		foreach (Channel strideArray in arrays) {
			strideArray.add(k, values);
			k += strideArray.strideSize;
		}
		++size;
	}

	/** Removes the element at the given index and swaps it with the last available element */
	public void removeElement (int index) {
		int last = size - 1;
		// Swap
		foreach (Channel strideArray in arrays) {
			strideArray.swap(index, last);
		}
		size = last;
	}

	/** @return the channel with the same id as the one in the descriptor */
	// TODO: @SuppressWarnings("unchecked")
	public T getChannel<T> (ChannelDescriptor descriptor) 
    where T: Channel{
		foreach (Channel array in arrays) {
			if (array.id == descriptor.id) return (T)array;
		}
		return null;
	}

	/** Removes all the channels and sets size to 0 */
	public void clear () {
		arrays.clear();
		size = 0;
	}

	/** Sets the capacity. Each contained channel will be resized to match the required capacity and the current data will be
	 * preserved. */
	public void setCapacity (int requiredCapacity) {
		if (capacity != requiredCapacity) {
			foreach (Channel channel in arrays) {
				channel.setCapacity(requiredCapacity);
			}
			capacity = requiredCapacity;
		}
	}

}
