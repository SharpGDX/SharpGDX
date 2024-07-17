using SharpGDX.Shims;
using SharpGDX.Mathematics;
using SharpGDX.Mathematics.Collision;
using SharpGDX.Utils;

// TODO: I am really unsure of this class.

namespace SharpGDX.Mathematics
{
	/** A static Octree implementation.
 *
 * Example of usage:
 *
 * <pre>
 * Vector3 min = new Vector3(-10, -10, -10);
 * Vector3 max = new Vector3(10, 10, 10);
 * octree = new Octree<GameObject>(min, max, MAX_DEPTH, MAX_ITEMS_PER_NODE, new Octree.Collider<GameObject>() {
 * 	&#64;Override
 * 	public boolean intersects (BoundingBox nodeBounds, GameObject geometry) {
 * 		return nodeBounds.intersects(geometry.box);
 * 	}
 *
 * 	&#64;Override
 * 	public boolean intersects (Frustum frustum, GameObject geometry) {
 * 		return frustum.boundsInFrustum(geometry.box);
 * 	}
 *
 * 	&#64;Override
 * 	public float intersects (Ray ray, GameObject geometry) {
 * 		if (Intersector.intersectRayBounds(ray, geometry.box, new Vector3())) {
 * 			return tmp.dst2(ray.origin);
 * 		}
 * 		return Float.MAX_VALUE;
 * 	}
 * });
 *
 * // Adding game objects to the octree
 * octree.add(gameObject1);
 * octree.add(gameObject2);
 *
 * // Querying the result
 * ObjectSet<GameObject> result = new ObjectSet<>();
 * octree.query(cam.frustum, result);
 *
 * // Rendering the result
 * for (GameObject gameObject : result) {
 * 	modelBatch.render(gameObject);
 * }
 * </pre>
 */
public class Octree<T> {

	internal readonly int maxItemsPerNode;

	private static Pool<OctreeNode> nodePool;

	private class OctreeNodePool : Pool<OctreeNode>
	{
		private readonly Octree<T> _octree;

		public OctreeNodePool(Octree<T> octree)
		{
			_octree = octree;
		}
		protected override OctreeNode newObject()
		{
			return new OctreeNode(_octree);
		}
		}

	protected OctreeNode root;
	readonly ICollider<T> collider;

	static readonly Vector3 tmp = new Vector3();

	public Octree (Vector3 minimum, Vector3 maximum, int maxDepth, int maxItemsPerNode, ICollider<T> collider) {
			nodePool = new OctreeNodePool(this);
			Vector3 realMin = new Vector3(Math.Min(minimum.x, maximum.x), Math.Min(minimum.y, maximum.y),
			Math.Min(minimum.z, maximum.z));
		Vector3 realMax = new Vector3(Math.Max(minimum.x, maximum.x), Math.Max(minimum.y, maximum.y),
			Math.Max(minimum.z, maximum.z));

		this.root = createNode(realMin, realMax, maxDepth);
		this.collider = collider;
		this.maxItemsPerNode = maxItemsPerNode;
	}

	OctreeNode createNode (Vector3 min, Vector3 max, int level) {
		OctreeNode node = nodePool.obtain();
		node.bounds.set(min, max);
		node.level = level;
		node.leaf = true;
		return node;
	}

	public void add (T obj) {
		root.add(obj);
	}

	public void remove (T obj) {
		root.remove(obj);
	}

	public void update (T obj) {
		root.remove(obj);
		root.add(obj);
	}

	/** Method to retrieve all the geometries.
	 * @param resultSet
	 * @return the result set */
	public ObjectSet<T> getAll (ObjectSet<T> resultSet) {
		root.getAll(resultSet);
		return resultSet;
	}

	/** Method to query geometries inside nodes that the aabb intersects. Can be used as broad phase.
	 * @param aabb - The bounding box to query
	 * @param result - Set to be populated with objects inside the BoundingBoxes */
	public ObjectSet<T> query (BoundingBox aabb, ObjectSet<T> result) {
		root.query(aabb, result);
		return result;
	}

