using SharpGDX.Files;

namespace SharpGDX.Assets;

/// <summary>
///     Describes an asset to be loaded by its filename and type and <see cref="IAssetLoaderParameters" />.
/// </summary>
/// <remarks>
///     Instances of this are used in <see cref="AssetLoadingTask" /> to load the actual asset.
/// </remarks>
public interface IAssetDescriptor
{
	/// <summary>
	///     Gets or sets the resolved file.
	/// </summary>
	/// <remarks>
	///     May be null if the fileName has not been resolved yet.
	/// </remarks>
	public FileHandle? File { get; set; }

	public string FileName { get; }

	public IAssetLoaderParameters? Parameters { get; }

	public Type Type { get; }
}