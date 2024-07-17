using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Utils.Compression;

namespace SharpGDX.Graphics.GLUtils
{
	/**
 * <p>
 * Convenience class for working with OpenGL vertex arrays. It interleaves all data in the order you specified in the constructor
 * via {@link VertexAttribute}.
 * </p>
 *
 * <p>
 * This class is not compatible with OpenGL 3+ core profiles. For this {@link VertexBufferObject}s are needed.
 * </p>
 *
 * @author mzechner, Dave Clayton <contact@redskyforge.com> */
public class VertexArray : IVertexData {
	readonly VertexAttributes attributes;
	readonly FloatBuffer buffer;
	readonly ByteBuffer byteBuffer;
	private byte[] bb;
	bool isBound = false;

	/** Constructs a new interleaved VertexArray
	 *
	 * @param numVertices the maximum number of vertices
	 * @param attributes the {@link VertexAttribute}s */
	public VertexArray (int numVertices, VertexAttribute[] attributes) 
	: this(numVertices, new VertexAttributes(attributes))
	{
		
	}

	/** Constructs a new interleaved VertexArray
	 *
	 * @param numVertices the maximum number of vertices
	 * @param attributes the {@link VertexAttributes} */
	public VertexArray (int numVertices, VertexAttributes attributes) {
		this.attributes = attributes;
		byteBuffer = BufferUtils.newUnsafeByteBuffer(this.attributes.vertexSize * numVertices);
		buffer = byteBuffer.asFloatBuffer();
		buffer.flip();
		byteBuffer.flip();

		bb = new byte[this.attributes.vertexSize * numVertices];
	}

	public void dispose () {
		BufferUtils.disposeUnsafeByteBuffer(byteBuffer);
	}
		
	public FloatBuffer getBuffer (bool forWriting) {
		return buffer;
	}

	public int getNumVertices () {
		return buffer.limit() * 4 / attributes.vertexSize;
	}

	public int getNumMaxVertices () {
		return byteBuffer.capacity() / attributes.vertexSize;
	}

	public void setVertices(float[] vertices, int offset, int count)
	{
			// TODO: Is this the proper use of offset and count?
			BufferUtils.copy(vertices, byteBuffer, count, offset);
			BufferUtils.copy(vertices, buffer, count, offset);
			buffer.position(0);
			buffer.limit(count);
			
			Array.Resize(ref bb, count << 2);

			// TODO: Is this the correct use of offset and count?
			System.Buffer.BlockCopy(vertices, 0, bb, offset, count << 2);

			var s = string.Join(", ", bb.Take(80));
	}

	public void updateVertices (int targetOffset, float[] vertices, int sourceOffset, int count) {
		int pos = byteBuffer.position();
		((Buffer)byteBuffer).position(targetOffset * 4);
		BufferUtils.copy(vertices, sourceOffset, count, byteBuffer);
		((Buffer)byteBuffer).position(pos);
	}

	public void bind (ShaderProgram shader) {
		bind(shader, null);
	}

	public void bind(ShaderProgram shader, int[] locations)
	{
		int numAttributes = attributes.size();
		((Buffer)byteBuffer).limit(buffer.limit() * 4);
		if (locations == null)
		{
			for (int i = 0; i < numAttributes; i++)
			{
				VertexAttribute attribute = attributes.get(i);
				int location = shader.getAttributeLocation(attribute.alias);
				if (location < 0) continue;
				shader.enableVertexAttribute(location);

				if (attribute.type == GL20.GL_FLOAT)
				{
						buffer.position(attribute.offset / 4);
						//shader.setVertexAttribute(location, attribute.numComponents, attribute.type, attribute.normalized,
						//	attributes.vertexSize, buffer);

						var farray = new float[buffer.limit()];
						Array.Copy(((FloatBuffer)buffer).array(), buffer.position(), farray, 0, farray.Length);
						var fs = string.Join(", ", farray);

						// TODO: This is not right, it's creating a new array each call, just trying to get it to work though. -RP
						// TODO: Is this the correct segment length from the original byte[]?
						
						var array = MemoryMarshal.Cast<byte, float>(bb.AsSpan(attribute.offset, bb.Length - attribute.offset))
						.ToArray();
					var s = string.Join(", ", array);


					if (s != fs)
					{
						var g = 1;
					}

						shader.setVertexAttribute(location, attribute.numComponents, attribute.type, attribute.normalized, attributes.vertexSize, array);
				}
				else
				{
						byteBuffer.position(attribute.offset);
						//shader.setVertexAttribute(location, attribute.numComponents, attribute.type, attribute.normalized,
						//	attributes.vertexSize, byteBuffer);


						var farray = new byte[byteBuffer.limit()];
						Array.Copy(((ByteBuffer)byteBuffer).array(), byteBuffer.position(), farray, 0, farray.Length);
						var fs = string.Join(", ", farray);
						// TODO: This is not right, it's creating a new array each call, just trying to get it to work though. -RP
						// TODO: Is this the correct segment length from the original byte[]?
						var array = bb.AsSpan(attribute.offset, bb.Length - attribute.offset)
							.ToArray();
						var s = string.Join(", ", array);

						if (s != fs)
						{
							var g = 1;
						}

						shader.setVertexAttribute(location, attribute.numComponents, attribute.type, attribute.normalized, attributes.vertexSize, array);
					}
				}
		}
		else
		{
			for (int i = 0; i < numAttributes; i++)
			{
				VertexAttribute attribute = attributes.get(i);
				int location = locations[i];
				if (location < 0) continue;
				shader.enableVertexAttribute(location);

				if (attribute.type == GL20.GL_FLOAT)
				{
					((Buffer)buffer).position(attribute.offset / 4);
					shader.setVertexAttribute(location, attribute.numComponents, attribute.type, attribute.normalized,
						attributes.vertexSize, buffer);
				}
				else
				{
					((Buffer)byteBuffer).position(attribute.offset);
					shader.setVertexAttribute(location, attribute.numComponents, attribute.type, attribute.normalized,
						attributes.vertexSize, byteBuffer);
				}
			}
		}

		isBound = true;
	}

	/** Unbinds this VertexBufferObject.
	 *
	 * @param shader the shader */
	public void unbind (ShaderProgram shader) {
		unbind(shader, null);
	}

	public void unbind (ShaderProgram shader, int[] locations) {
		 int numAttributes = attributes.size();
		if (locations == null) {
			for (int i = 0; i < numAttributes; i++) {
				shader.disableVertexAttribute(attributes.get(i).alias);
			}
		} else {
			for (int i = 0; i < numAttributes; i++) {
				 int location = locations[i];
				if (location >= 0) shader.disableVertexAttribute(location);
			}
		}
		isBound = false;
	}

	public VertexAttributes getAttributes () {
		return attributes;
	}

	public void invalidate () {
	}
}
}