	/** Method to query geometries inside nodes that the frustum intersects. Can be used as broad phase.
	 * @param frustum - The frustum to query
	 * @param result set populated with objects near from the frustum */
	public ObjectSet<T> query (Frustum frustum, ObjectSet<T> result) {
		root.query(frustum, result);
		return result;
	}

	public T rayCast (Ray ray, RayCastResult<T> result) {
		result.distance = result.maxDistanceSq;
		root.rayCast(ray, result);
		return result.geometry;
	}

	/** Method to get nodes as bounding boxes. Useful for debug purpose.
	 *
	 * @param boxes */
	public ObjectSet<BoundingBox> getNodesBoxes (ObjectSet<BoundingBox> boxes) {
		root.getBoundingBox(boxes);
		return boxes;
	}

	protected class OctreeNode {
		private readonly Octree<T> _octree;

		public OctreeNode(Octree<T> octree)
		{
			_octree = octree;
			geometries = new Array<T>(Math.Min(16, _octree.maxItemsPerNode));
		}

		internal int level;
		internal readonly BoundingBox bounds = new BoundingBox();
		internal bool leaf;
		private Octree<T>.OctreeNode[] children; // May be null when leaf is true.
		private readonly Array<T> geometries ;

		private void split () {
			float midx = (bounds.max.x + bounds.min.x) * 0.5f;
			float midy = (bounds.max.y + bounds.min.y) * 0.5f;
			float midz = (bounds.max.z + bounds.min.z) * 0.5f;

			int deeperLevel = level - 1;

			leaf = false;
			if (children == null) children = new Octree<T>.OctreeNode[8];
			children[0] = _octree.createNode(new Vector3(bounds.min.x, midy, midz), new Vector3(midx, bounds.max.y, bounds.max.z),
				deeperLevel);
			children[1] = _octree.createNode(new Vector3(midx, midy, midz), new Vector3(bounds.max.x, bounds.max.y, bounds.max.z),
				deeperLevel);
			children[2] = _octree.createNode(new Vector3(midx, midy, bounds.min.z), new Vector3(bounds.max.x, bounds.max.y, midz),
				deeperLevel);
			children[3] = _octree.createNode(new Vector3(bounds.min.x, midy, bounds.min.z), new Vector3(midx, bounds.max.y, midz),
				deeperLevel);
			children[4] = _octree.createNode(new Vector3(bounds.min.x, bounds.min.y, midz), new Vector3(midx, midy, bounds.max.z),
				deeperLevel);
			children[5] = _octree.createNode(new Vector3(midx, bounds.min.y, midz), new Vector3(bounds.max.x, midy, bounds.max.z),
				deeperLevel);
			children[6] = _octree.createNode(new Vector3(midx, bounds.min.y, bounds.min.z), new Vector3(bounds.max.x, midy, midz),
				deeperLevel);
			children[7] = _octree.createNode(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z), new Vector3(midx, midy, midz),
				deeperLevel);

			// Move geometries from parent to children
			foreach (Octree<T>.OctreeNode child in children) {
				foreach (T geometry in this.geometries) {
					child.add(geometry);
				}
			}
			this.geometries.clear();
		}

		private void merge () {
			clearChildren();
			leaf = true;
		}

		private void free () {
			geometries.clear();
			if (!leaf) clearChildren();
			nodePool.free(this);
		}

		private void clearChildren () {
			for (int i = 0; i < 8; i++) {
				children[i].free();
				children[i] = null;
			}
		}

		internal protected void add (T geometry) {
			if (!_octree.collider.intersects(bounds, geometry)) {
				return;
			}

			// If is not leaf, check children
			if (!leaf) {
				foreach (Octree<T>.OctreeNode child in children) {
					child.add(geometry);
				}
			} else {
				if (geometries.size >= _octree.maxItemsPerNode && level > 0) {
					split();
					foreach (Octree<T>.OctreeNode child in children) {
						child.add(geometry);
					}
				} else {
					geometries.add(geometry);
				}
			}
		}

