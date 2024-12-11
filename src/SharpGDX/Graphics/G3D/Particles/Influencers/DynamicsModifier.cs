using SharpGDX.Utils;
using static SharpGDX.Graphics.G2D.ParticleEmitter;
using SharpGDX.Mathematics;
using static SharpGDX.Graphics.G3D.Particles.ParallelArray;

namespace SharpGDX.Graphics.G3D.Particles.Influencers;

/** It's the base class for any kind of influencer which operates on angular velocity and acceleration of the particles. All the
 * classes that will inherit this base class can and should be used only as sub-influencer of an instance of
 * {@link DynamicsInfluencer} .
 * @author Inferno */
public abstract class DynamicsModifier : Influencer {
	protected static readonly Vector3 TMP_V1 = new Vector3(), TMP_V2 = new Vector3(), TMP_V3 = new Vector3();
	protected static readonly Quaternion TMP_Q = new Quaternion();

	public class FaceDirection : DynamicsModifier {
		FloatChannel rotationChannel, accellerationChannel;

		public FaceDirection () {
		}

		public FaceDirection (FaceDirection rotation) 
        : base(rotation)
        {
			
		}

        public override void allocateChannels () {
			rotationChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Rotation3D);
			accellerationChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Acceleration);
		}

        public override void update () {
			for (int i = 0, accelOffset = 0, c = i + controller.particles.size
				* rotationChannel.strideSize; i < c; i += rotationChannel.strideSize, accelOffset += accellerationChannel.strideSize) {

				Vector3 axisZ = TMP_V1.Set(accellerationChannel.data[accelOffset + ParticleChannels.XOffset],
					accellerationChannel.data[accelOffset + ParticleChannels.YOffset],
					accellerationChannel.data[accelOffset + ParticleChannels.ZOffset]).nor(),
					axisY = TMP_V2.Set(TMP_V1).crs(Vector3.Y).nor().crs(TMP_V1).nor(), axisX = TMP_V3.Set(axisY).crs(axisZ).nor();
				TMP_Q.setFromAxes(false, axisX.x, axisY.x, axisZ.x, axisX.y, axisY.y, axisZ.y, axisX.z, axisY.z, axisZ.z);
				rotationChannel.data[i + ParticleChannels.XOffset] = TMP_Q.x;
				rotationChannel.data[i + ParticleChannels.YOffset] = TMP_Q.y;
				rotationChannel.data[i + ParticleChannels.ZOffset] = TMP_Q.z;
				rotationChannel.data[i + ParticleChannels.WOffset] = TMP_Q.w;
			}
		}

        public override ParticleControllerComponent copy () {
			return new FaceDirection(this);
		}
	}

	public abstract class Strength : DynamicsModifier {
		protected FloatChannel strengthChannel;
		public ScaledNumericValue strengthValue;

		public Strength () {
			strengthValue = new ScaledNumericValue();
		}

		public Strength (Strength rotation) 
        : base(rotation)
        {
			
			strengthValue = new ScaledNumericValue();
			strengthValue.load(rotation.strengthValue);
		}

        public override void allocateChannels () {
			base.allocateChannels();
			ParticleChannels.Interpolation.id = controller.particleChannels.newId();
			strengthChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Interpolation);
		}

        public override void activateParticles (int startIndex, int count) {
			float start, diff;
			for (int i = startIndex * strengthChannel.strideSize,
				c = i + count * strengthChannel.strideSize; i < c; i += strengthChannel.strideSize) {
				start = strengthValue.newLowValue();
				diff = strengthValue.newHighValue();
				if (!strengthValue.isRelative()) diff -= start;
				strengthChannel.data[i + ParticleChannels.VelocityStrengthStartOffset] = start;
				strengthChannel.data[i + ParticleChannels.VelocityStrengthDiffOffset] = diff;
			}
		}

        public override void write (Json json) {
			base.write(json);
			json.writeValue("strengthValue", strengthValue);
		}

        public override void read (Json json, JsonValue jsonData) {
			base.read(json, jsonData);
			strengthValue = (ScaledNumericValue)json.readValue("strengthValue", typeof(ScaledNumericValue), jsonData);
		}
	}

	public abstract class Angular : Strength {
		protected FloatChannel angularChannel;
		/** Polar angle, XZ plane */
		public ScaledNumericValue thetaValue;
		/** Azimuth, Y */
		public ScaledNumericValue phiValue;

		public Angular () {
			thetaValue = new ScaledNumericValue();
			phiValue = new ScaledNumericValue();
		}

		public Angular (Angular value) 
        : base(value)
        {
			
			thetaValue = new ScaledNumericValue();
			phiValue = new ScaledNumericValue();
			thetaValue.load(value.thetaValue);
			phiValue.load(value.phiValue);
		}

        public override void allocateChannels () {
			base.allocateChannels();
			ParticleChannels.Interpolation4.id = controller.particleChannels.newId();
			angularChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Interpolation4);
		}

        public override void activateParticles (int startIndex, int count) {
			base.activateParticles(startIndex, count);
			float start, diff;
			for (int i = startIndex * angularChannel.strideSize,
				c = i + count * angularChannel.strideSize; i < c; i += angularChannel.strideSize) {

				// Theta
				start = thetaValue.newLowValue();
				diff = thetaValue.newHighValue();
				if (!thetaValue.isRelative()) diff -= start;
				angularChannel.data[i + ParticleChannels.VelocityThetaStartOffset] = start;
				angularChannel.data[i + ParticleChannels.VelocityThetaDiffOffset] = diff;

				// Phi
				start = phiValue.newLowValue();
				diff = phiValue.newHighValue();
				if (!phiValue.isRelative()) diff -= start;
				angularChannel.data[i + ParticleChannels.VelocityPhiStartOffset] = start;
				angularChannel.data[i + ParticleChannels.VelocityPhiDiffOffset] = diff;
			}
		}

        public override void write (Json json) {
			base.write(json);
			json.writeValue("thetaValue", thetaValue);
			json.writeValue("phiValue", phiValue);
		}

        public override void read (Json json, JsonValue jsonData) {
            base.read(json, jsonData);
			thetaValue = (ScaledNumericValue)json.readValue("thetaValue", typeof(ScaledNumericValue), jsonData);
			phiValue = (ScaledNumericValue)json.readValue("phiValue", typeof(ScaledNumericValue), jsonData);
		}
	}

	public class Rotational2D : Strength {
		FloatChannel rotationalVelocity2dChannel;

		public Rotational2D () {
		}

		public Rotational2D (Rotational2D rotation) 
        : base(rotation)
        {
			
		}

        public override void allocateChannels () {
            base.allocateChannels();
			rotationalVelocity2dChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.AngularVelocity2D);
		}

        public override void update () {
			for (int i = 0, l = ParticleChannels.LifePercentOffset, s = 0, c = i + controller.particles.size
				* rotationalVelocity2dChannel.strideSize; i < c; s += strengthChannel.strideSize, i += rotationalVelocity2dChannel.strideSize, l += lifeChannel.strideSize) {
				rotationalVelocity2dChannel.data[i] += strengthChannel.data[s + ParticleChannels.VelocityStrengthStartOffset]
					+ strengthChannel.data[s + ParticleChannels.VelocityStrengthDiffOffset]
						* strengthValue.getScale(lifeChannel.data[l]);
			}
		}

        public override Rotational2D copy () {
			return new Rotational2D(this);
		}
	}

	public class Rotational3D : Angular {
		FloatChannel rotationChannel, rotationalForceChannel;

		public Rotational3D () {
		}

		public Rotational3D (Rotational3D rotation) 
        : base(rotation)
        {
            
		}

        public override void allocateChannels () {
            base.allocateChannels();
			rotationChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Rotation3D);
			rotationalForceChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.AngularVelocity3D);
		}

        public override void update () {

			// Matrix3 I_t = defined by the shape, it's the inertia tensor
			// Vector3 r = position vector
			// Vector3 L = r.cross(v.mul(m)), It's the angular momentum, where mv it's the linear momentum
			// Inverse(I_t) = a diagonal matrix where the diagonal is IyIz, IxIz, IxIy
			// Vector3 w = L/I_t = inverse(I_t)*L, It's the angular velocity
			// Quaternion spin = 0.5f*Quaternion(w, 0)*currentRotation
			// currentRotation += spin*dt
			// normalize(currentRotation)

			// Algorithm 1
			// Consider a simple channel which represent an angular velocity w
			// Sum each w for each rotation
			// Update rotation

			// Algorithm 2
			// Consider a channel which represent a sort of angular momentum L (r, v)
			// Sum each L for each rotation
			// Multiply sum by constant quantity k = m*I_to(-1) , m could be optional while I is constant and can be calculated at
// start
// Update rotation

			// Algorithm 3
			// Consider a channel which represent a simple angular momentum L
			// Proceed as Algorithm 2

			for (int i = 0, l = ParticleChannels.LifePercentOffset, s = 0, a = 0, c = controller.particles.size
				* rotationalForceChannel.strideSize; i < c; s += strengthChannel.strideSize, i += rotationalForceChannel.strideSize, a += angularChannel.strideSize, l += lifeChannel.strideSize) {

				float lifePercent = lifeChannel.data[l],
					strength = strengthChannel.data[s + ParticleChannels.VelocityStrengthStartOffset]
						+ strengthChannel.data[s + ParticleChannels.VelocityStrengthDiffOffset] * strengthValue.getScale(lifePercent),
					phi = angularChannel.data[a + ParticleChannels.VelocityPhiStartOffset]
						+ angularChannel.data[a + ParticleChannels.VelocityPhiDiffOffset] * phiValue.getScale(lifePercent),
					theta = angularChannel.data[a + ParticleChannels.VelocityThetaStartOffset]
						+ angularChannel.data[a + ParticleChannels.VelocityThetaDiffOffset] * thetaValue.getScale(lifePercent);

				float cosTheta = MathUtils.cosDeg(theta), sinTheta = MathUtils.sinDeg(theta), cosPhi = MathUtils.cosDeg(phi),
					sinPhi = MathUtils.sinDeg(phi);

				TMP_V3.Set(cosTheta * sinPhi, cosPhi, sinTheta * sinPhi);
				TMP_V3.scl(strength * MathUtils.degreesToRadians);

				rotationalForceChannel.data[i + ParticleChannels.XOffset] += TMP_V3.x;
				rotationalForceChannel.data[i + ParticleChannels.YOffset] += TMP_V3.y;
				rotationalForceChannel.data[i + ParticleChannels.ZOffset] += TMP_V3.z;
			}
		}

        public override Rotational3D copy () {
			return new Rotational3D(this);
		}
	}

	public class CentripetalAcceleration : Strength {
		FloatChannel accelerationChannel;
		FloatChannel positionChannel;

		public CentripetalAcceleration () {
		}

		public CentripetalAcceleration (CentripetalAcceleration rotation) 
        : base(rotation)
        {
            
		}

        public override void allocateChannels () {
            base.allocateChannels();
			accelerationChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Acceleration);
			positionChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Position);
		}

        public override void update () {
			float cx = 0, cy = 0, cz = 0;
			if (!isGlobal) {
				float[] val = controller.transform.val;
				cx = val[Matrix4.M03];
				cy = val[Matrix4.M13];
				cz = val[Matrix4.M23];
			}

			int lifeOffset = ParticleChannels.LifePercentOffset, strengthOffset = 0, positionOffset = 0, forceOffset = 0;
			for (int i = 0,
				c = controller.particles.size; i < c; ++i, positionOffset += positionChannel.strideSize, strengthOffset += strengthChannel.strideSize, forceOffset += accelerationChannel.strideSize, lifeOffset += lifeChannel.strideSize) {

				float strength = strengthChannel.data[strengthOffset + ParticleChannels.VelocityStrengthStartOffset]
					+ strengthChannel.data[strengthOffset + ParticleChannels.VelocityStrengthDiffOffset]
						* strengthValue.getScale(lifeChannel.data[lifeOffset]);
				TMP_V3.Set(positionChannel.data[positionOffset + ParticleChannels.XOffset] - cx,
					positionChannel.data[positionOffset + ParticleChannels.YOffset] - cy,
					positionChannel.data[positionOffset + ParticleChannels.ZOffset] - cz).nor().scl(strength);
				accelerationChannel.data[forceOffset + ParticleChannels.XOffset] += TMP_V3.x;
				accelerationChannel.data[forceOffset + ParticleChannels.YOffset] += TMP_V3.y;
				accelerationChannel.data[forceOffset + ParticleChannels.ZOffset] += TMP_V3.z;
			}
		}

        public override CentripetalAcceleration copy () {
			return new CentripetalAcceleration(this);
		}
	}

	public class PolarAcceleration : Angular {
		FloatChannel directionalVelocityChannel;

		public PolarAcceleration () {
		}

		public PolarAcceleration (PolarAcceleration rotation) 
        : base(rotation)
        {
            
		}

        public override void allocateChannels () {
            base.allocateChannels();
			directionalVelocityChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Acceleration);
		}

        public override void update () {
			for (int i = 0, l = ParticleChannels.LifePercentOffset, s = 0, a = 0, c = i + controller.particles.size
				* directionalVelocityChannel.strideSize; i < c; s += strengthChannel.strideSize, i += directionalVelocityChannel.strideSize, a += angularChannel.strideSize, l += lifeChannel.strideSize) {

				float lifePercent = lifeChannel.data[l],
					strength = strengthChannel.data[s + ParticleChannels.VelocityStrengthStartOffset]
						+ strengthChannel.data[s + ParticleChannels.VelocityStrengthDiffOffset] * strengthValue.getScale(lifePercent),
					phi = angularChannel.data[a + ParticleChannels.VelocityPhiStartOffset]
						+ angularChannel.data[a + ParticleChannels.VelocityPhiDiffOffset] * phiValue.getScale(lifePercent),
					theta = angularChannel.data[a + ParticleChannels.VelocityThetaStartOffset]
						+ angularChannel.data[a + ParticleChannels.VelocityThetaDiffOffset] * thetaValue.getScale(lifePercent);

				float cosTheta = MathUtils.cosDeg(theta), sinTheta = MathUtils.sinDeg(theta), cosPhi = MathUtils.cosDeg(phi),
					sinPhi = MathUtils.sinDeg(phi);
				TMP_V3.Set(cosTheta * sinPhi, cosPhi, sinTheta * sinPhi).nor().scl(strength);

				if (!isGlobal) {
					controller.transform.getRotation(TMP_Q, true);
					TMP_V3.mul(TMP_Q);
				}

				directionalVelocityChannel.data[i + ParticleChannels.XOffset] += TMP_V3.x;
				directionalVelocityChannel.data[i + ParticleChannels.YOffset] += TMP_V3.y;
				directionalVelocityChannel.data[i + ParticleChannels.ZOffset] += TMP_V3.z;
			}
		}

        public override PolarAcceleration copy () {
			return new PolarAcceleration(this);
		}
	}

	public class TangentialAcceleration : Angular {
		FloatChannel directionalVelocityChannel, positionChannel;

		public TangentialAcceleration () {
		}

		public TangentialAcceleration (TangentialAcceleration rotation) 
        : base(rotation)
        {
            
		}

        public override void allocateChannels () {
            base.allocateChannels();
			directionalVelocityChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Acceleration);
			positionChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Position);
		}

        public override void update () {
			for (int i = 0, l = ParticleChannels.LifePercentOffset, s = 0, a = 0, positionOffset = 0,
				c = i + controller.particles.size
					* directionalVelocityChannel.strideSize; i < c; s += strengthChannel.strideSize, i += directionalVelocityChannel.strideSize, a += angularChannel.strideSize, l += lifeChannel.strideSize, positionOffset += positionChannel.strideSize) {

				float lifePercent = lifeChannel.data[l],
					strength = strengthChannel.data[s + ParticleChannels.VelocityStrengthStartOffset]
						+ strengthChannel.data[s + ParticleChannels.VelocityStrengthDiffOffset] * strengthValue.getScale(lifePercent),
					phi = angularChannel.data[a + ParticleChannels.VelocityPhiStartOffset]
						+ angularChannel.data[a + ParticleChannels.VelocityPhiDiffOffset] * phiValue.getScale(lifePercent),
					theta = angularChannel.data[a + ParticleChannels.VelocityThetaStartOffset]
						+ angularChannel.data[a + ParticleChannels.VelocityThetaDiffOffset] * thetaValue.getScale(lifePercent);

				float cosTheta = MathUtils.cosDeg(theta), sinTheta = MathUtils.sinDeg(theta), cosPhi = MathUtils.cosDeg(phi),
					sinPhi = MathUtils.sinDeg(phi);
				TMP_V3.Set(cosTheta * sinPhi, cosPhi, sinTheta * sinPhi);
				TMP_V1.Set(positionChannel.data[positionOffset + ParticleChannels.XOffset],
					positionChannel.data[positionOffset + ParticleChannels.YOffset],
					positionChannel.data[positionOffset + ParticleChannels.ZOffset]);
				if (!isGlobal) {
					controller.transform.getTranslation(TMP_V2);
					TMP_V1.sub(TMP_V2);
					controller.transform.getRotation(TMP_Q, true);
					TMP_V3.mul(TMP_Q);
				}
				TMP_V3.crs(TMP_V1).nor().scl(strength);
				directionalVelocityChannel.data[i + ParticleChannels.XOffset] += TMP_V3.x;
				directionalVelocityChannel.data[i + ParticleChannels.YOffset] += TMP_V3.y;
				directionalVelocityChannel.data[i + ParticleChannels.ZOffset] += TMP_V3.z;
			}
		}

        public override TangentialAcceleration copy () {
			return new TangentialAcceleration(this);
		}
	}

	public class BrownianAcceleration : Strength {
		FloatChannel accelerationChannel;

		public BrownianAcceleration () {
		}

		public BrownianAcceleration (BrownianAcceleration rotation) 
        : base(rotation)
        {
            
		}

        public override void allocateChannels () {
            base.allocateChannels();
			accelerationChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Acceleration);
		}

        public override void update () {
			int lifeOffset = ParticleChannels.LifePercentOffset, strengthOffset = 0, forceOffset = 0;
			for (int i = 0,
				c = controller.particles.size; i < c; ++i, strengthOffset += strengthChannel.strideSize, forceOffset += accelerationChannel.strideSize, lifeOffset += lifeChannel.strideSize) {

				float strength = strengthChannel.data[strengthOffset + ParticleChannels.VelocityStrengthStartOffset]
					+ strengthChannel.data[strengthOffset + ParticleChannels.VelocityStrengthDiffOffset]
						* strengthValue.getScale(lifeChannel.data[lifeOffset]);
				TMP_V3.Set(MathUtils.random(-1, 1f), MathUtils.random(-1, 1f), MathUtils.random(-1, 1f)).nor().scl(strength);
				accelerationChannel.data[forceOffset + ParticleChannels.XOffset] += TMP_V3.x;
				accelerationChannel.data[forceOffset + ParticleChannels.YOffset] += TMP_V3.y;
				accelerationChannel.data[forceOffset + ParticleChannels.ZOffset] += TMP_V3.z;
			}
		}

        public override BrownianAcceleration copy () {
			return new BrownianAcceleration(this);
		}
	}

	public bool isGlobal = false;
	protected FloatChannel lifeChannel;

	public DynamicsModifier () {
	}

	public DynamicsModifier (DynamicsModifier modifier) {
		this.isGlobal = modifier.isGlobal;
	}

    public override void allocateChannels () {
		lifeChannel = controller.particles.addChannel<FloatChannel>(ParticleChannels.Life);
	}

    public override void write (Json json) {
        base.write(json);
		json.writeValue("isGlobal", isGlobal);
	}

    public override void read (Json json, JsonValue jsonData) {
        base.read(json, jsonData);
		isGlobal = (bool)json.readValue("isGlobal", typeof(bool), jsonData);
	}

}
