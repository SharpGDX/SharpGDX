using SharpGDX.Utils;
using SharpGDX.Utils.Reflect;
using SharpGDX.Assets;
using SharpGDX.Shims;


namespace SharpGDX.Graphics.G3D.Particles;

/** This class handles the assets and configurations required by a given resource when de/serialized. It's handy when a given
 * object or one of its members requires some assets to be loaded to work properly after being deserialized. To save the assets,
 * the object should implement the {@link Configurable} interface and obtain a {@link SaveData} object to store every required
 * asset or information which will be used during the loading phase. The passed in {@link AssetManager} is generally used to find
 * the asset file name for a given resource of a given type. The class can also store global configurations, this is useful when
 * dealing with objects which should be allocated once (i.e singleton). The deserialization process must happen in the same order
 * of serialization, because the per object {@link SaveData} blocks are stored as an {@link Array} within the {@link ResourceData}
 * , while the global {@link SaveData} instances can be accessed in any order because require a unique {@link String} and are
 * stored in an {@link ObjectMap}.
 * @author Inferno */
public class ResourceData<T> : Json.Serializable {

	/** This interface must be implemented by any class requiring additional assets to be loaded/saved */
	public interface Configurable {
		public void save (AssetManager manager, ResourceData<T> resources);

		public void load (AssetManager manager, ResourceData<T> resources);
	}

	/** Contains all the saved data. {@link #data} is a map which link an asset name to its instance. {@link #assets} is an array
	 * of indices addressing a given {@link com.badlogic.gdx.graphics.g3d.particles.ResourceData.AssetData} in the
	 * {@link ResourceData} */
	public class SaveData : Json.Serializable {
		ObjectMap<String, Object> data;
		IntArray assets;
		private int loadIndex;
		internal protected ResourceData<T> resources;

		public SaveData () {
			data = new ObjectMap<String, Object>();
			assets = new IntArray();
			loadIndex = 0;
		}

		public SaveData (ResourceData<T> resources) {
			data = new ObjectMap<String, Object>();
			assets = new IntArray();
			loadIndex = 0;
			this.resources = resources;
		}

		public void saveAsset<K> (String filename, Type type) {
			int i = resources.getAssetData<K>(filename, type);
			if (i == -1) {
				resources.sharedAssets.Add(new AssetData(filename, type));
				i = resources.sharedAssets.size - 1;
			}
			assets.add(i);
		}

		public void save (String key, Object value) {
			data.put(key, value);
		}

		public AssetDescriptor loadAsset () {
			if (loadIndex == assets.size) return null;
			AssetData data = (AssetData)resources.sharedAssets.Get(assets.get(loadIndex++));
			return new AssetDescriptor(data.filename, data.type);
		}

		public  K load<K>(String key) {
			return (K)data.get(key);
		}

		public void write (Json json) {
			json.writeValue("data", data, typeof(ObjectMap<string, object>));
			json.writeValue("indices", assets.toArray(), typeof(int[]));
		}

		public void read (Json json, JsonValue jsonData) {
			data = (ObjectMap<string, object>)json.readValue("data", typeof(ObjectMap<string, object>), jsonData);
			assets.addAll((int[])json.readValue("indices", typeof(int[]), jsonData));
		}
	}

	/** This class contains all the information related to a given asset */
	public class AssetData : Json.Serializable {
		public String filename;
		public Type type;

		public AssetData () {
		}

		public AssetData (String filename, Type type) {
			this.filename = filename;
			this.type = type;
		}

		public void write (Json json) {
			json.writeValue("filename", filename);
			json.writeValue("type", type.Name);
		}

		public void read (Json json, JsonValue jsonData) {
			filename = (string)json.readValue("filename", typeof(string), jsonData);
			string className = (string)json.readValue("type", typeof(string), jsonData);
			try {
				type = (Type)ClassReflection.forName(className);
			} catch (ReflectionException e) {
				throw new GdxRuntimeException("Class not found: " + className, e);
			}
		}
	}

	/** Unique data, can be used to save/load generic data which is not always loaded back after saving. Must be used to store data
	 * which is uniquely addressable by a given string (i.e a system configuration). */
	private ObjectMap<String, SaveData> uniqueData;

	/** Objects save data, must be loaded in the same saving order */
	private Array<SaveData> data;

	/** Shared assets among all the configurable objects */
	Array<AssetData> sharedAssets;
	private int currentLoadIndex;
	public T resource;

	public ResourceData () {
		uniqueData = new ObjectMap<String, SaveData>();
		data = new Array<SaveData>(true, 3, typeof(SaveData));
		sharedAssets = new Array<AssetData>();
		currentLoadIndex = 0;
	}

	public ResourceData (T resource)
    :this(){
		
		this.resource = resource;
	}

	 int getAssetData<K>(String filename, Type type) {
		int i = 0;
		foreach(AssetData data in sharedAssets) {
			if (data.filename.Equals(filename) && data.type.Equals(type)) {
				return i;
			}
			++i;
		}
		return -1;
	}

	public Array<AssetDescriptor> getAssetDescriptors () {
		Array<AssetDescriptor> descriptors = new Array<AssetDescriptor>();
		foreach (AssetData data in sharedAssets) {
			descriptors.Add(new AssetDescriptor(data.filename, data.type));
		}
		return descriptors;
	}

	public Array<AssetData> getAssets () {
		return sharedAssets;
	}

	/** Creates and adds a new SaveData object to the save data list */
	public SaveData createSaveData () {
		SaveData saveData = new SaveData(this);
		data.Add(saveData);
		return saveData;
	}

	/** Creates and adds a new and unique SaveData object to the save data map */
	public SaveData createSaveData (String key) {
		SaveData saveData = new SaveData(this);
		if (uniqueData.containsKey(key)) throw new RuntimeException("Key already used, data must be unique, use a different key");
		uniqueData.put(key, saveData);
		return saveData;
	}

	/** @return the next save data in the list */
	public SaveData getSaveData () {
		return data.Get(currentLoadIndex++);
	}

	/** @return the unique save data in the map */
	public SaveData getSaveData (String key) {
		return uniqueData.get(key);
	}

	public void write (Json json) {
		json.writeValue("unique", uniqueData, typeof(ObjectMap<string, object>));
		json.writeValue("data", data, typeof(Array), typeof(SaveData));
		json.writeValue("assets", sharedAssets.toArray<AssetData>(typeof(AssetData)), typeof(AssetData[]));
		json.writeValue("resource", resource, null);
	}

	public void read (Json json, JsonValue jsonData) {
		uniqueData = (ObjectMap<string, SaveData>)json.readValue("unique", typeof(ObjectMap<string, SaveData>), jsonData);
		foreach (var entry in uniqueData.entries()) {
			entry.value.resources = this;
		}

		data = (Array<SaveData>)json.readValue("data", typeof(Array), typeof(SaveData), jsonData);
		foreach (SaveData saveData in data) {
			saveData.resources = this;
		}

		sharedAssets.addAll((Array<AssetData>)json.readValue("assets", typeof(Array), typeof(AssetData), jsonData));
		resource = (T)json.readValue("resource", null, jsonData);
	}

}
