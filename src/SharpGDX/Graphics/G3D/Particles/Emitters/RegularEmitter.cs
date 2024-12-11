using static SharpGDX.Graphics.G3D.Particles.ParallelArray;
using static SharpGDX.Graphics.G2D.ParticleEmitter;
using SharpGDX.Utils;

namespace SharpGDX.Graphics.G3D.Particles.Emitters;

/** It's a generic use {@link Emitter} which fits most of the particles simulation scenarios.
 * @author Inferno */
public class RegularEmitter : Emitter , Json.Serializable {

	/** Possible emission modes. Emission mode does not affect already emitted particles. */
	public enum EmissionMode {
		/** New particles can be emitted. */
		Enabled,
		/** Only valid for continuous emitters. It will only emit particles until the end of the effect duration. After that
		 * emission cycle will not be restarted. */
		EnabledUntilCycleEnd,
		/** Prevents new particle emission. */
		Disabled
	}

	public RangedNumericValue delayValue, durationValue;
	public ScaledNumericValue lifeOffsetValue, lifeValue, emissionValue;
	protected int emission, emissionDiff, emissionDelta;
	protected int lifeOffset, lifeOffsetDiff;
	protected int life, lifeDiff;
	protected float duration, delay, durationTimer, delayTimer;
	private bool continuous;
	private EmissionMode emissionMode;

	private FloatChannel lifeChannel;

	public RegularEmitter () {
		delayValue = new RangedNumericValue();
		durationValue = new RangedNumericValue();
		lifeOffsetValue = new ScaledNumericValue();
		lifeValue = new ScaledNumericValue();
		emissionValue = new ScaledNumericValue();

		durationValue.setActive(true);
		emissionValue.setActive(true);
		lifeValue.setActive(true);
		continuous = true;
		emissionMode = EmissionMode.Enabled;
	}

	public RegularEmitter (RegularEmitter regularEmitter) 
    : this()
    {
		
		set(regularEmitter);
	}

