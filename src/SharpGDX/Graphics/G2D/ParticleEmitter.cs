using System;
using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Mathematics.Collision;
using SharpGDX.Shims;
using SharpGDX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Graphics.G2D
{
	public class ParticleEmitter {
	static private readonly int UPDATE_SCALE = 1 << 0;
	static private readonly int UPDATE_ANGLE = 1 << 1;
	static private readonly int UPDATE_ROTATION = 1 << 2;
	static private readonly int UPDATE_VELOCITY = 1 << 3;
	static private readonly int UPDATE_WIND = 1 << 4;
	static private readonly int UPDATE_GRAVITY = 1 << 5;
	static private readonly int UPDATE_TINT = 1 << 6;
	static private readonly int UPDATE_SPRITE = 1 << 7;

	private RangedNumericValue delayValue = new RangedNumericValue();
	private IndependentScaledNumericValue lifeOffsetValue = new IndependentScaledNumericValue();
	private RangedNumericValue durationValue = new RangedNumericValue();
	private IndependentScaledNumericValue lifeValue = new IndependentScaledNumericValue();
	private ScaledNumericValue emissionValue = new ScaledNumericValue();
	private ScaledNumericValue xScaleValue = new ScaledNumericValue();
	private ScaledNumericValue yScaleValue = new ScaledNumericValue();
	private ScaledNumericValue rotationValue = new ScaledNumericValue();
	private ScaledNumericValue velocityValue = new ScaledNumericValue();
	private ScaledNumericValue angleValue = new ScaledNumericValue();
	private ScaledNumericValue windValue = new ScaledNumericValue();
	private ScaledNumericValue gravityValue = new ScaledNumericValue();
	private ScaledNumericValue transparencyValue = new ScaledNumericValue();
	private GradientColorValue tintValue = new GradientColorValue();
	private RangedNumericValue xOffsetValue = new ScaledNumericValue();
	private RangedNumericValue yOffsetValue = new ScaledNumericValue();
	private ScaledNumericValue spawnWidthValue = new ScaledNumericValue();
	private ScaledNumericValue spawnHeightValue = new ScaledNumericValue();
	private SpawnShapeValue spawnShapeValue = new SpawnShapeValue();

	private RangedNumericValue[] xSizeValues;
	private RangedNumericValue[] ySizeValues;
	private RangedNumericValue[] motionValues;

	private float accumulator;
	private Array<Sprite> sprites;
	private SpriteMode spriteMode = SpriteMode.single;
	private Particle[] particles;
	private int minParticleCount, maxParticleCount = 4;
	private float x, y;
	private String name;
	private Array<String> imagePaths;
	private int activeCount;
	private bool[] active;
	private bool firstUpdate;
	private bool flipX, _flipY;
	private int updateFlags;
	private bool _allowCompletion;
	private BoundingBox bounds;

	private int emission, emissionDiff, emissionDelta;
	private int lifeOffset, lifeOffsetDiff;
	private int life, lifeDiff;
	private float spawnWidth, spawnWidthDiff;
	private float spawnHeight, spawnHeightDiff;
	public float duration = 1, durationTimer;
	private float delay, delayTimer;

	private bool attached;
	private bool continuous;
	private bool aligned;
	private bool behind;
	private bool additive = true;
	private bool premultipliedAlpha = false;
	bool _cleansUpBlendFunction = true;

	public ParticleEmitter () {
		initialize();
	}

	public ParticleEmitter (BufferedReader reader) // TODO: throws IOException 
		{
		initialize();
		load(reader);
	}

	public ParticleEmitter (ParticleEmitter emitter) {
		sprites = new Array<Sprite>(emitter.sprites);
		name = emitter.name;
		imagePaths = new Array<String>(emitter.imagePaths);
		setMaxParticleCount(emitter.maxParticleCount);
		minParticleCount = emitter.minParticleCount;
		delayValue.load(emitter.delayValue);
		durationValue.load(emitter.durationValue);
		emissionValue.load(emitter.emissionValue);
		lifeValue.load(emitter.lifeValue);
		lifeOffsetValue.load(emitter.lifeOffsetValue);
		xScaleValue.load(emitter.xScaleValue);
		yScaleValue.load(emitter.yScaleValue);
		rotationValue.load(emitter.rotationValue);
		velocityValue.load(emitter.velocityValue);
		angleValue.load(emitter.angleValue);
		windValue.load(emitter.windValue);
		gravityValue.load(emitter.gravityValue);
		transparencyValue.load(emitter.transparencyValue);
		tintValue.load(emitter.tintValue);
		xOffsetValue.load(emitter.xOffsetValue);
		yOffsetValue.load(emitter.yOffsetValue);
		spawnWidthValue.load(emitter.spawnWidthValue);
		spawnHeightValue.load(emitter.spawnHeightValue);
		spawnShapeValue.load(emitter.spawnShapeValue);
		attached = emitter.attached;
		continuous = emitter.continuous;
		aligned = emitter.aligned;
		behind = emitter.behind;
		additive = emitter.additive;
		premultipliedAlpha = emitter.premultipliedAlpha;
		_cleansUpBlendFunction = emitter._cleansUpBlendFunction;
		spriteMode = emitter.spriteMode;
		setPosition(emitter.getX(), emitter.getY());
	}

	private void initialize () {
		sprites = new Array<Sprite>();
		imagePaths = new Array<String>();
		durationValue.setAlwaysActive(true);
		emissionValue.setAlwaysActive(true);
		lifeValue.setAlwaysActive(true);
		xScaleValue.setAlwaysActive(true);
		transparencyValue.setAlwaysActive(true);
		spawnShapeValue.setAlwaysActive(true);
		spawnWidthValue.setAlwaysActive(true);
		spawnHeightValue.setAlwaysActive(true);
	}

	public void setMaxParticleCount (int maxParticleCount) {
		this.maxParticleCount = maxParticleCount;
		active = new bool[maxParticleCount];
		activeCount = 0;
		particles = new Particle[maxParticleCount];
	}

	public void addParticle () {
		int activeCount = this.activeCount;
		if (activeCount == maxParticleCount) return;
		bool[] active = this.active;
		for (int i = 0, n = active.Length; i < n; i++) {
			if (!active[i]) {
				activateParticle(i);
				active[i] = true;
				this.activeCount = activeCount + 1;
				break;
			}
		}
	}

	public void addParticles (int count) {
		count = Math.Min(count, maxParticleCount - activeCount);
		if (count == 0) return;
		bool[] active = this.active;
		int index = 0, n = active.Length;
		outer:
		for (int i = 0; i < count; i++) {
			for (; index < n; index++) {
				if (!active[index]) {
					activateParticle(index);
					active[index++] = true;
					// TODO: Verify was continue outer;
					goto endOfOuter;
				}
			}
			break;
		}
		endOfOuter:
		this.activeCount += count;
	}

	public void update (float delta) {
		accumulator += delta * 1000;
		if (accumulator < 1) return;
		int deltaMillis = (int)accumulator;
		accumulator -= deltaMillis;

		if (delayTimer < delay) {
			delayTimer += deltaMillis;
		} else {
			bool done = false;
			if (firstUpdate) {
				firstUpdate = false;
				addParticle();
			}

			if (durationTimer < duration)
				durationTimer += deltaMillis;
			else {
				if (!continuous || _allowCompletion)
					done = true;
				else
					restart();
			}

			if (!done) {
				emissionDelta += deltaMillis;
				float emissionTime = emission + emissionDiff * emissionValue.getScale(durationTimer / (float)duration);
				if (emissionTime > 0) {
					emissionTime = 1000 / emissionTime;
					if (emissionDelta >= emissionTime) {
						int emitCount = (int)(emissionDelta / emissionTime);
						emitCount = Math.Min(emitCount, maxParticleCount - this.activeCount);
						emissionDelta = (int)(emissionDelta - emitCount * emissionTime);
							// TODO: Verify, was originally emissionDelta %= emissionTime;
							emissionDelta = (int)(emissionDelta % emissionTime);
						addParticles(emitCount);
					}
				}
				if (this.activeCount < minParticleCount) addParticles(minParticleCount - this.activeCount);
			}
		}

		bool[] active = this.active;
		int activeCount = this.activeCount;
		Particle[] particles = this.particles;
		for (int i = 0, n = active.Length; i < n; i++) {
			if (active[i] && !updateParticle(particles[i], delta, deltaMillis)) {
				active[i] = false;
				activeCount--;
			}
		}
		this.activeCount = activeCount;
	}

	public void draw (IBatch batch) {
		if (premultipliedAlpha) {
			batch.setBlendFunction(GL20.GL_ONE, GL20.GL_ONE_MINUS_SRC_ALPHA);
		} else if (additive) {
			batch.setBlendFunction(GL20.GL_SRC_ALPHA, GL20.GL_ONE);
		} else {
			batch.setBlendFunction(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
		}
		Particle[] particles = this.particles;
		bool[] active = this.active;

		for (int i = 0, n = active.Length; i < n; i++) {
			if (active[i]) particles[i].draw(batch);
		}

		if (_cleansUpBlendFunction && (additive || premultipliedAlpha))
			batch.setBlendFunction(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);

	}

	/** Updates and draws the particles. This is slightly more efficient than calling {@link #update(float)} and
	 * {@link #draw(Batch)} separately. */
	public void draw (IBatch batch, float delta) {
		accumulator += delta * 1000;
		if (accumulator < 1) {
			draw(batch);
			return;
		}
		int deltaMillis = (int)accumulator;
		accumulator -= deltaMillis;

		if (premultipliedAlpha) {
			batch.setBlendFunction(GL20.GL_ONE, GL20.GL_ONE_MINUS_SRC_ALPHA);
		} else if (additive) {
			batch.setBlendFunction(GL20.GL_SRC_ALPHA, GL20.GL_ONE);
		} else {
			batch.setBlendFunction(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
		}

		Particle[] particles = this.particles;
		bool[] active = this.active;
		int activeCount = this.activeCount;
		for (int i = 0, n = active.Length; i < n; i++) {
			if (active[i]) {
				Particle particle = particles[i];
				if (updateParticle(particle, delta, deltaMillis))
					particle.draw(batch);
				else {
					active[i] = false;
					activeCount--;
				}
			}
		}
		this.activeCount = activeCount;

		if (_cleansUpBlendFunction && (additive || premultipliedAlpha))
			batch.setBlendFunction(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);

		if (delayTimer < delay) {
			delayTimer += deltaMillis;
			return;
		}

		if (firstUpdate) {
			firstUpdate = false;
			addParticle();
		}

		if (durationTimer < duration)
			durationTimer += deltaMillis;
		else {
			if (!continuous || _allowCompletion) return;
			restart();
		}

		emissionDelta += deltaMillis;
		float emissionTime = emission + emissionDiff * emissionValue.getScale(durationTimer / (float)duration);
		if (emissionTime > 0) {
			emissionTime = 1000 / emissionTime;
			if (emissionDelta >= emissionTime) {
				int emitCount = (int)(emissionDelta / emissionTime);
				emitCount = Math.Min(emitCount, maxParticleCount - activeCount);
				emissionDelta = (int)(emissionDelta - emitCount * emissionTime);
					// TODO: Verify, was originally emissionDelta %= emissionTime;
					emissionDelta = (int)(emissionDelta % emissionTime);
				addParticles(emitCount);
			}
		}
		if (activeCount < minParticleCount) addParticles(minParticleCount - activeCount);
	}

	public void start () {
		firstUpdate = true;
		_allowCompletion = false;
		restart();
	}

	public void reset () {
		emissionDelta = 0;
		durationTimer = duration;
		bool[] active = this.active;
		for (int i = 0, n = active.Length; i < n; i++)
			active[i] = false;
		activeCount = 0;
		start();
	}

	private void restart () {
		delay = delayValue.active ? delayValue.newLowValue() : 0;
		delayTimer = 0;

		durationTimer -= duration;
		duration = durationValue.newLowValue();

		emission = (int)emissionValue.newLowValue();
		emissionDiff = (int)emissionValue.newHighValue();
		if (!emissionValue.isRelative()) emissionDiff -= emission;

		if (!lifeValue.independent) generateLifeValues();

		if (!lifeOffsetValue.independent) generateLifeOffsetValues();

		spawnWidth = spawnWidthValue.newLowValue();
		spawnWidthDiff = spawnWidthValue.newHighValue();
		if (!spawnWidthValue.isRelative()) spawnWidthDiff -= spawnWidth;

		spawnHeight = spawnHeightValue.newLowValue();
		spawnHeightDiff = spawnHeightValue.newHighValue();
		if (!spawnHeightValue.isRelative()) spawnHeightDiff -= spawnHeight;

		updateFlags = 0;
		if (angleValue.active && angleValue.timeline.Length > 1) updateFlags |= UPDATE_ANGLE;
		if (velocityValue.active) updateFlags |= UPDATE_VELOCITY;
		if (xScaleValue.timeline.Length > 1) updateFlags |= UPDATE_SCALE;
		if (yScaleValue.active && yScaleValue.timeline.Length > 1) updateFlags |= UPDATE_SCALE;
		if (rotationValue.active && rotationValue.timeline.Length > 1) updateFlags |= UPDATE_ROTATION;
		if (windValue.active) updateFlags |= UPDATE_WIND;
		if (gravityValue.active) updateFlags |= UPDATE_GRAVITY;
		if (tintValue.timeline.Length > 1) updateFlags |= UPDATE_TINT;
		if (spriteMode == SpriteMode.animated) updateFlags |= UPDATE_SPRITE;
	}

	protected Particle newParticle (Sprite sprite) {
		return new Particle(sprite);
	}

	protected Particle[] getParticles () {
		return particles;
	}

	private void activateParticle (int index) {
		Sprite sprite = null;
		switch (spriteMode) {
		case ParticleEmitter.SpriteMode.single:
		case SpriteMode.animated:
			sprite = sprites.first();
			break;
		case SpriteMode.random:
			sprite = sprites.random();
			break;
		}

		Particle particle = particles[index];
		if (particle == null) {
			particles[index] = particle = newParticle(sprite);
			particle.flip(flipX, _flipY);
		} else {
			particle.set(sprite);
		}

		float percent = durationTimer / (float)duration;
		int updateFlags = this.updateFlags;

		if (lifeValue.independent) generateLifeValues();

		if (lifeOffsetValue.independent) generateLifeOffsetValues();

		particle.currentLife = particle.life = life + (int)(lifeDiff * lifeValue.getScale(percent));

		if (velocityValue.active) {
			particle.velocity = velocityValue.newLowValue();
			particle.velocityDiff = velocityValue.newHighValue();
			if (!velocityValue.isRelative()) particle.velocityDiff -= particle.velocity;
		}

		particle.angle = angleValue.newLowValue();
		particle.angleDiff = angleValue.newHighValue();
		if (!angleValue.isRelative()) particle.angleDiff -= particle.angle;
		float angle = 0;
		if ((updateFlags & UPDATE_ANGLE) == 0) {
			angle = particle.angle + particle.angleDiff * angleValue.getScale(0);
			particle.angle = angle;
			particle.angleCos = MathUtils.cosDeg(angle);
			particle.angleSin = MathUtils.sinDeg(angle);
		}

		float spriteWidth = sprite.getWidth();
		float spriteHeight = sprite.getHeight();

		particle.xScale = xScaleValue.newLowValue() / spriteWidth;
		particle.xScaleDiff = xScaleValue.newHighValue() / spriteWidth;
		if (!xScaleValue.isRelative()) particle.xScaleDiff -= particle.xScale;

		if (yScaleValue.active) {
			particle.yScale = yScaleValue.newLowValue() / spriteHeight;
			particle.yScaleDiff = yScaleValue.newHighValue() / spriteHeight;
			if (!yScaleValue.isRelative()) particle.yScaleDiff -= particle.yScale;
			particle.setScale(particle.xScale + particle.xScaleDiff * xScaleValue.getScale(0),
				particle.yScale + particle.yScaleDiff * yScaleValue.getScale(0));
		} else {
			particle.setScale(particle.xScale + particle.xScaleDiff * xScaleValue.getScale(0));
		}

		if (rotationValue.active) {
			particle.rotation = rotationValue.newLowValue();
			particle.rotationDiff = rotationValue.newHighValue();
			if (!rotationValue.isRelative()) particle.rotationDiff -= particle.rotation;
			float rotation = particle.rotation + particle.rotationDiff * rotationValue.getScale(0);
			if (aligned) rotation += angle;
			particle.setRotation(rotation);
		}

		if (windValue.active) {
			particle.wind = windValue.newLowValue();
			particle.windDiff = windValue.newHighValue();
			if (!windValue.isRelative()) particle.windDiff -= particle.wind;
		}

		if (gravityValue.active) {
			particle.gravity = gravityValue.newLowValue();
			particle.gravityDiff = gravityValue.newHighValue();
			if (!gravityValue.isRelative()) particle.gravityDiff -= particle.gravity;
		}

		float[] color = particle.tint;
		if (color == null) particle.tint = color = new float[3];
		float[] temp = tintValue.getColor(0);
		color[0] = temp[0];
		color[1] = temp[1];
		color[2] = temp[2];

		particle.transparency = transparencyValue.newLowValue();
		particle.transparencyDiff = transparencyValue.newHighValue() - particle.transparency;

		// Spawn.
		float x = this.x;
		if (xOffsetValue.active) x += xOffsetValue.newLowValue();
		float y = this.y;
		if (yOffsetValue.active) y += yOffsetValue.newLowValue();
		switch (spawnShapeValue.shape) {
		case SpawnShape.square: {
			float width = spawnWidth + (spawnWidthDiff * spawnWidthValue.getScale(percent));
			float height = spawnHeight + (spawnHeightDiff * spawnHeightValue.getScale(percent));
			x += MathUtils.random(width) - width / 2;
			y += MathUtils.random(height) - height / 2;
			break;
		}
		case SpawnShape.ellipse: {
			float width = spawnWidth + (spawnWidthDiff * spawnWidthValue.getScale(percent));
			float height = spawnHeight + (spawnHeightDiff * spawnHeightValue.getScale(percent));
			float radiusX = width / 2;
			float radiusY = height / 2;
			if (radiusX == 0 || radiusY == 0) break;
			float scaleY = radiusX / (float)radiusY;
			if (spawnShapeValue.edges) {
				float spawnAngle;
				switch (spawnShapeValue.side) {
				case SpawnEllipseSide.top:
					spawnAngle = -MathUtils.random(179f);
					break;
				case SpawnEllipseSide.bottom:
					spawnAngle = MathUtils.random(179f);
					break;
				default:
					spawnAngle = MathUtils.random(360f);
					break;
				}
				float cosDeg = MathUtils.cosDeg(spawnAngle);
				float sinDeg = MathUtils.sinDeg(spawnAngle);
				x += cosDeg * radiusX;
				y += sinDeg * radiusX / scaleY;
				if ((updateFlags & UPDATE_ANGLE) == 0) {
					particle.angle = spawnAngle;
					particle.angleCos = cosDeg;
					particle.angleSin = sinDeg;
				}
			} else {
				float radius2 = radiusX * radiusX;
				while (true) {
					float px = MathUtils.random(width) - radiusX;
					float py = MathUtils.random(width) - radiusX;
					if (px * px + py * py <= radius2) {
						x += px;
						y += py / scaleY;
						break;
					}
				}
			}
			break;
		}
		case SpawnShape.line: {
			float width = spawnWidth + (spawnWidthDiff * spawnWidthValue.getScale(percent));
			float height = spawnHeight + (spawnHeightDiff * spawnHeightValue.getScale(percent));
			if (width != 0) {
				float lineX = width * MathUtils.random();
				x += lineX;
				y += lineX * (height / (float)width);
			} else
				y += height * MathUtils.random();
			break;
		}
		}

		particle.setBounds(x - spriteWidth / 2, y - spriteHeight / 2, spriteWidth, spriteHeight);

		int offsetTime = (int)(lifeOffset + lifeOffsetDiff * lifeOffsetValue.getScale(percent));
		if (offsetTime > 0) {
			if (offsetTime >= particle.currentLife) offsetTime = particle.currentLife - 1;
			updateParticle(particle, offsetTime / 1000f, offsetTime);
		}
	}

	private bool updateParticle (Particle particle, float delta, int deltaMillis) {
		int life = particle.currentLife - deltaMillis;
		if (life <= 0) return false;
		particle.currentLife = life;

		float percent = 1 - particle.currentLife / (float)particle.life;
		int updateFlags = this.updateFlags;

		if ((updateFlags & UPDATE_SCALE) != 0) {
			if (yScaleValue.active) {
				particle.setScale(particle.xScale + particle.xScaleDiff * xScaleValue.getScale(percent),
					particle.yScale + particle.yScaleDiff * yScaleValue.getScale(percent));
			} else {
				particle.setScale(particle.xScale + particle.xScaleDiff * xScaleValue.getScale(percent));
			}
		}

		if ((updateFlags & UPDATE_VELOCITY) != 0) {
			float velocity = (particle.velocity + particle.velocityDiff * velocityValue.getScale(percent)) * delta;

			float velocityX, velocityY;
			if ((updateFlags & UPDATE_ANGLE) != 0) {
				float angle = particle.angle + particle.angleDiff * angleValue.getScale(percent);
				velocityX = velocity * MathUtils.cosDeg(angle);
				velocityY = velocity * MathUtils.sinDeg(angle);
				if ((updateFlags & UPDATE_ROTATION) != 0) {
					float rotation = particle.rotation + particle.rotationDiff * rotationValue.getScale(percent);
					if (aligned) rotation += angle;
					particle.setRotation(rotation);
				}
			} else {
				velocityX = velocity * particle.angleCos;
				velocityY = velocity * particle.angleSin;
				if (aligned || (updateFlags & UPDATE_ROTATION) != 0) {
					float rotation = particle.rotation + particle.rotationDiff * rotationValue.getScale(percent);
					if (aligned) rotation += particle.angle;
					particle.setRotation(rotation);
				}
			}

			if ((updateFlags & UPDATE_WIND) != 0)
				velocityX += (particle.wind + particle.windDiff * windValue.getScale(percent)) * delta;

			if ((updateFlags & UPDATE_GRAVITY) != 0)
				velocityY += (particle.gravity + particle.gravityDiff * gravityValue.getScale(percent)) * delta;

			particle.translate(velocityX, velocityY);
		} else {
			if ((updateFlags & UPDATE_ROTATION) != 0)
				particle.setRotation(particle.rotation + particle.rotationDiff * rotationValue.getScale(percent));
		}

		float[] color;
		if ((updateFlags & UPDATE_TINT) != 0)
			color = tintValue.getColor(percent);
		else
			color = particle.tint;

		if (premultipliedAlpha) {
			float alphaMultiplier = additive ? 0 : 1;
			float a = particle.transparency + particle.transparencyDiff * transparencyValue.getScale(percent);
			particle.setColor(color[0] * a, color[1] * a, color[2] * a, a * alphaMultiplier);
		} else {
			particle.setColor(color[0], color[1], color[2],
				particle.transparency + particle.transparencyDiff * transparencyValue.getScale(percent));
		}

		if ((updateFlags & UPDATE_SPRITE) != 0) {
			int frame = Math.Min((int)(percent * sprites.size), sprites.size - 1);
			if (particle.frame != frame) {
				Sprite sprite = sprites.get(frame);
				float prevSpriteWidth = particle.getWidth();
				float prevSpriteHeight = particle.getHeight();
				particle.setRegion(sprite);
				particle.setSize(sprite.getWidth(), sprite.getHeight());
				particle.setOrigin(sprite.getOriginX(), sprite.getOriginY());
				particle.translate((prevSpriteWidth - sprite.getWidth()) / 2, (prevSpriteHeight - sprite.getHeight()) / 2);
				particle.frame = frame;
			}
		}

		return true;
	}

	private void generateLifeValues () {
		life = (int)lifeValue.newLowValue();
		lifeDiff = (int)lifeValue.newHighValue();
		if (!lifeValue.isRelative()) lifeDiff -= life;
	}

	private void generateLifeOffsetValues () {
		lifeOffset = lifeOffsetValue.active ? (int)lifeOffsetValue.newLowValue() : 0;
		lifeOffsetDiff = (int)lifeOffsetValue.newHighValue();
		if (!lifeOffsetValue.isRelative()) lifeOffsetDiff -= lifeOffset;
	}

	public void setPosition (float x, float y) {
		if (attached) {
			float xAmount = x - this.x;
			float yAmount = y - this.y;
			bool[] active = this.active;
			for (int i = 0, n = active.Length; i < n; i++)
				if (active[i]) particles[i].translate(xAmount, yAmount);
		}
		this.x = x;
		this.y = y;
	}

	public void setSprites (Array<Sprite> sprites) {
		this.sprites = sprites;
		if (sprites.size == 0) return;
		for (int i = 0, n = particles.Length; i < n; i++) {
			Particle particle = particles[i];
			if (particle == null) break;
			Sprite sprite = null;
			switch (spriteMode) {
			case SpriteMode.single:
				sprite = sprites.first();
				break;
			case SpriteMode.random:
				sprite = sprites.random();
				break;
			case SpriteMode.animated:
				float percent = 1 - particle.currentLife / (float)particle.life;
				particle.frame = Math.Min((int)(percent * sprites.size), sprites.size - 1);
				sprite = sprites.get(particle.frame);
				break;
			}
			particle.setRegion(sprite);
			particle.setOrigin(sprite.getOriginX(), sprite.getOriginY());
		}
	}

	public void setSpriteMode (SpriteMode spriteMode) {
		this.spriteMode = spriteMode;
	}

	/** Allocates max particles emitter can hold. Usually called early on to avoid allocation on updates.
	 * {@link #setSprites(Array)} must have been set before calling this method */
	public void preAllocateParticles () {
		if (sprites.isEmpty())
			throw new IllegalStateException("ParticleEmitter.setSprites() must have been called before preAllocateParticles()");
		for (int index = 0; index < particles.Length; index++) {
			Particle particle = particles[index];
			if (particle == null) {
				particles[index] = particle = newParticle(sprites.first());
				particle.flip(flipX, _flipY);
			}
		}
	}

	/** Ignores the {@link #setContinuous(bool) continuous} setting until the emitter is started again. This allows the emitter
	 * to stop smoothly. */
	public void allowCompletion () {
		_allowCompletion = true;
		durationTimer = duration;
	}

	public Array<Sprite> getSprites () {
		return sprites;
	}

	public SpriteMode getSpriteMode () {
		return spriteMode;
	}

	public String getName () {
		return name;
	}

	public void setName (String name) {
		this.name = name;
	}

	public ScaledNumericValue getLife () {
		return lifeValue;
	}

	public ScaledNumericValue getXScale () {
		return xScaleValue;
	}

	public ScaledNumericValue getYScale () {
		return yScaleValue;
	}

	public ScaledNumericValue getRotation () {
		return rotationValue;
	}

	public GradientColorValue getTint () {
		return tintValue;
	}

	public ScaledNumericValue getVelocity () {
		return velocityValue;
	}

	public ScaledNumericValue getWind () {
		return windValue;
	}

	public ScaledNumericValue getGravity () {
		return gravityValue;
	}

	public ScaledNumericValue getAngle () {
		return angleValue;
	}

	public ScaledNumericValue getEmission () {
		return emissionValue;
	}

	public ScaledNumericValue getTransparency () {
		return transparencyValue;
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

	public RangedNumericValue getXOffsetValue () {
		return xOffsetValue;
	}

	public RangedNumericValue getYOffsetValue () {
		return yOffsetValue;
	}

	public ScaledNumericValue getSpawnWidth () {
		return spawnWidthValue;
	}

	public ScaledNumericValue getSpawnHeight () {
		return spawnHeightValue;
	}

	public SpawnShapeValue getSpawnShape () {
		return spawnShapeValue;
	}

	public bool isAttached () {
		return attached;
	}

	public void setAttached (bool attached) {
		this.attached = attached;
	}

	public bool isContinuous () {
		return continuous;
	}

	public void setContinuous (bool continuous) {
		this.continuous = continuous;
	}

	public bool isAligned () {
		return aligned;
	}

	public void setAligned (bool aligned) {
		this.aligned = aligned;
	}

	public bool isAdditive () {
		return additive;
	}

	public void setAdditive (bool additive) {
		this.additive = additive;
	}

	/** @return Whether this ParticleEmitter automatically returns the {@link com.badlogic.gdx.graphics.g2d.Batch Batch}'s blend
	 *         function to the alpha-blending default (GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA) when done drawing. */
	public bool cleansUpBlendFunction () {
		return _cleansUpBlendFunction;
	}

	/** Set whether to automatically return the {@link com.badlogic.gdx.graphics.g2d.Batch Batch}'s blend function to the
	 * alpha-blending default (GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA) when done drawing. Is true by default. If set to false, the
	 * Batch's blend function is left as it was for drawing this ParticleEmitter, which prevents the Batch from being flushed
	 * repeatedly if consecutive ParticleEmitters with the same additive or pre-multiplied alpha state are drawn in a row.
	 * <p>
	 * IMPORTANT: If set to false and if the next object to use this Batch expects alpha blending, you are responsible for setting
	 * the Batch's blend function to (GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA) before that next object is drawn.
	 * @param cleansUpBlendFunction */
	public void setCleansUpBlendFunction (bool cleansUpBlendFunction) {
		this._cleansUpBlendFunction = cleansUpBlendFunction;
	}

	public bool isBehind () {
		return behind;
	}

	public void setBehind (bool behind) {
		this.behind = behind;
	}

	public bool isPremultipliedAlpha () {
		return premultipliedAlpha;
	}

	public void setPremultipliedAlpha (bool premultipliedAlpha) {
		this.premultipliedAlpha = premultipliedAlpha;
	}

	public int getMinParticleCount () {
		return minParticleCount;
	}

	public void setMinParticleCount (int minParticleCount) {
		this.minParticleCount = minParticleCount;
	}

	public int getMaxParticleCount () {
		return maxParticleCount;
	}

	public bool isComplete () {
		if (continuous && !_allowCompletion) return false;
		if (delayTimer < delay) return false;
		return durationTimer >= duration && activeCount == 0;
	}

	public float getPercentComplete () {
		if (delayTimer < delay) return 0;
		return Math.Min(1, durationTimer / (float)duration);
	}

	public float getX () {
		return x;
	}

	public float getY () {
		return y;
	}

	public int getActiveCount () {
		return activeCount;
	}

	public Array<String> getImagePaths () {
		return imagePaths;
	}

	public void setImagePaths (Array<String> imagePaths) {
		this.imagePaths = imagePaths;
	}

	public void setFlip (bool flipX, bool flipY) {
		this.flipX = flipX;
		this._flipY = flipY;
		if (particles == null) return;
		for (int i = 0, n = particles.Length; i < n; i++) {
			Particle particle = particles[i];
			if (particle != null) particle.flip(flipX, flipY);
		}
	}

	public void flipY () {
		angleValue.setHigh(-angleValue.getHighMin(), -angleValue.getHighMax());
		angleValue.setLow(-angleValue.getLowMin(), -angleValue.getLowMax());

		gravityValue.setHigh(-gravityValue.getHighMin(), -gravityValue.getHighMax());
		gravityValue.setLow(-gravityValue.getLowMin(), -gravityValue.getLowMax());

		windValue.setHigh(-windValue.getHighMin(), -windValue.getHighMax());
		windValue.setLow(-windValue.getLowMin(), -windValue.getLowMax());

		rotationValue.setHigh(-rotationValue.getHighMin(), -rotationValue.getHighMax());
		rotationValue.setLow(-rotationValue.getLowMin(), -rotationValue.getLowMax());

		yOffsetValue.setLow(-yOffsetValue.getLowMin(), -yOffsetValue.getLowMax());
	}

	/** Returns the bounding box for all active particles. z axis will always be zero. */
	public BoundingBox getBoundingBox () {
		if (this.bounds == null) this.bounds = new BoundingBox();

		Particle[] particles = this.particles;
		bool[] active = this.active;
		BoundingBox bounds = this.bounds;

		bounds.inf();
		for (int i = 0, n = active.Length; i < n; i++)
			if (active[i]) {
				Rectangle r = particles[i].getBoundingRectangle();
				bounds.ext(r.x, r.y, 0);
				bounds.ext(r.x + r.width, r.y + r.height, 0);
			}

		return bounds;
	}

	protected RangedNumericValue[] getXSizeValues () {
		if (xSizeValues == null) {
			xSizeValues = new RangedNumericValue[3];
			xSizeValues[0] = xScaleValue;
			xSizeValues[1] = spawnWidthValue;
			xSizeValues[2] = xOffsetValue;
		}
		return xSizeValues;
	}

	protected RangedNumericValue[] getYSizeValues () {
		if (ySizeValues == null) {
			ySizeValues = new RangedNumericValue[3];
			ySizeValues[0] = yScaleValue;
			ySizeValues[1] = spawnHeightValue;
			ySizeValues[2] = yOffsetValue;
		}
		return ySizeValues;
	}

	protected RangedNumericValue[] getMotionValues () {
		if (motionValues == null) {
			motionValues = new RangedNumericValue[3];
			motionValues[0] = velocityValue;
			motionValues[1] = windValue;
			motionValues[2] = gravityValue;
		}
		return motionValues;
	}

	/** Permanently scales the size of the emitter by scaling all its ranged values related to size. */
	public void scaleSize (float scale) {
		if (scale == 1f) return;
		scaleSize(scale, scale);
	}

	/** Permanently scales the size of the emitter by scaling all its ranged values related to size. */
	public void scaleSize (float scaleX, float scaleY) {
		if (scaleX == 1f && scaleY == 1f) return;
		foreach (RangedNumericValue value in getXSizeValues())
			value.scale(scaleX);
			foreach (RangedNumericValue value in getYSizeValues())
			value.scale(scaleY);
	}

	/** Permanently scales the speed of the emitter by scaling all its ranged values related to motion. */
	public void scaleMotion (float scale) {
		if (scale == 1f) return;
		foreach (RangedNumericValue value in getMotionValues())
			value.scale(scale);
	}

	/** Sets all size-related ranged values to match those of the template emitter. */
	public void matchSize (ParticleEmitter template) {
		matchXSize(template);
		matchYSize(template);
	}

	/** Sets all horizontal size-related ranged values to match those of the template emitter. */
	public void matchXSize (ParticleEmitter template) {
		RangedNumericValue[] values = getXSizeValues();
		RangedNumericValue[] templateValues = template.getXSizeValues();
		for (int i = 0; i < values.Length; i++) {
			values[i].set(templateValues[i]);
		}
	}

	/** Sets all vertical size-related ranged values to match those of the template emitter. */
	public void matchYSize (ParticleEmitter template) {
		RangedNumericValue[] values = getYSizeValues();
		RangedNumericValue[] templateValues = template.getYSizeValues();
		for (int i = 0; i < values.Length; i++) {
			values[i].set(templateValues[i]);
		}
	}

	/** Sets all motion-related ranged values to match those of the template emitter. */
	public void matchMotion (ParticleEmitter template) {
		RangedNumericValue[] values = getMotionValues();
		RangedNumericValue[] templateValues = template.getMotionValues();
		for (int i = 0; i < values.Length; i++) {
			values[i].set(templateValues[i]);
		}
	}

	public void save (Writer output) // TODO: throws IOException
		{
		output.write(name + "\n");
		output.write("- Delay -\n");
		delayValue.save(output);
		output.write("- Duration - \n");
		durationValue.save(output);
		output.write("- Count - \n");
		output.write("min: " + minParticleCount + "\n");
		output.write("max: " + maxParticleCount + "\n");
		output.write("- Emission - \n");
		emissionValue.save(output);
		output.write("- Life - \n");
		lifeValue.save(output);
		output.write("- Life Offset - \n");
		lifeOffsetValue.save(output);
		output.write("- X Offset - \n");
		xOffsetValue.save(output);
		output.write("- Y Offset - \n");
		yOffsetValue.save(output);
		output.write("- Spawn Shape - \n");
		spawnShapeValue.save(output);
		output.write("- Spawn Width - \n");
		spawnWidthValue.save(output);
		output.write("- Spawn Height - \n");
		spawnHeightValue.save(output);
		output.write("- X Scale - \n");
		xScaleValue.save(output);
		output.write("- Y Scale - \n");
		yScaleValue.save(output);
		output.write("- Velocity - \n");
		velocityValue.save(output);
		output.write("- Angle - \n");
		angleValue.save(output);
		output.write("- Rotation - \n");
		rotationValue.save(output);
		output.write("- Wind - \n");
		windValue.save(output);
		output.write("- Gravity - \n");
		gravityValue.save(output);
		output.write("- Tint - \n");
		tintValue.save(output);
		output.write("- Transparency - \n");
		transparencyValue.save(output);
		output.write("- Options - \n");
		output.write("attached: " + attached + "\n");
		output.write("continuous: " + continuous + "\n");
		output.write("aligned: " + aligned + "\n");
		output.write("additive: " + additive + "\n");
		output.write("behind: " + behind + "\n");
		output.write("premultipliedAlpha: " + premultipliedAlpha + "\n");
		output.write("spriteMode: " + spriteMode.ToString() + "\n");
		output.write("- Image Paths -\n");
		foreach (String imagePath in imagePaths) {
			output.write(imagePath + "\n");
		}
		output.write("\n");
	}

	public void load (BufferedReader reader) // TODO: throws IOException
		{
		try {
			name = readString(reader, "name");
			reader.readLine();
			delayValue.load(reader);
			reader.readLine();
			durationValue.load(reader);
			reader.readLine();
			setMinParticleCount(readInt(reader, "minParticleCount"));
			setMaxParticleCount(readInt(reader, "maxParticleCount"));
			reader.readLine();
			emissionValue.load(reader);
			reader.readLine();
			lifeValue.load(reader);
			reader.readLine();
			lifeOffsetValue.load(reader);
			reader.readLine();
			xOffsetValue.load(reader);
			reader.readLine();
			yOffsetValue.load(reader);
			reader.readLine();
			spawnShapeValue.load(reader);
			reader.readLine();
			spawnWidthValue.load(reader);
			reader.readLine();
			spawnHeightValue.load(reader);
			String line = reader.readLine();
			if (line.Trim().Equals("- Scale -")) {
				xScaleValue.load(reader);
				yScaleValue.setActive(false);
			} else {
				xScaleValue.load(reader);
				reader.readLine();
				yScaleValue.load(reader);
			}
			reader.readLine();
			velocityValue.load(reader);
			reader.readLine();
			angleValue.load(reader);
			reader.readLine();
			rotationValue.load(reader);
			reader.readLine();
			windValue.load(reader);
			reader.readLine();
			gravityValue.load(reader);
			reader.readLine();
			tintValue.load(reader);
			reader.readLine();
			transparencyValue.load(reader);
			reader.readLine();
			attached = readBoolean(reader, "attached");
			continuous = readBoolean(reader, "continuous");
			aligned = readBoolean(reader, "aligned");
			additive = readBoolean(reader, "additive");
			behind = readBoolean(reader, "behind");

			// Backwards compatibility
			line = reader.readLine();
			if (line.StartsWith("premultipliedAlpha")) {
				premultipliedAlpha = readBoolean(line);
				line = reader.readLine();
			}
			if (line.StartsWith("spriteMode")) {
				spriteMode = Enum.Parse<SpriteMode>(readString(line));
				line = reader.readLine();
			}

			Array<String> imagePaths = new Array<String>();
			while ((line = reader.readLine()) != null && !string.IsNullOrEmpty(line)) {
				imagePaths.add(line);
			}
			setImagePaths(imagePaths);
		} catch (RuntimeException ex) {
			if (name == null) throw ex;
			throw new RuntimeException("Error parsing emitter: " + name, ex);
		}
	}

	static String readString (String line) // TODO: throws IOException 
		{
		return line.Substring(line.IndexOf(":") + 1).Trim();
	}

	static String readString (BufferedReader reader, String name) // TODO: throws IOException 
		{
		String line = reader.readLine();
		if (line == null) throw new IOException("Missing value: " + name);
		return readString(line);
	}

	static bool readBoolean (String line) // TODO: throws IOException 
		{
		return bool.Parse(readString(line));
	}

	static bool readBoolean (BufferedReader reader, String name) // TODO: throws IOException 
		{
		return bool.Parse(readString(reader, name));
	}

	static int readInt (BufferedReader reader, String name) // TODO: throws IOException 
		{
		return int.Parse(readString(reader, name));
	}

	static float readFloat (BufferedReader reader, String name) // TODO: throws IOException 
	{
		return float.Parse(readString(reader, name));
	}

	public class Particle : Sprite {
		internal int life, currentLife;
		internal float xScale, xScaleDiff;
		internal float yScale, yScaleDiff;
		internal float rotation, rotationDiff;
		internal float velocity, velocityDiff;
		internal float angle, angleDiff;
		internal float angleCos, angleSin;
		internal float transparency, transparencyDiff;
		internal float wind, windDiff;
		internal float gravity, gravityDiff;
		internal float[] tint;
		internal int frame;

		public Particle (Sprite sprite) 
		: base(sprite)
		{
			
		}
	}

	public class ParticleValue {
		internal bool active;
		protected bool alwaysActive;

		public void setAlwaysActive (bool alwaysActive) {
			this.alwaysActive = alwaysActive;
		}

		public bool isAlwaysActive () {
			return alwaysActive;
		}

		public bool isActive () {
			return alwaysActive || active;
		}

		public void setActive (bool active) {
			this.active = active;
		}

		public virtual void save (Writer output) // TODO: throws IOException 
		{
			if (!alwaysActive)
				output.write("active: " + active + "\n");
			else
				active = true;
		}

		public virtual void load (BufferedReader reader) // TODO: throws IOException
			{
			if (!alwaysActive)
				active = readBoolean(reader, "active");
			else
				active = true;
		}

		public void load (ParticleValue value) {
			active = value.active;
			alwaysActive = value.alwaysActive;
		}
	}

	public class NumericValue : ParticleValue {
		private float value;

		public float getValue () {
			return value;
		}

		public void setValue (float value) {
			this.value = value;
		}

			public override void save (Writer output) // TODO: throws IOException 
	{
			base.save(output);
			if (!active) return;
			output.write("value: " + value + "\n");
		}

			public override void load (BufferedReader reader) // TODO: throws IOException 
{
			base.load(reader);
			if (!active) return;
			value = readFloat(reader, "value");
		}

		public void load (NumericValue value) {
			base.load(value);
			this.value = value.value;
		}
	}

	public class RangedNumericValue : ParticleValue {
		private float lowMin, lowMax;

		public float newLowValue () {
			return lowMin + (lowMax - lowMin) * MathUtils.random();
		}

		public void setLow (float value) {
			lowMin = value;
			lowMax = value;
		}

		public void setLow (float min, float max) {
			lowMin = min;
			lowMax = max;
		}

		public float getLowMin () {
			return lowMin;
		}

		public void setLowMin (float lowMin) {
			this.lowMin = lowMin;
		}

		public float getLowMax () {
			return lowMax;
		}

		public void setLowMax (float lowMax) {
			this.lowMax = lowMax;
		}

		/** permanently scales the range by a scalar. */
		public virtual void scale (float scale) {
			lowMin *= scale;
			lowMax *= scale;
		}

		public virtual void set (RangedNumericValue value) {
			this.lowMin = value.lowMin;
			this.lowMax = value.lowMax;
		}

			public override void save (Writer output) // TODO: throws IOException 
	{
			base.save(output);
			if (!active) return;
			output.write("lowMin: " + lowMin + "\n");
			output.write("lowMax: " + lowMax + "\n");
		}

			public override void load (BufferedReader reader) // TODO: throws IOException 
	{
			base.load(reader);
			if (!active) return;
			lowMin = readFloat(reader, "lowMin");
			lowMax = readFloat(reader, "lowMax");
		}

		public void load (RangedNumericValue value) {
			base.load(value);
			lowMax = value.lowMax;
			lowMin = value.lowMin;
		}
	}

	public class ScaledNumericValue : RangedNumericValue {
		private float[] scaling = {1};
		internal float[] timeline = {0};
		private float highMin, highMax;
		private bool relative;

		public float newHighValue () {
			return highMin + (highMax - highMin) * MathUtils.random();
		}

		public void setHigh (float value) {
			highMin = value;
			highMax = value;
		}

		public void setHigh (float min, float max) {
			highMin = min;
			highMax = max;
		}

		public float getHighMin () {
			return highMin;
		}

		public void setHighMin (float highMin) {
			this.highMin = highMin;
		}

		public float getHighMax () {
			return highMax;
		}

		public void setHighMax (float highMax) {
			this.highMax = highMax;
		}

			public override void scale (float scale) {
			base.scale(scale);
			highMin *= scale;
			highMax *= scale;
		}

			public override void set (RangedNumericValue value) {
			if (value is ScaledNumericValue)
				set((ScaledNumericValue)value);
			else
				base.set(value);
		}

			public virtual void set (ScaledNumericValue value) {
			base.set(value);
			this.highMin = value.highMin;
			this.highMax = value.highMax;
			if (scaling.Length != value.scaling.Length)
			{
				scaling = new float[value.scaling.Length];
				Array.Copy(value.scaling, scaling, value.scaling.Length);
			}
			else
				Array.Copy(value.scaling, 0, scaling, 0, scaling.Length);

			if (timeline.Length != value.timeline.Length)
			{
				timeline = new float[value.timeline.Length];
				Array.Copy(value.timeline, timeline, value.timeline.Length);
			}
			else
				Array.Copy(value.timeline, 0, timeline, 0, timeline.Length);
			this.relative = value.relative;
		}

		public float[] getScaling () {
			return scaling;
		}

		public void setScaling (float[] values) {
			this.scaling = values;
		}

		public float[] getTimeline () {
			return timeline;
		}

		public void setTimeline (float[] timeline) {
			this.timeline = timeline;
		}

		public bool isRelative () {
			return relative;
		}

		public void setRelative (bool relative) {
			this.relative = relative;
		}

		public float getScale (float percent) {
			int endIndex = -1;
			float[] timeline = this.timeline;
			int n = timeline.Length;
			for (int i = 1; i < n; i++) {
				float t = timeline[i];
				if (t > percent) {
					endIndex = i;
					break;
				}
			}
			if (endIndex == -1) return this.scaling[n - 1];
			float[] scaling = this.scaling;
			int startIndex = endIndex - 1;
			float startValue = scaling[startIndex];
			float startTime = timeline[startIndex];
			return startValue + (scaling[endIndex] - startValue) * ((percent - startTime) / (timeline[endIndex] - startTime));
		}

			public override void save (Writer output) // TODO: throws IOException 
	{
			base.save(output);
			if (!active) return;
			output.write("highMin: " + highMin + "\n");
			output.write("highMax: " + highMax + "\n");
			output.write("relative: " + relative + "\n");
			output.write("scalingCount: " + scaling.Length + "\n");
			for (int i = 0; i < scaling.Length; i++)
				output.write("scaling" + i + ": " + scaling[i] + "\n");
			output.write("timelineCount: " + timeline.Length + "\n");
			for (int i = 0; i < timeline.Length; i++)
				output.write("timeline" + i + ": " + timeline[i] + "\n");
		}

			public override void load (BufferedReader reader) // TODO: throws IOException 
	{
			base.load(reader);
			if (!active) return;
			highMin = readFloat(reader, "highMin");
			highMax = readFloat(reader, "highMax");
			relative = readBoolean(reader, "relative");
			scaling = new float[readInt(reader, "scalingCount")];
			for (int i = 0; i < scaling.Length; i++)
				scaling[i] = readFloat(reader, "scaling" + i);
			timeline = new float[readInt(reader, "timelineCount")];
			for (int i = 0; i < timeline.Length; i++)
				timeline[i] = readFloat(reader, "timeline" + i);
		}

		public void load (ScaledNumericValue value) {
			base.load(value);
			highMax = value.highMax;
			highMin = value.highMin;
			scaling = new float[value.scaling.Length];
			Array.Copy(value.scaling, 0, scaling, 0, scaling.Length);
			timeline = new float[value.timeline.Length];
			Array.Copy(value.timeline, 0, timeline, 0, timeline.Length);
			relative = value.relative;
		}
	}

	public class IndependentScaledNumericValue : ScaledNumericValue {
		internal bool independent;

		public bool isIndependent () {
			return independent;
		}

		public void setIndependent (bool independent) {
			this.independent = independent;
		}

			public override void set (RangedNumericValue value) {
			if (value is IndependentScaledNumericValue)
				set((IndependentScaledNumericValue)value);
			else
				base.set(value);
		}

			public override void set (ScaledNumericValue value) {
			if (value is IndependentScaledNumericValue)
				set((IndependentScaledNumericValue)value);
			else
				base.set(value);
		}

		public void set (IndependentScaledNumericValue value) {
			base.set(value);
			independent = value.independent;
		}

			public override void save (Writer output) // TODO: throws IOException 
	{
			base.save(output);
			output.write("independent: " + independent + "\n");
		}

			public override void load (BufferedReader reader) // TODO: throws IOException
{
			base.load(reader);
			// For backwards compatibility, independent property may not be defined
			if (reader.markSupported()) reader.mark(100);
			String line = reader.readLine();
			if (line == null) throw new IOException("Missing value: independent");
			if (line.Contains("independent"))
				independent = bool.Parse(readString(line));
			else if (reader.markSupported())
				reader.reset();
			else {
				// @see java.io.BufferedReader#markSupported may return false in some platforms (such as GWT),
				// in that case backwards commpatibility is not possible
				String errorMessage = "The loaded particle effect descriptor file uses an old invalid format. "
					+ "Please download the latest version of the Particle Editor tool and recreate the file by"
					+ " loading and saving it again.";
				Gdx.app.error("ParticleEmitter", errorMessage);
				throw new IOException(errorMessage);
			}
		}

		public void load (IndependentScaledNumericValue value) {
			base.load(value);
			independent = value.independent;
		}
	}

	public class GradientColorValue : ParticleValue {
		static private float[] temp = new float[4];

		private float[] colors = {1, 1, 1};
		internal float[] timeline = {0};

		public GradientColorValue () {
			alwaysActive = true;
		}

		public float[] getTimeline () {
			return timeline;
		}

		public void setTimeline (float[] timeline) {
			this.timeline = timeline;
		}

		/** @return the r, g and b values for every timeline position */
		public float[] getColors () {
			return colors;
		}

		/** @param colors the r, g and b values for every timeline position */
		public void setColors (float[] colors) {
			this.colors = colors;
		}

		public float[] getColor (float percent) {
			int startIndex = 0, endIndex = -1;
			float[] timeline = this.timeline;
			int n = timeline.Length;
			for (int i = 1; i < n; i++) {
				float t = timeline[i];
				if (t > percent) {
					endIndex = i;
					break;
				}
				startIndex = i;
			}
			float startTime = timeline[startIndex];
			startIndex *= 3;
			float r1 = colors[startIndex];
			float g1 = colors[startIndex + 1];
			float b1 = colors[startIndex + 2];
			if (endIndex == -1) {
				temp[0] = r1;
				temp[1] = g1;
				temp[2] = b1;
				return temp;
			}
			float factor = (percent - startTime) / (timeline[endIndex] - startTime);
			endIndex *= 3;
			temp[0] = r1 + (colors[endIndex] - r1) * factor;
			temp[1] = g1 + (colors[endIndex + 1] - g1) * factor;
			temp[2] = b1 + (colors[endIndex + 2] - b1) * factor;
			return temp;
		}

			public override void save (Writer output) // TODO: throws IOException 
		{
			base.save(output);
			if (!active) return;
			output.write("colorsCount: " + colors.Length + "\n");
			for (int i = 0; i < colors.Length; i++)
				output.write("colors" + i + ": " + colors[i] + "\n");
			output.write("timelineCount: " + timeline.Length + "\n");
			for (int i = 0; i < timeline.Length; i++)
				output.write("timeline" + i + ": " + timeline[i] + "\n");
		}

			public override void load (BufferedReader reader) // TODO: throws IOException
			{
			base.load(reader);
			if (!active) return;
			colors = new float[readInt(reader, "colorsCount")];
			for (int i = 0; i < colors.Length; i++)
				colors[i] = readFloat(reader, "colors" + i);
			timeline = new float[readInt(reader, "timelineCount")];
			for (int i = 0; i < timeline.Length; i++)
				timeline[i] = readFloat(reader, "timeline" + i);
		}

		public void load (GradientColorValue value) {
			base.load(value);
			colors = new float[value.colors.Length];
			Array.Copy(value.colors, 0, colors, 0, colors.Length);
			timeline = new float[value.timeline.Length];
			Array.Copy(value.timeline, 0, timeline, 0, timeline.Length);
		}
	}

	 public class SpawnShapeValue : ParticleValue {
		internal SpawnShape shape = SpawnShape.point;
		internal bool edges;
		internal SpawnEllipseSide side = SpawnEllipseSide.both;

		public SpawnShape getShape () {
			return shape;
		}

		public void setShape (SpawnShape shape) {
			this.shape = shape;
		}

		public bool isEdges () {
			return edges;
		}

		public void setEdges (bool edges) {
			this.edges = edges;
		}

		public SpawnEllipseSide getSide () {
			return side;
		}

		public void setSide (SpawnEllipseSide side) {
			this.side = side;
		}

			public override void save (Writer output) // TODO: throws IOException 
			{
			base.save(output);
			if (!active) return;
			output.write("shape: " + shape + "\n");
			if (shape == SpawnShape.ellipse) {
				output.write("edges: " + edges + "\n");
				output.write("side: " + side + "\n");
			}
		}

			public override void load (BufferedReader reader) // TODO: throws IOException 
			{
			base.load(reader);
			if (!active) return;
			shape = Enum.Parse<SpawnShape>(readString(reader, "shape"));
			if (shape == SpawnShape.ellipse) {
				edges = readBoolean(reader, "edges");
				side = Enum.Parse<SpawnEllipseSide>(readString(reader, "side"));
			}
		}

		public void load (SpawnShapeValue value) {
			base.load(value);
			shape = value.shape;
			edges = value.edges;
			side = value.side;
		}
	}

	 public enum SpawnShape {
		point, line, square, ellipse
	}

	 public enum SpawnEllipseSide {
		both, top, bottom
	}

	 public enum SpriteMode {
		single, random, animated
	}
}
}
