using SharpGDX.Audio;
using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders;

/** {@link AssetLoader} to load {@link Sound} instances.
 * @author mzechner */
public class SoundLoader : AsynchronousAssetLoader<ISound, SoundLoader.SoundParameter> {

	private ISound sound;

	public SoundLoader (IFileHandleResolver resolver) 
	: base(resolver)
	{
		
	}

	/** Returns the {@link Sound} instance currently loaded by this {@link SoundLoader}.
	 * 
	 * @return the currently loaded {@link Sound}, otherwise {@code null} if no {@link Sound} has been loaded yet. */
	protected ISound getLoadedSound () {
		return sound;
	}

	public override void loadAsync (AssetManager manager, String fileName, FileHandle file, SoundParameter parameter) {
		sound = Gdx.audio.newSound(file);
	}

	public override ISound loadSync (AssetManager manager, String fileName, FileHandle file, SoundParameter parameter) {
		ISound sound = this.sound;
		this.sound = null;
		return sound;
	}

	public override Array<AssetDescriptor<ISound>> getDependencies (String fileName, FileHandle file, SoundParameter parameter) {
		return null;
	}

	public class SoundParameter : AssetLoaderParameters<ISound> {
	}

}
