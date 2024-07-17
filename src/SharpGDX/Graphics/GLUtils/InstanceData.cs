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
	/** A InstanceData instance holds instance data for rendering with OpenGL. It is implemented as either a
 * {@link InstanceBufferObject} or a {@link InstanceBufferObjectSubData}. Both require Open GL 3.3+.
 *
 * @author mrdlink */
public interface InstanceData : Disposable {

	/** @return the number of vertices this InstanceData stores */
	public int getNumInstances ();

	/** @return the number of vertices this InstanceData can store */
	public int getNumMaxInstances ();

	/** @return the {@link VertexAttributes} as specified during construction. */
	public VertexAttributes getAttributes ();

	/** Sets the vertices of this InstanceData, discarding the old vertex data. The count must equal the number of floats per
	 * vertex times the number of vertices to be copied to this VertexData. The order of the vertex attributes must be the same as
	 * specified at construction time via {@link VertexAttributes}.
	 * <p>
	 * This can be called in between calls to bind and unbind. The vertex data will be updated instantly.
	 *
	 * @param data the instance data
	 * @param offset the offset to start copying the data from
	 * @param count the number of floats to copy */
	public void setInstanceData (float[] data, int offset, int count);

	/** Update (a portion of) the vertices. Does not resize the backing buffer.
	 *
	 * @param data the instance data
	 * @param sourceOffset the offset to start copying the data from
	 * @param count the number of floats to copy */
	public void updateInstanceData (int targetOffset, float[] data, int sourceOffset, int count);

	/** Sets the vertices of this InstanceData, discarding the old vertex data. The count must equal the number of floats per
	 * vertex times the number of vertices to be copied to this InstanceData. The order of the vertex attributes must be the same
	 * as specified at construction time via {@link VertexAttributes}.
	 * <p>
	 * This can be called in between calls to bind and unbind. The vertex data will be updated instantly.
	 *
	 * @param data the instance data
	 * @param count the number of floats to copy */
	public void setInstanceData (FloatBuffer data, int count);

	/** Update (a portion of) the vertices. Does not resize the backing buffer.
	 *
	 * @param data the vertex data
	 * @param sourceOffset the offset to start copying the data from
	 * @param count the number of floats to copy */
	public void updateInstanceData (int targetOffset, FloatBuffer data, int sourceOffset, int count);
		
	/** Returns the underlying FloatBuffer for reading or writing.
	 * @param forWriting when true, the underlying buffer will be uploaded on the next call to bind. If you need immediate
	 *           uploading use {@link #setInstanceData(float[], int, int)}.
	 * @return the underlying FloatBuffer holding the vertex data. */
	public FloatBuffer getBuffer (bool forWriting);

	/** Binds this InstanceData for rendering via glDrawArraysInstanced or glDrawElementsInstanced. */
	public void bind (ShaderProgram shader);

	/** Binds this InstanceData for rendering via glDrawArraysInstanced or glDrawElementsInstanced.
	 *
	 * @param locations array containing the attribute locations. */
	public void bind (ShaderProgram shader, int[] locations);

	/** Unbinds this InstanceData. */
	public void unbind (ShaderProgram shader);

	/** Unbinds this InstanceData.
	 *
	 * @param locations array containing the attribute locations. */
	public void unbind (ShaderProgram shader, int[] locations);

	/** Invalidates the InstanceData if applicable. Use this in case of a context loss. */
	public void invalidate ();

	/** Disposes this InstanceData and all its associated OpenGL resources. */
	public void dispose ();
}
}
