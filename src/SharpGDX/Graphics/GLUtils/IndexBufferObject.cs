﻿using System;
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
 * In IndexBufferObject wraps OpenGL's index buffer functionality to be used in conjunction with VBOs. This class can be
 * seamlessly used with OpenGL ES 1.x and 2.0.
 * </p>
 *
 * <p>
 * Uses indirect Buffers on Android 1.5/1.6 to fix GC invocation due to leaking PlatformAddress instances.
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
 * @author mzechner, Thorsten Schleinzer */
public class IndexBufferObject : IndexData {
	readonly ShortBuffer buffer;
	readonly ByteBuffer byteBuffer;
	readonly bool ownsBuffer;
	int bufferHandle;
	readonly bool isDirect;
	bool isDirty = true;
	bool isBound = false;
	readonly int usage;

	// used to work around bug: https://android-review.googlesource.com/#/c/73175/
	private readonly bool empty;

	/** Creates a new static IndexBufferObject to be used with vertex arrays.
	 *
	 * @param maxIndices the maximum number of indices this buffer can hold */
	public IndexBufferObject (int maxIndices) 
	: this(true, maxIndices)
	{
		
	}

	/** Creates a new IndexBufferObject.
	 *
	 * @param isStatic whether the index buffer is static
	 * @param maxIndices the maximum number of indices this buffer can hold */
	public IndexBufferObject (bool isStatic, int maxIndices) {

		empty = maxIndices == 0;
		if (empty) {
			maxIndices = 1; // avoid allocating a zero-sized buffer because of bug in Android's ART < Android 5.0
		}

		byteBuffer = BufferUtils.newUnsafeByteBuffer(maxIndices * 2);
		isDirect = true;

		buffer = byteBuffer.asShortBuffer();
		ownsBuffer = true;
		((Buffer)buffer).flip();
		((Buffer)byteBuffer).flip();
		bufferHandle = GDX.GL20.glGenBuffer();
		usage = isStatic ? IGL20.GL_STATIC_DRAW : IGL20.GL_DYNAMIC_DRAW;
	}

	public IndexBufferObject (bool isStatic, ByteBuffer data) {

		empty = data.limit() == 0;
		byteBuffer = data;
		isDirect = true;

		buffer = byteBuffer.asShortBuffer();
		ownsBuffer = false;
		bufferHandle = GDX.GL20.glGenBuffer();
		usage = isStatic ? IGL20.GL_STATIC_DRAW : IGL20.GL_DYNAMIC_DRAW;
	}

	/** @return the number of indices currently stored in this buffer */
	public int getNumIndices () {
		return empty ? 0 : buffer.limit();
	}

	/** @return the maximum number of indices this IndexBufferObject can store. */
	public int getNumMaxIndices () {
		return empty ? 0 : buffer.capacity();
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
	 * @param count the number of shorts to copy */
	public void setIndices (short[] indices, int offset, int count) {
		isDirty = true;
		((Buffer)buffer).clear();
		buffer.put(indices, offset, count);
		((Buffer)buffer).flip();
		((Buffer)byteBuffer).position(0);
		((Buffer)byteBuffer).limit(count << 1);

		if (isBound) {
			GDX.GL20.glBufferData(IGL20.GL_ELEMENT_ARRAY_BUFFER, byteBuffer.limit(), byteBuffer, usage);
			isDirty = false;
		}
	}

	public void setIndices (ShortBuffer indices) {
		isDirty = true;
		int pos = indices.position();
		((Buffer)buffer).clear();
		buffer.put(indices);
		((Buffer)buffer).flip();
		((Buffer)indices).position(pos);
		((Buffer)byteBuffer).position(0);
		((Buffer)byteBuffer).limit(buffer.limit() << 1);

		if (isBound) {
			GDX.GL20.glBufferData(IGL20.GL_ELEMENT_ARRAY_BUFFER, byteBuffer.limit(), byteBuffer, usage);
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
			GDX.GL20.glBufferData(IGL20.GL_ELEMENT_ARRAY_BUFFER, byteBuffer.limit(), byteBuffer, usage);
			isDirty = false;
		}
	}

	public ShortBuffer getBuffer (bool forWriting) {
		isDirty |= forWriting;
		return buffer;
	}

	/** Binds this IndexBufferObject for rendering with glDrawElements. */
	public void bind () {
		if (bufferHandle == 0) throw new GdxRuntimeException("No buffer allocated!");

		GDX.GL20.glBindBuffer(IGL20.GL_ELEMENT_ARRAY_BUFFER, bufferHandle);
		if (isDirty) {
			((Buffer)byteBuffer).limit(buffer.limit() * 2);
			GDX.GL20.glBufferData(IGL20.GL_ELEMENT_ARRAY_BUFFER, byteBuffer.limit(), byteBuffer, usage);
			isDirty = false;
		}
		isBound = true;
	}

	/** Unbinds this IndexBufferObject. */
	public void unbind () {
		GDX.GL20.glBindBuffer(IGL20.GL_ELEMENT_ARRAY_BUFFER, 0);
		isBound = false;
	}

	/** Invalidates the IndexBufferObject so a new OpenGL buffer handle is created. Use this in case of a context loss. */
	public void invalidate () {
		bufferHandle = GDX.GL20.glGenBuffer();
		isDirty = true;
	}

	/** Disposes this IndexBufferObject and all its associated OpenGL resources. */
	public void Dispose () {
		GDX.GL20.glBindBuffer(IGL20.GL_ELEMENT_ARRAY_BUFFER, 0);
		GDX.GL20.glDeleteBuffer(bufferHandle);
		bufferHandle = 0;

		if (ownsBuffer) {
			BufferUtils.disposeUnsafeByteBuffer(byteBuffer);
		}
	}
}
}
