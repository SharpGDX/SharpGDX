using SharpGDX.Files;

namespace SharpGDX.Desktop
{
	/** @author mzechner
 * @author Nathan Sweet */
	public sealed class DesktopFiles : IFiles
	{
		public static string externalPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		public static string localPath => "";


		public FileHandle GetFileHandle(String fileName, FileType type)
		{
			return new DesktopFileHandle(fileName, type);
		}

		public FileHandle Classpath(String path)
		{
			return new DesktopFileHandle(path, FileType.Classpath);
		}

		public FileHandle Internal(String path)
		{
			return new DesktopFileHandle(path, FileType.Internal);
		}

		public FileHandle External(String path)
		{
			return new DesktopFileHandle(path, FileType.External);
		}

		public FileHandle Absolute(String path)
		{
			return new DesktopFileHandle(path, FileType.Absolute);
		}

		public FileHandle Local(String path)
		{
			return new DesktopFileHandle(path, FileType.Local);
		}

		public String GetExternalStoragePath()
		{
			return externalPath;
		}

		public bool IsExternalStorageAvailable()
		{
			return true;
		}

		public String GetLocalStoragePath()
		{
			return localPath;
		}

		public bool IsLocalStorageAvailable()
		{
			return true;
		}
	}
}
