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
				textureDatas[i].consumeCustomData(GL30.GL_TEXTURE_2D_ARRAY);
				containsCustomData = true;
			} else {
				ITextureData texData = textureDatas[i];
				Pixmap pixmap = texData.consumePixmap();
				bool disposePixmap = texData.disposePixmap();
				if (texData.getFormat() != pixmap.getFormat()) {
					Pixmap temp = new Pixmap(pixmap.getWidth(), pixmap.getHeight(), texData.getFormat());
					temp.setBlending(Pixmap.Blending.None);
					temp.drawPixmap(pixmap, 0, 0, 0, 0, pixmap.getWidth(), pixmap.getHeight());
					if (texData.disposePixmap()) {
						pixmap.dispose();
					}
					pixmap = temp;
					disposePixmap = true;
				}
				Gdx.gl30.glTexSubImage3D(GL30.GL_TEXTURE_2D_ARRAY, 0, 0, 0, i, pixmap.getWidth(), pixmap.getHeight(), 1,
					pixmap.getGLInternalFormat(), pixmap.getGLType(), pixmap.getPixels());
				if (disposePixmap) pixmap.dispose();
			}
		}
		if (useMipMaps && !containsCustomData) {
			Gdx.gl20.glGenerateMipmap(GL30.GL_TEXTURE_2D_ARRAY);
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
		return Pixmap.FormatUtils.toGlFormat(format);
	}

	public int getGLType () {
		return Pixmap.FormatUtils.toGlType(format);
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
