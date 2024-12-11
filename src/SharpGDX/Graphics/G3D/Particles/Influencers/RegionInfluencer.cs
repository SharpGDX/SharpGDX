using SharpGDX.Assets;
using static SharpGDX.Graphics.G3D.Particles.ParallelArray;
using SharpGDX.Graphics.G2D;
using SharpGDX.Utils;
using SharpGDX.Graphics;

namespace SharpGDX.Graphics.G3D.Particles.Influencers;

/** It's an {@link Influencer} which assigns a region of a {@link Texture} to the particles.
 * @author Inferno */
public abstract class RegionInfluencer : Influencer {

	/** Assigns the first region of {@link RegionInfluencer#regions} to the particles. */
	public class Single : RegionInfluencer {
		public Single () {
		}

		public Single (Single regionInfluencer) 
        : base(regionInfluencer)
        {
            
		}

		public Single (TextureRegion textureRegion) 
        : base(textureRegion)
        {
            
		}

		public Single (Texture texture) 
        : base(texture)
        {
            
		}

		public override void init () {
			AspectTextureRegion region = regions.items[0];
			for (int i = 0,
				c = controller.emitter.maxParticleCount * regionChannel.strideSize; i < c; i += regionChannel.strideSize) {
				regionChannel.data[i + ParticleChannels.UOffset] = region.u;
				regionChannel.data[i + ParticleChannels.VOffset] = region.v;
				regionChannel.data[i + ParticleChannels.U2Offset] = region.u2;
				regionChannel.data[i + ParticleChannels.V2Offset] = region.v2;
				regionChannel.data[i + ParticleChannels.HalfWidthOffset] = 0.5f;
				regionChannel.data[i + ParticleChannels.HalfHeightOffset] = region.halfInvAspectRatio;
			}
		}

		public override Single copy () {
			return new Single(this);
		}
	}

	/** Assigns a random region of {@link RegionInfluencer#regions} to the particles. */
	public class Random : RegionInfluencer {
		public Random () {
		}

		public Random (Random regionInfluencer) 
        : base(regionInfluencer)
        {
            
		}

		public Random (TextureRegion textureRegion) 
        : base(textureRegion)
        {
            
		}

		public Random (Texture texture) 
        : base(texture)
        {
			
		}

		public override void activateParticles (int startIndex, int count) {
			for (int i = startIndex * regionChannel.strideSize,
				c = i + count * regionChannel.strideSize; i < c; i += regionChannel.strideSize) {
				AspectTextureRegion region = regions.random();
				regionChannel.data[i + ParticleChannels.UOffset] = region.u;
				regionChannel.data[i + ParticleChannels.VOffset] = region.v;
				regionChannel.data[i + ParticleChannels.U2Offset] = region.u2;
				regionChannel.data[i + ParticleChannels.V2Offset] = region.v2;
				regionChannel.data[i + ParticleChannels.HalfWidthOffset] = 0.5f;
				regionChannel.data[i + ParticleChannels.HalfHeightOffset] = region.halfInvAspectRatio;
			}
		}

		public override Random copy () {
			return new Random(this);
		}
	}

	/** Assigns a region to the particles using the particle life percent to calculate the current index in the
	 * {@link RegionInfluencer#regions} array. */
	public class Animated : RegionInfluencer {
		FloatChannel lifeChannel;

		public Animated () {
		}

		public Animated (Animated regionInfluencer) 
        : base(regionInfluencer)
        {
            
		}

		public Animated (TextureRegion textureRegion) 
        : base(textureRegion)
        {
            
		}

		public Animated (Texture texture) 
        : base(texture)
        {
            
		}

        public override void allocateChannels () {
            base.allocateChannels();
			lifeChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Life);
		}

        public override void update () {
			for (int i = 0, l = ParticleChannels.LifePercentOffset, c = controller.particles.size
				* regionChannel.strideSize; i < c; i += regionChannel.strideSize, l += lifeChannel.strideSize) {
				AspectTextureRegion region = regions.Get((int)(lifeChannel.data[l] * (regions.size - 1)));
				regionChannel.data[i + ParticleChannels.UOffset] = region.u;
				regionChannel.data[i + ParticleChannels.VOffset] = region.v;
				regionChannel.data[i + ParticleChannels.U2Offset] = region.u2;
				regionChannel.data[i + ParticleChannels.V2Offset] = region.v2;
				regionChannel.data[i + ParticleChannels.HalfWidthOffset] = 0.5f;
				regionChannel.data[i + ParticleChannels.HalfHeightOffset] = region.halfInvAspectRatio;
			}
		}

