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


namespace SharpGDX.Graphics.G3D.Utils;

/** Manages OpenGL state and tries to reduce state changes. Uses a {@link TextureBinder} to reduce texture binds as well. Call
 * {@link #begin()} to setup the context, call {@link #end()} to undo all state changes. Use the setters to change state, use
 * {@link #textureBinder} to bind textures.
 * @author badlogic, Xoppa */
public class RenderContext {
	/** used to bind textures **/
	public readonly TextureBinder textureBinder;
	private bool blending;
	private int blendSFactor;
	private int blendDFactor;
	private int depthFunc;
	private float depthRangeNear;
	private float depthRangeFar;
	private bool depthMask;
	private int cullFace;

	public RenderContext (TextureBinder textures) {
		this.textureBinder = textures;
	}

	/** Sets up the render context, must be matched with a call to {@link #end()}. */
	public void begin () {
		GDX.GL.glDisable(IGL20.GL_DEPTH_TEST);
		depthFunc = 0;
		GDX.GL.glDepthMask(true);
		depthMask = true;
		GDX.GL.glDisable(IGL20.GL_BLEND);
		blending = false;
		GDX.GL.glDisable(IGL20.GL_CULL_FACE);
		cullFace = blendSFactor = blendDFactor = 0;
		textureBinder.begin();
	}

	/** Resets all changed OpenGL states to their defaults. */
	public void end () {
		if (depthFunc != 0) GDX.GL.glDisable(IGL20.GL_DEPTH_TEST);
		if (!depthMask) GDX.GL.glDepthMask(true);
		if (blending) GDX.GL.glDisable(IGL20.GL_BLEND);
		if (cullFace > 0) GDX.GL.glDisable(IGL20.GL_CULL_FACE);
		textureBinder.end();
	}

	public void setDepthMask ( bool depthMask) {
		if (this.depthMask != depthMask) GDX.GL.glDepthMask(this.depthMask = depthMask);
	}

	public void setDepthTest ( int depthFunction) {
		setDepthTest(depthFunction, 0f, 1f);
	}

	public void setDepthTest ( int depthFunction,  float depthRangeNear,  float depthRangeFar) {
		 bool wasEnabled = depthFunc != 0;
		 bool enabled = depthFunction != 0;
		if (depthFunc != depthFunction) {
			depthFunc = depthFunction;
			if (enabled) {
				GDX.GL.glEnable(IGL20.GL_DEPTH_TEST);
				GDX.GL.glDepthFunc(depthFunction);
			} else
				GDX.GL.glDisable(IGL20.GL_DEPTH_TEST);
		}
		if (enabled) {
			if (!wasEnabled || depthFunc != depthFunction) GDX.GL.glDepthFunc(depthFunc = depthFunction);
			if (!wasEnabled || this.depthRangeNear != depthRangeNear || this.depthRangeFar != depthRangeFar)
				GDX.GL.glDepthRangef(this.depthRangeNear = depthRangeNear, this.depthRangeFar = depthRangeFar);
		}
	}

	public void setBlending ( bool enabled,  int sFactor,  int dFactor) {
		if (enabled != blending) {
			blending = enabled;
			if (enabled)
				GDX.GL.glEnable(IGL20.GL_BLEND);
			else
				GDX.GL.glDisable(IGL20.GL_BLEND);
		}
		if (enabled && (blendSFactor != sFactor || blendDFactor != dFactor)) {
			GDX.GL.glBlendFunc(sFactor, dFactor);
			blendSFactor = sFactor;
			blendDFactor = dFactor;
		}
	}

	public void setCullFace (int face) {
		if (face != cullFace) {
			cullFace = face;
			if ((face == IGL20.GL_FRONT) || (face == IGL20.GL_BACK) || (face == IGL20.GL_FRONT_AND_BACK)) {
				GDX.GL.glEnable(IGL20.GL_CULL_FACE);
				GDX.GL.glCullFace(face);
			} else
				GDX.GL.glDisable(IGL20.GL_CULL_FACE);
		}
	}
}
