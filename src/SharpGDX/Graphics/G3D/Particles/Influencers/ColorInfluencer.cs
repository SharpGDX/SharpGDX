using System;
using SharpGDX.Graphics.G3D.Particles.Values;
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


namespace SharpGDX.Graphics.G3D.Particles.Influencers;

/** It's an {@link Influencer} which controls particles color and transparency.
 * @author Inferno */
public abstract class ColorInfluencer : Influencer {

	/** It's an {@link Influencer} which assigns a random color when a particle is activated. */
	public class Random : ColorInfluencer {
		FloatChannel colorChannel;

        public override void allocateChannels () {
			colorChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Color);
		}

        public override void activateParticles (int startIndex, int count) {
			for (int i = startIndex * colorChannel.strideSize,
				c = i + count * colorChannel.strideSize; i < c; i += colorChannel.strideSize) {
				colorChannel.data[i + ParticleChannels.RedOffset] = MathUtils.random();
				colorChannel.data[i + ParticleChannels.GreenOffset] = MathUtils.random();
				colorChannel.data[i + ParticleChannels.BlueOffset] = MathUtils.random();
				colorChannel.data[i + ParticleChannels.AlphaOffset] = MathUtils.random();
			}
		}

        public override Random copy () {
			return new Random();
		}
	}

	/** It's an {@link Influencer} which manages the particle color during its life time. */
	public class Single : ColorInfluencer {
		FloatChannel alphaInterpolationChannel;
		FloatChannel lifeChannel;
		public ScaledNumericValue alphaValue;
		public GradientColorValue colorValue;

		public Single () {
			colorValue = new GradientColorValue();
			alphaValue = new ScaledNumericValue();
			alphaValue.setHigh(1);
		}

		public Single (Single billboardColorInfluencer) 
        : this()
        {
			
			set(billboardColorInfluencer);
		}

		public void set (Single colorInfluencer) {
			this.colorValue.load(colorInfluencer.colorValue);
			this.alphaValue.load(colorInfluencer.alphaValue);
		}

        public override void allocateChannels () {
			base.allocateChannels();
			// Hack this allows to share the channel descriptor structure but using a different id temporary
			ParticleChannels.Interpolation.id = controller.particleChannels.newId();
			alphaInterpolationChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Interpolation);
			lifeChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Life);
		}

        public override void activateParticles (int startIndex, int count) {
			for (int i = startIndex * colorChannel.strideSize, a = startIndex * alphaInterpolationChannel.strideSize,
				l = startIndex * lifeChannel.strideSize + ParticleChannels.LifePercentOffset, c = i + count
					* colorChannel.strideSize; i < c; i += colorChannel.strideSize, a += alphaInterpolationChannel.strideSize, l += lifeChannel.strideSize) {
				float alphaStart = alphaValue.newLowValue();
				float alphaDiff = alphaValue.newHighValue() - alphaStart;
				colorValue.getColor(0, colorChannel.data, i);
				colorChannel.data[i + ParticleChannels.AlphaOffset] = alphaStart
					+ alphaDiff * alphaValue.getScale(lifeChannel.data[l]);
				alphaInterpolationChannel.data[a + ParticleChannels.InterpolationStartOffset] = alphaStart;
				alphaInterpolationChannel.data[a + ParticleChannels.InterpolationDiffOffset] = alphaDiff;
			}
		}

        public override void update () {
			for (int i = 0, a = 0, l = ParticleChannels.LifePercentOffset, c = i + controller.particles.size
				* colorChannel.strideSize; i < c; i += colorChannel.strideSize, a += alphaInterpolationChannel.strideSize, l += lifeChannel.strideSize) {

				float lifePercent = lifeChannel.data[l];
				colorValue.getColor(lifePercent, colorChannel.data, i);
				colorChannel.data[i
					+ ParticleChannels.AlphaOffset] = alphaInterpolationChannel.data[a + ParticleChannels.InterpolationStartOffset]
						+ alphaInterpolationChannel.data[a + ParticleChannels.InterpolationDiffOffset]
							* alphaValue.getScale(lifePercent);
			}
		}

        public override Single copy () {
			return new Single(this);
		}

        public override void write (Json json) {
			json.writeValue("alpha", alphaValue);
			json.writeValue("color", colorValue);
		}

        public override void read (Json json, JsonValue jsonData) {
			alphaValue = (ScaledNumericValue)json.readValue("alpha", typeof(ScaledNumericValue), jsonData);
			colorValue = (GradientColorValue)json.readValue("color", typeof(GradientColorValue), jsonData);
		}
	}

	FloatChannel colorChannel;

    public override void allocateChannels () {
		colorChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Color);
	}
}
