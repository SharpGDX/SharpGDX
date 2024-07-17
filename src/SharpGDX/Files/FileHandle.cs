using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SharpGDX;
using SharpGDX.Shims;
using SharpGDX.Utils;
using static SharpGDX.IFiles;
using File = SharpGDX.Shims.File;

namespace SharpGDX.Files
{
	/** Represents a file or directory on the filesystem, classpath, Android app storage, or Android assets directory. FileHandles are
 * created via a {@link Files} instance.
 * 
 * Because some of the file types are backed by composite files and may be compressed (for example, if they are in an Android .apk
 * or are found via the classpath), the methods for extracting a {@link #path()} or {@link #file()} may not be appropriate for all
 * types. Use the Reader or Stream methods here to hide these dependencies from your platform independent code.
 * 
 * @author mzechner
 * @author Nathan Sweet */
	public class FileHandle
	{
		protected File _file;
		protected FileType _type;

		protected FileHandle()
		{
		}

		/** Creates a new absolute FileHandle for the file name. Use this for tools on the desktop that don't need any of the backends.
		 * Do not use this constructor in case you write something cross-platform. Use the {@link Files} interface instead.
		 * @param fileName the filename. */
		public FileHandle(String fileName)
		{
			this._file = new File(fileName);
			this._type = FileType.Absolute;
		}

		/** Creates a new absolute FileHandle for the {@link File}. Use this for tools on the desktop that don't need any of the
		 * backends. Do not use this constructor in case you write something cross-platform. Use the {@link Files} interface instead.
		 * @param file the file. */
		public FileHandle(File file)
		{
			this._file = file;
			this._type = FileType.Absolute;
		}

		protected FileHandle(String fileName, FileType type)
		{
			this._type = type;
			_file = new File(fileName);
		}

		protected FileHandle(File file, FileType type)
		{
			this._file = file;
			this._type = type;
		}

		/** @return the path of the file as specified on construction, e.g. Gdx.files.internal("dir/file.png") -> dir/file.png.
		 *         backward slashes will be replaced by forward slashes. */
		public String path()
		{
			return _file.getPath().Replace('\\', '/');
		}

		/** @return the name of the file, without any parent paths. */
		public String name()
		{
			return _file.getName();
		}

		/** Returns the file extension (without the dot) or an empty string if the file name doesn't contain a dot. */
		public String extension()
		{
			String name = _file.getName();
			int dotIndex = name.LastIndexOf('.');
			if (dotIndex == -1) return "";
			return name.Substring(dotIndex + 1);
		}

		/** @return the name of the file, without parent paths or the extension. */
		public String nameWithoutExtension()
		{
			String name = _file.getName();
			int dotIndex = name.LastIndexOf('.');
			if (dotIndex == -1) return name;
			return name.Substring(0, dotIndex);
		}

		/** @return the path and filename without the extension, e.g. dir/dir2/file.png -> dir/dir2/file. backward slashes will be
		 *         returned as forward slashes. */
		public String pathWithoutExtension()
		{
			String path = _file.getPath().Replace('\\', '/');
			int dotIndex = path.LastIndexOf('.');
			if (dotIndex == -1) return path;
			return path.Substring(0, dotIndex);
		}

		public FileType type()
		{
			return _type;
		}

		/** Returns a java.io.File that represents this file handle. Note the returned file will only be usable for
		 * {@link FileType#Absolute} and {@link FileType#External} file handles. */
		public virtual File file()
		{
			if (_type == FileType.External) return new File(Gdx.files.getExternalStoragePath(), _file.getPath());
			return _file;
		}

		private static InputStream getResourceAsStream(string resource)
		{
			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
			Stream resFilestream = a.GetManifestResourceStream(resource);
			
				return new InputStream(resFilestream);
		}

