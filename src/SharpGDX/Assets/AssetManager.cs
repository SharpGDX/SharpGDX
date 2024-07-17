using System;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Utils.Reflect;
using SharpGDX.Utils.Async;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Assets.Loaders;
using SharpGDX.Assets.Loaders.Resolvers;
using SharpGDX.Audio;
using SharpGDX.Scenes.Scene2D.UI;

namespace SharpGDX.Assets
{
	/// <summary>
	/// Loads and stores assets like textures, bitmap fonts, tile maps, sounds, music and so on.
	/// </summary>
	public class AssetManager : Disposable
	{
		readonly ObjectMap<Type, ObjectMap<String, RefCountedContainer>> assets = new();
		readonly ObjectMap<String, Type> assetTypes = new();
		readonly ObjectMap<String, Array<String>> assetDependencies = new();
		readonly ObjectSet<String> injected = new();

		readonly ObjectMap<Type, ObjectMap<String, IAssetLoader>> loaders = new();
		readonly Array<IAssetDescriptor> loadQueue = new();
		readonly AsyncExecutor executor;

		readonly Array<AssetLoadingTask> tasks = new();
		IAssetErrorListener listener;
		int loaded;
		int toLoad;
		int peakTasks;

		readonly IFileHandleResolver resolver;

		internal Logger log = new Logger("AssetManager", IApplication.LOG_NONE);

		/// <summary>
		/// Creates a new AssetManager with all default loaders.
		/// </summary>
		public AssetManager()
			: this(new InternalFileHandleResolver())
		{

		}

		/// <summary>
		/// Creates a new AssetManager with all default loaders.
		/// </summary>
		/// <param name="resolver"></param>
		public AssetManager(IFileHandleResolver resolver)
			: this(resolver, true)
		{

		}

		/// <summary>
		/// Creates a new AssetManager with optionally all default loaders.
		/// </summary>
		/// <remarks>
		/// If you don't add the default loaders then you do have to manually add the loaders you need, including any loaders they might depend on.
		/// </remarks>
		/// <param name="resolver"></param>
		/// <param name="defaultLoaders">Whether to add the default loaders.</param>
		public AssetManager(IFileHandleResolver resolver, bool defaultLoaders)
		{
			this.resolver = resolver;
			if (defaultLoaders)
			{
				setLoader(typeof(BitmapFont), new BitmapFontLoader(resolver));
				setLoader(typeof(IMusic), new MusicLoader(resolver));
				setLoader(typeof(Pixmap), new PixmapLoader(resolver));
				setLoader(typeof(ISound), new SoundLoader(resolver));
				setLoader(typeof(TextureAtlas), new TextureAtlasLoader(resolver));
				setLoader(typeof(Texture), new TextureLoader(resolver));
				// TODO: setLoader(typeof(Skin), new SkinLoader(resolver));
				setLoader(typeof(ParticleEffect), new ParticleEffectLoader(resolver));
				setLoader(typeof(ParticleEffect), new ParticleEffectLoader(resolver));
				setLoader(typeof(PolygonRegion), new PolygonRegionLoader(resolver));
				// TODO: setLoader(I18NBundle.class, new I18NBundleLoader(resolver));
				// TODO: setLoader(Model.class, ".g3dj", new G3dModelLoader(new JsonReader(), resolver));
				// TODO: setLoader(Model.class, ".g3db", new G3dModelLoader(new UBJsonReader(), resolver));
				// TODO: setLoader(Model.class, ".obj", new ObjLoader(resolver));
				setLoader(typeof(ShaderProgram), new ShaderProgramLoader(resolver));
				setLoader(typeof(Cubemap), new CubemapLoader(resolver));
			}

			executor = new AsyncExecutor(1, "AssetManager");
		}

		/// <summary>
		/// Returns the <see cref="IFileHandleResolver"/> for which this AssetManager was loaded with.
		/// </summary>
		/// <returns>The file handle resolver which this AssetManager uses.</returns>
		public IFileHandleResolver getFileHandleResolver()
		{
			return resolver;
		}

