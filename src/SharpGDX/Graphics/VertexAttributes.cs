using System.Collections;
using SharpGDX.Shims;
using System.Text;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics
{
	/** Instances of this class specify the vertex attributes of a mesh. VertexAttributes are used by {@link Mesh} instances to define
 * its vertex structure. Vertex attributes have an order. The order is specified by the order they are added to this class.
 * 
 * @author mzechner, Xoppa */
public sealed class VertexAttributes : IEnumerable<VertexAttribute>, IComparable<VertexAttributes> {
	/** The usage of a vertex attribute.
	 * 
	 * @author mzechner */
	public static class Usage {
		public static readonly int Position = 1;
		public static readonly int ColorUnpacked = 2;
		public static readonly int ColorPacked = 4;
		public static readonly int Normal = 8;
		public static readonly int TextureCoordinates = 16;
		public static readonly int Generic = 32;
		public static readonly int BoneWeight = 64;
		public static readonly int Tangent = 128;
		public static readonly int BiNormal = 256;
	}

	/** the attributes in the order they were specified **/
	private readonly VertexAttribute[] attributes;

	/** the size of a single vertex in bytes **/
	public readonly int vertexSize;

	/** cache of the value calculated by {@link #getMask()} **/
	private long mask = -1;

	/** cache for bone weight units. */
	private int boneWeightUnits = -1;

	/** cache for texture coordinate units. */
	private int textureCoordinates = -1;

	private ReadonlyIterable<VertexAttribute> iterable;

	/** Constructor, sets the vertex attributes in a specific order */
	public VertexAttributes (VertexAttribute[] attributes) {
		if (attributes.Length == 0) throw new IllegalArgumentException("attributes must be >= 1");

		VertexAttribute[] list = new VertexAttribute[attributes.Length];
		for (int i = 0; i < attributes.Length; i++)
			list[i] = attributes[i];

		this.attributes = list;
		vertexSize = calculateOffsets();
	}

	/** Returns the offset for the first VertexAttribute with the specified usage.
	 * @param usage The usage of the VertexAttribute. */
	public int getOffset (int usage, int defaultIfNotFound) {
		VertexAttribute vertexAttribute = findByUsage(usage);
		if (vertexAttribute == null) return defaultIfNotFound;
		return vertexAttribute.offset / 4;
	}

	/** Returns the offset for the first VertexAttribute with the specified usage.
	 * @param usage The usage of the VertexAttribute. */
	public int getOffset (int usage) {
		return getOffset(usage, 0);
	}

	/** Returns the first VertexAttribute for the given usage.
	 * @param usage The usage of the VertexAttribute to find. */
	public VertexAttribute? findByUsage (int usage) {
		int len = size();
		for (int i = 0; i < len; i++)
			if (get(i).usage == usage) return get(i);
		return null;
	}

	private int calculateOffsets () {
		int count = 0;
		for (int i = 0; i < attributes.Length; i++) {
			VertexAttribute attribute = attributes[i];
			attribute.offset = count;
			count += attribute.getSizeInBytes();
		}

		return count;
	}

	/** @return the number of attributes */
	public int size () {
		return attributes.Length;
	}

	/** @param index the index
	 * @return the VertexAttribute at the given index */
	public VertexAttribute get (int index) {
		return attributes[index];
	}

	public override String ToString () {
		StringBuilder builder = new StringBuilder();
		builder.Append("[");
		for (int i = 0; i < attributes.Length; i++) {
			builder.Append("(");
			builder.Append(attributes[i].alias);
			builder.Append(", ");
			builder.Append(attributes[i].usage);
			builder.Append(", ");
			builder.Append(attributes[i].numComponents);
			builder.Append(", ");
			builder.Append(attributes[i].offset);
			builder.Append(")");
			builder.Append("\n");
		}
		builder.Append("]");
		return builder.ToString();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override bool Equals (Object? obj) {
		if (obj == this) return true;
		if (!(obj is VertexAttributes)) return false;
		VertexAttributes other = (VertexAttributes)obj;
		if (this.attributes.Length != other.attributes.Length) return false;
		for (int i = 0; i < attributes.Length; i++) {
			if (!attributes[i].equals(other.attributes[i])) return false;
		}
		return true;
	}

	public override int GetHashCode () {
		long result = 61 * attributes.Length;
		for (int i = 0; i < attributes.Length; i++)
			result = result * 61 + attributes[i].GetHashCode();
		return (int)(result ^ (result >> 32));
	}

	/** Calculates a mask based on the contained {@link VertexAttribute} instances. The mask is a bit-wise or of each attributes
	 * {@link VertexAttribute#usage}.
	 * @return the mask */
	public long getMask () {
		if (mask == -1) {
			long result = 0;
			for (int i = 0; i < attributes.Length; i++) {
				result |= attributes[i].usage;
			}
			mask = result;
		}
		return mask;
	}

	/** Calculates the mask based on {@link VertexAttributes#getMask()} and packs the attributes count into the last 32 bits.
	 * @return the mask with attributes count packed into the last 32 bits. */
	public long getMaskWithSizePacked () {
		return getMask() | ((long)attributes.Length << 32);
	}

	/** @return Number of bone weights based on {@link VertexAttribute#unit} */
	public int getBoneWeights () {
		if (boneWeightUnits < 0) {
			boneWeightUnits = 0;
			for (int i = 0; i < attributes.Length; i++) {
				VertexAttribute a = attributes[i];
				if (a.usage == Usage.BoneWeight) {
					boneWeightUnits = Math.Max(boneWeightUnits, a.unit + 1);
				}
			}
		}
		return boneWeightUnits;
	}

	/** @return Number of texture coordinates based on {@link VertexAttribute#unit} */
	public int getTextureCoordinates () {
		if (textureCoordinates < 0) {
			textureCoordinates = 0;
			for (int i = 0; i < attributes.Length; i++) {
				VertexAttribute a = attributes[i];
				if (a.usage == Usage.TextureCoordinates) {
					textureCoordinates = Math.Max(textureCoordinates, a.unit + 1);
				}
			}
		}
		return textureCoordinates;
	}

	public int CompareTo (VertexAttributes o) {
		if (attributes.Length != o.attributes.Length) return attributes.Length - o.attributes.Length;
		 long m1 = getMask();
		 long m2 = o.getMask();
		if (m1 != m2) return m1 < m2 ? -1 : 1;
		for (int i = attributes.Length - 1; i >= 0; --i) {
			 VertexAttribute va0 = attributes[i];
			 VertexAttribute va1 = o.attributes[i];
			if (va0.usage != va1.usage) return va0.usage - va1.usage;
			if (va0.unit != va1.unit) return va0.unit - va1.unit;
			if (va0.numComponents != va1.numComponents) return va0.numComponents - va1.numComponents;
			if (va0.normalized != va1.normalized) return va0.normalized ? 1 : -1;
			if (va0.type != va1.type) return va0.type - va1.type;
		}
		return 0;
	}

	/** @see Collections#allocateIterators */
	public IEnumerator<VertexAttribute> GetEnumerator () {
		if (iterable == null) iterable = new ReadonlyIterable<VertexAttribute>(attributes);
		return iterable.GetEnumerator();
	}

	private class ReadonlyIterator<T> : IEnumerator<T>, IEnumerable<T> {
		private readonly T[] array;
		internal int index;
		internal bool valid = true;

			public void Dispose(){}

		public ReadonlyIterator (T[] array) {
			this.array = array;
		}

		public bool MoveNext () {
			if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
			return index < array.Length;
		}

		public T Current
		{
			get
			{
				if (index >= array.Length) throw new NoSuchElementException(index.ToString());
				if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
				return array[index++];
			}
		}

		public void remove () {
			throw new GdxRuntimeException("Remove not allowed.");
		}

		public void Reset () {
			index = 0;
		}

		object IEnumerator.Current => Current;

		public IEnumerator<T> GetEnumerator () {
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private class ReadonlyIterable<T> : IEnumerable<T> {
		private readonly T[] array;
		internal ReadonlyIterator<T> iterator1, iterator2;

		public ReadonlyIterable (T[] array) {
			this.array = array;
		}

		public IEnumerator<T> GetEnumerator () {
			if (Collections.allocateIterators) return new ReadonlyIterator<T>(array);
			if (iterator1 == null) {
				iterator1 = new (array);
				iterator2 = new (array);
			}
			if (!iterator1.valid) {
				iterator1.index = 0;
				iterator1.valid = true;
				iterator2.valid = false;
				return iterator1;
			}
			iterator2.index = 0;
			iterator2.valid = true;
			iterator1.valid = false;
			return iterator2;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
}