		/** Returns a stream for reading this file as bytes.
		 * @throws GdxRuntimeException if the file handle represents a directory, doesn't exist, or could not be read. */
		public InputStream read()
		{
			if (_type == FileType.Classpath || (_type == FileType.Internal && !file().exists())
			                                || (_type == FileType.Local && !file().exists()))
			{
				InputStream input = FileHandle.getResourceAsStream(_file.getPath().Replace('\\', '/'));
				if (input == null) throw new GdxRuntimeException("File not found: " + _file + " (" + _type + ")");
				return input;
			}

			try
			{
				return new FileInputStream(file());
			}
			catch (Exception ex)
			{
				if (file().isDirectory())
					throw new GdxRuntimeException("Cannot open a stream to a directory: " + _file + " (" + _type + ")",
						ex);
				throw new GdxRuntimeException("Error reading file: " + _file + " (" + _type + ")", ex);
			}
		}

		/** Returns a stream for reading this file as bytes.
		 * @throws GdxRuntimeException if the file handle represents a directory, doesn't exist, or could not be read. */
		public FileStream Read()
		{
			if (_type == FileType.Classpath || (_type == FileType.Internal && !file().exists())
			                                || (_type == FileType.Local && !file().exists()))
			{
				throw new NotImplementedException();
				//InputStream input = FileHandle.getResourceAsStream("/" + _file.getPath().Replace('\\', '/'));
				//if (input == null) throw new GdxRuntimeException("File not found: " + _file + " (" + _type + ")");
				//return input;
			}

			try
			{
				return System.IO.File.OpenRead(file().getCanonicalPath());
			}
			catch (Exception ex)
			{
				if (file().isDirectory())
					throw new GdxRuntimeException("Cannot open a stream to a directory: " + _file + " (" + _type + ")",
						ex);
				throw new GdxRuntimeException("Error reading file: " + _file + " (" + _type + ")", ex);
			}
		}

