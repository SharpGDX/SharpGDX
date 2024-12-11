using System;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Utils;
using SharpGDX.Graphics.G2D;

namespace SharpGDX.Graphics.G3D.Decals;

/** Material used by the {@link Decal} class */
public class DecalMaterial {
	public static readonly int NO_BLEND = -1;
	internal protected TextureRegion textureRegion;
	internal protected int srcBlendFactor;
	internal protected int dstBlendFactor;

	/** Binds the material's texture to the OpenGL context and changes the glBlendFunc to the values used by it. */
	public void set () {
		textureRegion.getTexture().bind(0);
		if (!isOpaque()) {
			GDX.GL.glBlendFunc(srcBlendFactor, dstBlendFactor);
		}
	}

	/** @return true if the material is completely opaque, false if it is not and therefor requires blending */
	public bool isOpaque () {
		return srcBlendFactor == NO_BLEND;
	}

	public int getSrcBlendFactor () {
		return srcBlendFactor;
	}

	public int getDstBlendFactor () {
		return dstBlendFactor;
	}

	public override bool Equals (Object? o) {
		if (o == null) return false;

		DecalMaterial material = (DecalMaterial)o;

		return dstBlendFactor == material.dstBlendFactor && srcBlendFactor == material.srcBlendFactor
			&& textureRegion.getTexture() == material.textureRegion.getTexture();

	}

	public override int GetHashCode () {
		int result = textureRegion.getTexture() != null ? textureRegion.getTexture().GetHashCode() : 0;
		result = 31 * result + srcBlendFactor;
		result = 31 * result + dstBlendFactor;
		return result;
	}
}
