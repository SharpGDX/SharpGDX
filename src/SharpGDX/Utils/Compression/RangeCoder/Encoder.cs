using System;
using System.Collections.Generic;
using System.Linq;
using SharpGDX.Shims;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils.Compression.RangeCoder
{
	public class Encoder {
	static readonly int kTopMask = ~((1 << 24) - 1);

	static readonly int kNumBitModelTotalBits = 11;
	static readonly int kBitModelTotal = (1 << kNumBitModelTotalBits);
	static readonly int kNumMoveBits = 5;

	OutputStream Stream;

	long Low;
	int Range;
	int _cacheSize;
	int _cache;

	long _position;

	public void SetStream (OutputStream stream) {
		Stream = stream;
	}

	public void ReleaseStream () {
		Stream = null;
	}

	public void Init () {
		_position = 0;
		Low = 0;
		Range = -1;
		_cacheSize = 1;
		_cache = 0;
	}

	public void FlushData () // TODO: throws IOException 
	{
		for (int i = 0; i < 5; i++)
			ShiftLow();
	}

	public void FlushStream () // TODO: throws IOException 
		{
		Stream.flush();
	}

	public void ShiftLow () // TODO: throws IOException 
		{
		int LowHi = (int)(Low >>> 32);
		if (LowHi != 0 || Low < 0xFF000000L) {
			_position += _cacheSize;
			int temp = _cache;
			do
			{
				throw new NotImplementedException();
				//Stream.write(temp + LowHi);
				//temp = 0xFF;
			} while (--_cacheSize != 0);
			_cache = (((int)Low) >>> 24);
		}
		_cacheSize++;
		Low = (Low & 0xFFFFFF) << 8;
	}

	public void EncodeDirectBits (int v, int numTotalBits) // TODO: throws IOException
		{
		for (int i = numTotalBits - 1; i >= 0; i--) {
			Range >>>= 1;
			if (((v >>> i) & 1) == 1) Low += Range;
			if ((Range & Encoder.kTopMask) == 0) {
				Range <<= 8;
				ShiftLow();
			}
		}
	}

	public long GetProcessedSizeAdd () {
		return _cacheSize + _position + 4;
	}

	static readonly int kNumMoveReducingBits = 2;
	public static readonly int kNumBitPriceShiftBits = 6;

	public static void InitBitModels (short[] probs) {
		for (int i = 0; i < probs.Length; i++)
		{
			// TODO: Verify. -RP
			probs[i] = (short)(kBitModelTotal >>> 1);
		}
	}

	public void Encode (short[] probs, int index, int symbol) // TODO: throws IOException 
		{
		int prob = probs[index];
		int newBound = (Range >>> kNumBitModelTotalBits) * prob;
		if (symbol == 0) {
			Range = newBound;
			probs[index] = (short)(prob + ((kBitModelTotal - prob) >>> kNumMoveBits));
		} else {
			Low += (newBound & 0xFFFFFFFFL);
			Range -= newBound;
			probs[index] = (short)(prob - ((prob) >>> kNumMoveBits));
		}
		if ((Range & kTopMask) == 0) {
			Range <<= 8;
			ShiftLow();
		}
	}

	private static int[] ProbPrices = new int[kBitModelTotal >>> kNumMoveReducingBits];

	static Encoder(){
		int kNumBits = (kNumBitModelTotalBits - kNumMoveReducingBits);
		for (int i = kNumBits - 1; i >= 0; i--) {
			int start = 1 << (kNumBits - i - 1);
			int end = 1 << (kNumBits - i);
			for (int j = start; j < end; j++)
				ProbPrices[j] = (i << kNumBitPriceShiftBits) + (((end - j) << kNumBitPriceShiftBits) >>> (kNumBits - i - 1));
		}
	}

	static public int GetPrice (int Prob, int symbol) {
		return ProbPrices[(((Prob - symbol) ^ ((-symbol))) & (kBitModelTotal - 1)) >>> kNumMoveReducingBits];
	}

	static public int GetPrice0 (int Prob) {
		return ProbPrices[Prob >>> kNumMoveReducingBits];
	}

	static public int GetPrice1 (int Prob) {
		return ProbPrices[(kBitModelTotal - Prob) >>> kNumMoveReducingBits];
	}
}
}
