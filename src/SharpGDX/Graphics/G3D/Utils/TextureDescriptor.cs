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

public class TextureDescriptor : IComparable<TextureDescriptor> 
{
	public GLTexture? texture = null;
	public Texture.TextureFilter? minFilter;
	public Texture.TextureFilter? magFilter;
	public Texture.TextureWrap? uWrap;
	public Texture.TextureWrap? vWrap;

	// TODO add other values, see http://www.opengl.org/sdk/docs/man/xhtml/glTexParameter.xml

	public TextureDescriptor (GLTexture texture, Texture.TextureFilter minFilter, Texture.TextureFilter magFilter,
		Texture.TextureWrap uWrap, Texture.TextureWrap vWrap) {
		set(texture, minFilter, magFilter, uWrap, vWrap);
	}

	public TextureDescriptor (GLTexture texture) 
    : this(texture, default, default, default, default)
    {
		
	}

	public TextureDescriptor () {
	}

	public void set (GLTexture texture, Texture.TextureFilter? minFilter, Texture.TextureFilter? magFilter,
		Texture.TextureWrap? uWrap, Texture.TextureWrap? vWrap) {
		this.texture = texture;
		this.minFilter = minFilter;
		this.magFilter = magFilter;
		this.uWrap = uWrap;
		this.vWrap = vWrap;
	}

	public void set (TextureDescriptor other)
    {
		texture = other.texture;
		minFilter = other.minFilter;
		magFilter = other.magFilter;
		uWrap = other.uWrap;
		vWrap = other.vWrap;
	}

	public override bool Equals (object? obj) {
		if (obj == null) return false;
		if (obj == this) return true;
		if (!(obj is TextureDescriptor)) return false;
		TextureDescriptor other = (TextureDescriptor)obj;
		return other.texture == texture && other.minFilter == minFilter && other.magFilter == magFilter && other.uWrap == uWrap
			&& other.vWrap == vWrap;
	}

	public override int GetHashCode () {
		long result = (texture == null ? 0 : texture.glTarget);
		result = 811 * result + (texture == null ? 0 : texture.getTextureObjectHandle());
		result = 811 * result + (minFilter == null ? 0 : minFilter.Value.getGLEnum());
		result = 811 * result + (magFilter == null ? 0 : magFilter.Value.getGLEnum());
		result = 811 * result + (uWrap == null ? 0 : uWrap.Value.getGLEnum());
		result = 811 * result + (vWrap == null ? 0 : vWrap.Value.getGLEnum());
		return (int)(result ^ (result >> 32));
	}

	public int CompareTo (TextureDescriptor o) {
		if (o == this) return 0;
		int t1 = texture == null ? 0 : texture.glTarget;
		int t2 = o.texture == null ? 0 : o.texture.glTarget;
		if (t1 != t2) return t1 - t2;
		int h1 = texture == null ? 0 : texture.getTextureObjectHandle();
		int h2 = o.texture == null ? 0 : o.texture.getTextureObjectHandle();
		if (h1 != h2) return h1 - h2;
		if (minFilter != o.minFilter)
			return (minFilter == null ? 0 : minFilter.Value.getGLEnum()) - (o.minFilter == null ? 0 : o.minFilter.Value.getGLEnum());
		if (magFilter != o.magFilter)
			return (magFilter == null ? 0 : magFilter.Value.getGLEnum()) - (o.magFilter == null ? 0 : o.magFilter.Value.getGLEnum());
		if (uWrap != o.uWrap) return (uWrap == null ? 0 : uWrap.Value.getGLEnum()) - (o.uWrap == null ? 0 : o.uWrap.Value.getGLEnum());
		if (vWrap != o.vWrap) return (vWrap == null ? 0 : vWrap.Value.getGLEnum()) - (o.vWrap == null ? 0 : o.vWrap.Value.getGLEnum());
		return 0;
	}
}
