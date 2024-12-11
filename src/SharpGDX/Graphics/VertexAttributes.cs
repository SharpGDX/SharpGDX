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
    public sealed partial class VertexAttributes : IEnumerable<VertexAttribute>, IComparable<VertexAttributes>
    {
        /// <summary>
        /// The attributes in the order they were specified.
        /// </summary>
        private readonly VertexAttribute[] attributes;

        /// <summary>
        /// The size of a single vertex in bytes.
        /// </summary>
        public readonly int vertexSize;

        /// <summary>
        /// Cache of the value calculated by <see cref="GetMask()"/>.
        /// </summary>
        private long mask = -1;

        /// <summary>
        /// Cache for bone weight units.
        /// </summary>
        private int boneWeightUnits = -1;

        /// <summary>
        /// Cache for texture coordinate units.
        /// </summary>
        private int textureCoordinates = -1;

        private ReadonlyIterable<VertexAttribute>? iterable;

        /// <summary>
        /// Constructor, sets the vertex attributes in a specific order.
        /// </summary>
        /// <param name="attributes"></param>
        /// <exception cref="IllegalArgumentException"></exception>
        public VertexAttributes(params VertexAttribute[] attributes)
        {
            if (attributes.Length == 0) throw new IllegalArgumentException("attributes must be >= 1");

            VertexAttribute[] list = new VertexAttribute[attributes.Length];
           
            for (int i = 0; i < attributes.Length; i++)
                list[i] = attributes[i];

            this.attributes = list;
            vertexSize = CalculateOffsets();
        }

        /** Returns the offset for the first VertexAttribute with the specified usage.
         * @param usage The usage of the VertexAttribute. */
        public int getOffset(int usage, int defaultIfNotFound)
        {
            VertexAttribute vertexAttribute = findByUsage(usage);
          
            if (vertexAttribute == null) return defaultIfNotFound;
           
            return vertexAttribute.offset / 4;
        }

        /** Returns the offset for the first VertexAttribute with the specified usage.
         * @param usage The usage of the VertexAttribute. */
        public int getOffset(int usage)
        {
            return getOffset(usage, 0);
        }

        /** Returns the first VertexAttribute for the given usage.
         * @param usage The usage of the VertexAttribute to find. */
        public VertexAttribute? findByUsage(int usage)
        {
            int len = Size();
           
            for (int i = 0; i < len; i++)
                if (Get(i).usage == usage)
                    return Get(i);
            
            return null;
        }

        private int CalculateOffsets()
        {
            int count = 0;
            
            for (int i = 0; i < attributes.Length; i++)
            {
                VertexAttribute attribute = attributes[i];
                attribute.offset = count;
                count += attribute.getSizeInBytes();
            }

            return count;
        }

        /// <summary>
        /// Returns the number of attributes.
        /// </summary>
        /// <returns>The number of attributes.</returns>
        public int Size()
        {
            return attributes.Length;
        }

        /** @param index the index
         * @return the VertexAttribute at the given index */
        public VertexAttribute Get(int index)
        {
            return attributes[index];
        }

        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            
            for (int i = 0; i < attributes.Length; i++)
            {
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

        public override bool Equals(object? obj)
        {
            if (obj == this) return true;
            
            if (!(obj is VertexAttributes)) return false;
            
            VertexAttributes other = (VertexAttributes)obj;
           
            if (this.attributes.Length != other.attributes.Length) return false;
            
            for (int i = 0; i < attributes.Length; i++)
            {
                if (!attributes[i].equals(other.attributes[i])) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            long result = 61 * attributes.Length;
            
            for (int i = 0; i < attributes.Length; i++)
                result = result * 61 + attributes[i].GetHashCode();
            
            return (int)(result ^ (result >> 32));
        }

        /** Calculates a mask based on the contained {@link VertexAttribute} instances. The mask is a bit-wise or of each attributes
         * {@link VertexAttribute#usage}.
         * @return the mask */
        public long GetMask()
        {
            if (mask == -1)
            {
                long result = 0;
                
                for (int i = 0; i < attributes.Length; i++)
                {
                    result |= attributes[i].usage;
                }

                mask = result;
            }

            return mask;
        }

        /** Calculates the mask based on {@link VertexAttributes#getMask()} and packs the attributes count into the last 32 bits.
         * @return the mask with attributes count packed into the last 32 bits. */
        public long GetMaskWithSizePacked()
        {
            return GetMask() | ((long)attributes.Length << 32);
        }

        /// <summary>
        /// Returns the number of bone weights based on <see cref="VertexAttribute.unit"/>.
        /// </summary>
        /// <returns>The number of bone weights based on <see cref="VertexAttribute.unit"/>.</returns>
        public int GetBoneWeights()
        {
            if (boneWeightUnits < 0)
            {
                boneWeightUnits = 0;
                
                for (int i = 0; i < attributes.Length; i++)
                {
                    VertexAttribute a = attributes[i];
                    
                    if (a.usage == Usage.BoneWeight)
                    {
                        boneWeightUnits = Math.Max(boneWeightUnits, a.unit + 1);
                    }
                }
            }

            return boneWeightUnits;
        }

        /// <summary>
        /// Returns the number of texture coordinates based on <see cref="VertexAttribute.unit"/>.
        /// </summary>
        /// <returns>The number of texture coordinates based on <see cref="VertexAttribute.unit"/>.</returns>
        public int GetTextureCoordinates()
        {
            if (textureCoordinates < 0)
            {
                textureCoordinates = 0;
                
                for (int i = 0; i < attributes.Length; i++)
                {
                    VertexAttribute a = attributes[i];
                    
                    if (a.usage == Usage.TextureCoordinates)
                    {
                        textureCoordinates = Math.Max(textureCoordinates, a.unit + 1);
                    }
                }
            }

            return textureCoordinates;
        }

        public int CompareTo(VertexAttributes o)
        {
            if (attributes.Length != o.attributes.Length) return attributes.Length - o.attributes.Length;
           
            long m1 = GetMask();
            long m2 = o.GetMask();
            
            if (m1 != m2) return m1 < m2 ? -1 : 1;
            
            for (int i = attributes.Length - 1; i >= 0; --i)
            {
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
        public IEnumerator<VertexAttribute> GetEnumerator()
        {
            iterable ??= new ReadonlyIterable<VertexAttribute>(attributes);

            return iterable.GetEnumerator();
        }
    }
}