		/** @param fileName the asset file name
		 * @return the asset
		 * @throws GdxRuntimeException if the asset is not loaded */
		public T get<T>(String fileName)
		{
			lock (this)
			{
				return get<T>(fileName, true);
			}
		}

		/** @param fileName the asset file name
		 * @param type the asset type
		 * @return the asset
		 * @throws GdxRuntimeException if the asset is not loaded */
		public T get<T>(String fileName, Type type)
		{
			lock (this)
			{
				return get<T>(fileName, type, true);
			}
		}

		/** @param fileName the asset file name
		 * @param required true to throw GdxRuntimeException if the asset is not loaded, else null is returned
		 * @return the asset or null if it is not loaded and required is false */
		public T? get<T>(String fileName, bool required)
		{
			lock (this)
			{
				Type type = assetTypes.get(fileName);
				if (type != null)
				{
					ObjectMap<String, RefCountedContainer> assetsByType = assets.get(type);
					if (assetsByType != null)
					{
						RefCountedContainer assetContainer = assetsByType.get(fileName);
						if (assetContainer != null) return (T)assetContainer.@object;
					}
				}

				if (required) throw new GdxRuntimeException("Asset not loaded: " + fileName);
				return default;
			}
		}

		/** @param fileName the asset file name
		 * @param type the asset type
		 * @param required true to throw GdxRuntimeException if the asset is not loaded, else null is returned
		 * @return the asset or null if it is not loaded and required is false */
		public T? get<T>(String fileName, Type type, bool required)
		{
			lock (this)
			{
				ObjectMap<String, RefCountedContainer> assetsByType = assets.get(type);
				if (assetsByType != null)
				{
					RefCountedContainer assetContainer = assetsByType.get(fileName);
					if (assetContainer != null) return (T)assetContainer.@object;
				}

				if (required) throw new GdxRuntimeException("Asset not loaded: " + fileName);
				return default;
			}
		}

		/** @param assetDescriptor the asset descriptor
		 * @return the asset
		 * @throws GdxRuntimeException if the asset is not loaded */
		public T get<T>(AssetDescriptor<T> assetDescriptor)
		{
			lock (this)
				return get<T>(assetDescriptor.FileName, assetDescriptor.Type, true);
		}

		/** @param type the asset type
		 * @return all the assets matching the specified type */
		public Array<T> getAll<T>(Type type, Array<T> @out)
		{
			lock (this)
			{
				ObjectMap<String, RefCountedContainer> assetsByType = assets.get(type);
				if (assetsByType != null)
				{
					foreach (RefCountedContainer assetRef in assetsByType.values())
						@out.add((T)assetRef.@object);
				}

				return @out;
			}
		}

		/** Returns true if an asset with the specified name is loading, queued to be loaded, or has been loaded. */
		public bool contains(String fileName)
		{
			lock (this)
			{
				if (tasks.size > 0 && tasks.first().assetDesc.FileName.Equals(fileName)) return true;

				for (int i = 0; i < loadQueue.size; i++)
					if (loadQueue.get(i).FileName.Equals(fileName))
						return true;

				return isLoaded(fileName);
			}
		}

		/** Returns true if an asset with the specified name and type is loading, queued to be loaded, or has been loaded. */
		public bool contains(String fileName, Type type)
		{
			lock (this)
			{
				if (tasks.size > 0)
				{
					var assetDesc = tasks.first().assetDesc;
					if (assetDesc.Type == type && assetDesc.FileName.Equals(fileName)) return true;
				}

				for (int i = 0; i < loadQueue.size; i++)
				{
					var assetDesc = loadQueue.get(i);
					if (assetDesc.Type == type && assetDesc.FileName.Equals(fileName)) return true;
				}

				return isLoaded(fileName, type);
			}
		}

