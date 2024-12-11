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

/** Responsible for sorting {@link Renderable} lists by whatever criteria (material, distance to camera, etc.)
 * @author badlogic */
public interface RenderableSorter {
	/** Sorts the array of {@link Renderable} instances based on some criteria, e.g. material, distance to camera etc.
	 * @param renderables the array of renderables to be sorted */
	public void sort (Camera camera, Array<Renderable> renderables);
}
