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
 * A {@link VertexData} implementation that uses vertex buffer objects and vertex array objects. (This is required for OpenGL 3.0+
 * core profiles. In particular, the default VAO has been deprecated, as has the use of client memory for passing vertex
 * attributes.) Use of VAOs should give a slight performance benefit since you don't have to bind the attributes on every draw
 * anymore.
 * </p>
 *
 * <p>
 * If the OpenGL ES context was lost you can call {@link #invalidate()} to recreate a new OpenGL vertex buffer object.
 * </p>
 *
 * <p>
 * VertexBufferObjectWithVAO objects must be disposed via the {@link #dispose()} method when no longer needed
 * </p>
 *
 * Code adapted from {@link VertexBufferObject}.
 * @author mzechner, Dave Clayton <contact@redskyforge.com>, Nate Austin <nate.austin gmail> */
public class VertexBufferObjectWithVAO : IVertexData {
	readonly static IntBuffer tmpHandle = BufferUtils.newIntBuffer(1);

	readonly VertexAttributes attributes;
	readonly FloatBuffer buffer;
	readonly ByteBuffer byteBuffer;
	readonly bool ownsBuffer;
	int bufferHandle;
	readonly bool isStatic;
	readonly int usage;
	bool isDirty = false;
	bool isBound = false;
	int vaoHandle = -1;
	IntArray cachedLocations = new IntArray();

	/** Constructs a new interleaved VertexBufferObjectWithVAO.
	 *
	 * @param isStatic whether the vertex data is static.
	 * @param numVertices the maximum number of vertices
	 * @param attributes the {@link com.badlogic.gdx.graphics.VertexAttribute}s. */
	public VertexBufferObjectWithVAO (bool isStatic, int numVertices, VertexAttribute[] attributes)
		:this(isStatic, numVertices, new VertexAttributes(attributes)){
		
	}

	/** Constructs a new interleaved VertexBufferObjectWithVAO.
	 *
	 * @param isStatic whether the vertex data is static.
	 * @param numVertices the maximum number of vertices
	 * @param attributes the {@link VertexAttributes}. */
	public VertexBufferObjectWithVAO (bool isStatic, int numVertices, VertexAttributes attributes) {
		this.isStatic = isStatic;
		this.attributes = attributes;

		byteBuffer = BufferUtils.newUnsafeByteBuffer(this.attributes.vertexSize * numVertices);
		buffer = byteBuffer.asFloatBuffer();
		ownsBuffer = true;
		((Buffer)buffer).flip();
		((Buffer)byteBuffer).flip();
		bufferHandle = GDX.GL20.glGenBuffer();
		usage = isStatic ? IGL20.GL_STATIC_DRAW : IGL20.GL_DYNAMIC_DRAW;
		createVAO();
	}

	public VertexBufferObjectWithVAO (bool isStatic, ByteBuffer unmanagedBuffer, VertexAttributes attributes) {
		this.isStatic = isStatic;
		this.attributes = attributes;

		byteBuffer = unmanagedBuffer;
		ownsBuffer = false;
		buffer = byteBuffer.asFloatBuffer();
		((Buffer)buffer).flip();
		((Buffer)byteBuffer).flip();
		bufferHandle = GDX.GL20.glGenBuffer();
		usage = isStatic ? IGL20.GL_STATIC_DRAW : IGL20.GL_DYNAMIC_DRAW;
		createVAO();
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
			GDX.GL20.glBindBuffer(IGL20.GL_ARRAY_BUFFER, bufferHandle);
			GDX.GL20.glBufferData(IGL20.GL_ARRAY_BUFFER, byteBuffer.limit(), byteBuffer, usage);
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

	/** Binds this VertexBufferObject for rendering via glDrawArrays or glDrawElements
	 *
	 * @param shader the shader */
	public void bind (ShaderProgram shader) {
		bind(shader, null);
	}

	public void bind (ShaderProgram shader, int[] locations) {
		IGL30 gl = GDX.GL30;

		gl.glBindVertexArray(vaoHandle);

		bindAttributes(shader, locations);

		// if our data has changed upload it:
		bindData(gl);

		isBound = true;
	}

	private void bindAttributes (ShaderProgram shader, int[] locations) {
		bool stillValid = this.cachedLocations.size != 0;
		int numAttributes = attributes.Size();

		if (stillValid) {
			if (locations == null) {
				for (int i = 0; stillValid && i < numAttributes; i++) {
					VertexAttribute attribute = attributes.Get(i);
					int location = shader.getAttributeLocation(attribute.alias);
					stillValid = location == this.cachedLocations.get(i);
				}
			} else {
				stillValid = locations.Length == this.cachedLocations.size;
				for (int i = 0; stillValid && i < numAttributes; i++) {
					stillValid = locations[i] == this.cachedLocations.get(i);
				}
			}
		}

		if (!stillValid) {
			GDX.GL.glBindBuffer(IGL20.GL_ARRAY_BUFFER, bufferHandle);
			unbindAttributes(shader);
			this.cachedLocations.clear();

			for (int i = 0; i < numAttributes; i++) {
				VertexAttribute attribute = attributes.Get(i);
				if (locations == null) {
					this.cachedLocations.add(shader.getAttributeLocation(attribute.alias));
				} else {
					this.cachedLocations.add(locations[i]);
				}

				int location = this.cachedLocations.get(i);
				if (location < 0) {
					continue;
				}

				shader.enableVertexAttribute(location);
				shader.setVertexAttribute(location, attribute.numComponents, attribute.type, attribute.normalized,
					attributes.vertexSize, attribute.offset);
			}
		}
	}

	private void unbindAttributes (ShaderProgram shaderProgram) {
		if (cachedLocations.size == 0) {
			return;
		}
		int numAttributes = attributes.Size();
		for (int i = 0; i < numAttributes; i++) {
			int location = cachedLocations.get(i);
			if (location < 0) {
				continue;
			}
			shaderProgram.disableVertexAttribute(location);
		}
	}

	private void bindData (IGL20 gl) {
		if (isDirty) {
			gl.glBindBuffer(IGL20.GL_ARRAY_BUFFER, bufferHandle);
			((Buffer)byteBuffer).limit(buffer.limit() * 4);
			gl.glBufferData(IGL20.GL_ARRAY_BUFFER, byteBuffer.limit(), byteBuffer, usage);
			isDirty = false;
		}
	}

	/** Unbinds this VertexBufferObject.
	 *
	 * @param shader the shader */
	public void unbind (ShaderProgram shader) {
		unbind(shader, null);
	}

	public void unbind (ShaderProgram shader, int[] locations) {
		IGL30 gl = GDX.GL30;
		gl.glBindVertexArray(0);
		isBound = false;
	}

	/** Invalidates the VertexBufferObject so a new OpenGL buffer handle is created. Use this in case of a context loss. */
	public void invalidate () {
		bufferHandle = GDX.GL30.glGenBuffer();
		createVAO();
		isDirty = true;
	}

	/** Disposes of all resources this VertexBufferObject uses. */
	public void Dispose () {
		IGL30 gl = GDX.GL30;

		gl.glBindBuffer(IGL20.GL_ARRAY_BUFFER, 0);
		gl.glDeleteBuffer(bufferHandle);
		bufferHandle = 0;
		if (ownsBuffer) {
			BufferUtils.disposeUnsafeByteBuffer(byteBuffer);
		}
		deleteVAO();
	}

	private void createVAO () {
		((Buffer)tmpHandle).clear();
		GDX.GL30.glGenVertexArrays(1, tmpHandle);
		vaoHandle = tmpHandle.get();
	}

	private void deleteVAO () {
		if (vaoHandle != -1) {
			((Buffer)tmpHandle).clear();
			tmpHandle.put(vaoHandle);
			((Buffer)tmpHandle).flip();
			GDX.GL30.glDeleteVertexArrays(1, tmpHandle);
			vaoHandle = -1;
		}
	}
}
}
