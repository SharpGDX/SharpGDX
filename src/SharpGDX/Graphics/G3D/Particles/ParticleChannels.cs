using FloatChannel = SharpGDX.Graphics.G3D.Particles.ParallelArray.FloatChannel;
using ChannelDescriptor = SharpGDX.Graphics.G3D.Particles.ParallelArray.ChannelDescriptor;

namespace SharpGDX.Graphics.G3D.Particles;

/** This contains all the definitions of particle related channels and channel initializers. It is also used by the
 * {@link ParticleController} to handle temporary channels allocated by influencers.
 * @author inferno */
public class ParticleChannels {
	private static int currentGlobalId;

	public static int newGlobalId () {
		return currentGlobalId++;
	}

	// Initializers
	public class TextureRegionInitializer : SharpGDX.Graphics.G3D.Particles.ParallelArray.ChannelInitializer<FloatChannel> {
		private static TextureRegionInitializer instance;

		public static TextureRegionInitializer get () {
			if (instance == null) instance = new TextureRegionInitializer();
			return instance;
		}

		public void init (FloatChannel channel) {
			for (int i = 0, c = channel.data.Length; i < c; i += channel.strideSize) {
				channel.data[i + ParticleChannels.UOffset] = 0;
				channel.data[i + ParticleChannels.VOffset] = 0;
				channel.data[i + ParticleChannels.U2Offset] = 1;
				channel.data[i + ParticleChannels.V2Offset] = 1;
				channel.data[i + ParticleChannels.HalfWidthOffset] = 0.5f;
				channel.data[i + ParticleChannels.HalfHeightOffset] = 0.5f;
			}
		}
	}

	public  class ColorInitializer : SharpGDX.Graphics.G3D.Particles.ParallelArray.ChannelInitializer<FloatChannel> {
		private static ColorInitializer instance;

		public static ColorInitializer get () {
			if (instance == null) instance = new ColorInitializer();
			return instance;
		}

		public void init (FloatChannel channel) {
			Array.Fill(channel.data, 0, channel.data.Length, 1);
		}
	}

	public  class ScaleInitializer : SharpGDX.Graphics.G3D.Particles.ParallelArray.ChannelInitializer<FloatChannel> {
		private static ScaleInitializer instance;

		public static ScaleInitializer get () {
			if (instance == null) instance = new ScaleInitializer();
			return instance;
		}

		public void init (FloatChannel channel) {
			Array.Fill(channel.data, 0, channel.data.Length, 1);
		}
	}

	public  class Rotation2dInitializer : SharpGDX.Graphics.G3D.Particles.ParallelArray.ChannelInitializer<FloatChannel> {
		private static Rotation2dInitializer instance;

		public static Rotation2dInitializer get () {
			if (instance == null) instance = new Rotation2dInitializer();
			return instance;
		}

		public void init (FloatChannel channel) {
			for (int i = 0, c = channel.data.Length; i < c; i += channel.strideSize) {
				channel.data[i + ParticleChannels.CosineOffset] = 1;
				channel.data[i + ParticleChannels.SineOffset] = 0;
			}
		}
	}

	public class Rotation3dInitializer : SharpGDX.Graphics.G3D.Particles.ParallelArray.ChannelInitializer<FloatChannel> {
		private static Rotation3dInitializer instance;

		public static Rotation3dInitializer get () {
			if (instance == null) instance = new Rotation3dInitializer();
			return instance;
		}

		public void init (FloatChannel channel) {
			for (int i = 0, c = channel.data.Length; i < c; i += channel.strideSize) {
				channel.data[i + ParticleChannels.XOffset] = channel.data[i
					+ ParticleChannels.YOffset] = channel.data[i + ParticleChannels.ZOffset] = 0;
				channel.data[i + ParticleChannels.WOffset] = 1;
			}
		}
	}

	// Channels
	/** Channels of common use like position, life, color, etc... */
	public static readonly ChannelDescriptor Life = new ChannelDescriptor(newGlobalId(), typeof(float), 3);
	public static readonly ChannelDescriptor Position = new ChannelDescriptor(newGlobalId(), typeof(float), 3); // gl units
	public static readonly ChannelDescriptor PreviousPosition = new ChannelDescriptor(newGlobalId(), typeof(float), 3);
	public static readonly ChannelDescriptor Color = new ChannelDescriptor(newGlobalId(), typeof(float), 4);
	public static readonly ChannelDescriptor TextureRegion = new ChannelDescriptor(newGlobalId(), typeof(float), 6);
	public static readonly ChannelDescriptor Rotation2D = new ChannelDescriptor(newGlobalId(), typeof(float), 2);
	public static readonly ChannelDescriptor Rotation3D = new ChannelDescriptor(newGlobalId(), typeof(float), 4);
	public static readonly ChannelDescriptor Scale = new ChannelDescriptor(newGlobalId(), typeof(float), 1);
	public static readonly ChannelDescriptor ModelInstance = new ChannelDescriptor(newGlobalId(), typeof(ModelInstance), 1);
	public static readonly ChannelDescriptor ParticleController = new ChannelDescriptor(newGlobalId(), typeof(ParticleController), 1);
	public static readonly ChannelDescriptor Acceleration = new ChannelDescriptor(newGlobalId(), typeof(float), 3); // gl units/s2
	public static readonly ChannelDescriptor AngularVelocity2D = new ChannelDescriptor(newGlobalId(), typeof(float), 1);
	public static readonly ChannelDescriptor AngularVelocity3D = new ChannelDescriptor(newGlobalId(), typeof(float), 3);
	public static readonly ChannelDescriptor Interpolation = new ChannelDescriptor(-1, typeof(float), 2);
	public static readonly ChannelDescriptor Interpolation4 = new ChannelDescriptor(-1, typeof(float), 4);
	public static readonly ChannelDescriptor Interpolation6 = new ChannelDescriptor(-1, typeof(float), 6);

	// Offsets
	/** Offsets to acess a particular value inside a stride of a given channel */
	public static readonly int CurrentLifeOffset = 0, TotalLifeOffset = 1, LifePercentOffset = 2;
	public static readonly int RedOffset = 0, GreenOffset = 1, BlueOffset = 2, AlphaOffset = 3;
	public static readonly int InterpolationStartOffset = 0, InterpolationDiffOffset = 1;
	public static readonly int VelocityStrengthStartOffset = 0, VelocityStrengthDiffOffset = 1, VelocityThetaStartOffset = 0,
		VelocityThetaDiffOffset = 1, VelocityPhiStartOffset = 2, VelocityPhiDiffOffset = 3;
	public static readonly int XOffset = 0, YOffset = 1, ZOffset = 2, WOffset = 3;
	public static readonly int UOffset = 0, VOffset = 1, U2Offset = 2, V2Offset = 3, HalfWidthOffset = 4, HalfHeightOffset = 5;
	public static readonly int CosineOffset = 0, SineOffset = 1;

	private int currentId;

	public ParticleChannels () {
		resetIds();
	}

	public int newId () {
		return currentId++;
	}

	internal protected void resetIds () {
		currentId = currentGlobalId;
	}

}
