using System;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D.Utils;

/** Responsible for binding textures, may implement a strategy to avoid binding a texture unnecessarily. A TextureBinder may
 * decide to which texture unit it binds a texture.
 * @author badlogic, Xoppa */
public interface TextureBinder {
	/** Prepares the binder for operation, must be matched with a call to {@link #end()}. */
	public void begin ();

	/** Disables all used texture units and unbinds textures. Resets the counts. */
	public void end ();

	/** Binds the texture to an available unit and applies the filters in the descriptor.
	 * @param textureDescriptor the {@link TextureDescriptor}
	 * @return the unit the texture was bound to */
	public int bind (TextureDescriptor textureDescriptor);

	/** Binds the texture to an available unit.
	 * @param texture the {@link Texture}
	 * @return the unit the texture was bound to */
	public int bind (GLTexture texture);

	/** @return the number of binds actually executed since the last call to {@link #resetCounts()} */
	public int getBindCount ();

	/** @return the number of binds that could be avoided by reuse */
	public int getReuseCount ();

	/** Resets the bind/reuse counts */
	public void resetCounts ();
}
