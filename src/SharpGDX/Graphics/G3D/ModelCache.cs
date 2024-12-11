using System;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G3D.Utils;
using SharpGDX.Graphics.G3D;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D;

/** ModelCache tries to combine multiple render calls into a single render call by merging them where possible. Can be used for
 * multiple type of models (e.g. varying vertex attributes or materials), the ModelCache will combine where possible. Can be used
 * dynamically (e.g. every frame) or statically (e.g. to combine part of scenery). Be aware that any combined vertices are
 * directly transformed, therefore the resulting {@link Renderable#worldTransform} might not be suitable for sorting anymore (such
 * as the default sorter of ModelBatch does).
 * @author Xoppa */
public class ModelCache : IDisposable, RenderableProvider {
	/** Allows to reuse one or more meshes while avoiding creating new objects. Depending on the implementation it might add memory
	 * optimizations as well. Call the {@link #obtain(VertexAttributes, int, int)} method to obtain a mesh which can at minimum the
	 * specified amount of vertices and indices. Call the {@link #flush()} method to flush the pool ant release all previously
	 * obtained meshes. */
	public interface MeshPool : IDisposable {
		/** Will try to reuse or, when not possible to reuse, optionally create a {@link Mesh} that meets the specified criteria.
		 * @param vertexAttributes the vertex attributes of the mesh to obtain
		 * @param vertexCount the minimum amount vertices the mesh should be able to store
		 * @param indexCount the minimum amount of indices the mesh should be able to store
		 * @return the obtained Mesh, or null when no mesh could be obtained. */
		Mesh obtain (VertexAttributes vertexAttributes, int vertexCount, int indexCount);

		/** Releases all previously obtained {@link Mesh}es using the the {@link #obtain(VertexAttributes, int, int)} method. */
		void flush ();
	}

	/** A basic {@link MeshPool} implementation that avoids creating new meshes at the cost of memory usage. It does this by making
	 * the mesh always the maximum (64k) size. Use this when for dynamic caching where you need to obtain meshes very frequently
	 * (typically every frame).
	 * @author Xoppa */
	public class SimpleMeshPool : MeshPool {
		// FIXME Make a better (preferable JNI) MeshPool implementation
		private Array<Mesh> freeMeshes = new Array<Mesh>();
		private Array<Mesh> usedMeshes = new Array<Mesh>();

		public void flush () {
			freeMeshes.addAll(usedMeshes);
			usedMeshes.clear();
		}

		public Mesh obtain (VertexAttributes vertexAttributes, int vertexCount, int indexCount) {
			for (int i = 0, n = freeMeshes.size; i < n; ++i) {
				 Mesh mesh = freeMeshes.Get(i);
				if (mesh.getVertexAttributes().Equals(vertexAttributes) && mesh.getMaxVertices() >= vertexCount
					&& mesh.getMaxIndices() >= indexCount) {
					freeMeshes.RemoveIndex(i);
					usedMeshes.Add(mesh);
					return mesh;
				}
			}
			vertexCount = MeshBuilder.MAX_VERTICES;
			indexCount = Math.Max(vertexCount, 1 << (32 - BitOperations.LeadingZeroCount((uint)indexCount - 1)));
			Mesh result = new Mesh(false, vertexCount, indexCount, vertexAttributes);
			usedMeshes.Add(result);
			return result;
		}

		public void Dispose () {
			foreach (Mesh m in usedMeshes)
				m.Dispose();
			usedMeshes.clear();
			foreach (Mesh m in  freeMeshes)
				m.Dispose();
			freeMeshes.clear();
		}
	}

	/** A tight {@link MeshPool} implementation, which is typically used for static meshes (create once, use many).
	 * @author Xoppa */
	public class TightMeshPool : MeshPool {
		private Array<Mesh> freeMeshes = new Array<Mesh>();
		private Array<Mesh> usedMeshes = new Array<Mesh>();

		public void flush () {
			freeMeshes.addAll(usedMeshes);
			usedMeshes.clear();
		}

		public Mesh obtain (VertexAttributes vertexAttributes, int vertexCount, int indexCount) {
			for (int i = 0, n = freeMeshes.size; i < n; ++i) {
				Mesh mesh = freeMeshes.Get(i);
				if (mesh.getVertexAttributes().Equals(vertexAttributes) && mesh.getMaxVertices() == vertexCount
					&& mesh.getMaxIndices() == indexCount) {
					freeMeshes.RemoveIndex(i);
					usedMeshes.Add(mesh);
					return mesh;
				}
			}
			Mesh result = new Mesh(true, vertexCount, indexCount, vertexAttributes);
			usedMeshes.Add(result);
			return result;
		}

		public void Dispose () {
			foreach (Mesh m in usedMeshes)
				m.Dispose();
			usedMeshes.clear();
			foreach (Mesh m in freeMeshes)
				m.Dispose();
			freeMeshes.clear();
		}
	}