		/** Returns a buffered stream for reading this file as bytes.
		 * @throws GdxRuntimeException if the file handle represents a directory, doesn't exist, or could not be read. */
		public BufferedInputStream read(int bufferSize)
{
	return new BufferedInputStream(read(), bufferSize);
}

/** Returns a reader for reading this file as characters the platform's default charset.
 * @throws GdxRuntimeException if the file handle represents a directory, doesn't exist, or could not be read. */
public Reader reader()
{
	return new InputStreamReader(read());
}

/** Returns a reader for reading this file as characters.
 * @throws GdxRuntimeException if the file handle represents a directory, doesn't exist, or could not be read. */
public Reader reader(String charset)
{
	InputStream stream = read();
	try
	{
		return new InputStreamReader(stream, charset);
	}
	catch (UnsupportedEncodingException ex)
	{
		StreamUtils.closeQuietly(stream);
		throw new GdxRuntimeException("Error reading file: " + this, ex);
	}
}

/** Returns a buffered reader for reading this file as characters using the platform's default charset.
 * @throws GdxRuntimeException if the file handle represents a directory, doesn't exist, or could not be read. */
public BufferedReader reader(int bufferSize)
{
	return new BufferedReader(new InputStreamReader(read()), bufferSize);
}

/** Returns a buffered reader for reading this file as characters.
 * @throws GdxRuntimeException if the file handle represents a directory, doesn't exist, or could not be read. */
public BufferedReader reader(int bufferSize, String charset)
{
	try
	{
		return new BufferedReader(new InputStreamReader(read(), charset), bufferSize);
	}
	catch (UnsupportedEncodingException ex)
	{
		throw new GdxRuntimeException("Error reading file: " + this, ex);
	}
}

/** Reads the entire file into a string using the platform's default charset.
 * @throws GdxRuntimeException if the file handle represents a directory, doesn't exist, or could not be read. */
public String readString()
{
	return readString(null);
}

/** Reads the entire file into a string using the specified charset.
 * @param charset If null the default charset is used.
 * @throws GdxRuntimeException if the file handle represents a directory, doesn't exist, or could not be read. */
public String readString(String charset)
{
	StringBuilder output = new StringBuilder(estimateLength());
	InputStreamReader reader = null;
	try
	{
		if (charset == null)
			reader = new InputStreamReader(read());
		else
			reader = new InputStreamReader(read(), charset);
		char[] buffer = new char[256];
		while (true)
		{
			int length = reader.read(buffer);
			if (length == -1) break;
			output.Append(buffer, 0, length);
		}
	}
	catch (IOException ex)
	{
		throw new GdxRuntimeException("Error reading layout file: " + this, ex);
	}
	finally
	{
		StreamUtils.closeQuietly(reader);
	}
	return output.ToString();
}

/** Reads the entire file into a byte array.
 * @throws GdxRuntimeException if the file handle represents a directory, doesn't exist, or could not be read. */
public byte[] readBytes()
{
	InputStream input = read();
	try
	{
		return StreamUtils.copyStreamToByteArray(input, estimateLength());
	}
	catch (IOException ex)
	{
		throw new GdxRuntimeException("Error reading file: " + this, ex);
	}
	finally
	{
		StreamUtils.closeQuietly(input);
	}
}

private int estimateLength()
{
	int length = (int)this.length();
	return length != 0 ? length : 512;
}

/** Reads the entire file into the byte array. The byte array must be big enough to hold the file's data.
 * @param bytes the array to load the file into
 * @param offset the offset to start writing bytes
 * @param size the number of bytes to read, see {@link #length()}
 * @return the number of read bytes */
public int readBytes(byte[] bytes, int offset, int size)
{
	InputStream input = read();
	int position = 0;
	try
	{
		while (true)
		{
			int count = input.read(bytes, offset + position, size - position);
			if (count <= 0) break;
			position += count;
		}
	}
	catch (IOException ex)
	{
		throw new GdxRuntimeException("Error reading file: " + this, ex);
	}
	finally
	{
		StreamUtils.closeQuietly(input);
	}
	return position - offset;
}

/** Attempts to memory map this file in READ_ONLY mode. Android files must not be compressed.
 * @throws GdxRuntimeException if this file handle represents a directory, doesn't exist, or could not be read, or memory
 *            mapping fails, or is a {@link FileType#Classpath} file. */
public ByteBuffer map()
{
	return map(FileChannel.MapMode.READ_ONLY);
}

/** Attempts to memory map this file. Android files must not be compressed.
 * @throws GdxRuntimeException if this file handle represents a directory, doesn't exist, or could not be read, or memory
 *            mapping fails, or is a {@link FileType#Classpath} file. */
public ByteBuffer map(FileChannel.MapMode mode)
{
	if (_type == FileType.Classpath) throw new GdxRuntimeException("Cannot map a classpath file: " + this);
	RandomAccessFile raf = null;
	try
	{
		File f = file();
		raf = new RandomAccessFile(f, mode == FileChannel.MapMode.READ_ONLY ? "r" : "rw");
		FileChannel fileChannel = raf.getChannel();
		ByteBuffer map = FileChannelUtils.map(fileChannel, mode, 0, f.length());
		map.order(ByteOrder.nativeOrder());
		return map;
	}
	catch (Exception ex)
	{
		throw new GdxRuntimeException("Error memory mapping file: " + this + " (" + type + ")", ex);
	}
	finally
	{
		StreamUtils.closeQuietly(raf);
	}
}

/** Returns a stream for writing to this file. Parent directories will be created if necessary.
 * @param append If false, this file will be overwritten if it exists, otherwise it will be appended.
 * @throws GdxRuntimeException if this file handle represents a directory, if it is a {@link FileType#Classpath} or
 *            {@link FileType#Internal} file, or if it could not be written. */
public OutputStream write(bool append)
{
	if (_type == FileType.Classpath) throw new GdxRuntimeException("Cannot write to a classpath file: " + file);
	if (_type == FileType.Internal) throw new GdxRuntimeException("Cannot write to an internal file: " + file);
	parent().mkdirs();
	try
	{
		return new FileOutputStream(file(), append);
	}
	catch (Exception ex)
	{
		if (file().isDirectory())
			throw new GdxRuntimeException("Cannot open a stream to a directory: " + file + " (" + type + ")", ex);
		throw new GdxRuntimeException("Error writing file: " + file + " (" + type + ")", ex);
	}
}

/** Returns a stream for writing to this file. Parent directories will be created if necessary.
 * @param append If false, this file will be overwritten if it exists, otherwise it will be appended.
 * @throws GdxRuntimeException if this file handle represents a directory, if it is a {@link FileType#Classpath} or
 *            {@link FileType#Internal} file, or if it could not be written. */
		public FileStream Write(bool append)
{
	if (_type == FileType.Classpath) throw new GdxRuntimeException("Cannot write to a classpath file: " + file);
	if (_type == FileType.Internal) throw new GdxRuntimeException("Cannot write to an internal file: " + file);
	parent().mkdirs();
	try
	{
		// TODO: Verify that this is the expected behavior.
		return new FileStream(file().getCanonicalPath(),append ? FileMode.Append : FileMode.Create);
	}
	catch (Exception ex)
	{
		if (file().isDirectory())
			throw new GdxRuntimeException("Cannot open a stream to a directory: " + file + " (" + type + ")", ex);
		throw new GdxRuntimeException("Error writing file: " + file + " (" + type + ")", ex);
	}
}