		internal protected bool remove (T obj) {
			if (!leaf) {
				bool removed = false;
				foreach (Octree<T>.OctreeNode node in children) {
					removed |= node.remove(obj);
				}

				if (removed) {
					ObjectSet<T> geometrySet = new ObjectSet<T>();
					foreach (Octree<T>.OctreeNode node in children) {
						node.getAll(geometrySet);
					}
					if (geometrySet.size <= _octree.maxItemsPerNode) {
						foreach (T geometry in geometrySet) {
							geometries.add(geometry);
						}
						merge();
					}
				}

				return removed;
			}
			return geometries.removeValue(obj, true);
		}

		protected bool isLeaf () {
			return leaf;
		}

		internal void query (BoundingBox aabb, ObjectSet<T> result) {
			if (!aabb.intersects(bounds)) {
				return;
			}

			if (!leaf) {
				foreach (Octree<T>.OctreeNode node in children) {
					node.query(aabb, result);
				}
			} else {
				foreach (T geometry in geometries) {
					// Filter geometries using collider
					if (_octree.collider.intersects(bounds, geometry)) {
						result.add(geometry);
					}
				}
			}
		}

		internal protected void query (Frustum frustum, ObjectSet<T> result) {
			if (!Intersector.intersectFrustumBounds(frustum, bounds)) {
				return;
			}
			if (!leaf) {
				foreach (Octree<T>.OctreeNode node in children) {
					node.query(frustum, result);
				}
			} else {
				foreach (T geometry in geometries) {
					// Filter geometries using collider
					if (_octree.collider.intersects(frustum, geometry)) {
						result.add(geometry);
					}
				}
			}
		}

		internal protected void rayCast (Ray ray, RayCastResult<T> result) {
			// Check intersection with node
			bool intersect = Intersector.intersectRayBounds(ray, bounds, tmp);
			if (!intersect) {
				return;
			} else {
				float dst2 = tmp.dst2(ray.origin);
				if (dst2 >= result.maxDistanceSq) {
					return;
				}
			}

			// Check intersection with children
			if (!leaf) {
				foreach (Octree<T>.OctreeNode child in children) {
					child.rayCast(ray, result);
				}
			} else {
				foreach (T geometry in geometries) {
					// Check intersection with geometries
					float distance = _octree.collider.intersects(ray, geometry);
					if (result.geometry == null || distance < result.distance) {
						result.geometry = geometry;
						result.distance = distance;
					}
				}
			}
		}

		/** Get all geometries using Depth-First Search recursion.
		 * @param resultSet */
		internal protected void getAll (ObjectSet<T> resultSet) {
			if (!leaf) {
				foreach (Octree<T>.OctreeNode child in children) {
					child.getAll(resultSet);
				}
			}
			resultSet.addAll(geometries);
		}

		/** Get bounding boxes using Depth-First Search recursion.
		 * @param bounds */
		internal protected void getBoundingBox (ObjectSet<BoundingBox> bounds) {
			if (!leaf) {
				foreach (Octree<T>.OctreeNode node in children) {
					node.getBoundingBox(bounds);
				}
			}
			bounds.add(this.bounds);
		}
	}

	/** Interface used by octree to handle geometries' collisions against BoundingBox, Frustum and Ray.
	 * @param <T> */
	public interface ICollider<T> {

		/** Method to calculate intersection between aabb and the geometry.
		 * @param nodeBounds
		 * @param geometry
		 * @return if they are intersecting */
		bool intersects (BoundingBox nodeBounds, T geometry);

		/** Method to calculate intersection between frustum and the geometry.
		 * @param frustum
		 * @param geometry
		 * @return if they are intersecting */
		bool intersects (Frustum frustum, T geometry);

		/** Method to calculate intersection between ray and the geometry.
		 * @param ray
		 * @param geometry
		 * @return distance between ray and geometry */
		float intersects (Ray ray, T geometry);
	}

	public class RayCastResult<T> {
		internal T geometry;
		internal float distance;
		internal float maxDistanceSq = float.MaxValue;
	}
}
}