	/** A {@link RenderableSorter} that sorts by vertex attributes, material attributes and primitive types (in that order), so
	 * that meshes can be easily merged.
	 * @author Xoppa */
	public class Sorter : RenderableSorter, IComparer<Renderable> {
		public void sort (Camera camera, Array<Renderable> renderables) {
			renderables.sort(this);
		}

		public int Compare (Renderable arg0, Renderable arg1) {
			 VertexAttributes va0 = arg0.meshPart.mesh.getVertexAttributes();
			 VertexAttributes va1 = arg1.meshPart.mesh.getVertexAttributes();
			 int vc = va0.CompareTo(va1);
			if (vc == 0) {
				 int mc = arg0.material.CompareTo(arg1.material);
				if (mc == 0) {
					return arg0.meshPart.primitiveType - arg1.meshPart.primitiveType;
				}
				return mc;
			}
			return vc;
		}
	}

	private Array<Renderable> renderables = new Array<Renderable>();

    private class RenderablePool : FlushablePool<Renderable>
    {
        internal protected override Renderable newObject()
        {
            return new Renderable();
        }
    }

	private FlushablePool<Renderable> renderablesPool = new RenderablePool();

private class MeshPartPool : FlushablePool<MeshPart>
{
    protected internal override MeshPart newObject()
    {
        return new MeshPart();
    }
}

	private FlushablePool<MeshPart> meshPartPool = new MeshPartPool();

	private Array<Renderable> items = new Array<Renderable>();
	private Array<Renderable> tmp = new Array<Renderable>();

	private MeshBuilder meshBuilder;
	private bool building;
	private RenderableSorter sorter;
	private MeshPool meshPool;
	private Camera camera;

	/** Create a ModelCache using the default {@link Sorter} and the {@link SimpleMeshPool} implementation. This might not be the
	 * most optimal implementation for you use-case, but should be good to start with. */
	public ModelCache () 
    : this(new Sorter(), new SimpleMeshPool())
    {
		
	}

	/** Create a ModelCache using the specified {@link RenderableSorter} and {@link MeshPool} implementation. The
	 * {@link RenderableSorter} implementation will be called with the camera specified in {@link #begin(Camera)}. By default this
	 * will be null. The sorter is important for optimizing the cache. For the best result, make sure that renderables that can be
	 * merged are next to each other. */
	public ModelCache (RenderableSorter sorter, MeshPool meshPool) {
		this.sorter = sorter;
		this.meshPool = meshPool;
		meshBuilder = new MeshBuilder();
	}

	/** Begin creating the cache, must be followed by a call to {@link #end()}, in between these calls one or more calls to one of
	 * the add(...) methods can be made. Calling this method will clear the cache and prepare it for creating a new cache. The
	 * cache is not valid until the call to {@link #end()} is made. Use one of the add methods (e.g. {@link #add(Renderable)} or
	 * {@link #add(RenderableProvider)}) to add renderables to the cache. */
	public void begin () {
		begin(null);
	}

	/** Begin creating the cache, must be followed by a call to {@link #end()}, in between these calls one or more calls to one of
	 * the add(...) methods can be made. Calling this method will clear the cache and prepare it for creating a new cache. The
	 * cache is not valid until the call to {@link #end()} is made. Use one of the add methods (e.g. {@link #add(Renderable)} or
	 * {@link #add(RenderableProvider)}) to add renderables to the cache.
	 * @param camera The {@link Camera} that will passed to the {@link RenderableSorter} */
	public void begin (Camera camera) {
		if (building) throw new GdxRuntimeException("Call end() after calling begin()");
		building = true;

		this.camera = camera;
		renderablesPool.flush();
		renderables.clear();
		items.clear();
		meshPartPool.flush();
		meshPool.flush();
	}

	private Renderable obtainRenderable (Material material, int primitiveType) {
		Renderable result = renderablesPool.obtain();
		result.bones = null;
		result.environment = null;
		result.material = material;
		result.meshPart.mesh = null;
		result.meshPart.offset = 0;
		result.meshPart.size = 0;
		result.meshPart.primitiveType = primitiveType;
		result.meshPart.center.Set(0, 0, 0);
		result.meshPart.halfExtents.Set(0, 0, 0);
		result.meshPart.radius = -1f;
		result.shader = null;
		result.userData = null;
		result.worldTransform.idt();
		return result;
	}

