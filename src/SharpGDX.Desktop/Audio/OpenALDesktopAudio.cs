using SharpGDX.Files;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using OpenTK.Audio.OpenAL;
using SharpGDX.Audio;
using SharpGDX.Shims;

namespace SharpGDX.Desktop.Audio
{
	/** @author Nathan Sweet */
	public class OpenALDesktopAudio : IDesktopAudio
	{
		private readonly int deviceBufferSize;
		private readonly int deviceBufferCount;
		private IntArray idleSources, allSources;
		private LongMap<int> soundIdToSource;
		private IntMap<long> sourceToSoundId;

		private long nextSoundId = 0;

		// TODO: Can this be generic for OpenALSound?
		private ObjectMap<String, Type> extensionToSoundClass = new();

		// TODO: Can this be generic for OpenALMusic?
		private ObjectMap<String, Type> extensionToMusicClass = new();
		private OpenALSound[] recentSounds;
		private int mostRecetSound = -1;
		private String preferredOutputDevice = null;
		private Thread observerThread;

		internal Array<OpenALMusic> music = new(false, 1, typeof(OpenALMusic));
		ALDevice device;
		ALContext context;
		internal bool noDevice = false;

		public OpenALDesktopAudio()
			: this(16, 9, 512)
		{

		}

		public OpenALDesktopAudio(int simultaneousSources, int deviceBufferCount, int deviceBufferSize)
		{
			this.deviceBufferSize = deviceBufferSize;
			this.deviceBufferCount = deviceBufferCount;

			//registerSound("ogg", Ogg.Sound.class);
			//registerMusic("ogg", Ogg.Music.class);
			registerSound("wav", typeof(Wav.Sound));
			registerMusic("wav", typeof(Wav.Music));
			//registerSound("mp3", Mp3.Sound.class);
			//registerMusic("mp3", Mp3.Music.class);

			device = ALC.OpenDevice(null);
			if (device == 0L)
			{
				noDevice = true;
				return;
			}

			// TODO: ALCCapabilities deviceCapabilities = ALC.createCapabilities(device);
			context = ALC.CreateContext((device), (int[]?)null);
			if (context == 0L)
			{
				ALC.CloseDevice((device));
				noDevice = true;
				return;
			}

			if (ALC.MakeContextCurrent((context)) == false)
			{
				noDevice = true;
				return;
			}
			// TODO: AL.createCapabilities(deviceCapabilities);

			AL.GetError();
			allSources = new IntArray(false, simultaneousSources);
			for (int i = 0; i < simultaneousSources; i++)
			{
				AL.GenSource(out int sourceID);
				if (AL.GetError() != (int)ALError.NoError) break;
				allSources.add(sourceID);
			}

			idleSources = new IntArray(allSources);
			soundIdToSource = new LongMap<int>();
			sourceToSoundId = new IntMap<long>();

			AL.Listener(ALListenerfv.Orientation, new float[] { 0.0f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f });
			AL.Listener(ALListener3f.Velocity, 0.0f, 0.0f, 0.0f);
			AL.Listener(ALListener3f.Position, 0.0f, 0.0f, 0.0f);

			// TODO: AL.Disable(SOFTXHoldOnDisconnect.AL_STOP_SOURCES_ON_DISCONNECT_SOFT);
			observerThread = new Thread(() =>
			{

				while (true)
				{
					// TODO: int ALC_CONNECTED = 0x313;
					// TODO: How can this be replicated?
					// ALC.GetInteger(device, AlcGetInteger. ALC_CONNECTED, 1, out int state);
					var state = 1;
					bool isConnected = state != 0;
					if (!isConnected)
					{
						// The device is at a state where it can't recover
						// This is usually the windows path on removing a device
						switchOutputDevice(null, false);
						continue;
					}

					if (preferredOutputDevice != null)
					{
						if (Arrays.asList(getAvailableOutputDevices()).Contains(preferredOutputDevice))
						{
							if (!preferredOutputDevice.Equals(ALC.GetString(device, AlcGetString.AllDevicesSpecifier)))
							{
								// The preferred output device is reconnected, let's switch back to it
								switchOutputDevice(preferredOutputDevice);
							}
						}
						else
						{
							// This is usually the mac/linux path
							if (preferredOutputDevice.Equals(ALC.GetString((device), AlcGetString.AllDevicesSpecifier)))
							{
								// The preferred output device is reconnected, let's switch back to it
								switchOutputDevice(null, false);
							}
						}
					}
					else
					{
						String[] currentDevices = getAvailableOutputDevices();
						// If a new device got added, re evaluate "auto" mode
						if (!Arrays.equals(currentDevices, lastAvailableDevices))
						{
							switchOutputDevice(null);
						}

						// Update last available devices
						lastAvailableDevices = currentDevices;
					}

					try
					{
						Thread.Sleep(1000);
					}
					catch (ThreadInterruptedException ignored)
					{
						return;
					}
				}

			});
			observerThread.IsBackground = (true);
			observerThread.Start();

			recentSounds = new OpenALSound[simultaneousSources];
		}

