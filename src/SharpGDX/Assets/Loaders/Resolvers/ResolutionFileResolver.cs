using SharpGDX.Files;
using SharpGDX.Shims;

namespace SharpGDX.Assets.Loaders.Resolvers;

/// <summary>
///     This <see cref="IFileHandleResolver" /> uses a given list of <see cref="Resolution" />s to determine the best match
///     based on the current back buffer size.
/// </summary>
public class ResolutionFileResolver : IFileHandleResolver
{
	protected readonly IFileHandleResolver BaseResolver;
	protected readonly Resolution[] Descriptors;

	/// <summary>
	///     Creates a <see cref="ResolutionFileResolver" /> based on a given <see cref="IFileHandleResolver" /> and a list of
	///     <see cref="Resolution" />s.
	/// </summary>
	/// <param name="baseResolver">The <see cref="IFileHandleResolver" /> that will ultimately be used to resolve the file.</param>
	/// <param name="descriptors">A list of <see cref="Resolution" />s. At least one has to be supplied.</param>
	/// <exception cref="IllegalArgumentException">Thrown when no <see cref="Resolution"/>s are supplied.</exception>
	public ResolutionFileResolver(IFileHandleResolver baseResolver, Resolution[] descriptors)
	{
		if (descriptors.Length == 0)
		{
			throw new IllegalArgumentException("At least one Resolution needs to be supplied.");
		}

		BaseResolver = baseResolver;
		Descriptors = descriptors;
	}

	public static Resolution Choose(Resolution[] descriptors)
	{
		int w = Gdx.graphics.getBackBufferWidth(), h = Gdx.graphics.getBackBufferHeight();
		var best = descriptors[0];

		if (w < h)
		{
			for (int i = 0, n = descriptors.Length; i < n; i++)
			{
				var other = descriptors[i];
				if (w >= other.PortraitWidth && other.PortraitWidth >= best.PortraitWidth && h >= other.PortraitHeight
				    && other.PortraitHeight >= best.PortraitHeight)
				{
					best = descriptors[i];
				}
			}
		}
		else
		{
			for (int i = 0, n = descriptors.Length; i < n; i++)
			{
				var other = descriptors[i];
				if (w >= other.PortraitHeight && other.PortraitHeight >= best.PortraitHeight && h >= other.PortraitWidth
				    && other.PortraitWidth >= best.PortraitWidth)
				{
					best = descriptors[i];
				}
			}
		}

		return best;
	}

	public FileHandle Resolve(string fileName)
	{
		var bestResolution = Choose(Descriptors);
		var originalHandle = new FileHandle(fileName);
		var handle = BaseResolver.Resolve(Resolve(originalHandle, bestResolution.Folder));
		if (!handle.exists())
		{
			handle = BaseResolver.Resolve(fileName);
		}

		return handle;
	}

	protected string Resolve(FileHandle originalHandle, string suffix)
	{
		var parentString = "";
		var parent = originalHandle.parent();

		if (parent != null && !parent.name().Equals(""))
		{
			parentString = parent + "/";
		}

		return parentString + suffix + "/" + originalHandle.name();
	}

	public class Resolution
	{
		/// <summary>
		///     The name of the folder, where the assets which fit this resolution, are located.
		/// </summary>
		public readonly string Folder;

		public readonly int PortraitHeight;
		public readonly int PortraitWidth;

		/// <summary>
		///     Constructs a <see cref="Resolution" />.
		/// </summary>
		/// <param name="portraitWidth">This resolution's width.</param>
		/// <param name="portraitHeight">This resolution's height.</param>
		/// <param name="folder">The name of the folder, where the assets which fit this resolution, are located.</param>
		public Resolution(int portraitWidth, int portraitHeight, string folder)
		{
			PortraitWidth = portraitWidth;
			PortraitHeight = portraitHeight;
			Folder = folder;
		}
	}
}