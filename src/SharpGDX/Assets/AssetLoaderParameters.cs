using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static SharpGDX.Assets.IAssetLoaderParameters;

namespace SharpGDX.Assets
{
	// TODO: Split to two files.

	public interface IAssetLoaderParameters
	{
		/**
		 * Callback interface that will be invoked when the {@link AssetManager} loaded an asset.
		 * @author mzechner
		 */
		public delegate void FinishedLoadingCallback(AssetManager assetManager, string fileName, Type type);

		public FinishedLoadingCallback loadedCallback { get; }
	}

	public class AssetLoaderParameters<T> : IAssetLoaderParameters
	{
		public FinishedLoadingCallback loadedCallback;
		FinishedLoadingCallback IAssetLoaderParameters.loadedCallback => loadedCallback;
	}
}
