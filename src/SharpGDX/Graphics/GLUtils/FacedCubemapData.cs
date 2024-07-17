﻿using SharpGDX.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Utils;

namespace SharpGDX.Graphics.GLUtils
{
	/** A FacedCubemapData holds a cubemap data definition based on a {@link TextureData} per face.
 * 
 * @author Vincent Nousquet */
public class FacedCubemapData : ICubemapData {

	protected readonly ITextureData?[] data = new ITextureData[6];

	/** Construct an empty Cubemap. Use the load(...) methods to set the texture of each side. Every side of the cubemap must be
	 * set before it can be used. */
	public FacedCubemapData () 
	: this((ITextureData?)null, (ITextureData?)null, (ITextureData?)null, (ITextureData?)null, (ITextureData?)null, (ITextureData?)null)
	{
		
	}

	/** Construct a Cubemap with the specified texture files for the sides, optionally generating mipmaps. */
	public FacedCubemapData (FileHandle positiveX, FileHandle negativeX, FileHandle positiveY, FileHandle negativeY,
		FileHandle positiveZ, FileHandle negativeZ) 
	: this(ITextureData.Factory.LoadFromFile(positiveX, false), ITextureData.Factory.LoadFromFile(negativeX, false),
		ITextureData.Factory.LoadFromFile(positiveY, false), ITextureData.Factory.LoadFromFile(negativeY, false),
		ITextureData.Factory.LoadFromFile(positiveZ, false), ITextureData.Factory.LoadFromFile(negativeZ, false))
	{
		
	}

	/** Construct a Cubemap with the specified texture files for the sides, optionally generating mipmaps. */
	public FacedCubemapData (FileHandle positiveX, FileHandle negativeX, FileHandle positiveY, FileHandle negativeY,
		FileHandle positiveZ, FileHandle negativeZ, bool useMipMaps) 
	: this(ITextureData.Factory.LoadFromFile(positiveX, useMipMaps), ITextureData.Factory.LoadFromFile(negativeX, useMipMaps),
		ITextureData.Factory.LoadFromFile(positiveY, useMipMaps), ITextureData.Factory.LoadFromFile(negativeY, useMipMaps),
		ITextureData.Factory.LoadFromFile(positiveZ, useMipMaps), ITextureData.Factory.LoadFromFile(negativeZ, useMipMaps))
	{
		
	}

	/** Construct a Cubemap with the specified {@link Pixmap}s for the sides, does not generate mipmaps. */
	public FacedCubemapData (Pixmap positiveX, Pixmap negativeX, Pixmap positiveY, Pixmap negativeY, Pixmap positiveZ,
		Pixmap negativeZ) 
	: this(positiveX, negativeX, positiveY, negativeY, positiveZ, negativeZ, false)
	{
		
	}

	/** Construct a Cubemap with the specified {@link Pixmap}s for the sides, optionally generating mipmaps. */
	public FacedCubemapData (Pixmap? positiveX, Pixmap? negativeX, Pixmap? positiveY, Pixmap? negativeY, Pixmap? positiveZ,
		Pixmap? negativeZ, bool useMipMaps) 
	: this(positiveX == null ? null : new PixmapTextureData(positiveX, null, useMipMaps, false),
		negativeX == null ? null : new PixmapTextureData(negativeX, null, useMipMaps, false),
		positiveY == null ? null : new PixmapTextureData(positiveY, null, useMipMaps, false),
		negativeY == null ? null : new PixmapTextureData(negativeY, null, useMipMaps, false),
		positiveZ == null ? null : new PixmapTextureData(positiveZ, null, useMipMaps, false),
		negativeZ == null ? null : new PixmapTextureData(negativeZ, null, useMipMaps, false))
	{
		
	}

	/** Construct a Cubemap with {@link Pixmap}s for each side of the specified size. */
	public FacedCubemapData (int width, int height, int depth, Pixmap.Format format) 
	: this(new PixmapTextureData(new Pixmap(depth, height, format), null, false, true),
		new PixmapTextureData(new Pixmap(depth, height, format), null, false, true),
		new PixmapTextureData(new Pixmap(width, depth, format), null, false, true),
		new PixmapTextureData(new Pixmap(width, depth, format), null, false, true),
		new PixmapTextureData(new Pixmap(width, height, format), null, false, true),
		new PixmapTextureData(new Pixmap(width, height, format), null, false, true))
	{
		
	}

	/** Construct a Cubemap with the specified {@link TextureData}'s for the sides */
	public FacedCubemapData (ITextureData? positiveX, ITextureData? negativeX, ITextureData? positiveY, ITextureData? negativeY,
		ITextureData? positiveZ, ITextureData? negativeZ) {
		data[0] = positiveX;
		data[1] = negativeX;
		data[2] = positiveY;
		data[3] = negativeY;
		data[4] = positiveZ;
		data[5] = negativeZ;
	}