		/** Removes the asset and all its dependencies, if they are not used by other assets.
		 * @param fileName the file name */
		public void unload(String fileName)
		{
			lock (this)
			{
				// check if it's currently processed (and the first element in the stack, thus not a dependency) and cancel if necessary
				if (tasks.size > 0)
				{
					AssetLoadingTask currentTask = tasks.first();
					if (currentTask.assetDesc.FileName.Equals(fileName))
					{
						log.info("Unload (from tasks): " + fileName);
						currentTask.cancel = true;
						currentTask.unload();
						return;
					}
				}

				Type type = assetTypes.get(fileName);

				// check if it's in the queue
				int foundIndex = -1;
				for (int i = 0; i < loadQueue.size; i++)
				{
					if (loadQueue.get(i).FileName.Equals(fileName))
					{
						foundIndex = i;
						break;
					}
				}

				if (foundIndex != -1)
				{
					toLoad--;
					var desc = loadQueue.removeIndex(foundIndex);
					log.info("Unload (from queue): " + fileName);

					// if the queued asset was already loaded, let the callback know it is available.
					if (type != null && desc.Parameters != null && desc.Parameters.loadedCallback != null)
						desc.Parameters.loadedCallback.Invoke(this, desc.FileName, desc.Type);
					return;
				}

				if (type == null) throw new GdxRuntimeException("Asset not loaded: " + fileName);

				RefCountedContainer assetRef = assets.get(type).get(fileName);

				// if it is reference counted, decrement ref count and check if we can really get rid of it.
				assetRef.refCount--;
				if (assetRef.refCount <= 0)
				{
					log.info("Unload (dispose): " + fileName);

					// if it is disposable dispose it
					if (assetRef.@object is Disposable) ((Disposable)assetRef.@object).dispose();

					// remove the asset from the manager.
					assetTypes.remove(fileName);
					assets.get(type).remove(fileName);
				}
				else
					log.info("Unload (decrement): " + fileName);

				// remove any dependencies (or just decrement their ref count).
				Array<String> dependencies = assetDependencies.get(fileName);
				if (dependencies != null)
				{
					foreach (String dependency in dependencies)
						if (isLoaded(dependency))
							unload(dependency);
				}

				// remove dependencies if ref count < 0
				if (assetRef.refCount <= 0) assetDependencies.remove(fileName);
			}
		}

		/** @param asset the asset
		 * @return whether the asset is contained in this manager */
		public bool containsAsset<T>(T asset)
		{
			lock (this)
			{
				ObjectMap<String, RefCountedContainer> assetsByType = assets.get(asset.GetType());
				if (assetsByType == null) return false;
				foreach (RefCountedContainer assetRef in assetsByType.values())
					if (ReferenceEquals(assetRef.@object, asset) || asset.Equals(assetRef.@object))
						return true;
				return false;
			}
		}

		/** @param asset the asset
		 * @return the filename of the asset or null */
		public String getAssetFileName<T>(T asset)
		{
			lock (this)
			{
				foreach (var assetType in assets.keys())
				{
					ObjectMap<String, RefCountedContainer> assetsByType = assets.get(assetType);
					foreach (var entry in assetsByType)
					{
						Object @object = entry.value.@object;
						if (ReferenceEquals(@object, asset) || asset.Equals(@object)) return entry.key;
					}
				}

				return null;
			}
		}

		/** @param assetDesc the AssetDescriptor of the asset
		 * @return whether the asset is loaded */
		public bool isLoaded(IAssetDescriptor assetDesc)
		{
			lock (this)
				return isLoaded(assetDesc.FileName);
		}

		/** @param fileName the file name of the asset
		 * @return whether the asset is loaded */
		public bool isLoaded(String fileName)
		{
			lock (this)
			{
				if (fileName == null) return false;
				return assetTypes.containsKey(fileName);
			}
		}

