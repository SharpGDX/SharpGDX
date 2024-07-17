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
 * IndexBufferObject wraps OpenGL's index buffer functionality to be used in conjunction with VBOs.
 * </p>
 *
 * <p>
 * You can also use this to store indices for vertex arrays. Do not call {@link #bind()} or {@link #unbind()} in this case but
 * rather use {@link #getBuffer()} to use the buffer directly with glDrawElements. You must also create the IndexBufferObject with
 * the second constructor and specify isDirect as true as glDrawElements in conjunction with vertex arrays needs direct buffers.
 * </p>
 *
 * <p>
 * VertexBufferObjects must be disposed via the {@link #dispose()} method when no longer needed
 * </p>
 *
 * @author mzechner */
public class IndexBufferObjectSubData : IndexData {
	readonly ShortBuffer buffer;
	readonly ByteBuffer byteBuffer;
	int bufferHandle;
	readonly bool isDirect;
	bool isDirty = true;
	bool isBound = false;
	readonly int usage;

	/** Creates a new IndexBufferObject.
	 *
	 * @param isStatic whether the index buffer is static
	 * @param maxIndices the maximum number of indices this buffer can hold */
	public IndexBufferObjectSubData (bool isStatic, int maxIndices) {
		byteBuffer = BufferUtils.newByteBuffer(maxIndices * 2);
		isDirect = true;

		usage = isStatic ? GL20.GL_STATIC_DRAW : GL20.GL_DYNAMIC_DRAW;
		buffer = byteBuffer.asShortBuffer();
		((Buffer)buffer).flip();
		((Buffer)byteBuffer).flip();
		bufferHandle = createBufferObject();
	}

	/** Creates a new IndexBufferObject to be used with vertex arrays.
	 *
	 * @param maxIndices the maximum number of indices this buffer can hold */
	public IndexBufferObjectSubData (int maxIndices) {
		byteBuffer = BufferUtils.newByteBuffer(maxIndices * 2);
		this.isDirect = true;

		usage = GL20.GL_STATIC_DRAW;
		buffer = byteBuffer.asShortBuffer();
		((Buffer)buffer).flip();
		((Buffer)byteBuffer).flip();
		bufferHandle = createBufferObject();
	}

	private int createBufferObject () {
		int result = Gdx.gl20.glGenBuffer();
		Gdx.gl20.glBindBuffer(GL20.GL_ELEMENT_ARRAY_BUFFER, result);
		Gdx.gl20.glBufferData(GL20.GL_ELEMENT_ARRAY_BUFFER, byteBuffer.capacity(), null, usage);
		Gdx.gl20.glBindBuffer(GL20.GL_ELEMENT_ARRAY_BUFFER, 0);
		return result;
	}

	/** @return the number of indices currently stored in this buffer */
	public int getNumIndices () {
		return buffer.limit();
	}

	/** @return the maximum number of indices this IndexBufferObject can store. */
	public int getNumMaxIndices () {
		return buffer.capacity();
	}

	/**
	 * <p>
	 * Sets the indices of this IndexBufferObject, discarding the old indices. The count must equal the number of indices to be
	 * copied to this IndexBufferObject.
	 * </p>
	 *
	 * <p>
	 * This can be called in between calls to {@link #bind()} and {@link #unbind()}. The index data will be updated instantly.
	 * </p>
	 *
	 * @param indices the vertex data
	 * @param offset the offset to start copying the data from
	 * @param count the number of floats to copy */
	public void setIndices (short[] indices, int offset, int count) {
		isDirty = true;
		((Buffer)buffer).clear();
		buffer.put(indices, offset, count);
		((Buffer)buffer).flip();
		((Buffer)byteBuffer).position(0);
		((Buffer)byteBuffer).limit(count << 1);

		if (isBound) {
			Gdx.gl20.glBufferSubData(GL20.GL_ELEMENT_ARRAY_BUFFER, 0, byteBuffer.limit(), byteBuffer);
			isDirty = false;
		}
	}

	public void setIndices (ShortBuffer indices) {
		int pos = indices.position();
		isDirty = true;
		((Buffer)buffer).clear();
		buffer.put(indices);
		((Buffer)buffer).flip();
		((Buffer)indices).position(pos);
		((Buffer)byteBuffer).position(0);
		((Buffer)byteBuffer).limit(buffer.limit() << 1);

		if (isBound) {
			Gdx.gl20.glBufferSubData(GL20.GL_ELEMENT_ARRAY_BUFFER, 0, byteBuffer.limit(), byteBuffer);
			isDirty = false;
		}
	}

	public void updateIndices (int targetOffset, short[] indices, int offset, int count) {
		isDirty = true;
		int pos = byteBuffer.position();
		((Buffer)byteBuffer).position(targetOffset * 2);
		BufferUtils.copy(indices, offset, byteBuffer, count);
		((Buffer)byteBuffer).position(pos);
		((Buffer)buffer).position(0);

		if (isBound) {
			Gdx.gl20.glBufferSubData(GL20.GL_ELEMENT_ARRAY_BUFFER, 0, byteBuffer.limit(), byteBuffer);
			isDirty = false;
		}
	}
		
	public ShortBuffer getBuffer (bool forWriting) {
		isDirty |= forWriting;
		return buffer;
	}

	/** Binds this IndexBufferObject for rendering with glDrawElements. */
	public void bind () {
		if (bufferHandle == 0) throw new GdxRuntimeException("IndexBufferObject cannot be used after it has been disposed.");

		Gdx.gl20.glBindBuffer(GL20.GL_ELEMENT_ARRAY_BUFFER, bufferHandle);
		if (isDirty) {
			((Buffer)byteBuffer).limit(buffer.limit() * 2);
			Gdx.gl20.glBufferSubData(GL20.GL_ELEMENT_ARRAY_BUFFER, 0, byteBuffer.limit(), byteBuffer);
			isDirty = false;
		}
		isBound = true;
	}

	/** Unbinds this IndexBufferObject. */
	public void unbind () {
		Gdx.gl20.glBindBuffer(GL20.GL_ELEMENT_ARRAY_BUFFER, 0);
		isBound = false;
	}

	/** Invalidates the IndexBufferObject so a new OpenGL buffer handle is created. Use this in case of a context loss. */
	public void invalidate () {
		bufferHandle = createBufferObject();
		isDirty = true;
	}

	/** Disposes this IndexBufferObject and all its associated OpenGL resources. */
	public void dispose () {
		GL20 gl = Gdx.gl20;
		gl.glBindBuffer(GL20.GL_ELEMENT_ARRAY_BUFFER, 0);
		gl.glDeleteBuffer(bufferHandle);
		bufferHandle = 0;
	}
}
}
