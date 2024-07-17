using SharpGDX.Files;
using SharpGDX.Utils;
using File = SharpGDX.Shims.File;

namespace SharpGDX.Headless;

public sealed class HeadlessFileHandle : FileHandle
{
	public HeadlessFileHandle(string fileName, IFiles.FileType type)
		: base(fileName, type)
	{
	}

	public HeadlessFileHandle(File file, IFiles.FileType type)
		: base(file, type)
	{
	}

	public override FileHandle child(string name)
	{
		return _file.getPath().Length == 0
			? new HeadlessFileHandle(new File(name), _type)
			: new HeadlessFileHandle(new File(_file, name), _type);
	}

	public override File file()
	{
		return _type switch
		{
			IFiles.FileType.External => new File(HeadlessFiles.ExternalPath, _file.getPath()),
			IFiles.FileType.Local => new File(HeadlessFiles.LocalPath, _file.getPath()),
			_ => _file
		};
	}

	public override FileHandle parent()
	{
		var parent = _file.getParentFile() ?? (_type == IFiles.FileType.Absolute ? new File("/") : new File(""));

		return new HeadlessFileHandle(parent, _type);
	}

	public override FileHandle sibling(string name)
	{
		if (_file.getPath().Length == 0)
		{
			throw new GdxRuntimeException("Cannot get the sibling of the root.");
		}

		return new HeadlessFileHandle(new File(_file.getParent(), name), _type);
	}
}