using System;
using static SharpGDX.Graphics.G3D.Particles.ParallelArray;
using SharpGDX.Graphics.G3D.Attributess;
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
using SharpGDX.Graphics.G3D.Particles.Batches;
using FloatChannel = SharpGDX.Graphics.G3D.Particles.ParallelArray.FloatChannel;

namespace SharpGDX.Graphics.G3D.Particles.Renderers;

/** A {@link ParticleControllerRenderer} which will render particles as {@link ModelInstance} to a
 * {@link ModelInstanceParticleBatch}.
 * @author Inferno */
public class ModelInstanceRenderer
	: ParticleControllerRenderer<ModelInstanceControllerRenderData, ModelInstanceParticleBatch> {
	private bool hasColor, hasScale, hasRotation;

	public ModelInstanceRenderer () 
    : base(new ModelInstanceControllerRenderData())
    {
		
	}

	public ModelInstanceRenderer (ModelInstanceParticleBatch batch) 
    : this()
    {
		
		setBatch(batch);
	}

	public override void allocateChannels () {
		renderData.positionChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Position);
	}

	public override void init () {
		renderData.modelInstanceChannel = controller.particles.getChannel<ObjectChannel<ModelInstance>>(ParticleChannels.ModelInstance);
		renderData.colorChannel = controller.particles.getChannel<FloatChannel>(ParticleChannels.Color);
		renderData.scaleChannel = controller.particles.getChannel<FloatChannel>(ParticleChannels.Scale);
		renderData.rotationChannel = controller.particles.getChannel<FloatChannel>(ParticleChannels.Rotation3D);
		hasColor = renderData.colorChannel != null;
		hasScale = renderData.scaleChannel != null;
		hasRotation = renderData.rotationChannel != null;
	}

	public override void update () {
		for (int i = 0, positionOffset = 0,
			c = controller.particles.size; i < c; ++i, positionOffset += renderData.positionChannel.strideSize) {
			ModelInstance instance = renderData.modelInstanceChannel.data[i];
			float scale = hasScale ? renderData.scaleChannel.data[i] : 1;
			float qx = 0, qy = 0, qz = 0, qw = 1;
			if (hasRotation) {
				int rotationOffset = i * renderData.rotationChannel.strideSize;
				qx = renderData.rotationChannel.data[rotationOffset + ParticleChannels.XOffset];
				qy = renderData.rotationChannel.data[rotationOffset + ParticleChannels.YOffset];
				qz = renderData.rotationChannel.data[rotationOffset + ParticleChannels.ZOffset];
				qw = renderData.rotationChannel.data[rotationOffset + ParticleChannels.WOffset];
			}

			instance.transform.set(renderData.positionChannel.data[positionOffset + ParticleChannels.XOffset],
				renderData.positionChannel.data[positionOffset + ParticleChannels.YOffset],
				renderData.positionChannel.data[positionOffset + ParticleChannels.ZOffset], qx, qy, qz, qw, scale, scale, scale);
			if (hasColor) {
				int colorOffset = i * renderData.colorChannel.strideSize;
				ColorAttribute colorAttribute = (ColorAttribute)instance.materials.Get(0).get(ColorAttribute.Diffuse);
				BlendingAttribute blendingAttribute = (BlendingAttribute)instance.materials.Get(0).get(BlendingAttribute.Type);
				colorAttribute.color.R = renderData.colorChannel.data[colorOffset + ParticleChannels.RedOffset];
				colorAttribute.color.G = renderData.colorChannel.data[colorOffset + ParticleChannels.GreenOffset];
				colorAttribute.color.B = renderData.colorChannel.data[colorOffset + ParticleChannels.BlueOffset];
				if (blendingAttribute != null)
					blendingAttribute.opacity = renderData.colorChannel.data[colorOffset + ParticleChannels.AlphaOffset];
			}
		}
		base.update();
	}

	public override ParticleControllerComponent copy () {
		return new ModelInstanceRenderer(batch);
	}

	public override bool isCompatible (ParticleBatch<ModelInstanceControllerRenderData> batch) {
		return batch is ModelInstanceParticleBatch;
	}

}
