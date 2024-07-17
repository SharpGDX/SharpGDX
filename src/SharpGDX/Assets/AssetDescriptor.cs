using System.Text;
using SharpGDX.Files;

namespace SharpGDX.Assets;

/// <inheritdoc cref="IAssetDescriptor" />
/// <typeparam name="T">The asset type.</typeparam>
public class AssetDescriptor<T> : IAssetDescriptor
{
	public AssetDescriptor(string fileName, Type assetType)
		: this(fileName, assetType, null)
	{
	}

	/// <summary>
	///     Creates an AssetDescriptor with an already resolved name.
	/// </summary>
	/// <param name="file">TODO</param>
	/// <param name="assetType">TODO</param>
	public AssetDescriptor(FileHandle file, Type assetType)
		: this(file, assetType, null)
	{
	}

	public AssetDescriptor(string fileName, Type assetType, AssetLoaderParameters<T>? @params)
	{
		FileName = fileName;
		Type = assetType;
		Parameters = @params;
	}

	/// <summary>
	///     Creates an AssetDescriptor with an already resolved name.
	/// </summary>
	/// <param name="file">TODO</param>
	/// <param name="assetType">TODO</param>
	/// <param name="params">TODO</param>
	public AssetDescriptor(FileHandle file, Type assetType, AssetLoaderParameters<T>? @params)
	{
		FileName = file.path();
		File = file;
		Type = assetType;
		Parameters = @params;
	}

	/// <summary>
	///     The resolved file.
	/// </summary>
	/// <remarks>
	///     May be null if the fileName has not been resolved yet.
	/// </remarks>
	public FileHandle? File { get; set; }

	public string FileName { get; }

	public AssetLoaderParameters<T>? Parameters { get; }

	public Type Type { get; }

	IAssetLoaderParameters? IAssetDescriptor.Parameters => Parameters;

	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.Append(FileName);
		sb.Append(", ");
		sb.Append(Type.Name);
		return sb.ToString();
	}
}