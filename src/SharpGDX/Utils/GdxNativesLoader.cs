using System.Runtime.InteropServices;

namespace SharpGDX.Utils;

public class GdxNativesLoader
{
	private static readonly object Lock = new();

	private static bool _nativesLoaded;

	public static bool DisableNativesLoading { get; set; } = false;

	/// <summary>
	///     Loads the sharpGDX native libraries if they have not already been loaded.
	/// </summary>
	public static void Load()
	{
		lock (Lock)
		{
			if (_nativesLoaded)
			{
				return;
			}

			if (DisableNativesLoading)
			{
				return;
			}

			NativeLibrary.Load("gdx2d");
			_nativesLoaded = true;
		}
	}
}