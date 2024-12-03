using SharpGDX.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics.GLUtils
{
	/** @author Tomski **/
public class FileTextureArrayData : ITextureArrayData {

	private ITextureData[] textureDatas;
	private bool prepared;
	private Pixmap.Format format;
	private int depth;
	bool useMipMaps;

	public FileTextureArrayData (Pixmap.Format format, bool useMipMaps, FileHandle[] files) {
		this.format = format;
		this.useMipMaps = useMipMaps;
		this.depth = files.Length;
		textureDatas = new ITextureData[files.Length];
		for (int i = 0; i < files.Length; i++) {
			textureDatas[i] = ITextureData.Factory.LoadFromFile(files[i], format, useMipMaps);
		}
	}

	public bool isPrepared () {
		return prepared;
	}

	public void prepare () {
		int width = -1;
		int height = -1;
		foreach (ITextureData data in textureDatas) {
			data.prepare();
			if (width == -1) {
				width = data.getWidth();
				height = data.getHeight();
				continue;
			}
			if (width != data.getWidth() || height != data.getHeight()) {
				throw new GdxRuntimeException(
					"Error whilst preparing TextureArray: TextureArray Textures must have equal dimensions.");
			}
		}
		prepared = true;
	}

	public void consumeTextureArrayData () {
		bool containsCustomData = false;
		for (int i = 0; i < textureDatas.Length; i++) {
			if (textureDatas[i].getType() == ITextureData.TextureDataType.Custom) {
				textureDatas[i].consumeCustomData(IGL30.GL_TEXTURE_2D_ARRAY);
				containsCustomData = true;
			} else {
				ITextureData texData = textureDatas[i];
				Pixmap pixmap = texData.consumePixmap();
				bool disposePixmap = texData.disposePixmap();
				if (texData.getFormat() != pixmap.GetFormat()) {
					Pixmap temp = new Pixmap(pixmap.GetWidth(), pixmap.GetHeight(), texData.getFormat());
					temp.SetBlending(Pixmap.Blending.None);
					temp.DrawPixmap(pixmap, 0, 0, 0, 0, pixmap.GetWidth(), pixmap.GetHeight());
					if (texData.disposePixmap()) {
						pixmap.Dispose();
					}
					pixmap = temp;
					disposePixmap = true;
				}
				GDX.GL30.glTexSubImage3D(IGL30.GL_TEXTURE_2D_ARRAY, 0, 0, 0, i, pixmap.GetWidth(), pixmap.GetHeight(), 1,
					pixmap.GetGLInternalFormat(), pixmap.GetGLType(), pixmap.GetPixels());
				if (disposePixmap) pixmap.Dispose();
			}
		}
		if (useMipMaps && !containsCustomData) {
			GDX.GL20.glGenerateMipmap(IGL30.GL_TEXTURE_2D_ARRAY);
		}
	}

	public int getWidth () {
		return textureDatas[0].getWidth();
	}

	public int getHeight () {
		return textureDatas[0].getHeight();
	}

	public int getDepth () {
		return depth;
	}

	public int getInternalFormat () {
		return Pixmap.FormatUtils.ToGlFormat(format);
	}

	public int getGLType () {
		return Pixmap.FormatUtils.ToGlType(format);
	}

	public bool isManaged () {
		foreach (ITextureData data in textureDatas) {
			if (!data.isManaged()) {
				return false;
			}
		}
		return true;
	}
}
}
