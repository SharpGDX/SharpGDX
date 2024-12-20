﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Utils;

namespace SharpGDX.Graphics.GLUtils
{
	public class MipMapGenerator {

	private MipMapGenerator () {
		// disallow, static methods only
	}

	private static bool useHWMipMap = true;

	static public void setUseHardwareMipMap (bool useHWMipMap) {
		MipMapGenerator.useHWMipMap = useHWMipMap;
	}

	/** Sets the image data of the {@link Texture} based on the {@link Pixmap}. The texture must be bound for this to work. If
	 * <code>disposePixmap</code> is true, the pixmap will be disposed at the end of the method.
	 * @param pixmap the Pixmap */
	public static void generateMipMap (Pixmap pixmap, int textureWidth, int textureHeight) {
		generateMipMap(IGL20.GL_TEXTURE_2D, pixmap, textureWidth, textureHeight);
	}

	/** Sets the image data of the {@link Texture} based on the {@link Pixmap}. The texture must be bound for this to work. If
	 * <code>disposePixmap</code> is true, the pixmap will be disposed at the end of the method. */
	public static void generateMipMap (int target, Pixmap pixmap, int textureWidth, int textureHeight) {
		if (!useHWMipMap) {
			generateMipMapCPU(target, pixmap, textureWidth, textureHeight);
			return;
		}

		if (GDX.App.GetType() == IApplication.ApplicationType.Android || GDX.App.GetType() == IApplication.ApplicationType.WebGL
                                                                      || GDX.App.GetType() == IApplication.ApplicationType.IOS) {
			generateMipMapGLES20(target, pixmap);
		} else {
			generateMipMapDesktop(target, pixmap, textureWidth, textureHeight);
		}
	}

	private static void generateMipMapGLES20 (int target, Pixmap pixmap) {
		GDX.GL.glTexImage2D(target, 0, pixmap.GetGLInternalFormat(), pixmap.GetWidth(), pixmap.GetHeight(), 0, pixmap.GetGLFormat(),
			pixmap.GetGLType(), pixmap.GetPixels());
		GDX.GL20.glGenerateMipmap(target);
	}

	private static void generateMipMapDesktop (int target, Pixmap pixmap, int textureWidth, int textureHeight) {
		// TODO: Update names
		if (GDX.Graphics.SupportsExtension("GL_ARB_framebuffer_object")
			|| GDX.Graphics.SupportsExtension("GL_EXT_framebuffer_object")
			|| GDX.GL20.GetType().Name.Equals("com.badlogic.gdx.backends.lwjgl3.Lwjgl3GLES20") // LWJGL3ANGLE
			|| GDX.GL30 != null) {
			GDX.GL.glTexImage2D(target, 0, pixmap.GetGLInternalFormat(), pixmap.GetWidth(), pixmap.GetHeight(), 0,
				pixmap.GetGLFormat(), pixmap.GetGLType(), pixmap.GetPixels());
			GDX.GL20.glGenerateMipmap(target);
		} else {
			generateMipMapCPU(target, pixmap, textureWidth, textureHeight);
		}
	}

	private static void generateMipMapCPU (int target, Pixmap pixmap, int textureWidth, int textureHeight) {
		GDX.GL.glTexImage2D(target, 0, pixmap.GetGLInternalFormat(), pixmap.GetWidth(), pixmap.GetHeight(), 0, pixmap.GetGLFormat(),
			pixmap.GetGLType(), pixmap.GetPixels());
		if ((GDX.GL20 == null) && textureWidth != textureHeight)
			throw new GdxRuntimeException("texture width and height must be square when using mipmapping.");
		int width = pixmap.GetWidth() / 2;
		int height = pixmap.GetHeight() / 2;
		int level = 1;
		while (width > 0 && height > 0) {
			Pixmap tmp = new Pixmap(width, height, pixmap.GetFormat());
			tmp.SetBlending(Pixmap.Blending.None);
			tmp.DrawPixmap(pixmap, 0, 0, pixmap.GetWidth(), pixmap.GetHeight(), 0, 0, width, height);
			if (level > 1) pixmap.Dispose();
			pixmap = tmp;

			GDX.GL.glTexImage2D(target, level, pixmap.GetGLInternalFormat(), pixmap.GetWidth(), pixmap.GetHeight(), 0,
				pixmap.GetGLFormat(), pixmap.GetGLType(), pixmap.GetPixels());

			width = pixmap.GetWidth() / 2;
			height = pixmap.GetHeight() / 2;
			level++;
		}
	}
}
}