		/** @param fileName the file name of the asset
		 * @return whether the asset is loaded */
		public bool isLoaded(String fileName, Type type)
		{
			lock (this)
			{
				ObjectMap<String, RefCountedContainer> assetsByType = assets.get(type);
				if (assetsByType == null) return false;
				return assetsByType.get(fileName) != null;
			}
		}

		/** Returns the default loader for the given type.
		 * @param type The type of the loader to get
		 * @return The loader capable of loading the type, or null if none exists */
		public IAssetLoader getLoader(Type type)
		{
			return getLoader(type, null);
		}

		/** Returns the loader for the given type and the specified filename. If no loader exists for the specific filename, the
		 * default loader for that type is returned.
		 * @param type The type of the loader to get
		 * @param fileName The filename of the asset to get a loader for, or null to get the default loader
		 * @return The loader capable of loading the type and filename, or null if none exists */
		public IAssetLoader getLoader(Type type, String fileName)
		{
			ObjectMap<String, IAssetLoader> loaders = this.loaders.get(type);
			if (loaders == null || loaders.size < 1) return null;
			if (fileName == null) return loaders.get("");
			IAssetLoader result = null;
			int length = -1;
			foreach (var entry in loaders.entries())
			{
				if (entry.key.Length > length && fileName.EndsWith(entry.key))
				{
					result = entry.value;
					length = entry.key.Length;
				}
			}

			return result;
		}

		/** Adds the given asset to the loading queue of the AssetManager.
		 * @param fileName the file name (interpretation depends on {@link AssetLoader})
		 * @param type the type of the asset. */
		public void load<T>(String fileName, Type type)
		{
			lock (this)
				load<T>(fileName, type, null);
		}

		/** Adds the given asset to the loading queue of the AssetManager.
		 * @param fileName the file name (interpretation depends on {@link AssetLoader})
		 * @param type the type of the asset.
		 * @param parameter parameters for the AssetLoader. */
		public void load<T>(String fileName, Type type, AssetLoaderParameters<T> parameter)
		{
			lock (this)
			{
				var loader = getLoader(type, fileName);
				if (loader == null)
					throw new GdxRuntimeException("No loader for type: " + ClassReflection.getSimpleName(type));

				// reset stats
				if (loadQueue.size == 0)
				{
					loaded = 0;
					toLoad = 0;
					peakTasks = 0;
				}

				// check if an asset with the same name but a different type has already been added.

				// check preload queue
				for (int i = 0; i < loadQueue.size; i++)
				{
					var desc = loadQueue.get(i);
					if (desc.FileName.Equals(fileName) && !desc.Type.Equals(type))
						throw new GdxRuntimeException(
							"Asset with name '" + fileName +
							"' already in preload queue, but has different type (expected: "
							+ ClassReflection.getSimpleName(type) + ", found: " +
							ClassReflection.getSimpleName(desc.Type) +
							")");
				}

				// check task list
				for (int i = 0; i < tasks.size; i++)
				{
					var desc = tasks.get(i).assetDesc;
					if (desc.FileName.Equals(fileName) && !desc.Type.Equals(type))
						throw new GdxRuntimeException(
							"Asset with name '" + fileName +
							"' already in task list, but has different type (expected: "
							+ ClassReflection.getSimpleName(type) + ", found: " +
							ClassReflection.getSimpleName(desc.Type) +
							")");
				}

				// check loaded assets
				Type otherType = assetTypes.get(fileName);
				if (otherType != null && !otherType.Equals(type))
					throw new GdxRuntimeException("Asset with name '" + fileName +
					                              "' already loaded, but has different type (expected: "
					                              + ClassReflection.getSimpleName(type) + ", found: " +
					                              ClassReflection.getSimpleName(otherType) + ")");

				toLoad++;
				var assetDesc = new AssetDescriptor<T>(fileName, type, parameter);
				loadQueue.add(assetDesc);
				log.debug("Queued: " + assetDesc);
			}
		}

