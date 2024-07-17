namespace SharpGDX.Shims;

/**
 * LocalStorage based File implementation for GWT. Should probably have used Harmony as a starting point instead of writing this
 * from scratch.
 * 
 * @author Stefan Haustein
 */
public class File
{
	public static readonly char pathSeparatorChar = ':';
	public static readonly string pathSeparator = "" + pathSeparatorChar;
	public static readonly File ROOT;
	public static readonly char separatorChar = '/';
	private static readonly string Separator = "" + separatorChar;

	static File()
	{
		ROOT = new("");
	}

	private static readonly string test = "fuck";
	
	private bool absolute;
	private readonly string name;

	// TODO: public static readonly Storage LocalStorage = Storage.getLocalStorageIfSupported();

	private readonly File parent;

	public File(string pathname)
	{
		while (pathname.EndsWith(Separator) && pathname.Length > 0)
		{
			pathname = pathname.Substring(0, pathname.Length - 1);
		}

		var cut = pathname.LastIndexOf(separatorChar);
		if (cut == -1)
		{
			name = pathname;
		}
		else if (cut == 0)
		{
			name = pathname.Substring(cut);
			parent = name.Equals("") ? null : ROOT;
		}
		else
		{
			name = pathname.Substring(cut + 1);
			parent = new File(pathname.Substring(0, cut));
		}

		// Compatibility.println("new File ('"+pathname+ "'); canonical name: '" + getCanonicalPath() + "'");
	}

	public File(string parent, string child)
		: this(new File(parent), child)
	{
	}

	public File(File parent, string child)
	{
		this.parent = parent;
		name = child;
	}

	public static File createTempFile(string prefix, string suffix, File directory) // TODO: throws IOException
	{
		throw new RuntimeException("NYI: createTempFile");
	}

	public static File createTempFile(string prefix, string suffix) // TODO: throws IOException
	{
		throw new RuntimeException("NYI: createTempFile");
	}

	public static File[] listRoots()
	{
		return new[] { ROOT };
	}

	/*
	 * public URL toURL() throws MalformedURLException { }
	 *
	 * public URI toURI() { }
	 */

	public bool canRead()
	{
		return true;
	}

	public bool canWrite()
	{
		return true;
	}

	public int compareTo(File pathname)
	{
		throw new RuntimeException("NYI: File.compareTo()");
	}

	public bool createNewFile() // TODO: throws IOException
	{
		if (exists())
		{
			return false;
		}

		if (!parent.exists())
		{
			return false;
		}

		// TODO: Should this be in a try/catch? -RP
		System.IO.File.Create(getCanonicalPath());
		return true;
	}

	public bool delete()
	{
		if (!exists())
		{
			return false;
		}

		// TODO: Should this be in a try/catch? -RP
		System.IO.File.Delete(getCanonicalPath());

		return true;
	}

