using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Graphics.G3D;
using SharpGDX.Graphics.G3D.Models.Data;
using SharpGDX.Graphics.G3D.Utils;
using SharpGDX.Files;

namespace SharpGDX.Assets.Loaders;

public abstract class ModelLoader : AsynchronousAssetLoader<Model, ModelLoader.ModelParameters>
{
	public ModelLoader(IFileHandleResolver resolver)
	: base(resolver)
    {
		
	}

	protected Array<ObjectMap<string, ModelData>.Entry> items = new Array<ObjectMap<string, ModelData>.Entry>();
	protected ModelParameters defaultParameters = new ModelParameters();

	/** Directly load the raw model data on the calling thread. */
	public abstract ModelData loadModelData(FileHandle fileHandle, ModelParameters parameters);

	/** Directly load the raw model data on the calling thread. */
	public ModelData loadModelData(FileHandle fileHandle)
	{
		return loadModelData(fileHandle, null);
	}

	/** Directly load the model on the calling thread. The model with not be managed by an {@link AssetManager}. */
	public Model loadModel(FileHandle fileHandle, TextureProvider textureProvider, ModelParameters parameters)
	{
		ModelData data = loadModelData(fileHandle, parameters);
		return data == null ? null : new Model(data, textureProvider);
	}

	/** Directly load the model on the calling thread. The model with not be managed by an {@link AssetManager}. */
	public Model loadModel(FileHandle fileHandle, ModelParameters parameters)
	{
		return loadModel(fileHandle, new TextureProvider.FileTextureProvider(), parameters);
	}

	/** Directly load the model on the calling thread. The model with not be managed by an {@link AssetManager}. */
	public Model loadModel(FileHandle fileHandle, TextureProvider textureProvider)
	{
		return loadModel(fileHandle, textureProvider, null);
	}

	/** Directly load the model on the calling thread. The model with not be managed by an {@link AssetManager}. */
	public Model loadModel(FileHandle fileHandle)
	{
		return loadModel(fileHandle, new TextureProvider.FileTextureProvider(), null);
	}
    
    public override Array<AssetDescriptor>? getDependencies(String fileName, FileHandle file, ModelParameters parameters)
	{
		Array<AssetDescriptor> deps = new ();
		ModelData data = loadModelData(file, parameters);
		if (data == null) return deps;

		var item = new ObjectMap<String, ModelData>.Entry();
		item.key = fileName;
		item.value = data;
		lock(items) {
			items.Add(item);
		}

		TextureLoader.TextureParameter textureParameter = (parameters != null) ? parameters.textureParameter
			: defaultParameters.textureParameter;

		foreach ( ModelMaterial modelMaterial in data.materials)
		{
			if (modelMaterial.textures != null)
			{
				foreach ( ModelTexture modelTexture in modelMaterial.textures)
					deps.Add(new AssetDescriptor(modelTexture.fileName, typeof(Texture), textureParameter));
			}
		}
		return deps;
	}

	public override void loadAsync(AssetManager manager, String fileName, FileHandle file, ModelParameters parameters)
{
}

	public override Model loadSync(AssetManager manager, String fileName, FileHandle file, ModelParameters parameters)
{
	ModelData? data = null;
	lock(items) {
		for (int i = 0; i < items.size; i++)
		{
			if (items.Get(i).key.Equals(fileName))
			{
				data = items.Get(i).value;
				items.RemoveIndex(i);
			}
		}
	}
	if (data == null) return null;
	 Model result = new Model(data, new TextureProvider.AssetTextureProvider(manager));
	// need to remove the textures from the managed disposables, or else ref counting
	// doesn't work!
	IEnumerator<IDisposable> disposables = result.getManagedDisposables().GetEnumerator();
	while (disposables.MoveNext())
	{
		IDisposable disposable = disposables.Current;

		// TODO: Need to remove them. -RP
		//if (disposable is Texture) {
		//	disposables.remove();
		//}
	}
	return result;
}

public class ModelParameters : AssetLoaderParameters {
		public TextureLoader.TextureParameter textureParameter;

public ModelParameters ()
{
	textureParameter = new TextureLoader.TextureParameter();
	textureParameter.minFilter = textureParameter.magFilter = Texture.TextureFilter.Linear;
	textureParameter.wrapU = textureParameter.wrapV = Texture.TextureWrap.Repeat;
}
	}
}
