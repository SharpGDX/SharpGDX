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
 * @author mzechner */
public class VertexBufferObjectSubData : IVertexData {
	readonly VertexAttributes attributes;
	readonly FloatBuffer buffer;
	readonly ByteBuffer byteBuffer;
	int bufferHandle;
	readonly bool isDirect;
	readonly bool isStatic;
	readonly int usage;
	bool isDirty = false;
	bool isBound = false;

	/** Constructs a new interleaved VertexBufferObject.
	 *
	 * @param isStatic whether the vertex data is static.
	 * @param numVertices the maximum number of vertices
	 * @param attributes the {@link VertexAttributes}. */
	public VertexBufferObjectSubData (bool isStatic, int numVertices, VertexAttribute[] attributes) 
	: this(isStatic, numVertices, new VertexAttributes(attributes))
	{
		
	}

	/** Constructs a new interleaved VertexBufferObject.
	 *
	 * @param isStatic whether the vertex data is static.
	 * @param numVertices the maximum number of vertices
	 * @param attributes the {@link VertexAttribute}s. */
	public VertexBufferObjectSubData (bool isStatic, int numVertices, VertexAttributes attributes) {
		this.isStatic = isStatic;
		this.attributes = attributes;
		byteBuffer = BufferUtils.newByteBuffer(this.attributes.vertexSize * numVertices);
		isDirect = true;

		usage = isStatic ? GL20.GL_STATIC_DRAW : GL20.GL_DYNAMIC_DRAW;
		buffer = byteBuffer.asFloatBuffer();
		bufferHandle = createBufferObject();
		((Buffer)buffer).flip();
		((Buffer)byteBuffer).flip();
	}

	private int createBufferObject () {
		int result = Gdx.gl20.glGenBuffer();
		Gdx.gl20.glBindBuffer(GL20.GL_ARRAY_BUFFER, result);
		Gdx.gl20.glBufferData(GL20.GL_ARRAY_BUFFER, byteBuffer.capacity(), null, usage);
		Gdx.gl20.glBindBuffer(GL20.GL_ARRAY_BUFFER, 0);
		return result;
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

	private void bufferChanged () {
		if (isBound) {
			Gdx.gl20.glBufferSubData(GL20.GL_ARRAY_BUFFER, 0, byteBuffer.limit(), byteBuffer);
			isDirty = false;
		}
	}

	public void setVertices (float[] vertices, int offset, int count) {
		isDirty = true;
		if (isDirect) {
			BufferUtils.copy(vertices, byteBuffer, count, offset);
			((Buffer)buffer).position(0);
			((Buffer)buffer).limit(count);
		} else {
			((Buffer)buffer).clear();
			buffer.put(vertices, offset, count);
			((Buffer)buffer).flip();
			((Buffer)byteBuffer).position(0);
			((Buffer)byteBuffer).limit(buffer.limit() << 2);
		}

		bufferChanged();
	}

	public void updateVertices (int targetOffset, float[] vertices, int sourceOffset, int count) {
		isDirty = true;
		if (isDirect) {
			int pos = byteBuffer.position();
			((Buffer)byteBuffer).position(targetOffset * 4);
			BufferUtils.copy(vertices, sourceOffset, count, byteBuffer);
			((Buffer)byteBuffer).position(pos);
		} else
			throw new GdxRuntimeException("Buffer must be allocated direct."); // Should never happen

		bufferChanged();
	}

	/** Binds this VertexBufferObject for rendering via glDrawArrays or glDrawElements
	 *
	 * @param shader the shader */
	public void bind ( ShaderProgram shader) {
		bind(shader, null);
	}

	public void bind ( ShaderProgram shader,  int[] locations) {
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
	public void unbind ( ShaderProgram shader) {
		unbind(shader, null);
	}

	public void unbind ( ShaderProgram shader,  int[] locations) {
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
		bufferHandle = createBufferObject();
		isDirty = true;
	}

	/** Disposes of all resources this VertexBufferObject uses. */
	public void dispose () {
		GL20 gl = Gdx.gl20;
		gl.glBindBuffer(GL20.GL_ARRAY_BUFFER, 0);
		gl.glDeleteBuffer(bufferHandle);
		bufferHandle = 0;
	}

	/** Returns the VBO handle
	 * @return the VBO handle */
	public int getBufferHandle () {
		return bufferHandle;
	}
}
}
