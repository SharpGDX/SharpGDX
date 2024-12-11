using System;
using SharpGDX.Assets;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G3D.Models.Data;
using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.G3D.Particles.Values;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Environments;
using FloatChannel = SharpGDX.Graphics.G3D.Particles.ParallelArray.FloatChannel;

namespace SharpGDX.Graphics.G3D.Particles.Influencers;

/** It's an {@link Influencer} which controls where the particles will be spawned.
 * @author Inferno */
public class SpawnInfluencer : Influencer {

	public SpawnShapeValue spawnShapeValue;
	FloatChannel positionChannel;
	FloatChannel rotationChannel;

	public SpawnInfluencer () {
		spawnShapeValue = new PointSpawnShapeValue();
	}

	public SpawnInfluencer (SpawnShapeValue spawnShapeValue) {
		this.spawnShapeValue = spawnShapeValue;
	}

	public SpawnInfluencer (SpawnInfluencer source) {
		spawnShapeValue = source.spawnShapeValue.copy();
	}

	public override void init () {
		spawnShapeValue.init();
	}

    public override void allocateChannels () {
		positionChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Position);
		rotationChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Rotation3D);
	}

    public override void start () {
		spawnShapeValue.start();
	}

    public override void activateParticles (int startIndex, int count) {
		for (int i = startIndex * positionChannel.strideSize,
			c = i + count * positionChannel.strideSize; i < c; i += positionChannel.strideSize) {
			spawnShapeValue.spawn(TMP_V1, controller.emitter.percent);
			TMP_V1.mul(controller.transform);
			positionChannel.data[i + ParticleChannels.XOffset] = TMP_V1.x;
			positionChannel.data[i + ParticleChannels.YOffset] = TMP_V1.y;
			positionChannel.data[i + ParticleChannels.ZOffset] = TMP_V1.z;
		}
		for (int i = startIndex * rotationChannel.strideSize,
			c = i + count * rotationChannel.strideSize; i < c; i += rotationChannel.strideSize) {
			controller.transform.getRotation(TMP_Q, true);
			rotationChannel.data[i + ParticleChannels.XOffset] = TMP_Q.x;
			rotationChannel.data[i + ParticleChannels.YOffset] = TMP_Q.y;
			rotationChannel.data[i + ParticleChannels.ZOffset] = TMP_Q.z;
			rotationChannel.data[i + ParticleChannels.WOffset] = TMP_Q.w;
		}
	}

    public override SpawnInfluencer copy () {
		return new SpawnInfluencer(this);
	}

    public override void write (Json json) {
		json.writeValue("spawnShape", spawnShapeValue, typeof(SpawnShapeValue));
	}

    public override void read (Json json, JsonValue jsonData) {
		spawnShapeValue = (SpawnShapeValue)json.readValue("spawnShape", typeof(SpawnShapeValue), jsonData);
	}

    public override void save (AssetManager manager, ResourceData<ParticleEffect> data) {
		spawnShapeValue.save(manager, data);
	}

    public override void load (AssetManager manager, ResourceData<ParticleEffect> data) {
		spawnShapeValue.load(manager, data);
	}
}
