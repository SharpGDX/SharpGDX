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
using static SharpGDX.Graphics.G3D.Particles.ParallelArray;


namespace SharpGDX.Graphics.G3D.Particles.Influencers;

/** It's an {@link Influencer} which updates the simulation of particles containing a {@link ParticleController}. Must be the last
 * influencer to be updated, so it has to be placed at the end of the influencers list when creating a {@link ParticleController}.
 * @author Inferno */
public class ParticleControllerFinalizerInfluencer : Influencer {
	FloatChannel positionChannel, scaleChannel, rotationChannel;
	ObjectChannel<ParticleController> controllerChannel;
	bool hasScale, hasRotation;

	public ParticleControllerFinalizerInfluencer () {
	}

    public override void init () {
		controllerChannel = controller.particles.getChannel<ObjectChannel<ParticleController>>(ParticleChannels.ParticleController);
		if (controllerChannel == null) throw new GdxRuntimeException(
			"ParticleController channel not found, specify an influencer which will allocate it please.");
		scaleChannel = controller.particles.getChannel<FloatChannel>(ParticleChannels.Scale);
		rotationChannel = controller.particles.getChannel<FloatChannel>(ParticleChannels.Rotation3D);
		hasScale = scaleChannel != null;
		hasRotation = rotationChannel != null;
	}

	public override void allocateChannels () {
		positionChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Position);
	}

    public override void update () {
		for (int i = 0, positionOffset = 0,
			c = controller.particles.size; i < c; ++i, positionOffset += positionChannel.strideSize) {
			ParticleController particleController = controllerChannel.data[i];
			float scale = hasScale ? scaleChannel.data[i] : 1;
			float qx = 0, qy = 0, qz = 0, qw = 1;
			if (hasRotation) {
				int rotationOffset = i * rotationChannel.strideSize;
				qx = rotationChannel.data[rotationOffset + ParticleChannels.XOffset];
				qy = rotationChannel.data[rotationOffset + ParticleChannels.YOffset];
				qz = rotationChannel.data[rotationOffset + ParticleChannels.ZOffset];
				qw = rotationChannel.data[rotationOffset + ParticleChannels.WOffset];
			}
			particleController.setTransform(positionChannel.data[positionOffset + ParticleChannels.XOffset],
				positionChannel.data[positionOffset + ParticleChannels.YOffset],
				positionChannel.data[positionOffset + ParticleChannels.ZOffset], qx, qy, qz, qw, scale);
			particleController.update();
		}
	}

    public override ParticleControllerFinalizerInfluencer copy () {
		return new ParticleControllerFinalizerInfluencer();
	}
}
