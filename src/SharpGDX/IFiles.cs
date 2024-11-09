using SharpGDX.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX
{
	/** Provides standard access to the filesystem, classpath, Android app storage (internal and external), and Android assets
 * directory.
 * @author mzechner
 * @author Nathan Sweet */
	public interface IFiles
	{
		/** Returns a handle representing a file or directory.
		 * @param type Determines how the path is resolved.
		 * @throws GdxRuntimeException if the type is classpath or internal and the file does not exist.
		 * @see FileType */
		public FileHandle GetFileHandle(String path, FileType type);

		/** Convenience method that returns a {@link FileType#Classpath} file handle. */
		public FileHandle Classpath(String path);

		/** Convenience method that returns a {@link FileType#Internal} file handle. */
		public FileHandle Internal (String path);

		/** Convenience method that returns a {@link FileType#External} file handle. */
		public FileHandle External(String path);

		/** Convenience method that returns a {@link FileType#Absolute} file handle. */
		public FileHandle Absolute(String path);

		/** Convenience method that returns a {@link FileType#Local} file handle. */
		public FileHandle Local(String path);

		/** Returns the external storage path directory. This is the app external storage on Android and the home directory of the
		 * current user on the desktop. */
		public String GetExternalStoragePath();

		/** Returns true if the external storage is ready for file IO. */
		public bool IsExternalStorageAvailable();

		/** Returns the local storage path directory. This is the private files directory on Android and the directory of the jar on
		 * the desktop. */
		public String GetLocalStoragePath();

		/** Returns true if the local storage is ready for file IO. */
		public bool IsLocalStorageAvailable();
	}
}