	public void allocateChannels () {
		lifeChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Life);
	}

	public void start () {
		delay = delayValue.active ? delayValue.newLowValue() : 0;
		delayTimer = 0;
		durationTimer = 0f;

		duration = durationValue.newLowValue();
		percent = durationTimer / (float)duration;

		emission = (int)emissionValue.newLowValue();
		emissionDiff = (int)emissionValue.newHighValue();
		if (!emissionValue.isRelative()) emissionDiff -= emission;

		life = (int)lifeValue.newLowValue();
		lifeDiff = (int)lifeValue.newHighValue();
		if (!lifeValue.isRelative()) lifeDiff -= life;

		lifeOffset = lifeOffsetValue.active ? (int)lifeOffsetValue.newLowValue() : 0;
		lifeOffsetDiff = (int)lifeOffsetValue.newHighValue();
		if (!lifeOffsetValue.isRelative()) lifeOffsetDiff -= lifeOffset;
	}

	public override void init () {
		base.init();
		emissionDelta = 0;
		durationTimer = duration;
	}

	public void activateParticles (int startIndex, int count) {
		int currentTotaLife = life + (int)(lifeDiff * lifeValue.getScale(percent)), currentLife = currentTotaLife;
		int offsetTime = (int)(lifeOffset + lifeOffsetDiff * lifeOffsetValue.getScale(percent));
		if (offsetTime > 0) {
			if (offsetTime >= currentLife) offsetTime = currentLife - 1;
			currentLife -= offsetTime;
		}
		float lifePercent = 1 - currentLife / (float)currentTotaLife;

		for (int i = startIndex * lifeChannel.strideSize,
			c = i + count * lifeChannel.strideSize; i < c; i += lifeChannel.strideSize) {
			lifeChannel.data[i + ParticleChannels.CurrentLifeOffset] = currentLife;
			lifeChannel.data[i + ParticleChannels.TotalLifeOffset] = currentTotaLife;
			lifeChannel.data[i + ParticleChannels.LifePercentOffset] = lifePercent;
		}
	}

	public void update () {
		float deltaMillis = controller.deltaTime * 1000;

		if (delayTimer < delay) {
			delayTimer += deltaMillis;
		} else {
			bool emit = emissionMode != EmissionMode.Disabled;
			// End check
			if (durationTimer < duration) {
				durationTimer += deltaMillis;
				percent = durationTimer / (float)duration;
			} else {
				if (continuous && emit && emissionMode == EmissionMode.Enabled)
					controller.start();
				else
					emit = false;
			}

			if (emit) {
				// TODO: The casts to int below had to be added. Should they be applied as floats and then the entire thing cast to an int? -RP
				// Emit particles
				emissionDelta += (int)deltaMillis;
				float emissionTime = emission + emissionDiff * emissionValue.getScale(percent);
				if (emissionTime > 0) {
					emissionTime = 1000 / emissionTime;
					if (emissionDelta >= emissionTime) {
						int emitCount = (int)(emissionDelta / emissionTime);
						emitCount = Math.Min(emitCount, maxParticleCount - controller.particles.size);
						emissionDelta -= (int)(emitCount * emissionTime);
						emissionDelta %= (int)emissionTime;
						addParticles(emitCount);
					}
				}
				if (controller.particles.size < minParticleCount) addParticles(minParticleCount - controller.particles.size);
			}
		}

		// Update particles
		int activeParticles = controller.particles.size;
		for (int i = 0, k = 0; i < controller.particles.size;) {
			if ((lifeChannel.data[k + ParticleChannels.CurrentLifeOffset] -= deltaMillis) <= 0) {
				controller.particles.removeElement(i);
				continue;
			} else {
				lifeChannel.data[k + ParticleChannels.LifePercentOffset] = 1
					- lifeChannel.data[k + ParticleChannels.CurrentLifeOffset]
						/ lifeChannel.data[k + ParticleChannels.TotalLifeOffset];
			}
			++i;
			k += lifeChannel.strideSize;
		}

		if (controller.particles.size < activeParticles) {
			controller.killParticles(controller.particles.size, activeParticles - controller.particles.size);
		}
	}

	private void addParticles (int count) {
		count = Math.Min(count, maxParticleCount - controller.particles.size);
		if (count <= 0) return;
		controller.activateParticles(controller.particles.size, count);
		controller.particles.size += count;
	}

	public ScaledNumericValue getLife () {
		return lifeValue;
	}

	public ScaledNumericValue getEmission () {
		return emissionValue;
	}

	public RangedNumericValue getDuration () {
		return durationValue;
	}

	public RangedNumericValue getDelay () {
		return delayValue;
	}

	public ScaledNumericValue getLifeOffset () {
		return lifeOffsetValue;
	}

	public bool isContinuous () {
		return continuous;
	}

	public void setContinuous (bool continuous) {
		this.continuous = continuous;
	}

	/** Gets current emission mode.
	 * @return Current emission mode. */
	public EmissionMode getEmissionMode () {
		return emissionMode;
	}

	/** Sets emission mode. Emission mode does not affect already emitted particles.
	 * @param emissionMode Emission mode to set. */
	public void setEmissionMode (EmissionMode emissionMode) {
		this.emissionMode = emissionMode;
	}

	public override bool isComplete () {
		if (delayTimer < delay) return false;
		return durationTimer >= duration && controller.particles.size == 0;
	}

	public float getPercentComplete () {
		if (delayTimer < delay) return 0;
		return Math.Min(1, durationTimer / (float)duration);
	}

	public void set (RegularEmitter emitter) {
		base.set(emitter);
		delayValue.load(emitter.delayValue);
		durationValue.load(emitter.durationValue);
		lifeOffsetValue.load(emitter.lifeOffsetValue);
		lifeValue.load(emitter.lifeValue);
		emissionValue.load(emitter.emissionValue);
		emission = emitter.emission;
		emissionDiff = emitter.emissionDiff;
		emissionDelta = emitter.emissionDelta;
		lifeOffset = emitter.lifeOffset;
		lifeOffsetDiff = emitter.lifeOffsetDiff;
		life = emitter.life;
		lifeDiff = emitter.lifeDiff;
		duration = emitter.duration;
		delay = emitter.delay;
		durationTimer = emitter.durationTimer;
		delayTimer = emitter.delayTimer;
		continuous = emitter.continuous;
	}

	public override ParticleControllerComponent copy () {
		return new RegularEmitter(this);
	}

	public override void write (Json json) {
		base.write(json);
		json.writeValue("continous", continuous);
		json.writeValue("emission", emissionValue);
		json.writeValue("delay", delayValue);
		json.writeValue("duration", durationValue);
		json.writeValue("life", lifeValue);
		json.writeValue("lifeOffset", lifeOffsetValue);
	}

	public override void read (Json json, JsonValue jsonData) {
		base.read(json, jsonData);
		continuous = (bool)json.readValue("continous", typeof(bool), jsonData);
		emissionValue = (ScaledNumericValue)json.readValue("emission", typeof(ScaledNumericValue), jsonData);
		delayValue = (RangedNumericValue)json.readValue("delay", typeof(RangedNumericValue), jsonData);
		durationValue = (RangedNumericValue)json.readValue("duration", typeof(RangedNumericValue), jsonData);
		lifeValue = (ScaledNumericValue)json.readValue("life", typeof(ScaledNumericValue), jsonData);
		lifeOffsetValue = (ScaledNumericValue)json.readValue("lifeOffset", typeof(ScaledNumericValue), jsonData);
	}
}
