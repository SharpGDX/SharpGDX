using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils.Compression
{
	public class CRC {
	static public int[] Table = new int[256];

	static CRC(){
		for (int i = 0; i < 256; i++) {
			int r = i;
			for (int j = 0; j < 8; j++)
				if ((r & 1) != 0)
				{
					// TODO: Verify. -RP
					r = (int)((r >>> 1) ^ 0xEDB88320);
				}
				else
					r >>>= 1;
			Table[i] = r;
		}
	}

	int _value = -1;

	public void Init () {
		_value = -1;
	}

	public void Update (byte[] data, int offset, int size) {
		for (int i = 0; i < size; i++)
			_value = Table[(_value ^ data[offset + i]) & 0xFF] ^ (_value >>> 8);
	}

	public void Update (byte[] data) {
		int size = data.Length;
		for (int i = 0; i < size; i++)
			_value = Table[(_value ^ data[i]) & 0xFF] ^ (_value >>> 8);
	}

	public void UpdateByte (int b) {
		_value = Table[(_value ^ b) & 0xFF] ^ (_value >>> 8);
	}

	public int GetDigest () {
		return _value ^ (-1);
	}
}
}
