using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.GLUtils
{
	/** A VertexData instance holds vertices for rendering with OpenGL. It is implemented as either a {@link VertexArray} or a
 * {@link VertexBufferObject}. Only the later supports OpenGL ES 2.0.
 * 
 * @author mzechner */
public interface IVertexData : Disposable {
	/** @return the number of vertices this VertexData stores */
	public int getNumVertices ();

	/** @return the number of vertices this VertedData can store */
	public int getNumMaxVertices ();

	/** @return the {@link VertexAttributes} as specified during construction. */
	public VertexAttributes getAttributes ();

	/** Sets the vertices of this VertexData, discarding the old vertex data. The count must equal the number of floats per vertex
	 * times the number of vertices to be copied to this VertexData. The order of the vertex attributes must be the same as
	 * specified at construction time via {@link VertexAttributes}.
	 * <p>
	 * This can be called in between calls to bind and unbind. The vertex data will be updated instantly.
	 * @param vertices the vertex data
	 * @param offset the offset to start copying the data from
	 * @param count the number of floats to copy */
	public void setVertices (float[] vertices, int offset, int count);

	/** Update (a portion of) the vertices. Does not resize the backing buffer.
	 * @param vertices the vertex data
	 * @param sourceOffset the offset to start copying the data from
	 * @param count the number of floats to copy */
	public void updateVertices (int targetOffset, float[] vertices, int sourceOffset, int count);
		
	/** Returns the underlying FloatBuffer for reading or writing.
	 * @param forWriting when true, the underlying buffer will be uploaded on the next call to bind. If you need immediate
	 *           uploading use {@link #setVertices(float[], int, int)}.
	 * @return the underlying FloatBuffer holding the vertex data. */
	public FloatBuffer getBuffer (bool forWriting);

	/** Binds this VertexData for rendering via glDrawArrays or glDrawElements. */
	public void bind (ShaderProgram shader);

	/** Binds this VertexData for rendering via glDrawArrays or glDrawElements.
	 * @param locations array containing the attribute locations. */
	public void bind (ShaderProgram shader, int[] locations);

	/** Unbinds this VertexData. */
	public void unbind (ShaderProgram shader);

	/** Unbinds this VertexData.
	 * @param locations array containing the attribute locations. */
	public void unbind (ShaderProgram shader, int[] locations);

	/** Invalidates the VertexData if applicable. Use this in case of a context loss. */
	public void invalidate ();

	/** Disposes this VertexData and all its associated OpenGL resources. */
	public void dispose ();
}
}
