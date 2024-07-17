using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.GLUtils
{
	/**
 * <p>
 * A {@link VertexData} implementation based on OpenGL vertex buffer objects.
 * <p>
 * If the OpenGL ES context was lost you can call {@link #invalidate()} to recreate a new OpenGL vertex buffer object.
 * <p>
 * The data is bound via glVertexAttribPointer() according to the attribute aliases specified via {@link VertexAttributes} in the
 * constructor.
 * <p>
 * VertexBufferObjects must be disposed via the {@link #dispose()} method when no longer needed
 *
 * @author mzechner, Dave Clayton <contact@redskyforge.com> */
public class VertexBufferObject : IVertexData {
	private VertexAttributes attributes;
	private FloatBuffer buffer;
	private ByteBuffer byteBuffer;
	private bool ownsBuffer;
	private int bufferHandle;
	private int usage;
	bool isDirty = false;
	bool isBound = false;

	/** Constructs a new interleaved VertexBufferObject.
	 *
	 * @param isStatic whether the vertex data is static.
	 * @param numVertices the maximum number of vertices
	 * @param attributes the {@link VertexAttribute}s. */
	public VertexBufferObject (bool isStatic, int numVertices, VertexAttribute[] attributes) 
	: this(isStatic, numVertices, new VertexAttributes(attributes))
	{
		
	}

	/** Constructs a new interleaved VertexBufferObject.
	 *
	 * @param isStatic whether the vertex data is static.
	 * @param numVertices the maximum number of vertices
	 * @param attributes the {@link VertexAttributes}. */
	public VertexBufferObject (bool isStatic, int numVertices, VertexAttributes attributes) {
		bufferHandle = Gdx.gl20.glGenBuffer();

		ByteBuffer data = BufferUtils.newUnsafeByteBuffer(attributes.vertexSize * numVertices);
		((Buffer)data).limit(0);
		setBuffer(data, true, attributes);
		setUsage(isStatic ? GL20.GL_STATIC_DRAW : GL20.GL_DYNAMIC_DRAW);
	}

	protected VertexBufferObject (int usage, ByteBuffer data, bool ownsBuffer, VertexAttributes attributes) {
		bufferHandle = Gdx.gl20.glGenBuffer();

		setBuffer(data, ownsBuffer, attributes);
		setUsage(usage);
	}

	public VertexAttributes getAttributes () {
		return attributes;
	}

	public int getNumVertices () {
		return buffer.limit() * 4 / attributes.vertexSize;
	}

	public int getNumMaxVertices () {
		return byteBuffer.capacity() / attributes.vertexSize;
	}

	public FloatBuffer getBuffer (bool forWriting) {
		isDirty |= forWriting;
		return buffer;
	}

	/** Low level method to reset the buffer and attributes to the specified values. Use with care!
	 * @param data
	 * @param ownsBuffer
	 * @param value */
	protected void setBuffer (Buffer data, bool ownsBuffer, VertexAttributes value) {
		if (isBound) throw new GdxRuntimeException("Cannot change attributes while VBO is bound");
		if (this.ownsBuffer && byteBuffer != null) BufferUtils.disposeUnsafeByteBuffer(byteBuffer);
		attributes = value;
		if (data is ByteBuffer)
			byteBuffer = (ByteBuffer)data;
		else
			throw new GdxRuntimeException("Only ByteBuffer is currently supported");
		this.ownsBuffer = ownsBuffer;

		int l = byteBuffer.limit();
		((Buffer)byteBuffer).limit(byteBuffer.capacity());
		buffer = byteBuffer.asFloatBuffer();
		((Buffer)byteBuffer).limit(l);
		((Buffer)buffer).limit(l / 4);
	}

	private void bufferChanged () {
		if (isBound) {
			Gdx.gl20.glBufferData(GL20.GL_ARRAY_BUFFER, byteBuffer.limit(), byteBuffer, usage);
			isDirty = false;
		}
	}

	public void setVertices (float[] vertices, int offset, int count) {
		isDirty = true;
		BufferUtils.copy(vertices, byteBuffer, count, offset);
		((Buffer)buffer).position(0);
		((Buffer)buffer).limit(count);
		bufferChanged();
	}

	public void updateVertices (int targetOffset, float[] vertices, int sourceOffset, int count) {
		isDirty = true;
		 int pos = byteBuffer.position();
		((Buffer)byteBuffer).position(targetOffset * 4);
		BufferUtils.copy(vertices, sourceOffset, count, byteBuffer);
		((Buffer)byteBuffer).position(pos);
		((Buffer)buffer).position(0);
		bufferChanged();
	}

	/** @return The GL enum used in the call to {@link GL20#glBufferData(int, int, java.nio.Buffer, int)}, e.g. GL_STATIC_DRAW or
	 *         GL_DYNAMIC_DRAW */
	protected int getUsage () {
		return usage;
	}

	/** Set the GL enum used in the call to {@link GL20#glBufferData(int, int, java.nio.Buffer, int)}, can only be called when the
	 * VBO is not bound. */
	protected void setUsage (int value) {
		if (isBound) throw new GdxRuntimeException("Cannot change usage while VBO is bound");
		usage = value;
	}

	/** Binds this VertexBufferObject for rendering via glDrawArrays or glDrawElements
	 * @param shader the shader */
	public void bind (ShaderProgram shader) {
		bind(shader, null);
	}

	public void bind (ShaderProgram shader, int[] locations) {
		GL20 gl = Gdx.gl20;

		gl.glBindBuffer(GL20.GL_ARRAY_BUFFER, bufferHandle);
		if (isDirty) {
			((Buffer)byteBuffer).limit(buffer.limit() * 4);
			gl.glBufferData(GL20.GL_ARRAY_BUFFER, byteBuffer.limit(), byteBuffer, usage);
			isDirty = false;
		}

		int numAttributes = attributes.size();
		if (locations == null) {
			for (int i = 0; i < numAttributes; i++) {
				 VertexAttribute attribute = attributes.get(i);
				 int location = shader.getAttributeLocation(attribute.alias);
				if (location < 0) continue;
				shader.enableVertexAttribute(location);

				shader.setVertexAttribute(location, attribute.numComponents, attribute.type, attribute.normalized,
					attributes.vertexSize, attribute.offset);
			}

		} else {
			for (int i = 0; i < numAttributes; i++) {
				 VertexAttribute attribute = attributes.get(i);
				 int location = locations[i];
				if (location < 0) continue;
				shader.enableVertexAttribute(location);

				shader.setVertexAttribute(location, attribute.numComponents, attribute.type, attribute.normalized,
					attributes.vertexSize, attribute.offset);
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
		 GL20 gl = Gdx.gl20;
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
		gl.glBindBuffer(GL20.GL_ARRAY_BUFFER, 0);
		isBound = false;
	}

	/** Invalidates the VertexBufferObject so a new OpenGL buffer handle is created. Use this in case of a context loss. */
	public void invalidate () {
		bufferHandle = Gdx.gl20.glGenBuffer();
		isDirty = true;
	}

	/** Disposes of all resources this VertexBufferObject uses. */
	public void dispose () {
		GL20 gl = Gdx.gl20;
		gl.glBindBuffer(GL20.GL_ARRAY_BUFFER, 0);
		gl.glDeleteBuffer(bufferHandle);
		bufferHandle = 0;
		if (ownsBuffer) BufferUtils.disposeUnsafeByteBuffer(byteBuffer);
	}
}
}