	/** Finishes creating the cache, must be called after a call to {@link #begin()}, only after this call the cache will be valid
	 * (until the next call to {@link #begin()}). Calling this method will process all renderables added using one of the add(...)
	 * methods and will combine them if possible. */
	public void end () {
		if (!building) throw new GdxRuntimeException("Call begin() prior to calling end()");
		building = false;

		if (items.size == 0) return;
		sorter.sort(camera, items);

		int itemCount = items.size;
		int initCount = renderables.size;

		 Renderable first = items.Get(0);
		VertexAttributes vertexAttributes = first.meshPart.mesh.getVertexAttributes();
		Material material = first.material;
		int primitiveType = first.meshPart.primitiveType;
		int offset = renderables.size;

		meshBuilder.begin(vertexAttributes);
		MeshPart part = meshBuilder.part("", primitiveType, meshPartPool.obtain());
		renderables.Add(obtainRenderable(material, primitiveType));

		for (int i = 0, n = items.size; i < n; ++i) {
			 Renderable renderable = items.Get(i);
			 VertexAttributes va = renderable.meshPart.mesh.getVertexAttributes();
			 Material mat = renderable.material;
			 int pt = renderable.meshPart.primitiveType;

			 bool sameAttributes = va.Equals(vertexAttributes);
			 bool indexedMesh = renderable.meshPart.mesh.getNumIndices() > 0;
			 int verticesToAdd = indexedMesh ? renderable.meshPart.mesh.getNumVertices() : renderable.meshPart.size;
			 bool canHoldVertices = meshBuilder.getNumVertices() + verticesToAdd <= MeshBuilder.MAX_VERTICES;
			 bool sameMesh = sameAttributes && canHoldVertices;
			 bool samePart = sameMesh && pt == primitiveType && mat.same(material, true);

			if (!samePart) {
				if (!sameMesh) {
					 Mesh mesh = meshBuilder
						.end(meshPool.obtain(vertexAttributes, meshBuilder.getNumVertices(), meshBuilder.getNumIndices()));
					while (offset < renderables.size)
						renderables.Get(offset++).meshPart.mesh = mesh;
					meshBuilder.begin(vertexAttributes = va);
				}

				 MeshPart newPart = meshBuilder.part("", pt, meshPartPool.obtain());
				 Renderable previous = renderables.Get(renderables.size - 1);
				previous.meshPart.offset = part.offset;
				previous.meshPart.size = part.size;
				part = newPart;

				renderables.Add(obtainRenderable(material = mat, primitiveType = pt));
			}

			meshBuilder.setVertexTransform(renderable.worldTransform);
			meshBuilder.addMesh(renderable.meshPart.mesh, renderable.meshPart.offset, renderable.meshPart.size);
		}

        {
            Mesh mesh = meshBuilder
                .end(meshPool.obtain(vertexAttributes, meshBuilder.getNumVertices(), meshBuilder.getNumIndices()));
            while (offset < renderables.size)
                renderables.Get(offset++).meshPart.mesh = mesh;

            Renderable previous = renderables.Get(renderables.size - 1);
            previous.meshPart.offset = part.offset;
            previous.meshPart.size = part.size;
        }
    }

	/** Adds the specified {@link Renderable} to the cache. Must be called in between a call to {@link #begin()} and
	 * {@link #end()}. All member objects might (depending on possibilities) be used by reference and should not change while the
	 * cache is used. If the {@link Renderable#bones} member is not null then skinning is assumed and the renderable will be added
	 * as-is, by reference. Otherwise the renderable will be merged with other renderables as much as possible, depending on the
	 * {@link Mesh#getVertexAttributes()}, {@link Renderable#material} and primitiveType (in that order). The
	 * {@link Renderable#environment}, {@link Renderable#shader} and {@link Renderable#userData} values (if any) are removed.
	 * @param renderable The {@link Renderable} to add, should not change while the cache is needed. */
	public void add (Renderable renderable) {
		if (!building) throw new GdxRuntimeException("Can only add items to the ModelCache in between .begin() and .end()");
		if (renderable.bones == null)
			items.Add(renderable);
		else
			renderables.Add(renderable);
	}

	/** Adds the specified {@link RenderableProvider} to the cache, see {@link #add(Renderable)}. */
	public void add ( RenderableProvider renderableProvider) {
		renderableProvider.getRenderables(tmp, renderablesPool);
		for (int i = 0, n = tmp.size; i < n; ++i)
			add(tmp.Get(i));
		tmp.clear();
	}

	/** Adds the specified {@link RenderableProvider}s to the cache, see {@link #add(Renderable)}. */
	public void add <T>(IEnumerable<T> renderableProviders)
	where T: RenderableProvider{
		foreach ( RenderableProvider renderableProvider in renderableProviders)
			add(renderableProvider);
	}

	public void getRenderables (Array<Renderable> renderables, Pool<Renderable> pool) {
		if (building) throw new GdxRuntimeException("Cannot render a ModelCache in between .begin() and .end()");
		foreach (Renderable r in this.renderables) {
			r.shader = null;
			r.environment = null;
		}
		renderables.addAll(this.renderables);
	}

	public void Dispose () {
		if (building) throw new GdxRuntimeException("Cannot dispose a ModelCache in between .begin() and .end()");
		meshPool.Dispose();
	}
}