	public void deleteOnExit()
	{
		throw new RuntimeException("NYI: File.deleteOnExit()");
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is File))
		{
			return false;
		}

		return getPath().Equals(((File)obj).getPath());
	}

	public bool exists()
	{
		//return LocalStorage.getItem(getCanonicalPath()) != null;
		return System.IO.File.Exists(getCanonicalPath());
	}

	public File getAbsoluteFile()
	{
		if (isAbsolute())
		{
			return this;
		}

		if (parent == null)
		{
			return new File(ROOT, name);
		}

		return new File(parent.getAbsoluteFile(), name);
	}

	public string getAbsolutePath()
	{
		var path = getAbsoluteFile().getPath();
		return path.Length == 0 ? "" : path;
	}

	public File getCanonicalFile()
	{
		var cParent = parent == null ? null : parent.getCanonicalFile();
		if (name.Equals("."))
		{
			return cParent == null ? ROOT : cParent;
		}

		if (cParent != null && cParent.name.Equals(""))
		{
			cParent = null;
		}

		if (name.Equals(".."))
		{
			if (cParent == null)
			{
				return ROOT;
			}

			if (cParent.parent == null)
			{
				return ROOT;
			}

			return cParent.parent;
		}

		if (cParent == null && !name.Equals(""))
		{
			return new File(ROOT, name);
		}

		return new File(cParent, name);
	}

	public string getCanonicalPath()
	{
		return getCanonicalFile().getAbsolutePath();
	}

	public override int GetHashCode()
	{
		return parent != null ? parent.GetHashCode() + name.GetHashCode() : name.GetHashCode();
	}

	/*
	 * public File(URI uri) { }
	 */

	public string getName()
	{
		return name;
	}

	public string getParent()
	{
		return parent == null ? "" : parent.getPath();
	}

	public File? getParentFile()
	{
		return parent;
	}

	public string getPath()
	{
		return parent == null ? name : parent.getPath() + (parent.getPath().Length > 0 ? separatorChar : "") + name;
	}

	public bool isAbsolute()
	{
		if (isRoot())
		{
			return true;
		}

		if (parent == null)
		{
			return false;
		}

		return parent.isAbsolute();
	}

	public bool isDirectory()
	{
		var s = getCanonicalPath();

		if (string.IsNullOrWhiteSpace(s))
		{
			return false;
		}

		return !System.IO.File.Exists(s) && Directory.Exists(s);
	}

	public bool isFile()
	{
		var s = getCanonicalPath();

		if (string.IsNullOrWhiteSpace(s))
		{
			return false;
		}

		return !Directory.Exists(s) && System.IO.File.Exists(s);
	}

	public bool isHidden()
	{
		return false;
	}

	public long lastModified()
	{
		return 0;
	}

	public long length()
	{
		try
		{
			if (!exists())
			{
				return 0;
			}

			var raf = new FileInfo(getCanonicalPath());

			var len = raf.Length;
			return len;
		}
		catch (IOException e)
		{
			return 0;
		}
	}

	public string[] list()
	{
		throw new RuntimeException("NYI: File.list()");
	}

	/*
	 * public String[] list(FilenameFilter filter) { return null; }
	 */

	public File[] listFiles()
	{
		return listFiles(null);
	}

	public File[] listFiles(FilenameFilter filter)
	{
		// TODO: Should this just list the files in the root directory? -RP
		// TODO: What about subdirectories? -RP
		throw new NotImplementedException();
		//List<File> files = new List<File>();
		//String prefix = getCanonicalPath();
		//if (!prefix.EndsWith(separator))
		//{
		//	prefix += separatorChar;
		//}
		//int cut = prefix.Length;
		//int cnt = LocalStorage.getLength();
		//for (int i = 0; i < cnt; i++)
		//{
		//	String key = LocalStorage.key(i);
		//	if (key.StartsWith(prefix) && key.IndexOf(separatorChar, cut) == -1)
		//	{
		//		String name = key.Substring(cut);
		//		if (filter == null || filter.accept(this, name))
		//		{
		//			files.add(new File(this, name));
		//		}
		//	}
		//}
		//return files.toArray(new File[files.size()]);
	}

	/*
	 * public File[] listFiles(FileFilter filter) { return null; }
	 */

	public bool mkdir()
	{
		if (parent != null && !parent.exists())
		{
			return false;
		}

		if (exists())
		{
			return false;
		}

		Directory.CreateDirectory(getCanonicalPath());

		return true;
	}

	public bool mkdirs()
	{
		if (parent != null)
		{
			parent.mkdirs();
		}

		return mkdir();
	}

	public bool renameTo(File dest)
	{
		throw new RuntimeException("renameTo()");
	}

	public bool setLastModified(long time)
	{
		return false;
	}

	public bool setReadOnly()
	{
		return false;
	}

	public override String ToString()
	{
		return name;
	}

	private bool isRoot()
	{
		return name.Equals("") && parent == null;
	}
}