		/** Adds the given asset to the loading queue of the AssetManager.
		 * @param desc the {@link AssetDescriptor} */
		public void load<T>(AssetDescriptor<T> desc)
		{
			lock (this)
				load<T>(desc.FileName, desc.Type, desc.Parameters);
		}

		/** Updates the AssetManager for a single task. Returns if the current task is still being processed or there are no tasks,
		 * otherwise it finishes the current task and starts the next task.
		 * @return true if all loading is finished. */
		public bool update()
		{
			lock (this)
			{
				try
				{
					if (tasks.size == 0)
					{
						// loop until we have a new task ready to be processed
						while (loadQueue.size != 0 && tasks.size == 0)
							nextTask();
						// have we not found a task? We are done!
						if (tasks.size == 0) return true;
					}

					return updateTask() && loadQueue.size == 0 && tasks.size == 0;
				}
				catch (Exception t)
				{
					handleTaskError(t);
					return loadQueue.size == 0;
				}
			}
		}

		/** Updates the AssetManager continuously for the specified number of milliseconds, yielding the CPU to the loading thread
		 * between updates. This may block for less time if all loading tasks are complete. This may block for more time if the portion
		 * of a single task that happens in the GL thread takes a long time. On GWT, updates for a single task instead (see
		 * {@link #update()}).
		 * @return true if all loading is finished. */
		public bool update(int millis)
		{
			if (Gdx.app.getType() == IApplication.ApplicationType.WebGL) return update();
			long endTime = TimeUtils.millis() + millis;
			while (true)
			{
				bool done = update();
				if (done || TimeUtils.millis() > endTime) return done;
				ThreadUtils.yield();
			}
		}

		/** Returns true when all assets are loaded. Can be called from any thread but note {@link #update()} or related methods must
		 * be called to process tasks. */
		public bool isFinished()
		{
			lock (this)
				return loadQueue.size == 0 && tasks.size == 0;
		}

		/** Blocks until all assets are loaded. */
		public void finishLoading()
		{
			log.debug("Waiting for loading to complete...");
			while (!update())
				ThreadUtils.yield();
			log.debug("Loading complete.");
		}

		/** Blocks until the specified asset is loaded.
		 * @param assetDesc the AssetDescriptor of the asset */
		public T finishLoadingAsset<T>(IAssetDescriptor assetDesc)
		{
			return finishLoadingAsset<T>(assetDesc.FileName);
		}

		/** Blocks until the specified asset is loaded.
		 * @param fileName the file name (interpretation depends on {@link AssetLoader}) */
		public T finishLoadingAsset<T>(String fileName)
		{
			log.debug("Waiting for asset to be loaded: " + fileName);
			while (true)
			{
				lock (this)
				{
					Type type = assetTypes.get(fileName);
					if (type != null)
					{
						ObjectMap<String, RefCountedContainer> assetsByType = assets.get(type);
						if (assetsByType != null)
						{
							RefCountedContainer assetContainer = assetsByType.get(fileName);
							if (assetContainer != null)
							{
								log.debug("Asset loaded: " + fileName);
								return (T)assetContainer.@object;
							}
						}
					}

					update();
				}

				ThreadUtils.yield();
			}
		}

		internal void injectDependencies(String parentAssetFilename, Array<IAssetDescriptor> dependendAssetDescs)
		{
			lock (this)
			{
				ObjectSet<String> injected = this.injected;
				foreach (var desc in dependendAssetDescs)
				{
					if (injected.contains(desc.FileName))
						continue; // Ignore subsequent dependencies if there are duplicates.
					injected.add(desc.FileName);
					injectDependency(parentAssetFilename, desc);
				}

				injected.clear(32);
			}
		}

