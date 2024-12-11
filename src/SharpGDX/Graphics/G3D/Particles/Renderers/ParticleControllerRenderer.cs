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
using SharpGDX.Graphics.G3D.Particles.Batches;
using SharpGDX.Graphics.G3D.Environments;
using FloatChannel = SharpGDX.Graphics.G3D.Particles.ParallelArray.FloatChannel;

namespace SharpGDX.Graphics.G3D.Particles.Renderers;

/** It's a {@link ParticleControllerComponent} which determines how the particles are rendered. It's the base class of every
 * particle renderer.
 * @author Inferno */
public abstract class ParticleControllerRenderer<D , T >
	: ParticleControllerComponent 
where D: ParticleControllerRenderData
where T: ParticleBatch<D>
{
	protected T batch;
	protected D renderData;

	protected ParticleControllerRenderer () {
	}

	protected ParticleControllerRenderer (D renderData) {
		this.renderData = renderData;
	}

	public override void update () {
		batch.draw(renderData);
	}

	// TODO: @SuppressWarnings("unchecked")
	public bool setBatch (ParticleBatch<D> batch) {
		if (isCompatible(batch)) {
			this.batch = (T)batch;
			return true;
		}
		return false;
	}

	public abstract bool isCompatible (ParticleBatch<D> batch);

	public override void set (ParticleController particleController) {
		base.set(particleController);
		if (renderData != null) renderData.controller = controller;
	}
}