		private String[] lastAvailableDevices = new String[0];

		public void registerSound(String extension, Type soundClass)
		{
			if (extension == null) throw new IllegalArgumentException("extension cannot be null.");
			if (soundClass == null) throw new IllegalArgumentException("soundClass cannot be null.");
			extensionToSoundClass.put(extension, soundClass);
		}

		public void registerMusic(String extension, Type musicClass)
		{
			if (extension == null) throw new IllegalArgumentException("extension cannot be null.");
			if (musicClass == null) throw new IllegalArgumentException("musicClass cannot be null.");
			extensionToMusicClass.put(extension, musicClass);
		}

		public ISound newSound(FileHandle file)
		{
			if (file == null) throw new IllegalArgumentException("file cannot be null.");
			Type soundClass = extensionToSoundClass.get(file.extension().ToLower());
			if (soundClass == null) throw new GdxRuntimeException("Unknown file extension for sound: " + file);
			try
			{
				return (ISound)soundClass.GetConstructor([typeof(OpenALDesktopAudio), typeof(FileHandle)])
					.Invoke([this, file]);
			}
			catch (Exception ex)
			{
				throw new GdxRuntimeException("Error creating sound " + soundClass.Name + " for file: " + file, ex);
			}
		}

		public IMusic newMusic(FileHandle file)
		{
			if (file == null) throw new IllegalArgumentException("file cannot be null.");
			Type musicClass = extensionToMusicClass.get(file.extension().ToLower());
			if (musicClass == null) throw new GdxRuntimeException("Unknown file extension for music: " + file);
			try
			{
				return (IMusic)musicClass.GetConstructor([typeof(OpenALDesktopAudio), typeof(FileHandle)])
					.Invoke([this, file]);
			}
			catch (Exception ex)
			{
				throw new GdxRuntimeException("Error creating music " + musicClass.Name + " for file: " + file, ex);
			}
		}

		public bool switchOutputDevice(String deviceIdentifier)
		{
			return switchOutputDevice(deviceIdentifier, true);
		}

		private bool switchOutputDevice(String deviceIdentifier, bool setPreferred)
		{
			if (setPreferred)
			{
				preferredOutputDevice = deviceIdentifier;
			}

			// TODO: return SOFTReopenDevice.alcReopenDeviceSOFT(device, deviceIdentifier, (IntBuffer)null);
			return false;
		}

		public String[] getAvailableOutputDevices()
		{
			// TODO: Verify
			var devices = ALC.GetString(AlcGetStringList.AllDevicesSpecifier);
			if (devices == null) return new String[0];
			return devices.ToArray();
		}

