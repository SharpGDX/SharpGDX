using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** ParticleEffectActor holds an {@link ParticleEffect} to use in Scene2d applications. The particle effect is positioned at 0, 0
 * in the ParticleEffectActor. Its bounding box is not limited to the size of this actor. */
public class ParticleEffectActor : Actor , Disposable {
	private readonly ParticleEffect particleEffect;
	protected float lastDelta;
	protected bool _isRunning;
	protected bool ownsEffect;
	private bool resetOnStart;
	private bool autoRemove;

	public ParticleEffectActor (ParticleEffect particleEffect, bool resetOnStart)
	{
		
		this.particleEffect = particleEffect;
		this.resetOnStart = resetOnStart;
	}

	public ParticleEffectActor (FileHandle particleFile, TextureAtlas atlas)
	{
		
		particleEffect = new ParticleEffect();
		particleEffect.load(particleFile, atlas);
		ownsEffect = true;
	}

	public ParticleEffectActor (FileHandle particleFile, FileHandle imagesDir)
	{
		
		particleEffect = new ParticleEffect();
		particleEffect.load(particleFile, imagesDir);
		ownsEffect = true;
	}

	public override void draw (IBatch batch, float parentAlpha) {
		particleEffect.setPosition(getX(), getY());
		if (lastDelta > 0) {
			particleEffect.update(lastDelta);
			lastDelta = 0;
		}
		if (_isRunning) {
			particleEffect.draw(batch);
			_isRunning = !particleEffect.isComplete();
		}
	}

	public override void act (float delta) {
		base.act(delta);
		// don't do particleEffect.update() here - the correct position is set just while we
		// are in draw() method. We save the delta here to update in draw()
		lastDelta += delta;

		if (autoRemove && particleEffect.isComplete()) {
			remove();
		}
	}

	public void start () {
		_isRunning = true;
		if (resetOnStart) {
			particleEffect.reset(false);
		}
		particleEffect.start();
	}

	public bool isResetOnStart () {
		return resetOnStart;
	}

	public ParticleEffectActor setResetOnStart (bool resetOnStart) {
		this.resetOnStart = resetOnStart;
		return this;
	}

	public bool isAutoRemove () {
		return autoRemove;
	}

	public ParticleEffectActor setAutoRemove (bool autoRemove) {
		this.autoRemove = autoRemove;
		return this;
	}

	public bool isRunning () {
		return _isRunning;
	}

	public ParticleEffect getEffect () {
		return this.particleEffect;
	}

	protected override void scaleChanged () {
		base.scaleChanged();
		particleEffect.scaleEffect(getScaleX(), getScaleY(), getScaleY());
	}

	public void cancel () {
		_isRunning = true;
	}

	public void allowCompletion () {
		particleEffect.allowCompletion();
	}

	public void dispose () {
		if (ownsEffect) {
			particleEffect.dispose();
		}
	}

}
