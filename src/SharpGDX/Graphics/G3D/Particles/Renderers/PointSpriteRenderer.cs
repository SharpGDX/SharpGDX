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
using SharpGDX.Graphics.G3D.Particles.Batches;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Environments;
using FloatChannel = SharpGDX.Graphics.G3D.Particles.ParallelArray.FloatChannel;
using static SharpGDX.Graphics.G3D.Particles.ParticleChannels;

namespace SharpGDX.Graphics.G3D.Particles.Renderers;

/** A {@link ParticleControllerRenderer} which will render particles as point sprites to a {@link PointSpriteParticleBatch} .
 * @author Inferno */
public class PointSpriteRenderer : ParticleControllerRenderer<PointSpriteControllerRenderData, PointSpriteParticleBatch> {
	public PointSpriteRenderer () 
    : base(new PointSpriteControllerRenderData())
    {
		
	}

	public PointSpriteRenderer (PointSpriteParticleBatch batch) 
    : this()
    {
		
		setBatch(batch);
	}

	public override void allocateChannels () {
		renderData.positionChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Position);
		renderData.regionChannel = controller.particles.addChannel(ParticleChannels.TextureRegion, TextureRegionInitializer.get());
		renderData.colorChannel = controller.particles.addChannel(ParticleChannels.Color, ColorInitializer.get());
		renderData.scaleChannel = controller.particles.addChannel(ParticleChannels.Scale, ScaleInitializer.get());
		renderData.rotationChannel = controller.particles.addChannel(ParticleChannels.Rotation2D, Rotation2dInitializer.get());
	}

	public override bool isCompatible (ParticleBatch<PointSpriteControllerRenderData> batch) {
		return batch is PointSpriteParticleBatch;
	}

	public override ParticleControllerComponent copy () {
		return new PointSpriteRenderer(batch);
	}

}
