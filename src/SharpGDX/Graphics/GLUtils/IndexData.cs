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
	/** An IndexData instance holds index data. Can be either a plain short buffer or an OpenGL buffer object.
 * @author mzechner */
public interface IndexData : Disposable {
	/** @return the number of indices currently stored in this buffer */
	public int getNumIndices ();

	/** @return the maximum number of indices this IndexBufferObject can store. */
	public int getNumMaxIndices ();

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
	 * @param indices the index data
	 * @param offset the offset to start copying the data from
	 * @param count the number of shorts to copy */
	public void setIndices (short[] indices, int offset, int count);

	/** Copies the specified indices to the indices of this IndexBufferObject, discarding the old indices. Copying start at the
	 * current {@link ShortBuffer#position()} of the specified buffer and copied the {@link ShortBuffer#remaining()} amount of
	 * indices. This can be called in between calls to {@link #bind()} and {@link #unbind()}. The index data will be updated
	 * instantly.
	 * @param indices the index data to copy */
	public void setIndices (ShortBuffer indices);

	/** Update (a portion of) the indices.
	 * @param targetOffset offset in indices buffer
	 * @param indices the index data
	 * @param offset the offset to start copying the data from
	 * @param count the number of shorts to copy */
	public void updateIndices (int targetOffset, short[] indices, int offset, int count);
		
	/** Returns the underlying ShortBuffer for reading or writing.
	 * @param forWriting when true, the underlying buffer will be uploaded on the next call to {@link #bind()}. If you need
	 *           immediate uploading use {@link #setIndices(short[], int, int)}.
	 * @return the underlying short buffer. */
	public ShortBuffer getBuffer (bool forWriting);

	/** Binds this IndexBufferObject for rendering with glDrawElements. */
	public void bind ();

	/** Unbinds this IndexBufferObject. */
	public void unbind ();

	/** Invalidates the IndexBufferObject so a new OpenGL buffer handle is created. Use this in case of a context loss. */
	public void invalidate ();

	/** Disposes this IndexDatat and all its associated OpenGL resources. */
	public void dispose ();
}
}
