using System;
using SharpGDX.Assets;
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
using FloatChannel = SharpGDX.Graphics.G3D.Particles.ParallelArray.FloatChannel;

namespace SharpGDX.Graphics.G3D.Particles.Values;

/** A value which has a defined minimum and maximum upper and lower bounds. Defines the variations of the value on a time line.
 * @author Inferno */
public class ScaledNumericValue : RangedNumericValue {
	private float[] scaling = {1};
	public float[] timeline = {0};
	private float highMin, highMax;
	private bool relative = false;

	public float newHighValue () {
		return highMin + (highMax - highMin) * MathUtils.random();
	}

	public void setHigh (float value) {
		highMin = value;
		highMax = value;
	}

	public void setHigh (float min, float max) {
		highMin = min;
		highMax = max;
	}

	public float getHighMin () {
		return highMin;
	}

	public void setHighMin (float highMin) {
		this.highMin = highMin;
	}

	public float getHighMax () {
		return highMax;
	}

	public void setHighMax (float highMax) {
		this.highMax = highMax;
	}

	public float[] getScaling () {
		return scaling;
	}

	public void setScaling (float[] values) {
		this.scaling = values;
	}

	public float[] getTimeline () {
		return timeline;
	}

	public void setTimeline (float[] timeline) {
		this.timeline = timeline;
	}

	public bool isRelative () {
		return relative;
	}

	public void setRelative (bool relative) {
		this.relative = relative;
	}

	public float getScale (float percent) {
		int endIndex = -1;
		int n = timeline.Length;
		// if (percent >= timeline[n-1])
		// return scaling[n - 1];
		for (int i = 1; i < n; i++) {
			float t = timeline[i];
			if (t > percent) {
				endIndex = i;
				break;
			}
		}
		if (endIndex == -1) return scaling[n - 1];
		int startIndex = endIndex - 1;
		float startValue = scaling[startIndex];
		float startTime = timeline[startIndex];
		return startValue + (scaling[endIndex] - startValue) * ((percent - startTime) / (timeline[endIndex] - startTime));
	}

	public void load (ScaledNumericValue value) {
		base.load(value);
		highMax = value.highMax;
		highMin = value.highMin;
		scaling = new float[value.scaling.Length];
		Array.Copy(value.scaling, 0, scaling, 0, scaling.Length);
		timeline = new float[value.timeline.Length];
		Array.Copy(value.timeline, 0, timeline, 0, timeline.Length);
		relative = value.relative;
	}

    public override void write (Json json) {
		base.write(json);
		json.writeValue("highMin", highMin);
		json.writeValue("highMax", highMax);
		json.writeValue("relative", relative);
		json.writeValue("scaling", scaling);
		json.writeValue("timeline", timeline);
	}

    public override void read (Json json, JsonValue jsonData) {
		base.read(json, jsonData);
		highMin = (float)json.readValue("highMin", typeof(float), jsonData);
		highMax = (float)json.readValue("highMax", typeof(float), jsonData);
		relative = (bool)json.readValue("relative", typeof(bool), jsonData);
		scaling = (float[])json.readValue("scaling", typeof(float[]), jsonData);
		timeline = (float[])json.readValue("timeline", typeof(float[]), jsonData);
	}

}
