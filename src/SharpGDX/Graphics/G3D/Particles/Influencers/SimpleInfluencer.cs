using System;
using static SharpGDX.Graphics.G3D.Particles.ParallelArray;
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
using SharpGDX.Graphics.G3D.Particles.Values;


namespace SharpGDX.Graphics.G3D.Particles.Influencers;

/** It's an {@link Influencer} which controls a generic channel of the particles. It handles the interpolation through time using
 * {@link ScaledNumericValue}.
 * @author Inferno */
public abstract class SimpleInfluencer : Influencer {

	public ScaledNumericValue value;
	internal FloatChannel valueChannel, interpolationChannel, lifeChannel;
	internal ChannelDescriptor valueChannelDescriptor;

	public SimpleInfluencer () {
		value = new ScaledNumericValue();
		value.setHigh(1);
	}

	public SimpleInfluencer (SimpleInfluencer billboardScaleinfluencer) 
    : this()
    {
		
		set(billboardScaleinfluencer);
	}

	private void set (SimpleInfluencer scaleInfluencer) {
		value.load(scaleInfluencer.value);
		valueChannelDescriptor = scaleInfluencer.valueChannelDescriptor;
	}

    public override void allocateChannels () {
		valueChannel = controller.particles.addChannel<FloatChannel>(valueChannelDescriptor);
		ParticleChannels.Interpolation.id = controller.particleChannels.newId();
		interpolationChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Interpolation);
		lifeChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Life);
	}

	public override void activateParticles (int startIndex, int count) {
		if (!value.isRelative()) {
			for (int i = startIndex * valueChannel.strideSize, a = startIndex * interpolationChannel.strideSize,
				c = i + count * valueChannel.strideSize; i < c; i += valueChannel.strideSize, a += interpolationChannel.strideSize) {
				float start = value.newLowValue();
				float diff = value.newHighValue() - start;
				interpolationChannel.data[a + ParticleChannels.InterpolationStartOffset] = start;
				interpolationChannel.data[a + ParticleChannels.InterpolationDiffOffset] = diff;
				valueChannel.data[i] = start + diff * value.getScale(0);
			}
		} else {
			for (int i = startIndex * valueChannel.strideSize, a = startIndex * interpolationChannel.strideSize,
				c = i + count * valueChannel.strideSize; i < c; i += valueChannel.strideSize, a += interpolationChannel.strideSize) {
				float start = value.newLowValue();
				float diff = value.newHighValue();
				interpolationChannel.data[a + ParticleChannels.InterpolationStartOffset] = start;
				interpolationChannel.data[a + ParticleChannels.InterpolationDiffOffset] = diff;
				valueChannel.data[i] = start + diff * value.getScale(0);
			}
		}
	}

    public override void update () {
		for (int i = 0, a = 0, l = ParticleChannels.LifePercentOffset, c = i + controller.particles.size
			* valueChannel.strideSize; i < c; i += valueChannel.strideSize, a += interpolationChannel.strideSize, l += lifeChannel.strideSize) {

			valueChannel.data[i] = interpolationChannel.data[a + ParticleChannels.InterpolationStartOffset]
				+ interpolationChannel.data[a + ParticleChannels.InterpolationDiffOffset] * value.getScale(lifeChannel.data[l]);
		}
	}

    public override void write (Json json) {
		json.writeValue("value", value);
	}

    public override void read (Json json, JsonValue jsonData) {
		value = (ScaledNumericValue)json.readValue("value", typeof(ScaledNumericValue), jsonData);
	}

}