		internal int obtainSource(bool isMusic)
		{
			if (noDevice) return 0;
			for (int i = 0, n = idleSources.size; i < n; i++)
			{
				int sourceId = idleSources.get(i);
				AL.GetSource(sourceId, ALGetSourcei.SourceState, out var state);
				if (state != (int)ALSourceState.Playing && state != (int)ALSourceState.Paused)
				{
					long? oldSoundId = sourceToSoundId.remove(sourceId);
					if (oldSoundId != null) soundIdToSource.remove(oldSoundId.Value);
					if (isMusic)
					{
						idleSources.removeIndex(i);
					}
					else
					{
						long soundId = nextSoundId++;
						sourceToSoundId.put(sourceId, soundId);
						soundIdToSource.put(soundId, sourceId);
					}

					AL.SourceStop(sourceId);
					AL.Source(sourceId, ALSourcei.Buffer, 0);
					AL.Source(sourceId, ALSourcef.Gain, 1);
					AL.Source(sourceId, ALSourcef.Pitch, 1);
					AL.Source(sourceId, ALSource3f.Position, 0, 0, 1f);
					// TODO: AL.Source(sourceId, SOFTDirectChannels.AL_DIRECT_CHANNELS_SOFT, SOFTDirectChannelsRemix.AL_REMIX_UNMATCHED_SOFT);
					return sourceId;
				}
			}

			return -1;
		}

		internal void freeSource(int sourceID)
		{
			if (noDevice) return;
			AL.SourceStop(sourceID);
			AL.Source(sourceID, ALSourcei.Buffer, 0);
			long? soundId = sourceToSoundId.remove(sourceID);
			if (soundId != null) soundIdToSource.remove(soundId.Value);
			idleSources.add(sourceID);
		}

		internal void freeBuffer(int bufferID)
		{
			if (noDevice) return;
			for (int i = 0, n = idleSources.size; i < n; i++)
			{
				int sourceID = idleSources.get(i);
				AL.GetSource(sourceID, ALGetSourcei.Buffer, out var buffer);
				if (buffer == bufferID)
				{
					long? soundId = sourceToSoundId.remove(sourceID);
					if (soundId != null) soundIdToSource.remove(soundId.Value);
					AL.SourceStop(sourceID);
					AL.Source(sourceID, ALSourcei.Buffer, 0);
				}
			}
		}

		internal void stopSourcesWithBuffer(int bufferID)
		{
			if (noDevice) return;
			for (int i = 0, n = idleSources.size; i < n; i++)
			{
				int sourceID = idleSources.get(i);
				AL.GetSource(sourceID, ALGetSourcei.Buffer, out var source);

				if (source == bufferID)
				{
					long? soundId = sourceToSoundId.remove(sourceID);
					if (soundId != null) soundIdToSource.remove(soundId.Value);
					AL.SourceStop(sourceID);
				}
			}
		}

		internal void pauseSourcesWithBuffer(int bufferID)
		{
			if (noDevice) return;
			for (int i = 0, n = idleSources.size; i < n; i++)
			{
				int sourceID = idleSources.get(i);
				AL.GetSource(sourceID, ALGetSourcei.Buffer, out var source);
				if (source == bufferID) AL.SourcePause(sourceID);
			}
		}

		internal void resumeSourcesWithBuffer(int bufferID)
		{
			if (noDevice) return;
			for (int i = 0, n = idleSources.size; i < n; i++)
			{
				int sourceID = idleSources.get(i);
				AL.GetSource(sourceID, ALGetSourcei.Buffer, out var buffer);
				if (buffer == bufferID)
				{
					AL.GetSource(sourceID, ALGetSourcei.SourceState, out var state);

					if (state == (int)ALSourceState.Paused)
					{
						AL.SourcePlay(sourceID);
					}
				}
			}
		}

		public void Update()
		{
			if (noDevice) return;
			for (int i = 0; i < music.size; i++)
				music.items[i].update();
		}

		public long getSoundId(int sourceId)
		{
			long? soundId = sourceToSoundId.get(sourceId);
			return soundId != null ? soundId.Value : -1;
		}

		public int getSourceId(long soundId)
		{
			int? sourceId = soundIdToSource.get(soundId);
			return sourceId != null ? sourceId.Value : -1;
		}

		public void stopSound(long soundId)
		{
			int? sourceId = soundIdToSource.get(soundId);
			if (sourceId != null) AL.SourceStop(sourceId.Value);
		}

		public void pauseSound(long soundId)
		{
			int? sourceId = soundIdToSource.get(soundId);
			if (sourceId != null) AL.SourcePause(sourceId.Value);
		}

