using System;
using SharpGDX.Graphics.GLUtils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D.Decals;

/**
 * <p>
 * Minimalistic grouping strategy useful for orthogonal scenes where the camera faces the negative z axis. Handles enabling and
 * disabling of blending and uses world-z only front to back sorting for transparent decals.
 * </p>
 * <p>
 * States (* = any, EV = entry value - same as value before flush):<br/>
 * <table>
 * <tr>
 * <td></td>
 * <td>expects</td>
 * <td>exits on</td>
 * </tr>
 * <tr>
 * <td>glDepthMask</td>
 * <td>true</td>
 * <td>EV | true</td>
 * </tr>
 * <tr>
 * <td>GL_DEPTH_TEST</td>
 * <td>enabled</td>
 * <td>EV</td>
 * </tr>
 * <tr>
 * <td>glDepthFunc</td>
 * <td>GL_LESS | GL_LEQUAL</td>
 * <td>EV</td>
 * </tr>
 * <tr>
 * <td>GL_BLEND</td>
 * <td>disabled</td>
 * <td>EV | disabled</td>
 * </tr>
 * <tr>
 * <td>glBlendFunc</td>
 * <td>*</td>
 * <td>*</td>
 * </tr>
 * <tr>
 * <td>GL_TEXTURE_2D</td>
 * <td>*</td>
 * <td>disabled</td>
 * </tr>
 * </table>
 * </p>
 */
public class SimpleOrthoGroupStrategy : GroupStrategy {
	private Comparator comparator = new Comparator();
	private static readonly int GROUP_OPAQUE = 0;
	private static readonly int GROUP_BLEND = 1;

	public int decideGroup (Decal decal) {
		return decal.getMaterial().isOpaque() ? GROUP_OPAQUE : GROUP_BLEND;
	}

	public void beforeGroup (int group, Array<Decal> contents) {
		if (group == GROUP_BLEND) {
			Sort<Decal>.instance().sort(contents, comparator);
			GDX.GL.glEnable(IGL20.GL_BLEND);
            // no need for writing into the z buffer if transparent decals are the last thing to be rendered
            // and they are rendered back to front
            GDX.GL.glDepthMask(false);
		} else {
			// FIXME sort by material
		}
	}

	public void afterGroup (int group) {
		if (group == GROUP_BLEND) {
            GDX.GL.glDepthMask(true);
            GDX.GL.glDisable(IGL20.GL_BLEND);
		}
	}

	public void beforeGroups () {
		GDX.GL.glEnable(IGL20.GL_TEXTURE_2D);
	}

	public void afterGroups () {
        GDX.GL.glDisable(IGL20.GL_TEXTURE_2D);
	}

	class Comparator : IComparer<Decal> {
		public int Compare (Decal a, Decal b) {
			if (a.getZ() == b.getZ()) return 0;
			return a.getZ() - b.getZ() < 0 ? -1 : 1;
		}
	}

	public ShaderProgram getGroupShader (int group) {
		return null;
	}
}