	public bool isManaged () {
		foreach (ITextureData? data in this.data)
			if (!data.isManaged()) return false;
		return true;
	}

	/** Loads the texture specified using the {@link FileHandle} and sets it to specified side, overwriting any previous data set
	 * to that side. Note that you need to reload through {@link Cubemap#load(CubemapData)} any cubemap using this data for the
	 * change to be taken in account.
	 * @param side The {@link CubemapSide}
	 * @param file The texture {@link FileHandle} */
	public void load (Cubemap.CubemapSide side, FileHandle file) {
		data[side.index] = ITextureData.Factory.LoadFromFile(file, false);
	}

	/** Sets the specified side of this cubemap to the specified {@link Pixmap}, overwriting any previous data set to that side.
	 * Note that you need to reload through {@link Cubemap#load(CubemapData)} any cubemap using this data for the change to be
	 * taken in account.
	 * @param side The {@link CubemapSide}
	 * @param pixmap The {@link Pixmap} */
	public void load (Cubemap.CubemapSide side, Pixmap? pixmap) {
		data[side.index] = pixmap == null ? null : new PixmapTextureData(pixmap, null, false, false);
	}

	/** @return True if all sides of this cubemap are set, false otherwise. */
	public bool isComplete () {
		for (int i = 0; i < data.Length; i++)
			if (data[i] == null) return false;
		return true;
	}

	/** @return The {@link TextureData} for the specified side, can be null if the cubemap is incomplete. */
	public ITextureData getTextureData (Cubemap.CubemapSide side) {
		return data[side.index];
	}

	public int getWidth () {
		int tmp, width = 0;
		if (data[Cubemap.CubemapSide.PositiveZ.index] != null && (tmp = data[Cubemap.CubemapSide.PositiveZ.index].getWidth()) > width) width = tmp;
		if (data[Cubemap.CubemapSide.NegativeZ.index] != null && (tmp = data[Cubemap.CubemapSide.NegativeZ.index].getWidth()) > width) width = tmp;
		if (data[Cubemap.CubemapSide.PositiveY.index] != null && (tmp = data[Cubemap.CubemapSide.PositiveY.index].getWidth()) > width) width = tmp;
		if (data[Cubemap.CubemapSide.NegativeY.index] != null && (tmp = data[Cubemap.CubemapSide.NegativeY.index].getWidth()) > width) width = tmp;
		return width;
	}

	public int getHeight () {
		int tmp, height = 0;
		if (data[Cubemap.CubemapSide.PositiveZ.index] != null && (tmp = data[Cubemap.CubemapSide.PositiveZ.index].getHeight()) > height)
			height = tmp;
		if (data[Cubemap.CubemapSide.NegativeZ.index] != null && (tmp = data[Cubemap.CubemapSide.NegativeZ.index].getHeight()) > height)
			height = tmp;
		if (data[Cubemap.CubemapSide.PositiveX.index] != null && (tmp = data[Cubemap.CubemapSide.PositiveX.index].getHeight()) > height)
			height = tmp;
		if (data[Cubemap.CubemapSide.NegativeX.index] != null && (tmp = data[Cubemap.CubemapSide.NegativeX.index].getHeight()) > height)
			height = tmp;
		return height;
	}

	public bool isPrepared () {
		return false;
	}

	public void prepare () {
		if (!isComplete()) throw new GdxRuntimeException("You need to complete your cubemap data before using it");
		for (int i = 0; i < data.Length; i++)
			if (!data[i].isPrepared()) data[i].prepare();
	}

	public void consumeCubemapData () {
		for (int i = 0; i < data.Length; i++) {
			if (data[i].getType() == ITextureData.TextureDataType.Custom) {
				data[i].consumeCustomData(GL20.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i);
			} else {
				Pixmap pixmap = data[i].consumePixmap();
				bool disposePixmap = data[i].disposePixmap();
				if (data[i].getFormat() != pixmap.getFormat()) {
					Pixmap tmp = new Pixmap(pixmap.getWidth(), pixmap.getHeight(), data[i].getFormat());
					tmp.setBlending(Pixmap.Blending.None);
					tmp.drawPixmap(pixmap, 0, 0, 0, 0, pixmap.getWidth(), pixmap.getHeight());
					if (data[i].disposePixmap()) pixmap.dispose();
					pixmap = tmp;
					disposePixmap = true;
				}
				Gdx.gl.glPixelStorei(GL20.GL_UNPACK_ALIGNMENT, 1);
				Gdx.gl.glTexImage2D(GL20.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, pixmap.getGLInternalFormat(), pixmap.getWidth(),
					pixmap.getHeight(), 0, pixmap.getGLFormat(), pixmap.getGLType(), pixmap.getPixels());
				if (disposePixmap) pixmap.dispose();
			}
		}
	}

}
}
