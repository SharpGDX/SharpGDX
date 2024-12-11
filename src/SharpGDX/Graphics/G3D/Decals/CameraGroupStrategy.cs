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


namespace SharpGDX.Graphics.G3D.Decals;

/**
 * <p>
 * Minimalistic grouping strategy that splits decals into opaque and transparent ones enabling and disabling blending as needed.
 * Opaque decals are rendered first (decal color is ignored in opacity check).<br/>
 * Use this strategy only if the vast majority of your decals are opaque and the few transparent ones are unlikely to overlap.
 * </p>
 * <p>
 * Can produce invisible artifacts when transparent decals overlap each other.
 * </p>
 * <p>
 * Needs to be explicitly disposed as it might allocate a ShaderProgram when GLSL 2.0 is used.
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
 * <td>EV</td>
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
public class CameraGroupStrategy : GroupStrategy, IDisposable {
	private static readonly int GROUP_OPAQUE = 0;
	private static readonly int GROUP_BLEND = 1;

    private class DecalArrayPool : Pool<Array<Decal>>
    {
        protected internal override Array<Decal> newObject()
        {
            return new Array<Decal>();
        }
    }

	Pool<Array<Decal>> arrayPool = new DecalArrayPool();
	Array<Array<Decal>> usedArrays = new Array<Array<Decal>>();
	ObjectMap<DecalMaterial, Array<Decal>> materialGroups = new ObjectMap<DecalMaterial, Array<Decal>>();

	Camera camera;
	ShaderProgram shader;
	private readonly IComparer<Decal> cameraSorter;

    private class CameraComparer : IComparer<Decal>
    {
        private readonly CameraGroupStrategy _strategy;

        internal CameraComparer(CameraGroupStrategy strategy)
        {
            _strategy = strategy;
        }

        public int Compare(Decal o1, Decal o2)
        {
            float dist1 = _strategy.camera.position.dst(o1.position);
            float dist2 = _strategy.camera.position.dst(o2.position);
            return (int)Math.Sign(dist2 - dist1);
        }
    }

	public CameraGroupStrategy (Camera camera) {
		this.camera = camera;
		this.cameraSorter = new CameraComparer(this);
		createDefaultShader();
	}

	public CameraGroupStrategy (Camera camera, IComparer<Decal> sorter) {
		this.camera = camera;
		this.cameraSorter = sorter;
		createDefaultShader();
	}

	public void setCamera (Camera camera) {
		this.camera = camera;
	}

	public Camera getCamera () {
		return camera;
	}

	public int decideGroup (Decal decal) {
		return decal.getMaterial().isOpaque() ? GROUP_OPAQUE : GROUP_BLEND;
	}

	public void beforeGroup (int group, Array<Decal> contents) {
		if (group == GROUP_BLEND) {
			GDX.GL.glEnable(IGL20.GL_BLEND);
			GDX.GL.glDepthMask(false);
			contents.sort(cameraSorter);
		} else {
			for (int i = 0, n = contents.size; i < n; i++) {
				Decal decal = contents.Get(i);
				Array<Decal> materialGroup = materialGroups.get(decal.material);
				if (materialGroup == null) {
					materialGroup = arrayPool.obtain();
					materialGroup.clear();
					usedArrays.Add(materialGroup);
					materialGroups.put(decal.material, materialGroup);
				}
				materialGroup.Add(decal);
			}

			contents.clear();
			foreach (Array<Decal> materialGroup in materialGroups.values()) {
				contents.addAll(materialGroup);
			}

			materialGroups.clear();
			arrayPool.freeAll(usedArrays);
			usedArrays.clear();
		}
	}

	public void afterGroup (int group) {
		if (group == GROUP_BLEND) {
			GDX.GL.glDisable(IGL20.GL_BLEND);
			GDX.GL.glDepthMask(true);
		}
	}

	public void beforeGroups () {
		GDX.GL.glEnable(IGL20.GL_DEPTH_TEST);
		shader.bind();
		shader.setUniformMatrix("u_projectionViewMatrix", camera.Combined);
		shader.setUniformi("u_texture", 0);
	}

	public void afterGroups () {
		GDX.GL.glDisable(IGL20.GL_DEPTH_TEST);
	}

	private void createDefaultShader () {
		String vertexShader = "attribute vec4 " + ShaderProgram.POSITION_ATTRIBUTE + ";\n" //
			+ "attribute vec4 " + ShaderProgram.COLOR_ATTRIBUTE + ";\n" //
			+ "attribute vec2 " + ShaderProgram.TEXCOORD_ATTRIBUTE + "0;\n" //
			+ "uniform mat4 u_projectionViewMatrix;\n" //
			+ "varying vec4 v_color;\n" //
			+ "varying vec2 v_texCoords;\n" //
			+ "\n" //
			+ "void main()\n" //
			+ "{\n" //
			+ "   v_color = " + ShaderProgram.COLOR_ATTRIBUTE + ";\n" //
			+ "   v_color.a = v_color.a * (255.0/254.0);\n" //
			+ "   v_texCoords = " + ShaderProgram.TEXCOORD_ATTRIBUTE + "0;\n" //
			+ "   gl_Position =  u_projectionViewMatrix * " + ShaderProgram.POSITION_ATTRIBUTE + ";\n" //
			+ "}\n";
		String fragmentShader = "#ifdef GL_ES\n" //
			+ "precision mediump float;\n" //
			+ "#endif\n" //
			+ "varying vec4 v_color;\n" //
			+ "varying vec2 v_texCoords;\n" //
			+ "uniform sampler2D u_texture;\n" //
			+ "void main()\n"//
			+ "{\n" //
			+ "  gl_FragColor = v_color * texture2D(u_texture, v_texCoords);\n" //
			+ "}";

		shader = new ShaderProgram(vertexShader, fragmentShader);
		if (!shader.isCompiled()) throw new IllegalArgumentException("couldn't compile shader: " + shader.getLog());
	}

	public ShaderProgram getGroupShader (int group) {
		return shader;
	}

	public void Dispose () {
		if (shader != null) shader.Dispose();
	}
}