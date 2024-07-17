using SharpGDX.Files;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics
{
	/** Used by a {@link TextureArray} to load the pixel data. The TextureArray will request the TextureArrayData to prepare itself
 * through {@link #prepare()} and upload its data using {@link #consumeTextureArrayData()}. These are the first methods to be
 * called by TextureArray. After that the TextureArray will invoke the other methods to find out about the size of the image data,
 * the format, whether the TextureArrayData is able to manage the pixel data if the OpenGL ES context is lost.
 * </p>
 *
 * Before a call to either {@link #consumeTextureArrayData()}, TextureArray will bind the OpenGL ES texture.
 * </p>
 *
 * Look at {@link FileTextureArrayData} for example implementation of this interface.
 * @author Tomski */
public interface ITextureArrayData {

	/** @return whether the TextureArrayData is prepared or not. */
	public bool isPrepared ();

	/** Prepares the TextureArrayData for a call to {@link #consumeTextureArrayData()}. This method can be called from a non OpenGL
	 * thread and should thus not interact with OpenGL. */
	public void prepare ();

	/** Uploads the pixel data of the TextureArray layers of the TextureArray to the OpenGL ES texture. The caller must bind an
	 * OpenGL ES texture. A call to {@link #prepare()} must preceed a call to this method. Any internal data structures created in
	 * {@link #prepare()} should be disposed of here. */
	public void consumeTextureArrayData ();

	/** @return the width of this TextureArray */
	public int getWidth ();

	/** @return the height of this TextureArray */
	public int getHeight ();

	/** @return the layer count of this TextureArray */
	public int getDepth ();

	/** @return whether this implementation can cope with a EGL context loss. */
	public bool isManaged ();

	/** @return the internal format of this TextureArray */
	public int getInternalFormat ();

	/** @return the GL type of this TextureArray */
	public int getGLType ();

	/** Provides static method to instantiate the right implementation.
	 * @author Tomski */
	public static class Factory {

		public static ITextureArrayData loadFromFiles (Pixmap.Format format, bool useMipMaps, FileHandle[] files) {
			return new FileTextureArrayData(format, useMipMaps, files);
		}

	}

}
}
