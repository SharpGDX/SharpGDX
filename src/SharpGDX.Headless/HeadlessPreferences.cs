using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SharpGDX.IFiles;

namespace SharpGDX.Headless
{
	public class HeadlessPreferences : IPreferences
	{
		private readonly Map<string, string> properties = new Map<string, string>();
		private readonly FileHandle file;

		public HeadlessPreferences(String name, String directory)
			// TODO: : this(new HeadlessFileHandle(new File(directory, name), FileType.External))
		{

		}

		public HeadlessPreferences(FileHandle file)
		{
			//this.file = file;
			//if (!file.exists()) return;
			//InputStream in = null;
			//try
			//{
			//	in = new BufferedInputStream(file.read());
			//	properties.loadFromXML(in);
			//}
			//catch (Throwable t)
			//{
			//	t.printStackTrace();
			//}
			//finally
			//{
			//	StreamUtils.closeQuietly(in);
			//}
		}

		public IPreferences PutBoolean(String key, bool val)
		{
			properties.put(key, (val).ToString());
			return this;
		}

		public IPreferences PutInteger(String key, int val)
		{
			properties.put(key, (val).ToString());
			return this;
		}

		public IPreferences PutLong(String key, long val)
		{
			properties.put(key, (val).ToString());
			return this;
		}

		public IPreferences PutFloat(String key, float val)
		{
			properties.put(key, (val).ToString());
			return this;
		}

		public IPreferences PutString(String key, String val)
		{
			properties.put(key, val);
			return this;
		}

		public IPreferences Put(Map<String, object> vals)
		{
			foreach (var val in vals.entrySet())
			{
				if (val.Value is Boolean) PutBoolean(val.Key, (Boolean)val.Value);
				if (val.Value is int) PutInteger(val.Key, (int)val.Value);
				if (val.Value is long) PutLong(val.Key, (long)val.Value);
				if (val.Value is String) PutString(val.Key, (String)val.Value);
				if (val.Value is float) PutFloat(val.Key, (float)val.Value);
			}

			return this;
		}

		public bool GetBoolean(String key)
		{
			return GetBoolean(key, false);
		}

		public int GetInteger(String key)
		{
			return GetInteger(key, 0);
		}

		public long GetLong(String key)
		{
			return GetLong(key, 0);
		}

		public float GetFloat(String key)
		{
			return GetFloat(key, 0);
		}

		public String GetString(String key)
		{
			return GetString(key, "");
		}

		public bool GetBoolean(String key, bool defValue)
		{
			return bool.Parse(properties.get(key, defValue.ToString()));
		}

		public int GetInteger(String key, int defValue)
		{
			return int.Parse(properties.get(key, (defValue).ToString()));
		}

		public long GetLong(String key, long defValue)
		{
			return long.Parse(properties.get(key, defValue.ToString()));
		}

		public float GetFloat(String key, float defValue)
		{
			return float.Parse(properties.get(key, (defValue).ToString()));
		}

		public String GetString(String key, String defValue)
		{
			return properties.get(key, defValue);
		}

		public Map<String, object> Get()
		{
			Map<String, Object> map = new();
			foreach (var val in properties.entrySet())
			{
				if (val.Value is Boolean)
					map.put((String)val.Key, bool.Parse((String)val.Value));
				if (val.Value is int) map.put((String)val.Key, int.Parse((String)val.Value));
				if (val.Value is long) map.put((String)val.Key, long.Parse((String)val.Value));
				if (val.Value is String) map.put((String)val.Key, (String)val.Value);
				if (val.Value is float) map.put((String)val.Key, float.Parse((String)val.Value));
			}

			return map;
		}

		public bool Contains(String key)
		{
			return properties.containsKey(key);
		}

		public void Clear()
		{
			properties.clear();
		}

		public void Flush()
		{
			//OutputStream out = null;
			//try
			//{
			//		out = new BufferedOutputStream(file.write(false));
			//	properties.storeToXML(out, null);
			//}
			//catch (Exception ex)
			//{
			//	throw new GdxRuntimeException("Error writing preferences: " + file, ex);
			//}
			//finally
			//{
			//	StreamUtils.closeQuietly(out);
			//}
		}

		public void Remove(String key)
		{
			properties.remove(key);
		}
	}
}