		public void resumeSound(long soundId)
		{
			int sourceId = soundIdToSource.get(soundId, -1);
			AL.GetSource(sourceId, ALGetSourcei.SourceState, out int state);

			if (sourceId != -1 && state == (int)ALSourceState.Paused) AL.SourcePlay(sourceId);
		}

		public void setSoundGain(long soundId, float volume)
		{
			int? sourceId = soundIdToSource.get(soundId);
			if (sourceId != null) AL.Source(sourceId.Value, ALSourcef.Gain, volume);
		}

		public void setSoundLooping(long soundId, bool looping)
		{
			int? sourceId = soundIdToSource.get(soundId);
			if (sourceId != null) AL.Source(sourceId.Value, ALSourceb.Looping, looping);
		}

		public void setSoundPitch(long soundId, float pitch)
		{
			int? sourceId = soundIdToSource.get(soundId);
			if (sourceId != null) AL.Source(sourceId.Value, ALSourcef.Pitch, pitch);
		}

		public void setSoundPan(long soundId, float pan, float volume)
		{
			int sourceId = soundIdToSource.get(soundId, -1);
			if (sourceId != -1)
			{
				AL.Source(sourceId, ALSource3f.Position, MathUtils.cos((pan - 1) * MathUtils.HALF_PI), 0,
					MathUtils.sin((pan + 1) * MathUtils.HALF_PI));
				AL.Source(sourceId, ALSourcef.Gain, volume);
			}
		}

		public void dispose()
		{
			if (noDevice) return;
			observerThread.Interrupt();
			for (int i = 0, n = allSources.size; i < n; i++)
			{
				int sourceID = allSources.get(i);
				AL.GetSource(sourceID, ALGetSourcei.SourceState, out int state);
				if (state != (int)ALSourceState.Stopped) AL.SourceStop(sourceID);

				// TODO: Verify
				AL.DeleteSource(sourceID);
			}

			sourceToSoundId = null;
			soundIdToSource = null;

			ALC.DestroyContext(context);
			ALC.CloseDevice(device);
		}

		public IAudioDevice newAudioDevice(int sampleRate, bool isMono)
		{
			if (noDevice)
				return new NoAudioDevice(isMono)
				{

				};
			return new OpenALAudioDevice(this, sampleRate, isMono, deviceBufferSize, deviceBufferCount);
		}

		private class NoAudioDevice : IAudioDevice
		{
			private readonly bool _isMono;

			public NoAudioDevice(bool isMono)
			{
				_isMono = isMono;
			}

			public void WriteSamples(float[] samples, int offset, int numSamples)
			{
			}

			public void WriteSamples(short[] samples, int offset, int numSamples)
			{
			}

			public void SetVolume(float volume)
			{
			}

			public bool IsMono()
			{
				return _isMono;
			}

			public int GetLatency()
			{
				return 0;
			}

			public void dispose()
			{
			}

			public void Pause()
			{
			}

			public void Resume()
			{
			}
		}

		public IAudioRecorder newAudioRecorder(int samplingRate, bool isMono)
		{
			if (noDevice)
				return new NoAudioRecorder()
				{

				};
			return new JavaSoundAudioRecorder(samplingRate, isMono);
		}

		private class NoAudioRecorder : IAudioRecorder
		{
			public void Read(short[] samples, int offset, int numSamples)
			{
			}

			public void dispose()
			{
			}
		}

		/** Retains a list of the most recently played sounds and stops the sound played least recently if necessary for a new sound to
		 * play */
		internal void retain(OpenALSound sound, bool stop)
		{
			// Move the pointer ahead and wrap
			mostRecetSound++;
			mostRecetSound %= recentSounds.Length;

			if (stop)
			{
				// Stop the least recent sound (the one we are about to bump off the buffer)
				if (recentSounds[mostRecetSound] != null) recentSounds[mostRecetSound].stop();
			}

			recentSounds[mostRecetSound] = sound;
		}

		/** Removes the disposed sound from the least recently played list */
		public void forget(OpenALSound sound)
		{
			for (int i = 0; i < recentSounds.Length; i++)
			{
				if (recentSounds[i] == sound) recentSounds[i] = null;
			}
		}
	}
}