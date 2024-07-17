using System;
using SharpGDX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Graphics.GLUtils
{
	/** This class will load each contained TextureData to the chosen mipmap level. All the mipmap levels must be defined and cannot
 * be null. */
	public class MipMapTextureData : ITextureData
	{
	ITextureData []
		mips;

	/** @param mipMapData must be != null and its length must be >= 1 */
	public MipMapTextureData(ITextureData[]mipMapData)
	{
		mips = new ITextureData[mipMapData.Length];
		Array.Copy(mipMapData, 0, mips, 0, mipMapData.Length);
	}

		public ITextureData.TextureDataType getType()
	{
		return ITextureData.TextureDataType.Custom;
	}

		public bool isPrepared()
	{
		return true;
	}

	public void prepare()
	{
	}

		public Pixmap consumePixmap()
	{
		throw new GdxRuntimeException("It's compressed, use the compressed method");
	}

		public bool disposePixmap()
	{
		return false;
	}

		public void consumeCustomData(int target)
	{
		for (int i = 0; i < mips.Length; ++i)
		{
			GLTexture.uploadImageData(target, mips[i], i);
		}
	}

		public int getWidth()
	{
		return mips[0].getWidth();
	}

		public int getHeight()
	{
		return mips[0].getHeight();
	}

	
	public Pixmap.Format? getFormat()
	{
		return mips[0].getFormat();
	}

		public bool useMipMaps()
	{
		return false;
	}

		public bool isManaged()
	{
		return true;
	}
	}
}
