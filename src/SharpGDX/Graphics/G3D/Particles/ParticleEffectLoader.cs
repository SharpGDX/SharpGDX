using System;
using SharpGDX.Graphics.G2D;
using SharpGDX.Utils.Reflect;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G3D.Models.Data;
using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Graphics.G3D.Particles.Batches;
using SharpGDX.Graphics.G3D.Particles.Renderers;

namespace SharpGDX.Graphics.G3D.Particles;

/** This class can save and load a {@link ParticleEffect}. It should be added as {@link AsynchronousAssetLoader} to the
 * {@link AssetManager} so it will be able to load the effects. It's important to note that the two classes
 * {@link ParticleEffectLoadParameter} and {@link ParticleEffectSaveParameter} should be passed in whenever possible, because when
 * present the batches settings will be loaded automatically. When the load and save parameters are absent, once the effect will
 * be created, one will have to set the required batches manually otherwise the {@link ParticleController} instances contained
 * inside the effect will not be able to render themselves.
 * @author inferno */
public class ParticleEffectLoader
	: AsynchronousAssetLoader<ParticleEffect, ParticleEffectLoader.ParticleEffectLoadParameter> {
	protected Array<ObjectMap<String, ResourceData<ParticleEffect>>.Entry> items = new Array<ObjectMap<String, ResourceData<ParticleEffect>>.Entry>();

	public ParticleEffectLoader (IFileHandleResolver resolver) 
    : base(resolver)
    {
		
	}

    public override void loadAsync (AssetManager manager, String fileName, FileHandle file, ParticleEffectLoadParameter parameter) {
	}

	public override Array<AssetDescriptor> getDependencies (String fileName, FileHandle file, ParticleEffectLoadParameter parameter) {
		Json json = new Json();
		ResourceData<ParticleEffect> data = (ResourceData < ParticleEffect > )json.fromJson(typeof(ResourceData<ParticleEffect>), file);
		Array<ResourceData<ParticleEffect>.AssetData> assets = null;
		lock (items) {
			ObjectMap<String, ResourceData<ParticleEffect>>.Entry entry = new ObjectMap<String, ResourceData<ParticleEffect>>.Entry();
			entry.key = fileName;
			entry.value = data;
			items.Add(entry);
			assets = data.getAssets();
		}

		Array<AssetDescriptor> descriptors = new Array<AssetDescriptor>();
		foreach (var assetData in assets) {

			// If the asset doesn't exist try to load it from loading effect directory
			if (!resolve(assetData.filename).exists()) {
				assetData.filename = file.parent().child(GDX.Files.Internal(assetData.filename).name()).path();
			}

			if (assetData.type == typeof(ParticleEffect)) {
				descriptors.Add(new AssetDescriptor(assetData.filename, assetData.type, parameter));
			} else
				descriptors.Add(new AssetDescriptor(assetData.filename, assetData.type));
		}

		return descriptors;

	}

	/** Saves the effect to the given file contained in the passed in parameter. */
	public void save (ParticleEffect effect, ParticleEffectSaveParameter parameter) // TODO: throws IOException 
    {
		ResourceData<ParticleEffect> data = new ResourceData<ParticleEffect>(effect);

		// effect assets
		effect.save(parameter.manager, data);

		// Batches configurations
		if (parameter.batches != null) {
			foreach (var batch in parameter.batches) {
				bool save = false;
				foreach (var controller in effect.getControllers()) {
					if (controller.renderer.isCompatible(batch)) {
						save = true;
						break;
					}
				}

				if (save) batch.save(parameter.manager, data);
			}
		}

		// save
		Json json = new Json(parameter.jsonOutputType);
		if (parameter.prettyPrint) {
			String prettyJson = json.prettyPrint(data);
			parameter.file.writeString(prettyJson, false);
		} else {
			json.toJson(data, parameter.file);
		}
	}

    public override ParticleEffect loadSync (AssetManager manager, String fileName, FileHandle file,
		ParticleEffectLoadParameter parameter) {
		ResourceData<ParticleEffect> effectData = null;
		lock (items) {
			for (int i = 0; i < items.size; ++i) {
				var entry = items.Get(i);
				if (entry.key.Equals(fileName)) {
					effectData = entry.value;
					items.RemoveIndex(i);
					break;
				}
			}
		}

		effectData.resource.load(manager, effectData);
		if (parameter != null) {
			if (parameter.batches != null) {
				foreach (var batch in parameter.batches) {
					batch.load(manager, effectData);
				}
			}
			effectData.resource.setBatch(parameter.batches);
		}
		return effectData.resource;
	}

	private  T? find<T>(Array<T> array, Type type) {
		foreach (Object obj in array) {
			if (ClassReflection.isAssignableFrom(type, obj.GetType())) return (T)obj;
		}
		return default;
	}

	public  class ParticleEffectLoadParameter : AssetLoaderParameters {
		internal Array<ParticleBatch<ParticleControllerRenderData>> batches;

		public ParticleEffectLoadParameter (Array<ParticleBatch<ParticleControllerRenderData>> batches) {
			this.batches = batches;
		}
	}

	public  class ParticleEffectSaveParameter : AssetLoaderParameters {
		/** Optional parameters, but should be present to correctly load the settings */
		internal Array<ParticleBatch<ParticleControllerRenderData>> batches;

		/** Required parameters */
		internal FileHandle file;
		internal AssetManager manager;
		internal JsonWriter.OutputType jsonOutputType;
		internal bool prettyPrint;

		public ParticleEffectSaveParameter (FileHandle file, AssetManager manager, Array<ParticleBatch<ParticleControllerRenderData>> batches) 
        : this(file, manager, batches, JsonWriter.OutputType.minimal, false)
        {
			
		}

		public ParticleEffectSaveParameter (FileHandle file, AssetManager manager, Array<ParticleBatch<ParticleControllerRenderData>> batches,
			JsonWriter.OutputType jsonOutputType, bool prettyPrint) {
			this.batches = batches;
			this.file = file;
			this.manager = manager;
			this.jsonOutputType = jsonOutputType;
			this.prettyPrint = prettyPrint;
		}
	}

}
