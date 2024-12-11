using System;
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

/** It's an {@link Influencer} which controls which {@link ParticleController} will be assigned to a particle.
 * @author Inferno */
public abstract class ParticleControllerInfluencer : Influencer {

	/** Assigns the first controller of {@link ParticleControllerInfluencer#templates} to the particles. */
	public class Single : ParticleControllerInfluencer {

		public Single (ParticleController[] templates) 
        : base(templates)
        {
			
		}

		public Single () 
        : base()
        {
            
		}

		public Single (Single particleControllerSingle) 
        : base(particleControllerSingle)
        {
            
		}

		public override void init () {
			ParticleController first = templates.first();
			for (int i = 0, c = controller.particles.capacity; i < c; ++i) {
				ParticleController copy = first.copy();
				copy.init();
				particleControllerChannel.data[i] = copy;
			}
		}

        public override void activateParticles (int startIndex, int count) {
			for (int i = startIndex, c = startIndex + count; i < c; ++i) {
				particleControllerChannel.data[i].start();
			}
		}

        public override void killParticles (int startIndex, int count) {
			for (int i = startIndex, c = startIndex + count; i < c; ++i) {
				particleControllerChannel.data[i].end();
			}
		}

        public override Single copy () {
			return new Single(this);
		}
	}

	/** Assigns a random controller of {@link ParticleControllerInfluencer#templates} to the particles. */
	public class Random : ParticleControllerInfluencer {
		private class ParticleControllerPool : Pool<ParticleController> {
            private readonly Random _influencer;

            public ParticleControllerPool (Random influencer)
            {
                _influencer = influencer;
            }

            protected internal override ParticleController newObject () {
				ParticleController controller = _influencer.templates.random().copy();
				controller.init();
				return controller;
			}

            public override void clear () {
				// Dispose every allocated instance because the templates may be changed
				for (int i = 0, free = _influencer.pool.getFree(); i < free; ++i) {
                    _influencer.pool.obtain().Dispose();
				}
				base.clear();
			}
		}

		ParticleControllerPool pool;

		public Random () 
        : base()
        {
			
			pool = new ParticleControllerPool(this);
		}

		public Random (ParticleController[] templates) 
        : base(templates)
        {
			
			pool = new ParticleControllerPool(this);
		}

		public Random (Random particleControllerRandom) 
        : base(particleControllerRandom)
        {
			
			pool = new ParticleControllerPool(this);
		}

        public override void init () {
			pool.clear();
			// Allocate the new instances
			for (int i = 0; i < controller.emitter.maxParticleCount; ++i) {
				pool.free(pool.newObject());
			}
		}

        public override void Dispose () {
			pool.clear();
			base.Dispose();
		}

        public override void activateParticles (int startIndex, int count) {
			for (int i = startIndex, c = startIndex + count; i < c; ++i) {
				ParticleController controller = pool.obtain();
				controller.start();
				particleControllerChannel.data[i] = controller;
			}
		}

        public override void killParticles (int startIndex, int count) {
			for (int i = startIndex, c = startIndex + count; i < c; ++i) {
				ParticleController controller = particleControllerChannel.data[i];
				controller.end();
				pool.free(controller);
				particleControllerChannel.data[i] = null;
			}
		}

        public override Random copy () {
			return new Random(this);
		}
	}

	public Array<ParticleController> templates;
	ObjectChannel<ParticleController> particleControllerChannel;

	public ParticleControllerInfluencer () {
		this.templates = new Array<ParticleController>(true, 1, typeof(ParticleController));
	}

	public ParticleControllerInfluencer (ParticleController[] templates) {
		this.templates = new Array<ParticleController>(templates);
	}

	public ParticleControllerInfluencer (ParticleControllerInfluencer influencer) 
    : this(influencer.templates.items)
    {
		
	}

    public override void allocateChannels () {
		particleControllerChannel = controller.particles.addChannel<ObjectChannel<ParticleController>>(ParticleChannels.ParticleController);
	}

    public override void end () {
		for (int i = 0; i < controller.particles.size; ++i) {
			particleControllerChannel.data[i].end();
		}
	}

    public override void Dispose () {
		if (controller != null) {
			for (int i = 0; i < controller.particles.size; ++i) {
				ParticleController controller = particleControllerChannel.data[i];
				if (controller != null) {
					controller.Dispose();
					particleControllerChannel.data[i] = null;
				}
			}
		}
	}

    public override void save (AssetManager manager, ResourceData<ParticleEffect>resources) {
        ResourceData<ParticleEffect>.SaveData data = resources.createSaveData();
		Array<ParticleEffect> effects = manager.getAll(typeof(ParticleEffect), new Array<ParticleEffect>());

		Array<ParticleController> controllers = new Array<ParticleController>(templates);
		Array<IntArray> effectsIndices = new Array<IntArray>();

		for (int i = 0; i < effects.size && controllers.size > 0; ++i) {
			ParticleEffect effect = effects.Get(i);
			Array<ParticleController> effectControllers = effect.getControllers();
			IEnumerator<ParticleController> iterator = controllers.GetEnumerator();
			IntArray indices = null;
			while (iterator.MoveNext()) {
				ParticleController controller = iterator.Current;
				int index = -1;
				if ((index = effectControllers.indexOf(controller, true)) > -1) {
					if (indices == null) {
						indices = new IntArray();
					}
					// TODO: iterator.remove();
					indices.add(index);
				}
			}

			if (indices != null) {
				data.saveAsset<ParticleEffect>(manager.getAssetFileName(effect), typeof(ParticleEffect));
				effectsIndices.Add(indices);
			}
		}
		data.save("indices", effectsIndices);
	}

    public override void load (AssetManager manager, ResourceData<ParticleEffect> resources) {
		ResourceData<ParticleEffect>.SaveData data = resources.getSaveData();
		Array<IntArray> effectsIndices = data.load<Array<IntArray>>("indices");
		AssetDescriptor descriptor;
		var iterator = effectsIndices.GetEnumerator();
		while ((descriptor = data.loadAsset()) != null) {
			ParticleEffect effect = (ParticleEffect)manager.get<ParticleEffect>(descriptor);
			if (effect == null) throw new RuntimeException("Template is null");
			Array<ParticleController> effectControllers = effect.getControllers();
			IntArray effectIndices = iterator.Current;

			for (int i = 0, n = effectIndices.size; i < n; i++) {
				templates.Add(effectControllers.Get(effectIndices.get(i)));
			}
		}
	}
}