		private void injectDependency(String parentAssetFilename, IAssetDescriptor dependendAssetDesc)
		{
			lock (this)
			{
				// add the asset as a dependency of the parent asset
				Array<String> dependencies = assetDependencies.get(parentAssetFilename);
				if (dependencies == null)
				{
					dependencies = new();
					assetDependencies.put(parentAssetFilename, dependencies);
				}

				dependencies.add(dependendAssetDesc.FileName);

				// if the asset is already loaded, increase its reference count.
				if (isLoaded(dependendAssetDesc.FileName))
				{
					log.debug("Dependency already loaded: " + dependendAssetDesc);
					Type type = assetTypes.get(dependendAssetDesc.FileName);
					RefCountedContainer assetRef = assets.get(type).get(dependendAssetDesc.FileName);
					assetRef.refCount++;
					incrementRefCountedDependencies(dependendAssetDesc.FileName);
				}
				else
				{
					// else add a new task for the asset.
					log.info("Loading dependency: " + dependendAssetDesc);
					addTask(dependendAssetDesc);
				}
			}
		}

		/** Removes a task from the loadQueue and adds it to the task stack. If the asset is already loaded (which can happen if it was
		 * a dependency of a previously loaded asset) its reference count will be increased. */
		private void nextTask()
		{
			IAssetDescriptor assetDesc = loadQueue.removeIndex(0);

			// if the asset not meant to be reloaded and is already loaded, increase its reference count
			if (isLoaded(assetDesc.FileName))
			{
				log.debug("Already loaded: " + assetDesc);
				Type type = assetTypes.get(assetDesc.FileName);
				RefCountedContainer assetRef = assets.get(type).get(assetDesc.FileName);
				assetRef.refCount++;
				incrementRefCountedDependencies(assetDesc.FileName);
				if (assetDesc.Parameters != null && assetDesc.Parameters.loadedCallback != null)
					assetDesc.Parameters.loadedCallback.Invoke(this, assetDesc.FileName, assetDesc.Type);
				loaded++;
			}
			else
			{
				// else add a new task for the asset.
				log.info("Loading: " + assetDesc);
				addTask(assetDesc);
			}
		}

		/** Adds a {@link AssetLoadingTask} to the task stack for the given asset. */
		private void addTask(IAssetDescriptor assetDesc)
		{
			IAssetLoader loader = getLoader(assetDesc.Type, assetDesc.FileName);
			if (loader == null)
				throw new GdxRuntimeException("No loader for type: " + ClassReflection.getSimpleName(assetDesc.Type));
			tasks.add(new AssetLoadingTask(this, assetDesc, loader, executor));
			peakTasks++;
		}

		/** Adds an asset to this AssetManager */
		protected void addAsset<T>(String fileName, Type type, T asset)
		{
			// add the asset to the filename lookup
			assetTypes.put(fileName, type);

			// add the asset to the type lookup
			ObjectMap<String, RefCountedContainer> typeToAssets = assets.get(type);
			if (typeToAssets == null)
			{
				typeToAssets = new ObjectMap<String, RefCountedContainer>();
				assets.put(type, typeToAssets);
			}

			RefCountedContainer assetRef = new RefCountedContainer();
			assetRef.@object = asset;
			typeToAssets.put(fileName, assetRef);
		}

		/** Updates the current task on the top of the task stack.
		 * @return true if the asset is loaded or the task was cancelled. */
		private bool updateTask()
		{
			AssetLoadingTask task = tasks.peek();

			bool complete = true;
			try
			{
				complete = task.cancel || task.update();
			}
			catch (RuntimeException ex)
			{
				task.cancel = true;
				taskFailed(task.assetDesc, ex);
			}

			// if the task has been cancelled or has finished loading
			if (complete)
			{
				// increase the number of loaded assets and pop the task from the stack
				if (tasks.size == 1)
				{
					loaded++;
					peakTasks = 0;
				}

				tasks.pop();

				if (task.cancel) return true;

				addAsset(task.assetDesc.FileName, task.assetDesc.Type, task.asset);

				// otherwise, if a listener was found in the parameter invoke it
				if (task.assetDesc.Parameters != null && task.assetDesc.Parameters.loadedCallback != null)
					task.assetDesc.Parameters.loadedCallback.Invoke(this, task.assetDesc.FileName, task.assetDesc.Type);

				long endTime = TimeUtils.nanoTime();
				log.debug("Loaded: " + (endTime - task.startTime) / 1000000f + "ms " + task.assetDesc);

				return true;
			}

			return false;
		}

