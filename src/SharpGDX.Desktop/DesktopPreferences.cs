using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using System.Text;
using System.Xml;

namespace SharpGDX.Desktop
{
	public class DesktopPreferences : IPreferences
	{
		private readonly Map<string, string> _properties = new Map<string, string>();
		private readonly FileHandle _file;

		public DesktopPreferences(String name, String directory)
			: this(new DesktopFileHandle(new SharpGDX.Shims.File(directory, name), FileType.External))
		{

		}

		public DesktopPreferences(FileHandle file)
		{
			this._file = file;
			if (!file.exists()) return;
			FileStream @in = null;
			try
			{
				@in = file.Read();
				// TODO: properties.loadFromXML(in);
			}
			catch (Exception t)
			{
				// TODO: t.printStackTrace();
				Console.WriteLine(t.StackTrace);
			}
			finally
			{
				@in.Close();
				// TODO: StreamUtils.closeQuietly(@in);
			}
		}

		public IPreferences PutBoolean(String key, bool val)
		{
			_properties.put(key, (val).ToString());
			return this;
		}

		public IPreferences PutInteger(String key, int val)
		{
			_properties.put(key, (val).ToString());
			return this;
		}

		public IPreferences PutLong(String key, long val)
		{
			_properties.put(key, (val).ToString());
			return this;
		}

		public IPreferences PutFloat(String key, float val)
		{
			_properties.put(key, (val).ToString());
			return this;
		}

		public IPreferences PutString(String key, String val)
		{
			_properties.put(key, val);
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
			return bool.Parse(_properties.get(key, defValue.ToString()));
		}

		public int GetInteger(String key, int defValue)
		{
			return int.Parse(_properties.get(key, (defValue).ToString()));
		}

		public long GetLong(String key, long defValue)
		{
			return long.Parse(_properties.get(key, defValue.ToString()));
		}

		public float GetFloat(String key, float defValue)
		{
			return float.Parse(_properties.get(key, (defValue).ToString()));
		}

		public String GetString(String key, String defValue)
		{
			return _properties.get(key, defValue);
		}

		public Map<String, object> Get()
		{
			Map<String, Object> map = new();
			foreach (var val in _properties.entrySet())
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
			return _properties.containsKey(key);
		}

		public void Clear()
		{
			_properties.clear();
		}

		public void Flush()
		{
			Stream @out = null;
			try
			{
				@out = _file.Write(false);
				using XmlTextWriter writer = new XmlTextWriter(@out, Encoding.UTF8);

				foreach (var item in _properties.entrySet())
				{
					writer.WriteElementString(item.Key, item.Value);
				}
			}
			catch (Exception ex)
			{
				throw new GdxRuntimeException("Error writing preferences: " + _file, ex);
			}
			finally
			{
				// TODO: StreamUtils.closeQuietly(@out);
				@out.Close();
			}
		}

		public void Remove(String key)
		{
			_properties.remove(key);
		}
	}
}