using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Utils;

namespace SharpGDX.Mathematics
{
	/** Track properties of a stream of float values. The properties (total value, minimum, etc) are updated as values are
 * {@link #put(float)} into the stream.
 * 
 * @author xoppa */
public class FloatCounter : IPoolable {
	/** The amount of values added */
	public int count;
	/** The sum of all values */
	public float total;
	/** The smallest value */
	public float min;
	/** The largest value */
	public float max;
	/** The average value (total / count) */
	public float average;
	/** The latest raw value */
	public float latest;
	/** The current windowed mean value */
	public float value;
	/** Provides access to the WindowedMean if any (can be null) */
	public readonly WindowedMean mean;

	/** Construct a new FloatCounter
	 * @param windowSize The size of the mean window or 1 or below to not use a windowed mean. */
	public FloatCounter (int windowSize) {
		mean = (windowSize > 1) ? new WindowedMean(windowSize) : null;
		reset();
	}

	/** Add a value and update all fields.
	 * @param value The value to add */
	public void put (float value) {
		latest = value;
		total += value;
		count++;
		average = total / count;

		if (mean != null) {
			mean.addValue(value);
			this.value = mean.getMean();
		} else
			this.value = latest;

		if (mean == null || mean.hasEnoughData()) {
			if (this.value < min) min = this.value;
			if (this.value > max) max = this.value;
		}
	}

	/** Reset all values to their default value. */
	public void reset () {
		count = 0;
		total = 0f;
		min = float.MaxValue;
		max = -float.MaxValue;
		average = 0f;
		latest = 0f;
		value = 0f;
		if (mean != null) mean.clear();
	}

	public override String ToString () {
		return "FloatCounter{" + "count=" + count + ", total=" + total + ", min=" + min + ", max=" + max + ", average=" + average
			+ ", latest=" + latest + ", value=" + value + '}';
	}
}
}
