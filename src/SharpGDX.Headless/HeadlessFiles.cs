using SharpGDX.Files;

namespace SharpGDX.Headless;

public sealed class HeadlessFiles : IFiles
{
	public static string ExternalPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

	public static string LocalPath => "";

	public FileHandle absolute(string path)
	{
		return new HeadlessFileHandle(path, IFiles.FileType.Absolute);
	}

	public FileHandle classpath(string path)
	{
		return new HeadlessFileHandle(path, IFiles.FileType.Classpath);
	}

	public FileHandle external(string path)
	{
		return new HeadlessFileHandle(path, IFiles.FileType.External);
	}

	public string getExternalStoragePath()
	{
		return ExternalPath;
	}


	public FileHandle getFileHandle(string fileName, IFiles.FileType type)
	{
		return new HeadlessFileHandle(fileName, type);
	}

	public string getLocalStoragePath()
	{
		return LocalPath;
	}

	public FileHandle @internal(string path)
	{
		return new HeadlessFileHandle(path, IFiles.FileType.Internal);
	}

	public bool isExternalStorageAvailable()
	{
		return true;
	}

	public bool isLocalStorageAvailable()
	{
		return true;
	}

	public FileHandle local(string path)
	{
		return new HeadlessFileHandle(path, IFiles.FileType.Local);
	}
}