		/** Returns a buffered stream for writing to this file. Parent directories will be created if necessary.
		 * @param append If false, this file will be overwritten if it exists, otherwise it will be appended.
		 * @param bufferSize The size of the buffer.
		 * @throws GdxRuntimeException if this file handle represents a directory, if it is a {@link FileType#Classpath} or
		 *            {@link FileType#Internal} file, or if it could not be written. */
		public OutputStream write(bool append, int bufferSize)
{
	return new BufferedOutputStream(write(append), bufferSize);
}

/** Reads the remaining bytes from the specified stream and writes them to this file. The stream is closed. Parent directories
 * will be created if necessary.
 * @param append If false, this file will be overwritten if it exists, otherwise it will be appended.
 * @throws GdxRuntimeException if this file handle represents a directory, if it is a {@link FileType#Classpath} or
 *            {@link FileType#Internal} file, or if it could not be written. */
public void write(InputStream input, bool append)
{
	OutputStream output = null;
	try
	{
		output = write(append);
		StreamUtils.copyStream(input, output);
	}
	catch (Exception ex)
	{
		throw new GdxRuntimeException("Error stream writing to file: " + file + " (" + type + ")", ex);
	}
	finally
	{
		StreamUtils.closeQuietly(input);
		StreamUtils.closeQuietly(output);
	}

}

/** Returns a writer for writing to this file using the default charset. Parent directories will be created if necessary.
 * @param append If false, this file will be overwritten if it exists, otherwise it will be appended.
 * @throws GdxRuntimeException if this file handle represents a directory, if it is a {@link FileType#Classpath} or
 *            {@link FileType#Internal} file, or if it could not be written. */
public Writer writer(bool append)
{
	return writer(append, null);
}

/** Returns a writer for writing to this file. Parent directories will be created if necessary.
 * @param append If false, this file will be overwritten if it exists, otherwise it will be appended.
 * @param charset May be null to use the default charset.
 * @throws GdxRuntimeException if this file handle represents a directory, if it is a {@link FileType#Classpath} or
 *            {@link FileType#Internal} file, or if it could not be written. */
public Writer writer(bool append, String charset)
{
	if (_type == FileType.Classpath) throw new GdxRuntimeException("Cannot write to a classpath file: " + file);
	if (_type == FileType.Internal) throw new GdxRuntimeException("Cannot write to an internal file: " + file);
	parent().mkdirs();
	try
	{
		FileOutputStream output = new FileOutputStream(file(), append);
		if (charset == null)
			return new OutputStreamWriter(output);
		else
			return new OutputStreamWriter(output, charset);
	}
	catch (IOException ex)
	{
		if (file().isDirectory())
			throw new GdxRuntimeException("Cannot open a stream to a directory: " + file + " (" + type + ")", ex);
		throw new GdxRuntimeException("Error writing file: " + file + " (" + type + ")", ex);
	}
}

/** Writes the specified string to the file using the default charset. Parent directories will be created if necessary.
 * @param append If false, this file will be overwritten if it exists, otherwise it will be appended.
 * @throws GdxRuntimeException if this file handle represents a directory, if it is a {@link FileType#Classpath} or
 *            {@link FileType#Internal} file, or if it could not be written. */
public void writeString(String @string, bool append)
{
	writeString(@string, append, null);
}

/** Writes the specified string to the file using the specified charset. Parent directories will be created if necessary.
 * @param append If false, this file will be overwritten if it exists, otherwise it will be appended.
 * @param charset May be null to use the default charset.
 * @throws GdxRuntimeException if this file handle represents a directory, if it is a {@link FileType#Classpath} or
 *            {@link FileType#Internal} file, or if it could not be written. */
public void writeString(String @string, bool append, String charset)
{
	Writer writer = null;
	try
	{
		writer = this.writer(append, charset);
		writer.write(@string);
	}
	catch (Exception ex)
	{
		throw new GdxRuntimeException("Error writing file: " + file + " (" + type + ")", ex);
	}
	finally
	{
		StreamUtils.closeQuietly(writer);
	}
}

/** Writes the specified bytes to the file. Parent directories will be created if necessary.
 * @param append If false, this file will be overwritten if it exists, otherwise it will be appended.
 * @throws GdxRuntimeException if this file handle represents a directory, if it is a {@link FileType#Classpath} or
 *            {@link FileType#Internal} file, or if it could not be written. */
public void writeBytes(byte[] bytes, bool append)
{
	OutputStream output = write(append);
	try
	{
		output.write(bytes);
	}
	catch (IOException ex)
	{
		throw new GdxRuntimeException("Error writing file: " + file + " (" + type + ")", ex);
	}
	finally
	{
		StreamUtils.closeQuietly(output);
	}
}

/** Writes the specified bytes to the file. Parent directories will be created if necessary.
 * @param append If false, this file will be overwritten if it exists, otherwise it will be appended.
 * @throws GdxRuntimeException if this file handle represents a directory, if it is a {@link FileType#Classpath} or
 *            {@link FileType#Internal} file, or if it could not be written. */
public void writeBytes(byte[] bytes, int offset, int length, bool append)
{
	OutputStream output = write(append);
	try
	{
		output.write(bytes, offset, length);
	}
	catch (IOException ex)
	{
		throw new GdxRuntimeException("Error writing file: " + file + " (" + type + ")", ex);
	}
	finally
	{
		StreamUtils.closeQuietly(output);
	}
}

/** Returns the paths to the children of this directory. Returns an empty list if this file handle represents a file and not a
 * directory. On the desktop, an {@link FileType#Internal} handle to a directory on the classpath will return a zero length
 * array.
 * @throws GdxRuntimeException if this file is an {@link FileType#Classpath} file. */
public FileHandle[] list()
{
	if (_type == FileType.Classpath) throw new GdxRuntimeException("Cannot list a classpath directory: " + _file);
	String[] relativePaths = file().list();
	if (relativePaths == null) return new FileHandle[0];
	FileHandle[] handles = new FileHandle[relativePaths.Length];
	for (int i = 0, n = relativePaths.Length; i < n; i++)
		handles[i] = child(relativePaths[i]);
	return handles;
}

/** Returns the paths to the children of this directory that satisfy the specified filter. Returns an empty list if this file
 * handle represents a file and not a directory. On the desktop, an {@link FileType#Internal} handle to a directory on the
 * classpath will return a zero length array.
 * @param filter the {@link FileFilter} to filter files
 * @throws GdxRuntimeException if this file is an {@link FileType#Classpath} file. */
public FileHandle[] list(FileFilter filter)
{
	if (_type == FileType.Classpath) throw new GdxRuntimeException("Cannot list a classpath directory: " + _file);
	File file = this.file();
	String[] relativePaths = file.list();
	if (relativePaths == null) return new FileHandle[0];
	FileHandle[] handles = new FileHandle[relativePaths.Length];
	int count = 0;
	for (int i = 0, n = relativePaths.Length; i < n; i++)
	{
		String path = relativePaths[i];
		FileHandle child = this.child(path);
		if (!filter.accept(child.file())) continue;
		handles[count] = child;
		count++;
	}
	if (count < relativePaths.Length)
	{
		FileHandle[] newHandles = new FileHandle[count];
		Array.Copy(handles, 0, newHandles, 0, count);
		handles = newHandles;
	}
	return handles;
}

/** Returns the paths to the children of this directory that satisfy the specified filter. Returns an empty list if this file
 * handle represents a file and not a directory. On the desktop, an {@link FileType#Internal} handle to a directory on the
 * classpath will return a zero length array.
 * @param filter the {@link FilenameFilter} to filter files
 * @throws GdxRuntimeException if this file is an {@link FileType#Classpath} file. */
public FileHandle[] list(FilenameFilter filter)
{
	if (_type == FileType.Classpath) throw new GdxRuntimeException("Cannot list a classpath directory: " + _file);
	File file = this.file();
	String[] relativePaths = file.list();
	if (relativePaths == null) return new FileHandle[0];
	FileHandle[] handles = new FileHandle[relativePaths.Length];
	int count = 0;
	for (int i = 0, n = relativePaths.Length; i < n; i++)
	{
		String path = relativePaths[i];
		if (!filter.accept(file, path)) continue;
		handles[count] = child(path);
		count++;
	}
	if (count < relativePaths.Length)
	{
		FileHandle[] newHandles = new FileHandle[count];
		Array.Copy(handles, 0, newHandles, 0, count);
		handles = newHandles;
	}
	return handles;
}

/** Returns the paths to the children of this directory with the specified suffix. Returns an empty list if this file handle
 * represents a file and not a directory. On the desktop, an {@link FileType#Internal} handle to a directory on the classpath
 * will return a zero length array.
 * @throws GdxRuntimeException if this file is an {@link FileType#Classpath} file. */
public FileHandle[] list(String suffix)
{
	if (_type == FileType.Classpath) throw new GdxRuntimeException("Cannot list a classpath directory: " + _file);
	String[] relativePaths = file().list();
	if (relativePaths == null) return new FileHandle[0];
	FileHandle[] handles = new FileHandle[relativePaths.Length];
	int count = 0;
	for (int i = 0, n = relativePaths.Length; i < n; i++)
	{
		String path = relativePaths[i];
		if (!path.EndsWith(suffix)) continue;
		handles[count] = child(path);
		count++;
	}
	if (count < relativePaths.Length)
	{
		FileHandle[] newHandles = new FileHandle[count];
		Array.Copy(handles, 0, newHandles, 0, count);
		handles = newHandles;
	}
	return handles;
}

/** Returns true if this file is a directory. Always returns false for classpath files. On Android, an
 * {@link FileType#Internal} handle to an empty directory will return false. On the desktop, an {@link FileType#Internal}
 * handle to a directory on the classpath will return false. */
public bool isDirectory()
{
	if (_type == FileType.Classpath) return false;
	return file().isDirectory();
}

/** Returns a handle to the child with the specified name. */
public virtual FileHandle child(String name)
{
	if (_file.getPath().Length == 0) return new FileHandle(new File(name), _type);
	return new FileHandle(new File(_file, name), _type);
}

/** Returns a handle to the sibling with the specified name.
 * @throws GdxRuntimeException if this file is the root. */
public virtual FileHandle sibling(String name)
{
	if (_file.getPath().Length == 0) throw new GdxRuntimeException("Cannot get the sibling of the root.");
	return new FileHandle(new File(_file.getParent(), name), _type);
}

public virtual FileHandle? parent()
{
	File parent = _file.getParentFile();
	if (parent == null)
	{
		if (_type == FileType.Absolute)
			parent = new File("/");
		else
			parent = new File("");
	}
	return new FileHandle(parent, _type);
}

/** @throws GdxRuntimeException if this file handle is a {@link FileType#Classpath} or {@link FileType#Internal} file. */
public void mkdirs()
{
	if (_type == FileType.Classpath) throw new GdxRuntimeException("Cannot mkdirs with a classpath file: " + _file);
	if (_type == FileType.Internal) throw new GdxRuntimeException("Cannot mkdirs with an internal file: " + _file);
	file().mkdirs();
}

/** Returns true if the file exists. On Android, a {@link FileType#Classpath} or {@link FileType#Internal} handle to a
 * directory will always return false. Note that this can be very slow for internal files on Android! */
public bool exists()
{
	switch (_type)
	{
		case FileType.Internal:
			if (file().exists()) return true;
			return getResource();

		case FileType.Classpath:
		{
			return getResource();
		}
			
		}

		return file().exists();

		bool getResource()
		{
			//return getResource("/" + _file.getPath().Replace('\\', '/')) != null;
			throw new NotImplementedException();
			}
	}