		/** Called when a task throws an exception during loading. The default implementation rethrows the exception. A subclass may
		 * supress the default implementation when loading assets where loading failure is recoverable. */
		protected void taskFailed(IAssetDescriptor assetDesc, RuntimeException ex)
		{
			throw ex;
		}

		private void incrementRefCountedDependencies(String parent)
		{
			Array<String> dependencies = assetDependencies.get(parent);
			if (dependencies == null) return;

			foreach (String dependency in dependencies)
			{
				Type type = assetTypes.get(dependency);
				RefCountedContainer assetRef = assets.get(type).get(dependency);
				assetRef.refCount++;
				incrementRefCountedDependencies(dependency);
			}
		}

		/** Handles a runtime/loading error in {@link #update()} by optionally invoking the {@link AssetErrorListener}.
		 * @param t */
		private void handleTaskError(Exception t)
		{
			log.error("Error loading asset.", t);

			if (tasks.isEmpty()) throw new GdxRuntimeException(t);

			// pop the faulty task from the stack
			AssetLoadingTask task = tasks.pop();
			var assetDesc = task.assetDesc;

			// remove all dependencies
			if (task.dependenciesLoaded && task.dependencies != null)
			{
				foreach (var desc in task.dependencies)
					unload(desc.FileName);
			}

			// clear the rest of the stack
			tasks.clear();

			// inform the listener that something bad happened
			if (listener != null)
				listener.Error(assetDesc, t);
			else
				throw new GdxRuntimeException(t);
		}

		/** Sets a new {@link AssetLoader} for the given type.
		 * @param type the type of the asset
		 * @param loader the loader */
		public void setLoader<T, P>(Type type, AssetLoader<T, P> loader)
			where P : AssetLoaderParameters<T>
		{
			lock (this)
				setLoader(type, null, loader);
		}

		/** Sets a new {@link AssetLoader} for the given type.
		 * @param type the type of the asset
		 * @param suffix the suffix the filename must have for this loader to be used or null to specify the default loader.
		 * @param loader the loader */
		public void setLoader<T, P>(Type type, String suffix,
			AssetLoader<T, P> loader)
			where P : AssetLoaderParameters<T>
		{
			lock (this)
			{
				if (type == null) throw new IllegalArgumentException("type cannot be null.");
				if (loader == null) throw new IllegalArgumentException("loader cannot be null.");
				log.debug("Loader set: " + ClassReflection.getSimpleName(type) + " -> " +
				          ClassReflection.getSimpleName(loader.GetType()));
				var loaders = this.loaders.get(type);
				if (loaders == null) this.loaders.put(type, loaders = new());
				loaders.put(suffix == null ? "" : suffix, loader);
			}
		}

		/** @return the number of loaded assets */
		public int getLoadedAssets()
		{
			lock (this)
				return assetTypes.size;
		}

		/** @return the number of currently queued assets */
		public int getQueuedAssets()
		{
			lock (this)
				return loadQueue.size + tasks.size;
		}

		/** @return the progress in percent of completion. */
		public float getProgress()
		{
			lock (this)
			{
				if (toLoad == 0) return 1;
				float fractionalLoaded = loaded;
				if (peakTasks > 0)
				{
					fractionalLoaded += ((peakTasks - tasks.size) / (float)peakTasks);
				}

				return Math.Min(1, fractionalLoaded / toLoad);
			}
		}

		/** Sets an {@link AssetErrorListener} to be invoked in case loading an asset failed.
		 * @param listener the listener or null */
		public void setErrorListener(IAssetErrorListener listener)
		{
			lock (this)
				this.listener = listener;
		}

