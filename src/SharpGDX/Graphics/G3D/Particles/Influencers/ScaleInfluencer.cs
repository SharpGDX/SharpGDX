using System;
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


namespace SharpGDX.Graphics.G3D.Particles.Influencers;

/** It's an {@link Influencer} which controls the scale of the particles.
 * @author Inferno */
public class ScaleInfluencer : SimpleInfluencer {

	public ScaleInfluencer () 
    : base()
    {
		
		valueChannelDescriptor = ParticleChannels.Scale;
	}

    public override void activateParticles (int startIndex, int count) {
		if (value.isRelative()) {
			for (int i = startIndex * valueChannel.strideSize, a = startIndex * interpolationChannel.strideSize,
				c = i + count * valueChannel.strideSize; i < c; i += valueChannel.strideSize, a += interpolationChannel.strideSize) {
				float start = value.newLowValue() * controller._scale.x;
				float diff = value.newHighValue() * controller._scale.x;
				interpolationChannel.data[a + ParticleChannels.InterpolationStartOffset] = start;
				interpolationChannel.data[a + ParticleChannels.InterpolationDiffOffset] = diff;
				valueChannel.data[i] = start + diff * value.getScale(0);
			}
		} else {
			for (int i = startIndex * valueChannel.strideSize, a = startIndex * interpolationChannel.strideSize,
				c = i + count * valueChannel.strideSize; i < c; i += valueChannel.strideSize, a += interpolationChannel.strideSize) {
				float start = value.newLowValue() * controller._scale.x;
				float diff = value.newHighValue() * controller._scale.x - start;
				interpolationChannel.data[a + ParticleChannels.InterpolationStartOffset] = start;
				interpolationChannel.data[a + ParticleChannels.InterpolationDiffOffset] = diff;
				valueChannel.data[i] = start + diff * value.getScale(0);
			}
		}
	}

	public ScaleInfluencer (ScaleInfluencer scaleInfluencer) 
    : base(scaleInfluencer)
    {
		
	}

    public override ParticleControllerComponent copy () {
		return new ScaleInfluencer(this);
	}

}
