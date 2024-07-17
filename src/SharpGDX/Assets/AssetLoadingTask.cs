using SharpGDX.Files;
using System;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Assets.Loaders.Resolvers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Utils.Async;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets
{
	/** Responsible for loading an asset through an {@link AssetLoader} based on an {@link AssetDescriptor}. Please don't forget to
 * update the overriding emu file on GWT backend when changing this file!
 * 
 * @author mzechner */
class AssetLoadingTask : IAsyncTask<object> {
	AssetManager manager;
	internal readonly IAssetDescriptor assetDesc;
	readonly IAssetLoader loader;
	readonly AsyncExecutor executor;
	internal readonly long startTime;

	volatile bool asyncDone;
	internal volatile bool dependenciesLoaded;
	internal volatile Array<IAssetDescriptor> dependencies;
	volatile AsyncResult<object> depsFuture;
	volatile AsyncResult<object> loadFuture;
	internal volatile Object asset;

	internal volatile bool cancel;

	public AssetLoadingTask (AssetManager manager, IAssetDescriptor assetDesc, IAssetLoader loader, AsyncExecutor threadPool) {
		this.manager = manager;
		this.assetDesc = assetDesc;
		this.loader = loader;
		this.executor = threadPool;
		startTime = manager.log.getLevel() == Logger.DEBUG ? TimeUtils.nanoTime() : 0;
	}

	/** Loads parts of the asset asynchronously if the loader is an {@link AsynchronousAssetLoader}. */
	public object? call () // TODO: throws Exception
                     {
		if (cancel) return null;
		IAsynchronousAssetLoader asyncLoader = (IAsynchronousAssetLoader)loader;
		if (!dependenciesLoaded) {
			dependencies = asyncLoader.getDependencies(assetDesc.FileName, resolve(loader, assetDesc), assetDesc.Parameters);
			if (dependencies != null) {
				removeDuplicates(dependencies);
				manager.injectDependencies(assetDesc.FileName, dependencies);
			} else {
				// if we have no dependencies, we load the async part of the task immediately.
				asyncLoader.loadAsync(manager, assetDesc.FileName, resolve(loader, assetDesc), assetDesc.Parameters);
				asyncDone = true;
			}
		} else {
			asyncLoader.loadAsync(manager, assetDesc.FileName, resolve(loader, assetDesc), assetDesc.Parameters);
			asyncDone = true;
		}
		return null;
	}

	/** Updates the loading of the asset. In case the asset is loaded with an {@link AsynchronousAssetLoader}, the loaders
	 * {@link AsynchronousAssetLoader#loadAsync(AssetManager, String, FileHandle, AssetLoaderParameters)} method is first called on
	 * a worker thread. Once this method returns, the rest of the asset is loaded on the rendering thread via
	 * {@link AsynchronousAssetLoader#loadSync(AssetManager, String, FileHandle, AssetLoaderParameters)}.
	 * @return true in case the asset was fully loaded, false otherwise
	 * @throws GdxRuntimeException */
	public bool update () {
		if (loader is ISynchronousAssetLoader)
			handleSyncLoader();
		else
			handleAsyncLoader();
		return asset != null;
	}

	private void handleSyncLoader () {
		ISynchronousAssetLoader syncLoader = (ISynchronousAssetLoader)loader;
		if (!dependenciesLoaded) {
			dependenciesLoaded = true;
			dependencies = syncLoader.getDependencies(assetDesc.FileName, resolve(loader, assetDesc), assetDesc.Parameters);
			if (dependencies == null) {
				asset = syncLoader.load(manager, assetDesc.FileName, resolve(loader, assetDesc), assetDesc.Parameters);
				return;
			}
			removeDuplicates(dependencies);
			manager.injectDependencies(assetDesc.FileName, dependencies);
		} else
			asset = syncLoader.load(manager, assetDesc.FileName, resolve(loader, assetDesc), assetDesc.Parameters);
	}

	private void handleAsyncLoader () {
		IAsynchronousAssetLoader asyncLoader = (IAsynchronousAssetLoader)loader;
		if (!dependenciesLoaded) {
			if (depsFuture == null)
				depsFuture = executor.submit(this);
			else if (depsFuture.isDone()) {
				try {
					depsFuture.get();
				} catch (Exception e) {
					throw new GdxRuntimeException("Couldn't load dependencies of asset: " + assetDesc.FileName, e);
				}
				dependenciesLoaded = true;
				if (asyncDone)
					asset = asyncLoader.loadSync(manager, assetDesc.FileName, resolve(loader, assetDesc), assetDesc.Parameters);
			}
		} else if (loadFuture == null && !asyncDone)
			loadFuture = executor.submit(this);
		else if (asyncDone)
			asset = asyncLoader.loadSync(manager, assetDesc.FileName, resolve(loader, assetDesc), assetDesc.Parameters);
		else if (loadFuture.isDone()) {
			try {
				loadFuture.get();
			} catch (Exception e) {
				throw new GdxRuntimeException("Couldn't load asset: " + assetDesc.FileName, e);
			}
			asset = asyncLoader.loadSync(manager, assetDesc.FileName, resolve(loader, assetDesc), assetDesc.Parameters);
		}
	}

	/** Called when this task is the task that is currently being processed and it is unloaded. */
	public void unload () {
		if (loader is IAsynchronousAssetLoader)
			((IAsynchronousAssetLoader)loader).unloadAsync(manager, assetDesc.FileName, resolve(loader, assetDesc), assetDesc.Parameters);
	}

	private FileHandle resolve (IAssetLoader loader, IAssetDescriptor assetDesc) {
		if (assetDesc.File == null) assetDesc.File = loader.resolve(assetDesc.FileName);
		return assetDesc.File;
	}

	private void removeDuplicates (Array<IAssetDescriptor> array) {
		bool ordered = array.ordered;
		array.ordered = true;
		for (int i = 0; i < array.size; ++i) {
			 String fn = array.get(i).FileName;
			 Type type = array.get(i).Type;
			for (int j = array.size - 1; j > i; --j)
				if (type == array.get(j).Type && fn.Equals(array.get(j).FileName)) array.removeIndex(j);
		}
		array.ordered = ordered;
	}
}
}
