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

/** Defines a variation of red, green and blue on a given time line.
 * @author Inferno */
public class GradientColorValue : ParticleValue {
	static private float[] temp = new float[3];

	private float[] colors = {1, 1, 1};
	public float[] timeline = {0};

	public float[] getTimeline () {
		return timeline;
	}

	public void setTimeline (float[] timeline) {
		this.timeline = timeline;
	}

	public float[] getColors () {
		return colors;
	}

	public void setColors (float[] colors) {
		this.colors = colors;
	}

	public float[] getColor (float percent) {
		getColor(percent, temp, 0);
		return temp;
	}

	public void getColor (float percent, float[] @out, int index) {
		int startIndex = 0, endIndex = -1;
		float[] timeline = this.timeline;
		int n = timeline.Length;
		for (int i = 1; i < n; i++) {
			float t = timeline[i];
			if (t > percent) {
				endIndex = i;
				break;
			}
			startIndex = i;
		}
		float startTime = timeline[startIndex];
		startIndex *= 3;
		float r1 = colors[startIndex];
		float g1 = colors[startIndex + 1];
		float b1 = colors[startIndex + 2];
		if (endIndex == -1) {
			@out[index] = r1;
            @out[index + 1] = g1;
            @out[index + 2] = b1;
			return;
		}
		float factor = (percent - startTime) / (timeline[endIndex] - startTime);
		endIndex *= 3;
        @out[index] = r1 + (colors[endIndex] - r1) * factor;
        @out[index + 1] = g1 + (colors[endIndex + 1] - g1) * factor;
		@out[index + 2] = b1 + (colors[endIndex + 2] - b1) * factor;
	}

	public override void write (Json json) {
		base.write(json);
		json.writeValue("colors", colors);
		json.writeValue("timeline", timeline);
	}

    public override void read (Json json, JsonValue jsonData) {
		base.read(json, jsonData);
		colors = (float[])json.readValue("colors", typeof(float[]), jsonData);
		timeline = (float[])json.readValue("timeline", typeof(float[]), jsonData);
	}

	public void load (GradientColorValue value) {
		base.load(value);
		colors = new float[value.colors.Length];
		Array.Copy(value.colors, 0, colors, 0, colors.Length);
		timeline = new float[value.timeline.Length];
		Array.Copy(value.timeline, 0, timeline, 0, timeline.Length);
	}
}
