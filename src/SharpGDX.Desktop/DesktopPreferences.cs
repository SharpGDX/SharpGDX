using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static SharpGDX.IFiles;

namespace SharpGDX.Desktop
{
	public class DesktopPreferences : IPreferences
	{
		private readonly Map<string, string> properties = new Map<string, string>();
		private readonly FileHandle file;

		public DesktopPreferences(String name, String directory)
			: this(new DesktopFileHandle(new SharpGDX.Shims.File(directory, name), FileType.External))
		{

		}

		public DesktopPreferences(FileHandle file)
		{
			this.file = file;
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

		public IPreferences putBoolean(String key, bool val)
		{
			properties.put(key, (val).ToString());
			return this;
		}

		public IPreferences putInteger(String key, int val)
		{
			properties.put(key, (val).ToString());
			return this;
		}

		public IPreferences putLong(String key, long val)
		{
			properties.put(key, (val).ToString());
			return this;
		}

		public IPreferences putFloat(String key, float val)
		{
			properties.put(key, (val).ToString());
			return this;
		}

		public IPreferences putString(String key, String val)
		{
			properties.put(key, val);
			return this;
		}

		public IPreferences put(Map<String, object> vals)
		{
			foreach (var val in vals.entrySet())
			{
				if (val.Value is Boolean) putBoolean(val.Key, (Boolean)val.Value);
				if (val.Value is int) putInteger(val.Key, (int)val.Value);
				if (val.Value is long) putLong(val.Key, (long)val.Value);
				if (val.Value is String) putString(val.Key, (String)val.Value);
				if (val.Value is float) putFloat(val.Key, (float)val.Value);
			}

			return this;
		}

		public bool getBoolean(String key)
		{
			return getBoolean(key, false);
		}

		public int getInteger(String key)
		{
			return getInteger(key, 0);
		}

		public long getLong(String key)
		{
			return getLong(key, 0);
		}

		public float getFloat(String key)
		{
			return getFloat(key, 0);
		}

		public String getString(String key)
		{
			return getString(key, "");
		}

		public bool getBoolean(String key, bool defValue)
		{
			return bool.Parse(properties.get(key, defValue.ToString()));
		}

		public int getInteger(String key, int defValue)
		{
			return int.Parse(properties.get(key, (defValue).ToString()));
		}

		public long getLong(String key, long defValue)
		{
			return long.Parse(properties.get(key, defValue.ToString()));
		}

		public float getFloat(String key, float defValue)
		{
			return float.Parse(properties.get(key, (defValue).ToString()));
		}

		public String getString(String key, String defValue)
		{
			return properties.get(key, defValue);
		}

		public Map<String, object> get()
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

		public bool contains(String key)
		{
			return properties.containsKey(key);
		}

		public void clear()
		{
			properties.clear();
		}

		public void flush()
		{
			Stream @out = null;
			try
			{
				@out = file.Write(false);
				using XmlTextWriter writer = new XmlTextWriter(@out, Encoding.UTF8);

				foreach (var item in properties.entrySet())
				{
					writer.WriteElementString(item.Key, item.Value);
				}
			}
			catch (Exception ex)
			{
				throw new GdxRuntimeException("Error writing preferences: " + file, ex);
			}
			finally
			{
				// TODO: StreamUtils.closeQuietly(@out);
				@out.Close();
			}
		}

		public void remove(String key)
		{
			properties.remove(key);
		}
	}
}