	/** Deletes this file or empty directory and returns success. Will not delete a directory that has children.
	 * @throws GdxRuntimeException if this file handle is a {@link FileType#Classpath} or {@link FileType#Internal} file. */
	public bool delete()
{
	if (_type == FileType.Classpath) throw new GdxRuntimeException("Cannot delete a classpath file: " + _file);
	if (_type == FileType.Internal) throw new GdxRuntimeException("Cannot delete an internal file: " + _file);
	return file().delete();
}

/** Deletes this file or directory and all children, recursively.
 * @throws GdxRuntimeException if this file handle is a {@link FileType#Classpath} or {@link FileType#Internal} file. */
public bool deleteDirectory()
{
	if (_type == FileType.Classpath) throw new GdxRuntimeException("Cannot delete a classpath file: " + _file);
	if (_type == FileType.Internal) throw new GdxRuntimeException("Cannot delete an internal file: " + _file);
	return deleteDirectory(file());
}

/** Deletes all children of this directory, recursively.
 * @throws GdxRuntimeException if this file handle is a {@link FileType#Classpath} or {@link FileType#Internal} file. */
public void emptyDirectory()
{
	emptyDirectory(false);
}

/** Deletes all children of this directory, recursively. Optionally preserving the folder structure.
 * @throws GdxRuntimeException if this file handle is a {@link FileType#Classpath} or {@link FileType#Internal} file. */
public void emptyDirectory(bool preserveTree)
{
	if (_type == FileType.Classpath) throw new GdxRuntimeException("Cannot delete a classpath file: " + _file);
	if (_type == FileType.Internal) throw new GdxRuntimeException("Cannot delete an internal file: " + _file);
	emptyDirectory(file(), preserveTree);
}

/** Copies this file or directory to the specified file or directory. If this handle is a file, then 1) if the destination is a
 * file, it is overwritten, or 2) if the destination is a directory, this file is copied into it, or 3) if the destination
 * doesn't exist, {@link #mkdirs()} is called on the destination's parent and this file is copied into it with a new name. If
 * this handle is a directory, then 1) if the destination is a file, GdxRuntimeException is thrown, or 2) if the destination is
 * a directory, this directory is copied into it recursively, overwriting existing files, or 3) if the destination doesn't
 * exist, {@link #mkdirs()} is called on the destination and this directory is copied into it recursively.
 * @throws GdxRuntimeException if the destination file handle is a {@link FileType#Classpath} or {@link FileType#Internal}
 *            file, or copying failed. */
public void copyTo(FileHandle dest)
{
	if (!isDirectory())
	{
		if (dest.isDirectory()) dest = dest.child(name());
		copyFile(this, dest);
		return;
	}
	if (dest.exists())
	{
		if (!dest.isDirectory()) throw new GdxRuntimeException("Destination exists but is not a directory: " + dest);
	}
	else
	{
		dest.mkdirs();
		if (!dest.isDirectory()) throw new GdxRuntimeException("Destination directory cannot be created: " + dest);
	}
	copyDirectory(this, dest.child(name()));
}

/** Moves this file to the specified file, overwriting the file if it already exists.
 * @throws GdxRuntimeException if the source or destination file handle is a {@link FileType#Classpath} or
 *            {@link FileType#Internal} file. */
public void moveTo(FileHandle dest)
{
	switch (_type)
	{
		case FileType.Classpath:
			throw new GdxRuntimeException("Cannot move a classpath file: " + _file);
		case FileType.Internal:
			throw new GdxRuntimeException("Cannot move an internal file: " + _file);
		case FileType.Absolute:
		case FileType.External:
			// Try rename for efficiency and to change case on case-insensitive file systems.
			if (file().renameTo(dest.file())) return;
			break;
	}
	copyTo(dest);
	delete();
	if (exists() && isDirectory()) deleteDirectory();
}

/** Returns the length in bytes of this file, or 0 if this file is a directory, does not exist, or the size cannot otherwise be
 * determined. */
public long length()
{
	if (_type == FileType.Classpath || (_type == FileType.Internal && !_file.exists()))
	{
		InputStream input = read();
		try
		{
			return input.available();
		}
		catch (Exception ignored)
		{
		}
		finally
		{
			StreamUtils.closeQuietly(input);
		}
		return 0;
	}
	return file().length();
}

/** Returns the last modified time in milliseconds for this file. Zero is returned if the file doesn't exist. Zero is returned
 * for {@link FileType#Classpath} files. On Android, zero is returned for {@link FileType#Internal} files. On the desktop, zero
 * is returned for {@link FileType#Internal} files on the classpath. */
public long lastModified()
{
	return file().lastModified();
}

public override bool Equals(Object? obj)
{
	if (!(obj is FileHandle)) return false;
	FileHandle other = (FileHandle)obj;
	return type == other.type && path().Equals(other.path());
}

public override int GetHashCode()
{
	int hash = 1;
	hash = hash * 37 + _type.GetHashCode();
	hash = hash * 67 + path().GetHashCode();
	return hash;
}

public override String ToString()
{
	return _file.getPath().Replace('\\', '/');
}

static public FileHandle tempFile(String prefix)
{
	try
	{
		return new FileHandle(File.createTempFile(prefix, null));
	}
	catch (IOException ex)
	{
		throw new GdxRuntimeException("Unable to create temp file.", ex);
	}
}

static public FileHandle tempDirectory(String prefix)
{
	try
	{
		File file = File.createTempFile(prefix, null);
		if (!file.delete()) throw new IOException("Unable to delete temp file: " + file);
		if (!file.mkdir()) throw new IOException("Unable to create temp directory: " + file);
		return new FileHandle(file);
	}
	catch (IOException ex)
	{
		throw new GdxRuntimeException("Unable to create temp file.", ex);
	}
}

static private void emptyDirectory(File file, bool preserveTree)
{
	if (file.exists())
	{
		File[] files = file.listFiles();
		if (files != null)
		{
			for (int i = 0, n = files.Length; i < n; i++)
			{
				if (!files[i].isDirectory())
					files[i].delete();
				else if (preserveTree)
					emptyDirectory(files[i], true);
				else
					deleteDirectory(files[i]);
			}
		}
	}
}

static private bool deleteDirectory(File file)
{
	emptyDirectory(file, false);
	return file.delete();
}

static private void copyFile(FileHandle source, FileHandle dest)
{
	try
	{
		dest.write(source.read(), false);
	}
	catch (Exception ex)
	{
		throw new GdxRuntimeException("Error copying source file: " + source.file + " (" + source.type + ")\n" //
			+ "To destination: " + dest.file + " (" + dest.type + ")", ex);
	}
}

static private void copyDirectory(FileHandle sourceDir, FileHandle destDir)
{
	destDir.mkdirs();
	FileHandle[] files = sourceDir.list();
	for (int i = 0, n = files.Length; i < n; i++)
	{
		FileHandle srcFile = files[i];
		FileHandle destFile = destDir.child(srcFile.name());
		if (srcFile.isDirectory())
			copyDirectory(srcFile, destFile);
		else
			copyFile(srcFile, destFile);
	}
}
}
}