		public override Animated copy () {
			return new Animated(this);
		}
	}

	/** It's a class used internally by the {@link RegionInfluencer} to represent a texture region. It contains the uv coordinates
	 * of the region and the region inverse aspect ratio. */
	public class AspectTextureRegion {
		public float u, v, u2, v2;
		public float halfInvAspectRatio;
		public String imageName;

		public AspectTextureRegion () {
		}

		public AspectTextureRegion (AspectTextureRegion aspectTextureRegion) {
			set(aspectTextureRegion);
		}

		public AspectTextureRegion (TextureRegion region) {
			set(region);
		}

		public void set (TextureRegion region) {
			this.u = region.getU();
			this.v = region.getV();
			this.u2 = region.getU2();
			this.v2 = region.getV2();
			this.halfInvAspectRatio = 0.5f * ((float)region.getRegionHeight() / region.getRegionWidth());
			if (region is TextureAtlas.AtlasRegion) {
				this.imageName = ((TextureAtlas.AtlasRegion)region).name;
			}
		}

		public void set (AspectTextureRegion aspectTextureRegion) {
			u = aspectTextureRegion.u;
			v = aspectTextureRegion.v;
			u2 = aspectTextureRegion.u2;
			v2 = aspectTextureRegion.v2;
			halfInvAspectRatio = aspectTextureRegion.halfInvAspectRatio;
			imageName = aspectTextureRegion.imageName;
		}

		public void updateUV (TextureAtlas atlas) {
			if (imageName == null) {
				return;
			}
			TextureAtlas.AtlasRegion region = atlas.findRegion(imageName);
			this.u = region.getU();
			this.v = region.getV();
			this.u2 = region.getU2();
			this.v2 = region.getV2();
			this.halfInvAspectRatio = 0.5f * ((float)region.getRegionHeight() / region.getRegionWidth());
		}
	}

	public Array<AspectTextureRegion> regions;
	FloatChannel regionChannel;
	public String atlasName;

	public RegionInfluencer (int regionsCount) {
		this.regions = new Array<AspectTextureRegion>(false, regionsCount, typeof(AspectTextureRegion));
	}

	public RegionInfluencer () 
    : this(1)
    {
		
		AspectTextureRegion aspectRegion = new AspectTextureRegion();
		aspectRegion.u = aspectRegion.v = 0;
		aspectRegion.u2 = aspectRegion.v2 = 1;
		aspectRegion.halfInvAspectRatio = 0.5f;
		regions.Add(aspectRegion);
	}

	/** All the regions must be defined on the same Texture */
	public RegionInfluencer (params TextureRegion[] regions) {
		setAtlasName(null);
		this.regions = new Array<AspectTextureRegion>(false, regions.Length, typeof(AspectTextureRegion));
		add(regions);
	}

	public RegionInfluencer (Texture texture)
    :this(new TextureRegion(texture)){
		
	}

	public RegionInfluencer (RegionInfluencer regionInfluencer) 
    : this(regionInfluencer.regions.size)
    {
		
		regions.ensureCapacity(regionInfluencer.regions.size);
		for (int i = 0; i < regionInfluencer.regions.size; ++i) {
			regions.Add(new AspectTextureRegion((AspectTextureRegion)regionInfluencer.regions.Get(i)));
		}
	}

	public void setAtlasName (String atlasName) {
		this.atlasName = atlasName;
	}

	public void add (TextureRegion[] regions) {
		this.regions.ensureCapacity(regions.Length);
		foreach (TextureRegion region in regions) {
			this.regions.Add(new AspectTextureRegion(region));
		}
	}

	public void clear () {
		atlasName = null;
		regions.clear();
	}

	private readonly static String ASSET_DATA = "atlasAssetData";

	public override void load (AssetManager manager, ResourceData<ParticleEffect> resources) {
		base.load(manager, resources);
        ResourceData<ParticleEffect> .SaveData data = resources.getSaveData(ASSET_DATA);
		if (data == null) {
			return;
		}
		TextureAtlas atlas;
		atlas = (TextureAtlas)manager.get<TextureAtlas>(data.loadAsset());
		foreach (AspectTextureRegion atr in regions) {
			atr.updateUV(atlas);
		}
	}

	public override void save (AssetManager manager, ResourceData<ParticleEffect> resources) {
		base.save(manager, resources);
		if (atlasName != null) {
            ResourceData<ParticleEffect> .SaveData data = resources.getSaveData(ASSET_DATA);
			if (data == null) {
				data = resources.createSaveData(ASSET_DATA);
			}
			data.saveAsset<TextureAtlas>(atlasName, typeof(TextureAtlas));
		}
	}

    public override void allocateChannels () {
		regionChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.TextureRegion);
	}

    public override void write (Json json) {
		json.writeValue("regions", regions, typeof(Array), typeof(AspectTextureRegion));
	}

	public override void read (Json json, JsonValue jsonData) {
		regions.clear();
		regions.addAll((Array<AspectTextureRegion>)json.readValue("regions", typeof(Array), typeof(AspectTextureRegion), jsonData));
	}
}
