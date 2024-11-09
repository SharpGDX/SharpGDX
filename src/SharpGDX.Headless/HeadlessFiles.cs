using SharpGDX.Files;

namespace SharpGDX.Headless;

public sealed class HeadlessFiles : IFiles
{
	public static string ExternalPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

	public static string LocalPath => "";

	public FileHandle Absolute(string path)
	{
		return new HeadlessFileHandle(path, FileType.Absolute);
	}

	public FileHandle Classpath(string path)
	{
		return new HeadlessFileHandle(path, FileType.Classpath);
	}

	public FileHandle External(string path)
	{
		return new HeadlessFileHandle(path, FileType.External);
	}

	public string GetExternalStoragePath()
	{
		return ExternalPath;
	}
    
	public FileHandle GetFileHandle(string fileName, FileType type)
	{
		return new HeadlessFileHandle(fileName, type);
	}

	public string GetLocalStoragePath()
	{
		return LocalPath;
	}

	public FileHandle Internal(string path)
	{
		return new HeadlessFileHandle(path, FileType.Internal);
	}

	public bool IsExternalStorageAvailable()
	{
		return true;
	}

	public bool IsLocalStorageAvailable()
	{
		return true;
	}

	public FileHandle Local(string path)
	{
		return new HeadlessFileHandle(path, FileType.Local);
	}
}