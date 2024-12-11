using System;
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
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Environments;


namespace SharpGDX.Graphics.G3D.Particles.Influencers;

/** It's a {@link ParticleControllerComponent} which usually modifies one or more properties of the particles(i.e color, scale,
 * graphical representation, velocity, etc...).
 * @author Inferno */
public abstract class Influencer : ParticleControllerComponent {

}
