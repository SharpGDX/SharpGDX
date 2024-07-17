using SharpGDX.Audio;
using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders;

/** {@link AssetLoader} for {@link Music} instances. The Music instance is loaded synchronously.
 * @author mzechner */
public class MusicLoader : AsynchronousAssetLoader<IMusic, MusicLoader.MusicParameter> {

	private IMusic music;

	public MusicLoader (IFileHandleResolver resolver) 
	: base(resolver)
	{
		
	}

	/** Returns the {@link Music} instance currently loaded by this {@link MusicLoader}.
	 * 
	 * @return the currently loaded {@link Music}, otherwise {@code null} if no {@link Music} has been loaded yet. */
	protected IMusic getLoadedMusic () {
		return music;
	}

	public override void loadAsync (AssetManager manager, String fileName, FileHandle file, MusicParameter parameter) {
		music = Gdx.audio.newMusic(file);
	}

	public override IMusic loadSync (AssetManager manager, String fileName, FileHandle file, MusicParameter parameter) {
		IMusic music = this.music;
		this.music = null;
		return music;
	}

	public override Array<AssetDescriptor<IMusic>> getDependencies (String fileName, FileHandle file, MusicParameter parameter) {
		return null;
	}

	public class MusicParameter : AssetLoaderParameters<IMusic> {
	}

}
