using System;
using SharpGDX.Graphics.G3D.Particles;
using static SharpGDX.Graphics.G3D.Particles.ParallelArray;
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


namespace SharpGDX.Graphics.G3D.Particles.Influencers;

/** It's an {@link Influencer} which controls which {@link Model} will be assigned to the particles as {@link ModelInstance}.
 * @author Inferno */
public abstract class ModelInfluencer : Influencer {

	/** Assigns the first model of {@link ModelInfluencer#models} to the particles. */
	public class Single : ModelInfluencer {

		public Single () 
            : base()
        {
			
		}

		public Single (Single influencer) 
        : base(influencer)
        {
            
		}

		public Single (Model[] models) 
        : base(models)
        {
            
		}

        public override void init () {
			Model first = models.first();
			for (int i = 0, c = controller.emitter.maxParticleCount; i < c; ++i) {
				modelChannel.data[i] = new ModelInstance(first);
			}
		}

        public override Single copy () {
			return new Single(this);
		}
	}

	/** Assigns a random model of {@link ModelInfluencer#models} to the particles. */
	public class Random : ModelInfluencer {
		private class ModelInstancePool : Pool<ModelInstance> {
            private readonly ModelInfluencer _modelInfluencer;

            public ModelInstancePool (ModelInfluencer modelInfluencer)
            {
                _modelInfluencer = modelInfluencer;
            }

            protected internal override ModelInstance newObject () {
				return new ModelInstance(_modelInfluencer.models.random());
			}
		}

		ModelInstancePool pool;

		public Random () 
        : base()
        {
			
			pool = new ModelInstancePool(this);
		}

		public Random (Random influencer) 
        : base(influencer)
        {
            
			pool = new ModelInstancePool(this);
		}

		public Random (Model[] models) 
        : base(models)
        {
            
			pool = new ModelInstancePool(this);
		}

        public override void init () {
			pool.clear();
		}

        public override void activateParticles (int startIndex, int count) {
			for (int i = startIndex, c = startIndex + count; i < c; ++i) {
				modelChannel.data[i] = pool.obtain();
			}
		}

        public override void killParticles (int startIndex, int count) {
			for (int i = startIndex, c = startIndex + count; i < c; ++i) {
				pool.free(modelChannel.data[i]);
				modelChannel.data[i] = null;
			}
		}

        public override Random copy () {
			return new Random(this);
		}
	}

	public Array<Model> models;
	ObjectChannel<ModelInstance> modelChannel;

	public ModelInfluencer () {
		this.models = new Array<Model>(true, 1, typeof(Model));
	}

	public ModelInfluencer (Model[] models) {
		this.models = new Array<Model>(models);
	}

	public ModelInfluencer (ModelInfluencer influencer) 
    : this((Model[])influencer.models.toArray<Model>(typeof(Model)))
    {
		
	}

    public override void allocateChannels () {
		modelChannel = controller.particles.addChannel<ObjectChannel<ModelInstance>>(ParticleChannels.ModelInstance);
	}

    public override void save (AssetManager manager, ResourceData<ParticleEffect>resources) {
        ResourceData<ParticleEffect>.SaveData data = resources.createSaveData();
		foreach (Model model in models)
			data.saveAsset<Model>(manager.getAssetFileName(model), typeof(Model));
	}

    public override void load (AssetManager manager, ResourceData<ParticleEffect> resources) {
        ResourceData<ParticleEffect>.SaveData data = resources.getSaveData();
		AssetDescriptor descriptor;
		while ((descriptor = data.loadAsset()) != null) {
			Model model = (Model)manager.get<Model>(descriptor);
			if (model == null) throw new RuntimeException("Model is null");
			models.Add(model);
		}
	}
}
