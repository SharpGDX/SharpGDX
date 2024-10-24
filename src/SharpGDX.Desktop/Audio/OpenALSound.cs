﻿using SharpGDX.Shims;
using Buffer = SharpGDX.Shims.Buffer;
using OpenTK.Audio.OpenAL;
using SharpGDX.Audio;

namespace SharpGDX.Desktop.Audio
{
	/** @author Nathan Sweet */
	public class OpenALSound : ISound
	{
	private int bufferID = -1;
	private readonly OpenALDesktopAudio audio;
	private float _duration;
	private int sampleRate, channels;

	public OpenALSound(OpenALDesktopAudio audio)
	{
		this.audio = audio;
	}

	protected void setup(byte[] pcm, int channels, int sampleRate)
	{
		int validBytes = pcm.Length - (pcm.Length % (channels > 1 ? 4 : 2));
		ByteBuffer buffer = BufferUtils.newByteBuffer(validBytes);
		buffer.put(pcm, 0, validBytes);
		((Buffer)buffer).flip();

		setup(buffer, channels, sampleRate);
	}

	protected void setup(ByteBuffer pcm, int channels, int sampleRate)
	{
		this.channels = channels;
		this.sampleRate = sampleRate;
		int sampleFrames = pcm.limit() / channels;
		_duration = sampleFrames / (float)sampleRate;

		if (bufferID == -1)
		{
			// TODO: I don't like this. -RP
			var bufferIds =new int[1];
			AL.GenBuffers(1, bufferIds);
			bufferID = bufferIds[0];
			AL.BufferData(bufferID, channels > 1 ? ALFormat.Stereo16 : ALFormat.Mono16, pcm.array(), sampleRate);
		}
	}
		
		public long Play()
	{
		return Play(1);
	}

	public long Play(float volume)
	{
		if (audio.noDevice) return 0;
		int sourceID = audio.obtainSource(false);
		if (sourceID == -1)
		{
			// Attempt to recover by stopping the least recently played sound
			audio.retain(this, true);
			sourceID = audio.obtainSource(false);
		}
		else
			audio.retain(this, false);
		// In case it still didn't work
		if (sourceID == -1) return -1;
		long soundId = audio.getSoundId(sourceID);
		AL.Source(sourceID, ALSourcei.Buffer, bufferID);
		AL.Source(sourceID, ALSourceb.Looping, false);
		AL.Source(sourceID, ALSourcef.Gain, volume);
		AL.SourcePlay(sourceID);
		return soundId;
	}

	public long Loop()
	{
		return Loop(1);
	}

	public long Loop(float volume)
	{
		if (audio.noDevice) return 0;
		int sourceID = audio.obtainSource(false);
		if (sourceID == -1) return -1;
		long soundId = audio.getSoundId(sourceID);
		AL.Source(sourceID, ALSourcei.Buffer, bufferID);
		AL.Source(sourceID, ALSourceb.Looping, true);
		AL.Source(sourceID, ALSourcef.Gain, volume);
		AL.SourcePlay(sourceID);
		return soundId;
	}

	public void Stop()
	{
		if (audio.noDevice) return;
		audio.stopSourcesWithBuffer(bufferID);
	}

	public void Dispose()
	{
		if (audio.noDevice) return;
		if (bufferID == -1) return;
		audio.freeBuffer(bufferID);
		AL.DeleteBuffers(1, new[] { bufferID });

	bufferID = -1;
		audio.forget(this);
	}

	public void Stop(long soundId)
	{
		if (audio.noDevice) return;
		audio.stopSound(soundId);
	}

	public void Pause()
	{
		if (audio.noDevice) return;
		audio.pauseSourcesWithBuffer(bufferID);
	}

	public void Pause(long soundId)
	{
		if (audio.noDevice) return;
		audio.pauseSound(soundId);
	}

	public void Resume()
	{
		if (audio.noDevice) return;
		audio.resumeSourcesWithBuffer(bufferID);
	}

	public void Resume(long soundId)
	{
		if (audio.noDevice) return;
		audio.resumeSound(soundId);
	}

	public void SetPitch(long soundId, float pitch)
	{
		if (audio.noDevice) return;
		audio.setSoundPitch(soundId, pitch);
	}

	public void SetVolume(long soundId, float volume)
	{
		if (audio.noDevice) return;
		audio.setSoundGain(soundId, volume);
	}

	public void SetLooping(long soundId, bool looping)
	{
		if (audio.noDevice) return;
		audio.setSoundLooping(soundId, looping);
	}

	public void SetPan(long soundId, float pan, float volume)
	{
		if (audio.noDevice) return;
		audio.setSoundPan(soundId, pan, volume);
	}

	public long Play(float volume, float pitch, float pan)
	{
		long id = Play();
		SetPitch(id, pitch);
		SetPan(id, pan, volume);
		return id;
	}

	public long Loop(float volume, float pitch, float pan)
	{
		long id = Loop();
		SetPitch(id, pitch);
		SetPan(id, pan, volume);
		return id;
	}

	/** Returns the length of the sound in seconds. */
	public float duration()
	{
		return _duration;
	}

	/** returns the original sample rate of the sound in Hz. */
	public int getRate()
	{
		return sampleRate;
	}

	/** returns the number of channels of the sound (1 for mono, 2 for stereo). */
	public int getChannels()
	{
		return channels;
	}
}
}
