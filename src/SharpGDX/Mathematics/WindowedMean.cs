using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Mathematics
{
	/** A simple class keeping track of the mean of a stream of values within a certain window. the WindowedMean will only return a
 * value in case enough data has been sampled. After enough data has been sampled the oldest sample will be replaced by the newest
 * in case a new sample is added.
 * 
 * @author badlogicgames@gmail.com */
	public sealed class WindowedMean
	{
		float[] values;
		int added_values = 0;
		int last_value;
		float mean = 0;
		bool dirty = true;

		/** constructor, window_size specifies the number of samples we will continuously get the mean and variance from. the class
		 * will only return meaning full values if at least window_size values have been added.
		 * 
		 * @param window_size size of the sample window */
		public WindowedMean(int window_size)
		{
			values = new float[window_size];
		}

		/** @return whether the value returned will be meaningful */
		public bool hasEnoughData()
		{
			return added_values >= values.Length;
		}

		/** clears this WindowedMean. The class will only return meaningful values after enough data has been added again. */
		public void clear()
		{
			added_values = 0;
			last_value = 0;
			for (int i = 0; i < values.Length; i++)
				values[i] = 0;
			dirty = true;
		}

		/** adds a new sample to this mean. In case the window is full the oldest value will be replaced by this new value.
		 * 
		 * @param value The value to add */
		public void addValue(float value)
		{
			if (added_values < values.Length) added_values++;
			values[last_value++] = value;
			if (last_value > values.Length - 1) last_value = 0;
			dirty = true;
		}

		/** returns the mean of the samples added to this instance. Only returns meaningful results when at least window_size samples
		 * as specified in the constructor have been added.
		 * @return the mean */
		public float getMean()
		{
			if (hasEnoughData())
			{
				if (dirty)
				{
					float mean = 0;
					for (int i = 0; i < values.Length; i++)
						mean += values[i];

					this.mean = mean / values.Length;
					dirty = false;
				}
				return this.mean;
			}
			else
				return 0;
		}

		/** @return the oldest value in the window */
		public float getOldest()
		{
			return added_values < values.Length ? values[0] : values[last_value];
		}

		/** @return the value last added */
		public float getLatest()
		{
			return values[last_value - 1 == -1 ? values.Length - 1 : last_value - 1];
		}

		/** @return The standard deviation */
		public float standardDeviation()
		{
			if (!hasEnoughData()) return 0;

			float mean = getMean();
			float sum = 0;
			for (int i = 0; i < values.Length; i++)
			{
				sum += (values[i] - mean) * (values[i] - mean);
			}

			return (float)Math.Sqrt(sum / values.Length);
		}

		public float getLowest()
		{
			float lowest = float.MaxValue;
			for (int i = 0; i < values.Length; i++)
				lowest = Math.Min(lowest, values[i]);
			return lowest;
		}

		public float getHighest()
		{
			// TODO: Is this an appropriate replacement?
			float lowest = (1 << 23) * float.Epsilon;
			for (int i = 0; i < values.Length; i++)
				lowest = Math.Max(lowest, values[i]);
			return lowest;
		}

		public int getValueCount()
		{
			return added_values;
		}

		public int getWindowSize()
		{
			return values.Length;
		}

		/** @return A new <code>float[]</code> containing all values currently in the window of the stream, in order from oldest to
		 *         latest. The length of the array is smaller than the window size if not enough data has been added. */
		public float[] getWindowValues()
		{
			float[] windowValues = new float[added_values];
			if (hasEnoughData())
			{
				for (int i = 0; i < windowValues.Length; i++)
				{
					windowValues[i] = values[(i + last_value) % values.Length];
				}
			}
			else
			{
				Array.Copy(values, 0, windowValues, 0, added_values);
			}
			return windowValues;
		}
	}
}
