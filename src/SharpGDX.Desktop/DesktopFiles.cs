using SharpGDX.Files;

namespace SharpGDX.Desktop
{
	/** @author mzechner
 * @author Nathan Sweet */
	public sealed class DesktopFiles : IFiles
	{
		public static string externalPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		public static string localPath => "";


		public FileHandle getFileHandle(String fileName, IFiles.FileType type)
		{
			return new DesktopFileHandle(fileName, type);
		}

		public FileHandle classpath(String path)
		{
			return new DesktopFileHandle(path, IFiles.FileType.Classpath);
		}

		public FileHandle @internal(String path)
		{
			return new DesktopFileHandle(path, IFiles.FileType.Internal);
		}

		public FileHandle external(String path)
		{
			return new DesktopFileHandle(path, IFiles.FileType.External);
		}

		public FileHandle absolute(String path)
		{
			return new DesktopFileHandle(path, IFiles.FileType.Absolute);
		}

		public FileHandle local(String path)
		{
			return new DesktopFileHandle(path, IFiles.FileType.Local);
		}

		public String getExternalStoragePath()
		{
			return externalPath;
		}

		public bool isExternalStorageAvailable()
		{
			return true;
		}

		public String getLocalStoragePath()
		{
			return localPath;
		}

		public bool isLocalStorageAvailable()
		{
			return true;
		}
	}
}