		/** Disposes all assets in the manager and stops all asynchronous loading. */
		public void dispose()
		{
			log.debug("Disposing.");
			clear();
			executor.dispose();
		}

		/** Clears and disposes all assets and the preloading queue. */
		public void clear()
		{
			lock (this)
			{
				loadQueue.clear();
			}

			// Lock is temporarily released to yield to blocked executor threads
			// A pending async task can cause a deadlock if we do not release

			finishLoading();

			lock (this)
			{
				ObjectIntMap<String> dependencyCount = new ObjectIntMap<String>();
				while (assetTypes.size > 0)
				{
					// for each asset, figure out how often it was referenced
					dependencyCount.clear(51);
					Array<String> assets = assetTypes.keys().toArray();
					foreach (String asset in assets)
					{
						Array<String> dependencies = assetDependencies.get(asset);
						if (dependencies == null) continue;
						foreach (String dependency in dependencies)
							dependencyCount.getAndIncrement(dependency, 0, 1);
					}

					// only dispose of assets that are root assets (not referenced)
					foreach (String asset in assets)
						if (dependencyCount.get(asset, 0) == 0)
							unload(asset);
				}

				this.assets.clear(51);
				this.assetTypes.clear(51);
				this.assetDependencies.clear(51);
				this.loaded = 0;
				this.toLoad = 0;
				this.peakTasks = 0;
				this.loadQueue.clear();
				this.tasks.clear();
			}
		}

		/** @return the {@link Logger} used by the {@link AssetManager} */
		public Logger getLogger()
		{
			return log;
		}

		public void setLogger(Logger logger)
		{
			log = logger;
		}

		/** Returns the reference count of an asset.
		 * @param fileName */
		public int getReferenceCount(String fileName)
		{
			lock (this)
			{
				Type type = assetTypes.get(fileName);
				if (type == null) throw new GdxRuntimeException("Asset not loaded: " + fileName);
				return assets.get(type).get(fileName).refCount;
			}
		}

		/** Sets the reference count of an asset.
		 * @param fileName */
		public void setReferenceCount(String fileName, int refCount)
		{
			lock (this)
			{
				Type type = assetTypes.get(fileName);
				if (type == null) throw new GdxRuntimeException("Asset not loaded: " + fileName);
				assets.get(type).get(fileName).refCount = refCount;
			}
		}

		/** @return a string containing ref count and dependency information for all assets. */
		public String getDiagnostics()
		{
			lock (this)
			{
				StringBuilder buffer = new StringBuilder(256);
				foreach (var entry in assetTypes)
				{
					String fileName = entry.key;
					Type type = entry.value;

					if (buffer.Length > 0) buffer.Append('\n');
					buffer.Append(fileName);
					buffer.Append(", ");
					buffer.Append(ClassReflection.getSimpleName(type));
					buffer.Append(", refs: ");
					buffer.Append(assets.get(type).get(fileName).refCount);

					Array<String> dependencies = assetDependencies.get(fileName);
					if (dependencies != null)
					{
						buffer.Append(", deps: [");
						foreach (String dep in dependencies)
						{
							buffer.Append(dep);
							buffer.Append(',');
						}

						buffer.Append(']');
					}
				}

				return buffer.ToString();
			}
		}

		/** @return the file names of all loaded assets. */
		public Array<String> getAssetNames()
		{
			lock (this)
				return assetTypes.keys().toArray();
		}

		/** @return the dependencies of an asset or null if the asset has no dependencies. */
		public Array<String> getDependencies(String fileName)
		{
			lock (this)
				return assetDependencies.get(fileName);
		}

		/** @return the type of a loaded asset. */
		public Type getAssetType(String fileName)
		{
			lock (this)
				return assetTypes.get(fileName);
		}

		class RefCountedContainer
		{
			internal Object @object;
			internal int refCount = 1;
		}
	}
}