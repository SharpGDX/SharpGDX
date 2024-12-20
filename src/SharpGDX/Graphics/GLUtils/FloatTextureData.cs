﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX.Graphics.GLUtils
{
	/** A {@link TextureData} implementation which should be used to create float textures. */
public class FloatTextureData : ITextureData {

	int width = 0;
	int height = 0;

	int internalFormat;
	int format;
	int type;

	bool isGpuOnly;

	bool _isPrepared = false;
	FloatBuffer buffer;

	public FloatTextureData (int w, int h, int internalFormat, int format, int type, bool isGpuOnly) {
		this.width = w;
		this.height = h;
		this.internalFormat = internalFormat;
		this.format = format;
		this.type = type;
		this.isGpuOnly = isGpuOnly;
	}

	public ITextureData.TextureDataType getType () {
		return ITextureData.TextureDataType.Custom;
	}

	public bool isPrepared () {
		return _isPrepared;
	}

	public void prepare () {
		if (_isPrepared) throw new GdxRuntimeException("Already prepared");
		if (!isGpuOnly) {
			int amountOfFloats = 4;
			if (GDX.Graphics.GetGLVersion().GetType().Equals(GLType.OpenGL)) {
				if (internalFormat == IGL30.GL_RGBA16F || internalFormat == IGL30.GL_RGBA32F) amountOfFloats = 4;
				if (internalFormat == IGL30.GL_RGB16F || internalFormat == IGL30.GL_RGB32F) amountOfFloats = 3;
				if (internalFormat == IGL30.GL_RG16F || internalFormat == IGL30.GL_RG32F) amountOfFloats = 2;
				if (internalFormat == IGL30.GL_R16F || internalFormat == IGL30.GL_R32F) amountOfFloats = 1;
			}
			this.buffer = BufferUtils.newFloatBuffer(width * height * amountOfFloats);
		}
		_isPrepared = true;
	}

	public void consumeCustomData (int target) {
		if (GDX.App.GetType() == IApplication.ApplicationType.Android || GDX.App.GetType() == IApplication.ApplicationType.IOS
                                                                      || (GDX.App.GetType() == IApplication.ApplicationType.WebGL && !GDX.Graphics.IsGL30Available())) {

			if (!GDX.Graphics.SupportsExtension("OES_texture_float"))
				throw new GdxRuntimeException("Extension OES_texture_float not supported!");

			// GLES and WebGL defines texture format by 3rd and 8th argument,
			// so to get a float texture one needs to supply GL_RGBA and GL_FLOAT there.
			GDX.GL.glTexImage2D(target, 0, IGL20.GL_RGBA, width, height, 0, IGL20.GL_RGBA, IGL20.GL_FLOAT, buffer);

		} else {
			if (!GDX.Graphics.IsGL30Available()) {
				if (!GDX.Graphics.SupportsExtension("GL_ARB_texture_float"))
					throw new GdxRuntimeException("Extension GL_ARB_texture_float not supported!");
			}
			// in desktop OpenGL the texture format is defined only by the third argument,
			// hence we need to use GL_RGBA32F there (this constant is unavailable in GLES/WebGL)
			GDX.GL.glTexImage2D(target, 0, internalFormat, width, height, 0, format, IGL20.GL_FLOAT, buffer);
		}
	}

	public Pixmap consumePixmap () {
		throw new GdxRuntimeException("This TextureData implementation does not return a Pixmap");
	}

	public bool disposePixmap () {
		throw new GdxRuntimeException("This TextureData implementation does not return a Pixmap");
	}

	public int getWidth () {
		return width;
	}

	public int getHeight () {
		return height;
	}

	public Pixmap.Format? getFormat () {
		return Pixmap.Format.RGBA8888; // it's not true, but FloatTextureData.getFormat() isn't used anywhere
	}

	public bool useMipMaps () {
		return false;
	}

	public bool isManaged () {
		return true;
	}

	public FloatBuffer getBuffer () {
		return buffer;
	}
}
}
